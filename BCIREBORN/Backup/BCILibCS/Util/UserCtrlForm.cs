using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BCILib.Util
{
    public partial class UserCtrlForm : Form
    {
        public UserCtrlForm()
        {
            InitializeComponent();
        }

        private DialogResult ShowUserCtrl(UserControl ctl)
        {
            // resize
            Rectangle rt = this.ClientRectangle;
            int w = rt.Width - 10;
            int h = buttonOK.Top - 10;

            this.Width += ctl.Width - w;
            this.Height += ctl.Height - h;
            ctl.Left = 5;
            ctl.Top = 5;
            ctl.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;

            this.Controls.Add(ctl);

            return this.ShowDialog();
        }

        public static DialogResult ShowCtrl(UserControl uctrl)
        {
            UserCtrlForm ctrl = new UserCtrlForm();
            return ctrl.ShowUserCtrl(uctrl);
        }
    }
}
