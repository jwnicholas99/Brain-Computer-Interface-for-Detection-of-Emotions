using System;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections;

using BCILib;
using BCILib.Util;
using BCILib.App;
using BCILib.Amp;
using System.Collections.Generic;
using System.Linq;

namespace BCILib.P300
{
	/// <summary>
	/// Given a number, a sperate thread is created to generate ramdom output can call
	/// to notify the output.
	/// All numbers are taken before next round.
	/// </summary>
    /// 
	public class P300Manager
	{
		public ManualResetEvent EvtStopManager = new ManualResetEvent(false);

		// configuration parameters
		protected int _cfg_num_stim = 9; // number of stimuluses (EEG/NumEpochPerRound)
		private int _cfg_num_col = 3; // number of columns (P300Speller/GUIColumns)
		private int _cfg_near_chk = 3; // number of contineous stims to check for near neighbour. (P300Speller/NumForNearCheck)
		private int _cfg_ndc_from = 0; // stim to check near from (P300Speller/StimNearCheckFrom)
		private int _cfg_ndc_to = 0; // stim to check near to (P300Speller/StimNearCheckFrom)

		protected int _cfg_num_round = 8; // EEG/NumRound
		protected int _cfg_intens_ms = 120; // P300EEGSignal/Intensification
		protected int _cfg_non_intens_ms = 0; // P300EEGSignal/NonIntensification
		protected int _cfg_pause_bw_char = 0; // P300EEGSignal/PauseBwChar

		private int _cfg_char_indication = 4000; // P300Training/CharIndication
		private int _cfg_char_blank = 500; // P300Training/CharBlank

		protected int _cfg_flash_duration = 1; // P300EEGSignal/FlashDuration
		private int _cfg_norep_length = 1;  // P300EEGSignal/NoRepeatTime
		private char[] _cfg_speller_chars = null;

		protected int _stop_mode_stimcode = 0;

		Random rnd = new Random();

		private Thread _myThread;
		protected int _currentTask = 0; // 1 -- training, 2 -- testing

		protected short [] _last_codes = null;

		public int GetCharIndex(char c) {
			for (int i = 0; i < _cfg_speller_chars.Length; i++)
				if (c == _cfg_speller_chars[i]) return i;
			return -1;
		}

		public char GetBtnChar(int idx) {
			if (idx >= 0 && idx < _cfg_speller_chars.Length)
				return _cfg_speller_chars[idx];
			else return '\0';
		}

		protected short [] GenerateSequence() {
			return GenerateSequence(_cfg_num_stim);
		}

		protected short [] GenerateSequence(int length)
		{
			short [] seq = new short[length];
			short [] candidates = new short[length];
			short [] ind = new short[length];
			int tries = 0;

		agn:
			for (int i = 0; i < length; i++) ind[i] = -1;

			int si; // index to seq
			for (si = 0; si < length; si++) 
			{
				// get candidate set for si from ind
				int nc = 0;
				for (int c = 0; c < length; c++)
				{
					// already selected
					if (ind[c] != -1) continue;

					// in last_codes
					if (si < _cfg_norep_length && _last_codes != null) 
					{
						int l;
						for (l = si; l < _cfg_norep_length; l++)
						{
							if (c == _last_codes[l]) break;
						}
						if (l < _cfg_norep_length) continue;
					}

					// Distance between c and from previous "tn" button in the
					// generated seqence.
					bool too_near = false;
					for (int tn = 1; tn <= _cfg_near_chk; tn++) {
						int pc = si - tn;
						if (si >= tn && c >= _cfg_ndc_from && c <= _cfg_ndc_to && 
							pc >= _cfg_ndc_from && pc <= _cfg_ndc_to)
						{
							int dx = (c % _cfg_num_col) - (seq[pc] % _cfg_num_col);
							if (dx < 0) dx = -dx;

							int dy = (c / _cfg_num_col) - (seq[pc] / _cfg_num_col);
							if (dy < 0) dy = - dy;

							if (dx <= 1 && dy <= 1)
							{
								too_near = true;
								break;
							}
						}
					}
					if (tries < 200 && too_near) continue;

					// candicate is found
					candidates[nc++] = (short) c;
				}

				// nc: number of candates
				if (nc == 0) 
				{
					break;
				}
	
				if (nc == 1) seq[si] = candidates[0];
				else 
				{
					seq[si] = candidates[rnd.Next(nc)];
				}

				// mark as selected
				ind[seq[si]] = (short)si;
			}

			tries++;
			if (si == length)
			{
				//rewrite last_codes
				if (_last_codes == null) _last_codes = new short[_cfg_norep_length];
				for (int i = 0; i < _cfg_norep_length; i++) 
				{
					_last_codes[i] = seq[length - _cfg_norep_length + i];
				}
				return seq;
			}

			// Console.WriteLine("Failed. length={0}", si);
			goto agn;
		}

