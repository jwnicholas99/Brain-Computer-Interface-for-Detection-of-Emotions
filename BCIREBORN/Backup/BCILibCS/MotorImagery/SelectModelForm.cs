using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace BCILib.MotorImagery {
    public partial class SelectModelForm : Form {
        public SelectModelForm()
        {
            InitializeComponent();

            LoadList();
        }

        private void LoadList()
        {
            lbModelList.Items.Clear();

            if (File.Exists(MIConstDef.ModelList)) {
                using (StreamReader sr = File.OpenText(MIConstDef.ModelList)) {
                    string mdl;
                    while ((mdl = sr.ReadLine()) != null) {
                        lbModelList.Items.Add(mdl);
                    }
                }
            }
        }

        private void lbModelList_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnDel.Enabled = lbModelList.SelectedIndex >= 0;
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (lbModelList.SelectedIndices.Count > 0 &&
                MessageBox.Show("Are you sure to delete the selected models?") == DialogResult.OK)
            {
                using (StreamWriter sw = File.CreateText(MIConstDef.ModelList)) {
                    for (int i = 0; i < lbModelList.Items.Count; i++) {
                        if (lbModelList.SelectedIndices.IndexOf(i) < 0) {
                            sw.WriteLine(lbModelList.Items[i]);
                        }
                    }
                }

                LoadList();
            }
        }

        public string SelectedModel
        {
            get
            {
                return lbModelList.SelectedItem as string;
            }
        }
    }
}
