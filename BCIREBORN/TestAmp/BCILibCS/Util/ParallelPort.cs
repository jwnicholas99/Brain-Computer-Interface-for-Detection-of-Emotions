using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Reflection;
using BCILib.App;

namespace BCILib.Util
{
    class ParallelPort
    {
        static private MethodInfo _portOut_Addr = null;
        //static private MethodInfo _PortIn_Addr = null;

        private static void InitOut32()
        {
            if (_portOut_Addr == null) {
                try {
                    // try to locate dll
                    string fpath = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath,
                        "inpout32.dll");
                    Assembly asb = Assembly.LoadFrom(fpath);

                    Type io_type = asb.GetType("BCILib.Util.InpOut32");
                    if (io_type != null) {
                        var tint = typeof(int);
                        _portOut_Addr = io_type.GetMethod("Out", new Type[] { tint, tint });
                        //_PortIn_Addr = io_type.GetMethod("Inp", new[] { tint });
                        _portOut_Addr.Invoke(null, new object[] {PortAddr, 0});
                    }
                }
                catch (Exception e) {
                    Console.WriteLine(e.Message);
                }
            }
        }

        internal static void Out(int evt)
        {
            Out(PortAddr, evt);
        }

        internal static void Out(int addr, int evt)
        {
            if (_portOut_Addr == null) InitOut32();
            if (_portOut_Addr == null) {
                Console.WriteLine("Error ParallePort.Out: Method not found!");
                return;
            }

            if (evt != 0) {
                //int iv = (int) _PortIn_Addr.Invoke(null, new object[] { addr });
                if (last_code == evt) {
                    _portOut_Addr.Invoke(null, new object[] { addr, 0 });
                    int ts = BCIApplication.ElaspedMilliSeconds;
                    int wt = 0;
                    while (wt < 10) {
                        for (int j = 0; j < 100; j++) ;
                        wt = BCIApplication.ElaspedMilliSeconds - ts;
                    }
                }
            }

            _portOut_Addr.Invoke(null, new object[] { addr, evt });
            last_code = evt;
        }

        public const int DEFAULT_PORTADDR = 0x378;
        public static int PortAddr = DEFAULT_PORTADDR;
        public static int last_code = 0;
    }
}
