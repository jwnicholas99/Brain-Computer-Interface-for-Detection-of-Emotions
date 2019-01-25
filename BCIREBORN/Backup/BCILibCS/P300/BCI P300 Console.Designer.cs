namespace BCILib.P300
{
    partial class BCI_P300_Console
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
            this.p300ConfigCtrl1 = new BCILib.P300.P300ConfigCtrl();
            this.SuspendLayout();
            // 
            // p300ConfigCtrl1
            // 
            this.p300ConfigCtrl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.p300ConfigCtrl1.Location = new System.Drawing.Point(0, 25);
            this.p300ConfigCtrl1.Name = "p300ConfigCtrl1";
            this.p300ConfigCtrl1.Size = new System.Drawing.Size(808, 392);
            this.p300ConfigCtrl1.TabIndex = 4;
            this.p300ConfigCtrl1.Tag = "m";
            // 
            // BCI_P300_Console
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(808, 439);
            this.Controls.Add(this.p300ConfigCtrl1);
            this.Name = "BCI_P300_Console";
            this.Text = "BCI P300 Console";
            this.Load += new System.EventHandler(this.BCI_P300_Console_Load);
            this.Controls.SetChildIndex(this.p300ConfigCtrl1, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private P300ConfigCtrl p300ConfigCtrl1;
    }
}
