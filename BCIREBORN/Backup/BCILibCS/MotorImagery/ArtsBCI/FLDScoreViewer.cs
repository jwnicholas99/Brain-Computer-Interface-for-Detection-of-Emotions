using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BCILib.Amp;
using BCILib.App;

namespace BCILib.MotorImagery.ArtsBCI
{
    public partial class FLDScoreViewer : UserControl
    {
        public FLDScoreViewer()
        {
            SetStyle(ControlStyles.UserPaint 
                | ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw,
                true);

            InitializeComponent();
        }

        enum DisplayMode { Moving, Wrapping };

        // pre settings
        /// <summary>
        /// Display mode, 0 = shift, 1 = fix
        /// </summary>
        DisplayMode disp_mode = DisplayMode.Moving;
        /// <summary>
        /// Full screen display time
        /// </summary>
        int disp_seconds = 5;

        public double max_scale = 0;

        bool LockBuffer = false;
        double[] draw_score = null;
        double[] draw_ascore = null;
        double[] draw_avg = null;
        bool[] draw_contribute = null;

        int draw_pos = 0;
        ulong num_score = 0;
        bool ChangeDispBuffer = false;
        Pen pen_avg = null;

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle rc = this.ClientRectangle;

            if (pen_avg == null) {
                pen_avg = new Pen(Color.Green, 10);
            }

