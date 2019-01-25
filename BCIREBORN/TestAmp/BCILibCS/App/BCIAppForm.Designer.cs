namespace BCILib.App
{
    partial class BCIAppForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BCIAppForm));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripSysConfig = new System.Windows.Forms.ToolStripButton();
            this.toolStripAmp = new System.Windows.Forms.ToolStripButton();
            this.toolStripViewOutput = new System.Windows.Forms.ToolStripButton();
            this.tsbPause = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusMessage = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslBias = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslRaw = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslBCI = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSysConfig,
            this.toolStripAmp,
            this.toolStripViewOutput,
            this.tsbPause});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(808, 25);
            this.toolStrip1.TabIndex = 1;
            // 
            // toolStripSysConfig
            // 
            this.toolStripSysConfig.Image = ((System.Drawing.Image)(resources.GetObject("toolStripSysConfig.Image")));
            this.toolStripSysConfig.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSysConfig.Name = "toolStripSysConfig";
            this.toolStripSysConfig.Size = new System.Drawing.Size(121, 22);
            this.toolStripSysConfig.Text = "System Configure";
            this.toolStripSysConfig.Click += new System.EventHandler(this.toolStripSysConfig_Click);
            // 
            // toolStripAmp
            // 
            this.toolStripAmp.Image = ((System.Drawing.Image)(resources.GetObject("toolStripAmp.Image")));
            this.toolStripAmp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripAmp.Name = "toolStripAmp";
            this.toolStripAmp.Size = new System.Drawing.Size(76, 22);
            this.toolStripAmp.Text = "Amplifier";
            this.toolStripAmp.Click += new System.EventHandler(this.toolStripAmp_Click);
            // 
            // toolStripViewOutput
            // 
            this.toolStripViewOutput.Image = ((System.Drawing.Image)(resources.GetObject("toolStripViewOutput.Image")));
            this.toolStripViewOutput.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripViewOutput.Name = "toolStripViewOutput";
            this.toolStripViewOutput.Size = new System.Drawing.Size(93, 22);
            this.toolStripViewOutput.Text = "View Output";
            this.toolStripViewOutput.Click += new System.EventHandler(this.toolStripViewLog_Click);
            // 
            // tsbPause
            // 
            this.tsbPause.Image = global::BCILib.Properties.Resources.Image_Pause;
            this.tsbPause.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbPause.Name = "tsbPause";
            this.tsbPause.Size = new System.Drawing.Size(58, 22);
            this.tsbPause.Text = "Pause";
            this.tsbPause.Click += new System.EventHandler(this.tsbPause_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusMessage,
            this.tsslBias,
            this.tsslRaw,
            this.tsslBCI});
            this.statusStrip1.Location = new System.Drawing.Point(0, 417);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(808, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusMessage
            // 
            this.statusMessage.AutoSize = false;
            this.statusMessage.Name = "statusMessage";
            this.statusMessage.Size = new System.Drawing.Size(703, 17);
            this.statusMessage.Spring = true;
            this.statusMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tsslBias
            // 
            this.tsslBias.AutoSize = false;
            this.tsslBias.Name = "tsslBias";
            this.tsslBias.Size = new System.Drawing.Size(30, 17);
            // 
            // tsslRaw
            // 
            this.tsslRaw.AutoSize = false;
            this.tsslRaw.Name = "tsslRaw";
            this.tsslRaw.Size = new System.Drawing.Size(30, 17);
            // 
            // tsslBCI
            // 
            this.tsslBCI.AutoSize = false;
            this.tsslBCI.Name = "tsslBCI";
            this.tsslBCI.Size = new System.Drawing.Size(30, 17);
            // 
            // BCIAppForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(808, 439);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BCIAppForm";
            this.Text = "BCIAppForm";
            this.Load += new System.EventHandler(this.BCIAppForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BCIAppForm_FormClosing);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStripButton toolStripAmp;
        protected System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripSysConfig;
        private System.Windows.Forms.ToolStripButton toolStripViewOutput;
        protected System.Windows.Forms.StatusStrip statusStrip1;
        protected System.Windows.Forms.ToolStripStatusLabel statusMessage;
        public System.Windows.Forms.ToolStripStatusLabel tsslBias;
        public System.Windows.Forms.ToolStripStatusLabel tsslRaw;
        public System.Windows.Forms.ToolStripStatusLabel tsslBCI;
        private System.Windows.Forms.ToolStripButton tsbPause;

    }
}