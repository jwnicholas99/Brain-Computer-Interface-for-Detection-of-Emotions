using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Pipes;
using System.IO;
using System.Runtime.InteropServices;

namespace BCILib.PlugIn
{
    /// <summary>
    /// No need for this plugin: Just copy interop.MLApp.dll in to working directory, the calling program can load MLAppClass, 
    /// creat an instance and call its members.
    /// </summary>
    public class ML_Animation
    {
        static ML_Animation singleton = null;

        static public ML_Animation GetMLApp(string mpath)
        {
            if (singleton == null) singleton = new ML_Animation(mpath);
            else if (!string.IsNullOrEmpty(mpath) && matlab != null) {
                matlab.Execute("cd " + mpath);
            }

            return singleton;
        }

        static public void StartServer(string wpath)
        {
            Type t = Type.GetTypeFromProgID("matlab.Desktop.application");
            Object o = Activator.CreateInstance(t);
            GC.SuppressFinalize(o);

            GetMLApp(wpath);
        }

        public static MLApp.MLAppClass matlab = null;
        private ML_Animation(string mpath)
        {
            if (matlab == null) {
                matlab = new MLApp.MLAppClass();
                //matlab.Visible = 1;
                //matlab.Execute("desktop;");
                //matlab.Execute("open AddContEEGFile.m");
                //matlab.Execute("open ProcessEEGData.m");
            }

            if (!string.IsNullOrEmpty(mpath)) {
                if (!Directory.Exists(mpath)) {
                    Directory.CreateDirectory(mpath);
                }

                matlab.Execute("cd " + mpath);
            }
        }
    }
}
