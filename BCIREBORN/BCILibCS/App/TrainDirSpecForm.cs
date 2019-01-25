using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;

using BCILib.Util;
using BCILib.App;

namespace BCILib
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class TrainDirSpecForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.TextBox textTrainName;
        private System.Windows.Forms.TextBox textTrainDir;
		private System.Windows.Forms.CheckBox checkClearNeeded;
		private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textDataDir;
        private ToolTip toolTip1;
        private IContainer components;

		public TrainDirSpecForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
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
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.textTrainName = new System.Windows.Forms.TextBox();
            this.textTrainDir = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.checkClearNeeded = new System.Windows.Forms.CheckBox();
            this.textDataDir = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(24, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "App Name:";
            // 
            // textTrainName
            // 
            this.textTrainName.Location = new System.Drawing.Point(128, 64);
            this.textTrainName.Name = "textTrainName";
            this.textTrainName.Size = new System.Drawing.Size(176, 20);
            this.textTrainName.TabIndex = 1;
            this.textTrainName.TextChanged += new System.EventHandler(this.textTrainName_TextChanged);
            // 
            // textTrainDir
            // 
            this.textTrainDir.Location = new System.Drawing.Point(128, 109);
            this.textTrainDir.Name = "textTrainDir";
            this.textTrainDir.ReadOnly = true;
            this.textTrainDir.Size = new System.Drawing.Size(176, 20);
            this.textTrainDir.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(24, 109);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 23);
            this.label2.TabIndex = 5;
            this.label2.Text = "Training Directory:";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(48, 173);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 7;
            this.btnOK.Text = "&OK";
            this.btnOK.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(192, 173);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.Click += new System.EventHandler(this.button2_Click);
            // 
            // checkClearNeeded
            // 
            this.checkClearNeeded.Location = new System.Drawing.Point(136, 141);
            this.checkClearNeeded.Name = "checkClearNeeded";
            this.checkClearNeeded.Size = new System.Drawing.Size(160, 24);
            this.checkClearNeeded.TabIndex = 9;
            this.checkClearNeeded.Text = "Clear Before Training";
            // 
            // textDataDir
            // 
            this.textDataDir.Location = new System.Drawing.Point(128, 24);
            this.textDataDir.Name = "textDataDir";
            this.textDataDir.ReadOnly = true;
            this.textDataDir.Size = new System.Drawing.Size(176, 20);
            this.textDataDir.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(24, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 23);
            this.label3.TabIndex = 0;
            this.label3.Text = "DataDirectory";
            // 
            // TrainDirSpecForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(328, 243);
            this.ControlBox = false;
            this.Controls.Add(this.checkClearNeeded);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.textTrainName);
            this.Controls.Add(this.textTrainDir);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.textDataDir);
            this.Controls.Add(this.label3);
            this.Name = "TrainDirSpecForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Training Specification";
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void button2_Click(object sender, System.EventArgs e) {
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void button1_Click(object sender, System.EventArgs e) {
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void SetTrainingDirectory () {
			string trname = textTrainName.Text;
			string trdir = String.Format("{0}_{1:yyyy-MM-dd}", trname, DateTime.Now);
			trdir += "_Training";
			trdir = Path.Combine(textDataDir.Text, trdir);

            // ccwang: directory starting with date
			trdir = Path.Combine(BCIApplication.DateString, trdir);

			bool bExist = Directory.Exists(trdir);
			checkClearNeeded.Enabled = bExist;
            textTrainDir.Text = trdir;
            toolTip1.SetToolTip(textTrainDir, trdir);
		}

		private void textTrainName_TextChanged(object sender, System.EventArgs e) {
			SetTrainingDirectory();
		}

		protected override void OnLoad(EventArgs e) {
			SetTrainingDirectory();
			base.OnLoad (e);
		}

		public string TrainingDirectory {
			get {
				return textTrainDir.Text;
			}
		}

		public string TrainSubject {
			get {
				return textTrainName.Text;
			}
			set {
				textTrainName.Text = value;
//				string trainee = value;
//				int len = trainee.Length;
//
//				// remove "Data/"
//				if (len > 5) {
//					string prefix = trainee.Substring(0, 5).ToLower();
//					if (prefix.Equals("data\\") || prefix.Equals("data/")) {
//						trainee = trainee.Substring(5);
//						len -= 5;
//					}
//				}
//
//				// remove suffix _Training or _Proc
//				if (trainee.EndsWith("_Training")) {
//					len -= 9;
//					trainee = trainee.Substring(0, len);
//				} else if (trainee.EndsWith("_Proc")) {
//					len -= 5;
//					trainee = trainee.Substring(0, len);
//				}
//
//				// remove _test
//				if (trainee.EndsWith("_test")) 
//				{
//					len -= 5;
//					trainee = trainee.Substring(0, len);
//				} 
//				else if (trainee.EndsWith("_model")) 
//				{
//					len -= 6;
//					trainee = trainee.Substring(0, len);
//				}
//				
//				// take off date suffix
//				if (len > 11) {
//					char [] al = trainee.Substring(len - 11).ToCharArray();
//					if (al[0] == '_' && al[5] == '-' && al[8] == '-') {
//						try {
//							int year = int.Parse(new string(al, 1, 4));
//							int month = int.Parse(new string(al, 6, 2));
//							int date = int.Parse(new string(al, 9, 2));
//							trainee = trainee.Substring(0, len - 11);
//							len -= 11;
//						} catch (Exception) {
//						}
//					}
//				}
//
//				textTrainName.Text = trainee;
				SetTrainingDirectory();
//
//				bool DirExists = Directory.Exists(textTrainDir.Text);
//				radioTraining.Checked = !DirExists;
//				radioTesting.Checked = DirExists;
			}
		}

		private bool ClearingNeeded {
			get {
				return checkClearNeeded.Checked;
			}
		}

        public string ChkTrainingPath(ResManager cfg)
        {
            return ChkTrainingPath(cfg, false);
        }

		public string ChkTrainingPath(ResManager cfg, bool UserVerify) {
			try {
				string dpath = cfg.GetConfigValue("EEG", "TrainDataDir");
				if (dpath == null) dpath = "DataTraining";
				textDataDir.Text = dpath;

				string trname = cfg.GetConfigValue("EEG", "AppName");

				int n = 1;
				while (true) {
					TrainSubject = string.Format("{0}_{1}", trname, n);
					string fn = TrainingDirectory;
					if (!Directory.Exists(fn)) break;
					n++;
				}

				if (UserVerify && ShowDialog() != DialogResult.OK) return null;

				trname = TrainingDirectory;
				if (ClearingNeeded) {
					// remove all contents under this directory
					try {
						Directory.Delete(trname, true);
					} catch (Exception ed) {
                        //MessageBox.Show(ed.Message);
                        Console.WriteLine("Error deleteing directory {0}: {1}", trname, ed.Message);
					}
				}

				if (!Directory.Exists(trname)) Directory.CreateDirectory(trname);

                //cfg.SetConfigValue("EEG", "EpochDir", trname);
				cfg.SaveFile();
				return trname;
			} catch (Exception e) {
				MessageBox.Show(e.Message);
				return null;
			}
		}

        public static string GetTrainingPath(ResManager cfg)
        {
            return GetTrainingPath(cfg, false);
        }

        public static string GetTrainingPath(ResManager cfg, bool UserVerify)
        {
			TrainDirSpecForm tsf = new TrainDirSpecForm();
			return tsf.ChkTrainingPath(cfg, UserVerify);
		}
    }
}
