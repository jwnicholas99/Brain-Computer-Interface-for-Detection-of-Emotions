using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BCILib.App;

namespace BCILib.MotorImagery
{
    public interface FeedbackInterface
    {
        bool StartGame(System.Threading.ManualResetEvent evt_proc, BCITask mtype, int ntrials, params int[] clabels);
        void StartTrial(MIAction act, int itrial);
        void SetMIStep(MI_STEP mI_STEP, int time, params int[] args);
        void Close();
    }
}
