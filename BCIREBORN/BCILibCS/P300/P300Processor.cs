using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BCILib.App;
using System.Reflection;
using System.IO;
using System.Runtime.InteropServices;

namespace BCILib.P300
{
    class P300Processor : BCIProcessor
    {
        static private MethodInfo eeg_average = null;
        static private MethodInfo train_model = null;
        static private MethodInfo get_result = null;

        static P300Processor()
        {
            Assembly asb = BCIEngine.ASB_BCIProcEngine;
            Type p300util = asb.GetType("BCILib.Processor.P300Util", false, true);
            if (p300util != null) {
                eeg_average = p300util.GetMethod("EEGChannelAverage");
                train_model = p300util.GetMethod("TrainModel");
                get_result = p300util.GetMethod("GetResult");
            }
        }

        private List<int> revt1 = new List<int>();

        private bool out_p300_score(double score)
        {
            short evt = (short)(_rd_event);
            int rdx = rstims.IndexOf(evt);
            if (rdx >= 0) {
                rscores[rdx] = score;
                revt1.Add(_rd_event);
            } else {
                rstims.Add(evt);
                rscores.Add(score);
            }

            if (rscores.Count == _num_stim) {
                // full
                _list_stim.AddRange(rstims);
                _list_score.AddRange(rscores);
                Console.Write(_list_stim.Count / _num_stim);
                if (revt1.Count > 0) {
                    Console.WriteLine("Received extra = {0}", string.Join(",", revt1.Select(x => x.ToString()).ToArray()));
                }
                rstims.Clear();
                rscores.Clear();
                revt1.Clear();
            }

            return true;
        }

        delegate bool engine_out(double score);
        engine_out _outh = null;
        internal P300Processor()
            : base(BCIEngine.BCIProcType.P300)
        {
            _outh = new engine_out(out_p300_score);
            proc_engine.SetFeedbackHandler(_outh);
        }

        internal override bool Initialize(System.Collections.Hashtable parameters)
        {
            return false;
        }

        internal bool Initialize(P300ConfigCtrl cfg)
        {
            if (!proc_engine.Initialize(Path.Combine("Config", "System.cfg"))) return false;

            _num_stim = cfg.NumEpochPerRound;
            _num_round = cfg.NumRound;
            if (_num_stim > 0 && _num_round > 0 && _num_stim > _num_round) {
                _list_stim = new List<short>(_num_round * _num_stim);
                _list_score = new List<double>(_num_stim * _num_round);
                _list_stim.Clear();
                _list_score.Clear();
                return true;
            }
            return false;
        }

        private int _num_stim = 0;
        private int _num_round = 0;

        private List<short> _list_stim;
        private List<double> _list_score;
        private Func<int, double, bool> _houtput;

        private List<short> rstims = new List<short>();
        private List<double> rscores = new List<double>();

        protected override void ProcessSelectedData()
        {
            if (_rd_event > _num_stim) return;

            proc_engine.ProcEEGBuf(pc_buf, proc_engine.NumChannelUsed, proc_engine.NumSampleUsed);

            if (_list_stim.Count == _num_stim * _num_round) {
                // outout
                if (get_result != null) {
                    P300Result rst = (P300Result)get_result.Invoke(null, new object[] { proc_engine.Processor, _list_score.ToArray(), _list_stim.ToArray(), _num_stim, _num_round });
                    if (rst.accept) {
                        if (_houtput != null) _houtput(rst.result, rst.confidence);
                        _list_stim.Clear();
                        _list_score.Clear();
                    }
                    else {
                        _list_stim.RemoveRange(0, _num_stim);
                        _list_score.RemoveRange(0, _num_stim);
                    }
                }
            }
        }

        public override void SetFeedbackHandler(Delegate handler)
        {
            if (handler is Func<int, double, bool>) {
                _houtput = (Func<int, double, bool>) handler;
            }
            else {
                throw new ArgumentException();
            }
        }

        public void SetFeedbackHandler(Func<int, double, bool> handler)
        {
            _houtput = handler;
        }

        #region P300Utility functions
        internal static bool P300Util_TrainModel(string p)
        {
            if (train_model == null) return false;
            return (bool) train_model.Invoke(null, new object[] { p });
        }

        internal static bool P300Util_EEGChannelAverage(string tf)
        {
            if (eeg_average == null) return false;
            return (bool) eeg_average.Invoke(null, new object[] { tf });
        }
        #endregion
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct P300Result
    {
        public int result;
        public double confidence;
        public double threshold;
        public bool accept;
    }
}
