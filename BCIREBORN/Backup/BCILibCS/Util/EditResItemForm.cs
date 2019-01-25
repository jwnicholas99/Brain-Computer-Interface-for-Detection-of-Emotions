using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace BCILib.Util
{
	/// <summary>
	/// Summary description for NewResItemForm.
	/// </summary>
	public class EditResItemForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox comboRes;
		private System.Windows.Forms.ComboBox comboPar;
		/// <summary>
		/// Required designer variable.
		/// </summary>D:\WorkSpace\Nirs\NIRO200\D_BCILib_CSharp\Util\HRTimer.cs
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.TextBox textValue;
		private System.Windows.Forms.Button buttonApply;
		private System.Windows.Forms.Button buttonClose;

		private ResManager _res;

		public EditResItemForm(ResManager res)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			_res = res;

			string[] rl = _res.GetAllResource();
			foreach (string rn in rl) {
				comboRes.Items.Add(rn);
			}

			if (comboRes.Items.Count > 0) comboRes.SelectedIndex = 0;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textValue = new System.Windows.Forms.TextBox();
            this.buttonClose = new System.Windows.Forms.Button();
            this.comboRes = new System.Windows.Forms.ComboBox();
            this.comboPar = new System.Windows.Forms.ComboBox();
            this.buttonApply = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "Resource:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 23);
            this.label2.TabIndex = 0;
            this.label2.Text = "Paramter:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(8, 96);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 23);
            this.label3.TabIndex = 0;
            this.label3.Text = "Value:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textValue
            // 
            this.textValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textValue.Location = new System.Drawing.Point(80, 96);
            this.textValue.Name = "textValue";
            this.textValue.Size = new System.Drawing.Size(184, 20);
            this.textValue.TabIndex = 1;
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonClose.Location = new System.Drawing.Point(208, 152);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(56, 24);
            this.buttonClose.TabIndex = 2;
            this.buttonClose.Text = "Close";
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // comboRes
            // 
            this.comboRes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboRes.Location = new System.Drawing.Point(80, 16);
            this.comboRes.Name = "comboRes";
            this.comboRes.Size = new System.Drawing.Size(184, 21);
            this.comboRes.TabIndex = 3;
            this.comboRes.SelectedIndexChanged += new System.EventHandler(this.comboRes_SelectedIndexChanged);
            // 
            // comboPar
            // 
            this.comboPar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboPar.Location = new System.Drawing.Point(80, 56);
            this.comboPar.Name = "comboPar";
            this.comboPar.Size = new System.Drawing.Size(184, 21);
            this.comboPar.TabIndex = 3;
            this.comboPar.SelectedIndexChanged += new System.EventHandler(this.comboPar_SelectedIndexChanged);
            // 
            // buttonApply
            // 
            this.buttonApply.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonApply.Location = new System.Drawing.Point(16, 152);
            this.buttonApply.Name = "buttonApply";
            this.buttonApply.Size = new System.Drawing.Size(56, 23);
            this.buttonApply.TabIndex = 2;
            this.buttonApply.Text = "Apply";
            this.buttonApply.Click += new System.EventHandler(this.buttonApply_Click);
            // 
            // EditResItemForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(280, 182);
            this.Controls.Add(this.comboRes);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textValue);
            this.Controls.Add(this.comboPar);
            this.Controls.Add(this.buttonApply);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditResItemForm";
            this.Text = "Edit Resource Item";
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void comboRes_SelectedIndexChanged(object sender, System.EventArgs e) {
			string rn = comboRes.Text;
			string[] pl = _res.GetAllResProps(rn);
			comboPar.Items.Clear();
			comboPar.Text = null;
			textValue.Text = null;
			if (pl != null) {
				foreach (string pn in pl) {
					comboPar.Items.Add(pn);
				}
				comboPar.SelectedIndex = 0;
			}
		}

		private void comboPar_SelectedIndexChanged(object sender, System.EventArgs e) {
			string rn = comboRes.Text;
			string pn = comboPar.Text;
			string val = _res.GetConfigValue(rn, pn);
			textValue.Text = val;
		}

		private void buttonApply_Click(object sender, System.EventArgs e) {
			string rn = comboRes.Text;
			string pn = comboPar.Text;
			string val = textValue.Text;
		
			_res.SetConfigValue(rn, pn, val);
			DialogResult = DialogResult.OK;
		}

		private void buttonClose_Click(object sender, System.EventArgs e) {
			this.Close();
		}
	}
}
