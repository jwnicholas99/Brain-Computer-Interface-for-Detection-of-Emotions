using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BCILib.Amp;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;

namespace ExtgUSBamp
{
    public partial class gUSBamp : LiveAmplifier
    {
        IntPtr handle = IntPtr.Zero;
        ConfigData cfg_data;
        const int HEADER_SIZE = 38;

        private string CfgFile
        {
            get
            {
                string cfg_dir = Path.GetDirectoryName(LogFileName);
                cfg_dir = Path.Combine(cfg_dir, "Config");
                string cfg_fn = Path.Combine(cfg_dir, TypeName + ".bac");
                return cfg_fn;
            }
        }

        public override string GetChannelNameString()
        {
            return "CH1,CH2,CH3,CH4,CH5,CH6,CH7,CH8,CH9,CH10,CH11,CH12,CH13,CH14,CH15,CH16";
        }

        public override bool Initialize()
        {
            if (!OpenDevice()) {
                return false;
            }

            header.nchan = 16;
            header.nevt = 1;
            header.blk_samples = 1;
            header.samplingrate = 256;
            header.resolution = 0;
            header.datasize = 4;
            return true;
        }

        private bool OpenDevice()
        {
            if (handle != IntPtr.Zero) {
                StringBuilder serial_str = new StringBuilder(512);
                int r = GT_GetSerial(handle, serial_str, serial_str.Capacity);
                string serial = serial_str.ToString();
                if (r == 0 || string.IsNullOrEmpty(serial)) {
                    handle = IntPtr.Zero;
                }
            }

            if (handle == IntPtr.Zero) {
                // try to open device
                for (int i = 0; i < 20; i++) {
                    IntPtr h = GT_OpenDevice(i);
                    if (h != IntPtr.Zero) {
                        handle = h;
                        break;
                    }
                }
            }

            if (handle == IntPtr.Zero) return false;

            string fn_cfg = CfgFile;
            if (File.Exists(fn_cfg)) {
                FileStream stream = File.OpenRead(fn_cfg);
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Binder = new gUSBampBinder();
                ConfigData cfg = null;
                try {
                    cfg = (ConfigData)formatter.Deserialize(stream);
                } catch (Exception e) {
                    Console.WriteLine("Error serializeation amp: {0}", e);
                }
                stream.Close();
                if (cfg != null) WriteConfigData(cfg);
                cfg_data = cfg;
            }
            ReadConfigData();

            return true;
        }

        private void ReadConfigData()
        {
            if (cfg_data == null) {
                cfg_data = new ConfigData();
            }

            // General Information
            cfg_data.driver_version = GT_GetDriverVersion();
            cfg_data.hw_version = GT_GetHWVersion(handle);
            StringBuilder serial = new StringBuilder(512);
            int r = GT_GetSerial(handle, serial, serial.Capacity);
            cfg_data.serial_number = serial.ToString();

            // Mode
            GT_GetMode(handle, out cfg_data.mode);
            GT_GetGround(handle, out cfg_data.ground);
            GT_GetReference(handle, out cfg_data.reference);

            // Calibrate
            cfg_data.scale.factor = new float[16];
            cfg_data.scale.offset = new float[16];
            for (int i = 0; i < 16; i++) {
                cfg_data.scale.factor[i] = 0;
                cfg_data.scale.offset[i] = 0;
            }
            GT_GetScale(handle, out cfg_data.scale);
            //GT_Calibrate(handle, ref cfg_data.scale);
            //GT_GetScale(handle, out cfg_data.scale);
        }

        private void WriteConfigData(ConfigData cfg_data)
        {
            // Mode
            GT_SetGround(handle, cfg_data.ground);
            GT_SetReference(handle, cfg_data.reference);
            GT_SetSampleRate(handle, cfg_data.sample_rate);
        }

