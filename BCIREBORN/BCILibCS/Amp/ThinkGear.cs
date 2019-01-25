using System;
using System.Collections.Generic;
using System.Text;

using System.Collections;
using System.Threading;
using Microsoft.Win32;
using BCILibCS.Util;

namespace BCILib.Amp
{
    internal partial class ThinkGear:LiveAmplifier
    {
        public ThinkGear()
        {
            header.nchan = 4;
            header.nevt = 1;
            header.blk_samples = 1;

            // to be decided later
            header.resolution = 0;
            header.datasize = 4;
            header.samplingrate = 256;
        }

        public override string GetChannelNameString()
        {
            //return "CH1,ATT,N";
            StringBuilder sb = new StringBuilder();
            int nraw = header.nchan - 2;
            for (int ci = 0; ci < nraw; ci++) {
                sb.Append("CH");
                sb.Append((ci + 1).ToString());
                sb.Append(",");
            }
            sb.Append("ATT,N");
            return sb.ToString();
        }

        int connectionID = -1;
        string comPortName = "\\\\.\\COM2";
        int baudRate = BAUD_57600;
        bool _interpolate = false;
        bool _savelog = false;

        public override bool Initialize()
        {
            return InitTGCD();
            //return InitParser();
        }

        private bool Open(string portname)
        {
            Close(); // close if open

            connectionID = TG_GetNewConnectionId();
            if (connectionID < 0)
            {
                LogMessage("Get connection id returned error value: {0}", connectionID);
                return false;
            }

            /* Attempt to connect the connection ID handle to serial port "COM2" */
            int errCode = TG_Connect(connectionID,
                                  ThinkGearConfigForm.COM_PREFIX + portname,
                                  baudRate,
                                  STREAM_PACKETS);
            if (errCode < 0)
            {
                // LogMessage("ERROR: TG_Connect() returned {0}.", errCode);
                Close();
                return false;
            }

            if (_savelog) {
                /* Set stream (raw bytes) log file for connection */
                errCode = TG_SetStreamLog(connectionID, "streamLog.txt");
                if (errCode < 0) {
                    LogMessage("ERROR: TG_SetStreamLog() returned {0}.", errCode);
                    //return false;
                }

                /* Set/open data (ThinkGear values) log file for connection */
                errCode = TG_SetDataLog(connectionID, "dataLog.txt");
                if (errCode < 0) {
                    LogMessage("ERROR: TG_SetDataLog() returned {0}.", errCode);
                    //return false;
                }
            }

            // check for a valid data
            for (int i = 0; i < 20; i++)
            {
                errCode = TG_ReadPackets(connectionID, 1);
                if (errCode > 0) break;
                Thread.Sleep(50);
            }

            if (errCode < 0) {
                // cannot receive any data
                Close();
                return false;
            }

            return true;
        }

        private bool InitTGCD()
        {
            Close();

            LogMessage("ThinkGear Initialize: driver vertion = {0}", TG_GetDriverVersion());

            if (!string.IsNullOrEmpty(comPortName))
            {
                if (comPortName.StartsWith(ThinkGearConfigForm.COM_PREFIX))
                    comPortName = comPortName.Substring(ThinkGearConfigForm.COM_PREFIX.Length);
            }

            // find all ports in current computer
            ArrayList portlist = new ArrayList();

            // problem with this function. use registry key instead.
            string[] pnames = //System.IO.Ports.SerialPort.GetPortNames();
                SerialPortCtrl.GetPortNames();

            int scid = -1;
            for (int i = 0; i < pnames.Length; i++)
            {
                string pname = pnames[i];

                // system bug?
                int len = pname.Length;
                int p1 = len - 1;
                while (!Char.IsDigit(pname[p1])) p1--;
                if (p1 != len - 1) pname = pname.Substring(0, p1 + 1);

                portlist.Add(pname);

                if (pname == comPortName) scid = i;
            }

            // no ports available
            if (portlist.Count == 0)
            {
                LogMessage("ThinkGear Initialization: no aviable com port!");
                return false;
            }

            if (scid > 0)
            {
                // Exchange so that old port will be the first
                object o = portlist[scid];
                portlist[scid] = portlist[0];
                portlist[0] = o;
                scid = 0;
            }

            for (int i = 0; i < portlist.Count; i++) {
                string comport = (string)portlist[i];
                if (Open(comport))
                {
                    if (scid != i)
                    {
                        this.comPortName = comport;
                        SaveConfig(BCILib.App.BCIApplication.AppResource);
                    }
                    break;
                }
            }

            if (connectionID < 0)
            {
                return false;
            }

            TG_SendByte(connectionID, 0x13);

            return true;
        }

