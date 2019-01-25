using System;
using System.Collections.Generic;
using System.IO;

using System.Reflection;

using BCILib.Util;
using System.Threading;
using BCILib.App;
using System.Linq;

using System.Configuration;
using System.Text;

namespace BCILib.Amp
{
    public abstract class Amplifier
    {
        protected StimMethod stim_method = StimMethod.Software;

        public static readonly string DefaultPackage = "BCILib.Amp.";

        public static string LogFileName
        {
            get
            {
                return Path.Combine(BCIApplication.RootPath, "Amplifier.log");
            }
        }

        public static void LogMessage(string fmt, params object[] args)
        {
            using (var fs = File.AppendText(LogFileName)) {
                fs.Write(DateTime.Now.ToString("s") + ":");
                fs.WriteLine(fmt, args);
            }
        }

        public AmplifierHeader header;
        private string id = null;
        public string ID
        {
            get
            {
                if (id == null) {
                    // create a unique amplifer
                    id = GetNewID(BCILib.App.BCIApplication.AppResource);
                }
                return id;
            }

            protected set
            {
                id = value;
            }
        }

        public enum CfgProp
        {
            InstalledDevices,
            Type,
            ChannelNames,
            Delay_SoftwareStim,
            AssemblyLocation
        }

        public void SaveConfig(ResManager rm)
        {
            if (id == null) {
                id = GetNewID(rm);
            }

            //string installed = rm.GetConfigValue(CfgProp.InstalledDevices.ToString());
            //if (string.IsNullOrEmpty(installed) || installed.IndexOf(ID) < 0) {
            //    if (!string.IsNullOrEmpty(installed)) installed = installed + ",";
            //    rm.SetConfigValue(CfgProp.InstalledDevices.ToString(), installed + ID);
            //}

            Type type = this.GetType();
            rm.SetConfigValue(ID, CfgProp.Type, GetTypeName(type));

            Assembly type_asm = typeof(Amplifier).Assembly;
            Type stype = type_asm.GetType(type.FullName);
            if (stype == null || !stype.IsSubclassOf(typeof(Amplifier))) {
                string location = BCIApplication.GetRelativePath(type.Assembly.Location, BCIApplication.RootPath);
                rm.SetConfigValue(ID, CfgProp.AssemblyLocation, location);
            }

            rm.SetConfigValue(ID, CfgProp.ChannelNames, GetChannelNameString());
            if (ms_software_stim_delay != 0) {
                rm.SetConfigValue(ID, CfgProp.Delay_SoftwareStim, ms_software_stim_delay.ToString());
            }

            SaveConfigSpecial(rm);

            rm.SaveFile();
        }

        private string GetNewID(ResManager rm)
        {
            string id = null;

            int amp_no = 0;
            rm.GetConfigValue("AmpliferNo", ref amp_no);
            amp_no++;

            string[] rn_list = rm.GetAllResource();

            while (true) {
                id = string.Format("{0}_{1}", TypeName, amp_no);
                if (Array.IndexOf(rn_list, id) < 0) break;
                amp_no++;
            }

            rm.SetConfigValue("AmpliferNo", amp_no.ToString());

            return id;
        }

        protected virtual void SaveConfigSpecial(ResManager rm)
        {
        }

        public abstract string GetChannelNameString();

