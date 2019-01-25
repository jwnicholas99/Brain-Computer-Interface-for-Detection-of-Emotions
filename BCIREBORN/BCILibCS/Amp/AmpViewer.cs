using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using BCILib.App;
using BCILib.sp;
using BCILib.Util;

namespace BCILib.Amp
{
    internal partial class AmpViewer : UserControl
    {
        public AmpViewer()
        {
            InitializeComponent();

            amp_rdata_handler = new Action<float[], int[]>(ReceiveRawData); 
            amp_status_handler = new Action<Amplifier>(StatusChanged);

            Amplifier.StatusChanged += amp_status_handler;
        }

        private void StatusChanged(Amplifier amp)
        {
            if (InvokeRequired) {
                BeginInvoke(amp_status_handler, amp);
            } else {
                if (amp == this.amp) {
                    toolStart.Enabled = (amp.Status == AmpStatus.Off);
                    toolStop.Enabled = (amp.Status != AmpStatus.Off);
                    toolRecord.Enabled = (amp.Status == AmpStatus.Connected && !amp.IsRecording);
                    if (amp.Status == AmpStatus.Off) {
                        StopRecording();
                        toolTextDesc.BackColor = Color.Red;
                    } else {
                        if (toolTextDesc.BackColor == Color.Red) {
                            // 20140214: configuration may be changed!
                            SetAmplifier(amp);
                        }
                        if (amp.Status == AmpStatus.Checking) {
                            toolTextDesc.BackColor = Color.Yellow;
                        } else {
                            toolTextDesc.BackColor = Color.Green;
                        }
                    }
                }
            }
        }

        string[] chn_names = null;
        private int[] used_idx = null;
        StringFormat sf = null;
        Pen axe_pen = null;
        Pen eeg_pen = null;

        static SolidBrush chn_br = null;
        static Font chn_fnt = null;

        int disp_seconds = 5;
		private double max_scale = 400;

        Amplifier amp = null;
        readonly Action<float[], int[]> amp_rdata_handler = null;
        readonly Action<Amplifier> amp_status_handler = null;

        bool ShowBigSamplingRate = false;

        public void SetAmplifier(Amplifier amp)
        {
            if (this.amp != null) {
                if (amp_rdata_handler != null) {
                    this.amp.evt_data_received -= amp_rdata_handler;
                }
                this.amp = null;
            }

            this.amp = amp;
            if (amp == null) return;

            toolAmpName.Text = amp.DevName;
            chn_names = amp.ChannelNames;

            amp.evt_data_received += amp_rdata_handler;

            LoadConfig();

            t0 = t1 = 0;
            rsz = 0;
            //eeg_data_queue.Clear();
        }

        private void LoadConfig()
        {
            ResManager rm = BCIApplication.AppResource;
            string chan_used = rm.GetConfigValue(amp.ID, "UsedChannels");
            string[] chan_names = chn_names;
            if (!string.IsNullOrEmpty(chan_used)) {
                string[] used_names = chan_used.Split(',');
                ArrayList al = new ArrayList();
                for (int i = 0; i < used_names.Length; i++) {
                    for (int j = 0; j < chan_names.Length; j++) {
                        if (chan_names[j] == used_names[i]) {
                            al.Add(j);
                            break;
                        }
                    }
                }
                used_idx = (int[])al.ToArray(typeof(int));
            }
            else {
                used_idx = new int[chan_names.Length];
                for (int j = 0; j < chan_names.Length; j++) {
                    used_idx[j] = j;
                }
            }
            ChangeEEGBuffer = true;

            rm.GetConfigValue(amp.ID, "DispSeconds", ref disp_seconds);
            rm.SaveIfChanged();
        }

        float[,] draw_eeg_data = null;
        short[] draw_evt_data = null;
        int draw_pos = 0;
        bool datafull = false;

        // ccwang: 20110906: ranges
        float[] rang_dl = null, rang_dh = null, rang_var = null;

        int rct = 1000;
        int rcl = 0, rc0 = 0;