        P300ConfigCtrl _cfgctl = null;
        Func<int, double, bool> _hProcOut = null;

        public P300Manager(P300ConfigCtrl cfgctl)
        {
            _cfgctl = cfgctl;
            _hProcOut = new Func<int, double, bool>(ProcStimResult);
            proc.SetFeedbackHandler(_hProcOut);
        }

		~P300Manager()
		{
        }

		public void StartManager()
		{
			EvtStopManager.Reset();

			_myThread = new Thread(new ThreadStart(RunManager));
			_myThread.Start();
		}

		public void StartTrainer()
		{
			EvtStopManager.Reset();

			_myThread = new Thread(new ThreadStart(RunTrainer));
			_myThread.Start();
		}

		protected void ReadConfig()
		{
            if (_cfgctl == null) return;

            _cfgctl.Save();

			_cfg_num_stim = _cfgctl.NumEpochPerRound;
            _cfg_num_col = _cfgctl.GUIColumns;
            _cfg_num_round = _cfgctl.NumRound;
            _cfg_near_chk = _cfgctl.NumForNearCheck;
            _cfg_ndc_from = _cfgctl.StimNearCheckFrom;
            _cfg_ndc_to = _cfgctl.StimNearCheckTo;
            _cfg_pause_bw_char = _cfgctl.PauseBwChar;
            _cfg_intens_ms = _cfgctl.Intensification;
            _cfg_non_intens_ms = _cfgctl.NonIntensification;
            _cfg_flash_duration = _cfgctl.FlashDuration;

            int nrt = _cfgctl.NoRepeatTime;
			_cfg_norep_length = nrt / (_cfg_intens_ms + _cfg_non_intens_ms);
			if (_cfg_norep_length >= (_cfg_num_stim / 2) ) _cfg_norep_length = _cfg_num_stim / 2;

            _cfg_char_indication = _cfgctl.CharIndication;
            _cfg_char_blank = _cfgctl.CharBlank;

			_cfg_speller_chars = ResManager.StringToCharList(_cfgctl.SpellerChars);
			if (_cfg_speller_chars == null || _cfg_speller_chars.Length < _cfg_num_stim) {
				MessageBox.Show("Wong in speller chars! Please check configuration file.");
			}

			Console.WriteLine("P300 Manager: number of stim codes: {0}", _cfg_num_stim);
		}

        internal event Func<int, bool> StateChanged = null;

		protected void ThreadEndCallBack(object o)
		{
            if (StateChanged != null) {
                while (IsRunning) {
                    Thread.Sleep(100);
                }

                _myThread = null;

                StateChanged((int) o);
            }
		}

