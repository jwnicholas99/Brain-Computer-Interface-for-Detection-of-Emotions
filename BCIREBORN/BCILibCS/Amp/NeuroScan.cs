using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Windows.Forms;
using System.Reflection;

using BCILib.Util;

namespace BCILib.Amp
{
    internal class NeuroScan : LiveAmplifier
    {
        public const string DllPath = "inpout32.dll";

        public const string NUAMPS_CHANNELS = "HEOL,HEOR,Fp1,Fp2,VEOU,VEOL,F7,F3,Fz,F4,F8,FT7,FC3,FCz,FC4,FT8,T7,C3,Cz,"
            + "C4,T8,TP7,CP3,CPz,CP4,TP8,A1,P7,P3,Pz,P4,P8,A2,O1,Oz,O2,FT9,FT10,PO1,PO2";
        public const string SYNAMPS2_CHANNELS = "Fp1,Fpz,Fp2,AF3,AF4,F7,F5,F3,F1,Fz,F2,F4,F6,F8,FT7,FC5,FC3,FC1,FCz,FC2,FC4,FC6,FT8,T7,C5,C3,"
            + "C1,Cz,C2,C4,C6,T8,M1,TP7,CP5,CP3,CP1,CPz,CP2,CP4,CP6,TP8,M2,P7,P5,P3,P1,Pz,P2,P4,P6,P8,PO7,PO5,PO3,POz,PO4,PO6,PO8,CB1,"
            + "O1,Oz,O2,CB2,VEO,HEO";

        public override string GetChannelNameString() {
            switch(device_type) {
                case Device.Nuamps:
                return NUAMPS_CHANNELS;
                case Device.SynAmp2:
                return SYNAMPS2_CHANNELS;
            }
            return null;
        }

        private int errCounts = 0;

        /// <summary>
        /// Reporting received stim code.
        /// </summary>
        /// <param name="evt">stim code</param>
        /// <param name="pos">position in blk</param>
        private void RecvEventCode(int evt, int pos)
        {
            if ((stim_method & StimMethod.ParallePort) != StimMethod.ParallePort) {
                // only for checking parallel port
                return;
            }

            if (evt_que.Count < 1) {
                Sound.BeepAsunc(800, 500);
                errCounts++;
                //something wrong here!
                // lost stim
                LogMessage("NEUROSCAN: received event but code queue empty! Lost?");
            } else {
                int code = evt_que.Peek();
                if (code != evt) {
                    if (!debug_mode) {
                        LogMessage("NeuroScan: received {0} differ from code {1}", evt, code);
                        error_type = ErrorType.EventDiff;
                        // consider correction?
                        if (errCounts == 0 || CheckEventTimeout) Sound.BeepAsunc(800, 500);
                        errCounts++;
                    }
                }
                evt_que.Dequeue();
            }

            if (!debug_mode && CheckEventTimeout && errCounts > 5) {
                Console.WriteLine("Too many errors! Stop?");
            }

            ResetEventTicks();

            return;
        }

        private Device device_type = Device.Nuamps;

        public Device DeviceType
        {
            get
            {
                return device_type;
            }
            set
            {
                device_type = value;
            }
        }

        public enum Device
        {
            Nuamps = 1,
            SynAmp2
        }

        public override string DevName
        {
            get {
                return TypeName + ":" + device_type.ToString();
            }
        }

		private Socket cli;

        public NeuroScan()
		{
            evt_stim_received += new Action<int, int>(RecvEventCode);

            stim_method = StimMethod.ParallePort;
            ms_software_stim_delay = 270;

            // initial header value
            header.blk_samples = 10;
            header.datasize = 4;
            header.nchan = 40; // nuamps
            header.nevt = 1;
            header.resolution = 0.06274173F;
            header.samplingrate = 250;
		}

        string host = "localhost";
        int port = 4000;

        public NeuroScan(string srv_host, int srv_port) : this() {
            host = srv_host;
            port = srv_port;
        }

