using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BCILib.Util
{
    /// <summary>
    /// Class wrapper for functions to capture keyboard/mouse input
    /// </summary>
    public class KMCapture
    {
        const int WH_KEYBOARD_LL = 13;
        const int WM_KEYDOWN = 0x0100;

        delegate int HookProc(int nCode, int wparam, IntPtr lparam);

        [DllImport("user32")]
        static extern int SetWindowsHookEx(int idHook, HookProc proc, IntPtr hMod, int hThread);
        [DllImport("user32")]
        static extern int CallNextHookEx(int hHook, int idHook, int wparam, IntPtr lparam);

        [DllImport("user32")]
        static extern bool UnhookWindowsHookEx(int hHook);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        static int _hHook = 0;

        public static Func<Keys, uint, uint, bool> kbd_output = null;

        [StructLayout(LayoutKind.Sequential)]
        struct KBDLLHOOKSTRUCT
        {
            public Keys vkCode;
            public uint scanCode;
            public uint flags;
            public uint time;
            public uint dwExtraInfo;
        }

        static HookProc dlg_hook = new HookProc(ProcessKeyboardInput);
        static KMCapture() {
            IntPtr hMod = GetModuleHandle("user32.dll");
            _hHook = SetWindowsHookEx(WH_KEYBOARD_LL, dlg_hook, hMod, 0);
            if (_hHook == 0)
            {
                Console.WriteLine("Error in SetWindowsHookEx: hMod = {0}", hMod);
            }
        }

        static int ProcessKeyboardInput(int nCode, int wmsg, IntPtr lparam)
        {
            bool processed = false;

            if (nCode >= 0)
            {
                if (wmsg == WM_KEYDOWN)
                {
                    KBDLLHOOKSTRUCT kinf = (KBDLLHOOKSTRUCT) Marshal.PtrToStructure(lparam, typeof(KBDLLHOOKSTRUCT));
                    if (kbd_output != null)
                    {
                        processed = kbd_output(kinf.vkCode, kinf.scanCode, kinf.flags);
                    }
                }
            }

            if (processed) return 1;
            else return CallNextHookEx(_hHook, nCode, wmsg, lparam);
        }
    }
}
