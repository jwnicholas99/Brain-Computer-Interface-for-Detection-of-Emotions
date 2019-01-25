using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BCILib.Amp
{
    internal partial class BioRadio150Cfg : Form
    {
        public BioRadio150Cfg()
        {
            InitializeComponent();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (comboBoxDevice.SelectedIndex < 0) {
                MessageBox.Show("Please select a device!");
                return;
            }
            DialogResult = DialogResult.OK;
        }

        public string SelectedDevice
        {
            get
            {
                return comboBoxDevice.Text;
            }
        }

        //public void AddDevice(string devIDStr, int devID, string port, bool sel)
        public void AddDevice(string port, bool sel)
        {
            //int ino = comboBoxDevice.Items.Add(string.Format("{0}|{1}|{2}", devIDStr, devID, port));
            int ino = comboBoxDevice.Items.Add(port);
            if (sel) comboBoxDevice.SelectedIndex = ino;
        }
    }
}
