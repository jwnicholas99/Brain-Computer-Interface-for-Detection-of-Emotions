using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Timers;
using System.Media;
using System.Configuration;
using System.Collections.Specialized;

using BCILib.App;
using BCILib.Amp;
using BCILib.Util;
using System.Threading;



namespace BCILib.MotorImagery
{
    public partial class PicLoader : Form
    {
        private string src = "../../pics/";
        private string dataFolder = "../../data/";
        private int noOfFolders;
        private int target;
        private int[][] chosenFiles;
        private int[] shuffledFiles;
        private string[] folderList;
        private string[][] fileList;
        private int[] randomized;
        private int[] shuffled;


        private int restTime = 3;
        private int openTime = 3;
        private int showTime = 3;
        private int rateTime = 9;

        private int stage = 5;
        private int totalNo;
        private int totalNoPS;
        private int picsDone = 0;
        private int sessionNo = 1;
        private bool renew = true;

        private int timeLeftInSecs = 0;

        private string indexNumber;
        private string dataPath;
        private string imgPath;
        private int fileIndex;
        private string shuffledPath;
        private string dpath;
        private string appname;

        private Amplifier amp;
        private ConsoleCapture.ConsoleFile cfile; 

        ManualResetEvent _evt_StopProc = new ManualResetEvent(false);
        WMCopyData client = null;
        int stimCode;

        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();
        
        public PicLoader()
        {
            InitializeComponent();
            LoadConfig();
        }

        private void LoadConfig()
        {
            LoadConfig(BCIApplication.SysResource);
        }

        private void LoadConfig(ResManager rm)
        {
            rm.GetConfigValue("PicLoader", "target", ref target);
            rm.GetConfigValue("PicLoader", "restTime", ref restTime);
            rm.GetConfigValue("PicLoader", "openTime", ref openTime);
            rm.GetConfigValue("PicLoader", "showTime", ref showTime);
            rm.GetConfigValue("PicLoader", "rateTime", ref rateTime);


            rm.SaveIfChanged();
            this.Invalidate();
        }

