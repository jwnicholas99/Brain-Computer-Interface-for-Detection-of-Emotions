using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using BCILib.Amp;

namespace StartAmplifier
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] argv)
        {
            AmpContainer.Main(argv);
        }
    }
}
