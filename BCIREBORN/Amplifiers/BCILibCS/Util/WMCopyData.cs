using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.Generic;

namespace BCILib.Util
{
	/// <summary>
	/// Summary description for WMCopyData.
	/// </summary>
	public class WMCopyData
	{
		delegate bool EnumWndCallBack(IntPtr hwnd, int lparam);

		[DllImport("User32.dll")]
		static extern bool EnumWindows(EnumWndCallBack cb, int lparam);

		[DllImport("User32.dll")]
		public static extern int GetProp(IntPtr hwnd, string param);

		[DllImport("User32.dll")]
		public static extern int SendMessage(IntPtr hwnd, uint msgID, 
			int wparam, int lparam);

		[DllImport("User32.dll")] 
		public static extern bool SetProp(IntPtr hwnd, string prop, int val);
		[DllImport("User32.dll")] 
		public static extern IntPtr RemoveProp(IntPtr hwnd, string prop);

		public const int WM_COPYDATA = 0x004A; // only for SendMessage
		private readonly string WM_PROPERTY;

		private List<IntPtr> cli_windows = new List<IntPtr>();
		private IntPtr serv_window;

		/// <summary>
		/// WMCopyData: class hold functions to send end client window messages
		/// </summary>
		/// <param name="wm_prop"> a string specify the property name.</param>
		public WMCopyData(string wm_prop, IntPtr swnd) {
			WM_PROPERTY = wm_prop;
			serv_window = swnd;
		}

        /// <summary>
        /// Constructor WMCopyData from known window handler.
        /// </summary>
        /// <param name="cwnd">client window handler</param>
        /// <param name="swnd">server window handler</param>
        public WMCopyData(IntPtr cwnd, IntPtr swnd)
        {
            WM_PROPERTY = null;
            cli_windows.Clear();
            cli_windows.Add(cwnd);
            serv_window = swnd;
        }

		public string Property {
			get {
				return WM_PROPERTY;
			}
		}

		private bool CheckClientWnd(IntPtr hwnd, int lparam) {
			int pv = GetProp(hwnd, WM_PROPERTY);
			if (pv > 0) {
				//Console.WriteLine("Find client window!");
				cli_windows.Add(hwnd);
			}
			return true;
		}

		public int GetAllGUIWindows() {
            return GetAllGUIWindows(true);
        }

        public IntPtr[] CliWnds
        {
            get
            {
                return cli_windows.ToArray();
            }
        }

        public int GetAllGUIWindows(bool DoEnum) {
            if (!string.IsNullOrEmpty(WM_PROPERTY)) {
                if (DoEnum) {
                    cli_windows.Clear();
                    EnumWindows(new EnumWndCallBack(CheckClientWnd), 0);
                    //Console.WriteLine("Clients found: {0}", cli_windows.Count);
                } else {
                    while (cli_windows.Count > 0) {
                        IntPtr clw = cli_windows[0];
                        if (GetProp(clw, WM_PROPERTY) != 0) break;
                        cli_windows.Remove(clw);
                    }
                }
            }

			return cli_windows.Count;
		}

		private Copy_Data ctrData = new Copy_Data();

        public IntPtr CliWnd
        {
            get
            {
                IntPtr clw = IntPtr.Zero;
                if (cli_windows.Count > 0)
                {
                    clw = cli_windows[0];
                    if (!string.IsNullOrEmpty(WM_PROPERTY)) {
                        while (GetProp(clw, WM_PROPERTY) == 0) {
                            cli_windows.Remove(clw);
                            if (cli_windows.Count < 1) {
                                clw = IntPtr.Zero;
                                break;
                            }
                            clw = (IntPtr)cli_windows[0];
                        }
                    }
                }
                if (clw == IntPtr.Zero)
                {
                    GetAllGUIWindows();
                    if (cli_windows.Count > 0) clw = (IntPtr)cli_windows[0];
                }

                return clw;
            }
        }

        public bool SendClient(int cmd)
        {
            return SendClient(cmd, (int[])null);
        }

