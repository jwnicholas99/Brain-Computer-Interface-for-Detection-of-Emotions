using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BCILib.App;
using System.Reflection;
using System.IO;
using System.Threading;
using BCILib.Util;

namespace BCILib.MotorImagery.ArtsBCI {
    [Flags]
    public enum FB_FLAG {
        None = 0,
        FLDScore = 1, 
        SmileyFace = 2,
        Message = 4,
        All = FLDScore | SmileyFace | Message
    };

    public partial class FB_Form : Form {

        List<FB_Accumulate> fb_list = new List<FB_Accumulate>();
        FB_FLAG fb_flags = FB_FLAG.All;

        public FB_Form()
        {
            InitializeComponent();
        }

        public void SetFeedbackFlag(FB_FLAG fg, bool on)
        {
            if (on) fb_flags |= fg;
            else {
                fb_flags &= ~fg;
            }

            if ((fg & FB_FLAG.FLDScore) == FB_FLAG.FLDScore) {
                foreach (FB_Accumulate ctrl in fb_list) {
                    ctrl.DisplayFLDScore((fb_flags & FB_FLAG.FLDScore) != FB_FLAG.None);
                }
            }

            UpdateFBStatus();
        }

        private void UpdateFBStatus()
        {
            if (InvokeRequired) {
                Invoke((Action)delegate()
                {
                    UpdateFBStatus();
                });
            } else {
                pictureBoxSmiley.Visible = ((fb_flags & FB_FLAG.SmileyFace) == FB_FLAG.SmileyFace);
                labelMessage.Visible = ((fb_flags & FB_FLAG.Message) == FB_FLAG.Message);
            }
        }

        public static Image[] smiley_faces;
        public enum SmileyFace {
            None, Sucess, Fail, Encourage
        };

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            uc_HProgressBar.Value = 0;
            labelMessage.Text = null;

            // Display maxmized on second screen
            Screen[] sl = Screen.AllScreens;
            if (sl.Length > 1) {
                int mi = 0;
                while (sl[mi].Primary) mi++;

                Rectangle rt = sl[mi].WorkingArea;
                this.Left = rt.Left + (rt.Width - this.Width) / 2;
                this.Height = rt.Top + (rt.Height - this.Height) / 2;
                WindowState = FormWindowState.Maximized;

                this.FormBorderStyle = FormBorderStyle.None;
            }

            LoadSmileyFaces();
        }

        public static void LoadSmileyFaces()
        {
            if (smiley_faces == null) {
                // load smiley images
                Assembly asb = Assembly.GetExecutingAssembly();
                smiley_faces = new Image[4];
                string sn = "BCILib.MotorImagery.ArtsBCI.smiley_";
                Stream sr = asb.GetManifestResourceStream(sn + "success.png");
                if (sr != null) {
                    smiley_faces[1] = Image.FromStream(sr);
                }
                sr = asb.GetManifestResourceStream(sn + "fail.png");
                if (sr != null) {
                    smiley_faces[2] = Image.FromStream(sr);
                }
                sr = asb.GetManifestResourceStream(sn + "encourage.png");
                if (sr != null) {
                    smiley_faces[3] = Image.FromStream(sr);
                }
            }
        }

        WMCopyData _hand_cue = null;
        WMCopyData _hk = null;

        internal void Set_HandCueWindow(WMCopyData cli)
        {
            // the hand cue window size is 790 * 268
            _hand_cue = cli;
            if (_hand_cue != null)
            {
                Rectangle rc = this.ClientRectangle;
                _hand_cue.SendClient(GameCommand.StartGame,
                    new int[] {
                    this.Left + rc.Left,
                    this.Top + panel1.Bottom,
                    rc.Width, 268
                });
            }
        }

        public void Set_Message(string msg)
        {
            if (InvokeRequired) {
                Invoke((Action<string>)((m) => { Set_Message(m); }),
                    new object[] {msg});
            } else {
                labelMessage.Text = msg;
            }
        }

        public void Set_Result(string msg)
        {
            if (InvokeRequired) {
                Invoke((Action<string>)((m) => { Set_Result(m); }),
                    new object[] { msg });
            } else {
                labelResult.Visible = !string.IsNullOrEmpty(msg);
                labelResult.Text = msg;
            }
        }

        internal void Show_SmileyFace(SmileyFace sface)
        {
            if (InvokeRequired) {
                Invoke((Action<SmileyFace>)((m) => { Show_SmileyFace(m); }),
                    new object[] { sface });
            } else {
                pictureBoxSmiley.Image = smiley_faces[(int)sface];
                Update();
            }
        }

        internal void Set_Progress(int p)
        {
            uc_HProgressBar.Value = p;
        }

        internal void Show_Progress(int cfg_rest_time,
            int cfg_beep_before_rest_end, ManualResetEvent e)
        {
            uc_HProgressBar.ShowProgress(cfg_rest_time, cfg_beep_before_rest_end, e);
        }

        private void MIFeedbackForm_Resize(object sender, EventArgs e)
        {
            Set_HandCueWindow(_hand_cue);
            Set_HKWindow(_hk);
        }

        internal void SetProcessor(FLDProcessor fldProcessor, int act_time)
        {
            //fb_FldScoreBar.SetProcessor(fldProcessor);
            //fb_FldScoreBar.Visible = (fldProcessor != null);
            foreach (FB_Accumulate fb in fb_list) {
                fb.SetProcessor(fldProcessor, act_time);
            }
        }

        internal void Reset()
        {
            user_reach_pos = 0;
            panel2.Invalidate();
            foreach (FB_Accumulate fb in fb_list) {
                fb.Reset();
            }
        }