        private void SetEEGBuffer()
        {
            if ((used_idx == null || used_idx.Length == 0) && uidx1 == null) return;

            if (LockBuffer) return;
            LockBuffer = true;

            if (uidx1 != null) {
                used_idx = uidx1;
                uidx1 = null;
            }

            if (amp.header.samplingrate == 0) {
                notch_filters = null;
                draw_eeg_data = null;
                draw_evt_data = null;
                LockBuffer = false;
                return;
            }

            int nch = used_idx.Length;
            int nspl = disp_seconds * amp.header.samplingrate;

            if (rang_dh == null || rang_dh.Length != nch) {
                rang_dh = new float[nch];
                rang_dl = new float[nch];
                rang_var = new float[nch];
            }

            if (draw_evt_data == null || draw_evt_data.Length != nspl ||
                nch != draw_eeg_data.GetLength(0))
            {

                float[,] neeg = new float[nch, nspl];
                short[] nevt = new short[nspl];

                draw_pos = 0;
                datafull = false;
                draw_eeg_data = neeg;
                draw_evt_data = nevt;

                if (notch_filters == null || notch_filters.Length < used_idx.Length) {
                    notch_filters = new Filter[used_idx.Length];
                    double[] B_notch =
                        new double[] { 1.789951120, -1.206032741, 1.789951120 };
                    double[] A_notch =
                        new double[] { 1.0, -0.60301637, 0.789951120 };

                    for (int i = 0; i < used_idx.Length; i++)
                        notch_filters[i] = new Filter(B_notch, A_notch);
                }

                for (int fi = 0; fi < notch_filters.Length; fi++)
                    notch_filters[fi].Init();
            }

            ChangeEEGBuffer = false;
            LockBuffer = false;
        }

        Filter[] notch_filters = null;

        private int rsz = 0;
        private long t0 = 0;
        private long t1 = 0;

        //private Queue eeg_data_queue = new Queue();

        public void InitSplStatistics(int t_seconds)
        {
            _secData = new int[t_seconds];
            //for (int i = 0; i < _secData.Length; i++) {
            //    _secData[i] = 0;
            //}
            _snow = 0;
            _IsSplRecording = true;
        }

        public void StopSplStatistics()
        {
            _IsSplRecording = false;
        }

        int [] _secData = null;
        int _snow = 0;
        bool _IsSplRecording = false;

        public int[] SPR_SecData
        {
            get
            {
                return _secData;
            }
        }

        private bool _UpdateStatus = false;
        private int _rSplRate = 0;
        private int _dSplRate = 0;

        public int[] SPR_Statistics
        {
            get
            {
                return _secData;
            }
        }

        private void ReceiveRawData(float[] buf, int[] evtlist)
        {
            if (t0 == 0) {
                // first time
                t0 = DateTime.Now.Ticks;
                t1 = 0;
                rsz = 0;
                _UpdateStatus = true;
                rcl = rct * amp.header.samplingrate / 1000;
                rc0 = draw_pos;
            }
            else t1 = DateTime.Now.Ticks - t0;

            if (ChangeEEGBuffer) {
                SetEEGBuffer();
            }

            if (t1 >= 10000000) { // 1s
                t0 += t1;
                t1 = 0;

                _rSplRate = rsz;
                _dSplRate = rsz;

                if (_IsSplRecording) {
                    if (_snow >= _secData.Length) {
                        _IsSplRecording = false;
                    } else {
                        _secData[_snow++] = rsz;
                    }
                }

                rsz = 0;
            }

            rsz += amp.header.blk_samples;

            if (draw_eeg_data == null || draw_evt_data == null) {
                return;
            }

            if (evtlist == null || evtlist.Length != amp.header.blk_samples ||
                buf == null || buf.Length != amp.header.blk_samples * amp.header.nchan)
            {
                //Console.WriteLine("Data eror!");
                return;
            }

            int nspls = amp.header.blk_samples;
            int nsz = draw_eeg_data.GetLength(1);
            int cpos = draw_pos;
            int nch = used_idx.Length;

            for (int isample = 0, off = 0; isample < nspls; isample++, off += amp.header.nchan) {
                for (int k = 0; k < nch; k++) {
                    float fval = buf[off + used_idx[k]];
                    if (NotchFilter) fval = (float)notch_filters[k].Process(fval);
                    draw_eeg_data[k, cpos] = fval;
                }


                draw_evt_data[cpos] = (short) evtlist[isample];

                cpos++;
                if (cpos >= nsz) {
                    cpos = 0;
                    datafull = true;
                }

                if ((cpos - rc0 + nsz) % nsz >= rcl) {
                    // get range and variance
                    for (int ich = 0; ich < nch; ich++) {
                        double m = 0;
                        rang_dl[ich] = rang_dh[ich] = draw_eeg_data[ich, rc0];

                        int rci = rc0;
                        for (int ispl = 0; ispl < rcl; ispl++) {
                            float fv = draw_eeg_data[ich, rci];

                            // range
                            if (rang_dh[ich] < fv) rang_dh[ich] = fv;
                            else if (rang_dl[ich] > fv) rang_dl[ich] = fv;

                            // sum
                            m += fv;

                            rci++;
                            if (rci >= nsz) rci = 0;
                        }

                        // mean
                        m /= rcl;

                        rci = rc0;
                        double dv = 0;
                        for (int ispl = 0; ispl < rcl; ispl++) {
                            float fv = draw_eeg_data[ich, rci];
                            double df = fv - m;
                            dv += df * df;

                            rci++;
                            if (rci >= nsz) rci = 0;
                        }
                        if (rcl > 1) dv /= (rcl - 1);
                        dv = Math.Sqrt(dv);

                        rang_var[ich] = (float) dv;
                    }

                    rc0 = cpos;
                }
            }

            draw_pos = cpos;
            panel1.Invalidate();
        }

