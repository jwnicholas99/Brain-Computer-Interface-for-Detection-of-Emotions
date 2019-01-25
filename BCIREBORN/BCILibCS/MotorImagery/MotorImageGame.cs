using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Threading;
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;

using BCILib.App;
using BCILib.Amp;
using BCILib.Util;

namespace BCILib.MotorImagery
{
    internal partial class MotorImageGame : BCIAppForm
    {
        public MotorImageGame()
        {
            InitializeComponent();
            //_proc.SetFeedbackHandler(new Dlg_MIOutput(OutputMIScore));
            _proc.SetFeedbackHandler(new MIProcessor.MIOutout(OutputMIScore));
        }

        private void MotorImageGame_Load(object sender, EventArgs e)
        {
            if (!this.DesignMode) {
                ResManager rm = BCIApplication.SysResource;

                LoadConfig(rm);
                LoadModel(rm);

                // try to set selected channels as defined in training cfg
                selAmpChannel1.ShowAmplifier();
                selAmpChannel1.SelectedString = rm.GetConfigValue(MIConstDef.ModelTrainingSetting, MIConstDef.Train_MTS_Channels);
            }
            externalGameChooser1.LoadGameList();
        }

        private int[] _cls_ids = null;
        private char[] _cls_symbols = new char[] {
            '\x00DC', '\x00DE', '\xDD', '\xDF'
        };

        private bool LoadModel(ResManager rm)
        {
            string line = rm.GetConfigValue(MIConstDef.MIProcess, MIConstDef.ClassLabels);
            if (string.IsNullOrEmpty(line)) {
                return false;
            }
            string[] cls_labels = line.Split(',');
            _cls_ids = new int[cls_labels.Length];
            _miscore = new double[cls_labels.Length];
            for (int iclass = 0; iclass < cls_labels.Length; iclass++ ) {
                _cls_ids[iclass] = int.Parse(cls_labels[iclass]);
            }
            cfg_num_tasks = cls_labels.Length;

            line = rm.GetConfigValue(MIConstDef.MIProcess, MIConstDef.ModelPattern);
            if (string.IsNullOrEmpty(line)) {
                return false;
            }

            int nmodel = cls_labels.Length;
            if (nmodel == 2) nmodel = 1; // for two-classes, only one model is needed

            string[] mdl_names = new string[nmodel];

            for (int iclass = 0; iclass < mdl_names.Length; iclass++) {
                string fn = string.Format(line, iclass + 1);
                fn = Path.Combine("Model", fn);
                if (!File.Exists(fn)) return false;
                mdl_names[iclass] = fn;
            }

            if (!_proc.Initialize(mdl_names)) {
                Console.WriteLine("Processor Initialization failed");
                return false;
            }

            //_erd.Initialize(mdl_names[0]); // load model

            if (InvokeRequired) {
                this.Invoke((Action)delegate()
                {
                    textNumChannels.Text = _proc.NumChannelUsed.ToString();
                });
            }
            else {
                textNumChannels.Text = _proc.NumChannelUsed.ToString();
            }

            return true;
        }

        private void toolStripTraining_Click(object sender, EventArgs e)
        {
            this.Hide();
            MITrainMTSModelForm dlg = new MITrainMTSModelForm();
            ArrayList ar = new ArrayList();
            string tasktr = miCfg.TaskCfg;
            for (int i = 0; i < tasktr.Length; i++) {
                if (tasktr[i] != 'N') ar.Add(cfg_stim_task_offset + i + 1);
            }
            dlg.SetMIClassLabels((int[]) ar.ToArray(typeof(int)));
            dlg.ShowDialog();
            this.Show();
            LoadModel(BCIApplication.SysResource);
        }

        ManualResetEvent _evt_StopProc = new ManualResetEvent(false);
        MIProcessor _proc = new MIProcessor();

        WMCopyData _wc_client = null;

        private void buttonStart_Click(object sender, EventArgs e)
        {
            _wc_client = externalGameChooser1.LaunchGame();
            
            //if (_wc_client == null) return;
            
            buttonStart.Enabled = false;
            buttonStop.Enabled = true;

            LoadConfig(BCIApplication.SysResource);

            Thread thd = new Thread(new ThreadStart(RunTest));
            thd.Start();
        }

