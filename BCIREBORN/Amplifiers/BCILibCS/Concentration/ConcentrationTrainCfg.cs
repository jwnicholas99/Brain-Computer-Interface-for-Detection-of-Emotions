using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using BCILib.Amp;
using BCILib.App;
using BCILib.Util;

namespace BCILib.Concentration
{
	/// <summary>
	/// Summary description for ConcentrationTrainModel.
	/// </summary>
	internal class ConcentrationTrainCfg : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.TextBox textBoxWndSecs;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox checkBoxUseCSP;
		private System.Windows.Forms.TextBox textBoxDownSpn;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button buttonSaveList;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button buttonAddPath;
		private System.Windows.Forms.Button buttonDelPath;
		private System.Windows.Forms.Button buttonReload;
		private System.Windows.Forms.Button buttonTrain;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private const string fn_folderlist = "ConcentrationTrainFolders.txt";
		private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonFindTool;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textBoxNumFolder;
		private System.Windows.Forms.TextBox textBoxWinShift;
		private System.Windows.Forms.CheckBox checkBoxGTest;
		private System.Windows.Forms.TextBox textBoxSelChannels;
		private System.Windows.Forms.TextBox textBoxNumSel;
		private System.Windows.Forms.Button buttonSelChannel;
		private System.Windows.Forms.ListView listViewFileList;
        private Label label7;
        private ComboBox comboAmplifier;
        private Button buttonRefresh;
		private const string fn_traincfg = @"Config\ConcentrationTrain.cfg";

