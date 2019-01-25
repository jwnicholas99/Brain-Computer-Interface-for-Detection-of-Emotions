using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Globalization;
using System.Threading;
using System.IO;
using System.Web.Services;
using BCIWork.BCIWebUpload;
using System.Diagnostics;
using System.Net.Security;
using System.Xml.Serialization;
using System.Windows.Forms;
using BCILib.App;

namespace BCIWork
{
    /// <summary>
    /// </summary>
    static class UploadWorker
    {
        public static BCIUpload sclient = null;
        static CookieContainer cookies = null;
        static Thread thread = null;
        static bool IsRunning = false;
        const int upd_chunk_size = 8192;
        static int max_workers = 5;

        static RemoteCertificateValidationCallback hcert = null;

        //question: use new one or SRService.sclient?
        static internal bool AppAuthenticate()
        {
            bool rst = false;
            try {
                if (sclient == null) {
                    sclient = new BCIUpload();
                    cookies = new CookieContainer();
                    sclient.CookieContainer = cookies;
                    sclient.Timeout = 10 * 60 * 60 * 1000;

                    if (hcert == null) {
                        hcert = (s, cert, chain, e) => true;
                        ServicePointManager.ServerCertificateValidationCallback += hcert;
                    }
                } else {
                    if (sclient.IsValidAccess()) return true;
                }

                // application authentication
                var sinf = sclient.GetServerDesc();
                var tserver = long.Parse(sinf[0], NumberStyles.AllowHexSpecifier);
                //Console.WriteLine("Server time = {0}", DateTime.FromBinary(tserver));

                long fl = 0;
                byte[] buf = ASCIIEncoding.ASCII.GetBytes(sinf[1]);
                int ipos = 0;
                foreach (byte c in buf) {
                    fl += (((long)c) << ipos);
                    ipos += 8;
                    if (ipos >= 57) ipos %= 57;
                }

                buf = ASCIIEncoding.ASCII.GetBytes(sinf[2]);
                foreach (byte c in buf) {
                    fl += (((long)c) << ipos);
                    ipos += 8;
                    if (ipos >= 57) ipos %= 57;
                }

                long mysecret = tserver ^ (long)fl;
                mysecret += (32876L << 32);

                //var ts1 = (mysecret - 19933520) ^ fl;
                //var stv = DateTime.FromBinary(ts1);

                rst = sclient.AuthenticateApp(mysecret);
                //Console.WriteLine("AuthenticateApp result={0}", rst);

                //Console.WriteLine("Cookies: {0}",
                //    string.Join(", ", cookies.GetCookies(new Uri(sclient.Url)).Cast<Cookie>().Select(c => c.Name + "=" + c.Value).ToArray()));

                rst = sclient.IsValidAccess();
                //Console.WriteLine("IsvalidAcess = {0}", rst);
                //if (!rst) {
                    //Console.WriteLine(MsgStrings.Error_AppAuthenticate);
                //}
            } catch {
                //Console.WriteLine(ex.Message, MsgStrings.Msg_TitleRehab);
            }
            return rst;
        }

        static string local_path = null;
        static string prod_name = null;

        // Uploading file has companion progress file with extention .-up
        public const string EXT_UPL = ".-up";
        public const string EXT_UPT = ".-ut";
        public const string PRE_UPT = "UpdTask-";

        public static event Action<Progress_Record, bool, bool> evt_rpttask;
        public static event Action<int, int, int> evt_rptspeed;

        private static List<Progress_Record> list_normal = new List<Progress_Record>();
        private static List<Progress_Record> list_fast = new List<Progress_Record>();
        private static List<Progress_Record> list_finish = new List<Progress_Record>();

        static UploadWorker() {
            new DataUploadAgent();
        }

        public class Progress_Record {
            public string fpath = null;
            public int fleng = 0;
            public int aleng = 0;
            public int uleng = 0;
            public bool chkcls = false;
            public List<Upload_Task> rem_tasks = new List<Upload_Task>();
            public List<Upload_Task> ass_tasks = new List<Upload_Task>();

            public Progress_Record(string fn, int fl, int al)
            {
                fpath = fn;
                fleng = fl;
                aleng = al;
            }
        }

        public class Upload_Task
        {
            public string fnTask = null;
            public Progress_Record pr = null;
            public int start = 0;
            public int len = 0;
        }

