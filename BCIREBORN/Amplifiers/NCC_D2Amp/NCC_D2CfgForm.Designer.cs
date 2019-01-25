namespace BCILib.Amp
{
    partial class NCC_D2CfgForm
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
            this.components = new System.ComponentModel.Container();
            this.buttonOK = new System.Windows.Forms.Button();
            this.labelSNO = new System.Windows.Forms.Label();
            this.textBoxSN = new System.Windows.Forms.TextBox();
            this.buttonChange = new System.Windows.Forms.Button();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.labelStatus = new System.Windows.Forms.Label();
            this.panelImpRadioButtons = new System.Windows.Forms.Panel();
            this.radioButton40K = new System.Windows.Forms.RadioButton();
            this.radioButton20K = new System.Windows.Forms.RadioButton();
            this.radioButton15K = new System.Windows.Forms.RadioButton();
            this.radioButton10K = new System.Windows.Forms.RadioButton();
            this.radioButton5K = new System.Windows.Forms.RadioButton();
            this.panelChannels = new System.Windows.Forms.Panel();
            this.buttonRange = new System.Windows.Forms.Button();
            this.panelImpLegend = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.labelSelChannel = new System.Windows.Forms.Label();
            this.labelFp2 = new System.Windows.Forms.Label();
            this.labelT6 = new System.Windows.Forms.Label();
            this.labelF8 = new System.Windows.Forms.Label();
            this.labelO2 = new System.Windows.Forms.Label();
            this.labelP4 = new System.Windows.Forms.Label();
            this.labelOz = new System.Windows.Forms.Label();
            this.labelF4 = new System.Windows.Forms.Label();
            this.labelO1 = new System.Windows.Forms.Label();
            this.labelPz = new System.Windows.Forms.Label();
            this.labelP3 = new System.Windows.Forms.Label();
            this.labelFz = new System.Windows.Forms.Label();
            this.labelF3 = new System.Windows.Forms.Label();
            this.labelA2 = new System.Windows.Forms.Label();
            this.labelT4 = new System.Windows.Forms.Label();
            this.labelC4 = new System.Windows.Forms.Label();
            this.labelCz = new System.Windows.Forms.Label();
            this.labelC3 = new System.Windows.Forms.Label();
            this.labelT3 = new System.Windows.Forms.Label();
            this.labelT5 = new System.Windows.Forms.Label();
            this.labelA1 = new System.Windows.Forms.Label();
            this.labelF7 = new System.Windows.Forms.Label();
            this.labelFp1 = new System.Windows.Forms.Label();
            this.labelSp2 = new System.Windows.Forms.Label();
            this.labelSp1 = new System.Windows.Forms.Label();
            this.panelImpRadioButtons.SuspendLayout();
            this.panelChannels.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(364, 430);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 3;
            this.buttonOK.Text = "&OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // labelSNO
            // 
            this.labelSNO.AutoSize = true;
            this.labelSNO.Location = new System.Drawing.Point(24, 17);
            this.labelSNO.Name = "labelSNO";
            this.labelSNO.Size = new System.Drawing.Size(79, 13);
            this.labelSNO.TabIndex = 4;
            this.labelSNO.Text = "Serial Number: ";
            // 
            // textBoxSN
            // 
            this.textBoxSN.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSN.Location = new System.Drawing.Point(125, 14);
            this.textBoxSN.Name = "textBoxSN";
            this.textBoxSN.ReadOnly = true;
            this.textBoxSN.Size = new System.Drawing.Size(564, 20);
            this.textBoxSN.TabIndex = 5;
            // 
            // buttonChange
            // 
            this.buttonChange.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonChange.Location = new System.Drawing.Point(716, 12);
            this.buttonChange.Name = "buttonChange";
            this.buttonChange.Size = new System.Drawing.Size(75, 23);
            this.buttonChange.TabIndex = 6;
            this.buttonChange.Text = "Change";
            this.buttonChange.UseVisualStyleBackColor = true;
            this.buttonChange.Click += new System.EventHandler(this.buttonChange_Click);
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // labelStatus
            // 
            this.labelStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelStatus.Location = new System.Drawing.Point(716, 51);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(75, 23);
            this.labelStatus.TabIndex = 7;
            this.labelStatus.Text = "Status";
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelImpRadioButtons
            // 
            this.panelImpRadioButtons.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelImpRadioButtons.Controls.Add(this.radioButton40K);
            this.panelImpRadioButtons.Controls.Add(this.radioButton20K);
            this.panelImpRadioButtons.Controls.Add(this.radioButton15K);
            this.panelImpRadioButtons.Controls.Add(this.radioButton10K);
            this.panelImpRadioButtons.Controls.Add(this.radioButton5K);
            this.panelImpRadioButtons.Location = new System.Drawing.Point(27, 40);
            this.panelImpRadioButtons.Name = "panelImpRadioButtons";
            this.panelImpRadioButtons.Size = new System.Drawing.Size(662, 34);
            this.panelImpRadioButtons.TabIndex = 8;
            // 
            // radioButton40K
            // 
            this.radioButton40K.AutoSize = true;
            this.radioButton40K.Checked = true;
            this.radioButton40K.Location = new System.Drawing.Point(349, 11);
            this.radioButton40K.Name = "radioButton40K";
            this.radioButton40K.Size = new System.Drawing.Size(44, 17);
            this.radioButton40K.TabIndex = 0;
            this.radioButton40K.TabStop = true;
            this.radioButton40K.Text = "40K";
            this.radioButton40K.UseVisualStyleBackColor = true;
            this.radioButton40K.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButton20K
            // 
            this.radioButton20K.AutoSize = true;
            this.radioButton20K.Location = new System.Drawing.Point(261, 11);
            this.radioButton20K.Name = "radioButton20K";
            this.radioButton20K.Size = new System.Drawing.Size(44, 17);
            this.radioButton20K.TabIndex = 0;
            this.radioButton20K.Text = "20K";
            this.radioButton20K.UseVisualStyleBackColor = true;
            this.radioButton20K.Visible = false;
            this.radioButton20K.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButton15K
            // 
            this.radioButton15K.AutoSize = true;
            this.radioButton15K.Location = new System.Drawing.Point(173, 11);
            this.radioButton15K.Name = "radioButton15K";
            this.radioButton15K.Size = new System.Drawing.Size(44, 17);
            this.radioButton15K.TabIndex = 0;
            this.radioButton15K.Text = "15K";
            this.radioButton15K.UseVisualStyleBackColor = true;
            this.radioButton15K.Visible = false;
            this.radioButton15K.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButton10K
            // 
            this.radioButton10K.AutoSize = true;
            this.radioButton10K.Location = new System.Drawing.Point(85, 11);
            this.radioButton10K.Name = "radioButton10K";
            this.radioButton10K.Size = new System.Drawing.Size(44, 17);
            this.radioButton10K.TabIndex = 0;
            this.radioButton10K.Text = "10K";
            this.radioButton10K.UseVisualStyleBackColor = true;
            this.radioButton10K.Visible = false;
            this.radioButton10K.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButton5K
            // 
            this.radioButton5K.AutoSize = true;
            this.radioButton5K.Location = new System.Drawing.Point(3, 11);
            this.radioButton5K.Name = "radioButton5K";
            this.radioButton5K.Size = new System.Drawing.Size(38, 17);
            this.radioButton5K.TabIndex = 0;
            this.radioButton5K.Text = "5K";
            this.radioButton5K.UseVisualStyleBackColor = true;
            this.radioButton5K.Visible = false;
            this.radioButton5K.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // panelChannels
            // 
            this.panelChannels.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelChannels.Controls.Add(this.buttonRange);
            this.panelChannels.Controls.Add(this.panelImpLegend);
            this.panelChannels.Controls.Add(this.label1);
            this.panelChannels.Controls.Add(this.labelSelChannel);
            this.panelChannels.Controls.Add(this.labelFp2);
            this.panelChannels.Controls.Add(this.labelT6);
            this.panelChannels.Controls.Add(this.labelF8);
            this.panelChannels.Controls.Add(this.labelO2);
            this.panelChannels.Controls.Add(this.labelP4);
            this.panelChannels.Controls.Add(this.labelOz);
            this.panelChannels.Controls.Add(this.labelF4);
            this.panelChannels.Controls.Add(this.labelO1);
            this.panelChannels.Controls.Add(this.labelPz);
            this.panelChannels.Controls.Add(this.labelP3);
            this.panelChannels.Controls.Add(this.labelFz);
            this.panelChannels.Controls.Add(this.labelF3);
            this.panelChannels.Controls.Add(this.labelA2);
            this.panelChannels.Controls.Add(this.labelT4);
            this.panelChannels.Controls.Add(this.labelC4);
            this.panelChannels.Controls.Add(this.labelCz);
            this.panelChannels.Controls.Add(this.labelC3);
            this.panelChannels.Controls.Add(this.labelT3);
            this.panelChannels.Controls.Add(this.labelT5);
            this.panelChannels.Controls.Add(this.labelA1);
            this.panelChannels.Controls.Add(this.labelF7);
            this.panelChannels.Controls.Add(this.labelFp1);
            this.panelChannels.Controls.Add(this.labelSp2);
            this.panelChannels.Controls.Add(this.labelSp1);
            this.panelChannels.Location = new System.Drawing.Point(27, 80);
            this.panelChannels.Name = "panelChannels";
            this.panelChannels.Size = new System.Drawing.Size(764, 344);
            this.panelChannels.TabIndex = 9;
            // 
            // buttonRange
            // 
            this.buttonRange.Location = new System.Drawing.Point(686, 146);
            this.buttonRange.Name = "buttonRange";
            this.buttonRange.Size = new System.Drawing.Size(75, 23);
            this.buttonRange.TabIndex = 4;
            this.buttonRange.Text = "Range ...";
            this.buttonRange.UseVisualStyleBackColor = true;
            this.buttonRange.Click += new System.EventHandler(this.buttonRange_Click);
            // 
            // panelImpLegend
            // 
            this.panelImpLegend.Location = new System.Drawing.Point(543, 45);
            this.panelImpLegend.Name = "panelImpLegend";
            this.panelImpLegend.Size = new System.Drawing.Size(137, 287);
            this.panelImpLegend.TabIndex = 3;
            this.panelImpLegend.Paint += new System.Windows.Forms.PaintEventHandler(this.panelImpLegend_Paint);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(558, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Impedance: (KΩ)";
            // 
            // labelSelChannel
            // 
            this.labelSelChannel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelSelChannel.AutoSize = true;
            this.labelSelChannel.Location = new System.Drawing.Point(70, 319);
            this.labelSelChannel.Name = "labelSelChannel";
            this.labelSelChannel.Size = new System.Drawing.Size(119, 13);
            this.labelSelChannel.TabIndex = 1;
            this.labelSelChannel.Text = "Selected channel value";
            // 
            // labelFp2
            // 
            this.labelFp2.BackColor = System.Drawing.Color.Fuchsia;
            this.labelFp2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelFp2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelFp2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelFp2.ForeColor = System.Drawing.Color.White;
            this.labelFp2.Location = new System.Drawing.Point(302, 61);
            this.labelFp2.Name = "labelFp2";
            this.labelFp2.Size = new System.Drawing.Size(43, 34);
            this.labelFp2.TabIndex = 0;
            this.labelFp2.Text = "Fp2";
            this.labelFp2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelFp2.Click += new System.EventHandler(this.labelChannel_Click);
            // 
            // labelT6
            // 
            this.labelT6.BackColor = System.Drawing.Color.Fuchsia;
            this.labelT6.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelT6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelT6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelT6.ForeColor = System.Drawing.Color.White;
            this.labelT6.Location = new System.Drawing.Point(378, 237);
            this.labelT6.Name = "labelT6";
            this.labelT6.Size = new System.Drawing.Size(43, 34);
            this.labelT6.TabIndex = 0;
            this.labelT6.Text = "T6";
            this.labelT6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelT6.Click += new System.EventHandler(this.labelChannel_Click);
            // 
            // labelF8
            // 
            this.labelF8.BackColor = System.Drawing.Color.Fuchsia;
            this.labelF8.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelF8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelF8.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelF8.ForeColor = System.Drawing.Color.White;
            this.labelF8.Location = new System.Drawing.Point(378, 113);
            this.labelF8.Name = "labelF8";
            this.labelF8.Size = new System.Drawing.Size(43, 34);
            this.labelF8.TabIndex = 0;
            this.labelF8.Text = "F8";
            this.labelF8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelF8.Click += new System.EventHandler(this.labelChannel_Click);
            // 
            // labelO2
            // 
            this.labelO2.BackColor = System.Drawing.Color.Fuchsia;
            this.labelO2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelO2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelO2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelO2.ForeColor = System.Drawing.Color.White;
            this.labelO2.Location = new System.Drawing.Point(309, 269);
            this.labelO2.Name = "labelO2";
            this.labelO2.Size = new System.Drawing.Size(43, 34);
            this.labelO2.TabIndex = 0;
            this.labelO2.Text = "O2";
            this.labelO2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelO2.Click += new System.EventHandler(this.labelChannel_Click);
            // 
            // labelP4
            // 
            this.labelP4.BackColor = System.Drawing.Color.Fuchsia;
            this.labelP4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelP4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelP4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelP4.ForeColor = System.Drawing.Color.White;
            this.labelP4.Location = new System.Drawing.Point(309, 217);
            this.labelP4.Name = "labelP4";
            this.labelP4.Size = new System.Drawing.Size(43, 34);
            this.labelP4.TabIndex = 0;
            this.labelP4.Text = "P4";
            this.labelP4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelP4.Click += new System.EventHandler(this.labelChannel_Click);
            // 
            // labelOz
            // 
            this.labelOz.BackColor = System.Drawing.Color.Fuchsia;
            this.labelOz.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelOz.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelOz.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelOz.ForeColor = System.Drawing.Color.White;
            this.labelOz.Location = new System.Drawing.Point(240, 269);
            this.labelOz.Name = "labelOz";
            this.labelOz.Size = new System.Drawing.Size(43, 34);
            this.labelOz.TabIndex = 0;
            this.labelOz.Text = "Oz";
            this.labelOz.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelOz.Click += new System.EventHandler(this.labelChannel_Click);
            // 
            // labelF4
            // 
            this.labelF4.BackColor = System.Drawing.Color.Fuchsia;
            this.labelF4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelF4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelF4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelF4.ForeColor = System.Drawing.Color.White;
            this.labelF4.Location = new System.Drawing.Point(309, 113);
            this.labelF4.Name = "labelF4";
            this.labelF4.Size = new System.Drawing.Size(43, 34);
            this.labelF4.TabIndex = 0;
            this.labelF4.Text = "F4";
            this.labelF4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelF4.Click += new System.EventHandler(this.labelChannel_Click);
            // 
            // labelO1
            // 
            this.labelO1.BackColor = System.Drawing.Color.Fuchsia;
            this.labelO1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelO1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelO1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelO1.ForeColor = System.Drawing.Color.White;
            this.labelO1.Location = new System.Drawing.Point(173, 269);
            this.labelO1.Name = "labelO1";
            this.labelO1.Size = new System.Drawing.Size(43, 34);
            this.labelO1.TabIndex = 0;
            this.labelO1.Text = "O1";
            this.labelO1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelO1.Click += new System.EventHandler(this.labelChannel_Click);
            // 
            // labelPz
            // 
            this.labelPz.BackColor = System.Drawing.Color.Fuchsia;
            this.labelPz.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelPz.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelPz.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPz.ForeColor = System.Drawing.Color.White;
            this.labelPz.Location = new System.Drawing.Point(240, 217);
            this.labelPz.Name = "labelPz";
            this.labelPz.Size = new System.Drawing.Size(43, 34);
            this.labelPz.TabIndex = 0;
            this.labelPz.Text = "Pz";
            this.labelPz.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelPz.Click += new System.EventHandler(this.labelChannel_Click);
            // 
            // labelP3
            // 
            this.labelP3.BackColor = System.Drawing.Color.Fuchsia;
            this.labelP3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelP3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelP3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelP3.ForeColor = System.Drawing.Color.White;
            this.labelP3.Location = new System.Drawing.Point(171, 217);
            this.labelP3.Name = "labelP3";
            this.labelP3.Size = new System.Drawing.Size(43, 34);
            this.labelP3.TabIndex = 0;
            this.labelP3.Text = "P3";
            this.labelP3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelP3.Click += new System.EventHandler(this.labelChannel_Click);
            // 
            // labelFz
            // 
            this.labelFz.BackColor = System.Drawing.Color.Fuchsia;
            this.labelFz.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelFz.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelFz.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelFz.ForeColor = System.Drawing.Color.White;
            this.labelFz.Location = new System.Drawing.Point(240, 113);
            this.labelFz.Name = "labelFz";
            this.labelFz.Size = new System.Drawing.Size(43, 34);
            this.labelFz.TabIndex = 0;
            this.labelFz.Text = "Fz";
            this.labelFz.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelFz.Click += new System.EventHandler(this.labelChannel_Click);
            // 
            // labelF3
            // 
            this.labelF3.BackColor = System.Drawing.Color.Fuchsia;
            this.labelF3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelF3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelF3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelF3.ForeColor = System.Drawing.Color.White;
            this.labelF3.Location = new System.Drawing.Point(171, 113);
            this.labelF3.Name = "labelF3";
            this.labelF3.Size = new System.Drawing.Size(43, 34);
            this.labelF3.TabIndex = 0;
            this.labelF3.Text = "F3";
            this.labelF3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelF3.Click += new System.EventHandler(this.labelChannel_Click);
            // 
            // labelA2
            // 
            this.labelA2.BackColor = System.Drawing.Color.Fuchsia;
            this.labelA2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelA2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelA2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelA2.ForeColor = System.Drawing.Color.White;
            this.labelA2.Location = new System.Drawing.Point(447, 165);
            this.labelA2.Name = "labelA2";
            this.labelA2.Size = new System.Drawing.Size(43, 34);
            this.labelA2.TabIndex = 0;
            this.labelA2.Text = "A2";
            this.labelA2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelA2.Click += new System.EventHandler(this.labelChannel_Click);
            // 
            // labelT4
            // 
            this.labelT4.BackColor = System.Drawing.Color.Fuchsia;
            this.labelT4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelT4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelT4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelT4.ForeColor = System.Drawing.Color.White;
            this.labelT4.Location = new System.Drawing.Point(378, 165);
            this.labelT4.Name = "labelT4";
            this.labelT4.Size = new System.Drawing.Size(43, 34);
            this.labelT4.TabIndex = 0;
            this.labelT4.Text = "T4";
            this.labelT4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelT4.Click += new System.EventHandler(this.labelChannel_Click);
            // 
            // labelC4
            // 
            this.labelC4.BackColor = System.Drawing.Color.Fuchsia;
            this.labelC4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelC4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelC4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelC4.ForeColor = System.Drawing.Color.White;
            this.labelC4.Location = new System.Drawing.Point(309, 165);
            this.labelC4.Name = "labelC4";
            this.labelC4.Size = new System.Drawing.Size(43, 34);
            this.labelC4.TabIndex = 0;
            this.labelC4.Text = "C4";
            this.labelC4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelC4.Click += new System.EventHandler(this.labelChannel_Click);
            // 
            // labelCz
            // 
            this.labelCz.BackColor = System.Drawing.Color.Fuchsia;
            this.labelCz.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelCz.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelCz.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCz.ForeColor = System.Drawing.Color.White;
            this.labelCz.Location = new System.Drawing.Point(240, 165);
            this.labelCz.Name = "labelCz";
            this.labelCz.Size = new System.Drawing.Size(43, 34);
            this.labelCz.TabIndex = 0;
            this.labelCz.Text = "Cz";
            this.labelCz.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelCz.Click += new System.EventHandler(this.labelChannel_Click);
            // 
            // labelC3
            // 
            this.labelC3.BackColor = System.Drawing.Color.Fuchsia;
            this.labelC3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelC3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelC3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelC3.ForeColor = System.Drawing.Color.White;
            this.labelC3.Location = new System.Drawing.Point(171, 165);
            this.labelC3.Name = "labelC3";
            this.labelC3.Size = new System.Drawing.Size(43, 34);
            this.labelC3.TabIndex = 0;
            this.labelC3.Text = "C3";
            this.labelC3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelC3.Click += new System.EventHandler(this.labelChannel_Click);
            // 
            // labelT3
            // 
            this.labelT3.BackColor = System.Drawing.Color.Fuchsia;
            this.labelT3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelT3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelT3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelT3.ForeColor = System.Drawing.Color.White;
            this.labelT3.Location = new System.Drawing.Point(102, 165);
            this.labelT3.Name = "labelT3";
            this.labelT3.Size = new System.Drawing.Size(43, 34);
            this.labelT3.TabIndex = 0;
            this.labelT3.Text = "T3";
            this.labelT3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelT3.Click += new System.EventHandler(this.labelChannel_Click);
            // 
            // labelT5
            // 
            this.labelT5.BackColor = System.Drawing.Color.Fuchsia;
            this.labelT5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelT5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelT5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelT5.ForeColor = System.Drawing.Color.White;
            this.labelT5.Location = new System.Drawing.Point(102, 237);
            this.labelT5.Name = "labelT5";
            this.labelT5.Size = new System.Drawing.Size(43, 34);
            this.labelT5.TabIndex = 0;
            this.labelT5.Text = "T5";
            this.labelT5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelT5.Click += new System.EventHandler(this.labelChannel_Click);
            // 
            // labelA1
            // 
            this.labelA1.BackColor = System.Drawing.Color.Fuchsia;
            this.labelA1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelA1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelA1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelA1.ForeColor = System.Drawing.Color.White;
            this.labelA1.Location = new System.Drawing.Point(33, 165);
            this.labelA1.Name = "labelA1";
            this.labelA1.Size = new System.Drawing.Size(43, 34);
            this.labelA1.TabIndex = 0;
            this.labelA1.Text = "A1";
            this.labelA1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelA1.Click += new System.EventHandler(this.labelChannel_Click);
            // 
            // labelF7
            // 
            this.labelF7.BackColor = System.Drawing.Color.Fuchsia;
            this.labelF7.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelF7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelF7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelF7.ForeColor = System.Drawing.Color.White;
            this.labelF7.Location = new System.Drawing.Point(102, 113);
            this.labelF7.Name = "labelF7";
            this.labelF7.Size = new System.Drawing.Size(43, 34);
            this.labelF7.TabIndex = 0;
            this.labelF7.Text = "F7";
            this.labelF7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelF7.Click += new System.EventHandler(this.labelChannel_Click);
            // 
            // labelFp1
            // 
            this.labelFp1.BackColor = System.Drawing.Color.Fuchsia;
            this.labelFp1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelFp1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelFp1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelFp1.ForeColor = System.Drawing.Color.White;
            this.labelFp1.Location = new System.Drawing.Point(184, 61);
            this.labelFp1.Name = "labelFp1";
            this.labelFp1.Size = new System.Drawing.Size(43, 34);
            this.labelFp1.TabIndex = 0;
            this.labelFp1.Text = "Fp1";
            this.labelFp1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelFp1.Click += new System.EventHandler(this.labelChannel_Click);
            // 
            // labelSp2
            // 
            this.labelSp2.BackColor = System.Drawing.Color.Fuchsia;
            this.labelSp2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelSp2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelSp2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSp2.ForeColor = System.Drawing.Color.White;
            this.labelSp2.Location = new System.Drawing.Point(349, 9);
            this.labelSp2.Name = "labelSp2";
            this.labelSp2.Size = new System.Drawing.Size(43, 34);
            this.labelSp2.TabIndex = 0;
            this.labelSp2.Text = "Sp2";
            this.labelSp2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelSp2.Click += new System.EventHandler(this.labelChannel_Click);
            // 
            // labelSp1
            // 
            this.labelSp1.BackColor = System.Drawing.Color.Fuchsia;
            this.labelSp1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelSp1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelSp1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSp1.ForeColor = System.Drawing.Color.White;
            this.labelSp1.Location = new System.Drawing.Point(160, 9);
            this.labelSp1.Name = "labelSp1";
            this.labelSp1.Size = new System.Drawing.Size(43, 34);
            this.labelSp1.TabIndex = 0;
            this.labelSp1.Text = "Sp1";
            this.labelSp1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelSp1.Click += new System.EventHandler(this.labelChannel_Click);
            // 
            // NCC_D2CfgForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(803, 456);
            this.Controls.Add(this.panelChannels);
            this.Controls.Add(this.panelImpRadioButtons);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.buttonChange);
            this.Controls.Add(this.textBoxSN);
            this.Controls.Add(this.labelSNO);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "NCC_D2CfgForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "NCC Amplifier D2 Configuration";
            this.Load += new System.EventHandler(this.NCC_D2CfgForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.NCC_D2CfgForm_FormClosing);
            this.panelImpRadioButtons.ResumeLayout(false);
            this.panelImpRadioButtons.PerformLayout();
            this.panelChannels.ResumeLayout(false);
            this.panelChannels.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Label labelSNO;
        private System.Windows.Forms.TextBox textBoxSN;
        private System.Windows.Forms.Button buttonChange;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Panel panelImpRadioButtons;
        private System.Windows.Forms.RadioButton radioButton40K;
        private System.Windows.Forms.RadioButton radioButton20K;
        private System.Windows.Forms.RadioButton radioButton15K;
        private System.Windows.Forms.RadioButton radioButton10K;
        private System.Windows.Forms.RadioButton radioButton5K;
        private System.Windows.Forms.Panel panelChannels;
        private System.Windows.Forms.Label labelFp2;
        private System.Windows.Forms.Label labelFz;
        private System.Windows.Forms.Label labelF3;
        private System.Windows.Forms.Label labelF7;
        private System.Windows.Forms.Label labelFp1;
        private System.Windows.Forms.Label labelSp2;
        private System.Windows.Forms.Label labelSp1;
        private System.Windows.Forms.Label labelF8;
        private System.Windows.Forms.Label labelF4;
        private System.Windows.Forms.Label labelA2;
        private System.Windows.Forms.Label labelT4;
        private System.Windows.Forms.Label labelC4;
        private System.Windows.Forms.Label labelCz;
        private System.Windows.Forms.Label labelC3;
        private System.Windows.Forms.Label labelT3;
        private System.Windows.Forms.Label labelA1;
        private System.Windows.Forms.Label labelT6;
        private System.Windows.Forms.Label labelO2;
        private System.Windows.Forms.Label labelP4;
        private System.Windows.Forms.Label labelOz;
        private System.Windows.Forms.Label labelO1;
        private System.Windows.Forms.Label labelPz;
        private System.Windows.Forms.Label labelP3;
        private System.Windows.Forms.Label labelT5;
        private System.Windows.Forms.Label labelSelChannel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonRange;
        private System.Windows.Forms.Panel panelImpLegend;
    }
}