using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BCILib.Util;
using BCILib.App;
using System.IO;
using BCILib.Amp;
using System.Net.Sockets;
using System.Threading;

namespace BCILib.MotorImagery
{
    public partial class BCICursorCtrl : Form
    {
        public BCICursorCtrl()
        {
            InitializeComponent();

            pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            pen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;

            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
        }

        private int tpos, cpos;
        private List<int> mlist = new List<int>();

        enum RobotCommand
        {
            None = 0,
            Left = 1,
            Right = 2
        }

        private void DoMove(RobotCommand rcmd)
        {
            string cmd = null;
            if (rcmd == RobotCommand.Left) {
                if (cpos > 0) {
                    cpos--;
                    cmd = "00:00:00:00, MEKAMotion, motion, 1, 1, 0";
                }
            } else if (rcmd == RobotCommand.Right) {
                if (cpos < cfg_hsteps - 1) {
                    cmd = "00:00:00:00, MEKAMotion, motion, 1, 2, 0";
                    cpos++;
                }
            } else {
                cmd = "00:00:00:00, MEKAMotion, motion, 0, 0, 1";
            }

            Console.WriteLine("{0}: Move command = {0}", rcmd, cmd);

            if (!string.IsNullOrEmpty(cmd) && robot_client != null && robot_client.Connected) {
                //"motion, start, direction, stop"
                //new NetworkStream(
                StreamWriter sw = new StreamWriter(robot_client.GetStream());

                Console.WriteLine("RobotDoMove: cmd={0}", cmd);
                try {
                    sw.Write(cmd);
                    sw.Flush();
                    Console.WriteLine("sent");
                } catch (Exception e) {
                    Console.WriteLine("RobotDoMove: error={0}", e.Message);
                    SetConnectionState();
                }
            }
        }

        private void BCICursorCtrl_KeyDown(object sender, KeyEventArgs e)
        {
            Rectangle rt = panelCursor.ClientRectangle;
            if (e.KeyCode == Keys.F4) {
                StartGame();
            } else if (e.KeyCode == Keys.F5) {
                StopGame();
            } else if (e.KeyCode == Keys.Z) {
                if (cpos > 0) cpos--;
            } else if (e.KeyCode == Keys.X) {
                if (cpos < cfg_hsteps - 1) cpos++;
            } else if (e.KeyCode == Keys.J) {
                DoMove(RobotCommand.Left);
            } else if (e.KeyCode == Keys.L) {
                DoMove(RobotCommand.Right);
             } else if (e.KeyCode == Keys.K) {
               DoMove(RobotCommand.None);
            }
        }

        Random rdm = null;
        ConsoleCapture.ConsoleFile cfile = null;

        private void StartGame()
        {
            ResManager rm = BCIApplication.SysResource;
            string ts = "Test_" + BCIApplication.TimeStamp;
            BCIApplication.SetProtocolName(rm, "MI_FLD_CourCtrl");
            string dir = TestDirSpecForm.GetProcPath(rm, false);
            if (string.IsNullOrEmpty(dir)) return;

            cfile = ConsoleCapture.StartConsoleFile(Path.Combine(dir, string.Format("{0}.log", ts)));
            AmpContainer.StartRecord(dir,  ts);

            mlist.Clear();
            rdm = new Random((int) DateTime.Now.Ticks);
            InitStart();
            timer.Start();
        }

        private void InitStart()
        {
            Rectangle rt = panelCursor.ClientRectangle;
            // target position
            tpos = rdm.Next(0, cfg_hsteps - cfg_tsize);
            cpos = cfg_hsteps / 2;
            // cpos = rdm.Next(0, cfg_hsteps - 1);

            wait_time = cfg_preptime * 1000;
            wait_start = BCIApplication.ElaspedMilliSeconds;
            mlist.Clear();
            mlist.Add(cpos);

            gstatus = game_satus.Start;
            calib_slist.Clear();

            panelCursor.Invalidate();
        }

        private void StopGame()
        {
            AmpContainer.StopRecord();
            cfile.EndLogFile();
            timer.Stop();
            gstatus = game_satus.None;

            if (robot_client != null) {
                robot_client.Close();
                robot_client = null;
                SetConnectionState();
            }
        }