        public static string GetRelativePath(string fn)
        {
            if (fn != null) {
                fn = Path.GetFullPath(fn);
                if (fn.StartsWith(local_path)) {
                    if (fn.Length > local_path.Length && fn[local_path.Length] == Path.DirectorySeparatorChar) {
                        fn = fn.Substring(local_path.Length + 1);
                    } else if (fn.Length == local_path.Length) {
                        fn = String.Empty;
                    }
                }
            }
            return fn;
        }

        private static Stopwatch _swatch = null;
        /// <summary>
        /// Note: system uptime must be less than 25 days because of the limit of integer.
        /// </summary>
        public static int ElaspedMilliSeconds
        {
            get
            {
                if (_swatch == null) _swatch = Stopwatch.StartNew();
                return (int)_swatch.ElapsedMilliseconds;
            }
        }

        internal static void Start(string lpath, string pname)
        {
            local_path = lpath;
            prod_name = pname;

            if (Directory.Exists(local_path)) {
                // find all unfinished uploading files and put into low priority queue
                foreach (var fn_progress in Directory.GetFiles(local_path, "*" + EXT_UPL, SearchOption.AllDirectories)) {
                    string fn_local = fn_progress.Substring(0, fn_progress.Length - EXT_UPL.Length);
                    var args = File.ReadAllLines(fn_progress).Last().Split(',');
                    if (args.Length >= 2) {
                        var finf = new FileInfo(fn_local);
                        var pr = new Progress_Record(
                            GetRelativePath(fn_local), (int)finf.Length, int.Parse(args[1]));
                        pr.chkcls = true;
                        list_normal.Add(pr);
                        pr.uleng = pr.aleng;
                    }
                }

                // find all unfinished tasks and update queue files
                foreach (var fn_task in Directory.GetFiles(local_path, PRE_UPT + "*" + EXT_UPT)) {
                    try {
                        using (var sr = File.OpenText(fn_task)) {
                            Upload_Task pt = new Upload_Task();
                            string fpath = sr.ReadLine();
                            pt.start = int.Parse(sr.ReadLine());
                            pt.len = int.Parse(sr.ReadLine());
                            pt.fnTask = Path.GetFileName(fn_task);

                            var pr = list_normal.FirstOrDefault(pi => pi.fpath.Equals(fpath, StringComparison.CurrentCultureIgnoreCase));
                            if (pr == null) {
                                // should not happen
                                var finf = new FileInfo(Path.Combine(local_path, fpath));
                                pr = new Progress_Record(fpath, (int) finf.Length, (int) finf.Length);
                                pr.uleng = pr.aleng;
                                list_normal.Add(pr);
                                LogError("UploadWorker.Start loading unfinished task - file wrong? {0}", pt.fnTask);
                            }
                            pr.uleng -= pt.len;
                            pt.pr = pr;
                            pr.rem_tasks.Add(pt);
                        }
                    } catch (Exception ex){
                        // just delete it
                        try {
                            File.Delete(fn_task);
                        } catch { }
                        LogError("UploadWorker.Start loading unfinished task - file wrong? {0}={1}", Path.GetFileName(fn_task), ex.Message);
                    }
                }

                if (evt_rpttask != null) {
                    foreach (var pr in list_normal) {
                        evt_rpttask(pr, false, false);
                    }
                }
            }

            thread = new Thread(RunUploading);
            thread.Start();
        }

        internal static void Stop()
        {
            IsRunning = false;
        }

        static List<object> thd_list = new List<object>();
        public static int NumberofWorkers
        {
            get
            {
                return thd_list.Count;
            }
        }

