using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BCILib.App;
using BCILib.Util;
using System.IO;

namespace BCILib.EngineProc
{
    public abstract class BCICSEngine: BCIEngine
    {
        public override int NumChannelUsed
        {
            get
            {
                return _ch_used != null ? _ch_used.Length : 0;
            }
        }

        public override int NumSampleUsed
        {
            get
            {
                return _numSampleUsed;
            }
        }

        public int SamplingRate = 0;
        protected double _resolution = 1.0;
        protected string[] _ch_names = null;
        protected int[] _ch_used = null;
        protected int _numSampleUsed = 0;

        public int[] SelectedChannels
        {
            get
            {
                return _ch_used;
            }
        }

        protected bool SetCommParameter(string vname, string arg, TextReader tr)
        {
            if (vname == "Sampling_Rate") {
                int.TryParse(arg, out SamplingRate);
            } else if (vname == "EEG_Resolution") {
                double.TryParse(arg, out _resolution);
            } else if (vname == "EEG_Resolution_Hex") {
                _resolution = NumberConv.HexToDouble(arg);
            } else if (vname == "Channel_Order") {
                int nch = 0;
                int.TryParse(arg, out nch);
                string line = tr.ReadLine();
                _ch_names = line.Split(StringTool.SEP, StringSplitOptions.RemoveEmptyEntries);
            } else if (vname == "Channel_Used") {
                int nused = 0;
                int.TryParse(arg, out nused);
                string line = tr.ReadLine();
                string[] cl = line.Split(StringTool.SEP, StringSplitOptions.RemoveEmptyEntries);
                _ch_used = new int[nused];
                for (int i = 0; i < nused; i++) {
                    _ch_used[i] = Array.IndexOf(_ch_names, cl[i]);
                }
            } else {
                return false;
            }

            return true;
        }
    }
}