            try {
                if (!LockBuffer) {
                    LockBuffer = true;

                    //g.FillRectangle(Brushes.White, rc);

                    int y0 = rc.Height / 2;
                    string label = string.Format("FLD Score, Scale={0:F2}", max_scale);
                    g.DrawString(label, this.Font, Brushes.Red, 5, Font.Size);

                    g.DrawLine(Pens.Blue, 0, y0, rc.Width - 1, y0);

                    int cpos = draw_pos;
                    int cp0 = accu_dp0, cp1 = accu_dp1, anum = accu_dn;

                    if (draw_score != null && num_score > 1) {
                        int call = draw_score.Length;

                        ulong np = (ulong)call;
                        if (np > num_score) np = num_score;

                        double x_factor = ((double)rc.Width) / (double)call;

                        PointF[] score_plg = null;

                        if (disp_mode == DisplayMode.Moving) score_plg = new PointF[np];

                        int p0 = 0;
                        double offy = 0;
                        double maxy = max_scale;

                        double y_factor = ((double)rc.Height) / (2 * maxy);
                        double x_pos;
                        double y_pos = y0;
                        if (num_score >= (ulong)draw_score.Length) {
                            if (disp_mode == DisplayMode.Wrapping) {
                                score_plg = new PointF[call - cpos];
                                x_pos = cpos * x_factor;
                            } else {
                                x_pos = 0;
                            }

                            for (int ip = cpos; ip < call; ip++) {
                                score_plg[p0].X = (float)x_pos;
                                x_pos += x_factor;
                                score_plg[p0].Y = (float)(y_pos - (draw_score[ip] - offy) * y_factor);

                                if (draw_avg[ip] >= 0) {
                                    g.DrawLine(Pens.Yellow, score_plg[p0].X, 0,
                                        score_plg[p0].X, rc.Height);
                                    int dv = (int) ((draw_avg[ip] - 0.5) * rc.Height / 2);
                                    pen_avg.Color = dv >= 0 ? Color.Green : Color.Red;
                                    pen_avg.Width = draw_contribute[ip]? 10:4;
                                    g.DrawLine(pen_avg, score_plg[p0].X, y0,
                                        score_plg[p0].X, y0 - dv);
                                }

                                p0++;
                            }
                            if (disp_mode == DisplayMode.Wrapping && p0 > 1) {
                                g.DrawLines(Pens.White, score_plg);
                            }
                        }

                        if (disp_mode == DisplayMode.Wrapping) {
                            p0 = 0;
                            x_pos = 0;
                            score_plg = new PointF[cpos];
                        } else {
                            x_pos = x_factor * (call - cpos);
                        }

                        for (int ip = 0; ip < cpos; ip++) {
                            if (p0 < 0 || p0 >= score_plg.Length) {
                                Console.WriteLine("Paint error p0 out of range!");
                            } else {
                                score_plg[p0].X = (float)x_pos;
                                x_pos += x_factor;
                                score_plg[p0].Y = (float)(y_pos - (draw_score[ip] - offy) * y_factor);
                            }

                            if (draw_avg[ip] >= 0) {
                                g.DrawLine(Pens.Yellow, score_plg[p0].X, 0,
                                    score_plg[p0].X, rc.Height);
                                int dv = (int)((draw_avg[ip] - 0.5) * rc.Height / 2);
                                pen_avg.Color = dv >= 0 ? Color.Green : Color.Red;
                                pen_avg.Width = draw_contribute[ip] ? 10 : 4;
                                g.DrawLine(pen_avg, score_plg[p0].X, y0,
                                    score_plg[p0].X, y0 - dv);
                            }

                            p0++;
                        }

                        try {
                            if (p0 > 1) {
                                g.DrawLines(Pens.White, score_plg);
                            }
                        } catch (Exception) {
                            Console.WriteLine("Something wrong here!");
                        }

                        // draw accumulated score
                        np = 0;
                        if (anum > 1) np = (ulong) ((cp1 - cp0 + 1 + call) % call);
                        if (np > 1) {
                            if (disp_mode == DisplayMode.Moving) score_plg = new PointF[np];

                            p0 = 0;
                            int dp0 = cp0;
                            if (cp1 < cp0) {
                                // draw from cp0 -> call
                                if (disp_mode == DisplayMode.Wrapping) {
                                    score_plg = new PointF[call - cp0];
                                    x_pos = cp0 * x_factor;
                                } else {
                                    x_pos = (cp0 - cpos) * x_factor;
                                }

                                for (int ip = cp0; ip < call; ip++) {
                                    score_plg[p0].X = (float)x_pos;
                                    x_pos += x_factor;
                                    score_plg[p0].Y = (float)(y_pos - (draw_ascore[ip] - offy) * y_factor);
                                    p0++;
                                }
                                if (disp_mode == DisplayMode.Wrapping && p0 > 1) {
                                    g.DrawLines(Pens.Green, score_plg);
                                }
                                dp0 = 0;
                            }

                            // from dp0 -> accu_dp1
                            if (disp_mode == DisplayMode.Wrapping) {
                                p0 = 0;
                                x_pos = dp0 * x_factor;
                                score_plg = new PointF[cp1 - dp0 + 1];
                            } else {
                                if (cpos < dp0) {
                                    x_pos = (dp0 - cpos) * x_factor;
                                } else if (cpos >= cp1) {
                                    x_pos = (call - cpos + dp0) * x_factor;
                                } else {
                                    Console.WriteLine("Error here");
                                }
                            }

                            for (int ip = dp0; ip <= cp1; ip++) {
                                if (p0 < 0 || p0 >= score_plg.Length) {
                                    Console.WriteLine("Paint error p0 out of range!");
                                    break;
                                } else {
                                    score_plg[p0].X = (float)x_pos;
                                    x_pos += x_factor;
                                    score_plg[p0].Y = (float)(y_pos - (draw_ascore[ip] - offy) * y_factor);
                                    p0++;
                                }
                            }

                            if (p0 != score_plg.Length) {
                                Console.WriteLine("Error!");
                            } else {
                                g.DrawLines(Pens.Green, score_plg);
                            }
                        }
                        // end accumulated score

                        // draw threshold
                        int y1 = rc.Height;
                        if (fld_dt > 0) {
                            y1 = rc.Height / 2 - (int)(fld_thr * rc.Height / (2 * fld_dt));
                        }
                        g.DrawEllipse(Pens.Blue, rc.Width - 20, y1 - 5, 10, 10);

                        // draw current score
                        if (accu_num > 0 && fld_dt > 0) {
                            double rs = accu_sum * 10 / fld_dt;
                            if (accu_num < accu_tstart) {
                                rs += (10 + s_bias) * accu_num / accu_tstart - 10;
                            } else {
                                rs += s_bias;
                            }

                            y1 = rc.Height / 2 - (int)(rc.Height * rs / 20);

                            if (y1 < 5) y1 = 5;
                            else if (y1 > rc.Height - 5) y1 = rc.Height - 5;

                            if (accu_started) {
                                g.FillEllipse(accu_num < accu_tstart ? Brushes.Green : Brushes.Red, rc.Width - 20, y1 - 5, 10, 10);
                            } else {
                                g.DrawEllipse(Pens.Red, rc.Width - 20, y1 - 5, 10, 10);
                            }
                        }

                        // time mark (s)
                        int steps = call / disp_seconds;
                        for (int ip = 0; ip < call; ip += steps) {
                            if (num_score < (ulong)draw_score.Length && ip >= cpos) {
                                break;
                            }

                            float x0 = ip;
                            if (disp_mode == DisplayMode.Moving) {
                                if (num_score >= (ulong) draw_score.Length && ip >= cpos) x0 -= cpos;
                                else x0 += call - cpos;
                            }
                            x0 = (float)(x0 * x_factor);
                            g.DrawLine(Pens.Blue, x0, rc.Bottom - 10, x0, rc.Bottom);
                        }

                        if (disp_mode == DisplayMode.Wrapping) {
                            //draw front line
                            float x0 = (float)(cpos * x_factor);
                            g.DrawLine(Pens.Yellow, x0, rc.Top + 10, x0, rc.Bottom - 20);
                        }
                    }
                }

                LockBuffer = false;
            } catch (Exception ex) {
                Console.WriteLine("Display exception = " + ex.Message);
            }
        }

