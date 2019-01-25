using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System.Threading;

namespace BCILib.Amp
{
    public abstract class LiveAmplifier : Amplifier
    {
        /// <summary>
        /// Get amplifer header information
        /// </summary>
        /// <returns></returns>
        protected abstract void ReceiveDataLoop();

        public override bool IsRealAmplifier()
        {
            return true;
        }

        public override bool IsAlive
        {
            get
            {
                return (thd != null && thd.IsAlive);
            }
        }

        protected bool bRunning = false;

        Thread thd = null;

        protected override bool StartRead()
        {
            if (IsAlive) return false;
            if (!Initialize()) return false;
            thd = new Thread(new ThreadStart(RecvMain));
            thd.Name = "Amplifier: " + DevName;
            thd.IsBackground = true;
            bRunning = true;
            thd.Start();
            return true;
        }

        private void RecvMain()
        {
            Status = AmpStatus.Connected;
            ReceiveDataLoop();
            Status = AmpStatus.Off;
        }

        protected override void StopRead()
        {
            bRunning = false;
            if (thd != null && thd.IsAlive) {
                thd.Join(1000);
            }
        }
    }
}
