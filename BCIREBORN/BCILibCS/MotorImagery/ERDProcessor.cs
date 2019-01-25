using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BCILib.App;
using System.IO;
using System.Globalization;
using BCILib.Util;
using System.Threading;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

namespace BCILib.MotorImagery
{
    class ERDProcessor : BCIProcessor
    {
        internal ERDProcessor():base(BCIEngine.BCIProcType.ERD)
        {
            take_latest = true;
        }

        protected override void ProcessSelectedData()
        {
            if (output_erd == null) return;

            // proc_engine.ProcEEGBuf(pc_buf, chsel.Length, proc_engine.NumChannelUsed);
            string path_erd_tool = BCIApplication.GetGamePath("ERDTool");
            if (string.IsNullOrEmpty(path_erd_tool)) {
                Console.WriteLine("ERD tool not defined!");
                return;
            }

            path_erd_tool = Path.GetDirectoryName(path_erd_tool);

            string fn_root = Path.Combine(path_erd_tool, "ERD_FB_" + BCIApplication.TimeStamp);
            using (BinaryWriter bw = new BinaryWriter(File.OpenWrite(fn_root + ".tmp"))) {
                foreach (float f in pc_buf) bw.Write(f);
                bw.Close();
            }

            File.Move(fn_root + ".tmp", fn_root + ".raw");

            int wt = 100;
            int sn = 10000 / wt;
            string fn_img = fn_root + ".jpg";
            string fn_ok = fn_root + ".ok";

            bool ok = false;
            for (int i = 0; i < sn; i++) {
                if (File.Exists(fn_ok)) {
                    byte[] imgbuf = null;
                    try {
                        imgbuf = File.ReadAllBytes(fn_img);
                        File.Delete(fn_img);
                        File.Delete(fn_ok);
                    }
                    catch (Exception  ex) {
                        Console.WriteLine("File not ready: {0}, wait ...", ex.Message);
                        Thread.Sleep(50);
                        continue;
                    }
                    try {
                        output_erd(Image.FromStream(new MemoryStream(imgbuf)));
                    }
                    catch (Exception ex2) {
                        Console.WriteLine("Set image error:" + ex2);
                        continue;
                    }

                    ok = true;
                    break;
                }
                else {
                    Thread.Sleep(100);
                }
            }

            if (!ok) {
                Console.WriteLine("!!!!No image found!");
            }
        }

        private string _model_fn = null;

        internal bool Initialize(string mfn)
        {
            StopERDTool();

            if (!File.Exists(mfn)) {
                return false;
            }

            NumChannelUsed = 0;
            NumSampleUsed = 500;

            string line;
            using (StreamReader sr = new StreamReader(mfn)) {
                while ((line = sr.ReadLine()) != null) {
                    string[] wl = line.Split(':');
                    if (wl == null || wl.Length < 2) continue;

                    if (wl[0] == "Sampling_Rate") {
                        int spl_rate = int.Parse(wl[1]);
                        NumSampleUsed = 2 * spl_rate;
                    }
                    else if (wl[0] == "ERD_Filter") {
                        string[] pl = wl[1].Trim().Split(' ');
                        int f0, f1, f_allow;
                        int.TryParse(pl[0], out f0);
                        int.TryParse(pl[1], out f1);
                        int.TryParse(pl[2], out f_allow);

                        line = sr.ReadLine();
                        wl = line.Split(':');
                        int n = 0;
                        int.TryParse(wl[1], out n);

                        double[] B = new double[n];
                        line = sr.ReadLine();
                        pl = line.Trim().Split(' ');
                        for (int i = 0; i < n; i++) {
                            B[i] = NumberConv.HexToDouble(pl[i]);
                        }

                        double[] A = new double[n];
                        line = sr.ReadLine();
                        line = sr.ReadLine();
                        pl = line.Trim().Split(' ');
                        for (int i = 0; i < n; i++) {
                            A[i] = NumberConv.HexToDouble(pl[i]);
                        }
                    }
                    else if (wl[0] == "ERD_Reference") {
                        NumChannelUsed = int.Parse(wl[1]);
                        double[] cref = new double[NumChannelUsed];
                        line = sr.ReadLine();
                        string[] pl = line.Trim().Split(' ');
                        for (int i = 0; i < NumChannelUsed; i++) {
                            cref[i] = NumberConv.HexToDouble(pl[i]);
                        }
                    }
                    else if (wl[0] == "ERD_CLimits") {
                        line = sr.ReadLine();
                        string[] pl = line.Trim().Split(' ');
                        double cmin = NumberConv.HexToDouble(pl[0]);
                        double cmax = NumberConv.HexToDouble(pl[1]);
                    }
                }
            }

            if (NumChannelUsed <= 0) return false;

            _model_fn = mfn;

            return true;
        }

        internal delegate void erd_fb_output(Image img);
        private erd_fb_output output_erd = null;

        public override void SetFeedbackHandler(Delegate handler)
        {
            if (handler is erd_fb_output) {
                output_erd = (erd_fb_output)handler;
            }
            else if (handler == null) {
                output_erd = null;
            }
        }

        public void StopERDTool()
        {
            //if (_tool_proc == null || !_tool_proc.HasExited)
            //{
            //    return;
            //}

            string erd_tool = BCIApplication.GetGamePath("ERDTool");
            if (!string.IsNullOrEmpty(erd_tool))
            {
                File.WriteAllText(Path.Combine(Path.GetDirectoryName(erd_tool), "stop.txt"),
                    DateTime.Now.ToString());
            }

            if (_tool_proc != null) {
                try {
                    _tool_proc.CancelOutputRead();
                    _tool_proc.CancelErrorRead();
                    _tool_proc.Close();
                    _tool_proc = null;
                } catch (Exception) { }
            }
        }

        Process _tool_proc = null;

        public bool StartERDTool()
        {
            if (_tool_proc != null && !_tool_proc.HasExited) {
                return true;
            }

            if (string.IsNullOrEmpty(_model_fn) || !File.Exists(_model_fn)) {
                MessageBox.Show("Model file not set!");
                return false;
            }

            string erd_tool = BCIApplication.GetGamePath("ERDTool");
            if (string.IsNullOrEmpty(erd_tool) || !File.Exists(erd_tool)) {
                System.Windows.Forms.MessageBox.Show("Cannot find erd tool!");
                return false;
            }

            // start image generation tool
            ProcessStartInfo pinf = new ProcessStartInfo();
            pinf.FileName = erd_tool;
            pinf.Arguments = string.Format("\"{0}\" \"\"", Path.GetFullPath(_model_fn));
            pinf.WorkingDirectory = Path.GetDirectoryName(erd_tool);

            pinf.UseShellExecute = false;
            pinf.RedirectStandardOutput = true;
            pinf.RedirectStandardError = true;
            pinf.CreateNoWindow = true;

            _tool_proc = new Process();
            _tool_proc.StartInfo = pinf;
            _tool_proc.OutputDataReceived += new DataReceivedEventHandler(proc_OutputDataReceived);
            _tool_proc.ErrorDataReceived += new DataReceivedEventHandler(proc_OutputDataReceived);

            ConsoleOutForm.ShowWindow();

            try {
                _tool_proc.Start();
                _tool_proc.BeginOutputReadLine();
                _tool_proc.BeginErrorReadLine();
                //proc.WaitForExit();
                //string msg = proc.StandardOutput.ReadToEnd();
            }
            catch (Exception e1) {
                Console.WriteLine(e1);
            }

            return true;
        }

        void proc_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data);
        }

        protected override void Dispose(bool disposing)
        {
            StopERDTool();
            base.Dispose(disposing);
        }
    }
}
