using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BCILib.Util
{
    public partial class TCPConfigCtrl : UserControl
    {
        public TCPConfigCtrl()
        {
            InitializeComponent();
        }

        public string Host
        {
            set
            {
                tbHost.Text = value;
            }

            get
            {
                return tbHost.Text;
            }
        }

        public int Port
        {
            set
            {
                tbPort.Text = value.ToString();
            }

            get
            {
                int port = 0;
                if (!string.IsNullOrEmpty(tbPort.Text)) {
                    int.TryParse(tbPort.Text, out port);
                }
                return port;
            }
        }
    }
}
