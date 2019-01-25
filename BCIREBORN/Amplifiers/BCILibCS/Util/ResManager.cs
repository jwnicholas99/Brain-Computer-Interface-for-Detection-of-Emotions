using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;

namespace BCILib.Util
{
	/// <summary>
	/// Summary description for ResManager.
	/// </summary>
	public class ResManager {
		private List<string> _AllResources = new List<string>();
        private List<List<string>> _ResPorps = new List<List<string>>();
		private List<List<string>> _ResVals = new List<List<string>>();

		private List<string> _cfgfiles = new List<string>();

		private bool _Saved = false;
		private bool _Changed = false;

        public bool Changed
        {
            get
            {
                return _Changed;
            }
        }

		public ResManager() {
		}

		public ResManager(string cfg) {
			LoadFile(cfg);
            _Changed = false;
        }

		// Get resource property. If not exists, create a new one.
		private int GetResProperty(string res, bool add) {
			if (!_AllResources.Contains(res)){
				if (add) {
					_AllResources.Add(res);
					_ResPorps.Add(new List<string>());
					_ResVals.Add(new List<string>());
				}
				else return -1;
			}
			return _AllResources.IndexOf(res); 
		}

		private int GetResProperty(string res) {
			return GetResProperty(res, false);
		}

		public void LoadFile(string conf) {
			LoadFile(conf, true);
		}

		public static string[] SplitString(string line) {
			return SplitString(line, null);
		}

		public static string[] SplitString(string line, string sep) {
			if (sep == null || sep.Length == 0) sep = " \t\n";
			char[] seperator = sep.ToCharArray();

			ArrayList al = new ArrayList();
			line = line.Trim();
			while (line != null && line.Length > 0) {
				int s1 = line.IndexOfAny(seperator);
				if (s1 > 0) {
					al.Add(line.Substring(0, s1));
					line = line.Substring(s1 + 1).Trim();
				} else {
					al.Add(line);
					line = null;
				}
			}

			return (string []) al.ToArray(typeof(string));
		}

		static public string[] SplitString(string line, int n) {
			if (n < 0) n = 0;

			char [] seperator = {' ', '\t', '\n'};
			string [] rst = new string[n];
			line = line.Trim();
			int i = 0;
			while (line != null && i < n - 1) {
				int s1 = line.IndexOfAny(seperator);
				if (s1 > 0) {
					rst[i] = line.Substring(0, s1);
					line = line.Substring(s1).Trim();
				} else {
					rst[i] = line;
					line = null;
				}
				i++;
			}

			if (i < n) rst[i++] = line;
			while(i < n) rst[i++] = null;

			return rst;
		}

		public void LoadFile(string conf, bool KeepUpdate) {
			if (conf == null) return;

			if (KeepUpdate && !_cfgfiles.Contains(conf)){
				_cfgfiles.Add(conf);
			}

            conf = Path.GetFullPath(conf);
            if (!File.Exists(conf)) return;

            Stream stream = File.OpenRead(conf);
            Load(stream);
            stream.Close();
        }

        public void Load(Stream stream)
        {
            StreamReader tr = new StreamReader(stream);

            string line = null, res = null;
            while ((line = tr.ReadLine()) != null) {
                line = line.Trim();
                if (line.Length == 0 || line.StartsWith("#")) continue;

                string[] sl = SplitString(line, 2);

                if (string.Compare(sl[0], "Resource", true) == 0) {
                    res = sl[1];
                }
                else {
                    if (res == null && sl[0] == "SaveFile" && sl[1] != null) {
                        if (!_cfgfiles.Contains(sl[1]) && File.Exists(sl[1])) _cfgfiles.Add(sl[1]);
                    }
                    else if (sl != null && sl.Length > 0) {
                        SetConfigValue(res, sl[0], sl[1]);
                    }
                }
            }
            _Changed = false;
        }

		public string GetConfigValue(object resource, object parameter) {
			int prop = GetResProperty(resource.ToString());
			if (prop < 0) return null;

			List<string> pl =  _ResPorps[prop];
			int pind = pl.IndexOf(parameter.ToString());
			if (pind < 0) return null;

			return _ResVals[prop][pind];
		}

