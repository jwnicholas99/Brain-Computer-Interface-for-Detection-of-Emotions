using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BCILib.P300
{
    public partial class BCI_P300_Console : BCILib.App.BCIAppForm
    {
        public BCI_P300_Console()
        {
            InitializeComponent();
        }

        private void BCI_P300_Console_Load(object sender, EventArgs e)
        {
            p300ConfigCtrl1.InitContent();

            P300Manager.VerifyCntTrain(@"C:\P300Speller\Users_BCI_P300\tester1\20141106\DataTraining\P300Speller_2_2014-11-06_Training\ACEGIKMOQZ_20141106-144411_NeuroScan_2.cnt",// 12, 8, 7);
                30, 8, 10);
        }

        protected override void LoadConfig(BCILib.Util.ResManager rm)
        {
            p300ConfigCtrl1.ResetConfig(rm);
        }
    }
}