		protected virtual void RunManager()
		{
			_currentTask = 2;

            if (StateChanged != null) StateChanged(_currentTask);

			try {
				ExeManager();
			} catch (Exception ex) {
				Console.WriteLine("Exception: {0}", ex);
			}
            ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadEndCallBack), _currentTask);
            //EvtStopManager.Set();
            //_myTimer = new System.Threading.Timer(new TimerCallback(ThreadEndCallBack), null, 0, Timeout.Infinite);
			_currentTask = 0;
		}

        /// <summary>
        /// Check if model files defined in system configuration file
        /// </summary>
        /// <returns>true if all necessary model files are found, otherwise false.</returns>
		protected bool CheckForTest() {
			try {
				ResManager cfg = _cfgctl.Resource;
				string rn = "Process";
				string pfn = cfg.GetConfigValue(rn, "P300ParaFile");;
				if (pfn == null) pfn = cfg.GetConfigValue(rn, "ParameterFile");
				if (pfn == null) {
					throw new Exception("Model file error: P300ParaFile=" + pfn);
				}

				string [] vals = {"SVMModel", "SVMRange"}; //"P300ParaFile",
				foreach (string v in vals) {
					string fn = cfg.GetConfigValue(rn, v);
					if (fn == null || !File.Exists(fn)) {
						throw new Exception("Model file error: " + v + "=" + fn);
					}
				}

			} catch (Exception e) {
				MessageBox.Show(e.Message);
				return false;
			}

			return true;
		}

		// delegate for epcoh result
		public event Func<int, double, bool> P300Output = null;
        public event Action<int, WaitHandle> PrepareProgress = null;
        public event Action<int, ButtonState> SetButtonState = null;

        Queue<int> _que_cmd = new Queue<int>();
        Queue<double> _que_score = new Queue<double>();

		private bool ProcStimResult(int stim, double score) {
            _que_score.Enqueue(score);
            _que_cmd.Enqueue(stim);

            return true;
		}

        P300Processor proc = new P300Processor();

		protected virtual void ExeManager()
		{
			if (!CheckForTest()) return;

			ReadConfig();
			_last_codes = null;

            if (AmpContainer.Count <= 0) {
                MessageBox.Show("Amplifer not defined!");
            }

            AmpContainer.StartAll();

            if (!proc.Initialize(_cfgctl)) {
                MessageBox.Show("Cannot initialize processer!");
                return;
            }

            proc.SetAmplifier(AmpContainer.GetAmplifier(0), _cfgctl.P300UsedChannels);
            proc.SetReadingCodes(_cfgctl.PreStimDuration * AmpContainer.GetAmplifier(0).header.samplingrate / 1000);

            //proc.SetReadingEvtOffsetMs();
			proc.StartEventProcessing();

            TestDirSpecForm tsf = new TestDirSpecForm();
            ResManager rm = _cfgctl.Resource;
            string trname = TestDirSpecForm.GetProcPath(rm, false);

            trname = string.Format("{0}\\{1:yyyy-MMM-dd-HH-mm-ss}.log", trname, DateTime.Now);
            Console.WriteLine("Logfile = {0}", trname);
            var cfile = ConsoleCapture.StartConsoleFile(trname);

			// indicate to start
			//_guiForm.IndicateStarting(_cfg_pause_bw_char, 1);

            _que_score.Clear();
            _que_cmd.Clear();

			long tc0 = HRTimer.GetTimestamp();
			while (!EvtStopManager.WaitOne(0, false))
			{
                if (!AmpContainer.AllAlive) {
                    Console.WriteLine("Error: processor stoped!");
                    EvtStopManager.Set();
                    break;
                }

                short[] ufs = new short[_cfg_flash_duration];
				for (int i = 0; i < _cfg_flash_duration; i++) ufs[i] = -1;

				//for (int r = 0; r < _cfg_num_round; r++) 
				{
					// generate sequence
					short [] seq = GenerateSequence();
                    //Console.WriteLine("Testing: {0}", string.Join(",", seq.Select(x => (x + 1).ToString()).ToArray()));
					for (int i = 0; i < seq.Length; i++) 
					{
						long dt0 = HRTimer.GetTimestamp();
                        if (SetButtonState != null) SetButtonState(seq[i], ButtonState.Highlight);
						for (int f = _cfg_flash_duration - 1; f > 0; f--) ufs[f] = ufs[f - 1];
						ufs[0] = seq[i];
						AmpContainer.SendAll(seq[i] + 1);

						int wt = (int) (_cfg_intens_ms - HRTimer.DeltaMilliseconds(dt0));
						if (wt < 5) wt = 5;
						EvtStopManager.WaitOne(wt, false);

                        if (ufs[_cfg_flash_duration - 1] != -1) {
                            SetButtonState(ufs[_cfg_flash_duration - 1], ButtonState.Normal);
                        }
                        if (EvtStopManager.WaitOne(_cfg_non_intens_ms, false)) break;

                        if (_que_cmd.Count > 0) {
                            if (P300Output != null) {
                                P300Output(_que_cmd.Dequeue(), _que_score.Dequeue());
                            }
                        }
                    }
				}

				for (int i = _cfg_flash_duration - 2; i >= 0; i--)
				{
					EvtStopManager.WaitOne(_cfg_intens_ms + _cfg_non_intens_ms, false);
					if (ufs[i] != -1) SetButtonState(ufs[i], ButtonState.Normal);

                    if (_que_cmd.Count > 0) {
                        if (P300Output != null) {
                            P300Output(_que_cmd.Dequeue(), _que_score.Dequeue());
                        }
                    }
                }
			}

            proc.StopEventProcessing();
		}

		// called by main thread
		public void StopManager()
		{
			if (EvtStopManager != null) 
			{
				EvtStopManager.Set();
			}
		}

		public bool IsRunning
		{
			get
			{
				return (_myThread != null && _myThread.IsAlive);
			}
		}

		private void RunTrainer()
		{
			_currentTask = 1;
            if (StateChanged != null) {
                StateChanged(_currentTask);
            }
			try {
                SetButtonState(0, ButtonState.Clear);
                if (string.IsNullOrEmpty(_cfgctl.TrainModelFileList)) ExeTrainer(1);
                if (string.IsNullOrEmpty(_cfgctl.TrainRejectFileList)) ExeTrainer(2);
			} 
			catch (Exception ex) {
                MessageBox.Show("Error: " + ex.Message + "\nOpen console to check details.");
				Console.WriteLine("ExeTrainer: exception = {0}", ex);
			}

			EvtStopManager.Set();
            ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadEndCallBack), (Int32) _currentTask);
            //_myTimer = new System.Threading.Timer(new TimerCallback(ThreadEndCallBack), null, 0, Timeout.Infinite);
			_currentTask = 0;
		}

        /// <summary>
        /// Collecting EEG data for model training.
        /// </summary>
        /// <param name="model_id">1: classification model, 2: rejection model</param>
		private void ExeTrainer(int model_id) {
			ResManager cfg = _cfgctl.Resource;
			TrainDirSpecForm tsf = new TrainDirSpecForm();

			string trname = tsf.ChkTrainingPath(cfg, true);
			if (trname == null) return;

            // Get current amplifier
            if (AmpContainer.Count != 1) {
                MessageBox.Show("Please define amplifier!");
                return;
            }

            Amplifier amp = AmpContainer.GetAmplifier(0);
            if (!amp.IsAlive && !amp.Start()) {
                MessageBox.Show("Cannot start amplifer!");
            }

			var cfile = ConsoleCapture.StartConsoleFile(Path.Combine(trname, "training.log"));

			// Read configuration
			ReadConfig();
			_last_codes = null;
            string time_fg = "_" + BCIApplication.TimeStamp;

			// Read trainning words
            List<String> words = new List<string>();
            try {
				StreamReader sr = new StreamReader("Config/Training.txt");
				string line;
				int num = 0, k = 0;
				while ((line = sr.ReadLine()) != null) {
					if (line.StartsWith("NumWords ")) {
						num = int.Parse(line.Substring(9));
					} 
					else if (line.StartsWith("StartWord ")) {
						int s = int.Parse(line.Substring(10));
						while (--s > 0) sr.ReadLine();
					} 
					else if (k < num) {
						words.Add(line.Trim());
						k++;
					}
				}
				sr.Close();
			} 
			catch (Exception e) {
				MessageBox.Show(e.Message);
				EvtStopManager.Set();
			}

			// Initialize parallel port
			AmpContainer.SendAll(0);

            List<int> rcodes = new List<int>();
            int rc = 0;
            List<int> rext = new List<int>();
            Action<int, int> hevt = (code, pos) =>
            {
                if (code <= 0 || code > _cfg_num_stim) return;

                int ridx = rcodes.IndexOf(code);
                if (ridx >= 0) {
                    rext.Add(code);
                } else {
                    rcodes.Add(code);
                }
                if (rcodes.Count >= _cfg_num_stim) {
                    rc++;
                    Console.Write(rc);
                    rcodes.Clear();
                    if (rext.Count > 0) {
                        Console.WriteLine("extra codes={0}", string.Join(",", rext.Select(x => x.ToString()).ToArray()));
                        rext.Clear();
                    }
                }
            };

            amp.evt_stim_received += hevt;

			foreach (string wd in words) {
				if (EvtStopManager.WaitOne(0, false)) break;
				//_guiForm.TrainWord(wd);
				if (EvtStopManager.WaitOne(0, false)) break;

                //// pause between words;
                //if (!trainer.StartTraining(wd, 
                //    wd.Length * _cfg_num_round * _cfg_num_stim)) {
                //    MessageBox.Show("Error in trainer.Start!");
                //    break;
                //}
                AmpContainer.StartRecord(trname, wd + time_fg); // d.Length * _cfg_num_round * _cfg_num_stim

				long tc0;
				foreach (char c in wd.ToCharArray()) {
					// Display C
					int cv = GetCharIndex(c);
					if (cv < 0 || cv >= _cfg_num_stim) {
						MessageBox.Show(string.Format("Character {0} not defined! Please check training configuration file.", c));
						EvtStopManager.Set();
						break;
					}

                    if (SetButtonState != null) {
                        SetButtonState(cv, ButtonState.Cue);
                    }
					EvtStopManager.WaitOne(_cfg_char_indication, false);

                    if (PrepareProgress != null) {
                        PrepareProgress(_cfg_pause_bw_char, EvtStopManager);
                    }
                    if (SetButtonState != null) {
                        SetButtonState(cv, ButtonState.Normal);
                    }
					if (EvtStopManager.WaitOne(_cfg_char_blank, false)) break;

					tc0 = HRTimer.GetTimestamp();

					// chracters to be unhighted.
					short [] ufs = new short[_cfg_flash_duration];
					for (int i = 0; i < _cfg_flash_duration; i++) ufs[i] = -1;

                    rc = 0;
                    if (rcodes.Count > 0) {
                        Console.WriteLine("Cleared events: {0}", string.Join(",", rcodes.Select(x => x.ToString()).ToArray()));
                        rcodes.Clear();
                    }
                    if (rext.Count > 0) {
                        Console.WriteLine("Cleared extra: {0}", string.Join(",", rext.Select(x => x.ToString()).ToArray()));
                        rext.Clear();
                    }

					// Send epoch
					while (rc < _cfg_num_round) {
                        if (!AmpContainer.AllAlive) {
                            EvtStopManager.Set();
                            MessageBox.Show("Amplifier stopped!");
                            break;
                        }

						short [] seq = GenerateSequence();

						for (int i = 0; i < seq.Length && rc < _cfg_num_round; i++) {
							if (!AmpContainer.AllAlive) {
								MessageBox.Show("Amplifier stoped.");
								EvtStopManager.Set();
							}

							long dt1 = HRTimer.GetTimestamp();
							SetButtonState(seq[i], ButtonState.Highlight);
							for (int f = _cfg_flash_duration - 1; f > 0; f--)
								ufs[f] = ufs[f - 1];
							ufs[0] = seq[i];

							AmpContainer.SendAll((short) (seq[i] + 1));

							int wt = (int) (_cfg_intens_ms - HRTimer.DeltaMilliseconds(dt1));
							if (wt < 5) wt = 5;
							EvtStopManager.WaitOne(wt, false);

                            if (ufs[_cfg_flash_duration - 1] != -1) {
                                SetButtonState(ufs[_cfg_flash_duration - 1], ButtonState.Normal);
                            }
							if (EvtStopManager.WaitOne(_cfg_non_intens_ms, false)) break;

							//Console.WriteLine("Time used for {0} is: {1}", seq[i] + 1, HRTimer.DeltaMilliseconds(dt1));
						}
						if (EvtStopManager.WaitOne(0, false)) break;
					}

					for (int i = _cfg_flash_duration - 2; i >= 0; i--) {
						EvtStopManager.WaitOne(
							_cfg_intens_ms + _cfg_non_intens_ms, false);
						if (ufs[i] != -1) SetButtonState(ufs[i], ButtonState.Normal);
					}

					if (EvtStopManager.WaitOne(0, false)) break;

					SetButtonState(GetCharIndex(c), ButtonState.Result);

					Console.WriteLine("Character time = {0}, suppose to be:{1}", 
						HRTimer.DeltaMilliseconds(tc0), (_cfg_non_intens_ms + _cfg_intens_ms) * _cfg_num_round * _cfg_num_stim);
				} // end of chars

                if (rcodes.Count > 0) {
                    Console.WriteLine("Cleared events: {0}", string.Join(",", rcodes.Select(x => x.ToString()).ToArray()));
                    rcodes.Clear();
                }
                if (rext.Count > 0) {
                    Console.WriteLine("Cleared extra: {0}", string.Join(",", rext.Select(x => x.ToString()).ToArray()));
                    rext.Clear();
                }

                // make sure all stim codes received?
                Thread.Sleep(2000);
                AmpContainer.StopRecord();

                // modify CNT to remove extra stimcodes
                if (!EvtStopManager.WaitOne(0, false)) {
                    foreach (var cfn in Directory.GetFiles(trname, wd + time_fg + "*.cnt")) {
                        if (!VerifyCntTrain(cfn, _cfg_num_stim, _cfg_num_round, wd.Length)) {
                            Console.WriteLine("Verify EEG file error!");
                        }
                    }
                }

            } // end of words

            amp.evt_stim_received -= hevt;

			cfile.EndLogFile();

			if (!EvtStopManager.WaitOne(0, false)) {
				// traning program successfully finished

				// Add directory to traininglist
				// ConcentrationTrainModel.AddTrainingDir(trname, 0);

				// set config file
                string tf = BuildP300TrainingDescFile(trname, _cfgctl.SpellerChars);
				if (tf != null) {
					// set training
					if (model_id == 1) {
						_cfgctl.TrainModelFileList = tf;
					} else {
                        _cfgctl.TrainRejectFileList = tf;
					}
                    _cfgctl.Save();
				}

				// copy configuration file into training directory
				string to = trname + "\\Config";
				if (!Directory.Exists(to)) Directory.CreateDirectory(to);
				BCIApplication.CopyFiles("Config", to);
				MessageBox.Show("Training process finished.");
			}
            SetButtonState(0, ButtonState.NewLine);
		}


		/// <summary>
		/// Get the task the manager is current doing: 0: nothing, 1: training; 2: testing
		/// </summary>
		public int CurrentTask
		{
			get
			{
				return _currentTask;
			}
		}

		public int CharFlashTime {
			get {
				return _cfg_intens_ms;
			}
		}

		public int CharIndicationTime {
			get {
				return _cfg_char_indication;
			}
		}

		public int NumberOfStim {
			get {
				return _cfg_num_stim;
			}
		}

		public int NumberOfRound {
			get {
				return _cfg_num_round;
			}
		}

        static public string BuildP300TrainingDescFile(string dir, string spl_chars)
        {
            if (!Directory.Exists(dir)) return null;

            //get all dat files
            string[] dats = Directory.GetFiles(dir, "*.cnt");
            if (dats == null || dats.Length == 0) {
                Console.WriteLine("Build P300 Training Description File Error: no cnt file in the directory {0}!", dir);
                return null;
            }
            else {
                int n = dats.Length;
                DateTime[] dtl = new DateTime[n];
                for (int i = 0; i < n; i++) dtl[i] = new FileInfo(dats[i]).LastWriteTime;
                Array.Sort(dtl, dats);
            }

            string txtfile = dir + ".txt";
            Console.WriteLine("Build file: {0}", txtfile);
            using (TextWriter tw = File.CreateText(txtfile)) {
                tw.WriteLine(@"TrainDir: {0}\", Path.GetFullPath(dir));
                tw.WriteLine();

                string str = Path.GetFileName(dir);
                int i0 = str.IndexOf("_20");
                if (i0 > 0) str = str.Substring(0, i0);
                tw.WriteLine("Trainer: {0}", str);
                tw.WriteLine();

                tw.WriteLine("Number of Files: {0}", dats.Length);
                foreach (string fn in dats) {
                    string file = fn;
                    if (file.StartsWith(dir)) file = file.Substring(dir.Length + 1);
                    tw.WriteLine(file);
                }
                tw.WriteLine();
                Console.WriteLine("All cnt files:{0}", dats.Length);

                tw.WriteLine("Number of Words: {0}", dats.Length);
                foreach (string fn in dats) {
                    string file = fn;
                    if (file.StartsWith(dir)) file = file.Substring(dir.Length + 1);
                    int p = 0;
                    while (spl_chars.IndexOf(file[p]) >= 0) p++;
                    tw.WriteLine("\t{0}", file.Substring(0, p));
                }
            }

            return txtfile;
        }

        internal static bool VerifyCntTrain(string fn, int nstim, int nr, int nc)
        {
            EEGCntFile cntf = new EEGCntFile();
            cntf.ReadCnt(fn);
            int[] rcodes = new int[nstim];
            int[] ridx = new int[nstim];
            int si = 0, slen = 0;

            int[] stim_code = (int[]) cntf.stim_code.Clone();
            int ci = 0;
            for (; ci < nc && si < stim_code.Length; ci++) {
                int ri = 0;
                while (ri < nr && si < stim_code.Length) {
                    for (int i = 0; i < nstim; i++) {
                        rcodes[i] = 0;
                    }
                    while (Array.IndexOf(rcodes, 0) >= 0 && si < stim_code.Length) {
                        int stim = stim_code[si];
                        if (stim < 1 || stim > nstim) {
                            stim_code[si] = 0;
                        } else {
                            if (rcodes[stim - 1] > 0) {
                                // remove this stim code
                                stim_code[ridx[stim - 1]] = 0;
                            }
                            rcodes[stim - 1]++;
                            ridx[stim - 1] = si;
                            slen = cntf.NumSamples - cntf.stim_pos[si];
                            if (si + 1 < stim_code.Length) slen = cntf.stim_pos[si + 1] - cntf.stim_pos[si];
                        }
                        si++;
                    }
                    if (Array.IndexOf(rcodes, 0) < 0) ri++;
                }
                if (ri < nr) {
                    return false;
                }

                // ignor rest of the stim code with length < 1s
                int nskip = 0;
                while (si < stim_code.Length && slen < cntf.Amp_Info.sampling_rate) {
                    slen = cntf.NumSamples - cntf.stim_pos[si];
                    if (si + 1 < stim_code.Length) {
                        slen = cntf.stim_pos[si + 1] - cntf.stim_pos[si];
                    }
                    stim_code[si] = 0;
                    si++;
                    nskip++;
                }
                if (nskip > 0) {
                    Console.WriteLine("Skepted codes = {0}", nskip);
                }
            }
            if (ci < nc) {
                return false;
            }

            while (si < stim_code.Length) {
                stim_code[si] = 0;
            }

            if (Array.IndexOf(stim_code, 0) < 0) {
                return true;
            }

            // remove extra stim code;
            using (FileStream fs = File.Open(fn, FileMode.Open, FileAccess.ReadWrite)) {
                BinaryReader br = new BinaryReader(fs);
                BinaryWriter bw = new BinaryWriter(fs);

                fs.Seek(24, SeekOrigin.Begin);
                si = 0;
                while (si < stim_code.Length) {
                    fs.Seek(cntf.Amp_Info.num_chan * 4, SeekOrigin.Current);
                    int evt = br.ReadInt32();
                    evt &= 0xFF;
                    if (evt > 0) {
                        if (stim_code[si] != evt) {
                            fs.Seek(-4, SeekOrigin.Current);
                            bw.Write(0);
                        }
                        si++;
                    }
                }
            }

            return true;
        }
    }
}
