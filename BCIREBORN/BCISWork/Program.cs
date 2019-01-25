using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using BCILib.App;

namespace BCIWork
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (!SessionLock()) {
                MessageBox.Show("Already started!");
                return;
            }

            CheckUpdate();

            // start upload agent
            DataUploadAgentForm agent = new DataUploadAgentForm();
            agent.WindowState = FormWindowState.Minimized;
            agent.Show();

            var mform = new BCIAppStartForm();
            FormClosingEventHandler h = (s, e) =>
            {
                if (UploadWorker.TotalFiles > 0 && !agent.CanClose)
                {
                    e.Cancel = true;
                    mform.Hide();
                    agent.CanClose = true;
                    agent.WindowState = FormWindowState.Normal;
                    agent.ShowInTaskbar = true;
                }
            };

            mform.FormClosing += h;

            agent.FormClosed += (s, e) =>
            {
                mform.FormClosing -= h;
                mform.Close();
            };

            Application.Run(mform);

            UploadWorker.Stop();
            agent.Close();
            SessionUnlock();
        }

        private static void CheckUpdate()
        {
            // Chuanchu: check for update
            bool updated = false;
            var rpath = Environment.CurrentDirectory;
            var upd_tool = Path.Combine(rpath, "BCIUpdate.exe");

            if (File.Exists(upd_tool)) {
                var pinf = new System.Diagnostics.ProcessStartInfo(upd_tool, "\"" + rpath + "\"");
                //pinf.WorkingDirectory = rpath;
                var proc = System.Diagnostics.Process.Start(pinf);
                proc.WaitForExit();
                updated = (proc.ExitCode != 0);
                proc.Close();
            }

            if (updated) {
                var pinf = new ProcessStartInfo(Application.ExecutablePath);
                pinf.WorkingDirectory = rpath;
                Process.Start(pinf).Close();
                Environment.Exit(0);
            }
        }

        internal static bool SessionLock()
        {
            string id_path = SessionIDPath;
            if (File.Exists(id_path)) {
                // File locked?
                try {
                    string line = File.ReadAllText(id_path);
                    int pid = int.Parse(line);
                    var proc = Process.GetProcessById(pid);
                    if (proc != null) {
                        return false;
                    }
                } catch (Exception) {
                }
            }

            using (var _flock = File.CreateText(id_path)) {
                _flock.WriteLine(Process.GetCurrentProcess().Id);
            }

            return true;
        }

        internal static void SessionUnlock()
        {
            string id_path = SessionIDPath;
            if (File.Exists(id_path)) {
                // File locked?
                try {
                    string line = File.ReadAllText(id_path);
                    int pid = int.Parse(line);
                    if (pid == Process.GetCurrentProcess().Id) {
                        File.Delete(id_path);
                    }
                } catch (Exception) {
                }
            }
        }

        static private string SessionIDPath
        {
            get
            {
                return Path.Combine(BCIApplication.RootPath, "SessionID.txt");
            }
        }
    }
}