        private void Close()
        {
            if (connectionID >= 0) {
                //TG_Disconnect(connectionID);
                TG_FreeConnection(connectionID);
                connectionID = -1;
            }
        }

        float atv = 0;
        float noice = -1;

        protected override void ReceiveDataLoop()
        {
            //ReceiveParserLoop();
            ReceiveTGCDLoop();
        }

        private void ReceiveTGCDLoop()
        {
            int last_evt = 0;
            int nraw = header.nchan - 2; // 2 special channels
            if (nraw > 7) nraw = 7;

            int data_full = 0;
            for (int ri = 0; ri < nraw; ri++)
            {
                data_full |= (1 << ri);
            }

            int rdata_bits = 0;
            int[] rtypes = {DATA_RAW1, DATA_RAW2, DATA_RAW3, DATA_RAW4, DATA_RAW5, DATA_RAW6, DATA_RAW7};
            int[] ttypes = {TG_DATA_EEG_RAW_TIMESTAMP1, TG_DATA_EEG_RAW_TIMESTAMP2,
                           TG_DATA_EEG_RAW_TIMESTAMP3, TG_DATA_EEG_RAW_TIMESTAMP4,
                           TG_DATA_EEG_RAW_TIMESTAMP5, TG_DATA_EEG_RAW_TIMESTAMP6, TG_DATA_EEG_RAW_TIMESTAMP7};

            byte[] buf = new byte[header.blk_samples * 4 * (header.nchan + 1)];
            int npr = 0; // read samples

            float[] rsample0 = new float[nraw], rsample = new float[nraw];
            int ts0 = -1, ts = -1; // timestamp
            int nok = 0;

            noice = -1;

            while (bRunning) {
                /* Attempt to read a Packet of data from the connection */
                int errCode = TG_ReadPackets(connectionID, 1);

                /* If TG_ReadPackets() was able to read a complete Packet of data... */
                if (errCode != 1) {
                    if (errCode != -2 && errCode != 0) {
                        LogMessage("ThinkGear: TG_ReadPackets return error:{0}", errCode);
                        break;
                    }
                    System.Threading.Thread.Sleep(10);
                    continue;
                }
                int rv = 0;

                /* If attention value has been updated by TG_ReadPackets()... */
                if ((rv = TG_GetValueStatus(connectionID, DATA_ATTENTION)) != 0) {
                    //LogMessage("Attention:{0}", rv);
                    atv = TG_GetValue(connectionID, DATA_ATTENTION);
                }

                if ((rv = TG_GetValueStatus(connectionID, DATA_POOR_SIGNAL)) != 0) {
                    //LogMessage("BadSignal:{0}", rv);
                    float nv = TG_GetValue(connectionID, DATA_POOR_SIGNAL);
                    if (nv != noice)
                    {
                        noice = nv;
                        if (noice > 10 && Status != AmpStatus.Checking) {
                            Status = AmpStatus.Checking;
                        } else if (Status != AmpStatus.Connected) {
                            Status = AmpStatus.Connected;
                        }
                    }
                }

                // 20091203: receive raw data
                for (int ri = 0; ri < nraw; ri++)
                {
                    if ((rv = TG_GetValueStatus(connectionID, rtypes[ri])) != 0) {
                        // get one data
                        float fv = TG_GetValue(connectionID, rtypes[ri]);
                        int tstamp = -1;

                        rv = TG_GetValueStatus(connectionID, ttypes[ri]);
                        if (rv != 0)
                        {
                            tstamp = (int)TG_GetValue(connectionID, ttypes[ri]);
                        }

                        if ((rdata_bits & (1 << ri)) != 0) { // next data ready! finish this sample
                            if (ts0 != -1) {
                                // ts range: 1-256
                                int nlost = (ts + 256 - ts0 - 1) % 256;
                                if (nlost > 0)
                                {
                                    if (nok > 0) LogMessage("Received={0}", nok);
                                    nok = 0;
                                    LogMessage("{0}-{1}: Lost samples = {2}", ts0, ts, nlost);

                                    if (_interpolate)
                                    {
                                        int rbits = rdata_bits;
                                        for (int i = 0; i < nlost; i++)
                                        {
                                            rdata_bits = data_full;
                                            // fill with old sample
                                            FillSample(rsample0, ref rdata_bits, data_full, buf, ref npr, ref last_evt);
                                        }
                                        rdata_bits = rbits;
                                    }
                                }
                            }
                            // fill current sample (not received channels will use old value)
                            FillSample(rsample, ref rdata_bits, data_full, buf, ref npr, ref last_evt);
                            nok++;
                            ts0 = ts;
                            Array.Copy(rsample, rsample0, nraw);
                            ts = -1;
                        }

                        if (ts == -1) ts = tstamp;
                        else if (ts != tstamp)
                        {
                            if (nok > 0)
                            {
                                LogMessage("Received={0}", nok);
                                nok = 0;
                            }
                            LogMessage("Wrong: Timestamp Not Agree! {0}: {1} - {2}", ri, ts, tstamp);
                            int nlost = (tstamp - ts0 - 1 + 256) % 256;
                            LogMessage("{0}-{1}, Cal lost =  {2}", ts0, tstamp, nlost);

                            if (_interpolate)
                            {
                                int rbits = rdata_bits;
                                for (int i = 0; i < nlost; i++)
                                {
                                    rdata_bits = data_full;
                                    FillSample(rsample, ref rdata_bits, data_full, buf, ref npr, ref last_evt);
                                }
                                rdata_bits = rbits;
                            }

                            ts0 = (tstamp - 1 + 256) % 256;
                            ts = tstamp;

                        }
                        rsample[ri] = fv;
                        rdata_bits |= (1 << ri);

                        //if (rdata_bits == data_full) FillSample();
                    }
                }
            }
            Close();
        }

