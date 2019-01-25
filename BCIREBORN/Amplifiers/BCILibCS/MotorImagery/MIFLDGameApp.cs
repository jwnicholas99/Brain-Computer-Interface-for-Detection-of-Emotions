using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using BCILib.Util;
using BCILib.App;
using BCILib.Amp;
using System.IO;
using BCILib.EngineProc;

namespace BCILib.MotorImagery
{
    internal partial class MIFLDGameApp : BCILib.App.BCIAppForm
    {
        public MIFLDGameApp()
        {
            InitializeComponent();

            fldScoreViewer.score_range_changed += new Action<double>(fldScoreViewer_score_range_changed);

            cur_frm = new BCICursorCtrl();
            LoadConfig();
        }

        void fldScoreViewer_score_range_changed(double arg)
        {
            if (InvokeRequired) {
                Invoke((Action)delegate()
                {
                    fldScoreViewer_score_range_changed(arg);
                });
            } else {
                tbLeftThr.Minimum = tbRightThr.Minimum = -(int)(arg * 100);
                tbLeftThr.Maximum = tbRightThr.Maximum = (int)(arg * 100);
            }
        }

        private string _key_cfg = null;

        private void btnChangeCfg_Click(object sender, EventArgs e)
        {
            MITaskConfig cfg = new MITaskConfig();
            cfg.TaskString = tbMIConfig.Text;
            cfg.KeyCodeString = _key_cfg;

            if (UserCtrlForm.ShowCtrl(cfg) == DialogResult.OK) {
                cfg.SaveConfigure(BCIApplication.SysResource);
                LoadConfig();
            }
        }

        /// <summary>
        /// Load config and fill GUI items, need to be run in main thread
        /// </summary>
        /// <param name="rm"></param>
        protected override void LoadConfig(ResManager rm)
        {
            // read parameters from training
            string task_cfg = "LR"; // LRTFI

            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Task_Configure, ref task_cfg);
            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Key_Configure, ref _key_cfg);

            tbMIConfig.Text = task_cfg;
            if (task_cfg.Length > 0) {
                lblClass1.Text = (MIAction.None + MITaskConfig.TASK_IDLIST.IndexOf(task_cfg[0]) + 1).ToString();
            } if (task_cfg.Length > 1) {
                lblClass2.Text = (MIAction.None + MITaskConfig.TASK_IDLIST.IndexOf(task_cfg[1]) + 1).ToString();
            }

            string line = MIConstDef.DefChannels;
            rm.GetConfigValue(MIConstDef.ModelTrainingSetting, MIConstDef.Train_MTS_Channels, ref line);
            textSelChannels.Tag = line;
            if (!string.IsNullOrEmpty(line)) {
                string[] slist = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                int n = 0;
                if (slist != null) n = slist.Length;
                textSelChannels.Text = n.ToString();
            }

            double dv = cur_frm.thr_left;
            rm.GetConfigValue(MIConstDef.Test, "Threshold_Left", ref dv);
            cur_frm.thr_left = dv;
            tbLeftThr.Value = (int)dv;

            dv = cur_frm.thr_right;
            rm.GetConfigValue(MIConstDef.Test, "Threshold_Right", ref dv);
            cur_frm.thr_right = dv;
            tbRightThr.Value = (int)dv;

            //rm.GetConfigValue(MIConstDef.MITest, "ScoreBias", ref cfg_score_bias);
            //rm.GetConfigValue(MIConstDef.MITest, "SaveBias", ref cfg_save_bias);
            //cbSaveBias.Checked = cfg_save_bias;

            //if (cfg_save_bias) {
            //    SetBias(cfg_score_bias, true, false);
            //} else {
            //    SetBias(0, true, false);
            //}

            SetCurAmplifer();