        static private void RunUploading()
        {
            IsRunning = true;
            byte[] buf = new byte[upd_chunk_size];
            int lsent = 0;
            int num_workers = 0;
            int t0 = -1;

            WaitCallback func = delegate(object s)
            {
                var t = DateTime.Now;
                lock (thd_list) {
                    thd_list.Add(t);
                }

                byte[] buf1 = new byte[upd_chunk_size];
                while (IsRunning) {
                    int len = DoOneTask(buf1);
                    if (len == 0) break;
                    lock (buf) {
                        lsent += len;
                    }
                }

                lock (thd_list) {
                    thd_list.Remove(t);
                    num_workers--;
                }
            };

            int ct = 0;
            while (IsRunning) {
                if (t0 < 0) {
                    ct = 0;
                    t0 = ElaspedMilliSeconds;
                }

                int len = DoOneTask(buf);

                if (len > 0) {
                    if (t0 < 0) {
                        t0 = ElaspedMilliSeconds;
                    } else {
                        lock (buf) {
                            lsent += len;
                        }
                        int t1 = ElaspedMilliSeconds;
                        ct = t1 - t0;
                    }
                }

                int sent = list_fast.Select(x => x.uleng).Sum() + list_normal.Select(x => x.uleng).Sum() + list_finish.Select(x => x.fleng).Sum();
                int total = list_fast.Select(x => x.fleng).Sum() + list_normal.Select(x => x.fleng).Sum() + list_finish.Select(x => x.fleng).Sum();

                if (evt_rptspeed != null) {
                    if (total == 0) {
                        evt_rptspeed(0, 0, 0);
                    } else if (ct > 0 && lsent > 0) {
                        int rt = (int) ((total - sent) * (double) ct / lsent /1000);
                        evt_rptspeed((int) (sent * 100.0 / total), (int) (lsent * 1000.0 / ct), rt);
                        if (rt > 200 && num_workers < max_workers) {
                            lock (thd_list) {
                                num_workers++;
                            }
                            ThreadPool.QueueUserWorkItem(func);
                        }
                    }
                }

                if (total == sent) {
                    if (t0 > 0) {
                        lock (buf) {
                            t0 = -1;
                            lsent = 0;
                        }
                    }

                    Thread.Sleep(100);
                }
            }
            agent_settings.Save();
        }

        private static int DoOneTask(byte[] buf)
        {
            Upload_Task pt = null;
            int slen = 0;

            lock (list_fast) {
                pt = FindTask(list_fast);

                if (pt == null) {
                    pt = FindTask(list_normal);
                }
            }

            if (pt == null) return 0;

            var fn_local = Path.Combine(local_path, pt.pr.fpath);
            try {
                AppAuthenticate();
                using (var fs = File.Open(fn_local, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete)) {
                    int rlen = pt.len;
                    fs.Seek(pt.start, SeekOrigin.Begin);
                    while (rlen > 0) {
                        int upl = fs.Read(buf, 0, rlen);
                        if (!sclient.UploadFilePart(prod_name, pt.pr.fpath, pt.start, upl, buf)) {
                            throw new Exception("UploadUserFilePart resulted false!");
                        }
                        rlen -= upl;
                    }
                }

                lock (list_fast) {
                    pt.pr.uleng += pt.len;
                    if (evt_rpttask != null) evt_rpttask(pt.pr, false, false);
                }
                // delete task file
                try {
                    File.Delete(Path.Combine(local_path, pt.fnTask));
                } catch { }

                slen = pt.len;
                lock (list_fast) {
                    pt.pr.ass_tasks.Remove(pt);
                }
            } catch (Exception ex) {
                lock (list_fast) {
                    pt.pr.ass_tasks.Remove(pt);
                    pt.pr.rem_tasks.Add(pt);
                    pt.pr.chkcls = true;
                    LogError("UploadWorker error = {0}", ex.Message);
                }
            }
            return slen;
        }

