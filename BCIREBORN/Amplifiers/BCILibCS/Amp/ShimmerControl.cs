/*
 * Copyright (c) 2010, Shimmer Research, Ltd.
 * All rights reserved
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are
 * met:

 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above
 *       copyright notice, this list of conditions and the following
 *       disclaimer in the documentation and/or other materials provided
 *       with the distribution.
 *     * Neither the name of Shimmer Research, Ltd. nor the names of its
 *       contributors may be used to endorse or promote products derived
 *       from this software without specific prior written permission.

 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
 * A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
 * OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
 * LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 * DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
 * THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *
 * @author Mike Healy
 * @date   January, 2011
 */


using System;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Collections.Generic;

namespace BCILib.Amp
{
    public partial class ShimmerControl : Form
    {
        private volatile bool stopReading = false;
        private volatile bool sensorsChangePending = false;
        private Thread readThread;
        private List<TextBox> channelTextBox = new List<TextBox>();
        private List<Label> channelLabel = new List<Label>();
        private TextBox extraInfoTextBox = new TextBox();
        private Label extraInfoLabel = new Label();

        private delegate void SetTextChannelsCallback(ShimmerDataPacket packet);
        private delegate void ShowChannelTextBoxesCallback();

        private ShimmerProfile pProfile;
        //private List<GraphForm> pChannelPlot;
        private StreamWriter pCsvFile;
        private bool pUpdateGraphs;
        public bool pSaveToFile;
        public delegate void ShowGraphsDelegate();
        ShowGraphsDelegate PShowGraphs;
        public delegate void ChangeStatusLabelDelegate(string text);
        public bool isStreaming;

        public event EventHandler<ShimmerDAQEventArgs> NewShimmerDataRecieved;

        private System.IO.FileStream m_Stream_DataFile = null;
        private System.IO.FileStream m_Stream_LogFile = null;
        private System.IO.BinaryWriter m_Writer_LogFile = null;
        private int m_iSampleCount_DataFile = 0;

        private int m_FileStartTimeCode = 0;


        private Point[] textBoxLocation = new Point[] {
#if _PLATFORM_LINUX
            new Point(10, 106),
            new Point(125, 106),
            new Point(240, 106),
            new Point(10, 147),
            new Point(125, 147),
            new Point(240, 147),
            new Point(10, 188),
            new Point(125, 188),
            new Point(240, 188),
            new Point(10, 229),
            new Point(125, 229),
            new Point(240, 229),
            new Point(10, 270)
#else
            // WINDOWS
            new Point(10, 97),
            new Point(107, 97),
            new Point(204, 97),
            new Point(10, 137),
            new Point(107, 137),
            new Point(204, 137),
            new Point(10, 177),
            new Point(107, 177),
            new Point(204, 177),
            new Point(10, 217),
            new Point(107, 217),
            new Point(204, 217),
            new Point(10, 257)
#endif
        };
        private Point[] labelLocation = new Point[] {
#if _PLATFORM_LINUX
            new Point(12, 88),
            new Point(129, 88),
            new Point(242, 88),
            new Point(12, 129),
            new Point(129, 129),
            new Point(242, 129),
            new Point(12, 170),
            new Point(129, 170),
            new Point(242, 170),
            new Point(12, 211),
            new Point(129, 211),
            new Point(242, 211), 
            new Point(12, 252)
#else
            // WINDOWS
            new Point(12, 81),
            new Point(109, 81),
            new Point(206, 81),
            new Point(12, 121),
            new Point(109, 121),
            new Point(206, 121),
            new Point(12, 161),
            new Point(109, 161),
            new Point(206, 161),
            new Point(12, 201),
            new Point(109, 201),
            new Point(206, 201), 
            new Point(12, 241)
#endif
        };
        public string ComPort {
            set
            {
                cmbComPortSelect.Text = value;
            }

            get
            {
                return cmbComPortSelect.Text;
            }
        }

        private void FillSerialCmbBox()
        {
            cmbComPortSelect.Items.Clear();
            
#if _PLATFORM_LINUX
            cmbComPortSelect.Items.Add("/dev/rfcomm0");
#endif
            string[] serialPorts = SerialPort.GetPortNames();
            foreach (string port in serialPorts)
            {
                cmbComPortSelect.Items.Add(port);
            }
        }

