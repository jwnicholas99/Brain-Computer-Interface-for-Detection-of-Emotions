using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Text;
using System.Linq;

using BCILib.Util;
using BCILib.App;

namespace BCILib.P300
{
	delegate void AppendText(string msg);
	/// <summary>
	/// Summary description for TrainModelForm.
	/// </summary>
	public class TrainModelForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.Button btnTrain;
		private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.Button btBrowse1;
        private System.Windows.Forms.Button btBrowse2;

		private System.Windows.Forms.Panel panel1;

		private const int MAX_VALUE = 100;
		private const int MARGIN_X = 20;
		private const int MARGIN_Y = 20;

		private int[,] target_data = null, nontarget_data = null;
		private int[] zeros = null;
		private string[] channel_name = null;
		// private CheckBox[] channel_chk = null;
		private bool[] channel_sel = null;
		private Rectangle[] channel_reg = null;
		private int num_column = 1;
		private int start_time = 100;
		private int end_time = 500;
		private double[] chnmax = null;
		private double[] chnmin = null;

		private System.Windows.Forms.Button btnBrowACE;
		private System.Windows.Forms.Button btnShowTrainACE;
		private System.Windows.Forms.Button btnShowTestACE;
		private System.Windows.Forms.Button btnSelectChannel;
		private System.Windows.Forms.Button btnSaveCfg;
        private System.Windows.Forms.Button buttonFilter;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textBox_VLD_Idle;
		private System.Windows.Forms.Button btBrowse_VLD_Idle;
		private System.Windows.Forms.TextBox textBox_VLD_Eval;
		private System.Windows.Forms.Button btBrowse_VLD_Eval;
		private System.Windows.Forms.StatusBar statusBar1;
		private System.Windows.Forms.Button buttonSaveWindow;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

        private P300ConfigCtrl _cfgctrl = null;

