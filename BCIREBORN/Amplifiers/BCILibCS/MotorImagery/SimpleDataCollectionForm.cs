using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using BCILib.Util;
using BCILib.App;

namespace BCILib.MotorImagery
{
    public partial class SimpleDataCollectionForm : Form, FeedbackInterface
    {

        public SimpleDataCollectionForm()
        {
            InitializeComponent();

            sfmt = new StringFormat();
            sfmt.LineAlignment = StringAlignment.Center;
            sfmt.Alignment = StringAlignment.Center;

            task_pen = new Pen(Color.White, 24);
            task_pen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
            task_pen.StartCap = System.Drawing.Drawing2D.LineCap.Flat;
        }

        #region FeedbackInterface Members
        void ShowProgress(int time, bool wait)
        {
            prog_time = time;
            Thread thd = new Thread(new ThreadStart(ExeProgress));
            thd.Start();

            if (wait) {
                if (InvokeRequired) thd.Join();

                else while (!thd.Join(10)) Application.DoEvents();
            }
        }

        private void ExeProgress()
        {
            long t0 = HRTimer.GetTimestamp();
            prog_now = 0;
            while (prog_now < prog_time) {
                prog_now = HRTimer.DeltaMilliseconds(t0);

                int wt = prog_now - prog_now;
                if (wt > 50) wt = 50;
                else if (wt < 0) wt = 0;
                if (evt_stopproc.WaitOne(wt, false)) break;
            }

            prog_time = prog_now = 0;
        }

        void FeedbackInterface.SetMIStep(MI_STEP mI_STEP, int time, params int[] args)
        {
            mi_step = mI_STEP;
            if (mi_step == MI_STEP.MI_None) {
                message = "Motor Imagery Data Collection";
                msz = 0;
            } else {
                message = null;
            }

            if (mi_step == MI_STEP.MI_Rest) ShowProgress(time, false);
            Invalidate();
        }


        bool FeedbackInterface.StartGame(ManualResetEvent evt_proc, BCITask mtype, int ntrials, params int[] clabels)
        {
            evt_stopproc = evt_proc;
            this.ntrials = ntrials;
            this.Show();
            return true;
        }

        void FeedbackInterface.StartTrial(MIAction task, int itrial)
        {
            trial_start = BCIApplication.ElaspedMilliSeconds;
            this.itrial = itrial;
            mi_act = task;

            trial_message = string.Format("Trail {0}/{1}: {2}", itrial + 1, ntrials, task);
        }

        #endregion

        ManualResetEvent evt_stopproc = null;
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Screen screen = Screen.AllScreens.FirstOrDefault(x => !x.Primary);
            if (screen != null) {
                Left = screen.Bounds.Left + (screen.Bounds.Width - Width) / 2;
                Top = screen.Bounds.Top + (screen.Bounds.Height - Height) / 2;
                WindowState = FormWindowState.Maximized;
            } else {
                var sz = Screen.PrimaryScreen.WorkingArea;
                Top = sz.Top;
                Left = sz.Left;
                Width = sz.Width;
                Height = sz.Height;
            }
        }

        string message = "Motor Image Data Collection";
        string trial_message = null;

        MI_STEP mi_step = MI_STEP.MI_None;
        MIAction mi_act = MIAction.None;

        StringFormat sfmt = null;
        int prog_time = 0;
        int prog_now = 0;
        int trial_start = 0;
        float msz = 0;
        Pen task_pen;
        int ntrials = 0;
        int itrial = 0;

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle rt = this.ClientRectangle;

            int x0 = rt.Width / 2;
            int y0 = rt.Height / 2;
            int w0 = rt.Width / 3;
            int h0 = rt.Height / 3;
            int r0 = Math.Min(w0, h0);

            Rectangle rt_cue = new Rectangle(x0 - r0 / 2, y0 - r0 / 2, r0, r0);

