using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using BCILib.App;
using BCILib.Amp;
using BCILib.Util;
using System.IO;
using System.Collections;
using BCILibCS.Util;
using System.IO.Ports;
using System.Threading;
using BCILib.EngineProc;
using System.Net.Sockets;
using System.Net;
using System.Text.RegularExpressions;

namespace BCILib.MotorImagery.BCIManus
{
    internal partial class BCIManusForm : BCILib.App.BCIAppForm
    {
        Action<double> dl_getscore = null;
        int _score_cls = -1;
        double _score_conf = 0;

        bool SupervisedTesting = false;

        private void MIFLD_GetScore(double score)
        {
            if (InvokeRequired) {
                BeginInvoke(dl_getscore, score);
            } else {
                _score_cls = (score >= 0) ? 0 : 1;
                _score_conf = score;

                tbFeedback.Text = fldScoreViewer.GetFeedbackMessage();

                if (SupervisedTesting) {
                    double acc = 0;
                    double bias = fldScoreViewer.Calculate_BestBias(out acc);

                    labelBestScore.Text = string.Format("Best score = {0:0.##} @ Bias = {1:0.##}", acc, bias);
                    btnSetBias.Tag = bias;

                    if (cbAutoAjust.Checked && fldScoreViewer.TotalTrials > 5) btnSetBias.PerformClick();
                } else {
                    panelBestBias.Visible = false;
                }
            }
        }

        public BCIManusForm()
        {
            InitializeComponent();

            dl_getscore = new Action<double>(MIFLD_GetScore);
        }

        //IPEndPoint broadcast = null;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            LoadAll();

            AmpContainer.AmpChanged += new Action(SetCurAmplifer);
            KMCapture.kbd_output += new Func<Keys,uint,uint,bool>(OnKeyboardInput);

            //IPAddress bip = SocketReadWrite.GetBroadcastAddress();
            //if (bip != null) {
            //    broadcast = new IPEndPoint(bip, 38632);
            //}
        }

        private bool OnKeyboardInput(Keys key, uint scan_code, uint flag)
        {
            if (key == Keys.F6) {
                SetBias(cfg_score_bias - 0.1, true, true);
            } else if (key == Keys.F5) {
                SetBias(cfg_score_bias + 0.1, true, true);
            }

            return false;
        }

        private void LoadAll()
        {
            ResManager rm = BCIApplication.SysResource;
            LoadConfig(rm);
            LoadModel(rm);
        }

        private string _key_cfg = null;

        private int cfg_stim_task_offset = 120;
        private int cfg_stim_rehab = 180;

        private double cfg_score_bias = 0;
        private bool cfg_save_bias = false;