        private void RunTest()
        {
            _evt_StopProc.Reset();
            try {
                ExeTest();
            }
            catch (Exception e) {
                MessageBox.Show("Runtest! Error = " + e.Message);
            }

            try {
                this.Invoke((Action)delegate()
                {
                    buttonStart.Enabled = true;
                    buttonStop.Enabled = false;
                });
            }
            catch (Exception) { }
        }

        double[] _miscore = null;

        int _score_label = 0;
        double _score_conf = 0;
        int _score_pos = 0;

        void OutputMIScore(double[] conf)
        {
            Console.Write("Score {0} = ", _score_pos);
            for (int i = 0; i < conf.Length; i++) {
                Console.Write("{0} ", conf[i]);
                _miscore[i] += conf[i]; 
            }
            Console.WriteLine();
            _score_pos++;

            if (_score_pos >= _cfg_num_score) {
                for (int i = 0; i < _miscore.Length; i++) {
                    _miscore[i] /= _cfg_num_score;
                }

                int imax = 0;
                for (int i = 1; i < _miscore.Length; i++) {
                    if (_miscore[i] > _miscore[imax]) imax = i;
                }

                _score_label = _cls_ids[imax];
                _score_conf = _miscore[imax];
                panel_MIScore.Invalidate();

                SendClientCmd(PredictClass, _score_label);

                StringBuilder sb = new StringBuilder();
                sb.Append(_miscore[0]);
                for (int i = 1; i < _miscore.Length; i++) {
                    sb.AppendFormat(",{0}", _miscore[i]);
                }
                SendClientCmd(ConfidenceScore, _score_conf);

                Console.WriteLine("Class={0} conf={1}", _score_label, sb.ToString());

                _score_pos = 0;
                for (int i = 0; i < _miscore.Length; i++) {
                    _miscore[i] = 0;
                }
            }
        }

        private int cfg_num_tasks = 2;
        private int cfg_num_trials = 20;
        private int cfg_waitstart_time = 2000;
        private int cfg_pre_time = 1000;
        private int cfg_cue_time = 0;
        private int cfg_act_time = 4000;
        private int cfg_rest_time = 4000;

        private int cfg_stim_prep = 100;
        private int cfg_stim_task_offset = 120;
        private int cfg_stim_keydown_offset = 130;
        private int cfg_stim_click_offset = 150;
        private int cfg_stim_rest = 199;

        private int cfg_img_wait = 0;

        private int cfg_beep_after_rest = 1000;

        // testing specific parameters
        private int _cfg_num_score = 7;
        private int _cfg_shift_score = 500;

        protected override void LoadConfig(ResManager rm)
        {
            // read parameters from training
            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Number_tasks, ref cfg_num_tasks);
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

