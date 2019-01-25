using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BCILib.Util;

namespace BCILib.sp
{
    internal class Transformation
    {
        double[,] _Matrix = null;

        internal void LoadFromHexReader(System.IO.TextReader sr)
        {
            string line = sr.ReadLine();
            string[] args = line.Split(':');
            args = args[1].Split(',');
            int nRow = int.Parse(args[0]);
            int nCol = int.Parse(args[1]);

            _Matrix = new double[nRow, nCol];

            for (int ri = 0; ri < nRow; ri++) {
                line = sr.ReadLine();
                double[] vl = StringTool.HexToDoubleArray(line);
                for (int ci = 0; ci < nCol; ci++) {
                    _Matrix[ri, ci] = vl[ci];
                }
            }
        }

        public int NumCol
        {
            get
            {
                return _Matrix.GetLength(1);
            }
        }

        public int NumRow
        {
            get
            {
                return _Matrix.GetLength(0);
            }
        }

        internal double ProcessCol(int mi, double[] _in_buffer)
        {
            double dv = 0;
            for (int ic = 0; ic < _in_buffer.Length; ic++) {
                dv += _Matrix[mi, ic] * _in_buffer[ic];
            }
            return dv;
        }
    }
}