		public override bool Initialize() {
			try {
                IPAddress[] sl = Dns.GetHostAddresses(host);
                if (sl == null || sl.Length == 0) return false;

                cli = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                cli.Connect(sl, port);
			} catch (Exception e) {
                if (cli != null) cli.Close();
				LogMessage("Exception in Start Service:{0}", e.Message);
				return false;
			}

            // Shake Hands
            // 1:- Get nothing from server!
            //cli.SendCommand(ScanRequest.CG_ReqForVersion);
            //cli.TryReceive(500);

            // 2: - Get no response
            //cli.SendCommand(ScanRequest.CC_ReqEDFHeader);
            //cli.TryReceive(500);

            // 3: - Get FILE
            //SendCommand(ClientControlCode, RequestAstFile);

            SendCommand(ScanRequest.CS_StartACQ);
            SendCommand(ScanRequest.CC_ReqBasicInf);
            SendCommand(ScanRequest.CC_ReqStartData);

            header.nchan = 0;

            while (TryReceive(500)) {
                if (header.nchan > 0) break;
            }

			return Connected;
		}

        private bool SendCommand(ScanRequest req) {
            ScanMessage msg = new ScanMessage(req);
            try {
                cli.Send(msg.GetTcpBuf());
                return true;
            } catch (Exception) {
                return false;
            }
        }

        private bool TryReceive(int wtime) {
            if (cli == null || !cli.Connected) {
                return false;
            }
            bool bEnd = false;

			ArrayList rl = new ArrayList(1);
			rl.Add(cli);
			try {
				Socket.Select(rl, null, null, wtime);
			} catch (Exception e1) {
				LogMessage("Select exception = {0}", e1);
                return false;
			}

            if (rl.Count == 0) return true;

            ScanMessage msg = new ScanMessage();
            byte[] bmsg = new byte[12];

		    try {
			    int lr = cli.Receive(bmsg);
                if (lr != 12) {
                    LogMessage("Receive msg error: length = {0}", lr);
                    bEnd = true;
                } else {
                    msg.ReadTcpBuf(bmsg);
                    //if (msg.ScanRequest != ScanRequest.DEEG_32Bit) {
                    //    LogMessage("Recv: {0}", msg);
                    //}
                    byte[] buf = null;

                    if (msg.size > 0) {
                        buf = new byte[msg.size];
                        int total = 0;
                        while (total < msg.size) {
                            lr = cli.Receive(buf, total, msg.size - total, SocketFlags.None);
                            if (lr == 0) {
                                LogMessage("Reaceive data returned 0!, socket closed?");
                                bEnd = true;
                                break;
                            }
                            total += lr;
                        }
					}
                    ProcessMessage(msg, buf);
				}
            } catch (Exception e3) {
                bEnd = true;
				LogMessage("Transportation Exception = {0}", e3);
			}

            if (bEnd) {
                try {
                    LogMessage("Connection closed.");
                    cli.Shutdown(SocketShutdown.Both);
                    cli.Close();
                    cli = null;
                }
                catch (Exception) {
			    }
            }
            return !bEnd;
        }

        protected override void ReceiveDataLoop()
        {
            bDiscFrmServer = false;
            last_code = 0;
            errCounts = 0;
            error_type = ErrorType.NoError;

            ResetEventTicks();

            while (TryReceive(500)) {
                if (RecvEventTimeout()) {
                    bRunning = false;
                    error_type = ErrorType.EventTimeout;
                }

                if (!bRunning) {
                    //quit gracefully
                    SendCommand(ScanRequest.CC_ReqStopData);
                    //SendCommand(ScanRequest.CS_StopACQ);
                    SendCommand(ScanRequest.CG_CloseupConnection);
                    break;
                }
            }

            while (TryReceive(500) && !bDiscFrmServer);

            if (error_type != ErrorType.NoError) {
                MessageBox.Show("Amplifier Error: " + error_type.ToString()
                    + "!\n\nCheck amplifier and connections!", "Amplifier Stopped");
            }
        }

        private bool bDiscFrmServer = false;
        private int last_code;
        private List<SentStim> dbg_list = new List<SentStim>();

