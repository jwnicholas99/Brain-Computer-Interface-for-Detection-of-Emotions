using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using BCILib.Amp;
using BCILib.Util;
using BCILib.App;
using System.Threading;
using System.IO;
using System.Linq;
using BCILib.EngineProc;
using System.Globalization;
using System.Text.RegularExpressions;

namespace BCILib.MotorImagery.BCILowerLimb
{
    public partial class BCILowerLimbRehab : BCILib.App.BCIAppForm
    {
        public BCILowerLimbRehab()
        {
            InitializeComponent();
        }

        const string BCIRecordFile = "BCIRecord.txt";

        protected override void OnLoad(EventArgs e)
        {
            Initialize();
            base.OnLoad(e);

            ResManager rm = BCIApplication.SysResource;
            LoadConfig(rm);
            Amplifier amp = AmpContainer.GetAmplifier(0);
            fld_accumulator.LoadModel(rm, amp, (string)textSelChannels.Tag);
            //btnStartRehab.Enabled = 
            //    (fld_accumulator.Processor != null && fld_accumulator.Processor.NumChannelUsed.ToString() == textSelChannels.Text);

            fb_form = new ExternalRehabGame(Handle, "BCILowerLimbGame");

            var tsi = toolStrip1.Items.Add("MOgreFB");
            tsi.Click += (s, ex) =>
            {
                (fb_form as ExternalRehabGame).StartGameIfNotFound();
            };
            tsi.PerformClick();

            // 2012 - Display expr records
            string[] rlines = null;
            var ufn = Path.Combine(BCIApplication.UserPath, BCIRecordFile);
            if (File.Exists(ufn)) rlines = File.ReadAllLines(ufn);

            if (rlines != null) {
                var tag = rlines.Select(x => long.Parse(x.Substring(0, x.IndexOf('=')), NumberStyles.HexNumber)).ToList();
                rtbRecords.Tag = tag;
                rtbRecords.Lines = rlines.Select((x, xi) =>
                    DateTime.FromBinary(tag[xi]).ToString("g", DateTimeFormatInfo.InvariantInfo) + " - " + x.Substring(x.IndexOf('=') + 1)).ToArray();
            }

            rtbRecords.SelectionStart = rtbRecords.TextLength;
            rtbRecords.SelectionLength = 0;
            rtbRecords.ScrollToCaret();
            UpdateNewRecords(false);
        }

        private void UpdateNewRecords(bool highlight)
        {
            List<long> tag = rtbRecords.Tag as List<long>;
            if (tag == null) tag = new List<long>();
            List<string> nrecs = new List<string>();

            var dates = Directory.GetDirectories(BCIApplication.UserPath)
                .Select(x => Path.GetFileName(x))
                .Where(x => x.Length == 8 && x.All(c => char.IsDigit(c)))
                .OrderBy(x => x)
                .ToList();
            if (dates.Count == 0) return;

            rtbRecords.Focus();
            rtbRecords.Select();

            // update gui
            Regex rfn = new Regex(@".*_(\d{8}-\d{6}).*");
            foreach (var dstr in dates) {
                var dpath = Path.Combine(BCIApplication.UserPath, dstr);
                var slist = Directory.GetFiles(dpath, "*.log", SearchOption.AllDirectories)
                    .Select(x =>
                    {
                        Match m =rfn.Match(Path.GetFileName(x));
                        if (m.Success) {
                            return new
                            {
                                date = DateTime.ParseExact(m.Groups[1].Value, BCIApplication.FMT_TIMESTAMP, CultureInfo.InvariantCulture).ToBinary(),
                                path = x
                            };
                        } else {
                            return null;
                        }
                    }).Where(x => x != null)
                    .OrderBy(x => x.date)
                    .ToList();

                foreach (var log_fn in slist) {
                    long key = log_fn.date;
                    DateTime ts = DateTime.FromBinary(key);
                    if (tag.Contains(key)) continue;

                    string rmsg = ReadRecord(log_fn.path);
                    if (string.IsNullOrEmpty(rmsg)) continue;

                    tag.Add(key);
                    nrecs.Add(key.ToString("X") + "=" + rmsg);

                    if (rtbRecords.TextLength > 0) rtbRecords.AppendText("\r\n");

                    rtbRecords.SelectionStart = rtbRecords.TextLength;
                    rtbRecords.SelectionLength = 0;
                    if (highlight) {
                        rtbRecords.SelectionColor = Color.Blue;
                    }
                    rtbRecords.SelectedText = DateTime.FromBinary(key).ToString("g", DateTimeFormatInfo.InvariantInfo)
                        + " - " + rmsg;
                }
            }

            if (nrecs.Count > 0) {
                rtbRecords.Tag = tag;
                var ufn = Path.Combine(BCIApplication.UserPath, BCIRecordFile);
                using (var fs = File.AppendText(ufn)) {
                    foreach (var rline in nrecs) {
                        fs.WriteLine(rline);
                    }
                }
            }

            btnShowTasks.Select();
        }

