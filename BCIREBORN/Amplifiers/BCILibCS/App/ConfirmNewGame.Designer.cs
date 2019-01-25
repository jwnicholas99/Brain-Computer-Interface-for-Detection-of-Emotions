namespace BCILib.App
{
    partial class ConfirmNewGame
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
            this.textGameName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonOkey = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textGameName
            // 
            this.textGameName.Location = new System.Drawing.Point(158, 29);
            this.textGameName.Name = "textGameName";
            this.textGameName.Size = new System.Drawing.Size(166, 20);
            this.textGameName.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(43, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Game Name:";
            // 
            // buttonOkey
            // 
            this.buttonOkey.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOkey.Location = new System.Drawing.Point(46, 99);
            this.buttonOkey.Name = "buttonOkey";
            this.buttonOkey.Size = new System.Drawing.Size(75, 23);
            this.buttonOkey.TabIndex = 2;
            this.buttonOkey.Text = "OK";
            this.buttonOkey.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(249, 99);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // ConfirmNewGame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(357, 172);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOkey);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textGameName);
            this.Name = "ConfirmNewGame";
            this.Text = "ConfirmNewGame";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textGameName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonOkey;
        private System.Windows.Forms.Button buttonCancel;
    }
}