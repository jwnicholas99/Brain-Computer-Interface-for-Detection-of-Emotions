using System;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using BCILib.App;
using System.IO;

namespace BCILib.Util
{
	/// <summary>
	/// Summary description for USBStimSender.
	/// </summary>
	public static class USBMOD4
	{
		private const uint FT_OPEN_BY_SERIAL_NUMBER = 1;
		private const uint FT_OPEN_BY_DESCRIPTION = 2;
		private const uint FT_OPEN_BY_LOCATION = 4;

		private const uint FT_LIST_NUMBER_ONLY = 0x80000000;
		private const uint FT_LIST_BY_INDEX	= 0x40000000;
		private const uint FT_LIST_ALL = 0x20000000;

		[DllImport("Ftd2xx.dll")]
		static extern uint FT_ListDevices(ref uint num_dev, uint iv, uint flag);
		[DllImport("Ftd2xx.dll")]
		static extern uint FT_ListDevices(int num_dev, StringBuilder inf, uint flag);
		[DllImport("Ftd2xx.dll")]
		static extern uint FT_ListDevices(int num_dev, ref long loc, uint flag);
		[DllImport("Ftd2xx.dll")]
		static extern uint FT_Open(int deviceNumber, ref IntPtr handle);
		[DllImport("Ftd2xx.dll")]
		static extern uint FT_Close(IntPtr dev);

		[DllImport("Ftd2xx.dll")]
		static extern uint FT_Write(IntPtr dev, ref byte data, uint size, ref uint wlen);

		[DllImport("Ftd2xx.dll")]
		static extern uint  FT_Read(IntPtr dev, ref byte data, uint size, ref uint rlen);

		[DllImport("Ftd2xx.dll")]
		static extern uint FT_SetBitMode(IntPtr dev, byte ucMask, byte ucEnable);

        static IntPtr dev = IntPtr.Zero;
        static int last_code = 0;

		public static void Close() {
            if (dev != IntPtr.Zero) {
                FT_Close(dev);
                dev = IntPtr.Zero;
            }
		}

        static public bool TryFind()
        {
            // try to find dirver file "Ftd2xx.dll"
            var spath = Environment.GetFolderPath(Environment.SpecialFolder.System);
            if (!File.Exists(Path.Combine(spath, "Ftd2xx.dll"))) {
                Console.WriteLine("USBMOD4: driver not installed!");
                return false;
            }

            uint ndev = 0;
            uint rv = FT_ListDevices(ref ndev, 0, FT_LIST_NUMBER_ONLY);
            if (rv == 0) {
                Console.WriteLine("USBStimSender: Number of devices found: {0}", ndev);
            } else {
                Console.WriteLine("FT_ListDevices: error = {0}", rv);
            }

            if (ndev <= 0) return false;

            int dno = 0;
            //for (int di = 0; di < ndev; di++) {
            //StringBuilder sno = new StringBuilder(512);
            //StringBuilder des = new StringBuilder(512);
            //long loc = 0;
            //FT_ListDevices(di, sno, FT_LIST_BY_INDEX | FT_OPEN_BY_SERIAL_NUMBER);
            //FT_ListDevices(di, des, FT_LIST_BY_INDEX | FT_OPEN_BY_DESCRIPTION);
            //FT_ListDevices(di, ref loc, FT_LIST_BY_INDEX | FT_OPEN_BY_LOCATION);
            //Console.WriteLine("Device {0}: serial no = {1}, desc = {2}, loc = {3}.", di, sno, des, loc);
            //    if (des.ToString().IndexOf("USBMOD4") >= 0) {
            //        Console.WriteLine("USBStimSender: select device {0}: {1}", di, des);
            //        dno = di;
            //        break;
            //    }
            //}

            if (dno >= 0) {
                rv = FT_Open(dno, ref dev);
                if (rv == 0) {
                    Console.WriteLine("USBStimSender: Open handler = {0}", dev);
                    FT_SetBitMode(dev, 0xFF, 0x1);
                    byte zero = 0;
                    uint wlen = 0;
                    FT_Write(dev, ref zero, 1, ref wlen);
                } else {
                    Console.WriteLine("FT_Open error={0}", rv);
                    Console.WriteLine("USBStimSender: Cannot open USB port");
                    return false;
                }
                return true;
            }

            return false;
        }

        public static void SendCode(int code)
        {
            if (code != 0) {
                if (last_code == code) {
                    Send(0);
                    int ts = BCIApplication.ElaspedMilliSeconds;
                    int wt = 0;
                    while (wt < 10) {
                        for (int j = 0; j < 100; j++) ;
                        wt = BCIApplication.ElaspedMilliSeconds - ts;
                    }
                }
            }
            Send(code);
            last_code = code;
        }

        private static void Send(int code)
        {
            byte wc = (byte)(code & 0XFF);
            uint wlen = 0;
            uint rv = 0;

            if (dev != IntPtr.Zero) {
                rv = FT_Write(dev, ref wc, 1, ref wlen);
                if (rv != 0) {
                    Console.WriteLine("FT_Write: error={0}", rv);
                    Close();
                }
            }

            if (dev == IntPtr.Zero) {
                TryFind();
                if (dev == IntPtr.Zero) return;

                rv = FT_Write(dev, ref wc, 1, ref wlen);
                if (rv != 0) {
                    Console.WriteLine("FT_Write: error = {0}", rv);
                    Close();
                }
            }
        }
    }
}

