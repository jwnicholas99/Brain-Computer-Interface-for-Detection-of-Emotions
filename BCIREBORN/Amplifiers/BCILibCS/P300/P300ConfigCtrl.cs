using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using BCILib.Util;
using BCILib.App;
using System.Diagnostics;
using System.IO;
using System.Threading;
using BCILib.Amp;
using System.Linq;

namespace BCILib.P300
{
    public partial class P300ConfigCtrl : UserControl
    {
        ResManager _rm = default(ResManager);

        P300Manager _manager = null;
        P300SpellerForm frm = null;
        bool _cfg_changed = false;
        bool _spl_changed = false;

        public string P300Speller
        {
            get
            {
                return ResName.P300Speller.ToString();
            }
        }

        public P300ConfigCtrl()
        {
            InitializeComponent();
        }

        #region Manager_Event_Handlers

        private void SendGameCommand(P300_WMCMD cmd, params object[] args) {
            if (_wmclient != null) {
                _wmclient.ResetPar();
                _wmclient.Add((byte)cmd);
                foreach (object arg in args) {
                    if (arg is Int32) {
                        _wmclient.Add((Int32)arg);
                    }
                    else if (arg is Double) {
                        _wmclient.Add((Double)arg);
                    }
                    else if (arg is ButtonState) {
                        _wmclient.Add((int)arg);
                    }
                    else {
                        throw new NotImplementedException("P300 game command parameter type not defined.");
                    }
                }
                _wmclient.SendPar(GameCommand.P300CMD);
            }
        }
        private bool manager_P300Output(int btn, double score)
        {
            //Console.WriteLine("P300Output");
            SendGameCommand(P300_WMCMD.Output_Code_Score, btn, score);
            return true;
        }

        private void manager_PrepareStart(int ms_time, WaitHandle waith)
        {
            //throw new NotImplementedException();
            Console.WriteLine("PrepareStart: {0}", ms_time);
            SendGameCommand(P300_WMCMD.Prepare_Start, ms_time);
        }

        private void manager_ButtonState(Int32 btn, ButtonState state)
        {
            //Console.WriteLine("Btn {0} -> {1}", btn, state);
            SendGameCommand(P300_WMCMD.Set_Button_State, btn, state);
        }

        private bool manager_StateChanged(int task)
        {
            if (InvokeRequired) {
                Func<int, bool> dlg_ChangeState = (int t) => manager_StateChanged(t);
                Invoke(dlg_ChangeState, task);
            }
            else {
                bool Running = _manager.IsRunning;
                toolStripStartTrain.Enabled = toolStripStartTest.Enabled = !Running;
                toolStripStop.Enabled = Running;

                if (task == 1) {
                    StartTrainingForm();
                }
            }
            return true;
        }
        #endregion 

        private void P300ConfigCtrl_Load(object sender, EventArgs e)
        {
            labelP300Speller.Text = P300Speller;
        }

        // ccwang: called only once by initializer
        public void InitContent()
        {
            RefreshContent();

            Control parent = this.Parent;
            while (!(parent is Form)) {
                parent = parent.Parent;
            }
            ((Form)parent).FormClosing += (object o, FormClosingEventArgs e1) =>
            {
                Save();
                if (_wmclient != null) {
                    _wmclient.SendClient(GameCommand.CloseGame);
                }
            };

            //textBoxTrainingSample.Disposed += (object o, EventArgs e1) => { Save(); };
            _manager = new P300Manager(this);

            _manager.StateChanged += task => manager_StateChanged(task);
            _manager.SetButtonState += (btn, state) => manager_ButtonState(btn, state);
            _manager.PrepareProgress += (ms_time, waith) => manager_PrepareStart(ms_time, waith);
            _manager.P300Output += (btn, score) => manager_P300Output(btn, score);

            externalBCIApp.LoadGameList();
        }

        enum ResName
        {
            EEG, P300EEGSignal, P300Training, P300Speller, ModelTrainingSetting
        }

