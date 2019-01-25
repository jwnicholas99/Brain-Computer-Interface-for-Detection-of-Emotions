using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Linq;

using BCILib.App;
using BCILib.Util;
using BCILib.Amp;
using System.IO;

namespace BCILib.Amp
{
    internal partial class SelectAmplifierForm : Form
    {
        public readonly string ExtAmpPath = "ExtAmp";

        public SelectAmplifierForm()
        {
            InitializeComponent();
        }

        private void buttonOkey_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();

            ResManager rm = BCIApplication.AppResource;
            rm.SetConfigValue("InstalledDevices", string.Join(",", checkedListBox1.CheckedItems.Cast<string>().ToArray()));
            rm.SaveFile();
        }

        private void SelectAmplifierForm_Load(object sender, EventArgs e)
        {
            ResManager rm = BCIApplication.AppResource;

            string line = rm.GetConfigValue("InstalledDevices");
            string[] clist = string.IsNullOrEmpty(line)? new string[0]: line.Split(',');

            string[] rl = rm.GetAllResource();
            foreach (string rn in rl) {
                string typename = rm.GetConfigValue(rn, "Type");
                if (string.IsNullOrEmpty(typename)) continue;
                if (Amplifier.FromConfigure(rm, rn) == null) continue;
                checkedListBox1.Items.Add(rn, clist.Any(x => x == rn));
            }

            comboBoxAmpList.Items.Clear();
            asm_locations.Clear();

            // 20140526 - create new amplifiers
            // internal amplifiers
            Type ampType = typeof(Amplifier);
            Assembly asm = ampType.Assembly;
            foreach (Type type in asm.GetTypes()) {
                if (!type.IsSubclassOf(ampType)) continue;
                if (type.IsAbstract) continue;
                CrtAmpItem(type);
            }

            // External amplifiers
            string ext_path = Path.Combine(BCIApplication.RootPath, ExtAmpPath);
            if (Directory.Exists(ext_path)) {
                string[] plist = Directory.GetFiles(ext_path, "*.dll");
                foreach (string dllpath in plist) {
                    try {
                        asm = Assembly.LoadFrom(dllpath);
                    } catch (Exception) {
                        continue;
                    }
                    if (asm != null) {
                        foreach (Type type in asm.GetTypes()) {
                            if (!type.IsSubclassOf(ampType)) continue;
                            if (type.IsAbstract) continue;

                            CrtAmpItem(type);
                        }
                    }
                }
            }

            // default amplifier display mode
            if (comboBoxAmpList.Items.Count > 0) {
                comboBoxAmpList.SelectedIndex = 0;
            }
        }

        List<string> asm_locations = new List<string>();

        private void CrtAmpItem(Type type)
        {
            string name = Amplifier.GetTypeName(type);

            // if amplifer available? OS installed?
            var ainf = type.GetField("DllPath"); //BindingFlags.Public | BindingFlags.Static
            if (ainf != null) {
                string fn = (string)ainf.GetValue(null);
                string[] dlist = fn.Split(';');
                foreach (string dfn in dlist) {
                    string tfn = Path.Combine(Path.GetDirectoryName(type.Assembly.Location), dfn);
                    if (!File.Exists(tfn) && !FindInSystemPath(dfn)) {
                        return;
                    }
                }
            }

            asm_locations.Add(type.Assembly.Location);
            comboBoxAmpList.Items.Add(name);
        }

        private bool FindInSystemPath(string fn)
        {
            if (File.Exists(fn)) return true;
            if (File.Exists(Path.Combine(BCIApplication.RootPath, fn))) return true;
            foreach (var sd in Environment.GetEnvironmentVariable("Path").Split(';')) {
                //Environment.GetFolderPath(Environment.SpecialFolder.System)
                string tfn = Path.Combine(sd, fn);
                if (File.Exists(tfn)) return true;
            }

            return false;
        }

        private void checkBoxMultiSelection_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBoxMultiSelection.Checked) {
                ClearCheck();
            }
        }

        private void ClearCheck()
        {
            foreach (int sno in checkedListBox1.CheckedIndices) {
                checkedListBox1.SetItemChecked(sno, false);
            }
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (!checkBoxMultiSelection.Checked) {
                if (e.NewValue == CheckState.Checked) {
                    ClearCheck();
                }
            }
        }

        private void buttonCreateAmp_Click(object sender, EventArgs e)
        {
            int sno = comboBoxAmpList.SelectedIndex;
            if (sno < 0) return;

            Assembly asm = Assembly.LoadFrom(asm_locations[sno]);
            string tname = comboBoxAmpList.Items[sno] as string;

            Amplifier amp = (Amplifier)asm.CreateInstance(tname);
            if (amp == null) amp = (Amplifier)asm.CreateInstance(Amplifier.DefaultPackage + tname);

            if (amp == null) {
                MessageBox.Show("Cannot find amplifer type!");
            } else if (amp.Configure()) {
                ResManager rm = BCIApplication.AppResource;
                amp.SaveConfig(rm);
                checkedListBox1.Items.Add(amp.ID, false);
            }
        }
    }
}
