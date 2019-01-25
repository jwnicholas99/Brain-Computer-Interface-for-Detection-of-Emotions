using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using BCILib.Util;

namespace BCILib.Amp
{
    public class FileSimulator : LiveAmplifier
    {
        string cnt_fn = null;
        string _chan_name_str = null;

        public FileSimulator()
        {
        }

        public override bool IsRealAmplifier()
        {
            return false;
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

        private int rd_pos = 0;

        public override void SendStimCode(int evt)
        {
        }

        protected override void ReceiveDataLoop()
        {
            rd_pos = Rd_GetPos();

            using (FileStream fs = File.OpenRead(Path.Combine(BCILib.App.BCIApplication.RootPath, cnt_fn))) {
                BinaryReader br = new BinaryReader(fs);

                ReadHeader(br);

                int last_evt = 0;

                int spl_sz = header.SampleSize;
                byte[] data = new byte[spl_sz * header.blk_samples];

                PoolInitialize();

                while (bRunning) {
                    // read one block of data
                    int rl = br.Read(data, 0, data.Length);
                    if (rl == data.Length) {
                        int epos = spl_sz - 4;
                        bool ProcCode = false;

                        for (int spl = 0; spl < header.blk_samples && !ProcCode && rd_pos < Rd_GetPos(); spl++)
                        {
                            int evt = BitConverter.ToInt32(data, epos) & 255;
                            epos += spl_sz;
                            last_evt = evt;
                            if (evt != 0) {
                                while (bRunning) {
                                    if (proc_codes != null) {
                                        ProcCode = proc_codes.Contains(evt);
                                        break;
                                    }
                                    if (rd_pos >= Rd_GetPos()) break;
                                    Thread.Sleep(20);
                                    TriggerDateRecv();
                                }
                            }
                        }

                        if (proc_codes == null) {
                            // wait for pool data to be read
                            int psz = rd_pos + pool_size - Rd_GetPos();
                            while (bRunning && psz < header.blk_samples) {
                                Thread.Sleep(15);
                                psz = rd_pos + pool_size - Rd_GetPos();
                                TriggerDateRecv();
                            }

                            if (rtime_waiting)
                            {
                                // wait for real amplifier time
                                int wt = header.blk_samples * 1000 / header.samplingrate;
                                if (wt > 0) Thread.Sleep(wt);
                            }
                        }
                        PoolAddBuffer(data);
                        if (ProcCode) {
                            proc_codes = null;
                            rtime_waiting = true;

                            // make sure at leat one blk can be read
                            int npos = Rd_GetPos() - pool_size + header.blk_samples;
                            if (rd_pos < npos) {
                                rd_pos = npos;
                            }
                        }
                    } else {
                        break;
                    }
                }
            }
            bRunning = false;
        }

        public override int Rd_GetBuf(float[] fbuf, ref int rpos, int nsamples, bool takeLatest)
        {
            int rv = base.Rd_GetBuf(fbuf, ref rpos, nsamples, takeLatest);
            rd_pos = rpos;
            if (rv < nsamples) rd_pos += header.blk_samples;

            return rv;
        }

        private bool ReadHeader(BinaryReader br)
        {
            // read header
            header.nchan = br.ReadInt32();
            header.nevt = br.ReadInt32();
            header.blk_samples = br.ReadInt32();
            header.samplingrate = br.ReadInt32();
            header.datasize = br.ReadInt32();
            header.resolution = br.ReadSingle();

            LogMessage("FileSimulator: {0}: {1}", cnt_fn, header.ToString());

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
            try {
                var fn = Path.Combine(BCILib.App.BCIApplication.RootPath, cnt_fn);
                using (FileStream fs = File.OpenRead(fn)) {
                    BinaryReader br = new BinaryReader(fs);

                    if (!ReadHeader(br)) {
                        LogMessage("FileSimulator Reading CNT-head error!");
                        return false;
                    }
                }
            }
            catch (Exception ex) {
                Console.WriteLine("Error: {0} in reading {1}", ex.Message, cnt_fn);
                return false;
            }

            return true;
        }

        public override bool Configure()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.RestoreDirectory = true;
            dlg.Title = "Set FileSimulator EEG CNT File";
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
            if (!Initialize()) {
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
            if (!Initialize()) {
                //throw new Exception("FileSimulator Reading CNT-head error!");
            }
        }

        List<int> proc_codes = null;
        bool rtime_waiting = false;

        public void ProceedToEvent(List<int> evt_codes)
        {
            proc_codes = evt_codes;
            if (evt_codes == null) rtime_waiting = false;
        }
    }
}
