namespace BCILib.Amp
{
    partial class ZeoConfigForm
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
            this.checkInterpolate = new System.Windows.Forms.CheckBox();
            this.checkRevChannel = new System.Windows.Forms.CheckBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.checkLowFilter = new System.Windows.Forms.CheckBox();
            this.checkDebugLog = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // checkInterpolate
            // 
            this.checkInterpolate.AutoSize = true;
            this.checkInterpolate.Location = new System.Drawing.Point(24, 25);
            this.checkInterpolate.Name = "checkInterpolate";
            this.checkInterpolate.Size = new System.Drawing.Size(125, 17);
            this.checkInterpolate.TabIndex = 0;
            this.checkInterpolate.Text = "Interpolate to 256 Hz";
            this.checkInterpolate.UseVisualStyleBackColor = true;
            // 
            // checkRevChannel
            // 
            this.checkRevChannel.AutoSize = true;
            this.checkRevChannel.Location = new System.Drawing.Point(24, 61);
            this.checkRevChannel.Name = "checkRevChannel";
            this.checkRevChannel.Size = new System.Drawing.Size(145, 17);
            this.checkRevChannel.TabIndex = 0;
            this.checkRevChannel.Text = "Add one reverse channel";
            this.checkRevChannel.UseVisualStyleBackColor = true;
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(100, 179);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 1;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(219, 179);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // checkLowFilter
            // 
            this.checkLowFilter.AutoSize = true;
            this.checkLowFilter.Location = new System.Drawing.Point(24, 95);
            this.checkLowFilter.Name = "checkLowFilter";
            this.checkLowFilter.Size = new System.Drawing.Size(100, 17);
            this.checkLowFilter.TabIndex = 0;
            this.checkLowFilter.Text = "Apply Low Filter";
            this.checkLowFilter.UseVisualStyleBackColor = true;
            // 
            // checkDebugLog
            // 
            this.checkDebugLog.AutoSize = true;
            this.checkDebugLog.Location = new System.Drawing.Point(24, 129);
            this.checkDebugLog.Name = "checkDebugLog";
            this.checkDebugLog.Size = new System.Drawing.Size(79, 17);
            this.checkDebugLog.TabIndex = 0;
            this.checkDebugLog.Text = "Debug Log";
            this.checkDebugLog.UseVisualStyleBackColor = true;
            // 
            // ZeoConfigForm
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(406, 270);
            this.ControlBox = false;
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.checkDebugLog);
            this.Controls.Add(this.checkLowFilter);
            this.Controls.Add(this.checkRevChannel);
            this.Controls.Add(this.checkInterpolate);
            this.Name = "ZeoConfigForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "ZeoConfigForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkInterpolate;
        private System.Windows.Forms.CheckBox checkRevChannel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.CheckBox checkLowFilter;
        private System.Windows.Forms.CheckBox checkDebugLog;
    }
}