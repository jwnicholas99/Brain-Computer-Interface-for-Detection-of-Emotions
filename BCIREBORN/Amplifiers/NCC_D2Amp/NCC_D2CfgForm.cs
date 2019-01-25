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
    public partial class NCC_D2CfgForm : Form
    {
        Label[] label_chanlist = null;

        public NCC_D2CfgForm()
        {
            InitializeComponent();

            // create channel list
            //var chnamestr = "Fp1,Fp2,F3,F4,C3,C4,P3,P4,O1,O2,F7,F8,T3,T4,T5,T6,Sp1,Sp2,Fz,Cz,Pz,Oz,A1,A2";
            label_chanlist = new[] {
                labelFp1, labelFp2, labelF3, labelF4, labelC3, labelC4, labelP3, labelP4, labelO1, labelO2,
                labelF7, labelF8, labelT3, labelT4, labelT5, labelT6, labelSp1, labelSp2, labelFz, labelCz,
                labelPz, labelOz, labelA1, labelA2
            };

            imp_rvalues = new double[label_chanlist.Length][];
            for (int i = 0; i < label_chanlist.Length; i++) imp_rvalues[i] = new double[64];
        }

        private void buttonChange_Click(object sender, EventArgs e)
        {
            if (textBoxSN.ReadOnly) {
                textBoxSN.ReadOnly = false;
                buttonChange.Text = "Set";
            } else {
                textBoxSN.ReadOnly = true;
            }
        }

        public string SerialNumber
        {
            get
            {
                return textBoxSN.Text;
            }
            set
            {
                textBoxSN.Text = value;
            }
        }

        public string ImpRange
        {
            set
            {
                var args = value.Split(',').Select(x => int.Parse(x)).ToArray();
                if (args.Length > 1 && args[0] < args[1]) {
                    imp_range[0] = args[0];
                    imp_range[1] = args[1];
                }
            }

            get
            {
                return string.Join(",", imp_range.Select(x => x.ToString()).ToArray(), 0, 2);
            }
        }

        int[] chlist = new int[128];
        double[][] imp_rvalues = null;
        uint[] rdata = new uint[1024];
        int rsi = 0;// sample counter

        int iread = 0; // index to the reading frame
        int nskip = 0; // number of int read before AA55


        private void timer_Tick(object sender, EventArgs e)
        {
            if (check_ok) {
                // ReadData first
                int nr = NCC_D2Amp.ReadData(rdata, rdata.Length * 4);
                if (nr > rdata.Length) {
                    nr = rdata.Length;
                }
                int nch = label_chanlist.Length;

                for (int ri = 0; ri < nr; ri++) {
                    if (iread == 0) {
                        if (rdata[ri] == 0x55AA55AA) {
                            iread++;
                            if (nskip > 0) {
                                Console.WriteLine(rsi.ToString() + ",NSKIP=" + nskip);
                            }
                        } else {
                            nskip++;
                        }
                        continue;
                    }

                    iread++;
                    if (iread < 2) {
                        continue;
                    }

                    if (iread <= 1 + nch) {
                        double dv = ((int)(rdata[ri] & 0XFFFFFF) - 0x800000) * 100.0 / 0x100000;
                        int ich = iread - 2;
                        imp_rvalues[ich][rsi] = dv;

                        if (ich + 1 == nch) {
                            iread = 0;
                            nskip = 0;
                            rsi++;

                            int nc = clr_list.Length;
                            double dz = (imp_range[1] - imp_range[0]) / (nc - 1.0);
                            if (rsi == 64) {
                                for (int i = 0; i < nch; i++) {
                                    double mv = imp_rvalues[i].Average();
                                    double var = Math.Sqrt(imp_rvalues[i].Select(x => (x - mv) * (x - mv)).Sum() / rsi);
                                    if (sel_ch == i) {
                                        labelSelChannel.Text = string.Format("{0}:{1:#0}", label_chanlist[sel_ch].Text, var);
                                    }
                                    chlist[i] = var < 70 ? 0 : 1;
                                    int ci = (int) Math.Ceiling((imp_range[1] - var * 0.6) / dz);
                                    if (ci < 0 || var >= 550) ci = 0;
                                    else if (ci >= nc) ci = nc - 1;
                                    label_chanlist[i].BackColor = clr_list[ci];
                                }

                                if (!NCC_D2Amp.SetImpTest(chlist)) {
                                    Console.WriteLine("Error SetImpTest");
                                }
                                rsi = 0;
                            }
                        }
                    }


                }

            }
        }

        NSD2_DATA_TYPE imp_dtype = NSD2_DATA_TYPE.IMPEDANCE_TEST_THRESHOLD_40K;
        bool check_ok = false;

        private void SetImpTest()
        {
            StopAmplifier();
            bool rv = NCC_D2Amp.OpenUsb();
            if (!rv) {
                labelStatus.Text = "Open Error";
                labelStatus.BackColor = Color.Red;
                NCC_D2Amp.CloseUsb();
                return;
            }

            if (!NCC_D2Amp.SetAmpVersion(SerialNumber)) {
                labelStatus.Text = "SNO Error.";
                labelStatus.BackColor = Color.Red;
                return;
            }

            if (!NCC_D2Amp.SetFreq(256)) {
                Console.WriteLine("Error in SetFreq!");
            }

            if (!NCC_D2Amp.SetMontage(4)) {
                Console.WriteLine("Error in SetMontage(4)");
            }


            int[] chlist = new int[128];
            for (int i = 0; i < chlist.Length; i++) chlist[i] = 1;
            if (!NCC_D2Amp.SetChannel(chlist)) {
                Console.WriteLine("Error in SetChannel");
            }


            //if (!ReSetStart()) {
            //    Console.WriteLine("Error in ReSetStart()");
            //} else {
            //    if (!ReSetStop()) {
            //        Console.WriteLine("Error in ReSetStop()");
            //    }
            //}

            if (!NCC_D2Amp.Begin(imp_dtype)) {
                labelSNO.Text = "Start Error";
                labelSNO.BackColor = Color.Red;
            }
            check_ok = true;
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            if (rb.Checked) {
                if (rb == radioButton5K) imp_dtype = NSD2_DATA_TYPE.IMPEDANCE_TEST_THRESHOLD_5K;
                else if (rb == radioButton10K) imp_dtype = NSD2_DATA_TYPE.IMPEDANCE_TEST_THRESHOLD_10K;
                else if (rb == radioButton15K) imp_dtype = NSD2_DATA_TYPE.IMPEDANCE_TEST_THRESHOLD_15K;
                else if (rb == radioButton20K) imp_dtype = NSD2_DATA_TYPE.IMPEDANCE_TEST_THRESHOLD_20K;
                else if (rb == radioButton40K) imp_dtype = NSD2_DATA_TYPE.IMPEDANCE_TEST_THRESHOLD_40K;
                SetImpTest();
            }
        }

        private void StopAmplifier()
        {
            check_ok = false;
            iread = nskip = rsi = 0;
            NCC_D2Amp.Stop(0);
            NCC_D2Amp.Stop(1);
            NCC_D2Amp.Stop(2);
            NCC_D2Amp.CloseUsb();
        }

        private void NCC_D2CfgForm_Load(object sender, EventArgs e)
        {
            SetImpTest();
        }

        private void NCC_D2CfgForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopAmplifier();
        }

        int sel_ch = 0;
        private void labelChannel_Click(object sender, EventArgs e)
        {
            sel_ch = Array.IndexOf(label_chanlist, sender);
        }

        Color[] clr_list = {
           Color.FromArgb(255, 0, 255),
           Color.FromArgb(128, 0, 128),
           Color.FromArgb(255, 0, 0),
           Color.FromArgb(192, 0, 0),
           Color.FromArgb(128, 0, 0),
           Color.FromArgb(192, 128, 0),
           Color.FromArgb(192, 192, 0),
           Color.FromArgb(0, 255, 0),
           Color.FromArgb(0, 192, 0),
           Color.FromArgb(0, 128, 0),
           Color.FromArgb(0, 192, 192),
           Color.FromArgb(0, 0, 255),
           Color.FromArgb(0, 0, 192),
           Color.FromArgb(0, 0, 128),
           Color.FromArgb(0, 0, 64),
        };

        int[] imp_range = { 5, 50 }; // K

        private void panelImpLegend_Paint(object sender, PaintEventArgs e)
        {
            int nc = clr_list.Length;
            Graphics g = e.Graphics;
            int y = 0;
            int dy = panelImpLegend.ClientRectangle.Height / nc;
            int w = panelImpLegend.ClientRectangle.Width / 4;
            double dr = (imp_range[1] - imp_range[0]) / (nc - 1.0);
            double z = imp_range[1];
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Near;
            sf.LineAlignment = StringAlignment.Center;

            for (int i = 0; i < nc; i++) {
                g.FillRectangle(new SolidBrush(clr_list[i]), 0, y, w, dy);
                g.DrawString(z.ToString("#0.#"), Font, Brushes.Black, new RectangleF(w, y, w * 3, dy), sf);
                y += dy;
                z -= dr;
            }
        }

        private void buttonRange_Click(object sender, EventArgs e)
        {
            ImpRangeDlg dlg = new ImpRangeDlg();
            dlg.MaxValue = imp_range[1];
            dlg.MinValue = imp_range[0];
            if (dlg.ShowDialog() == DialogResult.OK) {
                imp_range[0] = dlg.MinValue;
                imp_range[1] = dlg.MaxValue;
                panelImpLegend.Invalidate();
            }
        }
    }
}
