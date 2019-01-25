using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BCILib.Util;
using BCILib.App;
using System.IO;
using BCILib.Amp;
using BCILib.EngineProc;

namespace BCILib.MotorImagery
{
    public partial class FLDAccumulator : UserControl
    {
        public FLDAccumulator()
        {
            InitializeComponent();
        }

        public BCITask GameType = BCITask.None;

        public double cfg_score_bias = 0;
        internal bool cfg_save_bias = false;

        public void LoadConfig(BCILib.Util.ResManager rm)
        {
            rm.GetConfigValue(MIConstDef.MITest, "ScoreBias", ref cfg_score_bias);
            rm.GetConfigValue(MIConstDef.MITest, "SaveBias", ref cfg_save_bias);
            cbSaveBias.Checked = cfg_save_bias;

            if (cfg_save_bias) {
                SetBias(cfg_score_bias, true, false);
            } else {
                SetBias(0, true, false);
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

            fldScoreViewer.SetBias(nv);

            textBoxBias.Text = nv.ToString("0.##");
            tbFeedback.Text = fldScoreViewer.GetFeedbackMessage();

            cfg_score_bias = nv;
            if (Save) {
                SaveConfig();
            }
        }

        private void SaveConfig()
        {
            ResManager res = BCIApplication.SysResource;
            res.SetConfigValue(MIConstDef.MITest, "ScoreBias", cfg_score_bias.ToString());
            res.SetConfigValue(MIConstDef.MITest, "SaveBias", cbSaveBias.Checked.ToString());
            res.SaveFile();
        }

        public FLDProcessor Processor = null;

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
            return null;
        }

        public bool LoadModel(ResManager rm, Amplifier amp, string used_channels)
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

            int nmodel = cls_labels.Length;
            if (nmodel == 2) nmodel = 1; // for two-classes, only one model is needed

            string[] mdl_names = new string[nmodel];

            for (int iclass = 0; iclass < mdl_names.Length; iclass++) {
                string fn = string.Format(line, iclass + 1);
                fn = Path.Combine("Model", fn);
                if (!File.Exists(fn)) return false;
                mdl_names[iclass] = fn;
            }

            Processor = CreatBCIProcessor(mdl_names);
            //new MIProcessor();

            int nchan = 0;

            if (Processor != null) {
                nchan = Processor.NumChannelUsed;
            }

            textModelChannels.Text = nchan.ToString();

            Processor.SetAmplifier(amp, used_channels);

            fldScoreViewer.SetProcessor((FLDProcessor)Processor);

            FLDModel model = (Processor as FLDProcessor).GetFLDModel();
            lbModelInfo.Text = string.Format("Threshold={0:#.##}/{1} Range: {2:#.##}, {3:#.##}.",
                model.thr, model.atime, model.fld_r[0], model.fld_r[1]);

            return true;
        }

        private void btnBiasReset_Click(object sender, EventArgs e)
        {
            SetBias(0, true, true);
        }

        private void trackBarBias_ValueChanged(object sender, EventArgs e)
        {
            if (trackBarBias.Tag == null) {
                SetBias(trackBarBias.Value / 10.0, false, true);
            }
        }

        private void btnSetBias_Click(object sender, EventArgs e)
        {
            object tag = btnSetBias.Tag;
            if (tag is Double) {
                SetBias((double)tag, true, true);
            }
        }

        public void UpdateScore()
        {
            if (InvokeRequired) {
                Invoke((Action)(() => UpdateScore()));
            } else {
                //tbFeedback.Text = fldScoreViewer.GetFeedbackMessage();

                if (GameType == BCITask.Supervised) {
                    double acc = 0;
                    double bias = fldScoreViewer.Calculate_BestBias(out acc);

                    labelBestScore.Text = string.Format("Best score = {0:0.##} @ Bias = {1:0.##}", acc, bias);
                    btnSetBias.Tag = bias;

                    if (cbAutoAjust.Checked && fldScoreViewer.TotalTrials > 5) btnSetBias.PerformClick();
                } else {
                    panelBestBias.Visible = false;
                }
            }
        }

        public string FeedbackText
        {
            set
            {
                if (InvokeRequired) {
                    Invoke((Action)(() => { FeedbackText = value; }));
                } else {
                    tbFeedback.Text = value;
                }
            }
        }
    }
}
