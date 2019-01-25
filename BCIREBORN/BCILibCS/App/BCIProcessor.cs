using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Threading;

using BCILib.Amp;
using BCILib.Util;

namespace BCILib.App
{
    /// <summary>
    /// General BCI Processor - encapsulate all kinds of processors
    /// </summary>
    public class BCIProcessor : IDisposable
    {
        protected BCIEngine proc_engine = null;

        public BCIProcessor(BCIEngine.BCIProcType proc_type)
        {
            proc_engine = BCIEngine.CreateEngine(proc_type);
        }

        public BCIProcessor(BCIEngine engine)
        {
            proc_engine = engine;
        }

        public static BCIProcessor CrtProcessor(string procname)
        {
            BCIEngine.BCIProcType proc;
            try {
                proc = (BCIEngine.BCIProcType)Enum.Parse(typeof(BCIEngine.BCIProcType), procname);
            }
            catch (Exception) {
                MessageBox.Show("Invalive BCIProcess Name: " + procname);
                return null;
            }

            return new BCIProcessor(proc);
        }

        protected int[] chsel = null;

        private int _nspl = 0;
        public int NumSampleUsed
        {
            get
            {
                if (_nspl == 0) {
                    if (proc_engine != null) _nspl = proc_engine.NumSampleUsed;
                }
                return _nspl;
            }

            set
            {
                _nspl = value;
            }
        }

        private int _nch = 0;
        public int NumChannelUsed
        {
            get
            {
                if (_nch == 0) {
                    if (proc_engine != null) _nch = proc_engine.NumChannelUsed;
                }
                return _nch;
            }

            set
            {
                _nch = value;
            }
        }

        private bool SetSelChannels(string[] all, string[] sel)
        {
            chsel = new int[sel.Length];
            for (int i = 0; i < sel.Length; i++) {
                chsel[i] = Array.IndexOf(all, sel[i]);
                if (chsel[i] == -1) chsel[i] = i;
            }
            Array.Sort(chsel);
            NumChannelUsed = chsel.Length;
            return true;
        }

        private bool SetSelChannels(string[] all, string selstr)
        {
            return SetSelChannels(all, selstr.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries));
        }

        protected Amplifier _amp = null;

        public Amplifier Amplifier
        {
            get
            {
                if (_amp == null) {
                    if (AmpContainer.Count > 0) _amp = AmpContainer.GetAmplifier(0);
                }
                return _amp;
            }
        }

        public virtual bool SetAmplifier(Amplifier amp, string sel_channels)
        {
            StopEventProcessing(); // stop if have

            if (amp == null) return false;

            this._amp = amp;

            string[] chall = amp.ChannelNames;
            if (!SetSelChannels(chall, sel_channels)) return false;

            return InitBuffer();
        }

        private bool InitBuffer()
        {
            if (_amp == null) return false;

            int n = NumSampleUsed;
            if (n <= 0) return false;

            rd_buf = new float[_amp.header.nchan * n];
            pc_buf = new float[chsel.Length * n];

            return true;
        }

        protected int _rd_event = 0;
        protected int _rd_pos = 0;
        protected int _rd_shift_ms = 0;

        /// <summary>
        /// set shifting time in millisecond
        /// </summary>
        /// <param name="shift">millisecond</param>
        public virtual void SetReadingShift(int shift)
        {
            _rd_shift_ms = shift;
            //SetReadingPos(-NumSampleUsed);
        }

        /// <summary>
        /// Set amplifer reading position so that process
        /// can be started as soon as possible
        /// </summary>
        public int SetReadingPos()
        {
            return SetReadingPos(-NumSampleUsed);
        }

        /// <summary>
        /// Set amplifier reading position relative to current position
        /// </summary>
        /// <param name="rpos">offset relative to current position
        /// </param>
        public virtual int SetReadingPos(int rpos)
        {
            return _rd_pos = Amplifier.Rd_SetPos(rpos);
        }

        
        public virtual bool Process(out int filePos)
        {
            bool rst = false;
            if (_rd_shift_ms > 0) {
                rst = ProcessShiftBuffer(out filePos);
            }
            else {
                rst = ProcessEventBuffer(out filePos);
            }
            return rst;
        }

        protected Action<int, int> dlg_recv_evt = null;
        protected Action<float[], int[]> dlg_recv_dat = null;

        public void SetReadingCodes(int offset, params int[] codes)
        {
            Console.WriteLine("BCIProc: stim offset = {0}", offset);

            _event_offset = offset;

            _proc_codes = null;
            if (codes.Length > 0) {
                _proc_codes = new int[codes.Length];
                Array.Copy(codes, _proc_codes, codes.Length);
            }
        }

