using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BCILib.Util
{
    public partial class USBVibrationForm : Form
    {
        USBVibrator vibrator;

        public USBVibrationForm():
            this(new USBVibrator())
        {
        }

        public USBVibrationForm(USBVibrator uv)
        {
            InitializeComponent();
            vibrator = uv;
            vibrator.Open();
            vibrator.Config_Channel_Output((byte)numValue.Value);
            lblNumChannels.Text = vibrator.GetNumChannels().ToString();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            vibrator.Stop();
            btnStart.Enabled = !vibrator.Started;
            btnStop.Enabled = vibrator.Started;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            vibrator.Start();
            vibrator.Channel_Select(210);
            btnStart.Enabled = !vibrator.Started;
            btnStop.Enabled = vibrator.Started;
        }

        private void numValue_ValueChanged(object sender, EventArgs e)
        {
            vibrator.Config_Channel_Output((byte)numValue.Value);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            vibrator.Close();
        }

        internal static void ShowForm(USBVibrator usb_vibrator)
        {
            new USBVibrationForm(usb_vibrator).Show();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            //vibrator.Reset();
            btnStart.Enabled = !vibrator.Started;
            btnStop.Enabled = vibrator.Started;
        }
    }
}
