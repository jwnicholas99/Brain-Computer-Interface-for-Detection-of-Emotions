using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Management;

namespace BCILibCS.Util
{
    public partial class SerialPortCtrl : UserControl
    {
        public SerialPortCtrl()
        {
            InitializeComponent();
            RefreshPortList();
        }

        public void RefreshPortList()
        {
            string[] sel_ports = GetSelectedPorts();
            CloseOpenPorts();

            listView.Items.Clear();

            //string[] plist = SerialPort.GetPortNames();
            //if (plist.Length > 0) {
            //    foreach (string pname in plist) {
            //        int len = pname.Length;
            //        int p1 = len - 1;
            //        while (!Char.IsDigit(pname[p1])) p1--;
            //        ListViewItem it;
            //        if (p1 == len - 1) {
            //            it = listView.Items.Add(pname);
            //        }
            //        else {
            //            it = listView.Items.Add(pname.Substring(0, p1 + 1));
            //        }
            //        it.SubItems.Add("");

            //        if (Array.IndexOf(sel_ports, it.Text) >= 0) it.Checked = true;
            //    }
            //}

            PopulateDesc();

            foreach (ListViewItem it in listView.Items) {
                if (Array.IndexOf(sel_ports, it.Text) >= 0) it.Checked = true;
            }
        }

        private string[] GetSelectedPorts()
        {
            List<string> sel_list = new List<string>();
            foreach (ListViewItem it in listView.CheckedItems) {
                sel_list.Add(it.Text);
            }
            return sel_list.ToArray();
        }

        public string SelComport
        {
            set
            {
                foreach (ListViewItem it in listView.Items) {
                    if (it.Text == value) {
                        it.Checked = true;
                        break;
                    }
                }
            }

            get
            {
                if (listView.CheckedIndices.Count == 0) return null;
                return listView.CheckedItems[0].Text;
            }
        }

        List<SerialPort> all_openports = new List<SerialPort>();

        private void OpenPorts()
        {
            if (all_openports.Count <= 0) {
                foreach (ListViewItem it in listView.CheckedItems) {
                    try {
                        SerialPort sp = new SerialPort(it.Text);
                        sp.Open();
                        all_openports.Add(sp);
                    } catch (Exception) { }
                }
            }
        }

        private void buttonEnableDTR_Click(object sender, EventArgs e)
        {
            bool check = !checkBoxDTR.Checked;
            SetDtr(check);
        }

        public void SetDtr(bool check)
        {
            checkBoxDTR.Checked = check;
            OpenPorts();
            foreach (SerialPort sp in all_openports) {
                try {
                    if (!sp.IsOpen) sp.Open();
                    sp.DtrEnable = check;
                } catch (Exception) {
                    all_openports.Remove(sp);
                }
            }
        }

        private void buttonToggleRTS_Click(object sender, EventArgs e)
        {
            bool check = !checkBoxRTS.Checked;
            SetRTS(check);
        }

        public void SetRTS(bool check)
        {
            checkBoxRTS.Checked = check;
            OpenPorts();
            foreach (SerialPort sp in all_openports) {
                try {
                    if (!sp.IsOpen) sp.Open();
                    sp.RtsEnable = check;
                } catch (Exception) {
                    all_openports.Remove(sp);
                }
            }
        }

        public void CloseOpenPorts()
        {
            foreach (SerialPort sp in all_openports) {
                try {
                    if (sp.IsOpen) {
                        sp.Close();
                    }
                    sp.Dispose();
                } catch (Exception) { }
            }
            all_openports.Clear();
        }

        private void PopulateDesc()
        {
            ConnectionOptions options = new ConnectionOptions();
            options.Impersonation = ImpersonationLevel.Impersonate;
            options.Authentication = AuthenticationLevel.Default;
            options.EnablePrivileges = true;

            ManagementScope connectScope = new ManagementScope();
            connectScope.Path = new ManagementPath(@"\\" + Environment.MachineName + @"\root\CIMV2");
            connectScope.Options = options;
            connectScope.Connect();

            ObjectQuery objectQuery = new ObjectQuery("SELECT * FROM Win32_PnPEntity WHERE ConfigManagerErrorCode = 0");
            ManagementObjectSearcher comPortSearcher = new ManagementObjectSearcher(connectScope, objectQuery);

            Regex reg = new Regex(@"(.*)\((COM.*)\)$");

            using (comPortSearcher) {
                string caption = null;
                foreach (ManagementObject obj in comPortSearcher.Get()) {
                    if (obj != null) {
                        object captionObj = obj["Caption"];
                        if (captionObj != null) {
                            caption = captionObj.ToString();
                            Match m = reg.Match(caption);
                            if (m.Success) {
                                string name = m.Groups[1].Value;
                                string pn = m.Groups[2].Value;

                                ListViewItem it = listView.Items.Add(pn);
                                it.SubItems.Add(name);
                                Trace.WriteLine(caption);
                            }
                        } else {
                            Trace.WriteLine("Error?");
                        }
                    } else {
                        Trace.WriteLine("Error?");
                    }
                }
            }
        }

        public void SelKeyPorts(string key)
        {
            foreach (ListViewItem it in listView.Items) {
                if (it.SubItems[1].Text.IndexOf(key) >= 0) it.Checked = true;
            }
        }

        public string[] PortNames
        {
            get
            {
                List<string> pnames = new List<string>();
                foreach (ListViewItem it in listView.Items) {
                    pnames.Add(it.Text);
                }
                return pnames.ToArray();
            }
        }

        internal static string[] GetPortNames()
        {
            SerialPortCtrl ctrl = new SerialPortCtrl();
            ctrl.RefreshPortList();
            return ctrl.PortNames;
        }
    }
}