        enum PropName
        {
            AppName, NumEpochPerRound,  NumRound, PreStimDuration, PostStimDuration, ChannelNames, SamplingRate, P300UsedChannels,
            Intensification, NonIntensification, NumRow, NumColumn, SpellerChars, PauseBwChar, FlashDuration, NoRepeatTime,
            CharIndication, CharBlank,
            GUIColumns, NumForNearCheck, StimNearCheckFrom, StimNearCheckTo, FreqHighStop, 
            TrainModelFileList, TrainRejectFileList, StartProcTimeAftStim, EndProcTimeAftStim, UseDelta

        }

        public int NumEpochPerRound
        {
            get
            {
                int num_c = 9;
                Resource.GetConfigValue(ResName.EEG, PropName.NumEpochPerRound, ref num_c);

                NumRow = 1; // must be 1
                NumColumn = num_c;
                return num_c;
            }
            set
            {
                NumColumn = value;
                GUIColumns = (int)(Math.Sqrt(value) + 0.5);

                int nnear = 4;
                if (nnear > value / 4) nnear = value / 4;
                NumForNearCheck = nnear;

                StimNearCheckFrom = 1;
                StimNearCheckTo = value;

                Resource.SetConfigValue(ResName.EEG, PropName.NumEpochPerRound, value.ToString());
            }
        }

        public int NumRound
        {
            get
            {
                int nround = 8;
                Resource.GetConfigValue(ResName.EEG, PropName.NumRound, ref nround);
                return nround;
            }

            set
            {
                Resource.SetConfigValue(ResName.EEG, PropName.NumRound, value.ToString());
            }
        }

        public string AppName
        {
            get
            {
                return Resource.GetConfigValue(ResName.EEG, PropName.AppName);
            }

            set
            {
                //BCIApplication.SetProtocolName(Resource, value);
                Resource.SetConfigValue(ResName.EEG, PropName.AppName, value);
            }
        }

        public int PreStimDuration
        {
            get
            {
                int pre_ms = 0;
                Resource.GetConfigValue(ResName.EEG, PropName.PreStimDuration, ref pre_ms);
                return pre_ms;
            }

            set
            {
                Resource.SetConfigValue(ResName.EEG, PropName.PreStimDuration, value.ToString());
            }
        }
        
        public int PostStimDuration
        {
            get
            {
                int post_ms = 1000;
                Resource.GetConfigValue(ResName.EEG, PropName.PostStimDuration, ref post_ms);
                return post_ms;
            }

            set
            {
                Resource.SetConfigValue(ResName.EEG, PropName.PostStimDuration, value.ToString());
            }
        }

        public string ChannelNames
        {
            get
            {
                string ch_names = string.Empty;
                if (AmpContainer.Count > 0) {
                    ch_names = AmpContainer.GetAmplifier(0).GetChannelNameString();
                }
                Resource.GetConfigValue(ResName.EEG, PropName.ChannelNames, ref ch_names);
                return ch_names;
            }

            set
            {
                Resource.SetConfigValue(ResName.EEG, PropName.ChannelNames, value);
            }
        }

        public string P300UsedChannels
        {
            get
            {
                string ch_names = string.Empty;
                if (AmpContainer.Count > 0) {
                    ch_names = AmpContainer.GetAmplifier(0).GetChannelNameString();
                }
                Resource.GetConfigValue(ResName.EEG, PropName.P300UsedChannels, ref ch_names);
                return ch_names;
            }

            set
            {
                Resource.SetConfigValue(ResName.EEG, PropName.P300UsedChannels, value);
            }
        }

        public int SamplingRate
        {
            get
            {
                int srate = 250;
                if (AmpContainer.Count > 0) {
                    srate = AmpContainer.GetAmplifier(0).header.samplingrate;
                }
                if (srate == 0) srate = 250;
                Resource.GetConfigValue(ResName.EEG, PropName.SamplingRate, ref srate);
                return srate;
            }
            
            set
            {
                Resource.SetConfigValue(ResName.EEG, PropName.SamplingRate, value.ToString());
            }
        }

        public int Intensification
        {
            get {
                int time = 100;
                Resource.GetConfigValue(ResName.P300EEGSignal, PropName.Intensification, ref time);
                return time;
            }
            set {
                Resource.SetConfigValue(ResName.P300EEGSignal, PropName.Intensification, value.ToString());
            }
        }

