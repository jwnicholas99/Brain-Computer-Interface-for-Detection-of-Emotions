namespace BCILib.P300
{
    partial class ModelDataPathForm
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
            this.textBoxClassifyDataPath = new System.Windows.Forms.TextBox();
            this.buttonResetClsPath = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxRejectionDataPath = new System.Windows.Forms.TextBox();
            this.buttonResetRejPath = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(144, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Classification Model Training:";
            // 
            // textBoxClassifyDataPath
            // 
            this.textBoxClassifyDataPath.Location = new System.Drawing.Point(171, 37);
            this.textBoxClassifyDataPath.Name = "textBoxClassifyDataPath";
            this.textBoxClassifyDataPath.Size = new System.Drawing.Size(271, 20);
            this.textBoxClassifyDataPath.TabIndex = 1;
            // 
            // buttonResetClsPath
            // 
            this.buttonResetClsPath.Location = new System.Drawing.Point(459, 35);
            this.buttonResetClsPath.Name = "buttonResetClsPath";
            this.buttonResetClsPath.Size = new System.Drawing.Size(75, 23);
            this.buttonResetClsPath.TabIndex = 2;
            this.buttonResetClsPath.Text = "Re-Train";
            this.buttonResetClsPath.UseVisualStyleBackColor = true;
            this.buttonResetClsPath.Click += new System.EventHandler(this.buttonResetClsPath_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(128, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Rejection Model Training:";
            // 
            // textBoxRejectionDataPath
            // 
            this.textBoxRejectionDataPath.Location = new System.Drawing.Point(171, 66);
            this.textBoxRejectionDataPath.Name = "textBoxRejectionDataPath";
            this.textBoxRejectionDataPath.Size = new System.Drawing.Size(271, 20);
            this.textBoxRejectionDataPath.TabIndex = 1;
            // 
            // buttonResetRejPath
            // 
            this.buttonResetRejPath.Location = new System.Drawing.Point(459, 64);
            this.buttonResetRejPath.Name = "buttonResetRejPath";
            this.buttonResetRejPath.Size = new System.Drawing.Size(75, 23);
            this.buttonResetRejPath.TabIndex = 2;
            this.buttonResetRejPath.Text = "Re-Train";
            this.buttonResetRejPath.UseVisualStyleBackColor = true;
            this.buttonResetRejPath.Click += new System.EventHandler(this.buttonResetRejPath_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(88, 110);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 2;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(308, 110);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // TrainingModelSpecForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(552, 164);
            this.Controls.Add(this.buttonResetRejPath);
            this.Controls.Add(this.textBoxRejectionDataPath);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonResetClsPath);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxClassifyDataPath);
            this.Controls.Add(this.label1);
            this.Name = "TrainingModelSpecForm";
            this.Text = "Model Path Specification";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxClassifyDataPath;
        private System.Windows.Forms.Button buttonResetClsPath;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxRejectionDataPath;
        private System.Windows.Forms.Button buttonResetRejPath;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
    }
}