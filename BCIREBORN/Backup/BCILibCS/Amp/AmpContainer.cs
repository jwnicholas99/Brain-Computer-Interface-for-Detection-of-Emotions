using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using BCILib.App;
using BCILib.Util;
namespace BCILib.Amp
{
    public partial class AmpContainer : Form
    {
        private ArrayList amplist = new ArrayList();

        private static AmpContainer container = null;

        public static AmpContainer GlobalContainer
        {
            get
            {
                if (container == null) {
                    container = new AmpContainer();
                }

                return container;
            }
        }

        /// <summary>
        /// AmpContainer - contains all amplifers.
        /// Cannot reference in design mode!
        /// </summary>
        private AmpContainer()
        {
            if (container != null) {
                throw new Exception("Container already created!");
            }
            InitializeComponent();
        }

        private void PreStartAmplifier()
        {
            ClearAllAmplifiers();
            if (!DesignMode) {
                ResManager rm = BCIApplication.AppResource;
                string line = rm.GetConfigValue("InstalledDevices");
                if (line != null) {
                    string[] list = line.Split(',');
                    foreach (string ampn in list) {
                        if (!string.IsNullOrEmpty(ampn))
                            AddAmplifier(Amplifier.FromConfigure(rm, ampn.Trim()));
                    }
                }
                AmplifierLayout();
            }
        }

        private void AmplifierLayout()
        {
            int n = amplist.Count / 2;
            if (n <= 0) return;

            Rectangle rc = ClientRectangle;
            rc.Y += toolStrip1.Height;
            rc.Height -= toolStrip1.Height + statusStrip1.Height;
            int dy = rc.Height / n;
            int y0 = rc.Y;
            for (int na = 0; na < n; na++) {
                AmpViewer av = (AmpViewer) amplist[2 * na + 1];
                av.Location = new Point(0, y0);
                av.Width = rc.Width;
                av.Height = dy;
                y0 += dy;
            }
        }

        private void AddAmplifier(Amplifier amp)
        {
            if (amp != null) {
                amplist.Add(amp);
                AmpViewer av = new AmpViewer();
                av.SetAmplifier(amp);
                this.Controls.Add(av);
                av.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
                amplist.Add(av);
            }
        }

        internal void ClearAllAmplifiers()
        {
            int n = 0;
            while (n < amplist.Count) {
                try {
                    Amplifier amp = (Amplifier)amplist[n++];
                    amp.Stop();
                    AmpViewer av = (AmpViewer)amplist[n++];
                    av.SetAmplifier(null);
                    this.Controls.Remove(av);
                    av.Dispose();
                }
                catch (Exception ex) {
                    BCIApplication.AddAppLog(ex.ToString());
                }
            }
            amplist.Clear();
            //Close();
            //container = null;
        }

        private void toolStripConfig_Click(object sender, EventArgs e)
        {
            ResManager rm = BCIApplication.AppResource;
            if (rm.ShowDialog()) {
                // Changes have been made.
            }
        }

        private void AmpContainer_Resize(object sender, EventArgs e)
        {
            AmplifierLayout();
        }

        private void toolStripTestStim_Click(object sender, EventArgs e)
        {
            if (toolStripTestStim.Checked) {
                toolStripStatus.Text = "TestStim is off.";
                toolStripTestStim.Checked = false;
            }
            else {
                toolStripStatus.Text = "TestStim is on. Press space bar to send stim.";
                toolStripTestStim.Checked = true;
            }
        }

        private void AmpContainer_KeyDown(object sender, KeyEventArgs e)
        {
            if (toolStripTestStim.Checked) {
                for (int n = 0; n < amplist.Count; n += 2) {
                    Amplifier amp = (Amplifier)amplist[n];
                    amp.SendStimCode((byte)e.KeyData);
                }
            }
            e.SuppressKeyPress = true;
        }

        public static Amplifier GetAmplifier(int i) {
            var c = GlobalContainer;
            int no = i + i;
            if (no >= c.amplist.Count) return null;
            return (Amplifier)c.amplist[no];
        }

        internal static AmpViewer GetAmpViewer(int i)
        {
            var c = GlobalContainer;
            int no = i + i + 1;
            if (no > c.amplist.Count) return null;
            return (AmpViewer)c.amplist[no];
        }

        public static Amplifier GetAmplifier(string id)
        {
            int n = Count;
            for (int i = 0; i < Count; i++) {
                Amplifier amp = GetAmplifier(i);
                if (amp.ID == id) return amp;
            }
            return null;
        }

        public static int Count
        {
            get
            {
                return GlobalContainer.amplist.Count / 2;
            }
        }