        protected int[] _proc_codes = null;
        protected int _event_offset = 0;
        protected Queue<int> _que_evtents = new Queue<int>();
        
        protected virtual void recv_evt(int evt, int pos)
        {
            if (_proc_codes == null || Array.IndexOf(_proc_codes, evt) >= 0) {
                _que_evtents.Enqueue(evt);
                _que_evtents.Enqueue(pos + _event_offset);
            }
        }

        private float[] rd_buf = null;
        protected float[] pc_buf = null;
        protected bool take_latest = false;

        protected bool ProcessShiftBuffer(out int filePos)
        {
            if (rd_buf == null && !InitBuffer()) {
                filePos = Amplifier.ToFilePos(_rd_pos);
                return false;
            }

            if (Amplifier.Rd_GetBuf(rd_buf, ref _rd_pos, NumSampleUsed, take_latest) > 0) {
                filePos = Amplifier.ToFilePos(_rd_pos);
                ProcessEEGBuf();
                _rd_pos += _rd_shift_ms * _amp.header.samplingrate / 1000;
                return true;
            }
            else {
                filePos = -1;
            }
            return false;
        }

        protected bool ProcessEventBuffer(out int fpos)
        {
            fpos = -1;

            if (_rd_event == 0) {
                if (_que_evtents.Count < 2) {
                    return false;
                }

                _rd_event = _que_evtents.Dequeue();
                _rd_pos = _que_evtents.Dequeue();
            }

            int n = NumSampleUsed;
            if (n <= 0) return false;
            if (rd_buf == null || rd_buf.Length != n * Amplifier.header.nchan) {
                rd_buf = new float[n * Amplifier.header.nchan];
            }

            if (pc_buf == null || pc_buf.Length != n * NumChannelUsed) {
                pc_buf = new float[n * NumChannelUsed];
            }

            if (Amplifier.Rd_GetBuf(rd_buf, _rd_pos, NumSampleUsed) > 0) {
                fpos = Amplifier.ToFilePos(_rd_pos);
                //long t0 = DateTime.Now.Ticks;
                ProcessEEGBuf();
                //t0 = DateTime.Now.Ticks - t0;
                //Console.WriteLine("Proc time = {0}", t0);
                _rd_event = 0;
                _rd_pos = 0;
                return true;
            }

            return false;
        }

        internal virtual bool Initialize(Hashtable parameters)
        {
            if (proc_engine != null)
                return proc_engine.Initialize(parameters);
            return false;
        }

        public virtual void SetFeedbackHandler(Delegate handler)
        {
            if (proc_engine != null)
                proc_engine.SetFeedbackHandler(handler);
        }

        public void Free()
        {
            if (proc_engine != null) proc_engine.Free();
        }

        public void ProcessEEGBuf()
        {
            CopyProcData(rd_buf, NumChannelUsed, NumSampleUsed);
            ProcessSelectedData();
        }

        protected void CopyProcData(float[] data_source, int nch, int nspl)
        {
            int p1 = 0;
            for (int ich = 0; ich < nch; ich++) {
                int p0 = ich;
                if (chsel != null) p0 = chsel[ich];
                for (int ispl = 0; ispl < nspl; ispl++, p0 += Amplifier.header.nchan, p1++) {
                    pc_buf[p1] = data_source[p0];
                }
            }
        }

        protected virtual void ProcessSelectedData()
        {
            proc_engine.ProcEEGBuf(pc_buf, chsel.Length, NumChannelUsed);
        }

        protected int file_pos = 0;
        protected virtual void recv_dat(float[] data, int[] evt)
        {
            if (_rd_shift_ms > 0) ProcessShiftBuffer(out file_pos);
            else ProcessEventBuffer(out file_pos);
        }

        public void StartEventProcessing()
        {
            Amplifier amp = this.Amplifier;

            _que_evtents.Clear();

            if (dlg_recv_evt == null) {
                dlg_recv_evt = new Action<int, int>(recv_evt);
            }

            if (dlg_recv_dat == null) {
                dlg_recv_dat = new Action<float[],int[]>(recv_dat);
            }

            Amplifier.evt_stim_received += dlg_recv_evt;
            Amplifier.evt_data_received += dlg_recv_dat;
        }

        public void StopEventProcessing()
        {
            if (_amp != null) {
                if (dlg_recv_evt != null) {
                    _amp.evt_stim_received -= dlg_recv_evt;
                }

                if (dlg_recv_dat != null) {
                    _amp.evt_data_received -= dlg_recv_dat;
                }
            }
        }

        public bool Process()
        {
            int fpos;
            return Process(out fpos);
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        protected virtual void Dispose(bool disposing)
        {
            StopEventProcessing();
        }

        ~BCIProcessor()
        {
            SetAmplifier(null, null);
        }
    }
}
