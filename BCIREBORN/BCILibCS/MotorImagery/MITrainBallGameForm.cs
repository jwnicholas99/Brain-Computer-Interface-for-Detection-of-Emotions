using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

using System.Drawing.Drawing2D;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;

using BCILib;
using BCILib.App;
using BCILib.Amp;
using BCILib.Util;

namespace BCILib.MotorImagery
{
	/// <summary>
	/// Summary description for MIDataCollectForm.
	/// </summary>
	internal class MITrainBallGameForm : System.Windows.Forms.Form
	{
        private SelfDrawPannel panel1;
		private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelScore;
        private ToolStrip toolStrip1;
        private ToolStripButton toolStripSysConfig;
        private ToolStripButton toolStripStart;
        private ToolStripButton toolStripStop;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatus;
        private System.Windows.Forms.Timer timer1;
        private IContainer components;

		public MITrainBallGameForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			LoadConfig();
		}

        private string MIType = "MM";
        public MITrainBallGameForm(string MIType):this()
        {
            this.MIType = MIType;
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

			if (myJoy != null) {
				myJoy.Dispose();
				myJoy = null;
			}

			base.Dispose( disposing );
		}

		~MITrainBallGameForm() {
			Console.WriteLine("MIDataCollectForm Destructor called.");
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MITrainBallGameForm));
            this.panel1 = new BCILib.MotorImagery.SelfDrawPannel();
            this.label1 = new System.Windows.Forms.Label();
            this.labelScore = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripSysConfig = new System.Windows.Forms.ToolStripButton();
            this.toolStripStart = new System.Windows.Forms.ToolStripButton();
            this.toolStripStop = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.Gray;
            this.panel1.Location = new System.Drawing.Point(24, 80);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(656, 392);
            this.panel1.TabIndex = 0;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            this.panel1.Click += new System.EventHandler(this.panel1_Click);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(192, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(168, 32);
            this.label1.TabIndex = 3;
            this.label1.Text = "Moved:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelScore
            // 
            this.labelScore.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelScore.Location = new System.Drawing.Point(368, 44);
            this.labelScore.Name = "labelScore";
            this.labelScore.Size = new System.Drawing.Size(88, 32);
            this.labelScore.TabIndex = 3;
            this.labelScore.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSysConfig,
            this.toolStripStart,
            this.toolStripStop});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(704, 25);
            this.toolStrip1.TabIndex = 4;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripSysConfig
            // 
            this.toolStripSysConfig.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripSysConfig.Image = ((System.Drawing.Image)(resources.GetObject("toolStripSysConfig.Image")));
            this.toolStripSysConfig.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSysConfig.Name = "toolStripSysConfig";
            this.toolStripSysConfig.Size = new System.Drawing.Size(88, 22);
            this.toolStripSysConfig.Text = "System Config";
            this.toolStripSysConfig.Click += new System.EventHandler(this.toolStripSysConfig_Click);
            // 
            // toolStripStart
            // 
            this.toolStripStart.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripStart.Image = ((System.Drawing.Image)(resources.GetObject("toolStripStart.Image")));
            this.toolStripStart.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripStart.Name = "toolStripStart";
            this.toolStripStart.Size = new System.Drawing.Size(35, 22);
            this.toolStripStart.Text = "Start";
            this.toolStripStart.Click += new System.EventHandler(this.toolStripStart_Click);
            // 
            // toolStripStop
            // 
            this.toolStripStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripStop.Enabled = false;
            this.toolStripStop.Image = ((System.Drawing.Image)(resources.GetObject("toolStripStop.Image")));
            this.toolStripStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripStop.Name = "toolStripStop";
            this.toolStripStop.Size = new System.Drawing.Size(35, 22);
            this.toolStripStop.Text = "Stop";
            this.toolStripStop.Click += new System.EventHandler(this.toolStripStop_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 488);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(704, 22);
            this.statusStrip1.TabIndex = 5;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatus
            // 
            this.toolStripStatus.Name = "toolStripStatus";
            this.toolStripStatus.Size = new System.Drawing.Size(689, 17);
            this.toolStripStatus.Spring = true;
            // 
            // timer1
            // 
            this.timer1.Interval = 50;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // MIBallGameTrainForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(704, 510);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.labelScore);
            this.KeyPreview = true;
            this.Name = "MIBallGameTrainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Motor Imagery Data Collection";
            this.Load += new System.EventHandler(this.MIDataCollectForm_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MIDataCollectForm_KeyUp);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MIDataCollectForm_KeyDown);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private int cfg_num_trials = 20;
		private int cfg_waitstart_time = 3000;
		private int cfg_pre_time = 2000;
		private int cfg_cue_time = 0;
		private int cfg_act_time = 4000;
		private int cfg_rest_time = 2000;

		private int cfg_stim_prep = 100;
		//private int cfg_stim_cue = 110;
		private int cfg_stim_task_offset = 120;
		private int cfg_stim_keydown_offset = 130;
		private int cfg_stim_click_offset = 150;
		private int cfg_stim_rest = 199;

		private int cfg_mv_times = 3;
		private int cfg_left_click = 1;
		private int cfg_right_click = 1;
		private int cfg_img_wait = 0;

		private int rem_left = 20;
		private int rem_right = 20;
		private int mv_left = 0;
		private int mv_right = 0;
		private int mv_pos = 0;
		private int num_cstart = 0;
		private int time_now = 0;
		private int sum_score = 0;

		private int sz_ball = 30;
		private int ncol, nrow;
		private int margin_x = 5;
		private int margin_y = 10;

		private byte[] mv_flgs;
		private void panel1_Paint(object sender, System.Windows.Forms.PaintEventArgs e) {
			Rectangle drt = panel1.ClientRectangle;

            Graphics g = e.Graphics;

			g.FillRectangle(new SolidBrush(panel1.BackColor), drt);

            ncol = (int)Math.Sqrt(cfg_num_trials);
            nrow = (int)((cfg_num_trials + ncol - 1) / ncol);

			int w = (drt.Width / 2 - margin_x - margin_x) / 2 / ncol;
			int h = (drt.Height - margin_y) / 2 / (nrow + 1);

			sz_ball = w<h? w:h;

			w = ncol * sz_ball;
			h = nrow * sz_ball;

			Point[] tube0 = new Point[4];
			Point[] tube1 = new Point[4];

			// original box A
			int x0 = drt.Width / 2 - margin_x - w;
			int y0 = 10;
			g.FillRectangle(Brushes.LightCoral, x0, y0, w, h); //Brushes.Gray
			tube0[0].X = tube0[1].X = x0 + w - sz_ball / 2;
			tube0[0].Y = y0 + h;
			tube0[1].Y = tube0[2].Y = drt.Height / 2;

			int bx = x0 + w - sz_ball, by = y0 + h - sz_ball;
			for (int nb = 0; nb < rem_left; nb++) {
				int ir = nb / ncol;
				int ic = nb - ir * ncol;
				g.FillEllipse(Brushes.Red, bx - sz_ball * ic, 
					by - sz_ball * ir, sz_ball, sz_ball);
			}

			// original box B
			x0 = drt.Width / 2 + margin_x;
			g.FillRectangle(Brushes.CornflowerBlue, x0, y0, w, h); //Brushes.Gray
			tube1[0].X = tube1[1].X = x0 + sz_ball / 2;
			tube1[0].Y = y0 + h;
			tube1[1].Y = tube1[2].Y = drt.Height / 2;

			bx = x0;
			by = y0 + h - sz_ball;
			for (int nb = 0; nb < rem_right; nb++) {
				int ir = nb / ncol;
				int ic = nb - ir * ncol;
				g.FillEllipse(Brushes.Blue, bx + sz_ball * ic, 
					by - sz_ball * ir, sz_ball, sz_ball);
			}

			// collection box C
			y0 = drt.Height - h - margin_y;
			x0 = margin_x;
			g.FillRectangle(Brushes.LightCoral, x0, y0, w, h);
			tube0[2].X = tube0[3].X = x0 + sz_ball / 2;
			tube0[3].Y = y0;
			bx = x0;
			by = y0 + h - sz_ball;

			int ndown = cfg_num_trials - rem_left - mv_left;
			Pen rpen = new Pen(Color.Red);
			for (int nb = 0; nb < ndown; nb++) {
				int ir = nb / ncol;
				int ic = nb - ir * ncol;
				if (mv_flgs != null && (mv_flgs[nb] & 1) != 0) {
					g.DrawEllipse(rpen, bx + sz_ball * ic, 
						by - sz_ball * ir, sz_ball, sz_ball);
				} else {
					g.FillEllipse(Brushes.Red, bx + sz_ball * ic, 
						by - sz_ball * ir, sz_ball, sz_ball);
				}
			}

			// Collection Box D
			x0 = drt.Width - w - margin_x;
			g.FillRectangle(Brushes.CornflowerBlue, x0, y0, w, h);
			tube1[2].X = tube1[3].X = x0 + w - sz_ball/2;
			tube1[3].Y = y0;

			bx = x0 + w - sz_ball;
			by = y0 + h - sz_ball;

			ndown = cfg_num_trials - rem_right - mv_right;
			Pen bpen = new Pen(Color.Blue);
			for (int nb = 0; nb < ndown; nb++) {
				int ir = nb / ncol;
				int ic = nb - ir * ncol;
				if (mv_flgs != null && (mv_flgs[nb] & 2) != 0) {
					g.DrawEllipse(bpen, bx - sz_ball * ic, 
						by - sz_ball * ir, sz_ball, sz_ball);
				} else {
					g.FillEllipse(Brushes.Blue, bx - sz_ball * ic, 
						by - sz_ball * ir, sz_ball, sz_ball);
				}
			}

			// draw tubes
			Pen tp = new Pen(Color.DimGray, sz_ball);
			tp.LineJoin = LineJoin.Round;
			g.DrawLines(tp, tube0);
			g.DrawLines(tp, tube1);
//			g.DrawLines(new Pen(Color.LightCoral, sz_ball), tube0);
//			g.DrawLines(new Pen(Color.CornflowerBlue, sz_ball), tube1);

			Font fnt = new Font("Arial", 20, FontStyle.Bold);

			StringFormat sft = new StringFormat(StringFormatFlags.NoClip);
			sft.Alignment = StringAlignment.Center;
			sft.LineAlignment = StringAlignment.Center;
			Rectangle srt;

			if (mv_left == 1) {
				// draw moving ball
				x0 = tube0[1].X - sz_ball / 2 - mv_pos;
				y0 = (drt.Height - sz_ball) / 2;
				g.FillEllipse(Brushes.Red, x0, y0, sz_ball, sz_ball);

				// draw switch button
				x0 = tube0[2].X - sz_ball / 2;
				y0 -= sz_ball + 8;
				g.FillEllipse(Brushes.Black,
					x0, y0 - 8, sz_ball + 8, sz_ball + 8);
				g.FillEllipse(Brushes.Red,
					x0 + 4, y0 - 4, sz_ball, sz_ball);

				// draw counter
				if (num_cstart >= 0 && num_cstart < 4) {
					srt = new Rectangle(x0 + 4, y0 - 2, sz_ball, sz_ball);
					int rc = 3 - num_cstart;
					g.DrawString(rc.ToString(), fnt, Brushes.Black,
						srt, sft);
				}

				// draw text
                if (cfg_left_click > 0) {
                    srt = new Rectangle(10, 5, 1, 1);
                    srt.Width = drt.Width / 2 - 20 - w;
                    srt.Height = drt.Height / 2 - sz_ball - 16;
                    g.DrawString("Click Left Button", fnt, Brushes.Black, srt, sft);
                }

				// draw arrow
				x0 += 8;
				y0 += sz_ball / 2 - 4;
				Point[] tag = new Point[3];
				tag[0].X = x0;
				tag[0].Y = y0;
				tag[1].X = tag[2].X = x0 + sz_ball;
				tag[1].Y = y0 - sz_ball / 2;
				tag[2].Y = y0 + sz_ball / 2;

				for (int ic = 0; ic < 3; ic++) {
					for (int it = 0; it < tag.Length; it++) {
						tag[it].X += sz_ball;
					}
					g.FillPolygon(ic >= 3 - num_cstart? 
						Brushes.Red : Brushes.LightSteelBlue, tag); 
				}
			} else {
				// draw switch button
				x0 = tube0[2].X - sz_ball / 2;
				y0 = (drt.Height - sz_ball) / 2 - sz_ball - 8;
				g.FillEllipse(Brushes.Gainsboro,
					x0, y0 - 8, sz_ball + 8, sz_ball + 8);
				g.FillEllipse(Brushes.DarkGray,
					x0 + 4, y0 - 4, sz_ball, sz_ball);
			}
			
			if (mv_right == 1) {
				x0 = tube1[1].X - sz_ball / 2 + mv_pos;
				y0 = (drt.Height - sz_ball) / 2;
				g.FillEllipse(Brushes.Blue, x0, y0, sz_ball, sz_ball);

				// draw switch button
				x0 = tube1[2].X - sz_ball / 2 - 8;
				y0 -= sz_ball + 8;
				g.FillEllipse(Brushes.Black,
					x0, y0 - 8, sz_ball + 8, sz_ball + 8);
				g.FillEllipse(mv_right == 1? Brushes.Blue : Brushes.DarkGray,
					x0 + 4, y0 - 4, sz_ball, sz_ball);

				// draw counter
				if (num_cstart >= 0 && num_cstart < 4) {
					srt = new Rectangle(x0 + 4, y0 - 2, sz_ball, sz_ball);
					int rc = 3 - num_cstart;
					g.DrawString(rc.ToString(), fnt, Brushes.Black,
						srt, sft);
				}

				// draw text
                if (cfg_right_click > 0) {
                    srt = new Rectangle(drt.Width - 10, 5, 1, 1);
                    srt.Width = drt.Width / 2 - 20 - w;
                    srt.Height = drt.Height / 2 - sz_ball - 16;
                    srt.X -= srt.Width;
                    g.DrawString("Click Right Button", fnt, Brushes.Black, srt, sft);
                }

				// draw arrow
				x0 += sz_ball;
				y0 += sz_ball / 2 - 4;
				Point[] tag = new Point[3];
				tag[0].X = x0;
				tag[0].Y = y0;
				tag[1].X = tag[2].X = x0 - sz_ball;
				tag[1].Y = y0 - sz_ball / 2;
				tag[2].Y = y0 + sz_ball / 2;

				for (int ic = 0; ic < 3; ic++) {
					for (int it = 0; it < tag.Length; it++) {
						tag[it].X -= sz_ball;
					}
					g.FillPolygon(ic >= 3 - num_cstart? 
						Brushes.Blue : Brushes.LightSteelBlue, tag); 
				}
			} else {
				// draw switch button
				x0 = tube1[2].X - sz_ball / 2 - 8;
				y0 = (drt.Height - sz_ball) / 2 - sz_ball - 8;
				g.FillEllipse(Brushes.Gainsboro,
					x0, y0 - 8, sz_ball + 8, sz_ball + 8);
				g.FillEllipse(Brushes.DarkGray,
					x0 + 4, y0 - 4, sz_ball, sz_ball);
			}

            // Draw progress bar, three parts: prepare, action, rest
            x0 = w + 50;
            y0 = drt.Height - sz_ball - h / 2;
            int dw = drt.Width - x0 - x0 - 10;
            int time_all = cfg_pre_time + cfg_act_time + cfg_rest_time;
            int dl = time_now * dw / time_all;

            fnt = new Font("Arial", 12);
            sft.Alignment = StringAlignment.Center;
            sft.LineAlignment = StringAlignment.Center;
            sft.FormatFlags |= StringFormatFlags.NoClip;

            srt = new Rectangle(x0, y0, dw * cfg_pre_time / time_all, sz_ball);
            g.DrawRectangle(bpen, srt.X - 1, srt.Y - 1, srt.Width + 1, srt.Height + 1);
            if (dl > 0) {
                int dlw = dl;
                if (dlw > srt.Width) dlw = srt.Width;
                g.FillRectangle(Brushes.Lime, srt.X, srt.Y, dlw, srt.Height);
                dl -= dlw;
            }
            g.DrawString("Prep", fnt, Brushes.Black, srt, sft);

            srt.X += srt.Width + 5;
            srt.Width = dw * cfg_act_time / time_all;
            g.DrawRectangle(bpen, srt.X - 1, srt.Y - 1, srt.Width + 1, srt.Height + 1);
            if (dl > 0) {
                int dlw = dl;
                if (dlw > srt.Width) dlw = srt.Width;
                g.FillRectangle(Brushes.Lime, srt.X, srt.Y, dlw, srt.Height);
                dl -= dlw;
            }
            g.DrawString("Action", fnt, Brushes.Black, srt, sft);

            srt.X += srt.Width + 5;
            srt.Width = dw * cfg_rest_time / time_all;
            g.DrawRectangle(bpen, srt.X - 1, srt.Y - 1, srt.Width + 1, srt.Height + 1);
            if (dl > 0) {
                int dlw = dl;
                if (dlw > srt.Width) dlw = srt.Width;
                g.FillRectangle(Brushes.Lime, srt.X, srt.Y, dlw, srt.Height);
                dl -= dlw;
            }
            g.DrawString("Rest", fnt, Brushes.Black, srt, sft);
		}

		private void panel1_Click(object sender, System.EventArgs e) {
			Point cpos = Cursor.Position;
			cpos = panel1.PointToClient(cpos);
			toolStripStatus.Text = string.Format("{0}, {1}", cpos.X, cpos.Y);
		}

		private void RunDataCollection() {
			try {
				ExeDataCollection();
			} catch (Exception e) {
				Console.WriteLine("Error = {0}", e);
                AmpContainer.StopRecord();
                ConsoleCapture.EndLogFile();
                //report error
                MessageBox.Show("Error in ExeDataCollection: " + e);
            }

            Invoke((Action)delegate
            {
                toolStripStart.Enabled = true;
                toolStripStop.Enabled = false;
                this.ControlBox = true;
            });
            timer1.Stop();
		}

		ManualResetEvent evt_stopProc = new ManualResetEvent(false);

		private void LoadConfig() {
			LoadConfig(BCIApplication.SysResource);
		}

		private void LoadConfig(ResManager rm) {
			rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Number_Trials, ref cfg_num_trials);
			rem_left = rem_right = cfg_num_trials;

            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Time_WaitStart, ref cfg_waitstart_time);
            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Time_Prepare, ref cfg_pre_time);
            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Time_Cue, ref cfg_cue_time);
            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Time_Action, ref cfg_act_time);
            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Time_Rest, ref cfg_rest_time);
            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Time_Imagine_Wait, ref cfg_img_wait);

            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Stim_Prepare, ref cfg_stim_prep);
			//rm.GetConfigValue(rn, "Stim_Cue", ref cfg_stim_cue);
            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Stim_Task_Offset, ref cfg_stim_task_offset);
            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Stim_Click_Offset, ref cfg_stim_click_offset);
            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Stim_KeyDown_Offset, ref cfg_stim_keydown_offset);
            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Stim_Rest, ref cfg_stim_rest);
            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Num_Moving_Steps, ref cfg_mv_times);

			int cfg_finger_click = 1;
            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Finger_Click, ref cfg_finger_click);
			cfg_left_click = cfg_finger_click;
            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Finger_LeftClick, ref cfg_left_click);
			cfg_right_click = cfg_finger_click;
            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Finger_RightClick, ref cfg_right_click);

			rm.SaveIfChanged();

			if (cfg_finger_click ==0) cfg_left_click = cfg_right_click = 0;

			panel1.Invalidate();
		}

		private int num_click_left;
		private int num_click_right;
		private int ball_start = 0;

		private void ShowTimeProgress(int time_wait)
		{
			ShowTimeProgress(time_wait, 0);
		}

		private void RunBeep()
		{
			Sound.Beep(300, 500);
		}

		private void ShowTimeProgress(int time_wait, int beepBeforeEnd)
		{
			long t0 = HRTimer.GetTimestamp();
			int wtime = 0;
			int tsave = time_now;
			bool beeped = false;

			while (wtime < time_wait) {
				wtime = HRTimer.DeltaMilliseconds(t0);
				int wt = time_wait - wtime;
				if (beepBeforeEnd > 0 && !beeped && wt <= beepBeforeEnd) 
				{
					Thread thd = new Thread(new ThreadStart(RunBeep));
					thd.Start();
					beeped = true;
				}
				if (wt > 50) wt = 50;
				else if (wt < 0) wt = 0;
                time_now = tsave + wtime;
                if (evt_stopProc.WaitOne(wt, false)) break;
                //panel1.Invalidate(urt);
			}
		}

		private void ExeDataCollection() {
			ResManager rm = BCIApplication.SysResource;

            cfg_left_click = MIType[0] == 'M' ? 0 : 1;
            cfg_right_click = MIType[1] == 'M' ? 0 : 1;
            int finger_click = cfg_left_click | cfg_right_click;

            rm.SetConfigValue(MIConstDef.MITraining, MIConstDef.Finger_Click, finger_click.ToString());
            rm.SetConfigValue(MIConstDef.MITraining, MIConstDef.Finger_LeftClick, cfg_left_click.ToString());
            rm.SetConfigValue(MIConstDef.MITraining, MIConstDef.Finger_RightClick, cfg_right_click.ToString());

			// specify app name
			string appname = "BallGame" + MIType;
            BCIApplication.SetProtocolName(rm, appname);

			//LoadConfig(rm);
			string dpath = TrainDirSpecForm.GetTrainingPath(rm);
			if (dpath == null) return;

			string timestamp = BCIApplication.TimeStamp;

			//Logfile
			ConsoleCapture.StartLogFile(Path.Combine(dpath, appname + "_" + timestamp + ".log"));
            AmpContainer.StartAll();
            AmpContainer.StartRecord(dpath, appname);

			// initlaize variables
			mv_left = mv_right = 0;
			sum_score = 0;
			mv_flgs = new byte[cfg_num_trials];

			rem_left = rem_right = cfg_num_trials;
			panel1.Invalidate();
			Random rnd = new Random();

			if (cfg_waitstart_time < 0) cfg_waitstart_time = 0;
			evt_stopProc.WaitOne(cfg_waitstart_time, false);

			WaitHandle[] evts = new WaitHandle[] {evt_stopProc, evt_ActionDone};

            // Resting
			time_now = cfg_pre_time + cfg_act_time;
			ShowTimeProgress(cfg_rest_time, 1000);

			int rnd_num = 0;
			for (int itrial = 0; itrial < cfg_num_trials + cfg_num_trials; itrial++)
			{
                this.Invoke((Action)delegate()
                {
                    toolStripStatus.Text = string.Format("Trail {0} of {1}.", itrial + 1, cfg_num_trials << 1);
                });

				// Prepare
				time_now = 0;
				Console.WriteLine("Prepare...");
                AmpContainer.SendAll(cfg_stim_prep);
				ShowTimeProgress(cfg_pre_time);

				// Show task
				if (rem_left == 0) mv_right = 1;
				else if (rem_right == 0) mv_left = 1;
				else {
					if ((itrial & 1) == 0) rnd_num = rnd.Next(2);
					else rnd_num = 1 - rnd_num;
					mv_left = rnd_num;
					mv_right = 1 - rnd_num;
				}

				rem_left -= mv_left;
				rem_right -= mv_right;
				num_cstart = 0;
				ball_start = (panel1.ClientRectangle.Width / 2 - 15 - sz_ball) / 5;
				mv_pos = ball_start;
				//panel1.Invalidate();

				// cue with task
                int scode = cfg_stim_task_offset + mv_left + mv_right * 2;
                AmpContainer.SendAll(scode);
				Console.WriteLine("STIM_TASK:{0}", scode);

				// allow for click
				if ((mv_left == 1 && cfg_left_click != 0) || 
					(mv_right == 1 && cfg_right_click != 0))
                {
					num_click_left = num_click_right = 0;
					evt_ActionDone.Reset();
					bActionWait = true;
                    Invoke((Action)delegate()
                    {
                        this.Activate();
                    });
				} else { // imagine wait
					if (cfg_img_wait > 0) evt_stopProc.WaitOne(cfg_img_wait, false);
				}

				long t0 = HRTimer.GetTimestamp();
				int pwt = cfg_cue_time / 3;
				for (int ic = 0; ic < 3; ic++) {
					int wt = cfg_cue_time - HRTimer.DeltaMilliseconds(t0);
					if (wt > pwt) wt = pwt;
					if (wt < 0) wt = 0;
					if (evt_stopProc.WaitOne(wt, false)) break;
					num_cstart++;
                    //panel1.Invalidate(urt);
				}
				num_cstart++;
                //panel1.Invalidate(urt);

                time_now = cfg_pre_time;

                int mtime = cfg_act_time / cfg_mv_times;
                double max_pos = panel1.ClientRectangle.Width / 2 - 15 - sz_ball;
                double ml = (max_pos - ball_start + cfg_mv_times - 1) / cfg_mv_times;

                for (int i = 0; i < cfg_mv_times; i++) {
                    if ((cfg_left_click == 1 && mv_left == 1 && num_click_left <= i) ||
                        (cfg_right_click == 1 && mv_right == 1 && num_click_right <= i))
                    {
                        // Wait for button click
                        WaitHandle.WaitAny(evts);
                        evt_ActionDone.Reset();
                        bActionWait = true;
                    }

                    time_now = cfg_rest_time + (cfg_act_time * i) / cfg_mv_times;

                    // move one step
                    //Sound.PlayAsync(@"..\..\Sound\start.wav");
                    if (ml >= max_pos - mv_pos) ml = max_pos - mv_pos;

                    t0 = DateTime.Now.Ticks;
                    int save_pos = mv_pos;
                    double moved = 0;
                    int tsaved = time_now;
                    while (moved < ml) {
                        if (evt_stopProc.WaitOne(50, false)) break;

                        long t1 = DateTime.Now.Ticks - t0;
                        time_now = tsaved + (int) (t1 / 10000);

                        moved = t1 * ml / (mtime * 10000);
                        mv_pos = save_pos + (int)moved;
                    }
                }
                bActionWait = false;
                evt_ActionDone.Reset();

                time_now = cfg_rest_time + cfg_act_time;

                if (num_click_left == 0 && num_click_right == 0) {
                    AmpContainer.SendAll(cfg_stim_click_offset);
                    Console.WriteLine("STIM_CLICK:{0}", cfg_stim_click_offset);
                }

				if (evt_stopProc.WaitOne(0, false)) break;

				if ((mv_left == 1 && cfg_left_click == 1 && num_click_left == 0) ||
					(mv_right == 1 && cfg_right_click == 1 && num_click_right == 0))
				{ // no click has been done, failed
					if (mv_left == 1) {
						mv_flgs[cfg_num_trials - rem_left - 1] |= 1;
					} else {
						mv_flgs[cfg_num_trials - rem_right - 1] |= 2;
					}
					// Sound.PlayAsync(@"..\..\sound\Fail.wav");
				} else {
					// Sound.PlayAsync(@"..\..\sound\Success.wav");
					sum_score++;
                    this.Invoke((Action)delegate()
                    {
                        labelScore.Text = sum_score.ToString();
                    });
				}

				// rest
				mv_left = mv_right = 0;
				num_cstart = 0;
				panel1.Invalidate();
                AmpContainer.SendAll(cfg_stim_rest);
				Console.WriteLine("STIM_REST:{0}", cfg_stim_rest);

                if (evt_stopProc.WaitOne(cfg_rest_time, false)) break;

				ShowTimeProgress(cfg_rest_time, 1000);

                if (!AmpContainer.AllAlive) {
                    evt_stopProc.Set();
                    break;
                }
			}

			if (!evt_stopProc.WaitOne(0, false)) {
                this.Invoke((Action)delegate()
                {
                    label1.Text = "Finished";
                });
			}

			if (!evt_stopProc.WaitOne(0, false)) {
				// finished, add training dir to training list
                BCIApplication.AddTrainingPath(dpath);
                MessageBox.Show("Training session " + appname + " Finished!");
			}

            AmpContainer.StopRecord();
            ConsoleCapture.EndLogFile();
		}

		private bool bActionWait = false;
		private ManualResetEvent evt_ActionDone = new ManualResetEvent(false);

		private int numkeydown = 0;
		private void MIDataCollectForm_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			int key_type = 0;
			if (e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.Oemcomma) key_type = 1;
			else if(e.KeyCode == Keys.Right || e.KeyCode == Keys.OemPeriod 
				|| e.KeyCode == Keys.Enter) key_type = 2;
			if (key_type == 0) return;

			e.Handled = true;
			OnKeyDown(key_type);
		}

		private void OnKeyDown(int key_type) {
			numkeydown++;
			if (numkeydown > 1) return;

			if (key_type == 1) {
                AmpContainer.SendAll(cfg_stim_keydown_offset + 1);
                Console.WriteLine("STIM_CLICK:{0}", cfg_stim_keydown_offset + 1);
			} else {
                AmpContainer.SendAll(cfg_stim_keydown_offset + 2);
                Console.WriteLine("STIM_CLICK:{0}", cfg_stim_keydown_offset + 2);
			}
		}

		private void MIDataCollectForm_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e) {
			int key_type = 0;
			if (e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.Oemcomma) key_type = 1;
			else if(e.KeyCode == Keys.Right || e.KeyCode == Keys.OemPeriod
				|| e.KeyCode == Keys.Enter) key_type = 2;
			if (key_type == 0) return;
			e.Handled = true;
			OnKeyUp(key_type);
		}

		private void OnKeyUp(int key_type) {
			numkeydown = 0;
			if (key_type == 1) {
                AmpContainer.SendAll(cfg_stim_click_offset + 1);
                Console.WriteLine("STIM_CLICK:{0}", cfg_stim_click_offset + 1);
                num_click_left++;
			} else {
                AmpContainer.SendAll(cfg_stim_click_offset + 2);
                Console.WriteLine("STIM_CLICK:{0}", cfg_stim_click_offset + 2);
                num_click_right++;
			}

			if (!bActionWait) {
				Console.WriteLine("Key up: {0}", key_type);
				return;
			}

			if (key_type == 1 && mv_left == 1 || key_type == 2 && mv_right == 1
				|| mv_left == mv_right)
			{
				evt_ActionDone.Set();
                bActionWait = false;
            }
		}

		protected override bool IsInputKey( Keys keyData ) {
			switch ( keyData ) {
				case Keys.Up:
				case Keys.Down:
				case Keys.Left:
				case Keys.Right:
					return true;
			}
			return base.IsInputKey( keyData );
		}

		private JoyStick myJoy = null;
		private void MIDataCollectForm_Load(object sender, System.EventArgs e) {
			// set window position
			int n = Screen.AllScreens.GetUpperBound(0);
			Screen scn = Screen.AllScreens[n];
			Rectangle rt = scn.WorkingArea;

			this.Location = rt.Location + new Size((rt.Width - this.Width)/2, (rt.Height - this.Height)/2);
			this.WindowState = FormWindowState.Maximized;

			myJoy = new JoyStick();
            myJoy.SetKeyDownCallback(new Action<int>(OnKeyDown));
            myJoy.SetKeyUpCallback(new Action<int>(OnKeyUp));
		}

        private void toolStripSysConfig_Click(object sender, EventArgs e)
        {
            ResManager rm = BCIApplication.SysResource;
            if (rm.ShowDialog()) LoadConfig();
        }

        private void toolStripStart_Click(object sender, EventArgs e)
        {
            toolStripStart.Enabled = false;
            toolStripStop.Enabled = true;
            this.ControlBox = false;

            label1.Text = "Moved:";
            labelScore.Text = "0";

            timer1.Start();
            Thread thd = new Thread(new ThreadStart(RunDataCollection));
            evt_stopProc.Reset();
            thd.Start();
        }

        private void toolStripStop_Click(object sender, EventArgs e)
        {
            evt_stopProc.Set();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            panel1.Invalidate();
            panel1.Update();
        }
	}

    internal class SelfDrawPannel : Panel
    {
        public SelfDrawPannel()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint
                | ControlStyles.AllPaintingInWmPaint, true);
        }
    }
}