        protected override void ReceiveDataLoop()
        {
            header.samplingrate = cfg_data.sample_rate;

            int rv = GT_SetMode(handle, AmpMode.NORMAL);
            int num_scan = 8; //cfg_data.sample_rate * buf_time / 1000;
            header.blk_samples = num_scan;

            rv = GT_SetBufferSize(handle, (ushort) num_scan); 

            if (GT_Start(handle) == 0) {
                Console.WriteLine("gUSBamp: Cannot start!"); 
                return;
            }

            int last_evt = 0;

            rv = GT_ResetTransfer(handle);

            int raw_sz = HEADER_SIZE + num_scan * header.nchan * 4;
            IntPtr rdata = Marshal.AllocCoTaskMem(raw_sz);
            if (rdata == IntPtr.Zero) {
                MessageBox.Show("Cannot allocate data buffer!");
                Close();
                return;
            }

            byte[] blk_data = new byte[header.BlkSize];
            int blk_sn = 0;

            byte[] buf = new byte[raw_sz];

            NativeOverlapped nov = new NativeOverlapped();
            ManualResetEvent mevt = new ManualResetEvent(false);
            nov.EventHandle = mevt.SafeWaitHandle.DangerousGetHandle();

            while (bRunning) {
                rv = GT_GetData(handle, rdata, buf.Length, ref nov);
                //rv = WaitForSingleObject(hwait, 1000);
                if (!mevt.WaitOne(1000, false)) {
                    Console.WriteLine("gUSBamp: waiting time out. continue.");
                    rv = GT_ResetTransfer(handle);
                    continue;
                }
                //if (rv != 0) {
                //    if (rv == 0x102) {
                //        Console.WriteLine("gUSBamp: waiting time out. continue.");
                //        rv = GT_ResetTransfer(handle);
                //        continue;
                //    } else {
                //        Console.WriteLine("gUSBamp: Waiting error: {0}", rv);
                //        break;
                //    }
                //}

                int rsz = 0;
                GetOverlappedResult(handle, ref nov, out rsz, false);
                mevt.Reset();

                Marshal.Copy(rdata, buf, 0, rsz);

                int rspls = num_scan;
                if (rsz != buf.Length) {
                    rspls = (rsz - HEADER_SIZE) / (header.nchan * 4);
                }

                int buf_sn = 0;
                for (int sno = 0; sno < rspls; sno++) {
                    Array.Copy(buf, HEADER_SIZE + buf_sn, blk_data, blk_sn, header.nchan * 4);
                    blk_sn += header.nchan * 4;
                    buf_sn += header.nchan * 4;

                    int evt = 0;
                    if (last_evt == 0 && code_que.Count > 0) {
                        evt = code_que.Dequeue();
                    }
                    BitConverter.GetBytes(evt).CopyTo(blk_data, blk_sn);
                    blk_sn += 4;
                    last_evt = evt;

                    if (blk_sn >= blk_data.Length) {
                        PoolAddBuffer(blk_data);
                        blk_sn = 0;
                    }
                }
            }
            rv = GT_Stop(handle);

            Marshal.FreeCoTaskMem(rdata);
            Close();
        }

        private void Close()
        {
            if (handle != IntPtr.Zero) {
                GT_CloseDevice(ref handle);
            }
        }

        public override bool Configure()
        {
            if (!OpenDevice()) {
                MessageBox.Show("Cannot open EEG device!");
                return false;
            }

            string fn_cfg = CfgFile;

            gUSBampConfig cfg_form = new gUSBampConfig();

            // Filter
            int nflt = 0;
            GT_GetNumberOfFilter(out nflt);
            FILT[] fspec = new FILT[nflt];
            GT_GetFilterSpec(fspec);
            cfg_form.SetFilterSpec(fspec);

            GT_GetNumberOfNotch(out nflt);
            FILT[] nspec = new FILT[nflt];
            GT_GetNotchSpec(nspec);
            cfg_form.SetNotchData(nspec);

            cfg_form.SetConfigData(cfg_data);

            if ((cfg_form.ShowDialog() == System.Windows.Forms.DialogResult.OK)) {
                cfg_form.UpdateData(cfg_data);
                WriteConfigData(cfg_data);

                FileStream stream = File.OpenWrite(fn_cfg);
                BinaryFormatter formater = new BinaryFormatter();

                // write file
                try {
                    formater.Serialize(stream, cfg_data);
                } catch (Exception e) {
                    Console.WriteLine("Error serializeation amp: {0}", e); 
                }

                stream.Close();

                return true;
            }
            return false;
        }

        [Serializable]
        public class ConfigData
        {
            // General Infomation
            public float driver_version;
            public float hw_version;
            public string serial_number;

            // mode
            public AmpMode mode = AmpMode.NORMAL;
            public GND ground = new GND {GND1 = 1, GND2 = 1, GND3 = 1, GND4 = 1};
            public REF reference = new REF { ref1 = 1, ref2 = 1, ref3 = 1, ref4 = 1 };

            // scale
            public SCALE scale = new SCALE();
            public ushort sample_rate = 256;

            // filter
            public int band_pass = -1;
            public int notch = -1;
        }

        private class gUSBampBinder : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                return this.GetType().Assembly.GetType(typeName);
            }
        }
    }
}
