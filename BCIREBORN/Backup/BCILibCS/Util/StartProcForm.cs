using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

using System.IO;

namespace BCILib.Util
{
	/// <summary>
	/// Summary description for StartProcForm.
	/// </summary>
	public class StartProcForm : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textCommand;
		private System.Windows.Forms.Button buttonCopyCmd;
		private System.Windows.Forms.TextBox textOutput;
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.Button buttonRunCmd;

		private Process proc = new Process();

		public StartProcForm(ProcessStartInfo pinf)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			proc.StartInfo = pinf;
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
			this.textCommand = new System.Windows.Forms.TextBox();
			this.buttonCopyCmd = new System.Windows.Forms.Button();
			this.textOutput = new System.Windows.Forms.TextBox();
			this.buttonOk = new System.Windows.Forms.Button();
			this.buttonRunCmd = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(80, 20);
			this.label1.TabIndex = 0;
			this.label1.Text = "Command:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// textCommand
			// 
			this.textCommand.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.textCommand.Location = new System.Drawing.Point(96, 8);
			this.textCommand.Name = "textCommand";
			this.textCommand.ReadOnly = true;
			this.textCommand.Size = new System.Drawing.Size(512, 20);
			this.textCommand.TabIndex = 1;
			this.textCommand.Text = "textBox1";
			// 
			// buttonCopyCmd
			// 
			this.buttonCopyCmd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCopyCmd.Location = new System.Drawing.Point(616, 8);
			this.buttonCopyCmd.Name = "buttonCopyCmd";
			this.buttonCopyCmd.Size = new System.Drawing.Size(40, 23);
			this.buttonCopyCmd.TabIndex = 2;
			this.buttonCopyCmd.Text = "Copy";
			this.buttonCopyCmd.Click += new System.EventHandler(this.buttonCopyCmd_Click);
			// 
			// textOutput
			// 
			this.textOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.textOutput.Location = new System.Drawing.Point(8, 40);
			this.textOutput.Multiline = true;
			this.textOutput.Name = "textOutput";
			this.textOutput.ReadOnly = true;
			this.textOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textOutput.Size = new System.Drawing.Size(696, 416);
			this.textOutput.TabIndex = 3;
			this.textOutput.Text = "";
			// 
			// buttonOk
			// 
			this.buttonOk.Location = new System.Drawing.Point(312, 472);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.TabIndex = 4;
			this.buttonOk.Text = "Ok";
			this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
			// 
			// buttonRunCmd
			// 
			this.buttonRunCmd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonRunCmd.Location = new System.Drawing.Point(664, 8);
			this.buttonRunCmd.Name = "buttonRunCmd";
			this.buttonRunCmd.Size = new System.Drawing.Size(40, 23);
			this.buttonRunCmd.TabIndex = 2;
			this.buttonRunCmd.Text = "Run";
			this.buttonRunCmd.Click += new System.EventHandler(this.buttonRunCmd_Click);
			// 
			// StartProcForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(728, 502);
			this.Controls.Add(this.buttonOk);
			this.Controls.Add(this.textOutput);
			this.Controls.Add(this.buttonCopyCmd);
			this.Controls.Add(this.textCommand);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.buttonRunCmd);
			this.KeyPreview = true;
			this.Name = "StartProcForm";
			this.Text = "StartProcForm";
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.StartProcForm_KeyDown);
			this.Load += new System.EventHandler(this.StartProcForm_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonCopyCmd_Click(object sender, System.EventArgs e) {
			Clipboard.SetDataObject(textCommand.Text, true);
		}

        private Thread _thd_running = null;

		public void Run() {
			this.ControlBox = false;
			buttonOk.Enabled = false;
			buttonRunCmd.Enabled = false;
			_thd_running = new Thread(new ThreadStart(StartCommand));
			_thd_running.Start();
		}

        delegate void dgarg0();

		private void StartCommand() {
			try {
				ExeCommand();
			} catch (Exception e) {
                Invoke((dgarg0)delegate
                {
                    textOutput.AppendText(e.ToString());
                    textOutput.AppendText("\r\n");
                });
			}

            Invoke((dgarg0)delegate
            {
                this.ControlBox = true;
                buttonOk.Enabled = true;
                buttonRunCmd.Enabled = true;
            });
		}

		private void ExeCommand() {
			ProcessStartInfo pinf = proc.StartInfo;
			pinf.RedirectStandardInput = true;
			pinf.RedirectStandardOutput = true;
			pinf.RedirectStandardError = true;
			pinf.UseShellExecute = false;
			pinf.CreateNoWindow = true;

			proc.Start();

			Thread thd_stdout = new Thread(new ThreadStart(ExeReadStdout));
			thd_stdout.Start();

			Thread thd_stderr = new Thread(new ThreadStart(ExeReadStderr));
			thd_stderr.Start();

            Invoke((dgarg0)delegate
            {
                textOutput.AppendText("Wait for exit...");
                textOutput.AppendText("\r\n");
            });

			proc.WaitForExit();
			exit_code = proc.ExitCode;

            Invoke((dgarg0)delegate
            {
                textOutput.AppendText(string.Format("Exist code: {0}\r\n", exit_code));
            });

			thd_stdout.Join();
			thd_stderr.Join();
			proc.Close();
		}

		private void ExeReadStdout() {
			string line;
			while ((line = proc.StandardOutput.ReadLine()) != null) {
                Invoke((dgarg0)delegate
                {
                    textOutput.AppendText(line);
                    textOutput.AppendText("\r\n");
                });
			}			
		}

		private void ExeReadStderr() {
			string line;
			while ((line = proc.StandardError.ReadLine()) != null) {
                Invoke((dgarg0)delegate
                {
                    textOutput.AppendText(line);
                    textOutput.AppendText("\r\n");
                });
			}			
		}

		private void StartProcForm_Load(object sender, System.EventArgs e) {
			ProcessStartInfo pinf = proc.StartInfo;
			if (pinf != null) {
				textCommand.Text = pinf.FileName + " " + pinf.Arguments;
				textCommand.Select(0, 0);
			}
		}

		private void buttonRunCmd_Click(object sender, System.EventArgs e) {
			Run();
		}

		private void buttonOk_Click(object sender, System.EventArgs e) {
			this.Close();
		}

		private int exit_code = -1;

		private void StartProcForm_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			if (e.KeyCode == Keys.M) {
				System.Text.StringBuilder sb = new System.Text.StringBuilder();
				sb.Append(Path.GetFileNameWithoutExtension(proc.StartInfo.FileName));
				sb.Append("(");

				char[] args = proc.StartInfo.Arguments.ToCharArray();
				int i = 0;
				int n = args.Length;
				int pn = 0;
				bool sp = true;
				while (i < n) {
					if (char.IsWhiteSpace(args[i])) {
						if (!sp) {
							sp = true;
							sb.Append('\'');
						}
						i++;
					} else {
						if (sp) {
							if (pn > 0) sb.Append(",");
							sb.Append("'");
							pn++;
							sp = false;
						}

						if (args[i] == '"') {
							// sb.Append('\'');
							i++;
							while (i < n && args[i] != '"') {
								sb.Append(args[i]);
								i++;
							}
							//sb.Append('\'');
							i++;
						} else {
							sb.Append(args[i]);
							i++;
						}
					}
				}

				if (!sp) {
					sb.Append("'");
				}

				sb.Append(")");

				textOutput.AppendText(sb.ToString());
				textOutput.AppendText("\r\n");
				Clipboard.SetDataObject(sb.ToString(), true);
			}
		}

		public int ExitCode {
			get {
				return exit_code;
			}
		}

		public string OutputString {
			get {
				return textOutput.Text;
			}
		}

        internal bool IsRunning
        {
            get
            {
                return (_thd_running != null && _thd_running.IsAlive);
            }
        }
    }
}
