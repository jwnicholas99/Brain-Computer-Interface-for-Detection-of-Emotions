using System;
using System.Collections.Generic;
using System.Text;
using BCILib.App;

namespace BCILib.MotorImagery
{
    public class MIProcessor: BCIProcessor
    {
        public MIProcessor():base(BCIEngine.BCIProcType.MotorImagery)
        {
            _engine_outproc = new MIOutFunc(MIOutScore);
            base.SetFeedbackHandler(_engine_outproc);
        }

        public delegate void MIOutout(double[] conf);
        MIOutout _mi_output = null;

        public override void SetFeedbackHandler(Delegate handler)
        {
            _mi_output = handler as MIOutout;
        }

        // more classifiers
        BCIEngine[] _mproc = null;

        public bool Initialize(params string[] model_list)
        {
            if (model_list == null || model_list.Length < 1) {
                return false;
            }

            if (_mproc == null || _mproc.Length != model_list.Length) {
                _mproc = new BCIEngine[model_list.Length];
                _mproc[0] = proc_engine;
            }

            for (int iclass = 0; iclass < _mproc.Length; iclass++) {
                if (_mproc[iclass] == null) {
                    _mproc[iclass] = BCIEngine.CreateEngine(BCIEngine.BCIProcType.MotorImagery);
                    _mproc[iclass].SetFeedbackHandler(_engine_outproc);
                }

                if (!_mproc[iclass].Initialize(model_list[iclass])) return false;
            }

            return true;
        }

        float[] _engine_conf = new float[1];
        delegate void MIOutFunc(IntPtr prt, int len);
        MIOutFunc _engine_outproc = null;

        private void MIOutScore(IntPtr ptr, int len)
        {
            _engine_conf[0] = 0;
            if (len > 0) {
                System.Runtime.InteropServices.Marshal.Copy(ptr, _engine_conf, 0, 1);
            }
        }

        protected override void ProcessSelectedData()
        {
            int ncls = _mproc.Length;
            if (ncls == 1) ncls = 2;

            double[] score = new double[ncls];
            for (int iclass = 0; iclass < _mproc.Length; iclass++) {
                _mproc[iclass].ProcEEGBuf(pc_buf, chsel.Length, proc_engine.NumSampleUsed);
                score[iclass] = _engine_conf[0];
            }

            if (_mproc.Length < ncls) score[ncls - 1] = 1 - score[ncls - 2];

            if (_mi_output != null) _mi_output(score);
        }
    }
}
