using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using System.Threading;
using BCILib.Util;
using BCILib.Amp;
using System.Collections;


namespace BCILib.Amp
{
    public class FakeAmplifier: LiveAmplifier
    {
        public override string GetChannelNameString()
        {
            if (!string.IsNullOrEmpty(channel_namestr)) return channel_namestr;
            else return string.Join(",", Enumerable.Range(1, header.nchan).Select(x => "ch" + x).ToArray());
        }

        public override bool IsRealAmplifier()
        {
            return false;
        }

        public FakeAmplifier()
        {
            header.nchan = 4;
            header.nevt = 1;
            header.blk_samples = 10;
            header.samplingrate = 250;
            header.resolution = 0; // 0.036F;
            header.datasize = 4;
        }

        public override bool Initialize()
        {
            return true;
        }

        protected override void ReceiveDataLoop()
        {
            byte[] buf = new byte[header.BlkSize];
            Random rnd = new Random();

            int ctime = 1000 * header.blk_samples / header.samplingrate;
            int vmax = 50; // (int)(50 / header.resolution);
            int last_evt = 0;

            long t0 = HRTimer.GetTimestamp();
            int wt;
            while (bRunning) {
                // construct a buffer
                int off = 0;

                for (int bi = 0; bi < header.blk_samples; bi++) {
                    for (int ci = 0; ci < header.nchan; ci++, off += 4) {
                        float fv = (float) rnd.NextDouble() * vmax;
                        BitConverter.GetBytes(fv).CopyTo(buf, off);
                    }
                    int evt = 0;
                    if (last_evt == 0 && code_que.Count > 0) {
                        var scode = code_que.Peek();
                        if (scode.splno <= Rd_GetPos() + bi) {
                            evt = scode.code;
                            code_que.Dequeue();
                        }
                    }
                    BitConverter.GetBytes(evt).CopyTo(buf, off);
                    off += 4;
                    last_evt = evt;
                }

                PoolAddBuffer(buf);
                UpdateActSplRate();

                wt = HRTimer.DeltaMilliseconds(t0);
                wt = ctime - wt;
                if (wt > 0) Thread.Sleep(wt);
                t0 = HRTimer.GetTimestamp();
            }
        }

        public override bool Configure()
        {
            FakeAmplifier_Config cfg = new FakeAmplifier_Config();
            cfg.NumChannels = header.nchan;
            cfg.SampleRate = header.samplingrate;
            cfg.ChannelNameString = GetChannelNameString();

            if (cfg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                header.nchan = cfg.NumChannels;
                header.samplingrate = cfg.SampleRate;
                return true;
            }

            return false;
        }

        protected override void SaveConfigSpecial(ResManager rm)
        {
            rm.SetConfigValue(ID, "NumChannel", header.nchan);
            rm.SetConfigValue(ID, "SampleRate", header.samplingrate);
        }

        private string channel_namestr = null;
        protected override void SetSpecialConfig(ResManager rm)
        {
            rm.GetConfigValue(ID, "NumChannel", ref header.nchan);
            rm.GetConfigValue(ID, "SampleRate", ref header.samplingrate);
            channel_namestr = rm.GetConfigValue(ID, "ChannelNames");
        }
    }
}
