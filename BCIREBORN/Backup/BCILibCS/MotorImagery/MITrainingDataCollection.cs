using System;
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
    public partial class MITrainingDataCollection : Form
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
            get{ return this._stimCode; }
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

        public MITrainingDataCollection()
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
}
