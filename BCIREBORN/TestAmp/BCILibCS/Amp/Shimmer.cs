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
using System.Collections.Generic;
using System.Text;

namespace BCILib.Amp
{
    public class Shimmer
    {
        public static readonly string ApplicationName = "ShimmerConnect";

        public enum SamplingRates
        {
            Sampling1000Hz =    1,
            Sampling500Hz =     2,
            Sampling250Hz =     4,
            Sampling200Hz =     5,
            Sampling166Hz =     6,
            Sampling125Hz =     8,
            Sampling100Hz =     10,
            Sampling50Hz =      20,
            Sampling10Hz =      100,
            Sampling0HzOff =    255
        }

        public static readonly string[] SamplingRatesString = new string[] {
            "    0Hz (Off)",
            "  10Hz",
            "  50Hz",
            "100Hz",
            "125Hz",
            "166Hz",
            "200Hz",
            "250Hz",
            "500Hz",
            "    1kHz"};

        public enum PacketType : byte
        {
            DataPacket                  = 0x00,
            InquiryCommand              = 0x01,
            InquiryResponse             = 0x02,
            GetSamplingRateCommand      = 0x03,
            SamplingRateResponse        = 0x04,
            SetSamplingRateCommand      = 0x05,
            ToggleLedCommand            = 0x06,
            StartStreamingCommand       = 0x07,
            SetSensorsCommand           = 0x08,
            SetAccelRangeCommand        = 0x09,
            AccelRangeResponse          = 0x0A,
            GetAccelRangeCommand        = 0x0B,
            Set5VRegulatorCommand       = 0x0C,
            SetPowerMuxCommand          = 0x0D,
            SetConfigSetupByte0Command  = 0x0E,
            ConfigSetupByte0Response    = 0x0F,
            GetConfigSetupByte0Command  = 0x10,
            SetAccelCalibrationCommand  = 0x11,
            AccelCalibrationResponse    = 0x12,
            GetAccelCalibrationCommand  = 0x13,
            SetGyroCalibrationCommand   = 0x14,
            GyroCalibrationResponse     = 0x15,
            GetGyroCalibrationCommand   = 0x16,
            SetMagCalibrationCommand    = 0x17,
            MagCalibrationResponse      = 0x18,
            GetMagCalibrationCommand    = 0x19,
            StopStreamingCommand        = 0x20,
            SetGsrRangeCommand          = 0x21,
            gsrRangeResponse            = 0x22,
            GetGsrRangeCommand          = 0x23,
            GetShimmerVersionCommand    = 0x24,
            ShimmerVersionResponse      = 0x25,
            AckCommandProcessed         = 0xFF
        };

        public enum ChannelContents
        {
            XAccel      = 0x00,
            YAccel      = 0x01,
            ZAccel      = 0x02,
            XGyro       = 0x03,
            YGyro       = 0x04,
            ZGyro       = 0x05,
            XMag        = 0x06,
            YMag        = 0x07,
            ZMag        = 0x08,
            EcgRaLl     = 0x09,
            EcgLaLl     = 0x0A,
            GsrRaw      = 0x0B,
            GsrRes      = 0x0C,
            Emg         = 0x0D,
            AnExA0      = 0x0E,
            AnExA7      = 0x0F,
            StrainHigh  = 0x10,
            StrainLow   = 0x11,
            HeartRate   = 0x12
        }

        public enum AdcChannels
        {
            XAccel      = 0x00,
            YAccel      = 0x01,
            ZAccel      = 0x02,
            XGyro       = 0x03,
            YGyro       = 0x04,
            ZGyro       = 0x05,
            EcgRaLl     = 0x09,
            EcgLaLl     = 0x0A,
            GsrRaw      = 0x0B,
            GsrRes      = 0x0C,
            Emg         = 0x0D,
            AnExA0      = 0x0E,
            AnExA7      = 0x0F,
            StrainHigh  = 0x10,
            StrainLow   = 0x11
        }

        public enum TwoByteDigiChannels
        {
            XMag = 0x06,
            YMag = 0x07,
            ZMag = 0x08
        }

        public enum OneByteDigiChannels
        {
            HeartRate = 0x12
        }

