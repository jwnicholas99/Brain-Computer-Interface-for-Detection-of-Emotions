using System;
using System.Windows.Forms;
using System.IO;
using BCILib.App;
using System.Runtime.InteropServices;

namespace BCILib.Util
{
    /// <summary>
	/// Summary description for WMHelper.
	/// </summary>
    public class WMHelper
    {
        static DummyWnd _dummyWnd = new DummyWnd();

        static WMCopyData _copyData = null;
        const string ID_BrainpalGameCtrl = "BrainpalGameCtrl";

        static WMHelper()
        {
            _copyData = new WMCopyData(ID_BrainpalGameCtrl, IntPtr.Zero);
        }

        public static void Register(IntPtr hwnd, string prop)
        {
            if (hwnd == IntPtr.Zero)
            {
                throw new InvalidDataException("Window not been initialized.");
            }
            _dummyWnd.Activate(hwnd, prop);

            //_copyData.ServerWindow = hwnd;
        }

        public static void Deregister()
        {
            _dummyWnd.Deactivate();
        }

        public static void SetRecvCmdHandler(Action<ctrl_msg, IntPtr> proc)
        {
            _dummyWnd.OnWMCopyData = proc;
        }
    }

    internal class DummyWnd: NativeWindow
    {
		string _wmprop;

		public void Activate(IntPtr hwnd, string prop) {
			_wmprop = prop;
			WMCopyData.SetProp(hwnd, prop, 1);
            ReleaseHandle();
			AssignHandle(hwnd);
		}

		public Action<ctrl_msg, IntPtr> OnWMCopyData;

		protected override void WndProc(ref Message m) {
			if (m.Msg == WMCopyData.WM_COPYDATA) {
                ctrl_msg msg = WMCopyData.TranslateMessage(m);

				if (OnWMCopyData != null) {
					OnWMCopyData(msg, m.WParam);
                    m.Result = new IntPtr(2);
				}
                else m.Result = new IntPtr(1);
			} else {
				base.WndProc(ref m);
			}
		}

		public void Deactivate() {
            WMCopyData.RemoveProp(this.Handle, _wmprop);
			ReleaseHandle();
		}

        ~DummyWnd()
        {
			Deactivate();
		}
	}
}
