using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using System.IO;

using BCILib.Util;

namespace BCILib.App
{
	/// <summary>
	/// Summary description for SelectSubjectForm.
	/// </summary>
	public class SelectSubjectForm : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private ResManager _rm = null;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox comboSubject;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ComboBox comboDate;

		private bool cfg_user_with_date = false;

		public SelectSubjectForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			_rm =  BCIApplication.AppResource;
			RefreshContent();
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
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.comboSubject = new System.Windows.Forms.ComboBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.comboDate = new System.Windows.Forms.ComboBox();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(69, 144);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.TabIndex = 2;
			this.buttonOK.Text = "Ok";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(176, 144);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.TabIndex = 2;
			this.buttonCancel.Text = "Cancel";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 24);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(80, 23);
			this.label2.TabIndex = 0;
			this.label2.Text = "User Account";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboSubject
			// 
			this.comboSubject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboSubject.Location = new System.Drawing.Point(112, 24);
			this.comboSubject.Name = "comboSubject";
			this.comboSubject.Size = new System.Drawing.Size(128, 21);
			this.comboSubject.TabIndex = 1;
			this.comboSubject.SelectedIndexChanged += new System.EventHandler(this.comboSubject_SelectedIndexChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.comboSubject);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.comboDate);
			this.groupBox1.Location = new System.Drawing.Point(32, 16);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(264, 104);
			this.groupBox1.TabIndex = 4;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Subject";
			// 
			// comboDate
			// 
			this.comboDate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboDate.Location = new System.Drawing.Point(112, 56);
			this.comboDate.Name = "comboDate";
			this.comboDate.Size = new System.Drawing.Size(128, 21);
			this.comboDate.TabIndex = 1;
			this.comboDate.Visible = false;
			// 
			// SelectSubjectForm
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(320, 190);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.buttonCancel);
			this.Name = "SelectSubjectForm";
			this.Text = "System Configuration Form";
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonOK_Click(object sender, System.EventArgs e) {
			DialogResult = DialogResult.OK;
			Close();
		}

		private void RefreshContent() {
			// Subject
			comboSubject.Items.Clear();

			BCIApplication.AppResource.GetConfigValue("UserWithDate", ref cfg_user_with_date);

			string udir = BCIApplication.UsersRoot;
			if (!Directory.Exists(udir)) {
				buttonOK.Enabled = false;
				return;
			}

			string [] dirs = Directory.GetDirectories(udir);
			foreach (string dir in dirs) {
				string cdir = Path.GetFileName(dir);
				int uno = comboSubject.Items.Add(cdir);
			}
			if (comboSubject.Items.Count > 0) comboSubject.SelectedIndex = 0;
		}

		private void comboSubject_SelectedIndexChanged(object sender, System.EventArgs e) {
			comboDate.Visible = false;

			string subname = comboSubject.Text;

			string dir = Path.Combine(BCIApplication.UsersRoot, subname);
			string cfn = Path.Combine(dir, "user.cfg");
			if (!File.Exists(cfn)) return;
		
			ResManager rm = new ResManager(cfn);
			string line = rm.GetConfigValue("UserWithDate");
			if (line == null) return;
			line = line.ToLower();
			if (line.CompareTo("true") != 0) return;

			comboDate.Visible = true;
			comboDate.Items.Clear();

			DirectoryInfo[] dirl = new DirectoryInfo(dir).GetDirectories();
			foreach (DirectoryInfo di in dirl) {
				bool ok = true;
				foreach (char ci in di.Name) {
					if (!char.IsDigit(ci)) {
						ok = false;
						break;
					}
				}
				if (!ok) continue;

				comboDate.Items.Add(di);
			}

			if (comboDate.Items.Count > 0) {
				comboDate.SelectedIndex = 0;
			}
		}

		public string Subject {
			get {
				return comboSubject.Text;
			}
		}

		public string SubjectWithDate {
			get {
				if (comboDate.Text.Length > 0) {
					return Path.Combine(comboSubject.Text, comboDate.Text);
				} else {
					return comboSubject.Text;
				}
			}
		}
	}
}
