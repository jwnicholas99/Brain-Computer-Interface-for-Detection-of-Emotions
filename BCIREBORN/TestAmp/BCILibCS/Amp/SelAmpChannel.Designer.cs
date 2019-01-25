namespace BCILib.Amp
{
    partial class SelAmpChannel
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.comboAmplifier = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxNumSel = new System.Windows.Forms.TextBox();
            this.buttonSelAll = new System.Windows.Forms.Button();
            this.buttonClear = new System.Windows.Forms.Button();
            this.buttonAmpRefresh = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.checkedListBox1.CheckOnClick = true;
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Location = new System.Drawing.Point(3, 47);
            this.checkedListBox1.MultiColumn = true;
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(187, 184);
            this.checkedListBox1.TabIndex = 29;
            this.checkedListBox1.SelectedIndexChanged += new System.EventHandler(this.checkedListBox1_SelectedIndexChanged);
            // 
            // comboAmplifier
            // 
            this.comboAmplifier.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboAmplifier.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboAmplifier.FormattingEnabled = true;
            this.comboAmplifier.Location = new System.Drawing.Point(55, 1);
            this.comboAmplifier.Name = "comboAmplifier";
            this.comboAmplifier.Size = new System.Drawing.Size(103, 21);
            this.comboAmplifier.TabIndex = 28;
            this.comboAmplifier.SelectedIndexChanged += new System.EventHandler(this.comboAmplifier_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(0, 4);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(49, 13);
            this.label7.TabIndex = 24;
            this.label7.Text = "Amplifier:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(-3, 28);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(96, 13);
            this.label6.TabIndex = 25;
            this.label6.Text = "Selected Channels";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxNumSel
            // 
            this.textBoxNumSel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxNumSel.Location = new System.Drawing.Point(111, 25);
            this.textBoxNumSel.Name = "textBoxNumSel";
            this.textBoxNumSel.ReadOnly = true;
            this.textBoxNumSel.Size = new System.Drawing.Size(79, 20);
            this.textBoxNumSel.TabIndex = 26;
            this.textBoxNumSel.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // buttonSelAll
            // 
            this.buttonSelAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonSelAll.Location = new System.Drawing.Point(3, 235);
            this.buttonSelAll.Name = "buttonSelAll";
            this.buttonSelAll.Size = new System.Drawing.Size(75, 23);
            this.buttonSelAll.TabIndex = 30;
            this.buttonSelAll.Text = "All";
            this.buttonSelAll.UseVisualStyleBackColor = true;
            this.buttonSelAll.Click += new System.EventHandler(this.buttonSelAll_Click);
            // 
            // buttonClear
            // 
            this.buttonClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonClear.Location = new System.Drawing.Point(115, 234);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(75, 23);
            this.buttonClear.TabIndex = 30;
            this.buttonClear.Text = "Clear";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // buttonAmpRefresh
            // 
            this.buttonAmpRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAmpRefresh.Location = new System.Drawing.Point(164, 0);
            this.buttonAmpRefresh.Name = "buttonAmpRefresh";
            this.buttonAmpRefresh.Size = new System.Drawing.Size(26, 23);
            this.buttonAmpRefresh.TabIndex = 31;
            this.buttonAmpRefresh.Text = "R";
            this.buttonAmpRefresh.UseVisualStyleBackColor = true;
            this.buttonAmpRefresh.Click += new System.EventHandler(this.buttonAmpRefresh_Click);
            // 
            // SelAmpChannel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonAmpRefresh);
            this.Controls.Add(this.buttonClear);
            this.Controls.Add(this.buttonSelAll);
            this.Controls.Add(this.checkedListBox1);
            this.Controls.Add(this.comboAmplifier);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBoxNumSel);
            this.Name = "SelAmpChannel";
            this.Size = new System.Drawing.Size(195, 257);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox checkedListBox1;
        private System.Windows.Forms.ComboBox comboAmplifier;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxNumSel;
        private System.Windows.Forms.Button buttonSelAll;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.Button buttonAmpRefresh;

    }
}
