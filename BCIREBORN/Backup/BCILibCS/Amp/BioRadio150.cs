using System;
using System.Collections.Generic;
using System.Text;

using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Reflection;

using BCILib.App;
using BCILib.Util;

namespace BCILib.Amp
{
    internal partial class BioRadio150 : LiveAmplifier
    {
        static BioRadio150()
        {
            if (!ReadConfig()) {
                throw new Exception("Cannot read configuration file.");
            }
        }

        public BioRadio150()
        {
            header.samplingrate = sampling_rate;
            header.resolution = resolution;
            header.blk_samples = blk_samples;
        }

        public static string ChannelNameString
        {
            get
            {
                return "CH1,CH2,CH3,CH4,CH5,CH6,CH7,CH8";
            }
        }

        public override string GetChannelNameString()
        {
            return ChannelNameString;
        }

        public override string[] ChannelNames
        {
            get
            {
                return ChannelNameString.Split(',');
            }
        }

        private uint hd = 0;
        //private ushort did = 0;
        private string port = null;

        /// <summary>
        /// Open device, find amplifier configurations
        /// </summary>
        /// <returns></returns>
        public override bool Initialize()
        {
            // Find out all devices
            TDeviceInfo[] dinfo = new TDeviceInfo[16];
            int dc = 0;

            if (!FindDevices(dinfo, out dc, 16, null) || dc == 0)
            {
                LogMessage("BiodRadio150.FindDevice: Cannot find device!");
                return false;
            }

            if (dc <= 0) return false;
            int scid = -1;

            ArrayList portlist = new ArrayList(dinfo.Length);
            for (int i = 0; i < dc; i++)
            {
                int n = portlist.Add(dinfo[i].PortName);
                if (scid == -1 && dinfo[i].PortName == port) scid = i;
            }

            if (scid > 0)
            {
                // adjust the sequence so that scid is first to try.
                object o = portlist[scid];
                portlist[scid] = portlist[0];
                portlist[0] = o;
                scid = 0;
            }

            for (int i = 0; i < portlist.Count; i++)
            {
                hd = CreateBioRadio();
                int rv = StartCommunication(hd, (string)portlist[i]);
                if (rv != 1)
                {
                    DestroyBioRadio(hd);
                    hd = 0;
                    continue;
                }


                // Get other infomation
                ushort did1 = 0;
                GetDeviceID(hd, out did1);

                //if (did1 != did) {
                //    StopCommunication(hd);
                //    DestroyBioRadio(hd);
                //    hd = 0;
                //    continue;
                //}

                // change port number
                if (i != scid)
                {
                    port = (string)portlist[i];
                    SaveConfig(BCIApplication.AppResource);
                }

                break;
            }

            if (hd == 0) {
                return false;
            }

            DisableBuffering(hd);
            SetBadDataValues(hd, 0, (ushort)zeroOffset);

            if (StartAcq(hd, 1) != 1) {
                LogMessage("StartAcq failed!");
                StopCommunication(hd);
                DestroyBioRadio(hd);
                hd = 0;
                return false;
            }

            if (ProgramConfig(hd, 1, ConfigFile) != 1) {
                LogMessage("ProgramConfig failed!");
                StopAcq(hd);
                StopCommunication(hd);
                DestroyBioRadio(hd);
                hd = 0;
                return false;
            }

            header.nchan = GetNumEnabledFastInputs(hd);
            header.nevt = 1;
            header.datasize = 4;
            header.samplingrate = (int) GetSampleRate(hd);
            header.blk_samples = 10;

            return true;
        }

        private static string ConfigFile
        {
            get
            {
                string dir = BCIApplication.RootPath;
                return Path.Combine(dir, "BioRadio-8-EEG.ini");
            }
        }

        private static int sampling_rate = 0;
        private static int blk_samples = 0;
        private static float resolution = 0;
        private static int zeroOffset = 0;

