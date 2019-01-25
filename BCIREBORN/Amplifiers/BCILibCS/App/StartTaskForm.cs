using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BCILib.App;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Xml.Serialization;

namespace BCILib.App
{
    public partial class StartTaskForm : Form
    {
        int cfg_num_trials = 0;
        int cfg_num_supervise = 0;
        int cfg_num_rehab = 0;

        public StartTaskForm()
        {
            InitializeComponent();

            // task type list
            cmbType.Items.AddRange(Enum.GetNames(typeof(BCITask)).Skip(1).ToArray());
            cmbType.SelectedIndex = 0;
        }

        private void RefreshContents()
        {
            lvSchedule.Items.Clear();

            // fill history
            bool changed = false;

            if (BCISchedule.Instance == null) {
                BCISchedule.CreateDefSchedure();
                BCISchedule.SaveGlobal();
            }

            ListViewItem inew = null;
            ListViewItem iold = null;

            var tnums = new[] {
                new {task = BCITask.Training, num = cfg_num_trials},
                new {task = BCITask.Supervised, num = cfg_num_supervise},
                new {task = BCITask.Rehab, num = cfg_num_rehab}};

            foreach (var s in BCISchedule.Instance.sessions) {
                string sname = s.name;
                var tasks = s.items.ToList();
                for (int ti = 0; ti < tasks.Count; ti++) {
                    var t = tasks[ti];
                    if (t.ctime.HasValue && t.task == BCITask.Training && !(t is TrainTask)) {
                        TrainTask tt = new TrainTask(t);
                        tt.SetTrialInfo();
                        var idx = s.items.FindIndex(x => x.id == t.id);
                        s.items[idx] = t = tt;
                        changed = true;
                    }

                    var v = lvSchedule.Items.Add(new ListViewItem(sname));
                    sname = null;
                    v.Tag = t;
                    if (t.ctime.HasValue) {
                        v.SubItems.AddRange(new[] { t.desc, t.ctime.Value.ToString("g") + ": " + t.GetDescription() });
                        iold = v;
                    } else {
                        if (inew == null) inew = v;
                        int nt = tnums.First(x => x.task == t.task).num;
                        if (nt > 0) {
                            v.SubItems.AddRange(new[] { t.desc, "Trials:" + nt });
                        } else {
                            v.SubItems.Add(t.desc);
                        }
                        v.ForeColor = Color.Blue;
                    }
                }
            }

            lvSchedule.HideSelection = false;
            if (inew != null) {
                inew.Selected = true;
                cmbType.Text = (inew.Tag as TaskItem).task.ToString();
                inew.EnsureVisible();
            } else if (iold != null) {
                iold.EnsureVisible();
            }

            if (changed) BCISchedule.SaveLocal();
        }

        internal DialogResult ShowTasks(int ntrain, int nsupervise, int nrehab)
        {
            cfg_num_trials = ntrain;
            cfg_num_supervise = nsupervise;
            cfg_num_rehab = nrehab;
            RefreshContents();
            return ShowDialog();
        }

        public TaskItem SelectedTask()
        {
            TaskItem ti = null;
            BCITask task = (BCITask)Enum.Parse(typeof(BCITask), cmbType.Text);

            var vi = lvSchedule.SelectedItems.Cast<ListViewItem>().FirstOrDefault(x => !(x.Tag as TaskItem).ctime.HasValue);
            if (vi != null) {
                ti = vi.Tag as TaskItem;
                if (ti != null && ti.task == task) return ti;
            }

            ti = new TaskItem(task);
            return ti;
        }

        private void contextScheduleItem_Opening(object sender, CancelEventArgs e)
        {
            var p0 = lvSchedule.PointToClient(Cursor.Position);
            var vi = lvSchedule.GetItemAt(p0.X, p0.Y);
            if (lvSchedule.SelectedItems.Count <= 0 || lvSchedule.SelectedItems[0] != vi) vi = null;

            bool selected = (vi != null);
            toolDelTask.Visible = toolSetTaskPath.Visible = selected;
            if (selected)
            {
                toolDelTask.Text = "Delete(" + vi.Text + "-" + vi.SubItems[1].Text + ")";
            }
        }

