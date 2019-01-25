using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.IO;
using BCILib.Util;

namespace BCILib.App
{
    public partial class UserInfoForm : Form
    {
        public UserInfoForm()
        {
            InitializeComponent();

            textBoxDate.Text = DateTime.Now.ToString("yyyyMMdd");
        }

        public string Subject
        {
            set
            {
                textSubjectName.Text = value;
                textSubjectName.ReadOnly = true;
                buttonCreate.Text = "Save";

                // load file
                string fn = Path.Combine(BCIApplication.UsersRoot, value);
                fn = Path.Combine(fn, "user.cfg");
                if (File.Exists(fn)) {
                    ResManager rm = new ResManager(fn);
                    textBoxFullName.Text = rm.GetConfigValue("FullName");
                    comboGender.SelectedItem = rm.GetConfigValue("Gender");
                    textAge.Text = rm.GetConfigValue("Age");
                    textEEGCap.Text = rm.GetConfigValue("EEGCap");
                    string line = rm.GetConfigValue("Date");
                    textBoxComments.Text = rm.GetConfigValue("Conditions");
                }

                comboGender.Focus();
            }

            get
            {
                return textSubjectName.Text;
            }
        }

        private void buttonCreate_Click(object sender, EventArgs e)
        {
            if (textSubjectName.Text == null) return;

            string dir = Path.Combine(BCIApplication.UsersRoot, Subject);
            if (Directory.Exists(dir)) {
                if (!textSubjectName.ReadOnly) {
                    MessageBox.Show("Subject already created.");
                    textSubjectName.SelectAll();
                    textSubjectName.Focus();
                    return;
                }
            }

            Directory.CreateDirectory(dir);

            // Save user information
            ResManager rm = new ResManager();
            string cfn = Path.Combine(dir, "user.cfg");
            if (File.Exists(cfn)) rm.LoadFile(cfn);
            rm.SetConfigValue("UserName", textSubjectName.Text);
            rm.SetConfigValue("FullName", textBoxFullName.Text);
            rm.SetConfigValue("Gender", (string)comboGender.SelectedItem);
            rm.SetConfigValue("Age", textAge.Text);
            rm.SetConfigValue("EEGCap", textEEGCap.Text);
            rm.SetConfigValue("Conditions", textBoxComments.Text);
            rm.SaveFile(cfn);

            this.DialogResult = DialogResult.OK;
            Close();
        }
    }
}
