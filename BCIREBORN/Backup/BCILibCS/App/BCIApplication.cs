using System;

using System.Threading;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Text;
using System.Collections;

using BCILib.Util;
using System.Diagnostics;
using BCILib.Amp;

namespace BCILib.App
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class BCIApplication {
        public static bool DebugMode = false;

        public static string AppName
        {
            set
            {
                _AppName = value;
            }
            get
            {
                return _AppName;
            }
        }

        public static string UsersRoot
        {
            get
            {
                return Path.Combine(RootPath, "Users_" + AppName);
            }
        }

		//
		// [i,0]: Result configuration file merged from [i,j];
		//
		private string [][] cfg_files = null;
		private static string _AppName = null;

		private static string root_dir = null;

		public static ResManager SysResource {
			get {
				string fn = Path.Combine(UserPath, "Config");
                if (!Directory.Exists(fn)) {
                    Directory.CreateDirectory(fn);
                }
                fn = Path.Combine(fn, "System.cfg");
                if (!File.Exists(fn)) {
                    File.CreateText(fn).Close();
                }
                return new ResManager(fn);
			}
		}

        public static ResManager AppResource
        {
            get
            {
                string fn = RootPath;
                fn = Path.Combine(fn, "Config");
                if (!Directory.Exists(fn)) {
                    Directory.CreateDirectory(fn);
                }
                fn = Path.Combine(fn, "ConfigAll.cfg");
                if (!File.Exists(fn)) {
                    File.CreateText(fn).Close();
                }
                return new ResManager(fn);
			}
        }

		public const string FMT_TIMESTAMP = "yyyyMMdd-HHmmss";

		public static string TimeStamp {
			get {
				return DateTime.Now.ToString(FMT_TIMESTAMP);
			}
		}

		private static int quick_start = -1; // -1 = unset, 0 = false, 1 = true

		public static bool QuickStart {
			set {
				if (quick_start == -1) {
					quick_start = value? 1:0;
				}
			}

			get {
				return quick_start == 1;
			}
		}

        public BCIApplication(string[][] cfg)
        {
            cfg_files = cfg;
        }

		public static bool MoveFiles(string fdir, string tdir) {
			string [] files = Directory.GetFiles(fdir);
			foreach (string fn in files) {
				string to = tdir + "\\" + Path.GetFileName(fn);
				FileInfo fo = new FileInfo(fn);
				try {
					FileInfo fi = new FileInfo(to);
					if (fi.Exists) {
						if ((fi.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
							fi.Attributes -= FileAttributes.ReadOnly;
						fi.Refresh();
						fi.Delete();
					}
					fo.MoveTo(to);
				} catch (Exception e) {
					Console.WriteLine("Move {0} to {1}: Exception = {2}", fn, to, e.Message);
					return false;
				}
			} 
			return true;
		}

		public static bool CopyFiles(string fdir, string tdir) {
			string [] files = Directory.GetFiles(fdir);
			foreach (string fn in files) {
				if (!Directory.Exists(tdir)) Directory.CreateDirectory(tdir);

				string to = tdir + "\\" + Path.GetFileName(fn);
				FileInfo fo = new FileInfo(fn);
				try {
					FileInfo fi = new FileInfo(to);
					if (fi.Exists) {
						if ((fi.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
							fi.Attributes -= FileAttributes.ReadOnly;
						fi.Refresh();
						fi.Delete();
					}
					fo.CopyTo(to, true);
				} catch (Exception e) {
					Console.WriteLine("Move {0} to {1}: Exception = {2}", fn, to, e.Message);
					return false;
				}
			} 
			return true;
		}

        public static string AppUser = null;

		public bool SetUserEnvironment(string user) {
			// prepare for application
            string rpath = RootPath;
			string sub_dir = Path.Combine(UsersRoot, user);

            user_path = sub_dir;
            AppUser = user;

			string sub_cfg_dir = Path.Combine(sub_dir, "Config");

			// check all necessary configuration files
			if (cfg_files != null) {
				if (!Directory.Exists(sub_cfg_dir)) Directory.CreateDirectory(sub_cfg_dir);

				for (int i = 0; i < cfg_files.Length; i++) {
					string tf = sub_cfg_dir + "\\" + cfg_files[i][0];
					if (!File.Exists(tf)) {
						if (tf.EndsWith(".cfg")) 
						{
							ResManager rm = new ResManager();
							for (int j = 1; j < cfg_files[i].Length; j++) 
							{
								string fn = root_dir + "\\Config\\" + cfg_files[i][j];
								if (File.Exists(fn)) rm.LoadFile(fn, false);
							}

							rm.SaveFile(tf);
						} 
						else 
						{
							StreamWriter sw = File.AppendText(tf);
							for (int j = 1; j < cfg_files[i].Length; j++) 
							{
								string fn = root_dir + "\\Config\\" + cfg_files[i][j];
								if (File.Exists(fn)) 
								{
									StreamReader sr = File.OpenText(fn);
									string line;
									while ((line = sr.ReadLine()) != null) 
										sw.WriteLine(line);
									sr.Close();
								}
							}
							sw.Close();
						}
					}
				}
			}

			Environment.CurrentDirectory = sub_dir;
			return true;
		}

		public static string RootPath {
            //set {
            //    root_dir = value;
            //}

			get {
				if (string.IsNullOrEmpty(root_dir)) {
                    var cfg_fn = "Config\\ConfigAll.cfg";
                    root_dir = Environment.CurrentDirectory;
                    if (!File.Exists(Path.Combine(root_dir, cfg_fn))) {
                        string stdir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                        if (File.Exists(Path.Combine(stdir, cfg_fn))) {
                            root_dir = stdir;
                        }
                    }
				}

				return root_dir;
			}
		}

        static public void AddAppLog(string fmt, params object[] args)
        {
            using (StreamWriter sw = File.AppendText(Path.Combine(RootPath, "BCIApp.log"))) {
                sw.WriteLine(DateTime.Now.ToString("s") + ": " + fmt, args);
            }
        }

        private static string user_path = null;
        public static string UserPath
        {
            get
            {
                if (string.IsNullOrEmpty(user_path)) {
                    ResManager rm = AppResource;
                    string subject = rm.GetConfigValue(AppName, "Subject");
                    //if (!string.IsNullOrEmpty(subject)) {
                    //    bool withDate = false;
                    //    rm.GetConfigValue(AppName, "UserWithDate", ref withDate);
                    //    if (withDate) {
                    //        subject = Path.Combine(subject, rm.GetConfigValue(AppName, "DateString"));
                    //    }
                    //}
                    if (!string.IsNullOrEmpty(subject)) user_path = Path.Combine(UsersRoot, subject);
                }
                return user_path;
            }
        }
        public static string DatePath
        {
            get
            {
                return Path.Combine(UserPath, DateString);
            }
        }

		public static  int[] GenerateSeq(int nc) {
			return GenerateSeq(nc, nc);
		}

		public static  int[] GenerateSeq(int num, int nc) {
			int nr = (num + nc - 1) / nc;
			int[] nsample = new int[num];

			int[] tcols = new int[nc];
			int[] tkeys = new int[nc];

			Random rnd = new Random();

			for (int it = 0; it < nr; it++) {
				for (int ic = 0; ic < nc; ic++) {
					tcols[ic] = ic;
					tkeys[ic] = rnd.Next(1000);
				}

				Array.Sort(tkeys, tcols);

				int nstart = it * nc;
				if (it > 0 && tcols[0] == nsample[nstart - 1]) {
					int mi = tcols[0];
					tcols[0] = tcols[nc - 1];
					tcols[nc - 1] = mi;
				}

				int nlen = num - nstart;
				if (nlen > nc) nlen = nc;
				Array.Copy(tcols, 0, nsample, nstart, nlen);
			}

			return nsample;
		}

		public static string GetRelativePath(string fn, string rpath) {
			if (fn != null) {
				fn = Path.GetFullPath(fn);
				if (fn.StartsWith(rpath)) {
                    if (fn.Length > rpath.Length && fn[rpath.Length] == Path.DirectorySeparatorChar) {
                        fn = fn.Substring(rpath.Length + 1);
                    } else if (fn.Length == rpath.Length) {
                        fn = String.Empty;
                    }
				}
			}
			return fn;
		}

        public static string GetRelativeUserPath(string fn)
        {
            return GetRelativePath(fn, UserPath);
        }
        public static string GetRelativeAppPath(string fn)
        {
            return GetRelativePath(fn, RootPath);
        }

		public static void AddTrainingEEGFile(string cnt_fn) {
			FileInfo finf = new FileInfo(cnt_fn);
			if (!finf.Exists) return;

			cnt_fn = finf.FullName;
			if (cnt_fn.StartsWith(Environment.CurrentDirectory)) {
				cnt_fn = cnt_fn.Substring(Environment.CurrentDirectory.Length + 1);
			}

			finf = new FileInfo(BCICfg.TrainingFileName);
			if (finf.Exists) {
				StreamReader sr = finf.OpenText();
				string line;
				while ((line = sr.ReadLine()) != null) {
					bool flag = true;
					char[] fchars = line.ToCharArray(0,2);
					if (char.IsDigit(fchars[0]) && char.IsWhiteSpace(fchars[1])) {
						flag = (fchars[0] != '0');
						line = line.Substring(2).Trim();
					}

					if (line == cnt_fn) {
						Console.WriteLine("AddTrainingEEGFile: file {0} alread exists.", cnt_fn);
						continue;
					}
				}
				sr.Close();
			}

			StreamWriter sw = finf.AppendText();
			sw.WriteLine("1 {0}", cnt_fn);
			sw.Close();
		}

        public static void AddTrainingPath(string tdir)
        {
            AddTrainingPath(tdir, false);
        }

        public static void AddTrainingPath(string tdir, bool bRecursive)
        {
			string[] cnt_files = Directory.GetFiles(tdir, "*.cnt",
                bRecursive?SearchOption.AllDirectories:SearchOption.TopDirectoryOnly);
			foreach (string fi in cnt_files) {
				AddTrainingEEGFile(fi);
			}
		}

        internal static void AddTrainingPath(string tdir, string[] session_ID)
        {
            AddTrainingPath(tdir, session_ID, false);
        }

        internal static void AddTrainingPath(string tdir, string[] session_ID, bool bRecursive)
        {
            string[] cnt_files = Directory.GetFiles(tdir, "*.cnt", bRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            foreach (string fi in cnt_files) {
                string fn = Path.GetFileName(fi);
                bool match = false;
                foreach (string sid in session_ID) {
                    if (fn.StartsWith(sid)) {
                        match = true;
                        break;
                    }
                }
                if (match) AddTrainingEEGFile(fi);
            }
        }

		public static void SaveTrainingFileLists(ListView listViewTrainFiles) {
			FileInfo finf = new FileInfo(BCICfg.TrainingFileName);
			StreamWriter sw = finf.CreateText();
			int n = listViewTrainFiles.Items.Count;
			for (int fi = 0; fi < listViewTrainFiles.Items.Count; fi++) {
				sw.WriteLine("{0} {1}", listViewTrainFiles.Items[fi].Checked?1:0,
					listViewTrainFiles.Items[fi].Text);
			}
			sw.Close();
		}

        public static void LoadTraingFileLists(ListView listViewTrainFiles)
        {
            LoadTraingFileLists(listViewTrainFiles, true);
        }

		public static void LoadTraingFileLists(ListView listViewTrainFiles, bool checkExist) {
			listViewTrainFiles.Items.Clear();

			FileInfo finf = new FileInfo(BCICfg.TrainingFileName);
			if (finf.Exists) {
				StreamReader sr = finf.OpenText();
				string line;
				while ((line = sr.ReadLine()) != null) {
					bool flag = true;
					char[] fchars = line.ToCharArray(0,2);
					if (char.IsDigit(fchars[0]) && char.IsWhiteSpace(fchars[1])) {
						flag = (fchars[0] != '0');
						line = line.Substring(2).Trim();
					}
					if (!checkExist || File.Exists(line)) {
						ListViewItem item = listViewTrainFiles.Items.Add(line);
                        item.Checked = flag;
                        item.ToolTipText = line;
					}
				}
				sr.Close();
			}
		}

		public static string[] GetAppGames() {
            ResManager _rm = AppResource;
            string line = _rm.GetConfigValue(AppName, "AppGames");
            if (line != null) {
                string[] glist = line.Split(',');
                System.Collections.ArrayList al = new System.Collections.ArrayList(glist.Length);
                foreach (string gname in glist) {
                    string tmp = gname.Trim();
                    if (tmp.Length > 0) al.Add(tmp);
                }
                return (string[])al.ToArray(typeof(string));
            }
            return null;
		}

		public static string GetGamePath(string gname) {
			return GetGamePath(gname, false);
		}

        /// <summary>
        /// Read Game Path from Global definition file.
        /// </summary>
        /// <param name="gname">Game Idenifier</param>
        /// <param name="Find">Try to find a new one</param>
        /// <returns>Game Path</returns>
        public static string GetGamePath(string gname, bool FindNew)
        {
            return GetGamePath(gname, FindNew, FindNew);
        }

        /// <summary>
        /// Read Game Path from Global definition file.
        /// </summary>
        /// <param name="gname">Game Idenifier</param>
        /// <param name="BrowseIfNotFound">If not found, browse for it.</param>
        /// <param name="Find">Try to find a new one</param>
        /// <returns>Game Path</returns>
		public static string GetGamePath(string gname, bool BrowseIfNotFound, bool FindNew) {
			ResManager rm = AppResource;

            string game_path = rm.GetConfigValue(gname, "Game_Path");
            if (game_path != null) game_path = Path.Combine(RootPath, game_path);

            if (game_path != null) {
                if (!File.Exists(game_path)) {
                    game_path = null;
                }
            }

            if (FindNew || (string.IsNullOrEmpty(game_path) && BrowseIfNotFound))
            {
				OpenFileDialog fdlg = new OpenFileDialog();
				fdlg.RestoreDirectory = true;
                string tfn = game_path;
                if (string.IsNullOrEmpty(tfn)) tfn = Path.Combine(RootPath, gname + ".exe");
                fdlg.InitialDirectory = Path.GetDirectoryName(tfn);
                fdlg.FileName = Path.GetFileName(tfn);
                fdlg.Filter = "Exe files(*.exe)|*.exe|All files|*.*";

				if (fdlg.ShowDialog() != DialogResult.OK) {
					Console.WriteLine("Cannot find path for game {0}!", gname);
					game_path = null;
				} else {
					game_path = Path.Combine(RootPath, fdlg.FileName);
					rm.SetConfigValue(gname, "Game_Path", GetRelativePath(game_path, RootPath));
					rm.SaveFile();
				}
			}

			return game_path;
		}

        public static string BrowseForDirectory(string dir)
        {
            return BrowseForDirectory(dir, true);
        }

		public static string BrowseForDirectory(string dir, bool allow_create) {
			if (!Directory.Exists(dir)) dir = @".\";
			FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.ShowNewFolderButton = allow_create;
			dlg.SelectedPath = Path.GetFullPath(dir);
			if (dlg.ShowDialog() != DialogResult.OK) {
				return null;
			}

			dir = dlg.SelectedPath;
			string pwd = Environment.CurrentDirectory;
			if (dir.StartsWith(pwd) && dir != pwd) {
				dir = dir.Substring(pwd.Length + 1);
			}

			return dir;
		}

        public static void SetProtocolName(string name)
        {
            SetProtocolName(SysResource, name);
        }

        public static void SetProtocolName(ResManager res, string name)
        {
            SetProtocolName(res, name, true);
        }

        public static void SetProtocolName(ResManager rm, string name, bool bSaveFile)
        {
            rm.SetConfigValue("EEG", "AppName", name);
            if (bSaveFile) rm.SaveFile();
        }

        static string _DateString  = null;
        public static string DateString
        {
            get
            {
                if (_DateString == null) {
                    _DateString = DateTime.Today.ToString("yyyyMMdd");
                }
                return _DateString;
            }
        }

        public static void LogMessage(string fmt, params object[] arglist)
        {
            LogMessage(Console.Out, fmt, arglist);
        }

        public static void LogMessage(TextWriter ftw, string fmt, params object[] arglist)
        {
            ftw.Write("{0:HH:mm:ss.fff}, ", DateTime.Now);
            ftw.WriteLine(fmt, arglist);
        }

        public static string UserCfgPath
        {
            get
            {
                if (string.IsNullOrEmpty(user_path)) return null;
                return Path.Combine(Path.Combine(user_path, "Config"), "System.cfg");
            }
        }

        /// <summary>
        /// Start external tool program (.exe)
        /// </summary>
        /// <param name="tname">Game ID</param>
        /// <param name="arguments">Command line arguments</param>
        /// <param name="wait">Wait for input idle</param>
        /// <param name="BrowseIfNotFound">Browse for game if not found</param>
        /// <returns></returns>
        public static Process StartExternalProc(string tname, string arguments, bool wait, bool BrowseIfNotFound)
        {
            string tpath = GetGamePath(tname, BrowseIfNotFound, false);
            if (string.IsNullOrEmpty(tpath) || !File.Exists(tpath))
            {
                return null;
            }

            // start image generation tool
            ProcessStartInfo pinf = new ProcessStartInfo();
            pinf.FileName = tpath;
            pinf.Arguments = arguments;
            pinf.WorkingDirectory = Path.GetDirectoryName(tpath);

            pinf.UseShellExecute = false;
            pinf.CreateNoWindow = true;

            try
            {
                Process proc = Process.Start(pinf);
                if (wait) proc.WaitForInputIdle();
                return proc;
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message, "Start " + tpath);
                return null;
            }
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
                return (int) _swatch.ElapsedMilliseconds;
            }
        }

        public static long ElaspedLongMilliSeconds
        {
            get
            {
                if (_swatch == null) _swatch = Stopwatch.StartNew();
                return _swatch.ElapsedMilliseconds;
            }
        }

        public static string DataPath {set; get;}
        public static string[] GetCntFileList(string tdir, bool bRecursive)
        {
            return GetCntFileList(new DirectoryInfo(tdir), bRecursive);
        }

        private static string[] GetCntFileList(DirectoryInfo dinf, bool bRecursive)
        {
            // Get cnt from current directories
            ArrayList cnt_all = new ArrayList();
            FileInfo[] cnt_list = dinf.GetFiles("*.cnt");
            if (cnt_list != null) {
                foreach (FileInfo fi in cnt_list) cnt_all.Add(fi.FullName);
            }

            if (bRecursive) {
                DirectoryInfo[] dl = dinf.GetDirectories();
                foreach (DirectoryInfo di in dl) {
                    cnt_all.AddRange(GetCntFileList(di, true));
                }
            }

            return (string[])cnt_all.ToArray(typeof(string));
        }
    }
}