        public int NonIntensification {
            get {
                int time = 0;
                Resource.GetConfigValue(ResName.P300EEGSignal, PropName.NonIntensification, ref time);
                return time;
            }
            set {
                Resource.SetConfigValue(ResName.P300EEGSignal, PropName.NonIntensification, value.ToString());
            }
        }
        
        private int NumRow
        {
            get
            {
                int nrow = 1;
                Resource.GetConfigValue(ResName.P300EEGSignal, PropName.NumRow, ref nrow);
                return nrow;
            }
            set
            {
                Resource.SetConfigValue(ResName.P300EEGSignal, PropName.NumRow, value.ToString());
            }
        }
        
        private int NumColumn
        {
            get
            {
                int nc = 1;
                Resource.GetConfigValue(ResName.P300EEGSignal, PropName.NumColumn, ref nc);
                return nc;
            }
            set
            {
                Resource.SetConfigValue(ResName.P300EEGSignal, PropName.NumColumn, value.ToString());
            }
        }
        
        public string SpellerChars
        {
            get
            {
                string spellstr = "1,2,3,4,5,6,7,8,9";
                Resource.GetConfigValue(ResName.P300EEGSignal, PropName.SpellerChars, ref spellstr);
                return spellstr;
            }
            set
            {
                var chlist = ResManager.StringToCharList(value);
                var escstr = "\\";
                var chstr = string.Join(",", chlist.Select(c => c < 30 ? escstr + ((int)c).ToString("x") :
                    char.IsPunctuation(c)? escstr + c.ToString() : c.ToString()).ToArray());
                Resource.SetConfigValue(ResName.P300EEGSignal, PropName.SpellerChars, chstr);
            }
        }

        public int PauseBwChar
        {
            get
            {
                int time = 4000;
                Resource.GetConfigValue(ResName.P300EEGSignal, PropName.PauseBwChar, ref time);
                return time;
            }
            set
            {
                Resource.SetConfigValue(ResName.P300EEGSignal, PropName.PauseBwChar, value.ToString());
            }
        }
        
        public int FlashDuration
        {
            get
            {
                int nc = 1;
                Resource.GetConfigValue(ResName.P300EEGSignal, PropName.FlashDuration, ref nc);
                return nc;
            }
            set
            {
                Resource.SetConfigValue(ResName.P300EEGSignal, PropName.FlashDuration, value.ToString());
            }
        }

        public int NoRepeatTime
        {
            get
            {
                int nr = 500;
                Resource.GetConfigValue(ResName.P300EEGSignal, PropName.NoRepeatTime, ref nr);
                return nr;
            }
            set
            {
                Resource.SetConfigValue(ResName.P300EEGSignal, PropName.NoRepeatTime, value.ToString());
            }
        }

        // resource P300Training
        public int CharIndication
        {
            get
            {
                int time_ms = 2000;
                Resource.GetConfigValue(ResName.P300Training, PropName.CharIndication, ref time_ms);
                return time_ms;
            }
            set
            {
                Resource.SetConfigValue(ResName.P300Training, PropName.CharIndication, value.ToString());
            }
        }
        
        public int CharBlank
        {
            get
            {
                int time_ms = 1000;
                Resource.GetConfigValue(ResName.P300Training, PropName.CharBlank, ref time_ms);
                return time_ms;
            }
            set
            {
                Resource.SetConfigValue(ResName.P300Training, PropName.CharBlank, value.ToString());
            }
        }

        // resource P300Speller: for flashing sequence desgin
        public int GUIColumns
        {
            get
            {
                int nc = (int) (Math.Sqrt(NumEpochPerRound) + 0.5);
                Resource.GetConfigValue(ResName.P300Speller, PropName.GUIColumns, ref nc);
                return nc;
            }
            set
            {
                Resource.SetConfigValue(ResName.P300Speller, PropName.GUIColumns, value);
            }
        }

        public int NumForNearCheck
        {
            get
            {
                int nc = NumEpochPerRound;
                Resource.GetConfigValue(ResName.P300Speller, PropName.NumForNearCheck, ref nc);
                return nc;
            }
            set
            {
                Resource.SetConfigValue(ResName.P300Speller, PropName.NumForNearCheck, value);
            }
        }
        
