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
using System.Reflection;

namespace BCILib.MotorImagery.ArtsBCI
{

    internal partial class MIHapticKnobForm : Form
    {
        /// <summary>
        /// Motor Imagery Type for all tasks. The seqence is LRTFI (Left, Right, Tongue, Feet and Idle).
        /// Value for each Task: N = none, K = keyboard, T = tap, I = imagine
        /// </summary>
        private string _MIType = "IINNN";

        /// <summary>
        /// ????? - defintion for 5 tasks: N/K/T/I
        /// </summary>
        public string MIType
        {
            set
            {
                char[] nval = new string('N', 5).ToCharArray();
                for (int i = 0; i < nval.Length; i++) {
                    if (value.Length > i) {
                        char c = char.ToUpper(value[i]);
                        if (c == 'I' || c == 'K' || c == 'T' || c == 'N') {
                            nval[i] = c;
                        }
                    }
                }
                _MIType = new string(nval);
            }
        }

        private string _task_string = null;

        /// <summary>
        /// Combination of L/R/T/F/I.
        /// </summary>
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

        public MIHapticKnobForm()
        {
            InitializeComponent();

            LoadConfig();

            dl_getscore = new Action<double>(MIFLD_GetScore);
        }

        MI_STEP _MI_step = MI_STEP.MI_None;
        MIAction _MI_task = MIAction.None;

        Color _task_color = Color.White;
        Pen _task_pen = new Pen(Color.White, 5);

        FB_Form fb_form = new FB_Form();

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            BringToFront();

            fb_form.Show();
            fb_form.Set_HandCueWindow(_hand_cue);
            if (_mode == 1) {
                fb_form.Set_HKWindow(_haptic_knob);
                _haptic_knob.SendClient(GameCommand.HapticKnob, (int)RehabCommand.None);
            }

            KMCapture.kbd_output += new Func<Keys,uint,uint,bool>(OnKeyboardInput);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle rt = this.ClientRectangle;

            int x0 = rt.Width / 2;
            int y0 = rt.Height / 2;
            int w0 = rt.Width / 4;
            int h0 = rt.Height / 4;
            int r0 = Math.Min(w0, h0);
            y0 = rt.Bottom - r0 ;

            // draw task
            _task_pen.Color = _task_color;

            if (_MI_step == MI_STEP.MI_Prepare) {
                g.DrawLine(_task_pen, x0 - r0 / 2, y0, x0 + r0 / 2, y0);
                g.DrawLine(_task_pen, x0, y0 - r0 / 2, x0, y0 + r0 / 2);
            }
            else if (_MI_step == MI_STEP.MI_Cue || _MI_step == MI_STEP.MI_Action) {
                if (_MI_task == MIAction.Left) {
                    g.DrawLine(_task_pen, x0 - r0 / 2, y0, x0 + r0 / 2, y0);
                    g.DrawLine(_task_pen, x0 - r0 / 2, y0, x0 - r0 / 4, y0 - r0 / 8);
                    g.DrawLine(_task_pen, x0 - r0 / 2, y0, x0 - r0 / 4, y0 + r0 / 8);
                }
                else if (_MI_task == MIAction.Right) {
                    g.DrawLine(_task_pen, x0 - r0 / 2, y0, x0 + r0 / 2, y0);
                    g.DrawLine(_task_pen, x0 + r0 / 2 - 2, y0, x0 + r0 / 4, y0 - r0 / 8);
                    g.DrawLine(_task_pen, x0 + r0 / 2 - 2, y0, x0 + r0 / 4, y0 + r0 / 8);
                }
                else if (_MI_task == MIAction.MITongue) {
                    g.DrawLine(_task_pen, x0, y0 - r0 / 2, x0, y0 + r0 / 2);
                    g.DrawLine(_task_pen, x0, y0 - r0 / 2 + 2, x0 - r0 / 8, y0 - r0 / 4);
                    g.DrawLine(_task_pen, x0, y0 - r0 / 2 + 2, x0 + r0 / 8, y0 - r0 / 4);
                }
                else if (_MI_task == MIAction.Feet) {
                    g.DrawLine(_task_pen, x0, y0 - r0 / 2, x0, y0 + r0 / 2);
                    g.DrawLine(_task_pen, x0, y0 + r0 / 2 - 2, x0 - r0 / 8, y0 + r0 / 4);
                    g.DrawLine(_task_pen, x0, y0 + r0 / 2 - 2, x0 + r0 / 8, y0 + r0 / 4);
                } else if (_MI_task == MIAction.Idle) {
                    g.DrawEllipse(_task_pen, x0 - r0 / 2, y0 - r0 / 2, r0, r0);
                }
            }

            //if (_result_msg != null) {
            //    if (_fnt_msg == null) {
            //        _fnt_msg = new Font(FontFamily.GenericSerif, 24);
            //    }

            //    StringFormat sf = new StringFormat();
            //    sf.Alignment = StringAlignment.Center;
            //    sf.LineAlignment = StringAlignment.Center;
            //    rt = new Rectangle(rt.Left, rt.Height - 40 - statusStrip1.Height - 60, rt.Width, 40);
            //    g.DrawString("Finished.", _fnt_msg, Brushes.Wheat, rt, sf);
            //    rt.Y += 40;
            //    g.DrawString(_result_msg, _fnt_msg, Brushes.Wheat, rt, sf);
            //}
        }

        //private void toolStripTestClick(object sender, EventArgs e)
        //{
        //    if (sender == toolStripCross) {
        //        _MI_step = MI_STEP.MI_Prepare;
        //    } else if (sender == toolStripLeft) {
        //        _MI_step = MI_STEP.MI_Cue;
        //        _MI_task = MIAction.Left;
        //    }
        //    else if (sender == toolStripRight) {
        //        _MI_step = MI_STEP.MI_Cue;
        //        _MI_task = MIAction.Right;
        //    }
        //    else if (sender == toolStripTongue) {
        //        _MI_step = MI_STEP.MI_Cue;
        //        _MI_task = MIAction.Tongue;
        //    }
        //    else if (sender == toolStripFoot) {
        //        _MI_step = MI_STEP.MI_Cue;
        //        _MI_task = MIAction.Feet;
        //    }
        //    else if (sender == toolStripClear) {
        //        _MI_step = MI_STEP.MI_None;
        //    }
        //    this.Invalidate();
        //}

        private void toolStripStartTrain_Click(object sender, EventArgs e)
        {
            SetPause(false);

            fb_form.Set_Message(null);
            fb_form.Set_Result(null);
            fb_form.Set_HandCueWindow(_hand_cue);

            if (_mode == 1) {
                fb_form.Set_HKWindow(_haptic_knob);
            }

            toolStripStartTrain.Enabled = false;
            toolStripStop.Enabled = true;
            this.ControlBox = false;

            //miFeedback.Reset();

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

            try {
                if (_mode == 0)
                {
                    ExeDataCollection();
                } else if (_proc != null) {
                    ExeBCIHKRehab();
                } else {
                    ExeHKRehabOnly();
                }
            }
            catch (Exception e) {
                Console.WriteLine("Error = {0}", e);
            }

            try {
                if (_proc != null) {
                    _proc.SetFeedbackHandler(null);
                }
                //if (_erd != null) {
                //    _erd.SetFeedbackHandler(null);
                //}
                if (_hand_cue != null) {
                    _hand_cue.SendClient(GameCommand.StopGame);
                }
                if (_haptic_knob != null) {
                    _haptic_knob.SendClient(GameCommand.StopGame);
                }

                this.BeginInvoke((Action)delegate()
                {
                    toolStripStartTrain.Enabled = true;
                    toolStripStop.Enabled = false;
                    this.ControlBox = true;

                    fb_form.BringToFront();
                });
            }
            catch (Exception) { }

            fb_form.SetMIStep(MI_STEP.MI_None);
        }

