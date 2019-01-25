namespace BCILib.MotorImagery
{
    partial class BCICursorCfg
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbHSteps = new System.Windows.Forms.TextBox();
            this.tbVSteps = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbTargetSize = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.tbPrepSeconds = new System.Windows.Forms.TextBox();
            this.cbCalibrate = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Number of steps";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(42, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Horizontal:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(42, 70);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Vertical:";
            // 
            // tbHSteps
            // 
            this.tbHSteps.Location = new System.Drawing.Point(108, 45);
            this.tbHSteps.Name = "tbHSteps";
            this.tbHSteps.Size = new System.Drawing.Size(71, 20);
            this.tbHSteps.TabIndex = 1;
            this.tbHSteps.Text = "4";
            this.tbHSteps.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbVSteps
            // 
            this.tbVSteps.Location = new System.Drawing.Point(108, 67);
            this.tbVSteps.Name = "tbVSteps";
            this.tbVSteps.Size = new System.Drawing.Size(71, 20);
            this.tbVSteps.TabIndex = 1;
            this.tbVSteps.Text = "4";
            this.tbVSteps.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(22, 103);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Target Size:";
            // 
            // tbTargetSize
            // 
            this.tbTargetSize.Location = new System.Drawing.Point(108, 100);
            this.tbTargetSize.Name = "tbTargetSize";
            this.tbTargetSize.Size = new System.Drawing.Size(71, 20);
            this.tbTargetSize.TabIndex = 1;
            this.tbTargetSize.Text = "1";
            this.tbTargetSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(145, 208);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(22, 140);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(81, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "PrepareTime(s):";
            // 
            // tbPrepSeconds
            // 
            this.tbPrepSeconds.Location = new System.Drawing.Point(108, 137);
            this.tbPrepSeconds.Name = "tbPrepSeconds";
            this.tbPrepSeconds.Size = new System.Drawing.Size(71, 20);
            this.tbPrepSeconds.TabIndex = 1;
            this.tbPrepSeconds.Text = "5";
            this.tbPrepSeconds.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // cbCalibrate
            // 
            this.cbCalibrate.AutoSize = true;
            this.cbCalibrate.Location = new System.Drawing.Point(225, 139);
            this.cbCalibrate.Name = "cbCalibrate";
            this.cbCalibrate.Size = new System.Drawing.Size(125, 17);
            this.cbCalibrate.TabIndex = 3;
            this.cbCalibrate.Text = "Basement Calibration";
            this.cbCalibrate.UseVisualStyleBackColor = true;
            // 
            // BCICursorCfg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(390, 255);
            this.Controls.Add(this.cbCalibrate);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.tbPrepSeconds);
            this.Controls.Add(this.tbTargetSize);
            this.Controls.Add(this.tbVSteps);
            this.Controls.Add(this.tbHSteps);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Name = "BCICursorCfg";
            this.Text = "Configuration";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbHSteps;
        private System.Windows.Forms.TextBox tbVSteps;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbTargetSize;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbPrepSeconds;
        private System.Windows.Forms.CheckBox cbCalibrate;

    }
}