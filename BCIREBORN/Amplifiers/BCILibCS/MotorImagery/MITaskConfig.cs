using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using BCILib.MotorImagery.ArtsBCI;

namespace BCILib.MotorImagery
{
    public partial class MITaskConfig : UserControl
    {
        public MITaskConfig()
        {
            InitializeComponent();
        }

        private char MIType(CheckBox cboxAction, RadioButton rbtnImage, TextBox tboxCode) {
            if (!cboxAction.Checked) return 'N'; // not selected
            if (rbtnImage == null && tboxCode == null) return 'I';
            if (rbtnImage != null && rbtnImage.Checked) return 'I'; // imaginary
            if (tboxCode != null && !string.IsNullOrEmpty(tboxCode.Text)) return 'K'; // keyborad key
            else return 'T'; // tapping
        }

        private void SetMITask(char cfg, CheckBox cboxAction, RadioButton rbtnImage, RadioButton rbtnTap)
        {
            cboxAction.Checked = cfg != 'N';
            if (cboxAction.Checked) {
                if (rbtnImage != null) rbtnImage.Checked = (cfg == 'I');
                if (rbtnTap != null) rbtnTap.Checked = !rbtnImage.Checked;
            }
        }

        /// <summary>
        /// Five character string describe whole tasks with seqence as: left, right, tongue, feet and idle. M=Imagination, T=Finger Taping
        /// </summary>
        public string TaskCfg
        {
            get
            {
                char[] cfgstring = new char[5];
                cfgstring[0] = MIType(cboxLeft, rbtnLImage, tboxLKey);
                cfgstring[1] = MIType(cboxRight, rbtnRImage, tboxRKey);
                cfgstring[2] = MIType(cboxTongue, rbtnTImage, tboxTKey);
                cfgstring[3] = MIType(cboxFeet, rbtnFImage, tboxFKey);
                cfgstring[4] = MIType(cboxIdle, null, null);
                return new string(cfgstring);
            }

            //set
            //{
            //    int len = string.IsNullOrEmpty(value)? 0: value.Length;
            //    char[] cfg = new string('N', 5).ToCharArray();
            //    if (len > 0) cfg[0] = value[0];
            //    if (len > 1) cfg[1] = value[1];
            //    if (len > 2) cfg[2] = value[2];
            //    if (len > 3) cfg[3] = value[3];
            //    if (len > 4) cfg[4] = value[4];

            //    SetMITask(cfg[0], cboxLeft, rbtnLImage, rbtnLTap);
            //    SetMITask(cfg[1], cboxRight, rbtnRImage, rbtnRTap);
            //    SetMITask(cfg[2], cboxTongue, rbtnTImage, rbtnTTap);
            //    SetMITask(cfg[3], cboxFeet, rbtnFImage, rbtnFTap);
            //    SetMITask(cfg[4], cboxIdle, null, null);
            //}
        }

        public const string TASK_IDLIST = "LRTFI";

        public string TaskString
        {
            get
            {
                string taskcfg = TaskCfg;
                StringBuilder tstr = new StringBuilder();
                for (int i = 0; i < taskcfg.Length; i++) {
                    if (taskcfg[i] != 'N') tstr.Append(TASK_IDLIST[i]);
                }
                return tstr.ToString();
            }

            set
            {
                CheckBox[] tl = new CheckBox[] {cboxLeft, cboxRight, cboxTongue, cboxFeet, cboxIdle};
                foreach (CheckBox cbox in tl) cbox.Checked = false;

                foreach (char ti in value) {
                    int idx = TASK_IDLIST.IndexOf(ti);
                    if (idx >= 0 && idx < tl.Length) {
                        tl[idx].Checked = true;
                    }
                }
            }
        }

        public string[] KeyCodes
        {
            get
            {
                string[] keys = new string[4];
                keys[0] = tboxLKey.Text;
                keys[1] = tboxRKey.Text;
                keys[2] = tboxTKey.Text;
                keys[3] = tboxFKey.Text;
                return keys;
            }
            set
            {
                int len = value.Length;

                tboxLKey.Text = null;
                bool IM = true;
                if (len > 0) {
                    tboxLKey.Text = value[0];
                    IM = string.IsNullOrEmpty(value[0]);
                }
                rbtnLImage.Checked = IM;
                rbtnLTap.Checked = !IM;

                tboxRKey.Text = null;
                IM = true;
                if (len > 1) {
                    tboxRKey.Text = value[1];
                    IM = string.IsNullOrEmpty(value[1]);
                }
                rbtnRImage.Checked = IM;
                rbtnRTap.Checked = !IM;

                tboxFKey.Text = null;
                if (len > 3) {
                    tboxTKey.Text = value[2];
                    IM = string.IsNullOrEmpty(value[2]);
                }
                rbtnTImage.Checked = IM;
                rbtnTTap.Checked = !IM;

                tboxTKey.Text = null;
                if (len > 3) {
                    tboxFKey.Text = value[3];
                    IM = string.IsNullOrEmpty(value[3]);
                }
                rbtnFImage.Checked = IM;
                rbtnFTap.Checked = !IM;
            }
        }

