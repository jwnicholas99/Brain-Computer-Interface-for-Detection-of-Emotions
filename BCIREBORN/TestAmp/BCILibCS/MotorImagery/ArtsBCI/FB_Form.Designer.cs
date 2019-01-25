namespace BCILib.MotorImagery.ArtsBCI {
    partial class FB_Form {
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
            this.labelMessage = new System.Windows.Forms.Label();
            this.pictureBoxSmiley = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelResult = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.fb_RehabOpen = new BCILib.MotorImagery.ArtsBCI.FB_RehabScoreBar();
            this.fb_TwoClassBar = new BCILib.MotorImagery.ArtsBCI.FB_TwoClassBar();
            this.uc_HProgressBar = new BCILib.Util.UC_HProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSmiley)).BeginInit();
            this.SuspendLayout();
            // 
            // labelMessage
            // 
            this.labelMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMessage.ForeColor = System.Drawing.Color.Wheat;
            this.labelMessage.Location = new System.Drawing.Point(126, 311);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Size = new System.Drawing.Size(414, 44);
            this.labelMessage.TabIndex = 0;
            this.labelMessage.Text = "Message";
            this.labelMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBoxSmiley
            // 
            this.pictureBoxSmiley.Location = new System.Drawing.Point(0, 1);
            this.pictureBoxSmiley.Name = "pictureBoxSmiley";
            this.pictureBoxSmiley.Size = new System.Drawing.Size(120, 120);
            this.pictureBoxSmiley.TabIndex = 1;
            this.pictureBoxSmiley.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Location = new System.Drawing.Point(126, 1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(393, 51);
            this.panel1.TabIndex = 5;
            // 
            // labelResult
            // 
            this.labelResult.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelResult.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelResult.ForeColor = System.Drawing.Color.Wheat;
            this.labelResult.Location = new System.Drawing.Point(126, 180);
            this.labelResult.Name = "labelResult";
            this.labelResult.Size = new System.Drawing.Size(414, 131);
            this.labelResult.TabIndex = 0;
            this.labelResult.Text = "Result";
            this.labelResult.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.Location = new System.Drawing.Point(554, 7);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(60, 60);
            this.panel2.TabIndex = 6;
            this.panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.panel2_Paint);
            // 
            // fb_RehabOpen
            // 
            this.fb_RehabOpen.Location = new System.Drawing.Point(73, 127);
            this.fb_RehabOpen.Name = "fb_RehabOpen";
            this.fb_RehabOpen.Size = new System.Drawing.Size(47, 214);
            this.fb_RehabOpen.TabIndex = 4;
            this.fb_RehabOpen.Visible = false;
            // 
            // fb_TwoClassBar
            // 
            this.fb_TwoClassBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.fb_TwoClassBar.Location = new System.Drawing.Point(0, 127);
            this.fb_TwoClassBar.Name = "fb_TwoClassBar";
            this.fb_TwoClassBar.Size = new System.Drawing.Size(39, 214);
            this.fb_TwoClassBar.TabIndex = 3;
            this.fb_TwoClassBar.Visible = false;
            // 
            // uc_HProgressBar
            // 
            this.uc_HProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.uc_HProgressBar.Location = new System.Drawing.Point(126, 360);
            this.uc_HProgressBar.Name = "uc_HProgressBar";
            this.uc_HProgressBar.Size = new System.Drawing.Size(393, 44);
            this.uc_HProgressBar.TabIndex = 2;
            this.uc_HProgressBar.Value = 50;
            // 
            // FB_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(621, 407);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.fb_RehabOpen);
            this.Controls.Add(this.fb_TwoClassBar);
            this.Controls.Add(this.uc_HProgressBar);
            this.Controls.Add(this.pictureBoxSmiley);
            this.Controls.Add(this.labelResult);
            this.Controls.Add(this.labelMessage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.Name = "FB_Form";
            this.Text = "MIFeedbackForm";
            this.Resize += new System.EventHandler(this.MIFeedbackForm_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSmiley)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelMessage;
        private System.Windows.Forms.PictureBox pictureBoxSmiley;
        private BCILib.Util.UC_HProgressBar uc_HProgressBar;
        private FB_TwoClassBar fb_TwoClassBar;
        private FB_RehabScoreBar fb_RehabOpen;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label labelResult;
        private System.Windows.Forms.Panel panel2;
    }
}