            rm.SaveIfChanged();
        }

        private void SaveConfig()
        {
            ResManager rm = BCIApplication.SysResource;
            rm.SetConfigValue(MIConstDef.Test, "Threshold_Left", cur_frm.thr_left);
            rm.SetConfigValue(MIConstDef.Test, "Threshold_Right", cur_frm.thr_right);
            rm.SaveFile();
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

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            LoadAll();
            AmpContainer.AmpChanged += new Action(SetCurAmplifer);
        }

        private void LoadAll()
        {
            ResManager rm = BCIApplication.SysResource;
            LoadConfig(rm);
            LoadModel(rm);
        }
        FLDProcessor _proc = null;

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

            textModelChannels.Text = nchan.ToString();

            Amplifier amp = AmpContainer.GetAmplifier(0);
            if (amp == null) {
                return false;
            }

            _proc.SetAmplifier(amp, (string)textSelChannels.Tag);
            //_proc.SetReadingShift(500);

            fldScoreViewer.SetProcessor((FLDProcessor)_proc);

            FLDModel model = _proc.GetFLDModel();
            lbModelInfo.Text = string.Format("Threshold={0:#.##}/{1} Range: {2:#.##}, {3:#.##}.",
                model.thr, model.atime, model.fld_r[0], model.fld_r[1]);

            return true;
        }

        private FLDProcessor CreatBCIProcessor(string[] mdl_names)
        {
            MI_MODEL_TYPE mtype = MI_MODEL_TYPE.FB_ParzenWindow;

            if (mdl_names.Length < 1) {
                return null;
            }

            using (StreamReader sr = File.OpenText(mdl_names[0])) {
                string line;
                while ((line = sr.ReadLine()) != null) {
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
                //MIProcessor miproc = new MIProcessor();
                //if (!miproc.Initialize(mdl_names)) {
                //    Console.WriteLine("Processor Initialization failed");
                //    miproc = null;
                //}
                //return miproc;
                return null;
            } else if (mtype == MI_MODEL_TYPE.FB_FLD) {
                FLDProcessor fld_proc = new FLDProcessor();
                if (!fld_proc.Initialize(mdl_names)) {
                    return null;
                }
                return fld_proc;
            }

            throw new NotImplementedException();
        }

        private void buttonAmpSel_Click(object sender, EventArgs e)
        {
            SelAmpChannel cfg = new SelAmpChannel();
            cfg.ShowAmplifier();

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

        private void btnStartCrossArrowTraining_Click(object sender, EventArgs e)
        {
            MITrainCrossArrowForm frm = new MITrainCrossArrowForm();

            frm.TaskString = tbMIConfig.Text;
            if (!string.IsNullOrEmpty(_key_cfg)) {
                frm.KeyCodes = _key_cfg.Split('|');
            }

            ShowGameForm(frm);
        }

        private void ShowGameForm(Form frm)
        {
            btnStartCrossArrowTraining.Enabled = false;
            btnStartCursorCtrl.Enabled = false;
            ControlBox = false;

            Screen[] screens = Screen.AllScreens;
            if (screens.Length > 1) {
                foreach (Screen s in screens) {
                    if (s.Primary) continue;

                    frm.StartPosition = FormStartPosition.Manual;
                    frm.Location = new Point(s.Bounds.Left + (s.Bounds.Width - frm.Width) / 2,
                        s.Bounds.Top + (s.Bounds.Height - frm.Height) / 2);
                    frm.WindowState = FormWindowState.Maximized;

                    break;
                }
            }
            frm.Show();

            frm.Closed += new EventHandler(frm_Closed);
        }

        void frm_Closed(object sender, EventArgs e)
        {
            btnStartCrossArrowTraining.Enabled = true;
            btnStartCursorCtrl.Enabled = true;
            ControlBox = true;
            BringToFront();
        }

        private int cfg_stim_task_offset = 120;

        private void buttonTrainModel_Click(object sender, EventArgs e)
        {
            this.Hide();
            MITrainMTSModelForm dlg = new MITrainMTSModelForm();
            List<int> tlist = new List<int>();
            string tasktr = tbMIConfig.Text;
            for (int i = 0; i < tasktr.Length; i++) {
                int idx = MITaskConfig.TASK_IDLIST.IndexOf(tasktr[i]);
                if (idx >= 0) {
                    tlist.Add(cfg_stim_task_offset + idx + 1);
                }
            }
            dlg.SetMIClassLabels((int[])tlist.ToArray());
            dlg.ShowDialog();
            this.Show();

            LoadModel(BCIApplication.SysResource);
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

        BCICursorCtrl cur_frm;
        private void btnStartCursorCtrl_Click(object sender, EventArgs e)
        {
            _proc.Amplifier.Start();
            cur_frm.InitProcess(_proc);
            cur_frm.ThresholdValue_Changed += new Action<double, double>(cur_frm_ThresholdValue_Changed);


            ShowGameForm(cur_frm);
        }

        void cur_frm_ThresholdValue_Changed(double thr1, double thr2)
        {
            tbLeftThr.Value = (int)thr1;
            tbRightThr.Value = (int) thr2;
            SaveConfig();
        }

        private void tbClassThr_Scroll(object sender, EventArgs e)
        {
            cur_frm.thr_left = tbLeftThr.Value;
            cur_frm.thr_right = tbRightThr.Value;
            SaveConfig();
        }

        private void tbLeftThr_ValueChanged(object sender, EventArgs e)
        {
            lblLeftThr.Text = tbLeftThr.Value.ToString();
        }

        private void tbThrRight_ValueChanged(object sender, EventArgs e)
        {
            lblRightThr.Text = tbRightThr.Value.ToString();
        }

    }
}
