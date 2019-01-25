using System;
using System.IO;
using BCILib.Util;

namespace BCILib.sp
{
	/// <summary>
	/// Summary description for Filter.
	/// </summary>
	/// 

    public class Filter
    {
        private double[] B;
        private double[] A;

        private double[] Z;
        private double[] Zi;

        public Filter()
        {
            B = A = Z = Zi = null;
        }

        public Filter(double[] B, double[] A)
        {
            InitFilter(B, A);
        }

        private void InitFilter(double[] B, double[] A)
        {
            this.B = (double[])B.Clone();
            this.A = (double[])A.Clone();
            Init();
        }

        public void Init()
        {
            if (A == null || B == null) {
                Console.WriteLine("\nFilter.Init: B and/or B wrong, exit.");
                return;
            }

            int na = A.Length;
            int nb = B.Length;
            int nfilt = na > nb ? na : nb;

            if (na < nfilt) {
                double[] tA = new double[nfilt];
                Array.Copy(A, 0, tA, 0, na);
                A = tA;
                // initize rest to 0
            }

            if (nb < nfilt) {
                double[] tB = new double[nfilt];
                Array.Copy(B, 0, tB, 0, nb);
                B = tB;
                // initialize rest to 0
            }

            if (A[0] != 1.0) {
                for (int i = 0; i < nfilt; i++) {
                    A[i] /= A[0];
                    B[i] /= A[0];
                }
            }

            Z = new double[nfilt - 1];
            // initialize to 0
        }

        public void Process(float[] data)
        {
            if (A == null || B == null || A.Length != B.Length) {
                Console.WriteLine("\nA and/or B wrong, exit.");
                return;
            }

            int nfilt = A.Length;

            // init Z
            for (int k = 0; k < nfilt - 1; k++) Z[k] = 0;

            // filtering
            for (int k = 0; k < data.Length; k++) {
                // input a sample
                double xi = data[k];
                double yout = Z[0] + B[0] * xi;

                // update Z
                for (int i = 0; i < nfilt - 2; i++) {
                    Z[i] = Z[i + 1] + B[i + 1] * xi - A[i + 1] * yout;
                }
                Z[nfilt - 2] = B[nfilt - 1] * xi - A[nfilt - 1] * yout;

                // output
                data[k] = (float)yout;
            }
        }

        public double Process(double dval)
        {
            if (A == null || B == null || A.Length != B.Length) {
                Console.WriteLine("\nA and/or B wrong, exit.");
                return dval;
            }

            int nfilt = A.Length;

            double yout = Z[0] + dval * B[0];

            // update Z
            for (int i = 0; i < nfilt - 2; i++) {
                Z[i] = Z[i + 1] + B[i + 1] * dval - A[i + 1] * yout;
            }
            Z[nfilt - 2] = B[nfilt - 1] * dval - A[nfilt - 1] * yout;

            return yout;
        }

        internal void InitBAZiHexReader(TextReader sr)
        {
            InitBAHexReader(sr);

            string line = sr.ReadLine();
            string[] args = line.Split(':');
            int nZi = int.Parse(args[1]);
            line = sr.ReadLine();
            Zi = StringTool.HexToDoubleArray(line);

            //Init();
        }

        private void InitBAHexReader(TextReader sr)
        {
            string line;

            line = sr.ReadLine();
            string[] args = line.Split(':');

            int nB = int.Parse(args[1]);
            line = sr.ReadLine();
            B = StringTool.HexToDoubleArray(line);

            line = sr.ReadLine();
            args = line.Split(':');
            int nA = int.Parse(args[1]);
            line = sr.ReadLine();
            A = StringTool.HexToDoubleArray(line);

            Init();
        }

        internal Filter Duplicate()
        {
            Filter nf = new Filter();
            nf.B = (double[])B.Clone();
            nf.A = (double[])A.Clone();
            if (Zi != null) {
                nf.Zi = (double[])Zi.Clone();
            }
            nf.Init();
            return nf;
        }

        public bool FiltFilt(double[] fdata)
        {
            //if (!Check()) return false;
            if (A == null || A.Length == 0 || B == null || B.Length != A.Length) return false;


            int nfilt = A.Length;
            int nfact = 3 * (nfilt - 1);

            int ldata = fdata == null ? 0 : fdata.Length;
            if (ldata < nfact) {
                Console.WriteLine("FiltFilt: Data must have length more than 3 times filter order.");
                return false;
            }

            var pred = new double[nfact];
            var posd = new double[nfact];

            // y = [2*x(1)-x((nfact+1):-1:2);x;2*x(len)-x((len-1):-1:len-nfact)];
            for (int i = 0; i < nfact; i++) {
                pred[i] = 2 * fdata[0] - fdata[nfact - i];
                posd[i] = 2 * fdata[ldata - 1] - fdata[ldata - 2 - i];
            }

            //% filter, reverse data, filter again, and reverse data again
            //y = filter(b,a,y,[zi*y(1)]);

            // reset from mpZi
            if (Zi != null) {
                for (int i = 0; i < nfilt - 1; i++) {
                    Z[i] = Zi[i] * pred[0];
                }
            } else {
                for (int i = 0; i < nfilt - 1; i++) {
                    Z[i] = 0;
                }
            }

            for (int i = 0; i < nfact; i++) {
                pred[i] = Process(pred[i]);
            }

            for (int i = 0; i < ldata; i++) {
                fdata[i] = Process(fdata[i]);
            }
            for (int i = 0; i < nfact; i++) {
                posd[i] = Process(posd[i]);
            }

            //y = y(length(y):-1:1);
            //y = filter(b,a,y,[zi*y(1)]);
            //y = y(length(y):-1:1);

            // rest mpZi
            if (Zi != null) {
                for (int i = 0; i < nfilt - 1; i++) {
                    Z[i] = Zi[i] * posd[nfact - 1];
                }
            } else {
                for (int i = 0; i < nfilt - 1; i++) {
                    Z[i] = 0;
                }
            }

            for (int i = nfact - 1; i >= 0; i--) {
                posd[i] = Process(posd[i]);
            }
            for (int i = ldata - 1; i >= 0; i--) {
                fdata[i] = Process(fdata[i]);
            }
            //for (int i = nfact - 1; i >= 0; i--) {
            //	pred[i] = ProcessValue(pred[i]);
            //}

            return true;
        }
    }
}
