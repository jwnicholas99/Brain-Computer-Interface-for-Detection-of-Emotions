using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using BCILib.Util;
using BCILib.App;

namespace BCILib.MotorImagery.ArtsBCI {
    public partial class FB_RehabScoreBar : BCILib.MotorImagery.ArtsBCI.FB_Accumulate {

        private double _cfg_rehab_threshold = 0.66;

        private bool _change_data_size = true;
        private double[] _score_data = null;
        private int _score_pos = 0;
        private int _score_num = 0;

        public FB_RehabScoreBar()
        {
            InitializeComponent();

            font_sym = new Font("Symbol", 28);
            fmt_msg = new StringFormat();
            fmt_msg.Alignment = StringAlignment.Center;
            fmt_msg.LineAlignment = StringAlignment.Center;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!DesignMode) {
                LoadConfig();
            }
        }

        private void LoadConfig()
        {
            ResManager rm = BCIApplication.SysResource;
            rm.GetConfigValue("BCI_MI_FLD", "Rehab_Threshold_Factor", ref _cfg_rehab_threshold);
            rm.SaveIfChanged();
        }

        private void SaveConfig()
        {
            ResManager rm = BCIApplication.SysResource;
            rm.SetConfigValue("BCI_MI_FLD", "Rehab_Threshold_Factor", _cfg_rehab_threshold.ToString());
            rm.SaveFile();
        }

        Pen pen = new Pen(Color.OrangeRed, 5);
        Font font_sym = null;
        StringFormat fmt_msg = null;

        int h_threshold;

        protected override void OnResize(EventArgs e)
        {
            _change_data_size = true;
            h_threshold = (int)(_cfg_rehab_threshold * ClientRectangle.Height / 3);

            //font_sym = new Font("Symbol", Height / 8);
            //Invalidate();
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

            Invalidate();
        }

        private int _tblink = 0;

        //protected override void OnPaint(PaintEventArgs e)
        //{
        //    Graphics g = e.Graphics;
        //    Rectangle rt = ClientRectangle;

        //    // border
        //    pen.Color = Color.OrangeRed;
        //    pen.Width = 5;
        //    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
        //    g.DrawRectangle(pen, 0, 0, rt.Width - 1, rt.Height - 1);

        //    // accumulated score
        //    int l = (int) (rt.Height + rt.Height * 2 * _cur_vals[0] / _max_vals[0]) / 3;
        //    Color clr;
        //    if (_cur_vals[0] >= _max_vals[0] * _cfg_rehab_threshold) {
        //        clr = Color.Yellow;
        //    } else if (_mi_step == MI_STEP.MI_Rest) {
        //        clr = Color.DarkBlue;
        //    } else { 
        //        clr = Color.FromArgb(150, Color.Green);
        //    }
        //    g.FillRectangle(new SolidBrush(clr), 2, (rt.Height - l) / 2, rt.Width-5, l);

        //    // Starting Position
        //    l = rt.Height / 3;
        //    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
        //    pen.Width = 1;
        //    int y0 = (rt.Height - l) / 2;
        //    g.DrawLine(pen, 0, y0, rt.Width, y0);
        //    y0 += l;
        //    g.DrawLine(pen, 0, y0, rt.Width, y0);

        //    // Open Arrows - cue
        //    if (_mi_step == MI_STEP.MI_Prepare || _mi_step == MI_STEP.MI_Action) {
        //        g.DrawString(new string((char)0xDD, 1), font_sym, Brushes.Red,
        //            new Rectangle(rt.Left, rt.Top, rt.Width, rt.Height / 3), fmt_msg);
        //        g.DrawString(new string((char)0xDF, 1), font_sym, Brushes.Red,
        //            new Rectangle(rt.Left, rt.Bottom - rt.Height / 3 - 1,
        //                rt.Width, rt.Height / 3), fmt_msg);
        //    }

        //    // Instant FLD score
        //    if (_cfg_dsp_fld_score && !_change_data_size) {
        //        g.DrawLine(Pens.Blue, 2, rt.Height / 2, rt.Width - 3, rt.Height / 2);

        //        int nl = _score_data.Length;
        //        int pos = _score_pos;
        //        if (_score_num < nl) nl = pos;
        //        double scare = rt.Height / _cfg_max_score / 6;
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

        //    // threshold lines
        //    y0 = rt.Height / 3 - h_threshold;
        //    pen.Color = Color.Gold;
        //    pen.Width = 3;
        //    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
        //    g.DrawLine(pen, 2, y0, rt.Width-3, y0);

        //    y0 = 2 * rt.Height / 3 + h_threshold;
        //    g.DrawLine(pen, 2, y0, rt.Width-3, y0);

        //    // mouse hover
        //    //if (_thr_hover != 0) {
        //    //    clr = Color.FromArgb(128, Color.OrangeRed);
        //    //    g.FillRectangle(new SolidBrush(clr), 0, _thr_adjpos - 4, rt.Width, 9);
        //    //}
        //}

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle rt = ClientRectangle;

            // border
            pen.Color = Color.OrangeRed;
            pen.Width = 5;
            pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            g.DrawRectangle(pen, 0, 0, rt.Width - 1, rt.Height - 1);

