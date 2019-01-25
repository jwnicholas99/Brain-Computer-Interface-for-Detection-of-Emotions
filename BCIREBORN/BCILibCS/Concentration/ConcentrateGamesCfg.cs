using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;

using BCILib.App;
using BCILib.Amp;
using BCILib.Util;

namespace BCILib.Concentration
{
	/// <summary>
	/// Summary description for GamesCfg.
	/// </summary>
	internal class ConcentrateGamesCfg : System.Windows.Forms.UserControl {
		private System.Windows.Forms.Button buttonMosaicSaveCfg;
        private System.Windows.Forms.Label label18;
		private System.Windows.Forms.Label label6;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.GroupBox groupConfig;
        private System.Windows.Forms.GroupBox groupGames;
        private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox textBias;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textGain;
        private System.Windows.Forms.TextBox textScore;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textSpeed;
        private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button buttonSetModel2;
		private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBoxModelFile2;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.TextBox textBoxUsedChannel1;
		private System.Windows.Forms.TextBox textBoxModelFile1;
		private System.Windows.Forms.Button buttonChannel1;
        private System.Windows.Forms.Button buttonClearPlayer2;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Button buttonGainUp;

		//private ConcentrationFeedback fbForm;

		private string player1 = null;
		private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.RadioButton radioCalibration;
        private NumericUpDown numericScoreInterval;
        private NumericUpDown numericSmothFactor;
        private TextBox textBoxUsedChannel2;
        private TextBox textScore2;
        private Button buttonChannel2;
        private Label labelUsedChannel2;
        private TextBox textSpeed2;
        private Label label9;
        private Button buttonGainDown;
        private Button buttonBiasDown;
        private Button buttonBiasUp;
        private ComboBox comboAmplifier;
        private Label label7;
        private ComboBox comboAmplifier2;
        private Label label3;
        private TextBox textNumChannel;
        private TextBox textNumChannel2;
        private Button buttonFind;
        private ComboBox comboGameList;
        private RadioButton radioSelFromList;
        private Button buttonStopGame;
        private Button buttonStartGame;
		private string player2 = null;