        Action<double[]> dlg_out_score = null;
        FLDProcessor _proc = null;

        internal void SetProcessor(FLDProcessor fLDProcessor)
        {
            if (_proc != null && dlg_out_score != null) {
                _proc.evt_out_score -= dlg_out_score;
                _proc = null;
                dlg_out_score = null;
            }


            _proc = fLDProcessor;
            if (_proc == null) return;

            // set default processing parameters
            accu_delay = _proc.NumSampleUsed / 2;

            BCILib.EngineProc.FLDModel mdl = _proc.GetFLDModel();

            int spls = _proc.Amplifier.header.samplingrate * 2;
            if (spls == 0)
            {
                Console.WriteLine("Amplifier not started yet.!");
                spls = _proc.NumSampleUsed;
            }

            fld_thr = mdl.thr;
            if (mdl.fld_r != null) {
                fld_dt = Math.Max(Math.Abs(mdl.fld_r[0]), Math.Abs(mdl.fld_r[1]));
                fld_dt = Math.Max(fld_dt, Math.Abs(fld_thr));
            }

            accu_tstart = spls  + mdl.atime;
            int tstop = accu_tstart + spls / 2;
            if (accu_tstop < tstop) accu_tstop = tstop;

            if (dlg_out_score == null)
                dlg_out_score = new Action<double[]>(ReceiveScore);

            _proc.evt_out_score += dlg_out_score;

            ChangeDispBuffer = true;

            Console.WriteLine("SetProcessor: accu_delay={0}, accu_score={1}, acct_end={2}",
                accu_delay, accu_tstart, accu_tstop);

            max_scale = 0;
            if (mdl.score_ranges != null) {
                if (mdl.score_ranges.Length > 2) {
                    double r1 = Math.Abs(mdl.score_ranges[2]);
                    if (max_scale < r1) max_scale = r1;
                }
                
                if (mdl.score_ranges.Length > 3) {
                    double r2 = Math.Abs(mdl.score_ranges[3]);
                    if (max_scale < r2) max_scale = r2;
                }

                if (score_range_changed != null) score_range_changed(max_scale);
            }
        }

        internal event Action<double> score_range_changed;