        enum game_satus
        {
            None,
            Start,
            Move,
            Hit,
            Miss,
        }

        int wait_time = 0;
        int wait_start = 0;

        game_satus gstatus = game_satus.None;
        StringFormat sf = new StringFormat();

        List<double> calib_slist = new List<double>();

        private void timer_Tick(object sender, EventArgs e)
        {
            if (wait_time > 0) {
                int wt = BCIApplication.ElaspedMilliSeconds - wait_start;
                if (gstatus == game_satus.Hit) {
                    lblNumHit.Text = (wt >= wait_time || (wt / 200 & 1) == 1) ? num_hit.ToString() : "Hit";
                } else if (gstatus == game_satus.Miss) {
                    lblNumMiss.Text = (wt >= wait_time || (wt / 200 & 1) == 1) ? num_miss.ToString() : "Miss";
                }
                if (wt >= wait_time) {
                    if (gstatus != game_satus.Start) InitStart();
                    else {
                        if (cfg_calibrate) {
                            DoCalibration();
                        }
                        gstatus = game_satus.Move;
                        wait_time = 0;
                        wait_start = 0;
                    }
                }
                panelCursor.Invalidate();
                return;
            }

            //cpos.Y -= ystep;
            //move_plist.Insert(0, cpos);
            //int n = move_plist.Count;
            //if (n > 20) move_plist.RemoveAt(n - 1);
            //panelCursor.Invalidate();

            if (mlist.Count > cfg_vsteps) {
                if (cpos >= tpos && cpos < tpos + cfg_tsize) {
                    num_hit++;
                    gstatus = game_satus.Hit;
                } else {
                    num_miss++;
                    gstatus = game_satus.Miss;
                }

                wait_time = 3000; // 3 seconds
                wait_start = BCIApplication.ElaspedMilliSeconds;

                UpdateDispResults();
            }
        }

        public event Action<double, double> ThresholdValue_Changed;

        private void DoCalibration()
        {
            //double mean = calib_slist.Average();
            //thr_left = mean * 100 + 1;
            //thr_right = mean * 100 - 1;
            calib_slist.Sort();
            int n = calib_slist.Count;
            double dm = calib_slist[n / 2];
            if ((n & 1) == 0) { // even number
                dm = (dm + calib_slist[n / 2 - 1]) / 2;
            }

            double mean = calib_slist.Average();
            double s = 0;
            foreach (double d in calib_slist) {
                double v = d - mean;
                v *= v;
                s += v;
            }
            s = Math.Sqrt(s / n);

            thr_left = (dm + s)* 100;
            thr_right = (dm - s) * 100;

            BCIApplication.LogMessage("mean = {3}|std = {4}|median={0}|left={1}|right={2}",
                dm * 100, thr_left, thr_right, mean * 100, s * 100);

            if (ThresholdValue_Changed != null) ThresholdValue_Changed(thr_left, thr_right);
        }

        private void UpdateDispResults()
        {
            lblNumHit.Text = num_hit.ToString();
            lblNumMiss.Text = num_miss.ToString();
            if (num_hit + num_miss > 0) {
                lblAccurate.Text = ((double) num_hit / (num_hit + num_miss)).ToString("#0.##%");
            } else {
                lblAccurate.Text = null;
            }
        }

        int avg_num_scores = 250;
        double sum_score = 0;
        int num_score = 0;

        int num_hit = 0, num_miss = 0;

        internal void InitProcess(FLDProcessor _proc)
        {
            _proc.evt_out_score += new Action<double[]>(_proc_evt_out_score);
            avg_num_scores = _proc.Amplifier.header.samplingrate / 3;
        }

        // thresholds for left/ideal/right
        public double thr_left = 5;
        public double thr_right = -5;

        private List<double> score_his = new List<double>();