        private string ReadRecord(string logfn)
        {
            if (logfn.IndexOf(@"\DataTraining\") >= 0) {
                int nact = 0;
                int nidl = 0;
                using (var sr = File.OpenText(logfn)) {
                    string line = null;
                    while ((line = sr.ReadLine()) != null) {
                        if (line.IndexOf("STIM_TASK:125") >= 0) nidl++;
                        else if (line.IndexOf("STIM_TASK:124") >= 0) nact++;
                    }
                }
                return string.Format("DataCollection, trials: action={0}, idle={1}", nact, nidl);
            }

            string key = Path.GetFileName(logfn).Split(new [] {'_'}, 2).FirstOrDefault();
            var rlines = File.ReadAllLines(logfn);
            var saccu = rlines.LastOrDefault(x => x.StartsWith("Total/Correct="));

            if (!string.IsNullOrEmpty(saccu)) return key + ", " + saccu;

            int ntact = 0, ntidl = 0, nfact = 0, nfidl = 0;
            int label = -1, cls = -1;
            foreach (var line in rlines) {
                if (line.StartsWith("STIM_TASK:")) {
                    label = int.Parse(line.Substring(10)) - 124;
                } else if (line.StartsWith("Class=")) {
                    cls = line[6] - '0';
                    if (label == 0) {
                        if (cls == 0) ntact++;
                        else nfidl++;
                    } else if (label == 1) {
                        if (cls == 0) nfact++;
                        else ntidl++;
                    }
                    label = -1;
                }
            }
            int nc = ntact + nfact + ntidl + nfidl;
            if (nc == 0) {
                //var dpath = Path.GetDirectoryName(logfn);
                //try {
                //    Directory.Delete(dpath);
                //    Console.WriteLine("Directory {0} deleted.", dpath);
                //} catch {
                //    Console.WriteLine("Cannot delete {0}", dpath);
                //}
                return null;
            }

            return string.Format("{0}, calculated: {1} / {2}, acc={3:P}", key, nc, ntact + ntidl, (ntact + ntidl) / (double) nc);
        }

        private void Initialize()
        {
            AmpContainer.AmpChanged += () => SetCurrentAmplifier();
        }

        private void SetCurrentAmplifier()
        {
            if (AmpContainer.Count > 0) {
                labelAmplifier.Text = AmpContainer.GetAmplifier(0).DevName;
                if (!string.IsNullOrEmpty(textSelChannels.Text)) {
                    int nsel = int.Parse(textSelChannels.Text);
                    SelAmpChannel cfg = new SelAmpChannel();
                    cfg.ShowAmplifier();
                    cfg.SelectedString = (string)textSelChannels.Tag;
                    if (cfg.SelectedNum != nsel) {
                        cfg.SelectedString = MIConstDef.DefChannels;
                        if (cfg.SelectedNum != nsel) {
                            cfg.SelectedNum = nsel;
                        }
                        textSelChannels.Tag = cfg.SelectedString;
                    }
                }
            }
        }

        /// <summary>
        /// Load config and fill GUI items
        /// </summary>
        /// <param name="rm"></param>
        protected override void LoadConfig(ResManager rm)
        {
            if (InvokeRequired) {
                Invoke((Action<ResManager>)(x => LoadConfig(x)), rm);
                return;
            }

            string line = MIConstDef.DefChannels;
            rm.GetConfigValue(MIConstDef.ModelTrainingSetting, MIConstDef.Train_MTS_Channels, ref line);
            if (!string.IsNullOrEmpty(line)) {
                string[] slist = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                int n = 0;
                if (slist != null) n = slist.Length;
                textSelChannels.Text = n.ToString();
                textSelChannels.Tag = line;
            }

            fld_accumulator.LoadConfig(rm);

            SetCurrentAmplifier();

            line = rm.GetConfigValue(MIConstDef.Train, "TrainVersion");
            if (string.IsNullOrEmpty(line) || int.Parse(line) < cfg_train_version) {
                rm.SetConfigValue(MIConstDef.Train, "TrainVersion", cfg_train_version);
                rm.SetConfigValue(MIConstDef.Train, MIConstDef.Number_Trials, cfg_num_trials);
                rm.SetConfigValue(MIConstDef.Train, MIConstDef.Time_Prepare, cfg_time_prep);
                rm.SetConfigValue(MIConstDef.Train, MIConstDef.Time_Cue, cfg_time_cue);
                rm.SetConfigValue(MIConstDef.Train, MIConstDef.Time_Action, cfg_time_act);
                rm.SetConfigValue(MIConstDef.Train, MIConstDef.Time_Rest, cfg_time_rest);

                rm.SetConfigValue(MIConstDef.MITest, MIConstDef.MI_Num_Rehab, cfg_num_rehab);
                rm.SetConfigValue(MIConstDef.MITest, MIConstDef.MI_Num_Supervise, cfg_num_supervise);
            } else {
                rm.GetConfigValue(MIConstDef.Train, MIConstDef.Number_Trials, ref cfg_num_trials);
                rm.GetConfigValue(MIConstDef.Train, MIConstDef.Time_Prepare, ref cfg_time_prep);
                rm.GetConfigValue(MIConstDef.Train, MIConstDef.Time_Cue, ref cfg_time_cue);
                rm.GetConfigValue(MIConstDef.Train, MIConstDef.Time_Action, ref cfg_time_act);
                rm.GetConfigValue(MIConstDef.Train, MIConstDef.Time_Rest, ref cfg_time_rest);

                rm.GetConfigValue(MIConstDef.MITest, MIConstDef.MI_Num_Rehab, ref cfg_num_rehab);
                rm.GetConfigValue(MIConstDef.MITest, MIConstDef.MI_Num_Supervise, ref cfg_num_supervise);
            }
            rm.GetConfigValue(MIConstDef.MITest, "Rehab_Min_Walk", ref cfg_min_walk);
            rm.GetConfigValue(MIConstDef.MITest, "Rehab_Max_Walk", ref cfg_max_walk);

            rm.SaveIfChanged();
        }

        private void buttonAmpSel_Click(object sender, EventArgs e)
        {
            SelAmpChannel cfg = new SelAmpChannel();
            cfg.ShowAmplifier();

            string str_sch_list = (string)textSelChannels.Tag;
            if (str_sch_list == null) str_sch_list = MIConstDef.DefChannels;
            cfg.SelectedString = str_sch_list;

            if (UserCtrlForm.ShowCtrl(cfg) == DialogResult.OK) {
                string amp_str = cfg.SelectedAmplifier;
                Amplifier amp = AmpContainer.GetAmplifier(amp_str);
                if (amp != null) amp_str = amp.DevName;

                textSelChannels.Tag = cfg.SelectedString;
                textSelChannels.Text = cfg.SelectedNum.ToString();

                ResManager rm = BCIApplication.SysResource;
                rm.SetConfigValue(MIConstDef.ModelTrainingSetting, MIConstDef.Train_MTS_Channels, textSelChannels.Tag);
                rm.SaveIfChanged();
            }
        }

        FeedbackInterface fb_form = null;

        private void StartCalibrate()
        {
            if (AmpContainer.Count <= 0) {
                MessageBox.Show("Please select amplifer first.", "Error");
                return;
            }

            //if (radioSimpleCalib.Checked) {
            //    if (fb_form != null && !(fb_form is SimpleDataCollectionForm)) {
            //        fb_form.Close();
            //        fb_form = null;
            //    }
            //    if (fb_form == null)
            //        fb_form = new SimpleDataCollectionForm();
            //} else {
            //if (fb_form != null && !(fb_form is ExternalRehabGame)) {
            //    fb_form.Close();
            //    fb_form = null;
            //}
            //if (fb_form == null)
            //    fb_form = new ExternalRehabGame(Handle, "BCILowerLimbGame");
            //}

            LoadConfig();
            if (!fb_form.StartGame(evt_proc, BCITask.Training, cfg_num_trials)) return;

            gbAmplifier.Enabled = false;
            ControlBox = false;

            btnShowTasks.Text = "Stop";
            bgworker.RunWorkerAsync();
        }

        ManualResetEvent evt_proc = new ManualResetEvent(false);

        private void bgworker_DoWork(object sender, DoWorkEventArgs e)
        {
            evt_proc.Reset();
            if (e.Argument == null) {
                ExeDataCollection();
            } else {
                ExeBCIRehab();
            }
        }

        int cfg_num_trials = 40;
        int cfg_num_rehab = 40;
        int cfg_num_supervise = 20;

        private int cfg_stim_prep = 100;
        private int cfg_stim_task_offset = 120; // + task cue starting
        private int cfg_stim_rest = 199;

        // control to rewrite old one
        private int cfg_train_version = 2;

        private int cfg_time_prep = 2000;
        private int cfg_time_cue = 2000;
        private int cfg_time_act = 6000;
        private int cfg_time_rest = 6000;

        private int cfg_min_walk = 2000;
        private int cfg_max_walk = 6000;

        private void ExeDataCollection()
        {
            TrainTask bci_task = (TrainTask)this.bci_task;
            fb_form.SetMIStep(MI_STEP.MI_None, 0);

            ResManager rm = BCIApplication.SysResource;
            Amplifier amp = AmpContainer.GetAmplifier(0);
            Random rnd = new Random();

            string appname = null;
            string dpath = null;

            appname = fb_form is SimpleDataCollectionForm? "MISimple_LowLimb" : "MIMOgre_LowLimb";
            BCIApplication.SetProtocolName(rm, appname);
            LoadConfig(rm);
            dpath = TrainDirSpecForm.GetTrainingPath(rm, true);

            if (dpath == null) return;

            bci_task.datapath = dpath;

            string timestamp = BCIApplication.TimeStamp;
            bci_task.ctime = DateTime.ParseExact(timestamp, BCIApplication.FMT_TIMESTAMP, CultureInfo.InvariantCulture);

            //Logfile
            var cfile = ConsoleCapture.StartConsoleFile(Path.Combine(dpath, appname + "_" + timestamp + ".log"));
            AmpContainer.StartAll();
            AmpContainer.StartRecord(dpath, appname + "_" + timestamp);

            bci_task.ntrials = cfg_num_trials;
            bci_task.nclasses = 2;

            // Create randon seqence
            MIAction[] tall = new MIAction[cfg_num_trials];
            var tvar = new[] {MIAction.Feet, MIAction.Idle};
            for (int ti = 0; ti < cfg_num_trials; ti++) {
                tall[ti] = tvar[ti & 1];
            }
            tall = tall.OrderBy(x => rnd.NextDouble()).ToArray();

            // Rest
            fb_form.SetMIStep(MI_STEP.MI_Rest, cfg_time_rest);
            evt_proc.WaitOne(cfg_time_rest, false);

            int itrial = 0;
            for (; itrial < cfg_num_trials; itrial++) {
                if (evt_proc.WaitOne(0)) break;

                if (!amp.IsAlive) {
                    MessageBox.Show("Amplifer stopped!");
                    evt_proc.Set();
                    break;
                }

                var task = tall[itrial];
                int stim = cfg_stim_task_offset + (int)task;
                string msg = string.Format("Trail {0}/{1}: {2}", itrial + 1, cfg_num_trials, task);
                BCIApplication.LogMessage(msg);
                this.Invoke((Action)delegate()
                {
                    statusMessage.Text = msg;
                });

                fb_form.StartTrial(task, itrial);

                // Prepare
                if (cfg_time_prep > 0) {
                    fb_form.SetMIStep(MI_STEP.MI_Prepare, cfg_time_prep);
                    amp.SendStimCode(cfg_stim_prep);
                    Console.WriteLine("STIM_TASK:{0}", cfg_stim_prep);
                    if (evt_proc.WaitOne(cfg_time_prep, false)) break;
                }

                // Cue
                fb_form.SetMIStep(MI_STEP.MI_Cue, cfg_time_cue);
                amp.SendStimCode(stim);
                Console.WriteLine("STIM_TASK:{0}", stim);

                if (evt_proc.WaitOne(cfg_time_cue, false)) break;

                //Action
                fb_form.SetMIStep(MI_STEP.MI_Action, cfg_time_act);
                stim += 20;
                amp.SendStimCode(stim); // action
                Console.WriteLine("STIM_TASK:{0}", stim);

                if (cfg_time_act > 0 && evt_proc.WaitOne(cfg_time_act)) break;

                if (Paused) {
                    AmpContainer.SendAll(255);
                    MessageBox.Show("Paused. Click OK to continue.", "Haptic Knob Rehabilitation Paused");
                    AmpContainer.SendAll(254); // added 20120808
                    Paused = false;
                }

                // Rest
                if (cfg_time_rest > 0) {
                    fb_form.SetMIStep(MI_STEP.MI_Rest, cfg_time_rest);
                    amp.SendStimCode(cfg_stim_rest);
                    Console.WriteLine("STIM_TASK:{0}", cfg_stim_rest);
                    Console.WriteLine("Rest time = {0}", cfg_time_rest);
                    if (evt_proc.WaitOne(cfg_time_rest, false)) break;
                }
            }
            bci_task.ntrials = itrial + 1;

            fb_form.SetMIStep(MI_STEP.MI_None, 0);

            if (!evt_proc.WaitOne(0, false)) {
                // finished, add training dir to training list
                BCIApplication.AddTrainingPath(dpath);
                string msg = "Training session " + appname + " Finished!";
                MessageBox.Show(msg);
                Console.WriteLine(msg);
                bci_task.ftime = DateTime.Now;
            }

            AmpContainer.StopRecord();
            cfile.EndLogFile();
        }

        private void bgworker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (bci_task.ftime.HasValue) {
                BCISchedule.SaveUserRecord(bci_task);
                UpdateNewRecords(true);
            }

            gbAmplifier.Enabled = true;
            btnShowTasks.Text = "Show Tasks";
            ControlBox = true;
            fb_form.Close();
            contextBCIRecords.Enabled = true;
        }