        private void toolStart_Click(object sender, EventArgs e)
        {
            t0 = t1 = 0;
            rsz = 0;
            if (amp == null) return;
            if (amp.IsAlive) {
                MessageBox.Show("Amplifier already started!");
            }
            LockBuffer = false;
            if (!amp.Start()) {
                MessageBox.Show("Starting amplifer failed: " + toolAmpName.Text);
            }
        }

        private void toolStop_Click(object sender, EventArgs e)
        {
            if (amp != null) amp.Stop();
            t0 = t1 = 0;
            rsz = 0;
        }

        private int[] uidx1 = null;

        private void toolSetChannel_Click(object sender, EventArgs e)
        {
            if (amp == null) return;

            SelectStringList dlg = new SelectStringList(amp.ChannelNames);
            dlg.Text = "Select channels to display";
            dlg.SelectedListIndex = used_idx;
            if (dlg.ShowDialog() == DialogResult.OK) {
                ResManager rm = BCIApplication.AppResource;
                //rm.SetConfigValue(amp.ID, "ChannelNames", dlg.CandidateString);
                rm.SetConfigValue(amp.ID, "UsedChannels", dlg.SelectedString);
                rm.SaveFile();
                uidx1 = dlg.SelectedListIndex;

                if (!amp.IsAlive) {
                    SetEEGBuffer();
                    panel1.Invalidate();
                }
                else {
                    ChangeEEGBuffer = true;
                }
            }
        }

        private bool ChangeEEGBuffer = false;

        protected override void OnResize(EventArgs e)
        {
            panel1.Invalidate();
            base.OnResize(e);
        }

        private void toolAccelerate_Click(object sender, EventArgs e)
        {
            if (disp_seconds > 1){
                disp_seconds--;
                ChangeEEGBuffer = true;
            }
        }

        private void toolDecelerate_Click(object sender, EventArgs e)
        {
            disp_seconds++;
            ChangeEEGBuffer = true;
        }

        bool LockBuffer = false;

        int disp_mode = 0; // 0 = shift; 1 = fix

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (_UpdateStatus)
            {
                SetStatus();
            }
            //if (_rSplRate != 0)
            //{
            //    toolTextSamplingRate.Text = _rSplRate.ToString();
            //    _rSplRate = 0;
            //}
            toolTextSamplingRate.Text = amp.ActSampleRate.ToString();

