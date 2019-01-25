namespace BCILibCS.Util
{
    partial class SerialPortCtrl
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
            CloseOpenPorts();

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
            this.label1 = new System.Windows.Forms.Label();
            this.checkBoxDTR = new System.Windows.Forms.CheckBox();
            this.checkBoxRTS = new System.Windows.Forms.CheckBox();
            this.buttonToggleDTR = new System.Windows.Forms.Button();
            this.buttonToggleRTS = new System.Windows.Forms.Button();
            this.listView = new System.Windows.Forms.ListView();
            this.colPortNum = new System.Windows.Forms.ColumnHeader();
            this.colDesc = new System.Windows.Forms.ColumnHeader();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(32, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Select Port:";
            // 
            // checkBoxDTR
            // 
            this.checkBoxDTR.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.checkBoxDTR.AutoCheck = false;
            this.checkBoxDTR.AutoSize = true;
            this.checkBoxDTR.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxDTR.Location = new System.Drawing.Point(156, 255);
            this.checkBoxDTR.Name = "checkBoxDTR";
            this.checkBoxDTR.Size = new System.Drawing.Size(49, 17);
            this.checkBoxDTR.TabIndex = 4;
            this.checkBoxDTR.Text = "DTR";
            this.checkBoxDTR.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBoxDTR.UseVisualStyleBackColor = true;
            // 
            // checkBoxRTS
            // 
            this.checkBoxRTS.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.checkBoxRTS.AutoCheck = false;
            this.checkBoxRTS.AutoSize = true;
            this.checkBoxRTS.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxRTS.Location = new System.Drawing.Point(157, 278);
            this.checkBoxRTS.Name = "checkBoxRTS";
            this.checkBoxRTS.Size = new System.Drawing.Size(48, 17);
            this.checkBoxRTS.TabIndex = 4;
            this.checkBoxRTS.Text = "RTS";
            this.checkBoxRTS.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBoxRTS.UseVisualStyleBackColor = true;
            // 
            // buttonToggleDTR
            // 
            this.buttonToggleDTR.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonToggleDTR.Location = new System.Drawing.Point(229, 251);
            this.buttonToggleDTR.Name = "buttonToggleDTR";
            this.buttonToggleDTR.Size = new System.Drawing.Size(75, 23);
            this.buttonToggleDTR.TabIndex = 5;
            this.buttonToggleDTR.Text = "Toggle";
            this.buttonToggleDTR.UseVisualStyleBackColor = true;
            this.buttonToggleDTR.Click += new System.EventHandler(this.buttonEnableDTR_Click);
            // 
            // buttonToggleRTS
            // 
            this.buttonToggleRTS.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonToggleRTS.Location = new System.Drawing.Point(229, 274);
            this.buttonToggleRTS.Name = "buttonToggleRTS";
            this.buttonToggleRTS.Size = new System.Drawing.Size(75, 23);
            this.buttonToggleRTS.TabIndex = 5;
            this.buttonToggleRTS.Text = "Toggle";
            this.buttonToggleRTS.UseVisualStyleBackColor = true;
            this.buttonToggleRTS.Click += new System.EventHandler(this.buttonToggleRTS_Click);
            // 
            // listView
            // 
            this.listView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listView.CheckBoxes = true;
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colPortNum,
            this.colDesc});
            this.listView.Location = new System.Drawing.Point(35, 48);
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(433, 184);
            this.listView.TabIndex = 6;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            // 
            // colPortNum
            // 
            this.colPortNum.Text = "Com Port";
            this.colPortNum.Width = 101;
            // 
            // colDesc
            // 
            this.colDesc.Text = "Description";
            this.colDesc.Width = 322;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnRefresh.Location = new System.Drawing.Point(246, 16);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 5;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.buttonEnableDTR_Click);
            // 
            // SerialPortCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listView);
            this.Controls.Add(this.buttonToggleRTS);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.buttonToggleDTR);
            this.Controls.Add(this.checkBoxRTS);
            this.Controls.Add(this.checkBoxDTR);
            this.Controls.Add(this.label1);
            this.Name = "SerialPortCtrl";
            this.Size = new System.Drawing.Size(492, 327);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBoxDTR;
        private System.Windows.Forms.CheckBox checkBoxRTS;
        private System.Windows.Forms.Button buttonToggleDTR;
        private System.Windows.Forms.Button buttonToggleRTS;
        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.ColumnHeader colPortNum;
        private System.Windows.Forms.ColumnHeader colDesc;
        private System.Windows.Forms.Button btnRefresh;
    }
}
