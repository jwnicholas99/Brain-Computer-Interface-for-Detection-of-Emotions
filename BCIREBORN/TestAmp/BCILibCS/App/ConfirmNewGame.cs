using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BCILib.App
{
    public partial class ConfirmNewGame : Form
    {
        public ConfirmNewGame()
        {
            InitializeComponent();
        }

        public static string ShowDialog(string gameName)
        {
            ConfirmNewGame dlg = new ConfirmNewGame();
            dlg.textGameName.Text = gameName;
            dlg.textGameName.SelectAll();
            if (dlg.ShowDialog() == DialogResult.OK) {
                return dlg.textGameName.Text;
            }
            else {
                return null;
            }
        }
    }
}
