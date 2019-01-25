using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Text;

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Linq;

using BCILib.Util;
using BCILib.sp;
using BCILib.App;
using BCILib.Amp;

namespace BCILib.MotorImagery
{
	/// <summary>
	/// Summary description for MITrainMTSModelForm.
	/// </summary>
	public class MITrainMTSModelForm : System.Windows.Forms.Form
    {
		private ResManager cfg = null;
		private System.Windows.Forms.Panel panel1;

		private const int MARGIN_X = 20;
		private const int MARGIN_Y = 20;
		private int DSP_Y = 100;

		private string[] channel_name = null;
		// private CheckBox[] channel_chk = null;
		private bool[] channel_sel = null;
		private Rectangle[] channel_reg = null;
		private int num_column = 1;
		private System.Windows.Forms.Button btnShowFreqPower1;
		
		private System.Windows.Forms.StatusBar statusBar1;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabTrain;
		private System.Windows.Forms.TabPage tabFreqPower;
		private System.Windows.Forms.ToolBar toolBar1;
		private System.Windows.Forms.ToolBarButton toolBarReDraw;
		private System.Windows.Forms.ToolBarButton toolBarYIn;
		private System.Windows.Forms.ToolBarButton toolBarYOut;
		private System.Windows.Forms.ToolBarButton toolBarYInc;
		private System.Windows.Forms.ToolBarButton toolBarYDec;
		private System.Windows.Forms.ToolBarButton toolBarXIn;
		private System.Windows.Forms.ToolBarButton toolBarXOut;
		private System.Windows.Forms.Button buttonAddTrainFile;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btDelete;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button buttonTrainBrowse;
		private System.Windows.Forms.TextBox textBoxTrainEEGPath;
		private System.Windows.Forms.ListView listViewTrainFiles;
		private System.Windows.Forms.Button buttonSaveTrainList;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ToolBarButton toolBarSysConfig;
		private System.Windows.Forms.ToolBar toolBarMainWindow;
		private System.Windows.Forms.ToolBarButton toolBarLoadConfig;
		private System.Windows.Forms.Button buttonSaveJPeg;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Button buttonFindTool;
		private System.Windows.Forms.TabPage tabTest;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBoxTestFile;
		private System.Windows.Forms.Button buttonFindTestFile;
		private System.Windows.Forms.Button buttonStartTest;
		private System.Windows.Forms.Button buttonStop;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textBoxNumSegments;
		private System.Windows.Forms.Button buttonTrain;
        private System.Windows.Forms.Button buttonAddTrainDir;
        private SelAmpChannel selAmpChannel1;
        private Button buttonOpen;
		private System.ComponentModel.IContainer components;

