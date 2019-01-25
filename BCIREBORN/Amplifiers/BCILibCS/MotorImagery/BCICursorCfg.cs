using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BCILib.MotorImagery
{
    public partial class BCICursorCfg : Form
    {
        public BCICursorCfg()
        {
            InitializeComponent();
        }

        public int HSteps
        {
            get
            {
                int w = 4;
                int.TryParse(tbHSteps.Text, out w);
                return w;
            }

            set
            {
                tbHSteps.Text = value.ToString();
            }
        }

        public int VSteps
        {
            get
            {
                int h = 4;
                int.TryParse(tbVSteps.Text, out h);
                return h;
            }
            set
            {
                tbVSteps.Text = value.ToString();
            }
        }

        public int TargetSize
        {
            get
            {
                int s = 1;
                int.TryParse(tbTargetSize.Text, out s);
                return s;
            }

            set
            {
                tbTargetSize.Text = value.ToString();
            }
        }

        public int PrepareTime
        {
            get
            {
                int ptime = 3;
                int.TryParse(tbPrepSeconds.Text, out ptime);
                return ptime;
            }
            set
            {
                tbPrepSeconds.Text = value.ToString();
            }
        }

        public bool Calibrate
        {
            get
            {
                return cbCalibrate.Checked;
            }
            set
            {
                cbCalibrate.Checked = value;
            }
        }
    }
}