        public string KeyCodeString
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(tboxLKey.Text);
                sb.Append('|');
                sb.Append(tboxRKey.Text);
                sb.Append('|');
                sb.Append(tboxTKey.Text);
                sb.Append('|');
                sb.Append(tboxFKey.Text);
                return sb.ToString();
            }

            set
            {
                tboxLKey.Text = null;
                tboxRKey.Text = null;
                tboxFKey.Text = null;
                tboxTKey.Text = null;

                if (!string.IsNullOrEmpty(value)) {
                    KeyCodes = value.Split('|');
                }
            }
        }

        private void SetTapKey(object sender, KeyEventArgs e)
        {
            if (sender is TextBox) {
                TextBox tbox = (TextBox)sender;
                tbox.Text = e.KeyCode.ToString(); // Keycode is enouth, we don't use any modifiers

                if (tbox == tboxLKey) rbtnLTap.Checked = true;
                if (tbox == tboxRKey) rbtnRTap.Checked = true;
                if (tbox == tboxTKey) rbtnTTap.Checked = true;
                if (tbox == tboxFKey) rbtnFTap.Checked = true;
            }

            _changed = true;
        }

        private void MIActionCheckedChanged(object sender, EventArgs e)
        {
            if (sender is CheckBox) {
                if (sender == cboxLeft) panelLeft.Enabled = cboxLeft.Checked;
                if (sender == cboxRight) panelRight.Enabled = cboxRight.Checked;
                if (sender == cboxTongue) panelTongue.Enabled = cboxTongue.Checked;
                if (sender == cboxFeet) panelFeet.Enabled = cboxFeet.Checked;
            }

            _changed = true;
        }

        bool _changed = false;

        private void MIActModeChanged(object sender, EventArgs e)
        {
            _changed = true;
        }

        public void SaveConfigure(BCILib.Util.ResManager rm)
        {
            if (_changed) {
                rm.SetConfigValue(MIConstDef.MITraining, MIConstDef.Task_Configure, TaskString);
                rm.SetConfigValue(MIConstDef.MITraining, MIConstDef.Key_Configure, KeyCodeString);
                rm.SetConfigValue(MIConstDef.MITraining, MIConstDef.Rehab_Action, Action.ToString());
                rm.SetConfigValue(MIConstDef.MITraining, MIConstDef.Rehab_Repeat, nud_NumRepeats.Value.ToString());
                rm.SaveIfChanged();
            }
        }

        private void buttonAllImagery_Click(object sender, EventArgs e)
        {
            rbtnLImage.Checked = rbtnRImage.Checked = rbtnFImage.Checked = rbtnTImage.Checked = true;
        }

        //public static string ShortConfigString(string cfg_str)
        //{
        //    int n = cfg_str.Length;
        //    int i = 0;
        //    while (i < n && cfg_str[n - 1 - i] == 'N') i++;

        //    if (i > 0) cfg_str = cfg_str.Substring(0, n - i);
        //    return cfg_str;
        //}

        public RehabAction Action
        {
            get
            {
                if (rbOpenClose.Checked) return RehabAction.OpenClose;
                else if (rbRotate.Checked) return RehabAction.Rotate;
                else if (rbAlternative.Checked) return RehabAction.Alternative;
                else return RehabAction.None;
            }

            set
            {
                rbOpenClose.Checked = (value == RehabAction.OpenClose);
                rbRotate.Checked = (value == RehabAction.Rotate);
                rbAlternative.Checked = (value == RehabAction.Alternative);
            }
        }

        public int NumRepeats
        {
            get
            {
                return (int) nud_NumRepeats.Value;
            }

            set
            {
                nud_NumRepeats.Value = value;
            }
        }

        private void rbAlternative_CheckedChanged(object sender, EventArgs e)
        {
            labelRepeats.Visible = nud_NumRepeats.Visible = rbAlternative.Checked;
        }
    }
}