        public enum Sensor0Bitmap
        {
            SensorAccel     = 0x80,
            SensorGyro      = 0x40,
            SensorMag       = 0x20,
            SensorECG       = 0x10,
            SensorEMG       = 0x08,
            SensorGSR       = 0x04,
            SensorAnExA7    = 0x02,
            SensorAnExA0    = 0x01
        }

        public enum Sensor1Bitmap
        {
            SensorStrain    = 0x80,
            SensorHeart     = 0x40
        }

        public enum ConfigSetupByte0Bitmap
        {
            Config5VReg = 0x80,
            ConfigPMux  = 0x40, 
        }

        public enum AccelRange
        {
            RANGE_1_5G = 0,
            RANGE_2_0G = 1,
            RANGE_4_0G = 2,
            RANGE_6_0G = 3
        }

        public enum GsrRange
        {
            HW_RES_40K    = 0,
            HW_RES_287K   = 1,
            HW_RES_1M     = 2,
            HW_RES_3M3    = 3,
            AUTORANGE     = 4
        }

        public enum ShimmerVersion
        {
            SHIMMER1  = 0,
            SHIMMER2  = 1,
            SHIMMER2R = 2
        }

        public enum MaxNumChannels
        {
            MaxNum2ByteChannels = 11,   // (3xAccel) + (3xGyro) + (3xMag) + (2xAnEx)
            MaxNum1ByteChannels = 1,
            MaxNumChannels = MaxNum2ByteChannels + MaxNum1ByteChannels
        }

        public enum MaxPacketSizes
        {
            DataPacketSize      = 3 + ((int)MaxNumChannels.MaxNum2ByteChannels * 2) + (int)MaxNumChannels.MaxNum1ByteChannels,
            //ResponsePacketSize  = 6 + (int)MaxNumChannels.MaxNumChannels,
            ResponsePacketSize = 22,
            MaxCommandArgSize   = 21
        }

        public static int NumSensorBytes = 2;

        public static readonly string[] AccelRangeString = new string[] {
            "±1.5g",
            "±2g",
            "±4g",
            "±6g"};

        public static readonly string[] GsrRangeString = new string[] {
            "  10kΩ– 56kΩ", 
            "  56kΩ-220kΩ",
            "220kΩ-680kΩ",
            "680kΩ–4.7MΩ",
            "  Auto-Range"};
    }

    public class ShimmerDataPacket
    {

        private int timeStamp;
        private List<int> channels = new List<int>();
        private int numAdcChannels;
        private int num1ByteDigiChannels;
        private int num2ByteDigiChannels;
        private int numChannels;
        private bool isFilled = false;

        public ShimmerDataPacket(List<byte> packet, int numAdcChans, int num1ByteDigiChans, int num2ByteDigiChans)
        {
            int i;

            numAdcChannels = numAdcChans;
            num1ByteDigiChannels = num1ByteDigiChans;
            num2ByteDigiChannels = num2ByteDigiChans;
            numChannels = numAdcChannels + num1ByteDigiChannels + num2ByteDigiChannels;
            if (packet.Count >= (2 + ((numAdcChannels + num2ByteDigiChans)* 2) + num1ByteDigiChans))    // timestamp + channel data
            {
                timeStamp = (int)packet[0];
                timeStamp += ((int)packet[1] << 8) & 0xFF00;
                // 16 bit channels come first
                for (i = 0; i < (numAdcChannels + num2ByteDigiChannels); i++)
                {
                    channels.Add((int)packet[2 + (2 * i)]);
                    channels[i] += ((int)packet[2 + (2 * i) + 1] << 8) & 0xFF00;
                }
                // 8 bit channels
                i *= 2;
                for (int j = 0; j < num1ByteDigiChannels; j++, i++)
                {
                    channels.Add((int)packet[2 + i]);
                }
                isFilled = true;
            }
        }

        public int GetTimeStamp()
        {
            return timeStamp;
        }

        public int GetNumChannels()
        {
            return numChannels;
        }

        public int GetNumAdcChannels()
        {
            return numAdcChannels;
        }

        public int GetNum1ByteDigiChannels()
        {
            return num1ByteDigiChannels;
        }

        public int GetNum2ByteDigiChannels()
        {
            return num1ByteDigiChannels;
        }

        public int GetChannel(int channelNum)
        {
            // channelNum is indexed from 0
            if (channelNum >= numChannels)
                return -1;
            else
                return channels[channelNum];
        }

