using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BCILib.App
{
    public abstract class DataFileManager
    {
        private static DataFileManager instance = null;

        protected DataFileManager()
        {
            if (instance != null) {
                throw new Exception("Instance already created!");
            }
            instance = this;
        }

        public static void AddDataDir(string dpath)
        {
            if (instance != null) instance.AddAgentDataDir(dpath);
        }

        public static void AddDataFile(string dfn)
        {
            if (instance != null) instance.AddAgentDataFile(dfn);
        }

        protected abstract void AddAgentDataDir(string dpath);
        protected abstract void AddAgentDataFile(string dfn);

        internal static void AddEEGFiles(string dpath, string pref)
        {
            foreach (string cnt_fn in Directory.GetFiles(dpath, pref + "*.cnt")) {
                AddDataFile(cnt_fn);
            }
        }

        internal static void AddDataFiles(string mdir, string fn_key)
        {
            foreach (string m_fn in Directory.GetFiles(mdir, fn_key + "*.*")) {
                AddDataFile(m_fn);
            }
        }
    }
}