        public virtual string[] ChannelNames
        {
            get
            {
                return GetChannelNameString().Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        public abstract bool Initialize();
        //{
        //    return true;
        //}

        public bool Start()
        {
            if (IsAlive) return true;

            // Because of continued saving data file, the following two parameters cannot be initialized. 
            // The reason is they are used to calculated file position of current sample.
            // pool_cpos = pool_rn = 0;

            code_que.Clear();
            evt_que.Clear();
            queue_data.Clear();
            queue_evt.Clear();

            time_started = -1;
            act_splrate = 0;

            return StartRead();
        }

        public void Stop()
        {
            StopRead();
            // 20140214: maybe reconfigured!
            pool = null;
        }

        protected abstract bool StartRead();
        protected abstract void StopRead();
        public abstract bool IsAlive
        {
            get;
        }

        public event Action<float[], int[]> evt_data_received = null;

        /// <summary>
        /// only used in FileSimulator now
        /// </summary>
        protected void TriggerDateRecv()
        {
            if (evt_data_received != null && !IsEvtUpdating) {
                evt_data_received(null, null);
            }
        }

        public static string GetTypeName(Type type)
        {
            string tn = type.ToString();
            if (tn.StartsWith(DefaultPackage)) tn = tn.Substring(DefaultPackage.Length);
            return tn;
        }

        public virtual string TypeName
        {
            get
            {
                return GetTypeName(this.GetType());
            }
        }

        public virtual string DevName
        {
            get
            {
                return TypeName;
            }
        }

        public struct AmplifierHeader
        {
            public int nchan;		// Number of EEG channels
            public int nevt;		// Number of event channels
            public int blk_samples;		// Samples in block
            public int samplingrate;	// Sampling rate (in Hz)
            public int datasize;		// 2 for "short", 4 for "int" type of data
            public float resolution;	// Resolution for LSB

            public override string ToString()
            {
                return string.Format("C{0}/{1}, B{2}, S{3}, R{4}",
                    nchan, nevt, blk_samples, samplingrate, resolution);
            }

            public int SampleSize
            {
                get
                {
                    return (nchan + nevt) * datasize;
                }
            }

            public int BlkSize
            {
                get
                {
                    return blk_samples * SampleSize;
                }
            }
        }

        /// <summary>
        /// Amplifier configuration
        /// </summary>
        /// <param name="Existing">true if configure existing amplifer.</param>
        /// <returns>true: sucessful</returns>
        public abstract bool Configure();

        public static Amplifier FromConfigure(ResManager rm, string id)
        {
            string amp_name = rm.GetConfigValue(id, CfgProp.Type.ToString());
            string asm_path = rm.GetConfigValue(id, CfgProp.AssemblyLocation.ToString());

            if (string.IsNullOrEmpty(amp_name)) return null;

            Assembly asm = null;
            Type amp_type = null;

            if (!string.IsNullOrEmpty(asm_path)) {
                asm_path = Path.Combine(BCIApplication.RootPath, asm_path);
                if (File.Exists(asm_path)) {
                    asm = Assembly.LoadFile(asm_path);
                    amp_type = asm.GetType(amp_name);
                    if (amp_type == null) amp_type = asm.GetType(DefaultPackage + amp_name);
                }
            }

            if (amp_type == null) {
                asm = typeof(Amplifier).Assembly;
                amp_type = asm.GetType(amp_name);
                if (amp_type == null) {
                    amp_type = asm.GetType(DefaultPackage + amp_name);

                }
            }

            if (amp_type == null || !amp_type.IsSubclassOf(typeof(Amplifier))) return null;

            Amplifier amp = (Amplifier)amp_type.GetConstructor(new Type[] { }).Invoke(null);
            amp.SetConfig(rm, id);

            return amp;
        }

        protected virtual void SetSpecialConfig(ResManager rm)
        {
            // default: nothing special
        }

        protected void SetConfig(ResManager rm, string id)
        {
            this.ID = id;

            string tmp = rm.GetConfigValue(ID, "Delay_SoftwareStim");
            if (!string.IsNullOrEmpty(tmp)) {
                int.TryParse(tmp, out ms_software_stim_delay);
            }

            SetSpecialConfig(rm);
        }

        // software stim queue
        protected Queue<SentStim> code_que = new Queue<SentStim>();

        // parallel port stim queue
        protected Queue<byte> evt_que = new Queue<byte>();

        protected int ms_software_stim_delay = 0;

        long time_started = -1;
        int sno_started = -1;
        long time_update = -1;
        int sno_update = -1;
        double act_splrate = 0;

        public virtual void SendStimCode(int evt)
        {
            if ((stim_method & StimMethod.Software) == StimMethod.Software) {
                if (evt != 0) {
                    int s0 = ms_software_stim_delay * header.samplingrate / 1000;
                    if (act_splrate > 0) {
                        //                    lock (code_que) {
                        var asno = (BCIApplication.ElaspedLongMilliSeconds - time_update) * header.samplingrate / 1000.0;
                        code_que.Enqueue(new SentStim(evt, sno_update + s0 + (int)(asno + 0.5)));
                        //                    }
                    } else {
                        code_que.Enqueue(new SentStim(evt, pool_rn + header.blk_samples + s0));
                    }
                }
            }

            if ((stim_method & StimMethod.ParallePort) == StimMethod.ParallePort) {
                ParallelPort.Out(evt);
                if (evt != 0) {
                    evt_que.Enqueue((byte)evt);

                    if (CheckEventTimeout) {
                        if (event_ticks == 0) event_ticks = BCIApplication.ElaspedMilliSeconds;
                    }
                }
            }

            if ((stim_method & StimMethod.USBMOD4) == StimMethod.USBMOD4) {
                USBMOD4.SendCode(evt);
                if (evt != 0) {
                    //evt_que.Enqueue((byte)evt);
                    //if (CheckEventTimeout) {
                    //    if (event_ticks == 0) event_ticks = BCIApplication.ElaspedMilliSeconds;
                    //}
                }
            }
        }

        protected void UpdateActSplRate()
        {
            //lock (code_que) {
                sno_update = pool_rn;
                time_update = BCIApplication.ElaspedLongMilliSeconds;
            //}

            if (time_started < 0) {
                time_started = time_update;
                sno_started = -1;
                act_splrate = 0;
                return;
            }
            
            var dt = time_update - time_started;

            if (sno_started < 0) {
                if (dt < 5000) return;
                time_started = time_update;
                sno_started = sno_update;
                return;
            }

            act_splrate = (sno_update - sno_started) * 1000.0 / dt;
        }

        public int ActSampleRate
        {
            get
            {
                return (int)Math.Round(act_splrate);
            }
        }

        #region Event_Ticks
        // for hardware sending stim events. if within 1 second event not received,
        // error should be detected.

        protected int event_ticks = 0;

        // call before receive data and whenever evt is received.
        protected void ResetEventTicks()
        {
            event_ticks = (evt_que.Count == 0) ? 0 : BCIApplication.ElaspedMilliSeconds;
        }

        // call within receive data loop to deteck timeout
        protected bool RecvEventTimeout()
        {
            return (check_event_tick && event_ticks > 0 && (BCIApplication.ElaspedMilliSeconds - event_ticks) > 1000);
        }

        private bool check_event_tick = false;

        public bool CheckEventTimeout
        {
            get
            {
                return check_event_tick;
            }
            set
            {
                check_event_tick = value;
            }
        }

        #endregion

        // By default set buffer size to hold EEG data for 30 seconds.
        private int pool_seconds = 30;
        protected int pool_size = 0;
        private float[] pool = null;
        /// <summary>
        /// Next writing pool position
        /// </summary>
        private int pool_cpos = 0;

        /// <summary>
        /// Total number of samples currently read
        /// </summary>
        private int pool_rn = 0;

        protected void PoolInitialize()
        {
            pool_size = header.samplingrate * pool_seconds;

            // make sure pool_size is devidable by head.blk_samples
            pool_size += header.blk_samples - 1;
            pool_size /= header.blk_samples;
            pool_size *= header.blk_samples;

            pool = new float[pool_size * header.nchan];
            pool_rn = 0;
            pool_cpos = 0;
        }

        //private Queue<int[]> evt_rque = new Queue<int[]>();

        private int last_evt = 0;

        public event Action<int, int> evt_stim_received = null;

        // data and event for update gui, for FileSimulator only now.
        private Queue<float[]> queue_data = new Queue<float[]>();
        private Queue<int[]> queue_evt = new Queue<int[]>();
        private bool IsEvtUpdating = false;

        protected void PoolAddBuffer(byte[] buf)
        {
            if (buf.Length != header.BlkSize) {
                Console.WriteLine("xxxx");
            }

            if (isRecording && recWriter != null) {
                if (_recStartSpl < 0) _recStartSpl = pool_rn;
                recWriter.Write(buf);
            }

            if (pool == null) PoolInitialize();

            int off = 0;
            int k1 = pool_cpos * header.nchan;
            int[] evtlist = new int[header.blk_samples];
            if (header.resolution != 0) {
                for (int si = 0; si < header.blk_samples; si++) {
                    for (int ci = 0; ci < header.nchan; ci++, k1++, off += 4) {
                        pool[k1] = BitConverter.ToInt32(buf, off) * header.resolution;
                    }

                    int evt = BitConverter.ToInt32(buf, off) & 0xff;
                    off += 4;
                    evtlist[si] = 0;
                    if (evt != last_evt && evt > 0) {
                        //evt_rque.Enqueue(new int[] { evt, pool_cpos, si });
                        evtlist[si] = evt;
                        if (evt_stim_received != null) evt_stim_received(evt, si + pool_rn);
                    }
                    last_evt = evt;
                }
            } else {
                for (int si = 0; si < header.blk_samples; si++) {
                    for (int ci = 0; ci < header.nchan; ci++, k1++, off += 4) {
                        pool[k1] = BitConverter.ToSingle(buf, off);
                    }

                    int evt = BitConverter.ToInt32(buf, off) & 0xff;
                    off += 4;

                    evtlist[si] = 0;
                    if (evt != last_evt && evt > 0) {
                        //evt_rque.Enqueue(new int[] { evt, pool_rblks, si });
                        evtlist[si] = evt;
                        if (evt_stim_received != null) evt_stim_received(evt, si + pool_rn);
                    }
                    last_evt = evt;
                }
            }

            if (evt_data_received != null) {
                int blksz = header.nchan * header.blk_samples;
                float[] pbuf = new float[blksz];
                Array.Copy(pool, k1 - blksz, pbuf, 0, blksz);

                ProcessForDisplay(pbuf);
                evt_data_received(pbuf, evtlist);

                //lock (queue_data)
                //{
                //    queue_data.Enqueue(pbuf);
                //    queue_evt.Enqueue(evtlist);
                //}

                //if (!IsEvtUpdating)
                //{
                //    IsEvtUpdating = true;
                //    ThreadPool.QueueUserWorkItem(x =>
                //    {
                //        while (true)
                //        {
                //            if (queue_evt.Count == 0) break;
                //            float[] fd = null;
                //            int[] el = null;

                //            lock (queue_data)
                //            {
                //                fd = queue_data.Dequeue();
                //                el = queue_evt.Dequeue();
                //            }
                //            evt_data_received(fd, el);
                //        }
                //        IsEvtUpdating = false;
                //    });
                //}
            }

            pool_cpos += header.blk_samples;
            pool_cpos %= pool_size;
            pool_rn += header.blk_samples;
        }

        protected virtual void ProcessForDisplay(float[] pbuf)
        {
        }

        private BinaryWriter recWriter = null;
        private bool isRecording = false;
        private long _recStartSpl = 0;
        private long _lastReadSpl = 0;

        public bool IsRecording
        {
            get
            {
                return isRecording;
            }
        }
        public void StartRecord(string fn)
        {
            StopRecord();

            isRecording = false;

            recWriter = new BinaryWriter(
                new FileStream(fn, FileMode.Create, FileAccess.Write));
            recWriter.Write(header.nchan);
            recWriter.Write(header.nevt);
            recWriter.Write(header.blk_samples);
            recWriter.Write(header.samplingrate);
            recWriter.Write(header.datasize);
            recWriter.Write(header.resolution);

            //WriteChannelNames();

            recWriter.Flush();

            _recStartSpl = -1;
            isRecording = true;
            if (IsAlive && !(this is FileSimulator)) {
                int pos = Rd_GetPos();
                float[] buf = new float[header.BlkSize];
                while (_recStartSpl < 0) {
                    Thread.Sleep(10);
                    pos += Rd_GetBuf(buf, pos, header.blk_samples);
                }
            }

            return;
        }

        private void WriteChannelNames()
        {
            // ccwang: 20120105 file version 1.1 channel names
            int spz = header.nchan; // sample size

            // magic word
            string magic = "I2REEGCNT";
            string chanstr = GetChannelNameString();
            int nl = (magic.Length + 3 + chanstr.Length + spz - 1) / spz * spz;
            int n1 = 0;
            while (n1 < magic.Length) {
                if (header.resolution == 0) recWriter.Write((float)magic[n1]);
                else recWriter.Write((int)magic[n1]);
                n1++;
                if (n1 % spz == 0) recWriter.Write(0);
            }

            int[] vl = { 1, 1, chanstr.Length }; // version major, minor, channnel name string length
            for (int i = 0; i < vl.Length; i++) {
                if (header.resolution == 0) {
                    recWriter.Write((float)vl[i]);
                } else {
                    recWriter.Write((int)vl[i]);
                }
                n1++;
                if (n1 % spz == 0) recWriter.Write(0);
            }

            for (int i = 0; i < chanstr.Length; i++) {
                if (header.resolution == 0) recWriter.Write((float)chanstr[i]);
                else recWriter.Write((int)chanstr[i]);
                n1++;
                if (n1 % spz == 0) recWriter.Write(0);
            }

            while (n1 < nl) {
                if (header.resolution == 0) recWriter.Write((float)0);
                else recWriter.Write((int)0);
                n1++;
                if (n1 % spz == 0) recWriter.Write(0);
            }
            // end of version and channel names
        }
        public void StopRecord()
        {
            isRecording = false;
            System.Threading.Thread.Sleep(20);

            if (recWriter != null) {
                recWriter.Close();
                recWriter = null;
            }
        }

        /// <summary>
        /// Return next reading position
        /// </summary>
        /// <param name="nspl">number of samples for next reading</param>
        /// <returns></returns>
        public int Rd_SetPos(int nspl)
        {
            int newpos = 0;
            if (pool_size > 0) {
                newpos = pool_rn + nspl;
                if (newpos < 0) newpos = 0;
                if (newpos < pool_rn - pool_size) {
                    newpos = pool_rn - pool_size;
                }

                if (isRecording) {
                    if (newpos < _recStartSpl) newpos = (int)_recStartSpl;
                }
            }
            return newpos;
        }

        public int Rd_GetPos()
        {
            return pool_rn;
        }

        /// <summary>
        /// Read data from specified position
        /// </summary>
        /// <param name="fbuf"></param>
        /// reading buffer
        /// <param name="nsamples"></param>
        /// number of samples to read
        /// <param name="ishift"></param>
        /// number of samples to move the reading position forward after current reading
        /// <returns>Number of samples read.</returns>
        public int Rd_GetBuf(float[] fbuf, int rpos, int nsamples)
        {
            return Rd_GetBuf(fbuf, ref rpos, nsamples, false);
        }

        /// <summary>
        /// Read data from specified position
        /// </summary>
        /// <param name="fbuf"></param>
        /// reading buffer
        /// <param name="nsamples"></param>
        /// number of samples to read
        /// <param name="ishift"></param>
        /// number of samples to move the reading position forward after current reading
        /// <param name="takeLatest"></param>
        /// true if take the latest data if posible
        /// <returns>Number of samples read.</returns>
        public virtual int Rd_GetBuf(float[] fbuf, ref int rpos, int nsamples, bool takeLatest)
        {
            if (rpos + nsamples >= pool_rn) {
                // data not ready
                return 0;
            }

            if (takeLatest) {
                rpos = pool_rn - nsamples;
            }

            if (rpos < 0 || rpos < (pool_rn - pool_size)) {
                //warning
                LogMessage("Data (blkno {0}) not valid(overwitten)! (pool starting blk no = {1})", rpos, pool_rn - pool_size);
            }

            // copy data from pool
            if (nsamples > pool_size) {
                LogMessage("Read data ({0}) larger than pool size! ({1}), adjusted!", nsamples, pool_size);
                nsamples = pool_size;
            }

            int istart = rpos % pool_size;
            int copy_sz = pool_size - istart;
            if (copy_sz > nsamples) copy_sz = nsamples;
            Array.Copy(pool, istart * header.nchan, fbuf, 0, copy_sz * header.nchan);

            nsamples -= copy_sz;
            if (nsamples > 0) {
                Array.Copy(pool, 0, fbuf, copy_sz * header.nchan, nsamples * header.nchan);
            }

            _lastReadSpl = rpos;

            return (nsamples + copy_sz);
        }

        public int ToFilePos(int rdpos)
        {
            if (_recStartSpl > 0) {
                return ((int)(rdpos - _recStartSpl));
            } else {
                return -1;
            }
        }

        /// <summary>
        /// Last read starting sample in the recorded EEG file
        /// </summary>
        public long LastReadSplInFile
        {
            get
            {
                return (_lastReadSpl - _recStartSpl);
            }
        }

        protected enum ErrorType
        {
            NoError,
            EventTimeout,
            EventDiff,
        }

        protected ErrorType error_type = ErrorType.NoError;

        private AmpStatus _status = AmpStatus.Off;
        public AmpStatus Status
        {
            get
            {
                return _status;
            }

            protected set
            {
                _status = value;
                if (StatusChanged != null) StatusChanged(this);
            }
        }

        public static event Action<Amplifier> StatusChanged;

        public virtual bool IsRealAmplifier()
        {
            return false;
        }
    }

    [Flags]
    public enum StimMethod
    {
        None = 0,
        Software = 1,
        ParallePort = 2,
        USBMOD4 = 4,
    }

    public enum AmpStatus
    {
        Connected,
        Checking,
        Off
    }

    public class SentStim
    {
        /// <summary>
        /// stim code to send
        /// </summary>

        public int code;
        /// <summary>
        /// simple position
        /// </summary>
        public int splno;

        public SentStim(int scode, int pos)
        {
            code = scode;
            splno = pos;
        }

        public SentStim(int scode)
            : this(scode, -1)
        {
        }
    }
}
