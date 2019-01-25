namespace BCILib.App
{
    partial class ConsoleOutForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConsoleOutForm));
            this.textBoxConsoleOut = new System.Windows.Forms.TextBox();
            this.buttonClear = new System.Windows.Forms.Button();
            this.buttonTest = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxConsoleOut
            // 
            this.textBoxConsoleOut.Location = new System.Drawing.Point(12, 41);
            this.textBoxConsoleOut.Multiline = true;
            this.textBoxConsoleOut.Name = "textBoxConsoleOut";
            this.textBoxConsoleOut.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxConsoleOut.Size = new System.Drawing.Size(623, 432);
            this.textBoxConsoleOut.TabIndex = 0;
            // 
            // buttonClear
            // 
            this.buttonClear.Location = new System.Drawing.Point(560, 12);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(75, 23);
            this.buttonClear.TabIndex = 1;
            this.buttonClear.Text = "&Clear";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // buttonTest
            // 
            this.buttonTest.Location = new System.Drawing.Point(472, 12);
            this.buttonTest.Name = "buttonTest";
            this.buttonTest.Size = new System.Drawing.Size(75, 23);
            this.buttonTest.TabIndex = 2;
            this.buttonTest.Text = "Test";
            this.buttonTest.UseVisualStyleBackColor = true;
            this.buttonTest.Click += new System.EventHandler(this.buttonTest_Click);
            // 
            // ConsoleOutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(654, 485);
            this.Controls.Add(this.buttonTest);
            this.Controls.Add(this.buttonClear);
            this.Controls.Add(this.textBoxConsoleOut);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConsoleOutForm";
            this.ShowInTaskbar = false;
            this.Text = "ConsoleOutForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ConsoleOutForm_FormClosed);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConsoleOutForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxConsoleOut;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.Button buttonTest;
    }
}