        private static Upload_Task FindTask(List<Progress_Record> list_task)
        {
            Progress_Record pr = null;
            int sz = 0;
            Upload_Task pt = null;

            List<Progress_Record> elist = new List<Progress_Record>();
            // find item to send
            foreach (var pi in list_task) {
                // update file info
                var fn_local = Path.Combine(local_path, pi.fpath);
                var finf = new FileInfo(fn_local);
                if (!finf.Exists) {
                    foreach (var t in pi.rem_tasks) {
                        try {
                            File.Delete(Path.Combine(local_path, t.fnTask));
                        } catch { }
                    }
                    pi.rem_tasks.Clear();
                    if (pi.ass_tasks.Count <= 0) elist.Add(pi);
                    continue;
                }

                if (pi.chkcls) {
                    // check if file closed?
                    try {
                        File.Open(fn_local, FileMode.Open, FileAccess.Write, FileShare.None).Close();
                        int ol = pi.fleng;
                        finf = new FileInfo(fn_local);
                        pi.chkcls = false;
                        if (evt_rpttask != null) {
                            evt_rpttask(pi, false, false);
                        }
                    } catch {
                    }
                }

                if (finf.Length > pi.fleng) {
                    int ol = pi.fleng;
                    pi.fleng = (int)finf.Length;
                }

                if (pt != null) continue;

                if (pi.rem_tasks.Count > 0) {
                    pt = pi.rem_tasks[0];
                    pi.rem_tasks.RemoveAt(0);
                    pi.ass_tasks.Add(pt);
                    continue;
                } 

                // create new task
                int len = pi.fleng - pi.aleng;
                if (len > sz && sz < upd_chunk_size) {
                    pr = pi;
                    sz = len;
                } else if (len == 0 && !pi.chkcls && pi.rem_tasks.Count <= 0 && pi.ass_tasks.Count <= 0) {
                    elist.Add(pi);
                }
            }

            foreach (var pi in elist) {
                FinishProgress(list_task, pi);
            }
            elist.Clear();

            if (pt == null && sz > 0) {
                pt = new Upload_Task();
                pt.pr = pr;
                pt.start = pr.aleng;
                if (sz > upd_chunk_size) sz = upd_chunk_size;
                pt.len = sz;
                pr.ass_tasks.Add(pt);

                // write file in case cannot finish
                string fn_task = null;
                long ts = DateTime.Now.Ticks;
                do {
                    ts++;
                    pt.fnTask = "UpdTask-" + ts.ToString() + EXT_UPT;
                    fn_task = Path.Combine(local_path, pt.fnTask);
                } while (File.Exists(fn_task));

                using (var sw = File.CreateText(fn_task)) {
                    sw.WriteLine(pt.pr.fpath);
                    sw.WriteLine(pt.start);
                    sw.WriteLine(pt.len);
                }

                pr.aleng += pt.len;
                SaveProgress(pr);
            }

            return pt;
        }

        private static void FinishProgress(List<Progress_Record>q, Progress_Record pi)
        {
            if (!AppAuthenticate()) return;

            var fn_local = Path.Combine(local_path, pi.fpath);
            var finf = new FileInfo(fn_local);
            bool remove = false;
            bool reset = false;
            try {
                AppAuthenticate();
                if (finf.Exists) {
                    if (sclient.CheckFile(prod_name, pi.fpath, (int)finf.Length, finf.LastWriteTime.Ticks, finf.CreationTime.Ticks)) remove = true;
                    else reset = true;
                } else {
                    try {
                        //remove from server
                        sclient.CheckFile(prod_name, pi.fpath, 0, 0, 0);
                    } catch { }
                    remove = true;
                }
            } catch (Exception ex) {
                LogError("UploadWorkder.FinishProgress: error = {0}", ex.Message);
            }

            if (remove) {
                string fn_progress = fn_local + EXT_UPL;
                try {
                    File.Delete(fn_progress);
                } catch { };
                list_finish.Add(pi);
                q.Remove(pi);
                pi.uleng = pi.fleng;

                if (evt_rpttask != null) evt_rpttask(pi, true, false);
            }

            if (reset) {
                LogError("Reset file log: fleng={0}/{1}, uleng={2}", pi.fleng, finf.Length, pi.uleng);
                pi.fleng = (int) finf.Length;
                pi.aleng = 0;
                pi.uleng = 0;
                SaveProgress(pi);
            }
        }

        static string log_fn = null;

        private static void LogError(string fmt, params object[] args)
        {
            if (string.IsNullOrEmpty(log_fn)) log_fn = Path.Combine(local_path, "UploadWorkerErr.log");
            using (var sw = File.AppendText(log_fn))
            {
                sw.Write(DateTime.Now.ToString("s") + ":");
                sw.WriteLine(fmt, args);
            }
        }

