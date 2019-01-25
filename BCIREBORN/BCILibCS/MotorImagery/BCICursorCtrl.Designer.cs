using BCILib.Util;
namespace BCILib.MotorImagery
{
    partial class BCICursorCtrl
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.lblNumHit = new System.Windows.Forms.Label();
            this.lblNumMiss = new System.Windows.Forms.Label();
            this.bntReset = new System.Windows.Forms.Button();
            this.lblAccurate = new System.Windows.Forms.Label();
            this.btnCfg = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.panelCursor = new SelfDrawPannel();
            this.btnConnect = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // timer
            // 
            this.timer.Interval = 20;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // lblNumHit
            // 
            this.lblNumHit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNumHit.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNumHit.ForeColor = System.Drawing.Color.RoyalBlue;
            this.lblNumHit.Location = new System.Drawing.Point(25, 7);
            this.lblNumHit.Name = "lblNumHit";
            this.lblNumHit.Size = new System.Drawing.Size(100, 34);
            this.lblNumHit.TabIndex = 1;
            this.lblNumHit.Text = "0";
            this.lblNumHit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNumMiss
            // 
            this.lblNumMiss.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNumMiss.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNumMiss.ForeColor = System.Drawing.Color.Crimson;
            this.lblNumMiss.Location = new System.Drawing.Point(142, 7);
            this.lblNumMiss.Name = "lblNumMiss";
            this.lblNumMiss.Size = new System.Drawing.Size(100, 34);
            this.lblNumMiss.TabIndex = 1;
            this.lblNumMiss.Text = "0";
            this.lblNumMiss.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bntReset
            // 
            this.bntReset.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bntReset.Location = new System.Drawing.Point(543, 8);
            this.bntReset.Name = "bntReset";
            this.bntReset.Size = new System.Drawing.Size(88, 34);
            this.bntReset.TabIndex = 2;
            this.bntReset.Text = "Reset";
            this.bntReset.UseVisualStyleBackColor = true;
            this.bntReset.Click += new System.EventHandler(this.bntReset_Click);
            // 
            // lblAccurate
            // 
            this.lblAccurate.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAccurate.Location = new System.Drawing.Point(265, 7);
            this.lblAccurate.Name = "lblAccurate";
            this.lblAccurate.Size = new System.Drawing.Size(116, 34);
            this.lblAccurate.TabIndex = 1;
            this.lblAccurate.Text = "0%";
            this.lblAccurate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnCfg
            // 
            this.btnCfg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCfg.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCfg.Location = new System.Drawing.Point(822, 6);
            this.btnCfg.Name = "btnCfg";
            this.btnCfg.Size = new System.Drawing.Size(130, 36);
            this.btnCfg.TabIndex = 2;
            this.btnCfg.Text = "Configure";
            this.btnCfg.UseVisualStyleBackColor = true;
            this.btnCfg.Click += new System.EventHandler(this.btnCfg_Click);
            // 
            // btnStart
            // 
            this.btnStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStart.Location = new System.Drawing.Point(387, 7);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(72, 34);
            this.btnStart.TabIndex = 2;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStop.Location = new System.Drawing.Point(465, 7);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(72, 34);
            this.btnStop.TabIndex = 2;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // panelCursor
            // 
            this.panelCursor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelCursor.BackColor = System.Drawing.Color.Black;
            this.panelCursor.Location = new System.Drawing.Point(26, 48);
            this.panelCursor.Name = "panelCursor";
            this.panelCursor.Size = new System.Drawing.Size(926, 383);
            this.panelCursor.TabIndex = 0;
            this.panelCursor.Paint += new System.Windows.Forms.PaintEventHandler(this.panelCursor_Paint);
            this.panelCursor.Resize += new System.EventHandler(this.panelCursor_Resize);
            // 
            // btnConnect
            // 
            this.btnConnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConnect.Location = new System.Drawing.Point(637, 7);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(117, 34);
            this.btnConnect.TabIndex = 2;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // BCICursorCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(976, 443);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnCfg);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.bntReset);
            this.Controls.Add(this.lblAccurate);
            this.Controls.Add(this.lblNumMiss);
            this.Controls.Add(this.lblNumHit);
            this.Controls.Add(this.panelCursor);
            this.KeyPreview = true;
            this.Name = "BCICursorCtrl";
            this.Text = "BCICursorCtrl";
            this.Load += new System.EventHandler(this.BCICursorCtrl_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.BCICursorCtrl_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timer;
        private SelfDrawPannel panelCursor;
        private System.Windows.Forms.Label lblNumHit;
        private System.Windows.Forms.Label lblNumMiss;
        private System.Windows.Forms.Button bntReset;
        private System.Windows.Forms.Label lblAccurate;
        private System.Windows.Forms.Button btnCfg;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnConnect;
    }
}