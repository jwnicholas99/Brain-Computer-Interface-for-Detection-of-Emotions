using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MatlabStarter {
    public partial class Form1 : Form {
        public Form1()
        {
            InitializeComponent();
        }

        MLApp.MLAppClass matlab = null;

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (matlab == null) {
                Type mtype = Type.GetTypeFromProgID("matlab.desktop.application");
                object o = Activator.CreateInstance(mtype);
                GC.SuppressFinalize(o);

                matlab = new MLApp.MLAppClass();
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (matlab != null) {
                matlab.Quit();
                matlab = null;
            }
        }
    }
}