        public void SetChannel(int channelNum, int val)
        {
            // channelNum is indexed from 0
            if (channelNum < numChannels)
                channels[channelNum] = val;
        }

        public bool GetIsFilled()
        {
            return isFilled;
        }
    }

    public class ShimmerProfile
    {
        public bool changeSamplingRate;
        public bool changeSensors;
        public bool change5Vreg;
        public bool changePwrMux;
        public bool changeAccelSens;
        public bool changeGsrRange;

        public bool showMagHeading;
        public bool showGsrResistance;

        private int adcSamplingRate;
        private int numChannels;
        private int numAdcChannels;
        private int num1ByteDigiChannels;
        private int num2ByteDigiChannels;
        private int bufferSize;
        private List<int> channels = new List<int>();

        private int[] sensors = new int[Shimmer.NumSensorBytes];
        private int accelRange;
        private int configSetupByte0;
        private int gsrRange;

        private bool isFilled;

        public ShimmerProfile()
        {
            adcSamplingRate = -1;
            numChannels = 0;
            numAdcChannels = 0;
            num1ByteDigiChannels = 0;
            num2ByteDigiChannels = 0;
            bufferSize = 0;
            isFilled = false;
            sensors[0] = 0;
            sensors[1] = 0;
            accelRange = 0;
            configSetupByte0 = 0;
            gsrRange = 0;

            changeSamplingRate = false;
            changeSensors = false;
            change5Vreg = false;
            changePwrMux = false;
            changeAccelSens = false;
            changeGsrRange = false;
            showMagHeading = false;
            showGsrResistance = false;
        }

        public ShimmerProfile(List<byte> packet)
        {
            isFilled = false;
            accelRange = 0;
            configSetupByte0 = 0;
            gsrRange = 0;

            changeSamplingRate = false;
            changeSensors = false;
            change5Vreg = false;
            changePwrMux = false;
            changeAccelSens = false;
            changeGsrRange = false;
            showMagHeading = false;
            showGsrResistance = false;

            numAdcChannels = 0;
            num1ByteDigiChannels = 0;
            num2ByteDigiChannels = 0;

            fillProfile(packet);
        }

        public void fillProfile(List<byte> packet)
        {
            //check if this packet is sane, and not just random
            if ((packet.Count >= 5) // minimum size
                && (packet.Count < (int)Shimmer.MaxPacketSizes.ResponsePacketSize)     // maximum size
                && ((int)packet[3] <= (int)Shimmer.MaxNumChannels.MaxNumChannels)      // max number of channels currently allowable
                && (Enum.IsDefined(typeof(Shimmer.AccelRange), (int)packet[1])))       // ensure accel range is an allowable value
            {
                adcSamplingRate = (int)packet[0];
                accelRange = (int)packet[1];
                configSetupByte0 = (int)packet[2];
                numChannels = (int)packet[3];
                bufferSize = (int)packet[4];

                channels.Clear();

                for (int i = 0; i < numChannels; i++)
                {
                    channels.Add((int)packet[5 + i]);
                }
                isFilled = true;

                UpdateSensorsFromChannels();
            }
        }

        public int GetAdcSamplingRate()
        {
            return adcSamplingRate;
        }

        public void SetAdcSamplingRate(int rate)
        {
            if(Enum.IsDefined(typeof(Shimmer.SamplingRates), rate))
                adcSamplingRate = rate;
        }

        public int GetBufferSize()
        {
            return bufferSize;
        }

        public void SetBufferSize(int size)
        {
            bufferSize = size;
        }

        public int GetNumChannels()
        {
            return numChannels;
        }

        public void SetNumChannels(int num)
        {
            numChannels = num;
        }

        public int GetNumAdcChannels()
        {
            return numAdcChannels;
        }

        public int GetNum1ByteDigiChannels()
        {
            return num1ByteDigiChannels;
        }

        public int GetNum2ByteDigiChannels()
        {
            return num2ByteDigiChannels;
        }

        public int GetChannel(int channelNum)
        {
            if (channelNum >= numChannels)
                return -1;
            else
                return channels[channelNum];
        }

        public bool GetIsFilled()
        {
            return isFilled;
        }

