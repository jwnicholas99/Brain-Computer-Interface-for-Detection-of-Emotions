using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;

namespace BCILib.Util
{
    public class SocketReadWrite
    {
        Socket _sock = null;
        StreamReader _sr = null;
        

        public SocketReadWrite(Socket s)
        {
            _sock = s;
        }

        public string ReadLine(int tout)
        {
            if (_sr == null || _sr.EndOfStream) {
                List<Socket> lst = new List<Socket>();
                lst.Add(_sock);
                Socket.Select(lst, null, null, tout);
                if (lst.Count <= 0) return null;

                int len = _sock.Available;
                if (len == 0) {
                    _sock.Close();
                    return null;
                }

                byte[] buf = new byte[len];
                int l = _sock.Receive(buf);
                if (len != l) {
                    Console.WriteLine("SocketReader: Receive length error!");
                }

                _sr = new StreamReader(new MemoryStream(buf));
            }

            return _sr.ReadLine();
        }

        public void WriteLine(string line)
        {
            try {
                NetworkStream ns = new NetworkStream(_sock, false);
                StreamWriter sw = new StreamWriter(ns);
                sw.WriteLine(line);
                sw.Flush();
            }
            catch (Exception e) {
                Console.WriteLine("SocketIO.Write: error = {0}", e.Message);
                _sock.Close();
                return;
            }
        }

        public void WriteLine(string fmt, params object[] args)
        {
            WriteLine(string.Format(fmt, args));
        }

        public static IPAddress GetBroadcastAddress()
        {
            IPAddress l_ip = null;
            foreach (IPAddress ip in Dns.GetHostAddresses(Dns.GetHostName())) {
                if (ip.AddressFamily == AddressFamily.InterNetwork) {
                    l_ip = ip;
                    break;
                }
            }
            if (l_ip == null) return null;

            IPAddress mask = null;
            foreach (NetworkInterface adaper in NetworkInterface.GetAllNetworkInterfaces()) {
                foreach (UnicastIPAddressInformation uip in adaper.GetIPProperties().UnicastAddresses) {
                    if (uip.Address.AddressFamily == AddressFamily.InterNetwork) {
                        if (uip.Address.Equals(l_ip)) {
                            mask = uip.IPv4Mask;
                        }
                    }
                }
            }

            if (mask == null) return null;

            byte[] lbytes = l_ip.GetAddressBytes();
            byte[] mbytes = mask.GetAddressBytes();
            int n = lbytes.Length;
            byte[] bbytes = new byte[n];
            for (int i = 0; i < n; i++) {
                bbytes[i] = (byte)(lbytes[i] | ~mbytes[i]);
            }

            return new IPAddress(bbytes);
        }
    }
}
