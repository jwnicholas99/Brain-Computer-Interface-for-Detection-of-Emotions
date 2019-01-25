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
    public partial class ZeoConfigForm : Form
    {
        public ZeoConfigForm()
        {
            InitializeComponent();
        }

        public bool RevChannel
        {
            set
            {
                checkRevChannel.Checked = value;
            }
            get
            {
                return checkRevChannel.Checked;
            }
        }
        public bool Interpolate
        {
            set
            {
                checkInterpolate.Checked = value;
            }
            get
            {
                return checkInterpolate.Checked;
            }
        }
        public bool LowFilter
        {
            set
            {
                checkLowFilter.Checked = value;
            }
            get
            {
                return checkLowFilter.Checked;
            }
        }
        public bool LogDebug
        {
            get
            {
                return checkDebugLog.Checked;
            }
            set
            {
                checkDebugLog.Checked = value;
            }
        }
    }
}