        private int cfg_num_trials = 40;
        private int cfg_num_rehab = 40;
        private int cfg_waitstart_time = 2000;
        private int cfg_prep_time = 2000;
        private int cfg_cue_time = 0;
        private int cfg_act_time = 4000;
        private int cfg_rest_time = 6000;
        private int cfg_fail_rest = -1;
        private int cfg_fb_time = 2000;
        private RehabAction cfg_pm_action = RehabAction.OpenClose;

        private int cfg_stim_prep = 100;
        private int cfg_stim_task_offset = 120;
        private int cfg_stim_keydown_offset = 130;
        private int cfg_stim_click_offset = 150;
        private int cfg_stim_rest = 199;
        private int cfg_stim_rehab = 180;

        private int cfg_feedback_flag = (int)FB_FLAG.All;

        private int cfg_fail_tries = 2;

        private int cfg_img_wait = 0;
        private double cfg_score_bias = 0;
        private bool cfg_save_bias = false;

        /// <summary>
        /// Milliseconds before rest ends.
        /// </summary>
        private int cfg_beep_before_rest_end = 1000;

        // for feedback
        /// <summary>
        /// Number of scores to do average
        /// </summary>
        private int cfg_num_score = 5;
        /// <summary>
        /// milliseconds to calculate score contineously
        /// </summary>
        private int cfg_shift_score = 500;

        private void LoadConfig()
        {
            LoadConfig(BCIApplication.SysResource);
        }