		public MITrainMTSModelForm() {
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

            cfg = BCIApplication.SysResource;
			ResetParas();
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
            this.buttonAddTrainFile = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnShowFreqPower1 = new System.Windows.Forms.Button();
            this.statusBar1 = new System.Windows.Forms.StatusBar();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.buttonTrain = new System.Windows.Forms.Button();
            this.btDelete = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabTrain = new System.Windows.Forms.TabPage();
            this.buttonFindTool = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonSaveTrainList = new System.Windows.Forms.Button();
            this.listViewTrainFiles = new System.Windows.Forms.ListView();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonAddTrainDir = new System.Windows.Forms.Button();
            this.tabTest = new System.Windows.Forms.TabPage();
            this.buttonFindTestFile = new System.Windows.Forms.Button();
            this.textBoxTestFile = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonStartTest = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxNumSegments = new System.Windows.Forms.TextBox();
            this.tabFreqPower = new System.Windows.Forms.TabPage();
            this.buttonTrainBrowse = new System.Windows.Forms.Button();
            this.textBoxTrainEEGPath = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.toolBar1 = new System.Windows.Forms.ToolBar();
            this.toolBarReDraw = new System.Windows.Forms.ToolBarButton();
            this.toolBarYIn = new System.Windows.Forms.ToolBarButton();
            this.toolBarYOut = new System.Windows.Forms.ToolBarButton();
            this.toolBarYInc = new System.Windows.Forms.ToolBarButton();
            this.toolBarYDec = new System.Windows.Forms.ToolBarButton();
            this.toolBarXIn = new System.Windows.Forms.ToolBarButton();
            this.toolBarXOut = new System.Windows.Forms.ToolBarButton();
            this.buttonSaveJPeg = new System.Windows.Forms.Button();
            this.toolBarMainWindow = new System.Windows.Forms.ToolBar();
            this.toolBarSysConfig = new System.Windows.Forms.ToolBarButton();
            this.toolBarLoadConfig = new System.Windows.Forms.ToolBarButton();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.buttonOpen = new System.Windows.Forms.Button();
            this.selAmpChannel1 = new BCILib.Amp.SelAmpChannel();
            this.tabControl1.SuspendLayout();
            this.tabTrain.SuspendLayout();
            this.tabTest.SuspendLayout();
            this.tabFreqPower.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonAddTrainFile
            // 
            this.buttonAddTrainFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAddTrainFile.Location = new System.Drawing.Point(634, 48);
            this.buttonAddTrainFile.Name = "buttonAddTrainFile";
            this.buttonAddTrainFile.Size = new System.Drawing.Size(75, 23);
            this.buttonAddTrainFile.TabIndex = 2;
            this.buttonAddTrainFile.Text = "Add File";
            this.buttonAddTrainFile.Click += new System.EventHandler(this.ButtonAddTrainFile_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Location = new System.Drawing.Point(8, 72);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(693, 225);
            this.panel1.TabIndex = 4;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            this.panel1.Click += new System.EventHandler(this.panel1_Click);
            this.panel1.Resize += new System.EventHandler(this.panel1_Resize);
            // 
            // btnShowFreqPower1
            // 
            this.btnShowFreqPower1.Location = new System.Drawing.Point(16, 40);
            this.btnShowFreqPower1.Name = "btnShowFreqPower1";
            this.btnShowFreqPower1.Size = new System.Drawing.Size(72, 23);
            this.btnShowFreqPower1.TabIndex = 6;
            this.btnShowFreqPower1.Text = "Freq Power";
            this.btnShowFreqPower1.Click += new System.EventHandler(this.btnShowFreqPower_Click);
            // 
            // statusBar1
            // 
            this.statusBar1.Location = new System.Drawing.Point(0, 416);
            this.statusBar1.Name = "statusBar1";
            this.statusBar1.Size = new System.Drawing.Size(733, 22);
            this.statusBar1.TabIndex = 8;
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // buttonTrain
            // 
            this.buttonTrain.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonTrain.Location = new System.Drawing.Point(634, 266);
            this.buttonTrain.Name = "buttonTrain";
            this.buttonTrain.Size = new System.Drawing.Size(75, 23);
            this.buttonTrain.TabIndex = 10;
            this.buttonTrain.Text = "&Train";
            this.buttonTrain.Click += new System.EventHandler(this.buttonTrain_Click);
            // 
            // btDelete
            // 
            this.btDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btDelete.Location = new System.Drawing.Point(634, 112);
            this.btDelete.Name = "btDelete";
            this.btDelete.Size = new System.Drawing.Size(75, 23);
            this.btDelete.TabIndex = 11;
            this.btDelete.Text = "Delete";
            this.btDelete.Click += new System.EventHandler(this.btDelete_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabTrain);
            this.tabControl1.Controls.Add(this.tabTest);
            this.tabControl1.Controls.Add(this.tabFreqPower);
            this.tabControl1.Location = new System.Drawing.Point(4, 48);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(717, 361);
            this.tabControl1.TabIndex = 12;
            // 
            // tabTrain
            // 
            this.tabTrain.Controls.Add(this.selAmpChannel1);
            this.tabTrain.Controls.Add(this.buttonFindTool);
            this.tabTrain.Controls.Add(this.label4);
            this.tabTrain.Controls.Add(this.buttonOpen);
            this.tabTrain.Controls.Add(this.buttonSaveTrainList);
            this.tabTrain.Controls.Add(this.listViewTrainFiles);
            this.tabTrain.Controls.Add(this.label1);
            this.tabTrain.Controls.Add(this.buttonTrain);
            this.tabTrain.Controls.Add(this.buttonAddTrainFile);
            this.tabTrain.Controls.Add(this.btDelete);
            this.tabTrain.Controls.Add(this.buttonAddTrainDir);
            this.tabTrain.Location = new System.Drawing.Point(4, 22);
            this.tabTrain.Name = "tabTrain";
            this.tabTrain.Size = new System.Drawing.Size(709, 335);
            this.tabTrain.TabIndex = 0;
            this.tabTrain.Text = "Train";
            // 
            // buttonFindTool
            // 
            this.buttonFindTool.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonFindTool.Location = new System.Drawing.Point(634, 237);
            this.buttonFindTool.Name = "buttonFindTool";
            this.buttonFindTool.Size = new System.Drawing.Size(75, 23);
            this.buttonFindTool.TabIndex = 16;
            this.buttonFindTool.Text = "&FindTool";
            this.buttonFindTool.Click += new System.EventHandler(this.buttonFindTool_Click);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(270, 297);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(251, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "Checked items are to be included for model training.";
            // 
            // buttonSaveTrainList
            // 
            this.buttonSaveTrainList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSaveTrainList.Location = new System.Drawing.Point(634, 141);
            this.buttonSaveTrainList.Name = "buttonSaveTrainList";
            this.buttonSaveTrainList.Size = new System.Drawing.Size(75, 23);
            this.buttonSaveTrainList.TabIndex = 14;
            this.buttonSaveTrainList.Text = "&Save";
            this.buttonSaveTrainList.Click += new System.EventHandler(this.buttonSaveTrainList_Click);
            // 
            // listViewTrainFiles
            // 
            this.listViewTrainFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewTrainFiles.CheckBoxes = true;
            this.listViewTrainFiles.FullRowSelect = true;
            this.listViewTrainFiles.HoverSelection = true;
            this.listViewTrainFiles.Location = new System.Drawing.Point(196, 35);
            this.listViewTrainFiles.MultiSelect = false;
            this.listViewTrainFiles.Name = "listViewTrainFiles";
            this.listViewTrainFiles.Size = new System.Drawing.Size(425, 254);
            this.listViewTrainFiles.TabIndex = 13;
            this.listViewTrainFiles.UseCompatibleStateImageBehavior = false;
            this.listViewTrainFiles.View = System.Windows.Forms.View.List;
            this.listViewTrainFiles.SelectedIndexChanged += new System.EventHandler(this.listViewTrainFiles_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(356, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(133, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "EEG files for model training";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonAddTrainDir
            // 
            this.buttonAddTrainDir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAddTrainDir.Location = new System.Drawing.Point(634, 80);
            this.buttonAddTrainDir.Name = "buttonAddTrainDir";
            this.buttonAddTrainDir.Size = new System.Drawing.Size(75, 23);
            this.buttonAddTrainDir.TabIndex = 2;
            this.buttonAddTrainDir.Text = "Add Directory";
            this.buttonAddTrainDir.Click += new System.EventHandler(this.buttonAddTrainDir_Click);
            // 
            // tabTest
            // 
            this.tabTest.Controls.Add(this.buttonFindTestFile);
            this.tabTest.Controls.Add(this.textBoxTestFile);
            this.tabTest.Controls.Add(this.label2);
            this.tabTest.Controls.Add(this.buttonStartTest);
            this.tabTest.Controls.Add(this.buttonStop);
            this.tabTest.Controls.Add(this.label5);
            this.tabTest.Controls.Add(this.textBoxNumSegments);
            this.tabTest.Location = new System.Drawing.Point(4, 22);
            this.tabTest.Name = "tabTest";
            this.tabTest.Size = new System.Drawing.Size(709, 335);
            this.tabTest.TabIndex = 3;
            this.tabTest.Text = "Test";
            // 
            // buttonFindTestFile
            // 
            this.buttonFindTestFile.Location = new System.Drawing.Point(560, 40);
            this.buttonFindTestFile.Name = "buttonFindTestFile";
            this.buttonFindTestFile.Size = new System.Drawing.Size(40, 23);
            this.buttonFindTestFile.TabIndex = 2;
            this.buttonFindTestFile.Text = "...";
            this.buttonFindTestFile.Click += new System.EventHandler(this.buttonFindTestFile_Click);
            // 
            // textBoxTestFile
            // 
            this.textBoxTestFile.Location = new System.Drawing.Point(104, 40);
            this.textBoxTestFile.Name = "textBoxTestFile";
            this.textBoxTestFile.ReadOnly = true;
            this.textBoxTestFile.Size = new System.Drawing.Size(440, 20);
            this.textBoxTestFile.TabIndex = 1;
            this.textBoxTestFile.TextChanged += new System.EventHandler(this.textBoxTestFile_TextChanged);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(24, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 23);
            this.label2.TabIndex = 0;
            this.label2.Text = "Testing file:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // buttonStartTest
            // 
            this.buttonStartTest.Location = new System.Drawing.Point(120, 128);
            this.buttonStartTest.Name = "buttonStartTest";
            this.buttonStartTest.Size = new System.Drawing.Size(128, 32);
            this.buttonStartTest.TabIndex = 2;
            this.buttonStartTest.Text = "Start Testing ...";
            this.buttonStartTest.Click += new System.EventHandler(this.buttonStartTest_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.Enabled = false;
            this.buttonStop.Location = new System.Drawing.Point(368, 128);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(128, 32);
            this.buttonStop.TabIndex = 2;
            this.buttonStop.Text = "Stop";
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(24, 80);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(120, 23);
            this.label5.TabIndex = 0;
            this.label5.Text = "Num Segments";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxNumSegments
            // 
            this.textBoxNumSegments.Location = new System.Drawing.Point(160, 80);
            this.textBoxNumSegments.Name = "textBoxNumSegments";
            this.textBoxNumSegments.ReadOnly = true;
            this.textBoxNumSegments.Size = new System.Drawing.Size(56, 20);
            this.textBoxNumSegments.TabIndex = 1;
            // 
            // tabFreqPower
            // 
            this.tabFreqPower.Controls.Add(this.buttonTrainBrowse);
            this.tabFreqPower.Controls.Add(this.textBoxTrainEEGPath);
            this.tabFreqPower.Controls.Add(this.label3);
            this.tabFreqPower.Controls.Add(this.toolBar1);
            this.tabFreqPower.Controls.Add(this.btnShowFreqPower1);
            this.tabFreqPower.Controls.Add(this.panel1);
            this.tabFreqPower.Controls.Add(this.buttonSaveJPeg);
            this.tabFreqPower.Location = new System.Drawing.Point(4, 22);
            this.tabFreqPower.Name = "tabFreqPower";
            this.tabFreqPower.Size = new System.Drawing.Size(709, 335);
            this.tabFreqPower.TabIndex = 2;
            this.tabFreqPower.Text = "FreqPower";
            // 
            // buttonTrainBrowse
            // 
            this.buttonTrainBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonTrainBrowse.Location = new System.Drawing.Point(621, 8);
            this.buttonTrainBrowse.Name = "buttonTrainBrowse";
            this.buttonTrainBrowse.Size = new System.Drawing.Size(75, 23);
            this.buttonTrainBrowse.TabIndex = 17;
            this.buttonTrainBrowse.Text = "Browse";
            this.buttonTrainBrowse.Click += new System.EventHandler(this.buttonTrainBrowse_Click);
            // 
            // textBoxTrainEEGPath
            // 
            this.textBoxTrainEEGPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxTrainEEGPath.Location = new System.Drawing.Point(88, 8);
            this.textBoxTrainEEGPath.Name = "textBoxTrainEEGPath";
            this.textBoxTrainEEGPath.Size = new System.Drawing.Size(517, 20);
            this.textBoxTrainEEGPath.TabIndex = 16;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(16, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 16);
            this.label3.TabIndex = 15;
            this.label3.Text = "EEG path:";
            // 
            // toolBar1
            // 
            this.toolBar1.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.toolBarReDraw,
            this.toolBarYIn,
            this.toolBarYOut,
            this.toolBarYInc,
            this.toolBarYDec,
            this.toolBarXIn,
            this.toolBarXOut});
            this.toolBar1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolBar1.DropDownArrows = true;
            this.toolBar1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolBar1.ImageList = this.imageList1;
            this.toolBar1.Location = new System.Drawing.Point(0, 307);
            this.toolBar1.Name = "toolBar1";
            this.toolBar1.ShowToolTips = true;
            this.toolBar1.Size = new System.Drawing.Size(709, 28);
            this.toolBar1.TabIndex = 10;
            this.toolBar1.TextAlign = System.Windows.Forms.ToolBarTextAlign.Right;
            this.toolBar1.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar1_ButtonClick);
            // 
            // toolBarReDraw
            // 
            this.toolBarReDraw.Name = "toolBarReDraw";
            this.toolBarReDraw.Text = "ReDraw";
            // 
            // toolBarYIn
            // 
            this.toolBarYIn.Name = "toolBarYIn";
            this.toolBarYIn.Text = "YZoomIn";
            // 
            // toolBarYOut
            // 
            this.toolBarYOut.Name = "toolBarYOut";
            this.toolBarYOut.Text = "YZoomOut";
            // 
            // toolBarYInc
            // 
            this.toolBarYInc.Name = "toolBarYInc";
            this.toolBarYInc.Text = "YInc";
            // 
            // toolBarYDec
            // 
            this.toolBarYDec.Name = "toolBarYDec";
            this.toolBarYDec.Text = "YDec";
            // 
            // toolBarXIn
            // 
            this.toolBarXIn.Name = "toolBarXIn";
            this.toolBarXIn.Text = "XZoomIn";
            // 
            // toolBarXOut
            // 
            this.toolBarXOut.Name = "toolBarXOut";
            this.toolBarXOut.Text = "XZoomOut";
            // 
            // buttonSaveJPeg
            // 
            this.buttonSaveJPeg.Location = new System.Drawing.Point(104, 40);
            this.buttonSaveJPeg.Name = "buttonSaveJPeg";
            this.buttonSaveJPeg.Size = new System.Drawing.Size(72, 23);
            this.buttonSaveJPeg.TabIndex = 6;
            this.buttonSaveJPeg.Text = "Save JPEG";
            this.buttonSaveJPeg.Click += new System.EventHandler(this.buttonSaveJPeg_Click);
            // 
            // toolBarMainWindow
            // 
            this.toolBarMainWindow.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.toolBarSysConfig,
            this.toolBarLoadConfig});
            this.toolBarMainWindow.DropDownArrows = true;
            this.toolBarMainWindow.Location = new System.Drawing.Point(0, 0);
            this.toolBarMainWindow.Name = "toolBarMainWindow";
            this.toolBarMainWindow.ShowToolTips = true;
            this.toolBarMainWindow.Size = new System.Drawing.Size(733, 42);
            this.toolBarMainWindow.TabIndex = 13;
            this.toolBarMainWindow.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBarMainWindow_ButtonClick);
            // 
            // toolBarSysConfig
            // 
            this.toolBarSysConfig.Name = "toolBarSysConfig";
            this.toolBarSysConfig.Text = "System Config";
            // 
            // toolBarLoadConfig
            // 
            this.toolBarLoadConfig.Name = "toolBarLoadConfig";
            this.toolBarLoadConfig.Text = "Load Config";
            // 
            // buttonOpen
            // 
            this.buttonOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOpen.Enabled = false;
            this.buttonOpen.Location = new System.Drawing.Point(634, 170);
            this.buttonOpen.Name = "buttonOpen";
            this.buttonOpen.Size = new System.Drawing.Size(75, 23);
            this.buttonOpen.TabIndex = 14;
            this.buttonOpen.Text = "&Open";
            this.buttonOpen.Click += new System.EventHandler(this.buttonOpen_Click);
            // 
            // selAmpChannel1
            // 
            this.selAmpChannel1.Location = new System.Drawing.Point(0, 32);
            this.selAmpChannel1.Name = "selAmpChannel1";
            this.selAmpChannel1.SelectedList = new string[0];
            this.selAmpChannel1.SelectedNum = 0;
            this.selAmpChannel1.SelectedString = "";
            this.selAmpChannel1.Size = new System.Drawing.Size(195, 257);
            this.selAmpChannel1.TabIndex = 14;
            // 
            // MITrainMTSModelForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(733, 438);
            this.Controls.Add(this.toolBarMainWindow);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.statusBar1);
            this.Name = "MITrainMTSModelForm";
            this.Text = "Motor Imagery Multi -Time Segment Model Training";
            this.Load += new System.EventHandler(this.MITrainMTSModelForm_Load);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.MITrainMTSModelForm_Closing);
            this.tabControl1.ResumeLayout(false);
            this.tabTrain.ResumeLayout(false);
            this.tabTrain.PerformLayout();
            this.tabTest.ResumeLayout(false);
            this.tabTest.PerformLayout();
            this.tabFreqPower.ResumeLayout(false);
            this.tabFreqPower.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void ButtonClose_Click(object sender, System.EventArgs e) {
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void panel1_Paint(object sender, System.Windows.Forms.PaintEventArgs e) {
			DrawEEGFrezPower(e.Graphics);
		}

		private void DrawEEGFrezPower(Graphics g) {
			g.Clear(Color.White);

			if (avg_epoch_power == null) return;

			Font f = new Font("Arial", 8);
			SolidBrush br = new SolidBrush(Color.Black);

			Pen p1 = new Pen(Color.Red, 1);
			Pen p2 = new Pen(Color.Blue, 2);
			Pen p3 = new Pen(Color.Black);
			Pen p4 = new Pen(Color.Gray);

			Pen[] pens = new Pen[avg_epoch_power.GetLength(0)];
			pens[0] = p1;
			pens[1] = p2;
			int ncode = cfg_stim_list.Length;
			if (ncode > 2) ncode = 2;

			int dl = dsp_freq_range.Length;
			int num_channel = dsp_chn_idx.Length;
			if (channel_reg == null) channel_reg = new Rectangle[num_channel];
			if (channel_sel == null) channel_sel = new bool[num_channel];

			num_column = (panel1.ClientSize.Width - MARGIN_X) /
				(MARGIN_X + dl * dsp_x_factor);
			if (num_column < 1) num_column = 1;

			panel1.AutoScrollMinSize = new Size(
				num_column * (dl * dsp_x_factor + MARGIN_X) + MARGIN_X,
				(DSP_Y + MARGIN_Y) * ((num_channel + num_column - 1) / num_column)
				+ MARGIN_Y);

			Point[] points = new Point[dl];

			// calculate x-scale
			// fx = scale start, ft = scale increase step
			int fx = cfg_freq_dstop - cfg_freq_dstart;
			int ft = 1;
			while (fx > 10) {
				fx /= 10;
				ft *= 10;
			}

			fx = ft * cfg_nFFT / amp_inf.sampling_rate * dsp_x_factor;
			while (fx < 20) {
				ft <<= 1;
				fx = ft * cfg_nFFT / amp_inf.sampling_rate * dsp_x_factor;
			}

			while (ft > 1 && fx > 40) {
				ft >>= 1;
				fx = ft * cfg_nFFT / amp_inf.sampling_rate * dsp_x_factor;
			}

			fx = (cfg_freq_dstart) / ft * ft;
			if (fx < cfg_freq_dstart) fx += ft;

			for (int ich = 0; ich < num_channel; ich++) {
				int row = ich / num_column;
				int col = ich % num_column;
				int ic = dsp_chn_idx[ich];

				int x = col * (MARGIN_X + dl * dsp_x_factor) + MARGIN_X + 
					panel1.AutoScrollPosition.X;
				int y = row * (MARGIN_Y + DSP_Y) + MARGIN_Y + 
					panel1.AutoScrollPosition.Y;
				g.DrawRectangle(p3, x, y, dl * dsp_x_factor, DSP_Y);

				// scale
				for (int fi = fx; fi <= cfg_freq_dstop; fi += ft) {
					int x1 = fi * cfg_nFFT / amp_inf.sampling_rate
						- dsp_start_bin;
					x1 *= dsp_x_factor;
					x1 += x;

					g.DrawLine(p3, x1, y + DSP_Y, x1, y + DSP_Y - 5);

					string mstr = fi.ToString();
					int mw = (int) g.MeasureString(mstr, f).Width;
					g.DrawString(mstr, f, br, x1 - mw / 2, y + DSP_Y);
				}

				SizeF ssz = g.MeasureString(channel_name[ic], f);
				int cw = (int) ssz.Width;
				int ch = (int) ssz.Height - 4;
				int cx = x + (dl - cw - ch - 2) / 2;

				// check box
				channel_reg[ich] = new Rectangle(cx, y + 2, ch, ch);
				g.DrawRectangle(p3, channel_reg[ich]);
				if (channel_sel[ich]) {
					g.DrawLine(p3, cx, y + ch + 2, cx + ch, y + 2);
					g.DrawLine(p3, cx, y + 2, cx + ch, y + ch + 2);
				}
				g.DrawString(channel_name[ic], f, br, cx + ch + 2, y);

				if (dl > 1 && max_power > min_power) {
					for (int icode = 0; icode < ncode; icode++) {
						for (int i = 0; i < dl; i++) {
							points[i].X = x + i * dsp_x_factor;
							points[i].Y = (int)(y + DSP_Y - dsp_y_factor *
								(avg_epoch_power[icode][ic, i + dsp_start_bin]
								- min_power) * DSP_Y / max_power);
						}
						g.DrawLines(pens[icode], points);
					}
				}
			}
		}

		private void panel1_Resize(object sender, System.EventArgs e) {
			if (avg_epoch_power != null) {
				int dl = dsp_freq_range.Length;
				int num_channel = avg_epoch_power[0].GetUpperBound(0) + 1;
				int col = (panel1.ClientSize.Width - MARGIN_X) /
					(MARGIN_X + dl * dsp_x_factor);
				if (col < 1) col = 1;
				if (col != num_column) {
					panel1.Invalidate();
				}
			}
		}

		private void panel1_Click(object sender, System.EventArgs e) {
			if (channel_reg == null) return;

			Point mp = panel1.PointToClient(Control.MousePosition);

			int dl = avg_epoch_power[0].GetUpperBound(1) + 1;
			for (int ic = 0; ic < channel_reg.Length; ic++) {
				if (channel_reg[ic].Contains(mp)) {
					channel_sel[ic] = !channel_sel[ic];
					panel1.Invalidate(channel_reg[ic]);
					// btnSelectChannel.Enabled = true;
					break;
				}
			}
		}

		private void btnSelectChannel_Click(object sender, System.EventArgs e) {
			StringBuilder sb = new StringBuilder();
			bool first = true;
			for (int i = 0; i < channel_sel.Length; i++ ) {
				if (channel_sel[i]) {
					if (!first) {
						sb.Append(",");
					} else {
						first = false;
					}
					sb.Append(channel_name[dsp_chn_idx[i]]);
				}
			}

			string sc = sb.ToString();
			Console.WriteLine("Selected channels: {0}", sc);
			cfg.SetConfigValue("EEG", "UsedChannels", sc);
			cfg.SaveFile();

			// btnSelectChannel.Enabled = false;
		}

		private int[] cfg_stim_list = new int[] {121, 122};
		private int cfg_proc_start = 500;
		private int cfg_proc_end = 2500;
		private int cfg_nFFT = 1024;
		private string cfg_hide_channels = null;

		private float[][,] avg_epoch_power; // nclass X fft_length
		private int[] label_num; // length: nclass
		private float max_power, min_power;

		private int cfg_freq_dstart = 5;
		private int cfg_freq_dstop = 25;
		private float[] dsp_freq_range;
		private int dsp_start_bin = 0;
		private int dsp_x_factor = 1;
		private int dsp_y_factor = 1;
		private int[] dsp_chn_idx = null;

		// Rraining parameters
		private string cfg_train_para = "[121,122] [0,1] [0.5,2.5]"; //1.0,2.0;1.5,3.5;2.0,4.0;3.5,4.5];

		private AmpInfo amp_inf;

		private void btnShowFreqPower_Click(object sender, System.EventArgs e) {
			string cntfn = textBoxTrainEEGPath.Text;
			if (cntfn == null || cntfn.Length == 0) return;

			EEGCntFile cntf = new EEGCntFile();
			if (!cntf.ReadCnt(cntfn)) return;

			amp_inf = cntf.Amp_Info;
			int[] sel_idx = amp_inf.valid_idx;
			if (sel_idx == null) return;
			int nch = sel_idx.Length;
			if (nch == 0) return;

			avg_epoch_power = new float[cfg_stim_list.Length][,];
			int nEpoch = 0;
			for (int icode = 0; icode < cfg_stim_list.Length; icode++) {
				nEpoch += cntf.GetNumberOfStimcode(cfg_stim_list[icode]);
				avg_epoch_power[icode] =
					new float[nch, cfg_nFFT/2 + 1];
			}

			int iEpoch = 0;
			int spl_start = cfg_proc_start * amp_inf.sampling_rate / 1000;
			int spl_end = cfg_proc_end * amp_inf.sampling_rate / 1000;
			int xlen = spl_end - spl_start + 1;
			float[,] ebuf = new float[nch, xlen];
			float[] xbuf = new float[cfg_nFFT];
			label_num = new int[cfg_stim_list.Length];

			if (cfg_nFFT < xlen) {
				Console.WriteLine("Warning: FFT data length too long!");
			}


			for (int istim = 0; ; istim++) {
				int ec = cntf.GetStimCodeByNo(istim);
				if (ec == 0) break;

				int icode = 0;
				for (; icode < cfg_stim_list.Length; icode++) {
					if (ec == cfg_stim_list[icode]) break;
				}
				if (icode == cfg_stim_list.Length) continue;

				cntf.GetEpochByStimNo(ebuf, istim, spl_start, spl_end, sel_idx);
				label_num[icode]++;

				// calculate FFT
				for (int ich = 0; ich < nch; ich++) {
					int xi = 0;
					for (; xi < xlen; xi++) xbuf[xi] = ebuf[ich, xi];
					for (; xi < cfg_nFFT; xi++) xbuf[xi] = 0;

					//Hamming window
					float a = (float)(2 * Math.PI / (xlen - 1));
					for(xi = 0; xi < xlen; xi++) {
						xbuf[xi] = (float)(xbuf[xi] * 
							(0.54 - 0.46 * Math.Cos(a * xi)));
					}

					RealFFT.realfft(xbuf);

					// calculate spectrum
					float fv = xbuf[0] * xbuf[0];
					avg_epoch_power[icode][ich, 0] += fv / label_num[icode];
					for (int fi = 1; fi <= cfg_nFFT/2; fi++) {
						fv = (xbuf[fi] * xbuf[fi] + 
							xbuf[cfg_nFFT - fi] * xbuf[cfg_nFFT - fi]);
						avg_epoch_power[icode][ich, fi] += fv;
					}
				}
				iEpoch++;
			}

			// average
			for (int icode = 0; icode < cfg_stim_list.Length; icode++) {
				for (int ich = 0; ich < nch; ich++) {
					for (int fi = 0; fi <= cfg_nFFT/2; fi++) {
						avg_epoch_power[icode][ich, fi] /= label_num[icode];
					}
				}
			}

			// done with all calculation
			channel_name = new string[nch];
			for (int ic = 0; ic < nch; ic++) {
				channel_name[ic] = amp_inf.chan_names[sel_idx[ic]];
			}

			SetDispChannels();
			CalPowerRange();
		}

		private void CalPowerRange() {
			if (avg_epoch_power == null) return;
			int nch = dsp_chn_idx.GetLength(0);

			// max_min
			int start_bin =
				(cfg_freq_dstart * cfg_nFFT / amp_inf.sampling_rate);
			if (start_bin < 0) start_bin = 0;

			int stop_bin = (int) 
				((float)cfg_freq_dstop * cfg_nFFT / amp_inf.sampling_rate + 0.5);
			if (stop_bin > cfg_nFFT / 2) stop_bin = cfg_nFFT / 2;

			if (start_bin > stop_bin) start_bin = stop_bin;
			if (stop_bin < start_bin) stop_bin = start_bin;

			dsp_freq_range = new float[stop_bin - start_bin + 1];

			max_power = min_power = avg_epoch_power[0][dsp_chn_idx[0], start_bin];
			for (int iCode = 0; iCode < cfg_stim_list.Length; iCode++) {
				for (int cid = 0; cid < nch; cid++) {
					int ci = dsp_chn_idx[cid];
					for (int fi = start_bin; fi <= stop_bin; fi++) {
						float fv = avg_epoch_power[iCode][ci, fi];
						if (max_power < fv) max_power = fv;
						else if (min_power > fv) min_power = fv;
						dsp_freq_range[fi - start_bin] = (float)fi * amp_inf.sampling_rate / cfg_nFFT;
					}
				}
			}
			dsp_start_bin = start_bin;

			statusBar1.Text = string.Format("stim_list = {0}, time-win=({1}-{2}), freq=({3}-{4}), max_power={5}, min_power={6}", 
				PROC_STIM_LIST, cfg_proc_start, cfg_proc_end, 
				cfg_freq_dstart, cfg_freq_dstop, max_power, min_power);
			panel1.Invalidate();
		}

		private string PROC_STIM_LIST {
			get {
				StringBuilder sb = new StringBuilder();
				int ncode = cfg_stim_list.Length;
				for (int icode = 0; icode < ncode; icode++) {
					if (icode > 0) sb.Append(',');
					sb.Append(cfg_stim_list[icode]);
				}
				return sb.ToString();
			}
		}

		private string train_mtmi = "Train_MTS_PATH";

		private void ResetParas() {
			// parameters for calculate power
			cfg.GetConfigValue(MIConstDef.ModelTrainingSetting, "Proc_Start_MS", ref cfg_proc_start);
            cfg.GetConfigValue(MIConstDef.ModelTrainingSetting, "Proc_End_MS", ref cfg_proc_end);

            string line = cfg.GetConfigValue(MIConstDef.ModelTrainingSetting, "Proc_Stim_List");
			if (line == null) {
                cfg.SetConfigValue(MIConstDef.ModelTrainingSetting, "Proc_Stim_List", PROC_STIM_LIST);
			} else {
				string[] alist = ResManager.SplitString(line, ",");
				int ncode = alist.Length;
				cfg_stim_list = new int[ncode];
				for (int icode = 0; icode < ncode; icode++)
					cfg_stim_list[icode] = int.Parse(alist[icode]);
			}

			// parameters for display
            cfg.GetConfigValue(MIConstDef.ModelTrainingSetting, "DISP_Freq_Start", ref cfg_freq_dstart);
            cfg.GetConfigValue(MIConstDef.ModelTrainingSetting, "DISP_Freq_Stop", ref cfg_freq_dstop);

            cfg.GetConfigValue(MIConstDef.ModelTrainingSetting, "DISP_Hide_Channels", ref cfg_hide_channels);
			SetDispChannels();

            line = cfg.GetConfigValue(MIConstDef.ModelTrainingSetting, "TrainCntPath");
			if (line != null) {
				textBoxTrainEEGPath.Text = line;
				toolTip1.SetToolTip(textBoxTrainEEGPath, line);
			}

			// parameters for model training
            cfg.GetConfigValue(MIConstDef.ModelTrainingSetting, MIConstDef.Train_MTS_Para, ref cfg_train_para);
            string str_channels = cfg.GetConfigValue(MIConstDef.ModelTrainingSetting, MIConstDef.Train_MTS_Channels);
            selAmpChannel1.ShowAmplifier();

            if (!string.IsNullOrEmpty(str_channels)) {
                int nch = str_channels.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).Length;
                selAmpChannel1.SelectedString = str_channels;
                if (selAmpChannel1.SelectedNum != nch) {
                    selAmpChannel1.SelectedString = MIConstDef.DefChannels;
                    if (selAmpChannel1.SelectedNum != nch) {
                        selAmpChannel1.SelectedNum = nch;
                    }
                }
            }

            textBoxTestFile.Text = cfg.GetConfigValue(MIConstDef.ModelTrainingSetting, "TestCntFile");

			cfg.SaveIfChanged();
		}

