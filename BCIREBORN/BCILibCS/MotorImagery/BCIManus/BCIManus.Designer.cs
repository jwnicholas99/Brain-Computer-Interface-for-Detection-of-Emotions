namespace BCILib.MotorImagery.BCIManus
{
    partial class BCIManusForm
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
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label2;
            this.labelModelName = new System.Windows.Forms.Label();
            this.btnChangeCfg = new System.Windows.Forms.Button();
            this.btnStartDataCollection = new System.Windows.Forms.Button();
            this.btnStartRehab = new System.Windows.Forms.Button();
            this.buttonAmpSel = new System.Windows.Forms.Button();
            this.textSelChannels = new System.Windows.Forms.TextBox();
            this.textModelChannels = new System.Windows.Forms.TextBox();
            this.buttonTrainModel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tbMIConfig = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.labelSelAmplifier = new System.Windows.Forms.Label();
            this.groupBoxProcessor = new System.Windows.Forms.GroupBox();
            this.btnAccuLogFile = new System.Windows.Forms.Button();
            this.btnRehabAccHistory = new System.Windows.Forms.Button();
            this.btnSupervisedAccHistory = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnBiasReset = new System.Windows.Forms.Button();
            this.panelBestBias = new System.Windows.Forms.Panel();
            this.cbAutoAjust = new System.Windows.Forms.CheckBox();
            this.labelBestScore = new System.Windows.Forms.Label();
            this.btnSetBias = new System.Windows.Forms.Button();
            this.btnRecordEEG = new System.Windows.Forms.Button();
            this.btnStartServer = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.fldScoreViewer = new BCILib.MotorImagery.ArtsBCI.FLDScoreViewer();
            this.tbFeedback = new System.Windows.Forms.TextBox();
            this.trackBarBias = new System.Windows.Forms.TrackBar();
            this.textBoxBias = new System.Windows.Forms.TextBox();
            this.lbModelInfo = new System.Windows.Forms.Label();
            this.groupBoxCalibraion = new System.Windows.Forms.GroupBox();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.cbSupervised = new System.Windows.Forms.CheckBox();
            this.bw_Server = new System.ComponentModel.BackgroundWorker();
            this.bw_ClientSimulator = new System.ComponentModel.BackgroundWorker();
            this.cbSaveBias = new System.Windows.Forms.CheckBox();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBoxProcessor.SuspendLayout();
            this.panelBestBias.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarBias)).BeginInit();
            this.groupBoxCalibraion.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(19, 51);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(98, 13);
            label1.TabIndex = 4;
            label1.Text = "Selected channels:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(139, 22);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(81, 13);
            label2.TabIndex = 4;
            label2.Text = "Used channels:";
            // 
            // labelModelName
            // 
            this.labelModelName.AutoSize = true;
            this.labelModelName.Location = new System.Drawing.Point(39, 22);
            this.labelModelName.Name = "labelModelName";
            this.labelModelName.Size = new System.Drawing.Size(67, 13);
            this.labelModelName.TabIndex = 4;
            this.labelModelName.Text = "Model Name";
            // 
            // btnChangeCfg
            // 
            this.btnChangeCfg.Location = new System.Drawing.Point(105, 25);
            this.btnChangeCfg.Name = "btnChangeCfg";
            this.btnChangeCfg.Size = new System.Drawing.Size(89, 23);
            this.btnChangeCfg.TabIndex = 6;
            this.btnChangeCfg.Text = "Change";
            this.btnChangeCfg.UseVisualStyleBackColor = true;
            this.btnChangeCfg.Click += new System.EventHandler(this.btnChangeCfg_Click);
            // 
            // btnStartDataCollection
            // 
            this.btnStartDataCollection.Enabled = false;
            this.btnStartDataCollection.Location = new System.Drawing.Point(12, 22);
            this.btnStartDataCollection.Name = "btnStartDataCollection";
            this.btnStartDataCollection.Size = new System.Drawing.Size(133, 23);
            this.btnStartDataCollection.TabIndex = 6;
            this.btnStartDataCollection.Text = "Start Data Collection";
            this.btnStartDataCollection.UseVisualStyleBackColor = true;
            this.btnStartDataCollection.Click += new System.EventHandler(this.btnStartDataCollection_Click);
            // 
            // btnStartRehab
            // 
            this.btnStartRehab.Enabled = false;
            this.btnStartRehab.Location = new System.Drawing.Point(10, 44);
            this.btnStartRehab.Name = "btnStartRehab";
            this.btnStartRehab.Size = new System.Drawing.Size(128, 23);
            this.btnStartRehab.TabIndex = 6;
            this.btnStartRehab.Text = "Start Supervised Rehab";
            this.btnStartRehab.UseVisualStyleBackColor = true;
            this.btnStartRehab.Click += new System.EventHandler(this.btnStartRehab_Click);
            // 
            // buttonAmpSel
            // 
            this.buttonAmpSel.Location = new System.Drawing.Point(54, 81);
            this.buttonAmpSel.Name = "buttonAmpSel";
            this.buttonAmpSel.Size = new System.Drawing.Size(72, 23);
            this.buttonAmpSel.TabIndex = 6;
            this.buttonAmpSel.Text = "Select";
            this.buttonAmpSel.UseVisualStyleBackColor = true;
            this.buttonAmpSel.Click += new System.EventHandler(this.buttonAmpSel_Click);
            // 
            // textSelChannels
            // 
            this.textSelChannels.Location = new System.Drawing.Point(135, 48);
            this.textSelChannels.Name = "textSelChannels";
            this.textSelChannels.ReadOnly = true;
            this.textSelChannels.Size = new System.Drawing.Size(47, 20);
            this.textSelChannels.TabIndex = 8;
            this.textSelChannels.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textModelChannels
            // 
            this.textModelChannels.Location = new System.Drawing.Point(239, 19);
            this.textModelChannels.Name = "textModelChannels";
            this.textModelChannels.ReadOnly = true;
            this.textModelChannels.Size = new System.Drawing.Size(47, 20);
            this.textModelChannels.TabIndex = 9;
            this.textModelChannels.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // buttonTrainModel
            // 
            this.buttonTrainModel.Location = new System.Drawing.Point(302, 19);
            this.buttonTrainModel.Name = "buttonTrainModel";
            this.buttonTrainModel.Size = new System.Drawing.Size(87, 23);
            this.buttonTrainModel.TabIndex = 6;
            this.buttonTrainModel.Text = "Train Model";
            this.buttonTrainModel.UseVisualStyleBackColor = true;
            this.buttonTrainModel.Click += new System.EventHandler(this.buttonTrainModel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnChangeCfg);
            this.groupBox1.Controls.Add(this.tbMIConfig);
            this.groupBox1.Location = new System.Drawing.Point(12, 39);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 66);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "MotorImagery Configuration";
            // 
            // tbMIConfig
            // 
            this.tbMIConfig.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbMIConfig.ForeColor = System.Drawing.Color.Blue;
            this.tbMIConfig.Location = new System.Drawing.Point(6, 22);
            this.tbMIConfig.Name = "tbMIConfig";
            this.tbMIConfig.ReadOnly = true;
            this.tbMIConfig.Size = new System.Drawing.Size(78, 26);
            this.tbMIConfig.TabIndex = 8;
            this.tbMIConfig.Text = "LRTFI";
            this.tbMIConfig.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbMIConfig.TextChanged += new System.EventHandler(this.tbMIConfig_TextChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.labelSelAmplifier);
            this.groupBox2.Controls.Add(this.textSelChannels);
            this.groupBox2.Controls.Add(label1);
            this.groupBox2.Controls.Add(this.buttonAmpSel);
            this.groupBox2.Location = new System.Drawing.Point(12, 125);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 110);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Amplifier";
            // 
            // labelSelAmplifier
            // 
            this.labelSelAmplifier.AutoSize = true;
            this.labelSelAmplifier.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSelAmplifier.Location = new System.Drawing.Point(30, 16);
            this.labelSelAmplifier.Name = "labelSelAmplifier";
            this.labelSelAmplifier.Size = new System.Drawing.Size(116, 20);
            this.labelSelAmplifier.TabIndex = 5;
            this.labelSelAmplifier.Text = "Amplifier Name";
            // 
            // groupBoxProcessor
            // 
            this.groupBoxProcessor.Controls.Add(this.cbSaveBias);
            this.groupBoxProcessor.Controls.Add(this.btnAccuLogFile);
            this.groupBoxProcessor.Controls.Add(this.btnRehabAccHistory);
            this.groupBoxProcessor.Controls.Add(this.btnSupervisedAccHistory);
            this.groupBoxProcessor.Controls.Add(this.label4);
            this.groupBoxProcessor.Controls.Add(this.label5);
            this.groupBoxProcessor.Controls.Add(this.btnBiasReset);
            this.groupBoxProcessor.Controls.Add(this.panelBestBias);
            this.groupBoxProcessor.Controls.Add(this.btnRecordEEG);
            this.groupBoxProcessor.Controls.Add(this.btnStartServer);
            this.groupBoxProcessor.Controls.Add(this.label3);
            this.groupBoxProcessor.Controls.Add(this.fldScoreViewer);
            this.groupBoxProcessor.Controls.Add(this.tbFeedback);
            this.groupBoxProcessor.Controls.Add(this.trackBarBias);
            this.groupBoxProcessor.Controls.Add(this.textBoxBias);
            this.groupBoxProcessor.Controls.Add(this.textModelChannels);
            this.groupBoxProcessor.Controls.Add(this.buttonTrainModel);
            this.groupBoxProcessor.Controls.Add(this.lbModelInfo);
            this.groupBoxProcessor.Controls.Add(this.labelModelName);
            this.groupBoxProcessor.Controls.Add(label2);
            this.groupBoxProcessor.Location = new System.Drawing.Point(236, 39);
            this.groupBoxProcessor.Name = "groupBoxProcessor";
            this.groupBoxProcessor.Size = new System.Drawing.Size(428, 357);
            this.groupBoxProcessor.TabIndex = 10;
            this.groupBoxProcessor.TabStop = false;
            this.groupBoxProcessor.Text = "Server";
            // 
            // btnAccuLogFile
            // 
            this.btnAccuLogFile.Location = new System.Drawing.Point(255, 318);
            this.btnAccuLogFile.Name = "btnAccuLogFile";
            this.btnAccuLogFile.Size = new System.Drawing.Size(61, 23);
            this.btnAccuLogFile.TabIndex = 15;
            this.btnAccuLogFile.Text = "Log File";
            this.btnAccuLogFile.UseVisualStyleBackColor = true;
            this.btnAccuLogFile.Click += new System.EventHandler(this.btnAccuLogFile_Click);
            // 
            // btnRehabAccHistory
            // 
            this.btnRehabAccHistory.Location = new System.Drawing.Point(123, 334);
            this.btnRehabAccHistory.Name = "btnRehabAccHistory";
            this.btnRehabAccHistory.Size = new System.Drawing.Size(126, 23);
            this.btnRehabAccHistory.TabIndex = 15;
            this.btnRehabAccHistory.Text = "Rehab Acc History";
            this.btnRehabAccHistory.UseVisualStyleBackColor = true;
            this.btnRehabAccHistory.Click += new System.EventHandler(this.btnRehabAccHistory_Click);
            // 
            // btnSupervisedAccHistory
            // 
            this.btnSupervisedAccHistory.Location = new System.Drawing.Point(123, 305);
            this.btnSupervisedAccHistory.Name = "btnSupervisedAccHistory";
            this.btnSupervisedAccHistory.Size = new System.Drawing.Size(126, 23);
            this.btnSupervisedAccHistory.TabIndex = 15;
            this.btnSupervisedAccHistory.Text = "Supervised Acc History";
            this.btnSupervisedAccHistory.UseVisualStyleBackColor = true;
            this.btnSupervisedAccHistory.Click += new System.EventHandler(this.btnSupervisedAccHistory_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(326, 201);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(24, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Idle";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(184, 202);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(37, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Action";
            // 
            // btnBiasReset
            // 
            this.btnBiasReset.Location = new System.Drawing.Point(359, 215);
            this.btnBiasReset.Name = "btnBiasReset";
            this.btnBiasReset.Size = new System.Drawing.Size(48, 23);
            this.btnBiasReset.TabIndex = 7;
            this.btnBiasReset.Text = "Reset";
            this.btnBiasReset.UseVisualStyleBackColor = true;
            this.btnBiasReset.Click += new System.EventHandler(this.btnBiasReset_Click);
            // 
            // panelBestBias
            // 
            this.panelBestBias.Controls.Add(this.cbAutoAjust);
            this.panelBestBias.Controls.Add(this.labelBestScore);
            this.panelBestBias.Controls.Add(this.btnSetBias);
            this.panelBestBias.Location = new System.Drawing.Point(14, 255);
            this.panelBestBias.Name = "panelBestBias";
            this.panelBestBias.Size = new System.Drawing.Size(408, 33);
            this.panelBestBias.TabIndex = 12;
            // 
            // cbAutoAjust
            // 
            this.cbAutoAjust.AutoSize = true;
            this.cbAutoAjust.Location = new System.Drawing.Point(326, 10);
            this.cbAutoAjust.Name = "cbAutoAjust";
            this.cbAutoAjust.Size = new System.Drawing.Size(76, 17);
            this.cbAutoAjust.TabIndex = 11;
            this.cbAutoAjust.Text = "Autoadjust";
            this.cbAutoAjust.UseVisualStyleBackColor = true;
            // 
            // labelBestScore
            // 
            this.labelBestScore.AutoSize = true;
            this.labelBestScore.Location = new System.Drawing.Point(9, 11);
            this.labelBestScore.Name = "labelBestScore";
            this.labelBestScore.Size = new System.Drawing.Size(57, 13);
            this.labelBestScore.TabIndex = 6;
            this.labelBestScore.Text = "Best score";
            // 
            // btnSetBias
            // 
            this.btnSetBias.Location = new System.Drawing.Point(259, 5);
            this.btnSetBias.Name = "btnSetBias";
            this.btnSetBias.Size = new System.Drawing.Size(61, 23);
            this.btnSetBias.TabIndex = 7;
            this.btnSetBias.Text = "Set Bias";
            this.btnSetBias.UseVisualStyleBackColor = true;
            this.btnSetBias.Click += new System.EventHandler(this.btnSetBias_Click);
            // 
            // btnRecordEEG
            // 
            this.btnRecordEEG.Location = new System.Drawing.Point(318, 318);
            this.btnRecordEEG.Name = "btnRecordEEG";
            this.btnRecordEEG.Size = new System.Drawing.Size(104, 23);
            this.btnRecordEEG.TabIndex = 9;
            this.btnRecordEEG.Text = "Record EEG Data";
            this.btnRecordEEG.UseVisualStyleBackColor = true;
            this.btnRecordEEG.Click += new System.EventHandler(this.btnStartRecord_Click);
            // 
            // btnStartServer
            // 
            this.btnStartServer.Location = new System.Drawing.Point(13, 318);
            this.btnStartServer.Name = "btnStartServer";
            this.btnStartServer.Size = new System.Drawing.Size(104, 23);
            this.btnStartServer.TabIndex = 9;
            this.btnStartServer.Text = "Start Server";
            this.btnStartServer.UseVisualStyleBackColor = true;
            this.btnStartServer.Click += new System.EventHandler(this.btnStartServer_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(67, 220);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(30, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Bias:";
            // 
            // fldScoreViewer
            // 
            this.fldScoreViewer.BackColor = System.Drawing.Color.Black;
            this.fldScoreViewer.Location = new System.Drawing.Point(42, 69);
            this.fldScoreViewer.Name = "fldScoreViewer";
            this.fldScoreViewer.Size = new System.Drawing.Size(365, 103);
            this.fldScoreViewer.TabIndex = 10;
            // 
            // tbFeedback
            // 
            this.tbFeedback.Location = new System.Drawing.Point(42, 178);
            this.tbFeedback.Name = "tbFeedback";
            this.tbFeedback.ReadOnly = true;
            this.tbFeedback.Size = new System.Drawing.Size(365, 20);
            this.tbFeedback.TabIndex = 9;
            this.tbFeedback.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // trackBarBias
            // 
            this.trackBarBias.Location = new System.Drawing.Point(183, 209);
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
            this.textBoxBias.Location = new System.Drawing.Point(103, 217);
            this.textBoxBias.Name = "textBoxBias";
            this.textBoxBias.ReadOnly = true;
            this.textBoxBias.Size = new System.Drawing.Size(48, 20);
            this.textBoxBias.TabIndex = 5;
            // 
            // lbModelInfo
            // 
            this.lbModelInfo.Location = new System.Drawing.Point(57, 46);
            this.lbModelInfo.Name = "lbModelInfo";
            this.lbModelInfo.Size = new System.Drawing.Size(365, 20);
            this.lbModelInfo.TabIndex = 4;
            this.lbModelInfo.Text = "Model message";
            this.lbModelInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBoxCalibraion
            // 
            this.groupBoxCalibraion.Controls.Add(this.btnStartDataCollection);
            this.groupBoxCalibraion.Location = new System.Drawing.Point(34, 251);
            this.groupBoxCalibraion.Name = "groupBoxCalibraion";
            this.groupBoxCalibraion.Size = new System.Drawing.Size(157, 61);
            this.groupBoxCalibraion.TabIndex = 11;
            this.groupBoxCalibraion.TabStop = false;
            this.groupBoxCalibraion.Text = "BCI Calibration (with stop)";
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.cbSupervised);
            this.groupBox8.Controls.Add(this.btnStartRehab);
            this.groupBox8.Location = new System.Drawing.Point(34, 318);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(155, 78);
            this.groupBox8.TabIndex = 11;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "BCI Rehabilitation";
            // 
            // cbSupervised
            // 
            this.cbSupervised.AutoSize = true;
            this.cbSupervised.Checked = true;
            this.cbSupervised.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbSupervised.Location = new System.Drawing.Point(12, 21);
            this.cbSupervised.Name = "cbSupervised";
            this.cbSupervised.Size = new System.Drawing.Size(79, 17);
            this.cbSupervised.TabIndex = 15;
            this.cbSupervised.Text = "Supervised";
            this.cbSupervised.UseVisualStyleBackColor = true;
            // 
            // bw_Server
            // 
            this.bw_Server.WorkerSupportsCancellation = true;
            this.bw_Server.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bw_Server_DoWork);
            this.bw_Server.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bw_Server_RunWorkerCompleted);
            // 
            // bw_ClientSimulator
            // 
            this.bw_ClientSimulator.WorkerSupportsCancellation = true;
            this.bw_ClientSimulator.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bw_ClientSimulator_DoWork);
            this.bw_ClientSimulator.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bw_ClientSimulator_RunWorkerCompleted);
            // 
            // cbSaveBias
            // 
            this.cbSaveBias.AutoSize = true;
            this.cbSaveBias.Location = new System.Drawing.Point(341, 286);
            this.cbSaveBias.Name = "cbSaveBias";
            this.cbSaveBias.Size = new System.Drawing.Size(71, 17);
            this.cbSaveBias.TabIndex = 16;
            this.cbSaveBias.Text = "SaveBias";
            this.cbSaveBias.UseVisualStyleBackColor = true;
            this.cbSaveBias.CheckedChanged += new System.EventHandler(this.cbSaveBias_CheckedChanged);
            // 
            // BCIManusForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(696, 439);
            this.Controls.Add(this.groupBox8);
            this.Controls.Add(this.groupBoxCalibraion);
            this.Controls.Add(this.groupBoxProcessor);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Name = "BCIManusForm";
            this.Text = "BCI Manus for Stroke Rehabilitation";
            this.Controls.SetChildIndex(this.groupBox2, 0);
            this.Controls.SetChildIndex(this.groupBox1, 0);
            this.Controls.SetChildIndex(this.groupBoxProcessor, 0);
            this.Controls.SetChildIndex(this.groupBoxCalibraion, 0);
            this.Controls.SetChildIndex(this.groupBox8, 0);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBoxProcessor.ResumeLayout(false);
            this.groupBoxProcessor.PerformLayout();
            this.panelBestBias.ResumeLayout(false);
            this.panelBestBias.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarBias)).EndInit();
            this.groupBoxCalibraion.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnChangeCfg;
        private System.Windows.Forms.Button btnStartDataCollection;
        private System.Windows.Forms.Button btnStartRehab;
        private System.Windows.Forms.Button buttonAmpSel;
        private System.Windows.Forms.TextBox textSelChannels;
        private System.Windows.Forms.TextBox textModelChannels;
        private System.Windows.Forms.Button buttonTrainModel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label labelSelAmplifier;
        private System.Windows.Forms.GroupBox groupBoxProcessor;
        private System.Windows.Forms.TextBox tbMIConfig;
        private System.Windows.Forms.GroupBox groupBoxCalibraion;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.Label labelModelName;
        private System.Windows.Forms.Button btnStartServer;
        private System.ComponentModel.BackgroundWorker bw_Server;
        private System.ComponentModel.BackgroundWorker bw_ClientSimulator;
        private BCILib.MotorImagery.ArtsBCI.FLDScoreViewer fldScoreViewer;
        private System.Windows.Forms.Label lbModelInfo;
        private System.Windows.Forms.TextBox tbFeedback;
        private System.Windows.Forms.Button btnBiasReset;
        private System.Windows.Forms.Button btnSetBias;
        private System.Windows.Forms.Label labelBestScore;
        private System.Windows.Forms.TextBox textBoxBias;
        private System.Windows.Forms.TrackBar trackBarBias;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox cbAutoAjust;
        private System.Windows.Forms.Panel panelBestBias;
        private System.Windows.Forms.Button btnRecordEEG;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox cbSupervised;
        private System.Windows.Forms.Button btnSupervisedAccHistory;
        private System.Windows.Forms.Button btnAccuLogFile;
        private System.Windows.Forms.Button btnRehabAccHistory;
        private System.Windows.Forms.CheckBox cbSaveBias;

    }
}