        private void AmpContainer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Application.OpenForms.Count > 1) {
                e.Cancel = true;
                this.Hide();
            } else {
                //StopAll();
            }
        }

        public static void ShowContainer()
        {
            var c = GlobalContainer;
            c.Show();
            c.Activate();
        }

        public static void SendAll(int stim)
        {
            int n = Count;
            for (int i = 0; i < Count; i++) {
               GetAmplifier(i).SendStimCode(stim);
            }
        }

        public static bool StartAll()
        {
            int n = Count;
            if (n <= 0) return false;

            for (int i = 0; i < Count; i++) {
                Amplifier amp = GetAmplifier(i);
                if (!amp.Start()) {
                    MessageBox.Show("Cannot start amplifer " + amp.DevName);
                    return false;
                }
            }
            return true;
        }

        public static bool StopAll()
        {
            int n = Count;
            for (int i = 0; i < Count; i++)
            {
                Amplifier amp = GetAmplifier(i);
                amp.Stop();
            }
            return true;
        }

        public static bool AllAlive
        {
            get
            {
                int n = Count;
                for (int i = 0; i < Count; i++) {
                    Amplifier amp = GetAmplifier(i);
                    if (!amp.IsAlive) {
                        return false;
                    }
                }
                return true;
            }
        }

        private void toolStripViewLog_Click(object sender, EventArgs e)
        {
            string fn = Amplifier.LogFileName;
            if (File.Exists(fn)) {
                System.Diagnostics.Process.Start(fn);
            }
            else {
                MessageBox.Show("Log file not exists: " + fn);
            }
        }

        //private static string _record_fn = null;

        /// <summary>
        /// Start recording EEG datd for all amplifiers
        /// </summary>
        /// <param name="dir">Saving directory</param>
        /// <param name="npref">Name Prefix for the saving files</param>
        public static void StartRecord(string dir, string prefix)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            var c = GlobalContainer;

            c.SetTitle(c.SERVIER_ID + ": " + prefix);

            int n = Count;
            for (int i = 0; i < n; i++) {
                Amplifier amp = GetAmplifier(i);
                amp.Start();
                amp.StartRecord(Path.Combine(dir, string.Format("{0}_{1}.cnt", prefix, amp.ID)));

                //AmpViewer viewer = GetAmpViewer(i);
                //viewer.InitSplStatistics();

                //_record_fn = Path.Combine(dir, prefix);
            }
        }

        private void SetTitle(string p)
        {
            if (InvokeRequired)
            {
                Invoke((Action)delegate()
                {
                    SetTitle(p);
                });
                return;
            }
            else
            {
                this.Text = p;
            }
        }

        public static void StopRecord()
        {
            int n = Count;
            var c = GlobalContainer;
            for (int i = 0; i < Count; i++) {
                Amplifier amp = GetAmplifier(i);
                amp.StopRecord();

                //AmpViewer view = GetAmpViewer(i);
                //if (view != null)
                //{
                //    int[] spls = view.SPR_Statistics;
                //    if (spls != null)
                //    {
                //        StreamWriter sw = new StreamWriter(_record_fn + "_" + amp.ID + "_spl.txt");
                //        int nk = 0;
                //        for (int ki = 0; ki < spls.Length; ki++)
                //        {
                //            if (spls[ki] > 0)
                //            {
                //                sw.WriteLine("{0}={1}", ki, spls[ki]);
                //                nk += spls[ki];
                //            }
                //        }
                //        sw.WriteLine("Total: {0}", nk);
                //        sw.Close();
                //    }

                //}
            }

            c.SetTitle(c.SERVIER_ID);
        }

        private void toolStripRefresh_Click(object sender, EventArgs e)
        {
            PreStartAmplifier();
        }

        private string ServerID = "BCI_AmpContainer";

        internal string SERVIER_ID
        {
            set
            {
                ServerID = value;
            }

            get
            {
                return ServerID;
            }
        }

        private void AmpContainer_Load(object sender, EventArgs e)
        {
            WMCopyData.SetProp(this.Handle, ServerID, 1);
            this.Text = ServerID;

            // default amplifier display mode
            toolStripCBDisplayMode.SelectedIndex = 0;

            Intialize();
        }

        public static event Action AmpChanged = null;

        private void toolStripSelectAmp_Click(object sender, EventArgs e)
        {
            SelectAmplifierForm dlg = new SelectAmplifierForm();
            if (dlg.ShowDialog() == DialogResult.OK) {
                toolStripRefresh.PerformClick();

                if (AmpChanged != null) AmpChanged();
            }
        }

        private void toolStripCBDisplayMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 1; i < amplist.Count; i += 2) {
                AmpViewer av = (AmpViewer)amplist[i];
                av.DisplayMode = toolStripCBDisplayMode.SelectedIndex;
            }
        }

        [STAThread]
        public static void Main(string[] argv)
        {
            var c = GlobalContainer;
            if (argv.Length > 0)
            {
                c.SERVIER_ID = argv[0];
            }

            c.toolStripCBDisplayMode.SelectedIndex = 1;
            Application.Run(c);
            c.ClearAllAmplifiers();
        }

        System.Windows.Forms.Timer timer = null;
        long t_start = 0;
        int t_time = 10; // seconds;

        private void toolRecordSplRate_Click(object sender, EventArgs e)
        {
            toolRecordSplRate.Enabled = false;

            // initialize
            int n = Count;
            for (int ai = 0; ai < n; ai++)
            {
                Amplifier amp = GetAmplifier(ai);
                AmpViewer view = GetAmpViewer(ai);
                if (view != null) view.InitSplStatistics(t_time);
            }

            // set timer
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000; // 1 second
            timer.Tick += new EventHandler(GetSplData);
            t_start = BCIApplication.ElaspedMilliSeconds;
            timer.Start();
        }

        void GetSplData(Object sender, EventArgs e)
        {
            long t1 = BCIApplication.ElaspedMilliSeconds - t_start;
            int etime = (int) (t1 / 1000);
            int rtime = t_time - etime;
            if (rtime > 0) // display remaining time
            {
                toolRecordSplRate.Text = rtime.ToString();
            } else {
                toolRecordSplRate.Text = "RecordSamplingRate";
                Sound.Beep(1000, 1000);

                if (timer != null)
                {
                    timer.Stop();
                    timer = null;
                }

                // save result to clipboard
                StringWriter sw = new StringWriter();
                int n = Count;
                for (int ai = 0; ai < n; ai++)
                {
                    Amplifier amp = GetAmplifier(ai);
                    AmpViewer view = GetAmpViewer(ai);
                    if (view != null)
                    {
                        int[] slist = view.SPR_Statistics;
                        if (slist == null) continue;

                        int sall = 0;
                        int tall = slist.Length;

                        int smin = int.MaxValue;
                        int smax = 0;

                        for (int sr = 0; sr < tall; sr++)
                        {
                            sall += slist[sr];
                            if (smin > slist[sr]) smin = slist[sr];
                            if (smax < slist[sr]) smax = slist[sr];
                        }

                        if (sall == 0) continue;

                        double smean = (double)sall / (double)tall;
                        double svar = 0;
                        for (int sr = 0; sr < tall; sr++)
                        {
                            double dv = slist[sr] - smean;
                            svar += dv * dv;
                        }
                        if (tall > 1) svar /= (tall - 1);
                        svar = Math.Sqrt(svar); // std

                        sw.WriteLine("Testing done: {0}", DateTime.Now);
                        sw.WriteLine("amp:{0}, total = {5}, avg={1}, min = {2}, max = {3}, var = {4}",
                            amp.ID, sall / tall, smin, smax, svar, tall);

                        for (int sr = 0; sr < tall; sr++)
                        {
                            sw.WriteLine("{0}={1}", sr, slist[sr]);
                        }
                    }
                }

                string msg = sw.ToString();
                if (!string.IsNullOrEmpty(msg))
                {
                    Clipboard.SetText(msg);
                }
                else
                {
                    Console.WriteLine("How can!");
                }

                sw.Close();

                string fn = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                fn = Path.Combine(fn, "SamplingRateTest.txt");

                StreamWriter fw = new StreamWriter(fn);
                fw.Write(sw.ToString());
                fw.Close();

                Process proc = Process.Start("notepad.exe", fn);
                //proc.WaitForExit();
                //proc.Close();

                //File.Delete(fn);
                proc.Exited += (EventHandler)delegate(Object snd, EventArgs evt)
                {
                    MessageBox.Show("Exits");
                    proc.Close();
                };

                toolRecordSplRate.Enabled = true;
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WMCopyData.WM_COPYDATA) {
                ctrl_msg msg = WMCopyData.TranslateMessage(m);
                switch (msg.cmd)
                {
                    case GameCommand.StartRecord:
                        string fn = msg.strdata;
                        StartRecord(Path.GetDirectoryName(fn),
                            Path.GetFileNameWithoutExtension(fn));
                        break;
                    case GameCommand.StopRecord:
                        StopRecord();
                        break;
                    case GameCommand.SendStimCode:
                        SendAll(msg.msg);
                        break;
                }
            }

            base.WndProc(ref m);
        }

        internal static void Intialize()
        {
            var c = GlobalContainer;
            if (c.amplist.Count == 0) {
                c.PreStartAmplifier();
            }
        }
    }
}
