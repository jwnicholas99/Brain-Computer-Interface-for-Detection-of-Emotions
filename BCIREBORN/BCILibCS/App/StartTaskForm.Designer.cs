namespace BCILib.App
{
    partial class StartTaskForm
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
            this.lvSchedule = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonExit = new System.Windows.Forms.Button();
            this.buttonStart = new System.Windows.Forms.Button();
            this.contextScheduleItem = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolSetTaskPath = new System.Windows.Forms.ToolStripMenuItem();
            this.toolDelTask = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolSaveXML = new System.Windows.Forms.ToolStripMenuItem();
            this.contextScheduleItem.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvSchedule
            // 
            this.lvSchedule.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvSchedule.AutoArrange = false;
            this.lvSchedule.BackColor = System.Drawing.Color.SkyBlue;
            this.lvSchedule.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.lvSchedule.ContextMenuStrip = this.contextScheduleItem;
            this.lvSchedule.FullRowSelect = true;
            this.lvSchedule.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvSchedule.HideSelection = false;
            this.lvSchedule.Location = new System.Drawing.Point(12, 12);
            this.lvSchedule.MultiSelect = false;
            this.lvSchedule.Name = "lvSchedule";
            this.lvSchedule.ShowItemToolTips = true;
            this.lvSchedule.Size = new System.Drawing.Size(684, 348);
            this.lvSchedule.TabIndex = 12;
            this.lvSchedule.UseCompatibleStateImageBehavior = false;
            this.lvSchedule.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Session";
            this.columnHeader1.Width = 151;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Task";
            this.columnHeader2.Width = 112;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Desc";
            this.columnHeader3.Width = 373;
            // 
            // cmbType
            // 
            this.cmbType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Location = new System.Drawing.Point(713, 97);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(85, 21);
            this.cmbType.TabIndex = 14;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(719, 80);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Select Task";
            // 
            // buttonExit
            // 
            this.buttonExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonExit.Location = new System.Drawing.Point(716, 213);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(82, 35);
            this.buttonExit.TabIndex = 16;
            this.buttonExit.Text = "Cancel";
            this.buttonExit.UseVisualStyleBackColor = true;
            // 
            // buttonStart
            // 
            this.buttonStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonStart.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonStart.Location = new System.Drawing.Point(716, 154);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(82, 35);
            this.buttonStart.TabIndex = 15;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            // 
            // contextScheduleItem
            // 
            this.contextScheduleItem.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolSetTaskPath,
            this.toolDelTask,
            this.toolStripSeparator1,
            this.toolSaveXML});
            this.contextScheduleItem.Name = "contextScheduleItem";
            this.contextScheduleItem.Size = new System.Drawing.Size(213, 98);
            this.contextScheduleItem.Opening += new System.ComponentModel.CancelEventHandler(this.contextScheduleItem_Opening);
            // 
            // toolSetTaskPath
            // 
            this.toolSetTaskPath.Name = "toolSetTaskPath";
            this.toolSetTaskPath.Size = new System.Drawing.Size(212, 22);
            this.toolSetTaskPath.Text = "Set Data Path(finished task) ";
            this.toolSetTaskPath.Click += new System.EventHandler(this.toolSetTaskPath_Click);
            // 
            // toolDelTask
            // 
            this.toolDelTask.Name = "toolDelTask";
            this.toolDelTask.Size = new System.Drawing.Size(212, 22);
            this.toolDelTask.Text = "Delete task";
            this.toolDelTask.Click += new System.EventHandler(this.toolDelTask_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(209, 6);
            // 
            // toolSaveXML
            // 
            this.toolSaveXML.Name = "toolSaveXML";
            this.toolSaveXML.Size = new System.Drawing.Size(212, 22);
            this.toolSaveXML.Text = "Save XML Schedule ...";
            this.toolSaveXML.Click += new System.EventHandler(this.toolSaveXML_Click);
            // 
            // StartTaskForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(810, 372);
            this.Controls.Add(this.buttonExit);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.cmbType);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lvSchedule);
            this.Name = "StartTaskForm";
            this.Text = "StartTaskForm";
            this.contextScheduleItem.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvSchedule;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ComboBox cmbType;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ContextMenuStrip contextScheduleItem;
        private System.Windows.Forms.ToolStripMenuItem toolSetTaskPath;
        private System.Windows.Forms.ToolStripMenuItem toolDelTask;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem toolSaveXML;
    }
}