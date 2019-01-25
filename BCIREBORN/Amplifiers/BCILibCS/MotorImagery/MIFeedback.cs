using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BCILib.App;
using System.Reflection;
using System.IO;

namespace BCILib.MotorImagery
{
    public partial class MIFeedback : UserControl
    {
        public MIFeedback()
        {
            InitializeComponent();
        }

        int[] _tasks = {1, 2, 3, 4, 5};
        ToolStripItem _msg_ctrl = null;

        /// <summary>
        /// Define configurations
        /// </summary>
        /// <param name="cfg_string">task configuration string</param>
        /// <param name="msg_ctrl">The control to display accuracy message</param>
        public void Configurate(string cfg_string, ToolStripItem msg_ctrl)
        {
            // cfg_string -> disp_tasks
            List<int> tasks = new List<int>(5);
            for (int i = 0; i < cfg_string.Length; i++ ) {
                if (cfg_string[i] != 'N') tasks.Add(i + 1);
            }
            _tasks = new int[tasks.Count];
            tasks.CopyTo(_tasks);

            _msg_ctrl = msg_ctrl;

            ApplyBias();
            UpdateDisplay();
        }

        int _label = 0;
        int _predict = 2;
        double _score = 0.8;

        int _ntrial = 0;
        int _ncorrect = 0;

        int _nseq = 0;

        // 2-Class Bias ccwang 20110216: Add bias to 2-class classification
        List<double> _score_his = new List<double>();
        List<int> _label_his = new List<int>();

        double _bias = 0;

        public double MIBias
        {
            set
            {
                _bias = value;
                ApplyBias();
                UpdateDisplay();
            }

            get
            {
                return _bias;
            }
        }

        /// <summary>
        /// score for class 0
        /// </summary>
        double s_score = 0.5;

        /// <summary>
        /// apply bias to s_score and set _predict and _score
        /// </summary>
        /// <param name="AddTrial"></param>
        private void ApplyBias()
        {
            _score = s_score + _bias;

            if (_score < 0) _score = 0;
            else if (_score > 1) _score = 1;

            int predict = 0;
            if (_score < 0.5) {
                predict = 1;
                _score = 1 - _score;
            }
            _predict = _tasks[predict];
            cCorrect = _label == _predict ? '\u221A' : 'X';

            _ntrial = _score_his.Count;
            _ncorrect = 0;

            for (int i = 0; i < _ntrial; i++) {
                double iscore = _score_his[i];
                int ilabel = _label_his[i];
                iscore += _bias;
                int ipredict = iscore >= 0.5 ? 0 : 1;
                if (_tasks[ipredict] == ilabel) _ncorrect++;
            }
        }

        internal int Predict
        {
            get
            {
                return _predict;
            }
        }

        char  cCorrect = ' ';

        /// <summary>
        /// Add testing sample
        /// </summary>
        /// <param name="label">Label as grand true, value: 1-5</param>
        /// <param name="result">Predicted class label, 0-nclass</nclass></param>
        /// <param name="score">Dicision score</param>
        /// <param name="AddSample">If true, result is taken to calculate accuracy</param>
        public void ShowScore(int label, int predict, double score, bool AddSample)
        {
            if (predict >= 0) {
                _label = label;
                s_score = predict == 0? score : 1-score;

                if (AddSample) {
                    _label_his.Add(_label);
                    _score_his.Add(s_score);
                    _nseq = 0;
                } else {
                    _nseq++;
                }

                ApplyBias();
            }

            UpdateDisplay();
        }

        /// <summary>
        /// Rest statistcs variables. 
        /// </summary>
        public void Reset()
        {
            _score_his.Clear();
            _label_his.Clear();

            _ntrial = _ncorrect = 0;
            ClearDisp();
        }

        private void UpdateDisplay()
        {
            if (InvokeRequired) {
                Invoke(new Action(UpdateDisplay));
            }
            else {
                Invalidate();

                if (_msg_ctrl != null) {
                    if (_predict > 0) {
                        _msg_ctrl.Text = string.Format("Correct/Total={0}/{1}, Acc={2:#0.##}, {4}/{3:#0.##} {5}",
                            _ncorrect, _ntrial, _ntrial > 0 ? _ncorrect * 100.0 / _ntrial : 0, _score,
                            (MIAction)(MIAction.Left + _predict - 1), cCorrect);
                    }
                }
            }
        }

        [Flags]
        public enum MI_FLAGS {
            None = 0,
            Left = 1,
            Right = 2,
            Tongue = 4,
            Feet = 8,
            Idle = 16
        };

        private MI_FLAGS _mi_flags = MI_FLAGS.Left | MI_FLAGS.Right | MI_FLAGS.Idle;

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle rt = ClientRectangle;
            int w = rt.Width / 2;
            int h = rt.Height / 2;
            int x0 = rt.X + w;
            int y0 = rt.Y + h;
            int hsw = 5;

            w -= hsw + hsw;
            h -= hsw + hsw;

