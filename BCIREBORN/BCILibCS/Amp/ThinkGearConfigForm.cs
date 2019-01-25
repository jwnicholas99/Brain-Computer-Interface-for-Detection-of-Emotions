using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.IO.Ports;
using BCILibCS.Util;

namespace BCILib.Amp
{
    internal partial class ThinkGearConfigForm : Form
    {
        public ThinkGearConfigForm()
        {
            InitializeComponent();

            foreach (string pname in SerialPortCtrl.GetPortNames()) {
                int len = pname.Length;
                int p1 = len - 1;
                while (!Char.IsDigit(pname[p1])) p1--;
                if (p1 == len - 1) {
                    comboPortList.Items.Add(pname);
                }
                else {
                    comboPortList.Items.Add(pname.Substring(0, p1 + 1));
                }
            }
        }

        public const string COM_PREFIX = "\\\\.\\";

        public string Port
        {
            set
            {
                string port = value;
                if (port.StartsWith(COM_PREFIX)) {
                    port = port.Substring(COM_PREFIX.Length);
                }

                for (int i = 0; i < comboPortList.Items.Count; i++)
                {
                    if (port == (string) comboPortList.Items[i])
                    {
                        comboPortList.SelectedIndex = i;
                    }
                }
                // comboPortList.Text = port;
            }
            get
            {
                return comboPortList.Text;
            }
        }

        public int NumChannels
        {
            set
            {
                numericUpDownNumChannels.Value = value;
            }

            get
            {
                return (int) numericUpDownNumChannels.Value;
            }
        }

        private void ThinkGearConfigForm_Load(object sender, EventArgs e)
        {
            comboBoxBaudRate.SelectedIndex = 3;
        }

        public int BaudRate
        {
            get
            {
                return int.Parse(comboBoxBaudRate.Text);
            }
            set
            {
                comboBoxBaudRate.Text = value.ToString();
            }
        }

        public int BlkSize
        {
            get
            {
                return (int) numericBlkSize.Value;
            }
            set
            {
                numericBlkSize.Value = value;
            }
        }

        public bool Interpolate
        {
            get
            {
                return checkBoxInterpolation.Checked;
            }
            set
            {
                checkBoxInterpolation.Checked = value;
            }
        }

        public bool SaveLog
        {
            get
            {
                return checkBoxSaveLog.Checked;
            }

            set
            {
                checkBoxSaveLog.Checked = value;
            }
        }
    }
}