            // draw task
            if (mi_step == MI_STEP.MI_Prepare) { // +
                task_pen.EndCap = task_pen.StartCap;
                g.DrawLine(task_pen, x0 - r0 / 2, y0, x0 + r0 / 2, y0);
                g.DrawLine(task_pen, x0, y0 - r0 / 2, x0, y0 + r0 / 2);
            } else if (mi_step == MI_STEP.MI_Cue) { //
                var scap = task_pen.EndCap;
                task_pen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
                switch (mi_act) {
                    case MIAction.Left:
                        g.DrawLine(task_pen, x0 + r0 / 2, y0, x0 - r0 / 2, y0);
                        break;
                    case MIAction.Right:
                        g.DrawLine(task_pen, x0 - r0 / 2, y0, x0 + r0 / 2, y0);
                        break;
                    case MIAction.MITongue:
                        g.DrawLine(task_pen, x0, y0 + r0 / 2, x0, y0 - r0 / 2);
                        break;
                    case MIAction.Feet:
                        g.DrawLine(task_pen, x0, y0 - r0 / 2, x0, y0 + r0 / 2);
                        break;
                    case MIAction.Idle:
                        g.DrawEllipse(task_pen, x0 - r0 / 2, y0 - r0 / 2, r0, r0);
                        break;
                }
            }

            if (message != null) {
                if (msz <= 0) {
                    SizeF sf = g.MeasureString(message, Font);
                    int w = rt.Width * 3 / 4;
                    msz = Font.Size * w / sf.Width;
                }
                Font fnt = new Font(Font.FontFamily, msz);

                StringFormat sfmt = new StringFormat();
                sfmt.LineAlignment = sfmt.Alignment = StringAlignment.Center;
                g.DrawString(message, fnt, Brushes.PowderBlue, rt, sfmt);
            }

            // for rest or pause
            int t0 = prog_time;
            int t1 = prog_now;
            if (t0 > 0) {
                h0 = 40;
                y0 = rt.Height - h0 - 5;
                r0 = rt.Width / 2;
                g.DrawRectangle(Pens.Blue, x0 - r0 / 2, y0, r0, h0);
                g.FillRectangle(Brushes.Blue, x0 - r0 / 2, y0,
                    t1 * r0 / t0, h0);
            }

            // for timer
            if (mi_step != MI_STEP.MI_None && trial_start > 0) {
                int t = (BCIApplication.ElaspedMilliSeconds - trial_start) / 1000;
                g.DrawString(t.ToString(), this.Font, Brushes.Blue, 10, rt.Height - 30);
            }

            if (!string.IsNullOrEmpty(trial_message)) {
                StringFormat sfmt = new StringFormat();
                sfmt.LineAlignment = sfmt.Alignment = StringAlignment.Near;
                g.DrawString(trial_message, Font, Brushes.Blue, rt, sfmt);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            message = null;
            if (e.KeyCode == Keys.D1) {
                MI_STEP ns = mi_step;
                MIAction na = mi_act;
                if (ns == MI_STEP.MI_Cue) {
                    if (na >= MIAction.Idle) ns++;
                    else na++;
                } else if (ns == MI_STEP.MI_Rest) {
                    ns = MI_STEP.MI_Prepare;
                } else {
                    ns++;
                    if (ns == MI_STEP.MI_Cue) na = MIAction.None;
                }

                (this as FeedbackInterface).StartTrial(na, 1);
                (this as FeedbackInterface).SetMIStep(ns, 2000);

                msz = 16.0F;
                trial_message = ns.ToString();
                if (ns == MI_STEP.MI_Cue) trial_message += "," + na.ToString();
            }

            base.OnKeyDown(e);
        }

        Point org_start, org_cur;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && WindowState == FormWindowState.Normal) {
                org_start = Location;
                org_cur = Cursor.Position;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && WindowState == FormWindowState.Normal) {
                Left = org_start.X + (Cursor.Position.X - org_cur.X);
                Top = org_start.Y + (Cursor.Position.Y - org_cur.Y);
            }
        }

        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            this.Invalidate();
        }
    }
}