        void _proc_evt_out_score(double[] score)
        {
            if (gstatus == game_satus.Start) {
                // for calibration
                calib_slist.AddRange(score);
            }

            if (gstatus != game_satus.Move) return;

            foreach (double s in score) sum_score += s;
            num_score += score.Length;

            double dscore = sum_score / num_score * 100;
            if (dscore >= thr_left) dscore = (dscore - thr_left);
            else if (dscore <= thr_right) dscore = (dscore - thr_right);
            else dscore = 0;

            Rectangle rt = panelCursor.ClientRectangle;
            if (num_score >= avg_num_scores) {
                //cpos.X -= (float) ( dscore * curw);
                //if (cpos.X > rt.Width - curw) cpos.X = rt.Width - curw;
                //else if (cpos.X < curw) cpos.X = curw;

                num_score = 0;
                sum_score = 0;

                score_his.Add(dscore);


                if (score_his.Count >= 3) {
                    if (score_his[0] > 0 && score_his[1] > 0 && score_his[2] > 0) {
                        DoMove(RobotCommand.Left);
                    } else if (score_his[0] < 0 && score_his[1] < 0 && score_his[2] < 0) {
                        DoMove(RobotCommand.Right);
                    }
                    mlist.Add(cpos);
                    panelCursor.Invalidate();
                    score_his.Clear();
                }
            }
        }

        Pen pen = new Pen(Color.Yellow, 5);
        float xstep = 10F;
        float ystep = 10F;

        private void panelCursor_Paint(object sender, PaintEventArgs e)
        {
            pen.Width = xstep / 4;
            Graphics g = e.Graphics;

            Rectangle rt = panelCursor.ClientRectangle;

            // draw grid
            Pen gp = new Pen(Color.Gray);
            gp.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

            float x = 0;
            for (int i = 0; i < cfg_hsteps - 1; i++) {
                x += xstep;
                g.DrawLine(gp, x, 0, x, rt.Height);
            }

            float y = 0;
            for (int i = 0; i < cfg_vsteps; i++) {
                y += ystep;
                g.DrawLine(gp, 0, y, rt.Width, y);
            }

            // draw target
            if (gstatus != game_satus.Start || !cfg_calibrate) {
                x = tpos * xstep;
                g.FillRectangle(Brushes.Red, x, 0, xstep * cfg_tsize, ystep);

                // draw movement
                int nc = mlist.Count;
                if (nc > 0) {
                    int len = nc + 1;
                    PointF[] pline = new PointF[len];
                    pline[0].X = xstep / 2 + mlist[0] * xstep;
                    pline[0].Y = rt.Height;

                    y = rt.Height - ystep / 2;
                    for (int i = 1; i < len; i++) {
                        pline[i].X = xstep / 2 + mlist[i - 1] * xstep;
                        pline[i].Y = y;
                        y -= ystep;
                    }
                    g.DrawCurve(pen, pline);
                }
            }

            if (gstatus == game_satus.Start) {
                int wt = BCIApplication.ElaspedMilliSeconds - wait_start;
                wt = wait_time - wt;
                wt = (wt + 999) / 1000;
                if (wt > 0) {
                    Font font = new Font(FontFamily.GenericSansSerif, panelCursor.Height / 16F);
                    g.DrawString(wt.ToString(), font, Brushes.Yellow, panelCursor.ClientRectangle, sf);
                }
            }
        }

        private void bntReset_Click(object sender, EventArgs e)
        {
            num_hit = num_miss = 0;
            UpdateDispResults();
        }

        private void panelCursor_Resize(object sender, EventArgs e)
        {
            SetDispSizes();
        }

        private void SetDispSizes()
        {
            Rectangle rt = panelCursor.ClientRectangle;
            xstep = (float)rt.Width / cfg_hsteps;
            ystep = rt.Height / (cfg_vsteps + 1.0F);
        }

        int cfg_hsteps = 4;
        int cfg_vsteps = 4;
        int cfg_tsize = 1;
        int cfg_preptime = 3;
        bool cfg_calibrate = false;

        string robot_server = "localhost";
        int robot_port = 10000;

        private void btnCfg_Click(object sender, EventArgs e)
        {
            BCICursorCfg cfg = new BCICursorCfg();

            cfg.HSteps = cfg_hsteps;
            cfg.VSteps = cfg_vsteps;
            cfg.TargetSize = cfg_tsize;
            cfg.PrepareTime = cfg_preptime;
            cfg.Calibrate = cfg_calibrate;

            if (cfg.ShowDialog() == DialogResult.OK) {
                cfg_hsteps = cfg.HSteps;
                cfg_vsteps = cfg.VSteps;
                cfg_tsize = cfg.TargetSize;
                cfg_preptime = cfg.PrepareTime;
                cfg_calibrate = cfg.Calibrate;

                SaveConfig();

                LoadConfig();
                panelCursor.Invalidate();
            }
        }

