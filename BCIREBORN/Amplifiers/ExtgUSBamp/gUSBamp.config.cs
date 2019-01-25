using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ExtgUSBamp
{
    /// <summary>
    /// Channel
    /// Inpedance
    /// Ground (ABCD, 0:1): 1 to common ground
    /// Refernce(ABCD, 0:1): 1 = to common ground
    /// 
    /// </summary>
    public partial class gUSBampConfig : Form
    {
        public gUSBampConfig()
        {
            InitializeComponent();

            cmbSampleRate.Items.AddRange(new int[] {
                32, 64, 128, 256, 512, 600, 1200, 2400,
                4800, 9600, 19200, 38400}.OfType<object>().ToArray());
        }

        private void gUSBampConfig_Load(object sender, EventArgs e)
        {
            ActiveControl = btnOkey;
        }

        internal void SetConfigData(gUSBamp.ConfigData cfg_data)
        {
            lblGeneralInfo.Text = string.Format("Driver Version = {0}\r\n"
                + "Hardware version = {1}\r\nSerial No = {2}",
                cfg_data.driver_version, cfg_data.hw_version, cfg_data.serial_number);

            cbComGnd1.Checked = cfg_data.ground.GND1 == 1;
            cbComGnd2.Checked = cfg_data.ground.GND2 == 1;
            cbComGnd3.Checked = cfg_data.ground.GND3 == 1;
            cbComGnd4.Checked = cfg_data.ground.GND4 == 1;

            cbComRef1.Checked = cfg_data.reference.ref1 == 1;
            cbComRef2.Checked = cfg_data.reference.ref2 == 1;
            cbComRef3.Checked = cfg_data.reference.ref3 == 1;
            cbComRef4.Checked = cfg_data.reference.ref4 == 1;

            cmbMode.Items.Clear();
            cmbMode.Items.AddRange(Enum.GetNames(typeof(gUSBamp.AmpMode)));
            cmbMode.SelectedIndex = (int)cfg_data.mode;

            cmbSampleRate.Text = cfg_data.sample_rate.ToString();

            if (cfg_data.band_pass == -1) cmbFilter.SelectedIndex = 0;
            else if (filter_spec != null && cfg_data.band_pass >= 0 && cfg_data.band_pass < filter_spec.Length) {
                string desc = filter_spec[cfg_data.band_pass].ToString() + "----" + cfg_data.band_pass.ToString();
                cmbFilter.SelectedIndex = cmbFilter.Items.IndexOf(desc);
            }

            if (cfg_data.notch == -1) cmbNotch.SelectedIndex = 0;
            else if (notch_spec != null && cfg_data.notch >= 0 && cfg_data.notch < notch_spec.Length) {
                string desc = notch_spec[cfg_data.notch].ToString() + "----" + cfg_data.notch.ToString();
                cmbNotch.SelectedIndex = cmbNotch.Items.IndexOf(desc);
            }
        }

        internal void UpdateData(gUSBamp.ConfigData cfg_data)
        {
            cfg_data.ground.GND1 = cbComGnd1.Checked ? 1 : 0;
            cfg_data.ground.GND2 = cbComGnd2.Checked ? 1 : 0;
            cfg_data.ground.GND3 = cbComGnd3.Checked ? 1 : 0;
            cfg_data.ground.GND4 = cbComGnd4.Checked ? 1 : 0;
            cfg_data.reference.ref1 = cbComRef1.Checked ? 1 : 0;
            cfg_data.reference.ref2 = cbComRef2.Checked ? 1 : 0;
            cfg_data.reference.ref3 = cbComRef3.Checked ? 1 : 0;
            cfg_data.reference.ref4 = cbComRef4.Checked ? 1 : 0;
            cfg_data.mode = (gUSBamp.AmpMode) cmbMode.SelectedIndex;
            cfg_data.sample_rate = ushort.Parse(cmbSampleRate.Text);

            string line = cmbFilter.Text;
            int s0 = line.IndexOf("----");
            if (s0 >= 0) {
                cfg_data.band_pass = int.Parse(line.Substring(s0 + 4));
            }

            line = cmbNotch.Text;
            s0 = line.IndexOf("----");
            if (s0 >= 0) {
                cfg_data.notch = int.Parse(line.Substring(s0 + 4));
            }
        }

        gUSBamp.FILT[] filter_spec = null;

        internal void SetFilterSpec(gUSBamp.FILT[] fspec)
        {
            filter_spec = fspec;
        }

        private void cmbSampleRate_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbFilter.Items.Clear();
            cmbFilter.Items.Add("No filter ---- -1");
            cmbFilter.SelectedIndex = 0;
            ushort spr = ushort.Parse(cmbSampleRate.Text);

            if (filter_spec != null) {
                //var s = from it in filter_spec
                //        where it.fs == spr
                //        select it.ToString();
                //cmbFilter.Items.AddRange(s.ToArray<string>());
                var data = filter_spec.Select((it, i) => new {it, ind=i}).
                    Where(item => item.it.fs == spr);
                foreach (var itx in data) {
                    int ni = cmbFilter.Items.Add(itx.it.ToString() + "----" + itx.ind);
                }
            }

            cmbNotch.Items.Clear();
            cmbNotch.Items.Add("No Notch Filter ---- -1");
            cmbNotch.SelectedIndex = 0;
            if (notch_spec != null)
            {
                var data = notch_spec.Select((it, i) => new { it, ind = i })
                    .Where(itx => itx.it.fs == spr);
                foreach (var itx in data) {
                    cmbNotch.Items.Add(itx.it.ToString() + "----" + itx.ind);
                }
            }
        }

        gUSBamp.FILT[] notch_spec = null;
        internal void SetNotchData(gUSBamp.FILT[] nspec)
        {
            notch_spec = nspec;
        }
    }
}
