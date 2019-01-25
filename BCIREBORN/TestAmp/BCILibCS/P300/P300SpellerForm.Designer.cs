namespace BCILib.P300
{
    partial class P300SpellerForm
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
            this.textBoxOutput = new System.Windows.Forms.TextBox();
            this.panelFlashButtons = new System.Windows.Forms.Panel();
            this.progressBarPrepare = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // textBoxOutput
            // 
            this.textBoxOutput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxOutput.BackColor = System.Drawing.Color.DarkBlue;
            this.textBoxOutput.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxOutput.ForeColor = System.Drawing.Color.Yellow;
            this.textBoxOutput.Location = new System.Drawing.Point(25, 30);
            this.textBoxOutput.Multiline = true;
            this.textBoxOutput.Name = "textBoxOutput";
            this.textBoxOutput.Size = new System.Drawing.Size(746, 104);
            this.textBoxOutput.TabIndex = 0;
            // 
            // panelFlashButtons
            // 
            this.panelFlashButtons.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelFlashButtons.Location = new System.Drawing.Point(3, 140);
            this.panelFlashButtons.Name = "panelFlashButtons";
            this.panelFlashButtons.Size = new System.Drawing.Size(792, 283);
            this.panelFlashButtons.TabIndex = 1;
            this.panelFlashButtons.Resize += new System.EventHandler(this.panelFlashButtons_Resize);
            // 
            // progressBarPrepare
            // 
            this.progressBarPrepare.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBarPrepare.Location = new System.Drawing.Point(3, 429);
            this.progressBarPrepare.Name = "progressBarPrepare";
            this.progressBarPrepare.Size = new System.Drawing.Size(551, 23);
            this.progressBarPrepare.TabIndex = 2;
            this.progressBarPrepare.Visible = false;
            // 
            // P300SpellerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.RoyalBlue;
            this.ClientSize = new System.Drawing.Size(796, 453);
            this.Controls.Add(this.progressBarPrepare);
            this.Controls.Add(this.panelFlashButtons);
            this.Controls.Add(this.textBoxOutput);
            this.Name = "P300SpellerForm";
            this.ShowIcon = false;
            this.Text = "P300SpellerForm";
            this.Load += new System.EventHandler(this.P300SpellerForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxOutput;
        private System.Windows.Forms.Panel panelFlashButtons;
        private System.Windows.Forms.ProgressBar progressBarPrepare;
    }
}