            // accumulated score, for action/rehab/rest
            int l;
            if (s_mode >= 0) { //_mi_step >= MI_STEP.MI_Action
                l = (int)(rt.Height * (2 + s_score / 10) / 3);
                if (l < rt.Height / 3) l = rt.Height / 3;
                if (l > rt.Height) l = rt.Height;

                Color clr;
                if (s_mode < 2) {
                    clr = Color.FromArgb(150, Color.Green);
                } else if (s_score >= 0) {
                    clr = Color.Yellow;
                } else {
                    clr = Color.DarkBlue;
                }
                g.FillRectangle(new SolidBrush(clr), 2, (rt.Height - l) / 2, rt.Width - 5, l);
            }

            // Starting Position
            l = rt.Height / 3;
            pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            pen.Width = 1;
            int y0 = (rt.Height - l) / 2;
            g.DrawLine(pen, 0, y0, rt.Width, y0);
            y0 += l;
            g.DrawLine(pen, 0, y0, rt.Width, y0);

            // Open Arrows - cue
            if (_mi_step == MI_STEP.MI_Prepare || _mi_step == MI_STEP.MI_Action) {
                g.DrawString(new string((char)0xDD, 1), font_sym, Brushes.Red,
                    new Rectangle(rt.Left, rt.Top, rt.Width, rt.Height / 3), fmt_msg);
                g.DrawString(new string((char)0xDF, 1), font_sym, Brushes.Red,
                    new Rectangle(rt.Left, rt.Bottom - rt.Height / 3 - 1,
                        rt.Width, rt.Height / 3), fmt_msg);
            }

            // Instant FLD score
            if (_cfg_dsp_fld_score && !_change_data_size && _cfg_max_score > 0) {
                g.DrawLine(Pens.Blue, 2, rt.Height / 2, rt.Width - 3, rt.Height / 2);

                int nl = _score_data.Length;
                int pos = _score_pos;
                if (_score_num < nl) nl = pos;
                double scare = rt.Height / _cfg_max_score / 6;
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

            // threshold lines
            y0 = rt.Height / 6;
            pen.Color = Color.Gold;
            pen.Width = 3;
            pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            g.DrawLine(pen, 2, y0, rt.Width - 3, y0);

            y0 = 5 * rt.Height / 6;
            g.DrawLine(pen, 2, y0, rt.Width - 3, y0);

            // mouse hover
            //if (_thr_hover != 0) {
            //    clr = Color.FromArgb(128, Color.OrangeRed);
            //    g.FillRectangle(new SolidBrush(clr), 0, _thr_adjpos - 4, rt.Width, 9);
            //}
        }

        #region adject_thr

        //int _thr_hover = 0;
        //int _thr_adjpos = 0;

        //protected override void OnMouseMove(MouseEventArgs e)
        //{
        //        int h = ClientRectangle.Height;
        //    if (e.Button == MouseButtons.None) {
        //        // hover
        //        int hover = 0;
        //        int d = e.Y - (h / 3 - h_threshold);
        //        if (d < 5 && d > -5) {
        //            _thr_adjpos = h / 3 - h_threshold;
        //            hover = 1;
        //        } else {
        //            d = e.Y - (h * 2 / 3 + h_threshold);
        //            if (d < 5 && d > -5) {
        //                _thr_adjpos = h * 2 / 3 + h_threshold;
        //                hover = 2;
        //            }
        //        }

        //        Cursor = hover == 0 ? Cursors.Default : Cursors.NoMoveVert;

        //        _thr_hover = hover;
        //        Invalidate();
        //    } else if (e.Button == MouseButtons.Left && _thr_hover != 0) {
        //        int dm = e.Y - _y_origin;
        //        if (dm != 0) {
        //            int ny, min, max;
        //            if (_thr_hover == 1) {
        //                min = 0;
        //                max = h / 3;
        //                ny = h / 3 - h_threshold;
        //            } else {
        //                min = 2 * h / 3;
        //                max = h;
        //                ny = 2 * h / 3 + h_threshold;
        //            }
        //            ny += dm;
        //            if (ny < min) ny = min;
        //            else if (ny > max) ny = max;
        //            _thr_adjpos = ny;
        //            Invalidate();
        //        }
        //    }
        //}

        //int _y_origin = -1;
        //protected override void OnMouseDown(MouseEventArgs e)
        //{
        //    if (e.Button == MouseButtons.Left && _thr_hover != 0) {
        //        _y_origin = e.Y;
        //    }
        //}

        //protected override void OnMouseUp(MouseEventArgs e)
        //{
        //    if (e.Button == MouseButtons.Left && _thr_hover != 0) {
        //        // reset threshold
        //        if (_thr_hover == 1) {
        //            h_threshold = Height / 3 - _thr_adjpos;
        //        } else {
        //            h_threshold = _thr_adjpos - 2 * Height / 3;
        //        }

        //        Invalidate();

        //        _cfg_rehab_threshold = h_threshold * 3.0 / ClientRectangle.Height;
        //        SaveConfig();
        //    }
        //}

        #endregion adject_thr

        /// <summary>
        /// Get detection result: 0 for class 0, 1 for class 1
        /// </summary>
        /// <param name="score">output 1/0</param>
        /// <returns></returns>
        internal override int GetDetection(out double score)
        {
            _started = false;
            _tblink = 0;
            score = 1.0;
            if (_cur_vals[0] >= _max_vals[0] * _cfg_rehab_threshold) {
                return 0;
            } else {
                return 1;
            }
        }

        internal override bool ResultReached()
        {
            return (_cur_vals[0] >= _max_vals[0] * _cfg_rehab_threshold);
        }

        internal override void StartAccuScore()
        {
            base.StartAccuScore();
            _tblink = BCIApplication.ElaspedMilliSeconds;
        }
    }
}
