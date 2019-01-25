using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BCILib.Amp
{
    public partial class ImpRangeDlg : Form
    {
        public ImpRangeDlg()
        {
            InitializeComponent();
        }

        public int MinValue
        {
            get
            {
                return int.Parse(textBoxMinZ.Text);
            }
            set
            {
                textBoxMinZ.Text = value.ToString();
            }
        }

        public int MaxValue
        {
            get
            {
                return int.Parse(textBoxMaxZ.Text);
            }
            set
            {
                textBoxMaxZ.Text = value.ToString();
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (MaxValue <= MinValue) {
                MessageBox.Show(this, "Please check value!", "Range Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            } else {
                Close();
            }
        }

        private void ImpRangeDlg_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = MaxValue <= MinValue;
        }
    }
}
