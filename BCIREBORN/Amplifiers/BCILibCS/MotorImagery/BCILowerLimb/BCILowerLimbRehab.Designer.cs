namespace BCILib.MotorImagery.BCILowerLimb
{
    partial class BCILowerLimbRehab
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
            this.labelAmplifier = new System.Windows.Forms.Label();
            this.gbAmplifier = new System.Windows.Forms.GroupBox();
            this.textSelChannels = new System.Windows.Forms.TextBox();
            this.buttonAmpSel = new System.Windows.Forms.Button();
            this.bgworker = new System.ComponentModel.BackgroundWorker();
            this.fld_accumulator = new BCILib.MotorImagery.FLDAccumulator();
            this.buttonTrainModel = new System.Windows.Forms.Button();
            this.btnShowTasks = new System.Windows.Forms.Button();
            this.rtbRecords = new System.Windows.Forms.RichTextBox();
            this.contextBCIRecords = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gbAmplifier.SuspendLayout();
            this.contextBCIRecords.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelAmplifier
            // 
            this.labelAmplifier.AutoSize = true;
            this.labelAmplifier.Location = new System.Drawing.Point(6, 16);
            this.labelAmplifier.Name = "labelAmplifier";
            this.labelAmplifier.Size = new System.Drawing.Size(138, 13);
            this.labelAmplifier.TabIndex = 4;
            this.labelAmplifier.Text = "Amplifier selected channels:";
            // 
            // gbAmplifier
            // 
            this.gbAmplifier.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbAmplifier.Controls.Add(this.textSelChannels);
            this.gbAmplifier.Controls.Add(this.labelAmplifier);
            this.gbAmplifier.Controls.Add(this.buttonAmpSel);
            this.gbAmplifier.Location = new System.Drawing.Point(376, 371);
            this.gbAmplifier.Name = "gbAmplifier";
            this.gbAmplifier.Size = new System.Drawing.Size(468, 36);
            this.gbAmplifier.TabIndex = 11;
            this.gbAmplifier.TabStop = false;
            // 
            // textSelChannels
            // 
            this.textSelChannels.Location = new System.Drawing.Point(161, 13);
            this.textSelChannels.Name = "textSelChannels";
            this.textSelChannels.ReadOnly = true;
            this.textSelChannels.Size = new System.Drawing.Size(81, 20);
            this.textSelChannels.TabIndex = 8;
            this.textSelChannels.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // buttonAmpSel
            // 
            this.buttonAmpSel.Location = new System.Drawing.Point(268, 10);
            this.buttonAmpSel.Name = "buttonAmpSel";
            this.buttonAmpSel.Size = new System.Drawing.Size(55, 23);
            this.buttonAmpSel.TabIndex = 6;
            this.buttonAmpSel.Text = "Select";
            this.buttonAmpSel.UseVisualStyleBackColor = true;
            this.buttonAmpSel.Click += new System.EventHandler(this.buttonAmpSel_Click);
            // 
            // bgworker
            // 
            this.bgworker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgworker_DoWork);
            this.bgworker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgworker_RunWorkerCompleted);
            // 
            // fld_accumulator
            // 
            this.fld_accumulator.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fld_accumulator.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.fld_accumulator.Location = new System.Drawing.Point(376, 28);
            this.fld_accumulator.Name = "fld_accumulator";
            this.fld_accumulator.Size = new System.Drawing.Size(468, 327);
            this.fld_accumulator.TabIndex = 15;
            // 
            // buttonTrainModel
            // 
            this.buttonTrainModel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonTrainModel.Location = new System.Drawing.Point(251, 374);
            this.buttonTrainModel.Name = "buttonTrainModel";
            this.buttonTrainModel.Size = new System.Drawing.Size(111, 35);
            this.buttonTrainModel.TabIndex = 23;
            this.buttonTrainModel.Text = "Train Model";
            this.buttonTrainModel.UseVisualStyleBackColor = true;
            this.buttonTrainModel.Click += new System.EventHandler(this.buttonTrainModel_Click);
            // 
            // btnShowTasks
            // 
            this.btnShowTasks.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnShowTasks.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnShowTasks.Location = new System.Drawing.Point(12, 374);
            this.btnShowTasks.Name = "btnShowTasks";
            this.btnShowTasks.Size = new System.Drawing.Size(111, 35);
            this.btnShowTasks.TabIndex = 24;
            this.btnShowTasks.Text = "Show Tasks";
            this.btnShowTasks.UseVisualStyleBackColor = true;
            this.btnShowTasks.Click += new System.EventHandler(this.btnShowTasks_Click);
            // 
            // rtbRecords
            // 
            this.rtbRecords.ContextMenuStrip = this.contextBCIRecords;
            this.rtbRecords.Location = new System.Drawing.Point(12, 28);
            this.rtbRecords.Name = "rtbRecords";
            this.rtbRecords.Size = new System.Drawing.Size(350, 327);
            this.rtbRecords.TabIndex = 25;
            this.rtbRecords.Text = "";
            // 
            // contextBCIRecords
            // 
            this.contextBCIRecords.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshToolStripMenuItem});
            this.contextBCIRecords.Name = "contextBCIRecords";
            this.contextBCIRecords.Size = new System.Drawing.Size(114, 26);
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
            this.refreshToolStripMenuItem.Text = "Refresh";
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // BCILowerLimbRehab
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(848, 440);
            this.Controls.Add(this.rtbRecords);
            this.Controls.Add(this.fld_accumulator);
            this.Controls.Add(this.btnShowTasks);
            this.Controls.Add(this.gbAmplifier);
            this.Controls.Add(this.buttonTrainModel);
            this.Name = "BCILowerLimbRehab";
            this.Text = "BCI Lowerlimb Rehabilitation";
            this.Controls.SetChildIndex(this.buttonTrainModel, 0);
            this.Controls.SetChildIndex(this.gbAmplifier, 0);
            this.Controls.SetChildIndex(this.btnShowTasks, 0);
            this.Controls.SetChildIndex(this.fld_accumulator, 0);
            this.Controls.SetChildIndex(this.rtbRecords, 0);
            this.gbAmplifier.ResumeLayout(false);
            this.gbAmplifier.PerformLayout();
            this.contextBCIRecords.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbAmplifier;
        private System.Windows.Forms.TextBox textSelChannels;
        private System.Windows.Forms.Button buttonAmpSel;
        private System.ComponentModel.BackgroundWorker bgworker;
        private FLDAccumulator fld_accumulator;
        private System.Windows.Forms.Button buttonTrainModel;
        private System.Windows.Forms.Button btnShowTasks;
        private System.Windows.Forms.RichTextBox rtbRecords;
        private System.Windows.Forms.Label labelAmplifier;
        private System.Windows.Forms.ContextMenuStrip contextBCIRecords;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
    }
}