        private static bool ReadConfig()
        {
            string fn = ConfigFile;
            LogMessage("BioRadio150: configuration file:{0}", fn);

            if (!File.Exists(fn)) {
                LogMessage("BioRadio150: config file not exists!");
                return false;
            }

            StreamReader sr = new StreamReader(fn);
            string line;
            //int state = -1;

            while ((line = sr.ReadLine()) != null) {
                if (line == "[System]") {
                    //state = 1;
                    continue;
                } else if (line == "[DAQ Board]") {
        			//state = 2;
		        	continue;
		        }
		        // system settings
		        else if (line.StartsWith("SystemSampleRate=")) {
			        sampling_rate = int.Parse(line.Substring(17));
		        } else if (line.StartsWith("BitResolution=")) {
			        //LogMessage("BitResolution: {0}", int.Parse(line.Substring(14)));
		        } else if (line.StartsWith("SweepsPerPacket=")) {
                    blk_samples = int.Parse(line.Substring(16));
		        }
		        // channel settings
                else if (line.StartsWith("Channel_")) {
                    int ch = -1, enable = 0, adc_upper = 0, adc_lower = 0;
                    double scale_upper = 0, scale_lower = 0, unit = 1000000;

                    string[] alist = line.Split(',');
                    foreach (string vstr in alist) {
                        string[] nvpair = vstr.Split('=');
                        if (nvpair[0].StartsWith("Channel_")) {
                            ch = int.Parse(nvpair[0].Substring(8));
                            continue;
                        }

                        switch (nvpair[0]) {
                            case "Enabled":
                                enable = int.Parse(nvpair[1]);
                                break;
                            case "InputName":
                                break;
                            case "UpperScale":
                                scale_upper = double.Parse(nvpair[1]);
                                break;
                            case "LowerScale":
                                scale_lower = double.Parse(nvpair[1]);
                                break;
                            case "ADCUpper":
                                adc_upper = int.Parse(nvpair[1]);
                                break;
                            case "ADCLower":
                                adc_lower = int.Parse(nvpair[1]);
                                break;
                            case "Units":
                                if (string.Compare(nvpair[1], "uV", true) == 0) unit = 1;
                                else if (string.Compare(nvpair[1], "mV", true) == 0) unit = 1000;
                                break;
                        }
                    }

                    if (enable != 0 && ch >= 0 && ch <= 8) {
                        double res = (scale_upper - scale_lower) / (adc_upper - adc_lower);
                        if (resolution == 0) {
                            resolution = (float) res;
                            resolution = (float)(res * unit);
                            zeroOffset = (adc_upper + adc_lower) / 2;
                            //printf("Zero offset = %d, resulotion = %lg", m_nZeroOffset, get_resolution());
                        }
                        else if (resolution != res) {
                            LogMessage("Resolution is different for channels!");
                            //printf("Error in configuration file: setting different!\n");
                        }
                    }
                } // channel
			}

            sr.Close();
            return true;
        }

        private void Close()
        {
            if (hd != 0) {
                StopAcq(hd);
                StopCommunication(hd);
                DestroyBioRadio(hd);
                hd = 0;
            }
        }

        protected override void ReceiveDataLoop()
        {
            int RAW_DATA_SZ = 480;
            ushort[] raw_data_buf = new ushort[RAW_DATA_SZ];
            int nch = header.nchan;
            int blk_sn = 0;
            byte[] buf = new byte[header.BlkSize];

            EnableBuffering(hd);

            int last_evt = 0;
            while (bRunning) {
                // Reading data from devices
                int FastInputsNumRead = 0;
                int NumToRead = TransferBuffer(hd);
                if (NumToRead > 0)
                {
                    if (NumToRead > RAW_DATA_SZ)
                    {
                        LogMessage("BioRadio150.ReceiveData(dev{0}): WARNING received {1} too big for buffer size {2}.\n",
                            hd, NumToRead, RAW_DATA_SZ);
                    }

                    ReadRawData(hd, raw_data_buf, RAW_DATA_SZ, out FastInputsNumRead);

                    int nr = FastInputsNumRead / nch;
                    int cpos = 0;
                    for (int si = 0; si < nr; si++)
                    {
                        //Array.Copy(raw_data_buf, fpos, rec_data_buf, tpos, nch);
                        for (int c = 0; c < nch; c++, cpos++)
                        {
                            int val = raw_data_buf[cpos] - zeroOffset;
                            byte[] bvl = BitConverter.GetBytes(val);
                            bvl.CopyTo(buf, blk_sn);
                            blk_sn += 4;
                        }

                        int v = 0;
                        if (last_evt == 0 && code_que.Count > 0)
                        {
                            var scode = code_que.Peek();
                            if (scode.splno <= Rd_GetPos() + si) {
                                v = scode.code;
                                code_que.Dequeue();
                            }
                        }

                        BitConverter.GetBytes(v).CopyTo(buf, blk_sn);
                        blk_sn += 4;
                        last_evt = v;

                        if (blk_sn >= buf.Length)
                        {
                            PoolAddBuffer(buf);
                            blk_sn = 0;
                        }
                    }
                    UpdateActSplRate();
                }

                //printf("Reading = %d", nr);
                System.Threading.Thread.Sleep(10);
            }
            Close();
            LogMessage("BioRadio150.ReceiveData exits.");
        }

