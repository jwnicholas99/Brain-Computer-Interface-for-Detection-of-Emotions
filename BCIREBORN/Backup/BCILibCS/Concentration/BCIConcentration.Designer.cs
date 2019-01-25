namespace BCILib.Concentration
{
    partial class BCIConcentration
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BCIConcentration));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageStroop = new System.Windows.Forms.TabPage();
            this.stroopTestCfg1 = new BCILib.Concentration.StroopTestCfg();
            this.tabPageModel = new System.Windows.Forms.TabPage();
            this.trainModelCfg1 = new BCILib.Concentration.ConcentrationTrainCfg();
            this.tabPageGames = new System.Windows.Forms.TabPage();
            this.gamesCfg1 = new BCILib.Concentration.ConcentrateGamesCfg();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.tabPageNUSDataTest = new System.Windows.Forms.TabPage();
            this.buttonBrowseNUSData = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxNUSDataPath = new System.Windows.Forms.TextBox();
            this.buttonStartNUSDataTest = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPageStroop.SuspendLayout();
            this.tabPageModel.SuspendLayout();
            this.tabPageGames.SuspendLayout();
            this.tabPageNUSDataTest.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPageStroop);
            this.tabControl1.Controls.Add(this.tabPageModel);
            this.tabControl1.Controls.Add(this.tabPageGames);
            this.tabControl1.Controls.Add(this.tabPageNUSDataTest);
            this.tabControl1.ImageList = this.imageList1;
            this.tabControl1.Location = new System.Drawing.Point(0, 28);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.ShowToolTips = true;
            this.tabControl1.Size = new System.Drawing.Size(787, 488);
            this.tabControl1.TabIndex = 2;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
            // 
            // tabPageStroop
            // 
            this.tabPageStroop.Controls.Add(this.stroopTestCfg1);
            this.tabPageStroop.ImageIndex = 0;
            this.tabPageStroop.Location = new System.Drawing.Point(4, 23);
            this.tabPageStroop.Name = "tabPageStroop";
            this.tabPageStroop.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageStroop.Size = new System.Drawing.Size(779, 461);
            this.tabPageStroop.TabIndex = 0;
            this.tabPageStroop.Text = "StroopTest";
            this.tabPageStroop.UseVisualStyleBackColor = true;
            // 
            // stroopTestCfg1
            // 
            this.stroopTestCfg1.Location = new System.Drawing.Point(84, -1);
            this.stroopTestCfg1.Name = "stroopTestCfg1";
            this.stroopTestCfg1.Size = new System.Drawing.Size(610, 462);
            this.stroopTestCfg1.TabIndex = 0;
            // 
            // tabPageModel
            // 
            this.tabPageModel.Controls.Add(this.trainModelCfg1);
            this.tabPageModel.ImageIndex = 0;
            this.tabPageModel.Location = new System.Drawing.Point(4, 23);
            this.tabPageModel.Name = "tabPageModel";
            this.tabPageModel.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageModel.Size = new System.Drawing.Size(779, 461);
            this.tabPageModel.TabIndex = 1;
            this.tabPageModel.Text = "Model Training";
            this.tabPageModel.UseVisualStyleBackColor = true;
            // 
            // trainModelCfg1
            // 
            this.trainModelCfg1.Location = new System.Drawing.Point(31, 17);
            this.trainModelCfg1.Name = "trainModelCfg1";
            this.trainModelCfg1.Size = new System.Drawing.Size(723, 438);
            this.trainModelCfg1.TabIndex = 0;
            // 
            // tabPageGames
            // 
            this.tabPageGames.Controls.Add(this.gamesCfg1);
            this.tabPageGames.ImageIndex = 2;
            this.tabPageGames.Location = new System.Drawing.Point(4, 23);
            this.tabPageGames.Name = "tabPageGames";
            this.tabPageGames.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageGames.Size = new System.Drawing.Size(779, 461);
            this.tabPageGames.TabIndex = 2;
            this.tabPageGames.Text = "Games";
            this.tabPageGames.UseVisualStyleBackColor = true;
            // 
            // gamesCfg1
            // 
            this.gamesCfg1.Location = new System.Drawing.Point(114, 19);
            this.gamesCfg1.Name = "gamesCfg1";
            this.gamesCfg1.Size = new System.Drawing.Size(558, 408);
            this.gamesCfg1.TabIndex = 0;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "");
            this.imageList1.Images.SetKeyName(1, "");
            this.imageList1.Images.SetKeyName(2, "");
            this.imageList1.Images.SetKeyName(3, "");
            // 
            // tabPageNUSDataTest
            // 
            this.tabPageNUSDataTest.Controls.Add(this.buttonStartNUSDataTest);
            this.tabPageNUSDataTest.Controls.Add(this.textBoxNUSDataPath);
            this.tabPageNUSDataTest.Controls.Add(this.label1);
            this.tabPageNUSDataTest.Controls.Add(this.buttonBrowseNUSData);
            this.tabPageNUSDataTest.Location = new System.Drawing.Point(4, 23);
            this.tabPageNUSDataTest.Name = "tabPageNUSDataTest";
            this.tabPageNUSDataTest.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageNUSDataTest.Size = new System.Drawing.Size(779, 461);
            this.tabPageNUSDataTest.TabIndex = 3;
            this.tabPageNUSDataTest.Text = "NUSDataTest";
            this.tabPageNUSDataTest.UseVisualStyleBackColor = true;
            // 
            // buttonBrowseNUSData
            // 
            this.buttonBrowseNUSData.Location = new System.Drawing.Point(531, 100);
            this.buttonBrowseNUSData.Name = "buttonBrowseNUSData";
            this.buttonBrowseNUSData.Size = new System.Drawing.Size(75, 23);
            this.buttonBrowseNUSData.TabIndex = 0;
            this.buttonBrowseNUSData.Text = "Browse";
            this.buttonBrowseNUSData.UseVisualStyleBackColor = true;
            this.buttonBrowseNUSData.Click += new System.EventHandler(this.buttonBrowseNUSData_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(65, 105);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Data Directory";
            // 
            // textBoxNUSDataPath
            // 
            this.textBoxNUSDataPath.Location = new System.Drawing.Point(165, 102);
            this.textBoxNUSDataPath.Name = "textBoxNUSDataPath";
            this.textBoxNUSDataPath.ReadOnly = true;
            this.textBoxNUSDataPath.Size = new System.Drawing.Size(360, 20);
            this.textBoxNUSDataPath.TabIndex = 2;
            // 
            // buttonStartNUSDataTest
            // 
            this.buttonStartNUSDataTest.Location = new System.Drawing.Point(286, 196);
            this.buttonStartNUSDataTest.Name = "buttonStartNUSDataTest";
            this.buttonStartNUSDataTest.Size = new System.Drawing.Size(75, 23);
            this.buttonStartNUSDataTest.TabIndex = 3;
            this.buttonStartNUSDataTest.Text = "Start";
            this.buttonStartNUSDataTest.UseVisualStyleBackColor = true;
            this.buttonStartNUSDataTest.Click += new System.EventHandler(this.buttonStartNUSDataTest_Click);
            // 
            // BCIConcentration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(787, 543);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BCIConcentration";
            this.Text = "BCIConcentrationForm";
            this.Load += new System.EventHandler(this.BCIConcentrationForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BCIConcentrationForm_FormClosing);
            this.Controls.SetChildIndex(this.tabControl1, 0);
            this.tabControl1.ResumeLayout(false);
            this.tabPageStroop.ResumeLayout(false);
            this.tabPageModel.ResumeLayout(false);
            this.tabPageGames.ResumeLayout(false);
            this.tabPageNUSDataTest.ResumeLayout(false);
            this.tabPageNUSDataTest.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageStroop;
        private System.Windows.Forms.TabPage tabPageModel;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.TabPage tabPageGames;
        private StroopTestCfg stroopTestCfg1;
        private ConcentrationTrainCfg trainModelCfg1;
        private ConcentrateGamesCfg gamesCfg1;
        private System.Windows.Forms.TabPage tabPageNUSDataTest;
        private System.Windows.Forms.TextBox textBoxNUSDataPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonBrowseNUSData;
        private System.Windows.Forms.Button buttonStartNUSDataTest;
    }
}