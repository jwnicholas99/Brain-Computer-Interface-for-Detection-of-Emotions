using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;

using BCILib.Util;
using BCILib.Amp;
using BCILib.App;
using System.IO.Ports;

namespace BCILib.MotorImagery
{

    internal partial class MITrainCrossArrowForm : Form
    {
        private string _MIType = "IINNN";

        //public string MIType
        //{
        //    set
        //    {
        //        char[] nval = new string('N', 5).ToCharArray();
        //        for (int i = 0; i < nval.Length; i++) {
        //            if (value.Length > i) {
        //                char c = char.ToUpper(value[i]);
        //                if (c == 'I' || c == 'K' || c == 'T' || c == 'N') {
        //                    nval[i] = c;
        //                }
        //            }
        //        }
        //        _MIType = new string(nval);
        //    }
        //}

        private string _task_string = null;

        public string TaskString
        {
            set
            {
                StringBuilder sb = new StringBuilder("NNNNN");

                if (!string.IsNullOrEmpty(value)) {
                    foreach (char ti in value) {
                        int idx = MITaskConfig.TASK_IDLIST.IndexOf(ti);
                        if (idx >= 0 && idx < _MIType.Length) {
                            sb[idx] = 'I';
                        }
                    }
                }
                _MIType = sb.ToString();
                _task_string = value;
            }
        }

        private Keys[] _keyvals = new Keys[4] { Keys.None, Keys.None, Keys.None, Keys.None };

        public string[] KeyCodes
        {
            set
            {
                int n = 0;
                if (value != null && value.Length > 0) n = value.Length;
                if (n > 4) n = 4;
                for (int i = 0; i < n; i++) {
                    if (!string.IsNullOrEmpty(value[i])) {
                        Keys key = (Keys)Enum.Parse(typeof(Keys), value[i], true);
                        _keyvals[i] = key;
                    }
                }
            }
        }

        public MITrainCrossArrowForm()
        {
            InitializeComponent();

            LoadConfig();
        }

        MIAction _MI_task = MIAction.None;
        MI_STEP _MI_step = MI_STEP.MI_None;

        Color _task_color = Color.White;
        Pen _task_pen = new Pen(Color.White, 5);

        int _task_time = 0;
        int _prog_now = 0;

        private void MITrainCrossArrowForm_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                Graphics g = e.Graphics;
                Rectangle rt = this.ClientRectangle;

                int x0 = rt.Width / 2;
                int y0 = rt.Height / 2;
                int w0 = rt.Width / 4;
                int h0 = rt.Height / 4;
                int r0 = Math.Min(w0, h0);
                y0 = rt.Bottom - r0;

                // draw task
                _task_pen.Color = _task_color;

                if (_MI_step == MI_STEP.MI_Prepare)
                {
                    g.DrawLine(_task_pen, x0 - r0 / 2, y0, x0 + r0 / 2, y0);
                    g.DrawLine(_task_pen, x0, y0 - r0 / 2, x0, y0 + r0 / 2);
                }
                else if (_MI_step == MI_STEP.MI_Cue || _MI_step == MI_STEP.MI_Action)
                {
                    if (_MI_task == MIAction.Left)
                    {
                        g.DrawLine(_task_pen, x0 - r0 / 2, y0, x0 + r0 / 2, y0);
                        g.DrawLine(_task_pen, x0 - r0 / 2, y0, x0 - r0 / 4, y0 - r0 / 8);
                        g.DrawLine(_task_pen, x0 - r0 / 2, y0, x0 - r0 / 4, y0 + r0 / 8);
                    }
                    else if (_MI_task == MIAction.Right)
                    {
                        g.DrawLine(_task_pen, x0 - r0 / 2, y0, x0 + r0 / 2, y0);
                        g.DrawLine(_task_pen, x0 + r0 / 2 - 2, y0, x0 + r0 / 4, y0 - r0 / 8);
                        g.DrawLine(_task_pen, x0 + r0 / 2 - 2, y0, x0 + r0 / 4, y0 + r0 / 8);
                    }
                    else if (_MI_task == MIAction.Tongue)
                    {
                        g.DrawLine(_task_pen, x0, y0 - r0 / 2, x0, y0 + r0 / 2);
                        g.DrawLine(_task_pen, x0, y0 - r0 / 2 + 2, x0 - r0 / 8, y0 - r0 / 4);
                        g.DrawLine(_task_pen, x0, y0 - r0 / 2 + 2, x0 + r0 / 8, y0 - r0 / 4);
                    }
                    else if (_MI_task == MIAction.Feet)
                    {
                        g.DrawLine(_task_pen, x0, y0 - r0 / 2, x0, y0 + r0 / 2);
                        g.DrawLine(_task_pen, x0, y0 + r0 / 2 - 2, x0 - r0 / 8, y0 + r0 / 4);
                        g.DrawLine(_task_pen, x0, y0 + r0 / 2 - 2, x0 + r0 / 8, y0 + r0 / 4);
                    }
                    else if (_MI_task == MIAction.Idle)
                    {
                        g.DrawEllipse(_task_pen, x0 - r0 / 2, y0 - r0 / 2, r0, r0);
                    }
                }