        enum ConfigProp
        {
            HorizontalSteps,
            VerticalSteps,
            TargetSize,
            PrepTime,
            Calibrate,

            // TCP
            RobotServer,
            RobotPort
        }

        private void SaveConfig()
        {
            ResManager rm = BCIApplication.SysResource;
            string rn = this.GetType().ToString();
            rm.SetConfigValue(rn, ConfigProp.HorizontalSteps, cfg_hsteps);
            rm.SetConfigValue(rn, ConfigProp.VerticalSteps, cfg_vsteps);
            rm.SetConfigValue(rn, ConfigProp.TargetSize, cfg_tsize);
            rm.SetConfigValue(rn, ConfigProp.PrepTime, cfg_preptime);
            rm.SetConfigValue(rn, ConfigProp.Calibrate, cfg_calibrate);

            rm.SetConfigValue(rn, ConfigProp.RobotServer, robot_server);
            rm.SetConfigValue(rn, ConfigProp.RobotPort, robot_port);

            rm.SaveFile();
        }

        private void LoadConfig()
        {
            ResManager rm = BCIApplication.SysResource;
            string rn = this.GetType().ToString();
            rm.GetConfigValue(rn, ConfigProp.HorizontalSteps, ref cfg_hsteps);
            rm.GetConfigValue(rn, ConfigProp.VerticalSteps, ref cfg_vsteps);
            rm.GetConfigValue(rn, ConfigProp.TargetSize, ref cfg_tsize);

            rm.GetConfigValue(rn, ConfigProp.PrepTime, ref cfg_preptime);
            string line = rm.GetConfigValue(rn, ConfigProp.Calibrate.ToString());
            if (!string.IsNullOrEmpty(line)) {
                bool.TryParse(line, out cfg_calibrate);
            }

            string tstr = rm.GetConfigValue(rn, ConfigProp.RobotServer.ToString());
            if (!string.IsNullOrEmpty(tstr)) {
                robot_server = tstr;
            }
            rm.GetConfigValue(rn, ConfigProp.RobotPort, ref robot_port);

            cpos = cfg_hsteps / 2;

            SetDispSizes();
        }

        private void BCICursorCtrl_Load(object sender, EventArgs e)
        {
            LoadConfig();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            StartGame();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            StopGame();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (timer.Enabled) StopGame();

            if (robot_client != null && robot_client.Connected) {
                robot_client.Close();
                robot_client = null;
            }
        }

        TcpClient robot_client = null;

        private void btnConnect_Click(object sender, EventArgs e)
        {
            TCPConfigCtrl ctrl = new TCPConfigCtrl();
            ctrl.Host = robot_server;
            ctrl.Port = robot_port;
            if (UserCtrlForm.ShowCtrl(ctrl) == DialogResult.OK) {
                btnConnect.Enabled = false;
                robot_server = ctrl.Host;
                robot_port = ctrl.Port;
                SaveConfig();

                ThreadPool.QueueUserWorkItem((WaitCallback)delegate(object o)
                {
                    TcpClient tcp = new TcpClient();
                    try {
                        Console.WriteLine("Start robot connection ... ");
                        tcp.Connect(robot_server, robot_port);
                        Console.WriteLine("Connection successful.");
                        if (tcp.Connected) {
                            robot_client = tcp;
                        } else {
                            tcp.Close();
                        }
                        SetConnectionState();
                    } catch (Exception ex) {
                        Console.WriteLine("Connect robot server: error = " + ex.Message);
                    }

                    SetConnectionState();
                });
            }
        }

        private void SetConnectionState()
        {
            if (InvokeRequired) {
                Invoke((Action)delegate() {
                    SetConnectionState();
                });
            } else {
                if (robot_client != null && robot_client.Connected) {
                    btnConnect.BackColor = Color.Green;
                    btnConnect.Enabled = false;
                } else {
                    btnConnect.Enabled = true;
                    if (robot_client != null) {
                        robot_client.Close();
                        robot_client = null;
                    }
                }
            };
        }
    }
}
