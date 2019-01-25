namespace BCILib.Amp
{
    partial class NeuroScanCfg
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
            this.comboAmpType = new System.Windows.Forms.ComboBox();
            this.textAmpID = new System.Windows.Forms.TextBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textHost = new System.Windows.Forms.TextBox();
            this.textPort = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbCheckTOut = new System.Windows.Forms.CheckBox();
            this.cbDebug = new System.Windows.Forms.CheckBox();
            this.textBoxPortAddr = new System.Windows.Forms.TextBox();
            this.cbSoftware = new System.Windows.Forms.CheckBox();
            this.cbHardware = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioParallelPort = new System.Windows.Forms.RadioButton();
            this.radioUSBMOD4 = new System.Windows.Forms.RadioButton();
            this.panelHardware = new System.Windows.Forms.Panel();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panelHardware.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboAmpType
            // 
            this.comboAmpType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboAmpType.FormattingEnabled = true;
            this.comboAmpType.Location = new System.Drawing.Point(100, 39);
            this.comboAmpType.Name = "comboAmpType";
            this.comboAmpType.Size = new System.Drawing.Size(184, 21);
            this.comboAmpType.TabIndex = 0;
            // 
            // textAmpID
            // 
            this.textAmpID.Location = new System.Drawing.Point(24, 12);
            this.textAmpID.Name = "textAmpID";
            this.textAmpID.ReadOnly = true;
            this.textAmpID.Size = new System.Drawing.Size(260, 20);
            this.textAmpID.TabIndex = 2;
            this.textAmpID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonOK.Location = new System.Drawing.Point(53, 302);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 3;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonCancel.Location = new System.Drawing.Point(176, 302);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Server Host:";
            // 
            // textHost
            // 
            this.textHost.Location = new System.Drawing.Point(92, 9);
            this.textHost.Name = "textHost";
            this.textHost.Size = new System.Drawing.Size(147, 20);
            this.textHost.TabIndex = 4;
            this.textHost.Text = "localhost";
            // 
            // textPort
            // 
            this.textPort.Location = new System.Drawing.Point(92, 35);
            this.textPort.Name = "textPort";
            this.textPort.Size = new System.Drawing.Size(69, 20);
            this.textPort.TabIndex = 4;
            this.textPort.Text = "4000";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 38);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Server Port:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.panelHardware);
            this.groupBox1.Controls.Add(this.cbCheckTOut);
            this.groupBox1.Controls.Add(this.cbDebug);
            this.groupBox1.Controls.Add(this.cbSoftware);
            this.groupBox1.Controls.Add(this.cbHardware);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(24, 136);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(256, 136);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Stimulate Code";
            // 
            // cbCheckTOut
            // 
            this.cbCheckTOut.AutoSize = true;
            this.cbCheckTOut.Location = new System.Drawing.Point(179, 113);
            this.cbCheckTOut.Name = "cbCheckTOut";
            this.cbCheckTOut.Size = new System.Drawing.Size(60, 17);
            this.cbCheckTOut.TabIndex = 1;
            this.cbCheckTOut.Text = "Check ";
            this.cbCheckTOut.UseVisualStyleBackColor = true;
            // 
            // cbDebug
            // 
            this.cbDebug.AutoSize = true;
            this.cbDebug.Location = new System.Drawing.Point(16, 90);
            this.cbDebug.Name = "cbDebug";
            this.cbDebug.Size = new System.Drawing.Size(88, 17);
            this.cbDebug.TabIndex = 1;
            this.cbDebug.Text = "Debug Mode";
            this.cbDebug.UseVisualStyleBackColor = true;
            this.cbDebug.CheckedChanged += new System.EventHandler(this.cbDebug_CheckedChanged);
            // 
            // textBoxPortAddr
            // 
            this.textBoxPortAddr.Location = new System.Drawing.Point(90, 3);
            this.textBoxPortAddr.Name = "textBoxPortAddr";
            this.textBoxPortAddr.Size = new System.Drawing.Size(46, 20);
            this.textBoxPortAddr.TabIndex = 4;
            this.textBoxPortAddr.Text = "378";
            // 
            // cbSoftware
            // 
            this.cbSoftware.AutoSize = true;
            this.cbSoftware.Location = new System.Drawing.Point(16, 67);
            this.cbSoftware.Name = "cbSoftware";
            this.cbSoftware.Size = new System.Drawing.Size(68, 17);
            this.cbSoftware.TabIndex = 1;
            this.cbSoftware.Text = "Software";
            this.cbSoftware.UseVisualStyleBackColor = true;
            // 
            // cbHardware
            // 
            this.cbHardware.AutoSize = true;
            this.cbHardware.Location = new System.Drawing.Point(16, 22);
            this.cbHardware.Name = "cbHardware";
            this.cbHardware.Size = new System.Drawing.Size(72, 17);
            this.cbHardware.TabIndex = 1;
            this.cbHardware.Text = "Hardware";
            this.cbHardware.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 113);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Timeout Check";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textHost);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.textPort);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(24, 66);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(260, 54);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Server";
            // 
            // radioParallelPort
            // 
            this.radioParallelPort.AutoSize = true;
            this.radioParallelPort.Checked = true;
            this.radioParallelPort.Location = new System.Drawing.Point(3, 4);
            this.radioParallelPort.Name = "radioParallelPort";
            this.radioParallelPort.Size = new System.Drawing.Size(81, 17);
            this.radioParallelPort.TabIndex = 5;
            this.radioParallelPort.Text = "Parallel Port";
            this.radioParallelPort.UseVisualStyleBackColor = true;
            // 
            // radioUSBMOD4
            // 
            this.radioUSBMOD4.AutoSize = true;
            this.radioUSBMOD4.Location = new System.Drawing.Point(3, 27);
            this.radioUSBMOD4.Name = "radioUSBMOD4";
            this.radioUSBMOD4.Size = new System.Drawing.Size(78, 17);
            this.radioUSBMOD4.TabIndex = 5;
            this.radioUSBMOD4.Text = "USBMOD4";
            this.radioUSBMOD4.UseVisualStyleBackColor = true;
            // 
            // panelHardware
            // 
            this.panelHardware.Controls.Add(this.textBoxPortAddr);
            this.panelHardware.Controls.Add(this.radioUSBMOD4);
            this.panelHardware.Controls.Add(this.radioParallelPort);
            this.panelHardware.Location = new System.Drawing.Point(92, 19);
            this.panelHardware.Name = "panelHardware";
            this.panelHardware.Size = new System.Drawing.Size(158, 48);
            this.panelHardware.TabIndex = 7;
            // 
            // NeuroScanCfg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(305, 337);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.textAmpID);
            this.Controls.Add(this.comboAmpType);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "NeuroScanCfg";
            this.Text = "NeuroScan Amplifer Specification";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panelHardware.ResumeLayout(false);
            this.panelHardware.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboAmpType;
        private System.Windows.Forms.TextBox textAmpID;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textHost;
        private System.Windows.Forms.TextBox textPort;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox cbHardware;
        private System.Windows.Forms.CheckBox cbSoftware;
        private System.Windows.Forms.CheckBox cbDebug;
        private System.Windows.Forms.CheckBox cbCheckTOut;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxPortAddr;
        private System.Windows.Forms.RadioButton radioUSBMOD4;
        private System.Windows.Forms.RadioButton radioParallelPort;
        private System.Windows.Forms.Panel panelHardware;
    }
}