        public void SetVReg(bool val)
        {
            //vReg = val;
            if (val)
            {
                configSetupByte0 |= (int)Shimmer.ConfigSetupByte0Bitmap.Config5VReg;
            }
            else
            {
                configSetupByte0 &= ~(int)Shimmer.ConfigSetupByte0Bitmap.Config5VReg;   
            }
        }

        public bool GetVReg()
        {
            //return vReg;
            if ((configSetupByte0 & (int)Shimmer.ConfigSetupByte0Bitmap.Config5VReg) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void SetPMux(bool val)
        {
            if (val)
            {
                configSetupByte0 |= (int)Shimmer.ConfigSetupByte0Bitmap.ConfigPMux;
            }
            else
            {
                configSetupByte0 &= ~(int)Shimmer.ConfigSetupByte0Bitmap.ConfigPMux;
            }
        }

        public bool GetPMux()
        {
            //return pMux;
            if ((configSetupByte0 & (int)Shimmer.ConfigSetupByte0Bitmap.ConfigPMux) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void setSensors(int[] val)
        {
            val.CopyTo(sensors, 0);
        }

        public int[] GetSensors()
        {
            return sensors;
        }

        public void SetAccelRange(int val)
        {
            if(Enum.IsDefined(typeof(Shimmer.AccelRange), val))
                accelRange = val;
        }

        public int GetAccelRange()
        {
            return accelRange;
        }

        public void SetConfigSetupByte0(int val)
        {
            configSetupByte0 = val;
        }

        public int GetConfigSetupByte0()
        {
            return configSetupByte0;
        }

        public void SetGsrRange(int val)
        {
            if(Enum.IsDefined(typeof(Shimmer.GsrRange), val))
                gsrRange = val;
        }

        public int GetGsrRange()
        {
            return gsrRange;
        }

        public void UpdateSensorsFromChannels()
        {
            // set the sensors value
            // crude way of getting this value, but allows for more customised firmware
            // to still work with this application
            // e.g. if any axis of the accelerometer is being transmitted, then it will
            // recognise that the accelerometer is being sampled
            sensors[0] = 0;
            sensors[1] = 0;
            numAdcChannels = 0;
            num1ByteDigiChannels = 0;
            num2ByteDigiChannels = 0;
            showMagHeading = false;
            showGsrResistance = false;
            foreach (int channel in channels)
            {
                if ((channel == (int)Shimmer.ChannelContents.XAccel) ||
                    (channel == (int)Shimmer.ChannelContents.YAccel) ||
                    (channel == (int)Shimmer.ChannelContents.ZAccel))
                {
                    sensors[0] |= (int)Shimmer.Sensor0Bitmap.SensorAccel;
                }
                else if ((channel == (int)Shimmer.ChannelContents.XGyro) ||
                         (channel == (int)Shimmer.ChannelContents.YGyro) ||
                         (channel == (int)Shimmer.ChannelContents.ZGyro))
                {
                    sensors[0] |= (int)Shimmer.Sensor0Bitmap.SensorGyro;
                }
                else if ((channel == (int)Shimmer.ChannelContents.XMag) ||
                         (channel == (int)Shimmer.ChannelContents.YMag) ||
                         (channel == (int)Shimmer.ChannelContents.ZMag))
                {
                    sensors[0] |= (int)Shimmer.Sensor0Bitmap.SensorMag;
                    showMagHeading = true;
                }
                else if ((channel == (int)Shimmer.ChannelContents.EcgLaLl) ||
                         (channel == (int)Shimmer.ChannelContents.EcgRaLl))
                {
                    sensors[0] |= (int)Shimmer.Sensor0Bitmap.SensorECG;
                }
                else if (channel == (int)Shimmer.ChannelContents.Emg)
                {
                    sensors[0] |= (int)Shimmer.Sensor0Bitmap.SensorEMG;
                }
                else if (channel == (int)Shimmer.ChannelContents.AnExA0)
                {
                    sensors[0] |= (int)Shimmer.Sensor0Bitmap.SensorAnExA0;
                }
                else if (channel == (int)Shimmer.ChannelContents.AnExA7)
                {
                    sensors[0] |= (int)Shimmer.Sensor0Bitmap.SensorAnExA7;
                }
                else if ((channel == (int)Shimmer.ChannelContents.StrainHigh) ||
                         (channel == (int)Shimmer.ChannelContents.StrainLow))
                {
                    sensors[1] |= (int)Shimmer.Sensor1Bitmap.SensorStrain;
                }
                else if ((channel == (int)Shimmer.ChannelContents.GsrRaw) ||
                         (channel == (int)Shimmer.ChannelContents.GsrRes))
                {
                    sensors[0] |= (int)Shimmer.Sensor0Bitmap.SensorGSR;
                    showGsrResistance = true;
                }
                else if (channel == (int)Shimmer.ChannelContents.HeartRate)
                {
                    sensors[1] |= (int)Shimmer.Sensor1Bitmap.SensorHeart;
                }
                if (Enum.IsDefined(typeof(Shimmer.AdcChannels), channel))
                {
                    numAdcChannels++;
                }
                else if (Enum.IsDefined(typeof(Shimmer.OneByteDigiChannels), channel))
                {
                    num1ByteDigiChannels++;
                }
                else if (Enum.IsDefined(typeof(Shimmer.TwoByteDigiChannels), channel))
                {
                    num2ByteDigiChannels++;
                }
            }
        }

        public void UpdateChannelsFromSensors()
        {
            showMagHeading = false;
            showGsrResistance = false;
            channels.Clear();
            numAdcChannels = 0;
            num1ByteDigiChannels = 0;
            num2ByteDigiChannels = 0;
            if ((sensors[0] & (int)Shimmer.Sensor0Bitmap.SensorAccel) != 0)
            {
                channels.Add((int)Shimmer.ChannelContents.XAccel);
                channels.Add((int)Shimmer.ChannelContents.YAccel);
                channels.Add((int)Shimmer.ChannelContents.ZAccel);
                numAdcChannels += 3;
            }
            if ((sensors[0] & (int)Shimmer.Sensor0Bitmap.SensorGyro) != 0)
            {
                channels.Add((int)Shimmer.ChannelContents.XGyro);
                channels.Add((int)Shimmer.ChannelContents.YGyro);
                channels.Add((int)Shimmer.ChannelContents.ZGyro);
                numAdcChannels += 3;
            }
            if ((sensors[0] & (int)Shimmer.Sensor0Bitmap.SensorECG) != 0)
            {
                channels.Add((int)Shimmer.ChannelContents.EcgRaLl);
                channels.Add((int)Shimmer.ChannelContents.EcgLaLl);
                numAdcChannels += 2;
            }
            if ((sensors[0] & (int)Shimmer.Sensor0Bitmap.SensorEMG) != 0)
            {
                channels.Add((int)Shimmer.ChannelContents.Emg);
                numAdcChannels++;
            }
            if ((sensors[0] & (int)Shimmer.Sensor0Bitmap.SensorAnExA7) != 0)
            {
                channels.Add((int)Shimmer.ChannelContents.AnExA7);
                numAdcChannels++;
            }
            if ((sensors[0] & (int)Shimmer.Sensor0Bitmap.SensorAnExA0) != 0)
            {
                channels.Add((int)Shimmer.ChannelContents.AnExA0);
                numAdcChannels++;
            }
            if ((sensors[1] & (int)Shimmer.Sensor1Bitmap.SensorStrain) != 0)
            {
                channels.Add((int)Shimmer.ChannelContents.StrainHigh);
                channels.Add((int)Shimmer.ChannelContents.StrainLow);
                numAdcChannels += 2;
            }
            if ((sensors[0] & (int)Shimmer.Sensor0Bitmap.SensorGSR) != 0)
            {
                channels.Add((int)Shimmer.ChannelContents.GsrRaw);
                numAdcChannels++;
                showGsrResistance = true;
            }
            if ((sensors[0] & (int)Shimmer.Sensor0Bitmap.SensorMag) != 0)
            {
                channels.Add((int)Shimmer.ChannelContents.XMag);
                channels.Add((int)Shimmer.ChannelContents.YMag);
                channels.Add((int)Shimmer.ChannelContents.ZMag);
                showMagHeading = true;
                num2ByteDigiChannels += 3;
            }
            if ((sensors[1] & (int)Shimmer.Sensor1Bitmap.SensorHeart) != 0)
            {
                channels.Add((int)Shimmer.ChannelContents.HeartRate);
                num1ByteDigiChannels++;
            }
            numChannels = channels.Count;
        }
    }
}
