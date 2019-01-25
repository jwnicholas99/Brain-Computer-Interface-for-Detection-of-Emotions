using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;

namespace BCILib.Amp
{
    public class NCCAmp : LiveAmplifier
    {
        #region Driver Function
        [StructLayout(LayoutKind.Sequential)]
        class DevInfo
        {
            /// <summary>
            ///  devcie sequentiall no;
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] LicenseCode;

            /// <summary>
            /// 0=Ready, last command successful, can accept next command
            /// 1=Ready, last command failed, can accept next command
            /// 2=Ready for data transmission, PC can upload/download data
            /// 3=Busy in processing command, cannot accept new command
            /// </summary>
            public byte Status;

            /// <summary>
            /// 0x00 = 8 channel
            /// 0x40 = 8 channel + Sleep
            /// 0x04 = 16 channel
            /// 0x44 = 16 channel + 
            /// 0x08 = 32 channel
            /// 0x48 = 32 channel + 
            /// 0x09 = 64 channel
            /// 0x0a = 128 channel
            /// </summary>
            public byte MaxConfig;

            /// <summary>
            /// Sampling rate
            /// 0 = 128 Hz
            /// 1 = 256 Hz
            /// 2 = 512 Hz
            /// 3 = 1024 Hz
            /// </summary>
            public byte FreqIndex;

            /// <summary>
            /// Current configuration index, 0x04 = 16 channel
            /// </summary>
            public byte CurConfig;

            /// <summary>
            /// 0 = No SD, 1 = With SD
            /// </summary>
            public byte SDCard;
        }

        // External functions from HardUsb.dll

