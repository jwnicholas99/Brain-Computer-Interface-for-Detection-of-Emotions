namespace BCILib.MotorImagery
{
    partial class MIFLDGameApp
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnChangeCfg = new System.Windows.Forms.Button();
            this.tbMIConfig = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.labelSelAmplifier = new System.Windows.Forms.Label();
            this.textSelChannels = new System.Windows.Forms.TextBox();
            this.buttonAmpSel = new System.Windows.Forms.Button();
            this.groupBoxProcessor = new System.Windows.Forms.GroupBox();
            this.tbRightThr = new System.Windows.Forms.TrackBar();
            this.tbLeftThr = new System.Windows.Forms.TrackBar();
            this.btnSelModel = new System.Windows.Forms.Button();
            this.lblClass2 = new System.Windows.Forms.Label();
            this.lblRightThr = new System.Windows.Forms.Label();
            this.lblLeftThr = new System.Windows.Forms.Label();
            this.lblClass1 = new System.Windows.Forms.Label();
            this.fldScoreViewer = new BCILib.MotorImagery.ArtsBCI.FLDScoreViewer();
            this.textModelChannels = new System.Windows.Forms.TextBox();
            this.buttonTrainModel = new System.Windows.Forms.Button();
            this.lbModelInfo = new System.Windows.Forms.Label();
            this.labelModelName = new System.Windows.Forms.Label();
            this.btnStartCrossArrowTraining = new System.Windows.Forms.Button();
            this.btnStartCursorCtrl = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBoxProcessor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbRightThr)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbLeftThr)).BeginInit();
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
            label2.Location = new System.Drawing.Point(208, 22);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(81, 13);
            label2.TabIndex = 4;
            label2.Text = "Used channels:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnChangeCfg);
            this.groupBox1.Controls.Add(this.tbMIConfig);
            this.groupBox1.Location = new System.Drawing.Point(22, 43);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 66);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "MotorImagery Configuration";
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
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.labelSelAmplifier);
            this.groupBox2.Controls.Add(this.textSelChannels);
            this.groupBox2.Controls.Add(label1);
            this.groupBox2.Controls.Add(this.buttonAmpSel);
            this.groupBox2.Location = new System.Drawing.Point(22, 153);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 110);
            this.groupBox2.TabIndex = 12;
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
            // textSelChannels
            // 
            this.textSelChannels.Location = new System.Drawing.Point(135, 48);
            this.textSelChannels.Name = "textSelChannels";
            this.textSelChannels.ReadOnly = true;
            this.textSelChannels.Size = new System.Drawing.Size(47, 20);
            this.textSelChannels.TabIndex = 8;
            this.textSelChannels.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
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
            // groupBoxProcessor
            // 
            this.groupBoxProcessor.Controls.Add(this.tbRightThr);
            this.groupBoxProcessor.Controls.Add(this.tbLeftThr);
            this.groupBoxProcessor.Controls.Add(this.btnSelModel);
            this.groupBoxProcessor.Controls.Add(this.lblClass2);
            this.groupBoxProcessor.Controls.Add(this.lblRightThr);
            this.groupBoxProcessor.Controls.Add(this.lblLeftThr);
            this.groupBoxProcessor.Controls.Add(this.lblClass1);
            this.groupBoxProcessor.Controls.Add(this.fldScoreViewer);
            this.groupBoxProcessor.Controls.Add(this.textModelChannels);
            this.groupBoxProcessor.Controls.Add(this.buttonTrainModel);
            this.groupBoxProcessor.Controls.Add(this.lbModelInfo);
            this.groupBoxProcessor.Controls.Add(this.labelModelName);
            this.groupBoxProcessor.Controls.Add(label2);
            this.groupBoxProcessor.Location = new System.Drawing.Point(267, 43);
            this.groupBoxProcessor.Name = "groupBoxProcessor";
            this.groupBoxProcessor.Size = new System.Drawing.Size(520, 214);
            this.groupBoxProcessor.TabIndex = 13;
            this.groupBoxProcessor.TabStop = false;
            this.groupBoxProcessor.Text = "Processor";
            // 
            // tbRightThr
            // 
            this.tbRightThr.Location = new System.Drawing.Point(469, 68);
            this.tbRightThr.Minimum = -10;
            this.tbRightThr.Name = "tbRightThr";
            this.tbRightThr.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tbRightThr.Size = new System.Drawing.Size(45, 104);
            this.tbRightThr.TabIndex = 20;
            this.tbRightThr.Value = -3;
            this.tbRightThr.ValueChanged += new System.EventHandler(this.tbThrRight_ValueChanged);
            this.tbRightThr.Scroll += new System.EventHandler(this.tbClassThr_Scroll);
            // 
            // tbLeftThr
            // 
            this.tbLeftThr.Location = new System.Drawing.Point(23, 68);
            this.tbLeftThr.Minimum = -10;
            this.tbLeftThr.Name = "tbLeftThr";
            this.tbLeftThr.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tbLeftThr.Size = new System.Drawing.Size(45, 104);
            this.tbLeftThr.TabIndex = 20;
            this.tbLeftThr.Value = 3;
            this.tbLeftThr.ValueChanged += new System.EventHandler(this.tbLeftThr_ValueChanged);
            this.tbLeftThr.Scroll += new System.EventHandler(this.tbClassThr_Scroll);
            // 
            // btnSelModel
            // 
            this.btnSelModel.Location = new System.Drawing.Point(424, 20);
            this.btnSelModel.Name = "btnSelModel";
            this.btnSelModel.Size = new System.Drawing.Size(47, 23);
            this.btnSelModel.TabIndex = 14;
            this.btnSelModel.Text = "Sel";
            this.btnSelModel.UseVisualStyleBackColor = true;
            this.btnSelModel.Click += new System.EventHandler(this.btnSelModel_Click);
            // 
            // lblClass2
            // 
            this.lblClass2.AutoSize = true;
            this.lblClass2.Location = new System.Drawing.Point(466, 176);
            this.lblClass2.Name = "lblClass2";
            this.lblClass2.Size = new System.Drawing.Size(38, 13);
            this.lblClass2.TabIndex = 17;
            this.lblClass2.Text = "Class2";
            // 
            // lblRightThr
            // 
            this.lblRightThr.Location = new System.Drawing.Point(466, 52);
            this.lblRightThr.Name = "lblRightThr";
            this.lblRightThr.Size = new System.Drawing.Size(38, 13);
            this.lblRightThr.TabIndex = 18;
            this.lblRightThr.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblLeftThr
            // 
            this.lblLeftThr.Location = new System.Drawing.Point(20, 52);
            this.lblLeftThr.Name = "lblLeftThr";
            this.lblLeftThr.Size = new System.Drawing.Size(38, 13);
            this.lblLeftThr.TabIndex = 18;
            this.lblLeftThr.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblClass1
            // 
            this.lblClass1.AutoSize = true;
            this.lblClass1.Location = new System.Drawing.Point(20, 176);
            this.lblClass1.Name = "lblClass1";
            this.lblClass1.Size = new System.Drawing.Size(38, 13);
            this.lblClass1.TabIndex = 18;
            this.lblClass1.Text = "Class1";
            // 
            // fldScoreViewer
            // 
            this.fldScoreViewer.BackColor = System.Drawing.Color.Black;
            this.fldScoreViewer.Location = new System.Drawing.Point(91, 69);
            this.fldScoreViewer.Name = "fldScoreViewer";
            this.fldScoreViewer.Size = new System.Drawing.Size(365, 103);
            this.fldScoreViewer.TabIndex = 10;
            // 
            // textModelChannels
            // 
            this.textModelChannels.Location = new System.Drawing.Point(288, 19);
            this.textModelChannels.Name = "textModelChannels";
            this.textModelChannels.ReadOnly = true;
            this.textModelChannels.Size = new System.Drawing.Size(47, 20);
            this.textModelChannels.TabIndex = 9;
            this.textModelChannels.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // buttonTrainModel
            // 
            this.buttonTrainModel.Location = new System.Drawing.Point(342, 19);
            this.buttonTrainModel.Name = "buttonTrainModel";
            this.buttonTrainModel.Size = new System.Drawing.Size(76, 23);
            this.buttonTrainModel.TabIndex = 6;
            this.buttonTrainModel.Text = "Train Model";
            this.buttonTrainModel.UseVisualStyleBackColor = true;
            this.buttonTrainModel.Click += new System.EventHandler(this.buttonTrainModel_Click);
            // 
            // lbModelInfo
            // 
            this.lbModelInfo.Location = new System.Drawing.Point(106, 46);
            this.lbModelInfo.Name = "lbModelInfo";
            this.lbModelInfo.Size = new System.Drawing.Size(365, 20);
            this.lbModelInfo.TabIndex = 4;
            this.lbModelInfo.Text = "Model message";
            this.lbModelInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelModelName
            // 
            this.labelModelName.AutoSize = true;
            this.labelModelName.Location = new System.Drawing.Point(100, 22);
            this.labelModelName.Name = "labelModelName";
            this.labelModelName.Size = new System.Drawing.Size(67, 13);
            this.labelModelName.TabIndex = 4;
            this.labelModelName.Text = "Model Name";
            // 
            // btnStartCrossArrowTraining
            // 
            this.btnStartCrossArrowTraining.Location = new System.Drawing.Point(56, 343);
            this.btnStartCrossArrowTraining.Name = "btnStartCrossArrowTraining";
            this.btnStartCrossArrowTraining.Size = new System.Drawing.Size(148, 23);
            this.btnStartCrossArrowTraining.TabIndex = 6;
            this.btnStartCrossArrowTraining.Text = "Start CrossArrowTraining";
            this.btnStartCrossArrowTraining.UseVisualStyleBackColor = true;
            this.btnStartCrossArrowTraining.Click += new System.EventHandler(this.btnStartCrossArrowTraining_Click);
            // 
            // btnStartCursorCtrl
            // 
            this.btnStartCursorCtrl.Location = new System.Drawing.Point(500, 355);
            this.btnStartCursorCtrl.Name = "btnStartCursorCtrl";
            this.btnStartCursorCtrl.Size = new System.Drawing.Size(148, 23);
            this.btnStartCursorCtrl.TabIndex = 6;
            this.btnStartCursorCtrl.Text = "Start Cursor Control";
            this.btnStartCursorCtrl.UseVisualStyleBackColor = true;
            this.btnStartCursorCtrl.Click += new System.EventHandler(this.btnStartCursorCtrl_Click);
            // 
            // MIFLDGameApp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(808, 439);
            this.Controls.Add(this.groupBoxProcessor);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnStartCursorCtrl);
            this.Controls.Add(this.btnStartCrossArrowTraining);
            this.Name = "MIFLDGameApp";
            this.Text = "MIFLDGameApp";
            this.Controls.SetChildIndex(this.btnStartCrossArrowTraining, 0);
            this.Controls.SetChildIndex(this.btnStartCursorCtrl, 0);
            this.Controls.SetChildIndex(this.groupBox1, 0);
            this.Controls.SetChildIndex(this.groupBox2, 0);
            this.Controls.SetChildIndex(this.groupBoxProcessor, 0);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBoxProcessor.ResumeLayout(false);
            this.groupBoxProcessor.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbRightThr)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbLeftThr)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnChangeCfg;
        private System.Windows.Forms.TextBox tbMIConfig;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label labelSelAmplifier;
        private System.Windows.Forms.TextBox textSelChannels;
        private System.Windows.Forms.Button buttonAmpSel;
        private System.Windows.Forms.GroupBox groupBoxProcessor;
        private BCILib.MotorImagery.ArtsBCI.FLDScoreViewer fldScoreViewer;
        private System.Windows.Forms.TextBox textModelChannels;
        private System.Windows.Forms.Button buttonTrainModel;
        private System.Windows.Forms.Label lbModelInfo;
        private System.Windows.Forms.Label labelModelName;
        private System.Windows.Forms.Label lblClass2;
        private System.Windows.Forms.Label lblClass1;
        private System.Windows.Forms.Button btnStartCrossArrowTraining;
        private System.Windows.Forms.Button btnSelModel;
        private System.Windows.Forms.Button btnStartCursorCtrl;
        private System.Windows.Forms.TrackBar tbRightThr;
        private System.Windows.Forms.TrackBar tbLeftThr;
        private System.Windows.Forms.Label lblRightThr;
        private System.Windows.Forms.Label lblLeftThr;
    }
}