        private void toolSetTaskPath_Click(object sender, EventArgs e)
        {
            TaskItem ti = lvSchedule.SelectedItems.Cast<ListViewItem>().Select(x => x.Tag as TaskItem).
                FirstOrDefault();
            if (ti == null)
            {
                MessageBox.Show("Cannot find item from records!");
                return;
            }
            string folder = BCIApplication.BrowseForDirectory(BCIApplication.DatePath, false);
            if (string.IsNullOrEmpty(folder)) return;

            string logfn = Directory.GetFiles(folder, "*.log").FirstOrDefault();
            string cntfn = Directory.GetFiles(folder, "*.cnt").FirstOrDefault();
            if (string.IsNullOrEmpty(logfn) || string.IsNullOrEmpty(cntfn))
            {
                MessageBox.Show("Cannot find log/cnt files!");
                return;
            }

            Regex r = new Regex(@".*_(\d{8}-\d{6}).*.log");
            Match m = r.Match(logfn);
            if (!m.Success)
            {
                MessageBox.Show("log file name error!");
                return;
            }

            if (ti.task == BCITask.Training)
            {
                if (string.IsNullOrEmpty(File.ReadAllLines(logfn).LastOrDefault(x => x.StartsWith("Training session ")))) {
                    if (MessageBox.Show(this, "Training log file wrong or not finished! Continue?", "Set Path",
                        MessageBoxButtons.OKCancel) != DialogResult.OK) {
                        return;
                    }
                }

                // training task
                TrainTask tt = new TrainTask(ti);
                tt.datapath = folder;
                tt.ctime = DateTime.ParseExact(m.Groups[1].Value, BCIApplication.FMT_TIMESTAMP, CultureInfo.InvariantCulture);
                tt.SetTrialInfo();
                BCISchedule.SaveUserRecord(tt);
                RefreshContents();
            }
            else if (ti.task == BCITask.Supervised || ti.task == BCITask.Rehab)
            {
                RehabTask rt = new RehabTask(ti);
                rt.datapath = folder;
                rt.ctime = DateTime.ParseExact(m.Groups[1].Value, BCIApplication.FMT_TIMESTAMP, CultureInfo.InvariantCulture);
                rt.ftime = File.GetLastWriteTime(cntfn);
                using (StreamReader sr = File.OpenText(logfn))
                {
                    string line;
                    int code = 0;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.StartsWith("STIM_TASK:"))
                        {
                            int.TryParse(line.Substring(10), out code);
                        }

                        if (line.StartsWith("Class="))
                        {
                            int cls = new[] { 124, 125 }[line[6] - '0'];
                            if (cls == code)
                            {
                                if (cls == 124) rt.tacc++;
                                else rt.trej++;
                            }
                            else
                            {
                                if (cls == 124) rt.facc++;
                                else rt.frej++;
                            }
                        }
                    }
                }
                BCISchedule.SaveUserRecord(rt);
                RefreshContents();
            }
            return;
        }

        private void toolDelTask_Click(object sender, EventArgs e)
        {
            TaskItem ti = lvSchedule.SelectedItems.Cast<ListViewItem>().Select(x => x.Tag as TaskItem).FirstOrDefault();
            if (ti == null) return;
            var sdx = BCISchedule.Instance.sessions.FindIndex(x => x.items.Any(it => it.id == ti.id));
            if (MessageBox.Show(string.Format("Are you sure to delete selected task({0} - {1})? ",
                BCISchedule.Instance.sessions[sdx].name, ti.desc), "Delete Task Item", MessageBoxButtons.OKCancel)
                == DialogResult.OK)
            {
                BCISchedule.Instance.sessions[sdx].items.Remove(ti);
                if (BCISchedule.Instance.sessions[sdx].items.Count <= 0)
                {
                    BCISchedule.Instance.sessions.RemoveAt(sdx);
                }
                BCISchedule.SaveLocal();
                RefreshContents();
            }
        }

        private void toolSaveXML_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.InitialDirectory = BCIApplication.RootPath;
            dlg.Filter = "XML File(*.xml)|*.xml|All Files(*.*)|*.*";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                BCISchedule to_save = new BCISchedule();
                to_save.sessions = BCISchedule.Instance.sessions.Select((x, sdx) => new TaskSession()
                {
                    id = x.id,
                    name = x.name,
                    items = BCISchedule.Instance.sessions[sdx].items.Select(ti => new TaskItem()
                    {
                        task = ti.task,
                        id = ti.id,
                        desc = ti.desc
                    }).ToList()
                }).ToList();

                using (StreamWriter sw = File.CreateText(dlg.FileName))
                {
                    try
                    {
                        XmlSerializer x = new XmlSerializer(typeof(BCISchedule));
                        x.Serialize(sw, to_save);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error = " + ex.Message);
                    }
                }
            }
        }
    }
}
