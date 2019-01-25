using System;
using System.Globalization;

namespace BCILib.Util
{
    class StringTool
    {
        static char[] _sep = { ',', '\t', '\n', ' ' };

        public static char[] SEP
        {
            get
            {
                return _sep;
            }
        }

        internal static double[] HexToDoubleArray(string line)
        {
            string[] hexlist = line.Split(_sep, StringSplitOptions.RemoveEmptyEntries);
            double[] da = new double[hexlist.Length];
            for (int i = 0; i < hexlist.Length; i++) {
                da[i] = BitConverter.ToDouble(
                    BitConverter.GetBytes(long.Parse(hexlist[i], NumberStyles.HexNumber)), 0);
            }

            return da;
        }

        internal static int[] ToIntArray(string line)
        {
            string[] vlist = line.Split(_sep, StringSplitOptions.RemoveEmptyEntries);
            int[] da = new int[vlist.Length];
            for (int i = 0; i < da.Length; i++) {
                if (!int.TryParse(vlist[i], out da[i])) da[i] = 0; 
            }
            return da;
        }
    }
}
