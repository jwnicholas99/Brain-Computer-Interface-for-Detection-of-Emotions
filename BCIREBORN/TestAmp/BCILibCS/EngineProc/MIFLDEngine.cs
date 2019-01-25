using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BCILib.App;
using System.IO;

namespace BCILib.EngineProc
{
    public class MIFLDEngine : BCICSEngine
    {
        public Action<int, double[]> _output_score;

        public override void SetFeedbackHandler(Delegate handler)
        {
            _output_score += handler as Action<int, double[]>;
        }

        /// <summary>
        /// origila sample data
        /// </summary>
        private double[] _dbuf = null;

        /// <summary>
        /// Shift data buffer
        /// </summary>
        private double[,,] _shift_buf = null;
        private int _shift_pos = 0;
        private int _shift_num = 0;
        private double[,] _shift_sum = null;

        private double[] _fea_data = null;

        public override void ProcEEGBuf(float[] buf, int nch, int nspl)
        {
            int ispl = 0;
            int nm = _fbcsp.M + _fbcsp.M;

            while (ispl < nspl) {
                // copy data
                int ifrom = ispl;
                for (int ich = 0; ich < nch; ich++) {
                    _dbuf[ich] = buf[ifrom];
                    ifrom += nspl;
                }
                ispl++;

                // csp feature
                double[] fcsp = _fbcsp.ProcessSample(_dbuf);

                // copy to shift_buf
                for (int i = 0; i < fcsp.Length; i++) {
                    double dv = fcsp[i];
                    dv *= dv;

                    int ib = i / nm;
                    int im = i % nm;
                    _shift_buf[ib, im, _shift_pos] = dv;
                    _shift_sum[ib, im] += dv;
                }
                _shift_pos++;
                if (_shift_pos >= NumSampleUsed) _shift_pos = 0;
                _shift_num++;

                if (_shift_num >= NumSampleUsed) {
                    int fi = 0;
                    for (int ib = 0; ib < _fbcsp.NumBands; ib++) {
                        double ssum = 0;
                        for (int im = 0; im < nm; im++) {
                            ssum += _shift_sum[ib, im];
                        }

                        if (ssum == 0) ssum = 1;

                        for (int im = 0; im < nm; im++, fi++) {
                            int fn = _fbcsp.GetFeatureIdx(fi);
                            if (fn >= 0) {
                                _fea_data[fn] = _shift_sum[ib, im] / ssum;
                            }
                        }
                    }

                    // process
                    double[] score = { 0 };
                    score[0] = _model.GetScore(_fea_data);

                    if (_output_score != null) _output_score(1, score);

                    // move out one sample
                    for (int i = 0; i < fcsp.Length; i++) {
                        int ib = i / nm;
                        int im = i % nm;
                        _shift_sum[ib, im] -= _shift_buf[ib, im, _shift_pos];
                    }
                }
            }
        }

        public override bool Initialize(string mdl_fn)
        {
            if (!_fbcsp.LoadModel(mdl_fn)) return false;
            if (!_model.LoadModel(mdl_fn)) return false;

            Console.WriteLine("{0}: load model {1} ...",  this.GetType(), mdl_fn);

            using (TextReader sr = File.OpenText(mdl_fn)) {
                string line;

                while ((line = sr.ReadLine()) != null) {
                    line = line.Trim();
                    if (line.StartsWith("#")) continue;

                    string[] args = line.Split(':');
                    if (args.Length < 2) continue;

                    string vname = args[0].Trim();
                    if (base.SetCommParameter(vname, args[1], sr)) {
                    } else {
                        continue;
                    }

                    Console.WriteLine("Read {0}, {1}", vname, args[1]);
                }
                sr.Close();
            }

            _numSampleUsed = 2 * SamplingRate;

            int nch = _fbcsp.GetNumChannels();
            if (_ch_used == null || _ch_used.Length != nch && nch > 0) {
                _ch_used = new int[nch];
                for (int i = 0; i < nch; i++) _ch_used[i] = i;
            }

            _dbuf = new double[nch];
            _shift_buf = new double[_fbcsp.NumBands, _fbcsp.M * 2, _numSampleUsed];
            _shift_sum = new double[_fbcsp.NumBands, _fbcsp.M * 2];
            _shift_pos = _shift_num = 0;
            _fea_data = new double[_fbcsp.NumFeature];

            return true;
        }

        public override bool Initialize(System.Collections.Hashtable parameters)
        {
            throw new NotImplementedException();
        }

        public override void Free()
        {
        }

        public override IntPtr Processor
        {
            get { throw new NotImplementedException(); }
        }

        private FilterBankCSP _fbcsp = new FilterBankCSP();
        private FLDModel _model = new FLDModel();

        internal FLDModel FLDModel
        {
            get
            {
                return _model;
            }
        }
    }
}
