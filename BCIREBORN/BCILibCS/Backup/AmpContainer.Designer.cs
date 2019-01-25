namespace BCILib.Amp
{
    partial class AmpContainer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AmpContainer));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripAddAmplifier = new System.Windows.Forms.ToolStripSplitButton();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripConfig = new System.Windows.Forms.ToolStripButton();
            this.toolStripSelectAmp = new System.Windows.Forms.ToolStripButton();
            this.toolStripRefresh = new System.Windows.Forms.ToolStripButton();
            this.toolStripTestStim = new System.Windows.Forms.ToolStripButton();
            this.toolStripVuewLog = new System.Windows.Forms.ToolStripButton();
            this.toolStripCBDisplayMode = new System.Windows.Forms.ToolStripComboBox();
            this.toolRecordSplRate = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripAddAmplifier,
            this.toolStripSeparator,
            this.toolStripConfig,
            this.toolStripSelectAmp,
            this.toolStripRefresh,
            this.toolStripTestStim,
            this.toolStripVuewLog,
            this.toolStripCBDisplayMode,
            this.toolRecordSplRate});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(838, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripAddAmplifier
            // 
            this.toolStripAddAmplifier.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripAddAmplifier.Image = ((System.Drawing.Image)(resources.GetObject("toolStripAddAmplifier.Image")));
            this.toolStripAddAmplifier.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripAddAmplifier.Name = "toolStripAddAmplifier";
            this.toolStripAddAmplifier.Size = new System.Drawing.Size(97, 22);
            this.toolStripAddAmplifier.Text = "Add Amplifier";
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripConfig
            // 
            this.toolStripConfig.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripConfig.Image = ((System.Drawing.Image)(resources.GetObject("toolStripConfig.Image")));
            this.toolStripConfig.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripConfig.Name = "toolStripConfig";
            this.toolStripConfig.Size = new System.Drawing.Size(64, 22);
            this.toolStripConfig.Text = "Configure";
            this.toolStripConfig.Click += new System.EventHandler(this.toolStripConfig_Click);
            // 
            // toolStripSelectAmp
            // 
            this.toolStripSelectAmp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripSelectAmp.Image = ((System.Drawing.Image)(resources.GetObject("toolStripSelectAmp.Image")));
            this.toolStripSelectAmp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSelectAmp.Name = "toolStripSelectAmp";
            this.toolStripSelectAmp.Size = new System.Drawing.Size(42, 22);
            this.toolStripSelectAmp.Text = "Select";
            this.toolStripSelectAmp.Click += new System.EventHandler(this.toolStripSelectAmp_Click);
            // 
            // toolStripRefresh
            // 
            this.toolStripRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripRefresh.Image = ((System.Drawing.Image)(resources.GetObject("toolStripRefresh.Image")));
            this.toolStripRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripRefresh.Name = "toolStripRefresh";
            this.toolStripRefresh.Size = new System.Drawing.Size(50, 22);
            this.toolStripRefresh.Text = "Refresh";
            this.toolStripRefresh.Click += new System.EventHandler(this.toolStripRefresh_Click);
            // 
            // toolStripTestStim
            // 
            this.toolStripTestStim.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripTestStim.Image = ((System.Drawing.Image)(resources.GetObject("toolStripTestStim.Image")));
            this.toolStripTestStim.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripTestStim.Name = "toolStripTestStim";
            this.toolStripTestStim.Size = new System.Drawing.Size(60, 22);
            this.toolStripTestStim.Text = "Test Stim";
            this.toolStripTestStim.Click += new System.EventHandler(this.toolStripTestStim_Click);
            // 
            // toolStripVuewLog
            // 
            this.toolStripVuewLog.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripVuewLog.Image = ((System.Drawing.Image)(resources.GetObject("toolStripVuewLog.Image")));
            this.toolStripVuewLog.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripVuewLog.Name = "toolStripVuewLog";
            this.toolStripVuewLog.Size = new System.Drawing.Size(59, 22);
            this.toolStripVuewLog.Text = "View Log";
            this.toolStripVuewLog.Click += new System.EventHandler(this.toolStripVuewLog_Click);
            // 
            // toolStripCBDisplayMode
            // 
            this.toolStripCBDisplayMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripCBDisplayMode.DropDownWidth = 50;
            this.toolStripCBDisplayMode.Items.AddRange(new object[] {
            "Move",
            "Wrap"});
            this.toolStripCBDisplayMode.Name = "toolStripCBDisplayMode";
            this.toolStripCBDisplayMode.Size = new System.Drawing.Size(75, 25);
            this.toolStripCBDisplayMode.ToolTipText = "DispayMode";
            this.toolStripCBDisplayMode.SelectedIndexChanged += new System.EventHandler(this.toolStripCBDisplayMode_SelectedIndexChanged);
            // 
            // toolRecordSplRate
            // 
            this.toolRecordSplRate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolRecordSplRate.Image = ((System.Drawing.Image)(resources.GetObject("toolRecordSplRate.Image")));
            this.toolRecordSplRate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolRecordSplRate.Name = "toolRecordSplRate";
            this.toolRecordSplRate.Size = new System.Drawing.Size(121, 22);
            this.toolRecordSplRate.Text = "RecordSamplingRate";
            this.toolRecordSplRate.Click += new System.EventHandler(this.toolRecordSplRate_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 468);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(838, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatus
            // 
            this.toolStripStatus.Name = "toolStripStatus";
            this.toolStripStatus.Size = new System.Drawing.Size(823, 17);
            this.toolStripStatus.Spring = true;
            // 
            // AmpContainer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(838, 490);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "AmpContainer";
            this.Text = "Eval EEG Amplifiers";
            this.Load += new System.EventHandler(this.AmpContainer_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AmpContainer_FormClosing);
            this.Resize += new System.EventHandler(this.AmpContainer_Resize);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.AmpContainer_KeyDown);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatus;
        private System.Windows.Forms.ToolStripButton toolStripConfig;
        private System.Windows.Forms.ToolStripButton toolStripTestStim;
        private System.Windows.Forms.ToolStripButton toolStripVuewLog;
        private System.Windows.Forms.ToolStripButton toolStripRefresh;
        private System.Windows.Forms.ToolStripSplitButton toolStripAddAmplifier;
        private System.Windows.Forms.ToolStripButton toolStripSelectAmp;
        private System.Windows.Forms.ToolStripComboBox toolStripCBDisplayMode;
        private System.Windows.Forms.ToolStripButton toolRecordSplRate;
    }
}