        public int StimNearCheckFrom
        {
            get
            {
                int nc = 1;
                Resource.GetConfigValue(ResName.P300Speller, PropName.StimNearCheckFrom, ref nc);
                return nc;
            }
            set
            {
                Resource.SetConfigValue(ResName.P300Speller, PropName.StimNearCheckFrom, value);
            }
        }
        
        public int StimNearCheckTo
        {
            get
            {
                int nc = NumEpochPerRound;
                Resource.GetConfigValue(ResName.P300Speller, PropName.StimNearCheckTo, ref nc);
                return nc;
            }
            set
            {
                Resource.SetConfigValue(ResName.P300Speller, PropName.StimNearCheckTo, value);
            }
        }

        public ResManager Resource
        {
            get
            {
                if (_rm == null) _rm = BCIApplication.SysResource;
                return _rm;
            }
        }

        public void Save()
        {
            if (_cfg_changed) {
                try {
                    NumEpochPerRound = int.Parse(textBoxNumCodes.Text);
                    SpellerChars = textBoxSpellChars.Text;
                }
                catch (Exception) {
                    RefreshContent();
                }
                _cfg_changed = false;
            }
            Resource.SaveIfChanged();

            if (_spl_changed) {
                string tfn = TrainingSplFilePath;
                string[] words = textBoxTrainingSample.Text.Split(new char[] {' '},
                    StringSplitOptions.RemoveEmptyEntries);
                using (StreamWriter sw = File.CreateText(tfn)) {
                    sw.WriteLine("NumWords {0}", words.Length);
                    sw.WriteLine("StartWord 1");
                    foreach (string wd in words) {
                        sw.WriteLine(wd);
                    }
                }
                _spl_changed = false;
            }
        }

        public string TrainingSplFilePath
        {
            get
            {
                return Path.Combine(Path.Combine(BCIApplication.UserPath, "Config"), "training.txt");
            }
        }

        // resource ModelTrainingSetting
        public string TrainModelFileList
        {
            get
            {
                return Resource.GetConfigValue(ResName.ModelTrainingSetting, PropName.TrainModelFileList);
            }

            set
            {
                Resource.SetConfigValue(ResName.ModelTrainingSetting, PropName.TrainModelFileList, value);
            }
        }

        public string TrainRejectFileList
        {
            get
            {
                return Resource.GetConfigValue(ResName.ModelTrainingSetting, PropName.TrainRejectFileList);
            }

            set
            {
                Resource.SetConfigValue(ResName.ModelTrainingSetting, PropName.TrainRejectFileList, value);
            }
        }

        public int StartProcTimeAftStim
        {
            get
            {
                int ts_start = 150;
                Resource.GetConfigValue(ResName.ModelTrainingSetting, PropName.StartProcTimeAftStim, ref ts_start);
                return ts_start;
            }
            set
            {
                Resource.SetConfigValue(ResName.ModelTrainingSetting, PropName.StartProcTimeAftStim, value);
            }
        }

        public int EndProcTimeAftStim
        {
            get
            {
                int ts_start = 500;
                Resource.GetConfigValue(ResName.ModelTrainingSetting, PropName.EndProcTimeAftStim, ref ts_start);
                return ts_start;
            }
            set
            {
                Resource.SetConfigValue(ResName.ModelTrainingSetting, PropName.EndProcTimeAftStim, value);
            }
        }

        public int FreqHighStop
        {
            get
            {
                int ts_start = 25;
                Resource.GetConfigValue(ResName.P300Speller, PropName.FreqHighStop, ref ts_start);
                return ts_start;
            }
            set
            {
                Resource.SetConfigValue(ResName.P300Speller, PropName.FreqHighStop, value);
            }
        }

        public bool UseDelta
        {
            get
            {
                bool use_delta = true;
                Resource.GetConfigValue(ResName.ModelTrainingSetting, PropName.UseDelta, ref use_delta);
                return use_delta;
            }
            set
            {
                Resource.SetConfigValue(ResName.ModelTrainingSetting, PropName.UseDelta, value.ToString());
            }
        }

