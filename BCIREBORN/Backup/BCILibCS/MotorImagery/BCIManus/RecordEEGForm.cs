using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BCILib.App;
using System.IO;
using BCILib.Amp;
using BCILib.Util;

namespace BCILib.MotorImagery.BCIManus
{
    public partial class RecordEEGForm : Form
    {
        public RecordEEGForm()
        {
            InitializeComponent();

            LoadConfig();
        }

        private void LoadConfig()
        {
            ResManager rm = BCIApplication.AppResource;

            int rt = (int) numericUpDown1.Value;
            int rl = rt;
            rm.GetConfigValue("RecordEEG", "RecordMinutes", ref rl);
            if (rt != rl) {
                numericUpDown1.Value = rl;
            }
        }

        private void SaveConfig()
        {
            ResManager rm = BCIApplication.AppResource;

            int rt = (int)numericUpDown1.Value;
            int rl = 0;
            rm.GetConfigValue("RecordEEG", "RecordMinutes", ref rl);
            if (rt != rl) {
                rm.SetConfigValue("RecordEEG", "RecordMinutes", rt.ToString());
                rm.SaveFile();
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            SaveConfig();
            base.OnFormClosed(e);
        }

        private void tbDataLabel_TextChanged(object sender, EventArgs e)
        {
            UpdateRecordPath();
        }

        private void RecordEEGForm_Load(object sender, EventArgs e)
        {
            tbDataLabel.SelectionStart = tbDataLabel.Text.Length;
            tbDataLabel.Select();
            UpdateRecordPath();

            lbTime.Text = null;
        }

        private void UpdateRecordPath()
        {
            tbFilePath.Text = Path.Combine(BCIApplication.DateString, 
                string.Format("{0}_{1}m_{2:HHmmss}",
                tbDataLabel.Text, numericUpDown1.Value, DateTime.Now));
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            UpdateRecordPath();
        }

        long t_start = 0;
        Amplifier amp = null;

        private void btnStartStop_Click(object sender, EventArgs e)
        {
            bool is_running = timer1.Enabled;

            if (is_running) {
                btnStartStop.Text = "Start";
                timer1.Stop();

                amp.StopRecord();
                lbTime.Text = null;
                Close();
            } else {
                amp = AmpContainer.GetAmplifier(0);
                if (amp == null) {
                    MessageBox.Show("Cannot find amplifier!");
                    return;
                }
                if (!amp.Start()) {
                    MessageBox.Show("Cannot start amplifier!");
                    return;
                }

                string fn = tbFilePath.Text + "_" + amp.ID + ".cnt";

                if (File.Exists(fn) && 
                    MessageBox.Show(this, "EEG file already exists. overwrite?", "FileExists", MessageBoxButtons.OKCancel) != DialogResult.OK)
                {
                    return;
                }

                string dir = Path.GetDirectoryName(fn);
                if (!Directory.Exists(dir)) {
                    Directory.CreateDirectory(dir);
                }

                try {
                    amp.StartRecord(fn);
                } catch (Exception) {
                    MessageBox.Show("Cannot record file: " + fn + "!");
                    return;
                }

                timer1.Start();
                btnStartStop.Text = "Stop";
                t_start = DateTime.Now.Ticks;
            }

            is_running = !is_running;

            this.ControlBox = !is_running;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            long t_all = ((long) numericUpDown1.Value) * 60 * TimeSpan.TicksPerSecond;
            TimeSpan ts = TimeSpan.FromTicks(DateTime.Now.Ticks - t_start);
            lbTime.Text = string.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
            if (ts.Ticks >= t_all) {
                btnStartStop.PerformClick();
            }
        }
    }
}
