using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BCILib.App;
using BCILib.Util;
using System.Threading;

namespace BCILib.P300
{
    public partial class P300SpellerForm : Form
    {
        ParameterizedThreadStart dlg_prepare = null;

        private P300SpellerForm()
        {
            InitializeComponent();

            Func<int, int, bool> dlg_SetProgress = (int v, int v0) => {
                progressBarPrepare.Value = progressBarPrepare.Minimum +  
                    v * (progressBarPrepare.Maximum - progressBarPrepare.Minimum) / v0;
                return true;
            };

            dlg_prepare = (object o) =>
            {
                int ms = (int)o;
                if (ms <= 0) return;
 
                long tstart = HRTimer.GetTimestamp();
                int tnow = 0;
                while (tnow < ms) {
                    BeginInvoke(dlg_SetProgress, (ms - tnow), ms);

                    tnow = HRTimer.DeltaMilliseconds(tstart);
                    if (tnow > ms) tnow = ms;
                    Thread.Sleep(20);
                }
            };
        }

        P300ConfigCtrl _cfg = null;

        public P300SpellerForm(P300ConfigCtrl cfg)
            : this()
        {
            _cfg = cfg;
            InitializeP300Speller();
        }

        private Button[] _all_buttons = null;

        private void InitializeP300Speller()
        {
            int n = _cfg.NumEpochPerRound;
            char[] char_list = ResManager.StringToCharList(_cfg.SpellerChars);
            if (n > char_list.Length) n = char_list.Length;

            _all_buttons = new Button[n];
            EventHandler btn_handler = new EventHandler(btn_Click);
            for (int i = 0; i < n; i++) {
                Button btn = new Button();
                btn.Tag = char_list[i];
                switch(char_list[i]) {
                    case '\x000':
                        btn.Text = "\u2423"; // "_";
                        break;
                    case '\x001':
                        btn.Text = "\u21DA"; // "\u2408"; // "\u232B";// "\u2190"; // "\u2421"; // "\u21E6";
                        break;
                    default:
                        btn.Text = char_list[i].ToString();
                        break;
                }
                btn.Click += btn_handler;
                btn.FlatStyle = FlatStyle.Flat;
                btn.BackColor = SystemColors.Control;
                btn.ForeColor = SystemColors.WindowText;
                _all_buttons[i] = btn;
            }
            
            panelFlashButtons.Controls.AddRange(_all_buttons);
            ButtonsLayOut();
        }

        private void ButtonsLayOut()
        {
            if (panelFlashButtons.Width <= panelFlashButtons.MinimumSize.Width) {
                return;
            }

            if (_all_buttons == null) return;

            // resize and reposition buttons
            int nall = _all_buttons.Length;
            Button btn;
            int nc = _cfg.GUIColumns;
            int intsp = 2; // internal space
            int margin = 1;
            int nr = (nall + nc - 1) / nc;

            double xu = (double) panelFlashButtons.Width / ((intsp + margin)* nc + margin);
            double yu = (double) panelFlashButtons.Height / ((intsp + margin) * nr + margin);
            if (yu < 8) yu = 8;

            Font fnt = new Font(FontFamily.GenericMonospace, (float)yu, FontStyle.Bold);

            int w = (int) (intsp * xu);
            int h = (int)(intsp * yu);
            int n1 = nall / nc * nc;
            for (int i = 0; i < n1; i++) {
                btn = _all_buttons[i];
                int x = i % nc;
                int y = i / nc;
                btn.Width = w;
                btn.Height = h;
                btn.Left = (int) ((margin + (intsp + margin) * x) * xu);
                btn.Top = (int) ((margin + (intsp + margin) * y) * yu);
                btn.Font = fnt;
            }

            nc = nall - n1;
            if (nc > 0) {
                xu = (double) panelFlashButtons.Width / ((intsp + margin) * nc + margin);
                w = (int)(intsp * xu);
                for (int i = n1; i < nall; i++) {
                    btn = _all_buttons[i];
                    int x = i - n1;
                    int y = nr - 1;
                    btn.Width = w;
                    btn.Height = h;
                    btn.Left = (int)((margin + (intsp + margin) * x) * xu);
                    btn.Top = (int)((margin + (intsp + margin) * y) * yu);
                    btn.Font = fnt;
                }
            }
            fnt.Dispose();
        }

