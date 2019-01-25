namespace BCILib.MotorImagery
{
    partial class FLDAccumulator
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.Label label2;
            this.cbSaveBias = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnBiasReset = new System.Windows.Forms.Button();
            this.panelBestBias = new System.Windows.Forms.Panel();
            this.cbAutoAjust = new System.Windows.Forms.CheckBox();
            this.labelBestScore = new System.Windows.Forms.Label();
            this.btnSetBias = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.tbFeedback = new System.Windows.Forms.TextBox();
            this.trackBarBias = new System.Windows.Forms.TrackBar();
            this.textBoxBias = new System.Windows.Forms.TextBox();
            this.textModelChannels = new System.Windows.Forms.TextBox();
            this.lbModelInfo = new System.Windows.Forms.Label();
            this.labelModelName = new System.Windows.Forms.Label();
            this.fldScoreViewer = new BCILib.MotorImagery.ArtsBCI.FLDScoreViewer();
            label2 = new System.Windows.Forms.Label();
            this.panelBestBias.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarBias)).BeginInit();
            this.SuspendLayout();
            // 
            // cbSaveBias
            // 
            this.cbSaveBias.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbSaveBias.AutoSize = true;
            this.cbSaveBias.Location = new System.Drawing.Point(366, 311);
            this.cbSaveBias.Name = "cbSaveBias";
            this.cbSaveBias.Size = new System.Drawing.Size(71, 17);
            this.cbSaveBias.TabIndex = 31;
            this.cbSaveBias.Text = "SaveBias";
            this.cbSaveBias.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(366, 216);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(24, 13);
            this.label4.TabIndex = 30;
            this.label4.Text = "Idle";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(114, 217);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(37, 13);
            this.label5.TabIndex = 29;
            this.label5.Text = "Action";
            // 
            // btnBiasReset
            // 
            this.btnBiasReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnBiasReset.Location = new System.Drawing.Point(406, 225);
            this.btnBiasReset.Name = "btnBiasReset";
            this.btnBiasReset.Size = new System.Drawing.Size(48, 23);
            this.btnBiasReset.TabIndex = 24;
            this.btnBiasReset.Text = "Reset";
            this.btnBiasReset.UseVisualStyleBackColor = true;
            this.btnBiasReset.Click += new System.EventHandler(this.btnBiasReset_Click);
            // 
            // panelBestBias
            // 
            this.panelBestBias.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.panelBestBias.Controls.Add(this.cbAutoAjust);
            this.panelBestBias.Controls.Add(this.labelBestScore);
            this.panelBestBias.Controls.Add(this.btnSetBias);
            this.panelBestBias.Location = new System.Drawing.Point(7, 272);
            this.panelBestBias.Name = "panelBestBias";
            this.panelBestBias.Size = new System.Drawing.Size(447, 33);
            this.panelBestBias.TabIndex = 28;
            // 
            // cbAutoAjust
            // 
            this.cbAutoAjust.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbAutoAjust.AutoSize = true;
            this.cbAutoAjust.Location = new System.Drawing.Point(359, 10);
            this.cbAutoAjust.Name = "cbAutoAjust";
            this.cbAutoAjust.Size = new System.Drawing.Size(76, 17);
            this.cbAutoAjust.TabIndex = 11;
            this.cbAutoAjust.Text = "Autoadjust";
            this.cbAutoAjust.UseVisualStyleBackColor = true;
            // 
            // labelBestScore
            // 
            this.labelBestScore.AutoSize = true;
            this.labelBestScore.Location = new System.Drawing.Point(15, 11);
            this.labelBestScore.Name = "labelBestScore";
            this.labelBestScore.Size = new System.Drawing.Size(57, 13);
            this.labelBestScore.TabIndex = 6;
            this.labelBestScore.Text = "Best score";
            // 
            // btnSetBias
            // 
            this.btnSetBias.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSetBias.Location = new System.Drawing.Point(290, 5);
            this.btnSetBias.Name = "btnSetBias";
            this.btnSetBias.Size = new System.Drawing.Size(61, 23);
            this.btnSetBias.TabIndex = 7;
            this.btnSetBias.Text = "Set Bias";
            this.btnSetBias.UseVisualStyleBackColor = true;
            this.btnSetBias.Click += new System.EventHandler(this.btnSetBias_Click);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 230);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(30, 13);
            this.label3.TabIndex = 23;
            this.label3.Text = "Bias:";
            // 
            // tbFeedback
            // 
            this.tbFeedback.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tbFeedback.Location = new System.Drawing.Point(7, 191);
            this.tbFeedback.Name = "tbFeedback";
            this.tbFeedback.ReadOnly = true;
            this.tbFeedback.Size = new System.Drawing.Size(447, 20);
            this.tbFeedback.TabIndex = 26;
            this.tbFeedback.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // trackBarBias
            // 
            this.trackBarBias.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarBias.Location = new System.Drawing.Point(109, 224);
            this.trackBarBias.Maximum = 200;
            this.trackBarBias.Minimum = -200;
            this.trackBarBias.Name = "trackBarBias";
            this.trackBarBias.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.trackBarBias.Size = new System.Drawing.Size(291, 45);
            this.trackBarBias.TabIndex = 18;
            this.trackBarBias.TickFrequency = 10;
            this.trackBarBias.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.trackBarBias.ValueChanged += new System.EventHandler(this.trackBarBias_ValueChanged);
            // 
            // textBoxBias
            // 
            this.textBoxBias.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBoxBias.Location = new System.Drawing.Point(55, 227);
            this.textBoxBias.Name = "textBoxBias";
            this.textBoxBias.ReadOnly = true;
            this.textBoxBias.Size = new System.Drawing.Size(48, 20);
            this.textBoxBias.TabIndex = 21;
            // 
            // textModelChannels
            // 
            this.textModelChannels.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textModelChannels.Location = new System.Drawing.Point(310, 5);
            this.textModelChannels.Name = "textModelChannels";
            this.textModelChannels.ReadOnly = true;
            this.textModelChannels.Size = new System.Drawing.Size(47, 20);
            this.textModelChannels.TabIndex = 25;
            this.textModelChannels.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lbModelInfo
            // 
            this.lbModelInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbModelInfo.Location = new System.Drawing.Point(10, 29);
            this.lbModelInfo.Name = "lbModelInfo";
            this.lbModelInfo.Size = new System.Drawing.Size(444, 20);
            this.lbModelInfo.TabIndex = 17;
            this.lbModelInfo.Text = "Model message";
            this.lbModelInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelModelName
            // 
            this.labelModelName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelModelName.AutoSize = true;
            this.labelModelName.Location = new System.Drawing.Point(0, 8);
            this.labelModelName.Name = "labelModelName";
            this.labelModelName.Size = new System.Drawing.Size(67, 13);
            this.labelModelName.TabIndex = 20;
            this.labelModelName.Text = "Model Name";
            // 
            // label2
            // 
            label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(223, 8);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(81, 13);
            label2.TabIndex = 19;
            label2.Text = "Used channels:";
            // 
            // fldScoreViewer
            // 
            this.fldScoreViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fldScoreViewer.BackColor = System.Drawing.Color.Black;
            this.fldScoreViewer.Location = new System.Drawing.Point(7, 53);
            this.fldScoreViewer.Name = "fldScoreViewer";
            this.fldScoreViewer.Size = new System.Drawing.Size(447, 132);
            this.fldScoreViewer.TabIndex = 27;
            // 
            // FLDAccumulator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.cbSaveBias);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnBiasReset);
            this.Controls.Add(this.panelBestBias);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.fldScoreViewer);
            this.Controls.Add(this.tbFeedback);
            this.Controls.Add(this.trackBarBias);
            this.Controls.Add(this.textBoxBias);
            this.Controls.Add(this.textModelChannels);
            this.Controls.Add(this.lbModelInfo);
            this.Controls.Add(this.labelModelName);
            this.Controls.Add(label2);
            this.Name = "FLDAccumulator";
            this.Size = new System.Drawing.Size(460, 327);
            this.panelBestBias.ResumeLayout(false);
            this.panelBestBias.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarBias)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cbSaveBias;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnBiasReset;
        private System.Windows.Forms.Panel panelBestBias;
        private System.Windows.Forms.CheckBox cbAutoAjust;
        private System.Windows.Forms.Label labelBestScore;
        private System.Windows.Forms.Button btnSetBias;
        private System.Windows.Forms.Label label3;
        public BCILib.MotorImagery.ArtsBCI.FLDScoreViewer fldScoreViewer;
        private System.Windows.Forms.TextBox tbFeedback;
        private System.Windows.Forms.TrackBar trackBarBias;
        private System.Windows.Forms.TextBox textBoxBias;
        private System.Windows.Forms.TextBox textModelChannels;
        private System.Windows.Forms.Label lbModelInfo;
        private System.Windows.Forms.Label labelModelName;


    }
}
