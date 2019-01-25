using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BCILib.App;
using System.IO;

namespace BCILib.MotorImagery.ArtsBCI
{
    public partial class ERDFeedbackControl : UserControl
    {
        public ERDFeedbackControl()
        {
            InitializeComponent();
        }

        public bool IsVisible
        {
            set
            {
                if (InvokeRequired) {
                    this.Invoke((dlgarg0)delegate()
                    {
                        base.Visible = value;
                    });
                }
                else {
                    base.Visible = value;
                }
            }

            get
            {
                if (InvokeRequired) {
                    return (bool)this.Invoke((Func<bool>)delegate()
                    {
                        return this.Visible;
                    });
                }
                else {
                    return this.Visible;
                }
            }
        }

        public void ShowImage(Image img)
        {
            if (InvokeRequired) {
                Invoke((Func<Image, bool>)delegate(Image img_in)
                {
                    ShowImage(img_in);
                    return true;
                }, img);
            }
            else {
                pictureBox1.Image = img;

                if (img != null) {
                    _num++;
                    label1.Text = _num.ToString();
                }

                //Visible = true;
            }
        }

        private int _num = 0;
        internal void ClearDisp()
        {
            if (InvokeRequired) {
                Invoke((dlgarg0)delegate()
                {
                    ClearDisp();
                });
            }
            else {
                _num = 0;
                label1.Text = null;
                pictureBox1.Image = null;
                //Visible = false;
            }
        }
    }
}
