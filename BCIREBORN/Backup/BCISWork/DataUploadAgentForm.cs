using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Resources;
using System.Xml.Serialization;

namespace BCIWork
{
    public partial class DataUploadAgentForm : Form
    {
        public DataUploadAgentForm()
        {
            InitializeComponent();
        }

        Action<UploadWorker.Progress_Record, bool, bool> upl_report = null;
        Action<int, int, int> spd_report = null;

        private void DataUploadAgentForm_Load(object sender, EventArgs e)
        {
            var dirs = Directory.GetDirectories(Environment.CurrentDirectory, "Users_*");
            if (dirs.Length <= 0) {
                MessageBox.Show("Cannot find local data path!");
                Close();
            }
            string local_path = null;
            foreach (var upath in dirs) {
                if (string.IsNullOrEmpty(local_path) || upath.Length < local_path.Length) local_path = upath;
            }
            string prod_name = Path.GetFileName(local_path).Substring(6);

            if (!UploadWorker.AppAuthenticate()) {
                Close();
                return;
            }

            Text += ": " + new Uri(UploadWorker.sclient.Url).Host;

            upl_report = new Action<UploadWorker.Progress_Record, bool, bool>(UploadWorker_evt_report);
            spd_report = new Action<int, int, int>(UploadWorker_spd_report);
            UploadWorker.evt_rpttask += upl_report;
            UploadWorker.evt_rptspeed += spd_report;
            UploadWorker.Start(local_path, prod_name);
        }

        void UploadWorker_evt_report(UploadWorker.Progress_Record pr, bool end, bool fast)
        {
            if (InvokeRequired) {
                BeginInvoke(upl_report, pr, end, fast);
            } else {
                var it = listViewUplProgress.Items.Cast<ListViewItem>().FirstOrDefault(x => x.Tag == pr);

                string pct = ((double)pr.uleng / pr.fleng).ToString("P0");
                string size = pr.fleng.ToString();
                string fn = Path.GetFileName(pr.fpath);
                string writing = pr.chkcls? "Yes" : "No";
                labelUplStatus.Text = string.Join(", ", new[] { fn, pct });

                if (it == null) {
                    it = new ListViewItem(new[] { fn, size, writing, pct });
                    it.Tag = pr;
                    listViewUplProgress.Items.Insert(0, it);
                    
                } else {
                    //if (listViewUplProgress.Items[0] != it) {
                    //    listViewUplProgress.Items.Remove(it);
                    //    listViewUplProgress.Items.Insert(0, it);
                    //}
                    it.SubItems[1].Text = size;
                    it.SubItems[2].Text = writing;
                    it.SubItems[3].Text = pct;
                }
                it.EnsureVisible();

                labelUplStatus.Text = UploadWorker.TotalFiles.ToString();
            }
        }

        void UploadWorker_spd_report(int pct, int spt, int rem_time)
        {
            if (InvokeRequired) {
                BeginInvoke(spd_report, pct, spt, rem_time);
            } else {
                labelUpdWorkers.Text = UploadWorker.NumberofWorkers.ToString();

                if (spt == 0 && rem_time == 0) {
                    labelSpeed.Text = string.Empty;
                    return;
                }

                int s = rem_time % 60;
                rem_time /= 60;
                int m = rem_time % 60;
                rem_time /= 60;
                StringBuilder sb = new StringBuilder(string.Format("{0}%, {1}/s, remen time=", pct, spt));

                if (rem_time > 0) {
                    sb.Append(rem_time.ToString() + "h ");
                }

                if (rem_time > 0 || m > 0) {
                    sb.Append(m.ToString() + "m ");
                }

                sb.Append(s.ToString() + "s");

                labelSpeed.Text = sb.ToString();
            }
        }

        public bool CanClose = false;

        private void DataUploadAgentForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!CanClose) {
                e.Cancel = true;
                WindowState = FormWindowState.Minimized;
                ShowInTaskbar = false;
            }
        }

        private void toolStripButtonSearch_Click(object sender, EventArgs e)
        {
            UploadWorker.SearchNewFiles();
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            WindowState = FormWindowState.Normal;
            ShowInTaskbar = true;
        }
    }
}