        private void buttonTrainModel_Click(object sender, EventArgs e)
        {
            this.Hide();
            MITrainMTSModelForm dlg = new MITrainMTSModelForm();
            List<int> ar = new List<int>();
            string tasktr = "FI";
            for (int i = 0; i < tasktr.Length; i++) {
                int idx = MITaskConfig.TASK_IDLIST.IndexOf(tasktr[i]);
                if (idx >= 0) {
                    ar.Add(cfg_stim_task_offset + idx + 1);
                }
            }
            dlg.SetMIClassLabels(ar.ToArray());
            dlg.ShowDialog();
            this.Show();

            Amplifier amp = AmpContainer.GetAmplifier(0);
            fld_accumulator.LoadModel(BCIApplication.SysResource, amp, (string)textSelChannels.Tag);
            //btnStartRehab.Enabled = 
            //    (fld_accumulator.Processor != null && fld_accumulator.Processor.NumChannelUsed.ToString() == textSelChannels.Text);
        }

        private void StartRehab()
        {
            if (bgworker.IsBusy) {
                // stop: set flag
                evt_proc.Set();
                return;
            }

            if (fld_accumulator.Processor == null) {
                MessageBox.Show("Processor not set! Please train model first.", "Error");
                return;
            }

            //if (fb_form != null && !(fb_form is ExternalRehabGame)) {
            //    fb_form.Close();
            //    fb_form = null;
            //}
            //if (fb_form == null)
            //    fb_form = new ExternalRehabGame(Handle, "BCILowerLimbGame");

            LoadConfig();
            fld_accumulator.GameType = bci_task.task;
            int ntrials = fld_accumulator.GameType == BCITask.Supervised ? cfg_num_supervise : cfg_num_rehab;
            if (!fb_form.StartGame(evt_proc, fld_accumulator.GameType, ntrials)) return;

            gbAmplifier.Enabled = false;
            ControlBox = false;

            btnShowTasks.Text = "Stop";
            bgworker.RunWorkerAsync(fld_accumulator.GameType);
        }

