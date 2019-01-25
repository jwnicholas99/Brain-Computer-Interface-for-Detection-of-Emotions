using BCILib.Util;
namespace BCILib.Amp
{
    partial class AmpViewer
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AmpViewer));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolAmpName = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolSetChannel = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStart = new System.Windows.Forms.ToolStripButton();
            this.toolRecord = new System.Windows.Forms.ToolStripButton();
            this.toolStop = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolScaleUp = new System.Windows.Forms.ToolStripButton();
            this.toolScaleDown = new System.Windows.Forms.ToolStripButton();
            this.toolAutoScale = new System.Windows.Forms.ToolStripButton();
            this.toolShiftUp = new System.Windows.Forms.ToolStripButton();
            this.toolShiftDown = new System.Windows.Forms.ToolStripButton();
            this.toolAccelerate = new System.Windows.Forms.ToolStripButton();
            this.toolDecelerate = new System.Windows.Forms.ToolStripButton();
            this.toolNotch = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolTextDesc = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.menuStartAutoStim = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTextSamplingRate = new System.Windows.Forms.ToolStripTextBox();
            this.panel1 = new BCILib.Util.SelfDrawPannel();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolAmpName,
            this.toolStripSeparator1,
            this.toolSetChannel,
            this.toolStripSeparator4,
            this.toolStart,
            this.toolRecord,
            this.toolStop,
            this.toolStripSeparator2,
            this.toolScaleUp,
            this.toolScaleDown,
            this.toolAutoScale,
            this.toolShiftUp,
            this.toolShiftDown,
            this.toolAccelerate,
            this.toolDecelerate,
            this.toolNotch,
            this.toolStripSeparator3,
            this.toolTextDesc,
            this.toolStripDropDownButton1,
            this.toolTextSamplingRate});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(710, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolAmpName
            // 
            this.toolAmpName.BackColor = System.Drawing.Color.Red;
            this.toolAmpName.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolAmpName.Name = "toolAmpName";
            this.toolAmpName.Size = new System.Drawing.Size(56, 22);
            this.toolAmpName.Text = "Amplifier";
            this.toolAmpName.Click += new System.EventHandler(this.toolAmpName_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolSetChannel
            // 
            this.toolSetChannel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolSetChannel.Image = ((System.Drawing.Image)(resources.GetObject("toolSetChannel.Image")));
            this.toolSetChannel.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.toolSetChannel.Name = "toolSetChannel";
            this.toolSetChannel.Size = new System.Drawing.Size(23, 22);
            this.toolSetChannel.Text = "Select Channel";
            this.toolSetChannel.Click += new System.EventHandler(this.toolSetChannel_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStart
            // 
            this.toolStart.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStart.Image = ((System.Drawing.Image)(resources.GetObject("toolStart.Image")));
            this.toolStart.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.toolStart.Name = "toolStart";
            this.toolStart.Size = new System.Drawing.Size(23, 22);
            this.toolStart.Text = "Start";
            this.toolStart.Click += new System.EventHandler(this.toolStart_Click);
            // 
            // toolRecord
            // 
            this.toolRecord.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolRecord.Enabled = false;
            this.toolRecord.Image = ((System.Drawing.Image)(resources.GetObject("toolRecord.Image")));
            this.toolRecord.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.toolRecord.Name = "toolRecord";
            this.toolRecord.Size = new System.Drawing.Size(23, 22);
            this.toolRecord.Text = "Record";
            this.toolRecord.Click += new System.EventHandler(this.toolRecord_Click);
            // 
            // toolStop
            // 
            this.toolStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStop.Enabled = false;
            this.toolStop.Image = ((System.Drawing.Image)(resources.GetObject("toolStop.Image")));
            this.toolStop.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.toolStop.Name = "toolStop";
            this.toolStop.Size = new System.Drawing.Size(23, 22);
            this.toolStop.Text = "Stop";
            this.toolStop.Click += new System.EventHandler(this.toolStop_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolScaleUp
            // 
            this.toolScaleUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolScaleUp.Image = ((System.Drawing.Image)(resources.GetObject("toolScaleUp.Image")));
            this.toolScaleUp.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.toolScaleUp.Name = "toolScaleUp";
            this.toolScaleUp.Size = new System.Drawing.Size(23, 22);
            this.toolScaleUp.Text = "Scale Up";
            this.toolScaleUp.Click += new System.EventHandler(this.toolScaleUp_Click);
            // 
            // toolScaleDown
            // 
            this.toolScaleDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolScaleDown.Image = ((System.Drawing.Image)(resources.GetObject("toolScaleDown.Image")));
            this.toolScaleDown.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.toolScaleDown.Name = "toolScaleDown";
            this.toolScaleDown.Size = new System.Drawing.Size(23, 22);
            this.toolScaleDown.Text = "Scale Down";
            this.toolScaleDown.Click += new System.EventHandler(this.toolScaleDown_Click);
            // 
            // toolAutoScale
            // 
            this.toolAutoScale.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolAutoScale.Image = ((System.Drawing.Image)(resources.GetObject("toolAutoScale.Image")));
            this.toolAutoScale.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.toolAutoScale.Name = "toolAutoScale";
            this.toolAutoScale.Size = new System.Drawing.Size(23, 22);
            this.toolAutoScale.Text = "Auto Scale";
            this.toolAutoScale.Click += new System.EventHandler(this.toolAutoScale_Click);
            // 
            // toolShiftUp
            // 
            this.toolShiftUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolShiftUp.Image = ((System.Drawing.Image)(resources.GetObject("toolShiftUp.Image")));
            this.toolShiftUp.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.toolShiftUp.Name = "toolShiftUp";
            this.toolShiftUp.Size = new System.Drawing.Size(23, 22);
            this.toolShiftUp.Text = "Shift Up";
            this.toolShiftUp.ToolTipText = "Shift up";
            this.toolShiftUp.DoubleClick += new System.EventHandler(this.ResetDisplayShift);
            this.toolShiftUp.Click += new System.EventHandler(this.toolShiftUp_Click);
            // 
            // toolShiftDown
            // 
            this.toolShiftDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolShiftDown.Image = ((System.Drawing.Image)(resources.GetObject("toolShiftDown.Image")));
            this.toolShiftDown.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.toolShiftDown.Name = "toolShiftDown";
            this.toolShiftDown.Size = new System.Drawing.Size(23, 22);
            this.toolShiftDown.Text = "Shift Down";
            this.toolShiftDown.DoubleClick += new System.EventHandler(this.ResetDisplayShift);
            this.toolShiftDown.Click += new System.EventHandler(this.toolShiftDown_Click);
            // 
            // toolAccelerate
            // 
            this.toolAccelerate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolAccelerate.Image = ((System.Drawing.Image)(resources.GetObject("toolAccelerate.Image")));
            this.toolAccelerate.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.toolAccelerate.Name = "toolAccelerate";
            this.toolAccelerate.Size = new System.Drawing.Size(23, 22);
            this.toolAccelerate.Text = "Accelerate";
            this.toolAccelerate.Click += new System.EventHandler(this.toolAccelerate_Click);
            // 
            // toolDecelerate
            // 
            this.toolDecelerate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolDecelerate.Image = ((System.Drawing.Image)(resources.GetObject("toolDecelerate.Image")));
            this.toolDecelerate.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.toolDecelerate.Name = "toolDecelerate";
            this.toolDecelerate.Size = new System.Drawing.Size(23, 22);
            this.toolDecelerate.Text = "Decelerate";
            this.toolDecelerate.Click += new System.EventHandler(this.toolDecelerate_Click);
            // 
            // toolNotch
            // 
            this.toolNotch.CheckOnClick = true;
            this.toolNotch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolNotch.Image = ((System.Drawing.Image)(resources.GetObject("toolNotch.Image")));
            this.toolNotch.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.toolNotch.Name = "toolNotch";
            this.toolNotch.Size = new System.Drawing.Size(23, 22);
            this.toolNotch.Text = "Notch Filter";
            this.toolNotch.Click += new System.EventHandler(this.toolNotch_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolTextDesc
            // 
            this.toolTextDesc.BackColor = System.Drawing.Color.Red;
            this.toolTextDesc.Name = "toolTextDesc";
            this.toolTextDesc.ReadOnly = true;
            this.toolTextDesc.Size = new System.Drawing.Size(250, 25);
            this.toolTextDesc.Text = "Description";
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.None;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuStartAutoStim});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(13, 22);
            this.toolStripDropDownButton1.Text = "toolStripDropDownButton1";
            // 
            // menuStartAutoStim
            // 
            this.menuStartAutoStim.Name = "menuStartAutoStim";
            this.menuStartAutoStim.Size = new System.Drawing.Size(184, 22);
            this.menuStartAutoStim.Text = "Auto Test Stim Event";
            this.menuStartAutoStim.Click += new System.EventHandler(this.menuStartAutoStim_Click);
            // 
            // toolTextSamplingRate
            // 
            this.toolTextSamplingRate.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolTextSamplingRate.Name = "toolTextSamplingRate";
            this.toolTextSamplingRate.ReadOnly = true;
            this.toolTextSamplingRate.Size = new System.Drawing.Size(40, 25);
            this.toolTextSamplingRate.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTextSamplingRate.ToolTipText = "Sampling Rate";
            this.toolTextSamplingRate.Click += new System.EventHandler(this.toolTextSamplingRate_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Location = new System.Drawing.Point(0, 28);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(707, 367);
            this.panel1.TabIndex = 1;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // AmpViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "AmpViewer";
            this.Size = new System.Drawing.Size(710, 398);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStart;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolRecord;
        private System.Windows.Forms.ToolStripButton toolStop;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolScaleUp;
        private System.Windows.Forms.ToolStripButton toolScaleDown;
        private System.Windows.Forms.ToolStripButton toolAutoScale;
        private System.Windows.Forms.ToolStripButton toolAccelerate;
        private System.Windows.Forms.ToolStripButton toolDecelerate;
        private System.Windows.Forms.ToolStripButton toolNotch;
        private System.Windows.Forms.ToolStripButton toolSetChannel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripTextBox toolTextDesc;
        private System.Windows.Forms.ToolStripTextBox toolTextSamplingRate;
        private System.Windows.Forms.ToolStripLabel toolAmpName;
        private SelfDrawPannel panel1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem menuStartAutoStim;
        private System.Windows.Forms.ToolStripButton toolShiftUp;
        private System.Windows.Forms.ToolStripButton toolShiftDown;
    }
}
