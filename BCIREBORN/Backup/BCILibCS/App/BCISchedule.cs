using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BCILib.App;
using System.IO;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using BCILib.Amp;

namespace BCILib.App
{

    public enum BCITask
    {
        None = 0,
        Training = 1,
        Supervised = 2, // two class
        Rehab = 3, // nostop
    }

    [Serializable]
    public class TaskItem
    {
        public BCITask task;
        public string desc;
        public int id;

        /// <summary>
        /// starting time
        /// </summary>
        public DateTime? ctime;
        /// <summary>
        /// Finish time
        /// </summary>
        public DateTime? ftime;

        public string datapath;

        public TaskItem()
        {
        }

        public TaskItem(BCITask task)
            :this(task, task.ToString())
        {
        }

        public TaskItem(BCITask task, string desc) {
            this.task = task;
            this.desc = desc;
            id = BCISchedule.NewTaskID();
        }

        public TaskItem(TaskItem from)
        {
            Copy(from);
        }

        public override string ToString()
        {
            return desc;
        }

        public void Copy(TaskItem ti)
        {
            id = ti.id;
            task = ti.task;
            desc = ti.desc;
            ctime = ti.ctime;
            ftime = ti.ftime;
            datapath = ti.datapath;
        }

        public virtual string GetDescription()
        {
            if (ctime.HasValue && ftime.HasValue) {
                var dt = ftime.Value - ctime.Value;
                return string.Format("Duration={0:HH\\:mm\\:ss}", DateTime.Today + dt);
            }
            
            return null;
        }

        public static TaskItem NewTask(BCITask task)
        {
            TaskItem tt = null;
            if (task == BCITask.Training) tt = new TrainTask();
            else tt = new RehabTask();
            tt.task = task;
            return tt;
        }
    }

    [Serializable]
    [XmlInclude(typeof(RehabTask))]
    public class TaskSession
    {
        public string name;
        public int id;
        public List<TaskItem> items = new List<TaskItem>();

        public TaskSession()
        {
        }

        public TaskSession(string name)
        {
            this.name = name;
            id = BCISchedule.NewSessionID();
        }

        public override string ToString()
        {
            return name;
        }
    }

    [Serializable]
    public class BCISchedule
    {
        public double version = 1.0;

        public List<TaskSession> sessions = new List<TaskSession>();

        private static BCISchedule singleton = null;

        public static BCISchedule Instance
        {
            get
            {
                if (singleton == null) {
                    // user record file
                    string urn = Path.Combine(BCIApplication.UserPath, "TaskRecords.dat");
                    if (File.Exists(urn)) {
                        try {
                            BinaryFormatter bf = new BinaryFormatter();
                            using (FileStream fs = File.OpenRead(urn)) {
                                singleton = (BCISchedule)bf.Deserialize(fs);
                            }
                        } catch { }
                    }
                }

                if (singleton == null) {
                    string sdir = Path.Combine(BCIApplication.RootPath, "Config");
                    string spath = Path.Combine(sdir, "Schedule.xml");
                    if (File.Exists(spath)) {
                        try {
                            using (StreamReader sr = File.OpenText(spath)) {
                                try {
                                    XmlSerializer x = new XmlSerializer(typeof(BCISchedule));
                                    singleton = (BCISchedule)x.Deserialize(sr);
                                } catch (Exception e) {
                                    MessageBox.Show("Error = " + e.Message);
                                }
                                SaveLocal();
                            }
                        } catch { }
                    }

                    //var ns = new BCISchedule();
                    //if (singleton != null && singleton.version < ns.version) {
                    //    singleton = null;
                    //}

                    //if (singleton == null) {
                    //    singleton = ns;
                    //    singleton.CreateDefSchedure();
                    //}
                }

                if (singleton == null) singleton = new BCISchedule();

                return singleton;
            }
        }

        public static void SaveLocal()
        {
            string urn_new = Path.Combine(BCIApplication.UserPath, "TaskRecords_new.dat");
            try {
                using (FileStream fs = File.Create(urn_new)) {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, singleton);
                }
                string urn = Path.Combine(BCIApplication.UserPath, "TaskRecords.dat");
                if (File.Exists(urn)) File.Delete(urn);
                File.Move(urn_new, urn);
            } catch (Exception ex) {
                MessageBox.Show("Error in serialization: " + ex.Message);
            }
        }

        public static BCISchedule CreateDefSchedure()
        {
            singleton = new BCISchedule();

            singleton.sessions = new List<TaskSession>();

            // training
            var s = new TaskSession("1st Screening Session");
            singleton.sessions.Add(s);
            s.items.Add(new TaskItem(BCITask.Training, "Train 1"));
            s.items.Add(new TaskItem(BCITask.Training, "Train 2"));
            // training

            s = new TaskSession("2nd Screening Session");
            singleton.sessions.Add(s);
            s.items.Add(new TaskItem(BCITask.Training, "Train 1"));
            s.items.Add(new TaskItem(BCITask.Training, "Train 2"));

            // rehab
            string rehab_session = "Rehab Session ";
            for (int i = 0; i < 10; i++) {
                s = new TaskSession(rehab_session + (i + 1).ToString());
                singleton.sessions.Add(s);

                s.items.Add(new TaskItem(BCITask.Supervised, "Supervised Testing"));
                s.items.Add(new TaskItem(BCITask.Rehab, "Rehab 1"));
                s.items.Add(new TaskItem(BCITask.Rehab, "Rehab 2"));
            }

            SaveLocal();
            return singleton;
        }

