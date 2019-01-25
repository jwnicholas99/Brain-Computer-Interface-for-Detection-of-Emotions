using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace BCILib.Amp
{
    internal partial class NeuroScanCfg : Form
    {
        public NeuroScanCfg()
        {
            InitializeComponent();
            comboAmpType.Items.Add(NeuroScan.Device.Nuamps.ToString());
            comboAmpType.Items.Add(NeuroScan.Device.SynAmp2.ToString());
            comboAmpType.SelectedIndex = 0;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        public NeuroScan.Device Device
        {
            get
            {
                return (NeuroScan.Device)Enum.Parse(typeof(NeuroScan.Device), comboAmpType.Text);
            }
        }
        public string Host
        {
            get
            {
                return textHost.Text;
            }

            set
            {
                textHost.Text = value;
            }
        }

        public int Port
        {
            get
            {
                return int.Parse(textPort.Text);
            }

            set
            {
                textPort.Text = value.ToString();
            }
        }

        public string ID
        {
            set
            {
                textAmpID.Text = value;
            }
        }

        public StimMethod StimMethod
        {
            set
            {
                radioUSBMOD4.Checked = (value & StimMethod.USBMOD4) == StimMethod.USBMOD4;
                cbSoftware.Checked = ((value & StimMethod.Software) == StimMethod.Software);
                cbHardware.Checked = ((value & StimMethod.ParallePort) == StimMethod.ParallePort || (value & StimMethod.USBMOD4) == StimMethod.USBMOD4);
            }
            get
            {
                StimMethod sm = StimMethod.None;
                if (cbSoftware.Checked) sm |= StimMethod.Software;
                if (cbHardware.Checked) {
                    if (radioParallelPort.Checked) sm |= StimMethod.ParallePort;
                    else sm |= StimMethod.USBMOD4;
                }
                return sm;
            }
        }

        public bool DebugMode
        {
            get
            {
                return cbDebug.Checked;
            }

            set
            {
                cbDebug.Checked = value;
            }
        }

        private void cbDebug_CheckedChanged(object sender, EventArgs e)
        {
            if (cbDebug.Checked) {
                cbHardware.Checked = cbSoftware.Checked = true;
            }
            cbHardware.Enabled = cbSoftware.Enabled = !cbDebug.Checked;
        }

        public bool CheckTimeout
        {
            get
            {
                return cbCheckTOut.Checked;
            }
            set
            {
                cbCheckTOut.Checked = value;
            }
        }

        public int PortAddr
        {
            set
            {
                textBoxPortAddr.Text = value.ToString("X");
            }

            get
            {
                return int.Parse(textBoxPortAddr.Text, NumberStyles.AllowHexSpecifier);
            }
        }
    }
}