        private void FillSample(float[] rsample, ref int rdata_bits, int data_full, byte[] buf, ref int npr,
            ref int last_evt)
        {
            int nraw = rsample.Length;

            if (rdata_bits != data_full)
            {
                LogMessage("Warning some data absent!");
            }

            int evt = 0;
            if (last_evt == 0 && code_que.Count > 0)
            {
                var scode = code_que.Peek();
                if (scode.splno <= npr) {
                    evt = scode.code;
                    code_que.Dequeue();
                }
            }
            last_evt = evt;

            int p1 = npr * (header.nchan + 1) * 4; // buffer position for raw data
            for (int i = 0; i < nraw; i++)
            {
                BitConverter.GetBytes(rsample[i]).CopyTo(buf, p1);
                p1 += 4;
            }

            // attention
            BitConverter.GetBytes(atv).CopyTo(buf, p1);

            // noice
            p1 += 4;
            BitConverter.GetBytes(noice).CopyTo(buf, p1);

            // event
            p1 += 4;
            BitConverter.GetBytes(evt).CopyTo(buf, p1);

            npr++;

            if (npr == header.blk_samples)
            { // full, output
                PoolAddBuffer(buf);
                npr = 0;
            }

            rdata_bits = 0;
            //for (int bi = 0; bi < buf.Length; bi++) buf[bi] = 0;
        }

        public override bool Configure()
        {
            ThinkGearConfigForm dlg = new ThinkGearConfigForm();
            dlg.Port = comPortName;
            dlg.NumChannels = header.nchan - 2;
            dlg.BaudRate = baudRate;
            dlg.BlkSize = header.blk_samples;
            dlg.Interpolate = _interpolate;
            dlg.SaveLog = _savelog;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                comPortName = dlg.Port;
                baudRate = dlg.BaudRate;
                header.nchan = dlg.NumChannels + 2;
                header.blk_samples = dlg.BlkSize;
                _interpolate = dlg.Interpolate;
                _savelog = dlg.SaveLog;

                PoolInitialize();
                return true;
            }
            return false;
        }

        protected override void SaveConfigSpecial(BCILib.Util.ResManager rm)
        {
            rm.SetConfigValue(ID, "Port", comPortName);
            rm.SetConfigValue(ID, "NumRawData", (header.nchan - 2).ToString());
            rm.SetConfigValue(ID, "BaudRate", baudRate.ToString());
            rm.SetConfigValue(ID, "BlkSize", header.blk_samples.ToString());
            rm.SetConfigValue(ID, "Interpolate", _interpolate.ToString());
            rm.SetConfigValue(ID, "SaveLog", _savelog.ToString());
        }

        protected override void SetSpecialConfig(BCILib.Util.ResManager rm)
        {
            rm.GetConfigValue(ID, "Port", ref comPortName);
            int nraw = 1;
            rm.GetConfigValue(ID, "NumRawData", ref nraw);
            header.nchan = nraw + 2;
            rm.GetConfigValue(ID, "BaudRate", ref baudRate);
            rm.GetConfigValue(ID, "BlkSize", ref header.blk_samples);
            rm.GetConfigValue(ID, "Interpolate", ref _interpolate);
            rm.GetConfigValue(ID, "SaveLog", ref _savelog);
        }

        public override string DevName
        {
            get
            {
                return TypeName + ":" + comPortName;
            }
        }
    } // class
}