		public ConcentrateGamesCfg() {
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
			// LoadConfig();

            //fbForm = new ConcentrationFeedback();
            //fbForm.Hide();

            //fbForm.VisibleChanged += new EventHandler(fbForm_VisibleChanged);
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConcentrateGamesCfg));
            this.groupConfig = new System.Windows.Forms.GroupBox();
            this.numericScoreInterval = new System.Windows.Forms.NumericUpDown();
            this.numericSmothFactor = new System.Windows.Forms.NumericUpDown();
            this.label18 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.buttonMosaicSaveCfg = new System.Windows.Forms.Button();
            this.groupGames = new System.Windows.Forms.GroupBox();
            this.radioCalibration = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textNumChannel = new System.Windows.Forms.TextBox();
            this.comboAmplifier = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textScore = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textSpeed = new System.Windows.Forms.TextBox();
            this.textBoxUsedChannel1 = new System.Windows.Forms.TextBox();
            this.textBoxModelFile1 = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.buttonChannel1 = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.buttonGainUp = new System.Windows.Forms.Button();
            this.textBias = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textGain = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textNumChannel2 = new System.Windows.Forms.TextBox();
            this.comboAmplifier2 = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxUsedChannel2 = new System.Windows.Forms.TextBox();
            this.textScore2 = new System.Windows.Forms.TextBox();
            this.buttonChannel2 = new System.Windows.Forms.Button();
            this.labelUsedChannel2 = new System.Windows.Forms.Label();
            this.buttonSetModel2 = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.textBoxModelFile2 = new System.Windows.Forms.TextBox();
            this.buttonClearPlayer2 = new System.Windows.Forms.Button();
            this.textSpeed2 = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.buttonGainDown = new System.Windows.Forms.Button();
            this.buttonBiasDown = new System.Windows.Forms.Button();
            this.buttonBiasUp = new System.Windows.Forms.Button();
            this.buttonFind = new System.Windows.Forms.Button();
            this.comboGameList = new System.Windows.Forms.ComboBox();
            this.radioSelFromList = new System.Windows.Forms.RadioButton();
            this.buttonStopGame = new System.Windows.Forms.Button();
            this.buttonStartGame = new System.Windows.Forms.Button();
            this.groupConfig.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericScoreInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericSmothFactor)).BeginInit();
            this.groupGames.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupConfig
            // 
            this.groupConfig.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.groupConfig.Controls.Add(this.numericScoreInterval);
            this.groupConfig.Controls.Add(this.numericSmothFactor);
            this.groupConfig.Controls.Add(this.label18);
            this.groupConfig.Controls.Add(this.label6);
            this.groupConfig.Location = new System.Drawing.Point(0, 8);
            this.groupConfig.Name = "groupConfig";
            this.groupConfig.Size = new System.Drawing.Size(208, 90);
            this.groupConfig.TabIndex = 13;
            this.groupConfig.TabStop = false;
            this.groupConfig.Text = "Configuration";
            // 
            // numericScoreInterval
            // 
            this.numericScoreInterval.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numericScoreInterval.Location = new System.Drawing.Point(118, 21);
            this.numericScoreInterval.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericScoreInterval.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numericScoreInterval.Name = "numericScoreInterval";
            this.numericScoreInterval.Size = new System.Drawing.Size(62, 20);
            this.numericScoreInterval.TabIndex = 5;
            this.numericScoreInterval.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericScoreInterval.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
            // 
            // numericSmothFactor
            // 
            this.numericSmothFactor.Location = new System.Drawing.Point(118, 48);
            this.numericSmothFactor.Name = "numericSmothFactor";
            this.numericSmothFactor.Size = new System.Drawing.Size(62, 20);
            this.numericSmothFactor.TabIndex = 5;
            this.numericSmothFactor.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericSmothFactor.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // label18
            // 
            this.label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(8, 16);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(104, 26);
            this.label18.TabIndex = 2;
            this.label18.Text = "Interval (ms)";
            this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(8, 48);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(104, 26);
            this.label6.TabIndex = 2;
            this.label6.Text = "Smooth Number";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // buttonMosaicSaveCfg
            // 
            this.buttonMosaicSaveCfg.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonMosaicSaveCfg.Location = new System.Drawing.Point(456, 259);
            this.buttonMosaicSaveCfg.Name = "buttonMosaicSaveCfg";
            this.buttonMosaicSaveCfg.Size = new System.Drawing.Size(88, 40);
            this.buttonMosaicSaveCfg.TabIndex = 0;
            this.buttonMosaicSaveCfg.Text = "Save Config";
            this.buttonMosaicSaveCfg.Click += new System.EventHandler(this.buttonMosaicSaveCfg_Click);
            // 
            // groupGames
            // 
            this.groupGames.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.groupGames.Controls.Add(this.buttonFind);
            this.groupGames.Controls.Add(this.comboGameList);
            this.groupGames.Controls.Add(this.radioCalibration);
            this.groupGames.Controls.Add(this.radioSelFromList);
            this.groupGames.Location = new System.Drawing.Point(224, 8);
            this.groupGames.Name = "groupGames";
            this.groupGames.Size = new System.Drawing.Size(320, 90);
            this.groupGames.TabIndex = 14;
            this.groupGames.TabStop = false;
            this.groupGames.Text = "Games";
            // 
            // radioCalibration
            // 
            this.radioCalibration.AutoSize = true;
            this.radioCalibration.Location = new System.Drawing.Point(6, 19);
            this.radioCalibration.Name = "radioCalibration";
            this.radioCalibration.Size = new System.Drawing.Size(74, 17);
            this.radioCalibration.TabIndex = 0;
            this.radioCalibration.Text = "Calibration";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.textNumChannel);
            this.groupBox1.Controls.Add(this.comboAmplifier);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.textScore);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.textSpeed);
            this.groupBox1.Controls.Add(this.textBoxUsedChannel1);
            this.groupBox1.Controls.Add(this.textBoxModelFile1);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.buttonChannel1);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Location = new System.Drawing.Point(0, 104);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(208, 244);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Player1";
            // 
            // textNumChannel
            // 
            this.textNumChannel.Location = new System.Drawing.Point(97, 139);
            this.textNumChannel.Name = "textNumChannel";
            this.textNumChannel.ReadOnly = true;
            this.textNumChannel.Size = new System.Drawing.Size(38, 20);
            this.textNumChannel.TabIndex = 16;
            // 
            // comboAmplifier
            // 
            this.comboAmplifier.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboAmplifier.FormattingEnabled = true;
            this.comboAmplifier.Location = new System.Drawing.Point(13, 105);
            this.comboAmplifier.Name = "comboAmplifier";
            this.comboAmplifier.Size = new System.Drawing.Size(187, 21);
            this.comboAmplifier.TabIndex = 15;
            this.comboAmplifier.SelectedIndexChanged += new System.EventHandler(this.comboAmplifier_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 80);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(49, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Amplifier:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textScore
            // 
            this.textScore.ForeColor = System.Drawing.Color.White;
            this.textScore.Location = new System.Drawing.Point(141, 202);
            this.textScore.Name = "textScore";
            this.textScore.ReadOnly = true;
            this.textScore.Size = new System.Drawing.Size(59, 20);
            this.textScore.TabIndex = 0;
            this.textScore.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(10, 202);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 20);
            this.label4.TabIndex = 1;
            this.label4.Text = "Speed (p):";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textSpeed
            // 
            this.textSpeed.ForeColor = System.Drawing.Color.White;
            this.textSpeed.Location = new System.Drawing.Point(80, 202);
            this.textSpeed.Name = "textSpeed";
            this.textSpeed.ReadOnly = true;
            this.textSpeed.Size = new System.Drawing.Size(59, 20);
            this.textSpeed.TabIndex = 0;
            this.textSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBoxUsedChannel1
            // 
            this.textBoxUsedChannel1.ForeColor = System.Drawing.Color.White;
            this.textBoxUsedChannel1.Location = new System.Drawing.Point(10, 166);
            this.textBoxUsedChannel1.Name = "textBoxUsedChannel1";
            this.textBoxUsedChannel1.ReadOnly = true;
            this.textBoxUsedChannel1.Size = new System.Drawing.Size(187, 20);
            this.textBoxUsedChannel1.TabIndex = 0;
            this.textBoxUsedChannel1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBoxModelFile1
            // 
            this.textBoxModelFile1.ForeColor = System.Drawing.Color.White;
            this.textBoxModelFile1.Location = new System.Drawing.Point(72, 48);
            this.textBoxModelFile1.Name = "textBoxModelFile1";
            this.textBoxModelFile1.ReadOnly = true;
            this.textBoxModelFile1.Size = new System.Drawing.Size(130, 20);
            this.textBoxModelFile1.TabIndex = 0;
            this.textBoxModelFile1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBoxModelFile1.TextChanged += new System.EventHandler(this.textBoxModelFile1_TextChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(10, 142);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(79, 13);
            this.label12.TabIndex = 1;
            this.label12.Text = "Used Channels";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonChannel1
            // 
            this.buttonChannel1.Enabled = false;
            this.buttonChannel1.Location = new System.Drawing.Point(141, 137);
            this.buttonChannel1.Name = "buttonChannel1";
            this.buttonChannel1.Size = new System.Drawing.Size(56, 23);
            this.buttonChannel1.TabIndex = 3;
            this.buttonChannel1.Text = "Change...";
            this.buttonChannel1.Click += new System.EventHandler(this.buttonChannel1_Click);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(7, 51);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(58, 13);
            this.label13.TabIndex = 1;
            this.label13.Text = "Model File:";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "");
            this.imageList1.Images.SetKeyName(1, "");
            // 
            // buttonGainUp
            // 
            this.buttonGainUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonGainUp.ForeColor = System.Drawing.Color.Transparent;
            this.buttonGainUp.ImageIndex = 1;
            this.buttonGainUp.ImageList = this.imageList1;
            this.buttonGainUp.Location = new System.Drawing.Point(353, 364);
            this.buttonGainUp.Name = "buttonGainUp";
            this.buttonGainUp.Size = new System.Drawing.Size(31, 20);
            this.buttonGainUp.TabIndex = 5;
            this.buttonGainUp.Click += new System.EventHandler(this.buttonGainUp_Click);
            // 
            // textBias
            // 
            this.textBias.ForeColor = System.Drawing.Color.White;
            this.textBias.Location = new System.Drawing.Point(141, 364);
            this.textBias.Name = "textBias";
            this.textBias.ReadOnly = true;
            this.textBias.Size = new System.Drawing.Size(51, 20);
            this.textBias.TabIndex = 0;
            this.textBias.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(61, 364);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Bias (b):";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(227, 367);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Gain (g):";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textGain
            // 
            this.textGain.ForeColor = System.Drawing.Color.White;
            this.textGain.Location = new System.Drawing.Point(313, 364);
            this.textGain.Name = "textGain";
            this.textGain.ReadOnly = true;
            this.textGain.Size = new System.Drawing.Size(48, 20);
            this.textGain.TabIndex = 0;
            this.textGain.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox2.Controls.Add(this.textNumChannel2);
            this.groupBox2.Controls.Add(this.comboAmplifier2);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.textBoxUsedChannel2);
            this.groupBox2.Controls.Add(this.textScore2);
            this.groupBox2.Controls.Add(this.buttonChannel2);
            this.groupBox2.Controls.Add(this.labelUsedChannel2);
            this.groupBox2.Controls.Add(this.buttonSetModel2);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.textBoxModelFile2);
            this.groupBox2.Controls.Add(this.buttonClearPlayer2);
            this.groupBox2.Controls.Add(this.textSpeed2);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Location = new System.Drawing.Point(224, 104);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(208, 244);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Player2";
            // 
            // textNumChannel2
            // 
            this.textNumChannel2.Location = new System.Drawing.Point(92, 139);
            this.textNumChannel2.Name = "textNumChannel2";
            this.textNumChannel2.ReadOnly = true;
            this.textNumChannel2.Size = new System.Drawing.Size(38, 20);
            this.textNumChannel2.TabIndex = 16;
            // 
            // comboAmplifier2
            // 
            this.comboAmplifier2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboAmplifier2.FormattingEnabled = true;
            this.comboAmplifier2.Location = new System.Drawing.Point(11, 105);
            this.comboAmplifier2.Name = "comboAmplifier2";
            this.comboAmplifier2.Size = new System.Drawing.Size(184, 21);
            this.comboAmplifier2.TabIndex = 15;
            this.comboAmplifier2.SelectedIndexChanged += new System.EventHandler(this.comboAmplifier_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Amplifier:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxUsedChannel2
            // 
            this.textBoxUsedChannel2.ForeColor = System.Drawing.Color.White;
            this.textBoxUsedChannel2.Location = new System.Drawing.Point(11, 166);
            this.textBoxUsedChannel2.Name = "textBoxUsedChannel2";
            this.textBoxUsedChannel2.ReadOnly = true;
            this.textBoxUsedChannel2.Size = new System.Drawing.Size(184, 20);
            this.textBoxUsedChannel2.TabIndex = 0;
            this.textBoxUsedChannel2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textScore2
            // 
            this.textScore2.ForeColor = System.Drawing.Color.White;
            this.textScore2.Location = new System.Drawing.Point(143, 202);
            this.textScore2.Name = "textScore2";
            this.textScore2.ReadOnly = true;
            this.textScore2.Size = new System.Drawing.Size(59, 20);
            this.textScore2.TabIndex = 0;
            this.textScore2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // buttonChannel2
            // 
            this.buttonChannel2.Enabled = false;
            this.buttonChannel2.Location = new System.Drawing.Point(136, 137);
            this.buttonChannel2.Name = "buttonChannel2";
            this.buttonChannel2.Size = new System.Drawing.Size(56, 23);
            this.buttonChannel2.TabIndex = 3;
            this.buttonChannel2.Text = "Change...";
            this.buttonChannel2.Click += new System.EventHandler(this.buttonChannel2_Click);
            // 
            // labelUsedChannel2
            // 
            this.labelUsedChannel2.AutoSize = true;
            this.labelUsedChannel2.Location = new System.Drawing.Point(8, 142);
            this.labelUsedChannel2.Name = "labelUsedChannel2";
            this.labelUsedChannel2.Size = new System.Drawing.Size(79, 13);
            this.labelUsedChannel2.TabIndex = 1;
            this.labelUsedChannel2.Text = "Used Channels";
            this.labelUsedChannel2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonSetModel2
            // 
            this.buttonSetModel2.Location = new System.Drawing.Point(11, 18);
            this.buttonSetModel2.Name = "buttonSetModel2";
            this.buttonSetModel2.Size = new System.Drawing.Size(58, 23);
            this.buttonSetModel2.TabIndex = 3;
            this.buttonSetModel2.Text = "Set...";
            this.buttonSetModel2.Click += new System.EventHandler(this.buttonSetModel2_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(8, 51);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(58, 13);
            this.label10.TabIndex = 1;
            this.label10.Text = "Model File:";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textBoxModelFile2
            // 
            this.textBoxModelFile2.ForeColor = System.Drawing.Color.White;
            this.textBoxModelFile2.Location = new System.Drawing.Point(72, 48);
            this.textBoxModelFile2.Name = "textBoxModelFile2";
            this.textBoxModelFile2.ReadOnly = true;
            this.textBoxModelFile2.Size = new System.Drawing.Size(128, 20);
            this.textBoxModelFile2.TabIndex = 0;
            this.textBoxModelFile2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBoxModelFile2.TextChanged += new System.EventHandler(this.textBoxModelFile2_TextChanged);
            // 
            // buttonClearPlayer2
            // 
            this.buttonClearPlayer2.Location = new System.Drawing.Point(136, 18);
            this.buttonClearPlayer2.Name = "buttonClearPlayer2";
            this.buttonClearPlayer2.Size = new System.Drawing.Size(40, 23);
            this.buttonClearPlayer2.TabIndex = 3;
            this.buttonClearPlayer2.Text = "Clear";
            this.buttonClearPlayer2.Click += new System.EventHandler(this.buttonClearPlayer2_Click);
            // 
            // textSpeed2
            // 
            this.textSpeed2.ForeColor = System.Drawing.Color.White;
            this.textSpeed2.Location = new System.Drawing.Point(82, 202);
            this.textSpeed2.Name = "textSpeed2";
            this.textSpeed2.ReadOnly = true;
            this.textSpeed2.Size = new System.Drawing.Size(59, 20);
            this.textSpeed2.TabIndex = 0;
            this.textSpeed2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(16, 201);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(64, 20);
            this.label9.TabIndex = 1;
            this.label9.Text = "Speed (p):";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonGainDown
            // 
            this.buttonGainDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonGainDown.ForeColor = System.Drawing.Color.Transparent;
            this.buttonGainDown.ImageIndex = 0;
            this.buttonGainDown.ImageList = this.imageList1;
            this.buttonGainDown.Location = new System.Drawing.Point(289, 364);
            this.buttonGainDown.Name = "buttonGainDown";
            this.buttonGainDown.Size = new System.Drawing.Size(31, 20);
            this.buttonGainDown.TabIndex = 6;
            this.buttonGainDown.Click += new System.EventHandler(this.buttonGainDown_Click);
            // 
            // buttonBiasDown
            // 
            this.buttonBiasDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonBiasDown.ForeColor = System.Drawing.Color.Transparent;
            this.buttonBiasDown.ImageIndex = 0;
            this.buttonBiasDown.ImageList = this.imageList1;
            this.buttonBiasDown.Location = new System.Drawing.Point(117, 364);
            this.buttonBiasDown.Name = "buttonBiasDown";
            this.buttonBiasDown.Size = new System.Drawing.Size(24, 20);
            this.buttonBiasDown.TabIndex = 6;
            this.buttonBiasDown.Click += new System.EventHandler(this.buttonBiasDown_Click);
            // 
            // buttonBiasUp
            // 
            this.buttonBiasUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonBiasUp.ForeColor = System.Drawing.Color.Transparent;
            this.buttonBiasUp.ImageIndex = 1;
            this.buttonBiasUp.ImageList = this.imageList1;
            this.buttonBiasUp.Location = new System.Drawing.Point(198, 364);
            this.buttonBiasUp.Name = "buttonBiasUp";
            this.buttonBiasUp.Size = new System.Drawing.Size(24, 20);
            this.buttonBiasUp.TabIndex = 5;
            this.buttonBiasUp.Click += new System.EventHandler(this.buttonBiasUp_Click);
            // 
            // buttonFind
            // 
            this.buttonFind.Location = new System.Drawing.Point(281, 45);
            this.buttonFind.Name = "buttonFind";
            this.buttonFind.Size = new System.Drawing.Size(24, 23);
            this.buttonFind.TabIndex = 2;
            this.buttonFind.Text = "...";
            this.buttonFind.Click += new System.EventHandler(this.buttonFind_Click);
            // 
            // comboGameList
            // 
            this.comboGameList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboGameList.Location = new System.Drawing.Point(143, 47);
            this.comboGameList.Name = "comboGameList";
            this.comboGameList.Size = new System.Drawing.Size(132, 21);
            this.comboGameList.TabIndex = 1;
            this.comboGameList.SelectedValueChanged += new System.EventHandler(this.comboGameList_SelectedValueChanged);
            // 
            // radioSelFromList
            // 
            this.radioSelFromList.AutoSize = true;
            this.radioSelFromList.Checked = true;
            this.radioSelFromList.Location = new System.Drawing.Point(6, 51);
            this.radioSelFromList.Name = "radioSelFromList";
            this.radioSelFromList.Size = new System.Drawing.Size(131, 17);
            this.radioSelFromList.TabIndex = 0;
            this.radioSelFromList.TabStop = true;
            this.radioSelFromList.Text = "Select Game From List";
            // 
            // buttonStopGame
            // 
            this.buttonStopGame.Enabled = false;
            this.buttonStopGame.Location = new System.Drawing.Point(456, 201);
            this.buttonStopGame.Name = "buttonStopGame";
            this.buttonStopGame.Size = new System.Drawing.Size(88, 40);
            this.buttonStopGame.TabIndex = 12;
            this.buttonStopGame.Text = "Stop";
            this.buttonStopGame.Click += new System.EventHandler(this.buttonStopGame_Click);
            // 
            // buttonStartGame
            // 
            this.buttonStartGame.Location = new System.Drawing.Point(456, 128);
            this.buttonStartGame.Name = "buttonStartGame";
            this.buttonStartGame.Size = new System.Drawing.Size(88, 40);
            this.buttonStartGame.TabIndex = 12;
            this.buttonStartGame.Text = "Start";
            this.buttonStartGame.Click += new System.EventHandler(this.buttonStartGame_Click);
            // 
            // ConcentrateGamesCfg
            // 
            this.Controls.Add(this.buttonGainDown);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonGainUp);
            this.Controls.Add(this.groupGames);
            this.Controls.Add(this.buttonBiasDown);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupConfig);
            this.Controls.Add(this.textGain);
            this.Controls.Add(this.buttonBiasUp);
            this.Controls.Add(this.buttonStartGame);
            this.Controls.Add(this.textBias);
            this.Controls.Add(this.buttonStopGame);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonMosaicSaveCfg);
            this.Controls.Add(this.groupBox2);
            this.Name = "ConcentrateGamesCfg";
            this.Size = new System.Drawing.Size(558, 408);
            this.Load += new System.EventHandler(this.GamesCfg_Load);
            this.groupConfig.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericScoreInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericSmothFactor)).EndInit();
            this.groupGames.ResumeLayout(false);
            this.groupGames.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void SetStatuses(bool bRun) {
            if (InvokeRequired) {
                Invoke((Action)delegate
                {
                    SetStatuses(bRun);
                });
            }
            else {
                buttonStartGame.Enabled = !bRun;
                buttonStopGame.Enabled = bRun;
                ((Form)this.TopLevelControl).ControlBox = true;
            }
		}

		private void SetGame() {
			if (radioCalibration.Checked) {
				InitCalibration();
			} else {
				InitSelGame();
			}
		}

		private void InitSelGame() {
			// start game specified in the list
			string gname = comboGameList.Text;
			if (wmclient != null) {
				if (wmclient.Property != gname) {
					if (wmclient.GetAllGUIWindows() > 0) {
						wmclient.SendClient(GameCommand.CloseGame);
					}
					wmclient = null;
				}
			}
			if (wmclient == null) wmclient = new WMCopyData(gname, this.Handle);

			//ResManager rm = new ResManager(NIApplication.MAIN_CFG);
			if (wmclient.GetAllGUIWindows() == 0) {
				string game_path = BCIApplication.GetGamePath(gname);
				if (gname == null || !File.Exists(game_path)) return;

				// start program
				System.Diagnostics.ProcessStartInfo pinf = new System.Diagnostics.ProcessStartInfo();
				pinf.FileName = game_path;
				pinf.WorkingDirectory = Path.GetDirectoryName(game_path);
				Console.WriteLine("Start {0} in {1}", pinf.FileName, pinf.WorkingDirectory);
				System.Diagnostics.Process proc = System.Diagnostics.Process.Start(pinf);
				proc.Close();

				Thread.Sleep(1000);
			}

			wmclient.GetAllGUIWindows();
			SetStatuses(false);
		}

		private void gameForm_Closed(object sender, EventArgs e) {
			evt_StopProc.Set();
			SetStatuses(false);
		}

		private ManualResetEvent evt_StopProc = new ManualResetEvent(false);
		int cfg_stim_attdet = 200;
		int cfg_score_interval = 200;
		int cfg_score_num = 1;
		double cfg_score_bias = 0.0;
		double cfg_score_gain = 1.0;
		int cfg_eval_secs = 300;

		int cfg_player2 = 2; // 0 = none, 1 = player2, 2 = computer

        //private string cfg_chn_str = null;
            // "HEOL,HEOR,Fp1,Fp2,VEOU,VEOL,F7,F3,Fz,F4,F8,FT7,FC3,FCz,FC4,FT8,T3,C3,Cz,C4,T4,TP7,CP3,CPz,CP4,TP8,A1,T5,P3,Pz,P4,P6,A2,O1,Oz,O2,FT9,FT10,PO1,PO2";

		public void LoadConfig() {
			LoadConfig(BCIApplication.SysResource);
		}

        /// <summary>
        /// Need to seperate load content and GUI operations later
        /// </summary>
        /// <param name="rm"></param>
		private void LoadConfig(ResManager rm) {
			string rn;
			string line;

			rn = "EEG";
            //line = rm.GetConfigValue(rn, "ChannelNames");
            //if (line != null) cfg_chn_str = line;

            //// convert to cfg_chn_list
            //if (cfg_chn_str != null) {
            //    string[] clist = cfg_chn_str.Split(',');
            //    ArrayList al = new ArrayList(clist);
            //    foreach (string cn in clist) {
            //        string chn = cn.Trim();
            //        if (chn.Length == 0) continue;
            //        al.Add(chn);
            //    }
            //    //cfg_chn_list = (string[]) al.ToArray(typeof(string));
            //}

            // rn = "MosaicGame";
            // string imgpath  ="Images";

            rn = "GameParameters";
			rm.GetConfigValue(rn, "StimDetection", ref cfg_stim_attdet);
			rm.GetConfigValue(rn, "FreqDetection", ref cfg_score_interval);
			rm.GetConfigValue(rn, "ScoreSmoothNum", ref cfg_score_num);
			rm.GetConfigValue(rn, "ScoreBias", ref cfg_score_bias);
			rm.GetConfigValue(rn, "ScoreGain", ref cfg_score_gain);

			// 2009 02 17: calibration eval time in seconds
			rm.GetConfigValue(rn, "EvalTimeInSeconds", ref cfg_eval_secs);

			rn = BCICfg.AttentionDetection;
			line = rm.GetConfigValue(rn, "ModelFile");
			textBoxModelFile1.Text = line;
			line = rm.GetConfigValue(rn, "ModelUsedChannels");
			if (line != null) {
				textBoxUsedChannel1.Text = line;
			}

            line = rm.GetConfigValue(rn, "Amplifier");
            comboAmplifier.Items.Clear();
            int n = AmpContainer.Count;
            for (int i = 0; i < n; i++) {
                string ampid = AmpContainer.GetAmplifier(i).ID;
                comboAmplifier.Items.Add(ampid);
                if (line == ampid) comboAmplifier.SelectedIndex = i;
            }
            if (n > 0 && comboAmplifier.SelectedIndex < 0) comboAmplifier.SelectedIndex = 0;


			textBoxModelFile2.Text = line = rm.GetConfigValue(rn, "ModelFile_Player2");
			if (!string.IsNullOrEmpty(line)) {
				player2 = rm.GetConfigValue(rn, "Player2");
				SetPlayer2();
				textBoxUsedChannel2.Text = rm.GetConfigValue(rn, "ModelUsedChannels_Player2");
			} 
			else {
				SetPlayer2();
			}
            line = rm.GetConfigValue(rn, "Amplifier2");
            comboAmplifier2.Items.Clear();
            n = AmpContainer.Count;
            for (int i = 0; i < n; i++) {
                string ampid = AmpContainer.GetAmplifier(i).ID;
                comboAmplifier2.Items.Add(ampid);
                if (line == ampid) comboAmplifier2.SelectedIndex = i;
            }
            if (n > 0 && comboAmplifier2.SelectedIndex < 0) comboAmplifier2.SelectedIndex = 0;

			numericScoreInterval.Value = cfg_score_interval;
			numericSmothFactor.Value = cfg_score_num;

			textBias.Text = cfg_score_bias.ToString();
			textGain.Text = cfg_score_gain.ToString();

			if (cfg_score_num < 1) cfg_score_num = 1;
			if (score_list == null || score_list.GetLength(1) != cfg_score_num)
				score_list = new double[2,cfg_score_num];
			for (int i = 0; i < cfg_score_num; i++)
				score_list[0,i] = score_list[1,i] = 0;
			score_nxt[0] = score_nxt[1] = 0;

			rm.SaveIfChanged();
		}

		private delegate void output_result(int rtype, int n, IntPtr data);
		private double[,] score_list = null;
		private int[] score_nxt = new int[2] {0, 0};

        private ArrayList proc_list = new ArrayList();

		private void ExeTesting() {
            foreach (BCIProcessor p1 in proc_list) p1.Free();
            proc_list.Clear();

			HRTimer.GetTimestamp();

			ResManager rm = BCIApplication.SysResource;
			//LoadConfig(rm);
			rm.SetConfigValue(BCICfg.EEG, BCICfg.AppName, wmclient.Property);

			string dpath = null;

			if (!BCIApplication.QuickStart) {
				dpath = TestDirSpecForm.GetProcPath(rm, false);
				if (dpath == null) {
					if (TestDirSpecForm.dlg_result == DialogResult.Cancel) return;
				}
			} else {
				BCIApplication.QuickStart = false; // end quick starting here.
			}

            rm.SaveIfChanged();

            ConsoleCapture.ConsoleFile cfile = null;
            if (dpath != null) {
                string prefix_fn = wmclient.Property + "_" + BCIApplication.TimeStamp;

                if (!Directory.Exists(dpath)) {
                    Directory.CreateDirectory(dpath);
                }

                cfile = ConsoleCapture.StartConsoleFile(Path.Combine(dpath, string.Format("{0}.txt", prefix_fn)));
                AmpContainer.StartRecord(dpath, prefix_fn);
            }

            //
            // new implementation of amplifier/processor/worker
            //
            string ampid1 = null;
            string ampid2 = null;
            string mfn1 = null;
            string mfn2 = null;
            // get data info by invoking
            Invoke((Action)delegate
            {
                ampid1 = comboAmplifier.Text;
                ampid2 = comboAmplifier2.Text;

                mfn1 = textBoxModelFile1.Text;
                if (!string.IsNullOrEmpty(mfn1)) mfn1 = Path.Combine(BCIApplication.UserPath, mfn1);
                if (!File.Exists(mfn1)) mfn1 = null;

                mfn2 = textBoxModelFile2.Text;
                if (!string.IsNullOrEmpty(mfn2)) mfn2 = Path.Combine(BCIApplication.UserPath, mfn2);
                if (!File.Exists(mfn2)) mfn2 = null;
            });

            // player1
            BCIProcessor proc = null;
            if (!String.IsNullOrEmpty(mfn1)) {
                proc = new BCIProcessor(BCILib.App.BCIEngine.BCIProcType.Concentration);
                Console.WriteLine("Create processor for player1");
                if (proc != null) {
                    Hashtable parameters = new Hashtable();

                    parameters.Add("Model", mfn1);
                    if (!proc.Initialize(parameters)) {
                        Console.WriteLine("Processor1 Initialization failed");
                        proc.Free();
                        proc = null;
                    }
                }

                if (proc != null) {
                    proc.SetFeedbackHandler((output_result)delegate(int which, int n, IntPtr rd)
                    {
                        OutputResult(0, n, rd);
                    });

                    Amplifier amp = AmpContainer.GetAmplifier(ampid1);
                    if (amp != null) {
                        if (!proc.SetAmplifier(amp, mfn2)) {
                            MessageBox.Show("Player1: selectec channels wrong!");
                        }
                        else {
                            proc.SetReadingShift(cfg_score_interval);
                            proc_list.Add(proc);
                            proc = null;
                        }
                    }
                }
                if (proc != null) {
                    proc.Free();
                    proc = null;
                }
            }

            // player2
            if (!string.IsNullOrEmpty(player2)) {
                proc = BCIProcessor.CrtProcessor("Concentration");
            }

            if (proc != null) {
                Hashtable parameters = new Hashtable();
                parameters.Add("Model", Path.Combine(BCIApplication.UserPath, textBoxModelFile2.Text));
                if (!proc.Initialize(parameters)) {
                    Console.WriteLine("Processor2 Initialization failed");
                    proc = null;
                }
            }

            if (proc != null) {
                proc.SetFeedbackHandler((output_result)delegate(int which, int n, IntPtr rd)
                {
                    OutputResult(1, n, rd);
                });

                Amplifier amp = AmpContainer.GetAmplifier(ampid2);
                if (amp != null) {
                    if (proc.SetAmplifier(amp, textBoxUsedChannel2.Text)) {
                        proc.SetReadingShift(cfg_score_interval);
                        proc_list.Add(proc);
                        proc = null;
                    }
                }
            }
            if (proc != null) {
                proc.Free();
                proc = null;
            }

            if (proc_list.Count == 0) {
                Console.WriteLine("Processor started: 0");
                MessageBox.Show("Cannot start processor! No processor ccreated!");
                evt_StopProc.Set();
            }

            // setting reading position
            while (!evt_StopProc.WaitOne(0, false)) {
                if (wmclient.GetAllGUIWindows() == 0) break;
                int np = 0;
                foreach (BCIProcessor pi in proc_list) {
                    if (pi.Process()) np++;
                }

                if (np == 0) {
                    evt_StopProc.WaitOne(cfg_score_interval / 3);
                }

                if (!AmpContainer.AllAlive) {
                    evt_StopProc.Set();
                }
            }

            if (dpath != null) {
                cfile.EndLogFile();
                AmpContainer.StopRecord();
            }
        }

        /// <summary>
        /// Output game score
        /// </summary>
        /// <param name="user">0 = player1, 1= player2</param>
        /// <param name="n">data length</param>
        /// <param name="data">data array</param>
        private void OutputResult(int user, int n, IntPtr data)
        {
            double[] rdata = new double[n > 2 ? n : 2];
            Marshal.Copy(data, rdata, 0, n);
            //if (rtype == 0)
            {
                // average over cfg_score_num results
                score_list[user, score_nxt[user]] = rdata[0];
                score_nxt[user]++;
                if (score_nxt[user] >= cfg_score_num) score_nxt[user] = 0;

                rdata[0] = 0;
                for (int i = 0; i < cfg_score_num; i++) {
                    rdata[0] += score_list[user, i];
                }
                rdata[0] /= cfg_score_num;
                score_queue.Enqueue(user);
                score_queue.Enqueue(rdata[0]);
                evt_ScoreReady.Set();
            }
                //fbForm.AddScore(rdata);
                /*
            } else if (rtype == 1) {
                //fbForm.AddAlpha(rdata);
            } else {
                //fbForm.AddBeta(rdata);
            }
                 */
        }

		private void RunTesting() {
			evt_StopProc.Reset();
			evt_ScoreReady.Reset();
			score_queue.Clear();

			Thread thd = new Thread(new ThreadStart(ExeUpdateScore));
			thd.Start();

			try {
				ExeTesting();
			} catch (Exception ex) {
				Console.WriteLine("MosaicGame2Players.StartTesting: Exception={0}", ex);
                MessageBox.Show("Error in ExeTesting:" + ex.Message + "!\nPlease check log file!");
            }

            foreach (BCIProcessor pi in proc_list) {
                pi.Free();
            }
            proc_list.Clear();
			evt_StopProc.Set();
			thd.Join();
			wmclient.SendClient(GameCommand.StopGame);
			SetStatuses(false);
		}

		public void Close() {
			evt_StopProc.Set();

			if (wmclient != null) wmclient.SendClient(GameCommand.CloseGame);
			//if (fbForm != null) fbForm.Close();

			if (ConfigChanged) SaveConfig();
		}

		private Form gameForm = null;
		private WMCopyData wmclient = null;

		private void buttonStopGame_Click(object sender, System.EventArgs e) {
			evt_StopProc.Set();
			wmclient.SendClient(GameCommand.StopGame);
			SetStatuses(false);
		}

		private int[] mdl_chanels = new int[2] {0, 0};

		private void textBoxModelFile2_TextChanged(object sender, System.EventArgs e) {
			SetPlayer2();
		}

		private void SetPlayer2 () {
			FileInfo fi = null;
			if (textBoxModelFile2.Text.Length > 0) {
				fi = new FileInfo(textBoxModelFile2.Text);
				if (!fi.Exists) {
					textBoxModelFile2.Text = null;
				}
			}

			if (textBoxModelFile2.Text.Length == 0) {
				textBoxUsedChannel2.Text = null;
				buttonChannel2.Enabled = false;

                //panelPlayer2.Visible = false;
                //panelComputer.Visible = true;

                //cfg_player2 = checkBoxComputer.Checked? 2:0;
                //player2 = checkBoxComputer.Checked? "Computer":null;
                //groupBox2.Text = checkBoxComputer.Checked? "Player2:Computer":"Player2";

				return;
			}

			StreamReader sr = fi.OpenText();
			string line;
			mdl_chanels[1] = 0;
			textBoxUsedChannel2.Text = null;
			while ((line = sr.ReadLine()) != null) {
				if (line.StartsWith("Channel_Order:")) {
					mdl_chanels[1] = int.Parse(line.Substring(line.LastIndexOf(':') + 1));
                    textNumChannel2.Text = mdl_chanels[1].ToString();

					line = sr.ReadLine();
                    textBoxUsedChannel2.Text = VerifyChannels(line, comboAmplifier2.Text);
					buttonChannel2.Enabled = true;
					break;
				}
			}
			sr.Close();

			if (mdl_chanels[1] == 0) {
				textBoxUsedChannel2.Text = null;
				buttonChannel2.Enabled = false;
				return;
			}

			cfg_player2 = 1;
			groupBox2.Text = "Player2: " + player2;
		}

		private void buttonSetModel2_Click(object sender, System.EventArgs e) {
			SelectSubjectForm dlg = new SelectSubjectForm();
			if (dlg.ShowDialog(this) != DialogResult.OK) return;

			// Find model file
			string udir = Path.Combine(BCIApplication.UsersRoot, dlg.SubjectWithDate);


            string fn = Path.Combine(udir, Path.Combine("Config", "System.cfg"));
            if (File.Exists(fn)) {
                ResManager rm = new ResManager(fn);
                fn = rm.GetConfigValue("AttentionDetection", "ModelFile");
            }
            else {
                fn = null;
            }

            if (!string.IsNullOrEmpty(fn)) {
                fn = Path.Combine(udir, fn);
            }

			if (string.IsNullOrEmpty(fn) || !File.Exists(fn)) {
				MessageBox.Show("Model file not exists!", fn);
				return;
			}

			textBoxModelFile2.Text = Path.GetFullPath(fn);

			player2 = dlg.Subject;
			groupBox2.Text = "Player2: " + player2;
		}

		private void buttonChannel2_Click(object sender, System.EventArgs e) {
            SelectStringList dlg = new SelectStringList();
			dlg.Candidates = AmpContainer.GetAmplifier(comboAmplifier2.Text).ChannelNames;
			dlg.SelectedString = textBoxUsedChannel2.Text;
			if (dlg.ShowDialog(this) == DialogResult.OK) {
				if (dlg.SelectedNum != mdl_chanels[1]) {
					MessageBox.Show("Number of used channels is wrong!");
					return;
				}
				textBoxUsedChannel2.Text = dlg.SelectedString;
			}
		}

		private string VerifyChannels(string line, string amplifer) {
            // find amplifier
            Amplifier amp = AmpContainer.GetAmplifier(amplifer);
            if (amp == null) return null;
            string[] chlist = amp.ChannelNames;

			// verify channel names
			string[] ch_l = line.Split(',');
			ArrayList ca = new ArrayList(ch_l.Length);
			int cno = 0;
			foreach(string chn in ch_l) {
				string cstr = chn.Trim();
				if (cstr.Length == 0) continue;
				int ni = Array.IndexOf(chlist, cstr);
				if (ni < 0) {
					cstr = chlist[cno++];
				} else {
					cno = ni;
				}
				ca.Add(cstr);
			}

			StringBuilder sb = new StringBuilder();
			foreach(string chn in ca) {
				sb.Append(',');
				sb.Append(chn);
			}
			return sb.ToString(1, sb.Length - 1);			
		}

		private void textBoxModelFile1_TextChanged(object sender, System.EventArgs e) {
			FileInfo fi = null;
			if (textBoxModelFile1.Text != null && textBoxModelFile1.Text != "") {
				fi = new FileInfo(textBoxModelFile1.Text);
				if (!fi.Exists) {
					textBoxModelFile1.Text = null;
					fi = null;
				}
			}

			if (fi == null) {
				textBoxUsedChannel1.Text = null;
				buttonChannel1.Enabled = false;
				return;
			}

			StreamReader sr = fi.OpenText();
			string line;
			mdl_chanels[0] = 0;
			textBoxUsedChannel1.Text = null;
			while ((line = sr.ReadLine()) != null) {
				if (line.StartsWith("Channel_Order:")) {
					mdl_chanels[0] = int.Parse(line.Substring(line.LastIndexOf(':') + 1));
                    textNumChannel.Text = mdl_chanels[0].ToString();

					line = sr.ReadLine();
                    textBoxUsedChannel1.Text = VerifyChannels(line, comboAmplifier.Text);
					buttonChannel1.Enabled = true;
					break;
				}
			}
			sr.Close();

			if (mdl_chanels[0] == 0) {
				textBoxUsedChannel1.Text = null;
				buttonChannel1.Enabled = false;
				return;
			}	
		}

		private void buttonChannel1_Click(object sender, System.EventArgs e) {
            SelectStringList dlg = new SelectStringList();
			dlg.Candidates = AmpContainer.GetAmplifier(comboAmplifier.Text).ChannelNames;
			dlg.SelectedString = textBoxUsedChannel1.Text;
			if (dlg.ShowDialog(this) == DialogResult.OK) {
				if (dlg.SelectedNum != mdl_chanels[0]) {
					MessageBox.Show("Number of used channels is wrong!");
					return;
				}
				textBoxUsedChannel1.Text = dlg.SelectedString;
			}
		}

		private void buttonMosaicSaveCfg_Click(object sender, System.EventArgs e) {
			SaveConfig();
		}

		private void SaveConfig() {
			ResManager rm = BCIApplication.SysResource;
            string rn = "GameParameters";
			rm.SetConfigValue(rn, "FreqDetection", numericScoreInterval.Value.ToString());
			rm.SetConfigValue(rn, "ScoreSmoothNum", numericSmothFactor.Value.ToString());

			rm.SetConfigValue(rn, "ScoreBias", textBias.Text);
			rm.SetConfigValue(rn, "ScoreGain", textGain.Text);

			rn = BCICfg.AttentionDetection;
			if (buttonChannel1.Enabled) {
				rm.SetConfigValue(rn, "ModelUsedChannels", textBoxUsedChannel1.Text);
			}
            rm.SetConfigValue(rn, "Amplifier", comboAmplifier.Text);

			rm.SetConfigValue(rn, "Player2", cfg_player2 == 1? player2 : null);
			rm.SetConfigValue(rn, "ModelFile_Player2", textBoxModelFile2.Text);
			rm.SetConfigValue(rn, "ModelUsedChannels_Player2", textBoxUsedChannel2.Text);
            rm.SetConfigValue(rn, "Amplifier2", comboAmplifier2.Text);

			try {
				LoadConfig(rm);
			} catch (Exception) {
				LoadConfig();
			}
			ConfigChanged = false;
		}

		private void buttonClearPlayer2_Click(object sender, System.EventArgs e) {
			textBoxModelFile2.Text = null;
		}


		private void buttonStartGame_Click(object sender, System.EventArgs e) {
            SetGame();

			if (wmclient.GetAllGUIWindows() == 0) {
                MessageBox.Show("Cannot find client game!");
				return;
			}

			SaveConfig();
			//fbForm.SetDisplayChannels();

            if (!AmpContainer.StartAll()) {
                MessageBox.Show("Cannot start amplifier!", "BCI Concentration Game");
                return;
            }

			if (player1 != null) wmclient.SendClient(GameCommand.SetPlayerName, player1);
			if (player2 != null) wmclient.SendClient(GameCommand.SetPlayerName2, player2);

			wmclient.SendClient(GameCommand.StartGame, cfg_player2 == 0? 1:2);
			SetStatuses(true);

            Form frm = (Form)this.TopLevelControl;
            frm.ControlBox = false;
			Thread thd = new Thread(new ThreadStart(RunTesting));
			thd.Start();
		}

		private ManualResetEvent evt_ScoreReady = new ManualResetEvent(false);
		private Queue score_queue = new Queue();

		private void ExeUpdateScore() {
			WaitHandle[] wall = new WaitHandle[] {evt_StopProc, evt_ScoreReady};
            string fmt = "#0.0";
            double sc2 = 0;

			while (true) {
				if (WaitHandle.WaitAny(wall) == 0) break;
				while (score_queue.Count >= 2) {
                    int user = (int)score_queue.Dequeue();
					double score = (double) score_queue.Dequeue();

					double sp1 = (score - cfg_score_bias) * cfg_score_gain;
					if (sp1 < 0) sp1 = 0;

                    // update GUI
                    this.BeginInvoke((Action)delegate()
                    {
                        if (user == 0) {
                            textSpeed.Text = sp1.ToString(fmt);
                            textScore.Text = score.ToString(fmt);
                        }
                        else {
                            textSpeed2.Text = sp1.ToString(fmt);
                            textScore2.Text = score.ToString(fmt);
                        }
                    });

                    if (user == 0) {
                        double sp2 = (sc2 - cfg_score_bias) * cfg_score_gain;
                        if (sp2 < 0) sp2 = 0;
                        wmclient.SendClient(GameCommand.SetBCIScore, sp1, sp2);
                    }
                    else {
                        wmclient.SendClient(GameCommand.SetBCIScore2, sp1);
                        sc2 = score; // save player 2 score
                    }

                    if (score_queue.Count > 0) {
						Console.Write("UpdateScore: queue length = {0}\n", score_queue.Count);
					}
				}
				evt_ScoreReady.Reset();
			}
		}

		private void buttonBiasUp_Click(object sender, System.EventArgs e) {
			cfg_score_bias += 10;
			textBias.Text = cfg_score_bias.ToString();
		}

		private void buttonBiasDown_Click(object sender, System.EventArgs e) {
			cfg_score_bias -= 10;
			textBias.Text = cfg_score_bias.ToString();
		}

		private double[] gains = new double[] {0.01, 0.1, 0.2, 0.5, 0.8, 1.0, 2.0};
		private bool ConfigChanged = false;

		private void buttonGainDown_Click(object sender, System.EventArgs e) {
			int n = gains.Length;
			int i = 0;
			while (cfg_score_gain > gains[i] && i < n) i++;
			if (i > 0) i--;
			cfg_score_gain = gains[i];
			textGain.Text = cfg_score_gain.ToString();
			ConfigChanged = true;
		}

		private void buttonGainUp_Click(object sender, System.EventArgs e) {
			int n = gains.Length;
			int i = n - 1;
			while (cfg_score_gain < gains[i] && i >= 0) i--;
			if (i < n - 1) i++;
			cfg_score_gain = gains[i];
			textGain.Text = cfg_score_gain.ToString();
			ConfigChanged = true;
		}

		private void GamesCfg_Load(object sender, System.EventArgs e) {
            if (!DesignMode) {
                InitGamePlayers();
            }
		}

        public void InitGamePlayers()
        {
            if (comboGameList.Items.Count == 0) {
                ResManager _rm = BCIApplication.AppResource;
                player1 = _rm.GetConfigValue("Subject");
                groupBox1.Text = groupBox1.Text + ": " + player1;

                comboGameList.Items.Add("New Game ...");
                comboGameList.SelectedIndex = 0;
                string[] glist = BCIApplication.GetAppGames();
                if (glist != null) {
                    foreach (string gname in glist) {
                        comboGameList.Items.Add(gname);
                    }

                    if (comboGameList.Items.Count > 1) {
                        comboGameList.SelectedIndex = 1;
                    }
                }
            }
        }

		/// <summary>
		/// Quick start to the selected game
		/// </summary>
		public void QuickStart() {
			// Start car racing game
			// radioCarRace.PerformClick();
		}

		private void InitCalibration() {
			if (wmclient != null) {
				if (wmclient.Property != StoryForm.AppName) {
					if (wmclient.GetAllGUIWindows() > 0) {
						wmclient.SendClient(GameCommand.CloseGame);
					}
					wmclient = null;
				}
			}
			if (wmclient == null) wmclient = new WMCopyData(StoryForm.AppName, this.Handle);

			if (wmclient.GetAllGUIWindows() == 0 || gameForm == null) {
				if (gameForm != null && !gameForm.IsDisposed) {
					gameForm.Close();
					gameForm.Dispose();
				}

				gameForm = new StoryForm();
				gameForm.Closed += new EventHandler(DoCalibration);
			}

			gameForm.Show();
			wmclient.GetAllGUIWindows();
			SetStatuses(false);
		}

		private void DoCalibration(object sender, EventArgs e) {
			gameForm_Closed(sender, e);

            double[] eval_scores = null; // fbForm.GetPlayerScores(0);
            if (eval_scores == null) return;

			if (gameForm.DialogResult != DialogResult.OK || eval_scores == null) return;

			int neval = cfg_eval_secs * 1000 / cfg_score_interval;
			if (neval > eval_scores.Length) neval = eval_scores.Length;

			Array.Sort(eval_scores, 0, neval);

			cfg_score_bias = eval_scores[neval * 40 / 100];
			cfg_score_gain = 90.0 / (eval_scores[neval * 95 / 100] - cfg_score_bias);

			textBias.Text = cfg_score_bias.ToString("0");
			textGain.Text = cfg_score_gain.ToString("#.00");
			textScore.Text = null;
			textSpeed.Text = null;

			Console.WriteLine("Eval done. neval = {0}/{1}, bias = {2}, gain = {3}.",
				neval, 0, //fbForm.TotalNumberOfScores, 
                cfg_score_bias, cfg_score_gain);
		}

		private void comboGameList_SelectedValueChanged(object sender, System.EventArgs e) {
		}

		private void buttonFind_Click(object sender, System.EventArgs e) {
            if (comboGameList.SelectedIndex == 0) {
                OpenFileDialog fdlg = new OpenFileDialog();
                fdlg.RestoreDirectory = true;
                fdlg.Filter = "Exe files|*.exe|All Files|*.*";
                if (fdlg.ShowDialog() != DialogResult.OK) {
                    return;
                }

                string game_path = fdlg.FileName;
                string game_name = Path.GetFileNameWithoutExtension(game_path);
                game_name = ConfirmNewGame.ShowDialog(game_name);
                if (game_name == null) return;

                if (comboGameList.Items.IndexOf(game_name) >= 0) {
                    MessageBox.Show("Game name already exists!");
                    return;
                }

                ResManager rm = BCIApplication.AppResource;
                string line = rm.GetConfigValue(BCIApplication.AppName, "AppGames");
                if (string.IsNullOrEmpty(line)) line = game_name;
                else line = line + "," + game_name;
                rm.SetConfigValue(BCIApplication.AppName, "AppGames", line);

                rm.SetConfigValue(game_name, "Game_Path", game_path);

                int gn = comboGameList.Items.Add(game_name);
                comboGameList.SelectedIndex = gn;
                rm.SaveFile();
                radioSelFromList.PerformClick();
            }
            else if (comboGameList.SelectedIndex > 0) {
                string game_path = BCIApplication.GetGamePath(comboGameList.Text, true);
                radioSelFromList.PerformClick();
            }
		}

        private void comboAmplifier_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConfigChanged = true;
        }
	}
}