        void btn_Click(object sender, EventArgs e)
        {
            if (sender is Button) {
                char c = (char)((Button)sender).Tag;
                switch (c) {
                    case '\x000':
                        textBoxOutput.AppendText(" ");
                        break;
                    case '\x001':
                        int len = textBoxOutput.Text.Length;
                        if (len > 0) {
                            textBoxOutput.Text = textBoxOutput.Text.Substring(0, len - 1);
                        }
                        break;
                    default:
                        textBoxOutput.AppendText(((Button)sender).Text);
                        break;
                }
            }
        }

        private void panelFlashButtons_Resize(object sender, EventArgs e)
        {
            ButtonsLayOut();
        }

        private void P300SpellerForm_Load(object sender, EventArgs e)
        {
            WMCopyData.SetProp(this.Handle, _cfg.P300Speller, 1);

            var srn = Screen.AllScreens.FirstOrDefault(x => !x.Primary);
            if (srn != null) {
                // center position.
                var rpos = srn.WorkingArea;
                Left = rpos.Left + (rpos.Width - Width) / 2;
                Top = rpos.Top + (rpos.Height - Height) / 2;
                WindowState = FormWindowState.Maximized;
                textBoxOutput.Font = new Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            }

            progressBarPrepare.Select();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WMCopyData.WM_COPYDATA) {
                ctrl_msg mdata = WMCopyData.TranslateMessage(m);
                switch (mdata.cmd) {
                    case GameCommand.CloseGame:
                        Close();
                        break;
                    case GameCommand.P300CMD:
                        P300CMDReader rd = new P300CMDReader(mdata.bva);
                        switch (rd.ReadCommand()) {
                            case P300_WMCMD.Set_Button_State:
                                //SendGameCommand(P300_WMCMD.Set_Button_State, btn, state);
                                int btn = rd.ReadInt32();
                                ButtonState state = (ButtonState)rd.ReadInt32();
                                SetButtonState(btn, state);
                                break;
                            case P300_WMCMD.Prepare_Start:
                                int tms = rd.ReadInt32();
                                PrepareStart(tms);
                                break;
                            case P300_WMCMD.Output_Code_Score:
                                btn = rd.ReadInt32();
                                double score = rd.ReadDouble();
                                OutputCodeScore(btn - 1, score);
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
                return;
            }
            base.WndProc(ref m);
        }

        private void OutputCodeScore(int bno, double score)
        {
            if (bno >= 0 && bno < _all_buttons.Length) {
                Button btn = _all_buttons[bno];
                btn.PerformClick();
            }
        }

        private void PrepareStart(int tms)
        {
            ControlBox = false;
            progressBarPrepare.Visible = true;

            Thread thd = new Thread(dlg_prepare);
            thd.Start(tms);


            while (!thd.Join(50)) {
                Application.DoEvents();
            }

            progressBarPrepare.Visible = false;
            ControlBox = true;
        }

        private void SetButtonState(int bno, ButtonState state)
        {
            if (bno >= 0 && bno < _all_buttons.Length) {
                Button btn = _all_buttons[bno];
                switch (state) {
                    case ButtonState.Cue:
                    case ButtonState.Highlight:
                        btn.BackColor = Color.Black;
                        btn.ForeColor = Color.Red;
                        break;
                    case ButtonState.Normal:
                        btn.BackColor = SystemColors.Control;
                        btn.ForeColor = SystemColors.WindowText;
                        break;
                    case ButtonState.Result:
                        //btn.BackColor = Color.Blue;
                        //btn.ForeColor = Color.Red;
                        btn.PerformClick();
                        break;
                    case ButtonState.NewLine:
                        textBoxOutput.AppendText("\r\n");
                        break;
                    case ButtonState.Clear:
                        textBoxOutput.Clear();
                        break;
                    default:
                        break;
                }
                btn.Update();
            }
        }
    }
}
