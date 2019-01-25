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

/* Hi we are Nicholas, Gabriel and Tze Jit, have fun understanding*/

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

        private string IAPSFile;
        private string IADSFile;
        private int stage = 5;
        private int totalNo;
        private int totalNoPS;
        private int picsDone = 0;
        private int sessionNo = 1;
        private bool renew = true;

        private int timeLeftInSecs = 0;

        private string indexNumber;
        private string dataPath;
        private string imgNo;
        private string imgPath;
        private string soundNo;
        private string soundPath;
        private int fileIndex;
        private string shuffledPath;
        private string dpath;
        private string appname;
        private string selectedsession;  

        private Amplifier amp;
        private ConsoleCapture.ConsoleFile cfile; 

        ManualResetEvent _evt_StopProc = new ManualResetEvent(false);
        WMCopyData client = null;
        int stimCode;

        private static readonly Random random = new Random();
        private static readonly object syncLock = new object(); /* All the above are to create and the value of the objects to be used later*/
        
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

        private void IAPSbtn_Click(object sender, EventArgs e)  // This function details what happens when the button to select the text files for IAPS is pressed 
        {
            indexNumber = indexNo.Text;
            string appname = indexNumber;
            ResManager rm = BCIApplication.SysResource;
            BCIApplication.SetProtocolName(rm, appname); // used for specify training directory
            LoadConfig(rm);
            dpath = TrainDirSpecForm.GetTrainingPath(rm, true);
            if (dpath == null) return;


            //opens a dialog window to select the text file containing the order of pictures to be shown
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = "../bin/shuffledTextPics";  // Referring to the text files containing the randomised IAPS pictures, the line of text (for example "/pics\LVLA (4.3)\2490.jpg") is combined with "../bin/shuffledTextPics" so that a complete directory is created  
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";   // This is a filter such that only text files are read
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                IAPSFile = openFileDialog1.FileName;
            }

          
            

        }
        private void IADSbtn_Click(object sender, EventArgs e)   // Same function as the IAPS button except for IADS text files
        {
            OpenFileDialog openFileDialog2 = new OpenFileDialog();
            openFileDialog2.InitialDirectory = "../shuffledTextAudio";
            openFileDialog2.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            DialogResult result = openFileDialog2.ShowDialog();
            if (result == DialogResult.OK)
            {
                IADSFile = openFileDialog2.FileName;
            }
            timerText.Text = "Ready";
            timer1.Tick += new EventHandler(timer_Tick);
        }



        private void prepareButton_Click(object sender, EventArgs e)  // function to start the experiment
        {
            totalNo = File.ReadAllLines(@IAPSFile).Length; // this is to find out how many pictures there are from the number of lines of text in the text file
            totalNoPS = totalNo / 40; //Divide by 40 because 4 types, each with 10 blocks
            //creates a file to obtain data
            string timestamp = BCIApplication.TimeStamp;
            dataPath = Path.Combine(Path.GetDirectoryName(dpath), "ratings_" + selectedsession + timestamp + ".txt");
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(dataPath))
            {
                file.WriteLine("start");
            }


            //Logfile
            cfile = ConsoleCapture.StartConsoleFile(Path.Combine(dpath, appname + "_" + selectedsession + timestamp + ".log"));
            AmpContainer.StartAll();
            AmpContainer.StartRecord(dpath, appname);
            
            _evt_StopProc.WaitOne(2000, false);

            Random rnd = new Random();
            amp = AmpContainer.GetAmplifier(0);

            ConsoleOutForm.ShowWindow();

            //sends stimcode 1
            AmpContainer.SendAll(1);
            Console.WriteLine("Stimcode={0}", 1);
            stimCode = 1;

            timer1.Enabled = true;

            stage = 0;
            doRest();   

        }

       

      

        public void timer_Tick(object source, EventArgs e) // this function links the timer displayed on the programme to the actions that the programme should execute
        {
            if (timeLeftInSecs==0)
            {
                stage += 1;
                if (stage == 15)
                {
                    stage = 0;
                }

                if (stage == 0){
                    doRest();
                }
                    
                if (stage == 1){
                    doOpen();
                }

                if (stage > 1 && stage < 14)
                {
                    doShow();
                }

                if (stage == 14)
                {
                    doRate();
                }
           
            }
            timeLeftInSecs -= 1;
            timerText.Text = "Timer:\n" + timeLeftInSecs.ToString();
            
        }

        private void doRest() //rest before a trial starts
        {
            AmpContainer.SendAll(5);
            Console.WriteLine("Stimcode={0}", 5);
            stimCode = 5;

            if (picsDone >= 1)
            {    sendData();
            }
            
            panelA.Visible = false; // false means it cannot be seen
            panelV.Visible = false;
            submitButton.Visible = false;
            timeLeftInSecs = restTime;
            timerText.Text = "Timer:\n" + timeLeftInSecs.ToString();
            picBox.Visible = true;
            picBox.Image = Image.FromFile(src + "/black.jpg");

            // The below if loop is to allow for the first runthrough (where picsDone%(anything) will give 0) to not trigger sessionNo++ since renew is true, and is just to keep switching between the two states of renew (true and false)
            if (picsDone%(totalNoPS*2)== totalNoPS && renew)
            {
                renew = false;
                sessionNo++;
               // stopRecording();
               // timer1.Enabled = true;
            }

            if (picsDone%(totalNoPS*2)==0 && !renew)
            {
                renew = true;
                sessionNo++;
               // stopRecording();
               // timer1.Enabled = true;
            }
            
            sessionNumber.Text = "Session\n" + sessionNo.ToString();

            if (picsDone == totalNoPS*40) // this is for when the session is over
            {
                sessionNumber.Text = "Thank you";
                stopRecording();
            }
            
        }

        private void doOpen() // signals start of a trial
        {
            stimCode = 9;
            AmpContainer.SendAll(stimCode);
            Console.WriteLine("Stimcode={0}", stimCode);
            
            timeLeftInSecs = openTime;
            timerText.Text = "Timer:\n" + timeLeftInSecs.ToString();
            Console.Beep();
            picBox.Visible = true;
            picBox.Image = Image.FromFile(src + "/white.jpg");
            
        }

        private void doShow() // starts showing pictures and play audio files
        {
            picBox.Visible = true;

          // fileIndex = shuffled[picsDone];
            
                using (StreamReader sr = new StreamReader(IAPSFile))
                {
                    imgNo = File.ReadLines(IAPSFile).Skip(picsDone).Take(1).First(); // this means to read the first line of the text file

                    if (stage == 2)
                    {
                        if (imgNo.Contains("HVHA"))
                        {
                            AmpContainer.SendAll(50);
                            Console.WriteLine("Stimcode={0}", 50);
                            stimCode = 50;
                        }

                        if (imgNo.Contains("HVLA"))
                        {
                            AmpContainer.SendAll(100);
                            Console.WriteLine("Stimcode={0}", 100);
                            stimCode = 100;
                        }
                        if (imgNo.Contains("LVHA"))
                        {
                            AmpContainer.SendAll(150);
                            Console.WriteLine("Stimcode={0}", 150);
                            stimCode = 150;
                        }
                        if (imgNo.Contains("LVLA"))
                        {
                            AmpContainer.SendAll(200);
                            Console.WriteLine("Stimcode={0}", 200);
                            stimCode = 200;
                        }
                    }
                    if (stage > 2 && stage < 14)
                    {

                        stimCode = stimCode + 25;
                        AmpContainer.SendAll(stimCode);
                        Console.WriteLine("Stimcode={0}", stimCode);
                        stimCode = stimCode - 25;

                    }

                timeLeftInSecs = showTime;
                timerText.Text = "Timer:\n" + timeLeftInSecs.ToString();

 
                    imgPath = Path.GetFullPath(imgNo);
                    picBox.Height = ClientRectangle.Height * 95 / 100; // this is so that the height of the box showing the pictures is 95% the height of the window
                    picBox.Width = ClientRectangle.Width * 90 / 100;
                    picBox.Image = Image.FromFile(imgPath);
                    picBox.SizeMode = PictureBoxSizeMode.StretchImage; 
                    picsDone++;
                }
                using (StreamReader sr = new StreamReader(IADSFile))
                {

                    soundNo = File.ReadLines(IADSFile).Skip(picsDone - 1).Take(1).First();
                    soundPath = Path.GetFullPath(soundNo);
                    SoundPlayer sndplayer = new SoundPlayer(soundPath);
                    sndplayer.Play();
                }


                /* if (File.Exists(@"C:\Users\Nicholas\Documents\Science\JC 1\CenTad\Tiffany work\Stimuli\IAPS\HVHA (5.8)\" + imgNo + ".jpg"))
                    {
                        imgPath = @"C:\Users\Nicholas\Documents\Science\JC 1\CenTad\Tiffany work\Stimuli\IAPS\HVHA (5.8)\" + imgNo + ".jpg";
                        picBox.Image = Image.FromFile(imgPath);
                        picsDone++;
                    }

                    if (File.Exists(@"C:\Users\Nicholas\Documents\Science\JC 1\CenTad\Tiffany work\Stimuli\IAPS\HVLA (6,4)\" + imgNo + ".jpg"))
                    {
                        imgPath = @"C:\Users\Nicholas\Documents\Science\JC 1\CenTad\Tiffany work\Stimuli\IAPS\HVLA (6,4)\" + imgNo + ".jpg";
                        picBox.Image = Image.FromFile(imgPath);
                        picsDone++;
                    }

                    if (File.Exists(@"C:\Users\Nicholas\Documents\Science\JC 1\CenTad\Tiffany work\Stimuli\IAPS\LVHA (4,6)\" + imgNo + ".jpg"))
                    {
                        imgPath = @"C:\Users\Nicholas\Documents\Science\JC 1\CenTad\Tiffany work\Stimuli\IAPS\LVHA (4,6)\" + imgNo + ".jpg";
                        picBox.Image = Image.FromFile(imgPath);
                        picsDone++;
                    }

                   if (File.Exists(@"C:\Users\Nicholas\Documents\Science\JC 1\CenTad\Tiffany work\Stimuli\IAPS\LVLA (4.3)\" + imgNo + ".jpg"))
                    {
                        imgPath = @"C:\Users\Nicholas\Documents\Science\JC 1\CenTad\Tiffany work\Stimuli\IAPS\LVLA (4.3)\" +imgNo + ".jpg";
                        picBox.Image = Image.FromFile(imgPath);
                        picsDone
                 * 
                    }
                 */
                int picsDoneTR = picsDone - totalNoPS * (sessionNo - 1);
                trialNo.Text = "Trial No:\n" + picsDoneTR.ToString() + "/" + totalNoPS.ToString();
            
        }

        private void doRate()  // this is the rating part of each trial
        {
            AmpContainer.SendAll(13);
            Console.WriteLine("Stimcode={0}", 13);
            stimCode = 13;

            trackBarA.Value = 5;
            trackBarV.Value = 5;
            timeLeftInSecs = rateTime;
            timerText.Text = "Timer:\n" + timeLeftInSecs.ToString();
            picBox.Visible = false;
            panelA.Visible = true;
            panelV.Visible = true;
            submitButton.Visible = true;
            if (timeLeftInSecs == 0)
            {
                using (StreamReader sr = new StreamReader(IAPSFile))
                {
                    imgNo = File.ReadLines(IAPSFile).Skip(picsDone).Take(1).First();
                    if (imgNo.Contains("HVHA"))
                    {
                        AmpContainer.SendAll(99);
                        Console.WriteLine("Stimcode={0}", 99);
                        stimCode = 99;
                    }

       
                    if (imgNo.Contains("HVLA"))
                    {
                        AmpContainer.SendAll(149);
                        Console.WriteLine("Stimcode={0}", 149);
                        stimCode = 149;
                    }
                    if (imgNo.Contains("LVHA"))
                    {
                        AmpContainer.SendAll(199);
                        Console.WriteLine("Stimcode={0}", 199);
                        stimCode = 199;
                    }
                    if (imgNo.Contains("LVLA"))
                    {
                        AmpContainer.SendAll(249);
                        Console.WriteLine("Stimcode={0}", 249);
                        stimCode = 249;
                    }
                }
            }
        }
        private void submitButton_Click(object sender, EventArgs e)
        {
            timeLeftInSecs = 0;
        }

        private void Pausebtn_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            AmpContainer.SendAll(17);
            Console.WriteLine("Stimcode={0}", 17);
            stimCode = 17;
        }

        private void Continuebtn_Click(object sender, EventArgs e)
        {
            timer1.Start();
            AmpContainer.SendAll(21);
            Console.WriteLine("Stimcode={0}", 21);
            stimCode = 21;
        }
        private void sendData() // this sends data to the text file that is recording the ratings of each trial
        {
            
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(dataPath, true))
            {
                string imgName = Path.GetFileName(imgNo);
                string imgType = Path.GetFileName(Path.GetDirectoryName(imgNo));
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

        // the following are some useless functions

        private void loadSBtn_Click(object sender, EventArgs e)
        {

        }

        private void submitButton_Click_1(object sender, EventArgs e)
        {
            timeLeftInSecs = 0;
        }

        private void picBox_Click(object sender, EventArgs e)
        {

        }

        private void sessionNumber_Click(object sender, EventArgs e)
        {

        }

        private void loadSBtn_Click_1(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }
private void PicLoader_Load(object sender, EventArgs e)
        {

        }

private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
{
    selectedsession = comboBox1.GetItemText(comboBox1.SelectedItem); 
}

private void btnStop_Click_1(object sender, EventArgs e)
{
    stopRecording();
}






        
        

        
    }


}
/**
This is a previous programme created by our predecessor, Tiffany

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
