namespace BCILib.Amp
{
    partial class SelectAmplifierForm
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
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.buttonOkey = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.checkBoxMultiSelection = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxAmpList = new System.Windows.Forms.ComboBox();
            this.buttonCreateAmp = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.checkedListBox1.CheckOnClick = true;
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Location = new System.Drawing.Point(38, 32);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(352, 274);
            this.checkedListBox1.TabIndex = 0;
            this.checkedListBox1.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBox1_ItemCheck);
            // 
            // buttonOkey
            // 
            this.buttonOkey.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonOkey.Location = new System.Drawing.Point(76, 357);
            this.buttonOkey.Name = "buttonOkey";
            this.buttonOkey.Size = new System.Drawing.Size(75, 23);
            this.buttonOkey.TabIndex = 1;
            this.buttonOkey.Text = "Ok";
            this.buttonOkey.UseVisualStyleBackColor = true;
            this.buttonOkey.Click += new System.EventHandler(this.buttonOkey_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(278, 357);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // checkBoxMultiSelection
            // 
            this.checkBoxMultiSelection.AutoSize = true;
            this.checkBoxMultiSelection.Location = new System.Drawing.Point(42, 9);
            this.checkBoxMultiSelection.Name = "checkBoxMultiSelection";
            this.checkBoxMultiSelection.Size = new System.Drawing.Size(95, 17);
            this.checkBoxMultiSelection.TabIndex = 2;
            this.checkBoxMultiSelection.Text = "Multi Selection";
            this.checkBoxMultiSelection.UseVisualStyleBackColor = true;
            this.checkBoxMultiSelection.CheckedChanged += new System.EventHandler(this.checkBoxMultiSelection_CheckedChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(35, 328);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Select New Amplifier:";
            // 
            // comboBoxAmpList
            // 
            this.comboBoxAmpList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.comboBoxAmpList.FormattingEnabled = true;
            this.comboBoxAmpList.Location = new System.Drawing.Point(145, 325);
            this.comboBoxAmpList.Name = "comboBoxAmpList";
            this.comboBoxAmpList.Size = new System.Drawing.Size(178, 21);
            this.comboBoxAmpList.TabIndex = 4;
            // 
            // buttonCreateAmp
            // 
            this.buttonCreateAmp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCreateAmp.Location = new System.Drawing.Point(329, 323);
            this.buttonCreateAmp.Name = "buttonCreateAmp";
            this.buttonCreateAmp.Size = new System.Drawing.Size(61, 23);
            this.buttonCreateAmp.TabIndex = 5;
            this.buttonCreateAmp.Text = "Create";
            this.buttonCreateAmp.UseVisualStyleBackColor = true;
            this.buttonCreateAmp.Click += new System.EventHandler(this.buttonCreateAmp_Click);
            // 
            // SelectAmplifierForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(428, 385);
            this.Controls.Add(this.buttonCreateAmp);
            this.Controls.Add(this.comboBoxAmpList);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.checkBoxMultiSelection);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOkey);
            this.Controls.Add(this.checkedListBox1);
            this.Name = "SelectAmplifierForm";
            this.Text = "Amplifier Selection";
            this.Load += new System.EventHandler(this.SelectAmplifierForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox checkedListBox1;
        private System.Windows.Forms.Button buttonOkey;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.CheckBox checkBoxMultiSelection;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxAmpList;
        private System.Windows.Forms.Button buttonCreateAmp;
    }
}