        public void RefreshContent()
        {
            textBoxNumCodes.Text = NumEpochPerRound.ToString();
            textBoxSpellChars.Text = SpellerChars;
            _cfg_changed = false;
            Console.WriteLine(PostStimDuration);
            Console.WriteLine(ChannelNames);
            Console.WriteLine(SamplingRate);
            Console.WriteLine(StartProcTimeAftStim);
            Console.WriteLine(EndProcTimeAftStim);
            Console.WriteLine(FreqHighStop);

            // training sample file
            string tfn = TrainingSplFilePath;
            textBoxTrainingSample.Text = null;
            if (File.Exists(tfn)) {
                using (StreamReader sr = File.OpenText(tfn)) {
                    sr.ReadLine();
                    sr.ReadLine();
                    int n = 0;
                    string line;
                    while ((line = sr.ReadLine()) != null) {
                        if (n > 0) textBoxTrainingSample.AppendText(" ");
                        textBoxTrainingSample.AppendText(line);
                        n++;
                    }

                }
                _spl_changed = false;
            }
            else {
                char[] clist = ResManager.StringToCharList(SpellerChars);
                StringBuilder sb = new StringBuilder();
                foreach (char c in clist) {
                    if (c > 32 && c < 128) sb.Append(c);
                }
                textBoxTrainingSample.Text = sb.ToString();

                _spl_changed = true;
            }
        }

        private void CfgChanged(object sender, EventArgs e)
        {
            _cfg_changed = true;
        }

        private void toolStripStartTrain_Click(object sender, EventArgs e)
        {
            Save();

            string cls_path = TrainModelFileList;
            string rej_path = TrainRejectFileList;

            // allow to retrain
            ModelDataPathForm tfrm = new ModelDataPathForm();
            tfrm.ClassifyModelPath = cls_path;
            tfrm.RejectModelPath = rej_path;
            if (tfrm.ShowDialog() != DialogResult.OK) return;
            cls_path = TrainModelFileList = tfrm.ClassifyModelPath;
            rej_path = TrainRejectFileList = tfrm.RejectModelPath;
            Save();

            if (string.IsNullOrEmpty(cls_path) || string.IsNullOrEmpty(rej_path)) {
                // start p300 GUI
                if (!StartP300Speller()) return; // or external P300 GUI, later to implement
                _manager.StartTrainer();
            }

            if (!string.IsNullOrEmpty(cls_path) && !string.IsNullOrEmpty(rej_path)) {
                StartTrainingForm();
            }
        }

        private void toolStripStartTest_Click(object sender, EventArgs e)
        {
            if (!StartP300Speller()) return;
            _manager.StartManager();
        }

        private void StartTrainingForm()
        {
            if (!string.IsNullOrEmpty(TrainModelFileList) && !string.IsNullOrEmpty(TrainRejectFileList)) {
                Save();
                TrainModelForm tmf = new TrainModelForm(this);
                tmf.ShowDialog();
            }
        }

        private void toolStripStop_Click(object sender, EventArgs e)
        {
            _manager.StopManager();
        }

        private void SplChanged(object sender, EventArgs e)
        {
            _spl_changed = true;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            Save();
        }

        WMCopyData _wmclient = null;
        private void buttonStartSpeller_Click(object sender, EventArgs e)
        {
            StartP300Speller();
        }

        private bool StartP300Speller()
        {
            
            Save();

            InitWMCopyData(P300Speller);

            if (_wmclient.GetAllGUIWindows() <= 0) {
                frm = new P300SpellerForm(this);
                frm.FormClosed += (s, e) =>
                {
                    _manager.StopManager();
                };
                frm.Show();
                _wmclient.GetAllGUIWindows();
            }

            return true;
        }

        private void InitWMCopyData(string appID)
        {
            AppName = appID;

            if (_wmclient != null) {
                if (_wmclient.Property != appID) {
                    _wmclient.SendClient(GameCommand.CloseGame);
                    _wmclient = null;
                }
            }

            if (_wmclient == null) {
                if (!string.IsNullOrEmpty(appID)) {
                    _wmclient = new WMCopyData(appID, Handle);
                }
            }
        }

        internal void ResetConfig(ResManager rm)
        {
            _rm = rm;
            RefreshContent();
        }
    }
}