        void ReceiveScore(double[] score)
        {
            if (ChangeDispBuffer) {
                SetDispBuffer();
            }

            if (draw_score == null) return;

            int n = score.Length;
            int cpos = draw_pos;
            int nsz = draw_score.Length;

            bool scale_changed = false;

            for (int i = 0; i < n; i++) {
                double dv = Math.Abs(score[i]);
                if (max_scale < dv) {
                    max_scale = dv;
                    scale_changed = true;
                }

                draw_score[cpos] = score[i];
                draw_avg[cpos] = -1;

                if (accu_started) {
                    if (delay_no < accu_delay) delay_no++;
                    else {
                        accu_sum += score[i];
                        accu_dn++;

                        accu_num++;
                        draw_ascore[cpos] = accu_sum / accu_num;
                        if (accu_num == 1) accu_dp0 = cpos;

                        accu_dp1 = cpos;
                    }
                }

                cpos++;
                if (cpos >= nsz) {
                    cpos = 0;
                }

                if (cpos == accu_dp0 && accu_dn > 0) {
                    accu_dp0++;
                    if (accu_dp0 >= nsz) accu_dp0 = 0;
                    accu_dn--;
                }
            }

            if (scale_changed && score_range_changed != null) score_range_changed(max_scale);

            if (accu_started) {
                if (accu_num >= accu_tstop || (accu_num >= accu_tstart && accu_sum >= fld_thr)) {
                    accu_started = false;
                }

                if (dl_upd_score != null && fld_dt > 0) {
                    double rs = (accu_sum - fld_thr) * 10 / fld_dt;
                    int mode = 0;
                    if (accu_num < accu_tstart) {
                        rs += (10 + s_bias) * accu_num / accu_tstart - 10;
                    } else {
                        rs += s_bias;
                        mode++;
                        if (!accu_started) mode++;
                    }
                    dl_upd_score(rs, mode);
                }

                if (!accu_started) {
                    Calculate_Score();
                }
            }

            num_score += (ulong) n;
            draw_pos = cpos;
            Invalidate();
        }

        private void SetDispBuffer()
        {
            if (_proc == null) return;
            Amplifier amp = _proc.Amplifier;
            if (amp == null) return;
            if (amp.header.samplingrate == 0) return;
            if (LockBuffer) return;

            LockBuffer = true;

            int nspl = disp_seconds * amp.header.samplingrate;

            if (draw_score == null || draw_score.Length != nspl)
            {
                double[] new_score = new double[nspl];
                double[] new_avg = new double[nspl];
                double[] new_ascore = new double[nspl];

                for (int i = 0; i < nspl; i++) new_avg[i] = -1;

                draw_pos = 0;
                num_score = 0;

                draw_score = new_score;
                draw_avg = new_avg;
                draw_contribute = new bool[nspl];

                draw_ascore = new_ascore;
            }

            ChangeDispBuffer = false;
            LockBuffer = false;
        }

        // Start cccumulating score:
        double accu_sum;
        int accu_num, accu_dn;

        // queue pointer
        int accu_dp0 = -1;
        int accu_dp1 = -1;

        /// <summary>
        /// accumulating time in milliseconds, defined by model after training
        /// </summary>
        int accu_tstart = 500;

        /// <summary>
        /// 2s more than accu_tstart
        /// </summary>
        int accu_tstop = 1000;

        int accu_label = -1;
        bool accu_started = false;
        int accu_delay = 0;
        int delay_no = 0;

        Action<double> dl_accu_score = null;

        /// <summary>
        /// for score updating: parameters score, mode = (0-started, accumulating, 1-testing, 2-time reached)
        /// </summary>
        internal Action<double, int> dl_upd_score = null;

        double fld_thr = 0;
        double fld_dt = 0;

        /// <summary>
        /// Start accumulating score. If need call SetAccuTimes to set timing parameters
        /// </summary>
        /// <param name="label"></param>
        /// <param name="rst_dt"></param>
        public void StartAccumulate(int label, Action<double> rst_dt)
        {
            accu_sum = 0;
            accu_num = 0;
            accu_dn = 0;
            dl_accu_score = rst_dt;

            delay_no = 0;
            accu_started = true;
            accu_label = label;
        }

        /// <summary>
        /// output score = raw_score + s_bias
        /// </summary>
        double s_bias = 0;
        int s_true = 0;

        public void Initialize()
        {
            _label_his.Clear();
            _score_his.Clear();

            s_true = 0;
        }

        List<int> _label_his = new List<int>();
        List<double> _score_his = new List<double>();

