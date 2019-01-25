using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

using BCILib.Util;
using GameCommand = BCILib.App.GameCommand;

namespace BCILib.Concentration
{
	/// <summary>
	/// Summary description for StoryForm.
	/// </summary>
	internal class StoryForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.Button button1;
		private WebBrowser webBrowser1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox TextTime;
		private System.Windows.Forms.Timer timer1;
        private BindingSource bindingSource1;
		private System.ComponentModel.IContainer components;

		public StoryForm()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StoryForm));
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.TextTime = new System.Windows.Forms.TextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // comboBox1
            // 
            this.comboBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox1.Items.AddRange(new object[] {
            "http://bygosh.com/kidsstories.htm",
            "http://www.magickeys.com/books/",
            "http://bygosh.com/stories_title.htm",
            "http://www.kidsgen.com/short_stories/"});
            this.comboBox1.Location = new System.Drawing.Point(8, 8);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(840, 21);
            this.comboBox1.TabIndex = 1;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            this.comboBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.comboBox1_KeyDown);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(856, 8);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(48, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "&Go";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // webBrowser1
            // 
            this.webBrowser1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.webBrowser1.Location = new System.Drawing.Point(8, 40);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(896, 544);
            this.webBrowser1.TabIndex = 3;
            // 
            // button2
            // 
            this.button2.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.button2.Location = new System.Drawing.Point(728, 592);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "&OK";
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.button3.Location = new System.Drawing.Point(832, 592);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 4;
            this.button3.Text = "&Cancel";
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.Location = new System.Drawing.Point(8, 592);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 23);
            this.label1.TabIndex = 5;
            this.label1.Text = "Elapsed Time: ";
            // 
            // TextTime
            // 
            this.TextTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.TextTime.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TextTime.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextTime.Location = new System.Drawing.Point(120, 589);
            this.TextTime.Name = "TextTime";
            this.TextTime.ReadOnly = true;
            this.TextTime.Size = new System.Drawing.Size(100, 15);
            this.TextTime.TabIndex = 6;
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // bindingSource1
            // 
            this.bindingSource1.BindingComplete += new System.Windows.Forms.BindingCompleteEventHandler(this.bindingSource1_BindingComplete);
            // 
            // StoryForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(912, 620);
            this.Controls.Add(this.TextTime);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.webBrowser1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.button3);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "StoryForm";
            this.Text = "Reading Stories";
            this.Load += new System.EventHandler(this.StoryForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		DateTime timestart = DateTime.MinValue;
		private void button1_Click(object sender, System.EventArgs e) {
			object empty = System.Reflection.Missing.Value;
			webBrowser1.Navigate(comboBox1.Text);

			timestart = DateTime.MinValue;
			//axWebBrowser1.DocumentComplete += new AxSHDocVw.DWebBrowserEvents2_DocumentCompleteEventHandler(axWebBrowser1_DocumentComplete);
		}

		private void comboBox1_Leave(object sender, System.EventArgs e) {

			Console.WriteLine("Leave changed!");		
		}

		private void comboBox1_SelectedIndexChanged(object sender, System.EventArgs e) {
			button1.PerformClick();
		}

		private void comboBox1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			if (e.KeyCode == Keys.Enter) {
				Console.WriteLine("Enter caputered");
				e.Handled = true;
			}
		}

		private void button2_Click(object sender, System.EventArgs e) {
			timer1.Stop();
			this.DialogResult = DialogResult.OK;
			Close();
		}

		private void button3_Click(object sender, System.EventArgs e) {
            timer1.Stop();
			this.DialogResult = DialogResult.Cancel;
			Close();
		}

		public static string AppName {
			get {
				return "StoryForm_Calibrate";
			}
		}

		protected override void WndProc(ref Message m) {
			if (m.Msg == WMCopyData.WM_COPYDATA) {
				ctrl_msg cmsg = WMCopyData.TranslateMessage(m);
				switch (cmsg.cmd) {
					case GameCommand.StartGame:
						//cfg_num_player = cmsg.msg;
						//StartGame();
						break;
                    case GameCommand.SetBCIScore:
//						player[0].speed = checkspeed(cmsg.dva[0]);
//						player[1].speed = checkspeed(cmsg.dva[1]);
//						textSpeed.Text = player[0].speed.ToString();
//						textSpeed1.Text = player[1].speed.ToString();
						break;
                    case GameCommand.StopGame:
//						StopGame();
						break;
                    case GameCommand.CloseGame:
						this.DialogResult = DialogResult.Cancel;
						this.Close();
						break;
                    case GameCommand.SetPlayerName:
//						player[0].Name = cmsg.strdata;
						break;
                    case GameCommand.SetPlayerName2:
//						player[1].Name = cmsg.strdata;
						break;
					default:
						break;
				}

				return;
			}

			// TODO:  Add EEGRecordForm.WndProc implementation
			base.WndProc (ref m);
		}

		private void StoryForm_Load(object sender, System.EventArgs e) {
			WMCopyData.SetProp(this.Handle, AppName, 1);

			int n = Screen.AllScreens.Length - 1;
			if (n >= 1) n = 1;
			Rectangle rt = Screen.AllScreens[n].Bounds;
			this.Location = rt.Location + 
				new Size((rt.Width - this.Width) /2, (rt.Height - this.Height) / 2);
			if (n > 0) this.WindowState = FormWindowState.Maximized;
		}

        //private void axWebBrowser1_DocumentComplete(object sender, AxSHDocVw.DWebBrowserEvents2_DocumentCompleteEvent e) {
        //}

		private void timer1_Tick(object sender, System.EventArgs e) {
			if (timestart > DateTime.MinValue) {
				TimeSpan ts = DateTime.Now - timestart;
				TextTime.Text = string.Format("{0,3:0}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
			}
		}

        private void bindingSource1_BindingComplete(object sender, BindingCompleteEventArgs e)
        {
            if (timestart == DateTime.MinValue) {
                timestart = DateTime.Now;
                timer1.Start();
            }
        }
	}
}