        private void LoadConfig(ResManager rm)
        {
            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Number_Trials, ref cfg_num_trials);
            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Time_WaitStart, ref cfg_waitstart_time);
            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Time_Prepare, ref cfg_prep_time);
            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Time_Cue, ref cfg_cue_time);
            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Time_Action, ref cfg_act_time);
            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Time_Rest, ref cfg_rest_time);

            if (cfg_fail_rest < 0) cfg_fail_rest = cfg_rest_time / 2;
            rm.GetConfigValue(MIConstDef.MITraining, "Fail_Rest", ref cfg_fail_rest);

            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Time_Imagine_Wait, ref cfg_img_wait);

            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Stim_Prepare, ref cfg_stim_prep);
            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Stim_Task_Offset, ref cfg_stim_task_offset);
            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Stim_Click_Offset, ref cfg_stim_click_offset);
            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Stim_KeyDown_Offset, ref cfg_stim_keydown_offset);
            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Stim_Rest, ref cfg_stim_rest);

            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.BeepAfterRest, ref cfg_beep_before_rest_end);
            string pm_action = cfg_pm_action.ToString();
            rm.GetConfigValue(MIConstDef.MITraining, "PM_Action", ref pm_action);
            cfg_pm_action = (RehabAction)Enum.Parse(typeof(RehabAction), pm_action, true);

            rm.GetConfigValue(MIConstDef.MITest, MIConstDef.MI_Num_Score, ref cfg_num_score);
            rm.GetConfigValue(MIConstDef.MITest, MIConstDef.MI_Shift_Score, ref cfg_shift_score);
            rm.GetConfigValue(MIConstDef.MITest, MIConstDef.MI_Num_Rehab, ref cfg_num_rehab);
            rm.GetConfigValue(MIConstDef.MITest, "Num_Failed_Tries", ref cfg_fail_tries);

            // Feedback Flag
            rm.GetConfigValue(MIConstDef.MITest, "FeedbackFlag", ref cfg_feedback_flag);
            rm.GetConfigValue(MIConstDef.MITest, "SaveBias", ref cfg_save_bias);
            rm.GetConfigValue(MIConstDef.MITest, "ScoreBias", ref cfg_score_bias);

            rm.SaveIfChanged();

            FB_FLAG fg = (FB_FLAG)cfg_feedback_flag;
            cbFLDScore.Checked = (fg & FB_FLAG.FLDScore) == FB_FLAG.FLDScore;
            cbSmiley.Checked = (fg & FB_FLAG.SmileyFace) == FB_FLAG.SmileyFace;
            cbDispMsg.Checked = (fg & FB_FLAG.Message) == FB_FLAG.Message;
            fb_form.SetFeedbackFlag(fg);
            fb_form.Set_Result(null);

            cbSaveBias.Checked = cfg_save_bias;
            if (cfg_save_bias) {
                SetBias(cfg_score_bias, true, false);
            } else {
                SetBias(0, true, false);
            }
        }

        enum WaitAction
        {
            None, Keyboard, Hapticknob
        };

        WaitAction actionWait = WaitAction.None;
        MIAction ctask = MIAction.None;

        int _score_cls = 0;
        double _score_conf = 0;

        int _cue_label = -1;

        private void ExeDataCollection()
        {
            ResManager rm = BCIApplication.SysResource;

            // specify app name
            string appname;
            if (_proc == null) {
                // no feedback
                if (_haptic_knob != null) appname = "PM";
                else appname = "MI";
            }
            else
            {
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

            appname += "_" + _task_string;

            BCIApplication.SetProtocolName(rm, appname); // used to define data directory

            Invoke((Action)delegate()
            {
                LoadConfig(rm);
            });

            string dpath = TrainDirSpecForm.GetTrainingPath(rm, true);
            if (dpath == null) return;

            string timestamp = BCIApplication.TimeStamp;

            //Logfile
            var cfile = ConsoleCapture.StartConsoleFile(Path.Combine(dpath, appname + "_" + timestamp + "_Main.log"));
            AmpContainer.StartAll();
            AmpContainer.StartRecord(dpath, appname + "_" + timestamp);

            if (_haptic_knob != null)
            {
                _haptic_knob.SendClient(GameCommand.StartGame, 
                    Path.GetFullPath(Path.Combine(dpath, appname + "_" + timestamp + "_HK.log")));
            }

            if (cfg_waitstart_time < 0) cfg_waitstart_time = 0;
            _evt_StopProc.WaitOne(cfg_waitstart_time, false);

            WaitHandle[] evts = new WaitHandle[] { _evt_StopProc, _evt_ActionDone };

            if (_proc != null) { // classification feedback
                Console.WriteLine("Start BCI calibraion, bias = {0}.", cfg_score_bias);

                //_proc.SetFeedbackHandler(new MIProcessor.MIOutout(ReceiveMIScore));
                //_proc.SetReadingShift(cfg_shift_score);
                fldScoreViewer.Initialize();
                fldScoreViewer.SetAccuTimes(1000, 0);
            }

            //if (_erd != null) {
            //    _erd.SetFeedbackHandler((ERDProcessor.erd_fb_output)delegate(Image img)
            //    {
            //        //erdFeedback.ShowImage(img);
            //    });
            //    //_erd.SetReadingShift(500); //_cfg_shift_score
            //}

            ConsoleOutForm.ShowWindow();

            int ntasks = _MIType.Length;

            MIAction[] tasks = new MIAction[ntasks];
            int[] keys = new int[ntasks];
            Random rnd = new Random();
            Amplifier amp = AmpContainer.GetAmplifier(0);

            fb_form.Reset();
            fb_form.Show_Progress(cfg_rest_time, cfg_beep_before_rest_end, _evt_StopProc);

            int ntrials = cfg_num_trials;

            //double acc = 0;
            //double bias = 0;
            //miFeedback.CalculateBestBias(out acc, out bias);

            // 20110775: new FLD algo parameters:
            int tmin = 2 * amp.header.samplingrate;
            int tmax = 4 * amp.header.samplingrate;

            for (int itrial = 0; itrial < ntrials; itrial++) {
                if (_evt_StopProc.WaitOne(0)) break;

                if (!amp.IsAlive) {
                    MessageBox.Show("Amplifer is not run!");
                    _evt_StopProc.Set();
                    break;
                }

                // generate tasks and randomize it
                for (int itask = 0; itask < ntasks; itask++) {
                    tasks[itask] = (MIAction)(itask + 1);
                    keys[itask] = rnd.Next(1000);
                }
                Array.Sort(keys, tasks);

                for (int itask = 0; itask < ntasks; itask++) {
                    ctask = tasks[itask];
                    char task_cfg = _MIType[ctask - MIAction.Left];
                    if (task_cfg == 'N') continue; // MI-Action not defined

                    Keys task_key = Keys.None;
                    if (task_cfg == 'K') task_key = _keyvals[(int)ctask - 1];

                    this.Invoke((Action)delegate()
                    {
                        toolStripStatusMsg.Text = string.Format("Trail {0}/{1}: task = {2}",
                            itrial + 1, ntrials, ctask);
                        toolStripStatusFeedBack.Text = fldScoreViewer.GetAccMessage();
                    });

                    // 20110510: For PM-calibration, only Open/Close
                    RehabAction act = _action;
                    if (act == RehabAction.Alternative)
                    {
                        act = (((itask / _repeats) & 1) == 0) ? RehabAction.OpenClose : RehabAction.Rotate;
                    }

                    if (cfg_pm_action != RehabAction.None) {
                        act = cfg_pm_action;
                    }

                    //Prepare
                    fb_form.SetMIStep(MI_STEP.MI_Prepare);
                    Console.WriteLine("Stimcode={0}", cfg_stim_prep);
                    AmpContainer.SendAll(cfg_stim_prep);
                    if (_hand_cue == null)
                    {
                        ShowTask(MI_STEP.MI_Prepare);
                    }
                    else
                    {
                        // Fade In
                        _hand_cue.SendClient(GameCommand.MI_Cue, (int) RehabCommand.FadeIn, (int)act, (int)ctask);
                        if (ctask != MIAction.Idle) {
                            _hand_cue.SendClient(GameCommand.MI_Cue, (int)RehabCommand.PassiveMovement, (int)act);
                        }
                    }

                    if (_proc == null && _haptic_knob != null && ctask != MIAction.Idle)
                    {
                        // Haptic Knob Prepare: Set to start position
                        //_haptic_knob.SendClient(GameCommand.HapticKnob, (int) RehabCommand.Reset, (int) act);
                    }

                    if (_evt_StopProc.WaitOne(cfg_prep_time, false)) {
                        break;
                    }
 
                    // Show task
                    fb_form.SetMIStep(MI_STEP.MI_Cue);
                    Console.WriteLine("STIM_TASK:{0}", cfg_stim_task_offset + ctask);
                    AmpContainer.SendAll(cfg_stim_task_offset + (int)ctask);
                    _cue_label = (int)ctask;

                    if (task_key != Keys.None) {
                        // Wait for keyboard input
                        _evt_ActionDone.Reset();
                        actionWait = WaitAction.Keyboard;

                        // Wait for action done, not timeout here
                        while (WaitHandle.WaitAny(evts, 50, false) == WaitHandle.WaitTimeout) {
                            this.Activate();
                        }
                    }
                    else { // System wait for a short time for motor imagery
                        if (cfg_img_wait > 0) _evt_StopProc.WaitOne(cfg_img_wait, false);
                    }

                    // Action
                    fb_form.SetMIStep(MI_STEP.MI_Action);

                    _hand_cue.SendClient(GameCommand.MI_Cue, (int)RehabCommand.FadeOut);

                    // Passive Movement
                    if (_proc == null && _haptic_knob != null && ctask != MIAction.Idle)
                    {
                        // activate passive movement
                        _haptic_knob.SendClient(GameCommand.HapticKnob, (int) RehabCommand.PassiveMovement, (int)act);
                    }

                    if (_hand_cue == null) {
                        _task_color = Color.Yellow;
                        this.Invalidate();
                    } else {
                        _hand_cue.SendClient(GameCommand.MI_Cue, (int)RehabCommand.Stop, (int)act);
                    }

                    if (_proc == null) { // no feedback, data collection only
                        if (_evt_StopProc.WaitOne(cfg_act_time, false)) break;
                    } else {
                        _score_cls = -1;
                        fldScoreViewer.StartAccumulate(GetClassNo(_cue_label), dl_getscore); //tmin, tmax, 

                        // wait for score out
                        while (_score_cls == -1 && amp.IsAlive) {
                            if (_evt_StopProc.WaitOne(15, false)) break;
                        }
                        if (_evt_StopProc.WaitOne(0)) break;

                        //int si = 0;
                        //avt_start = 5;
                        //_score_pos = 0;

                        // start score feedback
                        //_proc.SetReadingPos();

                        //if (_erd != null) {
                        //    // start erd feedback as soon as possbile
                        //    _erd.SetReadingPos();
                        //}

                        //Thread thd_erd = null;

                        //_score_cls = -1;
                        //while (si < cfg_num_score + avt_start) {
                        //    if (si == avt_start) fb_form.StartAccuScore();
                        //    else if (si > avt_start) {
                        //        if (fb_form.ResultReached()) break;
                        //    }

                        //    bool idl = true;
                        //    if (_proc.Process()) {
                        //        si++;
                        //        idl = false;
                        //    }

                        //    //if (_erd != null && (thd_erd == null || !thd_erd.IsAlive)) {
                        //    //    thd_erd = new Thread((ThreadStart)delegate()
                        //    //    {
                        //    //        _erd.Process();
                        //    //    });
                        //    //    thd_erd.Start();
                        //    //}

                        //    int wt = idl ? 50 : 0;
                        //    if (_evt_StopProc.WaitOne(wt)) break;
                        //}

                        //if (_score_label < 0) {
                        //    // get from avg score
                        //    double[] mi_score = MI_Score;
                        //    int imax = 0;
                        //    for (int i = 1; i < mi_score.Length; i++) {
                        //        if (mi_score[i] > mi_score[imax]) imax = i;
                        //    }

                        //    _score_label = imax;
                        //    _score_conf = mi_score[imax];
                        //}
                        //_score_cls = fb_form.GetDetection(out _score_conf); // 0 / 1
                    }

                    _task_color = Color.White;
                    if (_hand_cue == null) {
                        ShowTask(MI_STEP.MI_None);
                    } else {
                        // Fade out
                        //_hand_cue.SendClient(GameCommand.MI_Cue, (int) RehabCommand.FadeOut);
                    }

                    if (_haptic_knob != null)
                    {
                        // stop passive movement
                        _haptic_knob.SendClient(GameCommand.HapticKnob, (int) RehabCommand.WaitForFinish);
                    }

                    actionWait = WaitAction.None;
                    _evt_ActionDone.Reset();

                    // feedback time
                    if (_proc != null) {
                        //Thread.Sleep(cfg_shift_score);
                        

                        //miFeedback.ShowScore(_cue_label, _score_label, _score_conf, true);
                        //miFeedback.CalculateBestBias(out acc, out bias);
                        //Invoke((Action)delegate()
                        //{
                        //    labelBestScore.Text = string.Format("Best score = {0:0.##} @ Bias = {1:0.##}", acc, bias);
                        //    btnSetBias.Tag = bias;
                        //});

                        Console.WriteLine("Class={0} conf={1}", _score_cls, _score_conf);
                        fb_form.Show_SmileyFace(_score_cls == GetClassNo(_cue_label)?
                            FB_Form.SmileyFace.Sucess : FB_Form.SmileyFace.Fail);

                        if (_evt_StopProc.WaitOne(cfg_fb_time, false)) break;
                    }

                    if (_pause) {
                        AmpContainer.SendAll(255);
                        MessageBox.Show("Paused. Click OK to continue.", "Data Collection Paused");
                        AmpContainer.SendAll(254);
                        SetPause(false);
                    }

                    // Rest
                    fb_form.SetMIStep(MI_STEP.MI_Rest);
                    Console.WriteLine("STIM_REST:{0}", cfg_stim_rest);
                    AmpContainer.SendAll(cfg_stim_rest);
                    fb_form.Show_Progress(cfg_rest_time, cfg_beep_before_rest_end, _evt_StopProc);
                    fb_form.Show_SmileyFace(0);

                    fb_form.Reset();

                    if (!AmpContainer.AllAlive) {
                        _evt_StopProc.Set();
                        break;
                    }
                }
            }

            //_result_msg = "Training session " + appname + " Finished!";
            if (_proc != null && fldScoreViewer.TotalTrials > 0) { // && miFeedback.TotalTrials > 0
                //string result_msg = string.Format("Accuracy = {0:0.##}% ({1}/{2}) with Bias @ {3}.",
                //    miFeedback.CorrectTrials * 100.0 / miFeedback.TotalTrials,
                //    miFeedback.CorrectTrials, miFeedback.TotalTrials,
                //    miFeedback.MIBias);

                string result_msg = fldScoreViewer.GetAccMessage();
                fb_form.Set_Result(result_msg);
                Console.WriteLine(result_msg);

                double acc = 0;
                double bias = fldScoreViewer.Calculate_BestBias(out acc);
                Console.WriteLine("Best acc = {0} @ Bias = {1}", acc, bias);
            } else if (!_evt_StopProc.WaitOne(0, false)) {
                // finished, add training dir to training list
                BCIApplication.AddTrainingPath(dpath);
                fb_form.Set_Result("Motor imagery data collection is done.");
            }

            Invalidate();

            AmpContainer.StopRecord();
            cfile.EndLogFile();
        }

        /// <summary>
        /// Get class no
        /// </summary>
        /// <param name="task_no">1 to 5.</param>
        /// <returns>starting from 0.</returns>
        private int GetClassNo(int task_no)
        {
            return _task_string.IndexOf(MITaskConfig.TASK_IDLIST[task_no - 1]);
        }

        private int GetTaskNo(int cls_no)
        {
            return (MITaskConfig.TASK_IDLIST.IndexOf(_task_string[cls_no])) + 1;
        }

        /// <summary>
        /// User Rehabilitation Result
        /// </summary>
        int user_rehab_result = -1;

        /// <summary>
        /// Rehab control with Haptic Knob
        /// </summary>
        private void ExeBCIHKRehab()
        {
            if (_haptic_knob == null || _proc == null) {
                MessageBox.Show("Wrong Setting for Starting Rehabilitation (processor or habtic knob problem)!");
                return;
            }

            MIAction ctask = MIAction.None;
            for (int itask = 0; itask < _MIType.Length; itask++) {
                if (_MIType[itask] != 'N') {
                    ctask = (MIAction) (itask + 1);
                    break;
                }
            }
            if (ctask != MIAction.Left && ctask != MIAction.Right) {
                MessageBox.Show("Wrong task configuration!");
            }
            char task_cfg = _MIType[(int)ctask - 1];

            ResManager rm = BCIApplication.SysResource;

            // specify app name
            string mfn = rm.GetConfigValue(MIConstDef.MIProcess, MIConstDef.ModelPattern);
            string appname = "Model";
            if (!string.IsNullOrEmpty(mfn))
            {
                mfn = Path.GetFileNameWithoutExtension(mfn);
                int idx = mfn.IndexOf('_');
                if (idx > 0)
                {
                    appname = mfn.Substring(0, idx);
                }
            }
            appname += "_" + ctask + "_" + _action.ToString();

            BCIApplication.SetProtocolName(rm, appname); // used to define data directory
            Invoke((Action)delegate()
            {
                LoadConfig(rm);
            });

            string dpath = TestDirSpecForm.GetProcPath(rm, true, true);
            if (dpath == null) return;

            string timestamp = BCIApplication.TimeStamp;

            //Logfile
            var cfile = ConsoleCapture.StartConsoleFile(Path.Combine(dpath, appname + "_" + timestamp + "_Main.log"));
            AmpContainer.StartAll();
            AmpContainer.StartRecord(dpath, appname + "_" + timestamp);

            Console.WriteLine("Start BCI HapticKnob Rehabilitation, bias = {0}.", cfg_score_bias);

            if (_haptic_knob != null)
            {
                _haptic_knob.SendClient(GameCommand.StartGame,
                    Path.GetFullPath(Path.Combine(dpath, appname + "_" + timestamp + "_HK.log")));
            }

            if (cfg_waitstart_time < 0) cfg_waitstart_time = 0;
            else _evt_StopProc.WaitOne(cfg_waitstart_time, false);

            WaitHandle[] evts = new WaitHandle[] { _evt_StopProc, _evt_ActionDone };

            //_proc.SetFeedbackHandler(new MIProcessor.MIOutout(ReceiveMIScore));
            //_proc.SetReadingShift(cfg_shift_score);

            fldScoreViewer.Initialize();
            fldScoreViewer.SetAccuTimes(1000, 1000);

            //if (_erd != null)
            //{
            //    _erd.SetFeedbackHandler((ERDProcessor.erd_fb_output)delegate(Image img)
            //    {
            //        erdFeedback.ShowImage(img);
            //    });
            //    _erd.SetReadingShift(500); //_cfg_shift_score
            //}

            ConsoleOutForm.ShowWindow();

            Amplifier amp = AmpContainer.GetAmplifier(0);

            // show rest progress
            fb_form.Reset();
            fb_form.Show_Progress(cfg_rest_time, cfg_beep_before_rest_end, _evt_StopProc);

            int ntrials = cfg_num_rehab;
            int itrial = 0;
            int iFail = 0;

            int nTotal = 0;
            int nFail = 0;

            // Rehab task, Open/Close or Clockwise/Anticlockwise
            int[] rtask = { 0, 0 };
            // statistics for rehab action: Open/Close/Clockwise/AntiClockwise
            int[] rreach = { 0, 0, 0, 0 };

            int tmin = 2 * amp.header.samplingrate;
            int tmax = 4 * amp.header.samplingrate;

            while (!_evt_StopProc.WaitOne(0) && itrial < ntrials) {
                if (!amp.IsAlive)
                {
                    MessageBox.Show("Amplifer is not run!");
                    _evt_StopProc.Set();
                    break;
                }

                RehabAction act = _action;
                if (act == RehabAction.Alternative)
                {
                    act = (((itrial / _repeats) & 1) == 0) ? RehabAction.OpenClose : RehabAction.Rotate;
                }
                Console.WriteLine("Action={0}", act);

                Keys task_key = Keys.None;
                if (task_cfg == 'K') task_key = _keyvals[(int)ctask - 1];

                this.Invoke((Action)delegate()
                {
                    if (nFail == 0)
                    {
                        toolStripStatusMsg.Text = string.Format("Rehab Trail {0}/{1}: task={2}", itrial + 1, ntrials, ctask);
                    }
                    else
                    {
                        toolStripStatusMsg.Text = string.Format("Rehab Trail {0}/{1}: task={2}, Fail={3}",
                            itrial + 1, ntrials, ctask, iFail);
                    }
                    toolStripStatusFeedBack.Text = fldScoreViewer.GetAccMessage();
                });

                //Prepare
                fb_form.SetMIStep(MI_STEP.MI_Prepare);
                AmpContainer.SendAll(cfg_stim_prep);
                Console.WriteLine("Stimcode={0}", cfg_stim_prep);
                if (_hand_cue == null)
                {
                    ShowTask(MI_STEP.MI_Prepare);
                    if (_evt_StopProc.WaitOne(cfg_prep_time, false)) {
                        break;
                    }
                }
                else
                {
                    // Task FadeIn 1-5
                    _hand_cue.SendClient(GameCommand.MI_Cue, (int)RehabCommand.FadeIn, (int)act, (int)ctask);
                    // Cue for hand movement
                    _hand_cue.SendClient(GameCommand.MI_Cue, (int)RehabCommand.PassiveMovement, (int)act);
                    _evt_StopProc.WaitOne(cfg_prep_time, false);
                }

                // Show task (cue)
                fb_form.SetMIStep(MI_STEP.MI_Cue);
                AmpContainer.SendAll(cfg_stim_task_offset + (int)ctask);
                Console.WriteLine("STIM_TASK:{0}", cfg_stim_task_offset + ctask);

                _cue_label = (int)ctask;
                //miFeedback.ShowScore(_cue_label, -1, 0, false);

                // keyboard input if have
                if (task_key != Keys.None)
                {
                    // Wait for keyboard input
                    _evt_ActionDone.Reset();
                    actionWait = WaitAction.Keyboard;

                    // Wait for action done - no time limit
                    while (WaitHandle.WaitAny(evts, 50, false) == WaitHandle.WaitTimeout)
                    {
                        this.Activate();
                    }
                }
                else
                { // No keyboard input
                    if (cfg_img_wait > 0) _evt_StopProc.WaitOne(cfg_img_wait, false);
                }

                // Action
                fb_form.SetMIStep(MI_STEP.MI_Action);
                if (_hand_cue == null)
                {
                    _task_color = Color.Yellow;
                    this.Invalidate();
                }
                else
                {
                    // Stop animation
                    _hand_cue.SendClient(GameCommand.MI_Cue, (int)RehabCommand.FadeOut);
                    _hand_cue.SendClient(GameCommand.MI_Cue, (int)RehabCommand.Stop, (int)act);
                }
                // 20110715: new accumulating algo
                _score_cls = -1;
                fldScoreViewer.StartAccumulate(GetClassNo(_cue_label), dl_getscore); //tmin, tmax, 

                // wait for score out
                while (_score_cls == -1 && amp.IsAlive) {
                    if (_evt_StopProc.WaitOne(15, false)) break;
                }
                if (_evt_StopProc.WaitOne(0)) break;

                //int si = 0;
                //avt_start = 5;
                //_score_pos = 0;
                //_proc.SetReadingPos();

                //if (_erd != null) {
                //    _erd.SetReadingPos();
                //}

                // ERD in seperate thread - just for display.
                //Thread thd_erd = null;

                //while (si < cfg_num_score + avt_start) {
                //    if (si == avt_start) fb_form.StartAccuScore();
                //    else if (si >= avt_start) {
                //        if (fb_form.ResultReached()) break;
                //    }

                //    bool idl = true;
                //    if (_proc.Process()) {
                //        si++;
                //        idl = false;
                //    }

                //    //if (_erd != null && (thd_erd == null || !thd_erd.IsAlive)) {
                //    //    thd_erd = new Thread((ThreadStart)delegate()
                //    //    {
                //    //        _erd.Process();
                //    //    });
                //    //    thd_erd.Start();
                //    //}

                //    int wt = idl ? 50 : 0;
                //    if (_evt_StopProc.WaitOne(wt)) break;
                //}
                //_score_cls = fb_form.GetDetection(out _score_conf);

                _task_color = Color.White;
                if (_hand_cue == null)
                {
                    ShowTask(MI_STEP.MI_Action, ctask);
                } else {
                    // Fade out
                    //_hand_cue.SendClient(GameCommand.MI_Cue, (int)RehabCommand.FadeOut);
                }

                actionWait = WaitAction.None;
                _evt_ActionDone.Reset();

                //Thread.Sleep(cfg_shift_score);

                // Show finial result
                //miFeedback.ShowScore(_cue_label, _score_label, _score_conf, true);
                Console.WriteLine("Class={0} conf={1}", _score_cls, _score_conf);

                //miFeedback.CalculateBestBias(out acc, out bias);
                //Invoke((Action)delegate()
                //{
                //    labelBestScore.Text = string.Format("Best score = {0:0.##} @ Bias = {1:0.##}", acc, bias);
                //    btnSetBias.Tag = bias;
                //});

                int rtime = cfg_rest_time;
                nTotal++;
                if (_score_cls != GetClassNo(_cue_label))
                {
                    iFail++;
                    nFail++;

                    this.Invoke((Action)delegate()
                    {
                        toolStripStatusMsg.Text = string.Format("Rehab Trail {0}/{1}: task = {2} Fail={3}",
                            itrial + 1, ntrials, ctask, iFail);
                    });

                    if (iFail < cfg_fail_tries) {
                        if (cfg_fail_rest >= 0) rtime = cfg_fail_rest;
                        fb_form.Set_Message("Try again ...");
                    }
                }

                if (_score_cls == GetClassNo(_cue_label) || (cfg_fail_tries > 0 && iFail >= cfg_fail_tries)) {
                    fb_form.Show_SmileyFace(iFail >= cfg_fail_tries ? FB_Form.SmileyFace.Encourage : FB_Form.SmileyFace.Sucess);

                    AmpContainer.SendAll(cfg_stim_rehab);
                    Console.WriteLine("STIM_Rehab:{0}", cfg_stim_rehab);

                    fb_form.Set_Message("Rehab ... ");

                    itrial++;

                    // start rehabilitation
                    _haptic_knob.SendClient(GameCommand.HapticKnob,
                        (int)RehabCommand.AssistiveMovement, // for rehabilitation
                        (int)act // 1 = OpenClose, 2 = Rotate
                        );

                    _evt_StopProc.WaitOne(cfg_act_time, false);

                    // wait for action to finish.
                    _evt_ActionDone.Reset();
                    user_rehab_result = -1;
                    actionWait = WaitAction.Hapticknob;
                    _haptic_knob.SendClient(GameCommand.HapticKnob, (int)RehabCommand.WaitForFinish);
                    if (WaitHandle.WaitAny(evts) == 1) {
                        Console.WriteLine("Rehab {0}: {1}", act, user_rehab_result);
                        fb_form.Set_Message(null);

                        rtask[(int)act - 1]++;

                        // defined in HapticKnobCtrlForm.cs
                        //[Flags]
                        //public enum HKPosition {
                        //  None = 0,
                        //  Close = 1,
                        //  Open = 2,
                        //  Rotate1 = 4,
                        //  Rotate2 = 8,
                        //  OpenClose = Close | Open,
                        //  RotateBoth = Rotate1 | Rotate2,
                        //  All = OpenClose | RotateBoth
                        //}
                        if ((user_rehab_result & 1) == 1) // Close
                            rreach[1]++;

                        if ((user_rehab_result & 2) == 2) //Open
                            rreach[0]++;

                        if ((user_rehab_result & 4) == 4) // Anti-Clockwise
                            rreach[3]++;

                        if ((user_rehab_result & 8) == 8) // clockwise
                            rreach[2]++;

                        fb_form.SetUserReachPostions(user_rehab_result);
                    }
                } else {
                    fb_form.Show_SmileyFace(FB_Form.SmileyFace.Fail);
                }

                // Clear Feedback
                //Invoke((Action)delegate()
                //{
                //    miFeedback.ClearDisp();
                //    if (_erd != null) {
                //        erdFeedback.ClearDisp();
                //    }
                //});

                if (_pause) {
                    AmpContainer.SendAll(255);
                    MessageBox.Show("Paused. Click OK to continue.", "Haptic Knob Rehabilitation Paused");
                    AmpContainer.SendAll(254);
                    SetPause(false);
                }

                // Rest
                if (rtime > 0) {
                    fb_form.SetMIStep(MI_STEP.MI_Rest);
                    AmpContainer.SendAll(cfg_stim_rest);
                    Console.WriteLine("STIM_REST:{0}", cfg_stim_rest);
                    fb_form.Show_Progress(rtime, cfg_beep_before_rest_end, _evt_StopProc);
                }            

                fb_form.Reset();
                fb_form.Show_SmileyFace(0);
                fb_form.Set_Message(null);

                if (!AmpContainer.AllAlive)
                {
                    _evt_StopProc.Set();
                    break;
                }

                if (_score_cls == GetClassNo(_cue_label) || iFail >= cfg_fail_tries) {
                    // Clear haptic knob window display
                    _haptic_knob.SendClient(GameCommand.HapticKnob, (int)RehabCommand.None);
                    iFail = 0;
                }
            }

            if (fldScoreViewer.TotalTrials > 0) {
                string result_msg;
                //= string.Format("Accuracy = {0:0.##}% ({1}/{2}).",
                //    miFeedback.CorrectTrials * 100.0 / miFeedback.TotalTrials,
                //    miFeedback.CorrectTrials, miFeedback.TotalTrials);

                //// fb_form.Set_ResultMessage(result_msg);
                //Console.WriteLine(result_msg);
                result_msg = fldScoreViewer.GetAccMessage();

                if (_action == RehabAction.OpenClose || _action == RehabAction.Alternative) {
                    result_msg += string.Format("\r\nUser reached: Open={0}, Close={1}",
                        rreach[0], rreach[1]);
                }
                if (_action == RehabAction.Rotate || _action == RehabAction.Alternative) {
                    result_msg += string.Format("\r\nUser reached: Clockwise={0}, Anticlockwise={1}",
                        rreach[2], rreach[3]);
                }
                Console.WriteLine(result_msg);
                fb_form.Set_Result(result_msg);
            }

            AmpContainer.StopRecord();
            cfile.EndLogFile();
        }
        /// <summary>
        /// Rehab control with Haptic Knob
        /// </summary>
        /// 
        private void ExeHKRehabOnly()
        {
            if (_haptic_knob == null) {
                MessageBox.Show("Wrong Setting for Starting Rehabilitation (processor or habtic knob problem)!");
                return;
            }

            MIAction ctask = MIAction.None;
            for (int itask = 0; itask < _MIType.Length; itask++) {
                if (_MIType[itask] != 'N') {
                    ctask = (MIAction)(itask + 1);
                    break;
                }
            }
            if (ctask != MIAction.Left && ctask != MIAction.Right) {
                MessageBox.Show("Wrong task configuration!");
            }
            char task_cfg = _MIType[(int)ctask - 1];

            ResManager rm = BCIApplication.SysResource;

            // specify app name
            string appname = "HK_" + ctask + "_" + _action.ToString();

            BCIApplication.SetProtocolName(rm, appname); // used to define data directory
            Invoke((Action)delegate()
            {
                LoadConfig(rm);
            });

            string dpath = TestDirSpecForm.GetProcPath(rm, true, true);
            if (dpath == null) return;

            string timestamp = BCIApplication.TimeStamp;

            //Logfile
            var cfile = ConsoleCapture.StartConsoleFile(Path.Combine(dpath, appname + "_" + timestamp + "_Main.log"));
            AmpContainer.StartAll();
            AmpContainer.StartRecord(dpath, appname + "_" + timestamp);

            _haptic_knob.SendClient(GameCommand.StartGame,
                Path.GetFullPath(Path.Combine(dpath, appname + "_" + timestamp + "_HK.log")));

            if (cfg_waitstart_time < 0) cfg_waitstart_time = 0;
            else _evt_StopProc.WaitOne(cfg_waitstart_time, false);

            WaitHandle[] evts = new WaitHandle[] { _evt_StopProc, _evt_ActionDone };

            Amplifier amp = AmpContainer.GetAmplifier(0);

            // show rest progress
            fb_form.Reset();
            fb_form.Show_Progress(cfg_rest_time, cfg_beep_before_rest_end, _evt_StopProc);

            int ntrials = cfg_num_rehab;
            int itrial = 0;

            // Rehab task, Open/Close or Clockwise/Anticlockwise
            int[] rtask = { 0, 0 };

            // statistics for rehab action: Open/Close/Clockwise/AntiClockwise
            int[] rreach = { 0, 0, 0, 0 };

            int tmin = 2 * amp.header.samplingrate;
            int tmax = 4 * amp.header.samplingrate;

            while (!_evt_StopProc.WaitOne(0) && itrial < ntrials) {
                if (!amp.IsAlive) {
                    MessageBox.Show("Amplifer is not run!");
                    _evt_StopProc.Set();
                    break;
                }

                RehabAction act = _action;
                if (act == RehabAction.Alternative) {
                    act = (((itrial / _repeats) & 1) == 0) ? RehabAction.OpenClose : RehabAction.Rotate;
                }
                Console.WriteLine("Action={0}", act);

                Keys task_key = Keys.None;
                if (task_cfg == 'K') task_key = _keyvals[(int)ctask - 1];

                this.Invoke((Action)delegate()
                {
                    toolStripStatusMsg.Text = string.Format("Rehab Trail {0}/{1}: task={2}", itrial + 1, ntrials, ctask);
                });

                //Prepare
                fb_form.SetMIStep(MI_STEP.MI_Prepare);
                AmpContainer.SendAll(cfg_stim_prep);
                Console.WriteLine("Stimcode={0}", cfg_stim_prep);
                if (_hand_cue == null) {
                    ShowTask(MI_STEP.MI_Prepare);
                } else {
                    // Task FadeIn 1-5
                    _hand_cue.SendClient(GameCommand.MI_Cue, (int)RehabCommand.FadeIn, (int)act, (int)ctask);
                    // Cue for hand movement
                    _hand_cue.SendClient(GameCommand.MI_Cue, (int)RehabCommand.PassiveMovement, (int)act);
                }
                if (_evt_StopProc.WaitOne(cfg_prep_time, false)) {
                    break;
                }

                // Show task (cue)
                fb_form.SetMIStep(MI_STEP.MI_Cue);
                AmpContainer.SendAll(cfg_stim_task_offset + (int)ctask);
                Console.WriteLine("STIM_TASK:{0}", cfg_stim_task_offset + ctask);

                _cue_label = (int)ctask;

                // Action
                fb_form.SetMIStep(MI_STEP.MI_Action);
                if (_hand_cue == null) {
                    _task_color = Color.Yellow;
                    this.Invalidate();
                } else {
                    // Stop animation
                    _hand_cue.SendClient(GameCommand.MI_Cue, (int)RehabCommand.FadeOut);
                    _hand_cue.SendClient(GameCommand.MI_Cue, (int)RehabCommand.Stop, (int)act);
                }

                if (_evt_StopProc.WaitOne(0)) break;

                _task_color = Color.White;
                if (_hand_cue == null) {
                    ShowTask(MI_STEP.MI_Action, ctask);
                }

                actionWait = WaitAction.None;
                _evt_ActionDone.Reset();

                AmpContainer.SendAll(cfg_stim_rehab);
                Console.WriteLine("STIM_Rehab:{0}", cfg_stim_rehab);

                fb_form.Set_Message("Start rehabilitation ... ");

                itrial++;

                // start rehabilitation
                _haptic_knob.SendClient(GameCommand.HapticKnob,
                    (int)RehabCommand.AssistiveMovement, // for rehabilitation
                    (int)act // 1 = OpenClose, 2 = Rotate
                    );

                // wait for action to finish.
                _evt_ActionDone.Reset();
                user_rehab_result = -1;
                actionWait = WaitAction.Hapticknob;
                _haptic_knob.SendClient(GameCommand.HapticKnob, (int)RehabCommand.WaitForFinish);

                if (WaitHandle.WaitAny(evts) == 1) {
                    Console.WriteLine("Rehab {0}: {1}", act, user_rehab_result);
                    fb_form.Set_Message(null);

                    rtask[(int)act - 1]++;

                    if ((user_rehab_result & 1) == 1) // Close
                        rreach[1]++;

                    if ((user_rehab_result & 2) == 2) //Open
                        rreach[0]++;

                    if ((user_rehab_result & 4) == 4) // Anti-Clockwise
                        rreach[3]++;

                    if ((user_rehab_result & 8) == 8) // clockwise
                        rreach[2]++;

                    fb_form.SetUserReachPostions(user_rehab_result);
                }

                if (_pause) {
                    AmpContainer.SendAll(255);
                    MessageBox.Show("Paused. Click OK to continue.", "Haptic Knob Rehabilitation Paused");
                    AmpContainer.SendAll(254);
                    SetPause(false);
                }

                // Rest
                fb_form.SetMIStep(MI_STEP.MI_Rest);
                AmpContainer.SendAll(cfg_stim_rest);
                Console.WriteLine("STIM_REST:{0}", cfg_stim_rest);
                fb_form.Show_Progress(cfg_rest_time, cfg_beep_before_rest_end, _evt_StopProc);

                fb_form.Reset();
                fb_form.Show_SmileyFace(0);
                fb_form.Set_Message(null);

                if (!AmpContainer.AllAlive) {
                    _evt_StopProc.Set();
                    break;
                }

                _haptic_knob.SendClient(GameCommand.HapticKnob, (int)RehabCommand.None);
            }

            if (!_evt_StopProc.WaitOne(0, false)) {
                //if (miFeedback.TotalTrials > 0)
                {
                    string result_msg;
                    result_msg = "Rehab finished.\r\n";
                    if (_action == RehabAction.OpenClose || _action == RehabAction.Alternative) {
                        result_msg += string.Format("Rehab Task: OpenClose. User reached: Open={0}, Close={1}\r\n",
                            rreach[0], rreach[1]);
                    }
                    if (_action == RehabAction.Rotate || _action == RehabAction.Alternative) {
                        result_msg += string.Format("Rehab Task: Rotation. User reached: Clockwise={0}, Anticlockwise={1}",
                            rreach[2], rreach[3]);
                    }
                    Console.WriteLine(result_msg);
                    fb_form.Set_Result(result_msg);
                }

                Invalidate();
            }

            AmpContainer.StopRecord();
            cfile.EndLogFile();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WMCopyData.WM_COPYDATA) {
                Copy_Data mdata = (Copy_Data)m.GetLParam(typeof(Copy_Data));
                user_rehab_result = (int) mdata.wdata;
                
                if (actionWait == WaitAction.Hapticknob) { //  && m.WParam == IntPtr.Zero
                    _evt_ActionDone.Set();
                }
            }
            try {
                base.WndProc(ref m);
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }

        private void toolStripConfig_Click(object sender, EventArgs e)
        {
            BCIApplication.SysResource.ShowDialog();
        }

        private int numkeydown = 0;
        private void KeyDown_Process(int key_type)
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

            if (actionWait != WaitAction.Keyboard) {
                Console.WriteLine("Key Down: {0}", key_type);
                return;
            }
        }

        private void KeyUp_Process(MIAction key_type)
        {
            numkeydown = 0;

            AmpContainer.SendAll(cfg_stim_click_offset + (int) key_type);
            Console.WriteLine("STIM_CLICK:{0}", cfg_stim_click_offset + 1);

            if (actionWait != WaitAction.Keyboard) {
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

        private void MIFLD_GetScore(double score)
        {
            if (InvokeRequired) {
                BeginInvoke(dl_getscore, score);
                return;
            }

            _score_cls = (score >= 0) ? 0 : 1;
            _score_conf = score;

            toolStripStatusFeedBack.Text = fldScoreViewer.GetFeedbackMessage();

            if (_mode == 0) {
                double acc = 0;
                double bias = fldScoreViewer.Calculate_BestBias(out acc);

                labelBestScore.Text = string.Format("Best score = {0:0.##} @ Bias = {1:0.##}", acc, bias);
                btnSetBias.Tag = bias;

                if (cbAutoAjust.Checked) {
                    btnSetBias.PerformClick();
                }
            }
        }

        private Action<double> dl_getscore = null;
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Alt) {
                if (e.KeyCode == Keys.I) {
                    bool visible = !toolStripStatusFeedBack.Visible;
                    toolStripStatusFeedBack.Visible = visible;
                    e.Handled = true;
                    return;
                } else if (e.KeyCode == Keys.S) {
                    fb_form.SetTestSuccess(TestKeyAction.Sucess);
                    e.Handled = true;
                    return;
                } else if (e.KeyCode == Keys.F) {
                    fb_form.SetTestSuccess(TestKeyAction.Fail);
                    e.Handled = true;
                    return;
                } else if (e.KeyCode == Keys.A) {
                    toolStripStatusFeedBack.Text = null;
                    fb_form.SetMIStep(MI_STEP.MI_Rest);

                    int cno = 0;
                    if (_mode == 0) {
                        cno = new Random().Next(2);
                    }

                    _cue_label = GetTaskNo(cno);

                    toolStripStatusMsg.Text = "Trial: " + (fldScoreViewer.TotalTrials + 1) + " " + (MIAction) _cue_label;

                    fldScoreViewer.StartAccumulate(cno, dl_getscore); //500, 1000, 
                    e.Handled = true;
                    return;
                }
            }

            int key_type = 0;

            for (int i = 0; i < _MIType.Length; i++) {
                if (_MIType[i] == 'K' && e.KeyCode == _keyvals[i]) {
                    key_type = i + 1;
                    break;
                }
            }

            if (key_type == 0) return;

            e.Handled = true;
            KeyDown_Process(key_type);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            MIAction key_type = 0;
            if (e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.Oemcomma) key_type = MIAction.Left;
            else if (e.KeyCode == Keys.Right || e.KeyCode == Keys.OemPeriod
                || e.KeyCode == Keys.Enter) key_type = MIAction.Right;
            if (key_type == 0) return;
            e.Handled = true;
            KeyUp_Process(key_type);
        }

        private void toolStripStop_Click(object sender, EventArgs e)
        {
            _evt_StopProc.Set();
        }

        int _mode = 0;
        BCIProcessor _proc = null;
        //BCIProcessor _erd = null;
        WMCopyData _hand_cue = null;
        WMCopyData _haptic_knob = null;
        RehabAction _action = RehabAction.None;
        int _repeats = 1;

        internal void SetProfile(int mode, WMCopyData mi_cue, WMCopyData hk, BCIProcessor mi_proc, BCIProcessor erd_proc,
            RehabAction act, int rpt)
        {
            _repeats = rpt;
            SetProfile(mode, mi_cue, hk, mi_proc, erd_proc, act);
        }

        /// <summary>
        /// Motor Imagery Profile Setting
        /// </summary>
        /// <param name="">Mode: 0 for data collection (2 classes) 1 for rehab (1 class only) </param>
        /// <param name="mi_cue">External tool to show motor imagery cue</param>
        /// <param name="hk">External Haptic Knob Contrl tool to do passive movment and rehabilitation</param>
        /// <param name="mi_proc">MI Processor to calculate MI score for feedback</param>
        /// <param name="erd_proc">ERD Processor</param>
        internal void SetProfile(int mode, WMCopyData mi_cue, WMCopyData hk, BCIProcessor mi_proc, BCIProcessor erd_proc, RehabAction act)
        {
            _mode = mode;
            _hand_cue = mi_cue;
            _haptic_knob = hk;
            _proc = mi_proc;
            //_erd = erd_proc;
            _action = act;

            StringBuilder sb = new StringBuilder();
            sb.Append(mode == 0? " Calibration": " Hapticknob Rehab");

            if (_proc != null) {
                //miFeedback.Configurate(_MIType, toolStripStatusFeedBack);
                //miFeedback.Reset();
                sb.Append(" with Feedback");

                if (_proc is FLDProcessor) {
                    fldScoreViewer.SetProcessor(_proc as FLDProcessor);
                    fb_form.SetFeedbackType(_mode == 0 ? FB_Form.FBType.TwoClassBar : FB_Form.FBType.RehabOpen);
                    fb_form.SetProcessor(_proc as FLDProcessor, cfg_act_time);
                    fldScoreViewer.dl_upd_score = fb_form.GetAccuDelegate() as Action<double, int>;
                    BCILib.EngineProc.FLDModel mdl = (_proc as FLDProcessor).GetFLDModel();
                    tbThreshold.Text = string.Format("Threshold = {0:#.##}/{1}, range = {2:#.##}, {3:#.##}",
                        mdl.thr, mdl.atime, mdl.fld_r[0], mdl.fld_r[1]);
                    //SetBias(0, true, false);
                } else {
                    fldScoreViewer.Visible = false;
                }

                if (mode == 1) {
                    panelBestBias.Visible = false;
                }
            } else {
                panelBiasSetting.Visible = false;
                fldScoreViewer.Visible = false;
                groupBoxFBConfig.Visible = false;
            }
            
            if (hk != null && mode == 0) {
                sb.Append(" Passive Movement");
            }

            //miFeedback.IsVisible = _proc != null;
            //erdFeedback.IsVisible = _erd != null;

            this.Text += sb.ToString();

            if (_hand_cue != null) _hand_cue.SetThisWindow(Handle);
            if (_haptic_knob != null) _haptic_knob.SetThisWindow(Handle);
        }

        private void CloseCtrlPort()
        {
            if (_haptic_knob != null) {
                _haptic_knob = null;
            }
        }

        private void MIHapticKnobForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseCtrlPort();

            fldScoreViewer.SetProcessor(null);

            if (fb_form != null) {
                fb_form.SetProcessor(null, cfg_act_time);
                fb_form.Close();
            }
        }

        bool _pause = false;

        private void toolStripBtnPause_Click(object sender, EventArgs e)
        {
            SetPause(true);
        }

        private void SetPause(bool pause)
        {
            if (InvokeRequired) {
                Invoke((Action<bool>)((p) => SetPause(p)), pause);
                return;
            }

            _pause = pause;
            toolStripBtnPause.Enabled = !_pause;
        }

        private void trackBarBias_ValueChanged(object sender, EventArgs e)
        {
            if (trackBarBias.Tag == null) {
                SetBias(trackBarBias.Value / 10.0, false, true);
            }
        }

        private void SetBias(double nv, bool UpdateScrollBar, bool Save)
        {
            if (UpdateScrollBar) {
                int iv = (int)(nv * 10);

                if (iv < trackBarBias.Minimum) iv = trackBarBias.Minimum;
                else if (iv > trackBarBias.Maximum) iv = trackBarBias.Maximum;

                if (iv != trackBarBias.Value) {
                    object ov = trackBarBias.Tag;
                    trackBarBias.Tag = 1;
                    trackBarBias.Value = iv;
                    trackBarBias.Tag = ov;
                }
            }

            //miFeedback.MIBias = nv;
            fldScoreViewer.SetBias(nv);

            textBoxBias.Text = nv.ToString("0.###");
            toolStripStatusFeedBack.Text = fldScoreViewer.GetFeedbackMessage();

            Console.WriteLine("Bias set to {0} from cfg {1}", nv, cfg_score_bias);
            cfg_score_bias = nv;
            if (Save) {
                SaveConfig();
            }
        }

        static int test = 0;
        private void toolStripTest_Click(object sender, EventArgs e)
        {
            test++;
            if ((test & 1) == 1) {
                fb_form.Set_HandCueWindow(_hand_cue);
                fb_form.Set_Progress(10);
                fb_form.Show_SmileyFace((FB_Form.SmileyFace) (1 + test / 3 % 3));
                _hand_cue.SendClient(GameCommand.MI_Cue, (int)RehabCommand.FadeIn,
                    (int)RehabAction.OpenClose, (int)MIAction.Left);
                fb_form.Set_Message("Processing");
                fb_form.Set_Result("Display Result");
            } else {
                fb_form.Set_Progress(0);
                fb_form.Show_SmileyFace(0);
                _hand_cue.SendClient(GameCommand.StopGame);
                Thread.Sleep(1000);
                this.BringToFront();
                this.Update();
                fb_form.Set_Message(null);
                fb_form.Set_Result(null);
            }
        }

        private void btnSetBias_Click(object sender, EventArgs e)
        {
            object tag = btnSetBias.Tag;
            if (tag is Double) {
                SetBias((double)tag, true, true);
            }
        }

        private void cbFLDScore_Click(object sender, EventArgs e)
        {
            fb_form.SetFeedbackFlag(FB_FLAG.FLDScore, cbFLDScore.Checked);
            SaveConfig();
        }

        private void cbSmiley_Click(object sender, EventArgs e)
        {
            fb_form.SetFeedbackFlag(FB_FLAG.SmileyFace, cbSmiley.Checked);
            SaveConfig();
        }

        private void cbDispMsg_Click(object sender, EventArgs e)
        {
            fb_form.SetFeedbackFlag(FB_FLAG.Message, cbDispMsg.Checked);
            SaveConfig();
        }

        private void SaveConfig()
        {
            cfg_feedback_flag = fb_form.GetFeedbackFlag();
            ResManager res = BCIApplication.SysResource;
            res.SetConfigValue(MIConstDef.MITest, "FeedbackFlag", cfg_feedback_flag);
            res.SetConfigValue(MIConstDef.MITest, "SaveBias", cfg_save_bias);
            res.SetConfigValue(MIConstDef.MITest, "ScoreBias", cfg_score_bias);
            res.SaveFile();
        }

        private bool OnKeyboardInput(Keys key, uint scan_code, uint flag)
        {
            if (panelBiasSetting.Visible)
            {
                if (key == Keys.F6)
                {
                    SetBias(cfg_score_bias - 0.1, true, true);
                }
                else if (key == Keys.F5)
                {
                    SetBias(cfg_score_bias + 0.1, true, true);
                }
            }

            return false;
        }

        private void btnBiasReset_Click(object sender, EventArgs e)
        {
            SetBias(0, true, true);
        }

        private void cbSaveBias_CheckedChanged(object sender, EventArgs e)
        {
            cfg_save_bias = cbSaveBias.Checked;
            SaveConfig();
            if (cfg_save_bias) {
                SetBias(cfg_score_bias, true, false);
            }
        }
    }
}