        public override bool Configure()
        {
            BioRadio150Cfg cfg = new BioRadio150Cfg();

            // find all devices

            TDeviceInfo[] dinfo = new TDeviceInfo[16];
            int dc = 0;

            if (!FindDevices(dinfo, out dc, 16, null) || dc == 0) {
                LogMessage("BiodRadio150.FindDevice: Cannot find device!");
                return false;
            }

            LogMessage("BiodRadio150.FindDevice: found {0} devices.", dc);
            if (dc <= 0) return false;

            for (int i = 0; i < dc; i++) {
                LogMessage("Found device {0}:{1}", i + 1, dinfo[i].PortName);
                //uint hd = CreateBioRadio();
                //int rv = StartCommunication(hd, dinfo[i].PortName);
                //if (rv != 1) {
                //    LogMessage("StartCommunication failed!", i);
                //    DestroyBioRadio(hd);
                //    continue;
                //}

                //// Get other infomation
                //StringBuilder model = new StringBuilder(256);
                //ushort did = 0;
                //StringBuilder IDString = new StringBuilder(256);

                //GetBioRadioModelString(hd, model, 256);
                //GetDeviceID(hd, out did);
                //GetDeviceIDString(hd, IDString, 256);

                //StringBuilder firmware = new StringBuilder(256);
                //GetFirmwareVersionString(hd, firmware, 256);

                //StopCommunication(hd);
                //DestroyBioRadio(hd);

                ////string did_str = IDString.ToString();
                //LogMessage("Model={0}, deviceID={1}/{3}, firmware={2}.",
                //    model, did, firmware, IDString);

                cfg.AddDevice(//new string(new char[] {(char) (did >> 8), (char)(did & 0xFF)}),
                    //did, 
                    dinfo[i].PortName, dinfo[i].PortName.Equals(port)); //did == this.did
            }

            if (cfg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                // Save port and id
                string devstr = cfg.SelectedDevice;
                //string[] alist = devstr.Split('|');
                //if (!Reconfig) {
                    // set ID for new amplifier
                    //ID = TypeName + "_" + alist[1];
                //}

                // save comport and id
                //did = Convert.ToUInt16(alist[1]);
                //port = alist[2];
                port = devstr;
                return true;
            }

            return false;
        }

        protected override void SaveConfigSpecial(ResManager rm)
        {
            rm.SetConfigValue(ID, "Port", port);
            //rm.SetConfigValue(ID, "ID", did.ToString());
        }

        protected override void SetSpecialConfig(ResManager rm)
        {
            rm.GetConfigValue(ID, "Port", ref port);
            //int iv = 0;
            //rm.GetConfigValue(ID, "ID", ref iv);
            //did = (ushort)iv;
        }

        public override string DevName
        {
            get
            {
                //char[] id = new char[] {(char) (did >> 8),  (char) (did & 0xFF)};
                //return TypeName + ":" + new string(id);
                return base.DevName + ":" + port;
            }
        }
    }
}
