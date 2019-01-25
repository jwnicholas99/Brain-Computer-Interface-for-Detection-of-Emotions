namespace BCILib.MotorImagery.ArtsBCI
{
    partial class BCIHapticKnob
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
            this.buttonStartPassiveMI = new System.Windows.Forms.Button();
            this.buttonStartRehab = new System.Windows.Forms.Button();
            this.buttonAmpSel = new System.Windows.Forms.Button();
            this.textSelChannels = new System.Windows.Forms.TextBox();
            this.textModelChannels = new System.Windows.Forms.TextBox();
            this.buttonTrainModel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lb_RehabRepeats = new System.Windows.Forms.Label();
            this.tbRehabAction = new System.Windows.Forms.TextBox();
            this.tbMIConfig = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.labelSelAmplifier = new System.Windows.Forms.Label();
            this.groupBoxProcessor = new System.Windows.Forms.GroupBox();
            this.btnSelModel = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.lb_TestRepeats = new System.Windows.Forms.Label();
            this.cmbHKCommand = new System.Windows.Forms.ComboBox();
            this.tbRhAction = new System.Windows.Forms.TextBox();
            this.btnHKFind = new System.Windows.Forms.Button();
            this.btnHKRehab = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.cmdHandCommand = new System.Windows.Forms.ComboBox();
            this.cboxMIAction = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.btnHandSend = new System.Windows.Forms.Button();
            this.groupBoxCalibraion = new System.Windows.Forms.GroupBox();
            this.cbFeedback = new System.Windows.Forms.CheckBox();
            this.cbPassiveMovement = new System.Windows.Forms.CheckBox();
            this.btnFBAccuHistory = new System.Windows.Forms.Button();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.labelRehab = new System.Windows.Forms.Label();
            this.btnAccuHistory = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBoxProcessor.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
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
            label2.Location = new System.Drawing.Point(6, 23);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(134, 13);
            label2.TabIndex = 4;
            label2.Text = "Numbfer of used channels:";
            // 
            // labelModelName
            // 
            this.labelModelName.AutoSize = true;
            this.labelModelName.Location = new System.Drawing.Point(6, 86);
            this.labelModelName.Name = "labelModelName";
            this.labelModelName.Size = new System.Drawing.Size(67, 13);
            this.labelModelName.TabIndex = 4;
            this.labelModelName.Text = "Model Name";
            // 
            // btnChangeCfg
            // 
            this.btnChangeCfg.Location = new System.Drawing.Point(52, 81);
            this.btnChangeCfg.Name = "btnChangeCfg";
            this.btnChangeCfg.Size = new System.Drawing.Size(89, 23);
            this.btnChangeCfg.TabIndex = 6;
            this.btnChangeCfg.Text = "Change";
            this.btnChangeCfg.UseVisualStyleBackColor = true;
            this.btnChangeCfg.Click += new System.EventHandler(this.btnChange_Click);
            // 
            // buttonStartPassiveMI
            // 
            this.buttonStartPassiveMI.Location = new System.Drawing.Point(22, 62);
            this.buttonStartPassiveMI.Name = "buttonStartPassiveMI";
            this.buttonStartPassiveMI.Size = new System.Drawing.Size(159, 23);
            this.buttonStartPassiveMI.TabIndex = 6;
            this.buttonStartPassiveMI.Text = "Start Data Collection";
            this.buttonStartPassiveMI.UseVisualStyleBackColor = true;
            this.buttonStartPassiveMI.Click += new System.EventHandler(this.buttonStartDataCollection_Click);
            // 
            // buttonStartRehab
            // 
            this.buttonStartRehab.Location = new System.Drawing.Point(24, 62);
            this.buttonStartRehab.Name = "buttonStartRehab";
            this.buttonStartRehab.Size = new System.Drawing.Size(103, 23);
            this.buttonStartRehab.TabIndex = 6;
            this.buttonStartRehab.Text = "Start Rehab";
            this.buttonStartRehab.UseVisualStyleBackColor = true;
            this.buttonStartRehab.Click += new System.EventHandler(this.buttonStartRehab_Click);
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
            this.textModelChannels.Location = new System.Drawing.Point(146, 20);
            this.textModelChannels.Name = "textModelChannels";
            this.textModelChannels.ReadOnly = true;
            this.textModelChannels.Size = new System.Drawing.Size(47, 20);
            this.textModelChannels.TabIndex = 9;
            this.textModelChannels.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // buttonTrainModel
            // 
            this.buttonTrainModel.Location = new System.Drawing.Point(13, 53);
            this.buttonTrainModel.Name = "buttonTrainModel";
            this.buttonTrainModel.Size = new System.Drawing.Size(87, 23);
            this.buttonTrainModel.TabIndex = 6;
            this.buttonTrainModel.Text = "Train Model";
            this.buttonTrainModel.UseVisualStyleBackColor = true;
            this.buttonTrainModel.Click += new System.EventHandler(this.buttonTrainModel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lb_RehabRepeats);
            this.groupBox1.Controls.Add(this.btnChangeCfg);
            this.groupBox1.Controls.Add(this.tbRehabAction);
            this.groupBox1.Controls.Add(this.tbMIConfig);
            this.groupBox1.Location = new System.Drawing.Point(12, 39);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 110);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "MotorImagery Configuration";
            // 
            // lb_RehabRepeats
            // 
            this.lb_RehabRepeats.AutoSize = true;
            this.lb_RehabRepeats.Location = new System.Drawing.Point(126, 61);
            this.lb_RehabRepeats.Name = "lb_RehabRepeats";
            this.lb_RehabRepeats.Size = new System.Drawing.Size(13, 13);
            this.lb_RehabRepeats.TabIndex = 12;
            this.lb_RehabRepeats.Text = "1";
            this.lb_RehabRepeats.Visible = false;
            // 
            // tbRehabAction
            // 
            this.tbRehabAction.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbRehabAction.ForeColor = System.Drawing.Color.Blue;
            this.tbRehabAction.Location = new System.Drawing.Point(89, 32);
            this.tbRehabAction.Name = "tbRehabAction";
            this.tbRehabAction.ReadOnly = true;
            this.tbRehabAction.Size = new System.Drawing.Size(105, 26);
            this.tbRehabAction.TabIndex = 8;
            this.tbRehabAction.Text = "OpenClose";
            this.tbRehabAction.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbRehabAction.TextChanged += new System.EventHandler(this.tbRehabAction_TextChanged);
            // 
            // tbMIConfig
            // 
            this.tbMIConfig.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbMIConfig.ForeColor = System.Drawing.Color.Blue;
            this.tbMIConfig.Location = new System.Drawing.Point(6, 32);
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
            this.groupBox2.Location = new System.Drawing.Point(12, 174);
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
            this.labelSelAmplifier.Size = new System.Drawing.Size(70, 20);
            this.labelSelAmplifier.TabIndex = 5;
            this.labelSelAmplifier.Text = "Amplifier";
            // 
            // groupBoxProcessor
            // 
            this.groupBoxProcessor.Controls.Add(this.textModelChannels);
            this.groupBoxProcessor.Controls.Add(this.btnSelModel);
            this.groupBoxProcessor.Controls.Add(this.buttonTrainModel);
            this.groupBoxProcessor.Controls.Add(this.labelModelName);
            this.groupBoxProcessor.Controls.Add(label2);
            this.groupBoxProcessor.Location = new System.Drawing.Point(464, 39);
            this.groupBoxProcessor.Name = "groupBoxProcessor";
            this.groupBoxProcessor.Size = new System.Drawing.Size(220, 110);
            this.groupBoxProcessor.TabIndex = 10;
            this.groupBoxProcessor.TabStop = false;
            this.groupBoxProcessor.Text = "Processor";
            // 
            // btnSelModel
            // 
            this.btnSelModel.Location = new System.Drawing.Point(113, 53);
            this.btnSelModel.Name = "btnSelModel";
            this.btnSelModel.Size = new System.Drawing.Size(87, 23);
            this.btnSelModel.TabIndex = 6;
            this.btnSelModel.Text = "Select Model";
            this.btnSelModel.UseVisualStyleBackColor = true;
            this.btnSelModel.Click += new System.EventHandler(this.btnSelModel_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.lb_TestRepeats);
            this.groupBox4.Controls.Add(this.cmbHKCommand);
            this.groupBox4.Controls.Add(this.tbRhAction);
            this.groupBox4.Controls.Add(this.btnHKFind);
            this.groupBox4.Controls.Add(this.btnHKRehab);
            this.groupBox4.Location = new System.Drawing.Point(238, 39);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(200, 110);
            this.groupBox4.TabIndex = 10;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Haptic Knob";
            // 
            // lb_TestRepeats
            // 
            this.lb_TestRepeats.AutoSize = true;
            this.lb_TestRepeats.Location = new System.Drawing.Point(77, 27);
            this.lb_TestRepeats.Name = "lb_TestRepeats";
            this.lb_TestRepeats.Size = new System.Drawing.Size(13, 13);
            this.lb_TestRepeats.TabIndex = 12;
            this.lb_TestRepeats.Text = "0";
            this.lb_TestRepeats.Visible = false;
            // 
            // cmbHKCommand
            // 
            this.cmbHKCommand.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbHKCommand.FormattingEnabled = true;
            this.cmbHKCommand.Location = new System.Drawing.Point(97, 23);
            this.cmbHKCommand.Name = "cmbHKCommand";
            this.cmbHKCommand.Size = new System.Drawing.Size(97, 21);
            this.cmbHKCommand.TabIndex = 7;
            // 
            // tbRhAction
            // 
            this.tbRhAction.Location = new System.Drawing.Point(6, 23);
            this.tbRhAction.Name = "tbRhAction";
            this.tbRhAction.ReadOnly = true;
            this.tbRhAction.Size = new System.Drawing.Size(71, 20);
            this.tbRhAction.TabIndex = 7;
            // 
            // btnHKFind
            // 
            this.btnHKFind.Location = new System.Drawing.Point(6, 53);
            this.btnHKFind.Name = "btnHKFind";
            this.btnHKFind.Size = new System.Drawing.Size(75, 23);
            this.btnHKFind.TabIndex = 6;
            this.btnHKFind.Text = "Find HK Ctrl";
            this.btnHKFind.UseVisualStyleBackColor = true;
            this.btnHKFind.Click += new System.EventHandler(this.btnHKFind_Click);
            // 
            // btnHKRehab
            // 
            this.btnHKRehab.Location = new System.Drawing.Point(97, 53);
            this.btnHKRehab.Name = "btnHKRehab";
            this.btnHKRehab.Size = new System.Drawing.Size(97, 23);
            this.btnHKRehab.TabIndex = 6;
            this.btnHKRehab.Text = "Send";
            this.btnHKRehab.UseVisualStyleBackColor = true;
            this.btnHKRehab.Click += new System.EventHandler(this.btnHKRehab_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.cmdHandCommand);
            this.groupBox5.Controls.Add(this.cboxMIAction);
            this.groupBox5.Controls.Add(this.button1);
            this.groupBox5.Controls.Add(this.btnHandSend);
            this.groupBox5.Location = new System.Drawing.Point(238, 174);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(200, 110);
            this.groupBox5.TabIndex = 10;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Animation Cue";
            // 
            // cmdHandCommand
            // 
            this.cmdHandCommand.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmdHandCommand.FormattingEnabled = true;
            this.cmdHandCommand.Location = new System.Drawing.Point(97, 19);
            this.cmdHandCommand.Name = "cmdHandCommand";
            this.cmdHandCommand.Size = new System.Drawing.Size(97, 21);
            this.cmdHandCommand.TabIndex = 7;
            // 
            // cboxMIAction
            // 
            this.cboxMIAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboxMIAction.FormattingEnabled = true;
            this.cboxMIAction.Location = new System.Drawing.Point(6, 19);
            this.cboxMIAction.Name = "cboxMIAction";
            this.cboxMIAction.Size = new System.Drawing.Size(83, 21);
            this.cboxMIAction.TabIndex = 7;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(6, 49);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(83, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "Find Cue View";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnHandSend
            // 
            this.btnHandSend.Location = new System.Drawing.Point(107, 48);
            this.btnHandSend.Name = "btnHandSend";
            this.btnHandSend.Size = new System.Drawing.Size(72, 23);
            this.btnHandSend.TabIndex = 6;
            this.btnHandSend.Text = "Send";
            this.btnHandSend.UseVisualStyleBackColor = true;
            this.btnHandSend.Click += new System.EventHandler(this.btnMove_Click);
            // 
            // groupBoxCalibraion
            // 
            this.groupBoxCalibraion.Controls.Add(this.cbFeedback);
            this.groupBoxCalibraion.Controls.Add(this.cbPassiveMovement);
            this.groupBoxCalibraion.Controls.Add(this.btnFBAccuHistory);
            this.groupBoxCalibraion.Controls.Add(this.buttonStartPassiveMI);
            this.groupBoxCalibraion.Location = new System.Drawing.Point(12, 304);
            this.groupBoxCalibraion.Name = "groupBoxCalibraion";
            this.groupBoxCalibraion.Size = new System.Drawing.Size(322, 100);
            this.groupBoxCalibraion.TabIndex = 11;
            this.groupBoxCalibraion.TabStop = false;
            this.groupBoxCalibraion.Text = "BCI Calibration (with stop)";
            // 
            // cbFeedback
            // 
            this.cbFeedback.AutoSize = true;
            this.cbFeedback.Checked = true;
            this.cbFeedback.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbFeedback.Location = new System.Drawing.Point(150, 29);
            this.cbFeedback.Name = "cbFeedback";
            this.cbFeedback.Size = new System.Drawing.Size(99, 17);
            this.cbFeedback.TabIndex = 7;
            this.cbFeedback.Text = "With Feedback";
            this.cbFeedback.UseVisualStyleBackColor = true;
            // 
            // cbPassiveMovement
            // 
            this.cbPassiveMovement.AutoSize = true;
            this.cbPassiveMovement.Checked = true;
            this.cbPassiveMovement.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbPassiveMovement.Location = new System.Drawing.Point(22, 29);
            this.cbPassiveMovement.Name = "cbPassiveMovement";
            this.cbPassiveMovement.Size = new System.Drawing.Size(113, 17);
            this.cbPassiveMovement.TabIndex = 7;
            this.cbPassiveMovement.Text = "PassiveMovement";
            this.cbPassiveMovement.UseVisualStyleBackColor = true;
            // 
            // btnFBAccuHistory
            // 
            this.btnFBAccuHistory.Location = new System.Drawing.Point(212, 62);
            this.btnFBAccuHistory.Name = "btnFBAccuHistory";
            this.btnFBAccuHistory.Size = new System.Drawing.Size(103, 23);
            this.btnFBAccuHistory.TabIndex = 6;
            this.btnFBAccuHistory.Text = "Accuracy History";
            this.btnFBAccuHistory.UseVisualStyleBackColor = true;
            this.btnFBAccuHistory.Click += new System.EventHandler(this.btnFBAccuHistory_Click);
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.labelRehab);
            this.groupBox8.Controls.Add(this.buttonStartRehab);
            this.groupBox8.Controls.Add(this.btnAccuHistory);
            this.groupBox8.Location = new System.Drawing.Point(345, 304);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(319, 100);
            this.groupBox8.TabIndex = 11;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "BCI Rehabilitation (no stop)";
            // 
            // labelRehab
            // 
            this.labelRehab.AutoSize = true;
            this.labelRehab.Location = new System.Drawing.Point(21, 29);
            this.labelRehab.Name = "labelRehab";
            this.labelRehab.Size = new System.Drawing.Size(145, 13);
            this.labelRehab.TabIndex = 7;
            this.labelRehab.Text = "(Haptic Knob with Feedback)";
            // 
            // btnAccuHistory
            // 
            this.btnAccuHistory.Location = new System.Drawing.Point(172, 62);
            this.btnAccuHistory.Name = "btnAccuHistory";
            this.btnAccuHistory.Size = new System.Drawing.Size(103, 23);
            this.btnAccuHistory.TabIndex = 6;
            this.btnAccuHistory.Text = "Accuracy History";
            this.btnAccuHistory.UseVisualStyleBackColor = true;
            this.btnAccuHistory.Click += new System.EventHandler(this.btnAccuHistory_Click);
            // 
            // BCIHapticKnob
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(696, 439);
            this.Controls.Add(this.groupBox8);
            this.Controls.Add(this.groupBoxCalibraion);
            this.Controls.Add(this.groupBoxProcessor);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Name = "BCIHapticKnob";
            this.Text = "BCI Haptic Knob Rehabilitation";
            this.Controls.SetChildIndex(this.groupBox2, 0);
            this.Controls.SetChildIndex(this.groupBox1, 0);
            this.Controls.SetChildIndex(this.groupBox4, 0);
            this.Controls.SetChildIndex(this.groupBox5, 0);
            this.Controls.SetChildIndex(this.groupBoxProcessor, 0);
            this.Controls.SetChildIndex(this.groupBoxCalibraion, 0);
            this.Controls.SetChildIndex(this.groupBox8, 0);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBoxProcessor.ResumeLayout(false);
            this.groupBoxProcessor.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBoxCalibraion.ResumeLayout(false);
            this.groupBoxCalibraion.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnChangeCfg;
        private System.Windows.Forms.Button buttonStartPassiveMI;
        private System.Windows.Forms.Button buttonStartRehab;
        private System.Windows.Forms.Button buttonAmpSel;
        private System.Windows.Forms.TextBox textSelChannels;
        private System.Windows.Forms.TextBox textModelChannels;
        private System.Windows.Forms.Button buttonTrainModel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label labelSelAmplifier;
        private System.Windows.Forms.GroupBox groupBoxProcessor;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btnHKRehab;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox tbMIConfig;
        private System.Windows.Forms.TextBox tbRehabAction;
        private System.Windows.Forms.ComboBox cboxMIAction;
        private System.Windows.Forms.TextBox tbRhAction;
        private System.Windows.Forms.Button btnHKFind;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBoxCalibraion;
        private System.Windows.Forms.CheckBox cbPassiveMovement;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.CheckBox cbFeedback;
        private System.Windows.Forms.ComboBox cmbHKCommand;
        private System.Windows.Forms.ComboBox cmdHandCommand;
        private System.Windows.Forms.Button btnHandSend;
        private System.Windows.Forms.Label labelRehab;
        private System.Windows.Forms.Label labelModelName;
        private System.Windows.Forms.Label lb_RehabRepeats;
        private System.Windows.Forms.Label lb_TestRepeats;
        private System.Windows.Forms.Button btnSelModel;
        private System.Windows.Forms.Button btnAccuHistory;
        private System.Windows.Forms.Button btnFBAccuHistory;

    }
}
