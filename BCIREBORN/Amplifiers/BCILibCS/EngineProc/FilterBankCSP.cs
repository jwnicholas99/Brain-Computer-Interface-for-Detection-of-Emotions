using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using BCILib.sp;
using BCILib.Util;

namespace BCILib.EngineProc
{
    class FilterBankCSP
    {
        int _m = 2;
        int[] _SelBands = null;
        Filter[] _Filters = null;
        Transformation[] _Trans = null;
        int[] _SelFeatures = null;
        int[] _fea_idx = null;

        double[] _in_buffer = null;
        double[] _out_buffer = null;

        public bool LoadModel(string mfn)
        {
            Console.WriteLine("FilterBankCSP: Load model {0} ...", mfn);

            using (StreamReader sr = File.OpenText(mfn)) {
                string line;

                while ((line = sr.ReadLine()) != null) {
                    line = line.Trim();
                    if (line.StartsWith("#")) continue;

                    string[] args = line.Split(':');
                    if (args.Length < 1) continue;

                    string vname = args[0].Trim();

                    if (vname == "SelectedBanks") {
                        int numBands = int.Parse(args[1]);

                        _SelBands = new int[numBands];
                        line = sr.ReadLine();
                        _SelBands = StringTool.ToIntArray(line);
                        for (int i = 0; i < numBands; i++) {
                            _SelBands[i]--;
                        }
                    } else if (vname == "FileterBank") {
                        int nbands = int.Parse(args[1]);
                        _Filters = new Filter[nbands];
                        for (int bi = 0; bi < nbands; bi++) {
                            Filter f = new Filter();
                            f.InitBAZiHexReader(sr);
                            _Filters[bi] = f;
                        }
                    } else if (vname == "Model_M") {
                        _m = int.Parse(args[1]);
                    } else if (vname == "CSP_Transform_Bands") {
                        int n = int.Parse(args[1]);
                        _Trans = new Transformation[n];

                        for (int bi = 0; bi < n; bi++) {
                            Transformation tr = new Transformation();
                            tr.LoadFromHexReader(sr);
                            _Trans[bi] = tr;
                        }
                    } else if (vname == "Model_F") {
                        int nFeature = int.Parse(args[1]);
                        line = sr.ReadLine();
                        _SelFeatures = StringTool.ToIntArray(line);

                        int nBands = _SelBands.Length;
                        int nm = _m + _m;
                        _fea_idx = new int[nBands * nm];
                        for (int i = 0; i < _fea_idx.Length; i++) {
                            _fea_idx[i] = -1;
                        }

                        for (int i = 0; i < _SelFeatures.Length; i++) {
                            int fno = --_SelFeatures[i];
                            int iband = fno / nm;
                            iband = Array.IndexOf(_SelBands, iband);
                            int midx = fno % nm;
                            _fea_idx[iband * nm + midx] = i;
                        }
                    } else {
                        continue;
                    }

                    Console.WriteLine("Read {0} {1}", vname, args[1]);
                }

                sr.Close();

                _in_buffer = new double[_Trans[0].NumCol];
                _out_buffer = new double[(_m + _m) * _SelBands.Length];
            }

            // Filter -> for per channel per band
            int nch = _Trans[0].NumCol;
            Filter[] tflt = _Filters;
            _Filters = new Filter[_SelBands.Length * nch];
            int fi = 0;
            for (int ib = 0; ib < _SelBands.Length; ib++) {
                for (int ich = 0; ich < nch; ich++) {
                    _Filters[fi++] = tflt[ib].Duplicate();
                }
            }

            return true;
        }

        internal int GetNumChannels()
        {
            return _Trans == null ? 0 : _Trans[0].NumCol;
        }

        public int M
        {
            get
            {
                return _m;
            }
        }

        public int NumBands
        {
            get
            {
                return _SelBands == null ? 0 : _SelBands.Length;
            }
        }

        public int NumFeature
        {
            get
            {
                return _SelFeatures.Length;
            }
        }

        internal double[] ProcessSample(double[] _dbuf)
        {
            int nch = _dbuf.Length;
            int fi = 0;
            int di = 0;
            for (int bi = 0; bi < _SelBands.Length; bi++) {
                // Filtering
                for (int ich = 0; ich < nch; ich++, fi++) {
                    _in_buffer[ich] = _Filters[fi].Process(_dbuf[ich]);
                }

                // CSP
                for (int mi = 0; mi < _m + _m; mi++, di++) {
                    _out_buffer[di] = _Trans[bi].ProcessCol(mi, _in_buffer);
                }
            }

            return _out_buffer;
        }

        internal int GetFeatureIdx(int fi)
        {
            return _fea_idx[fi];
        }
    }
}
