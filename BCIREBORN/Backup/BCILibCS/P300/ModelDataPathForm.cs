using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BCILib.P300
{
    public partial class ModelDataPathForm : Form
    {
        public ModelDataPathForm()
        {
            InitializeComponent();
        }

        public string ClassifyModelPath
        {
            set
            {
                textBoxClassifyDataPath.Text = value;
            }
            get
            {
                return textBoxClassifyDataPath.Text;
            }
        }

        public string RejectModelPath
        {
            set
            {
                textBoxRejectionDataPath.Text = value;
            }
            get
            {
                return textBoxRejectionDataPath.Text;
            }
        }

        private void buttonResetClsPath_Click(object sender, EventArgs e)
        {
            textBoxClassifyDataPath.Text = null;
        }

        private void buttonResetRejPath_Click(object sender, EventArgs e)
        {
            textBoxRejectionDataPath.Text = null;
        }
    }
}
