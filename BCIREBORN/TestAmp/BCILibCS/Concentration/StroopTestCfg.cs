using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using System.Collections;
using BCILib.App;
using BCILib.Util;

namespace BCILib.Concentration
{
    internal partial class StroopTestCfg : UserControl
    {
        //private string[] AllKnownColors = null;

        public StroopTestCfg()
        {
            InitializeComponent();

            // Get all kown colors
            string[] colnames = Enum.GetNames(typeof(KnownColor));
            ArrayList al = new ArrayList();
            //ArrayList ak = new ArrayList();
            foreach (string cname in colnames) {
                KnownColor kc = (KnownColor)Enum.Parse(typeof(KnownColor), cname);
                if (kc > KnownColor.Transparent) {
                    al.Add(kc);
                    //					Color clr = Color.FromName(cname);
                    //					int v = (clr.R + clr.G + clr.B) / 3;
                    //					v = Math.Abs(clr.R - v) + Math.Abs(clr.G - v) + Math.Abs(clr.B - v);
                    //					ak.Add(v);
                }
            }

            //KnownColor[] val = (KnownColor[])al.ToArray(typeof(KnownColor));
            ////			int[] keys = (int[]) ak.ToArray(typeof(int));
            ////			Array.Sort(keys, val);
            //AllKnownColors = new String[al.Count];
            //int n = 0;
            //foreach (KnownColor kc in val) {
            //    string cname = Enum.GetName(typeof(KnownColor), kc);
            //    AllKnownColors[n] = cname;
            //    Color clr = Color.FromName(cname);
            //    Image img = new Bitmap(imageListColors.ImageSize.Width, imageListColors.ImageSize.Height);
            //    Graphics g = Graphics.FromImage(img);
            //    g.FillRectangle(new SolidBrush(clr), 0, 0, img.Width, img.Height);
            //    g.DrawRectangle(Pens.Black, 0, 0, img.Width - 1, img.Height - 1);
            //    g.Dispose();
            //    imageListColors.Images.Add(img);

            //    ListViewItem it = listAllCols.Items.Add(cname);
            //    it.ImageIndex = n++;
            //}
        }
        private void LoadContent()
        {
            LoadContent(BCIApplication.SysResource);
        }

        private void LoadContent(ResManager rm)
        {
            int[] cfg_level_seq = null;
            int cfg_timecue = 1000;
            int cfg_wtimeout = 10000;
            int cfg_timerest = 200;
            int cfg_timerelax = 6000;
            int cfg_ntrials = 20;
            int cfg_relaximage = 0;

            //string cfg_colors = "White,Red,Green,Yellow";

            string rn = "StroopTest";

            rm.GetConfigValue(rn, "TimeCue", ref cfg_timecue);
            rm.GetConfigValue(rn, "TimeoutWait", ref cfg_wtimeout);
            rm.GetConfigValue(rn, "TimeRest", ref cfg_timerest);
            rm.GetConfigValue(rn, "TimeRelax", ref cfg_timerelax);
            rm.GetConfigValue(rn, "NumTrials", ref cfg_ntrials);
            //rm.GetConfigValue(rn, "ColorList", ref cfg_colors);
            rm.GetConfigValue(rn, "RelaxDispImage", ref cfg_relaximage);

            string strSeqence = "3,3";
            rm.GetConfigValue(rn, "TestSessions", ref strSeqence);
            string[] slist = strSeqence.Split(',', ' ', '\t');
            ArrayList al = new ArrayList();
            bool bchanged = false;
            foreach (string si in slist) {
                int level = 0;
                try {
                    level = int.Parse(si);
                }
                catch (Exception) {
                    bchanged = true;
                    continue;
                }

                if (level < 1 || level > 3) {
                    bchanged = true;
                    continue;
                }

                al.Add(level);
            }
            cfg_level_seq = (int[])al.ToArray(typeof(int));

            //Populate configuration interface
            StringBuilder sb = new StringBuilder();
            foreach (int li in cfg_level_seq) {
                sb.Append(li.ToString());
                sb.Append(',');
            }
            int n = sb.Length;
            if (n > 0) {
                strSeqence = sb.ToString(0, n - 1);
            }
            else {
                strSeqence = null;
            }
            if (bchanged) {
                rm.SetConfigValue(rn, "TestSessions", strSeqence);
            }

            // colors
            //al.Clear();
            //slist = cfg_colors.Split(',');
            //bchanged = false;
            //foreach (string si in slist) {
            //    try {
            //        Color clr = Color.FromName(si.Trim());
            //        al.Add(clr);
            //    }
            //    catch (Exception) {
            //        bchanged = true;
            //    }
            //}

            //listSelCols.Items.Clear();
            //sb = new StringBuilder();
            //foreach (Color clr in al) {
            //    string cname = clr.Name;
            //    ListViewItem item = listSelCols.Items.Add(cname);
            //    item.ImageIndex = Array.IndexOf(AllKnownColors, cname);
            //    sb.Append(cname);
            //    sb.Append(',');
            //}
            //if (bchanged) rm.SetConfigValue(rn, "ColorList", sb.ToString(0, sb.Length - 1));

            textStroopSequence.Text = strSeqence;
            textStroopTrials.Text = cfg_ntrials.ToString();
            textStroopTimeCue.Text = cfg_timecue.ToString();
            textStroopTimeOut.Text = cfg_wtimeout.ToString();
            textStroopTimeRest.Text = cfg_timerest.ToString();
            textStroopTimeRelax.Text = cfg_timerelax.ToString();
            checkStroopRelaxImage.Checked = (cfg_relaximage != 0);

            rm.SaveIfChanged();
        }