            try {
                Rectangle rc = panel1.ClientRectangle;

                //lock (this)
                if (!LockBuffer) {
                    LockBuffer = true;

                    Graphics g = e.Graphics;
                    //Bitmap img = new Bitmap(rc.Width, rc.Height);
                    //Graphics g = Graphics.FromImage(img);
                    g.FillRectangle(Brushes.White, rc);

                    int w_chname = 60;
                    if (sf == null) {
                        sf = new StringFormat(StringFormatFlags.NoWrap);
                        sf.LineAlignment = StringAlignment.Center;
                        sf.Alignment = StringAlignment.Near;

                        axe_pen = new Pen(this.ForeColor);
                        axe_pen.DashStyle = DashStyle.Dot;
                        eeg_pen = new Pen(this.ForeColor);
                        chn_br = new SolidBrush(this.ForeColor);
                    }

                    if (chn_fnt == null) {
                        try {
                            chn_fnt = new Font(FontFamily.GenericMonospace, 8.0f, GraphicsUnit.Point);
                        }
                        catch (Exception ex) {
                            Console.WriteLine("AmpViewer_Paint: error in font:{0}", ex);
                        }
                    }

                    Font fnt = chn_fnt != null ? chn_fnt : toolTextDesc.Font;

                    float y0 = rc.Y;
                    float h1 = 1;
                    float ch = 10;

                    // draw channel names
                    if (used_idx != null) {
                        h1 = (rc.Height - ch) / (used_idx.Length + 1);
                        y0 += h1 / 2;
                        for (int ich = 0; ich < used_idx.Length; ich++) {
                            string chn_msg = chn_names[used_idx[ich]];
                            if (rcl > 0 && !IsSpecialChannel(ich)) {
                                chn_msg += string.Format("\n{0:#.#}/{1:#.#}", rang_dh[ich] - rang_dl[ich], rang_var[ich]);
                            }

                            // channel name
                            g.DrawString(chn_msg, fnt, chn_br,
                                new RectangleF(0, y0, w_chname * 2, h1), sf);

                            y0 += h1 / 2;
                            g.DrawLine(axe_pen, w_chname, y0, rc.Width - 1, y0);
                            y0 += (h1 - h1 / 2);
                        }
                    }

                    bool datafull = this.datafull; // 20131127: it can be changed if not get a local copy!
                    int cpos = draw_pos;

                    if (draw_eeg_data != null && (datafull || cpos > 1)) {
                        int call = draw_evt_data.Length;
                        int nch = draw_eeg_data.GetLength(0);

                        int np = call;
                        if (!datafull) np = cpos;

                        double x_factor = ((double)(rc.Width - w_chname)) / (double)call;

                        PointF[] eeg_plg = null;
                        
                        if (disp_mode == 0) eeg_plg = new PointF[np];

                        float y_pos = rc.Y + h1;
                        int p0 = 0;
                        for (int ich = 0; ich < nch; ich++) {
                            double offy = dsp_offset;
                            double maxy = max_scale;
                            if (chn_names[used_idx[ich]] == "ATT") {
                                maxy = 100;
                                offy = 0;
                            }
                            else if (chn_names[used_idx[ich]] == "N") {
                                maxy = 200;
                                offy = 0;
                            }

                            double y_factor = ((double)h1) / (2 * maxy);
                            double x_pos = w_chname;
                            if (disp_mode == 1) {
                                eeg_plg = new PointF[call - cpos];
                                x_pos += cpos * x_factor;
                            }
                            p0 = 0;
                            if (datafull) {
                                for (int ip = cpos; ip < call; ip++) {
                                    eeg_plg[p0].X = (float)x_pos;
                                    x_pos += x_factor;
                                    eeg_plg[p0].Y = (float)(y_pos - (draw_eeg_data[ich, ip] + offy) * y_factor);
                                    p0++;
                                }
                                if (disp_mode == 1 && p0 > 1) {
                                    g.DrawLines(eeg_pen, eeg_plg);
                                }
                            }
                            else {
                                x_pos += x_factor * (call - cpos);
                            }

                            if (disp_mode == 1) {
                                p0 = 0;
                                x_pos = w_chname;
                                eeg_plg = new PointF[cpos];
                            }

                            for (int ip = 0; ip < cpos; ip++) {
                                if (p0 < 0 || p0 >= eeg_plg.Length)
                                {
                                    Console.WriteLine("Paint error: p0={0} out of range ({1})!", p0, eeg_plg.Length);
                                }
                                else
                                {
                                    eeg_plg[p0].X = (float)x_pos;
                                    x_pos += x_factor;
                                    eeg_plg[p0].Y = (float)(y_pos - (draw_eeg_data[ich, ip] + offy) * y_factor);
                                }
                                p0++;
                            }

                            try {
                                if (p0 > 1) {
                                    g.DrawLines(eeg_pen, eeg_plg);
                                }
                            }
                            catch (Exception) {
                                Console.WriteLine("Something wrong here!");
                            }
                            y_pos += h1;
                        }

                        // event
                        p0 = 0;
                        if (disp_mode == 1) p0 = cpos;

                        if (datafull) {
                            for (int ip = cpos; ip < call; ip++, p0++) {
                                if (draw_evt_data[ip] > 0) {
                                    int xs = (int)(w_chname + p0 * x_factor);
                                    g.DrawString(draw_evt_data[ip].ToString(), fnt, Brushes.Blue, xs, y0);
                                }
                            }
                        }
                        else {
                            p0 += call - cpos;
                        }

                        if (disp_mode == 1) p0 = 0;

                        for (int ip = 0; ip < cpos; ip++, p0++) {
                            if (draw_evt_data[ip] > 0) {
                                int xs = (int)(w_chname + p0 * x_factor);
                                g.DrawString(draw_evt_data[ip].ToString(), fnt, Brushes.Blue, xs, y0);
                            }
                        }

                        // time mark (s)
                        int steps = call / disp_seconds;
                        for (int ip = 0; ip < call; ip += steps) {
                            if (!datafull && ip >= cpos) break;
                            float x0 = ip;
                            if (disp_mode == 0) {
                                if (datafull && ip >= cpos) x0 -= cpos;
                                else x0 += call - cpos;
                            }
                            x0 = (float)(x0 * x_factor) + w_chname;
                            g.DrawLine(Pens.Blue, x0, y_pos, x0, rc.Bottom);
                        }

                        if (disp_mode == 1) {
                            //draw front line
                            float x0 = (float) (w_chname + cpos * x_factor);
                            g.DrawLine(Pens.Blue, x0, rc.Top + 10, x0, rc.Bottom - ch);
                        }
                    }

                    LockBuffer = false;

                    if (_IsSplRecording || ShowBigSamplingRate)
                    {
                        Font sfnt = new Font(FontFamily.GenericMonospace, (float)rc.Height / 2, FontStyle.Bold);

                        StringFormat sfmt = new StringFormat(StringFormatFlags.NoWrap);
                        sfmt.LineAlignment = StringAlignment.Center;
                        sfmt.Alignment = StringAlignment.Center;

                        g.DrawString(_dSplRate.ToString(), sfnt, Brushes.Red, rc, sfmt);
                    }
                }
            }
            catch (Exception ex) {
                Console.WriteLine("Display exception = " + ex.Message);
            }
        }

