using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using System.Diagnostics;
using BCILib.sp;
using System.Globalization;
using System.IO;
using BCILib.App;

namespace BCILib.Amp
{
    internal class ZeoEEG : LiveAmplifier
    {
        /// <summary>
        /// currently only used to check serial port number.
        /// </summary>
        static public string DllPath = "ftd2xx.dll";
        bool Interpolate = true;
        bool AddRevChan = true;
        bool LowFilter = true;
        bool DebugLog = false;

        Filter filter = null;
        ReaderWriterLock stimlock = new ReaderWriterLock();

        public ZeoEEG()
        {
            // low filter
            const string B = "3f9d8b57c832d8b6,3f8728aaf6b6f11e,3fb00abf3bc5a62e,3fa7e28ac90323e9,3fb29234bafd120d,3fa7e28ac9032411,3fb00abf3bc5a60c,3f8728aaf6b6f1ac,3f9d8b57c832d881";
            const string A = "3ff0000000000000,c00411ab7081798d,400e2485f0c0e7f2,c00b02ddc0144065,400127ae5d24ee3a,bfebfdec535f3f15,3fd02b82919fe0de,bfa32716416638d5,3f6f328d0f96c83e";
            filter = new Filter(
                B.Split(',').Select(x => BitConverter.ToDouble(BitConverter.GetBytes(long.Parse(x, NumberStyles.HexNumber)), 0)).ToArray(),
                A.Split(',').Select(x => BitConverter.ToDouble(BitConverter.GetBytes(long.Parse(x, NumberStyles.HexNumber)), 0)).ToArray());
        }

        internal enum ZeoEvent
        {
            NoEvent = 0x00,
            NightStart = 0x05,
            SleepOnset = 0x07,
            HeadbandDocked = 0x0E,
            HeadbandUnDocked = 0x0F,
            AlarmOff = 0x10,
            AlarmSnooze = 0x11,
            AlarmPlay = 0x13,
            NightEnd = 0x15,
            NewHeadband = 0x24,
        }

        public override string DevName
        {
            get
            {
                if (string.IsNullOrEmpty(port)) return base.DevName;
                else return base.DevName + ":" + port;
            }
        }

        SerialPort sport = null;
        byte[] buffer = null;

        byte[] cnt_buf = null;
        int sno = 0;
        int off = 0;

        Queue<DateTime> ts_que = new Queue<DateTime>();

        public override void SendStimCode(int evt)
        {
            stimlock.AcquireWriterLock(Timeout.Infinite);
            code_que.Enqueue((byte)evt);
            ts_que.Enqueue(DateTime.Now);
            stimlock.ReleaseWriterLock();
        }

