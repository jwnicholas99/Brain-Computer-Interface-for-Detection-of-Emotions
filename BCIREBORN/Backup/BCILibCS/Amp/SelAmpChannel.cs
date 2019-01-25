using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using BCILib.App;

namespace BCILib.Amp
{
    public partial class SelAmpChannel : UserControl
    {
        Action dlg_amp_change = null;
        public SelAmpChannel()
        {
            InitializeComponent();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
            if (dlg_amp_change != null) {
                AmpContainer.AmpChanged -= dlg_amp_change;
                dlg_amp_change = null;
            }
        }

        public void ShowAmplifier()
        {
            if (dlg_amp_change == null) {
                dlg_amp_change = new Action(ShowAmplifier);
                AmpContainer.AmpChanged += dlg_amp_change;
            }

            string selAmp = comboAmplifier.Text;
            comboAmplifier.Items.Clear();
            int n = AmpContainer.Count;
            for (int i = 0; i < n; i++) {
                string ampid = AmpContainer.GetAmplifier(i).ID;
                comboAmplifier.Items.Add(ampid);
                if (selAmp == ampid) comboAmplifier.SelectedIndex = i;
            }
            if (n > 0 && comboAmplifier.SelectedIndex < 0) comboAmplifier.SelectedIndex = 0;
        }

        private void comboAmplifier_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (checkedListBox1.Items.Count > 0) {
                checkedListBox1.Tag = SelectedList;
            }

            checkedListBox1.Items.Clear();
            textBoxNumSel.Text = "0";

            if (string.IsNullOrEmpty(comboAmplifier.Text)) {
                return;
            }

            Amplifier amp = AmpContainer.GetAmplifier(comboAmplifier.Text);
            checkedListBox1.Items.AddRange(amp.ChannelNames);

            string[] slist = (string[])checkedListBox1.Tag;
            if (slist != null && slist.Length > 0) {
                SelectedList = slist;
            }
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBoxNumSel.Text = checkedListBox1.CheckedIndices.Count.ToString();
        }

        public int SelectedNum
        {
            get
            {
                return checkedListBox1.CheckedIndices.Count;
            }
            set
            {
                int sel_num = value;
                int na = checkedListBox1.Items.Count;
                int[] slist = SelectedItemIndices;
                
                if (slist.Length > sel_num) {
                    // remove extra checed items
                    for (int n = sel_num; n < slist.Length; n++) {
                        checkedListBox1.SetItemChecked(slist[n], false);
                    }
                }
                else if (slist.Length < sel_num) {
                    // set more checked items
                    int nc = slist.Length;
                    for (int n = 0; nc < sel_num && n < na; n++) {
                        if (Array.IndexOf(slist, n) < 0) {
                            checkedListBox1.SetItemChecked(n, true);
                            nc++;
                        }
                    }
                }
                textBoxNumSel.Text = checkedListBox1.CheckedIndices.Count.ToString();
            }
        }

        public int[] SelectedItemIndices
        {
            get
            {
                int[] idx = new int[SelectedNum];
                checkedListBox1.CheckedIndices.CopyTo(idx, 0);
                return idx;
            }
        }

        public string SelectedAmplifier
        {
            get
            {
                return comboAmplifier.Text;
            }
        }

        public string[] SelectedList
        {
            set
            {
                checkedListBox1.Tag = value;
                for (int it = 0; it < checkedListBox1.Items.Count; it++) {
                    if (Array.IndexOf(value, checkedListBox1.Items[it]) >= 0) {
                        checkedListBox1.SetItemChecked(it, true);
                    }
                }
                textBoxNumSel.Text = checkedListBox1.CheckedIndices.Count.ToString();
            }
            get
            {
                string[] list = new string[checkedListBox1.CheckedItems.Count];
                checkedListBox1.CheckedItems.CopyTo(list, 0);
                return list;
            }
        }

        public string SelectedString
        {
            get
            {
                return string.Join(",", checkedListBox1.CheckedItems.Cast<string>().ToArray());
            }

            set
            {
                ResetSelChannelList();
                if (!string.IsNullOrEmpty(value)) {
                    SelectedList = value.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                }
            }
        }

        private void buttonSelAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBox1.Items.Count; i++) {
                checkedListBox1.SetItemChecked(i, true);
            }
            textBoxNumSel.Text = checkedListBox1.CheckedIndices.Count.ToString();
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            ResetSelChannelList();
        }

        private void ResetSelChannelList()
        {
            for (int i = 0; i < checkedListBox1.Items.Count; i++) {
                checkedListBox1.SetItemChecked(i, false);
            }
            textBoxNumSel.Text = checkedListBox1.CheckedIndices.Count.ToString();
        }

        private void buttonAmpRefresh_Click(object sender, EventArgs e)
        {
            ShowAmplifier();
        }
    }
}
