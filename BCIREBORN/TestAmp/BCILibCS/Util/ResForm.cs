using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;

namespace BCILib.Util
{
	/// <summary>
	/// Summary description for ConfigForm.
	/// </summary>
    class ResForm : System.Windows.Forms.Form
    {
        private IContainer components;
		private ArrayList _nameList = new ArrayList();
		private System.Windows.Forms.Panel _contentPanel;
		private ArrayList _valList = new ArrayList();
		private System.Windows.Forms.MainMenu mainMenu;
		private System.Windows.Forms.MenuItem fileMenu;
		private System.Windows.Forms.MenuItem menuSave;
		private System.Windows.Forms.MenuItem menuClose;
		private bool _isValueChanged = false;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuEdit;

		private ResManager _rm;

		public ResForm(ResManager rm) {
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			_rm = rm;
			InitializeContents();

			fileMenu.Popup += new EventHandler(onFilePopup);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if(components != null) {
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
		private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ResForm));
            this._contentPanel = new System.Windows.Forms.Panel();
            this.mainMenu = new System.Windows.Forms.MainMenu(this.components);
            this.fileMenu = new System.Windows.Forms.MenuItem();
            this.menuSave = new System.Windows.Forms.MenuItem();
            this.menuClose = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuEdit = new System.Windows.Forms.MenuItem();
            this.SuspendLayout();
            // 
            // _contentPanel
            // 
            this._contentPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._contentPanel.AutoScroll = true;
            this._contentPanel.Location = new System.Drawing.Point(0, 0);
            this._contentPanel.Name = "_contentPanel";
            this._contentPanel.Size = new System.Drawing.Size(496, 499);
            this._contentPanel.TabIndex = 1;
            // 
            // mainMenu
            // 
            this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.fileMenu,
            this.menuItem1});
            // 
            // fileMenu
            // 
            this.fileMenu.Index = 0;
            this.fileMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuSave,
            this.menuClose});
            this.fileMenu.Text = "&File";
            // 
            // menuSave
            // 
            this.menuSave.Index = 0;
            this.menuSave.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
            this.menuSave.Text = "&Save";
            this.menuSave.Click += new System.EventHandler(this.menuSave_Click);
            // 
            // menuClose
            // 
            this.menuClose.Index = 1;
            this.menuClose.Shortcut = System.Windows.Forms.Shortcut.CtrlQ;
            this.menuClose.Text = "&Close";
            this.menuClose.Click += new System.EventHandler(this.menuClose_Click);
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 1;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuEdit});
            this.menuItem1.Text = "&Tools";
            // 
            // menuEdit
            // 
            this.menuEdit.Index = 0;
            this.menuEdit.Text = "Edit Item";
            this.menuEdit.Click += new System.EventHandler(this.menuEdit_Click);
            // 
            // ResForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(496, 497);
            this.Controls.Add(this._contentPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Menu = this.mainMenu;
            this.MinimizeBox = false;
            this.Name = "ResForm";
            this.ShowInTaskbar = false;
            this.Text = "ConfigForm";
            this.ResumeLayout(false);

		}
		#endregion

		private void toolBar_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e) {
		
		}

		private void AddItem(string prop, string val, int x, int y) {
			Label nl = new Label();
			// nl.BorderStyle = BorderStyle.FixedSingle;
			nl.Text = prop;
			_nameList.Add(nl);
			_contentPanel.Controls.Add(nl);

			nl.Location = new Point(x, y);
			nl.AutoSize = true;
			nl.TextAlign = ContentAlignment.MiddleRight;

			TextBox vb = new TextBox();
			vb.Text = "";
			if (val != null) vb.Text = val;
			_valList.Add(vb);
			_contentPanel.Controls.Add(vb);
			vb.Top = y;
		}

		private void InitializeContents() {
			// clear all contents
			_nameList.Clear();
			_valList.Clear();
			_contentPanel.Controls.Clear();

			string [] res_all = _rm.GetAllResource();
			if (res_all == null) return;

			int mx = 16, my = 2;
			int x = 0, y = 0;
			int w = 150;
			int h = 20;
			foreach (string res in res_all) {
				y += h;
				AddItem("Resource", res, x, y);
				y += my + h;

				string [] pall = _rm.GetAllResProps(res);
				if (pall == null) continue;
				foreach (string prop in pall) {
					AddItem(prop, _rm.GetConfigValue(res, prop), x, y);
					y += my + h;
				}
			}

			int maxn = 100;
			foreach (Label nl in _nameList) {
				if (nl.Width > maxn) maxn = nl.Width;
			}

			foreach (Label nl in _nameList) {
				nl.Width = maxn;
				nl.AutoSize = false;
			}

			w = _contentPanel.ClientSize.Width - x - maxn - mx - 2;
			foreach (TextBox vb in _valList) {
				vb.Left = x + maxn + mx;
				vb.Width = w;
				vb.Anchor = (AnchorStyles) (AnchorStyles.Left | AnchorStyles.Right);
				vb.TextChanged += new EventHandler(OnValueChanged);
			}

			_contentPanel.AutoScrollMinSize = new Size(maxn + mx + 100, y);
		}

		private void OnValueChanged(object sender, EventArgs e) {
			_isValueChanged = true;
		}

		private void onFilePopup(object sender, EventArgs e) {
			this.menuSave.Enabled = _isValueChanged;
		}

		private void menuSave_Click(object sender, System.EventArgs e) {
			SaveFile();
		}
	
		protected override void OnClosing(CancelEventArgs e) {
			if (_isValueChanged) {
				DialogResult res = MessageBox.Show("Document has been changed. Do you want to save the changes?", 
					"Close Window", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
				if (res == DialogResult.Cancel) {
					e.Cancel = true;
					return;
				} 
				else if (res == DialogResult.Yes) {
					SaveFile();
				}
			}

			base.OnClosing (e);
		}

		public void SaveFile() {
			string res = null;
			int n = _nameList.Count;
			for (int i = 0; i < n; i++) {
				string par = ((Label) _nameList[i]).Text;
				string val = ((TextBox) _valList[i]).Text;
				if (string.Compare("Resource", par, true) == 0) {
					res = val;
					continue;
				}

				_rm.SetConfigValue(res, par, val);
			} 
			_rm.UpdateAllFiles();
			_isValueChanged = false;
		}

		private void menuClose_Click(object sender, System.EventArgs e) {
			Close();
		}

		private void menuEdit_Click(object sender, System.EventArgs e) {		
			if (_isValueChanged) {
				if (MessageBox.Show("Save changes?", "Save Change", MessageBoxButtons.YesNo)
					== DialogResult.Yes) {
					SaveFile();
				}
			}

			EditResItemForm iform = new EditResItemForm(_rm);
			if (iform.ShowDialog() == DialogResult.OK) {
				_rm.SaveFile();

				InitializeContents();
			}
		}
	}
}
