namespace BCILib.MotorImagery
{
    partial class MITrainingDataCollection
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
            if (disposing && (components != null))
            {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MITrainingDataCollection));
            this.buttonStart = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.InfoTrialNum = new System.Windows.Forms.Label();
            this.InfoStimCode = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripConfig = new System.Windows.Forms.ToolStripButton();
            this.extAppLauncher = new BCILib.App.ExternalBCIApp();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(12, 131);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(75, 23);
            this.buttonStart.TabIndex = 0;
            this.buttonStart.Text = "Start Train";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.Enabled = false;
            this.buttonStop.Location = new System.Drawing.Point(93, 131);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(75, 23);
            this.buttonStop.TabIndex = 10;
            this.buttonStop.Text = "Stop";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // InfoTrialNum
            // 
            this.InfoTrialNum.AutoSize = true;
            this.InfoTrialNum.Location = new System.Drawing.Point(12, 167);
            this.InfoTrialNum.Name = "InfoTrialNum";
            this.InfoTrialNum.Size = new System.Drawing.Size(30, 13);
            this.InfoTrialNum.TabIndex = 11;
            this.InfoTrialNum.Text = "Trial:";
            // 
            // InfoStimCode
            // 
            this.InfoStimCode.AutoSize = true;
            this.InfoStimCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InfoStimCode.Location = new System.Drawing.Point(60, 189);
            this.InfoStimCode.Name = "InfoStimCode";
            this.InfoStimCode.Size = new System.Drawing.Size(184, 37);
            this.InfoStimCode.TabIndex = 12;
            this.InfoStimCode.Text = "STIMCODE";
            this.InfoStimCode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripConfig});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(309, 25);
            this.toolStrip1.TabIndex = 13;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripConfig
            // 
            this.toolStripConfig.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripConfig.Image = ((System.Drawing.Image)(resources.GetObject("toolStripConfig.Image")));
            this.toolStripConfig.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripConfig.Name = "toolStripConfig";
            this.toolStripConfig.Size = new System.Drawing.Size(105, 22);
            this.toolStripConfig.Text = "System Configure";
            this.toolStripConfig.Click += new System.EventHandler(this.toolStripConfig_Click);
            // 
            // extAppLauncher
            // 
            this.extAppLauncher.Location = new System.Drawing.Point(12, 46);
            this.extAppLauncher.Name = "extAppLauncher";
            this.extAppLauncher.Size = new System.Drawing.Size(280, 79);
            this.extAppLauncher.TabIndex = 9;
            // 
            // MITrainingDataCollection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(309, 262);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.InfoStimCode);
            this.Controls.Add(this.InfoTrialNum);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.extAppLauncher);
            this.Controls.Add(this.buttonStart);
            this.Name = "MITrainingDataCollection";
            this.Text = "MITrainingDataCollection";
            this.Load += new System.EventHandler(this.MITrainingDataCollection_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MITrainingDataCollection_FormClosing);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonStart;
        private BCILib.App.ExternalBCIApp extAppLauncher;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.Label InfoTrialNum;
        private System.Windows.Forms.Label InfoStimCode;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripConfig;
    }
}