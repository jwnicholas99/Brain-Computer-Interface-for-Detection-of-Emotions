namespace BCILib.MotorImagery
{
    partial class MotorImageGame
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MotorImageGame));
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripCrossArrow = new System.Windows.Forms.ToolStripButton();
            this.toolStripTraining = new System.Windows.Forms.ToolStripButton();
            this.miCfg = new BCILib.MotorImagery.MITaskConfig();
            this.panel_MIScore = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.selAmpChannel1 = new BCILib.Amp.SelAmpChannel();
            this.buttonStart = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.textNumChannels = new System.Windows.Forms.TextBox();
            this.externalGameChooser1 = new BCILib.App.ExternalBCIApp();
            this.labelResult = new System.Windows.Forms.Label();
            this.buttonStartTrain = new System.Windows.Forms.Button();
            this.toolStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip2
            // 
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripCrossArrow,
            this.toolStripTraining});
            this.toolStrip2.Location = new System.Drawing.Point(0, 25);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(643, 25);
            this.toolStrip2.TabIndex = 5;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // toolStripCrossArrow
            // 
            this.toolStripCrossArrow.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripCrossArrow.Image = ((System.Drawing.Image)(resources.GetObject("toolStripCrossArrow.Image")));
            this.toolStripCrossArrow.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripCrossArrow.Name = "toolStripCrossArrow";
            this.toolStripCrossArrow.Size = new System.Drawing.Size(145, 22);
            this.toolStripCrossArrow.Text = "Record EEG(Cross/Arrow)";
            this.toolStripCrossArrow.Click += new System.EventHandler(this.toolStripCrossArrow_Click);
            // 
            // toolStripTraining
            // 
            this.toolStripTraining.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripTraining.Image = ((System.Drawing.Image)(resources.GetObject("toolStripTraining.Image")));
            this.toolStripTraining.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripTraining.Name = "toolStripTraining";
            this.toolStripTraining.Size = new System.Drawing.Size(75, 22);
            this.toolStripTraining.Text = "Train Model";
            this.toolStripTraining.Click += new System.EventHandler(this.toolStripTraining_Click);
            // 
            // miCfg
            // 
            this.miCfg.Action = BCILib.MotorImagery.ArtsBCI.RehabAction.OpenClose;
            this.miCfg.KeyCodes = new string[] {
        "None",
        "None",
        "",
        "None"};
            this.miCfg.KeyCodeString = "None|None||None";
            this.miCfg.Location = new System.Drawing.Point(0, 68);
            this.miCfg.Name = "miCfg";
            this.miCfg.NumRepeats = 1;
            this.miCfg.Size = new System.Drawing.Size(305, 257);
            this.miCfg.TabIndex = 0;
            this.miCfg.TaskString = "LR";
            // 
            // panel_MIScore
            // 
            this.panel_MIScore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.panel_MIScore.BackColor = System.Drawing.Color.White;
            this.panel_MIScore.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel_MIScore.Font = new System.Drawing.Font("Arial Black", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel_MIScore.Location = new System.Drawing.Point(410, 419);
            this.panel_MIScore.Name = "panel_MIScore";
            this.panel_MIScore.Size = new System.Drawing.Size(114, 24);
            this.panel_MIScore.TabIndex = 3;
            this.panel_MIScore.Paint += new System.Windows.Forms.PaintEventHandler(this.panel_MIScore_Paint);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.Location = new System.Drawing.Point(320, 373);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(192, 23);
            this.label3.TabIndex = 1;
            this.label3.Text = "Motor Imagery Result";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // selAmpChannel1
            // 
            this.selAmpChannel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.selAmpChannel1.Location = new System.Drawing.Point(323, 68);
            this.selAmpChannel1.Name = "selAmpChannel1";
            this.selAmpChannel1.SelectedList = new string[0];
            this.selAmpChannel1.SelectedNum = 0;
            this.selAmpChannel1.SelectedString = "";
            this.selAmpChannel1.Size = new System.Drawing.Size(217, 270);
            this.selAmpChannel1.TabIndex = 5;
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(546, 158);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(75, 23);
            this.buttonStart.TabIndex = 6;
            this.buttonStart.Text = "Test";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.Enabled = false;
            this.buttonStop.Location = new System.Drawing.Point(547, 194);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(75, 23);
            this.buttonStop.TabIndex = 6;
            this.buttonStop.Text = "Stop";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // textNumChannels
            // 
            this.textNumChannels.Location = new System.Drawing.Point(550, 93);
            this.textNumChannels.Name = "textNumChannels";
            this.textNumChannels.ReadOnly = true;
            this.textNumChannels.Size = new System.Drawing.Size(71, 20);
            this.textNumChannels.TabIndex = 7;
            this.textNumChannels.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // externalGameChooser1
            // 
            this.externalGameChooser1.Location = new System.Drawing.Point(25, 373);
            this.externalGameChooser1.Name = "externalGameChooser1";
            this.externalGameChooser1.Size = new System.Drawing.Size(280, 79);
            this.externalGameChooser1.TabIndex = 8;
            // 
            // labelResult
            // 
            this.labelResult.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelResult.Font = new System.Drawing.Font("Symbol", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.labelResult.Location = new System.Drawing.Point(348, 414);
            this.labelResult.Name = "labelResult";
            this.labelResult.Size = new System.Drawing.Size(44, 37);
            this.labelResult.TabIndex = 9;
            this.labelResult.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // buttonStartTrain
            // 
            this.buttonStartTrain.Location = new System.Drawing.Point(546, 120);
            this.buttonStartTrain.Name = "buttonStartTrain";
            this.buttonStartTrain.Size = new System.Drawing.Size(75, 23);
            this.buttonStartTrain.TabIndex = 10;
            this.buttonStartTrain.Text = "Train";
            this.buttonStartTrain.UseVisualStyleBackColor = true;
            this.buttonStartTrain.Click += new System.EventHandler(this.buttonStartTrain_Click);
            // 
            // MotorImageGame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(643, 510);
            this.Controls.Add(this.buttonStartTrain);
            this.Controls.Add(this.labelResult);
            this.Controls.Add(this.externalGameChooser1);
            this.Controls.Add(this.textNumChannels);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.selAmpChannel1);
            this.Controls.Add(this.panel_MIScore);
            this.Controls.Add(this.miCfg);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.toolStrip2);
            this.Name = "MotorImageGame";
            this.Text = "MotorImageGame";
            this.Load += new System.EventHandler(this.MotorImageGame_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MotorImageGame_FormClosing);
            this.Controls.SetChildIndex(this.toolStrip2, 0);
            this.Controls.SetChildIndex(this.label3, 0);
            this.Controls.SetChildIndex(this.miCfg, 0);
            this.Controls.SetChildIndex(this.panel_MIScore, 0);
            this.Controls.SetChildIndex(this.selAmpChannel1, 0);
            this.Controls.SetChildIndex(this.buttonStart, 0);
            this.Controls.SetChildIndex(this.buttonStop, 0);
            this.Controls.SetChildIndex(this.textNumChannels, 0);
            this.Controls.SetChildIndex(this.externalGameChooser1, 0);
            this.Controls.SetChildIndex(this.labelResult, 0);
            this.Controls.SetChildIndex(this.buttonStartTrain, 0);
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip2;
        private MITaskConfig miCfg;
        private System.Windows.Forms.ToolStripButton toolStripTraining;
        private System.Windows.Forms.Panel panel_MIScore;
        private System.Windows.Forms.Label label3;
        private BCILib.Amp.SelAmpChannel selAmpChannel1;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.TextBox textNumChannels;
        private System.Windows.Forms.ToolStripButton toolStripCrossArrow;
        private BCILib.App.ExternalBCIApp externalGameChooser1;
        private System.Windows.Forms.Label labelResult;
        private System.Windows.Forms.Button buttonStartTrain;

    }
}