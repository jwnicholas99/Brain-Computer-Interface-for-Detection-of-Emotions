using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.IO;
using System.Collections;
using System.Reflection;
using System.Linq;

using BCILib.Util;
using BCILib.Amp;
using System.Diagnostics;

namespace BCILib.App
{
    public partial class BCIAppStartForm : Form
    {
        public BCIAppStartForm()
            :this(null, null, null)
        {
        }

        internal BCIAppStartForm(Type ftype, string title, string app_name)
        {
            InitializeComponent();

            var ar = BCIApplication.AppResource;
            string app_name1 = ar.GetConfigValue("AppName");
            if (string.IsNullOrEmpty(app_name1)) {
                app_name1 = app_name;
                if (string.IsNullOrEmpty(app_name1) && !string.IsNullOrEmpty(title)) {
                    app_name1 = title.Replace(" ", "_");
                }
                if (!string.IsNullOrEmpty(app_name1)) {
                    ar.SetConfigValue("AppName", app_name1);
                    ar.SaveFile();
                }
            }
            BCIApplication.AppName = app_name1;

            if (ftype != null) {
                if (!typeof(Form).IsAssignableFrom(ftype)) {
                    MessageBox.Show("BCIApp Start Error: not a form class!");
                    return;
                }
                buttonFindApp.Visible = buttonCfg.Visible = false;
                toolTip1.SetToolTip(labelApplication, ftype.ToString());

                if (string.IsNullOrEmpty(title)) title = ftype.Name;
            }

            // load user infomation
            LoadContent();

            if (!string.IsNullOrEmpty(title)) {
                labelApplication.Text = title;
                labelApplication.AutoSize = true;
                labelApplication.Left = (this.Width - labelApplication.Width) / 2;
            }
        }

        private void LoadContent()
        {
            string appName = BCIApplication.AppName;
            buttonEdit.Enabled = false;
            buttonStart.Enabled = false;

            ResManager rm = BCIApplication.AppResource;

            // load app definitions
            string appType = toolTip1.GetToolTip(labelApplication);
            if (string.IsNullOrEmpty(appType)) {
                rm.GetConfigValue("TypeName", ref appType);
                toolTip1.SetToolTip(labelApplication, appType);
            }

            if (string.IsNullOrEmpty(appType)) return;

            if (string.IsNullOrEmpty(appName)) {
                int i = appType.LastIndexOf('.');
                if (i >= 0) {
                    appName = appType.Substring(i + 1);
                } else {
                    appName = appType;
                }
                labelApplication.Text = appName;
            }

            BCIApplication.AppName = appName;

            labelApplication.Tag = null;
            string line = null;
            rm.GetConfigValue(appName, "CfgFiles", ref line);
            if (!string.IsNullOrEmpty(line)) {
                string[] cfl = ResManager.SplitString(line, ",");
                if (cfl != null) {
                    int nf = cfl.Length;
                    var al = new List<string[]>();
                    for (int j = 0; j < nf; j++) {
                        line = rm.GetConfigValue(appName, cfl[j]);
                        if (line == null) continue;

                        string[] fl = ResManager.SplitString(line, ",");
                        if (fl != null && fl.Length > 0) {
                            string[] flist = new string[fl.Length + 1];
                            flist[0] = cfl[j];
                            fl.CopyTo(flist, 1);
                            al.Add(flist);
                        }
                    }
                    if (al.Count > 0) {
                        labelApplication.Tag = al.ToArray();
                    }
                }
            }

            // Subject
            comboSubject.Items.Clear();

            string subject = rm.GetConfigValue(appName, "Subject");

            string udir = BCIApplication.UsersRoot;
            if (!Directory.Exists(udir)) {
                Directory.CreateDirectory(udir);
            }
            string[] dirs = Directory.GetDirectories(udir);
            foreach (string dir in dirs) {
                string cdir = Path.GetFileName(dir);
                int uno = comboSubject.Items.Add(cdir);
            }

            comboSubject.Text = subject;

            if (comboSubject.SelectedIndex >= 0) {
                buttonEdit.Enabled = true;
                buttonStart.Enabled = true;
            }

            rm.SaveIfChanged();
        }

        private void buttonCrtSubject_Click(object sender, EventArgs e)
        {
            UserInfoForm fm = new UserInfoForm();
            if (fm.ShowDialog() == DialogResult.OK) {
                LoadContent();
                
                comboSubject.Text = fm.Subject;
                buttonStart.Enabled = true;
            }
        }

        Form appForm = null;

        private void buttonStart_Click(object sender, EventArgs e)
        {
            string subject = null;

            subject = comboSubject.Text;

            ResManager _rm = BCIApplication.AppResource;
            _rm.SetConfigValue(BCIApplication.AppName, "Subject", subject);

            string dir = Path.Combine(BCIApplication.UsersRoot, subject);
            string cfn = Path.Combine(dir, "user.cfg");
            if (File.Exists(cfn)) {
                ResManager rm = new ResManager(cfn);
                rm.SaveFile();
            }

            _rm.SaveFile();

            if (!string.IsNullOrEmpty(subject)) {
                if (!Directory.Exists(dir)) {
                    subject = null;
                }
            }

            if (string.IsNullOrEmpty(subject)) {
                MessageBox.Show("Subject info error!");
                return;
            }

            string tstr = toolTip1.GetToolTip(labelApplication);
            if (string.IsNullOrEmpty(tstr)) {
                MessageBox.Show("Type not defined!");
                return;
            }

            try {
                BCIApplication app = new BCIApplication((string[][])labelApplication.Tag);
                app.SetUserEnvironment(subject);

                if (appForm != null) {
                    appForm.Close();
                    appForm = null;
                }

                Type apptype = this.GetType().Assembly.GetType(tstr);

                if (apptype == null) {
                    apptype = Assembly.GetEntryAssembly().GetType(tstr);
                }
                if (apptype == null || !typeof(Form).IsAssignableFrom(apptype)) {
                    MessageBox.Show("Cannot find type: " + tstr);
                    return;
                }

                //appForm = (Form)apptype.GetConstructor(new Type[] { }).Invoke(null);
                appForm = (Form) Activator.CreateInstance(apptype);

                appForm.FormClosed += (s, args) =>
                {
                    appForm.Dispose();
                    appForm = null;
                    CheckUploadTasks();
                    Show();
                    BringToFront();
                    Select();
                    WindowsUtil.RelSystemDisplay();
                };

                BCISchedule.Reset();
                WindowsUtil.ReqSystemDisplay();
                appForm.Show();
                this.Hide();
            }
            catch (Exception ex) {
                MessageBox.Show("Start application error:" + ex.Message);
            }
        }