        public bool SendClient(int cmd, params byte[] args)
        {
            IntPtr clw = CliWnd;
            if (clw == IntPtr.Zero) return false;

            // construct message sent to clients
            ctrData.wdata = ((uint)cmd & 0xff) | (DATATYPE_BYTE << 16);
            ctrData.sz = 0;
            ctrData.pdata = IntPtr.Zero;

            GCHandle gch_args;
            if (args != null && args.Length > 0) {
                gch_args = GCHandle.Alloc(args, GCHandleType.Pinned);
                ctrData.sz = args.Length;
                ctrData.pdata = gch_args.AddrOfPinnedObject();
            }
            else {
                gch_args = GCHandle.FromIntPtr(IntPtr.Zero);
            }

            GCHandle gch_data = GCHandle.Alloc(ctrData, GCHandleType.Pinned);

            bool rst = true;
            try {
                SendMessage(clw, WM_COPYDATA, serv_window.ToInt32(),
                    gch_data.AddrOfPinnedObject().ToInt32());
            }
            catch (Exception e) {
                Console.WriteLine("Exception = {0}", e);
                rst = false;
            }

            gch_args.Free();
            gch_data.Free();

            return rst;
        }

		public bool SendClient(int cmd, params Int32[] args) {
            IntPtr clw = CliWnd;
            if (clw == IntPtr.Zero) return false;

			// construct message sent to clients
			ctrData.wdata = ((uint) cmd & 0xff) | (DATATYPE_INT << 16);
			ctrData.sz = 0;
			ctrData.pdata = IntPtr.Zero;

            GCHandle gch_args;

            if (args == null) args = new int[0];

            gch_args = GCHandle.Alloc(args, GCHandleType.Pinned);
            ctrData.sz = Marshal.SizeOf(typeof(Int32)) * args.Length;
            ctrData.pdata = gch_args.AddrOfPinnedObject();

			GCHandle gch_data = GCHandle.Alloc(ctrData, GCHandleType.Pinned);

			bool rst = true;
			try {
				SendMessage(clw, WM_COPYDATA, serv_window.ToInt32(), 
					gch_data.AddrOfPinnedObject().ToInt32());
			} catch (Exception e) {
				Console.WriteLine("Exception = {0}", e);
				rst = false;
			}

            gch_args.Free();
			gch_data.Free();

			return rst;
		}

		private const uint DATATYPE_INT = 1;
		private const uint DATATYPE_DOUBLE = 2;
		private const uint DATATYPE_STRING = 3;
        private const uint DATATYPE_BYTE = 4;

        private MemoryStream _ms = null;
        private BinaryWriter _bw = null;

        private BinaryWriter ParWriter
        {
            get
            {
                if (_bw == null) {
                    _ms = new MemoryStream();
                    _bw = new BinaryWriter(_ms);
                }
                return _bw;
            }
        }

        public void Add(Int32 iv) {
            ParWriter.Write(iv);
        }
        public void Add(double dv)
        {
            ParWriter.Write(dv);
        }

        internal void ResetPar()
        {
            if (_bw != null) {
                _bw.Close();
                _bw = null;
                _ms = null;
            }
        }

        public bool SendPar(int cmd)
        {
            bool rst = SendClient(cmd, _ms.GetBuffer());
            ResetPar();
            return rst;
        }

		// variables length of double values
		public bool SendClient(int cmd, params double[] vals) {
            IntPtr clw = CliWnd;
            if (clw == IntPtr.Zero) return false;

			// construct message sent to clients
			ctrData.wdata = (uint) cmd & 0xff;
			ctrData.sz = 0;
			ctrData.pdata = IntPtr.Zero;

			bool rst = true;
			int nv = vals.Length;
			if (nv == 0) {
				GCHandle gch_data = GCHandle.Alloc(ctrData, GCHandleType.Pinned);
				try {
					SendMessage(clw, WM_COPYDATA, serv_window.ToInt32(), 
						gch_data.AddrOfPinnedObject().ToInt32());
				} catch (Exception e) {
					Console.WriteLine("Exception = {0}", e);
					rst = false;
				}
				gch_data.Free();
			} else {
				ctrData.wdata |= DATATYPE_DOUBLE << 16;
				GCHandle gch_f = GCHandle.Alloc(vals, GCHandleType.Pinned);
				ctrData.sz = Marshal.SizeOf(typeof(double)) * nv;
				ctrData.pdata = gch_f.AddrOfPinnedObject();
				GCHandle gch_data = GCHandle.Alloc(ctrData, GCHandleType.Pinned);

                int rv = 0;
				try {
					rv = SendMessage(clw, WM_COPYDATA, serv_window.ToInt32(), 
						gch_data.AddrOfPinnedObject().ToInt32());
				} catch (Exception e) {
					Console.WriteLine("Exception = {0}", e);
					rst = false;
				}

				gch_f.Free();
				gch_data.Free();
			}

			return rst;
		}