        internal int GetDetection(out double score)
        {
            score = 0;
            if (fb_list == null || fb_list.Count < 1) return 0;
            return fb_list[0].GetDetection(out score);
        }

        internal bool ResultReached()
        {
            if (fb_list == null || fb_list.Count < 1) return false;
            return fb_list[0].ResultReached();
        }

        [Flags]
        public enum FBType {None, TwoClassBar, RehabOpen};

        public void SetFeedbackType(FBType fb_type)
        {
            ClearProcessor();
            Controls.Remove(fb_RehabOpen);
            Controls.Remove(fb_TwoClassBar);

            int tw = 0; // total width
            if ((fb_type & FBType.TwoClassBar) == FBType.TwoClassBar) {
                fb_list.Add(fb_TwoClassBar);
                tw += fb_TwoClassBar.Width;
            }

            if ((fb_type & FBType.RehabOpen) == FBType.RehabOpen) {
                fb_list.Add(fb_RehabOpen);
                tw += fb_RehabOpen.Width;
            }

            int n = fb_list.Count;
            int mi = 0;
            if (n > 1) mi = pictureBoxSmiley.Width - tw / (n - 1);
            int x0 = (pictureBoxSmiley.Width - (tw + mi * (n - 1))) / 2;
            foreach (FB_Accumulate ctrl in fb_list) {
                ctrl.Reset();
                ctrl.DisplayFLDScore((fb_flags & FB_FLAG.FLDScore) != FB_FLAG.None);
                ctrl.Left = x0;
                ctrl.Visible = true;
                x0 += ctrl.Width + mi;
                Controls.Add(ctrl);
            }

            SetFeedbackBarSizes();
        }

        private void SetFeedbackBarSizes()
        {
            int h = (uc_HProgressBar.Top - pictureBoxSmiley.Height) / 2;
            int y = labelMessage.Top - h;
            pictureBoxSmiley.Top = y - pictureBoxSmiley.Height - 10;

            foreach (FB_Accumulate ctrl in fb_list) {
                ctrl.Height = h;
                ctrl.Top = y;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            SetFeedbackBarSizes();
        }

        private void ClearProcessor()
        {
            foreach (FB_Accumulate fb in fb_list) {
                fb.SetProcessor(null, 0);
            }
            fb_list.Clear();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Alt) {
                if (e.KeyCode == Keys.S) SetTestSuccess(TestKeyAction.Sucess);
                else if (e.KeyCode == Keys.F) SetTestSuccess(TestKeyAction.Fail);
            } else {
                base.OnKeyDown(e);
            }
        }

        internal void Set_HKWindow(WMCopyData _haptic_knob)
        {
            _hk = _haptic_knob;
            if (_hk != null) {
                _haptic_knob.SendClient(GameCommand.HapticKnob,
                    (int)RehabCommand.SetWindow,
                    this.Left + panel1.Left, this.Top + panel1.Top,
                    panel1.Width, panel1.Height);
            }
        }

        internal void SetTestSuccess(TestKeyAction rst)
        {
            if (fb_list == null || fb_list.Count < 1) return;
            fb_list[0].SetTestSuccess(rst);
        }

        internal void SetMIStep(MI_STEP mi_step)
        {
            if (fb_list == null || fb_list.Count < 1) return;
            fb_list[0].SetMIStep(mi_step);
        }

        internal void StartAccuScore()
        {
            if (fb_list == null || fb_list.Count < 1) return;
            fb_list[0].StartAccuScore();
        }

        internal void SetFeedbackFlag(FB_FLAG fg)
        {
            fb_flags = fg;

            fb_TwoClassBar.DisplayFLDScore((fb_flags & FB_FLAG.FLDScore) != FB_FLAG.None);
            fb_RehabOpen.DisplayFLDScore((fb_flags & FB_FLAG.FLDScore) != FB_FLAG.None);

            UpdateFBStatus();
        }

        internal int GetFeedbackFlag()
        {
            return (int) fb_flags;
        }

        internal Action<double, int> GetAccuDelegate()
        {
            if (fb_list.Count > 0) {
                return (Action<double, int>)((s, m) =>
                {
                    fb_list[0].UpdateScore(s, m);
                });
            } else {
                return null;
            }
        }

        int user_reach_pos = 0;

        internal void SetUserReachPostions(int user_rehab_result)
        {
            //if (InvokeRequired) {
            //    BeginInvoke((VFunc<int>)(x => SetUserReachPostions(x)), user_rehab_result);
            //}
            user_reach_pos = user_rehab_result;
            panel2.Invalidate();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle rt = panel2.ClientRectangle;
            int x = rt.Width / 2 - 5;
            int y = rt.Height - 11;
            if ((user_reach_pos & 1) == 1) {
                g.FillRectangle(Brushes.Green, x, y, 10, 10);
            } else {
                g.DrawRectangle(Pens.White, x, y, 10, 10);
            }

            y = 0;
            if ((user_reach_pos & 2) == 2) {
                g.FillRectangle(Brushes.Green, x, y, 10, 10);
            } else {
                g.DrawRectangle(Pens.White, x, y, 10, 10);
            }

            x = 0;
            y = rt.Height / 2 - 5;
            if ((user_reach_pos & 4) == 4) {
                g.FillRectangle(Brushes.Green, x, y, 10, 10);
            } else {
                g.DrawRectangle(Pens.White, x, y, 10, 10);
            }

            x = rt.Width - 11;
            if ((user_reach_pos & 8) == 8) {
                g.FillRectangle(Brushes.Green, x, y, 10, 10);
            } else {
                g.DrawRectangle(Pens.White, x, y, 10, 10);
            }
        }
    }
}
