using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BCILib.App;
using System.Threading;

namespace BCILib.Util {
    public partial class UC_HProgressBar : UserControl {
        public UC_HProgressBar()
        {
            InitializeComponent();

            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        }

        /// <summary>
        /// Set value maximum between 0 and 100
        /// </summary>
        /// <param name="value"></param>
        /// 


        int _val = 0;

        public int Value
        {
            set
            {
                _val = value;
                if (_val < 0) _val = 0;
                else if (_val > 100) _val = 100;
                Invalidate();
                Update();
            }

            get
            {
                return _val ;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_val > 0) {
                Graphics g = e.Graphics;
                Rectangle rt = ClientRectangle;
                rt.Width--;
                rt.Height--;
                g.DrawRectangle(Pens.Blue, rt);
                g.FillRectangle(Brushes.Blue, 0, 0, _val * rt.Width / 100, rt.Height);
            }
        }

        /// <summary>
        /// Return only after specified time elapsed
        /// </summary>
        /// <param name="time">total time</param>
        /// <param name="beep_before_end">time from end to beep, 0 no beep</param>
        public void ShowProgress(int time, int beep_before_end, ManualResetEvent _evt_StopProc)
        {
            Thread thd = new Thread((ThreadStart) delegate() {
                long t0 = HRTimer.GetTimestamp();
                int _prog_now = 0;
                while (_prog_now < time) {
                    _prog_now = HRTimer.DeltaMilliseconds(t0);
                    _val = _prog_now * 100 / time;
                    Invalidate();

                    int wt = time - _prog_now;
                    if (wt > 50) wt = 50;
                    else if (wt < 0) wt = 0;
                    if (_evt_StopProc.WaitOne(wt, false)) break;
                }

                _val = 0;
                Invalidate();
            });

            thd.Start();

            uint beep_freq = 300;
            uint beep_time = 500; // milliseconds,

            if (beep_before_end > 0)
            {
                int wt = time - beep_before_end;
                if (wt < 0) wt = 0;
                if (!_evt_StopProc.WaitOne(wt, false))
                {
                    Sound.BeepAsunc(beep_freq, beep_time);
                }
                wt = time - wt;
                if (wt < 0) wt = 0;
                _evt_StopProc.WaitOne(wt, false);
            }

            if (InvokeRequired) thd.Join();
            else while (!thd.Join(10)) Application.DoEvents();
        }
    }
}
