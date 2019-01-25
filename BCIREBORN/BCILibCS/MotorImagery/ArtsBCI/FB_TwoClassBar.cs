using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.IO;
using BCILib.Util;

namespace BCILib.MotorImagery.ArtsBCI {
    public partial class FB_TwoClassBar : FB_Accumulate {

        // variables for display online FLD score
        private bool _change_data_size = true;
        private double[] _score_data = null;
        private int _score_pos = 0;
        private int _score_num = 0;

        public FB_TwoClassBar()
        {
            InitializeComponent();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
        }

        protected override void ReceiveScore(double[] score)
        {
            if (score == null || score.Length == 0) return;

            base.ReceiveScore(score);

            if (_change_data_size) {
                _score_data = new double[ClientRectangle.Width];
                _score_num = 0;
                _score_pos = 0;
                _change_data_size = false;
            }

            double ds = 0;
            foreach (double sv in score) {
                ds += sv;
            }
            ds /= score.Length;

            if (ds > _cfg_max_score) ds = _cfg_max_score;
            else if (ds < -_cfg_max_score) ds = -_cfg_max_score;
            _score_data[_score_pos++] = ds;
            if (_score_pos >= _score_data.Length) _score_pos = 0;
            _score_num += 1;

            if (_sock != null && _sock.Connected) {
                _sw.WriteLine("OnlineScore {0:#.0#}", ds);
            }

            Invalidate();
        }

        //protected override void OnPaint(PaintEventArgs e)
        //{
        //    Graphics g = e.Graphics;
        //    Rectangle rt = ClientRectangle;

        //    // Border
        //    Pen pen;
        //    if (_mi_step == MI_STEP.MI_Rest) {
        //        pen = (_cur_vals[0] >= _cur_vals[1]) ? Pens.Yellow : Pens.DarkBlue;
        //    } else {
        //        pen = Pens.AliceBlue;
        //    }
        //    g.DrawRectangle(pen, 0, 0, rt.Width - 1, rt.Height - 1);
        //    int y1 = rt.Height / 2;
        //    g.DrawLine(Pens.AliceBlue, 0, y1, rt.Width, rt.Height / 2);

        //    // action accumulated score
        //    int h1 = y1 - 1;
        //    int l = (int) (_cur_vals[0] * h1 / _max_vals[0]);
        //    g.FillRectangle(Brushes.Green, 1, y1 - l, rt.Width - 2, l);

        //    // rest accumulated score
        //    int h2 = rt.Height - 2 - y1;
        //    l = (int) (_cur_vals[1] * h2 / _max_vals[1]);
        //    g.FillRectangle(Brushes.Red, 1, y1 + 1, rt.Width - 2, l);

        //    if (_cfg_dsp_fld_score && !_change_data_size) {
        //        // Instant FLD score
        //        g.DrawLine(Pens.Blue, 0, rt.Height / 2, rt.Width, rt.Height / 2);

        //        int nl = _score_data.Length;
        //        int pos = _score_pos;
        //        if (_score_num < nl) nl = pos;

        //        double scare = rt.Height / 6;
        //        if (scare < 50) scare = 50;
        //        if (scare > rt.Height/2) scare = rt.Height/2;

        //        scare /= _cfg_max_score;
        //        if (nl >= 2) {
        //            Point[] plist = new Point[nl];
        //            int pi = 0;
        //            // nl > pos
        //            for (int p0 = pos; p0 < nl; p0++, pi++) {
        //                plist[pi].X = pi;
        //                plist[pi].Y = (int)(rt.Height / 2 - _score_data[p0] * scare);
        //            }

        //            for (int p0 = 0; p0 < pos; p0++, pi++) {
        //                plist[pi].X = pi + 2;
        //                plist[pi].Y = (int)(rt.Height / 2 - _score_data[p0] * scare);
        //            }

        //            g.DrawLines(Pens.White, plist);
        //        }
        //    }
        //}

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle rt = ClientRectangle;

            // Border
            Pen pen;
            if (s_mode == 2) {
                pen = (s_score >= 0) ? Pens.Yellow : Pens.DarkBlue;
            } else {
                pen = Pens.AliceBlue;
            }
            g.DrawRectangle(pen, 0, 0, rt.Width - 1, rt.Height - 1);

            int y1 = rt.Height / 2;
            g.DrawLine(Pens.AliceBlue, 0, y1, rt.Width, rt.Height / 2);

            // action accumulated score
            if (_mi_step == MI_STEP.MI_Action || _mi_step == MI_STEP.MI_Rest) {
                int sv = (int)Math.Round(y1 - (s_score * (rt.Height - 2)) / 20);
                if (sv > rt.Height - 1) sv = rt.Height - 1;
                else if (sv < 1) sv = 1;
                if (sv > y1) {
                    g.FillRectangle(Brushes.Green, 1, y1 + 1, rt.Width - 2, sv - y1 - 1);
                } else {
                    g.FillRectangle(Brushes.Red, 1, sv, rt.Width - 2, y1 - sv);
                }
            }

            if (_cfg_dsp_fld_score && !_change_data_size && _cfg_max_score > 0) {
                // Instant FLD score
                g.DrawLine(Pens.Blue, 0, rt.Height / 2, rt.Width, rt.Height / 2);

                int nl = _score_data.Length;
                int pos = _score_pos;
                if (_score_num < nl) nl = pos;

                double scare = rt.Height / 6;
                if (scare < 50) scare = 50;
                if (scare > rt.Height / 2) scare = rt.Height / 2;

                scare /= _cfg_max_score;
                if (nl >= 2) {
                    Point[] plist = new Point[nl];
                    int pi = 0;
                    // nl > pos
                    for (int p0 = pos; p0 < nl; p0++, pi++) {
                        plist[pi].X = pi;
                        plist[pi].Y = (int)(rt.Height / 2 - _score_data[p0] * scare);
                    }

                    for (int p0 = 0; p0 < pos; p0++, pi++) {
                        plist[pi].X = pi + 2;
                        plist[pi].Y = (int)(rt.Height / 2 - _score_data[p0] * scare);
                    }

                    g.DrawLines(Pens.White, plist);
                }
            }
        }

        internal override int GetDetection(out double score)
        {
            _started = false;

            double dsum = _cur_vals[0] + _cur_vals[1];
            if (dsum == 0) {
                score = 0.5;
                return 0;
            }

            double score0 = _cur_vals[0] / (_cur_vals[0] + _cur_vals[1]);
            double score1 = 1 - score0;

            score = Math.Max(score0, score1);
            return score0 >= score1 ? 0 : 1;
        }

        internal override bool ResultReached()
        {
            return (_cur_vals[0] >= _max_vals[0] || _cur_vals[1] >= _max_vals[1]);
        }

        Socket _sock = null;
        SocketReadWrite _sw = null;

        internal void SetFBSocket(Socket s, SocketReadWrite sw)
        {
            _sw = sw;
            _sock = s;
        }
    }
}