            string task_cfg = "IINN"; // N/I/K/T
            string key_cfg = null;

            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Task_Configure, ref task_cfg);
            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Key_Configure, ref key_cfg);

            // These two lines require this funtion to be run in the main thread!!!!!!!!!!
            miCfg.TaskString = task_cfg;
            miCfg.KeyCodeString = key_cfg;

            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.BeepAfterRest, ref cfg_beep_after_rest);

            // Specific testing parameters
            rm.GetConfigValue(MIConstDef.MITest, MIConstDef.MI_Num_Score, ref _cfg_num_score);
            rm.GetConfigValue(MIConstDef.MITest, MIConstDef.MI_Shift_Score, ref _cfg_shift_score);

            rm.SaveIfChanged();
        }

        //Queue<long> _qstim_time  = new Queue<long>();

        // commands sent to client
        const string StimCode = "StimCode";
        const string PredictClass = "PredictClass";
        const string ConfidenceScore = "ConfidenceScore";

        private void SendClientCmd(string cmd, object val)
        {
            if (_wc_client != null) _wc_client.SendCmdString("{0}, {1}", cmd, val);
        }

        //ERDProcessor _erd = new ERDProcessor();

        private void ExeTest()
        {

            // Testing data directory
            ResManager rm = BCIApplication.SysResource;
            //if (miCfg.Changed) SaveConfigure(rm);

            if (!LoadModel(rm)) {
                MessageBox.Show("Load model failed!");
                return;
            }

            // Application Name
            string APP_NAME = miCfg.TaskString;
            int nc = APP_NAME.Length;
            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Number_tasks, ref nc);
            //if (nc < APP_NAME.Length) {
            //    APP_NAME = APP_NAME.Substring(0, nc);
            //}
            rm.SetConfigValue(BCICfg.EEG, BCICfg.AppName, "MITesting_" + APP_NAME);

            string dpath = null;
            dpath = TestDirSpecForm.GetProcPath(rm, true);
            if (dpath == null) {
                if (TestDirSpecForm.dlg_result == DialogResult.Cancel) return;
            }

            string ampid1 = null;
            // get data info by invoking
            Invoke((Action)delegate
            {
                ampid1 = selAmpChannel1.SelectedAmplifier;
            });


            Amplifier amp = AmpContainer.GetAmplifier(ampid1);
            if (amp == null || !_proc.SetAmplifier(amp, selAmpChannel1.SelectedString)) {
                MessageBox.Show("Set amplifier error!");
                _evt_StopProc.Set();
            }
            //else {
            //    // processing all motor imagery tasks
            //    _proc.SetReadingCodes(_cfg_cue_offset * amp.header.samplingrate / 1000, 120, 121, 122, 123, 124);
            //}
            _proc.SetReadingShift(_cfg_shift_score);

            //_erd.SetAmplifier(amp, selAmpChannel1.SelectedString);
            //_erd.SetReadingShift(200); // Display interval

            if (!amp.IsAlive) amp.Start();

            ConsoleCapture.ConsoleFile cfile = null;

            if (dpath != null) {
                string timestamp = BCIApplication.TimeStamp;
                if (!Directory.Exists(dpath)) {
                    Directory.CreateDirectory(dpath);
                }

                cfile = ConsoleCapture.StartConsoleFile(Path.Combine(dpath,
                    string.Format("{0}_{1}.log", APP_NAME, timestamp)));

                amp.StartRecord(Path.Combine(dpath, string.Format("{0}_{1}.cnt", APP_NAME, timestamp)));
            }

            HRTimer.GetTimestamp();

            WaitHandle[] evts = new WaitHandle[] { _evt_StopProc }; //, _evt_ActionDone };

            uint beep_freq = 300;
            uint beep_time = 500; // milliseconds,

            if (cfg_beep_after_rest > 0) {
                //ShowProgress(cfg_rest_time, false);
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
                //ShowProgress(cfg_rest_time, true);
                if (cfg_rest_time > 0) _evt_StopProc.WaitOne(cfg_rest_time);
            }

            int[] tasks = new int[cfg_num_tasks];
            int[] keys = new int[cfg_num_tasks];
            Random rnd = new Random();
            for (int itrial = 0; itrial < cfg_num_trials; itrial++) {
                if (_evt_StopProc.WaitOne(0)) break;
                if (!amp.IsAlive) {
                    MessageBox.Show("Amplifer is not run!");
                    _evt_StopProc.Set();
                    break;
                }

                // generate tasks and randomize it
                for (int itask = 0; itask < cfg_num_tasks; itask++) {
                    //tasks[itask] = itask + 1;
					tasks[itask] = _cls_ids[itask] - cfg_stim_task_offset;
                    keys[itask] = rnd.Next(1000);
                }
                Array.Sort(keys, tasks);

                for (int itask = 0; itask < cfg_num_tasks; itask++) {
                    int ctask = tasks[itask];
                    this.Invoke((Action)delegate()
                    {
                        statusMessage.Text = string.Format("Trial {0}/{1}: task {2} = {3}",
                            itrial + 1, cfg_num_trials, itask + 1, (MIAction)(ctask + MIAction.Left - 1));
                    });

                    //Prepare
                    AmpContainer.SendAll(cfg_stim_prep);
                    Console.WriteLine("Stimcode={0}", cfg_stim_prep);
                    //ShowTask(MI_TASK.MI_Cross);
                    SendClientCmd(StimCode, cfg_stim_prep);

                    if (_evt_StopProc.WaitOne(cfg_pre_time, false)) {
                        break;
                    }
                    //ShowTask(MI_TASK.MI_None);

                    // Show task
                    AmpContainer.SendAll(cfg_stim_task_offset + ctask);
                    Console.WriteLine("STIM_TASK:{0}", cfg_stim_task_offset + ctask);
                    //ShowTask((MI_TASK)(ctask - 1 + MI_TASK.MI_Left));
                    SendClientCmd(StimCode, cfg_stim_task_offset + ctask);

                    //if ((ctask == 1 && cfg_left_click != 0) ||
                    //    (ctask == 2 && cfg_right_click != 0)) {
                    //    // Wait for keyboard input
                    //    //_evt_ActionDone.Reset();
                    //    //bActionWait = true;
                    //    //this.Activate();

                    //    // Wait for action done
                    //    while (WaitHandle.WaitAny(evts, 50, false) == WaitHandle.WaitTimeout) {
                    //        this.Activate();
                    //    }
                    //}
                    //else
                    { // No keyboard input
                        if (cfg_img_wait > 0) _evt_StopProc.WaitOne(cfg_img_wait, false);
                    }

                    //if (_evt_StopProc.WaitOne(cfg_act_time, false)) break;
                    //bActionWait = false;


                    //_evt_ActionDone.Reset();

                    // Here receiving result
                    _proc.SetReadingPos(_cfg_shift_score * _proc.Amplifier.header.samplingrate / 1000);

                    int si = 0;
                    //int vi = 0;
                    //int vn = 1 + (4500 - 2000) / 200;

                    //_erd.SetReadingPos(0);

                    while (si < _cfg_num_score) {
                        if (si < _cfg_num_score) {
                            if (_proc.Process()) {
                                si++;
                            }
                        }

                        //if (vi < vn) {
                        //    if (_erd.Process()) {
                        //        vi++;
                        //    }
                        //}

                        if (_evt_StopProc.WaitOne(10)) break;
                    }

                    // rest
                    AmpContainer.SendAll(cfg_stim_rest);
                    Console.WriteLine("STIM_REST:{0}", cfg_stim_rest);
                    SendClientCmd(StimCode, cfg_stim_rest);

                    if (cfg_beep_after_rest > 0) {
                        //ShowProgress(cfg_rest_time, false);
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
                        //ShowProgress(cfg_rest_time, true);
                        if (cfg_rest_time > 0) _evt_StopProc.WaitOne(cfg_rest_time);
                    }

                    if (!AmpContainer.AllAlive) {
                        _evt_StopProc.Set();
                        break;
                    }
                }
            }

            if (dpath != null) {
                amp.StopRecord();
                cfile.EndLogFile();
            }
        }

        private void panel_MIScore_Paint(object sender, PaintEventArgs e)
        {
            if (_cls_ids != null) {
                Graphics g = e.Graphics;
                Rectangle drt = panel_MIScore.ClientRectangle;

                Brush br = new SolidBrush(Color.FromArgb(190, Color.Red));
                int w = (int)(_score_conf * drt.Width);
                g.FillRectangle(br, 0, 0, w, drt.Height);

                int idx = _score_label - _cls_ids[0];
                if (idx >= 0 && idx < _cls_symbols.Length) {
                    labelResult.Text = new string(_cls_symbols[idx], 1);
                } else {
                    labelResult.Text = null;
                }
            }
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            _evt_StopProc.Set();
        }

        private void MotorImageGame_FormClosing(object sender, FormClosingEventArgs e)
        {
            _evt_StopProc.Set();
        }

        private void toolStripCrossArrow_Click(object sender, EventArgs e)
        {
            miCfg.SaveConfigure(BCIApplication.SysResource);

            MITrainCrossArrowForm frm = new MITrainCrossArrowForm();

            frm.TaskString = miCfg.TaskString;
            frm.KeyCodes = miCfg.KeyCodes;

            this.Hide();
            frm.ShowDialog();
            this.Show();
        }

        protected override void OnClosed(EventArgs e)
        {
            miCfg.SaveConfigure(BCIApplication.SysResource);
        }

        private void buttonStartTrain_Click(object sender, EventArgs e)
        {
            miCfg.SaveConfigure(BCIApplication.SysResource);

            PicLoader frm = new PicLoader();
            //frm.TaskString = miCfg.TaskString;
            //frm.KeyCodes = miCfg.KeyCodes;
            this.Hide();
            frm.ShowDialog();
            this.Show();
        }
    }
}