        /// <summary>
        /// Load config and fill GUI items, need to be run in main thread
        /// </summary>
        /// <param name="rm"></param>
        protected override void LoadConfig(BCILib.Util.ResManager rm)
        {
            // read parameters from training
            string task_cfg = "LI"; // LRTFI

            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Task_Configure, ref task_cfg);
            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Key_Configure, ref _key_cfg);

            tbMIConfig.Text = task_cfg;
            string line = MIConstDef.DefChannels;
            rm.GetConfigValue(MIConstDef.ModelTrainingSetting, MIConstDef.Train_MTS_Channels, ref line);
            textSelChannels.Tag = line;
            if (!string.IsNullOrEmpty(line)) {
                string[] slist = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                int n = 0;
                if (slist != null) n = slist.Length;
                textSelChannels.Text = n.ToString();
            }

            rm.GetConfigValue(MIConstDef.MITest, "ScoreBias", ref cfg_score_bias);
            rm.GetConfigValue(MIConstDef.MITest, "SaveBias", ref cfg_save_bias);
            cbSaveBias.Checked = cfg_save_bias;

            if (cfg_save_bias)
            {
                SetBias(cfg_score_bias, true, false);
            }
            else
            {
                SetBias(0, true, false);
            }

            SetCurAmplifer();

            rm.SaveIfChanged();
        }

        private void SetBias(double nv, bool UpdateScrollBar, bool Save)
        {
            if (UpdateScrollBar) {
                int iv = (int)(nv * 10);

                if (iv < trackBarBias.Minimum) iv = trackBarBias.Minimum;
                else if (iv > trackBarBias.Maximum) iv = trackBarBias.Maximum;

                if (iv != trackBarBias.Value) {
                    object ov = trackBarBias.Tag;
                    trackBarBias.Tag = 1;
                    trackBarBias.Value = iv;
                    trackBarBias.Tag = ov;
                }
            }

            fldScoreViewer.SetBias(nv);

            textBoxBias.Text = nv.ToString("0.##");
            tbFeedback.Text = fldScoreViewer.GetFeedbackMessage();

            cfg_score_bias = nv;
            if (Save) {
                SaveConfig();
            }
        }

        private void SaveConfig()
        {
            ResManager res = BCIApplication.SysResource;
            res.SetConfigValue(MIConstDef.MITest, "ScoreBias", cfg_score_bias.ToString());
            res.SetConfigValue(MIConstDef.MITest, "SaveBias", cbSaveBias.Checked.ToString());
            res.SaveFile();
        }

        int cfg_prep_time = 2000;
        int cfg_act_time = 4000;
        int cfg_rest_time = 6000;
        int cfg_beep_before_rest_end = 1000;

        private void SetCurAmplifer()
        {
            labelSelAmplifier.Text = null;
            if (AmpContainer.Count > 0) {
                labelSelAmplifier.Text = AmpContainer.GetAmplifier(0).DevName;
                if (!string.IsNullOrEmpty(textSelChannels.Text)) {
                    int nsel = int.Parse(textSelChannels.Text);
                    SelAmpChannel cfg = new SelAmpChannel();
                    cfg.ShowAmplifier();
                    cfg.SelectedString = (string)textSelChannels.Tag;
                    if (cfg.SelectedNum != nsel) {
                        cfg.SelectedString = MIConstDef.DefChannels;
                        if (cfg.SelectedNum != nsel) {
                            cfg.SelectedNum = nsel;
                        }
                        textSelChannels.Tag = cfg.SelectedString;
                    }
                }
            }
        }

        FLDProcessor _proc = null;

        private bool LoadModel(ResManager rm)
        {
            string line = rm.GetConfigValue(MIConstDef.MIProcess, MIConstDef.ClassLabels);
            if (string.IsNullOrEmpty(line)) {
                return false;
            }
            string[] cls_labels = line.Split(',');

            line = rm.GetConfigValue(MIConstDef.MIProcess, MIConstDef.ModelPattern);
            if (string.IsNullOrEmpty(line)) {
                return false;
            }

            int p0 = line.IndexOf("_{0}");
            if (p0 > 0) {
                labelModelName.Text = line.Substring(0, p0);
            } else {
                labelModelName.Text = line;
            }

            int nmodel = cls_labels.Length - 1;
            if (nmodel == 2) nmodel = 1; // for two-classes, only one model is needed

            string[] mdl_names = new string[nmodel];

            for (int iclass = 0; iclass < mdl_names.Length; iclass++) {
                string fn = string.Format(line, iclass + 1);
                fn = Path.Combine("Model", fn);
                if (!File.Exists(fn)) return false;
                mdl_names[iclass] = fn;
            }

            _proc = CreatBCIProcessor(mdl_names);
            //new MIProcessor();

            int nchan = 0;

            if (_proc != null) {
                nchan = _proc.NumChannelUsed;
            }

            textModelChannels.Text = nchan.ToString();

            Amplifier amp = AmpContainer.GetAmplifier(0);
            _proc.SetAmplifier(amp, (string)textSelChannels.Tag);
            //_proc.SetReadingShift(500);

            fldScoreViewer.SetProcessor((FLDProcessor)_proc);

            FLDModel model = (_proc as FLDProcessor).GetFLDModel();
            lbModelInfo.Text = string.Format("Threshold={0:#.##}/{1} Range: {2:#.##}, {3:#.##}.",
                model.thr, model.atime, model.fld_r[0], model.fld_r[1]);

            return true;
        }

        enum MI_MODEL_TYPE {
            None,
            FB_ParzenWindow,//Feature Selection using MI based on Best Individual Features with Parzen Window
            FB_FLD
        };

        private FLDProcessor CreatBCIProcessor(string[] mdl_names)
        {
            MI_MODEL_TYPE mtype = MI_MODEL_TYPE.FB_ParzenWindow;

            if (mdl_names.Length < 1) {
                return null;
            }

            using (StreamReader sr = File.OpenText(mdl_names[0])) {
                string line;
                while ((line = sr.ReadLine())!= null) {
                    line = line.Trim();
                    if (line.StartsWith("#")) break;

                    string[] wlist = line.Split(':');
                    if (wlist.Length == 2 && wlist[0].Trim() == "Model_Type"
                        && wlist[1].Trim() == "BCI_MI_FBFLD") {
                        mtype = MI_MODEL_TYPE.FB_FLD;
                    }
                    break;
                }
                
            }

            if (mtype == MI_MODEL_TYPE.FB_ParzenWindow) {
                //MIProcessor miproc = new MIProcessor();
                //if (!miproc.Initialize(mdl_names)) {
                //    Console.WriteLine("Processor Initialization failed");
                //    miproc = null;
                //}
                //return miproc;
                return null;
            } else if (mtype == MI_MODEL_TYPE.FB_FLD) {
                FLDProcessor fld_proc = new FLDProcessor();
                if (!fld_proc.Initialize(mdl_names)) {
                    return null;
                }
                return fld_proc;
            }
            return null;
        }

        private void buttonAmpSel_Click(object sender, EventArgs e)
        {
            SelAmpChannel cfg = new SelAmpChannel();
            cfg.ShowAmplifier();

            string str_sch_list = (string) textSelChannels.Tag;
            if (str_sch_list == null) str_sch_list = MIConstDef.DefChannels;
            cfg.SelectedString = str_sch_list;

            if (UserCtrlForm.ShowCtrl(cfg) == DialogResult.OK) {
                string amp_str = cfg.SelectedAmplifier;
                Amplifier amp = AmpContainer.GetAmplifier(amp_str);
                if (amp != null) amp_str = amp.DevName;

                textSelChannels.Tag = cfg.SelectedString;
                textSelChannels.Text = cfg.SelectedNum.ToString();

                ResManager rm = BCIApplication.SysResource;
                rm.SetConfigValue(MIConstDef.ModelTrainingSetting, MIConstDef.Train_MTS_Channels, textSelChannels.Tag);
                rm.SaveIfChanged();
            }
        }

        private void buttonTrainModel_Click(object sender, EventArgs e)
        {
            this.Hide();
            MITrainMTSModelForm dlg = new MITrainMTSModelForm();
            List<int> ar = new List<int>();
            string tasktr = tbMIConfig.Text;
            for (int i = 0; i < tasktr.Length; i++) {
                int idx = MITaskConfig.TASK_IDLIST.IndexOf(tasktr[i]);
                if (idx >= 0) {
                    ar.Add(cfg_stim_task_offset + idx + 1);
                }
            }
            dlg.SetMIClassLabels(ar.ToArray());
            dlg.ShowDialog();
            this.Show();

            LoadModel(BCIApplication.SysResource);
        }

        private void buttonFindTool_Click(object sender, EventArgs e)
        {
            BCIApplication.GetGamePath("ERDTool", true);
        }

        private bool CheckExternalTool(WMCopyData _tool)
        {
            return CheckExternalTool(_tool, true);
        }

        private bool CheckExternalTool(WMCopyData _tool, bool BrowseIfNotFound)
        {
            if (_tool.GetAllGUIWindows(false) > 0) {
                return true;
            }

            if (_tool.GetAllGUIWindows() <= 0) {

                if (BCIApplication.StartExternalProc(_tool.Property, null, true, BrowseIfNotFound) == null) {
                    return false;
                }
            }

            for (int i = 0; _tool.GetAllGUIWindows() <= 0 && i < 10; i++)
            {
                Thread.Sleep(100);
            }

            if (_tool.GetAllGUIWindows() <= 0)
            {
                MessageBox.Show("Cannot find client window!");
                return false;
            }

            // ccwang: Initialize command
            _tool.SendClient(GameCommand.Initialize, BCIApplication.UserPath);

            return true;
        }

        MIAction _action_hand = MIAction.None;

        private void tbMIConfig_TextChanged(object sender, EventArgs e)
        {
            char c = tbMIConfig.Text[0];
            int idx = MITaskConfig.TASK_IDLIST.IndexOf(c);
            _action_hand = (MIAction)(idx + 1);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_proc != null) {
                _proc.Dispose();
                _proc = null;
            }

            base.OnFormClosing(e);
        }

        private void btnChangeCfg_Click(object sender, EventArgs e)
        {
            MITaskConfig cfg = new MITaskConfig();
            cfg.TaskString = tbMIConfig.Text;
            cfg.KeyCodeString = _key_cfg;

            if (UserCtrlForm.ShowCtrl(cfg) == DialogResult.OK) {
                cfg.SaveConfigure(BCIApplication.SysResource);
                LoadConfig();
            }
        }

        enum ServerStatus
        {
            None, Startup, Started
        };

        ServerStatus _server_status = ServerStatus.None;

        private void btnStartServer_Click(object sender, EventArgs e)
        {
            if (AmpContainer.Count <= 0) {
                MessageBox.Show("No amplifier defined!", "Amplifier error");
                return;
            }

            if (!AmpContainer.StartAll()) {
                return;
            }

            Amplifier amp = AmpContainer.GetAmplifier(0);
            if (amp.CheckEventTimeout) {
                amp.CheckEventTimeout = false; // ccwang 20120504: stim code was sent from another machine!
                amp.SaveConfig(BCIApplication.AppResource);
            }

            if (!amp.IsAlive) {
                MessageBox.Show("Cannot start amplifier!", "Amplifier error");
            }
            

            switch (_server_status) {
                case ServerStatus.None:
                    _server_status = ServerStatus.Startup;
                    btnStartServer.Text = "Stop Server";
                    btnStartDataCollection.Enabled = true;
                    btnStartRehab.Enabled = true;
                    bw_Server.RunWorkerAsync();
                    btnRecordEEG.Enabled = false;
                    break;
                case ServerStatus.Startup:
                    break;
                case ServerStatus.Started:
                    bw_Server.CancelAsync();
                    break;
            }
        }

        private void bw_Server_DoWork(object sender, DoWorkEventArgs e)
        {
            // for broadcasting
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            s.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.IpTimeToLive, 1);
            IPEndPoint mip = new IPEndPoint(IPAddress.Parse("225.0.0.37"), 38632);

            TcpListener tcp_server = new TcpListener(IPAddress.Any, 8002);
            tcp_server.Start();
            Console.WriteLine("Server listening on {0}:8002.", Dns.GetHostName());

            _server_status = ServerStatus.Started;
            Console.WriteLine("Server started.");

            List<Socket> llistener = new List<Socket>();

            int status = -1;
            while (!bw_Server.CancellationPending) {
                llistener.Clear();
                llistener.Add(tcp_server.Server);
                Socket.Select(llistener, null, null, 500000);
                if (llistener.Count == 0) {
                    byte[] message = ASCIIEncoding.ASCII.GetBytes("BCIManus Server " + DateTime.Now.ToString());
                    s.SendTo(message, mip);
                    if (status != 0) {
                        status = 0;
                        Console.WriteLine("Multicasting: {0}", mip.ToString());
                    }
                    continue;
                }

                status = 1;
                TcpClient client = tcp_server.AcceptTcpClient();
                Console.WriteLine("Get connection from {0}", client.Client.RemoteEndPoint);

                SocketReadWrite rw = new SocketReadWrite(client.Client);

                while (!bw_Server.CancellationPending) {
                    string line = rw.ReadLine(15000);
                    if (line == null) {
                        if (client.Connected) continue;
                        else {
                            Console.WriteLine("Server - connection broke.");
                            break;
                        }
                    }

                    Console.WriteLine("Server: received={0}", line);

                    if (line.Equals("BCIStartTraining")) {
                        Console.WriteLine("Server DoWork: send back OK");
                        rw.WriteLine("OK");
                        ExeServerDataCollection(client.Client, rw);
                    } else if (line.StartsWith("BCIStartTesting")) {
                        string kw = "Rehab";
                        string[] wl = line.Split(' ');
                        if (wl.Length > 1) kw = wl[1];
                        Console.WriteLine("Server DoWork: send back OK");
                        rw.WriteLine("OK");
                        ExeBCIServTesting(client.Client, rw, kw);
                    } else {
                        Console.WriteLine("Unknown command: {0}", line);
                    }

                    break;
                }

                Console.WriteLine("Connection ends.");
                client.Client.Close();
                client.Close();
            }

            tcp_server.Server.Close();
            tcp_server.Stop();
            Console.WriteLine("Server ends.");
        }

        private void ExeBCIServTesting(Socket client, SocketReadWrite rw, string kw)
        {
            SupervisedTesting = (!string.IsNullOrEmpty(kw) && kw.StartsWith("Supervised"));//SupervisedTest

            if (_proc == null || _proc.NumChannelUsed <= 0 || AmpContainer.Count <= 0) {
                Console.WriteLine("Server: Processor not ready! check model and amplifier!");
                return;
            }

            fldScoreViewer.SetProcessor((FLDProcessor)_proc);
            fldScoreViewer.Initialize();
            fldScoreViewer.SetAccuTimes(1000, SupervisedTesting ? 0 : 1000);

            Amplifier amp = AmpContainer.GetAmplifier(0);
            amp.Start();

            ResManager rm = BCIApplication.SysResource;
            rm.SetConfigValue("EEG", "TestDataDir", "DataTesting_Robot");

            string app_name = kw + "_" + tbMIConfig.Text;
            string rend = client.RemoteEndPoint.ToString();
            int k = rend.IndexOf(':');
            if (k > 0) rend = rend.Substring(0, k);
            //app_name += "_" + rend;

            BCIApplication.SetProtocolName(rm, app_name);
			//LoadConfig(rm);

			string dpath = TestDirSpecForm.GetProcPath(rm, true, true);
			if (dpath == null) {
				return;
			}

			string timestamp = BCIApplication.TimeStamp;
            string fn_key = app_name + "_" + timestamp;

			//Logfile
            var cfile = ConsoleCapture.StartConsoleFile(Path.Combine(dpath, fn_key + ".log"));
            AmpContainer.StartRecord(dpath, fn_key);
            DataFileManager.AddEEGFiles(dpath, fn_key);

            try {
                string cfg_fn = BCIApplication.UserCfgPath;
                if (File.Exists(cfg_fn)) {
                    File.Copy(cfg_fn, Path.Combine(dpath, fn_key + ".cfg"));
                }
            } catch { }

            _evt_client_simulate.Reset();

			Console.WriteLine("BCIServTesting: new connection at {0} for {1}",
                DateTime.Now, app_name);

			// initialize supervised testing varibables
			int supervised_n = 0;
			int supervised_ta = 0;
			int supervised_tr = 0;
			int supervised_fa = 0;
			int supervised_fr = 0;
			int moveflag = 0;

            MI_STEP step = MI_STEP.MI_None;

			// tells client that server is OK
            string rpl = ReplyInitClient();
            Console.WriteLine("Server Rehab: send={0}", rpl);
			rw.WriteLine(rpl);

            //fB_TwoClassBar.SetFBSocket(client, rw);

            //int nScore = 4;
            //int nStart = 4;
            //int iScore = 1;
            //double vScore = 0;

            //_proc.SetFeedbackHandler((MIProcessor.MIOutout)delegate(double[] sv)
            //{
            //    vScore = sv[0];
            //});

            int tmin = 2 * amp.header.samplingrate;
            int tmax = 4 * amp.header.samplingrate;

            for (; ; ) {
                if (bw_Server.CancellationPending) {
                    Console.WriteLine("ServerDC: break due to server stopped.");
                    break;
                }

                if (!amp.IsAlive) {
                    Console.WriteLine("ServerDC: break due to amplifer stopped!");
                    break;
                }

                string line = rw.ReadLine(15000);

                if (line != null) {
                    Console.WriteLine("Server Rehab: recv = {0}", line);
                    ServerProcessLine(line);

                    Match m = Regex.Match(line, @"Stim (\d+)");
                    if (m.Success) {
                        int code = int.Parse(m.Groups[1].Value);
                        if (code == 100) {
                            Console.WriteLine("Server Rehab: Prepare");
                            step = MI_STEP.MI_Prepare;
                            //fB_TwoClassBar.Reset();
                        } else if (code >= 120 && code < 130) {
                            _score_cls = -1;
                            Console.WriteLine("Server Rehab: Action");
                            step = MI_STEP.MI_Action;
                        } else if (code == 199) {
                            Console.WriteLine("Server Rehab: Rest");
                            step = MI_STEP.MI_Rest;
                        } else if (code == cfg_stim_rehab) {
                            Console.WriteLine("Server Rehab: Rehab");
                            step = MI_STEP.MI_Rehab;
                        }

                        //fB_TwoClassBar.SetMIStep(step);
                        continue;
                    }

                    // For action
                    if (line.Equals("StartBCI")) {
                        supervised_n++;
                        moveflag = 0;
                        //_proc.SetReadingPos(0);
                        //iScore = 0;
                        fldScoreViewer.StartAccumulate(1, dl_getscore); //tmin, tmax, 
                    } else if (line.Equals("StartBCIMove")) {
                        supervised_n++;
                        moveflag = 1;
                        //_proc.SetReadingPos(0);
                        //iScore = 0;
                        fldScoreViewer.StartAccumulate(0, dl_getscore); //tmin, tmax, 
                    }
                    continue;
                }

                if (!client.Connected) {
                    Console.WriteLine("Server Rehab: connection broken!");
                    break;
                }

                if (step != MI_STEP.MI_Action) continue;

                //if (!_proc.Process()) continue;
                if (_score_cls == -1) continue;

                if (_evt_client_simulate.WaitOne(0, false)) break;

                //iScore++;

                //if (iScore == nStart) {
                //    fB_TwoClassBar.StartAccuScore();
                //}
                //if (iScore < nStart + nScore) continue;

                // get result
                //double fscore = 0.5;
                //int rst = fB_TwoClassBar.GetDetection(out fscore);
                //Console.WriteLine("Proc score={0}, rst={1} (0=action, 1=rest)", fscore, rst);
                Console.WriteLine("Proc score={0}, rst={1} (0=action, 1=rest)",
                    _score_conf, _score_cls);

                //Invoke((Action)delegate()
                //{
                //    statusMessage.Text = string.Format("Score = {0}", fscore);
                //});

                double fscore = 0.5 + _score_conf / 10;
                if (fscore < 0) fscore = 0;
                else if (fscore > 1) fscore = 1;

                //if (rst == 1) {
                //    // change score from idle to action hand
                //    fscore = 1 - fscore;
                //}

                step = MI_STEP.MI_Rest;
                //fB_TwoClassBar.SetMIStep(step);

                if (_action_hand == MIAction.Right) fscore = 1 - fscore;

                int conf = (int)(fscore * 100);
                conf -= (100 - conf);
                if (conf < 0) conf = -conf;

                Console.WriteLine("Server: send=BCIScore {0:0.00####}, conf={1}",
                    fscore, conf);
                rw.WriteLine("BCIScore 0 {0:0.00####}", fscore);

                bool bci_action = false;

                //if (conf > 10)
                {
                    if (_score_cls == 0) {
                        bci_action = true;
                    }
                }

                Console.WriteLine("Server Rehab: send=BCIResult {0}", bci_action ? "yes" : "no");
                rw.WriteLine("BCIResult {0}", bci_action ? "yes" : "no");

                if (moveflag != 0) {
                    if (bci_action) supervised_ta++;
                    else supervised_fr++;
                } else {
                    if (bci_action) supervised_fa++;
                    else supervised_tr++;
                }
            }
            //_proc.SetFeedbackHandler(null);

            AmpContainer.StopRecord();

			Console.WriteLine("BCIServer: connection ends at {0}", DateTime.Now);
			if (supervised_ta + supervised_fr > 0) {
				Console.WriteLine("All BCI trials:{0}, true acc:{1} true rej:{2} false acc:{3}, false rej:{4}", 
					supervised_n, supervised_ta, supervised_tr, supervised_fa, supervised_fr);
				Console.WriteLine("Accuracy = {0}%", (supervised_ta + supervised_tr) * 100 / supervised_n);
			}
			Console.WriteLine("\n\n");

			cfile.EndLogFile();
            //fB_TwoClassBar.Reset();
            DataFileManager.AddDataDir(dpath);
		}

        private string ReplyInitClient()
        {
            // Initialize client variables
            double thr = 0.5;
            double df = 0.1;
            if (_action_hand == MIAction.Left) {
                // left hand
                thr += df;
            } else if (_action_hand == MIAction.Right) {
                thr -= df;
            }

            //if (cfg_act_hand == 1) { // left hand action
            //    thr += cfg_conf_thr / 200.0;
            //} else {
            //    thr -= cfg_conf_thr / 200.0;
            //}

            string reply = string.Format("BCIInit {{{0} {1} {2} {3} {4} {5} }}",
                cfg_rest_time, cfg_beep_before_rest_end, cfg_prep_time, cfg_act_time, 1, thr);

            return reply;
        }

        private void ExeServerDataCollection(Socket client, SocketReadWrite rw)
        {
            ResManager rm = BCIApplication.SysResource;

            // specify app name
            string app_name = "MI_" + tbMIConfig.Text;
            string rend = client.RemoteEndPoint.ToString();
            int k = rend.IndexOf(':');
            if (k > 0) rend = rend.Substring(0, k);
            //app_name += "_" + rend;

            BCIApplication.SetProtocolName(rm, app_name);
            //LoadConfig(rm);

            string dpath = TrainDirSpecForm.GetTrainingPath(rm, true);
            if (dpath == null) return;

            string timestamp = BCIApplication.TimeStamp;
            string fn_key = app_name + "_" + timestamp;

            // Logfile
            var cfile = ConsoleCapture.StartConsoleFile(Path.Combine(dpath, fn_key + "_Main.log"));

            // Start Amplifier
            AmpContainer.StartAll();
            AmpContainer.StartRecord(dpath, fn_key);
            DataFileManager.AddEEGFiles(dpath, fn_key);

            Console.WriteLine("Server Train: new data collection at {0}", DateTime.Now);
            try {
                string cfg_fn = BCIApplication.UserCfgPath;
                if (File.Exists(cfg_fn)) {
                    File.Copy(cfg_fn, Path.Combine(dpath, fn_key + ".cfg"));
                }
            } catch { }

            // Pass parameters
            rw.WriteLine(ReplyInitClient());

            List<Socket> sl = new List<Socket>();

            for (; ; ) {
                //evt_StartBCIProc.Reset();
                if (bw_Server.CancellationPending) {
                    Console.WriteLine("ServerDC: break due to server stopped.");
                    break;
                }

                if (!AmpContainer.AllAlive) {
                    //evt_stopProc.Set();
                    Console.WriteLine("ServerDC: break due to amplifer stopped!");
                    break;
                }

                sl.Clear();
                sl.Add(client);
                Socket.Select(sl, null, null, 100000);
                if (sl.Count == 0) {
                    continue;
                }

                string line = rw.ReadLine(15000);
                if (line == null) {
                    if (!client.Connected) {
                        Console.WriteLine("Server DC: connection broken!");
                        break;
                    }
                } else {
                    Console.WriteLine("ServerDC: recv={0}", line);
                    if (!ServerProcessLine(line)) {
                    }
                }
            }
            AmpContainer.StopRecord();

            if (!bw_Server.CancellationPending) {
                Console.WriteLine("Training finished, add cnt to training list: {0}", dpath);
                BCIApplication.AddTrainingPath(dpath);
            }

            Console.WriteLine("\n\n");
            cfile.EndLogFile();
            DataFileManager.AddDataDir(dpath);
        }

        /// <summary>
        /// Process client command
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private bool ServerProcessLine(string line)
        {
            if (line.StartsWith("Stim ")) {
                int sc = int.Parse(line.Substring(5));
                sc &= 0xFF;
                Console.WriteLine("Send stim {0}", sc);
                AmpContainer.SendAll(sc);
                return true;
            }

            return false;
        }

        private void bw_Server_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _evt_client_simulate.Set();
            bw_ClientSimulator.CancelAsync();

            btnStartServer.Text = "Start Server";
            _server_status = ServerStatus.None;

            btnStartDataCollection.Enabled = false;
            btnStartRehab.Enabled = false;
            btnRecordEEG.Enabled = true;
        }

        private void btnStartDataCollection_Click(object sender, EventArgs e)
        {
            if (bw_ClientSimulator.IsBusy) {
                _evt_client_simulate.Set();
                bw_ClientSimulator.CancelAsync();
            } else {
                btnStartDataCollection.Text = "Stop Data Collection";
                btnStartRehab.Enabled = false;
                bw_ClientSimulator.RunWorkerAsync(1);
            }
        }

        private void bw_ClientSimulator_DoWork(object sender, DoWorkEventArgs e)
        {
            int tt = (int) e.Argument;
            switch (tt) {
                case 1:
                    Simulate_ClientDataCollection();
                    break;
                case 2:
                    Simulate_ClientRehab();
                    break;
            }
        }

        private void Simulate_ClientRehab()
        {
            TcpClient client = new TcpClient();
            client.Connect("localhost", 8002);

            SocketReadWrite rw = new SocketReadWrite(client.Client);

            // Rehab / SupervisedTest
            if (supervised) {
                Console.WriteLine("Client Rehab: start SupervisedTest");
                if (client.Connected) rw.WriteLine("BCIStartTesting SupervisedTest");
            } else {
                Console.WriteLine("Client Rehab: start Rehab");
                if (client.Connected) rw.WriteLine("BCIStartTesting Rehab");
            }

            int time_rest = 2000;
            int time_beep1 = 1000;
            int time_prep = 2000;
            int time_act = 4000;
            double thr = 0.50;

            _evt_client_simulate.Reset();

            MI_STEP mi_step = MI_STEP.MI_None;
            int act_time = 0;
            bool beep = false;

            Queue<int> tasks = new Queue<int>();
            Random rnd = new Random();

            int act_hand = 1; // 1=left, 2=right, updated by server
            int itrial = 0;
            int act = 5;

            while (!bw_ClientSimulator.CancellationPending) {
                string line = rw.ReadLine(15000);

                if (line == null) {
                    if (!client.Connected) {
                        Console.WriteLine("Client: connected broken");
                        break;
                    }

                    // change state
                    int tnow = BCIApplication.ElaspedMilliSeconds;
                    if (act_time == 0 || tnow < act_time) continue;

                    switch (mi_step) {
                        case MI_STEP.MI_Prepare:
                            // change to action
                            if (supervised) {
                                if (tasks.Count == 0) {
                                    int t = rnd.Next(2);
                                    tasks.Enqueue(5 + (act_hand - 5) * t);
                                    tasks.Enqueue(act_hand - (act_hand - 5) * t);
                                }

                                act = tasks.Dequeue();
                            } else {
                                act = act_hand;
                            }

                            Console.WriteLine("Client Rehab: send=Stim {0}", cfg_stim_task_offset + act);
                            rw.WriteLine("Stim {0}", cfg_stim_task_offset + act);

                            string cmd = act == 5 ? "StartBCI" : "StartBCIMove";
                            Console.WriteLine("Client Rehab: send={0}", cmd);
                            if (client.Connected) rw.WriteLine(cmd);

                            act_time = 0; // wait for server response
                            mi_step = MI_STEP.MI_Action;

                            break;
                        case MI_STEP.MI_Rehab:
                            Console.WriteLine("Client: Rest");
                            if (client.Connected) rw.WriteLine("Stim 199");

                            beep = true;
                            act_time = BCIApplication.ElaspedMilliSeconds + time_rest - time_beep1;
                            mi_step = MI_STEP.MI_Rest;
                            break;
                        case MI_STEP.MI_Rest:
                            if (beep) {
                                Console.WriteLine("Client: Beep");
                                Sound.BeepAsunc(500, 300);

                                beep = false;
                                act_time = BCIApplication.ElaspedMilliSeconds + time_beep1;
                            } else {
                                if (itrial >= 10) {
                                    bw_ClientSimulator.CancelAsync();
                                    break;
                                }

                                act_time = BCIApplication.ElaspedMilliSeconds + time_prep;
                                mi_step = MI_STEP.MI_Prepare;

                                Console.WriteLine("Client Rehab: prepare");
                                rw.WriteLine("Stim 100");
                            }
                            break;
                    }

                    continue;
                }

                if (line.StartsWith("OnlineScore")) {
                    continue;
                }

                Console.WriteLine("Client: receive={0}", line);

                Match m; 
                if (line.StartsWith("BCIInit")) {
                    m = Regex.Match(line, @"^BCIInit\s*\{\s*(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d*(\.\d*)?)\s*\}\s*$");
                    int n = m.Groups.Count;
                    if (n > 1) int.TryParse(m.Groups[1].Value, out time_rest);
                    if (n > 2) int.TryParse(m.Groups[2].Value, out time_beep1);
                    if (n > 3) int.TryParse(m.Groups[3].Value, out time_prep);
                    if (n > 4) int.TryParse(m.Groups[4].Value, out time_act);
                    if (n > 6) double.TryParse(m.Groups[6].Value, out thr);

                    act_hand = thr > 0.5 ? 1 : 2;

                    act_time = BCIApplication.ElaspedMilliSeconds + time_prep;
                    mi_step = MI_STEP.MI_Prepare;

                    Console.WriteLine("Client Rehab: prepare");
                    rw.WriteLine("Stim 100");
                    continue;
                }

                m = Regex.Match(line, @"^BCIScore (\d+) (\d*\.?\d*)");
                if (m.Success) {
                    Console.WriteLine("Score={0}", m.Groups[2]);
                    continue;
                }

                m = Regex.Match(line, @"^BCIResult\s+(\w+)");
                if (m.Success) {
                    bool bYes = false;
                    if (string.Compare("yes", m.Groups[1].Value, true) == 0)
                        bYes = true;

                    if (bYes && act != 5) {
                        if (client.Connected) rw.WriteLine("Stim 180");
                        Console.WriteLine("Rehab ...\n");

                        act_time = BCIApplication.ElaspedMilliSeconds + 2000;
                        mi_step = MI_STEP.MI_Rehab;
                    } else {
                        Console.WriteLine("Client: Rest");
                        if (client.Connected) rw.WriteLine("Stim 199");

                        mi_step = MI_STEP.MI_Rest;
                        beep = true;
                        act_time = BCIApplication.ElaspedMilliSeconds + time_rest - time_beep1;
                    }
                    itrial++;
                    continue;
                }
            }

            Console.WriteLine("Client: stop.");
            client.Client.Close();
            client.Close();
        }

        ManualResetEvent _evt_client_simulate = new ManualResetEvent(false);

        private void Simulate_ClientDataCollection()
        {
            TcpClient client = new TcpClient();
            client.Connect("localhost", 8002);

            NetworkStream ns = new NetworkStream(client.Client);
            StreamReader sr = new StreamReader(ns);
            StreamWriter sw = new StreamWriter(ns);
            sw.AutoFlush = true;

            if (client.Connected) sw.WriteLine("BCIStartTraining");

            int time_rest = 2000;
            int time_beep1 = 1000;
            int time_prep = 2000;
            int time_act = 4000;
            double thr = 0.50;

            _evt_client_simulate.Reset();

            List<Socket> llist = new List<Socket>();
            while (true) {
                llist.Clear();
                llist.Add(client.Client);
                Socket.Select(llist, null, null, 100000);
                if (llist.Count == 0) continue;

                string line = sr.ReadLine();
                if (line == null) {
                    Console.WriteLine("Client: connected broken");
                    break; // return?
                }

                Console.WriteLine("Client: receive={0}", line);

                if (line.StartsWith("BCIInit")) {
                    Match m = Regex.Match(line, @"^BCIInit\s*\{\s*(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d*(\.\d*)?)\s*\}\s*$");
                    int n = m.Groups.Count;
                    if (n > 1) int.TryParse(m.Groups[1].Value, out time_rest);
                    if (n > 2) int.TryParse(m.Groups[2].Value, out time_beep1);
                    if (n > 3) int.TryParse(m.Groups[3].Value, out time_prep);
                    if (n > 4) int.TryParse(m.Groups[4].Value, out time_act);
                    if (n > 6) double.TryParse(m.Groups[6].Value, out thr);
                    break;
                }
            }

            Queue<int> tasks = new Queue<int>();
            Random rnd = new Random();

            int act_hand = thr >= 0.5 ? 1 : 2;

            for (int itrial = 0; itrial < 10; itrial++) {
                if (bw_ClientSimulator.CancellationPending) break;
                if (!client.Connected) break;

                Console.WriteLine("Client: prepare");
                if (client.Connected) sw.WriteLine("Stim 100");
                if (_evt_client_simulate.WaitOne(time_prep)) break;

                if (tasks.Count == 0) {
                    int t = rnd.Next(2);
                    tasks.Enqueue(5 + (act_hand - 5) * t);
                    tasks.Enqueue(act_hand - (act_hand - 5) * t);
                }

                // prepare
                int act = tasks.Dequeue();
                Console.WriteLine("Client: Action={0}", act);
                if (client.Connected) sw.WriteLine("Stim {0}", cfg_stim_task_offset + act);
                if (_evt_client_simulate.WaitOne(time_prep)) break;

                // action
                if (_evt_client_simulate.WaitOne(time_act)) break;

                // Rest
                Console.WriteLine("Client: Rest");
                if (client.Connected) sw.WriteLine("Stim 199");
                int wt = time_rest - time_beep1;
                if (_evt_client_simulate.WaitOne(wt, false)) break;
                if (time_beep1 > 0) {
                    Sound.BeepAsunc(500, 300);
                    if (_evt_client_simulate.WaitOne(time_beep1, false)) break;
                }
            }

            Console.WriteLine("Client: stop.");
            client.Client.Close();
            client.Close();
        }

        private void bw_ClientSimulator_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnStartDataCollection.Text = "Start Data Collection";
            btnStartRehab.Text = "Start Rehabilitation";

            bool enable = (_server_status != ServerStatus.None);
            btnStartRehab.Enabled = btnStartDataCollection.Enabled = enable;
        }

        bool supervised = false;
        private void btnStartRehab_Click(object sender, EventArgs e)
        {
            if (bw_ClientSimulator.IsBusy) {
                _evt_client_simulate.Set();
                bw_ClientSimulator.CancelAsync();
            } else {
                btnStartRehab.Text = "Stop Rehabilitation";
                btnStartDataCollection.Enabled = false;
                supervised = cbSupervised.Checked;
                bw_ClientSimulator.RunWorkerAsync(2);
            }
        }

        private void btnSetBias_Click(object sender, EventArgs e)
        {
            object tag = btnSetBias.Tag;
            if (tag is Double) {
                SetBias((double)tag, true, true);
            }
        }

        private void btnBiasReset_Click(object sender, EventArgs e)
        {
            SetBias(0, true, true);
        }

        private void btnStartRecord_Click(object sender, EventArgs e)
        {
            RecordEEGForm rf = new RecordEEGForm();
            AmpContainer.StartAll();
            rf.ShowDialog();
        }

        private void trackBarBias_ValueChanged(object sender, EventArgs e)
        {
            if (trackBarBias.Tag == null)
            {
                SetBias(trackBarBias.Value / 10.0, false, true);
            }
        }

        private void btnSupervisedAccHistory_Click(object sender, EventArgs e)
        {
            CalculateUserAccuracy("Supervised");
        }

        private void CalculateUserAccuracy(string keyword)
        {
            string accu_fn;
            Regex r;

            accu_fn = keyword + "_Accu_His.txt";

            //.\20110905\DataTesting_Robot\SupervisedTest_LI_2011-09-05\
            r = new Regex(@"\.\\(\d*)\\DataTesting.*\\.*\\" + keyword + @".*\.log"); // [^\\]*

            int tall = 0;
            int tc = 0;

            using (StreamWriter sw = File.CreateText(accu_fn)) {
                string[] log_fl = Directory.GetFiles(".", keyword + "*.log", SearchOption.AllDirectories);
                if (log_fl.Length > 0) {
                    long[] ctl = new long[log_fl.Length];
                    for (int i = 0; i < log_fl.Length; i++) {
                        // sorting
                        ctl[i] = File.GetCreationTime(log_fl[i]).Ticks;
                    }
                    Array.Sort<long, string>(ctl, log_fl);
                }
                foreach (string fn in log_fl) {
                    Match m = r.Match(fn);
                    if (m.Success) {
                        int ntotal = 0;
                        int ntrue = 0;
                        FindAccFromLogfile(fn, false, out ntotal, out ntrue, sw);
                        tall += ntotal;
                        tc += ntrue;
                    }
                }

                sw.WriteLine();
                double a = 0;
                if (tall > 0) a = tc * 100.0 / tall;

                sw.WriteLine("Total trials = {0}, Correct = {1}, Acc = {2:#0.00}%", tall, tc, a);
            }

            System.Diagnostics.Process.Start(accu_fn);
        }

        private void FindAccFromLogfile(string fn, bool recal, out int ntotal, out int ntrue, TextWriter sw)
        {
            // All BCI trials:3, true acc:0 true rej:1 false acc:0, false rej:2
            Regex ar = new Regex(@"^All BCI trials:(\d+), true acc:(\d+) true rej:(\d+) false acc:(\d+), false rej:(\d+)$");

            sw.WriteLine("{0}:", Path.GetFileName(fn));

            ntotal = ntrue = 0;
            int nta = 0, ntr = 0, nfa = 0, nfr = 0;

            // find accuracy find
            bool found = false;
            double acc_r = 0;

            string[] lines = File.ReadAllLines(fn);
            if (!recal) {
                foreach (string line in lines) {
                    Match m = ar.Match(line);
                    if (m.Success) {
                        found = true;
                        sw.WriteLine(line);
                        int.TryParse(m.Groups[1].Value, out ntotal);
                        int.TryParse(m.Groups[2].Value, out nta);
                        int.TryParse(m.Groups[3].Value, out ntr);
                        int.TryParse(m.Groups[4].Value, out nfa);
                        int.TryParse(m.Groups[5].Value, out nfr);
                        ntrue = nta + ntr;
                        acc_r = ntrue * 100.0 / ntotal;
                        sw.WriteLine("Accuracy = {0:#0.00}%", acc_r);
                    }
                }
            }

            if (!found) {
                ntotal = nta = ntr = nfa = nfr = 0;
                sw.WriteLine("Calculating...");
                int label = 0;

                Regex bci_move = new Regex(@".*recv\s*=\s*StartBCIMove$");
                Regex bci_stop = new Regex(@".*recv\s*=\s*StartBCI$");
                foreach (string line in lines) {
                    if (bci_move.Match(line).Success) {
                        label = 0; // action
                    } else if (bci_stop.Match(line).Success) {
                        label = 1; // rest
                    } else if (line.EndsWith("send=BCIResult yes")) {
                        if (label == 0) nta++;
                        else nfa++;
                    } else if (line.EndsWith("send=BCIResult no")) {
                        if (label == 0) nfr++;
                        else ntr++;
                    }
                }

                double acc = 0;
                ntrue = nta + ntr;
                ntotal = ntrue + nfa + nfr;
                if (ntotal > 0) acc = ntrue * 100.0 / ntotal;

                string msg = string.Format("All BCI trials:{0}, true acc:{1} true rej:{2} false acc:{3}, false rej:{4}\r\nAccuracy = {5:#0.00}%",
                    ntotal, nta, ntr, nfa, nfr, acc);
                sw.WriteLine(msg);
                File.AppendAllText(fn, msg + "\r\n");
            }

            sw.WriteLine();
        }

        private void btnAccuLogFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Log file (*.log)|*.log|All Files|*.*";
            dlg.RestoreDirectory = true;
            if (dlg.ShowDialog() == DialogResult.OK) {
                string fn = Path.GetTempFileName();
                int ntotal, ntrue;
                FindAccFromLogfile(dlg.FileName, true, out ntotal, out ntrue, Console.Out);
                ConsoleOutForm.ShowWindow();
            }
        }

        private void btnRehabAccHistory_Click(object sender, EventArgs e)
        {
            CalculateUserAccuracy("Rehab");
        }

        private void cbSaveBias_CheckedChanged(object sender, EventArgs e)
        {
            SaveConfig();
            cfg_save_bias = cbSaveBias.Checked;
            if (cfg_save_bias) {
                SetBias(cfg_score_bias, true, false);
            }
        }
    }
}
