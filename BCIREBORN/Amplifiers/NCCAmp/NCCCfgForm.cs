using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BCILib.Amp
{
    public partial class NCCCfgForm : Form
    {
        public NCCCfgForm()
        {
            InitializeComponent();
        }

        public string[] ChannelNames
        {
            set
            {
                listBoxChannelNames.Items.Clear();
                int nch = value.Length;
                for (int i = 0; i < nch; i++) {
                    listBoxChannelNames.Items.Add(string.Format("{0,3}: {1}", i + 1, value[i]));
                }
            }

            get
            {
                int pos;
                return listBoxChannelNames.Items.Cast<string>()
                    .Select(x => (pos = x.LastIndexOf(':')) >= 0?x.Substring(pos +1).Trim() : x).ToArray();
            }
        }

        private void comboBoxNewName_TextChanged(object sender, EventArgs e)
        {
            buttonChange.Enabled = true;
        }

        string[] all_names = ("HEOL,HEOR,Fp1,Fp2,VEOU,VEOL,F7,F3,Fz,F4,F8,FT7,FC3,FCz,FC4,FT8,T7,C3,Cz,"
            + "C4,T8,TP7,CP3,CPz,CP4,TP8,A1,P7,P3,Pz,P4,P8,A2,O1,Oz,O2,FT9,FT10,PO1,PO2").Split(',');

        private void listBoxChannelNames_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBoxNewName.Items.Clear();
            int sno = listBoxChannelNames.SelectedIndex;
            if (sno >= 0) {
                comboBoxNewName.Items.Add("Ch" + (sno + 1).ToString());
                comboBoxNewName.Items.AddRange(all_names);
                string oname = (string) listBoxChannelNames.Items[sno];
                int pos = oname.LastIndexOf(':');
                if (pos >= 0) oname = oname.Substring(pos + 1).Trim();
                comboBoxNewName.Text = oname;

                comboBoxNewName.Enabled = true;
                buttonChange.Enabled = false;
            } else {
                comboBoxNewName.Enabled = buttonChange.Enabled = false;
            }
        }

        private void buttonChange_Click(object sender, EventArgs e)
        {
            int sno = listBoxChannelNames.SelectedIndex;
            if (sno >= 0) {
                listBoxChannelNames.Items[sno] = string.Format("{0,3}: {1}", sno + 1, comboBoxNewName.Text);
                buttonChange.Enabled = false;
                buttonOK.Enabled = true;
            }
        }
    }
}