        public ShimmerControl()
        {
            InitializeComponent();
            isStreaming = false;
            btnDisconnect.Enabled = false;
            btnStart.Enabled = false;
            btnStop.Enabled = false;
            FillSerialCmbBox();
        }

        private void DispMessage(string msg)
        {
            if (InvokeRequired) {
                Invoke(new Action(() => textBoxMessage.AppendText(msg + "\r\n")));
            } else {
                textBoxMessage.AppendText(msg + "\r\n");
            }
        }

        public ShimmerControl(ShimmerProfile profile, bool saveToFile, StreamWriter csvFile, 
            bool updateGraphs, ShowGraphsDelegate ShowGraphs, ChangeStatusLabelDelegate ChangeStatusLabel)
            : this()
        { 
            pProfile = profile;
            pSaveToFile = saveToFile;
            pCsvFile = csvFile;
            pUpdateGraphs = updateGraphs;
            if (ShowGraphs != null) {
                PShowGraphs += ShowGraphs;
            }
        }

        private void cmbComPortSelect_DropDown(object sender, EventArgs e)
        {
            FillSerialCmbBox();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (cmbComPortSelect.Text != "") {
                if (serialPort1.IsOpen) {
                    serialPort1.Close();
                }
                serialPort1.PortName = cmbComPortSelect.Text;
            }

            try
            {
                serialPort1.Open();
                serialPort1.DiscardInBuffer();
                serialPort1.DiscardOutBuffer();
                DispMessage("Connected to " + serialPort1.PortName);
                stopReading = false;
                readThread = new Thread(new ThreadStart(ReadData));
                readThread.Start();
                // change configuration if required
                ChangeConfiguration();
                // give the shimmer time to make the changes before continuing (required?)
                System.Threading.Thread.Sleep(5000);
                // Read Shimmer Profile
                if (serialPort1.IsOpen)
                {
                    serialPort1.Write(new byte[1] { (byte)Shimmer.PacketType.InquiryCommand }, 0, 1);
                    // give the shimmer a chance to process the previous command (required?)
                    System.Threading.Thread.Sleep(500);
                    // Not strictly necessary here unless the GSR sensor is selected, but easier to get this value set correctly to begin with
                    serialPort1.Write(new byte[1] { (byte)Shimmer.PacketType.GetGsrRangeCommand}, 0, 1);
                }
                if (pSaveToFile)
                {
                    System.Threading.Thread.Sleep(100);
                    WriteHeaderToFile();
                }
                btnConnect.Enabled = false;
                btnDisconnect.Enabled = true;
                btnStart.Enabled = true;
                btnStop.Enabled = true;
                cmbComPortSelect.Enabled = false;
            }
            catch
            {
                cmbComPortSelect.SelectedText = "";
                DispMessage("Cannot connect to specified serial port!");
                MessageBox.Show("Cannot open " + serialPort1.PortName, Shimmer.ApplicationName,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }            
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            Disconnect();
        }

        private void Disconnect()
        {
            if (serialPort1.IsOpen)
            {
                stopReading = true;
                DispMessage("Disconnected");
            }
            isStreaming = false;
            btnDisconnect.Enabled = false;
            btnStart.Enabled = false;
            btnStop.Enabled = false;
            btnConnect.Enabled = true;
            cmbComPortSelect.Enabled = true;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                if (pProfile.GetIsFilled())
                {
                    ShowChannelTextBoxes();
                    if (pUpdateGraphs && PShowGraphs != null)
                    {
                        PShowGraphs();
                    }
                    serialPort1.Write(new byte[1] { (byte)Shimmer.PacketType.StartStreamingCommand }, 0, 1);
                    isStreaming = true;
                    if ((pProfile.GetSensors()[1] & (int)Shimmer.Sensor1Bitmap.SensorStrain) != 0)
                        pProfile.SetVReg(true);
                }
                else
                {
                    MessageBox.Show("Failed to read configuration information from shimmer. Please ensure correct shimmer is connected", Shimmer.ApplicationName,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    serialPort1.Write(new byte[1] { (byte)Shimmer.PacketType.InquiryCommand }, 0, 1);
                }
            }
            else
                MessageBox.Show("No serial port is open", Shimmer.ApplicationName,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Write(new byte[1] { (byte)Shimmer.PacketType.StopStreamingCommand }, 0, 1);
                isStreaming = false;
                if ((pProfile.GetSensors()[1] & (int)Shimmer.Sensor1Bitmap.SensorStrain) != 0)
                    pProfile.SetVReg(false);
            }
            else
                MessageBox.Show("No serial port is open", Shimmer.ApplicationName,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ReadData()
        {
            List<byte> buffer = new List<byte>();
            int i;
            serialPort1.DiscardInBuffer();

            Console.WriteLine("ShimmerControl: ReadData starts");
            while (!stopReading)    //continues to read for approx 1s (until serial read timeout)
            {
                try
                {
                    Shimmer.PacketType pkt_type = (Shimmer.PacketType)serialPort1.ReadByte();
                    DispMessage("Data: type=" + pkt_type);

                    switch (pkt_type)
                    {
                        case Shimmer.PacketType.DataPacket:
                            if (pProfile.GetIsFilled())
                            {
                                for (i = 0; i < (2 + ((pProfile.GetNumAdcChannels() + pProfile.GetNum2ByteDigiChannels()) * 2) + pProfile.GetNum1ByteDigiChannels()); i++)
                                {
                                    buffer.Add((byte)serialPort1.ReadByte());
                                }
                                ShimmerDataPacket packet = new ShimmerDataPacket(buffer, pProfile.GetNumAdcChannels(), pProfile.GetNum1ByteDigiChannels(), pProfile.GetNum2ByteDigiChannels());
                                if (packet.GetIsFilled())
                                {
                                    SetTextChannels(packet);
                                    UpdateDataSample(packet);
                                }
                                buffer.Clear();
                            }
                            break;

                        case Shimmer.PacketType.InquiryResponse:
                            for (i = 0; i < 5; i++)
                            {
                                // get Sampling rate, accel range, config setup byte0, num chans and buffer size
                                buffer.Add((byte)serialPort1.ReadByte());
                            }
                            for (i = 0; i < (int)buffer[3]; i++)
                            {
                                // read each channel type for the num channels
                                buffer.Add((byte)serialPort1.ReadByte());
                            }
                            pProfile.fillProfile(buffer);
                            buffer.Clear();
                            break;

                        case Shimmer.PacketType.SamplingRateResponse:
                            pProfile.SetAdcSamplingRate(serialPort1.ReadByte());
                            break;

                        case Shimmer.PacketType.AccelRangeResponse:
                            pProfile.SetAccelRange(serialPort1.ReadByte());
                            break;
                        case Shimmer.PacketType.ConfigSetupByte0Response:
                            pProfile.SetConfigSetupByte0(serialPort1.ReadByte());
                            break;
                        case Shimmer.PacketType.gsrRangeResponse:
                            pProfile.SetGsrRange(serialPort1.ReadByte());
                            break;
                        case Shimmer.PacketType.AckCommandProcessed:
                            if (sensorsChangePending)
                            {
                                pProfile.UpdateChannelsFromSensors();
                                ShowChannelTextBoxes();
                                if (pUpdateGraphs && PShowGraphs != null)
                                {
                                    PShowGraphs();
                                }
                                if (pSaveToFile)
                                    WriteHeaderToFile();
                                sensorsChangePending = false;
                            }
                            break;

                        default:
                            break;
                    }
                }

                catch (System.TimeoutException)
                {
                    // do nothing
                }
                catch (System.InvalidOperationException)
                {
                    // do nothing
                    // gets here if serial port is forcibly closed
                }
                catch (System.IO.IOException)
                {
                    // do nothing
                }
                /*
                catch
                {
                    // should really try to kill the thread more gracefully
                }
                */
            }
            // only stop reading when disconnecting, so disconnect serial port here too
            serialPort1.Close();
            
        }

        private void UpdateDataSample(ShimmerDataPacket packet)
        {
            int [] Data=new int[packet.GetNumChannels()];

            int iTimeStamp = packet.GetTimeStamp();
            string strLineToWrite = "";
            bool bpSaveToFile = pSaveToFile;
            if (bpSaveToFile)
            {
                int iTickCountOffset = Environment.TickCount - m_FileStartTimeCode;
                strLineToWrite=iTickCountOffset.ToString()+","+packet.GetTimeStamp().ToString();
            }

            for (int i = 0; i < packet.GetNumChannels(); i++)
            {
                if (pProfile.GetChannel(i) == (int)Shimmer.ChannelContents.XMag ||
                    pProfile.GetChannel(i) == (int)Shimmer.ChannelContents.YMag ||
                    pProfile.GetChannel(i) == (int)Shimmer.ChannelContents.ZMag)
                {
                    // The magnetometer gives a signed 16 bit integer per channel
                    packet.SetChannel(i, ((Int16)packet.GetChannel(i)));
                }
                if(pProfile.GetChannel(i) == (int)Shimmer.ChannelContents.GsrRaw)
                {
                    int adcVal = packet.GetChannel(i);

                    if(pProfile.GetGsrRange()==(int)Shimmer.GsrRange.AUTORANGE)
                    {
                        int gain = (adcVal & 0xC000) >> 14;
                        Data[i] = GsrResistance((adcVal & 0x3FFF), gain);
                    }
                    else
                        Data[i] = GsrResistance(adcVal, pProfile.GetGsrRange());
                }

                if (bpSaveToFile)
                {
                    if (pProfile.GetChannel(i) == (int)Shimmer.ChannelContents.GsrRaw)
                    {
                        if (pProfile.GetGsrRange() == (int)Shimmer.GsrRange.AUTORANGE)
                        {
                            int adcVal = packet.GetChannel(i);
                            strLineToWrite=strLineToWrite+"," + ((adcVal & 0xC000)>>14).ToString() + "," + (adcVal & 0x3FFF).ToString();
                        }
                        else
                        {
                            strLineToWrite=strLineToWrite+"," + pProfile.GetGsrRange().ToString() + "," + packet.GetChannel(i).ToString();
                        }
                    }
                    else
                        strLineToWrite=strLineToWrite+"," + packet.GetChannel(i).ToString();
                    strLineToWrite += "," + Data[i].ToString();
                }
            }
            if (bpSaveToFile)
            {
                pCsvFile.Write(strLineToWrite+"\n");
            }
            m_iSampleCount_DataFile++;
            if (NewShimmerDataRecieved != null) NewShimmerDataRecieved(this, new ShimmerDAQEventArgs(Data));
        }

        public void InsertEvent(int iEvent)
        {
            int iTickCountOffset = Environment.TickCount - m_FileStartTimeCode;

            if (m_Stream_DataFile != null && m_Stream_LogFile != null)
            {
                m_Writer_LogFile.Write(iTickCountOffset);
                m_Writer_LogFile.Write((int)1);
                m_Writer_LogFile.Write(iEvent);
            }
        }

        

        // This method demonstrates a pattern for making thread-safe
        // calls on a Windows Forms ShimmerControl. 
        //
        // If the calling thread is different from the thread that
        // created the TextBox ShimmerControl, this method creates a
        // SetTextChannelsCallback and calls itself asynchronously using the
        // Invoke method.
        //
        // If the calling thread is the same as the thread that created
        // the TextBox control, the Text property is set directly. 
        // For full details see: http://msdn.microsoft.com/en-us/library/ms171728%28VS.80%29.aspx
        private void SetTextChannels(ShimmerDataPacket packet)
        {
            try
            {
                if (this.channelTextBox[0].InvokeRequired) // all will be in the same thread
                {
                    SetTextChannelsCallback d = new SetTextChannelsCallback(SetTextChannels);
                    this.Invoke(d, new object[] { packet });
                }
                else
                {
                    if (pSaveToFile && false)
                    {
                        pCsvFile.Write(packet.GetTimeStamp().ToString());
                    }
                    for (int i = 0; i < packet.GetNumChannels(); i++)
                    {
                        if (pProfile.GetChannel(i) == (int)Shimmer.ChannelContents.XMag ||
                            pProfile.GetChannel(i) == (int)Shimmer.ChannelContents.YMag ||
                            pProfile.GetChannel(i) == (int)Shimmer.ChannelContents.ZMag)
                        {
                            // The magnetometer gives a signed 16 bit integer per channel
                            packet.SetChannel(i, ((Int16)packet.GetChannel(i)));
                        }
                        this.channelTextBox[i].Text = packet.GetChannel(i).ToString();
                        if (pProfile.showMagHeading && pProfile.GetChannel(i) == (int)Shimmer.ChannelContents.XMag)
                        {
                            // Current channel + next 2 are mag channels
                            this.extraInfoTextBox.Text = MagHeading((Int16)packet.GetChannel(i),
                                                                     (Int16)packet.GetChannel(i + 1),
                                                                     (Int16)packet.GetChannel(i + 2)).ToString();
                        }
                        else if(pProfile.showGsrResistance && pProfile.GetChannel(i) == (int)Shimmer.ChannelContents.GsrRaw)
                        {
                            int adcVal = packet.GetChannel(i);
                            switch (pProfile.GetGsrRange())
                            {
                                case (int)Shimmer.GsrRange.HW_RES_40K:
                                    if(adcVal < 1140)
                                        this.extraInfoTextBox.Text = "> max";
                                    else if(adcVal > 3400)
                                        this.extraInfoTextBox.Text = "< min";
                                    else
                                        this.extraInfoTextBox.Text = GsrResistance(adcVal, pProfile.GetGsrRange()).ToString();
                                    break;
                                case (int)Shimmer.GsrRange.HW_RES_287K:
                                    if (adcVal < 1490)
                                        this.extraInfoTextBox.Text = "> max";
                                    else if (adcVal > 3800)
                                        this.extraInfoTextBox.Text = "< min";
                                    else
                                        this.extraInfoTextBox.Text = GsrResistance(adcVal, pProfile.GetGsrRange()).ToString();
                                    break;
                                case (int)Shimmer.GsrRange.HW_RES_1M:
                                    if (adcVal < 1630)
                                        this.extraInfoTextBox.Text = "> max";
                                    else if (adcVal > 3700)
                                        this.extraInfoTextBox.Text = "< min";
                                    else
                                        this.extraInfoTextBox.Text = GsrResistance(adcVal, pProfile.GetGsrRange()).ToString();
                                    break;
                                case (int)Shimmer.GsrRange.HW_RES_3M3:
                                    if (adcVal < 1125)
                                        this.extraInfoTextBox.Text = "> max";
                                    else if (adcVal > 3930)
                                        this.extraInfoTextBox.Text = "< min";
                                    else
                                        this.extraInfoTextBox.Text = GsrResistance(adcVal, pProfile.GetGsrRange()).ToString();
                                    break;
                                case (int)Shimmer.GsrRange.AUTORANGE:
                                    int gain = (adcVal & 0xC000) >> 14;
                                    this.extraInfoTextBox.Text = GsrResistance((adcVal & 0x3FFF), gain).ToString();
                                    break;
                            }
                        }
                        if (pUpdateGraphs)
                        {
                            if (pProfile.GetChannel(i) == (int)Shimmer.ChannelContents.XMag ||
                                pProfile.GetChannel(i) == (int)Shimmer.ChannelContents.YMag ||
                                pProfile.GetChannel(i) == (int)Shimmer.ChannelContents.ZMag)
                            {
                                // scale the mag data to be positive
                                //pChannelPlot[i].psQueue.Enqueue(new Point(0, (packet.GetChannel(i) + 2048)));  
                            }
                            else if((pProfile.GetChannel(i) == (int)Shimmer.ChannelContents.GsrRaw) &&
                                    (pProfile.GetGsrRange() == (int)Shimmer.GsrRange.AUTORANGE))
                            {
                                // remove the two bits indicating range
                                //pChannelPlot[i].psQueue.Enqueue(new Point(0, (packet.GetChannel(i) & 0x3FFF)));  
                            }
                            //else
                                //pChannelPlot[i].psQueue.Enqueue(new Point(0, packet.GetChannel(i)));
                            //pChannelPlot[i].Invalidate();
                        }
                        if (pSaveToFile && false)
                        {
                            if (pProfile.GetChannel(i) == (int)Shimmer.ChannelContents.GsrRaw)
                            {
                                if (pProfile.GetGsrRange() == (int)Shimmer.GsrRange.AUTORANGE)
                                {
                                    int adcVal = packet.GetChannel(i);
                                    pCsvFile.Write("," + ((adcVal & 0xC000)>>14).ToString() + "," + (adcVal & 0x3FFF).ToString());
                                }
                                else
                                {
                                    pCsvFile.Write("," + pProfile.GetGsrRange().ToString() + "," + packet.GetChannel(i).ToString());
                                }
                            }
                            else
                                pCsvFile.Write("," + packet.GetChannel(i).ToString());
                        }
                    }
                    if (pSaveToFile && false)
                    {
                        pCsvFile.Write("\n");
                    }
                }
            }
            catch (System.ArgumentOutOfRangeException)
            {
                Disconnect();
                serialPort1.Close();
                MessageBox.Show("Receiving incorrect or corrupt packets. Closing the connection...", Shimmer.ApplicationName,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult == System.Windows.Forms.DialogResult.OK && string.IsNullOrEmpty(cmbComPortSelect.Text)) {
                e.Cancel = true;
                return;
            }

            if (pSaveToFile) StopRecording();
            if(serialPort1.IsOpen)
                serialPort1.Close();
            if (readThread != null)
                readThread.Abort();
            
            //if (pCsvFile != null)
            //    pCsvFile.Close();
        }

        public void ChangeConfiguration()
        {
            bool wait = false;          // give shimmer chance to process command before sending another

            if (serialPort1.IsOpen)
            {
                if (pProfile.changeSamplingRate)
                {
                    Console.WriteLine("ShimmerControl: changeSamplingRate");
                    serialPort1.Write(new byte[2] { (byte)Shimmer.PacketType.SetSamplingRateCommand, 
                        (byte)pProfile.GetAdcSamplingRate() }, 0, 2);
                    pProfile.changeSamplingRate = false;
                    wait = true;
                }
                if (pProfile.changeSensors)
                {
                    Console.WriteLine("ShimmerControl: changeSensors");
                    if (wait)
                    {
                        System.Threading.Thread.Sleep(500);
                    }
                    serialPort1.Write(new byte[3] { (byte)Shimmer.PacketType.SetSensorsCommand,
                        (byte)pProfile.GetSensors()[0], (byte)pProfile.GetSensors()[1]}, 0, 3);
                    pProfile.changeSensors = false;
                    wait = true;
                    sensorsChangePending = true;
                }
                if (pProfile.change5Vreg)
                {
                    Console.WriteLine("ShimmerControl: change5Vreg");
                    if (wait)
                        System.Threading.Thread.Sleep(500);
                    serialPort1.Write(new byte[2] { (byte)Shimmer.PacketType.Set5VRegulatorCommand,
                        (pProfile.GetVReg()?(byte)1:(byte)0)}, 0, 2);
                    pProfile.change5Vreg = false;
                    wait = true;
                }
                if (pProfile.changePwrMux)
                {
                    Console.WriteLine("ShimmerControl: changePwrMux");
                    if (wait)
                        System.Threading.Thread.Sleep(500);
                    serialPort1.Write(new byte[2] { (byte)Shimmer.PacketType.SetPowerMuxCommand,
                        (pProfile.GetPMux()?(byte)1:(byte)0)}, 0, 2);
                    pProfile.changePwrMux = false;
                    wait = true;
                    sensorsChangePending = true;
                }
                if (pProfile.changeAccelSens)
                {
                    Console.WriteLine("ShimmerControl: changeAccelSens");
                    if (wait)
                        System.Threading.Thread.Sleep(500);
                    serialPort1.Write(new byte[2] { (byte)Shimmer.PacketType.SetAccelRangeCommand,
                        (byte)pProfile.GetAccelRange()}, 0, 2);
                    pProfile.changeAccelSens = false;
                    wait = true;
                }
                if (pProfile.changeGsrRange)
                {
                    Console.WriteLine("ShimmerControl: changeGsrRange");
                    if (wait)
                        System.Threading.Thread.Sleep(500);
                    serialPort1.Write(new byte[2] { (byte)Shimmer.PacketType.SetGsrRangeCommand, 
                        (byte)pProfile.GetGsrRange()}, 0, 2);
                    pProfile.changeGsrRange = false;
                }
            }
        }

        public void ToggleLED()
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Write(new byte[1] { (byte)Shimmer.PacketType.ToggleLedCommand }, 0, 1);
            }
            else
                MessageBox.Show("No serial port is open", Shimmer.ApplicationName,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void ShowChannelTextBoxes()
        {
            if(this.btnConnect.InvokeRequired)  // will be in the same thread as the controls to be added
            {
                ShowChannelTextBoxesCallback d = new ShowChannelTextBoxesCallback(ShowChannelTextBoxes);
                this.Invoke(d);
            }
            else
            {
                if (channelTextBox.Count > pProfile.GetNumChannels())
                {
                    while (channelTextBox.Count != pProfile.GetNumChannels())
                    {
                        this.Controls.Remove(channelTextBox[channelTextBox.Count - 1]);
                        channelTextBox.RemoveAt(channelTextBox.Count - 1);

                        this.Controls.Remove(channelLabel[channelLabel.Count - 1]);
                        channelLabel.RemoveAt(channelLabel.Count - 1);
                    }
                }
                for (int i = 0; i < pProfile.GetNumChannels(); i++)
                {
                    if (i == channelTextBox.Count)
                    {
                        channelTextBox.Add(new TextBox());
                        channelTextBox[i].BackColor = System.Drawing.SystemColors.Window;
                        channelTextBox[i].Location = textBoxLocation[i];
                        channelTextBox[i].ReadOnly = true;
#if _PLATFORM_LINUX
                        channelTextBox[i].Size = new System.Drawing.Size(108, 20);
#else
                        // WINDOWS
                        channelTextBox[i].Size = new System.Drawing.Size(90, 20);
#endif
                        this.Controls.Add(channelTextBox[i]);

                        channelLabel.Add(new Label());
                        channelLabel[i].AutoSize = true;
                        channelLabel[i].Location = labelLocation[i];
                        this.Controls.Add(channelLabel[i]);
                    }
                    if ((pProfile.GetChannel(i) == (int)Shimmer.ChannelContents.AnExA0) && pProfile.GetPMux())
                    {
                        channelLabel[i].Text = "VSenseReg";
                    }
                    else if ((pProfile.GetChannel(i) == (int)Shimmer.ChannelContents.AnExA7) && pProfile.GetPMux())
                    {
                        channelLabel[i].Text = "VSenseBatt";
                    }
                    else
                    {
                        channelLabel[i].Text = Enum.GetName(typeof(Shimmer.ChannelContents), pProfile.GetChannel(i));
                    }
                    
                }
                if (pProfile.showMagHeading || pProfile.showGsrResistance)
                {
                    extraInfoTextBox.BackColor = System.Drawing.SystemColors.Window;
                    extraInfoTextBox.Location = textBoxLocation[channelTextBox.Count];
                    extraInfoTextBox.ReadOnly = true;
#if _PLATFORM_LINUX
                    extraInfoTextBox.Size = new System.Drawing.Size(90, 20);
#else
                    // WINDOWS
                    extraInfoTextBox.Size = new System.Drawing.Size(90, 20);
#endif
                    this.Controls.Add(extraInfoTextBox);

                    extraInfoLabel.AutoSize = true;
                    extraInfoLabel.Location = labelLocation[channelLabel.Count];
                    if (pProfile.showMagHeading)
                    {
                        extraInfoLabel.Text = "MagHeading";
                    }
                    else if (pProfile.showGsrResistance)
                    {
                        extraInfoLabel.Text = "GsrResistance";
                    }
                    this.Controls.Add(extraInfoLabel);
                }
                else
                {
                    this.Controls.Remove(extraInfoTextBox);
                    this.Controls.Remove(extraInfoLabel);
                }
            }
        }

        public void StartRecording(string sFileName)
        {
            int iTickCount=Environment.TickCount;
            if (sFileName.Length != 0)
            {
                string sFileName_Data = sFileName + ".dat";
                string sFileName_Log = sFileName + ".log";

                if (File.Exists(sFileName_Data)) File.Delete(sFileName_Data);
                if (File.Exists(sFileName_Log)) File.Delete(sFileName_Log);
                m_Stream_DataFile = File.Create(sFileName_Data);
                m_Stream_LogFile = File.Create(sFileName_Log);
                m_Writer_LogFile = new BinaryWriter(m_Stream_LogFile);
                m_iSampleCount_DataFile = 0;
            }

            m_FileStartTimeCode = Environment.TickCount;

            SaveToFile(true, new StreamWriter(m_Stream_DataFile));
        }

        public void StopRecording()
        {
            pSaveToFile=false;
            m_iSampleCount_DataFile = 0;
            if (m_Stream_DataFile != null) { pCsvFile.Flush(); m_Stream_DataFile.Flush(); m_Stream_DataFile.Close(); m_Stream_DataFile = null; }
            if (m_Stream_LogFile != null) { m_Stream_LogFile.Flush(); m_Stream_LogFile.Close(); m_Stream_LogFile = null; }
        }

        public void SaveToFile(bool saveToFile, StreamWriter csvFile)
        {
            pCsvFile = csvFile;
            SaveToFile(saveToFile);
        }

        public void SaveToFile(bool saveToFile)
        {
            pSaveToFile = saveToFile;
            if (pSaveToFile && pProfile.GetIsFilled() && serialPort1.IsOpen)
                WriteHeaderToFile();
        }

        public void WriteHeaderToFile()
        {
            pCsvFile.Write("TimeStamp");
            for (int i = 0; i < pProfile.GetNumChannels(); i++)
            {
                if ((pProfile.GetChannel(i) == (int)Shimmer.ChannelContents.AnExA0) && pProfile.GetPMux())
                {
                    pCsvFile.Write("," + "VSenseReg");
                }
                else if ((pProfile.GetChannel(i) == (int)Shimmer.ChannelContents.AnExA7) && pProfile.GetPMux())
                {
                    pCsvFile.Write("," + "VSenseBatt");
                }
                else if (pProfile.GetChannel(i) == (int)Shimmer.ChannelContents.GsrRaw)
                {
                    // Add extra column to record the active GSR range
                    pCsvFile.Write("," + "Active GSR Range" + "," + Enum.GetName(typeof(Shimmer.ChannelContents), pProfile.GetChannel(i)));
                }
                else
                {
                    pCsvFile.Write("," + Enum.GetName(typeof(Shimmer.ChannelContents), pProfile.GetChannel(i)));
                }
            }
            pCsvFile.Write("\n");
        }

        public void UpdateGraphs(bool val)
        {
            pUpdateGraphs = val;
        }

        public int MagHeading(Int16 x, Int16 y, Int16 z)
        {
            int heading;

            if (x == 0)
            {
                if (y < 0)
                    heading = 270;
                else
                    heading = 90;
            }
            else if (z < 0)
                heading = (int)(180.0 - RadianToDegree(Math.Atan2((float)y, (float)-x)));
            else
                heading = (int)(180.0 - RadianToDegree(Math.Atan2((float)y, (float)x)));

            return heading;
        }

        private double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }

        public int GsrResistance(int gsrAdcVal, int gsrRange)
        {
            int resistance = 0;
            switch (gsrRange)
            {
                // curve fitting using a 4th order polynomial
                case (int)Shimmer.GsrRange.HW_RES_40K:
                    resistance = (int)(
                        ((0.0000000065995) * Math.Pow(gsrAdcVal, 4)) +
                        ((-0.000068950)    * Math.Pow(gsrAdcVal, 3)) +
                        ((0.2699)          * Math.Pow(gsrAdcVal, 2)) +
                        ((-476.9835)       * Math.Pow(gsrAdcVal, 1)) + 340351.3341);
                    break;
                case (int)Shimmer.GsrRange.HW_RES_287K:
                    resistance = (int)(
                        ((0.000000013569627) * Math.Pow(gsrAdcVal, 4)) +
                        ((-0.0001650399)     * Math.Pow(gsrAdcVal, 3)) +
                        ((0.7541990)         * Math.Pow(gsrAdcVal, 2)) +
                        ((-1572.6287856)     * Math.Pow(gsrAdcVal, 1)) + 1367507.9270);
                    break;
                case (int)Shimmer.GsrRange.HW_RES_1M:
                    resistance = (int)(
                        ((0.00000002550036498) * Math.Pow(gsrAdcVal, 4)) +
                        ((-0.00033136)         * Math.Pow(gsrAdcVal, 3)) +
                        ((1.6509426597)        * Math.Pow(gsrAdcVal, 2)) +
                        ((-3833.348044)        * Math.Pow(gsrAdcVal, 1)) + 3806317.6947);
                    break;
                case (int)Shimmer.GsrRange.HW_RES_3M3:
                    resistance = (int)(
                        ((0.00000037153627) * Math.Pow(gsrAdcVal, 4)) +
                        ((-0.004239437)     * Math.Pow(gsrAdcVal, 3)) +
                        ((17.905709)        * Math.Pow(gsrAdcVal, 2)) +
                        ((-33723.8657)      * Math.Pow(gsrAdcVal, 1)) + 25368044.6279);
                    break;
            }
            return resistance;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cmbComPortSelect.Text)) {
                MessageBox.Show("ComPort not selected!");
                return;
            }
        }
    }

    public class ShimmerDAQEventArgs : EventArgs {
        public ShimmerDAQEventArgs(int[] indata)
        {
            Data = indata;
        }

        /// <summary>
        /// Byte array containing data from FiberSensorDAQ
        /// </summary>
        public int[] Data;
    }
}
