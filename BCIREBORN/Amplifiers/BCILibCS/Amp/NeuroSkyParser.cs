using System;
using System.Collections.Generic;
using System.Text;

using System.IO.Ports;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Linq;

namespace BCILib.Amp
{
    public class NeuroSkyParser : LiveAmplifier
    {
        string port_name = null;

/* Parser types */
        enum ParserType
        {
            PARSER_TYPE_NULL = 0x00,
            PARSER_TYPE_PACKETS = 0x01,    /* Stream bytes as ThinkGear Packets */
            PARSER_TYPE_2BYTERAW = 0x02    /* Stream bytes as 2-byte raw data */
        }

/* Data CODE definitions */
        private const int PARSER_CODE_BATTERY = 0x01;
        private const int PARSER_CODE_POOR_QUALITY = 0x02;
        private const int PARSER_CODE_ATTENTION = 0x04; 
        private const int PARSER_CODE_MEDITATION = 0x05;
        private const int PARSER_CODE_8BITRAW_SIGNAL = 0x06;
        private const int PARSER_CODE_RAW_MARKER = 0x07;

        private const int PARSER_CODE_RAW_SIGNAL = 0x80;
        private const int PARSER_CODE_EEG_POWERS = 0x81;
        private const int PARSER_CODE_ASIC_EEG_POWER_INT = 0x83;
            //eight big-endian 3-byte unsigned integer values
            //representing delta, theta, low-alpha, high-alpha, low-beta, high-beta,low-gamma, and mid-gamma EEG band
            //power values

        private const int PARSER_CODE_RAW_WAVE_VALUE = 0x90;
        //The Data Value consists of two bytes, and represents a single raw wave sample. Its value is a signed
        //16-bit integer that ranges from -32768 to 32767. e 􀃫rst byte of the Value represents the high-order
        //bits of the twos-compliment value, while the second byte represents the low-order bits. To reconstruct
        //the full raw wave value, simply shift the 􀃫rst byte left by 8 bits, and bitwise-or with the second byte:

        private const int SYNC = 0xAA;  /* Syncronization byte */
        private const int EXCODE = 0x55;  /* EXtended CODE level byte */

        public NeuroSkyParser()
        {
            header.nchan = 3;
            header.nevt = 1;
            header.blk_samples = 8;
            header.resolution = 0;
            header.datasize = 4;
            header.samplingrate = 256;
        }

        static List<string> port_list = new List<string>();

        static string GetPortName()
        {
            string pname = null;
            lock (port_list) {
                if (port_list.Count == 0) {
                    port_list.AddRange(SerialPort.GetPortNames());
                    if (port_list.Count == 0) return null;
                }
                pname = port_list[0];
                port_list.Remove(pname);
            }
            if (string.IsNullOrEmpty(pname)) return null;

            int len = pname.Length;
            for (int i = 0; i < pname.Length; i++) {
                if (char.IsNumber(pname, len - i - 1)) {
                    return i == 0 ? pname : pname.Substring(0, len - i);
                }
            }
            return null;
        }

        protected override void ReceiveDataLoop()
        {
            buf = new byte[(header.nchan + 1) * header.blk_samples * 4];
            bpos = 0;

            byte[] payload = new byte[169];
            int checksum, c, pl;

            SerialPort port = null;

            int error_length = 0;
            noice = 200;
            Status = AmpStatus.Checking;

            string port_name = this.port_name;
            while (bRunning) {
                if (port == null) {
                    if (string.IsNullOrEmpty(port_name)) port_name = GetPortName();
                    if (!string.IsNullOrEmpty(port_name)) {
                        try {
                            port = new SerialPort(port_name);
                            port.Open();
                            port.ReadTimeout = 500;
                        } catch {
                            if (port != null) {
                                try {
                                    port.Close();
                                } catch { }
                                port = null;
                            }
                        }
                    }
                }

                if (port == null) {
                    if (Status != AmpStatus.Off) {
                        Status = AmpStatus.Off;
                    }
                    Thread.Sleep(100);
                    port_name = null;
                    continue;
                }

                try {
                    // read synchronization bytes
                    c = port.ReadByte();
                    if (c != SYNC) continue;

                    c = port.ReadByte();
                    if (c != SYNC) continue;

                    pl = 0;
                    while ((pl = port.ReadByte()) == SYNC && bRunning) ;
                    if (pl > payload.Length || pl <= 0) {
                        error_length += pl <= 0 ? 0 : pl;
                        continue;
                    }

                    port.Read(payload, 0, pl);
                    checksum = 0;
                    for (int i = 0; i < pl; i++) checksum += payload[i];
                    checksum = ~checksum & 0xFF;

                    if (checksum != port.ReadByte()) {
                        error_length += pl;
                        if (error_length > payload.Length * 5) {
                            throw new Exception("Serial port wrong, consider change?");
                        }
                        continue;
                    }
                } catch (Exception ex) {
                    Console.WriteLine("NeuroskyParser({0}): error={1}", port_name, ex.Message);
                    if (port != null) {
                        try {
                            port.Close();
                        } catch { }
                    }

                    if (Status != AmpStatus.Off) {
                        Status = AmpStatus.Off;
                    }
                    port = null;
                    continue;
                }

                if (port_name != this.port_name) {
                    // Update port number
                    this.port_name = port_name;
                    SaveConfig(BCILib.App.BCIApplication.AppResource);
                }

                error_length = 0;
                ParsePayload(payload, pl);
            }

            if (port != null) {
                try {
                    port.Close();
                } catch {
                }
                port = null;
            }
        }