        private void toolAutoScale_Click(object sender, EventArgs e)
        {
            int cpos = draw_pos;

            if (draw_eeg_data == null || cpos == 0 && !datafull) return;

            // find min/max value
            float fmin = float.MaxValue, fmax = float.MinValue;
            int count = draw_evt_data.Length;
            if (!datafull) count = cpos;

            for (int i = 0; i < count; i++) {
                for (int ichan = 0; ichan < used_idx.Length; ichan++) {
                    if (IsSpecialChannel(ichan)) continue;
                    float v = draw_eeg_data[ichan, i];
                    if (fmin > v) fmin = v;
                    else if (fmax < v) fmax = v;
                }
            }

            dsp_offset = -(fmin + fmax) / 2;
            max_scale = fmax + dsp_offset;
            if (max_scale < 0.1) max_scale = 0.1;
            panel1.Invalidate();

            SetStatus();
        }

        private bool IsSpecialChannel(int ichan)
        {
            if (string.Equals(chn_names[used_idx[ichan]], "N")) return true;
            if (string.Equals(chn_names[used_idx[ichan]], "ATT")) return true;
            return false;
        }

        private void SetStatus()
        {
            toolAmpName.Text = amp.DevName;

            string msg = string.Format("Scale:{0:####.##}", max_scale);
            if (amp != null) msg += " " + amp.header.ToString();
            if (NotchFilter) msg += " Notch";
            toolTextDesc.Text = msg;
            toolShiftUp.ToolTipText = string.Format("Shift up({0:#0})", dsp_offset);
            toolShiftDown.ToolTipText = string.Format("Shift down({0:#0})", dsp_offset);
            _UpdateStatus = false;
        }

        private void toolScaleUp_Click(object sender, EventArgs e)
        {
            max_scale /= 2;
            panel1.Invalidate();
            SetStatus();
        }

        private void toolScaleDown_Click(object sender, EventArgs e)
        {
            max_scale *= 2;
            panel1.Invalidate();
            SetStatus();
        }

        private void toolAmpName_Click(object sender, EventArgs e)
        {
            if (amp.IsAlive && MessageBox.Show("Configue amplifier parameters, amplifer will be stopped. Continue?", "Ampilifier Configuration", MessageBoxButtons.OKCancel) != DialogResult.OK) {
                return;
            }
            amp.Stop();

            if (amp.Configure()) {
                amp.SaveConfig(BCIApplication.AppResource);
                chn_names = amp.ChannelNames;
                rsz = 0;
                draw_pos = 0;
                datafull = false;
                panel1.Invalidate();
                LoadConfig();

                toolAmpName.Text = amp.DevName;
            }
        }