		private void SetDispChannels() {
			if (channel_name != null) {
				int nch = channel_name.Length;
				int[] dsp_idx = new int[nch];
				int ni = 0;
				for (int ich = 0; ich < nch; ich++) {
					if (cfg_hide_channels != null &&
						cfg_hide_channels.IndexOf(channel_name[ich]) >= 0) {
						continue;
					} else {
						dsp_idx[ni] = ich;
						ni++;
					}
				}
				dsp_chn_idx = new int[ni];
				Array.Copy(dsp_idx, dsp_chn_idx, ni);
			}
		}

		private void toolBar1_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e) {
			if (e.Button == toolBarReDraw) {
				ResetParas();
				CalPowerRange();
			} else if (e.Button == toolBarYIn) {
				if (dsp_y_factor < 0x1FFFFFFF) {
					dsp_y_factor <<= 1;
					panel1.Invalidate();
				}
			} else if (e.Button == toolBarYOut) {
				if (dsp_y_factor > 1) {
					dsp_y_factor >>= 1;
					panel1.Invalidate();
				}
			} else if (e.Button == toolBarYInc) {
				if (DSP_Y < 500) {
					DSP_Y <<= 1;
					panel1.Invalidate();
				}
			} else if (e.Button == toolBarYDec) {
				if (DSP_Y > 100) {
					DSP_Y >>= 1;
					panel1.Invalidate();
				}
			} else if (e.Button == toolBarXIn) {
				dsp_x_factor <<= 1;
				panel1.Invalidate();
			} else if (e.Button == toolBarXOut) {
				if (dsp_x_factor > 1) {
					dsp_x_factor >>= 1;
					panel1.Invalidate();
				}
			}
		}

        int[] _class_labels = null;

        public void SetMIClassLabels(params int[] tasks)
        {
            _class_labels = tasks;
        }

		private void buttonTrain_Click(object sender, System.EventArgs e) {
            BCIApplication.SaveTrainingFileLists(listViewTrainFiles);
            TrainModelFile();
        }

        /// <summary>
        /// Train Model File
        /// </summary>
        private void TrainModelFile()
        {
            // 2012.01.18: check amplifier
            Amplifier amp = AmpContainer.GetAmplifier(0);
            int nch = selAmpChannel1.SelectedNum;
            if (amp == null || nch <= 0) {
                MessageBox.Show("Amplifer/channel not selected!", "Error");
                return;
            }
            
            // get EEG file
            string key_wd = null;
            foreach (ListViewItem it in listViewTrainFiles.Items) {
                if (it.Checked) {
                    if (key_wd == null) {
                        key_wd = it.Text;
                    }
                    //break;

                    using (FileStream fs = File.OpenRead(it.Text)) {
                        BinaryReader br = new BinaryReader(fs);
                        if (br.ReadInt32() != amp.header.nchan) {
                            MessageBox.Show("Amplifer does not mach eeg data!", "Error");
                            return;
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(key_wd)) {
                MessageBox.Show("No training file!");
                return;
            }

            // get EEG key word
            string strdate = File.GetLastWriteTime(key_wd).ToString("yyyyMMdd");
            key_wd = Path.GetFileNameWithoutExtension(key_wd);
            int n = key_wd.IndexOf('_');
            if (n > 0) key_wd = key_wd.Substring(0, n);

            key_wd = key_wd + "_" + strdate;

            int nclasses = 0;
            if (_class_labels != null) nclasses = _class_labels.Length;

            if (nclasses < 2) {
                MessageBox.Show("Number of classes must be at least 2!");
                return;
            }

            int nmodel = nclasses;
            if (nmodel == 2) {
                nmodel = nclasses - 1; // one model for 2 classes
            }

			//check training program
			string str_train = BCIApplication.GetGamePath(train_mtmi);
			if (!File.Exists(str_train)) {
				str_train = BCIApplication.GetGamePath(train_mtmi, true);
				if (!File.Exists(str_train)) {
					Console.WriteLine("Cannot find training tool {0}.", str_train);
					return;
				}
			}

			Console.WriteLine("Using tool {0}", str_train);
			
			string m_dir = "Model";
			DirectoryInfo dinf1 = new DirectoryInfo(m_dir);
			if (!dinf1.Exists) dinf1.Create();


			string timestamp = BCIApplication.TimeStamp;

            StringBuilder sb;
            string model_pattern = key_wd + "_" + 
                Path.GetFileNameWithoutExtension(str_train) + timestamp + "_{0}.txt";

            bool trainDone = true;

            for (int imodel = 0; imodel < nmodel; imodel++) {
                string m_path = Path.Combine(m_dir, string.Format(model_pattern, imodel + 1));

                ProcessStartInfo pinf = new ProcessStartInfo();

                StreamWriter sw = new StreamWriter(MIConstDef.TrainingCfgFileName);
                sw.WriteLine("{0} {1}", MIConstDef.MultiTimeModel, m_path);
                sw.WriteLine("{0} {1}", MIConstDef.TrainingFileListName, BCICfg.TrainingFileName);

                sw.WriteLine("{0} [{1}] [{2}] [0.5,2.5]", MIConstDef.Train_MTS_Para, 
                    string.Join(",", _class_labels.Select(x=>x.ToString()).ToArray()),
                    string.Join(",", _class_labels.Select((x, idx) => imodel == idx? "0":"1").ToArray())
                );

                // new parameter: UsedChannelIdx
                sw.WriteLine("{0} [{1}]", MIConstDef.UsedChannelIdx,
                    string.Join(" ", selAmpChannel1.SelectedItemIndices.Select(x => x.ToString()).ToArray()));
                sw.Close();

                pinf.FileName = str_train;
                pinf.Arguments = Path.GetFullPath(MIConstDef.TrainingCfgFileName);
                pinf.WorkingDirectory = Path.GetDirectoryName(str_train);

                Console.WriteLine("Call cmd: {0} {1}", pinf.FileName, pinf.Arguments);

                StartProcForm pfm = new StartProcForm(pinf);

                // Manual
                pfm.ShowDialog();

                // save m_path into system.cfg;
                if (File.Exists(m_path)) {
                    // save log file
                    FileInfo fi = new FileInfo(m_path);
                    string fn_key = Path.GetFileNameWithoutExtension(m_path);
                    string fn_log = Path.Combine(m_dir, fn_key + ".log");
                    Console.WriteLine("Save Model Log file: {0}", fn_log);

                    File.Copy(MIConstDef.TrainingCfgFileName, fn_log);
                    File.AppendAllText(fn_log, pfm.OutputString);

                    DataFileManager.AddDataFiles(m_dir, fn_key);
                }
                else if (!string.IsNullOrEmpty(pfm.OutputString)) {
                    MessageBox.Show(pfm.OutputString);
                    trainDone = false;
                    break;
                }
            }

            if (trainDone) {
                sb = new StringBuilder();
                for (int iclass = 0; iclass < nclasses; iclass++) {
                    if (iclass > 0) sb.Append(',');
                    sb.Append(_class_labels[iclass]);
                }

                cfg.SetConfigValue(MIConstDef.MIProcess, MIConstDef.ClassLabels, sb.ToString());
                cfg.SetConfigValue(MIConstDef.MIProcess, MIConstDef.ModelPattern, model_pattern);
                cfg.SaveFile();

                using (StreamWriter sw = File.AppendText(MIConstDef.ModelList)) {
                    sw.WriteLine(model_pattern);
                }
            }
		}

		private void ButtonAddTrainFile_Click(object sender, System.EventArgs e) {
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.RestoreDirectory = true;
			dlg.Filter = "cnt File(*.cnt)|*.cnt|All files(*.*)|*.*";
			//dlg.InitialDirectory = BCIApplication.GetTrainDataDir(".");
			DialogResult rst = dlg.ShowDialog(this);

			if (rst == DialogResult.OK) {
				string fn = dlg.FileName;
				string pwd = Environment.CurrentDirectory;
				if (fn.StartsWith(pwd)) fn = fn.Substring(pwd.Length + 1);

				// Add training file list
				for (int i = 0; i < listViewTrainFiles.Items.Count; i++) {
					string fo = listViewTrainFiles.Items[i].Text;
					if (fo == fn) {
						MessageBox.Show("File already included!");
						return;
					}
				}
				listViewTrainFiles.Items.Add(fn).Checked = true;
				BCIApplication.SaveTrainingFileLists(listViewTrainFiles);
			}
		}


		private void MITrainMTSModelForm_Load(object sender, System.EventArgs e) {
			BCIApplication.LoadTraingFileLists(listViewTrainFiles);

            // temporally remove other two tabs
            tabControl1.TabPages.RemoveAt(1); // test
            tabControl1.TabPages.RemoveAt(1); // channel freq power
		}

		private void btDelete_Click(object sender, System.EventArgs e) {
            int n = listViewTrainFiles.SelectedIndices.Count;
            if (n > 0) {
                int sel = listViewTrainFiles.SelectedIndices[0];
                listViewTrainFiles.Items.RemoveAt(sel);
                BCIApplication.SaveTrainingFileLists(listViewTrainFiles);
            }
		}

		private void buttonTrainBrowse_Click(object sender, System.EventArgs e) {
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.RestoreDirectory = true;
			dlg.FileName = textBoxTrainEEGPath.Text;
			dlg.Filter = "EEG cnt file(*.cnt)|*.cnt|All Files(*.*)|*.*";
			dlg.FilterIndex = 0;
			if (dlg.ShowDialog() == DialogResult.OK) {
				string fn = dlg.FileName;
				if (fn.StartsWith(Environment.CurrentDirectory))
					fn = fn.Substring(Environment.CurrentDirectory.Length + 1);
				textBoxTrainEEGPath.Text = fn;
				toolTip1.SetToolTip(textBoxTrainEEGPath, fn);
                cfg.SetConfigValue(MIConstDef.ModelTrainingSetting, "TrainCntPath", fn);
				cfg.SaveFile();
			}
		}

        private void SaveTrainingCfg()
        {
            // save training file list
            BCIApplication.SaveTrainingFileLists(listViewTrainFiles);

            // save selchannels;
            cfg.SetConfigValue(MIConstDef.ModelTrainingSetting, "Train_MTS_Channels", selAmpChannel1.SelectedString);
            cfg.SaveFile();
        }

		private void buttonSaveTrainList_Click(object sender, System.EventArgs e) {
            SaveTrainingCfg();
		}

		private void MITrainMTSModelForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveTrainingCfg();
		}

		private void toolBarMainWindow_ButtonClick(object sender,
			System.Windows.Forms.ToolBarButtonClickEventArgs e) {
			if (e.Button == toolBarSysConfig) {
				if (cfg.ShowDialog()) {
					ResetParas();
					panel1.Invalidate();
				}
			} else if (e.Button == toolBarLoadConfig) {
				OpenFileDialog fd = new OpenFileDialog();
				fd.RestoreDirectory = true;
				fd.FileName = "Config\\System.cfg";

				if (fd.ShowDialog() == DialogResult.OK) {
					string cf = fd.FileName;
					Console.WriteLine("Load configuration file {0}.", cf);
					cfg.LoadFile(cf, false);
					ResetParas();
					cfg.SaveFile();
				}
			}
		}

		private void buttonSaveJPeg_Click(object sender, System.EventArgs e) {
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.RestoreDirectory = true;
			dlg.Filter = "JPEG file (*.jpg)|*.jpg;*.jpeg";

			if (dlg.ShowDialog() != DialogResult.OK) return;

			Bitmap bmp = new Bitmap(panel1.ClientRectangle.Width, panel1.ClientRectangle.Height);
			Graphics g = Graphics.FromImage(bmp);
			DrawEEGFrezPower(g);
			g.Dispose();


			bmp.Save(dlg.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
			bmp.Dispose();
		}

		private void buttonFindTool_Click(object sender, System.EventArgs e) {
			BCIApplication.GetGamePath(train_mtmi, true);
		}

		private void buttonFindTestFile_Click(object sender, System.EventArgs e) {
			OpenFileDialog fdlg = new OpenFileDialog();
			fdlg.RestoreDirectory = true;
			fdlg.FileName = textBoxTestFile.Text;
			fdlg.InitialDirectory = cfg.GetConfigValue("EEG", "TestDataDir");
			if (fdlg.InitialDirectory == "") {
				fdlg.InitialDirectory = Environment.CurrentDirectory;
			}
			fdlg.Filter = "EEG cnt file(*.cnt)|*.cnt|All Files|(*.*)";
			fdlg.Multiselect = false;
			if (fdlg.ShowDialog(this) == DialogResult.OK) {
				textBoxTestFile.Text = BCIApplication.GetRelativeUserPath(fdlg.FileName);
                cfg.SetConfigValue(MIConstDef.ModelTrainingSetting, "TestCntFile", textBoxTestFile.Text);
			} else {
				textBoxTestFile.Text = null;
			}
		}

		ManualResetEvent evt_StopTest = new ManualResetEvent(false);

		private void buttonStartTest_Click(object sender, System.EventArgs e) {
			buttonStop.Enabled = true;
			buttonStartTest.Enabled = false;

			Thread thd = new Thread(new ThreadStart(RunTest));
			thd.Start();
		}

		private void SetNumJ() {
			if (textBoxTestFile.Text != "") {
				EEGCntFile cntf = new EEGCntFile();
				num_j = 1;
				if (cntf.ReadCnt(textBoxTestFile.Text)) {
					EEGCntFile.StimCodeList clist = cntf.CodeList;
					int nc = -1;
					int nl = 0;
					int cl = clist.Count;
					int[] proc_codes = new int[] {121, 122};
					for (int i = 0; i < clist.Count; i++) {
						int ci = clist[i];
						int ti = Array.IndexOf(proc_codes, ci);

						if (nc == ci) {
							nl++;
						} else {
							if (cl > nl && nl > 0) cl = nl;
							nc = -1;
							nl = 0;
							if (ti >= 0) {
								nc = ci;
								nl = 1;
							}
						}
					}
					if (cl < clist.Count) {
						num_j = cl;
					}
				}
			}
			textBoxNumSegments.Text = num_j.ToString();
		}

		private delegate void recv_stim_score(IntPtr score_ptr);

		private int num_all = 0;
		private int num_true = 0;

		private int num_c = 0;
		private int num_j = 1; // every num_j, count 1

		private void RecvStimScore(IntPtr score_ptr) {
			if (num_c % num_j == 0) {
				float[] score_data = new float[3];
				Marshal.Copy(score_ptr, score_data, 0, 3);
				int code = BitConverter.ToInt32(BitConverter.GetBytes(score_data[2]), 0);
				num_all++;
				bool ok = false;
				if (code == 121 && score_data[0] > 0.5 || code == 122 && score_data[0] < 0.5) {
					num_true++;
					ok = true;
				}
				Console.WriteLine("Stim: {0}, Score: {1}, result= {2}",
					code, score_data[0], ok);
			}
			num_c++;
		}

		private void RunTest() {
			evt_StopTest.Reset();
			num_c = num_all = num_true = 0;

			// backup old amplifer config
            //string sv_device = rm.GetConfigValue("Amplifier", "Device");
            //string sv_cntfn = rm.GetConfigValue("Simulator", "InputFileName");

            //EEGAmplifier amp = EEGAmplifier.CreateEEGAmplifier(false);
            //amp.Stop();

            //rm.SetConfigValue("Amplifier", "Device", "Simulator");
            //rm.SetConfigValue("Simulator", "InputFileName",
            //    BCIApplication.GetRelativePath(Path.Combine(Environment.CurrentDirectory, textBoxTestFile.Text), BCIApplication.RootPath));
            //rm.SaveFile();

            //amp.ReCreate();
            //amp.Start();

			try {
				ExeTest();
			} catch (Exception ex) {
				Console.WriteLine("Exception in ExeTest: {0}", ex);
			}

			// restore amplifer config
            //amp.Stop();
            //rm.SetConfigValue("Amplifier", "Device", sv_device);
            //rm.SetConfigValue("Simulator", "InputFileName", sv_cntfn);
            //rm.SaveFile();
            //amp.ReCreate();

			buttonStartTest.Enabled = true;
			buttonStop.Enabled = false;
		}

		private void ExeTest() {
			cfg.SetConfigValue("EEG", "TestDataDir", "Data_ModelTest");
			string tpath = TestDirSpecForm.GetProcPath(cfg);
			if (tpath == null) {
				return;
			}

            //proc.SetCallBack(new recv_stim_score(RecvStimScore));

			var cfile = ConsoleCapture.StartConsoleFile(Path.Combine(tpath, "ModelTest-" + BCIApplication.TimeStamp + ".log"));
			Console.WriteLine("Testing cnt file = {0}", textBoxTestFile.Text);

			while (!evt_StopTest.WaitOne(5, false)) {
                //ssender.SendCode(1);
			}

			Console.WriteLine("All trials:{0}, correct: {1}, acc={2}%", num_all, num_true,
				num_true * 100.0 / num_all);

			cfile.EndLogFile();

			MessageBox.Show("Testing Finished.");
		}

		private void buttonStop_Click(object sender, System.EventArgs e) {
			evt_StopTest.Set();
		}

		private void textBoxTestFile_TextChanged(object sender, System.EventArgs e) {
			SetNumJ();
		}

		private void buttonAddTrainDir_Click(object sender, System.EventArgs e) {
			string cdir = BCIApplication.BrowseForDirectory("."); //BCIApplication.GetTrainDataDir("."));
			if (cdir == null) return;

			string[] cnt_all = Directory.GetFiles(cdir, "*.cnt", SearchOption.AllDirectories);
			if (cnt_all == null || cnt_all.Length == 0 ||
				MessageBox.Show(this, string.Format("{0}: {1} files found. Add?", cdir, cnt_all.Length),
				"Training files", MessageBoxButtons.OKCancel) != DialogResult.OK)
			{
				return;
			}

			string pwd = Environment.CurrentDirectory;
			int nadded = 0;
			foreach (string fni in cnt_all) {
				string fn = fni;
				if (fn.StartsWith(pwd)) fn = fn.Substring(pwd.Length + 1);

				// Add training file list
				bool found = false;
				for (int i = 0; i < listViewTrainFiles.Items.Count; i++) {
					string fo = listViewTrainFiles.Items[i].Text;
					if (fo == fn) {
						MessageBox.Show(this, fn + ": File already included!");
						found = true;
						break;
					}
				}
				if (!found) {
					listViewTrainFiles.Items.Add(fn).Checked = true;
					nadded++;
				}
			}
			if (nadded > 0) BCIApplication.SaveTrainingFileLists(listViewTrainFiles);
		}

        private void listViewTrainFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonOpen.Enabled = listViewTrainFiles.SelectedItems.Count == 1;
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            if (listViewTrainFiles.SelectedItems.Count != 1) return;
            string cnt_fn = Path.Combine(BCIApplication.UserPath, listViewTrainFiles.SelectedItems[0].Text);
            if (!File.Exists(cnt_fn)) {
                MessageBox.Show("File not exists!");
                return;
            }

            Process.Start(cnt_fn);//.WaitForExit();
        }
	}
}