        public void GetConfigValue(object parameter, ref int ival)
        {
            GetConfigValue(def_rn, parameter, ref ival);
        }

		public void GetConfigValue(object resource, object parameter, ref int ival) {
			string val = GetConfigValue(resource, parameter);
			if (!string.IsNullOrEmpty(val)) ival = int.Parse(val);
			else {
				SetConfigValue(resource, parameter, ival);
				_Changed = true;
			}
		}

        public void GetConfigValue(object parameter, ref bool bval)
        {
            GetConfigValue(def_rn, parameter, ref bval);
        }

        public void GetConfigValue(object resource, object parameter, ref bool bval)
        {
            string val = GetConfigValue(resource, parameter);
            bool got = false;
            if (!string.IsNullOrEmpty(val)) {
                try {
                    bval = bool.Parse(val);
                    got = true;
                }
                catch (Exception) { }
            }
            if (!got) {
                SetConfigValue(resource, parameter, bval);
                _Changed = true;
            }
        }

		public void GetConfigValue(object resource, object parameter, 
			ref double fval)
		{
			string val = GetConfigValue(resource, parameter);
			if (val != null) fval = double.Parse(val);
			else {
				SetConfigValue(resource, parameter, fval);
				_Changed = true;
			}
		}


        public bool GetConfigValue(object parameter, ref string sval)
        {
            return GetConfigValue(def_rn, parameter, ref sval);
        }

		public bool GetConfigValue(object resource, object parameter, 
			ref string sval)
		{
			string val = GetConfigValue(resource, parameter);
            if (val != null) {
                sval = val;
                return true;
            } else {
                SetConfigValue(resource, parameter, sval);
                _Changed = true;
                return false;
            }
		}

		public void SaveIfChanged() {
			if (_Changed) SaveFile();
		}

        private const string def_rn = "Global";
		public string GetConfigValue(string parameter) {
			return GetConfigValue(def_rn, parameter);
		}

		public void SetConfigValue(object resource, object parameter, object pval) {
            if (resource == null) {
                SetConfigValue(parameter, pval);
            }
			int prop = GetResProperty(resource.ToString(), true);
			List<string> pl = _ResPorps[prop];
			List<string> vl = _ResVals[prop];
			int pind = pl.IndexOf(parameter.ToString());
			if (pind < 0) {
				pl.Add(parameter.ToString());
				vl.Add(pval == null? null:pval.ToString());
			} else {
                vl[pind] = pval == null ? null : pval.ToString();
			}
			_Changed = true;
		}

        public void SetConfigValue(object par, object val)
        {
            SetConfigValue(def_rn, par, val);
        }

		public static char [] StringToCharList(string val) {
			if (val != null) {
				char [] ca = val.ToCharArray();
				ArrayList al = new ArrayList();
				int ci = 0;
				int n = ca.Length;
				while (ci < n) {
					while (ci < n && (char.IsSeparator(ca[ci]) || ca[ci] == ',')) ci++;
					if (ci >= n) continue;
					char c = '\x000';
					if (ca[ci] == '\\') { // special character
						if (++ci >= n) continue;
						if (char.IsDigit(ca[ci])) {
							int c0 = ci++;
							while (ci < n && char.IsDigit(ca[ci])) ci++;
							c = (char)int.Parse(new string(ca, c0, ci - c0));
						} else {
							c = ca[ci++];
						}
					} else {
						c = ca[ci++];
					}
					al.Add(c);
				}

				ca = (char [])al.ToArray(typeof(char));
				return ca;
			}
			return null;
		}

		public string [] GetAllResource() {
			return _AllResources.ToArray();
		}

		public string [] GetAllResProps(string res) {
			int ind = GetResProperty(res);
			if (ind < 0) return null;
			return _ResPorps[ind].ToArray();
		}

