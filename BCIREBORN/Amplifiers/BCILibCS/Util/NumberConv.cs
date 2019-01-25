using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace BCILib.Util
{
    public class NumberConv
    {
        public static double HexToDouble(string hex)
        {
            try {
                long lv = 0;
                long.TryParse(hex, NumberStyles.HexNumber, null, out lv);
                return BitConverter.ToDouble(BitConverter.GetBytes(lv), 0);
            }
            catch (Exception e) {
                Console.WriteLine(e);
                return 0;
            }
        }
    }
}