            Pen pen = new Pen(Color.Yellow);
            for (int i = 0; i < 5; i++) {
                // 20110406: skip the hidden ones
                if ((_mi_flags & (MI_FLAGS) (1 << i)) == MI_FLAGS.None) continue;

                pen.DashStyle = Array.IndexOf(_tasks, i + 1) < 0? System.Drawing.Drawing2D.DashStyle.Dot : System.Drawing.Drawing2D.DashStyle.Solid;

                switch (i + 1) {
                    case 1: // Left
                        g.DrawRectangle(pen, x0 - w + hsw, y0 - hsw, w, hsw + hsw);
                        break;
                    case 2: //Right
                        g.DrawRectangle(pen, x0 - hsw, y0 - hsw, w, hsw + hsw);
                        break;
                    case 3: // Tongue
                        g.DrawRectangle(pen, x0 - hsw, y0 - h + hsw, hsw + hsw, h);
                        break;
                    case 4: // Feet
                        g.DrawRectangle(pen, x0 - hsw, y0 - hsw, hsw + hsw, h);
                        break;
                    case 5: // idle
                        g.DrawEllipse(pen, x0 - w/2, y0 - h/2, w, h);
                        break;
                    default:
                        break;
                }
            }

            // draw label
            switch (_label) {
                case 1: // Left
                    g.FillRectangle(Brushes.LightBlue, x0 - w + hsw, y0 - hsw, w, hsw + hsw);
                    break;
                case 2: //Right
                    g.FillRectangle(Brushes.LightBlue, x0 - hsw, y0 - hsw, w, hsw + hsw);
                    break;
                case 3: // Tongue
                    g.FillRectangle(Brushes.LightBlue, x0 - hsw, y0 - h + hsw, hsw + hsw, h);
                    break;
                case 4: // Feet
                    g.FillRectangle(Brushes.LightBlue, x0 - hsw, y0 - hsw, hsw + hsw, h);
                    break;
                case 5:
                    g.FillEllipse(Brushes.LightBlue, x0 - w / 2, y0 - h / 2, w, h);
                    break;
                default:
                    break;
            }

            // draw score
            int ls;
            switch (_predict) {
                case 1: // Left
                    ls = (int)(w * _score * .8) ;
                    g.FillRectangle(Brushes.Red, x0 - ls + hsw, y0 - hsw, ls, hsw + hsw);
                    break;
                case 2: //Right
                    ls = (int)(w * _score * .8);
                    g.FillRectangle(Brushes.Red, x0 - hsw, y0 - hsw, ls, hsw + hsw);
                    break;
                case 3: // Tongue
                    ls = (int)(h * _score * .8);
                    g.FillRectangle(Brushes.Red, x0 - hsw, y0 - ls + hsw, hsw + hsw, ls);
                    break;
                case 4: // Feet
                    ls = (int)(h * _score * .8);
                    g.FillRectangle(Brushes.Red, x0 - hsw, y0 - hsw, hsw + hsw, ls);
                    break;
                case 5:
                    int ls1 = (int)(w * _score * .8);
                    int ls2 = (int)(h * _score * .8);
                    g.FillEllipse(Brushes.Red, x0 - ls1/2, y0 - ls2/2, ls1, ls2);
                    break;
                default:
                    break;
            }

            if (_nseq > 0) {
                g.DrawString(_nseq.ToString(), this.Font, Brushes.White, x0 + hsw, y0 + h - 20);
            } else if (_score_his.Count > 0 && _label > 0 || DesignMode) {
                pen.Width = 2;
                g.DrawRectangle(pen, x0 - w, y0 - h, w + w, h + h);
            }
        }

        public bool IsVisible
        {
            set
            {
                if (InvokeRequired) {
                    this.Invoke((Action)delegate()
                    {
                        base.Visible = value;
                    });
                } else {
                    base.Visible = value;
                }
            }

            get
            {
                if (InvokeRequired) {
                    return (bool) this.Invoke((Func<bool>)delegate()
                    {
                        return this.Visible;
                    });
                } else {
                    return this.Visible;
                }
            }
        }

        internal void ClearDisp()
        {
            //_predict = _label = -1;
            _nseq = 0;
            _score = 0;
            _label = 0;
            UpdateDisplay();
        }

        internal int CorrectTrials
        {
            get
            {
                return _ncorrect;
            }
        }

        internal int TotalTrials
        {
            get
            {
                return _ntrial;
            }
        }

        internal int Prediction
        {
            get
            {
                return _predict;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
        }

        internal void CalculateBestBias(out double acc, out double bias)
        {
            acc = 0;
            bias = 0;

            int n = _score_his.Count;
            if (n == 0) return;

            List<double> bl = new List<double>();
            {
                double[] bv = new double[n + 2];
                bv[0] = 0;
                bv[1] = 1.1;
                _score_his.CopyTo(bv, 2);
                Array.Sort<double>(bv);

                bias = 0;

                for (int bi = 1; bi < n + 2; bi++) {
                    double dv = bv[bi] - bv[bi - 1];
                    if (dv <= 0) continue;

                    double g = 1;
                    for (; dv <= 2; g *= 10, dv *= 10) ;

                    dv = (bv[bi - 1] + bv[bi]) / 2;
                    dv = Math.Round(dv * g) / g;
                    bl.Add(dv);
                }
            }

            int max_true = 0;
            for (int bi = 0; bi < bl.Count; bi++) {
                int nright = 0;
                for (int i = 0; i < n; i++) {
                    double iscore = _score_his[i];
                    int ilabel = _label_his[i];
                    int ipredict = iscore >= bl[bi] ? 0 : 1;
                    if (_tasks[ipredict] == ilabel) nright++;
                }
                if (nright > max_true) {
                    max_true = nright;
                    bias = bl[bi];
                } else if (nright == max_true) {
                    if (Math.Abs(0.5 - bl[bi]) < Math.Abs(0.5 - bias)) {
                        bias = bl[bi];
                    }
                }
            }

            acc = max_true * 100.0 / n;
            bias = 0.5 - bias;
        }
    }
}