        public static void SaveGlobal()
        {
            if (singleton != null) {
                string sdir = Path.Combine(BCIApplication.RootPath, "Config");
                string spath = Path.Combine(sdir, "Schedule.xml");

                using (StreamWriter sw = File.CreateText(spath)) {
                    try {
                        XmlSerializer x = new XmlSerializer(singleton.GetType());
                        x.Serialize(sw, singleton);
                    } catch (Exception e) {
                        MessageBox.Show("Error = " + e.Message);
                    }
                }
            }
        }

        internal static int NewTaskID()
        {
            return singleton.sessions.SelectMany(x => x.items.Select(y => y.id)).DefaultIfEmpty().Max() + 1;
        }

        internal static int NewSessionID()
        {
            return singleton.sessions.Select(x => x.id).DefaultIfEmpty().Max() + 1;
        }

        public static void SaveUserRecord(TaskItem uit)
        {
            var inst = Instance;

            if (uit.id > 0) {
                // locate user record
                foreach (var ss in inst.sessions) {
                    int idx = ss.items.FindIndex(x => x.id == uit.id);
                    if (idx >= 0) {
                        ss.items[idx] = uit;
                        SaveLocal();
                        return;
                    }
                }
            } else {
                uit.id = BCISchedule.NewTaskID();
            }

            // new task item
            TaskSession ts = null;
            int sno = 0;
            int tno = 0;
            ts = inst.sessions.LastOrDefault(x => x.items.Any(y => y.ctime.HasValue && y.ctime.Value.Date == uit.ctime.Value.Date));

            if (ts == null) ts = inst.sessions.LastOrDefault(s => s.items.Count == 0);
            if (ts == null) {
                ts = new TaskSession("Added Session");
                sno = inst.sessions.FindIndex(s => s.items.Any(x => x.ctime.HasValue && x.ctime > uit.ctime));
                if (sno < 0) sno = inst.sessions.Count;
                inst.sessions.Insert(sno, ts);
            } else {
                tno = ts.items.FindIndex(x => x.ctime > uit.ctime);
                if (tno == -1) tno = ts.items.Count;
            }

            ts.items.Insert(tno, uit);
            SaveLocal();
        }

        internal static void Reset()
        {
            singleton = null;
        }
    }

    [Serializable]
    public class RehabTask : TaskItem
    {
        public int tacc = 0;
        public int trej = 0;
        public int facc = 0;
        public int frej = 0;

        public RehabTask()
        {
            task = BCITask.Rehab;
        }

        public RehabTask(TaskItem ti)
            : base(ti)
        {
        }

        public override string GetDescription()
        {
            int ntotal = tacc + trej + facc + frej;
            if (ntotal > 0) return string.Format("{0},Correct/Total={1}/{2},Acc={3:P}",
                base.GetDescription(), tacc + trej, ntotal, (tacc + trej) / (double) ntotal);
            return null;
        }
    }

    [Serializable]
    public class TrainTask : TaskItem
    {
        public int ntrials = 0;
        public int nclasses = 0;

        public TrainTask()
        {
            task = BCITask.Training;
        }

        public TrainTask(TaskItem ti)
            : base(ti)
        {
        }

        public override string GetDescription()
        {
            return string.Format("{0},ntrials={1}", base.GetDescription(), ntrials);
        }

        public void SetTrialInfo()
        {
            if (string.IsNullOrEmpty(datapath) || !Directory.Exists(datapath)) return;
            string fn = Directory.GetFiles(datapath, "*.cnt").FirstOrDefault();
            if (string.IsNullOrEmpty(fn)) return;

            EEGCntFile cnt = new EEGCntFile();
            cnt.ReadCnt(fn);
            var clist = cnt.CodeList;
            var vlist = Enumerable.Range(0, clist.Count).Select(x => clist[x]).Where(x => x > 120 && x < 130).ToList();
            int nact = vlist.Count();
            int nidl = vlist.Where(x => x == 125).Count();
            nact -= nidl;
            if (nact > 0 && nidl > 0) {
                nclasses = 2;
                ntrials = Math.Min(nact, nidl);
            } else {
                nclasses = 1;
                ntrials = Math.Max(nact, nidl);
            }

            if (string.IsNullOrEmpty(desc)) {
                desc = BCITask.Training.ToString();
                if (nact > 0) {
                    desc += "-" + ((BCILib.MotorImagery.MIAction)(vlist.FirstOrDefault(x => x != 125) - 120)).ToString();
                }
            }

            ftime = ctime + TimeSpan.FromMilliseconds(cnt.NumSamples * 1000.0 / cnt.Amp_Info.sampling_rate);
        }
    }
}