        private void ProcessMessage(ScanMessage msg, byte[] body) {
            ScanRequest req = msg.ScanRequest;
            switch (req) {
                case ScanRequest.CG_CloseupConnection:
                    bDiscFrmServer = true;
                    break;

                case ScanRequest.DINF_BasicHeader:
                    uint size = BitConverter.ToUInt32(body, 0);
                    header.nchan = BitConverter.ToInt32(body, 4);
                    header.nevt = BitConverter.ToInt32(body, 8);
                    header.blk_samples = BitConverter.ToInt32(body, 12);
                    header.samplingrate = BitConverter.ToInt32(body, 16);
                    header.datasize = BitConverter.ToInt32(body, 20);
                    header.resolution = BitConverter.ToSingle(body, 24);
                    break;
                case ScanRequest.DEEG_32Bit:
                    if ((stim_method & StimMethod.Software) == StimMethod.Software) {
                        int off = header.nchan;
                        for (int si = 0; off * 4 < body.Length; si++ ) {
                            int evt = 0;
                            if (last_code == 0 && code_que.Count > 0) {
                                var scode = code_que.Peek();
                                int snow = Rd_GetPos() + si;
                                if (scode.splno <= snow) {
                                    if (scode.splno < snow) {
                                        Console.WriteLine(">>>>{0}", scode.splno - snow);
                                    }
                                    evt = scode.code;
                                    code_que.Dequeue();
                                    if (debug_mode) {
                                        evt += 128;
                                    }
                                    dbg_list.Add(scode);
                                }
                            }
                            last_code = evt;

                            if (debug_mode && evt != 0) {
                                // extra event to check
                                evt_que.Enqueue((byte)evt);
                            }

                            if (evt != 0 || !debug_mode) {
                                // over-write hardware event!!!!!!!!!
                                BitConverter.GetBytes(evt).CopyTo(body, off * 4);
                            }

                            off += header.nchan + 1;
                        }
                    }
                    PoolAddBuffer(body);
                    UpdateActSplRate();
                    break;
            }
        }

        bool Connected
        {
            get
            {
                return (cli != null && cli.Connected);
            }
        }

        private void Close()
        {
            if (cli != null) {
                try {
                    cli.Shutdown(SocketShutdown.Both);
                    cli.Close();
                    cli = null;
                }
                catch (Exception) { };
            }
        }

