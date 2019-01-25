using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BCILib.Util;
using BCILib.App;

namespace BCILib.MotorImagery.ArtsBCI {
    public class FB_Accumulate :  UserControl {
        /// <summary>
        /// Threshold for accumulated total score
        /// </summary>
        protected double[] _max_vals = { 400, 400 };

        protected double[] _thr_vals = { 300, 300 };

        /// <summary>
        /// Accumulated total score
        /// </summary>
        protected double[] _cur_vals = { 0, 0 };

        /// <summary>
        /// Process started
        /// </summary>
        protected bool _started = false;

        /// <summary>
        /// gain for action class
        /// </summary>
        protected double _cfg_gain0 = 1.0;
        /// <summary>
        /// gain for idle class
        /// </summary>
        protected double _cfg_gain1 = 1.0;

        /// <summary>
        /// Process time window in seconds
        /// </summary>
        protected int _cfg_proc_time = 2000;
        /// <summary>
        /// threshold factor
        /// </summary>
        protected double _cfg_threshold_factor = 0.8;
        protected double _cfg_max_score = 0;

        /// <summary>
        /// Maximum contribution score
        /// </summary>
        protected readonly double MAX_SCORE = 2;

        public FB_Accumulate()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!this.DesignMode)
            {

                LoadConfig();
            }
        }

        private void LoadConfig()
        {
            ResManager rm = BCIApplication.SysResource;
            rm.GetConfigValue("BCI_MI_FLD", "Threshold_Factor", ref _cfg_threshold_factor);
            rm.SaveIfChanged();
        }

        Action<double[]> dlg_out_score = null;
        FLDProcessor _proc = null;

        internal void SetProcessor(FLDProcessor fLDProcessor, int act_time)
        {
            if (act_time > 0) {
                _cfg_proc_time = act_time;
            }

            if (_proc != null && dlg_out_score != null) {
                _proc.evt_out_score -= dlg_out_score;
                _proc = null;
                dlg_out_score = null;
            }

            _proc = fLDProcessor;
            if (_proc == null) return;

            _max_vals[0] = _max_vals[1] = _cfg_proc_time 
                * _proc.Amplifier.header.samplingrate * MAX_SCORE 
                * _cfg_threshold_factor / 1000;

            if (dlg_out_score == null)
                dlg_out_score = new Action<double[]>(ReceiveScore);

            _proc.evt_out_score += dlg_out_score;
        }

        internal void Reset()
        {
            SetTestSuccess(TestKeyAction.None);
            _started = false;
            _cur_vals[0] = _cur_vals[1] = 0;
            s_mode = -1;
            Invalidate();
        }

        internal virtual bool ResultReached()
        {
            throw new NotImplementedException();
        }

        protected virtual void ReceiveScore(double[] score)
        {
            int n = score.Length;

            foreach (double v in score) {
                double sv = v;
                double av = Math.Abs(sv);
                if (_cfg_max_score < av) _cfg_max_score = av;

                if (!_started) continue;

                if (_test_key_act == TestKeyAction.Sucess) {
                    sv = av = 1;
                } else if (_test_key_act == TestKeyAction.Fail) {
                    sv = -1;
                    av = 1;
                }

                if (sv > 0) {
                    av *= _cfg_gain0;
                    if (av > MAX_SCORE) av = MAX_SCORE;
                    _cur_vals[0] += av;
                } else {
                    av *= _cfg_gain1;
                    if (av > MAX_SCORE) av = MAX_SCORE;
                    _cur_vals[1] += av;
                }
            }

            Invalidate();
        }

        internal virtual int GetDetection(out double score)
        {
            throw new NotImplementedException();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // FB_Accumulate
            // 
            this.DoubleBuffered = true;
            this.Name = "FB_Accumulate";
            this.ResumeLayout(false);

        }

        protected TestKeyAction _test_key_act = TestKeyAction.None;

        internal void SetTestSuccess(TestKeyAction rst)
        {
            _test_key_act = rst;
        }

        protected MI_STEP _mi_step = MI_STEP.MI_None;

        internal virtual void SetMIStep(MI_STEP mi_step)
        {
            _mi_step = mi_step;
            if (_mi_step < MI_STEP.MI_Action) s_mode = -1;
        }

        internal virtual void StartAccuScore()
        {
            _started = true;
        }

        protected bool _cfg_dsp_fld_score = true;

        internal void DisplayFLDScore(bool on)
        {
            _cfg_dsp_fld_score = on;
        }


        protected double s_score = -10;
        protected int s_mode = -1;

        internal virtual void UpdateScore(double s, int mode)
        {
            s_score = s;
            if (mode != -1) s_mode = mode;
        }
    }

    public enum TestKeyAction
    {
        None, Sucess, Fail
    };
}
