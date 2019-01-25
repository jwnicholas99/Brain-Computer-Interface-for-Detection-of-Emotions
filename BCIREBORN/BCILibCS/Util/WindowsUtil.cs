using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace BCILib.Util
{
    public class WindowsUtil
    {
        [Flags]
        public enum EXECUTION_STATE:uint
        {
            ES_None = 0,
            ES_SYSTEM_REQUIRED = 1,
            ES_DISPLAY_REQUIRED = 2,
            ES_CONTINEOUS = 0x80000000
        }

        [DllImport("kernel32.dll")]
        static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE flags);

        const int SPI_GETSCREENSAVEACTIVE = 0x0010;
        const int SPI_SETSCREENSAVEACTIVE = 0x0011;
        [DllImport("user32.dll")]
        static extern int SystemParametersInfo(int uAction, int uParam, ref int lpvParam, int fWini);

        static EXECUTION_STATE _save_state;
        static public void ReqSystemDisplay()
        {
            // display settings in power options
            _save_state = SetThreadExecutionState(EXECUTION_STATE.ES_CONTINEOUS | EXECUTION_STATE.ES_SYSTEM_REQUIRED | EXECUTION_STATE.ES_DISPLAY_REQUIRED);

            int val = 0; // set screensaver into inactive
            SystemParametersInfo(SPI_SETSCREENSAVEACTIVE, 0, ref val, 0);
        }

        static public void RelSystemDisplay()
        {
            SetThreadExecutionState(_save_state);
            _save_state = EXECUTION_STATE.ES_None;
        }

        [DllImport("user32")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("User32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int ProcessId);
    }
}