		public void SaveFile(string tfn) {
			FileInfo fi = new FileInfo(tfn);
			if (fi.Exists && (fi.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly) {
				fi.Attributes -= FileAttributes.ReadOnly;
			}

            string fn_dir = Path.GetDirectoryName(tfn);
            string fn_tmp = Path.GetFileNameWithoutExtension(tfn);
            string fn_ext = Path.GetExtension(tfn);
            if (string.IsNullOrEmpty(fn_ext)) {
                fn_tmp = "$$$";
            } else {
                var ca = fn_ext.ToCharArray();
                int fl = ca.Length;
                int cl = fl;
                char fc = '$';
                if (fl < 2) {
                    fl = 2;
                } else {
                    cl--;
                    if (ca[cl] == fc) fc = '!';
                }
                char[] cb = new char[fl];
                Array.Copy(ca, cb, cl);
                while (cl < fl) cb[cl++] = fc;
                fn_ext = new string(cb);
            }

            fn_tmp = Path.Combine(fn_dir, fn_tmp + fn_ext);

            if (File.Exists(fn_tmp)) {
                Console.WriteLine("tmp config file already exists: {0}!", fn_tmp);
            }

            DateTime now = DateTime.Now;
            SetConfigValue("Timestamp", now.ToBinary().ToString("X"));
            SetConfigValue("Timestamp-fmt", now.ToString());

			// savefile list
			StreamWriter sw = File.CreateText(fn_tmp);
			tfn = Path.GetFullPath(tfn);
			bool found = false;
			foreach (string svf in _cfgfiles) {
				string af = Path.GetFullPath(svf);
				if (af != tfn) {
                    //sw.WriteLine("SaveFile\t{0}", af);
				} else  {
					found = true;
				}
			}

			if (!found) {
				// change default file name
				if (_cfgfiles.Count > 0) {
					_cfgfiles[0] = tfn;
				} else {
					_cfgfiles.Add(tfn);
				}
			}
		
			{
				string [] pall = GetAllResProps(null);
				if (pall != null) {
					foreach (string prop in pall) {
						sw.WriteLine("{0}\t{1}", prop, GetConfigValue(null, prop));
					}
					sw.WriteLine();
				}
			}

			string [] res_all = GetAllResource();
			if (res_all != null) {
				foreach (string res in res_all){
					if (res == null) continue;

					sw.WriteLine("Resource\t{0}", res);
					string [] pall = GetAllResProps(res);
					if (pall != null) {
						foreach (string prop in pall) {
							sw.WriteLine("{0}\t{1}", prop, GetConfigValue(res, prop));
						}
					}
					sw.WriteLine();
				}
			}
			sw.Close();
			_Saved = true;

			int idx = _cfgfiles.IndexOf(tfn);
			if (idx < 0) {
				_cfgfiles.Insert(0, tfn);
			} else if (idx != 0) {
				_cfgfiles.RemoveAt(idx);
				_cfgfiles.Insert(0, tfn);
			}

			_Changed = false;

            try {
                File.Copy(fn_tmp, tfn, true);
                File.Delete(fn_tmp);
            } catch (Exception ex) {
                Console.WriteLine("Error moving {0} to {1}: {2}", fn_tmp, tfn, ex.Message);
            }
        }

		public void SaveFile() {
			if (_cfgfiles.Count > 0) {
				SaveFile((string) _cfgfiles[0]);
			}
		}

		public bool ShowDialog() {
			ResForm rform = new ResForm(this);
			rform.ShowDialog();
			return _Saved;
		}

		public void UpdateFile(string fn) {
			ResManager rm1 = new ResManager(fn);
			string [] rall = rm1.GetAllResource();
			if (rall == null) return;

			foreach (string res in rall) {
				string [] pall = rm1.GetAllResProps(res);
				if (pall == null) continue;
				foreach (string prop in pall) {
					rm1.SetConfigValue(res, prop, GetConfigValue(res, prop));
				}
			}

			rm1.SaveFile(fn);
		}

		public void UpdateAllFiles() {
			foreach (string fn in _cfgfiles) {
				UpdateFile(fn);
			}
            _Saved = true;
        }

        internal DateTime TimeStamp
        {
            get
            {
                string line = GetConfigValue("Timestamp");
                if (string.IsNullOrEmpty(line)) return DateTime.MinValue;
                return DateTime.FromBinary(long.Parse(line, System.Globalization.NumberStyles.HexNumber));
            }
        }

        internal string GetFileName()
        {
            if (_cfgfiles.Count > 0) return _cfgfiles[0];
            else return null;
        }
    }
}
