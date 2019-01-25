namespace BCILib.Concentration
{
    partial class StroopTestCfg
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
            this.components = new System.ComponentModel.Container();
            this.buttonStartStroopTest = new System.Windows.Forms.Button();
            this.buttonTryStroopTest = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkStroopRelaxImage = new System.Windows.Forms.CheckBox();
            this.textStroopSequence = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textStroopTimeCue = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textStroopTimeOut = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.textStroopTimeRest = new System.Windows.Forms.TextBox();
            this.buttonSaveStroopConfig = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.textStroopTrials = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.textStroopTimeRelax = new System.Windows.Forms.TextBox();
            this.buttonDefaultStroop = new System.Windows.Forms.Button();
            this.labelDesignMode = new System.Windows.Forms.Label();
            this.imageListColors = new System.Windows.Forms.ImageList(this.components);
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonStartStroopTest
            // 
            this.buttonStartStroopTest.Location = new System.Drawing.Point(456, 219);
            this.buttonStartStroopTest.Name = "buttonStartStroopTest";
            this.buttonStartStroopTest.Size = new System.Drawing.Size(88, 40);
            this.buttonStartStroopTest.TabIndex = 14;
            this.buttonStartStroopTest.Text = "Start";
            this.buttonStartStroopTest.Click += new System.EventHandler(this.buttonStartStroopTest_Click);
            // 
            // buttonTryStroopTest
            // 
            this.buttonTryStroopTest.Location = new System.Drawing.Point(456, 115);
            this.buttonTryStroopTest.Name = "buttonTryStroopTest";
            this.buttonTryStroopTest.Size = new System.Drawing.Size(88, 40);
            this.buttonTryStroopTest.TabIndex = 13;
            this.buttonTryStroopTest.Text = "Try";
            this.buttonTryStroopTest.Click += new System.EventHandler(this.buttonTryStroopTest_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkStroopRelaxImage);
            this.groupBox1.Controls.Add(this.textStroopSequence);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.textStroopTimeCue);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.textStroopTimeOut);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.textStroopTimeRest);
            this.groupBox1.Controls.Add(this.buttonSaveStroopConfig);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.textStroopTrials);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.textStroopTimeRelax);
            this.groupBox1.Controls.Add(this.buttonDefaultStroop);
            this.groupBox1.Location = new System.Drawing.Point(48, 35);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(328, 271);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Configuration";
            // 
            // checkStroopRelaxImage
            // 
            this.checkStroopRelaxImage.Location = new System.Drawing.Point(14, 232);
            this.checkStroopRelaxImage.Name = "checkStroopRelaxImage";
            this.checkStroopRelaxImage.Size = new System.Drawing.Size(120, 24);
            this.checkStroopRelaxImage.TabIndex = 8;
            this.checkStroopRelaxImage.Text = "Use relax image";
            // 
            // textStroopSequence
            // 
            this.textStroopSequence.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textStroopSequence.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textStroopSequence.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textStroopSequence.Location = new System.Drawing.Point(168, 16);
            this.textStroopSequence.Name = "textStroopSequence";
            this.textStroopSequence.Size = new System.Drawing.Size(152, 26);
            this.textStroopSequence.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(8, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(136, 26);
            this.label5.TabIndex = 2;
            this.label5.Text = "Level Sequences:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(8, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(136, 26);
            this.label3.TabIndex = 2;
            this.label3.Text = "Time Cue (ms)";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textStroopTimeCue
            // 
            this.textStroopTimeCue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textStroopTimeCue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textStroopTimeCue.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textStroopTimeCue.Location = new System.Drawing.Point(168, 80);
            this.textStroopTimeCue.Name = "textStroopTimeCue";
            this.textStroopTimeCue.Size = new System.Drawing.Size(152, 26);
            this.textStroopTimeCue.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(8, 112);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(136, 26);
            this.label4.TabIndex = 2;
            this.label4.Text = "Time Out (ms)";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textStroopTimeOut
            // 
            this.textStroopTimeOut.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textStroopTimeOut.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textStroopTimeOut.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textStroopTimeOut.Location = new System.Drawing.Point(168, 112);
            this.textStroopTimeOut.Name = "textStroopTimeOut";
            this.textStroopTimeOut.Size = new System.Drawing.Size(152, 26);
            this.textStroopTimeOut.TabIndex = 4;
            // 
            // label9
            // 
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(8, 144);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(136, 26);
            this.label9.TabIndex = 2;
            this.label9.Text = "Time Rest  (ms)";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textStroopTimeRest
            // 
            this.textStroopTimeRest.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textStroopTimeRest.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textStroopTimeRest.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textStroopTimeRest.Location = new System.Drawing.Point(168, 144);
            this.textStroopTimeRest.Name = "textStroopTimeRest";
            this.textStroopTimeRest.Size = new System.Drawing.Size(152, 26);
            this.textStroopTimeRest.TabIndex = 4;
            // 
            // buttonSaveStroopConfig
            // 
            this.buttonSaveStroopConfig.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonSaveStroopConfig.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonSaveStroopConfig.Location = new System.Drawing.Point(245, 223);
            this.buttonSaveStroopConfig.Name = "buttonSaveStroopConfig";
            this.buttonSaveStroopConfig.Size = new System.Drawing.Size(75, 32);
            this.buttonSaveStroopConfig.TabIndex = 0;
            this.buttonSaveStroopConfig.Text = "Save";
            this.buttonSaveStroopConfig.Click += new System.EventHandler(this.buttonSaveStroopConfig_Click);
            // 
            // label12
            // 
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(8, 48);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(136, 26);
            this.label12.TabIndex = 2;
            this.label12.Text = "Number of trials:";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textStroopTrials
            // 
            this.textStroopTrials.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textStroopTrials.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textStroopTrials.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textStroopTrials.Location = new System.Drawing.Point(168, 48);
            this.textStroopTrials.Name = "textStroopTrials";
            this.textStroopTrials.Size = new System.Drawing.Size(152, 26);
            this.textStroopTrials.TabIndex = 4;
            // 
            // label13
            // 
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(8, 176);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(136, 26);
            this.label13.TabIndex = 2;
            this.label13.Text = "Time Relax  (ms)";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textStroopTimeRelax
            // 
            this.textStroopTimeRelax.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textStroopTimeRelax.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textStroopTimeRelax.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textStroopTimeRelax.Location = new System.Drawing.Point(168, 176);
            this.textStroopTimeRelax.Name = "textStroopTimeRelax";
            this.textStroopTimeRelax.Size = new System.Drawing.Size(152, 26);
            this.textStroopTimeRelax.TabIndex = 4;
            // 
            // buttonDefaultStroop
            // 
            this.buttonDefaultStroop.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonDefaultStroop.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonDefaultStroop.Location = new System.Drawing.Point(149, 223);
            this.buttonDefaultStroop.Name = "buttonDefaultStroop";
            this.buttonDefaultStroop.Size = new System.Drawing.Size(75, 32);
            this.buttonDefaultStroop.TabIndex = 0;
            this.buttonDefaultStroop.Text = "Reset";
            this.buttonDefaultStroop.Click += new System.EventHandler(this.buttonSaveStroopConfig_Click);
            // 
            // labelDesignMode
            // 
            this.labelDesignMode.BackColor = System.Drawing.Color.White;
            this.labelDesignMode.Location = new System.Drawing.Point(453, 268);
            this.labelDesignMode.Name = "labelDesignMode";
            this.labelDesignMode.Size = new System.Drawing.Size(100, 23);
            this.labelDesignMode.TabIndex = 15;
            // 
            // imageListColors
            // 
            this.imageListColors.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageListColors.ImageSize = new System.Drawing.Size(16, 16);
            this.imageListColors.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // StroopTestCfg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelDesignMode);
            this.Controls.Add(this.buttonStartStroopTest);
            this.Controls.Add(this.buttonTryStroopTest);
            this.Controls.Add(this.groupBox1);
            this.Name = "StroopTestCfg";
            this.Size = new System.Drawing.Size(610, 346);
            this.Load += new System.EventHandler(this.StroopTestCfg_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonStartStroopTest;
        private System.Windows.Forms.Button buttonTryStroopTest;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkStroopRelaxImage;
        private System.Windows.Forms.TextBox textStroopSequence;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textStroopTimeCue;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textStroopTimeOut;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textStroopTimeRest;
        private System.Windows.Forms.Button buttonSaveStroopConfig;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox textStroopTrials;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox textStroopTimeRelax;
        private System.Windows.Forms.Button buttonDefaultStroop;
        private System.Windows.Forms.Label labelDesignMode;
        private System.Windows.Forms.ImageList imageListColors;
    }
}