        private void ExeBCIRehab()
        {
            fb_form.SetMIStep(MI_STEP.MI_None, 0);

            ResManager rm = BCIApplication.SysResource;
            RehabTask bci_task = (RehabTask) this.bci_task;

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

            var gt = fld_accumulator.GameType;
            appname = gt.ToString() + "_" + appname;

            BCIApplication.SetProtocolName(rm, appname); // used to define data directory
            LoadConfig(rm);

            string dpath = TestDirSpecForm.GetProcPath(rm, true, true);
            if (dpath == null) return;

            bci_task.datapath = dpath;
            string timestamp = BCIApplication.TimeStamp;
            bci_task.ctime = DateTime.ParseExact(timestamp, BCIApplication.FMT_TIMESTAMP, CultureInfo.InvariantCulture);

            //Logfile
            var cfile = ConsoleCapture.StartConsoleFile(Path.Combine(dpath, appname + "_" + timestamp + ".log"));
            AmpContainer.StartAll();
            AmpContainer.StartRecord(dpath, appname + "_" + timestamp);

            Console.WriteLine("Start BCILowerlibRehab, bias = {0}.", fld_accumulator.cfg_score_bias);
            Console.WriteLine("Model=" + rm.GetConfigValue(MIConstDef.MIProcess, MIConstDef.ModelPattern));

            fld_accumulator.fldScoreViewer.Initialize();
            fld_accumulator.fldScoreViewer.SetAccuTimes(1000, 1000);

            ConsoleOutForm.ShowWindow();
            Amplifier amp = AmpContainer.GetAmplifier(0);

            // show rest progress
            fb_form.SetMIStep(MI_STEP.MI_Rest, cfg_time_rest);
            evt_proc.WaitOne(cfg_time_rest, false);

            int ntrials = 0;
            MIAction[] tall = new MIAction[0];
            var tasks = new[] { MIAction.Feet, MIAction.Idle };
            if (fld_accumulator.GameType == BCITask.Rehab) {
                ntrials = cfg_num_rehab;
                tall = Enumerable.Repeat(MIAction.Feet, ntrials).ToArray();
            } else if (fld_accumulator.GameType == BCITask.Supervised) {
                ntrials = cfg_num_supervise;
                Random rnd = new Random((int) (DateTime.Now.Ticks & int.MaxValue));
                tall = new MIAction[ntrials];
                for (int it = 0; it < ntrials; it++) tall[it] = tasks[it & 1];
                tall = tall.OrderBy(x => rnd.NextDouble()).ToArray();
            }

            int itrial = 0;
            while (!evt_proc.WaitOne(0) && itrial < ntrials) {
                if (!amp.IsAlive) {
                    MessageBox.Show("Amplifer is not run!");
                    evt_proc.Set();
                    break;
                }

                var ctask = tall[itrial];
                var msg = string.Format("Trail {0}/{1}: task={2}", itrial + 1, ntrials, ctask);
                BCIApplication.LogMessage(msg);
                this.Invoke((Action)delegate()
                {
                    statusMessage.Text = msg;
                });

                fb_form.StartTrial(ctask, itrial);

                //Prepare
                fb_form.SetMIStep(MI_STEP.MI_Prepare, cfg_time_prep);
                AmpContainer.SendAll(cfg_stim_prep);
                Console.WriteLine("Stimcode={0}", cfg_stim_prep);
                if (evt_proc.WaitOne(cfg_time_prep, false)) break;

                // Cue
                fb_form.SetMIStep(MI_STEP.MI_Cue, cfg_time_cue);
                AmpContainer.SendAll(cfg_stim_task_offset + (int)ctask);
                Console.WriteLine("STIM_TASK:{0}", cfg_stim_task_offset + ctask);
                if (evt_proc.WaitOne(cfg_time_cue, false)) break;

                int cue_label = (int)ctask;

                // Action
                fb_form.SetMIStep(MI_STEP.MI_Action, cfg_time_act);

                int score_cls = -1;
                double score_val = 0;
                fld_accumulator.fldScoreViewer.StartAccumulate(ctask == MIAction.Idle ? 1 : 0,
                    (Action<double>)(x =>
                    {
                        score_cls = x >= 0 ? 0 : 1;
                        score_val = x;
                        fld_accumulator.UpdateScore();
                    }));

                // wait for score out
                while (score_cls == -1 && amp.IsAlive) {
                    if (evt_proc.WaitOne(15, false)) break;
                }
                if (evt_proc.WaitOne(0)) break;

                fld_accumulator.FeedbackText = fld_accumulator.fldScoreViewer.GetFeedbackMessage();

                Console.WriteLine("Class={0} score={1}", score_cls, score_val);

                int tscore = (int)Math.Abs(score_val);
                if (tscore > 10) tscore = 10;
                int wt = (int)(cfg_min_walk + (cfg_max_walk - cfg_min_walk) * tscore / 10);
                Console.WriteLine("Feedback wait={0}", wt);

                var rtask = tasks[score_cls];

                int code = cfg_stim_task_offset + 30 + (int)rtask;
                AmpContainer.SendAll(code);
                Console.WriteLine("Send stimcode = {0}", code);

                if (rtask == ctask) {
                    fb_form.SetMIStep(MI_STEP.MI_Success, wt);
                    if (score_cls == 0) bci_task.tacc++;
                    else bci_task.trej++;
                } else {
                    fb_form.SetMIStep(MI_STEP.MI_Fail, wt);
                    if (score_cls == 0) bci_task.facc++;
                    else bci_task.frej++;
                }

                if (evt_proc.WaitOne(wt, false)) break;

                itrial++;

                if (Paused) {
                    AmpContainer.SendAll(255);
                    MessageBox.Show("Paused. Click OK to continue.", "Haptic Knob Rehabilitation Paused");
                    AmpContainer.SendAll(254); // added 20120808
                    Paused = false;
                }

                if (!AmpContainer.AllAlive) {
                    evt_proc.Set();
                    break;
                }

                // Rest
                fb_form.SetMIStep(MI_STEP.MI_Rest, cfg_time_rest);
                AmpContainer.SendAll(cfg_stim_rest);
                Console.WriteLine("STIM_REST:{0}", cfg_stim_rest);
                evt_proc.WaitOne(cfg_time_rest, false);
            }
            AmpContainer.StopRecord();

            if (fld_accumulator.fldScoreViewer.TotalTrials > 0) {
                string result_msg;
                result_msg = fld_accumulator.fldScoreViewer.GetAccMessage();
                Console.WriteLine(result_msg);

                bci_task.ftime = DateTime.Now;
            //} else {
            //    try {
            //        Directory.Delete(dpath, true);
            //    } catch { }
            }

            fb_form.SetMIStep(MI_STEP.MI_None, 0);

            cfile.EndLogFile();
        }

