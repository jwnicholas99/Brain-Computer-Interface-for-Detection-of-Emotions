using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BCILib.App;
using BCILib.Util;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace BCILib.MotorImagery
{
    public class ExternalRehabGame : FeedbackInterface
    {
        #region FeedbackInterface Members

        ManualResetEvent evt_proc;
        bool FeedbackInterface.StartGame(ManualResetEvent evt, BCITask mtype, int ntrials, params int[] clabels)
        {
            evt_proc = evt;
            if (!StartGameIfNotFound()) return ErrorClientNotFound();
            wmclient.SendClient(GameCommand.StartGame, new[] {(int)mtype, ntrials}.Concat(clabels).ToArray());
            error_reported = false;
            return true;
        }

        bool error_reported = false;

        bool ErrorClientNotFound()
        {
            if (evt_proc != null) evt_proc.Set();
            if (!error_reported) {
                MessageBox.Show("ExternalRehabGame not found!", "MOgreLowerLibRehab");
                error_reported = true;
            }
            return false;
        }

        void FeedbackInterface.StartTrial(MIAction act, int itrial)
        {
            if (wmclient.GetAllGUIWindows() <= 0) {
                ErrorClientNotFound();
                return;
            }
            wmclient.SendClient(GameCommand.StartTrial, (int) act, itrial);
        }

        void FeedbackInterface.SetMIStep(MI_STEP mI_STEP, int time, params int[] args)
        {
            if (wmclient.GetAllGUIWindows() <= 0) {
                ErrorClientNotFound();
                return;
            }
            wmclient.SendClient(GameCommand.MIStep, new [] {(int) mI_STEP, time}.Concat(args).ToArray());
        }

        void FeedbackInterface.Close()
        {
            wmclient.SendClient(GameCommand.StopGame);
        }

        #endregion

        WMCopyData wmclient = null;

        public ExternalRehabGame(IntPtr ptr, string game_id)
        {
            wmclient = new WMCopyData(game_id, ptr);
        }

        public bool StartGameIfNotFound()
        {
            if (wmclient.GetAllGUIWindows() > 0) return true;

            string game_path = BCIApplication.GetGamePath(wmclient.Property, true, false);
            if (!File.Exists(game_path)) {
                return false;
            }
            // start program
            ProcessStartInfo pinf = new ProcessStartInfo();
            pinf.FileName = game_path;
            pinf.WorkingDirectory = Path.GetDirectoryName(game_path);
            Console.WriteLine("Start {0} in {1}", pinf.FileName, pinf.WorkingDirectory);

            Process game_proc = new Process();
            game_proc.StartInfo = pinf;
            game_proc.Start();

            // wait for game start up
            int t0 = BCIApplication.ElaspedMilliSeconds;
            while (BCIApplication.ElaspedMilliSeconds - t0 < 20000) {
                if (wmclient.GetAllGUIWindows() > 0) return true;
                if (game_proc.HasExited) return false;
                Thread.Sleep(100);
            }

            return false;
        }
    }
}
