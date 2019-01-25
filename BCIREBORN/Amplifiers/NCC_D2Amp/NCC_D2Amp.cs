using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;

namespace BCILib.Amp
{
    public class NCC_D2Amp : LiveAmplifier
    {
        #region Driver Function

        // External functions from HardUsb-D2.dll
        [DllImport("HardUsb-D2.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern bool ReSetStart();

        [DllImport("HardUsb-D2.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern bool ReSetStop();

        [DllImport("HardUsb-D2.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern bool Begin(NSD2_DATA_TYPE dtype); // Start ACQ

        [DllImport("HardUsb-D2.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern bool Stop(int size); // Stop ACQ

        [DllImport("HardUsb-D2.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern bool SetFlash(int freq); // Set Flash frequency

        [DllImport("HardUsb-D2.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern bool FlashCtrl(int size); // Flash control

        [DllImport("HardUsb-D2.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern bool SetFreq(int freq);

        [DllImport("HardUsb-D2.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern bool SetChannel(int[] channels); // Set current acuisition channels;  length = 128?

        /// <summary>
        /// Set maximum channel configuration.
        /// </summary>
        /// <param name="size">
        /// 0=ConfigI: 24EEG+12BiPolar
        /// 1=ConfigII: 24EEG+12BiPolar+Sleep
        /// 2=ConfigIII: 48EEG+12BiPolar
        /// 3=ConfigIV: 48EEG+12BiPolar+Sleep
        /// 4=ConfigV: 24EEG
        /// </param>
        /// <returns></returns>
        [DllImport("HardUsb-D2.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern bool SetMontage(int size); // Set maximum channel confiuration

        [DllImport("HardUsb-D2.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern bool SetImpTest([Out] int[] channels); // Set PC impedance test result / length = 128

        [DllImport("HardUsb-D2.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern bool SetAmpVersion(string amp_version); // Set amplifier serial number /length = 5

        [DllImport("HardUsb-D2.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern bool ReadDevStat([Out] NSD2_DEV_STAT dev_stat);

        [DllImport("HardUsb-D2.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern bool RefreshAmpInfo();

        [DllImport("HardUsb-D2.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern bool ReadAmpInfo([Out] NSD2_AMP_STAT amp_stat);

        [DllImport("HardUsb-D2.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern bool SetStim(int size, byte[] data); // data length = 54

        [DllImport("HardUsb-D2.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int ReadData([Out] uint[] buf, int rlen);

        [DllImport("HardUsb-D2.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern bool OpenUsb();

        [DllImport("HardUsb-D2.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern bool CloseUsb();

        #endregion Driver Function

        private string cfg_serial_no = "13000113";

        public NCC_D2Amp()
        {
            header.nchan = 24;
            header.nevt = 1;
            header.samplingrate = 256;
            header.blk_samples = 10;
            header.datasize = 4;
            header.resolution = (float)(100.0 / 0X100000);
        }

        public override bool Initialize()
        {
            return true;
        }

        protected override void ReceiveDataLoop()
        {
            byte[] wbuf = new byte[(header.nchan + 1) * header.blk_samples * 4];
            int rsz = 1024;
            uint[] rdata = new uint[rsz];
            int last_evt = 0;

            //int[] tv = new int[header.samplingrate];
            //int ti = 0;


            while (bRunning) {
                bool rv = OpenUsb();
                if (!rv) {
                    if (Status != AmpStatus.Checking) {
                        Console.WriteLine("NCCAmp: error OpenUsb!");
                        Status = AmpStatus.Checking;
                    }
                    CloseUsb();
                    Thread.Sleep(500);
                    continue;
                }

                //if (!SetFlash(1)) {
                //    Debug.Print("SetFalsh returned false!");
                //} else if (!FlashCtrl(0)) {
                //    Debug.Print("Start Flash Error");
                //} else {
                //    Thread.Sleep(10);
                //    if (!FlashCtrl(1)) {
                //        Debug.Print("Stop Flash Error!");
                //    }
                //}

                if (!RefreshAmpInfo()) {
                    Console.WriteLine("RefreshAmpInfo Error!");
                }

                NSD2_AMP_STAT amp_stat = new NSD2_AMP_STAT();
                if (!ReadAmpInfo(amp_stat)) {
                    Console.WriteLine("Error in ReadAmpInfo())");
                }

                NSD2_DEV_STAT dev_stat = new NSD2_DEV_STAT();
                if (!SetAmpVersion(cfg_serial_no)) {
                    Console.WriteLine("Error in SetAmpVersion()!");
                }

                if (!ReadDevStat(dev_stat)) {
                    Console.WriteLine("Error in ReadDevStat()");
                }

                //if (!SetImpTest())

                if (!SetFreq(256)) {
                    Console.WriteLine("Error in SetFreq!");
                }

                if (!SetMontage(4)) {
                    Console.WriteLine("Error in SetMontage(4)");
                }

                int[] chlist = new int[128];
                for (int i = 0; i < header.nchan; i++) chlist[i] = 1;
                if (!SetChannel(chlist)) {
                    Console.WriteLine("Error in SetChannel");
                }


                if (!ReSetStart()) {
                    Console.WriteLine("Error in ReSetStart()");
                } else {
                    if (!ReSetStop()) {
                        Console.WriteLine("Error in ReSetStop()");
                    }
                }

                //Begin(NSD2_DATA_TYPE.IMPEDANCE_TEST_THRESHOLD_20K);
                //Thread.Sleep(5000);

                //for (int i = 0; i < chlist.Length; i++) chlist[i] = 1;
                //if (!SetImpTest(chlist)) {
                //    Debug.Print("SetImpTest returns error!");
                //}

                //Stop(1);

                if (!Begin(NSD2_DATA_TYPE.EEG_DATA)) {
                    if (Status != AmpStatus.Checking) {
                        Console.WriteLine("Error Start!");
                        Status = AmpStatus.Checking;
                    }
                    Stop(0);
                    CloseUsb();
                    Thread.Sleep(100);
                    continue;
                }

                int iread = 0; // index to the reading frame
                int cpos = 0; // writing position into wbuf
                int nskip = 0; // number of int read before AA55
                long rsi = 0; // sample counter
                int nfhd = 1; //3

                while (bRunning) {
                    // read header
                    int nr = ReadData(rdata, rsz);

                    //Debug.Print("Read size={0}", nr);
                    //Debug.Print("rn={0}, cpos = {1}, iread={2}", Rd_GetPos(), cpos, iread);
                    if (nr > rsz / 4) nr = rsz / 4;

                    for (int ri = 0; ri < nr; ri++) {
                        if (iread == 0) {
                            if (rdata[ri] == 0x55AA55AA) {
                                iread++;
                                rsi++;
                                if (nskip > 0) {
                                    Debug.Print(rsi.ToString() + ",NSKIP=" + nskip);
                                }
                            } else {
                                nskip++;
                            }
                            continue;
                        //} else if (iread == 1) {
                        //    //Debug.Print("Status: {0:X}", rdata[ri]);
                        //} else if (iread == 2) {
                        //    //Debug.Print("Data:  {0:X}", rdata[ri]);
                        }

                        iread++;
                        if (iread <= nfhd) {
                            continue;
                        }

                        if (iread - nfhd <= header.nchan) {
                            int iv = (int)(rdata[ri] & 0XFFFFFF) - 0x800000;
                            BitConverter.GetBytes(iv).CopyTo(wbuf, cpos + (iread - nfhd - 1) * 4);

                            //if (ti < header.samplingrate && iread == nfhd + 1) {
                            //    tv[ti] = iv;
                            //    ti++;
                            //    if (ti == header.samplingrate) {
                            //        int avr = (int) tv.Average();
                            //        Console.WriteLine("Average={0}, {1:X}", avr, avr);
                            //        int xmax = tv.Max();
                            //        Console.WriteLine("Maximum={0}, {1:X}", xmax, xmax);
                            //        int xmin = tv.Min();
                            //        Console.WriteLine("Minumum={0}, {1:X}", xmin, xmin);
                            //        ti = 0;
                            //    }
                            //}

                            if (iread - nfhd == header.nchan) {
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

                                iread = 0;
                                nskip = 0;
                            }
                        }

                    }

                    if (Status != AmpStatus.Connected) {
                        Status = AmpStatus.Connected;
                    }
                }
            }

            Stop(0);
            Stop(1);
            Stop(2);
            CloseUsb();
        }

        public override string GetChannelNameString()
        {
            return "Fp1,Fp2,F3,F4,C3,C4,P3,P4,O1,O2,F7,F8,T3,T4,T5,T6,Sp1,Sp2,Fz,Cz,Pz,Oz,A1,A2";
        }

        public override bool Configure()
        {
            Stop();
            var dlg = new NCC_D2CfgForm();
            dlg.SerialNumber = cfg_serial_no;
            dlg.ImpRange = cfg_imp_range;
            if (dlg.ShowDialog() == DialogResult.OK) {
                cfg_serial_no = dlg.SerialNumber;
                cfg_imp_range = dlg.ImpRange;
                return true;
            }

            return false;
        }

        private string cfg_imp_range = "5,100";

        protected override void SetSpecialConfig(BCILib.Util.ResManager rm)
        {
            string vstr = rm.GetConfigValue(ID, "SerialNumber");
            if (!string.IsNullOrEmpty(vstr)) cfg_serial_no = vstr;
            vstr = rm.GetConfigValue(ID, "ImpRange");
            if (!string.IsNullOrEmpty(vstr)) cfg_imp_range = vstr;
        }

        protected override void SaveConfigSpecial(BCILib.Util.ResManager rm)
        {
            if (!string.IsNullOrEmpty(cfg_serial_no)) {
                rm.SetConfigValue(ID, "SerialNumber", cfg_serial_no);
            }
            rm.SetConfigValue(ID, "ImpRange", cfg_imp_range);
        }
    }

    public enum NSD2_DATA_TYPE
    {
        EEG_DATA,
        IMPEDANCE_TEST_THRESHOLD_5K,
        IMPEDANCE_TEST_THRESHOLD_10K,
        IMPEDANCE_TEST_THRESHOLD_15K,
        IMPEDANCE_TEST_THRESHOLD_20K,
        IMPEDANCE_TEST_THRESHOLD_40K,
        INTERNAL_CALIBRA_onetenthF_50uAPP_SINE,
        INTERNAL_CALIBRA_onetenthF_100uAPP_SINE,
        INTERNAL_CALIBRA_onetenthF_200uAPP_SINE,
        INTERNAL_CALIBRA_2F_50uAPP_SINE,
        INTERNAL_CALIBRA_2F_100uAPP_SINE,
        INTERNAL_CALIBRA_2F_200uAPP_SINE,
        INTERNAL_CALIBRA_5F_50uAPP_SINE,
        INTERNAL_CALIBRA_5F_100uAPP_SINE,
        INTERNAL_CALIBRA_5F_200uAPP_SINE,
        INTERNAL_CALIBRA_10F_50uAPP_SINE,
        INTERNAL_CALIBRA_10F_100uAPP_SINE,
        INTERNAL_CALIBRA_10F_200uAPP_SINE,
        INTERNAL_CALIBRA_20F_50uAPP_SINE,
        INTERNAL_CALIBRA_20F_100uAPP_SINE,
        INTERNAL_CALIBRA_20F_200uAPP_SINE,
        INTERNAL_CALIBRA_30F_50uAPP_SINE,
        INTERNAL_CALIBRA_30F_100uAPP_SINE,
        INTERNAL_CALIBRA_30F_200uAPP_SINE,
        INTERNAL_CALIBRA_50F_50uAPP_SINE,
        INTERNAL_CALIBRA_50F_100uAPP_SINE,
        INTERNAL_CALIBRA_50F_200uAPP_SINE,
        INTERNAL_CALIBRA_60F_50uAPP_SINE,
        INTERNAL_CALIBRA_60F_100uAPP_SINE,
        INTERNAL_CALIBRA_60F_200uAPP_SINE,
        INTERNAL_CALIBRA_onetenthF_50uAPP_SQUARE,
        INTERNAL_CALIBRA_onetenthF_100uAPP_SQUARE,
        INTERNAL_CALIBRA_onetenthF_200uAPP_SQUARE,
        INTERNAL_CALIBRA_2F_50uAPP_SQUARE,
        INTERNAL_CALIBRA_2F_100uAPP_SQUARE,
        INTERNAL_CALIBRA_2F_200uAPP_SQUARE,
        INTERNAL_CALIBRA_5F_50uAPP_SQUARE,
        INTERNAL_CALIBRA_5F_100uAPP_SQUARE,
        INTERNAL_CALIBRA_5F_200uAPP_SQUARE,
        INTERNAL_CALIBRA_10F_50uAPP_SQUARE,
        INTERNAL_CALIBRA_10F_100uAPP_SQUARE,
        INTERNAL_CALIBRA_10F_200uAPP_SQUARE,
        INTERNAL_CALIBRA_20F_50uAPP_SQUARE,
        INTERNAL_CALIBRA_20F_100uAPP_SQUARE,
        INTERNAL_CALIBRA_20F_200uAPP_SQUARE,
        INTERNAL_CALIBRA_30F_50uAPP_SQUARE,
        INTERNAL_CALIBRA_30F_100uAPP_SQUARE,
        INTERNAL_CALIBRA_30F_200uAPP_SQUARE,
        INTERNAL_CALIBRA_50F_50uAPP_SQUARE,
        INTERNAL_CALIBRA_50F_100uAPP_SQUARE,
        INTERNAL_CALIBRA_50F_200uAPP_SQUARE,
        INTERNAL_CALIBRA_60F_50uAPP_SQUARE,
        INTERNAL_CALIBRA_60F_100uAPP_SQUARE,
        INTERNAL_CALIBRA_60F_200uAPP_SQUARE
    } ;

    [StructLayout(LayoutKind.Sequential)]
    public class NSD2_DEV_STAT
    {
        public byte SetState;       //命令执行结果,0: 命令执行正确1: 命令正在执行过程中2：命令执行错误3: 命令在传输过程中超时
        public byte MacState;       //主机状态0: 待机1: 正在采集2: 主机未接主电源
        public byte DCJState;       //主电刺激器状态0：未开机或没有检测到主电流刺激器1：待机2：正在刺激
        public byte SCJState;       //音频刺激器状态0：未开机或没有检测到音频刺激器1：待机2：短音刺激3：编程刺激4：Oddball模式刺激
        public byte SPCJState;      //视频刺激器状态0：未开机或没有检测到视频刺激器1：待机2：模式翻转刺激3：随机模式刺激
        public byte OTHERState;     //其他刺激器状态
        public byte MacMCUvsn1;     //主控盒MCU固件的主版本号
        public byte MacMCUvsn2;     //主控盒MCU本固件的次版本号
        public byte MacFPGAvsn1;    //主控盒FPGA主版本号
        public byte MacFPGAvsn2;    //主控盒FPGA次版本号
        public byte PreAMPState;    //与前置放大器的通讯状态：0x00：接收；0x01正常
        public byte FLASHState;     //闪光刺激器状态
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 42)]
        public byte[] Reserves;   //预留字节 42
        public byte AnswerCode;     //前置应答命令码
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] AnswerData;  //前置应答数据 3
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] MacSerialNum;//主机序列号，为无符号长整形数据格式，低位在后。
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] MacVersion;  //主机版本信息，有2位小数的压缩BCD码，低字节在后。例如：0x200表示主机版本为2.00。
    }


    [StructLayout(LayoutKind.Sequential)]
    public class NSD2_AMP_STAT
    {
        public byte Freqency;      //前置放大器当前采样频率
        public byte FPGAvsn1;      //前置放大器FPGA主版本号
        public byte FPGAvsn2;      //前置放大器FPGA子版本号
        public byte CPUvsn1;       //前置放大器CPU主版本号
        public byte CPUvsn2;       //前置放大器CPU子版本号
        public byte Hardwarevsn1;  //前置放大器硬件主版本号
        public byte Hardwarevsn2;  //前置放大器硬件子版本号
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] SerialNum;  //前置放大器流水号，高字节在前，低字节在后
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] CellVotage; //电池电压，单位mv；为无符号长整形数据格式，低位在后。
        public byte CellState;     //电池状态，bit0: =1正在充电，bit1: =1外接电源接入
        public byte MaxConfig;     //前置最大配置
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] ChoiceState;//前置通道选择状态

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 31)]
        public byte[] Reserves;  //预留字节	无意义
    }
}
