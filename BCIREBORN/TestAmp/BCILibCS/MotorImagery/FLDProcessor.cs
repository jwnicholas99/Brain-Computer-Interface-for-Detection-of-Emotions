using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BCILib.App;
using BCILib.EngineProc;
using BCILib.Amp;

namespace BCILib.MotorImagery
{
    public class FLDProcessor : BCIProcessor
    {
        private Action<float[], int[]> dlg_receive_data = null;

        internal FLDProcessor() : base(new MIFLDEngine())
        {
            proc_engine.SetFeedbackHandler(new Action<int, double[]>(GetFLDScore));
        }

        internal override bool Initialize(System.Collections.Hashtable parameters)
        {
            throw new NotImplementedException();
        }

        internal bool Initialize(params string[] mdl_names)
        {
            if (!proc_engine.Initialize(mdl_names[0])) return false;
            return true;
        }

        /// <summary>
        /// added event handler for Amplifier.evt_data_received
        /// </summary>
        /// <param name="amp"></param>
        /// <param name="sel_channels"></param>
        /// <returns></returns>
        public override bool SetAmplifier(BCILib.Amp.Amplifier amp, string sel_channels)
        {
            if (dlg_receive_data != null && _amp != null) {
                _amp.evt_data_received -= dlg_receive_data;
            }

            if (!base.SetAmplifier(amp, sel_channels)) return false;

            if (dlg_receive_data == null) {
                dlg_receive_data = new Action<float[],int[]>(amp_data_received);
            }
            amp.evt_data_received += dlg_receive_data;

            return true;
        }

        List<double> _blk_score = new List<double>();
        public event Action<double[]> evt_out_score;

        /// <summary>
        /// Receive output score from Engine and add to _blk_score
        /// </summary>
        /// <param name="n"></param>
        /// <param name="score"></param>
        void GetFLDScore(int n, double[] score)
        {
            for (int i = 0; i < n; i++) {
                if (double.IsNaN(score[i])) {
                    score[i] = 0;
                }
            }

            _blk_score.AddRange(score);
        }

        public override void SetReadingShift(int shift)
        {
            base.SetReadingShift(shift);
        }

        void amp_data_received(float[] buf, int[] evt_data)
        {
            if (buf == null || buf.Length == 0) return;

            // current amplifier data position
            int cpos = _amp.Rd_GetPos();
            int nspl = buf.Length / _amp.header.nchan;

            if (pc_buf == null || pc_buf.Length < nspl * chsel.Length) {
                pc_buf = new float[nspl * chsel.Length];
            }
            CopyProcData(buf, chsel.Length, nspl);

            _blk_score.Clear();
            proc_engine.ProcEEGBuf(pc_buf, base.NumChannelUsed, nspl);

            if (evt_out_score != null) {
                // display in FLDScoreViewer
                evt_out_score(_blk_score.ToArray());
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (dlg_receive_data != null && _amp != null) {
                _amp.evt_data_received -= dlg_receive_data;
            }
        }

        ~FLDProcessor()
        {
            Dispose(false);
        }

        MIProcessor.MIOutout _output_score;

        public override void SetFeedbackHandler(Delegate handler)
        {
            _output_score = handler as MIProcessor.MIOutout;
        }

        internal FLDModel GetFLDModel()
        {
            return (proc_engine as MIFLDEngine).FLDModel;
        }
    }
}
