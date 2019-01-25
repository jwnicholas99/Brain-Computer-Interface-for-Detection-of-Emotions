using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Forms;

using BCILib.Util;
using System.Threading;
using System.Diagnostics;

namespace BCILib.App
{
    public partial class ConsoleOutForm : Form
    {
        Action<string> _output_handler = null;
        static ConsoleOutForm _mySingleConsole = null;
        static Thread thd = null;

        private ConsoleOutForm()
        {
            InitializeComponent();

            SetCaputureOut();
        }

        static public void ShowWindow() {
            var cform = ConsoleOutForm._mySingleConsole;
            if (cform == null) return;

            if (cform.InvokeRequired) {
                cform.Invoke((ThreadStart) delegate()
                {
                    ShowWindow();
                });
            } else {
                cform.ShowInTaskbar = true;
                cform.WindowState = FormWindowState.Normal;
            }
        }

        static internal void HideWindow()
        {
            ConsoleOutForm cform = ConsoleOutForm._mySingleConsole;
            if (cform != null) {
                if (cform.InvokeRequired) {
                    cform.Invoke((ThreadStart)delegate()
                    {
                        HideWindow();
                    });
                }
                else {
                    cform.WindowState = FormWindowState.Minimized;
                    cform.ShowInTaskbar = false;
                }
            }
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            textBoxConsoleOut.Clear();
        }

        void Console_OutString(string msg) {
            if (InvokeRequired) {
                BeginInvoke((Action)delegate()
                {
                    Console_OutString(msg);
                });
            }
            else {
                if (textBoxConsoleOut.MaxLength <= textBoxConsoleOut.Text.Length + msg.Length) {
                    string svmsg = textBoxConsoleOut.Text;
                    int len = 0;
                    while (len < textBoxConsoleOut.MaxLength / 2 && len < svmsg.Length && len >= 0) {
                        len = svmsg.IndexOf('\n', len + 1);
                    }
                    if (len > 0)
                        textBoxConsoleOut.Text = svmsg.Substring(len);
                    else
                        textBoxConsoleOut.Clear();
                }

                textBoxConsoleOut.AppendText(msg);
            }
        }

        private void ConsoleOutForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            SetNoCapture();
        }

        private void SetCaputureOut()
        {
            if (_output_handler == null) {
                // make sure handles are created in the calling thread. 
                if (!IsHandleCreated) CreateHandle();

                _output_handler = new Action<string>(Console_OutString);
                ConsoleCapture.AddHandler(_output_handler);
                Console.WriteLine("CaptureOut started.");
            }
        }

        private void SetNoCapture()
        {
            if (_output_handler != null) {
                ConsoleCapture.DelHandle(_output_handler);
            }
            ConsoleCapture.StopCapture();
        }

        private void buttonTest_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Test button clicked.");
        }

        static bool AppClosed = false;
        private void ConsoleOutForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!AppClosed) {
                e.Cancel = true;
                WindowState = FormWindowState.Minimized;
                ShowInTaskbar = false;
            }
            else {
                Console.WriteLine("Really close!");
            }
        }

        private void MyCloseWindow()
        {
            if (InvokeRequired) {
                this.Invoke(new Action(() =>  MyCloseWindow()));
                thd.Join();
                thd = null;
                return;
            }

            //Application.ApplicationExit -= _hclose;
            //_hclose = null;

            AppClosed = true;
            //SetNoCapture();

            try {
                if (IsHandleCreated) {
                    this.Close();
                }
            }
            catch (Exception e) {
                Console.WriteLine("Error " + e.Message);
            }
            _mySingleConsole = null;
        }

        internal static void CloseWindow()
        {
            if (_mySingleConsole != null) {
                _mySingleConsole.MyCloseWindow();
            }
        }

        internal static bool Initialize()
        {
            if (_mySingleConsole == null) {
                thd = new Thread((ThreadStart)(() =>
                {
                    _mySingleConsole = new ConsoleOutForm();

                    Application.Run(_mySingleConsole);
                    Debug.Print("ConsoleOutForm Closed!");
                }));

                thd.IsBackground = true;
                thd.Start();

                while (_mySingleConsole == null) {
                    Thread.Sleep(100);
                }
            }
            return _mySingleConsole != null;
        }
    }
}