        public override string[] ChannelNames
        {
            get
            {
                string cnames;
                if (device_type == Device.Nuamps) cnames = NUAMPS_CHANNELS;
                else if (device_type == Device.SynAmp2) cnames = SYNAMPS2_CHANNELS;
                else cnames = "";
                return cnames.Split(new char[] {',', ' '}, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        bool debug_mode = false;

        public override bool Configure()
        {
            NeuroScanCfg dlg = new NeuroScanCfg();
            dlg.ID = this.ID;
            dlg.Host = host;
            dlg.Port = port;
            dlg.StimMethod = stim_method;
            dlg.DebugMode = debug_mode;
            dlg.CheckTimeout = CheckEventTimeout;
            dlg.PortAddr = ParallelPort.PortAddr;

            if (dlg.ShowDialog() == DialogResult.OK) {
                device_type = dlg.Device;
                host = dlg.Host;
                port = dlg.Port;
                stim_method = dlg.StimMethod;
                debug_mode = dlg.DebugMode;
                CheckEventTimeout = dlg.CheckTimeout;
                ParallelPort.PortAddr = dlg.PortAddr;
                return true;
            }
            return false;
        }

        enum ConfigProp
        {
            Device,
            Host,
            Port,
            StimMethod,
            CheckTout,
            PortAddr
        }

        protected override void SaveConfigSpecial(BCILib.Util.ResManager rm)
        {
            rm.SetConfigValue(ID, ConfigProp.Device, device_type.ToString());
            rm.SetConfigValue(ID, ConfigProp.Host, host);
            rm.SetConfigValue(ID, ConfigProp.Port, port);

            int pad = ParallelPort.DEFAULT_PORTADDR;
            var line = rm.GetConfigValue(ID, ConfigProp.PortAddr.ToString());
            if (!string.IsNullOrEmpty(line)) {
                int.TryParse(line, out pad);
            }

            if (pad != ParallelPort.PortAddr) {
                rm.SetConfigValue(ID, ConfigProp.PortAddr, ParallelPort.PortAddr);
            }

            if (!debug_mode) {
                rm.SetConfigValue(ID, ConfigProp.StimMethod, stim_method.ToString());
            }

            rm.SetConfigValue(ID, ConfigProp.CheckTout, CheckEventTimeout);
        }

        protected override void SetSpecialConfig(BCILib.Util.ResManager rm)
        {
            rm.GetConfigValue(ID, ConfigProp.Host.ToString(), ref host);
            rm.GetConfigValue(ID, ConfigProp.Port.ToString(), ref port);
            string dstr = rm.GetConfigValue(ID, ConfigProp.Device.ToString());
            device_type = (Device) Enum.Parse(typeof(Device), dstr);

            string stim_str = rm.GetConfigValue(ID, ConfigProp.StimMethod.ToString());
            if (!string.IsNullOrEmpty(stim_str)) {
                stim_method = (StimMethod) Enum.Parse(typeof(StimMethod), stim_str);
            }

            bool checktout = true;
            rm.GetConfigValue(ID, ConfigProp.CheckTout.ToString(), ref checktout);
            CheckEventTimeout = checktout;

            rm.GetConfigValue(ID, ConfigProp.PortAddr, ref ParallelPort.PortAddr);
        }
    }

    enum ScanRequest
    {
        CG_ReqForVersion = 111,
        CG_CloseupConnection = 112,
        CG_ServeDisconnected = 113, // from c

        CS_StartACQ = 121,
        CS_StopACQ = 122,
        CS_StartImp = 123,
        CS_ChangeSetup = 124,
        CS_DCCorection = 125,

        CC_ReqEDFHeader = 131,
        CC_ReqASTFile = 132,
        CC_ReqStartData = 133,
        CC_ReqStopData = 134,
        CC_ReqBasicInf = 135,

        FSETUP_ASTFile = 211,

        DINF_VersionInf = 311,
        DINF_EDFHeader = 312,
        DINF_BasicHeader = 313,

        DEEG_16Bit = 321,
        DEEG_32Bit = 322
    }

    class ScanMessage
    {
        string ID;
        short id;
        short code;
        short req;
        public int size;

        public ScanMessage()
        {
        }

        static string[] ID_CODES = { "CTRL", "FILE", "DATA" };

        public ScanMessage(ScanRequest reqest)
        {
            ScanRequest = reqest;
        }
        public ScanRequest ScanRequest
        {
            get
            {
                return (ScanRequest)(100 * id + 10 * code + req);
            }
            set
            {
                int rv = (int)value;
                id = (short)(rv / 100);
                if (id > 0 && id <= ID_CODES.Length) ID = ID_CODES[id - 1];
                rv %= 100;
                code = (short)(rv / 10);
                req = (short)(rv % 10);
            }
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}", ScanRequest, size);
        }

        public void ReadTcpBuf(byte[] buf)
        {
            ID = ASCIIEncoding.ASCII.GetString(buf, 0, 4);
            try {
                id = (short)(Array.IndexOf(ID_CODES, ID) + 1);
            }
            catch (Exception) {
                id = 0;
            }

            code = BitConverter.ToInt16(buf, 4);
            req = BitConverter.ToInt16(buf, 6);
            size = BitConverter.ToInt32(buf, 8);

            code = IPAddress.NetworkToHostOrder(code);
            req = IPAddress.NetworkToHostOrder(req);
            size = IPAddress.NetworkToHostOrder(size);
        }

        public byte[] GetTcpBuf()
        {
            byte[] buf = new byte[12];

            ASCIIEncoding.ASCII.GetBytes(ID, 0, 4, buf, 0);

            BitConverter.GetBytes(IPAddress.HostToNetworkOrder(code)).CopyTo(buf, 4);
            BitConverter.GetBytes(IPAddress.HostToNetworkOrder(req)).CopyTo(buf, 6);
            BitConverter.GetBytes(IPAddress.HostToNetworkOrder(size)).CopyTo(buf, 8);

            return buf;
        }
    }
}