        private void comboSubject_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonStart.Enabled = buttonEdit.Enabled = comboSubject.SelectedIndex >= 0;
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            UserInfoForm fm = new UserInfoForm();
            fm.Subject = comboSubject.Text;
            if (fm.ShowDialog() == DialogResult.OK) {
                LoadContent();
                comboSubject.Text = fm.Subject;
            }
        }

        private void buttonFindApp_Click(object sender, EventArgs e)
        {
            FindAppForm dlg = new FindAppForm();
            if (dlg.ShowDialog() == DialogResult.OK) {
                ResManager rm = BCIApplication.AppResource;
                rm.SetConfigValue("TypeName", dlg.Type);
                rm.SaveFile();
                LoadContent();
            }
        }

        private void buttonCfg_Click(object sender, EventArgs e)
        {
            ResManager rm = BCIApplication.AppResource;
            rm.ShowDialog();
        }

        public static void Run<TForm>(string title)
        {
            Application.Run(new BCIAppStartForm(typeof(TForm), title, null));
        }

        public static void Run<TForm>(string title, string app_name)
        {
            Application.Run(new BCIAppStartForm(typeof(TForm), title, app_name));
        }

        public static void Run()
        {
            Application.Run(new BCIAppStartForm());
        }

        public static void CreateUplTask()
        {
            if (!Directory.Exists(BCIApplication.DataPath)) {
                return;
            }
            CreateUplTask(BCIApplication.DataPath);
        }

        public static void CreateUplTask(string data_path)
        {
            data_path = Path.GetFullPath(data_path);
            if (!Directory.Exists(data_path)) return;

            const int upd_chunk_size = 8192;
            string upl_path = Path.Combine(BCIApplication.RootPath, "Upload_TaskPath");
            if (!Directory.Exists(upl_path)) Directory.CreateDirectory(upl_path);

            string src_path = BCIApplication.UsersRoot;
            int ls = src_path.Length + 1;
            var uflist = Directory.GetFiles(data_path, "*", SearchOption.AllDirectories).ToList();
            uflist = uflist.Select(x => x.Substring(ls)).ToList();
            if (uflist == null || uflist.Count <= 0) return;

            var sflist = Directory.GetFiles(upl_path, "upltask_*.txt")
                .SelectMany(x => File.ReadAllLines(x).Select(l => l.Split('|')[1])).Distinct().ToList();
            if (sflist != null && sflist.Count > 0) {
                uflist = uflist.Where(x => sflist.All(s => !string.Equals(s, x, StringComparison.CurrentCultureIgnoreCase))).ToList();
            }
            if (uflist == null || uflist.Count == 0) return;

            string task_fn = Path.Combine(upl_path, "upltask_" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".txt");
            using (StreamWriter sw = File.CreateText(task_fn)) {
                foreach (string fn in uflist) {
                    int fsz = (int)new FileInfo(Path.Combine(src_path, fn)).Length;
                    int pos = 0;
                    if (fsz == 0) {
                        sw.WriteLine("{0:X}|{1}|0|0", DateTime.Now.ToBinary(), fn);
                    } else {
                        while (pos < fsz) {
                            int dsz = fsz - pos;
                            if (dsz > upd_chunk_size) dsz = upd_chunk_size;
                            sw.WriteLine("{0:X}|{1}|{2}|{3}", DateTime.Now.ToBinary(), fn, pos, dsz);
                            pos += dsz;
                        }
                    }
                }
            }
        }

        protected override void OnShown(EventArgs e)
        {
            CheckUploadTasks();
            base.OnShown(e);
        }

        private void CheckUploadTasks()
        {
            string rpath = BCIApplication.RootPath;
            string upl_tool = Path.Combine(rpath, "UploadClient.exe");
            string upl_path = Path.Combine(rpath, "Upload_TaskPath");
            panelUpload.Visible = File.Exists(upl_tool) && Directory.Exists(upl_path) && Directory.GetFiles(upl_path, "upltask_*.txt").Length > 0;
        }

        private void buttonUpload_Click(object sender, EventArgs e)
        {
            string rpath = BCIApplication.RootPath;
            string upl_tool = Path.Combine(rpath, "UploadClient.exe");
            if (File.Exists(upl_tool)) {
                ProcessStartInfo pinf = new ProcessStartInfo(upl_tool, "\"" + rpath + "\"");
                pinf.WorkingDirectory = rpath;
                Process proc = Process.Start(pinf);
                proc.WaitForExit();
                CheckUploadTasks();
            }
        }
    }
}
