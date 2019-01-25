using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using System.Threading;
using Mindo;

namespace BCILib.Amp
{
    public class MindoAmp : LiveAmplifier
    {
        static public string DllPath = "MindoAmp.dll;InTheHand.Net.Personal.dll";
        public MindoAmp()
        {
            header.nchan = 4;
            header.nevt = 1;
            header.samplingrate = 256;
            header.blk_samples = 8;
            header.datasize = 4;
            header.resolution = 0;
        }

        protected override void ReceiveDataLoop()
        {
            byte[] buf = new byte[header.BlkSize];
            int off = 0;
            int last_evt = 0;

            while (bRunning) {
                var bc = new BluetoothClient();
                BluetoothRadio radio = BluetoothRadio.PrimaryRadio;
                if (radio != null) {
                    if (radio.Mode == RadioMode.PowerOff) radio.Mode = RadioMode.Connectable;
                    var dlist = bc.DiscoverDevices(999, true, false, false);
                    foreach (var di in dlist) {
                        if (!di.DeviceName.StartsWith("Mindo", StringComparison.InvariantCultureIgnoreCase)) continue;
                        try {
                            bc.Connect(di.DeviceAddress, BluetoothService.SerialPort);
                            break;
                        } catch {
                        }
                    }
                }

                if (!bc.Connected) {
                    if (Status != AmpStatus.Checking) {
                        Console.WriteLine("MindoAmp: cannot find device!");
                        Status = AmpStatus.Checking;
                    }
                    Thread.Sleep(500);
                    continue;
                }

                var ms = new MindoStream();
                try {
                    ms.Start(bc.GetStream(), 16, header.samplingrate, header.nchan, 1);
                } catch (Exception ex) {
                    if (Status != AmpStatus.Checking) {
                        Console.WriteLine("MindoAmp: error = {0}", ex.Message);
                        Status = AmpStatus.Checking;
                        bc.Close();
                        continue;
                    }
                }

                if (Status != AmpStatus.Connected) {
                    Console.WriteLine("Amplifer: reading data...");
                    Status = AmpStatus.Connected;
                }

                while (bRunning) {
                    int data_lost = 0;
                    float[] data = null;
                    try {
                        data = ms.readData(1361, out data_lost);
                    } catch (Exception ex) {
                        if (Status != AmpStatus.Checking) {
                            Console.WriteLine("MindoAmp.readData: error = {0}", ex.Message);
                            Status = AmpStatus.Checking;
                            bc.Close();
                            break;
                        }
                    }

                    if (data_lost > 0) {
                        Console.WriteLine("DataLost = {0}", data_lost);
                    }

                    for (int ci = 0; ci < header.nchan; ci++) {
                        float fv = data[ci] * 1000000; // v => uv
                        BitConverter.GetBytes(fv).CopyTo(buf, off);
                        off += 4;
                    }
                    int evt = 0;
                    if (last_evt == 0 && code_que.Count > 0) {
                        evt = code_que.Dequeue();
                    }
                    BitConverter.GetBytes(evt).CopyTo(buf, off);
                    off += 4;
                    last_evt = evt;

                    if (off >= buf.Length) {
                        PoolAddBuffer(buf);
                        off = 0;
                    }
                }

                bc.Close();
            }
            Status = AmpStatus.Off;
        }

        public override string GetChannelNameString()
        {
            return string.Join(",", Enumerable.Range(1, header.nchan)
                .Select(x => "Ch" + x.ToString()).ToArray());
        }

        public override bool Initialize()
        {
            return true;
        }

        public override bool Configure()
        {
            return false;
        }
    }
}
