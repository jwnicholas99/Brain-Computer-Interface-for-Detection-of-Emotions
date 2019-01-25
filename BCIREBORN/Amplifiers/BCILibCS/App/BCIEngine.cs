using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;
using System.Reflection;
using BCILib.Util;

namespace BCILib.App
{
    /// <summary>
    /// Abstract class for BCI Engine.
    /// Can be implemented in C++ or CSharp
    /// </summary>
    public abstract class BCIEngine
    {
		public abstract void SetFeedbackHandler(Delegate handler);

        public enum BCIProcType
        {
            None = 0,
            Concentration = 1,
            MotorImagery = 2,
            ERD = 21,
            P300 = 3,
        };

        private static Assembly _asb_engine = null;

        public static Assembly ASB_BCIProcEngine
        {
            get
            {
                if (_asb_engine == null) {
                    try {
                        // try to locate dll
                        string fpath = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath,
                            "BCIProcEngine.dll");
                        _asb_engine = Assembly.LoadFrom(fpath);
                    }
                    catch (Exception) {
                        //Console.WriteLine(e.Message);
                    }
                }
                return _asb_engine;
            }
        }

        static MethodInfo rseth = null;
        public static BCIEngine CreateEngine(BCIProcType bptype)
        {
            BCIEngine proc = null;

            // Find BCIProcEngine.dll
            // future implementation: find in plugin directory
            Assembly asm = ASB_BCIProcEngine;
            if (asm != null) {
                Type[] types = asm.GetTypes();
                foreach (Type ptype in types) {
                    if (ptype.IsSubclassOf(typeof(BCIEngine)) && !ptype.IsAbstract) {
                        ConstructorInfo cinf = ptype.GetConstructor(new Type[1] {typeof(BCIProcType)});
                        if (cinf != null) {
                            try {
                                proc = (BCIEngine)cinf.Invoke(new object[] { bptype });
                            }
                            catch (Exception) {
                            }
                            if (proc != null) {
                                MethodInfo seth = ptype.GetMethod("SetRedirectConsole");
                                SafeHandle wh = ConsoleCapture.WriteHandle;
                                if (wh != null && !wh.IsInvalid && seth != null) {
                                    seth.Invoke(null, new object[] { wh });
                                }

                                if (rseth == null) {
                                    rseth = ptype.GetMethod("ResetRedirectConsole");
                                    if (rseth != null) {
                                        ConsoleCapture.evt_resetconsole += () => rseth.Invoke(null, null);
                                        Console.WriteLine("BCIEngine.cs: get redierctConsole method.");
                                    }
                                }

                                return proc;
                            }
                        }
                    }
                }
            }

            return proc;
        }

        public abstract int NumSampleUsed
        {
            get;
        }

        public abstract int NumChannelUsed
        {
            get;
        }

        public abstract IntPtr Processor { get;}

		public abstract void ProcEEGBuf (float[] buf, int nch, int nspl);
		public abstract bool Initialize(Hashtable parameters);
        public abstract bool Initialize(string mdl_fn);
        public abstract void Free();
    }
}
