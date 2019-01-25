using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using BCILib.Amp;
using BCILib.Util;
using System.Threading;
using System.Diagnostics;

namespace BCILib.App
{
    public partial class BCIAppForm : Form
    {
        public BCIAppForm()
        {
            InitializeComponent();
        }

        private void toolStripAmp_Click(object sender, EventArgs e)
        {
            AmpContainer.ShowContainer();
        }

        private void toolStripSysConfig_Click(object sender, EventArgs e)
        {
            ResManager rm = BCIApplication.SysResource;
            if (rm.ShowDialog()) {
                LoadConfig(rm);
            }
        }
        protected void LoadConfig()
        {
            LoadConfig(SysResource);
        }

        protected virtual void LoadConfig(ResManager rm)
        {
        }

        protected void LoadAppConfig()
        {
            LoadAppConfig(AppResource);
        }
        protected virtual void LoadAppConfig(ResManager rm)
        {
        }

        protected void SaveAppConfig()
        {
            SaveAppConfig(AppResource);
        }
        protected virtual void SaveAppConfig(ResManager rm)
        {
        }

        private void toolStripViewLog_Click(object sender, EventArgs e)
        {
            ConsoleOutForm.ShowWindow();
        }

        private void BCIAppForm_Load(object sender, EventArgs e)
        {
            if (!DesignMode) {
                ConsoleOutForm.Initialize();
                this.Text = string.Format(this.Text + ": " + BCIApplication.AppUser);
                toolStripSysConfig.ToolTipText = "System Config: " + BCIApplication.RootPath;
                AmpContainer.Intialize();

                WindowsUtil.ReqSystemDisplay();
            }
        }

        private void tsbPause_Click(object sender, EventArgs e)
        {
            tsbPause.Checked = !tsbPause.Checked;
        }

        protected bool Paused
        {
            get {
                if (InvokeRequired) {
                    bool pause = false;
                    Invoke((Action)(() =>
                        pause = tsbPause.Checked
                    ));
                    return pause;
                } else {
                    return tsbPause.Checked;
                }
            }

            set {
                if (InvokeRequired) {
                    Invoke((Action)(() => tsbPause.Checked = value));
                } else {
                    tsbPause.Checked = value;
                }
            }
        }

        public static string TimeStamp
        {
            get
            {
                return BCIApplication.TimeStamp;
            }
        }

        public static string RootPath
        {
            get
            {
                return BCIApplication.RootPath;
            }
        }

        protected static void LogMessage(string fmt, params object[] args)
        {
            BCIApplication.LogMessage(fmt, args);
        }

        protected ConsoleCapture.ConsoleFile cfile = null;

        protected BCITask bci_work = BCITask.Training;
        protected string bci_path;
        protected ManualResetEvent evt_stop = new ManualResetEvent(false);
        protected ManualResetEvent evt_kb = new ManualResetEvent(false);

        protected bool StartWork(string protocol, string key, bool confirm_path)
        {
            if (AmpContainer.Count <= 0) {
                MessageBox.Show("No amplifer find!");
                return false;
            }

            Amplifier amp = AmpContainer.GetAmplifier(0);
            if (amp is FakeAmplifier || amp is FileSimulator || amp is LiveSimulator) {
                if (MessageBox.Show("Amplifier is not real amplifer, continue?", "Warning", MessageBoxButtons.OKCancel) == DialogResult.Cancel) {
                    return false;
                }
            }

            ResManager rm = BCIApplication.SysResource;
            BCIApplication.SetProtocolName(rm, protocol);

            bci_path = bci_work == BCITask.Training? TrainDirSpecForm.GetTrainingPath(rm, confirm_path)
                :TestDirSpecForm.GetProcPath(rm, confirm_path);

            if (string.IsNullOrEmpty(bci_path)) return false;

            // start log
            if (!Directory.Exists(bci_path)) {
                Directory.CreateDirectory(bci_path);
            }
            cfile = ConsoleCapture.StartConsoleFile(Path.Combine(bci_path, key + "_main.log"));
            AmpContainer.StartRecord(bci_path, key);

            return true;
        }

        protected void StopWork()
        {
            AmpContainer.StopRecord();
            if (cfile != null) cfile.EndLogFile();

            if (bci_work == BCITask.Training && !evt_stop.WaitOne(0, false)) {
                BCIApplication.AddTrainingPath(bci_path);
            }
        }

        protected void SendCode(int code)
        {
            AmpContainer.SendAll(code);
            LogMessage("StimCode={0}", code);
        }

        protected ResManager AppResource
        {
            get
            {
                return BCIApplication.AppResource;
            }
        }

        protected ResManager SysResource
        {
            get
            {
                return BCIApplication.SysResource;
            }
        }

        private void BCIAppForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!DesignMode) {
                ConsoleOutForm.CloseWindow();
                //BCILib.Amp.AmpContainer.GlobalContainer.ClearAllAmplifiers();
                WindowsUtil.RelSystemDisplay();
                if (kb_func != null) {
                    KMCapture.kbd_output -= kb_func;
                    kb_func = null;
                }
            }
            AmpContainer.StopAll();
        }

        Func<Keys, uint, uint, bool> kb_func = null;
        protected List<Keys> _user_keys = new List<Keys>();
        protected bool _chk_keys = false;

        // keyboard events
        protected void KBCapture()
        {
            if (kb_func == null) {
                kb_func = (k, x, y) => ProcessKBEvent(k, x ,y);
            }
            KMCapture.kbd_output += kb_func;
        }

        private bool ProcessKBEvent(Keys k, uint x, uint y)
        {
            if (_chk_keys) {
                _user_keys.Add(k);
                evt_kb.Set();
                AmpContainer.SendAll(198);
            }
            return false;
        }
    }
}