        protected override void ReceiveDataLoop()
        {
            ts_que.Clear();

            buffer = new byte[512];
            sport = new SerialPort(port, 38400, Parity.None, 8, StopBits.One);
            sport.Open();
            sport.DiscardInBuffer();
            float fv_last = 0;

            int nread = 0;
            var message = new ZeoMessage();

            // for EEG data
            cnt_buf = new byte[header.BlkSize];
            sno = 0;
            off = 0;
            fv_last = 0;

            Stopwatch sw = Stopwatch.StartNew();

            // impedance check
            int zlow = 80;
            int zhigh = 1100;
            bool[] zlist = new bool[7]; // default to false;
            int zpos = 0;
            int nbad = 0;

            StreamWriter log_sw = null;
            long log_t0 = sw.ElapsedMilliseconds;

            if (DebugLog) {
                string fn_log = Path.Combine(BCIApplication.RootPath, "ZeoEEG_" + BCIApplication.TimeStamp + ".log");
                log_sw = File.CreateText(fn_log);
                log_sw.AutoFlush = true;
            }

            while (bRunning) {
                int rHeader = sport.ReadByte();
                if (rHeader == 'A') {
                    rHeader = sport.ReadByte();
                    if (rHeader == '4') {
                        // ReadA4Message
                        //•AncllLLTttsid

                        var rst = ReadA4Meesage(message);
                        if (rst == ZeoDataType.SliceEnd) {
                            if (message.Impedance.HasValue) {
                                zlist[zpos++] = (message.Impedance.Value >= zlow) && (message.Impedance.Value <= zhigh);
                                if (zpos >= zlist.Length) zpos = 0;
                                int noff = zlist.Where(x => !x).Count();
                                if (Status != AmpStatus.Connected && noff < 3) {
                                    Status = AmpStatus.Connected;
                                } else if (Status != AmpStatus.Checking && noff > 4) {
                                    Status = AmpStatus.Checking;
                                }
                            }

                            if (message.BadSignal.HasValue && message.BadSignal.Value) {
                                nbad++;
                            }

                            if (log_sw != null ) {
                                long t1 = sw.ElapsedMilliseconds;
                                if (t1 - log_t0 >= 60000) {
                                    log_sw.WriteLine("{0}: {1}", BCIApplication.TimeStamp, nbad);
                                    nbad = 0;
                                    log_t0 = t1;
                                }
                            }
                            
                            if (message.Waveform != null) {
                                Debug.WriteLine("ReadDataLoop: Read mesage: " + message);
                                int nspl = message.Waveform.Length;
                                var evts = new int[nspl];

                                DateTime tnow = DateTime.Now;

                                // get stim code
                                if (code_que.Count > 0) {
                                    stimlock.AcquireWriterLock(Timeout.Infinite);
                                    int tk = 0;
                                    while (code_que.Count > 0) {
                                        int code = code_que.Dequeue();
                                        DateTime ts = ts_que.Dequeue();
                                        int tp = nspl - (int) ((tnow - ts).TotalMilliseconds * 128 / 1000) - 1;
                                        if (tp < tk) {
                                            tp = tk;
                                        }
                                        evts[tp] = code;
                                        tk = tp + 2;
                                        if (tk >= nspl) {
                                            break;
                                        }
                                    }
                                    stimlock.ReleaseWriterLock();
                                }

                                for (int si = 0; si < nspl; si++) {
                                    float fv = message.Waveform[si];
                                    if (Interpolate) AddSample((fv + fv_last) / 2, evts[si]);
                                    AddSample(fv, evts[si]);
                                    fv_last = fv;
                                }
                            }
                            nread++;
                            message = new ZeoMessage();
                        } else if (rst == ZeoDataType.Error) {
                            message = new ZeoMessage();
                        } else if (message.Event == ZeoEvent.HeadbandDocked) {
                            bRunning = false;
                        }
                    }
                } else if (rHeader == 'Z') {
                    if (rHeader == '9') {
                        //ReadZ9 Message
                        ReadZ9Message();
                    }
                }
            }

            if (log_sw != null) log_sw.Close();

            sport.Close();
        }

        private void AddSample(float v, int evt)
        {
            BitConverter.GetBytes(v).CopyTo(cnt_buf, off);
            off += 4;

            if (AddRevChan) {
                var nv = -v;
                BitConverter.GetBytes(nv).CopyTo(cnt_buf, off);
                off += 4;
            }

            BitConverter.GetBytes(evt).CopyTo(cnt_buf, off);
            off += 4;

            sno++;
            if (sno >= header.blk_samples) {
                PoolAddBuffer(cnt_buf);
                sno = 0;
                off = 0;
            }
        }

        private void ReadNBytes(int count)
        {
            if (sport != null && sport.IsOpen) {
                int n = 0;
                while (n < count) {
                    n += sport.Read(buffer, n, count - n);
                }
            }
        }

        internal enum ZeoDataType
        {
            Event = 0x00,
            SliceEnd = 0x02,
            Version = 0x03,
            Waveform = 0x80,
            FrequencyBins = 0x83,
            SQI = 0x84,
            ZeoTimestamp = 0x8A,
            Impedance = 0x97,
            BadSignal = 0x9C,
            SleepStage = 0x9D,
            Error = 0xFF,
        }
        internal enum ZeoSleepStage
        {
            Undefined0 = 0,
            Awake = 1,
            REM = 2,
            Light = 3,
            Deep = 4,
            Undefined = 5,
            Sleep = 6,  // REM || Light || Deep
        }
        internal class ZeoMessage
        {
            public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1);

            public static readonly int SamplesPerMessage = 128;

            public static readonly int FrequencyBinsLength = 7;

            public static readonly int MinSoundVolume = -10000;

            public ZeoMessage()
            {
                this.SoundAlarmVolume = ZeoMessage.MinSoundVolume;
            }

            public float[] Waveform { get; set; }

            public float? Impedance { get; set; }

            public ZeoEvent? Event { get; set; }

            public int ZeoTimestamp { get; set; }

            public bool? BadSignal { get; set; }

            public int? SQI { get; set; }

            public float[] FrequencyBins { get; set; }

            public ZeoSleepStage? SleepStage { get; set; }

            public float Second { get; set; }