        private void buttonSaveStroopConfig_Click(object sender, System.EventArgs e)
        {
            SaveConfig();
        }

        public void SaveConfig()
        {
            ResManager rm = BCIApplication.SysResource;
            string rn = "StroopTest";
            rm.SetConfigValue(rn, "TestSessions", textStroopSequence.Text);
            rm.SetConfigValue(rn, "NumTrials", textStroopTrials.Text);
            rm.SetConfigValue(rn, "TimeCue", textStroopTimeCue.Text);
            rm.SetConfigValue(rn, "TimeoutWait", textStroopTimeOut.Text);
            rm.SetConfigValue(rn, "TimeRest", textStroopTimeRest.Text);
            rm.SetConfigValue(rn, "TimeRelax", textStroopTimeRelax.Text);
            rm.SetConfigValue(rn, "RelaxDispImage", (checkStroopRelaxImage.Checked ? 1 : 0).ToString());

            // colors
            //StringBuilder sb = new StringBuilder();
            //string clist = null;
            //foreach (ListViewItem it in listSelCols.Items) {
            //    sb.Append(it.Text);
            //    sb.Append(',');
            //}
            //if (sb.Length > 0) clist = sb.ToString(0, sb.Length - 1);
            //rm.SetConfigValue(rn, "ColorList", clist);

            try {
                LoadContent(rm);
            }
            catch (Exception) {
                MessageBox.Show("Error in configuration!");
                LoadContent();
            }
        }

        private void buttonTryStroopTest_Click(object sender, System.EventArgs e)
        {
            buttonSaveStroopConfig.PerformClick();
            StroopTestForm frm = new StroopTestForm();
            frm.DryRun = true;
            this.TopLevelControl.Hide();
            frm.ShowDialog();
            this.TopLevelControl.Show();
        }

        private void buttonStartStroopTest_Click(object sender, System.EventArgs e)
        {
            buttonSaveStroopConfig.PerformClick();
            StroopTestForm frm = new StroopTestForm();
            this.TopLevelControl.Hide();
            frm.ShowDialog();
            this.TopLevelControl.Show();
        }

        //private void buttonStroopAddColor_Click(object sender, System.EventArgs e)
        //{
        //    string scol = (string)listAllCols.SelectedItems[0].Text;
        //    foreach (ListViewItem it in listSelCols.Items) {
        //        string ycol = it.Text;
        //        if (scol == ycol) return;
        //    }
        //    ListViewItem it1 = listSelCols.Items.Add(scol);
        //    it1.ImageIndex = Array.IndexOf(AllKnownColors, scol);
        //}

        //private void buttonStroopDelColor_Click(object sender, System.EventArgs e)
        //{
        //    foreach (int sidx in listSelCols.SelectedIndices) {
        //        if (sidx >= 0) {
        //            listSelCols.Items.RemoveAt(sidx);
        //        }
        //    }
        //}

        private void StroopTestCfg_Load(object sender, EventArgs e)
        {
            if (!DesignMode) {
                LoadContent();
            }
        }

        private void buttonDefaultStroop_Click(object sender, System.EventArgs e)
        {
            ResManager rm = new ResManager();
            LoadContent(rm);
            buttonSaveStroopConfig.PerformClick();
        }
    }
}