		/// <summary>
		/// Send command with string data
		/// </summary>
		/// <param name="cmd">command</param>
		/// <param name="strdata">string data</param>
		/// <returns>true indicates sucess</returns>
		public bool SendClient(int cmd, string strdata) {
            IntPtr clw = CliWnd;
            if (clw == IntPtr.Zero) return false;

			// construct message sent to clients
			ctrData.wdata = (uint) cmd & 0xffff;

			bool rst = true;
			ctrData.wdata |= DATATYPE_STRING << 16;

			int len = strdata.Length;
			byte[] str_bytes = null;
            str_bytes = System.Text.ASCIIEncoding.ASCII.GetBytes(strdata);

			GCHandle gch_str = GCHandle.Alloc(str_bytes, GCHandleType.Pinned);
			ctrData.sz = Marshal.SizeOf(typeof(byte)) * str_bytes.Length;
			ctrData.pdata = gch_str.AddrOfPinnedObject();
			GCHandle gch_data = GCHandle.Alloc(ctrData, GCHandleType.Pinned);

			try {
				SendMessage(clw, WM_COPYDATA, serv_window.ToInt32(), gch_data.AddrOfPinnedObject().ToInt32());
			} catch (Exception e) {
				Console.WriteLine("Exception = {0}", e);
                // remove the window that are not working
                cli_windows.Remove(clw);
				rst = false;
			}

			gch_str.Free();
			gch_data.Free();

			return rst;
		}

        public bool SendCmdString(string fmt, params object[] args)
        {
            return SendClient(BCILib.App.GameCommand.SendCmdString, string.Format(fmt, args));
        }

		public static ctrl_msg TranslateMessage(System.Windows.Forms.Message m)
		{
            return TranslateMessage((Copy_Data) m.GetLParam(typeof(Copy_Data)));
		}

        public static ctrl_msg TranslateMessage(Copy_Data mdata)
        {
            ctrl_msg cmsg = new ctrl_msg();

            cmsg.cmd = (int)(mdata.wdata & 0xffff);
            cmsg.msg = (int)(mdata.wdata >> 16);
            cmsg.iva = null;
            cmsg.dva = null;

            if (mdata.sz > 0) {
                if (cmsg.msg == DATATYPE_INT) {
                    int nv = mdata.sz / Marshal.SizeOf(typeof(Int32));
                    cmsg.iva = new Int32[nv];
                    Marshal.Copy(mdata.pdata, cmsg.iva, 0, nv);
                    cmsg.msg = cmsg.iva[0];
                } else if (cmsg.msg == DATATYPE_DOUBLE) {
                    int nv = mdata.sz / Marshal.SizeOf(typeof(double));
                    cmsg.dva = new double[nv];
                    Marshal.Copy(mdata.pdata, cmsg.dva, 0, nv);
                } else if (cmsg.msg == DATATYPE_STRING) {
                    byte[] buf = new byte[mdata.sz];
                    Marshal.Copy(mdata.pdata, buf, 0, mdata.sz);
                    int len = mdata.sz;
                    if (buf[len - 1] == 0) len--;
                    cmsg.strdata = System.Text.Encoding.ASCII.GetString(buf, 0, len);
                } else if (cmsg.msg == DATATYPE_BYTE) {
                    // will be the most common use
                    int nv = mdata.sz;
                    cmsg.bva = new byte[nv];
                    Marshal.Copy(mdata.pdata, cmsg.bva, 0, nv);
                }
            }

            return cmsg;
        }

        internal void SetThisWindow(IntPtr Handle)
        {
            serv_window = Handle;
        }
    }

	[StructLayout(LayoutKind.Sequential)]
	public struct Copy_Data {
		public uint wdata;
		public int sz;
		public IntPtr pdata;
	}

	public struct ctrl_msg {
		public int cmd;
		public int msg;
        public byte[] bva;
        public int[] iva;
        public double[] dva;
		public string strdata;
	}
}