        /// <summary>
        /// Open DAQ and Flash devices.
        /// </summary>
        /// <returns>
        /// 0 = successul for both devices;
        /// -1 = failure for both devices;
        /// 1 = DAQ sucess, Flash fail
        /// 2 = DAQ fail, Flash success
        /// </returns>
        [DllImport("HardUsb.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int OpenDevice();

        /// <summary>
        /// Close DAQ and Flash devices
        /// </summary>
        [DllImport("HardUsb.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern void CloseDevice();

        /// <summary>
        /// Start DAQ
        /// </summary>
        /// <returns></returns>
        [DllImport("HardUsb.dll", EntryPoint = "StartGather", CallingConvention = CallingConvention.StdCall)]
        private static extern bool StartACQ();

        /// <summary>
        /// Stop ACQ
        /// </summary>
        /// <returns></returns>
        [DllImport("HardUsb.dll", EntryPoint = "StopGather", CallingConvention = CallingConvention.StdCall)]
        private static extern bool StopACQ();

        /// <summary>
        /// Configure amplifer parameters
        /// </summary>
        /// <param name="cfg_idx">1-8</param>
        /// <param name="freq">128/256/512/1024</param>
        /// <param name="gain"></param>
        /// <returns></returns>
        [DllImport("HardUsb.dll", EntryPoint = "SetUpGatherDevice", CallingConvention = CallingConvention.StdCall)]
        private static extern bool SetupACQ(int cfg_idx, int freq, int gain);

        /// <summary>
        /// Configure Flash device
        /// </summary>
        /// <param name="freq"></param>
        /// <returns></returns>
        [DllImport("HardUsb.dll", EntryPoint = "SetUpFlashDevice", CallingConvention = CallingConvention.StdCall)]
        private static extern bool SetupFlash(int freq);

        /// <summary>
        /// Start Flash Device
        /// </summary>
        /// <returns></returns>
        [DllImport("HardUsb.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern bool StartFlash();

        /// <summary>
        /// Stop Flash devie
        /// </summary>
        /// <returns></returns>
        [DllImport("HardUsb.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern bool StopFlash();

        [DllImport("HardUsb.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int ReadData([Out] ushort[] data, int dlen);


        [DllImport("HardUsb.dll", EntryPoint = "GetGatherDeviceInfo", CallingConvention = CallingConvention.StdCall)]
        private static extern bool GetACQDeviceInfo([Out] DevInfo dinf);

        #endregion Driver Function

        public NCCAmp()
        {
            header.nchan = 16;
            header.nevt = 1;
            header.samplingrate = 256;
            header.blk_samples = 8;
            header.datasize = 4;
            header.resolution = (float)(2400000.0 / 4096 / 1150);
        }

        public override bool Initialize()
        {
            return true;
        }

        protected override void ReceiveDataLoop()
        {
            byte[] wbuf = new byte[(header.nchan + 1) * header.blk_samples * 4];
            // trick: must be multiplier of 64!
            int rsz = 64 * 16;
            ushort[] rdata = new ushort[rsz / 2];
            int last_evt = 0;

            while (bRunning) {
                int rv = OpenDevice();
                if (rv != 0 && rv != 1) {
                    if (Status != AmpStatus.Checking) {
                        Console.WriteLine("NCCAmp: error OpenDevice!");
                        Status = AmpStatus.Checking;
                    }
                    CloseDevice();
                    Thread.Sleep(500);
                    continue;
                }


                DevInfo dinf = new DevInfo();
                if (!SetupACQ(3, 256, 1) || !GetACQDeviceInfo(dinf) || dinf.FreqIndex != 1) {
                    if (Status != AmpStatus.Checking) {
                        Console.WriteLine("NCCAmp: Error SetupACQ!");
                        Status = AmpStatus.Checking;
                    }
                    StopACQ();
                    CloseDevice();
                    Thread.Sleep(100);
                    continue;
                }

                if (!StartACQ()) {
                    if (Status != AmpStatus.Checking) {
                        Console.WriteLine("NCCAmp: Error StartACQ!");
                        Status = AmpStatus.Checking;
                    }
                    StopACQ();
                    CloseDevice();
                    Thread.Sleep(100);
                    continue;
                }

                int iread = 0; // index to the reading frame
                int cpos = 0; // writing position into wbuf
                int nskip = 0; // number of words read before AA55
                long rsi = 0; // sample counter

                byte[] rseq = { 0, 2, 4, 6, 1, 3, 5, 7, 8, 10, 12, 14, 9, 11, 13, 15, 16, 18, 17, 19};

                while (bRunning) {
                    // read header
                    int nr = ReadData(rdata, rsz);
                    if (nr == -1) {
                        if (Status != AmpStatus.Checking) {
                            Status = AmpStatus.Checking;
                        }
                        Debug.WriteLine("NCCAmp: read -1? rsz=" + rsz);
                        break;
                    }

                    Debug.Print("Read size={0}", nr);
                    Debug.Print("rn={0}, cpos = {1}", Rd_GetPos(), cpos);
                    for (int ri = 0; ri < nr / 2; ri++) {
                        if (iread == 0) {
                            if (rdata[ri] == 0xAA55) {
                                iread++;
                                rsi++;
                                if (nskip > 0) {
                                    Debug.Print(rsi.ToString() + ",NSKIP=" + nskip);
                                }
                            } else {
                                nskip++;
                            }
                            continue;
                        } else if (iread == 1) {
                            //Debug.Print("Status: {0:X}", rdata[ri]);
                        } else if (iread == 2) {
                            //Debug.Print("Data:  {0:X}", rdata[ri]);
                        }

                        iread++;
                        if (iread <= 3) {
                            continue;
                        }

                        if (iread - 3 <= header.nchan && nskip == 0) {
                            int iv = rdata[ri] - 2048;
                            BitConverter.GetBytes(iv).CopyTo(wbuf, cpos + rseq[iread - 4] * 4);

                            if (iread - 3 == header.nchan) {
                                cpos += header.nchan * 4;

                                int evt = 0;
                                if (last_evt == 0 && code_que.Count > 0) {
                                    evt = code_que.Dequeue();
                                }
                                BitConverter.GetBytes(evt).CopyTo(wbuf, cpos);
                                cpos += 4;
                                last_evt = evt;

                                if (cpos == wbuf.Length) {
                                    PoolAddBuffer(wbuf);
                                    cpos = 0;
                                } else if (cpos > wbuf.Length) {
                                    // Wrong
                                    Debug.Print("Wrong???");
                                }
                            }
                        }

                        if (iread == 23) {
                            iread = 0;
                            nskip = 0;
                        }
                    }

                    if (Status != AmpStatus.Connected) {
                        Status = AmpStatus.Connected;
                    }
                }
            }

            StopACQ();
            CloseDevice();
        }

        public override string GetChannelNameString()
        {
            if (!string.IsNullOrEmpty(channel_namestr)) {
                return channel_namestr;
            } else {
                return string.Join(",", Enumerable.Range(1, header.nchan)
                    .Select(x => "Ch" + x.ToString()).ToArray());
            }
        }

        string channel_namestr = null;

        public override bool Configure()
        {
            var dlg = new NCCCfgForm();
            dlg.ChannelNames = ChannelNames;
            if (dlg.ShowDialog() == DialogResult.OK) {
                channel_namestr = string.Join(",", dlg.ChannelNames);
                return true;
            }

            return false;
        }

        protected override void SetSpecialConfig(BCILib.Util.ResManager rm)
        {
            channel_namestr = rm.GetConfigValue(ID, "ChannelNameString");
        }

        protected override void SaveConfigSpecial(BCILib.Util.ResManager rm)
        {
            if (!string.IsNullOrEmpty(channel_namestr)) {
                rm.SetConfigValue(ID, "ChannelNameString", channel_namestr);
            }
        }
    }
}