		public TrainModelForm(P300ConfigCtrl cfgctrl)
		{
            _cfgctrl = cfgctrl;

			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			String dir = _cfgctrl.TrainModelFileList;
			if (dir != null && dir.EndsWith(".txt")) dir = dir.Substring(0, dir.Length - 4);
			//if (dir == null || !Directory.Exists(dir)) dir = "Data";
			ModelTrainingPath = BCIApplication.GetRelativeUserPath(dir);

			dir = _cfgctrl.TrainRejectFileList;
			if (dir != null && dir.EndsWith(".txt")) dir = dir.Substring(0, dir.Length - 4);
			//if (dir == null || !Directory.Exists(dir)) dir = "Data";
			RejectionTrainingPath = BCIApplication.GetRelativeUserPath(dir);

            //dir = cfg.GetConfigValue("ModelTrainingSetting", "TrainVariLenDetect_IdleFileList");
            //if (dir != null && dir.EndsWith(".txt")) dir = dir.Substring(0, dir.Length - 4);
            ////if (dir == null || !Directory.Exists(dir)) dir = @".\Data";
            //VLDIdleTrainingPath = BCIApplication.GetRelativePath(dir);

            //dir = cfg.GetConfigValue("ModelTrainingSetting", "TrainVariLenDetect_EvalFileList");
            //if (dir != null && dir.EndsWith(".txt")) dir = dir.Substring(0, dir.Length - 4);
            ////if (dir == null || !Directory.Exists(dir)) dir = @".\Data";
            //VLDEvalTrainingPath = BCIApplication.GetRelativePath(dir);
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
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.btBrowse1 = new System.Windows.Forms.Button();
            this.btBrowse2 = new System.Windows.Forms.Button();
            this.btnTrain = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnBrowACE = new System.Windows.Forms.Button();
            this.btnShowTrainACE = new System.Windows.Forms.Button();
            this.btnShowTestACE = new System.Windows.Forms.Button();
            this.btnSelectChannel = new System.Windows.Forms.Button();
            this.btnSaveCfg = new System.Windows.Forms.Button();
            this.buttonFilter = new System.Windows.Forms.Button();
            this.textBox_VLD_Idle = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btBrowse_VLD_Idle = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox_VLD_Eval = new System.Windows.Forms.TextBox();
            this.btBrowse_VLD_Eval = new System.Windows.Forms.Button();
            this.statusBar1 = new System.Windows.Forms.StatusBar();
            this.buttonSaveWindow = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(144, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Model training directory:";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(144, 23);
            this.label2.TabIndex = 0;
            this.label2.Text = "Rejection training directory:";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(160, 8);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(264, 20);
            this.textBox1.TabIndex = 1;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(160, 32);
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(264, 20);
            this.textBox2.TabIndex = 1;
            // 
            // btBrowse1
            // 
            this.btBrowse1.Location = new System.Drawing.Point(432, 8);
            this.btBrowse1.Name = "btBrowse1";
            this.btBrowse1.Size = new System.Drawing.Size(75, 23);
            this.btBrowse1.TabIndex = 2;
            this.btBrowse1.Text = "Browse";
            this.btBrowse1.Click += new System.EventHandler(this.ButtonBrowse_Click);
            // 
            // btBrowse2
            // 
            this.btBrowse2.Location = new System.Drawing.Point(432, 32);
            this.btBrowse2.Name = "btBrowse2";
            this.btBrowse2.Size = new System.Drawing.Size(75, 23);
            this.btBrowse2.TabIndex = 2;
            this.btBrowse2.Text = "Browse";
            this.btBrowse2.Click += new System.EventHandler(this.ButtonBrowse_Click);
            // 
            // btnTrain
            // 
            this.btnTrain.Location = new System.Drawing.Point(456, 118);
            this.btnTrain.Name = "btnTrain";
            this.btnTrain.Size = new System.Drawing.Size(64, 23);
            this.btnTrain.TabIndex = 3;
            this.btnTrain.Text = "&Train";
            this.btnTrain.Click += new System.EventHandler(this.ButtonTrain_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(550, 64);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 36);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "&Close";
            this.btnClose.Click += new System.EventHandler(this.ButtonClose_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Location = new System.Drawing.Point(16, 150);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(660, 365);
            this.panel1.TabIndex = 4;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            this.panel1.Click += new System.EventHandler(this.panel1_Click);
            this.panel1.Resize += new System.EventHandler(this.panel1_Resize);
            // 
            // btnBrowACE
            // 
            this.btnBrowACE.Location = new System.Drawing.Point(16, 118);
            this.btnBrowACE.Name = "btnBrowACE";
            this.btnBrowACE.Size = new System.Drawing.Size(75, 23);
            this.btnBrowACE.TabIndex = 5;
            this.btnBrowACE.Text = "&DataFile:";
            this.btnBrowACE.Click += new System.EventHandler(this.btnBrowACE_Click);
            // 
            // btnShowTrainACE
            // 
            this.btnShowTrainACE.Location = new System.Drawing.Point(528, 8);
            this.btnShowTrainACE.Name = "btnShowTrainACE";
            this.btnShowTrainACE.Size = new System.Drawing.Size(112, 23);
            this.btnShowTrainACE.TabIndex = 6;
            this.btnShowTrainACE.Text = "ShowAverageEEG";
            this.btnShowTrainACE.Click += new System.EventHandler(this.btnShowTrainACE_Click);
            // 
            // btnShowTestACE
            // 
            this.btnShowTestACE.Location = new System.Drawing.Point(528, 32);
            this.btnShowTestACE.Name = "btnShowTestACE";
            this.btnShowTestACE.Size = new System.Drawing.Size(112, 23);
            this.btnShowTestACE.TabIndex = 6;
            this.btnShowTestACE.Text = "ShowAverageEEG";
            this.btnShowTestACE.Click += new System.EventHandler(this.btnShowTestACE_Click);
            // 
            // btnSelectChannel
            // 
            this.btnSelectChannel.Enabled = false;
            this.btnSelectChannel.Location = new System.Drawing.Point(104, 118);
            this.btnSelectChannel.Name = "btnSelectChannel";
            this.btnSelectChannel.Size = new System.Drawing.Size(96, 23);
            this.btnSelectChannel.TabIndex = 7;
            this.btnSelectChannel.Text = "&Select All";
            this.btnSelectChannel.Click += new System.EventHandler(this.btnSelectChannel_Click);
            // 
            // btnSaveCfg
            // 
            this.btnSaveCfg.Location = new System.Drawing.Point(208, 118);
            this.btnSaveCfg.Name = "btnSaveCfg";
            this.btnSaveCfg.Size = new System.Drawing.Size(80, 23);
            this.btnSaveCfg.TabIndex = 3;
            this.btnSaveCfg.Text = "Con&figure";
            this.btnSaveCfg.Click += new System.EventHandler(this.btnCfg_Click);
            // 
            // buttonFilter
            // 
            this.buttonFilter.Location = new System.Drawing.Point(384, 118);
            this.buttonFilter.Name = "buttonFilter";
            this.buttonFilter.Size = new System.Drawing.Size(64, 23);
            this.buttonFilter.TabIndex = 8;
            this.buttonFilter.Text = "&Filter";
            this.buttonFilter.Click += new System.EventHandler(this.buttonFilter_Click);
            // 
            // textBox_VLD_Idle
            // 
            this.textBox_VLD_Idle.Location = new System.Drawing.Point(160, 80);
            this.textBox_VLD_Idle.Name = "textBox_VLD_Idle";
            this.textBox_VLD_Idle.Size = new System.Drawing.Size(264, 20);
            this.textBox_VLD_Idle.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(8, 80);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(144, 23);
            this.label4.TabIndex = 9;
            this.label4.Text = "VariLenDetect: Idle Files";
            // 
            // btBrowse_VLD_Idle
            // 
            this.btBrowse_VLD_Idle.Location = new System.Drawing.Point(432, 80);
            this.btBrowse_VLD_Idle.Name = "btBrowse_VLD_Idle";
            this.btBrowse_VLD_Idle.Size = new System.Drawing.Size(75, 23);
            this.btBrowse_VLD_Idle.TabIndex = 11;
            this.btBrowse_VLD_Idle.Text = "Browse";
            this.btBrowse_VLD_Idle.Click += new System.EventHandler(this.ButtonBrowse_Click);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(8, 56);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(144, 23);
            this.label5.TabIndex = 12;
            this.label5.Text = "VariLenDetect: Eval Files";
            // 
            // textBox_VLD_Eval
            // 
            this.textBox_VLD_Eval.Location = new System.Drawing.Point(160, 56);
            this.textBox_VLD_Eval.Name = "textBox_VLD_Eval";
            this.textBox_VLD_Eval.Size = new System.Drawing.Size(264, 20);
            this.textBox_VLD_Eval.TabIndex = 13;
            // 
            // btBrowse_VLD_Eval
            // 
            this.btBrowse_VLD_Eval.Location = new System.Drawing.Point(432, 56);
            this.btBrowse_VLD_Eval.Name = "btBrowse_VLD_Eval";
            this.btBrowse_VLD_Eval.Size = new System.Drawing.Size(75, 23);
            this.btBrowse_VLD_Eval.TabIndex = 0;
            this.btBrowse_VLD_Eval.Text = "Browse";
            this.btBrowse_VLD_Eval.Click += new System.EventHandler(this.ButtonBrowse_Click);
            // 
            // statusBar1
            // 
            this.statusBar1.Location = new System.Drawing.Point(0, 521);
            this.statusBar1.Name = "statusBar1";
            this.statusBar1.Size = new System.Drawing.Size(688, 22);
            this.statusBar1.TabIndex = 14;
            // 
            // buttonSaveWindow
            // 
            this.buttonSaveWindow.Location = new System.Drawing.Point(528, 118);
            this.buttonSaveWindow.Name = "buttonSaveWindow";
            this.buttonSaveWindow.Size = new System.Drawing.Size(64, 23);
            this.buttonSaveWindow.TabIndex = 3;
            this.buttonSaveWindow.Text = "Save&JPG";
            this.buttonSaveWindow.Click += new System.EventHandler(this.buttonSaveWindow_Click);
            // 
            // TrainModelForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(688, 543);
            this.Controls.Add(this.statusBar1);
            this.Controls.Add(this.btBrowse_VLD_Eval);
            this.Controls.Add(this.textBox_VLD_Eval);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBox_VLD_Idle);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btBrowse_VLD_Idle);
            this.Controls.Add(this.buttonFilter);
            this.Controls.Add(this.btnSelectChannel);
            this.Controls.Add(this.btnShowTrainACE);
            this.Controls.Add(this.btnBrowACE);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.btnTrain);
            this.Controls.Add(this.btBrowse1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btBrowse2);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnShowTestACE);
            this.Controls.Add(this.btnSaveCfg);
            this.Controls.Add(this.buttonSaveWindow);
            this.Name = "TrainModelForm";
            this.Text = "TrainModelForm";
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void ButtonTrain_Click(object sender, System.EventArgs e) {
            if (!SelectChannels()) {
                MessageBox.Show(this, "Channel selection error!", "Error");
                return;
            }

            Thread thd = new Thread(new ThreadStart(BuildModel));
			thd.Start();
		}

		private void BuildModel()
        {
			Cursor savedCursor = this.Cursor;

            Control.CheckForIllegalCrossThreadCalls = false;

			this.Cursor = Cursors.WaitCursor;
			CreateDescFile(3);

			string modelprefix = Path.GetFileName(ModelTrainingPath);
			if (modelprefix.EndsWith("_Training")) modelprefix = modelprefix.Substring(0, modelprefix.Length - 9);
            modelprefix += "_" + BCIApplication.TimeStamp;

			//ModelList directory
			string mldir = "ModelList";
			if (!Directory.Exists(mldir)) Directory.CreateDirectory(mldir);

			string ln = Path.Combine(mldir, modelprefix + ".cfg");

			string dir = ModelDirectory;
			if (!Directory.Exists(dir)) 
			{
				Directory.CreateDirectory(dir);
			}

			modelprefix = Path.Combine(dir, modelprefix);

			// set other parameters
			string OutputModelFile = modelprefix + "_Scaled.model";
			string OutputProcParameterFile = modelprefix + "_P300Parameters.cfg";
			string OutputFeatureRangeFile = modelprefix + "_FeatureRange.txt";
			string OutputModelScoreFile =modelprefix+"_ModelScore.dat";

            ResManager cfg = _cfgctrl.Resource;
			cfg.SetConfigValue("ModelTrainingSetting", "OutputModelFile", OutputModelFile);
			cfg.SetConfigValue("ModelTrainingSetting", "OutputProcParameterFile", OutputProcParameterFile);
			cfg.SetConfigValue("ModelTrainingSetting", "OutputFeatureRangeFile", OutputFeatureRangeFile);
			cfg.SetConfigValue("ModelTrainingSetting", "OutputModelScoreFile", OutputModelScoreFile);

			cfg.SaveFile();


            // capture output to save log file
            var cfile = ConsoleCapture.StartConsoleFile(modelprefix + ".log");
            Console.WriteLine("Log file is saved to {0}.log", modelprefix);
            ConsoleOutForm.ShowWindow();
            bool rv = P300Processor.P300Util_TrainModel(Path.Combine("Config", "System.cfg"));
            cfile.EndLogFile();
            MessageBox.Show("Training finished.");
            ConsoleOutForm.HideWindow();

			if (rv) {
				ResManager res = new ResManager();
				res.SetConfigValue("Process", "P300ParaFile", OutputProcParameterFile);
				res.SetConfigValue("Process", "SVMModel", OutputModelFile);
				res.SetConfigValue("Process", "SVMRange", OutputFeatureRangeFile);
				res.SetConfigValue("Process", "SVMModelScore", OutputModelScoreFile);
                res.SetConfigValue("Process", "RejectionThreshold_ManualSet", 0);
				res.SetConfigValue("Process", "RejectionThresholdBias", 0);

                //string sc = cfg.GetConfigValue("EEG", "P300UsedChannels");
                //if (sc != null) res.SetConfigValue("EEG", "P300UsedChannels", sc);

				res.SaveFile(ln);

				cfg.LoadFile(ln, false);
				cfg.SaveFile();
			} else {
				MessageBox.Show(this, "Training model failed!", "Error");
			}

			this.Cursor = savedCursor;
            Control.CheckForIllegalCrossThreadCalls = true;

            return;
		}

		private void ButtonClose_Click(object sender, System.EventArgs e) {
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void ButtonBrowse_Click(object sender, System.EventArgs e) {
			Button btn = (Button) sender;
			TextBox box = null;
			if (btn == btBrowse1) box = textBox1;
			else if (btn == btBrowse2) box = textBox2;
			else if (btn == btBrowse_VLD_Idle) box = textBox_VLD_Idle;
			else if (btn == btBrowse_VLD_Eval) box = textBox_VLD_Eval;
			
			if (box != null) {
				string dir = BrowseForDirectory(box.Text);
				if (dir != null) 
				{
					box.Text = dir;
					if (box == textBox1) CreateDescFile(1);
					else if (box == textBox2) CreateDescFile(2);
					else if (box == textBox_VLD_Idle) CreateDescFile(4);
					else if (box == textBox_VLD_Eval) CreateDescFile(5);
				}
			}
		}

		//
		// which == 1: Training Directory
		// which == 2: Testing Directory
		// which == 3: All Directory
		//
		private void CreateDescFile(int which) {
			if ((which & 1) == 1) {
				string dir = this.ModelTrainingPath;
				string tf = dir + ".txt";
				DirectoryInfo dinf = new DirectoryInfo(dir);
				FileInfo finf = new FileInfo(tf);
				if (dinf.Exists && (!finf.Exists || finf.LastWriteTime < dinf.LastWriteTime)) {
					// create description test
					tf = P300Manager.BuildP300TrainingDescFile(dir, _cfgctrl.SpellerChars);
					finf = new FileInfo(tf);
				}
				if (finf.Exists) {
					_cfgctrl.TrainModelFileList = tf;
				}
			}

			if ((which & 2) == 2) {
				string dir = this.RejectionTrainingPath;
				string tf = dir + ".txt";
				DirectoryInfo dinf = new DirectoryInfo(dir);
				FileInfo finf = new FileInfo(tf);
				if (dinf.Exists && (!finf.Exists || finf.LastWriteTime < dinf.LastWriteTime)) {
					// create description test
					tf = P300Manager.BuildP300TrainingDescFile(dir, _cfgctrl.SpellerChars);
					finf = new FileInfo(tf);
				}
				if (finf.Exists) {
					_cfgctrl.TrainRejectFileList =tf;
				}
			}

            //if(which == 4) 
            //{
            //    string dir = this.VLDIdleTrainingPath;
            //    string tf = dir + ".txt";
            //    DirectoryInfo dinf = new DirectoryInfo(dir);
            //    FileInfo finf = new FileInfo(tf);
            //    if (dinf.Exists && (!finf.Exists || finf.LastWriteTime < dinf.LastWriteTime)) 
            //    {
            //        // create description test
            //        tf = P300Manager.BuildP300TrainingDescFile(dir, _cfgctrl.SpellerChars);
            //        finf = new FileInfo(tf);
            //    }
            //    if (finf.Exists) 
            //    {
            //        cfg.SetConfigValue("ModelTrainingSetting", "TrainVariLenDetect_IdleFileList", tf);
            //    }
            //}

            //if(which == 5) 
            //{
            //    string dir = this.VLDEvalTrainingPath;
            //    string tf = dir + ".txt";
            //    DirectoryInfo dinf = new DirectoryInfo(dir);
            //    FileInfo finf = new FileInfo(tf);
            //    if (dinf.Exists && (!finf.Exists || finf.LastWriteTime < dinf.LastWriteTime)) 
            //    {
            //        // create description test
            //        tf = P300Manager.BuildP300TrainingDescFile(dir, _cfgctrl.SpellerChars);
            //        finf = new FileInfo(tf);
            //    }
            //    if (finf.Exists) 
            //    {
            //        cfg.SetConfigValue("ModelTrainingSetting", "TrainVariLenDetect_EvalFileList", tf);
            //    }
            //}

			_cfgctrl.Save();
		}

		public string ModelTrainingPath {
			get {
				return textBox1.Text;
			}
			set {
				textBox1.Text = value;
			}
		}

		public string RejectionTrainingPath {
			get {
				return textBox2.Text;
			}
			set {
				textBox2.Text = value;
			}
		}

		public string VLDIdleTrainingPath 
		{
			get 
			{
				return textBox_VLD_Idle.Text;
			}
			set 
			{
				textBox_VLD_Idle.Text = value;
			}
		}

		public string VLDEvalTrainingPath 
		{
			get 
			{
				return textBox_VLD_Eval.Text;
			}
			set 
			{
				textBox_VLD_Eval.Text = value;
			}
		}
		private string BrowseForDirectory(string dir) {
			if (!Directory.Exists(dir)) dir = @".\";
			FolderBrowserDialog dlg = new FolderBrowserDialog();
			dlg.SelectedPath = Path.GetFullPath(dir);
			if (dlg.ShowDialog() != DialogResult.OK) {
				return null;
			}

			dir = dlg.SelectedPath;
			string pwd = Environment.CurrentDirectory;
			if (dir.StartsWith(pwd)) {
				dir = dir.Substring(pwd.Length + 1);
			}

			return dir;
		}

		private void LoadFile(string fn) {
			BinaryReader br = new BinaryReader(new FileStream(fn, FileMode.Open, FileAccess.Read));
			int num_channel = br.ReadInt32();
			int num_sample = br.ReadInt32();
			double max_value = br.ReadDouble();
			double min_value = br.ReadDouble();

			start_time = br.ReadInt32();
			end_time = br.ReadInt32();

			channel_name = new string[num_channel];
			target_data = new int[num_channel, num_sample];
			nontarget_data = new int[num_channel, num_sample];
			zeros = new int[num_channel];
			double range = max_value - min_value;

			chnmin = new double[num_channel];
			chnmax = new double[num_channel];

			for (int i = 0; i < num_channel; i++) {
				int len = br.ReadInt32();
				channel_name[i] = System.Text.ASCIIEncoding.ASCII.GetString(br.ReadBytes(len));

				double cmax = br.ReadDouble();
				double cmin = br.ReadDouble();

				chnmin[i] = cmin;
				chnmax[i] = cmax;

				Console.WriteLine("Channel {0} range: {1} - {2}", channel_name[i], cmax, cmin);

				min_value = cmin;
				range = cmax - cmin;
				if (range == 0.0) range = 1.0;

				for (int j = 0; j < num_sample; j++) {
					target_data[i, j] = (int) ((br.ReadDouble() - min_value) * MAX_VALUE / range);
				}
				for (int j = 0; j < num_sample; j++) {
					nontarget_data[i, j] = (int) ((br.ReadDouble() - min_value) * MAX_VALUE / range);
				}
				zeros[i] = (int) ((0 - min_value) * MAX_VALUE / range);
				if (zeros[i] < 0) zeros[i] = 0;
				if (zeros[i] > MAX_VALUE) zeros[i] = MAX_VALUE;
			}
			br.Close();

			panel1.AutoScroll = true;
			num_column = (panel1.ClientSize.Width - MARGIN_X) / (MARGIN_X + num_sample);
			if (num_column < 1) num_column = 1;

			channel_sel = new bool[num_channel];
			for (int ic = 0; ic < num_channel; ic++) channel_sel[ic] = false;
			// check used channel names in configuration file
			string line = _cfgctrl.P300UsedChannels;
            if (line != null) {
                string[] ucl = ResManager.SplitString(line, ",");
                bool reset = false;
                foreach (string cn in ucl) {
                    int ic = 0;
                    for (; ic < num_channel; ic++) {
                        if (cn == channel_name[ic]) break;
                    }
                    if (ic == num_channel) {
                        // not found
                        reset = true;
                    } else channel_sel[ic] = true;
                }
                btnSelectChannel.Enabled = reset;
            } else {
                // select all channels
                for (int ic = 0; ic < num_channel; ic++) {
                    channel_sel[ic] = true;
                }
                _cfgctrl.P300UsedChannels = string.Join(",", channel_name);
            }

			channel_reg = new Rectangle[num_channel];

			panel1.AutoScrollMinSize = new Size(num_column * (num_sample + MARGIN_X) + MARGIN_X,
				(MAX_VALUE + MARGIN_Y) * ((num_channel + num_column - 1) / num_column) + MARGIN_Y);
			panel1.Invalidate();

			statusBar1.Text = fn;
			ace_filename = fn;

            btnSelectChannel.Enabled = true;
        }

		private void panel1_Paint(object sender, System.Windows.Forms.PaintEventArgs e) {
			Graphics g =  e.Graphics;
			g.Clear(Color.White);

			if (target_data == null) return;

			Font f = new Font("Arial", 8);
			SolidBrush br = new SolidBrush(Color.Black);

			Pen p1 = new Pen(Color.Red, 1);
			Pen p2 = new Pen(Color.Blue, 2);
			Pen p3 = new Pen(Color.Black);
			//atchBrush hb = new HatchBrush(HatchStyle.DashedHorizontal, Color.DarkSlateGray, Color.Transparent);
			Pen p4 = new Pen(Color.Gray);

			int dl = target_data.GetUpperBound(1) + 1;
			Point[] points = new Point[dl];

			for (int ic = 0; ic < channel_name.Length; ic++) {
				int row = ic / num_column;
				int col = ic % num_column;

				int x = col * (MARGIN_X + dl) + MARGIN_X + panel1.AutoScrollPosition.X;
				int y = row * (MARGIN_Y + MAX_VALUE) + MARGIN_Y + panel1.AutoScrollPosition.Y;
				g.DrawRectangle(p3, x, y, dl, MAX_VALUE);

				for (int i = 0; i < end_time; i += 100) {
					if (i < start_time) continue;

					int x1 = x + dl * (i - start_time) / (end_time - start_time);
					g.DrawLine(p3, x1, y + MAX_VALUE, x1, y + MAX_VALUE - 5);

					string mstr = i.ToString();
					int mw = (int) g.MeasureString(mstr, f).Width;
					g.DrawString(mstr, f, br, x1 - mw / 2, y + MAX_VALUE);
				}

				SizeF ssz = g.MeasureString(channel_name[ic], f);
				int cw = (int) ssz.Width;
				int ch = (int) ssz.Height - 4;
				int cx = x + (dl - cw - ch - 2) / 2;

				// check box
				channel_reg[ic] = new Rectangle(cx, y + 2, ch, ch);
				g.DrawRectangle(p3, channel_reg[ic]);
				if (channel_sel[ic]) {
					g.DrawLine(p3, cx, y + ch + 2, cx + ch, y + 2);
					g.DrawLine(p3, cx, y + 2, cx + ch, y + ch + 2);
				}
				g.DrawString(channel_name[ic], f, br, cx + ch + 2, y);

				for (int i = 0; i < dl; i++) {
					points[i].X = x + i;
					points[i].Y = y + MAX_VALUE - nontarget_data[ic, i];
				}
				g.DrawLines(p2, points);

				for (int i = 0; i < dl; i++) {
					points[i].X = x + i;
					points[i].Y = y + MAX_VALUE - target_data[ic, i];
				}
				g.DrawLines(p1, points);

				//string range = string.Format("{0,4} - {1,4}", chnmin[ic], chnmax[ic]);
				//g.DrawString(range, f, br, x, y);
			}
		}

		private void panel1_Resize(object sender, System.EventArgs e) {
			int dl = 1;
			if (target_data != null) dl = target_data.GetUpperBound(1) + 1;
			int num_channel = 1;
			if (target_data != null) num_channel = target_data.GetUpperBound(0) + 1;

			int col = (panel1.ClientSize.Width - MARGIN_X) / (MARGIN_X + dl);
			if (col < 1) col = 1;
			if (col != num_column) {
				num_column = col;
				panel1.AutoScrollMinSize = new Size(num_column * (dl + MARGIN_X) + MARGIN_X,
					(MAX_VALUE + MARGIN_Y) * ((num_channel + num_column - 1) / num_column) + MARGIN_Y);
				panel1.Invalidate();
			}
		}

		private string ace_filename;

		private void btnBrowACE_Click(object sender, System.EventArgs e) {
			// It is strange: after this call, current directory changed!
			string cdir = Environment.CurrentDirectory;
			OpenFileDialog dlg = new OpenFileDialog();
            dlg.RestoreDirectory = true;
			dlg.Filter = "Average Chanel EEG (*.ace)|*.ace|All files(*.*)|*.*";
			if (dlg.ShowDialog() == DialogResult.OK) {
				LoadFile(dlg.FileName);
			}
			Environment.CurrentDirectory = cdir;
		}

		private void btnShowTrainACE_Click(object sender, System.EventArgs e) {
			CreateDescFile(1);
            string tf = _cfgctrl.TrainModelFileList;
            if (!string.IsNullOrEmpty(tf))
			{
                if (P300Processor.P300Util_EEGChannelAverage(tf)) {
                    tf = Path.Combine(ModelTrainingPath, "chan_avg_eeg.ace");
                    if (File.Exists(tf)) LoadFile(tf);
                }
			}
		}

		private void btnShowTestACE_Click(object sender, System.EventArgs e) {
			CreateDescFile(2);
			string tf = _cfgctrl.TrainRejectFileList;
            if (!string.IsNullOrEmpty(tf)) {
                if (P300Processor.P300Util_EEGChannelAverage(tf)) {
                    tf = Path.Combine(RejectionTrainingPath, "chan_avg_eeg.ace");
                    if (File.Exists(tf)) LoadFile(tf);
                }
            }
		}

		private void panel1_Click(object sender, System.EventArgs e) {
			if (channel_reg == null) return;

			Point mp = panel1.PointToClient(Control.MousePosition);

			int dl = target_data.GetUpperBound(1) + 1;
			for (int ic = 0; ic < channel_name.Length; ic++) {
				if (channel_reg[ic].Contains(mp)) {
					channel_sel[ic] = !channel_sel[ic];
					panel1.Invalidate(channel_reg[ic]);
					btnSelectChannel.Enabled = true;
					break;
				}
			}
		}

		private void btnSelectChannel_Click(object sender, System.EventArgs e) {
            for (int i = 0; i < channel_sel.Length; i++) channel_sel[i] = true;
            panel1.Invalidate();
        }

        private bool SelectChannels()
        {
            if (channel_sel != null) {
                _cfgctrl.P300UsedChannels =
                    string.Join(",", channel_name.Where((n, i) => channel_sel[i]).ToArray());
            }

            var all_names = _cfgctrl.ChannelNames.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var sel_names = _cfgctrl.P300UsedChannels.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (sel_names == null || sel_names.Length <= 0 || sel_names.Any(x => Array.IndexOf<string>(all_names, x) < 0)) return false;
            return true;
        }

        private void btnCfg_Click(object sender, System.EventArgs e)
		{
			_cfgctrl.Resource.ShowDialog();		
		}

		private void buttonFilter_Click(object sender, System.EventArgs e) {
            //FilterDesignForm frm = new FilterDesignForm();
            //if (frm.ShowDialog() == DialogResult.OK) {
            //    cfg.SetConfigValue("ModelTrainingSetting", "Filter_A", frm.Filter_A);
            //    cfg.SetConfigValue("ModelTrainingSetting", "Filter_B", frm.Filter_B);
            //    cfg.SaveFile();
            //}
		}

		public string ModelDirectory {
            get
            {
                return "Model";
            }
		}

		private void buttonSaveWindow_Click(object sender, System.EventArgs e) {
			if (target_data == null) return;

			FileInfo finf = new FileInfo(ace_filename);

			string svdir = Environment.CurrentDirectory;
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.FileName = finf.DirectoryName + "\\P300Avg";

			dlg.Filter = "Jpeg file(*.jpg)|*.jpg|All files|*.*";
			DialogResult rv = dlg.ShowDialog();
			Environment.CurrentDirectory = svdir;

			if (rv != DialogResult.OK) return;

			Size dsz = panel1.AutoScrollMinSize;
			Bitmap bmp = new Bitmap(dsz.Width, dsz.Height);
			Graphics g = Graphics.FromImage(bmp);
			g.Clear(Color.White);

			Font f = new Font("Arial", 8);
			SolidBrush br = new SolidBrush(Color.Black);

			Pen p1 = new Pen(Color.Red, 1);
			Pen p2 = new Pen(Color.Blue, 2);
			Pen p3 = new Pen(Color.Black);
			Pen p4 = new Pen(Color.Gray);

			int dl = target_data.GetUpperBound(1) + 1;
			Point[] points = new Point[dl];

			for (int ic = 0; ic < channel_name.Length; ic++) {
				int row = ic / num_column;
				int col = ic % num_column;

				int x = col * (MARGIN_X + dl) + MARGIN_X;
				int y = row * (MARGIN_Y + MAX_VALUE) + MARGIN_Y;
				g.DrawRectangle(p3, x, y, dl, MAX_VALUE);

				for (int i = 0; i < end_time; i += 100) {
					if (i < start_time) continue;

					int x1 = x + dl * (i - start_time) / (end_time - start_time);
					g.DrawLine(p3, x1, y + MAX_VALUE, x1, y + MAX_VALUE - 5);

					string mstr = i.ToString();
					int mw = (int) g.MeasureString(mstr, f).Width;
					g.DrawString(mstr, f, br, x1 - mw / 2, y + MAX_VALUE);
				}

				SizeF ssz = g.MeasureString(channel_name[ic], f);
				int cw = (int) ssz.Width;
				int ch = (int) ssz.Height - 4;
				int cx = x + (dl - cw - ch - 2) / 2;

				// check box
				channel_reg[ic] = new Rectangle(cx, y + 2, ch, ch);
				g.DrawRectangle(p3, channel_reg[ic]);
				if (channel_sel[ic]) {
					g.DrawLine(p3, cx, y + ch + 2, cx + ch, y + 2);
					g.DrawLine(p3, cx, y + 2, cx + ch, y + ch + 2);
				}
				g.DrawString(channel_name[ic], f, br, cx + ch + 2, y);

				for (int i = 0; i < dl; i++) {
					points[i].X = x + i;
					points[i].Y = y + MAX_VALUE - nontarget_data[ic, i];
				}
				g.DrawLines(p2, points);

				for (int i = 0; i < dl; i++) {
					points[i].X = x + i;
					points[i].Y = y + MAX_VALUE - target_data[ic, i];
				}
				g.DrawLines(p1, points);

				//string range = string.Format("{0,4} - {1,4}", chnmin[ic], chnmax[ic]);
				//g.DrawString(range, f, br, x, y);
			}
			g.Dispose();
			bmp.Save(dlg.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
			bmp.Dispose();
		}
	}
}
