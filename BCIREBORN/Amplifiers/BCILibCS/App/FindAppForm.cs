using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Reflection;

namespace BCILib.App
{
    public partial class FindAppForm : Form
    {
        public FindAppForm()
        {
            InitializeComponent();
        }

        private void FindAppForm_Load(object sender, EventArgs e)
        {
            Assembly asm = this.GetType().Assembly;
            foreach (Type type in asm.GetTypes()) {
                if (!type.IsSubclassOf(typeof(BCIAppForm))) continue;
                listBox1.Items.Add(type.ToString());
            }
        }

        public string Type
        {
            get
            {
                if (listBox1.SelectedIndex >= 0) {
                    return (string)listBox1.Items[listBox1.SelectedIndex];
                }
                else {
                    return null;
                }
            }
            set
            {
                listBox1.SelectedIndex = listBox1.Items.IndexOf(value);
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0) {
                MessageBox.Show("Please select one type, or click cancel.");
                DialogResult = DialogResult.None;
            }
        }
    }
}
