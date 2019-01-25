using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using BCILib.Amp;
using BCILib.App;
using BCILib.Util;
using System.IO;

namespace BCILib.Concentration
{
    internal partial class BCIConcentration : BCIAppForm
    {
        public BCIConcentration()
        {
            InitializeComponent();
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabPageModel) {
                trainModelCfg1.InitStart();
            }
            else if (tabControl1.SelectedTab == tabPageGames) {
                gamesCfg1.LoadConfig();
            }
        }

        private void BCIConcentrationForm_Load(object sender, EventArgs e)
        {
            //ResManager rm = BCIApplication.AppResource;
            //string subject = rm.GetConfigValue(BCIApplication.AppName, "Subject");
            //bool withDate = false;
            //string line = rm.GetConfigValue(BCIApplication.AppName, "UserWithDate");
            //if (!string.IsNullOrEmpty(line)) {
            //    try {
            //        withDate = bool.Parse(line);
            //    }
            //    catch (Exception) { }
            //}
            //if (withDate) {
            //    this.Text = this.Text + " Subject: " + subject + " - " + rm.GetConfigValue(BCIApplication.AppName, "DateString");
            //}
            //else {
            //    this.Text = this.Text + " Subject: " + subject;
            //}
        }

        private void BCIConcentrationForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            gamesCfg1.Close();
        }

        private void buttonBrowseNUSData_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog() != DialogResult.OK) return;

            textBoxNUSDataPath.Text = dlg.SelectedPath;
        }

        private void buttonStartNUSDataTest_Click(object sender, EventArgs e)
        {
            // Get model file
            ResManager rm = BCIApplication.SysResource;
            string mfn = rm.GetConfigValue(BCICfg.AttentionDetection, BCICfg.ModelFile);
            if (string.IsNullOrEmpty(mfn)) {
                MessageBox.Show("System.cfg: Model file not defined!");
                return;
            }

            // Testing tool
            string test_tool = Path.Combine(BCIApplication.RootPath, "NUSDataTest.exe");
            if (!File.Exists(test_tool)) {
                MessageBox.Show("Testtool missing!");
                return;
            }

            string[] cntfiles = Directory.GetFiles(textBoxNUSDataPath.Text, "*.cnt", SearchOption.TopDirectoryOnly);
            if (cntfiles == null || cntfiles.Length == 0) {
                MessageBox.Show("Not EEG data found!");
                return;
            }

            foreach (string fn in cntfiles) {
                string fout = Path.Combine(Path.GetDirectoryName(fn), Path.GetFileNameWithoutExtension(fn) + ".csv");

                try {
                    System.Diagnostics.Process.Start(test_tool, string.Format("\"{0}\" \"{1}\" \"{2}\"", 
                        mfn, fn, fout));
                }
                catch (Exception e1) {
                    MessageBox.Show("Exception here: " + e1.Message);
                }
            }
        }
    }
}