                int t0 = _task_time;
                int t1 = _prog_now;
                if (t0 > 0)
                {
                    h0 = 40;
                    y0 = rt.Height - h0 - statusStrip1.Height - 5;
                    r0 = rt.Width / 2;
                    g.DrawRectangle(Pens.Blue, x0 - r0 / 2, y0, r0, h0);
                    g.FillRectangle(Brushes.Blue, x0 - r0 / 2, y0,
                        t1 * r0 / t0, h0);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in paint: error={0}", ex.Message);
            }
        }

        private void toolStripTestClick(object sender, EventArgs e)
        {
            if (sender == toolStripCross) {
                _MI_step = MI_STEP.MI_Prepare;
            } else if (sender == toolStripLeft) {
                _MI_step = MI_STEP.MI_Cue;
                _MI_task = MIAction.Left;
            }
            else if (sender == toolStripRight) {
                _MI_step = MI_STEP.MI_Cue;
                _MI_task = MIAction.Right;
            }
            else if (sender == toolStripTongue) {
                _MI_step = MI_STEP.MI_Cue;
                _MI_task = MIAction.Tongue;
            }
            else if (sender == toolStripFoot) {
                _MI_step = MI_STEP.MI_Cue;
                _MI_task = MIAction.Feet;
            }
            else if (sender == toolStripClear) {
                _MI_step = MI_STEP.MI_Cue;
                _MI_task = MIAction.None;
            }
            this.Invalidate();
        }

        private void toolStripStartTrain_Click(object sender, EventArgs e)
        {
            toolStripStartTrain.Enabled = false;
            toolStripStop.Enabled = true;
            this.ControlBox = false;

            Thread thd = new Thread(new ThreadStart(RunDataCollection));
            _evt_StopProc.Reset();
            thd.Start();
        }

        ManualResetEvent _evt_StopProc = new ManualResetEvent(false);
        ManualResetEvent _evt_ActionDone = new ManualResetEvent(false);

        private void ShowTask(MI_STEP step, MIAction task)
        {
            _MI_step = step;
            _MI_task = task;
            this.Invalidate();
        }
        private void ShowTask(MI_STEP step)
        {
            ShowTask(step, MIAction.None);
        }

        private void RunDataCollection()
        {
            ShowTask(MI_STEP.MI_None);
            //SetState(true);

            WindowsUtil.ReqSystemDisplay();

            try {
                ExeDataCollection();
            }
            catch (Exception e) {
                Console.WriteLine("Error = {0}", e);
            }

            try {
                this.BeginInvoke((dlgarg0)delegate()
                {
                    toolStripStartTrain.Enabled = true;
                    toolStripStop.Enabled = false;
                    this.ControlBox = true;
                });
            }
            catch (Exception) { }

            WindowsUtil.RelSystemDisplay();
        }

        private int cfg_num_trials = 40;
        private int cfg_waitstart_time = 2000;
        private int cfg_pre_time = 2000;
        private int cfg_cue_time = 0;
        private int cfg_act_time = 4000;
        private int cfg_rest_time = 6000;
        private int _cfg_fb_time = 2000;

        private int cfg_stim_prep = 100;
        private int cfg_stim_task_offset = 120;
        private int cfg_stim_keydown_offset = 130;
        private int cfg_stim_click_offset = 150;
        private int cfg_stim_rest = 199;

        private int cfg_img_wait = 0;

        /// <summary>
        /// Milliseconds before rest ends.
        /// </summary>
        private int cfg_beep_after_rest = 1000;