        private void ParsePayload(byte[] payload, int pl) {
            int byteParsed = 0;
            int extendedCodeLevel;
            int code;
            int length = 0;

            while (byteParsed < pl) {
                extendedCodeLevel = 0;
                while (payload[byteParsed] == EXCODE) {
                    extendedCodeLevel++;
                    byteParsed++;
                }
                code = payload[byteParsed++];

                if ((code & 0x80) != 0) length = payload[byteParsed++];
                else length = 1;

                /* TODO: Based on the extendedCodeLevel, code, length,
                * and the [CODE] Definitions Table, handle the next
                * "length" bytes of data from the payload as
                * appropriate for your application.
                */
                //printf("EXCODE level: %d CODE: 0x%02X length: %d\n",
                //extendedCodeLevel, code, length);
                //printf("Data value(s):");
                //for (i = 0; i < length; i++) {
                //    printf(" %02X", payload[bytesParsed + i] & 0xFF);
                //}
                //printf("\n");
                /* Increment the bytesParsed by the length of the Data Value */
                if (extendedCodeLevel == 0) {
                    switch (code) {
                        /* [CODE]: ATTENTION eSense */
                        case PARSER_CODE_ATTENTION: //(0x04):
                            atv = payload[byteParsed];
                            //LogMessage("Attention Level({0}):{1}", length, atv);
                            break;
                        /* [CODE]: MEDITATION eSense */
                        case PARSER_CODE_MEDITATION: // (0x05):
                            //LogMessage("Meditation Level({0}):{1}", length, payload[byteParsed]);
                            break;
                        case PARSER_CODE_RAW_SIGNAL: // ox80
                            int hi = payload[byteParsed];
                            int lo = payload[byteParsed + 1];

                            //// output
                            //{
                            //    float fv = ((hi << 8) + lo);
                            //    byte[] buf = new byte[8];
                            //    BitConverter.GetBytes(fv).CopyTo(buf, 0);
                            //    BitConverter.GetBytes(evt).CopyTo(buf, 4);
                            //}
                            break;
                        case PARSER_CODE_POOR_QUALITY:
                            int q = payload[byteParsed++];
                            if (noice != q) {
                                noice = q;
                                if (noice > 10 && Status != AmpStatus.Checking) {
                                    Status = AmpStatus.Checking;
                                } else if (Status != AmpStatus.Connected) {
                                    Status = AmpStatus.Connected;
                                }
                            }
                            break;
                        /* Other [CODE]s */
                        case PARSER_CODE_RAW_WAVE_VALUE:
                            ushort c1 = payload[byteParsed++];
                            ushort c2 = payload[byteParsed++];
                            int t1 = (short)((c1 << 8) | c2);

                            //skip the counter
                            ushort counter1 = payload[byteParsed++];

                            c1 = payload[byteParsed++];
                            c2 = payload[byteParsed++];
                            //int t2 = (int)(c1 * 256 + c2);
                            //if (t2 >= 32768)
                            //    t2 = t2 - 32768;

                            ushort counter2 = payload[byteParsed++];
                            int t2 = (short)((c1 << 8) | c2);

                            // fill sample
                            int evt = 0;
                            if (last_evt == 0 && code_que.Count > 0) {
                                var scode = code_que.Peek();
                                if (scode.splno <= Rd_GetPos() + bpos / header.SampleSize) {
                                    evt = scode.code;
                                    code_que.Dequeue();
                                }
                            }
                            last_evt = evt;

                            float fv = t1;
                            BitConverter.GetBytes(fv).CopyTo(buf, bpos);
                            bpos += 4;
                            fv = t2;
                            BitConverter.GetBytes(fv).CopyTo(buf, bpos);
                            bpos += 4;
                            fv = noice;
                            BitConverter.GetBytes(fv).CopyTo(buf, bpos);
                            bpos += 4;
                            BitConverter.GetBytes(evt).CopyTo(buf, bpos);
                            bpos += 4;
                            if (bpos >= buf.Length) {
                                PoolAddBuffer(buf);
                                bpos = 0;
                            }

                            break;
                        default:
                            Console.WriteLine("EXCODE level: {0} CODE: 0X{1:X2} vLength: {2}",
                                extendedCodeLevel, code, length);
                            break;
                    }
                }
                byteParsed += length;
            }
            UpdateActSplRate();
        }

