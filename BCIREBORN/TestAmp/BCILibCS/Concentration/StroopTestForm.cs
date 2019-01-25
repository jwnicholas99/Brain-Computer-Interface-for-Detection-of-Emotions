using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using System.IO;

using BCILib.Util;
using BCILib.App;
using BCILib.Amp;

namespace BCILib.Concentration
{
	/// <summary>
	/// Summary description for ConcentrationStroopTest.
	/// </summary>
	public class StroopTestForm : System.Windows.Forms.Form
	{
        private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label labelColorCue;
        private System.Windows.Forms.Label labelLevel;
		private System.Windows.Forms.Label labelLeftArrow;
		private System.Windows.Forms.Label labelRightArrow;
		private System.Windows.Forms.Label labelLeftColName;
		private System.Windows.Forms.Label labelRightColName;
		private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.Label labelTimer;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Label labelResult;
		private System.Windows.Forms.Label labelSession;
		private System.Windows.Forms.Label labelTrial;
        private Label labelExplanation;
        private Label labelHint;
        private Label labelInstruction;

		public StroopTestForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			wevts = new WaitHandle[] {evt_proc_stop, evt_key_pressed};
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			evt_proc_stop.Set();
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StroopTestForm));
            this.label9 = new System.Windows.Forms.Label();
            this.labelColorCue = new System.Windows.Forms.Label();
            this.labelLevel = new System.Windows.Forms.Label();
            this.labelLeftArrow = new System.Windows.Forms.Label();
            this.labelRightArrow = new System.Windows.Forms.Label();
            this.labelLeftColName = new System.Windows.Forms.Label();
            this.labelRightColName = new System.Windows.Forms.Label();
            this.buttonStop = new System.Windows.Forms.Button();
            this.labelTimer = new System.Windows.Forms.Label();
            this.labelResult = new System.Windows.Forms.Label();
            this.labelSession = new System.Windows.Forms.Label();
            this.labelTrial = new System.Windows.Forms.Label();
            this.labelInstruction = new System.Windows.Forms.Label();
            this.labelExplanation = new System.Windows.Forms.Label();
            this.labelHint = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(255)))));
            this.label9.Location = new System.Drawing.Point(40, 543);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(829, 10);
            this.label9.TabIndex = 21;
            // 
            // labelColorCue
            // 
            this.labelColorCue.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelColorCue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(255)))));
            this.labelColorCue.Font = new System.Drawing.Font("Arial", 72F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelColorCue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(204)))), ((int)(((byte)(0)))));
            this.labelColorCue.Location = new System.Drawing.Point(142, 181);
            this.labelColorCue.Name = "labelColorCue";
            this.labelColorCue.Size = new System.Drawing.Size(621, 204);
            this.labelColorCue.TabIndex = 17;
            this.labelColorCue.Text = "RED";
            this.labelColorCue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelLevel
            // 
            this.labelLevel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelLevel.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelLevel.Location = new System.Drawing.Point(33, 13);
            this.labelLevel.Name = "labelLevel";
            this.labelLevel.Size = new System.Drawing.Size(815, 38);
            this.labelLevel.TabIndex = 12;
            this.labelLevel.Text = "Patient Calibration";
            // 
            // labelLeftArrow
            // 
            this.labelLeftArrow.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.labelLeftArrow.Font = new System.Drawing.Font("Symbol", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.labelLeftArrow.Location = new System.Drawing.Point(308, 407);
            this.labelLeftArrow.Name = "labelLeftArrow";
            this.labelLeftArrow.Size = new System.Drawing.Size(83, 24);
            this.labelLeftArrow.TabIndex = 18;
            this.labelLeftArrow.Text = "<";
            this.labelLeftArrow.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelRightArrow
            // 
            this.labelRightArrow.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.labelRightArrow.Font = new System.Drawing.Font("Symbol", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.labelRightArrow.Location = new System.Drawing.Point(495, 407);
            this.labelRightArrow.Name = "labelRightArrow";
            this.labelRightArrow.Size = new System.Drawing.Size(75, 24);
            this.labelRightArrow.TabIndex = 13;
            this.labelRightArrow.Text = ">";
            this.labelRightArrow.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelLeftColName
            // 
            this.labelLeftColName.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.labelLeftColName.BackColor = System.Drawing.Color.Transparent;
            this.labelLeftColName.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelLeftColName.Location = new System.Drawing.Point(308, 393);
            this.labelLeftColName.Name = "labelLeftColName";
            this.labelLeftColName.Size = new System.Drawing.Size(83, 24);
            this.labelLeftColName.TabIndex = 16;
            this.labelLeftColName.Text = "Green";
            this.labelLeftColName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelRightColName
            // 
            this.labelRightColName.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.labelRightColName.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelRightColName.Location = new System.Drawing.Point(495, 393);
            this.labelRightColName.Name = "labelRightColName";
            this.labelRightColName.Size = new System.Drawing.Size(75, 24);
            this.labelRightColName.TabIndex = 15;
            this.labelRightColName.Text = "Red";
            this.labelRightColName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonStop
            // 
            this.buttonStop.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonStop.Enabled = false;
            this.buttonStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonStop.Location = new System.Drawing.Point(415, 599);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(75, 32);
            this.buttonStop.TabIndex = 11;
            this.buttonStop.TabStop = false;
            this.buttonStop.Text = "Stop";
            this.buttonStop.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.buttonStop_PreviewKeyDown);
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // labelTimer
            // 
            this.labelTimer.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.labelTimer.BackColor = System.Drawing.Color.Transparent;
            this.labelTimer.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTimer.ForeColor = System.Drawing.Color.Blue;
            this.labelTimer.Location = new System.Drawing.Point(808, 351);
            this.labelTimer.Name = "labelTimer";
            this.labelTimer.Size = new System.Drawing.Size(40, 40);
            this.labelTimer.TabIndex = 14;
            this.labelTimer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelResult
            // 
            this.labelResult.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.labelResult.BackColor = System.Drawing.Color.Transparent;
            this.labelResult.Font = new System.Drawing.Font("Symbol", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.labelResult.Location = new System.Drawing.Point(573, 392);
            this.labelResult.Name = "labelResult";
            this.labelResult.Size = new System.Drawing.Size(43, 24);
            this.labelResult.TabIndex = 22;
            this.labelResult.Text = "?";
            this.labelResult.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelSession
            // 
            this.labelSession.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.labelSession.BackColor = System.Drawing.Color.Transparent;
            this.labelSession.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSession.Location = new System.Drawing.Point(59, 613);
            this.labelSession.Name = "labelSession";
            this.labelSession.Size = new System.Drawing.Size(72, 24);
            this.labelSession.TabIndex = 23;
            this.labelSession.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelTrial
            // 
            this.labelTrial.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.labelTrial.BackColor = System.Drawing.Color.Transparent;
            this.labelTrial.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTrial.Location = new System.Drawing.Point(139, 613);
            this.labelTrial.Name = "labelTrial";
            this.labelTrial.Size = new System.Drawing.Size(80, 24);
            this.labelTrial.TabIndex = 24;
            this.labelTrial.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelInstruction
            // 
            this.labelInstruction.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelInstruction.Location = new System.Drawing.Point(40, 69);
            this.labelInstruction.Name = "labelInstruction";
            this.labelInstruction.Size = new System.Drawing.Size(829, 199);
            this.labelInstruction.TabIndex = 25;
            this.labelInstruction.Text = "labelInstruction";
            // 
            // labelExplanation
            // 
            this.labelExplanation.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelExplanation.Location = new System.Drawing.Point(40, 432);
            this.labelExplanation.Name = "labelExplanation";
            this.labelExplanation.Size = new System.Drawing.Size(829, 106);
            this.labelExplanation.TabIndex = 25;
            this.labelExplanation.Text = "labelExplanation";
            // 
            // labelHint
            // 
            this.labelHint.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelHint.Location = new System.Drawing.Point(40, 563);
            this.labelHint.Name = "labelHint";
            this.labelHint.Size = new System.Drawing.Size(829, 33);
            this.labelHint.TabIndex = 25;
            this.labelHint.Text = "labelHint";
            // 
            // StroopTestForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(904, 662);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.labelTimer);
            this.Controls.Add(this.labelRightColName);
            this.Controls.Add(this.labelRightArrow);
            this.Controls.Add(this.labelLeftColName);
            this.Controls.Add(this.labelTrial);
            this.Controls.Add(this.labelSession);
            this.Controls.Add(this.labelResult);
            this.Controls.Add(this.labelLevel);
            this.Controls.Add(this.labelLeftArrow);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.labelColorCue);
            this.Controls.Add(this.labelInstruction);
            this.Controls.Add(this.labelHint);
            this.Controls.Add(this.labelExplanation);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "StroopTestForm";
            this.Text = "Patient Calibration";
            this.Load += new System.EventHandler(this.ConcentrationStroopTest_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.StroopTestForm_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.StroopTestForm_KeyDown);
            this.ResumeLayout(false);

		}
		#endregion

		string[] img_list = null;
		private void LoadRelaxImages() {
			// Find image list
			string rdir = Path.Combine(BCIApplication.RootPath, "Images-Relax");
            if (Directory.Exists(rdir)) {
                img_list = Directory.GetFiles(rdir, "*.jpg");
            }
		}

		private void buttonStart_Click(object sender, System.EventArgs e) {
            //textInstruction.Text = null;
            //textHint.Text = null;
            //buttonStart.Enabled = false;
            //buttonStop.Enabled = false;
            //MessageBox.Show("Calibration finished.", "Stroop Test");
            //buttonStart.Enabled = false;
            //buttonStop.Enabled = true;
            //return;

            StartCalibration();
        }

        private void StartCalibration() {
            labelExplanation.Text = null;
			//LoadRelaxImages();
			evt_proc_stop.Reset();
			Thread thd = new Thread(new ThreadStart(RunRecording));
			SetDCRunning(true);
            thd.Start();
		}

		ManualResetEvent evt_key_pressed = new ManualResetEvent(false);
		ManualResetEvent evt_proc_stop = new ManualResetEvent(false);
		WaitHandle[] wevts;

		private int required_key = 0;
		private Color[] color_values = {Color.White, Color.Red, Color.FromArgb(0x99, 0x66, 0), Color.FromArgb(0, 0xCC, 0)};
        private string[] color_names = {"White", "Red", "Brown", "Green"};

		private int[] cfg_level_seq = null;
        private int cfg_beeptime = 3000;
		private int cfg_timecue = 1000;
		private int cfg_wtimeout = 10000;
		private int cfg_timerest = 0;
		private int cfg_timerelax = 6000;
		private int cfg_ntrials = 20;
		private int cfg_relaximage = 0;

        //added 2010.05.02
        private int cfg_calib_threshold = 90;

		private void LoadCfg() {
			LoadCfg(BCIApplication.SysResource);
		}

		private void LoadCfg(ResManager rm) {
			string rn = "StroopTest";
            //string cfg_colors = "White,Red,Green,Brown";

            rm.GetConfigValue(rn, "BeepTiom", ref cfg_beeptime);
			rm.GetConfigValue(rn, "TimeCue", ref cfg_timecue);
			rm.GetConfigValue(rn, "TimeoutWait", ref cfg_wtimeout);
			rm.GetConfigValue(rn, "TimeRest", ref cfg_timerest);
			rm.GetConfigValue(rn, "TimeRelax", ref cfg_timerelax);
			rm.GetConfigValue(rn, "NumTrials", ref cfg_ntrials);
            //rm.GetConfigValue(rn, "ColorList", ref cfg_colors);
			rm.GetConfigValue(rn, "RelaxDispImage", ref cfg_relaximage);
            rm.GetConfigValue(rn, "AcceptScore", ref cfg_calib_threshold);

			string strSeqence = "3,3";
			rm.GetConfigValue(rn,"TestSessions", ref strSeqence);
			string[] slist = strSeqence.Split(',', ' ', '\t');
			ArrayList al = new ArrayList();
			bool bchanged = false;
			foreach (string si in slist) {
				int level = 0;
				try {
					level = int.Parse(si);
				} catch(Exception) {
					bchanged = true;
					continue;
				}

				if (level < 1 || level > 3) {
					bchanged = true;
					continue;
				}

				al.Add(level);
			}
			cfg_level_seq = (int[]) al.ToArray(typeof(int));

			if (bchanged) {
				rm.SetConfigValue(rn, "TestSessions", strSeqence);
			}

            //al.Clear();
            //slist = cfg_colors.Split(',');
            //bchanged = false;
            //foreach (string si in slist) {
            //    try {
            //        Color clr = Color.FromName(si.Trim());
            //        al.Add(clr);
            //    } catch (Exception) {
            //        bchanged = true;
            //    }
            //}

            //colors = (Color[]) al.ToArray(typeof(Color));

			rm.SetConfigValue("EEG", "AppName", "StroopTest");
			rm.SetConfigValue("EEG", "TrainDataDir", "DataTrain_StroopTest");

			rm.SaveIfChanged();
		}

		private bool bDryRun = false;

		public bool DryRun {
			set {
				bDryRun = value;
			}
			get {
				return bDryRun;
			}
		}

		private void SetDCRunning(bool running) {
            if (this.InvokeRequired) {
                this.Invoke((Action) delegate()
                {
                    SetDCRunning(running);
                });
            }
            else {
                //buttonStart.Enabled = !running;
                buttonStop.Enabled = running;
                this.ControlBox = !running;
            }
		}

		private void InitStroopTest() {
            //if (InvokeRequired) {
            //    Invoke((Action)delegate
            //    {
            //        InitStroopTest();
            //    });
            //}
            //else {
            //    labelLevel.Text = //"Stroop Test for Concentration Data Collection";
            //        "Patient Calibration";
            //    textInstruction.Visible = true;
            //    textInstruction.Text = //"You are instructed to go through a number of levels of concentration/relaxing experiments.\r\nThe EEG data will be collected for analysis.";
            //        "You are instructed to go through a number of sessions of concentration/relaxing experiments.\r\n"
            //        + "EEG data will be collected for analysis.";
            //    labelColorCue.Visible = false;
            //    labelLeftArrow.Visible = labelRightArrow.Visible = false;
            //    labelLeftColName.Text = labelRightColName.Text = null;
            //    textHint.Text = //"Click Start to start experiment.";
            //        "Click Start to start calibration.";
            //    labelTimer.Text = null;
            //}
		}

		private void RunRecording() {
			try {
				ExeRecording();

                if (!evt_proc_stop.WaitOne(0, false)) {
                    MessageBox.Show("Calibration finished.", "Calibration");
                    this.BeginInvoke((Action)delegate() {
                        this.Close();
                    });
                    return;
                }
            }
            catch (Exception ex) {
				MessageBox.Show(string.Format("RunRecording: Exception = {0}", ex));
			}
			SetDCRunning(false);
            DisplayMCQInstruction();
		}

        private void ClearTraining(string trname, string fn_prefix, bool DeleteDir)
        {
            if (!Directory.Exists(trname)) return;

            // find cnt file and delete it
            string[] fn_list = Directory.GetFiles(trname, fn_prefix + "*.*");
            foreach (string fn in fn_list) {
                BCIApplication.LogMessage("Delete  file {0} ...", fn);
                try {
                    File.Delete(fn);
                }
                catch (Exception) {
                    BCIApplication.LogMessage("Error to delete file {0}!", fn);
                }
            }

            if (DeleteDir) {
                try {
                    Directory.Delete(trname);
                    BCIApplication.LogMessage("Directory deleted: {0}.", trname);
                }
                catch (Exception) {
                    BCIApplication.LogMessage("Error to delete dir {0}!", trname);
                }
            }
        }

        private int ans_correct = 0;

        private void DisplayCalibInstruction()
        {
            labelInstruction.Text = //"The calibration process includes 20 trials of attention and relaxation periods.\r\n\r\n"
               "During the calibration period, a word will be displayed in a color different from the color it means. "
                + "The names of two colors will appear below the word. "
                + "Use the arrow keys to choose the word that matched the color that the word is display in.\r\n\r\n";
                //+ "During the relax period, move your eyes around to relax yourself; do not focus on a particular point.";

            //labelColorCue.Visible = 
            labelLeftColName.Visible = labelRightColName.Visible
                = labelResult.Visible = labelLeftArrow.Visible = labelRightArrow.Visible = false;

            labelExplanation.Text = "To ensure you remain fully focused to achieve the best result, please follow the advices given below.\r\n"
                + "    -    Avoid unnecessary body movement;\r\n"
                + "    -    Do not speak;\r\n"
                + "    -    Concentrate to decide on the colour in which the word is displayed in and press the corresponding arrow key as quick as possible.";

            labelHint.Text = "Press Enter to continue.";

            required_key = 5;
        }

        private void DisplayMCQInstruction()
        {
            if (InvokeRequired) {
                this.Invoke((Action)delegate() {
                    DisplayMCQInstruction();
                });
                return;
            }

            labelLevel.Text = "Practice for Calibration";
            labelInstruction.Visible = true;
            labelInstruction.Text = //"In this calibration you are required to name the colour which word is displayed in, not what the word means.";
               "During the concentration, a word will be displayed in a color different from the color it means. "
                + "The names of two colors will appear below the word. "
                + "Use the arrow keys to choose the color name that matched the color that the word is displayed in.\r\n\r\n";

            Random rnd = new Random();
            int n = color_names.Length;
            int c0 = rnd.Next(n);
            int c1 = rnd.Next(n - 1);
            if (c1 == c0) c1++;

            labelColorCue.BackColor = Color.FromArgb(51, 51, 255);
            labelColorCue.Visible = labelLeftColName.Visible = labelRightColName.Visible
                = labelResult.Visible = labelLeftArrow.Visible = labelRightArrow.Visible = true;

            labelColorCue.Text = color_names[c0];
            labelColorCue.ForeColor = color_values[c1]; // should be the correct color
            ans_correct = rnd.Next(2);
            if (ans_correct == 0) {
                labelLeftColName.Text = color_names[c1];
                labelRightColName.Text = color_names[c0];
            }
            else {
                labelLeftColName.Text = color_names[c0];
                labelRightColName.Text = color_names[c1];
            }

            //labelExplanation.Text = "In the above example, a word \"" + color_names[c0] + "\" is displayed in "
            //    + color_names[c1] + " , and two choices are given below it.\r\n"
            //    + " .Press left arrow key(<) to select left answer shown on left side;\r\n" 
            //    + " .press right arrow key (>) to select the answer shown on the right side.\r\n" 
            //    + "As the word is " + color_names[c0] + " in color, you should press "
            //    + ((ans_correct == 0)? "Left arrow (<)" : "Right arrow (<))")
            //    + " to select the answer shown on the "
            //    + ((ans_correct == 0)? "Left" : "Right") + " site.";

            labelExplanation.Text = "To ensure you remain fully focused to achieve the best result, please follow the advices given below.\r\n"
                + "    -    Avoid unnecessary body movement;\r\n"
                + "    -    Do not speak;\r\n"
                + "    -    Concentrate to decide on the colour in which the word is displayed in and press the corresponding arrow key as quick as possible.";

            labelHint.Text = "Press left/right arrow key to practise, press Enter to continue.";
            required_key = 3;
            labelResult.Text = null;
        }

        private void DisplayRelaxInstruction()
        {
            labelLevel.Text = "Practice for Calibration";
            labelInstruction.Text = "During the Rest period, the word \"Rest\" will be displayed as below:";

            labelColorCue.Text = "Rest";
            labelColorCue.ForeColor = SystemColors.WindowText; // should be the correct color
            labelColorCue.BackColor = Color.Transparent;

            labelLeftColName.Text = labelRightColName.Text = null;
            labelLeftArrow.Visible = labelRightArrow.Visible = labelResult.Visible = false;

            labelExplanation.Text = "Move your eyes around and do not focus on a particular point.";

            labelHint.Text = "Press left arrow key to go back, press Enter to start calibration.";
            required_key = 4;
            labelResult.Text = null;
        }

		private void ExeRecording() {
			ResManager cfg = BCIApplication.SysResource;
			LoadCfg(cfg);
			if (cfg_level_seq == null || cfg_level_seq.Length == 0) return;

            string trname = null;
            int si = 0;
            string[] session_ID = new string[cfg_level_seq.Length];

            if (bDryRun) trname = null;
            else {
                if (AmpContainer.Count <= 0) {
                    MessageBox.Show("No amplifiers defined!");
                    return;
                }
                if (!AmpContainer.StartAll()) return;

                //if (trname == null) {
                //    trname = TrainDirSpecForm.GetTrainingPath(cfg, false);
                //    if (trname == null) return;
                //}
                trname = Path.Combine(BCIApplication.DatePath, "DataTrain_StroopTest");
                trname = Path.Combine(trname, DateTime.Now.ToBinary().ToString("X"));
                Directory.CreateDirectory(trname);
            }

            while (si < cfg_level_seq.Length && !evt_proc_stop.WaitOne(0, false))
                //(int ilevel in cfg_level_seq)
            {
                int ilevel = cfg_level_seq[si];

                this.Invoke((Action)delegate
                {   
                    ++si;
                    labelSession.Text = string.Format("Session: {0}", si);

                    // display instruction
                    labelColorCue.Text = null;
                    //labelLevel.Text = string.Format("Level {0}: Practice", ilevel);
                    labelLevel.Text = string.Format("Calibration Session {0}", si);
                    labelInstruction.Visible = true;
                    labelInstruction.Text = instructions[ilevel - 1];
                });

                if (!DryRun && !AmpContainer.AllAlive) evt_proc_stop.Set();

                int score = 0;

				while (!evt_proc_stop.WaitOne(0)) {
                    // while loop to one session, maybe need to redo 
					MessageBox.Show(
                        //string.Format("Session {0} / {1}, level {2}.\nClick OK to start.", si, cfg_level_seq.Length, ilevel),
                        string.Format("Session {0} / {1}. Click OK to start.", si, cfg_level_seq.Length),
                        "Stroop Test", MessageBoxButtons.OK, MessageBoxIcon.Information);

					string fn_prefix = string.Format("Session{0}Level{1}_{2}_", si, ilevel,
                        DateTime.Now.ToString(BCIApplication.FMT_TIMESTAMP));

                    ConsoleCapture.ConsoleFile cfile = null;

                    // record
                    if (!DryRun) {
                        cfg.SaveFile(Path.Combine(trname, fn_prefix + "system.cfg"));
                        cfile = ConsoleCapture.StartConsoleFile(Path.Combine(trname, fn_prefix + "log.txt"));
                        AmpContainer.StartRecord(trname, fn_prefix);
                    }

                    score = ExeStroopTest(ilevel);
                    if (!DryRun) {
                        AmpContainer.StopRecord();
                        if (!AmpContainer.AllAlive) evt_proc_stop.Set();
                        cfile.EndLogFile();
                    }

                    session_ID[si - 1] = fn_prefix;

                    if (evt_proc_stop.WaitOne(0, false) || score >= cfg_calib_threshold) {
                        break;
                    }

					DialogResult rst = MessageBox.Show(
						string.Format("Your score is {0}, You need to redo session {1}!\nClick YES to redo (Data will be removed) or NO to continue (Data is accepted).", score, si),
						"Stroop Test: " + fn_prefix, MessageBoxButtons.YesNo); 

					if (rst != DialogResult.Yes) {
						break;
					}

                    // redo calibration
                    labelLevel.Text = string.Format("Calibration Session {0} (Redo)", si);
                    labelInstruction.Visible = true;
                    labelInstruction.Text = instructions[ilevel - 1];

                    ClearTraining(trname, fn_prefix, false);
				}

                if (evt_proc_stop.WaitOne(0, false)) break;

                if (si < cfg_level_seq.Length && score >= cfg_calib_threshold) {
                    // not last session, wait to continue
                    Invoke((Action)delegate {
                        labelHint.Text = "Press return to continue";
                        Activate();
                    });

                    required_key = 1;
                    evt_key_pressed.Reset();

                    WaitHandle.WaitAny(wevts);
                }

                Invoke((Action)delegate
                {
                    labelHint.Text = null;
                    labelTrial.Text = null;
                });
            }

			if (!bDryRun) {
				if (!evt_proc_stop.WaitOne(0, false)) {
                    // ccwang 1: Add files into training list
                    BCIApplication.AddTrainingPath(trname, session_ID);
				}
			}
		}

		private string[] instructions =
			new string[] {
							 "A colored box will appear below. The names of two colors will appear below the box. Use the arrow keys to choose the word that matches the color in the box.",
							 "The name of a color will appear below. Below that, two other words will appear. Use the arrow keys to choose the word that matches the word on top.",
							 //"This time the word will be written in a color different from the color it means. For example, the word \"red\" will be written in green. You should choose green.",
							 "A word will be written in a color different from the color it means. For example, if the word \"red\" is written in green. You should choose green.",
		};
		private string[] hinds =
			new string[] {
							 "Use the arrow keys to choose the word that matches the color in the box",
							 "Use the arrow keys to choose the word that matches the word at the top",
							 "Use the arrow keys to choose the color name that matches the color of the word in the box",
		};

		private int ExeStroopTest(int level) {
			int lidx = level - 1;
			int offset_lev = level * 50;
			int offset_cue = 10;
			//int offset_btn = 20;
			int offset_clk = 30;

			int stim_trial_start = 1;
			int stim_relax_start = 0;

			int ntrials = cfg_ntrials;

            Invoke((Action)delegate
            {
                labelInstruction.Visible = false;
                labelColorCue.Text = null;
            });

            int nDone = 0;
            int nCorrect = 0;
            int stim = 0;

			// color random sequence
			int[] clr_rnd = BCIApplication.GenerateSeq(ntrials, color_names.Length);
			int clr_ri = 0;

			// random generator
			Random rnd = new Random();

			for (int it = 0; it < ntrials; it++) {
                Sound.Beep(500, 500);
                if (evt_proc_stop.WaitOne(cfg_beeptime - 500, false)) break;

                Invoke((Action)delegate
                {
                    labelTrial.Text = string.Format("Trial {0}", it + 1);

                    if (lidx != 0)
                        labelColorCue.BackColor = Color.FromArgb(51, 51, 255);
                    if (lidx != 2)
                        labelColorCue.ForeColor = Color.White;
                });

				// trial start
				stim = offset_lev + stim_trial_start;
				BCIApplication.LogMessage("Stim Trial Starts, {0}", stim);
				if (!DryRun) AmpContainer.SendAll(stim);

				long t0 = HRTimer.GetTimestamp(); // concentration start
				int tt = 0;

				while (!evt_proc_stop.WaitOne(0, false)) {
                    if (!DryRun && !AmpContainer.AllAlive) evt_proc_stop.Set();

					if (clr_ri == 0) {
						clr_rnd = BCIApplication.GenerateSeq(color_names.Length);
					}
					int icol = clr_rnd[clr_ri++];
					clr_ri %= color_names.Length;

					int isel = rnd.Next(2);
					int jcol = rnd.Next(color_names.Length - 1);
					if (jcol >= icol) jcol++;

                    // display color word
                    Invoke((Action)delegate
                    {
                        if (lidx == 0) { // level1
                            labelColorCue.BackColor = color_values[icol];
                        }
                        else if (lidx == 1) { // level2

                            labelColorCue.Text = color_names[icol];
                            labelColorCue.ForeColor = color_values[icol];

                        }
                        else if (lidx == 2) { // level3
                            labelColorCue.Text = color_names[jcol];
                            labelColorCue.ForeColor = color_values[icol];
                        }

                        labelColorCue.Visible = true;
                    });

					stim = offset_lev + offset_cue + icol + 1;
                    if (!DryRun) AmpContainer.SendAll(stim);
					BCIApplication.LogMessage("Stim cue = {0}", stim);

					if (evt_proc_stop.WaitOne(cfg_timecue, false)) break;

					int cleft = icol;
					int cright = jcol;
					if (isel == 1) {
						cleft = jcol;
						cright = icol;
					}

                    this.Invoke((Action)delegate
                    {
                        // display choices and hint
                        labelLeftArrow.Visible = true;
                        labelRightArrow.Visible = true;
                        labelLeftColName.Text = color_names[cleft];
                        labelRightColName.Text = color_names[cright];
                        labelHint.Text = hinds[lidx];
                    });

					//	stim = offset_lev + offset_btn + cleft + 1;
					//	ssender.SendCode(stim);
					//	BCIApplication.LogMessage("Stim button_left = {0}", stim);
					//	stim = offset_lev + offset_btn + cright + 1;
					//	ssender.SendCode(stim);
					//	BCIApplication.LogMessage("Stim button_right = {0}", stim);

					// wait for selection
					required_key = 2;
					evt_key_pressed.Reset();
					long t1 = HRTimer.GetTimestamp();
					while (true) {
						int tn = HRTimer.DeltaMilliseconds(t1);
						if (tn >= cfg_wtimeout) break;
						int dt = (tn + 900) / 1000;

                        this.Invoke((Action)delegate
                        {
                            labelTimer.Text = dt.ToString();
                            this.Activate();
                        });

						int wt = cfg_wtimeout - tn;
						if (wt > 200) wt = 200;
						if (WaitHandle.WaitAny(wevts, wt, false) != WaitHandle.WaitTimeout) break;
					}
					if (evt_proc_stop.WaitOne(0, false)) break;

                    nDone++;
                    int cclk = icol;

                    this.Invoke((Action)delegate
                    {
                        labelHint.Text = null;

                        if (required_key < 10) { // timeout
                            cclk = -1;
                            labelResult.Text = "?";
                            labelResult.ForeColor = Color.Orange;
                        }
                        else if (isel == 0 && required_key == 11 || isel == 1 && required_key == 12) {
                            labelResult.Text = char.ToString((char)(0xD6));
                            labelResult.ForeColor = Color.Green;
                            nCorrect++;
                        }
                        else {
                            labelResult.Text = char.ToString((char)(0xB4));
                            labelResult.ForeColor = Color.Red;
                            cclk = jcol;
                        }
                        labelLeftArrow.Visible = labelLeftColName.Visible = (required_key == 11);
                        labelRightArrow.Visible = labelRightColName.Visible = (required_key == 12);
                        labelResult.Visible = true;

                        labelTimer.Text = null;

                        if (it + 1 < ntrials) {
                            labelHint.Text = "Prepare for the next question";
                        }
                        else {
                            labelHint.Text = null;
                        }

                        this.Activate();
                    });
				
					stim = offset_lev + offset_clk + cclk + 1;
                    if (!DryRun) AmpContainer.SendAll(stim);
					BCIApplication.LogMessage("Stim click = {0}", stim);

                    // wait fro display
                    if (evt_proc_stop.WaitOne(500, false)) break;

					required_key = 1;
					evt_key_pressed.Reset();
					WaitHandle.WaitAny(wevts, cfg_timerest, false);

                    Invoke((Action)delegate
                    {
                        labelHint.Text = null;
                        labelResult.Text = null;

                        //labelColorCue.Visible = false;
                        labelLeftArrow.Visible = labelRightArrow.Visible = false;
                        labelLeftColName.Text = labelRightColName.Text = null;
                        labelLeftColName.Visible = labelRightColName.Visible = true;
                    });

					tt = HRTimer.DeltaMilliseconds(t0);
					if (tt >= cfg_timerelax) break;
				}

				if (evt_proc_stop.WaitOne(0, false)) break;

				if (cfg_timerelax > 0) {
					// relax image random sequence
					int[] rx_rnd = null;
					int rx_ri = 0;

					if (rx_ri == 0 && img_list != null) {
						rx_rnd = BCIApplication.GenerateSeq(img_list.Length);
					}

                    int bkidx = -1;
                    if (rx_rnd != null) {
                        bkidx = rx_ri;
                        rx_ri = (rx_ri + 1) % rx_rnd.Length;
                    }

                    Invoke((Action)delegate
                    {
                        // display instruction
                        labelColorCue.Text = "Rest";
                        labelColorCue.BackColor = Color.Transparent;
                        labelColorCue.ForeColor = SystemColors.WindowText;

                        labelHint.Text = string.Format("Rest for {0} seconds.",
                            tt / 1000);
                    });

					stim = offset_lev + stim_relax_start;
                    if (!DryRun) AmpContainer.SendAll(stim);
					BCIApplication.LogMessage("Stim Rest {0}", stim);

					t0 = HRTimer.GetTimestamp();
					bool balerted = false;
					while (true) {
						int tw = HRTimer.DeltaMilliseconds(t0);
						if (tw >= tt) break;
						tw = tt - tw;
                        Invoke((Action)delegate
                        {
                            labelTimer.Text = ((tw + 999) / 1000).ToString();
                        });
						if (!balerted && tw < 1000) {
							//Sound.BeepAsunc(500, 500);
							balerted = true;
						}
						if (tw > 200) tw = 200;
						if (evt_proc_stop.WaitOne(tw, false)) break;
					}

                    Invoke((Action)delegate
                    {
                        labelTimer.Text = null;
                        labelColorCue.Text = null;
                    });

					if (evt_proc_stop.WaitOne(0, false)) break;
				}
            }

            Invoke((Action)delegate
            {
                labelLevel.Text = string.Format("You have got {0} correct of {1}.", nCorrect, nDone);
                labelColorCue.Text = null;
                labelHint.Text = null;
            });

            if (nDone == 0) return 0;
			else return (100 * nCorrect / nDone);
		}

        private void buttonStop_Click(object sender, System.EventArgs e) {
			evt_proc_stop.Set();
		}

        private int kid = 0;

        protected override bool IsInputKey(System.Windows.Forms.Keys keyData)
        {
            if (keyData == Keys.Enter || keyData == Keys.Left || keyData == Keys.Right)
                return true;

            return base.IsInputKey(keyData);
        }


		private void StroopTestForm_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
            e.Handled = ProcessKeyDown(e.KeyData);
        }

        private bool ProcessKeyDown(Keys key_data) {
            if (key_data == Keys.Return && required_key == 1) {
				required_key = 10;
				evt_key_pressed.Set();
				return true;
			} else if (required_key == 2) {
                if (key_data == Keys.Left || key_data == Keys.NumPad4) {
					required_key = 11;
					evt_key_pressed.Set();
					return true;
                }
                else if (key_data == Keys.Right || key_data == Keys.NumPad6) {
					required_key = 12;
					evt_key_pressed.Set();
					return true;
				}
            }
            else if (required_key == 3) {
                if (key_data == Keys.Left || key_data == Keys.Right
                    || key_data == Keys.NumPad4 || key_data == Keys.NumPad6)
                {
                    kid++;
                    bool left = (key_data == Keys.Left || key_data == Keys.NumPad4);
                    labelLeftColName.Visible = labelLeftArrow.Visible = left;
                    labelRightColName.Visible = labelRightArrow.Visible = !left;
                    if (left && ans_correct == 0 || !left && ans_correct > 0) {
                        labelResult.Text = char.ToString((char)(0xD6));
                        labelResult.ForeColor = Color.Green;
                    }
                    else {
                        labelResult.Text = char.ToString((char)(0xB4));
                        labelResult.ForeColor = Color.Red;
                    }

                    int kid0 = kid;
                    System.Timers.Timer timer = new System.Timers.Timer();
                    timer.Interval = 1000;
                    timer.AutoReset = false;
                    timer.Elapsed += (System.Timers.ElapsedEventHandler)delegate(object s, System.Timers.ElapsedEventArgs evt) {
                        if (kid0 == kid) DisplayMCQInstruction();
                        timer.Stop();
                        timer.Dispose();
                    };
                    timer.Start();
                    return true;
                }
                else if (key_data == Keys.Return) {
                    DisplayRelaxInstruction();
                    return true;
                }
            }
            //else if (required_key == 5) {
            //    if (e.KeyCode == Keys.Return) {
            //        DisplayMCQInstruction();
            //    }
            //}
            else if (required_key == 4) {
                if (key_data == Keys.Left || key_data == Keys.NumPad4) {
                    //DisplayCalibInstruction();
                    DisplayMCQInstruction();
                    return true;
                }
                else if (key_data == Keys.Enter) {
                    StartCalibration();
                    return true;
                }
            }
            return false;
		}

		private void ConcentrationStroopTest_Load(object sender, System.EventArgs e) {
			InitStroopTest();
			LoadCfg();

            //labelLevel.Text = string.Format("You have got {0} correct of {1} ({2:#.##}%).", 55, 58, 55.0 * 100 / 58.0);
            //textInstruction.Text = null;
            //textHint.Text = "Press return to continue";
            //labelResult.Text = null;

            //labelColorCue.Visible = false;
            //labelLeftArrow.Visible = labelRightArrow.Visible = false;
            //labelLeftColName.Text = labelRightColName.Text = null;
            //labelLeftColName.Visible = labelRightColName.Visible = true;

            //buttonStart.Enabled = false;
            //buttonStop.Enabled = true;

            //MessageBox.Show(
            //    string.Format("Your score is 80%, You need to redo session 1!\nClick YES to redo (Data will be deleted) or NO to continue (Data is accepted)."),
            //    "Stroop Test", MessageBoxButtons.YesNo);

            // senerio 2
            labelLeftArrow.Text = new string((char) 0xAC, 1);
            labelRightArrow.Text = new string((char) 0xAE, 1);

            //DisplayCalibInstruction();
            DisplayMCQInstruction();
        }

        private void StroopTestForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Stop calibration if it is running
            evt_proc_stop.Set();
        }

        private void buttonStop_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right || e.KeyCode == Keys.Return)
                e.IsInputKey = true;
                //ProcessKeyDown(e.KeyCode);
        }
	}
}
