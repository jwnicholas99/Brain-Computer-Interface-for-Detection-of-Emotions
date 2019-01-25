using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace BCILib.Util {
	/// <summary>
	/// Summary description for ConsoleCapture.
	/// </summary>
	public class ConsoleCapture {
		[DllImport("Kernel32.dll")]
        static private extern bool CreatePipe(out SafeFileHandle rp, out SafeFileHandle wp, IntPtr attr, int size);

		[DllImport("Kernel32.dll")]
		static private extern bool CloseHandle(IntPtr p);

        /*
        [DllImport("Kernel32.dll")]
        static private extern int GetLastError();
        */

		[DllImport("Kernel32.dll")]
		static private extern SafeFileHandle GetCurrentProcess();

		[DllImport("Kernel32.dll")]
		static private extern bool DuplicateHandle(
			SafeFileHandle srcProcess, SafeFileHandle srcHandle, SafeFileHandle targetProcess,	ref SafeFileHandle targetHandle,
			int ignore,	bool inherit, int option);

		[DllImport("Kernel32.dll")]
		static private extern IntPtr GetStdHandle(int which); //STD_OUTPUT_HANDLE = -11
		
		private Thread _thdReading = null;
		//private int _svout = -1;

		public event Action<string> OutString;
		private static ConsoleCapture _outcap = null;

        public static event Action evt_resetconsole;
        public static void ResetConsole()
        {
            if (evt_resetconsole != null) {
                evt_resetconsole();
            }
        }

        // Singleton
		private ConsoleCapture () {
		}

		private static ConsoleCapture MyConsole {
            get
            {
                if (_outcap == null) _outcap = new ConsoleCapture();
                return _outcap;
            }
		}

        public static Action<string> AddHandler(Action<string> hdl)
        {
            var c = MyConsole;
            c.OutString += hdl;
            c.StartSingleton();
            return hdl;
        }

        public static void DelHandle(Action<string> hdl)
        {
            MyConsole.OutString -= hdl;
        }

        public class ConsoleFile
        {
            private StreamWriter sw = null;
            Action<string> hdl = null;

            public ConsoleFile(string fn)
            {
                try {
                    string dpath = Path.GetDirectoryName(fn);
                    if (!Directory.Exists(dpath)) Directory.CreateDirectory(dpath);
                    sw = File.CreateText(fn);
                    sw.AutoFlush = true;
                } catch (Exception e) {
                    Console.WriteLine("ConsoleCapture: cannot open {0} for writing logfile: {1}",
                        fn, e.Message);
                    return;
                }

                hdl = AddHandler(s =>
                {
                    if (sw != null) sw.Write(s);
                });
            }

            public void EndLogFile()
            {
                Thread.Sleep(400);
                var c = MyConsole;
                c.OutString -= hdl;

                Thread.Sleep(100);
                if (sw != null) {
                    sw.Close();
                    sw = null;
                }
            }
        }

        public static ConsoleFile StartConsoleFile(string fn)
        {
            return new ConsoleFile(fn);
        }

        private TextWriter _swout = null;
        private TextWriter _swerr = null;

        static SafeFileHandle _cap_rh = null;
        static SafeFileHandle _cap_wh = null;
        static StreamReader _capReader = null;
        static StreamWriter _capWriter = null;

        private void InitCapture()
        {
            if (_cap_wh != null) return;

            bool res = CreatePipe(out _cap_rh, out _cap_wh, IntPtr.Zero, 0);
            if (!res) {
                Console.WriteLine("CreatePipe returned false");
            }
            _capReader = new StreamReader(new FileStream(_cap_rh, FileAccess.Read));
            _capWriter = new StreamWriter(new FileStream(_cap_wh, FileAccess.Write));
            _capWriter.AutoFlush = true;
        }

        public static SafeHandle WriteHandle
        {
            get
            {
                return _cap_wh;
            }
        }

		protected bool StartSingleton() {
			if (_thdReading != null && _thdReading.IsAlive) 
			{
                //Console.WriteLine("Already started!");
				return false;
			}

            InitCapture();
			
			Console.WriteLine("ConsoleCapture: StartCapture called.");

			// set console output to the pipe writer
            //Console.Out.Close();
            _swout = Console.Out;
            _swerr = Console.Error;

			Console.SetOut(_capWriter);
			Console.SetError(_capWriter);

			// start read thread
			_thdReading = new Thread(new ThreadStart(this.ExeRead));
			_thdReading.Start();

			return true;
		}

		protected void StopSingleton() {
            _doReading = false;
            if (evt_resetconsole != null) {
                evt_resetconsole();
            }

            if (_swout != null) {
                Console.SetOut(_swout);
                _swout = null;
            }
            if (_swerr != null) {
                Console.SetError(_swerr);
                _swerr = null;
            }
            if (_capReader != null) {
                try {
                    _capReader.Close();
                } catch { }
            }
            if (_capWriter != null) {
                try {
                    _capWriter.Close();
                } catch { }
            }

            if (_thdReading != null && _thdReading.IsAlive) {

				_thdReading.Join();
				_thdReading = null;

                OutString = null;
                Console.WriteLine("StopCapture called.");
            }

            _capReader = null;
            _capWriter = null;

            if (_cap_wh != null) {
                if (!_cap_wh.IsClosed) {
                    try {
                        CloseHandle(_cap_wh.DangerousGetHandle());
                    } catch { }
                }
                _cap_wh = null;
            }

            if (_cap_rh != null) {
                if (!_cap_rh.IsClosed) {
                    try {
                        CloseHandle(_cap_rh.DangerousGetHandle());
                    } catch { }
                }
                _cap_rh = null;
            }
        }

        bool _doReading = true;

		private void ExeRead() {
			if (_capReader == null) return;

			char [] buf = new char[1024];
			int len = 0;
            _doReading = true;

			while (true) {
                try {
                    len = _capReader.Read(buf, 0, 1024);
                } catch {
                    break;
                }

                if (!_doReading) break;


                if (OutString != null) OutString(new String(buf, 0, len));
			}
		}

        public static void StopCapture()
        {
            MyConsole.StopSingleton();
        }
    }
}
