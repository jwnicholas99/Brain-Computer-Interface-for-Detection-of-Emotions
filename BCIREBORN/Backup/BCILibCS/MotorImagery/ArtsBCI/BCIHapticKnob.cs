using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using BCILib.App;
using BCILib.Amp;
using BCILib.Util;
using System.IO;
using System.Collections;
using BCILibCS.Util;
using System.IO.Ports;
using System.Threading;
using BCILib.EngineProc;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace BCILib.MotorImagery.ArtsBCI
{
    internal partial class BCIHapticKnob : BCILib.App.BCIAppForm
    {
        public BCIHapticKnob()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            LoadAll();

            _haptic_knob = new WMCopyData(ExternalTools.WIN_HAPTIC_KNOB.ToString(), this.Handle);
            _hand_cue = new WMCopyData(ExternalTools.WIN_HANDCUE01.ToString(), this.Handle);

            cmbHKCommand.Items.AddRange(new string[] {
                RehabCommand.None.ToString(),
                RehabCommand.Reset.ToString(),
                RehabCommand.PassiveMovement.ToString(),
                RehabCommand.AssistiveMovement.ToString(),
                RehabCommand.WaitForFinish.ToString(),
                RehabCommand.Stop.ToString()});
            cmbHKCommand.SelectedIndex = 0;

            cmdHandCommand.Items.AddRange(new string[] {
                RehabCommand.None.ToString(),
                RehabCommand.FadeIn.ToString(),
                RehabCommand.PassiveMovement.ToString(),
                RehabCommand.FadeOut.ToString()
            });
            cmdHandCommand.SelectedIndex = 0;

            AmpContainer.AmpChanged += new Action(SetCurAmplifer);
        }

        private void LoadAll()
        {
            ResManager rm = BCIApplication.SysResource;
            LoadConfig(rm);
            LoadModel(rm);
        }

        private string _key_cfg = null;

        private int cfg_stim_task_offset = 120;

        /// <summary>
        /// Load config and fill GUI items, need to be run in main thread
        /// </summary>
        /// <param name="rm"></param>
        protected override void LoadConfig(BCILib.Util.ResManager rm)
        {
            ResManager apcfg = BCIApplication.AppResource;

            // read parameters from training
            string task_cfg = "LI"; // LRTFI

            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Task_Configure, ref task_cfg);
            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Key_Configure, ref _key_cfg);

            string rehab_act = RehabAction.OpenClose.ToString();
            apcfg.GetConfigValue(MIConstDef.Rehab_Action, ref rehab_act);
            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Rehab_Action, ref rehab_act);
            tbRehabAction.Text = rehab_act;
            int nrepeats = 1;
            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Rehab_Repeat, ref nrepeats);
            lb_RehabRepeats.Text = nrepeats.ToString();

            tbMIConfig.Text = task_cfg;
            string line = MIConstDef.DefChannels;
            rm.GetConfigValue(MIConstDef.ModelTrainingSetting, MIConstDef.Train_MTS_Channels, ref line);
            textSelChannels.Tag = line;
            if (!string.IsNullOrEmpty(line)) {
                string[] slist = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                int n = 0;
                if (slist != null) n = slist.Length;
                textSelChannels.Text = n.ToString();
            }

            SetCurAmplifer();

            rm.SaveIfChanged();
            apcfg.SaveIfChanged();
        }

        private void SetCurAmplifer()
        {
            labelSelAmplifier.Text = null;
            if (AmpContainer.Count > 0) {
                labelSelAmplifier.Text = AmpContainer.GetAmplifier(0).DevName;
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

        BCIProcessor _proc = null;
        //ERDProcessor _erd = null;

        private bool LoadModel(ResManager rm)
        {
            string line = rm.GetConfigValue(MIConstDef.MIProcess, MIConstDef.ClassLabels);
            if (string.IsNullOrEmpty(line)) {
                return false;
            }
            string[] cls_labels = line.Split(',');

            line = rm.GetConfigValue(MIConstDef.MIProcess, MIConstDef.ModelPattern);
            if (string.IsNullOrEmpty(line)) {
                return false;
            }

            int p0 = line.IndexOf("_{0}");
            if (p0 > 0) {
                labelModelName.Text = line.Substring(0, p0);
            } else {
                labelModelName.Text = line;
            }

            int nmodel = cls_labels.Length - 1;
            if (nmodel == 2) nmodel = 1; // for two-classes, only one model is needed

            string[] mdl_names = new string[nmodel];

            for (int iclass = 0; iclass < mdl_names.Length; iclass++) {
                string fn = string.Format(line, iclass + 1);
                fn = Path.Combine("Model", fn);
                if (!File.Exists(fn)) return false;
                mdl_names[iclass] = fn;
            }

            _proc = CreatBCIProcessor(mdl_names);
            //new MIProcessor();

            int nchan = 0;

            if (_proc != null) {
                nchan = _proc.NumChannelUsed;
            }
            if (InvokeRequired) {
                this.Invoke((Action)delegate()
                {
                    textModelChannels.Text = nchan.ToString();
                });
            } else {
                textModelChannels.Text = nchan.ToString();
            }

            // model loaded, update buttons
            bool FBEnable = (textSelChannels.Text == textModelChannels.Text);
            cbFeedback.Enabled = buttonStartRehab.Enabled = FBEnable;
            if (!FBEnable) cbFeedback.Checked = false;

            return true;
        }

        private BCIProcessor CreatBCIProcessor(string[] mdl_names)
        {
            MI_MODEL_TYPE mtype = MI_MODEL_TYPE.FB_ParzenWindow;

            if (mdl_names.Length < 1) {
                return null;
            }

            using (StreamReader sr = File.OpenText(mdl_names[0])) {
                string line;
                while ((line = sr.ReadLine())!= null) {
                    line = line.Trim();
                    if (line.StartsWith("#")) break;

                    string[] wlist = line.Split(':');
                    if (wlist.Length == 2 && wlist[0].Trim() == "Model_Type"
                        && wlist[1].Trim() == "BCI_MI_FBFLD") {
                        mtype = MI_MODEL_TYPE.FB_FLD;
                    }
                    break;
                }
                
            }

            if (mtype == MI_MODEL_TYPE.FB_ParzenWindow) {
                MIProcessor miproc = new MIProcessor();
                if (!miproc.Initialize(mdl_names)) {
                    Console.WriteLine("Processor Initialization failed");
                    miproc = null;
                } else {
                    //_erd = new ERDProcessor();
                    //_erd.Initialize(mdl_names[0]);
                }
                return miproc;
            } else if (mtype == MI_MODEL_TYPE.FB_FLD) {
                FLDProcessor fld_proc = new FLDProcessor();
                if (!fld_proc.Initialize(mdl_names)) {
                    return null;
                }
                //_erd = new ERDProcessor();
                //_erd.Initialize(mdl_names[0]);
                return fld_proc;
            }

            throw new NotImplementedException();
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            MITaskConfig cfg = new MITaskConfig();
            cfg.TaskString = tbMIConfig.Text;
            cfg.KeyCodeString = _key_cfg;
            cfg.Action = (RehabAction) Enum.Parse(typeof(RehabAction), tbRehabAction.Text);
            int nrepeat = 1;
            int.TryParse(lb_RehabRepeats.Text, out nrepeat);
            cfg.NumRepeats = nrepeat;

            if (UserCtrlForm.ShowCtrl(cfg) == DialogResult.OK) {
                cfg.SaveConfigure(BCIApplication.SysResource);
                LoadConfig();
            }
        }

        private void buttonAmpSel_Click(object sender, EventArgs e)
        {
            SelAmpChannel cfg = new SelAmpChannel();

            string str_sch_list = (string) textSelChannels.Tag;
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

        private void buttonStartDataCollection_Click(object sender, EventArgs e)
        {
            string errmsg = null;
            bool chk_ok = true;
            bool withFeedback = cbFeedback.Checked;

            if (!CheckExternalTool(_hand_cue)) {
                chk_ok = false;
                errmsg += "Cannot initiate Hand Cue program.\n";
            }

            bool HabticKnobOK = CheckExternalTool(_haptic_knob, false);

            if (chk_ok && cbPassiveMovement.Checked)
            {
                if (!HabticKnobOK)
                {
                    chk_ok = false;
                    errmsg += "Cannot initiate Haptic Knob program.\n";
                }
            }

            Amplifier amp = AmpContainer.GetAmplifier(0);

            // Check amplifier
            if (chk_ok)
            {
                if (amp == null)
                {
                    errmsg += "No amplifier error!\n";
                    chk_ok = false;
                }
                else if (!amp.Start())
                {
                    errmsg += "Cannot start amplifier!";
                    chk_ok = false;
                }
            }

            string tmp = textSelChannels.Text;
            if (string.IsNullOrEmpty(tmp)) {
                errmsg += "Selected channel empty!\n";
                chk_ok = false;
            }

            if (chk_ok && withFeedback)
            { // check proc model
                int nsel = 0;
                int.TryParse(tmp, out nsel);

                tmp = textModelChannels.Text;
                if (_proc == null || nsel <= 0 || string.IsNullOrEmpty(tmp))
                {
                    errmsg += "Model error!\n";
                    chk_ok = false;
                }
                else
                {
                    int nmdl = int.Parse(tmp);
                    if (nmdl != nsel)
                    {
                        errmsg += "Selected changes not matching model!\n";
                        chk_ok = false;
                    }
                    else if (!_proc.SetAmplifier(amp, (string)textSelChannels.Tag))
                    {
                        errmsg += "Error set proc amplifier!\n";
                        chk_ok = false;
                    }
                    //else if (cbERDFeedback.Checked && !_erd.SetAmplifier(amp, (string)textSelChannels.Tag))
                    //{
                    //    errmsg += "Error set ERD processer!\n";
                    //    chk_ok = false;
                    //}
                }
            }

            //if (withFeedback && cbERDFeedback.Checked && !_erd.StartERDTool()) {
            //    chk_ok = false;
            //    errmsg += "Cannot start ERD Tool!\n";
            //}

            if (!chk_ok)
            {
                MessageBox.Show(errmsg, "Start Data Calibration");
                return;
            }

            //***************************
            // clear window
            _hand_cue.SendClient(GameCommand.MI_Cue, (int) RehabCommand.FadeOut);

            MIHapticKnobForm frm = new MIHapticKnobForm();

            frm.TaskString = tbMIConfig.Text;
            //frm.KeyCodes = _key_cfg;
            frm.SetProfile(
                0, // data collection
                _hand_cue, 
                cbPassiveMovement.Checked? _haptic_knob:null,
                withFeedback? _proc:null,
                //withFeedback && cbERDFeedback.Checked? _erd:
                null,
                (RehabAction) Enum.Parse(typeof(RehabAction), tbRehabAction.Text),
                int.Parse(lb_RehabRepeats.Text));

            this.Hide();
            frm.Show();
            frm.FormClosed += (FormClosedEventHandler)delegate(object so, FormClosedEventArgs arg)
            {
                this.Show();
            };
        }

        WMCopyData _hand_cue = null;
        WMCopyData _haptic_knob = null;

        private void buttonStartRehab_Click(object sender, EventArgs e)
        {
            if (!CheckExternalTool(_hand_cue) || !CheckExternalTool(_haptic_knob)) return;

            bool chk_ok = true;
            string tmp = textSelChannels.Text;
            if (string.IsNullOrEmpty(tmp)) chk_ok = false;

            if (chk_ok && labelRehab.Visible) {
                int nsel = int.Parse(tmp);
                tmp = textModelChannels.Text;
                if (nsel <= 0 || string.IsNullOrEmpty(tmp)) {
                    chk_ok = false;
                } else {
                    int nmdl = int.Parse(tmp);
                    if (nmdl != nsel) {
                        chk_ok = false;
                    }
                }
            }

            if (chk_ok) {
                Amplifier amp = AmpContainer.GetAmplifier(0);
                if (amp == null) {
                    chk_ok = false;
                } else if (!amp.Start()) {
                    chk_ok = false;
                } else if (labelRehab.Visible) {
                    if (!_proc.SetAmplifier(amp, (string)textSelChannels.Tag)) {
                        chk_ok = false;
                    }
                    //else if (cbERDFeedback.Checked) {
                    //    _erd.SetAmplifier(amp, (string)textSelChannels.Tag);
                    //}
                }
            }

            //if (labelRehab.Visible && cbERDFeedback.Checked)
            //{
            //    chk_ok = _erd.StartERDTool();
            //}

            if (!chk_ok) {
                MessageBox.Show("Cannot start Motor Imagery with Feedback!\nPlease check amplifer and model.", "Start Feedback");
                return;
            }

            MIHapticKnobForm frm = new MIHapticKnobForm();

            frm.TaskString = tbMIConfig.Text;
            //frm.KeyCodes = _key_cfg;
            frm.SetProfile(1, _hand_cue, _haptic_knob, _proc,
                //cbERDFeedback.Checked? _erd:
                null,
                (RehabAction) Enum.Parse(typeof(RehabAction), tbRehabAction.Text),
                int.Parse(lb_RehabRepeats.Text));

            this.Hide();
            frm.Show();
            frm.FormClosed += (FormClosedEventHandler)delegate(object so, FormClosedEventArgs arg)
            {
                this.Show();
            };
        }

        private void buttonTrainModel_Click(object sender, EventArgs e)
        {
            this.Hide();
            MITrainMTSModelForm dlg = new MITrainMTSModelForm();
            ArrayList ar = new ArrayList();
            string tasktr = tbMIConfig.Text;
            for (int i = 0; i < tasktr.Length; i++) {
                int idx = MITaskConfig.TASK_IDLIST.IndexOf(tasktr[i]);
                if (idx >= 0) {
                    ar.Add(cfg_stim_task_offset + idx + 1);
                }
            }
            dlg.SetMIClassLabels((int[])ar.ToArray(typeof(int)));
            dlg.ShowDialog();
            this.Show();

            LoadModel(BCIApplication.SysResource);
        }

        private void buttonFindTool_Click(object sender, EventArgs e)
        {
            BCIApplication.GetGamePath("ERDTool", true);
        }

        //private void buttonStartTool_Click(object sender, EventArgs e)
        //{
        //    _erd.StartERDTool();
        //}

        private bool CheckExternalTool(WMCopyData _tool)
        {
            return CheckExternalTool(_tool, true);
        }

        private bool CheckExternalTool(WMCopyData _tool, bool BrowseIfNotFound)
        {
            if (_tool.GetAllGUIWindows(false) > 0) {
                return true;
            }

            if (_tool.GetAllGUIWindows() <= 0) {

                if (BCIApplication.StartExternalProc(_tool.Property, null, true, BrowseIfNotFound) == null) {
                    return false;
                }
            }

            for (int i = 0; _tool.GetAllGUIWindows() <= 0 && i < 10; i++)
            {
                Thread.Sleep(100);
            }

            if (_tool.GetAllGUIWindows() <= 0)
            {
                MessageBox.Show("Cannot find client window!");
                return false;
            }

            // ccwang: Initialize command
            _tool.SendClient(GameCommand.Initialize, BCIApplication.UserPath);

            return true;
        }

        private void btnMove_Click(object sender, EventArgs e)
        {
            UpdateHKAction();
            if (!CheckExternalTool(_hand_cue)) return;

            RehabCommand rcmd = (RehabCommand)Enum.Parse(typeof(RehabCommand), cmdHandCommand.Text);
            MIAction mact = (MIAction) Enum.Parse(typeof(MIAction), cboxMIAction.Text);
            RehabAction ract = (RehabAction)Enum.Parse(typeof(RehabAction), tbRhAction.Text, true);

            _hand_cue.SendClient(GameCommand.MI_Cue, (int)rcmd, (int)ract, (int)mact);
        }

        private void tbMIConfig_TextChanged(object sender, EventArgs e)
        {
            cboxMIAction.Items.Clear();

            foreach (char c in tbMIConfig.Text)
            {
                int idx = MITaskConfig.TASK_IDLIST.IndexOf(c);
                MIAction act = (MIAction)(idx + 1);
                cboxMIAction.Items.Add(act.ToString());
            }
            if (cboxMIAction.Items.Count > 0) cboxMIAction.SelectedIndex = 0;
        }

        private void UpdateHKAction()
        {
            RehabAction act = (RehabAction)Enum.Parse(typeof(RehabAction), tbRehabAction.Text);
            if (string.IsNullOrEmpty(tbRhAction.Text) || act != RehabAction.Alternative)
            {
                tbRhAction.Text = tbRehabAction.Text;
            }
            else
            {
                int cfg_repeats = int.Parse(lb_RehabRepeats.Text);
                int tst_repeats = int.Parse(lb_TestRepeats.Text);
                if (tst_repeats >= cfg_repeats) {
                    act = (RehabAction)Enum.Parse(typeof(RehabAction), tbRhAction.Text);
                    act = 3 - act;
                    tbRhAction.Text = act.ToString();
                    tst_repeats = 1;
                } else {
                    tst_repeats++;
                }
                lb_TestRepeats.Text = tst_repeats.ToString();
            }
        }

        private void tbRehabAction_TextChanged(object sender, EventArgs e)
        {
            RehabAction act = (RehabAction)Enum.Parse(typeof(RehabAction), tbRehabAction.Text);
            if (act != RehabAction.Alternative)
            {
                tbRhAction.Text = act.ToString();
            }
            else
            {
                tbRhAction.Text = RehabAction.OpenClose.ToString();
                lb_TestRepeats.Text = 1.ToString();
            }
            lb_RehabRepeats.Visible = lb_TestRepeats.Visible = (act == RehabAction.Alternative);
        }

        private void btnHKRehab_Click(object sender, EventArgs e)
        {
            UpdateHKAction();
            if (!CheckExternalTool(_haptic_knob)) return;

            RehabCommand cmd = (RehabCommand)Enum.Parse(typeof(RehabCommand), cmbHKCommand.Text);
            RehabAction act = (RehabAction)Enum.Parse(typeof(RehabAction), tbRhAction.Text);

            _haptic_knob.SendClient(GameCommand.HapticKnob, (int)cmd, (int)act);
        }

        private void btnHKFind_Click(object sender, EventArgs e)
        {
            BCIApplication.GetGamePath(_haptic_knob.Property, true);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BCIApplication.GetGamePath(_hand_cue.Property, true, true);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_proc != null) {
                _proc.Dispose();
                _proc = null;
            }

            //if (_erd != null)
            //{
            //    _erd.Dispose();
            //    _erd = null;
            //}

            if (_haptic_knob != null) _haptic_knob.SendClient(GameCommand.CloseGame);
            if (_hand_cue != null) _hand_cue.SendClient(GameCommand.CloseGame);

            base.OnFormClosing(e);
        }

        private void btnSelModel_Click(object sender, EventArgs e)
        {
            SelectModelForm fm = new SelectModelForm();
            if (fm.ShowDialog() == DialogResult.OK) {
                string mdl_patten = fm.SelectedModel;
                if (!string.IsNullOrEmpty(mdl_patten)) {
                    ResManager cfg = BCIApplication.SysResource;
                    cfg.SetConfigValue(MIConstDef.MIProcess, MIConstDef.ModelPattern, mdl_patten);
                    cfg.SaveFile();
                    LoadModel(cfg);
                } else {
                    _proc = null;
                    labelModelName.Text = null;
                }
            }
        }

        private void btnAccuHistory_Click(object sender, EventArgs e)
        {
            CalculateUserAccuracy(1);
        }

        private void CalculateUserAccuracy(int which)
        {
            string accu_fn;
            Regex r, ar;

            if (which == 1) {
                accu_fn = "Rehab_Accu_His.txt";

                //20110819\DataTesting\MI_Right_Alternative_1_2011-08-19
                r = new Regex(@".\\(\d*)\\DataTesting\\.*\\([^\\]*Main.log)"); // [^\\]*

                // Total/Correct=10/10, Acc=100
                ar = new Regex(@"^Total/Correct=(\d+)\/(\d+),\s+Acc=(.*)$");
            } else {
                accu_fn = "Calib_Accu_His.txt";

                //20110819\DataTesting\MI_Right_Alternative_1_2011-08-19
                r = new Regex(@".\\(\d*)\\DataTraining\\FB_.*\\([^\\]*Main.log)"); // [^\\]*

                // Total/Correct=10/10, Acc=100
                ar = new Regex(@"^Total/Correct=(\d+)\/(\d+),\s+Acc=(.*)$");
            }

            int tall = 0;
            int tc = 0;

            using (StreamWriter sw = File.CreateText(accu_fn)) {
                string[] log_fl = Directory.GetFiles(".", "*Main.log", SearchOption.AllDirectories);
                foreach (string fn in log_fl) {
                    Match m = r.Match(fn);
                    if (m.Success) {
                        sw.WriteLine("{0}:{1}", m.Groups[1], m.Groups[2]);

                        int ntrue = 0, ntotal = 0;

                        // find accuracy find
                        bool found = false;
                        string[] lines = File.ReadAllLines(fn);
                        foreach (string line in lines) {
                            m = ar.Match(line);
                            if (m.Success) {
                                found = true;
                                sw.WriteLine(line);
                                int.TryParse(m.Groups[1].Value, out ntotal);
                                int.TryParse(m.Groups[2].Value, out ntrue);
                            }
                        }

                        if (!found) {
                            sw.WriteLine("Calculating...");
                            int label = 0;
                            foreach (string line in lines) {
                                if (line.StartsWith("STIM_TASK:")) {
                                    int.TryParse(line.Substring(10), out label);
                                } else if (line.StartsWith("Class=")) {
                                    int cls = 0;
                                    int.TryParse(line.Substring(6, 2), out cls);
                                    ntotal++;
                                    if (cls == 1 && label == 125 || cls == 0 && label != 125) ntrue++;
                                }
                            }

                            double acc = 0;
                            if (ntotal > 0) acc = ntrue * 100.0 / ntotal;

                            string msg = string.Format("Total/Correct={0}/{1}, Acc={2:#0.##}", ntotal, ntrue, acc);
                            sw.WriteLine(msg);
                            File.AppendAllText(fn, msg + "\r\n");
                        }

                        tall += ntotal;
                        tc += ntrue;
                    }
                }

                sw.WriteLine();
                double a = 0;
                if (tall > 0) a = tc * 100.0 / tall;

                sw.WriteLine("Total trials = {0}, Correct = {1}, Acc = {2:#0.##}", tall, tc, a);
            }

            Process.Start(accu_fn);
        }

        private void btnFBAccuHistory_Click(object sender, EventArgs e)
        {
            CalculateUserAccuracy(0);
        }
    }
}