		public ConcentrationTrainCfg()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
		}

		public void InitStart() {
            ShowAmplifier();
			LoadConfig();
			LoadFolderList();
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.textBoxWndSecs = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBoxUseCSP = new System.Windows.Forms.CheckBox();
            this.textBoxDownSpn = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonSaveList = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.listViewFileList = new System.Windows.Forms.ListView();
            this.buttonAddPath = new System.Windows.Forms.Button();
            this.buttonDelPath = new System.Windows.Forms.Button();
            this.buttonReload = new System.Windows.Forms.Button();
            this.buttonTrain = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonFindTool = new System.Windows.Forms.Button();
            this.textBoxNumFolder = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxWinShift = new System.Windows.Forms.TextBox();
            this.checkBoxGTest = new System.Windows.Forms.CheckBox();
            this.textBoxSelChannels = new System.Windows.Forms.TextBox();
            this.textBoxNumSel = new System.Windows.Forms.TextBox();
            this.buttonSelChannel = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.comboAmplifier = new System.Windows.Forms.ComboBox();
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxWndSecs
            // 
            this.textBoxWndSecs.Location = new System.Drawing.Point(160, 64);
            this.textBoxWndSecs.Name = "textBoxWndSecs";
            this.textBoxWndSecs.Size = new System.Drawing.Size(40, 20);
            this.textBoxWndSecs.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(144, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "Window length (seconds):";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // checkBoxUseCSP
            // 
            this.checkBoxUseCSP.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxUseCSP.Location = new System.Drawing.Point(72, 168);
            this.checkBoxUseCSP.Name = "checkBoxUseCSP";
            this.checkBoxUseCSP.Size = new System.Drawing.Size(80, 24);
            this.checkBoxUseCSP.TabIndex = 2;
            this.checkBoxUseCSP.Text = "Use CSP:";
            // 
            // textBoxDownSpn
            // 
            this.textBoxDownSpn.Location = new System.Drawing.Point(160, 88);
            this.textBoxDownSpn.Name = "textBoxDownSpn";
            this.textBoxDownSpn.Size = new System.Drawing.Size(40, 20);
            this.textBoxDownSpn.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 322);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Selected Channels";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonSaveList
            // 
            this.buttonSaveList.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonSaveList.Location = new System.Drawing.Point(539, 416);
            this.buttonSaveList.Name = "buttonSaveList";
            this.buttonSaveList.Size = new System.Drawing.Size(40, 23);
            this.buttonSaveList.TabIndex = 10;
            this.buttonSaveList.Text = "&Save";
            this.buttonSaveList.Click += new System.EventHandler(this.buttonSaveList_Click);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.Location = new System.Drawing.Point(248, 8);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(478, 16);
            this.label4.TabIndex = 6;
            this.label4.Text = "Training Raw EEG Data Files";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // listViewFileList
            // 
            this.listViewFileList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewFileList.CheckBoxes = true;
            this.listViewFileList.FullRowSelect = true;
            this.listViewFileList.Location = new System.Drawing.Point(208, 29);
            this.listViewFileList.Name = "listViewFileList";
            this.listViewFileList.ShowItemToolTips = true;
            this.listViewFileList.Size = new System.Drawing.Size(550, 376);
            this.listViewFileList.TabIndex = 7;
            this.listViewFileList.UseCompatibleStateImageBehavior = false;
            this.listViewFileList.View = System.Windows.Forms.View.List;
            // 
            // buttonAddPath
            // 
            this.buttonAddPath.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonAddPath.Location = new System.Drawing.Point(379, 416);
            this.buttonAddPath.Name = "buttonAddPath";
            this.buttonAddPath.Size = new System.Drawing.Size(40, 23);
            this.buttonAddPath.TabIndex = 11;
            this.buttonAddPath.Text = "&Add";
            this.buttonAddPath.Click += new System.EventHandler(this.buttonAddPath_Click);
            // 
            // buttonDelPath
            // 
            this.buttonDelPath.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonDelPath.Location = new System.Drawing.Point(435, 416);
            this.buttonDelPath.Name = "buttonDelPath";
            this.buttonDelPath.Size = new System.Drawing.Size(40, 23);
            this.buttonDelPath.TabIndex = 8;
            this.buttonDelPath.Text = "&Del";
            this.buttonDelPath.Click += new System.EventHandler(this.buttonDelPath_Click);
            // 
            // buttonReload
            // 
            this.buttonReload.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonReload.Location = new System.Drawing.Point(483, 416);
            this.buttonReload.Name = "buttonReload";
            this.buttonReload.Size = new System.Drawing.Size(40, 23);
            this.buttonReload.TabIndex = 9;
            this.buttonReload.Text = "&Load";
            this.buttonReload.Click += new System.EventHandler(this.buttonReload_Click);
            // 
            // buttonTrain
            // 
            this.buttonTrain.Location = new System.Drawing.Point(131, 394);
            this.buttonTrain.Name = "buttonTrain";
            this.buttonTrain.Size = new System.Drawing.Size(48, 23);
            this.buttonTrain.TabIndex = 12;
            this.buttonTrain.Text = "&Train...";
            this.buttonTrain.Click += new System.EventHandler(this.buttonTrain_Click);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(8, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(144, 23);
            this.label3.TabIndex = 0;
            this.label3.Text = "Downsamping ratio";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // buttonFindTool
            // 
            this.buttonFindTool.Location = new System.Drawing.Point(19, 394);
            this.buttonFindTool.Name = "buttonFindTool";
            this.buttonFindTool.Size = new System.Drawing.Size(64, 23);
            this.buttonFindTool.TabIndex = 12;
            this.buttonFindTool.Text = "&FindTool";
            this.buttonFindTool.Click += new System.EventHandler(this.buttonFindTool_Click);
            // 
            // textBoxNumFolder
            // 
            this.textBoxNumFolder.Location = new System.Drawing.Point(160, 112);
            this.textBoxNumFolder.Name = "textBoxNumFolder";
            this.textBoxNumFolder.Size = new System.Drawing.Size(40, 20);
            this.textBoxNumFolder.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(8, 112);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(144, 23);
            this.label5.TabIndex = 0;
            this.label5.Text = "N-Folder";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(8, 136);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(144, 23);
            this.label6.TabIndex = 0;
            this.label6.Text = "Win Shift(seconds)";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxWinShift
            // 
            this.textBoxWinShift.Location = new System.Drawing.Point(160, 136);
            this.textBoxWinShift.Name = "textBoxWinShift";
            this.textBoxWinShift.Size = new System.Drawing.Size(40, 20);
            this.textBoxWinShift.TabIndex = 1;
            // 
            // checkBoxGTest
            // 
            this.checkBoxGTest.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxGTest.Location = new System.Drawing.Point(72, 192);
            this.checkBoxGTest.Name = "checkBoxGTest";
            this.checkBoxGTest.Size = new System.Drawing.Size(80, 24);
            this.checkBoxGTest.TabIndex = 2;
            this.checkBoxGTest.Text = "G-Test:";
            // 
            // textBoxSelChannels
            // 
            this.textBoxSelChannels.Location = new System.Drawing.Point(10, 360);
            this.textBoxSelChannels.Name = "textBoxSelChannels";
            this.textBoxSelChannels.ReadOnly = true;
            this.textBoxSelChannels.Size = new System.Drawing.Size(192, 20);
            this.textBoxSelChannels.TabIndex = 1;
            // 
            // textBoxNumSel
            // 
            this.textBoxNumSel.Location = new System.Drawing.Point(119, 319);
            this.textBoxNumSel.Name = "textBoxNumSel";
            this.textBoxNumSel.ReadOnly = true;
            this.textBoxNumSel.Size = new System.Drawing.Size(24, 20);
            this.textBoxNumSel.TabIndex = 1;
            // 
            // buttonSelChannel
            // 
            this.buttonSelChannel.Location = new System.Drawing.Point(152, 317);
            this.buttonSelChannel.Name = "buttonSelChannel";
            this.buttonSelChannel.Size = new System.Drawing.Size(48, 23);
            this.buttonSelChannel.TabIndex = 12;
            this.buttonSelChannel.Text = "&Sel...";
            this.buttonSelChannel.Click += new System.EventHandler(this.buttonSelChannel_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 223);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(49, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Amplifier:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // comboAmplifier
            // 
            this.comboAmplifier.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboAmplifier.FormattingEnabled = true;
            this.comboAmplifier.Location = new System.Drawing.Point(11, 253);
            this.comboAmplifier.Name = "comboAmplifier";
            this.comboAmplifier.Size = new System.Drawing.Size(189, 21);
            this.comboAmplifier.TabIndex = 13;
            this.comboAmplifier.SelectedIndexChanged += new System.EventHandler(this.comboAmplifier_SelectedIndexChanged);
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.Location = new System.Drawing.Point(59, 32);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(75, 23);
            this.buttonRefresh.TabIndex = 14;
            this.buttonRefresh.Text = "Refresh";
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            // 
            // ConcentrationTrainCfg
            // 
            this.Controls.Add(this.buttonRefresh);
            this.Controls.Add(this.comboAmplifier);
            this.Controls.Add(this.buttonTrain);
            this.Controls.Add(this.buttonSaveList);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.listViewFileList);
            this.Controls.Add(this.buttonAddPath);
            this.Controls.Add(this.buttonDelPath);
            this.Controls.Add(this.buttonReload);
            this.Controls.Add(this.checkBoxUseCSP);
            this.Controls.Add(this.textBoxDownSpn);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxWndSecs);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.buttonFindTool);
            this.Controls.Add(this.textBoxNumFolder);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBoxWinShift);
            this.Controls.Add(this.checkBoxGTest);
            this.Controls.Add(this.textBoxSelChannels);
            this.Controls.Add(this.textBoxNumSel);
            this.Controls.Add(this.buttonSelChannel);
            this.Name = "ConcentrationTrainCfg";
            this.Size = new System.Drawing.Size(766, 448);
            this.Load += new System.EventHandler(this.TrainModelCfg_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private static ListViewItem CreateFolderItem(string line) {
			bool sel = true;
			int type = 1;

			bool readsel = false;
			bool readtyp = false;

			char[] sep = new char[] {' '};

			while (true) {
				string[] vals = line.Split(sep, 2);
				if (vals.Length == 2) {
					vals[0] = vals[0].ToLower().Trim();
					if (!readsel && (vals[0] == "false" || vals[0] == "true")) {
						sel = bool.Parse(vals[0]);
						line = vals[1];
						readsel = true;
						continue;
					}

					if (!readtyp) {
						bool isdigit = true;
						foreach (char c in vals[0]) {
							if (!char.IsDigit(c)) {
								isdigit = false;
								break;
							}
						}
						if (isdigit) {
							type = int.Parse(vals[0]);
							line = vals[1];
							readtyp = true;
							continue;
						}
					}
				}
				break;
			}

			ListViewItem item = new ListViewItem(type.ToString());
			item.Checked = sel;
			item.SubItems.Add(line);

			return item;
		}

		private int cfg_winlen_sec = 2;
		private int cfg_ds_rate = 1;
		private int cfg_use_csp = 0;
		private int cfg_num_folder = 2;
		private double cfg_win_shift = 1.0;
		private string cfg_chn_all = null;
        private string cfg_used_chidx = "[0,1]";

		public void LoadConfig() {
			ResManager rm = new ResManager();
			if (File.Exists(fn_traincfg)) {
				rm.LoadFile(fn_traincfg);
			}
			LoadConfig(rm);
		}

		private void LoadConfig(ResManager rm) {
			// Initialize for tabTrainModel
			string rn = "ModelSetting";
			rm.GetConfigValue(rn, "WindowLenSec", ref cfg_winlen_sec);
			rm.GetConfigValue(rn, "DownsampleRate", ref cfg_ds_rate);
			rm.GetConfigValue(rn, "IfCSP", ref cfg_use_csp);

			rm.GetConfigValue(rn, "nFold", ref cfg_num_folder);
			rm.GetConfigValue(rn, "WinShift", ref cfg_win_shift);

			rm.SaveIfChanged();

			textBoxWndSecs.Text = cfg_winlen_sec.ToString();
			textBoxDownSpn.Text = cfg_ds_rate.ToString();

			textBoxNumFolder.Text = cfg_num_folder.ToString();
			textBoxWinShift.Text = cfg_win_shift.ToString();

            // initial channel names
			string line = rm.GetConfigValue(rn, "ChannelList");
            int nc = 0;
			textBoxSelChannels.Text = line;
			if (line != null) {
				string[] slist = line.Split(',');
				foreach (string cn in slist) {
					string sn = cn.Trim();
					if (sn.Length == 0) continue;
					nc++;
				}
                //if (nc >= 3) cfg_use_csp = 1;
			}

            // load cfg_used_chidx
            cfg_used_chidx = rm.GetConfigValue(rn, "UsedChannelIdx");

            if (nc == 0) {
                Amplifier amp = AmpContainer.GetAmplifier(comboAmplifier.Text);
                if (amp != null) {
                    string[] clist = amp.ChannelNames;
                    if (clist.Length >= 2) line = string.Format("{0},{1}", clist[0], clist[1]);
                    textBoxSelChannels.Text = line;
                    nc = 2;

                    cfg_used_chidx = "[0,1]";
                }
            }
            textBoxNumSel.Text = nc.ToString();
            if (nc < 3) cfg_use_csp = 0;
			checkBoxUseCSP.Checked = (cfg_use_csp != 0);
		}

		public void LoadFolderList() {
			BCIApplication.LoadTraingFileLists(listViewFileList);
		}

		private void SaveTrainConfig() {
			// if configure file not exists, create it
			FileInfo finf = new FileInfo(fn_traincfg);
			if (!finf.Directory.Exists) {
				finf.Directory.Create();
			}
			if (!finf.Exists) {
				finf.CreateText().Close();
			}

			ResManager rm = new ResManager(fn_traincfg);
			string rn = "ModelSetting";

			rm.SetConfigValue(rn, "WindowLenSec", textBoxWndSecs.Text);
            rm.SetConfigValue(rn, "ModelType", "Task vs Relax");
			rm.SetConfigValue(rn, "DownsampleRate", textBoxDownSpn.Text);
			rm.SetConfigValue(rn, "IfCSP", (checkBoxUseCSP.Checked? 1:0).ToString());
			rm.SetConfigValue(rn, "nFold", textBoxNumFolder.Text);
			rm.SetConfigValue(rn, "WinShift", textBoxWinShift.Text);
            // actually should be  UesedChannelList
			rm.SetConfigValue(rn, "ChannelList", textBoxSelChannels.Text);
            // added on 20090906 -- ccwang, training should be changed.
            rm.SetConfigValue(rn, "UsedChannelIdx", cfg_used_chidx);

			try {
				LoadConfig(rm);
			} catch (Exception) {
				LoadConfig();
			}
		}

		private void buttonSaveList_Click(object sender, System.EventArgs e) {
			SaveTrainConfig();
			BCIApplication.SaveTrainingFileLists(listViewFileList);
		}

		private void buttonDelPath_Click(object sender, System.EventArgs e) {
			if (listViewFileList.SelectedIndices.Count == 0) {
                MessageBox.Show("Please select item first");
                return;
            }

            while (listViewFileList.SelectedIndices.Count > 0) {
                int sel = listViewFileList.SelectedIndices[0];
                listViewFileList.Items.RemoveAt(sel);
			}
            BCIApplication.SaveTrainingFileLists(listViewFileList);
        }

		private void buttonReload_Click(object sender, System.EventArgs e) {
			LoadConfig();
			LoadFolderList();
		}

		private void buttonTrain_Click(object sender, System.EventArgs e) {
            int n = 0;

            try {
                n = int.Parse(textBoxNumSel.Text);
            }
            catch (Exception) {
            }

            if (n < 1) {
                MessageBox.Show("Channels not set correctly!");
                return ;
            }

            if (n < 3 && checkBoxUseCSP.Checked) {
                //if (MessageBox.Show(this, "Cannot use CSP for this number of channels!", "Parameter Error", MessageBoxButtons.OKCancel) == DialogResult.Cancel) {
                //    return;
                //}
                checkBoxUseCSP.Checked = false;
            }

            // Save training config
            buttonSaveList.PerformClick();

            if (StartTraining(true)) {
                Control parent = this.Parent;
                if (parent is Form) ((Form)parent).Close();
            }
        }

        public static bool TrainModel()
        {
            //// used channel
            //cfg_used_chidx = "[1,2]";
            //// no CSP
            //checkBoxUseCSP.Checked = false;

            ResManager rm = new ResManager();
            if (File.Exists(fn_traincfg)) {
                rm.LoadFile(fn_traincfg);
            }

            string rn = "ModelSetting";

            rm.SetConfigValue(rn, "WindowLenSec", 2.ToString());
            rm.SetConfigValue(rn, "ModelType", "Task vs Relax");
            rm.SetConfigValue(rn, "DownsampleRate", 1.ToString());
            rm.SetConfigValue(rn, "IfCSP", 0.ToString());
            rm.SetConfigValue(rn, "nFold", 10.ToString());
            rm.SetConfigValue(rn, "WinShift", 1.ToString());
            // actually should be  UesedChannelList
            rm.SetConfigValue(rn, "ChannelList", "Ch1,Ch2");
            // added on 20090906 -- ccwang, training should be changed.
            rm.SetConfigValue(rn, "UsedChannelIdx", "[1,2]");
            rm.SaveFile(fn_traincfg);

            return StartTraining(false);
        }

        private static bool StartTraining(bool ShowProcWindow)
        {
			//check training program
            string tpath = BCIApplication.GetGamePath("TrainTool");
            if (tpath == null || !File.Exists(tpath))
            {
                MessageBox.Show("Cannot find training tool path.");
                return false;
            }

			Console.WriteLine("Using tool {0}", tpath);

            // check training file

			string m_path = "Model";
			DirectoryInfo dinf1 = new DirectoryInfo(m_path);
			if (!dinf1.Exists) dinf1.Create();
			string timestamp = BCIApplication.TimeStamp;

			// ModelFile
			m_path = Path.Combine(dinf1.FullName, string.Format("{0}_{1}.txt",
                Path.GetFileNameWithoutExtension(tpath), timestamp));
			FileInfo finf = new FileInfo(m_path);

			// Start process
			ProcessStartInfo pinf = new ProcessStartInfo();
			pinf.FileName = tpath;
			//int gtest = checkBoxGTest.Checked? 1:0;
            int gtest = 0;
			pinf.Arguments = string.Format("\"{0}\" \"{1}\" \"{2}\" {3}", m_path, 
				Path.GetFullPath(fn_traincfg), Path.GetFullPath(BCICfg.TrainingFileName), gtest);
			pinf.WorkingDirectory = Path.GetDirectoryName(tpath);
			Console.WriteLine("Call cmd: {0} {1}", pinf.FileName, pinf.Arguments);

			StartProcForm pfm = new StartProcForm(pinf);

            if (ShowProcWindow) {
                //this.TopLevelControl.Hide();
                pfm.ShowDialog();
                //this.TopLevelControl.Show();
            }
            else {
                //pfm.ShowInTaskbar = false;
                //pfm.WindowState = FormWindowState.Minimized;
                pfm.Show();

                pfm.Run();
                // wait for result to be out?
                while (!pfm.ControlBox) {
                    Application.DoEvents();
                    Thread.Sleep(100);
                }

                pfm.Hide();
            }

			// save m_path into system.cfg;
			string par_mdl = "ModelFile";
            m_path = BCIApplication.GetRelativeUserPath(m_path);
			if (File.Exists(m_path)) {
				FileInfo fi = new FileInfo(m_path);
				int n = fi.FullName.Length - fi.Extension.Length;
				string fn_log = fi.FullName.Substring(0, n) + ".log";
				Console.WriteLine("Save Model Log file: {0}", fn_log);

				StreamWriter sw = File.CreateText(fn_log);
				sw.Write(pfm.OutputString);
				sw.Close();

                ResManager rm = BCIApplication.SysResource;
                string rn = BCICfg.AttentionDetection;
                rm.SetConfigValue(rn, par_mdl, m_path);
                Console.WriteLine("{0} = {1}", par_mdl, m_path);

                if (ShowProcWindow) {
                    MessageBox.Show("Training finished, model file: " + m_path, "Training");
                }
                else {
                    StringReader sr = new StringReader(pfm.OutputString);
                    string line;
                    double acc_val = 0;
                    while ((line = sr.ReadLine()) != null) {
                        string acc_flag = "CV accuracy =";
                        if (line.StartsWith(acc_flag)) {
                            acc_val = double.Parse(line.Substring(acc_flag.Length));
                            break;
                        }
                    }

                    double acc_th = 60;
                    rm.GetConfigValue(BCICfg.Train, "Attention_Accuracy_Threashold", ref acc_th);

                    rm.SetConfigValue(BCICfg.Test, BCICfg.UseGlobalProfile, 
                        (acc_val >= acc_th? false : true).ToString());

                    sr.Close();

                    MessageBox.Show("Training finished. Accuracy = " + acc_val.ToString("0.##") + 
                        " / threshold = " + acc_th.ToString("0.##"));
                }

                //rm.SetConfigValue(BCICfg.Train, BCICfg.Current_Train, null);

				// 20080919: set some parameters
				// set trial selection threshold to 0 -- model set
				rm.SetConfigValue(rn, "TrialSelThreshold", "0");

				rm.SaveFile();

                return true;
            } else if (!ShowProcWindow) {
                MessageBox.Show("Training failed!" + pfm.OutputString);
            }

            return false;
		}

		private void buttonAddPath_Click(object sender, System.EventArgs e) {
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.RestoreDirectory = true;
			dlg.Filter = "cnt File(*.cnt)|*.cnt|All files(*.*)|*.*";
            //dlg.InitialDirectory = BCIApplication.GetTrainDataDir();
			dlg.Multiselect = true;
            dlg.InitialDirectory = Directory.GetCurrentDirectory();
			DialogResult rst = dlg.ShowDialog(this);

			if (rst == DialogResult.OK) {
				string pwd = Environment.CurrentDirectory;
				foreach (string fni in dlg.FileNames) {
					string fn = fni;
					if (fn.StartsWith(pwd)) fn = fn.Substring(pwd.Length + 1);

					// Add training file list
					for (int i = 0; i < listViewFileList.Items.Count; i++) {
						string fo = listViewFileList.Items[i].Text;
						if (fo == fn) {
							MessageBox.Show("File already included!");
							continue;
						}
					}
					listViewFileList.Items.Add(fn).Checked = true;
				}
				BCIApplication.SaveTrainingFileLists(listViewFileList);
			}
		}

		private void buttonSelChannel_Click(object sender, System.EventArgs e) {
            SelectStringList dlg = new SelectStringList();
            if (comboAmplifier.Text != null) {
                Amplifier amp = AmpContainer.GetAmplifier(comboAmplifier.Text);
                if (amp != null) {
                    cfg_chn_all = amp.GetChannelNameString();
                }
            }

            dlg.CandidateString = cfg_chn_all;
			dlg.SelectedString = textBoxSelChannels.Text;

			if (dlg.ShowDialog() == DialogResult.OK) {
				textBoxSelChannels.Text = dlg.SelectedString;
				int n = dlg.SelectedNum;
				textBoxNumSel.Text = n.ToString();
				if (n >= 3) cfg_use_csp = 1;
				else cfg_use_csp = 0;
				checkBoxUseCSP.Checked = (cfg_use_csp != 0);

                // calculate cfg_used_chidx
                int[] slist = dlg.SelectedListIndex;
                StringBuilder sb = new StringBuilder("[");
                foreach (int i in slist) {
                    sb.Append(i.ToString());
                    sb.Append(',');
                }
                sb.Append("]");
                cfg_used_chidx = sb.ToString();

                SaveTrainConfig();
			}
		}

		private void TrainModelCfg_Load(object sender, System.EventArgs e) {
		}

		private void buttonFindTool_Click(object sender, System.EventArgs e) {
            BCIApplication.GetGamePath("TrainTool", true);
		}

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            InitStart();
        }

        private void ShowAmplifier()
        {
            string selAmp = comboAmplifier.Text;
            comboAmplifier.Items.Clear();
            int n = AmpContainer.Count;
            for (int i = 0; i < n; i++) {
                string ampid = AmpContainer.GetAmplifier(i).ID;
                comboAmplifier.Items.Add(ampid);
                if (selAmp == ampid) comboAmplifier.SelectedIndex = i;
            }
            if (n > 0 && comboAmplifier.SelectedIndex < 0) comboAmplifier.SelectedIndex = 0;
        }

        private void comboAmplifier_SelectedIndexChanged(object sender, EventArgs e)
        {
            Amplifier amp = AmpContainer.GetAmplifier(comboAmplifier.Text);
            if (amp != null) {
                SelectStringList dlg = new SelectStringList();
                dlg.Candidates = amp.ChannelNames;
                int n = 0;
                if (!string.IsNullOrEmpty(cfg_used_chidx)) {
                    string[] clist = cfg_used_chidx.Split("[], ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    n = clist.Length;
                    if (n > 0) {
                        int[] sidx = new int[n];
                        for (int i = 0; i < n; i++) {
                            sidx[i] = int.Parse(clist[i]);
                        }

                        dlg.SelectedListIndex = sidx;
                    }
                }
                cfg_chn_all = dlg.CandidateString;
                textBoxNumSel.Text = n.ToString();
                textBoxSelChannels.Text = dlg.SelectedString;
                checkBoxUseCSP.Checked = (n > 2);
            }
        }
    }
}
