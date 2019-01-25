namespace BCILib.Amp
{
    partial class FakeAmplifier_Config
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
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label3;
            this.numericUpDownNumChannel = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownSplRate = new System.Windows.Forms.NumericUpDown();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.textBoxChannelString = new System.Windows.Forms.TextBox();
            this.buttonChangeChannel = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownNumChannel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSplRate)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(67, 30);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(106, 13);
            label1.TabIndex = 0;
            label1.Text = "Number of channers:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(67, 67);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(79, 13);
            label2.TabIndex = 0;
            label2.Text = "Sampling Rate:";
            // 
            // numericUpDownNumChannel
            // 
            this.numericUpDownNumChannel.Location = new System.Drawing.Point(219, 28);
            this.numericUpDownNumChannel.Name = "numericUpDownNumChannel";
            this.numericUpDownNumChannel.Size = new System.Drawing.Size(120, 20);
            this.numericUpDownNumChannel.TabIndex = 1;
            this.numericUpDownNumChannel.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // numericUpDownSplRate
            // 
            this.numericUpDownSplRate.Location = new System.Drawing.Point(219, 65);
            this.numericUpDownSplRate.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDownSplRate.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownSplRate.Name = "numericUpDownSplRate";
            this.numericUpDownSplRate.Size = new System.Drawing.Size(120, 20);
            this.numericUpDownSplRate.TabIndex = 1;
            this.numericUpDownSplRate.Value = new decimal(new int[] {
            250,
            0,
            0,
            0});
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(90, 262);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 2;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(246, 262);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(67, 113);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(54, 13);
            label3.TabIndex = 0;
            label3.Text = "Channels:";
            // 
            // textBoxChannelString
            // 
            this.textBoxChannelString.Location = new System.Drawing.Point(127, 110);
            this.textBoxChannelString.Name = "textBoxChannelString";
            this.textBoxChannelString.ReadOnly = true;
            this.textBoxChannelString.Size = new System.Drawing.Size(299, 20);
            this.textBoxChannelString.TabIndex = 3;
            // 
            // buttonChangeChannel
            // 
            this.buttonChangeChannel.Location = new System.Drawing.Point(127, 150);
            this.buttonChangeChannel.Name = "buttonChangeChannel";
            this.buttonChangeChannel.Size = new System.Drawing.Size(103, 23);
            this.buttonChangeChannel.TabIndex = 4;
            this.buttonChangeChannel.Text = "Change...";
            this.buttonChangeChannel.UseVisualStyleBackColor = true;
            this.buttonChangeChannel.Click += new System.EventHandler(this.buttonChangeChannel_Click);
            // 
            // FakeAmplifier_Config
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(438, 371);
            this.Controls.Add(this.buttonChangeChannel);
            this.Controls.Add(this.textBoxChannelString);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.numericUpDownSplRate);
            this.Controls.Add(this.numericUpDownNumChannel);
            this.Controls.Add(label3);
            this.Controls.Add(label2);
            this.Controls.Add(label1);
            this.Name = "FakeAmplifier_Config";
            this.ShowIcon = false;
            this.Text = "FakeAmplifier Configuration";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownNumChannel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSplRate)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown numericUpDownNumChannel;
        private System.Windows.Forms.NumericUpDown numericUpDownSplRate;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.TextBox textBoxChannelString;
        private System.Windows.Forms.Button buttonChangeChannel;
    }
}