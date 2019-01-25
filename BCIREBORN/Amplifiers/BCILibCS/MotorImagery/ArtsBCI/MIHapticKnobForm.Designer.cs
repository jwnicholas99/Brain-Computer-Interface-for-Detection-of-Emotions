namespace BCILib.MotorImagery.ArtsBCI
{
    partial class MIHapticKnobForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MIHapticKnobForm));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripConfig = new System.Windows.Forms.ToolStripButton();
            this.toolStripStartTrain = new System.Windows.Forms.ToolStripButton();
            this.toolStripStop = new System.Windows.Forms.ToolStripButton();
            this.toolStripBtnPause = new System.Windows.Forms.ToolStripButton();
            this.toolStripTest = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusMsg = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusFeedBack = new System.Windows.Forms.ToolStripStatusLabel();
            this.trackBarBias = new System.Windows.Forms.TrackBar();
            this.textBoxBias = new System.Windows.Forms.TextBox();
            this.panelBiasSetting = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tbThreshold = new System.Windows.Forms.TextBox();
            this.btnBiasReset = new System.Windows.Forms.Button();
            this.fldScoreViewer = new BCILib.MotorImagery.ArtsBCI.FLDScoreViewer();
            this.panelBestBias = new System.Windows.Forms.Panel();
            this.labelBestScore = new System.Windows.Forms.Label();
            this.cbAutoAjust = new System.Windows.Forms.CheckBox();
            this.btnSetBias = new System.Windows.Forms.Button();
            this.groupBoxFBConfig = new System.Windows.Forms.GroupBox();
            this.cbDispMsg = new System.Windows.Forms.CheckBox();
            this.cbSmiley = new System.Windows.Forms.CheckBox();
            this.cbFLDScore = new System.Windows.Forms.CheckBox();
            this.cbSaveBias = new System.Windows.Forms.CheckBox();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarBias)).BeginInit();
            this.panelBiasSetting.SuspendLayout();
            this.panelBestBias.SuspendLayout();
            this.groupBoxFBConfig.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripConfig,
            this.toolStripStartTrain,
            this.toolStripStop,
            this.toolStripBtnPause,
            this.toolStripTest});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(802, 25);
            this.toolStrip1.TabIndex = 0;
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
            // toolStripStartTrain
            // 
            this.toolStripStartTrain.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripStartTrain.Image = ((System.Drawing.Image)(resources.GetObject("toolStripStartTrain.Image")));
            this.toolStripStartTrain.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripStartTrain.Name = "toolStripStartTrain";
            this.toolStripStartTrain.Size = new System.Drawing.Size(35, 22);
            this.toolStripStartTrain.Text = "Start";
            this.toolStripStartTrain.Click += new System.EventHandler(this.toolStripStartTrain_Click);
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
            // toolStripBtnPause
            // 
            this.toolStripBtnPause.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripBtnPause.Image = ((System.Drawing.Image)(resources.GetObject("toolStripBtnPause.Image")));
            this.toolStripBtnPause.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtnPause.Name = "toolStripBtnPause";
            this.toolStripBtnPause.Size = new System.Drawing.Size(42, 22);
            this.toolStripBtnPause.Text = "Pause";
            this.toolStripBtnPause.Click += new System.EventHandler(this.toolStripBtnPause_Click);
            // 
            // toolStripTest
            // 
            this.toolStripTest.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripTest.Image = ((System.Drawing.Image)(resources.GetObject("toolStripTest.Image")));
            this.toolStripTest.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripTest.Name = "toolStripTest";
            this.toolStripTest.Size = new System.Drawing.Size(33, 22);
            this.toolStripTest.Text = "Test";
            this.toolStripTest.Click += new System.EventHandler(this.toolStripTest_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusMsg,
            this.toolStripStatusFeedBack});
            this.statusStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.statusStrip1.Location = new System.Drawing.Point(0, 607);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(802, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusMsg
            // 
            this.toolStripStatusMsg.BackColor = System.Drawing.Color.Transparent;
            this.toolStripStatusMsg.Name = "toolStripStatusMsg";
            this.toolStripStatusMsg.Size = new System.Drawing.Size(53, 17);
            this.toolStripStatusMsg.Spring = true;
            this.toolStripStatusMsg.Text = "Message";
            // 
            // toolStripStatusFeedBack
            // 
            this.toolStripStatusFeedBack.AutoSize = false;
            this.toolStripStatusFeedBack.BackColor = System.Drawing.Color.Transparent;
            this.toolStripStatusFeedBack.Name = "toolStripStatusFeedBack";
            this.toolStripStatusFeedBack.Size = new System.Drawing.Size(240, 17);
            this.toolStripStatusFeedBack.Text = "MI Feedback";
            // 
            // trackBarBias
            // 
            this.trackBarBias.Location = new System.Drawing.Point(134, 169);
            this.trackBarBias.Maximum = 200;
            this.trackBarBias.Minimum = -200;
            this.trackBarBias.Name = "trackBarBias";
            this.trackBarBias.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.trackBarBias.Size = new System.Drawing.Size(164, 45);
            this.trackBarBias.TabIndex = 4;
            this.trackBarBias.TickFrequency = 10;
            this.trackBarBias.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.trackBarBias.ValueChanged += new System.EventHandler(this.trackBarBias_ValueChanged);
            // 
            // textBoxBias
            // 
            this.textBoxBias.Location = new System.Drawing.Point(75, 183);
            this.textBoxBias.Name = "textBoxBias";
            this.textBoxBias.ReadOnly = true;
            this.textBoxBias.Size = new System.Drawing.Size(39, 20);
            this.textBoxBias.TabIndex = 5;
            this.textBoxBias.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // panelBiasSetting
            // 
            this.panelBiasSetting.BackColor = System.Drawing.SystemColors.Control;
            this.panelBiasSetting.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelBiasSetting.Controls.Add(this.label2);
            this.panelBiasSetting.Controls.Add(this.cbSaveBias);
            this.panelBiasSetting.Controls.Add(this.label1);
            this.panelBiasSetting.Controls.Add(this.tbThreshold);
            this.panelBiasSetting.Controls.Add(this.btnBiasReset);
            this.panelBiasSetting.Controls.Add(this.fldScoreViewer);
            this.panelBiasSetting.Controls.Add(this.panelBestBias);
            this.panelBiasSetting.Controls.Add(this.textBoxBias);
            this.panelBiasSetting.Controls.Add(this.trackBarBias);
            this.panelBiasSetting.Location = new System.Drawing.Point(22, 48);
            this.panelBiasSetting.Name = "panelBiasSetting";
            this.panelBiasSetting.Size = new System.Drawing.Size(392, 324);
            this.panelBiasSetting.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(286, 156);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(24, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Idle";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(144, 157);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Action";
            // 
            // tbThreshold
            // 
            this.tbThreshold.Location = new System.Drawing.Point(15, 13);
            this.tbThreshold.Name = "tbThreshold";
            this.tbThreshold.ReadOnly = true;
            this.tbThreshold.Size = new System.Drawing.Size(357, 20);
            this.tbThreshold.TabIndex = 9;
            this.tbThreshold.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnBiasReset
            // 
            this.btnBiasReset.Location = new System.Drawing.Point(314, 181);
            this.btnBiasReset.Name = "btnBiasReset";
            this.btnBiasReset.Size = new System.Drawing.Size(48, 23);
            this.btnBiasReset.TabIndex = 7;
            this.btnBiasReset.Text = "Reset";
            this.btnBiasReset.UseVisualStyleBackColor = true;
            this.btnBiasReset.Click += new System.EventHandler(this.btnBiasReset_Click);
            // 
            // fldScoreViewer
            // 
            this.fldScoreViewer.BackColor = System.Drawing.Color.Black;
            this.fldScoreViewer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.fldScoreViewer.Location = new System.Drawing.Point(13, 48);
            this.fldScoreViewer.Name = "fldScoreViewer";
            this.fldScoreViewer.Size = new System.Drawing.Size(365, 103);
            this.fldScoreViewer.TabIndex = 6;
            // 
            // panelBestBias
            // 
            this.panelBestBias.Controls.Add(this.labelBestScore);
            this.panelBestBias.Controls.Add(this.cbAutoAjust);
            this.panelBestBias.Controls.Add(this.btnSetBias);
            this.panelBestBias.Location = new System.Drawing.Point(28, 220);
            this.panelBestBias.Name = "panelBestBias";
            this.panelBestBias.Size = new System.Drawing.Size(334, 43);
            this.panelBestBias.TabIndex = 10;
            // 
            // labelBestScore
            // 
            this.labelBestScore.AutoSize = true;
            this.labelBestScore.Location = new System.Drawing.Point(12, 16);
            this.labelBestScore.Name = "labelBestScore";
            this.labelBestScore.Size = new System.Drawing.Size(60, 13);
            this.labelBestScore.TabIndex = 6;
            this.labelBestScore.Text = "Best score:";
            // 
            // cbAutoAjust
            // 
            this.cbAutoAjust.AutoSize = true;
            this.cbAutoAjust.Location = new System.Drawing.Point(255, 17);
            this.cbAutoAjust.Name = "cbAutoAjust";
            this.cbAutoAjust.Size = new System.Drawing.Size(76, 17);
            this.cbAutoAjust.TabIndex = 12;
            this.cbAutoAjust.Text = "Autoadjust";
            this.cbAutoAjust.UseVisualStyleBackColor = true;
            // 
            // btnSetBias
            // 
            this.btnSetBias.Location = new System.Drawing.Point(179, 11);
            this.btnSetBias.Name = "btnSetBias";
            this.btnSetBias.Size = new System.Drawing.Size(61, 23);
            this.btnSetBias.TabIndex = 7;
            this.btnSetBias.Text = "Set Bias";
            this.btnSetBias.UseVisualStyleBackColor = true;
            this.btnSetBias.Click += new System.EventHandler(this.btnSetBias_Click);
            // 
            // groupBoxFBConfig
            // 
            this.groupBoxFBConfig.BackColor = System.Drawing.SystemColors.Control;
            this.groupBoxFBConfig.Controls.Add(this.cbDispMsg);
            this.groupBoxFBConfig.Controls.Add(this.cbSmiley);
            this.groupBoxFBConfig.Controls.Add(this.cbFLDScore);
            this.groupBoxFBConfig.Location = new System.Drawing.Point(427, 347);
            this.groupBoxFBConfig.Name = "groupBoxFBConfig";
            this.groupBoxFBConfig.Size = new System.Drawing.Size(320, 122);
            this.groupBoxFBConfig.TabIndex = 8;
            this.groupBoxFBConfig.TabStop = false;
            this.groupBoxFBConfig.Text = "Feedback Configuration";
            // 
            // cbDispMsg
            // 
            this.cbDispMsg.AutoSize = true;
            this.cbDispMsg.Location = new System.Drawing.Point(46, 90);
            this.cbDispMsg.Name = "cbDispMsg";
            this.cbDispMsg.Size = new System.Drawing.Size(69, 17);
            this.cbDispMsg.TabIndex = 0;
            this.cbDispMsg.Text = "Message";
            this.cbDispMsg.UseVisualStyleBackColor = true;
            this.cbDispMsg.Click += new System.EventHandler(this.cbDispMsg_Click);
            // 
            // cbSmiley
            // 
            this.cbSmiley.AutoSize = true;
            this.cbSmiley.Location = new System.Drawing.Point(46, 67);
            this.cbSmiley.Name = "cbSmiley";
            this.cbSmiley.Size = new System.Drawing.Size(83, 17);
            this.cbSmiley.TabIndex = 0;
            this.cbSmiley.Text = "Smiley Face";
            this.cbSmiley.UseVisualStyleBackColor = true;
            this.cbSmiley.Click += new System.EventHandler(this.cbSmiley_Click);
            // 
            // cbFLDScore
            // 
            this.cbFLDScore.AutoSize = true;
            this.cbFLDScore.Location = new System.Drawing.Point(46, 44);
            this.cbFLDScore.Name = "cbFLDScore";
            this.cbFLDScore.Size = new System.Drawing.Size(77, 17);
            this.cbFLDScore.TabIndex = 0;
            this.cbFLDScore.Text = "FLD Score";
            this.cbFLDScore.UseVisualStyleBackColor = true;
            this.cbFLDScore.Click += new System.EventHandler(this.cbFLDScore_Click);
            // 
            // cbSaveBias
            // 
            this.cbSaveBias.AutoSize = true;
            this.cbSaveBias.Location = new System.Drawing.Point(283, 269);
            this.cbSaveBias.Name = "cbSaveBias";
            this.cbSaveBias.Size = new System.Drawing.Size(71, 17);
            this.cbSaveBias.TabIndex = 12;
            this.cbSaveBias.Text = "SaveBias";
            this.cbSaveBias.UseVisualStyleBackColor = true;
            this.cbSaveBias.CheckedChanged += new System.EventHandler(this.cbSaveBias_CheckedChanged);
            // 
            // MIHapticKnobForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(802, 629);
            this.Controls.Add(this.groupBoxFBConfig);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.panelBiasSetting);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "MIHapticKnobForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Text = "BCI Motor Imagery";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MIHapticKnobForm_FormClosing);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarBias)).EndInit();
            this.panelBiasSetting.ResumeLayout(false);
            this.panelBiasSetting.PerformLayout();
            this.panelBestBias.ResumeLayout(false);
            this.panelBestBias.PerformLayout();
            this.groupBoxFBConfig.ResumeLayout(false);
            this.groupBoxFBConfig.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripStartTrain;
        private System.Windows.Forms.ToolStripButton toolStripStop;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripButton toolStripConfig;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusMsg;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusFeedBack;
        private System.Windows.Forms.ToolStripButton toolStripBtnPause;
        private System.Windows.Forms.TrackBar trackBarBias;
        private System.Windows.Forms.TextBox textBoxBias;
        private FLDScoreViewer fldScoreViewer;
        private System.Windows.Forms.ToolStripButton toolStripTest;
        private System.Windows.Forms.Panel panelBiasSetting;
        private System.Windows.Forms.Button btnSetBias;
        private System.Windows.Forms.Label labelBestScore;
        private System.Windows.Forms.GroupBox groupBoxFBConfig;
        private System.Windows.Forms.CheckBox cbFLDScore;
        private System.Windows.Forms.CheckBox cbSmiley;
        private System.Windows.Forms.CheckBox cbDispMsg;
        private System.Windows.Forms.TextBox tbThreshold;
        private System.Windows.Forms.Button btnBiasReset;
        private System.Windows.Forms.CheckBox cbAutoAjust;
        private System.Windows.Forms.Panel panelBestBias;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cbSaveBias;
    }
}