            public int SoundAlarmVolume { get; set; }

            public override string ToString()
            {
                StringBuilder stringBuilder = new StringBuilder();
                if (this.Waveform != null) {
                    stringBuilder.AppendFormat("Waveform; Second: {0:.00000}\r\n", this.Second);
                }

                if (this.Impedance != null) {
                    stringBuilder.AppendFormat("Impedance: {0:0.000}\r\n", this.Impedance);
                }

                if (this.Event != null) {
                    stringBuilder.AppendFormat("Event: {0}\r\n", Enum.GetName(typeof(ZeoEvent), this.Event));
                }

                stringBuilder.AppendFormat("ZeoTimestamp: {0}\r\n", UnixEpoch.AddSeconds(this.ZeoTimestamp));

                if (this.BadSignal != null) {
                    stringBuilder.AppendFormat("BadSignal: {0}\r\n", this.BadSignal);
                }

                if (this.SQI != null) {
                    stringBuilder.AppendFormat("SQI: {0}\r\n", this.SQI);
                }

                if (this.FrequencyBins != null) {
                    stringBuilder.Append("FrequencyBins: ");
                    for (int i = 0; i < this.FrequencyBins.Length; i++) {
                        stringBuilder.AppendFormat("{0:0.00} ", this.FrequencyBins[i]);
                    }

                    stringBuilder.AppendLine();
                }

                if (this.SleepStage != null) {
                    stringBuilder.AppendFormat("SleepStage: {0}", Enum.GetName(typeof(ZeoSleepStage), this.SleepStage));
                }

                return stringBuilder.ToString().Trim();
            }
        }

        private int GetInt32()
        {
            return BitConverter.ToInt32(buffer, 0);
        }
        private float GetImpedance()
        {
            ushort[] shorts = new ushort[2] {
                BitConverter.ToUInt16(buffer, 0),
                BitConverter.ToUInt16(buffer, 2),
            };

            shorts[0] -= 0x8000;
            shorts[1] -= 0x8000;
            if (shorts[0] != 0x7fff) {
                float imp = (float)Math.Sqrt((shorts[0] * shorts[0]) + (shorts[1] * shorts[1]));
                return float.IsNaN(imp) == false ? imp : 0;
            } else {
                return 0;
            }
        }

        private ZeoDataType ReadA4Meesage(ZeoMessage zeoMessage)
        {
            //[An]cllLLTttsid
            int checkSum = sport.ReadByte();

            ReadNBytes(2);
            int length = this.buffer[0] | this.buffer[1] << 8;

            ReadNBytes(2);
            int lengthN = this.buffer[0] | this.buffer[1] << 8;

            if ((short)length != (short)(~lengthN)) {
                // TODO: log error
                return ZeoDataType.Error;
            }

            int unixT = sport.ReadByte();

            this.ReadNBytes(2);
            float subsecondT = (this.buffer[0] | this.buffer[1] << 8) / 65535.0f;

            int sequence = sport.ReadByte();

            ZeoDataType dataType = (ZeoDataType)sport.ReadByte();

            this.ReadNBytes(length - 1);
            //Debug.WriteLine(string.Format("Read {0}: length={1}T{2}tt{3:0.###}S{4}", dataType, length, unixT, subsecondT, sequence));

            switch (dataType) {
                case ZeoDataType.FrequencyBins:
                    short[] shorts = new short[7];
                    Buffer.BlockCopy(this.buffer, 0, shorts, 0, 14);
                    float[] floats = new float[7];
                    for (int k = 0; k < 7; k++) {
                        floats[k] = (shorts[k] / 32767.0f) * 100.0f;
                    }
                    zeoMessage.FrequencyBins = floats;
                    break;

                case ZeoDataType.Waveform:
                    shorts = new short[128];
                    Buffer.BlockCopy(this.buffer, 0, shorts, 0, 256);

                    floats = new float[128];
                    for (int k = 0; k < 128; k++) {
                        floats[k] = (shorts[k] / 32767.0f) * 315.0f;
                    }

                    zeoMessage.Waveform = floats;
                    zeoMessage.Second = unixT + subsecondT;

                    // 128 16-bit samples per ~1.005 sec
                    break;

                case ZeoDataType.ZeoTimestamp:
                    zeoMessage.ZeoTimestamp = this.GetInt32();
                    break;

                case ZeoDataType.Event:
                    zeoMessage.Event = (ZeoEvent)this.GetInt32();
                    break;

                case ZeoDataType.Version:
                    int version = BitConverter.ToInt32(buffer, 1);
                    Debug.WriteLine("version=" + version.ToString("X"));
                    break;

                case ZeoDataType.SQI:
                    zeoMessage.SQI = this.GetInt32();
                    break;

                case ZeoDataType.BadSignal:
                    zeoMessage.BadSignal = this.GetInt32() == 0 ? false : true;
                    break;

                case ZeoDataType.Impedance:
                    zeoMessage.Impedance = this.GetImpedance();
                    break;

                case ZeoDataType.SleepStage:
                    zeoMessage.SleepStage = (ZeoSleepStage)this.GetInt32();
                    break;

                case ZeoDataType.SliceEnd:
                    break;

                default:
                    return ZeoDataType.Error;
            }

            return dataType;
        }

