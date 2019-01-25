using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using BCILib.Util;

namespace BCILib.Amp
{
    internal class LiveSimulator : LiveAmplifier
    {
        string cnt_fn = null;
        FileStream cnt_fs = null;

        string _chan_name_str = null;

        public LiveSimulator()
        {
        }

        public override bool IsRealAmplifier()
        {
            return false;
        }

        public LiveSimulator(string cnt_fn)
        {
            this.cnt_fn = cnt_fn;
            if (!ReadHeader()) {
                throw new Exception("LiveSimulator Reading CNT-head error!");
            }
        }

        public override string GetChannelNameString()
        {
            if (_chan_name_str != null) return _chan_name_str;

            switch (header.nchan) {
                case 66:
                    return NeuroScan.SYNAMPS2_CHANNELS;
                case 40:
                    return NeuroScan.NUAMPS_CHANNELS;
                case 8:
                    return BioRadio150.ChannelNameString;
                default:
                    return string.Join(",", Enumerable.Range(1, header.nchan).Select(x => "ch" + x).ToArray());
            }
        }

        public override void SendStimCode(int evt)
        {
        }

        protected override void ReceiveDataLoop()
        {
            int wtime = header.blk_samples * 1000 / header.samplingrate;
            if (cnt_fs == null) return;

            BinaryReader br = new BinaryReader(cnt_fs);
            long t0 = DateTime.Now.Ticks;
            long t1;
            byte[] data = new byte[(header.nchan + 1) * header.blk_samples * 4];
            while (bRunning) {
                int rl = br.Read(data, 0, data.Length);
                if (rl != data.Length) {
                    break;
                }
                PoolAddBuffer(data);

                t1 = DateTime.Now.Ticks;
                int wt = wtime - ((int)((t1 - t0 + 999999) / 1000000));
                if (wt > 0) {
                    System.Threading.Thread.Sleep(wt);
                }
                t0 = t1;
            }

            if (cnt_fs != null) {
                LogMessage("LiveSimulator closed: {0}", cnt_fn);
                cnt_fs.Close();
                cnt_fs = null;
            }
        }

        private bool ReadHeader()
        {
            if (cnt_fs == null) {
                var fn = Path.Combine(BCILib.App.BCIApplication.RootPath, cnt_fn);
                if (string.IsNullOrEmpty(fn) || !File.Exists(fn)) {
                    LogMessage("LiveSimilator: file not exist: {0}", fn);
                    return false;
                }
                cnt_fs = File.OpenRead(fn);
            }
            else {
                cnt_fs.Seek(0, SeekOrigin.Begin);
            }

            BinaryReader br = new BinaryReader(cnt_fs);

            // read header
            header.nchan = br.ReadInt32();
            header.nevt = br.ReadInt32();
            header.blk_samples = br.ReadInt32();
            header.samplingrate = br.ReadInt32();
            header.datasize = br.ReadInt32();
            header.resolution = br.ReadSingle();

            LogMessage("LiveSimulator: {0}: {1}", cnt_fn, header.ToString());

            // ccwang 20120109
            string magic = "I2REEGCNT";
            long pos = br.BaseStream.Position;
            int spz = header.nchan;
            char[] buf = new char[magic.Length];
            int n0 = 0;
            for (int i = 0; i < buf.Length; i++) {
                if (header.resolution == 0) {
                    buf[i] = (char)br.ReadSingle();
                } else {
                    buf[i] = (char)br.ReadInt32();
                }
                n0++;
                if (n0 % spz == 0) br.ReadInt32();
            }
            string rword = new string(buf);
            if (string.Compare(rword, magic, true) == 0) {
                int[] vl = new int[3];
                for (int i = 0; i < vl.Length; i++) {
                    if (header.resolution == 0) {
                        vl[i] = (char)br.ReadSingle();
                    } else {
                        vl[i] = (char)br.ReadInt32();
                    }
                    n0++;
                    if (n0 % spz == 0) br.ReadInt32();
                }

                char[] rch = new char[vl[2]];
                for (int i = 0; i < rch.Length; i++) {
                    if (header.resolution == 0) {
                        rch[i] = (char)br.ReadSingle();
                    } else {
                        rch[i] = (char)br.ReadInt32();
                    }
                    n0++;
                    if (n0 % spz == 0) br.ReadInt32();
                }

                int nl = magic.Length + 3 + vl[2];
                nl = (nl + spz - 1) / spz * spz;
                while (n0 < nl) {
                    br.ReadInt32();
                    n0++;
                    if (n0 % spz == 0) br.ReadInt32();
                }
                _chan_name_str = new string(rch);
            } else {
                br.BaseStream.Seek(pos, SeekOrigin.Begin);
            }

            return (header.nchan > 0 && header.nchan < 1024);
        }

        public override bool Initialize()
        {
            return ReadHeader();
        }

        public override bool Configure()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.RestoreDirectory = true;
            dlg.Title = "Set LiveSimulator File";
            dlg.Filter = "cnt File(*.cnt)|*.cnt|All Files(*.*)|*.*";

            if (!string.IsNullOrEmpty(cnt_fn)) {
                var fn = Path.Combine(BCILib.App.BCIApplication.RootPath, cnt_fn);
                dlg.FileName = Path.GetFileName(fn);
                dlg.InitialDirectory = Path.GetDirectoryName(fn);
            } else {
                dlg.InitialDirectory = BCILib.App.BCIApplication.UserPath;
            }

            if (dlg.ShowDialog() != DialogResult.OK) return false;

            string sfn = cnt_fn;
            cnt_fn = BCILib.App.BCIApplication.GetRelativePath(dlg.FileName, BCILib.App.BCIApplication.RootPath);
            if (!ReadHeader()) {
                cnt_fn = sfn;
                return false;
            }
            return true;
        }

        protected override void SaveConfigSpecial(ResManager rm)
        {
            rm.SetConfigValue(ID, "InputFileName", cnt_fn);
        }

        protected override void SetSpecialConfig(ResManager rm)
        {
            rm.GetConfigValue(ID, "InputFileName", ref cnt_fn);
            if (!ReadHeader()) {
                //throw new Exception("LiveSimulator Reading CNT-head error!");
            }
        }
    }
}