        private void shuffleBtn_Click(object sender, EventArgs e)
        {
            indexNumber = indexNo.Text;
            string appname = indexNumber;
            ResManager rm = BCIApplication.SysResource;
            BCIApplication.SetProtocolName(rm, appname); // used for specify training directory
            LoadConfig(rm);
            dpath = TrainDirSpecForm.GetTrainingPath(rm, true);
            if (dpath == null) return;


            //gets the names of folders
            folderList = Directory.GetDirectories(src);
            noOfFolders = folderList.Length;

            fileList = new string[noOfFolders][];
            chosenFiles = new int[noOfFolders][];

            //creates a list of files and a selection of random numbers for each folder
            for (int i = 0; i < folderList.Length; i++)
            {
                string[] filesInList = Directory.GetFiles(folderList[i]);

                int noOfFiles = filesInList.Length;

                int[] chosenNos = select(noOfFiles, target);

                fileList[i] = new string[noOfFiles];
                chosenFiles[i] = new int[target];


                fileList[i] = filesInList;
                chosenFiles[i] = chosenNos;
            }

            totalNo = noOfFolders * target;
            totalNoPS = totalNo / 6;

            randomized = new int[totalNo];
            for (int i = 0; i < chosenFiles.Length; i++)
            {
                for (int j = 0; j < chosenFiles[i].Length; j++)
                {
                    randomized[i * target + j] = i * 1000 + chosenFiles[i][j];
                }

            }

            shuffled = shuffle(randomized);

            //creates a file to store data
            shuffledPath = Path.Combine(Path.GetDirectoryName(dpath), "shuffled.txt");

            for (int i = 0; i < shuffled.Length; i++)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(shuffledPath, true))
                {
                    file.WriteLine(shuffled[i].ToString());
                }
            }

          
            timerText.Text = "READY";
            timer1.Tick += new EventHandler(timer_Tick);

        }


        private void prepareButton_Click(object sender, EventArgs e)
        {

            //creates a file to obtain data
            string timestamp = BCIApplication.TimeStamp;
            dataPath = Path.Combine(Path.GetDirectoryName(dpath), "ratings_" + timestamp + ".txt");
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(dataPath))
            {
                file.WriteLine("start");
            }


            //Logfile
            cfile = ConsoleCapture.StartConsoleFile(Path.Combine(dpath, appname + "_" + timestamp + ".log"));
            AmpContainer.StartAll();
            AmpContainer.StartRecord(dpath, appname);
            
            _evt_StopProc.WaitOne(2000, false);

            Random rnd = new Random();
            amp = AmpContainer.GetAmplifier(0);

            ConsoleOutForm.ShowWindow();

            //sends stimcode 200
            AmpContainer.SendAll(200);
            Console.WriteLine("Stimcode={0}", 200);
            stimCode = 200;

            timer1.Enabled = true;

            stage = 0;
            doRest();   

        }

        //place a total number of files, and this function will select a target number of files
        private int[] select(int totalNo, int target)
        {
            int[] totalNos = new int[totalNo];
            int[] chosenNos = new int[target];
            for (int i = 0; i < totalNo; i++)
            {
                totalNos[i] = i;
            }
            for (int t = 0; t < target; t++)
            {
                int r = RandomNumber(0, totalNo - t); //http://stackoverflow.com/questions/767999/random-number-generator-only-generating-one-random-number
                chosenNos[t] = totalNos[r];
                totalNos[r] = totalNos[totalNo - t - 1];
            }

            return chosenNos;
        }

        //shuffles a list
        private int[] shuffle(int[] shuffledNos)
        {

            for (int u = 0; u < 10000; u++)
            { 
                for (int t = 0; t < shuffledNos.Length; t++)
                {
                    int Seed = (int)DateTime.Now.Ticks;
                    Random rnd = new Random(Seed);
                    int r = rnd.Next(1, shuffledNos.Length);

                    int temporaryValue = shuffledNos[t];
                    shuffledNos[t] = shuffledNos[r];
                    shuffledNos[r] = temporaryValue;
                 }
            }


            /**for (int i = 0; i < shuffledNos.Length; i++)
            {
                abc.Text += shuffledNos[i].ToString() + ",";
            }**/

            return shuffledNos;
        }

        public static int RandomNumber(int min, int max)
        {
            lock (syncLock)
            { // synchronize
                return random.Next(min, max);
            }
        }


        public void timer_Tick(object source, EventArgs e)
        {
            if (timeLeftInSecs==0)
            {
                stage += 1;
                if (stage == 4)
                {
                    stage = 0;
                }

                switch (stage)
                {
                    case 0:
                        doRest();
                        break;
                    case 1:
                        doOpen();
                        break;
                    case 2:
                        doShow();
                        break;
                    case 3:
                        doRate();
                        break;
                }

            }
            timeLeftInSecs -= 1;
            timerText.Text = "Timer:\n" + timeLeftInSecs.ToString();
            
        }

        private void doRest()
        {
            AmpContainer.SendAll(210);
            Console.WriteLine("Stimcode={0}", 210);
            stimCode = 210;

            if (picsDone >= 1)
            {    sendData();
            }
            
            panelA.Visible = false;
            panelV.Visible = false;
            submitButton.Visible = false;
            timeLeftInSecs = restTime;
            timerText.Text = "Timer:\n" + timeLeftInSecs.ToString();
            picBox.Visible = true;
            picBox.Image = Image.FromFile(src + "/black.jpg");

            if (picsDone%(totalNoPS*2)== totalNoPS && renew)
            {
                renew = false;
                sessionNo++;
                stopRecording();
            }

            if (picsDone%(totalNoPS*2)==0 && !renew)
            {
                renew = true;
                sessionNo++;
                stopRecording();
            }

            sessionNumber.Text = "Session\n" + sessionNo.ToString();

            if (picsDone == totalNoPS*6)
            {
                sessionNumber.Text = "Thank you";
                stopRecording();
            }

        }

        private void doOpen()
        {
            stimCode = 220;
            AmpContainer.SendAll(stimCode);
            Console.WriteLine("Stimcode={0}", stimCode);
            
            timeLeftInSecs = openTime;
            timerText.Text = "Timer:\n" + timeLeftInSecs.ToString();
            Console.Beep();
            picBox.Visible = true;
            picBox.Image = Image.FromFile(src + "/white.jpg");
            
        }

        private void doShow()
        {
            picBox.Visible = true;
            
            fileIndex = shuffled[picsDone];

            stimCode = 230 + fileIndex / 1000;
            AmpContainer.SendAll(stimCode);
            Console.WriteLine("Stimcode={0}", stimCode);

            timeLeftInSecs = showTime;
            timerText.Text = "Timer:\n" + timeLeftInSecs.ToString();

            imgPath = fileList[fileIndex/1000][fileIndex%1000];
            picBox.Image = Image.FromFile(imgPath);
            picsDone++;

            int picsDoneTR = picsDone - totalNoPS * (sessionNo - 1);
            trialNo.Text = "Trial No:\n" + picsDoneTR.ToString() + "/" + totalNoPS.ToString();

        }

        private void doRate()
        {
            AmpContainer.SendAll(240);
            Console.WriteLine("Stimcode={0}", 240);
            stimCode = 240;

            trackBarA.Value = 0;
            trackBarV.Value = 0;
            timeLeftInSecs = rateTime;
            timerText.Text = "Timer:\n" + timeLeftInSecs.ToString();
            picBox.Visible = false;
            panelA.Visible = true;
            panelV.Visible = true;
            submitButton.Visible = true;
        }

        private void submitButton_Click(object sender, EventArgs e)
        {
            timeLeftInSecs = 0;
        }

        private void sendData()
        {
            
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(dataPath, true))
            {
                string imgName = Path.GetFileName(imgPath);
                string imgType = Path.GetFileName(Path.GetDirectoryName(imgPath));
                file.WriteLine(imgName + ";" + imgType + ";" + trackBarA.Value + ";" + trackBarV.Value);
            }
            

        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            stopRecording();
        }

        private void PicLoader_FormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            stopRecording();
        }


        private void stopRecording()
        {

            picBox.Image = Image.FromFile(src + "/thankyou.jpg");
            AmpContainer.SendAll(250);
            Console.WriteLine("Stimcode={0}", 250);
            stimCode = 250;

            cfile.EndLogFile();
            amp.StopRecord();
            timer1.Enabled = false;
        }

        private void loadSBtn_Click(object sender, EventArgs e)
        {

        }

        private void submitButton_Click_1(object sender, EventArgs e)
        {
            timeLeftInSecs = 0;
        }

        
    }


}
/**

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

using BCILib.App;
using BCILib.Amp;
using BCILib.Util;

namespace BCILib.MotorImagery
{
    public partial class PicLoader : Form
    {
        private int cfg_num_tasks = 2;
        private int cfg_num_trials = 40;
        private int cfg_waitstart_time = 2000;
        private int cfg_pre_time = 2000;
        private int cfg_cue_time = 0;
        private int cfg_act_time = 4000;
        private int cfg_rest_time = 6000;
        private int cfg_img_wait = 0;


        private int cfg_stim_prep = 100;
        private int cfg_stim_task_offset = 120;
        private int cfg_stim_keydown_offset = 130;
        private int cfg_stim_click_offset = 150;
        private int cfg_stim_rest = 199;

        private int cfg_beep_after_rest = 1000; // Milliseconds before rest ends
        private int _cfg_num_score = 5; // Number of scores to do average
        private int _cfg_shift_score = 500; // milliseconds to calculate score contineously

        private int _trialNum = 0;
        private int trialNum
        {
            get { return this._trialNum; }
            set { this._trialNum = value; UpdateUI(); }
        }

        private int _stimCode = 0;
        private int stimCode
        {
            get { return this._stimCode; }
            set { this._stimCode = value; UpdateUI(); }
        }




        double[,] _miscore_his = null;
        double[] mi_score = null;

        WMCopyData client = null;

        ManualResetEvent _evt_StopProc = new ManualResetEvent(false);
        ManualResetEvent _evt_ActionDone = new ManualResetEvent(false);


        private string _MIType;
        private string _task_string = null;
        public string TaskString
        {
            set
            {
                //init
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < MITaskConfig.TASK_IDLIST.Length; i++)
                {
                    sb.Append('N');
                }
                _MIType = sb.ToString();

                //set
                if (!string.IsNullOrEmpty(value))
                {
                    foreach (char ti in value)
                    {
                        int idx = MITaskConfig.TASK_IDLIST.IndexOf(ti); //search for its presence and exact location
                        if (idx >= 0 && idx < MITaskConfig.TASK_IDLIST.Length)
                        {
                            sb[idx] = 'I';
                        }
                    }
                }
                _MIType = sb.ToString();
                _task_string = value;
            }
        }


        public PicLoader()
        {
            InitializeComponent();
            LoadConfig();
        }

        private void MITrainingDataCollection_Load(object sender, EventArgs e)
        {
            extAppLauncher.LoadGameList();
        }

        private void LoadConfig()
        {
            LoadConfig(BCIApplication.SysResource);
        }

        private void LoadConfig(ResManager rm)
        {

            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Number_Trials, ref cfg_num_trials);
            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Time_WaitStart, ref cfg_waitstart_time);
            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Time_Prepare, ref cfg_pre_time);
            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Time_Cue, ref cfg_cue_time);
            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Time_Action, ref cfg_act_time);
            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Time_Rest, ref cfg_rest_time);
            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Time_Imagine_Wait, ref cfg_img_wait);

            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Stim_Prepare, ref cfg_stim_prep);
            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Stim_Task_Offset, ref cfg_stim_task_offset);
            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Stim_Click_Offset, ref cfg_stim_click_offset);
            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Stim_KeyDown_Offset, ref cfg_stim_keydown_offset);
            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.Stim_Rest, ref cfg_stim_rest);

            rm.GetConfigValue(MIConstDef.MITraining, MIConstDef.BeepAfterRest, ref cfg_beep_after_rest);

            rm.GetConfigValue(MIConstDef.MITest, MIConstDef.MI_Num_Score, ref _cfg_num_score);
            rm.GetConfigValue(MIConstDef.MITest, MIConstDef.MI_Shift_Score, ref _cfg_shift_score);


            //_miscore_his = new double[_cfg_num_score, _MIType.Length];

            rm.SaveIfChanged();
            this.Invalidate();
        }

        private void RunDataCollection()
        {
            try
            {
                ExeDataCollection();
            }
            catch (Exception e)
            {
                MessageBox.Show("Runtest! Error = " + e.Message);
            }

            try
            {
                this.Invoke((Action)delegate()
                {
                    buttonStart.Enabled = true;
                    buttonStop.Enabled = false;
                });
            }
            catch (Exception) { }
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            client = extAppLauncher.LaunchGame();

            buttonStart.Enabled = false;
            buttonStop.Enabled = true;

            LoadConfig(BCIApplication.SysResource);

            Thread thd = new Thread(new ThreadStart(RunDataCollection));
            thd.Start();
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            _evt_StopProc.Set();
        }

        private void ExeDataCollection()
        {
            ResManager rm = BCIApplication.SysResource;

            string appname = "MI";

            BCIApplication.SetProtocolName(rm, appname); // used for specify training directory
            LoadConfig(rm);

            string dpath = TrainDirSpecForm.GetTrainingPath(rm, true);
            if (dpath == null) return;

            string timestamp = BCIApplication.TimeStamp;

            //Logfile
            var cfile = ConsoleCapture.StartConsoleFile(Path.Combine(dpath, appname + "_" + timestamp + ".log"));
            AmpContainer.StartAll();
            AmpContainer.StartRecord(dpath, appname);

            if (cfg_waitstart_time < 0) cfg_waitstart_time = 0;

            _evt_StopProc.WaitOne(cfg_waitstart_time, false);

            WaitHandle[] evts = new WaitHandle[] { _evt_StopProc };

            ConsoleOutForm.ShowWindow();

            int ntasks = _MIType.Length;

            int[] tasks = new int[ntasks];
            int[] keys = new int[ntasks];
            Random rnd = new Random();
            Amplifier amp = AmpContainer.GetAmplifier(0);

            //first beep
            uint beep_freq = 300;
            uint beep_time = 500; // milliseconds,

            trialNum = 0;

            if (cfg_beep_after_rest > 0)
            {
                int wt = cfg_rest_time - cfg_beep_after_rest;
                if (wt < 0) wt = 0;
                if (!_evt_StopProc.WaitOne(wt, false))
                {
                    Sound.Beep(beep_freq, beep_time);
                }
                wt = cfg_rest_time - wt - (int)beep_time;
                if (wt < 0) wt = 0;
                _evt_StopProc.WaitOne(wt, false);
            }
            else
            {
                if (cfg_rest_time > 0) _evt_StopProc.WaitOne(cfg_rest_time);
            }

            //start trials
            for (int itrial = 0; itrial < cfg_num_trials; itrial++)
            {
                if (_evt_StopProc.WaitOne(0)) 
                    break;

                if (!amp.IsAlive)
                {
                    MessageBox.Show("Amplifer is not run!");
                    _evt_StopProc.Set();
                    break;
                }

                // generate tasks and randomize it
                for (int itask = 0; itask < ntasks; itask++)
                {
                    tasks[itask] = itask + 1;
                    keys[itask] = rnd.Next(1000);
                }
                Array.Sort(keys, tasks);

                for (int itask = 0; itask < ntasks; itask++)
                {
                    int ctask = tasks[itask];
                    char task_cfg = _MIType[ctask - 1];
                    if (task_cfg == 'N') continue;

                    //Prepare
                    AmpContainer.SendAll(cfg_stim_prep);
                    Console.WriteLine("Stimcode={0}", cfg_stim_prep);
                    stimCode = cfg_stim_prep;

                    SendClientCmd(StimCode, cfg_stim_prep);

                    if (_evt_StopProc.WaitOne(cfg_pre_time, false))
                    {
                        break;
                    }

                    // Show task
                    stimCode = cfg_stim_task_offset + ctask;
                    AmpContainer.SendAll(stimCode);
                    Console.WriteLine("STIM_TASK:{0}", stimCode);
                    SendClientCmd(StimCode, stimCode);

                    // No keyboard input
                    if (cfg_img_wait > 0) _evt_StopProc.WaitOne(cfg_img_wait, false);

                    this.Invalidate();

                    if (_evt_StopProc.WaitOne(cfg_act_time, false)) break;


                    // rest
                    stimCode = cfg_stim_rest;
                    AmpContainer.SendAll(cfg_stim_rest);
                    Console.WriteLine("STIM_REST:{0}", cfg_stim_rest);
                    SendClientCmd(StimCode, cfg_stim_rest);

                    if (cfg_beep_after_rest > 0)
                    {
                        //ShowProgress(cfg_rest_time, false);
                        int wt = cfg_rest_time - cfg_beep_after_rest;
                        if (wt < 0) wt = 0;
                        if (!_evt_StopProc.WaitOne(wt, false))
                        {
                            Sound.Beep(beep_freq, beep_time);
                        }
                        wt = cfg_rest_time - wt - (int)beep_time;
                        if (wt < 0) wt = 0;
                        _evt_StopProc.WaitOne(wt, false);
                    }
                    else
                    {
                        //ShowProgress(cfg_rest_time, true);
                        if (cfg_rest_time > 0) _evt_StopProc.WaitOne(cfg_rest_time);
                    }

                    trialNum += 1;

                    if (!AmpContainer.AllAlive)
                    {
                        _evt_StopProc.Set();
                        break;
                    }
                }
            }

            if (!_evt_StopProc.WaitOne(0, false))
            {
                // finished, add training dir to training list
                BCIApplication.AddTrainingPath(dpath);
                string msg = "Training session " + appname + " Finished!";
                Console.WriteLine(msg);
                MessageBox.Show(msg);
            }
            amp.StopRecord(); //sometimes amp or Amplifer by cc
            cfile.EndLogFile();
        }


        // commands sent to client
        const string StimCode = "StimCode";

        private void SendClientCmd(string cmd, object val)
        {
            if (client != null) client.SendCmdString("{0}, {1}", cmd, val);
        }

        private void MITrainingDataCollection_FormClosing(object sender, FormClosingEventArgs e)
        {
            _evt_StopProc.Set();
        }

        private void UpdateUI()
        {
            try
            {
                this.Invoke((Action)delegate()
                {
                    InfoTrialNum.Text = "Trial " + trialNum.ToString() + " completed";
                    InfoStimCode.Text = stimCode.ToString();
                });
            }
            catch (Exception) { }
        }

        private void toolStripConfig_Click(object sender, EventArgs e)
        {
            BCIApplication.SysResource.ShowDialog();
        }


    }
}**/
