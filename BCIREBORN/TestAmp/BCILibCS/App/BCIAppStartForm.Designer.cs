namespace BCILib.App
{
    partial class BCIAppStartForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BCIAppStartForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboSubject = new System.Windows.Forms.ComboBox();
            this.buttonEdit = new System.Windows.Forms.Button();
            this.buttonCrtSubject = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonStart = new System.Windows.Forms.Button();
            this.labelApplication = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.buttonFindApp = new System.Windows.Forms.Button();
            this.buttonCfg = new System.Windows.Forms.Button();
            this.panelUpload = new System.Windows.Forms.Panel();
            this.buttonUpload = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.panelUpload.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboSubject);
            this.groupBox1.Controls.Add(this.buttonEdit);
            this.groupBox1.Controls.Add(this.buttonCrtSubject);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(46, 92);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(384, 85);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Subject";
            // 
            // comboSubject
            // 
            this.comboSubject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboSubject.Location = new System.Drawing.Point(112, 41);
            this.comboSubject.Name = "comboSubject";
            this.comboSubject.Size = new System.Drawing.Size(128, 21);
            this.comboSubject.TabIndex = 1;
            this.comboSubject.SelectedIndexChanged += new System.EventHandler(this.comboSubject_SelectedIndexChanged);
            // 
            // buttonEdit
            // 
            this.buttonEdit.Enabled = false;
            this.buttonEdit.Location = new System.Drawing.Point(320, 41);
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.Size = new System.Drawing.Size(48, 23);
            this.buttonEdit.TabIndex = 3;
            this.buttonEdit.Text = "Edit";
            this.buttonEdit.Click += new System.EventHandler(this.buttonEdit_Click);
            // 
            // buttonCrtSubject
            // 
            this.buttonCrtSubject.Location = new System.Drawing.Point(256, 41);
            this.buttonCrtSubject.Name = "buttonCrtSubject";
            this.buttonCrtSubject.Size = new System.Drawing.Size(48, 23);
            this.buttonCrtSubject.TabIndex = 3;
            this.buttonCrtSubject.Text = "Create";
            this.buttonCrtSubject.Click += new System.EventHandler(this.buttonCrtSubject_Click);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 23);
            this.label2.TabIndex = 0;
            this.label2.Text = "User Account";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // buttonStart
            // 
            this.buttonStart.Enabled = false;
            this.buttonStart.Location = new System.Drawing.Point(189, 256);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(75, 23);
            this.buttonStart.TabIndex = 6;
            this.buttonStart.Text = "Start";
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // labelApplication
            // 
            this.labelApplication.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelApplication.Location = new System.Drawing.Point(41, 29);
            this.labelApplication.Name = "labelApplication";
            this.labelApplication.Size = new System.Drawing.Size(286, 23);
            this.labelApplication.TabIndex = 8;
            this.labelApplication.Text = "Application";
            this.labelApplication.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonFindApp
            // 
            this.buttonFindApp.Location = new System.Drawing.Point(333, 29);
            this.buttonFindApp.Name = "buttonFindApp";
            this.buttonFindApp.Size = new System.Drawing.Size(45, 23);
            this.buttonFindApp.TabIndex = 9;
            this.buttonFindApp.Text = "Set...";
            this.buttonFindApp.UseVisualStyleBackColor = true;
            this.buttonFindApp.Click += new System.EventHandler(this.buttonFindApp_Click);
            // 
            // buttonCfg
            // 
            this.buttonCfg.Location = new System.Drawing.Point(385, 29);
            this.buttonCfg.Name = "buttonCfg";
            this.buttonCfg.Size = new System.Drawing.Size(45, 23);
            this.buttonCfg.TabIndex = 9;
            this.buttonCfg.Text = "Cfg...";
            this.buttonCfg.UseVisualStyleBackColor = true;
            this.buttonCfg.Click += new System.EventHandler(this.buttonCfg_Click);
            // 
            // panelUpload
            // 
            this.panelUpload.Controls.Add(this.buttonUpload);
            this.panelUpload.Controls.Add(this.label1);
            this.panelUpload.Location = new System.Drawing.Point(0, 183);
            this.panelUpload.Name = "panelUpload";
            this.panelUpload.Size = new System.Drawing.Size(487, 55);
            this.panelUpload.TabIndex = 10;
            this.panelUpload.Visible = false;
            // 
            // buttonUpload
            // 
            this.buttonUpload.Location = new System.Drawing.Point(402, 13);
            this.buttonUpload.Name = "buttonUpload";
            this.buttonUpload.Size = new System.Drawing.Size(75, 23);
            this.buttonUpload.TabIndex = 1;
            this.buttonUpload.Text = "Upload";
            this.buttonUpload.UseVisualStyleBackColor = true;
            this.buttonUpload.Click += new System.EventHandler(this.buttonUpload_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(367, 26);
            this.label1.TabIndex = 0;
            this.label1.Text = "There are data files to upload. \r\nIf PC is connected to Internet, you can click \"" +
                "Upload\" to send files to server.";
            // 
            // BCIAppStartForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(489, 316);
            this.Controls.Add(this.panelUpload);
            this.Controls.Add(this.buttonCfg);
            this.Controls.Add(this.buttonFindApp);
            this.Controls.Add(this.labelApplication);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonStart);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BCIAppStartForm";
            this.Text = "BCI Application: User Logon";
            this.groupBox1.ResumeLayout(false);
            this.panelUpload.ResumeLayout(false);
            this.panelUpload.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox comboSubject;
        private System.Windows.Forms.Button buttonEdit;
        private System.Windows.Forms.Button buttonCrtSubject;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Label labelApplication;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button buttonFindApp;
        private System.Windows.Forms.Button buttonCfg;
        private System.Windows.Forms.Panel panelUpload;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonUpload;
    }
}