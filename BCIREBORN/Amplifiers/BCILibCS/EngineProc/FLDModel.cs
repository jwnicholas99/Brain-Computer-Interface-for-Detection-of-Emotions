using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using BCILib.Util;

namespace BCILib.EngineProc
{
    class FLDModel
    {
        double[] _sm = null;
        double[] _sd = null;
        double[] _z = null;
        double _b = 0;
        int _s = 0;

        double version = 1.0;

        public double thr = 0;
        public int atime = 0;
        public double[] fld_r = {0, 0};

        // mean, std, min, max
        public double[] score_ranges = null;

        internal bool LoadModel(string mfn)
        {
            Console.WriteLine("FLDModel: Load model {0} ...", mfn);

            List<double> score_inf = new List<double>();

            using (StreamReader sr = File.OpenText(mfn)) {
                string line;
                bool FLDStarted = false;

                while ((line = sr.ReadLine()) != null) {
                    line = line.Trim();
                    if (line.StartsWith("#")) continue;

                    string[] args = line.Split(':');
                    if (args.Length < 1) continue;

                    string vname = args[0].Trim();

                    // 20110713: Verison 1.1 -> fld_threshold
                    if (vname == "Version") {
                        double.TryParse(args[1], out version);
                    }

                    if (vname == "Classification") {
                        if (args[1].Trim() == "FLD") {
                            FLDStarted = true;
                        }
                        continue;
                    }

                    if (!FLDStarted) continue;

                    // 20110713: Verison 1.1 -> fld_threshold
                    if (vname == "FLD_Threshold") {
                        thr = NumberConv.HexToDouble(args[1]);
                        Console.WriteLine("thr = {0}", thr);
                    } else if (vname == "FLD_AccumulateTime") {
                        int.TryParse(args[1], out atime);
                        Console.WriteLine("Accumulate time: {0}", atime);
                    } else if (vname == "FLD_Mean_Std") {
                        double[] mstd = StringTool.HexToDoubleArray(args[1]);
                        fld_r[0] = mstd[0] - mstd[1];
                        fld_r[1] = mstd[0] + mstd[1];
                    }

                    if (vname == "FLD_FeatureMean") {
                        int n = 0;
                        int.TryParse(args[1], out n);

                        line = sr.ReadLine();
                        _sm = StringTool.HexToDoubleArray(line);
                    } else if (vname == "FLD_FeatureSTD") {
                        int n = 0;
                        int.TryParse(args[1], out n);

                        line = sr.ReadLine();
                        _sd = StringTool.HexToDoubleArray(line);
                    } else if (vname == "FLD_LinearTransformVector") {
                        int n = 0;
                        int.TryParse(args[1], out n);

                        line = sr.ReadLine();
                        _z = StringTool.HexToDoubleArray(line);
                    } else if (vname == "FLD_Bias") {
                        _b = NumberConv.HexToDouble(args[1]);
                    } else if (vname == "FLD_S") {
                        int.TryParse(args[1], out _s);
                    }
                        // added 2012/04/19
                    else if (vname == "Score_mean") {
                        string[] vl = args[1].Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                        if (vl.Length > 0) score_inf.Add(NumberConv.HexToDouble(vl[0]));
                    } else if (vname == "Score_std") {
                        string[] vl = args[1].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (vl.Length > 0) score_inf.Add(NumberConv.HexToDouble(vl[0]));
                    } else if (vname == "Score_range") {
                        string[] vl = args[1].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (vl.Length > 0) score_inf.Add(NumberConv.HexToDouble(vl[0]));
                        if (vl.Length > 1) score_inf.Add(NumberConv.HexToDouble(vl[1]));
                    }
                        //end
                    else {
                        continue;
                    }

                    Console.WriteLine("Read {0} {1}", vname, args[1]);
                }
                sr.Close();
            }

            if (_s == 0 || version == 1.1) {
                thr = -thr;
                if (fld_r != null) {
                    double max = -fld_r[0];
                    fld_r[0] = -fld_r[1];
                    fld_r[1] = max;
                }
            }

            if (score_inf.Count > 0) {
                score_ranges = score_inf.ToArray();
            }

            return true;
        }

        internal double GetScore(double[] _fea_data)
        {
            double score = 0;

            for (int i = 0; i < _fea_data.Length; i++ ) {
                double fv = _fea_data[i];
                fv -= _sm[i];
                fv /= _sd[i];

                score += fv * _z[i];
            }

            score -= _b;

            if (_s == 0 || version == 1.1) score = -score;

            return score;
        }
    }
}