        TaskItem bci_task = null;
        private void btnShowTasks_Click(object sender, EventArgs e)
        {
            if (bgworker.IsBusy) {
                evt_proc.Set();
            } else {
                LoadConfig();
                StartTaskForm task_form = new StartTaskForm();
                bci_task = null;
                if (task_form.ShowTasks(cfg_num_trials, cfg_num_supervise, cfg_num_rehab) == DialogResult.OK) {
                    TaskItem ti = task_form.SelectedTask();
                    if (ti != null) {
                        if (ti.task == BCITask.Training) {
                            bci_task = new TrainTask(ti);
                        } else if (ti.task == BCITask.Rehab || ti.task == BCITask.Supervised) {
                            bci_task = new RehabTask(ti);
                        } else {
                            bci_task = new TaskItem(ti);
                        }
                    }
                }
                if (bci_task == null) return;

                if (AmpContainer.Count == 0) {
                    MessageBox.Show("No amplifier selected!", "Amplifier Error");
                    return;
                }

                Amplifier amp = AmpContainer.GetAmplifier(0);
                if (amp is FakeAmplifier) {
                    if (MessageBox.Show("Current amplifer is FakeAmplifier! Continue?", "Amplifier warning", MessageBoxButtons.OKCancel) != DialogResult.OK) {
                        return;
                    }
                }

                if (!amp.Start()) {
                    return;
                }

                contextBCIRecords.Enabled = false;
                if (bci_task.task == BCITask.Training) StartCalibrate();
                else if (bci_task.task == BCITask.Supervised || bci_task.task == BCITask.Rehab) StartRehab();
                else {
                    contextBCIRecords.Enabled = true;
                }
            }
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtbRecords.Clear();
            rtbRecords.Tag = null;
            var ufn = Path.Combine(BCIApplication.UserPath, BCIRecordFile);
            if (File.Exists(ufn)) File.Delete(ufn);
            UpdateNewRecords(false);
        }
    }
}
