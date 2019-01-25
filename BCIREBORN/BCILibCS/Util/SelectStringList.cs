using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BCILib.Util
{
    public partial class SelectStringList : Form
    {
        public SelectStringList()
        {
            InitializeComponent();
            Candidates = null;
        }

        public SelectStringList(string[] candidates):this()
        {
            Candidates = candidates;
        }

        public string CandidateString
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                bool first = true;
                foreach (string cn in checkedListBox1.Items) {
                    if (!first) {
                        sb.Append(',');
                    }
                    else {
                        first = false;
                    }
                    sb.Append(cn);
                }
                return sb.ToString();
            }
            set
            {
                Candidates = value.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        public string[] Candidates
        {
            set
            {
                checkedListBox1.Items.Clear();
                if (value != null) {
                    checkedListBox1.Items.AddRange(value);
                }
            }

            get
            {
                string[] list = new string[checkedListBox1.Items.Count];
                checkedListBox1.Items.CopyTo(list, 0);
                return list;
            }
        }

        public string[] SelectedList
        {
            set
            {
                for (int it = 0; it < checkedListBox1.Items.Count; it++) {
                    if (Array.IndexOf(value, checkedListBox1.Items[it]) >= 0) {
                        checkedListBox1.SetItemChecked(it, true);
                    }
                }
            }
            get
            {
                string[] list = new string[checkedListBox1.CheckedItems.Count];
                checkedListBox1.CheckedItems.CopyTo(list, 0);
                return list;
            }
        }

        /// <summary>
        /// Get or set the checked items index
        /// </summary>
        public int[] SelectedListIndex
        {
            get
            {
                int[] list = new int[checkedListBox1.CheckedIndices.Count];
                checkedListBox1.CheckedIndices.CopyTo(list, 0);
                return list;
            }

            set
            {
                if (value != null) {
                    foreach (int i in value) {
                        checkedListBox1.SetItemChecked(i, true);
                    }
                }
            }
        }

        private void buttonSelectAll_Click(object sender, EventArgs e)
        {
            for (int it = 0; it < checkedListBox1.Items.Count; it++) {
                checkedListBox1.SetItemChecked(it, true);
            }
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            for (int it = 0; it < checkedListBox1.Items.Count; it++) {
                checkedListBox1.SetItemChecked(it, false);
            }
        }

        public string SelectedString
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                bool first = true;
                foreach (string cn in checkedListBox1.CheckedItems) {
                    if (!first) {
                        sb.Append(',');
                    } else {
                        first = false;
                    }
                    sb.Append(cn);
                }
                return sb.ToString();
            }

            set
            {
                SelectedList = value.Split(new char[] {',', ' '}, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        public int SelectedNum
        {
            get
            {
                return checkedListBox1.CheckedIndices.Count;
            }
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
    }
}