        float atv = 0;
        int noice = 0;
        int last_evt = 0;
        byte[] buf = null;
        int bpos = 0;
    
        internal void TryParse()
        {
            byte[] buf = File.ReadAllBytes("NeuroSkyParser.dat");
            int pos = 0;

            while (pos < buf.Length) {
                if (buf[pos++] != SYNC) continue;
                if (pos >= buf.Length) break;
                if (buf[pos++] != SYNC) continue;
                //Console.WriteLine("Frame starting pos: {0}", pos);

                while (buf[pos] == SYNC) {
                    pos++;
                }
                if (buf[pos] > 169) {
                    pos++;
                    continue;
                }

                int pl = buf[pos++];
                if (pos + pl + 1> buf.Length) {
                    break;
                }

                byte[] payload = new byte[pl];
                Array.Copy(buf, pos, payload, 0, pl);
                pos += pl;

                int checksum = 0;
                for (int i = 0; i < pl; i++) checksum += payload[i];
                checksum &= 0xFF;
                checksum = ~checksum & 0xFF;

                if (checksum != buf[pos++]) {
                    continue;
                }
                ParsePayload(payload, pl);
            }
        }

        public override string GetChannelNameString()
        {
            return string.Join(",", Enumerable.Range(1, header.nchan -1).Select(x=>x.ToString("Ch#")).ToArray())
                 + ",N";
        }

        public override bool Initialize()
        {
            return true;
        }

        public override bool Configure()
        {
            ThinkGearConfigForm dlg = new ThinkGearConfigForm();
            dlg.Port = port_name;
            dlg.NumChannels = header.nchan - 1;
            //dlg.BaudRate = baudRate;
            dlg.BlkSize = header.blk_samples;
            //dlg.Interpolate = _interpolate;
            //dlg.SaveLog = _savelog;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                port_name = dlg.Port;
                //baudRate = dlg.BaudRate;
                header.nchan = dlg.NumChannels + 1;
                header.blk_samples = dlg.BlkSize;
                //_interpolate = dlg.Interpolate;
                //_savelog = dlg.SaveLog;

                PoolInitialize();
                return true;
            }
            return false;
        }

        protected override void SaveConfigSpecial(BCILib.Util.ResManager rm)
        {
            rm.SetConfigValue(ID, "Port", port_name);
            rm.SetConfigValue(ID, "NumRawData", (header.nchan - 1).ToString());
            //rm.SetConfigValue(ID, "BaudRate", baudRate.ToString());
            rm.SetConfigValue(ID, "BlkSize", header.blk_samples.ToString());
            //rm.SetConfigValue(ID, "Interpolate", _interpolate.ToString());
            //rm.SetConfigValue(ID, "SaveLog", _savelog.ToString());
        }

        protected override void SetSpecialConfig(BCILib.Util.ResManager rm)
        {
            rm.GetConfigValue(ID, "Port", ref port_name);
            int nraw = 1;
            rm.GetConfigValue(ID, "NumRawData", ref nraw);
            header.nchan = nraw + 1;
            //rm.GetConfigValue(ID, "BaudRate", ref baudRate);
            rm.GetConfigValue(ID, "BlkSize", ref header.blk_samples);
            //rm.GetConfigValue(ID, "Interpolate", ref _interpolate);
            //rm.GetConfigValue(ID, "SaveLog", ref _savelog);
        }

        public override string DevName
        {
            get
            {
                return "NSParser:" + port_name;
            }
        }
    }
}
