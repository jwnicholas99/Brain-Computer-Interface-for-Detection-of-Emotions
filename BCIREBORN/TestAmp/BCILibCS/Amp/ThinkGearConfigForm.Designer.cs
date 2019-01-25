namespace BCILib.Amp
{
    partial class ThinkGearConfigForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.comboPortList = new System.Windows.Forms.ComboBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDownNumChannels = new System.Windows.Forms.NumericUpDown();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxBaudRate = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.numericBlkSize = new System.Windows.Forms.NumericUpDown();
            this.checkBoxInterpolation = new System.Windows.Forms.CheckBox();
            this.checkBoxSaveLog = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownNumChannels)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericBlkSize)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Select Port:";
            // 
            // comboPortList
            // 
            this.comboPortList.FormattingEnabled = true;
            this.comboPortList.Location = new System.Drawing.Point(140, 13);
            this.comboPortList.Name = "comboPortList";
            this.comboPortList.Size = new System.Drawing.Size(120, 21);
            this.comboPortList.TabIndex = 1;
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(15, 200);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 2;
            this.buttonOK.Text = "Okey";
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(185, 200);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Number of Channels:";
            // 
            // numericUpDownNumChannels
            // 
            this.numericUpDownNumChannels.Location = new System.Drawing.Point(140, 45);
            this.numericUpDownNumChannels.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.numericUpDownNumChannels.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownNumChannels.Name = "numericUpDownNumChannels";
            this.numericUpDownNumChannels.Size = new System.Drawing.Size(120, 20);
            this.numericUpDownNumChannels.TabIndex = 3;
            this.numericUpDownNumChannels.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 78);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Braud Rate";
            // 
            // comboBoxBaudRate
            // 
            this.comboBoxBaudRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBaudRate.FormattingEnabled = true;
            this.comboBoxBaudRate.Items.AddRange(new object[] {
            "1200",
            "4800",
            "9600",
            "57600",
            "115200"});
            this.comboBoxBaudRate.Location = new System.Drawing.Point(140, 75);
            this.comboBoxBaudRate.Name = "comboBoxBaudRate";
            this.comboBoxBaudRate.Size = new System.Drawing.Size(120, 21);
            this.comboBoxBaudRate.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 109);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Block Size";
            // 
            // numericBlkSize
            // 
            this.numericBlkSize.Location = new System.Drawing.Point(140, 107);
            this.numericBlkSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericBlkSize.Name = "numericBlkSize";
            this.numericBlkSize.Size = new System.Drawing.Size(120, 20);
            this.numericBlkSize.TabIndex = 3;
            this.numericBlkSize.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // checkBoxInterpolation
            // 
            this.checkBoxInterpolation.AutoSize = true;
            this.checkBoxInterpolation.Location = new System.Drawing.Point(58, 144);
            this.checkBoxInterpolation.Name = "checkBoxInterpolation";
            this.checkBoxInterpolation.Size = new System.Drawing.Size(159, 17);
            this.checkBoxInterpolation.TabIndex = 4;
            this.checkBoxInterpolation.Text = "Interpolation for lost samples";
            this.checkBoxInterpolation.UseVisualStyleBackColor = true;
            // 
            // checkBoxSaveLog
            // 
            this.checkBoxSaveLog.AutoSize = true;
            this.checkBoxSaveLog.Location = new System.Drawing.Point(58, 167);
            this.checkBoxSaveLog.Name = "checkBoxSaveLog";
            this.checkBoxSaveLog.Size = new System.Drawing.Size(136, 17);
            this.checkBoxSaveLog.TabIndex = 4;
            this.checkBoxSaveLog.Text = "Save Stream/Data Log";
            this.checkBoxSaveLog.UseVisualStyleBackColor = true;
            // 
            // ThinkGearConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 264);
            this.Controls.Add(this.checkBoxSaveLog);
            this.Controls.Add(this.checkBoxInterpolation);
            this.Controls.Add(this.numericBlkSize);
            this.Controls.Add(this.numericUpDownNumChannels);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.comboBoxBaudRate);
            this.Controls.Add(this.comboPortList);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "ThinkGearConfigForm";
            this.Text = "ThinkGear Amplifier Configuration";
            this.Load += new System.EventHandler(this.ThinkGearConfigForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownNumChannels)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericBlkSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboPortList;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDownNumChannels;
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBoxBaudRate;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numericBlkSize;
        private System.Windows.Forms.CheckBox checkBoxInterpolation;
        private System.Windows.Forms.CheckBox checkBoxSaveLog;

    }
}