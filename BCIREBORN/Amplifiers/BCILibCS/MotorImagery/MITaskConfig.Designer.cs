namespace BCILib.MotorImagery
{
    partial class MITaskConfig
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
            System.Windows.Forms.GroupBox groupBox1;
            System.Windows.Forms.Label label1;
            this.labelRepeats = new System.Windows.Forms.Label();
            this.nud_NumRepeats = new System.Windows.Forms.NumericUpDown();
            this.panelTongue = new System.Windows.Forms.Panel();
            this.tboxTKey = new System.Windows.Forms.TextBox();
            this.rbtnTTap = new System.Windows.Forms.RadioButton();
            this.rbtnTImage = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbAlternative = new System.Windows.Forms.RadioButton();
            this.rbRotate = new System.Windows.Forms.RadioButton();
            this.rbOpenClose = new System.Windows.Forms.RadioButton();
            this.panelFeet = new System.Windows.Forms.Panel();
            this.tboxFKey = new System.Windows.Forms.TextBox();
            this.rbtnFTap = new System.Windows.Forms.RadioButton();
            this.rbtnFImage = new System.Windows.Forms.RadioButton();
            this.panelRight = new System.Windows.Forms.Panel();
            this.tboxRKey = new System.Windows.Forms.TextBox();
            this.rbtnRTap = new System.Windows.Forms.RadioButton();
            this.rbtnRImage = new System.Windows.Forms.RadioButton();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.tboxLKey = new System.Windows.Forms.TextBox();
            this.rbtnLTap = new System.Windows.Forms.RadioButton();
            this.rbtnLImage = new System.Windows.Forms.RadioButton();
            this.buttonAllImagery = new System.Windows.Forms.Button();
            this.cboxTongue = new System.Windows.Forms.CheckBox();
            this.cboxIdle = new System.Windows.Forms.CheckBox();
            this.cboxFeet = new System.Windows.Forms.CheckBox();
            this.cboxRight = new System.Windows.Forms.CheckBox();
            this.cboxLeft = new System.Windows.Forms.CheckBox();
            groupBox1 = new System.Windows.Forms.GroupBox();
            label1 = new System.Windows.Forms.Label();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_NumRepeats)).BeginInit();
            this.panelTongue.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panelFeet.SuspendLayout();
            this.panelRight.SuspendLayout();
            this.panelLeft.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            groupBox1.Controls.Add(this.labelRepeats);
            groupBox1.Controls.Add(this.nud_NumRepeats);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(this.panelTongue);
            groupBox1.Controls.Add(this.panel1);
            groupBox1.Controls.Add(this.panelFeet);
            groupBox1.Controls.Add(this.panelRight);
            groupBox1.Controls.Add(this.panelLeft);
            groupBox1.Controls.Add(this.buttonAllImagery);
            groupBox1.Controls.Add(this.cboxTongue);
            groupBox1.Controls.Add(this.cboxIdle);
            groupBox1.Controls.Add(this.cboxFeet);
            groupBox1.Controls.Add(this.cboxRight);
            groupBox1.Controls.Add(this.cboxLeft);
            groupBox1.Location = new System.Drawing.Point(0, 0);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(343, 302);
            groupBox1.TabIndex = 6;
            groupBox1.TabStop = false;
            groupBox1.Text = "Motor Imagery Configuration";
            // 
            // labelRepeats
            // 
            this.labelRepeats.AutoSize = true;
            this.labelRepeats.Location = new System.Drawing.Point(72, 278);
            this.labelRepeats.Name = "labelRepeats";
            this.labelRepeats.Size = new System.Drawing.Size(179, 13);
            this.labelRepeats.TabIndex = 7;
            this.labelRepeats.Text = "Number of  Repeats Before Change:";
            this.labelRepeats.Visible = false;
            // 
            // nud_NumRepeats
            // 
            this.nud_NumRepeats.Location = new System.Drawing.Point(261, 276);
            this.nud_NumRepeats.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nud_NumRepeats.Name = "nud_NumRepeats";
            this.nud_NumRepeats.Size = new System.Drawing.Size(62, 20);
            this.nud_NumRepeats.TabIndex = 4;
            this.nud_NumRepeats.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nud_NumRepeats.Visible = false;
            this.nud_NumRepeats.ValueChanged += new System.EventHandler(this.MIActModeChanged);
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(13, 250);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(72, 13);
            label1.TabIndex = 3;
            label1.Text = "Rehab Action";
            // 
            // panelTongue
            // 
            this.panelTongue.Controls.Add(this.tboxTKey);
            this.panelTongue.Controls.Add(this.rbtnTTap);
            this.panelTongue.Controls.Add(this.rbtnTImage);
            this.panelTongue.Enabled = false;
            this.panelTongue.Location = new System.Drawing.Point(97, 131);
            this.panelTongue.Name = "panelTongue";
            this.panelTongue.Size = new System.Drawing.Size(193, 29);
            this.panelTongue.TabIndex = 2;
            // 
            // tboxTKey
            // 
            this.tboxTKey.Location = new System.Drawing.Point(117, 4);
            this.tboxTKey.Name = "tboxTKey";
            this.tboxTKey.ReadOnly = true;
            this.tboxTKey.Size = new System.Drawing.Size(70, 20);
            this.tboxTKey.TabIndex = 1;
            this.tboxTKey.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SetTapKey);
            // 
            // rbtnTTap
            // 
            this.rbtnTTap.AutoSize = true;
            this.rbtnTTap.Location = new System.Drawing.Point(66, 5);
            this.rbtnTTap.Name = "rbtnTTap";
            this.rbtnTTap.Size = new System.Drawing.Size(44, 17);
            this.rbtnTTap.TabIndex = 0;
            this.rbtnTTap.Text = "Tap";
            this.rbtnTTap.UseVisualStyleBackColor = true;
            this.rbtnTTap.CheckedChanged += new System.EventHandler(this.MIActModeChanged);
            // 
            // rbtnTImage
            // 
            this.rbtnTImage.AutoSize = true;
            this.rbtnTImage.Checked = true;
            this.rbtnTImage.Location = new System.Drawing.Point(4, 5);
            this.rbtnTImage.Name = "rbtnTImage";
            this.rbtnTImage.Size = new System.Drawing.Size(62, 17);
            this.rbtnTImage.TabIndex = 0;
            this.rbtnTImage.TabStop = true;
            this.rbtnTImage.Text = "Imagine";
            this.rbtnTImage.UseVisualStyleBackColor = true;
            this.rbtnTImage.CheckedChanged += new System.EventHandler(this.MIActModeChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rbAlternative);
            this.panel1.Controls.Add(this.rbRotate);
            this.panel1.Controls.Add(this.rbOpenClose);
            this.panel1.Location = new System.Drawing.Point(97, 239);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(230, 29);
            this.panel1.TabIndex = 2;
            // 
            // rbAlternative
            // 
            this.rbAlternative.AutoSize = true;
            this.rbAlternative.Location = new System.Drawing.Point(149, 9);
            this.rbAlternative.Name = "rbAlternative";
            this.rbAlternative.Size = new System.Drawing.Size(77, 17);
            this.rbAlternative.TabIndex = 9;
            this.rbAlternative.Text = "AlterNative";
            this.rbAlternative.UseVisualStyleBackColor = true;
            this.rbAlternative.CheckedChanged += new System.EventHandler(this.rbAlternative_CheckedChanged);
            // 
            // rbRotate
            // 
            this.rbRotate.AutoSize = true;
            this.rbRotate.Location = new System.Drawing.Point(89, 9);
            this.rbRotate.Name = "rbRotate";
            this.rbRotate.Size = new System.Drawing.Size(57, 17);
            this.rbRotate.TabIndex = 9;
            this.rbRotate.Text = "Rotate";
            this.rbRotate.UseVisualStyleBackColor = true;
            // 
            // rbOpenClose
            // 
            this.rbOpenClose.AutoSize = true;
            this.rbOpenClose.Checked = true;
            this.rbOpenClose.Location = new System.Drawing.Point(4, 9);
            this.rbOpenClose.Name = "rbOpenClose";
            this.rbOpenClose.Size = new System.Drawing.Size(82, 17);
            this.rbOpenClose.TabIndex = 8;
            this.rbOpenClose.TabStop = true;
            this.rbOpenClose.Text = "Open/Close";
            this.rbOpenClose.UseVisualStyleBackColor = true;
            // 
            // panelFeet
            // 
            this.panelFeet.Controls.Add(this.tboxFKey);
            this.panelFeet.Controls.Add(this.rbtnFTap);
            this.panelFeet.Controls.Add(this.rbtnFImage);
            this.panelFeet.Enabled = false;
            this.panelFeet.Location = new System.Drawing.Point(97, 168);
            this.panelFeet.Name = "panelFeet";
            this.panelFeet.Size = new System.Drawing.Size(193, 29);
            this.panelFeet.TabIndex = 2;
            // 
            // tboxFKey
            // 
            this.tboxFKey.Location = new System.Drawing.Point(117, 4);
            this.tboxFKey.Name = "tboxFKey";
            this.tboxFKey.ReadOnly = true;
            this.tboxFKey.Size = new System.Drawing.Size(70, 20);
            this.tboxFKey.TabIndex = 1;
            this.tboxFKey.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SetTapKey);
            // 
            // rbtnFTap
            // 
            this.rbtnFTap.AutoSize = true;
            this.rbtnFTap.Location = new System.Drawing.Point(66, 5);
            this.rbtnFTap.Name = "rbtnFTap";
            this.rbtnFTap.Size = new System.Drawing.Size(44, 17);
            this.rbtnFTap.TabIndex = 0;
            this.rbtnFTap.Text = "Tap";
            this.rbtnFTap.UseVisualStyleBackColor = true;
            this.rbtnFTap.CheckedChanged += new System.EventHandler(this.MIActModeChanged);
            // 
            // rbtnFImage
            // 
            this.rbtnFImage.AutoSize = true;
            this.rbtnFImage.Checked = true;
            this.rbtnFImage.Location = new System.Drawing.Point(4, 5);
            this.rbtnFImage.Name = "rbtnFImage";
            this.rbtnFImage.Size = new System.Drawing.Size(62, 17);
            this.rbtnFImage.TabIndex = 0;
            this.rbtnFImage.TabStop = true;
            this.rbtnFImage.Text = "Imagine";
            this.rbtnFImage.UseVisualStyleBackColor = true;
            this.rbtnFImage.CheckedChanged += new System.EventHandler(this.MIActModeChanged);
            // 
            // panelRight
            // 
            this.panelRight.Controls.Add(this.tboxRKey);
            this.panelRight.Controls.Add(this.rbtnRTap);
            this.panelRight.Controls.Add(this.rbtnRImage);
            this.panelRight.Location = new System.Drawing.Point(97, 96);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(193, 29);
            this.panelRight.TabIndex = 2;
            // 
            // tboxRKey
            // 
            this.tboxRKey.Location = new System.Drawing.Point(117, 4);
            this.tboxRKey.Name = "tboxRKey";
            this.tboxRKey.ReadOnly = true;
            this.tboxRKey.Size = new System.Drawing.Size(70, 20);
            this.tboxRKey.TabIndex = 1;
            this.tboxRKey.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SetTapKey);
            // 
            // rbtnRTap
            // 
            this.rbtnRTap.AutoSize = true;
            this.rbtnRTap.Location = new System.Drawing.Point(66, 5);
            this.rbtnRTap.Name = "rbtnRTap";
            this.rbtnRTap.Size = new System.Drawing.Size(44, 17);
            this.rbtnRTap.TabIndex = 0;
            this.rbtnRTap.Text = "Tap";
            this.rbtnRTap.UseVisualStyleBackColor = true;
            this.rbtnRTap.CheckedChanged += new System.EventHandler(this.MIActModeChanged);
            // 
            // rbtnRImage
            // 
            this.rbtnRImage.AutoSize = true;
            this.rbtnRImage.Checked = true;
            this.rbtnRImage.Location = new System.Drawing.Point(4, 5);
            this.rbtnRImage.Name = "rbtnRImage";
            this.rbtnRImage.Size = new System.Drawing.Size(62, 17);
            this.rbtnRImage.TabIndex = 0;
            this.rbtnRImage.TabStop = true;
            this.rbtnRImage.Text = "Imagine";
            this.rbtnRImage.UseVisualStyleBackColor = true;
            this.rbtnRImage.CheckedChanged += new System.EventHandler(this.MIActModeChanged);
            // 
            // panelLeft
            // 
            this.panelLeft.Controls.Add(this.tboxLKey);
            this.panelLeft.Controls.Add(this.rbtnLTap);
            this.panelLeft.Controls.Add(this.rbtnLImage);
            this.panelLeft.Location = new System.Drawing.Point(97, 55);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(193, 29);
            this.panelLeft.TabIndex = 2;
            // 
            // tboxLKey
            // 
            this.tboxLKey.Location = new System.Drawing.Point(117, 4);
            this.tboxLKey.Name = "tboxLKey";
            this.tboxLKey.ReadOnly = true;
            this.tboxLKey.Size = new System.Drawing.Size(70, 20);
            this.tboxLKey.TabIndex = 1;
            this.tboxLKey.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SetTapKey);
            // 
            // rbtnLTap
            // 
            this.rbtnLTap.AutoSize = true;
            this.rbtnLTap.Location = new System.Drawing.Point(66, 5);
            this.rbtnLTap.Name = "rbtnLTap";
            this.rbtnLTap.Size = new System.Drawing.Size(44, 17);
            this.rbtnLTap.TabIndex = 0;
            this.rbtnLTap.Text = "Tap";
            this.rbtnLTap.UseVisualStyleBackColor = true;
            this.rbtnLTap.CheckedChanged += new System.EventHandler(this.MIActModeChanged);
            // 
            // rbtnLImage
            // 
            this.rbtnLImage.AutoSize = true;
            this.rbtnLImage.Checked = true;
            this.rbtnLImage.Location = new System.Drawing.Point(4, 5);
            this.rbtnLImage.Name = "rbtnLImage";
            this.rbtnLImage.Size = new System.Drawing.Size(62, 17);
            this.rbtnLImage.TabIndex = 0;
            this.rbtnLImage.TabStop = true;
            this.rbtnLImage.Text = "Imagine";
            this.rbtnLImage.UseVisualStyleBackColor = true;
            this.rbtnLImage.CheckedChanged += new System.EventHandler(this.MIActModeChanged);
            // 
            // buttonAllImagery
            // 
            this.buttonAllImagery.Location = new System.Drawing.Point(139, 19);
            this.buttonAllImagery.Name = "buttonAllImagery";
            this.buttonAllImagery.Size = new System.Drawing.Size(116, 23);
            this.buttonAllImagery.TabIndex = 1;
            this.buttonAllImagery.Text = "All Imagine";
            this.buttonAllImagery.UseVisualStyleBackColor = true;
            this.buttonAllImagery.Click += new System.EventHandler(this.buttonAllImagery_Click);
            // 
            // cboxTongue
            // 
            this.cboxTongue.AutoSize = true;
            this.cboxTongue.Location = new System.Drawing.Point(16, 139);
            this.cboxTongue.Name = "cboxTongue";
            this.cboxTongue.Size = new System.Drawing.Size(63, 17);
            this.cboxTongue.TabIndex = 0;
            this.cboxTongue.Text = "Tongue";
            this.cboxTongue.UseVisualStyleBackColor = true;
            this.cboxTongue.CheckedChanged += new System.EventHandler(this.MIActionCheckedChanged);
            // 
            // cboxIdle
            // 
            this.cboxIdle.AutoSize = true;
            this.cboxIdle.Location = new System.Drawing.Point(16, 214);
            this.cboxIdle.Name = "cboxIdle";
            this.cboxIdle.Size = new System.Drawing.Size(43, 17);
            this.cboxIdle.TabIndex = 0;
            this.cboxIdle.Text = "Idle";
            this.cboxIdle.UseVisualStyleBackColor = true;
            this.cboxIdle.CheckedChanged += new System.EventHandler(this.MIActionCheckedChanged);
            // 
            // cboxFeet
            // 
            this.cboxFeet.AutoSize = true;
            this.cboxFeet.Location = new System.Drawing.Point(16, 175);
            this.cboxFeet.Name = "cboxFeet";
            this.cboxFeet.Size = new System.Drawing.Size(47, 17);
            this.cboxFeet.TabIndex = 0;
            this.cboxFeet.Text = "Feet";
            this.cboxFeet.UseVisualStyleBackColor = true;
            this.cboxFeet.CheckedChanged += new System.EventHandler(this.MIActionCheckedChanged);
            // 
            // cboxRight
            // 
            this.cboxRight.AutoSize = true;
            this.cboxRight.Checked = true;
            this.cboxRight.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cboxRight.Location = new System.Drawing.Point(16, 100);
            this.cboxRight.Name = "cboxRight";
            this.cboxRight.Size = new System.Drawing.Size(80, 17);
            this.cboxRight.TabIndex = 0;
            this.cboxRight.Text = "Right Hand";
            this.cboxRight.UseVisualStyleBackColor = true;
            this.cboxRight.CheckedChanged += new System.EventHandler(this.MIActionCheckedChanged);
            // 
            // cboxLeft
            // 
            this.cboxLeft.AutoSize = true;
            this.cboxLeft.Checked = true;
            this.cboxLeft.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cboxLeft.Location = new System.Drawing.Point(16, 60);
            this.cboxLeft.Name = "cboxLeft";
            this.cboxLeft.Size = new System.Drawing.Size(73, 17);
            this.cboxLeft.TabIndex = 0;
            this.cboxLeft.Text = "Left Hand";
            this.cboxLeft.UseVisualStyleBackColor = true;
            this.cboxLeft.CheckedChanged += new System.EventHandler(this.MIActionCheckedChanged);
            // 
            // MITaskConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(groupBox1);
            this.Name = "MITaskConfig";
            this.Size = new System.Drawing.Size(346, 319);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_NumRepeats)).EndInit();
            this.panelTongue.ResumeLayout(false);
            this.panelTongue.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panelFeet.ResumeLayout(false);
            this.panelFeet.PerformLayout();
            this.panelRight.ResumeLayout(false);
            this.panelRight.PerformLayout();
            this.panelLeft.ResumeLayout(false);
            this.panelLeft.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox cboxTongue;
        private System.Windows.Forms.CheckBox cboxFeet;
        private System.Windows.Forms.CheckBox cboxRight;
        private System.Windows.Forms.CheckBox cboxLeft;
        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.TextBox tboxLKey;
        private System.Windows.Forms.RadioButton rbtnLTap;
        private System.Windows.Forms.RadioButton rbtnLImage;
        private System.Windows.Forms.Button buttonAllImagery;
        private System.Windows.Forms.Panel panelTongue;
        private System.Windows.Forms.TextBox tboxTKey;
        private System.Windows.Forms.RadioButton rbtnTTap;
        private System.Windows.Forms.RadioButton rbtnTImage;
        private System.Windows.Forms.Panel panelFeet;
        private System.Windows.Forms.TextBox tboxFKey;
        private System.Windows.Forms.RadioButton rbtnFTap;
        private System.Windows.Forms.RadioButton rbtnFImage;
        private System.Windows.Forms.Panel panelRight;
        private System.Windows.Forms.TextBox tboxRKey;
        private System.Windows.Forms.RadioButton rbtnRTap;
        private System.Windows.Forms.RadioButton rbtnRImage;
        private System.Windows.Forms.CheckBox cboxIdle;
        private System.Windows.Forms.RadioButton rbRotate;
        private System.Windows.Forms.RadioButton rbOpenClose;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton rbAlternative;
        private System.Windows.Forms.Label labelRepeats;
        private System.Windows.Forms.NumericUpDown nud_NumRepeats;
    }
}
