namespace BCIWork
{
    partial class DataUploadAgentForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataUploadAgentForm));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonSearch = new System.Windows.Forms.ToolStripButton();
            this.panelUploading = new System.Windows.Forms.Panel();
            this.labelSpeed = new System.Windows.Forms.Label();
            this.labelUpdWorkers = new System.Windows.Forms.Label();
            this.labelUplStatus = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.listViewUplProgress = new System.Windows.Forms.ListView();
            this.columnHeaderFileName = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderSize = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderIsWrting = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderPercentage = new System.Windows.Forms.ColumnHeader();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.toolStrip1.SuspendLayout();
            this.panelUploading.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonSearch});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(586, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonSearch
            // 
            this.toolStripButtonSearch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonSearch.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSearch.Image")));
            this.toolStripButtonSearch.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSearch.Name = "toolStripButtonSearch";
            this.toolStripButtonSearch.Size = new System.Drawing.Size(46, 22);
            this.toolStripButtonSearch.Text = "Search";
            this.toolStripButtonSearch.ToolTipText = "Search files for uploading";
            this.toolStripButtonSearch.Click += new System.EventHandler(this.toolStripButtonSearch_Click);
            // 
            // panelUploading
            // 
            this.panelUploading.Controls.Add(this.labelSpeed);
            this.panelUploading.Controls.Add(this.labelUpdWorkers);
            this.panelUploading.Controls.Add(this.labelUplStatus);
            this.panelUploading.Controls.Add(this.label18);
            this.panelUploading.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelUploading.Location = new System.Drawing.Point(0, 348);
            this.panelUploading.Name = "panelUploading";
            this.panelUploading.Size = new System.Drawing.Size(586, 17);
            this.panelUploading.TabIndex = 4;
            // 
            // labelSpeed
            // 
            this.labelSpeed.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelSpeed.BackColor = System.Drawing.Color.LightGray;
            this.labelSpeed.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelSpeed.Location = new System.Drawing.Point(108, 2);
            this.labelSpeed.Name = "labelSpeed";
            this.labelSpeed.Size = new System.Drawing.Size(429, 13);
            this.labelSpeed.TabIndex = 3;
            // 
            // labelUpdWorkers
            // 
            this.labelUpdWorkers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelUpdWorkers.BackColor = System.Drawing.Color.LightGray;
            this.labelUpdWorkers.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelUpdWorkers.Location = new System.Drawing.Point(543, 2);
            this.labelUpdWorkers.Name = "labelUpdWorkers";
            this.labelUpdWorkers.Size = new System.Drawing.Size(39, 13);
            this.labelUpdWorkers.TabIndex = 1;
            // 
            // labelUplStatus
            // 
            this.labelUplStatus.BackColor = System.Drawing.Color.LightGray;
            this.labelUplStatus.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelUplStatus.Location = new System.Drawing.Point(63, 2);
            this.labelUplStatus.Name = "labelUplStatus";
            this.labelUplStatus.Size = new System.Drawing.Size(39, 13);
            this.labelUplStatus.TabIndex = 1;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label18.Location = new System.Drawing.Point(2, 2);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(55, 13);
            this.label18.TabIndex = 2;
            this.label18.Text = "Total files:";
            // 
            // listViewUplProgress
            // 
            this.listViewUplProgress.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderFileName,
            this.columnHeaderSize,
            this.columnHeaderIsWrting,
            this.columnHeaderPercentage});
            this.listViewUplProgress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewUplProgress.Location = new System.Drawing.Point(0, 25);
            this.listViewUplProgress.Name = "listViewUplProgress";
            this.listViewUplProgress.Size = new System.Drawing.Size(586, 323);
            this.listViewUplProgress.TabIndex = 5;
            this.listViewUplProgress.UseCompatibleStateImageBehavior = false;
            this.listViewUplProgress.View = System.Windows.Forms.View.Details;
            // 
            // columnHeaderFileName
            // 
            this.columnHeaderFileName.Text = "File name";
            this.columnHeaderFileName.Width = 100;
            // 
            // columnHeaderSize
            // 
            this.columnHeaderSize.Text = "Size";
            this.columnHeaderSize.Width = 75;
            // 
            // columnHeaderIsWrting
            // 
            this.columnHeaderIsWrting.Text = "Writing";
            this.columnHeaderIsWrting.Width = 61;
            // 
            // columnHeaderPercentage
            // 
            this.columnHeaderPercentage.Text = "Percentage";
            this.columnHeaderPercentage.Width = 80;
            // 
            // notifyIcon
            // 
            this.notifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "notifyIcon1";
            this.notifyIcon.Visible = true;
            this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseDoubleClick);
            // 
            // DataUploadAgentForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(586, 365);
            this.Controls.Add(this.listViewUplProgress);
            this.Controls.Add(this.panelUploading);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DataUploadAgentForm";
            this.ShowInTaskbar = false;
            this.Text = "Data Upload Form";
            this.Load += new System.EventHandler(this.DataUploadAgentForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DataUploadAgentForm_FormClosing);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panelUploading.ResumeLayout(false);
            this.panelUploading.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.Panel panelUploading;
        private System.Windows.Forms.Label labelSpeed;
        private System.Windows.Forms.Label labelUpdWorkers;
        private System.Windows.Forms.Label labelUplStatus;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.ToolStripButton toolStripButtonSearch;
        private System.Windows.Forms.ListView listViewUplProgress;
        private System.Windows.Forms.ColumnHeader columnHeaderFileName;
        private System.Windows.Forms.ColumnHeader columnHeaderSize;
        private System.Windows.Forms.ColumnHeader columnHeaderIsWrting;
        private System.Windows.Forms.ColumnHeader columnHeaderPercentage;
        private System.Windows.Forms.NotifyIcon notifyIcon;
    }
}