        public static void AddRelativeFile(string fpath, bool fast, bool chklen)
        {
            lock (list_fast) {
                if (list_finish.Any(x => x.fpath.Equals(fpath, StringComparison.CurrentCultureIgnoreCase))) {
                    return;
                }

                Progress_Record pr = list_fast.FirstOrDefault(x => x.fpath.Equals(fpath, StringComparison.CurrentCultureIgnoreCase));
                if (pr != null) {
                    // Already in queue
                    if (fast) {
                        if (list_fast[0] != pr) {
                            list_fast.Remove(pr);
                            list_fast.Insert(0, pr);
                        }
                    } else {
                        list_fast.Remove(pr);
                        list_normal.Add(pr);
                    }
                    return;
                }

                for (int i = 0; i < list_normal.Count; i++) {
                    if (fpath.Equals(list_normal[i].fpath, StringComparison.CurrentCultureIgnoreCase)) {
                        pr = list_normal[i];
                        break;
                    }
                }

                if (pr != null) {
                    if (fast) {
                        list_normal.Remove(pr);
                        list_fast.Insert(0, pr);
                    }
                    return;
                }

                string fn_local = Path.Combine(local_path, fpath);
                var finf = new FileInfo(fn_local);
                pr = new Progress_Record(fpath, (int)finf.Length, 0);
                pr.chkcls = chklen;

                SaveProgress(pr);

                if (evt_rpttask != null) evt_rpttask(pr, false, fast);

                if (fast) list_fast.Insert(0, pr);
                else list_normal.Add(pr);
            }
        }

        private static void SaveProgress(Progress_Record ufp)
        {
            string fn_local = Path.Combine(local_path, ufp.fpath);
            var finf = new FileInfo(fn_local);
            string fn_progress = fn_local + EXT_UPL;
            using (var sw = File.AppendText(fn_progress)) {
                sw.WriteLine("{0},{1}", ufp.fleng, ufp.aleng);
            }
        }

        internal static void AddDataDir(string dpath)
        {
            if (!Directory.Exists(dpath)) return;

            foreach (var fn_local in Directory.GetFiles(dpath, "*.*", SearchOption.AllDirectories)) {
                if (fn_local.EndsWith(EXT_UPL)) continue;
                AddRelativeFile(GetRelativePath(fn_local), false, false);
            }
        }

        public static int TotalFiles
        {
            get
            {
                return list_fast.Count + list_normal.Count;
            }
        }

        public class DASettings
        {
            private static string file_path = Path.Combine(Application.CommonAppDataPath, "DataAgentSettings.xml");
            private bool changed = false;
            private DateTime? last_mtime = null;
            public DateTime? LatestSearchTime
            {
                get
                {
                    return last_mtime;
                }

                set
                {
                    last_mtime = value;
                    changed = true;
                }
            }

            internal static DASettings Load()
            {
                DASettings ds = null;
                if (File.Exists(file_path)) {
                    using (FileStream fs = File.OpenRead(file_path)) {
                        XmlSerializer xml = new XmlSerializer(typeof(DASettings));
                        ds = (DASettings)xml.Deserialize(fs);
                    }
                }

                if (ds == null) {
                    ds = new DASettings();
                }

                return ds;
            }

            internal void Save()
            {
                if (changed) {
                    using (FileStream fs = File.Create(file_path)) {
                        XmlSerializer xml = new XmlSerializer(typeof(DASettings));
                        xml.Serialize(fs, this);
                    }
                }
            }
        }

        static DASettings agent_settings = DASettings.Load();

        internal static void SearchNewFiles()
        {
            DateTime? ltime = null;
            List<string> flist = new List<string>();
            foreach (string fn in Directory.GetDirectories(local_path).SelectMany(d => Directory.GetFiles(d, "*", SearchOption.AllDirectories))) {
                var ft = File.GetLastWriteTime(fn);
                if (ltime == null || ft > ltime) ltime = ft;
                if (agent_settings.LatestSearchTime == null || ft > agent_settings.LatestSearchTime) flist.Add(GetRelativePath(fn));
            }
            if (ltime != null) agent_settings.LatestSearchTime = ltime.Value;

            // check with server
            int[] slist = sclient.CheckUploadFiles(prod_name, flist.ToArray());
            flist = slist.Select(i => flist[i]).ToList();

            foreach (string fn in flist) {
                AddRelativeFile(fn, false, true);
            }
        }
    }

    class DataUploadAgent : DataFileManager
    {
        protected override void AddAgentDataDir(string dpath)
        {
            UploadWorker.AddDataDir(dpath);
        }

        protected override void AddAgentDataFile(string dfn)
        {
            UploadWorker.AddRelativeFile(UploadWorker.GetRelativePath(dfn), true, true);
        }
    }
}