        internal enum Z9DataType
        {
            SoundAlarmVolume = 0x00,
            SoundAlarmEnabled = 0x01,
        }

        private void ReadZ9Message()
        {
            this.ReadNBytes(2);
            int length = this.buffer[0] | this.buffer[1] << 8;

            // ZeoTimestamp is used as an index to update ZeoMessage in the list
            this.ReadNBytes(4);
            uint zeoTimestamp = (uint)this.GetInt32();
            //ZeoMessage zeoMessage = this.GetZeoMessage(zeoTimestamp);

            Z9DataType dataType = (Z9DataType)sport.ReadByte();

            this.ReadNBytes(length);

            switch (dataType) {
                case Z9DataType.SoundAlarmVolume:
                    //zeoMessage.SoundAlarmVolume = this.GetInt32();
                    break;
                case Z9DataType.SoundAlarmEnabled:
                    //this.SoundAlarmEnabled = this.buffer[0] != 0;
                    break;
            }
        }

        public override string GetChannelNameString()
        {
            // one channel only
            if (AddRevChan) return "Ch1,Ch2";
            else return "Ch1";
        }

        string port = null;

        public override bool Initialize()
        {
            port = Ftdi.GetComPortList().FirstOrDefault();
            if (string.IsNullOrEmpty(port)) {
                return false;
            }

            header.nchan = AddRevChan? 2:1;
            header.samplingrate = Interpolate? 256:128;
            header.blk_samples = 16; // for lowpass display, it must be multiplier of 4 if Interpolate and AddRevChan are true.
            header.datasize = 4;
            header.resolution = 0;
            header.nevt = 1;

            return true;
        }

        public override bool Configure()
        {
            ZeoConfigForm form = new ZeoConfigForm();
            form.Interpolate = Interpolate;
            form.RevChannel = AddRevChan;
            form.LowFilter = LowFilter;
            form.LogDebug = DebugLog;

            if (form.ShowDialog() == DialogResult.OK) {
                Interpolate = form.Interpolate;
                AddRevChan = form.RevChannel;
                LowFilter = form.LowFilter;
                DebugLog = form.LogDebug;
                return true;
            }
            return false;
        }

        enum ZeoCfg {
            Interpolate = 1,
            AddRevChan = 2,
            LowPassFilter = 3,
        }
        protected override void SaveConfigSpecial(BCILib.Util.ResManager rm)
        {
            rm.SetConfigValue(ID, ZeoCfg.AddRevChan.ToString(), AddRevChan.ToString());
            rm.SetConfigValue(ID, ZeoCfg.Interpolate.ToString(), Interpolate.ToString());
            rm.SetConfigValue(ID, ZeoCfg.LowPassFilter.ToString(), LowFilter.ToString());
        }

        protected override void SetSpecialConfig(BCILib.Util.ResManager rm)
        {
            rm.GetConfigValue(ID, ZeoCfg.Interpolate.ToString(), ref Interpolate);
            rm.GetConfigValue(ID, ZeoCfg.AddRevChan.ToString(), ref AddRevChan);
            rm.GetConfigValue(ID, ZeoCfg.LowPassFilter.ToString(), ref LowFilter);
        }

        float pv = 0;
        protected override void ProcessForDisplay(float[] pbuf)
        {
            if (!LowFilter) return;

            int si = 0;
            while (si < pbuf.Length) {
                int ri = si;
                float fv = (float) filter.Process(pbuf[ri]);
                if (!Interpolate) fv = (float) filter.Process(pbuf[ri]);
                pbuf[si++] = fv;
                if (AddRevChan) pbuf[si++] = -fv;
                pv = fv;
            }
        }
    }
}