        private void Calculate_Score()
        {
            double score = 0;
            if (fld_dt > 0) score = (accu_sum - fld_thr) * 10 / fld_dt;

            // add to history
            _label_his.Add(accu_label);
            _score_his.Add(score);

            score += s_bias;
            if (accu_label == (score >= 0 ? 0 : 1)) s_true++;

            if (dl_accu_score != null) dl_accu_score(score);
        }

        internal double Calculate_BestBias(out double acc)
        {
            double bias = 0;
            acc = 0;

            int max_true = 0;

            int n = _score_his.Count;
            if (n == 0) return bias;

            // testing bias
            List<double> bl = new List<double>();
            {
                double[] bv = new double[n + 2];
                _score_his.CopyTo(bv, 1);
                Array.Sort<double>(bv, 1, n);
                bv[0] = bv[1] - 1;
                bv[n] = bv[n - 1] + 1;

                bias = 0;

                // truncate digits
                for (int bi = 1; bi < n + 2; bi++) {
                    double dv = bv[bi] - bv[bi - 1];
                    if (dv <= 0) {
                        continue;
                    }

                    double g = 1;
                    for (; dv <= 2; g *= 10, dv *= 10) ;

                    dv = (bv[bi - 1] + bv[bi]) / 2;
                    dv = Math.Round(dv * g) / g;
                    bl.Add(dv);
                }
            }

            for (int bi = 0; bi < bl.Count; bi++) {
                int nright = 0;
                for (int i = 0; i < n; i++) {
                    double iscore = _score_his[i];
                    int ilabel = _label_his[i];
                    int ipredict = iscore >= bl[bi] ? 0 : 1;
                    if (ipredict == ilabel) nright++;
                }
                if (nright > max_true) {
                    max_true = nright;
                    bias = -bl[bi];
                }
            }

            acc = max_true * 100.0 / n;

            return bias;
        }

        public void SetBias(double bias)
        {
            s_bias = bias;

            // recalculate
            s_true = 0;
            int n = _score_his.Count;
            for (int i = 0; i < n; i++) {
                if (_label_his[i] == (_score_his[i] + s_bias >= 0 ? 0 : 1)) s_true++;
            }

            if (dl_upd_score != null && fld_dt > 0) {
                dl_upd_score((accu_sum - fld_thr) * 10 / fld_dt + s_bias, -1);
            }
        }

        public double Accuracy
        {
            get
            {
                double acc = 0;

                int n = _score_his.Count;
                if (n > 0) acc = 100.0 * s_true / n;

                return acc;
            }
        }
        internal string GetFeedbackMessage()
        {
            return GetFeedbackMessage("Action", "Idle");
        }

        public string GetFeedbackMessage(string task1, string task2)
        {
            int ntotal = _score_his.Count;
            if (ntotal <= 0) return null;

            double score = _score_his[ntotal - 1] + s_bias;
            bool correct = (accu_label == (score >= 0 ? 0 : 1));

            return string.Format("Total/Correct={0}/{1}, Acc={2:#0.##}, {3}/{4:#0.##} {5}",
                ntotal, s_true, s_true * 100.0 / ntotal,
                accu_label==0?task1:task2, score, correct ? '\u221A' : 'X');
        }

        public string GetAccMessage()
        {
            int ntotal = _score_his.Count;
            if (ntotal <= 0) return null;

            return string.Format("Total/Correct={0}/{1}, Acc={2:P}",
                ntotal, s_true, s_true / (double) ntotal);
        }

        public int TotalTrials
        {
            get
            {
                return _score_his.Count;
            }
        }

        /// <summary>
        /// Set processing timing parameters
        /// </summary>
        /// <param name="t_delay">time in milliseconds to start score accumulation</param>
        /// <param name="t_accumax">extra time in milliseconds to accumulate before reaching threshold</param>
        public void SetAccuTimes(int t_delay, int t_accumax)
        {
            accu_delay = t_delay * _proc.Amplifier.header.samplingrate / 1000;
            accu_tstop = accu_tstart + t_accumax * _proc.Amplifier.header.samplingrate / 1000;
        }
    }
}