        private void StopRecording()
        {
            if (amp != null) amp.StopRecord();
            toolRecord.Checked = false;
        }

        private void toolRecord_Click(object sender, EventArgs e)
        {
            if (toolRecord.Checked) {
                StopRecording();
            }
            else {// Browse for recording directory
                if (amp != null) {
                    SaveFileDialog dlg = new SaveFileDialog();
                    dlg.RestoreDirectory = true;
                    dlg.DefaultExt = ".cnt";
                    dlg.Filter = "Cnt File(*.cnt)|*.cnt|All Files(*.*)|*.*";
                    if (dlg.ShowDialog() != DialogResult.OK) {
                        return;
                    }
                    StartRecord(dlg.FileName);
                }
            }
        }

        private void StartRecord(string fn)
        {
            amp.StartRecord(fn);
            toolRecord.Checked = true;
        }

        bool NotchFilter = false;
        private void toolNotch_Click(object sender, EventArgs e)
        {
            NotchFilter = toolNotch.Checked;
            SetStatus();
        }

        float dsp_offset = 0;

        public int DisplayMode
        {
            get
            {
                return disp_mode;
            }
            set
            {
                disp_mode = value;
                //panel1.Invalidate();
            }
        }

        private void toolTextSamplingRate_Click(object sender, EventArgs e)
        {
            ShowBigSamplingRate = !ShowBigSamplingRate;
        }

        ~AmpViewer()
        {
            SetAmplifier(null);
        }

        private void menuStartAutoStim_Click(object sender, EventArgs e)
        {
            if (!amp.Start()) return;

            menuStartAutoStim.Enabled = false;
            ThreadPool.QueueUserWorkItem(x =>
            {
                ManualResetEvent evt = new ManualResetEvent(false);
                List<int> rc = new List<int>();
                Action<int, int> rh = (code, pos) =>
                {
                    rc.Add(code);
                    //evt.Set();
                };

                amp.evt_stim_received += rh;

                int rclen = rc.Count;

                amp.SendStimCode(0);
                Thread.Sleep(10);

                for (int code = 1; code < 128 && amp.IsAlive; code++)
                {
                    int t0 = BCIApplication.ElaspedMilliSeconds;
                    amp.SendStimCode(code);

                    int wt = 200 - (BCIApplication.ElaspedMilliSeconds - t0);
                    if (wt > 0) evt.WaitOne(wt, false);
                }

                if (amp.IsAlive) {
                    for (int i = 0; i < 100; i++) {
                        if (rc.Count >= 127) break;
                        evt.WaitOne(20, false);
                    }

                    int s0 = 1;
                    List<string> errors = new List<string>();

                    for (int i = 0; i < rc.Count; i++) {
                        if (rc[i] == s0) {
                            s0++;
                            continue;
                        }

                        if (rc[i] > s0 && i < rc.Count - 1 && rc[i] < rc[i + 1]) {
                            errors.Add("Missing " + string.Join(",", Enumerable.Range(s0, rc[i] - s0).Select(s => s.ToString()).ToArray()));
                            s0 = rc[i] + 1;
                            continue;
                        } else {
                            errors.Add(string.Format("Wrong code: {0}({1})", rc[i], s0));
                            s0++;
                        }
                    }

                    if (s0 < 128) {
                        errors.Add("Missing " + string.Join(",", Enumerable.Range(s0, 128 - s0).Select(s => s.ToString()).ToArray()));
                    }

                    if (errors.Count > 0) {
                        MessageBox.Show("Error!\r\n" + string.Join("\r\n", errors.ToArray()) + "\r\n" + "Recv=" +
                            string.Join(",", rc.Select(s => s.ToString()).ToArray()));
                    }
                }

                Invoke((Action)(() => { menuStartAutoStim.Enabled = true; }));
            });
        }

        private void toolShiftUp_Click(object sender, EventArgs e)
        {
            dsp_offset += (float)max_scale / 5;
            panel1.Invalidate();
            SetStatus();
        }

        private void toolShiftDown_Click(object sender, EventArgs e)
        {
            dsp_offset -= (float)max_scale / 5;
            panel1.Invalidate();
            SetStatus();
        }

        private void ResetDisplayShift(object sender, EventArgs e)
        {
            dsp_offset = 0;
            panel1.Invalidate();
            SetStatus();
        }
    }
}
