using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;

using BCILib.Util;

namespace BCILib.App
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class TestDirSpecForm : System.Windows.Forms.Form
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
        private System.Windows.Forms.Button buttonNoRecording;
        private IContainer components;
        private ToolTip toolTip1;

		public static DialogResult dlg_result;

        public TestDirSpecForm()
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
            this.buttonNoRecording = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(32, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "App Name:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textTrainName
            // 
            this.textTrainName.Location = new System.Drawing.Point(136, 64);
            this.textTrainName.Name = "textTrainName";
            this.textTrainName.Size = new System.Drawing.Size(176, 20);
            this.textTrainName.TabIndex = 4;
            this.textTrainName.TextChanged += new System.EventHandler(this.textTrainName_TextChanged);
            // 
            // textTrainDir
            // 
            this.textTrainDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textTrainDir.Location = new System.Drawing.Point(136, 104);
            this.textTrainDir.Name = "textTrainDir";
            this.textTrainDir.ReadOnly = true;
            this.textTrainDir.Size = new System.Drawing.Size(321, 20);
            this.textTrainDir.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(32, 104);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 23);
            this.label2.TabIndex = 5;
            this.label2.Text = "Testing Directory:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(32, 184);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "&OK";
            this.btnOK.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(304, 184);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.Click += new System.EventHandler(this.button2_Click);
            // 
            // checkClearNeeded
            // 
            this.checkClearNeeded.Location = new System.Drawing.Point(136, 144);
            this.checkClearNeeded.Name = "checkClearNeeded";
            this.checkClearNeeded.Size = new System.Drawing.Size(160, 24);
            this.checkClearNeeded.TabIndex = 6;
            this.checkClearNeeded.Text = "Clear Before Training";
            // 
            // textDataDir
            // 
            this.textDataDir.Location = new System.Drawing.Point(136, 24);
            this.textDataDir.Name = "textDataDir";
            this.textDataDir.ReadOnly = true;
            this.textDataDir.Size = new System.Drawing.Size(176, 20);
            this.textDataDir.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(32, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 23);
            this.label3.TabIndex = 0;
            this.label3.Text = "Data Directory";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonNoRecording
            // 
            this.buttonNoRecording.Location = new System.Drawing.Point(152, 184);
            this.buttonNoRecording.Name = "buttonNoRecording";
            this.buttonNoRecording.Size = new System.Drawing.Size(112, 23);
            this.buttonNoRecording.TabIndex = 1;
            this.buttonNoRecording.Text = "&No Data Recording";
            this.buttonNoRecording.Visible = false;
            this.buttonNoRecording.Click += new System.EventHandler(this.buttonNoRecording_Click);
            // 
            // TestDirSpecForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(467, 230);
            this.ControlBox = false;
            this.Controls.Add(this.checkClearNeeded);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.textTrainName);
            this.Controls.Add(this.textTrainDir);
            this.Controls.Add(this.textDataDir);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.buttonNoRecording);
            this.Name = "TestDirSpecForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Testing Specification";
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

		private void SetDataDirectory () {
			string trname = textTrainName.Text;
			string trdir = String.Format("{0}_{1:yyyy-MM-dd}", trname, DateTime.Now);
			trdir = Path.Combine(textDataDir.Text, trdir);

            trdir = Path.Combine(BCIApplication.DateString, trdir);
            toolTip1.SetToolTip(textTrainDir, trdir);


            textTrainDir.Text = trdir;
			bool bExist = Directory.Exists(trdir);
			checkClearNeeded.Enabled = bExist;
		}

        private void SetNewDiretory(string trname)
        {
            int n = 1;
            while (true) {
                TestSubject = string.Format("{0}_{1}", trname, n);
                string fn = DataDirectory;
                if (!Directory.Exists(fn)) break;
                n++;
            }
        }

		private void textTrainName_TextChanged(object sender, System.EventArgs e) {
			SetDataDirectory();
		}

		protected override void OnLoad(EventArgs e) {
			SetDataDirectory();
			base.OnLoad (e);
		}

		public string DataDirectory {
			get {
				return textTrainDir.Text;
			}
		}

		public string TestSubject {
			get {
				return textTrainName.Text;
			}
			set {
				textTrainName.Text = value;
				SetDataDirectory();
			}
		}

		public bool ClearingNeeded {
			get {
				return checkClearNeeded.Checked;
			}
		}

		public static string GetProcPath(ResManager cfg) {
			return GetProcPath(cfg, true);
		}

		private void buttonNoRecording_Click(object sender, System.EventArgs e) {
			this.DialogResult = DialogResult.No;
		}

		public static string GetProcPath(ResManager cfg, bool confirm) {
            return GetProcPath(cfg, confirm, false);
        }

        public static string GetProcPath(ResManager cfg, bool confirm, bool CreateNew)
        {
			string dpath = cfg.GetConfigValue("EEG", "TestDataDir");
			if (dpath == null) dpath = "DataTesting";
			string trname = cfg.GetConfigValue("EEG", "AppName");

			// 20050310: Processing directory
			TestDirSpecForm tsf = new TestDirSpecForm();
			tsf.textDataDir.Text = dpath;
			tsf.TestSubject = trname;

            if (CreateNew) {
                tsf.SetNewDiretory(trname);
            }

			dlg_result = DialogResult.OK;
			if (confirm) {
				dlg_result = tsf.ShowDialog();
				if (dlg_result != DialogResult.OK) return null;
			}

			trname = tsf.DataDirectory;

			if (tsf.checkClearNeeded.Checked) {
				// remove all contents inside this directory
				try {
					Directory.Delete(trname, true);
				} catch (Exception ){ // ed
                    //MessageBox.Show(ed.Message);
				}
			}

			if (!Directory.Exists(trname)) Directory.CreateDirectory(trname);

			cfg.SaveFile();

			return trname;
        }
	}
}
