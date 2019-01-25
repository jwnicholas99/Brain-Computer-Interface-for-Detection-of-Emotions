namespace BCILib.P300
{
    partial class P300ConfigCtrl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(P300ConfigCtrl));
            this.buttonSave = new System.Windows.Forms.Button();
            this.textBoxTrainingSample = new System.Windows.Forms.TextBox();
            this.textBoxSpellChars = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxNumCodes = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripStartTrain = new System.Windows.Forms.ToolStripButton();
            this.toolStripStartTest = new System.Windows.Forms.ToolStripButton();
            this.toolStripStop = new System.Windows.Forms.ToolStripButton();
            this.buttonStartSpeller = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.externalBCIApp = new BCILib.App.ExternalBCIApp();
            this.labelP300Speller = new System.Windows.Forms.Label();
            groupBox1 = new System.Windows.Forms.GroupBox();
            groupBox1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(this.buttonSave);
            groupBox1.Controls.Add(this.textBoxTrainingSample);
            groupBox1.Controls.Add(this.textBoxSpellChars);
            groupBox1.Controls.Add(this.label3);
            groupBox1.Controls.Add(this.label2);
            groupBox1.Controls.Add(this.textBoxNumCodes);
            groupBox1.Controls.Add(this.label1);
            groupBox1.Location = new System.Drawing.Point(21, 40);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(346, 223);
            groupBox1.TabIndex = 3;
            groupBox1.TabStop = false;
            groupBox1.Text = "P300 configuration";
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(120, 194);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 23);
            this.buttonSave.TabIndex = 2;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // textBoxTrainingSample
            // 
            this.textBoxTrainingSample.Location = new System.Drawing.Point(110, 70);
            this.textBoxTrainingSample.Name = "textBoxTrainingSample";
            this.textBoxTrainingSample.Size = new System.Drawing.Size(221, 20);
            this.textBoxTrainingSample.TabIndex = 1;
            this.textBoxTrainingSample.TextChanged += new System.EventHandler(this.SplChanged);
            // 
            // textBoxSpellChars
            // 
            this.textBoxSpellChars.Location = new System.Drawing.Point(110, 46);
            this.textBoxSpellChars.Name = "textBoxSpellChars";
            this.textBoxSpellChars.Size = new System.Drawing.Size(221, 20);
            this.textBoxSpellChars.TabIndex = 1;
            this.textBoxSpellChars.TextChanged += new System.EventHandler(this.CfgChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Training Samples";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Button Labels:";
            // 
            // textBoxNumCodes
            // 
            this.textBoxNumCodes.Location = new System.Drawing.Point(110, 20);
            this.textBoxNumCodes.Name = "textBoxNumCodes";
            this.textBoxNumCodes.Size = new System.Drawing.Size(97, 20);
            this.textBoxNumCodes.TabIndex = 1;
            this.textBoxNumCodes.TextChanged += new System.EventHandler(this.CfgChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Number of Buttons:";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStartTrain,
            this.toolStripStartTest,
            this.toolStripStop});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(748, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripStartTrain
            // 
            this.toolStripStartTrain.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripStartTrain.Image = ((System.Drawing.Image)(resources.GetObject("toolStripStartTrain.Image")));
            this.toolStripStartTrain.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripStartTrain.Name = "toolStripStartTrain";
            this.toolStripStartTrain.Size = new System.Drawing.Size(38, 22);
            this.toolStripStartTrain.Text = "Train";
            this.toolStripStartTrain.Click += new System.EventHandler(this.toolStripStartTrain_Click);
            // 
            // toolStripStartTest
            // 
            this.toolStripStartTest.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripStartTest.Image = ((System.Drawing.Image)(resources.GetObject("toolStripStartTest.Image")));
            this.toolStripStartTest.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripStartTest.Name = "toolStripStartTest";
            this.toolStripStartTest.Size = new System.Drawing.Size(33, 22);
            this.toolStripStartTest.Text = "Test";
            this.toolStripStartTest.Click += new System.EventHandler(this.toolStripStartTest_Click);
            // 
            // toolStripStop
            // 
            this.toolStripStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripStop.Enabled = false;
            this.toolStripStop.Image = ((System.Drawing.Image)(resources.GetObject("toolStripStop.Image")));
            this.toolStripStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripStop.Name = "toolStripStop";
            this.toolStripStop.Size = new System.Drawing.Size(35, 22);
            this.toolStripStop.Text = "Stop";
            this.toolStripStop.Click += new System.EventHandler(this.toolStripStop_Click);
            // 
            // buttonStartSpeller
            // 
            this.buttonStartSpeller.Location = new System.Drawing.Point(198, 39);
            this.buttonStartSpeller.Name = "buttonStartSpeller";
            this.buttonStartSpeller.Size = new System.Drawing.Size(58, 23);
            this.buttonStartSpeller.TabIndex = 4;
            this.buttonStartSpeller.Text = "Start";
            this.buttonStartSpeller.UseVisualStyleBackColor = true;
            this.buttonStartSpeller.Click += new System.EventHandler(this.buttonStartSpeller_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.externalBCIApp);
            this.groupBox2.Controls.Add(this.labelP300Speller);
            this.groupBox2.Controls.Add(this.buttonStartSpeller);
            this.groupBox2.Location = new System.Drawing.Point(390, 40);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(334, 223);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "P300 Application";
            // 
            // externalBCIApp
            // 
            this.externalBCIApp.Location = new System.Drawing.Point(20, 115);
            this.externalBCIApp.Name = "externalBCIApp";
            this.externalBCIApp.Size = new System.Drawing.Size(242, 54);
            this.externalBCIApp.TabIndex = 6;
            // 
            // labelP300Speller
            // 
            this.labelP300Speller.AutoSize = true;
            this.labelP300Speller.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelP300Speller.Location = new System.Drawing.Point(27, 44);
            this.labelP300Speller.Name = "labelP300Speller";
            this.labelP300Speller.Size = new System.Drawing.Size(0, 20);
            this.labelP300Speller.TabIndex = 5;
            // 
            // P300ConfigCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(groupBox1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "P300ConfigCtrl";
            this.Size = new System.Drawing.Size(748, 320);
            this.Tag = "m";
            this.Load += new System.EventHandler(this.P300ConfigCtrl_Load);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxNumCodes;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripStartTrain;
        private System.Windows.Forms.ToolStripButton toolStripStartTest;
        private System.Windows.Forms.ToolStripButton toolStripStop;
        private System.Windows.Forms.TextBox textBoxSpellChars;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxTrainingSample;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonStartSpeller;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label labelP300Speller;
        private BCILib.App.ExternalBCIApp externalBCIApp;
    }
}