        // for feedback
        /// <summary>
        /// Number of scores to do average
        /// </summary>
        private int _cfg_num_score = 5;
        /// <summary>
        /// milliseconds to calculate score contineously
        /// </summary>
        private int _cfg_shift_score = 500;

        private void LoadConfig()
        {
            LoadConfig(BCIApplication.SysResource);
        }

        private void LoadConfig(ResManager rm)
        {
            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Number_Trials, ref cfg_num_trials);
            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Time_WaitStart, ref cfg_waitstart_time);
            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Time_Prepare, ref cfg_pre_time);
            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Time_Cue, ref cfg_cue_time);
            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Time_Action, ref cfg_act_time);
            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Time_Rest, ref cfg_rest_time);
            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Time_Imagine_Wait, ref cfg_img_wait);

            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Stim_Prepare, ref cfg_stim_prep);
            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Stim_Task_Offset, ref cfg_stim_task_offset);
            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Stim_Click_Offset, ref cfg_stim_click_offset);
            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Stim_KeyDown_Offset, ref cfg_stim_keydown_offset);
            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Stim_Rest, ref cfg_stim_rest);

            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.BeepAfterRest, ref cfg_beep_after_rest);

            rm.GetConfigValue(MIConstDef.MITest, MIConstDef.MI_Num_Score, ref _cfg_num_score);
            rm.GetConfigValue(MIConstDef.MITest, MIConstDef.MI_Shift_Score, ref _cfg_shift_score);

            _miscore_his = new double[_cfg_num_score, _MIType.Length];

            rm.SaveIfChanged();
            this.Invalidate();
        }

        public void ShowProgress(int time, bool bWait)
        {
            _task_time = time;
            Thread thd = new Thread(new ThreadStart(ExeProgress));
            thd.Start();

            if (bWait) {
                if (InvokeRequired) thd.Join();
                else while (!thd.Join(10)) Application.DoEvents();
            }
        }

        private void ExeProgress()
        {
            long t0 = HRTimer.GetTimestamp();
            _prog_now = 0;
            while (_prog_now < _task_time) {
                _prog_now = HRTimer.DeltaMilliseconds(t0);
                this.Invalidate();

                int wt = _task_time - _prog_now;
                if (wt > 50) wt = 50;
                else if (wt < 0) wt = 0;
                if (_evt_StopProc.WaitOne(wt, false)) break;
            }

            _task_time = _prog_now = 0;
            this.Invalidate();
        }

        bool bActionWait = false;
        int ctask = 0;

        double[,] _miscore_his = null;
        double[] mi_score = null;

        int _score_label = 0;
        double _score_conf = 0;
        int _score_pos = 0;

        double[] MI_Score
        {
            get
            {
                int nscore = _miscore_his.GetLength(0);
                int ntask = _miscore_his.GetLength(1);
                double[] sl = new double[ntask];

                for (int ti = 0; ti < ntask; ti++) {
                    sl[ti] = 0;

                    for (int ni = 0; ni < nscore; ni++) {
                        sl[ti] += _miscore_his[ni, ti];
                    }

                    sl[ti] /= nscore;
                }

                return sl;
            }
        }

        int _cue_label = -1;
        int avt_start = 0;

        void ReceiveMIScore(double[] conf)
        {
            int imax = 0;
            if (_cfg_num_score > 1) {
                for (int i = 1; i < conf.Length; i++) {
                    if (conf[i] > conf[imax]) imax = i;
                }
                miFeedback.ShowScore(_cue_label, imax, conf[imax], false);
            }

            if (_score_pos >= avt_start) {
                Console.Write("Score {0} = ", _score_pos);
                for (int i = 0; i < conf.Length; i++) {
                    Console.Write("{0} ", conf[i]);
                    _miscore_his[_score_pos - avt_start, i] = conf[i];
                }
                Console.WriteLine();
            }

            _score_pos++;

            if (_score_pos >= _cfg_num_score + avt_start) {

                mi_score = MI_Score;

                imax = 0;
                for (int i = 1; i < conf.Length; i++) {
                    if (mi_score[i] > mi_score[imax]) imax = i;
                }

                _score_label = imax;
                _score_conf = mi_score[imax];

                _score_pos = 0;
            }
        }

        private void ExeDataCollection()
        {
            ResManager rm = BCIApplication.SysResource;

            // specify app name
            string appname = "";
            if (_proc != null) {
                appname = "FB";

                string mfn = rm.GetConfigValue(MIConstDef.MIProcess, MIConstDef.ModelPattern);
                if (!string.IsNullOrEmpty(mfn)) {
                    mfn = Path.GetFileNameWithoutExtension(mfn);
                    int idx = mfn.IndexOf('_');
                    if (idx > 0) {
                        appname += "_" + mfn.Substring(0, idx);
                    }
                }
            }
            else {
                if (_ctrl_port != null) appname = "PM";
                else appname = "MI";
            }

            appname += "_" + _task_string;

            BCIApplication.SetProtocolName(rm, appname); // used for specify training directory
            LoadConfig(rm);

            string dpath = TrainDirSpecForm.GetTrainingPath(rm, true);
            if (dpath == null) return;

            string timestamp = BCIApplication.TimeStamp;

            //Logfile
            ConsoleCapture.StartLogFile(Path.Combine(dpath, appname + "_" + timestamp + ".log"));
            AmpContainer.StartAll();
            AmpContainer.StartRecord(dpath, appname);

            if (cfg_waitstart_time < 0) cfg_waitstart_time = 0;
            _evt_StopProc.WaitOne(cfg_waitstart_time, false);

            WaitHandle[] evts = new WaitHandle[] { _evt_StopProc, _evt_ActionDone };

            uint beep_freq = 300;
            uint beep_time = 500; // milliseconds,

            if (_proc != null) { // classification feedback
                _proc.SetFeedbackHandler(new MIProcessor.MIOutout(ReceiveMIScore));
                _proc.SetReadingShift(_cfg_shift_score);

                cfg_num_trials = 20; // temporarily set here.
            }

            if (_erd != null) {
                _erd.SetFeedbackHandler((ERDProcessor.erd_fb_output)delegate(Image img)
                {
                    erdFeedback.ShowImage(img);
                });
            }

            ConsoleOutForm.ShowWindow();

            int ntasks = _MIType.Length;

            int[] tasks = new int[ntasks];
            int[] keys = new int[ntasks];
            Random rnd = new Random();
            Amplifier amp = AmpContainer.GetAmplifier(0);

            if (_erd != null) {
                _erd.SetReadingShift(500); //_cfg_shift_score
            }

            if (cfg_beep_after_rest > 0) {
                ShowProgress(cfg_rest_time, false);
                int wt = cfg_rest_time - cfg_beep_after_rest;
                if (wt < 0) wt = 0;
                if (!_evt_StopProc.WaitOne(wt, false)) {
                    Sound.Beep(beep_freq, beep_time);
                }
                wt = cfg_rest_time - wt - (int)beep_time;
                if (wt < 0) wt = 0;
                _evt_StopProc.WaitOne(wt, false);
            }
            else {
                ShowProgress(cfg_rest_time, true);
            }

            for (int itrial = 0; itrial < cfg_num_trials; itrial++) {
                if (_evt_StopProc.WaitOne(0)) break;

                if (!amp.IsAlive) {
                    MessageBox.Show("Amplifer is not run!");
                    _evt_StopProc.Set();
                    break;
                }

                // generate tasks and randomize it
                for (int itask = 0; itask < ntasks; itask++) {
                    tasks[itask] = itask + 1;
                    keys[itask] = rnd.Next(1000);
                }
                Array.Sort(keys, tasks);

                for (int itask = 0; itask < ntasks; itask++) {
                    ctask = tasks[itask];
                    char task_cfg = _MIType[ctask - 1];
                    if (task_cfg == 'N') continue;

                    Keys task_key = Keys.None;
                    if (task_cfg == 'K') task_key = _keyvals[ctask - 1];

                    this.Invoke((dlgarg0)delegate()
                    {
                        toolStripStatusMsg.Text = string.Format("Trail {0}/{1}: task {2} = {3}",
                            itrial + 1, cfg_num_trials, itask + 1, (MIAction)(ctask + MIAction.Left - 1));
                    });

                    //Prepare
                    AmpContainer.SendAll(cfg_stim_prep);
                    Console.WriteLine("Stimcode={0}", cfg_stim_prep);
                    ShowTask(MI_STEP.MI_Prepare);
                    if (_evt_StopProc.WaitOne(cfg_pre_time, false)) {
                        break;
                    }
                    ShowTask(MI_STEP.MI_None);

                    // Show task
                    AmpContainer.SendAll(cfg_stim_task_offset + ctask);
                    Console.WriteLine("STIM_TASK:{0}", cfg_stim_task_offset + ctask);
                    ShowTask(MI_STEP.MI_Cue, (MIAction)(ctask - 1 + MIAction.Left));

                    _cue_label = ctask;
                    miFeedback.ShowScore(_cue_label, -1, 0, false);

                    if (task_key != Keys.None) {
                        // Wait for keyboard input
                        _evt_ActionDone.Reset();
                        bActionWait = true;

                        // Wait for action done
                        while (WaitHandle.WaitAny(evts, 50, false) == WaitHandle.WaitTimeout) {
                            this.Activate();
                        }
                    }
                    else { // No keyboard input
                        if (cfg_img_wait > 0) _evt_StopProc.WaitOne(cfg_img_wait, false);
                    }

                    _task_color = Color.Yellow;
                    this.Invalidate();

                    // Passive Movement
                    if (_ctrl_port != null && ctask != 5) _ctrl_port.DtrEnable = _ctrl_port.RtsEnable = true;

                    avt_start = 0;

                    if (_proc == null && _erd == null) {
                        if (_evt_StopProc.WaitOne(cfg_act_time, false)) break;
                    }
                    else {
                        int si = 0;
                        if (_proc != null) {
                            avt_start = 5;
                            _proc.SetReadingPos(); //_cfg_shift_score * _proc.Amplifier.header.samplingrate / 1000
                        }

                        //int vi = 0;
                        //int vn = 0;
                        if (_erd != null) {
                            _erd.SetReadingPos();
                            //vn = 1 + (4500 - 2000) / _cfg_shift_score;
                        }

                        Thread thd_erd = null;

                        while (si < _cfg_num_score + avt_start) {// || vi < vn
                            bool idl = true;
                            if (_proc.Process()) {
                                si++;
                                idl = false;
                            }

                            if (thd_erd == null || !thd_erd.IsAlive) {
                                thd_erd = new Thread((ThreadStart)delegate()
                                {
                                    _erd.Process();
                                });
                                thd_erd.Start();
                            }

                            int wt = idl ? 50 : 0;
                            if (_evt_StopProc.WaitOne(wt)) break;
                        }
                    }

                    _task_color = Color.White;
                    ShowTask(0);
                    bActionWait = false;

                    _evt_ActionDone.Reset();

                    // feedback time
                    if (_proc != null) { // || _erd != null
                        Thread.Sleep(_cfg_shift_score);
                        miFeedback.ShowScore(_cue_label, _score_label, _score_conf, true);
                        Console.WriteLine("Class={0} conf={1}", _score_label, _score_conf);

                        if (_evt_StopProc.WaitOne(_cfg_fb_time, false)) break;

                        Invoke((dlgarg0)delegate()
                        {
                            miFeedback.ClearDisp();
                            erdFeedback.ClearDisp();
                        });
                    }

                    // rest
                    if (_ctrl_port != null) _ctrl_port.DtrEnable = _ctrl_port.RtsEnable = false;

                    AmpContainer.SendAll(cfg_stim_rest);
                    Console.WriteLine("STIM_REST:{0}", cfg_stim_rest);

                    if (cfg_beep_after_rest > 0) {
                        ShowProgress(cfg_rest_time, false);
                        int wt = cfg_rest_time - 1000;
                        if (wt < 0) wt = 0;
                        if (!_evt_StopProc.WaitOne(wt, false)) {
                            Sound.Beep(beep_freq, beep_time);
                        }
                        wt = cfg_rest_time - wt - (int)beep_time;
                        if (wt < 0) wt = 0;
                        _evt_StopProc.WaitOne(wt, false);
                    }
                    else {
                        ShowProgress(cfg_rest_time, true);
                    }

                    if (!AmpContainer.AllAlive) {
                        _evt_StopProc.Set();
                        break;
                    }
                }
            }

            if (!_evt_StopProc.WaitOne(0, false)) {
                // finished, add training dir to training list
                BCIApplication.AddTrainingPath(dpath);
                string msg = "Training session " + appname + " Finished!";
                if (_proc != null && miFeedback.TotalTrials > 0) {
                    msg += string.Format("\n Accuracy = {0:0.##}% ({1}/{2}.", 
                        miFeedback.CorrectTrials * 100.0 / miFeedback.TotalTrials, miFeedback.CorrectTrials, miFeedback.TotalTrials);
                }
                Console.WriteLine(msg);
                MessageBox.Show(msg);
            }

            AmpContainer.StopRecord();
            ConsoleCapture.EndLogFile();
            if (_proc != null) {
                _proc.SetFeedbackHandler(null);
            }

            if (_erd != null) {
                _erd.SetFeedbackHandler(null);
            }
        }

        private void toolStripConfig_Click(object sender, EventArgs e)
        {
            BCIApplication.SysResource.ShowDialog();
        }

        private int numkeydown = 0;
        private void OnKeyDown(int key_type)
        {
            numkeydown++;
            if (numkeydown > 1) return;

            if (key_type == 1) {
                AmpContainer.SendAll(cfg_stim_keydown_offset + 1);
                Console.WriteLine("STIM_CLICK:{0}", cfg_stim_keydown_offset + 1);
            }
            else {
                AmpContainer.SendAll(cfg_stim_keydown_offset + 2);
                Console.WriteLine("STIM_CLICK:{0}", cfg_stim_keydown_offset + 2);
            }

            if (!bActionWait) {
                Console.WriteLine("Key Down: {0}", key_type);
                return;
            }
        }

        private void OnKeyUp(int key_type)
        {
            numkeydown = 0;

            AmpContainer.SendAll(cfg_stim_click_offset + key_type);
            Console.WriteLine("STIM_CLICK:{0}", cfg_stim_click_offset + 1);

            if (!bActionWait) {
                Console.WriteLine("Key up: {0}", key_type);
                return;
            }

            if (key_type == ctask) {
                _evt_ActionDone.Set();
            }
        }

        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData) {
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                    return true;
            }
            return base.IsInputKey(keyData);
        }

        private void MITrainCrossArrowForm_KeyDown(object sender, KeyEventArgs e)
        {
            int key_type = 0;

            for (int i = 0; i < _MIType.Length; i++) {
                if (_MIType[i] == 'K' && e.KeyCode == _keyvals[i]) {
                    key_type = i + 1;
                    break;
                }
            }

            if (key_type == 0) return;

            e.Handled = true;
            OnKeyDown(key_type);
        }

        private void MITrainCrossArrowForm_KeyUp(object sender, KeyEventArgs e)
        {
            int key_type = 0;
            if (e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.Oemcomma) key_type = 1;
            else if (e.KeyCode == Keys.Right || e.KeyCode == Keys.OemPeriod
                || e.KeyCode == Keys.Enter) key_type = 2;
            if (key_type == 0) return;
            e.Handled = true;
            OnKeyUp(key_type);
        }

        private void toolStripStop_Click(object sender, EventArgs e)
        {
            _evt_StopProc.Set();
        }

        MIProcessor _proc = null;
        ERDProcessor _erd = null;
        SerialPort _ctrl_port = null;

        /// <summary>
        /// Set training feedback configuration
        /// </summary>
        /// <param name="mi_proc">MI processor, if not null, show MI score as feedback;</param>
        /// <param name="erd_proc">ERD processor, if not null, show ERD feedback;</param>
        /// <param name="ctrl_port">If true, apply passive movement.</param>
        internal void SetFeedback(MIProcessor mi_proc, ERDProcessor erd_proc, SerialPort ctrl_port)
        {
            StringBuilder sb = new StringBuilder();
            _proc = mi_proc;
            _erd = erd_proc;
            _ctrl_port = ctrl_port;

            if (_proc != null) {
                miFeedback.Configurate(_MIType, toolStripStatusFeedBack);
                miFeedback.Reset();
                sb.Append(" Feedback");
            }

            if (ctrl_port != null) {
                sb.Append(" Passive Movement");
            }

            miFeedback.IsVisible = _proc != null;
            erdFeedback.IsVisible = _erd != null;

            this.Text += sb.ToString();
        }

        private void MITrainCrossArrowForm_Load(object sender, EventArgs e)
        {
        }

        private void CloseCtrlPort()
        {
            if (_ctrl_port != null) {
                if (_ctrl_port.IsOpen) _ctrl_port.Close();
                _ctrl_port = null;
            }
        }

        private void MITrainCrossArrowForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseCtrlPort();
        }
    }
}
