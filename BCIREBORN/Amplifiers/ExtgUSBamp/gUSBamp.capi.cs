using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;

namespace ExtgUSBamp
{
    partial class gUSBamp
    {
        // this file is located in C:\Windows\System32
        public const string DllPath = "gUSBamp.dll";

        public enum CHName : byte
        {
            CH1, CH2, CH3, CH4, CH5, CH6, CH7, CH8,
            CH9, CH10, CH11, CH12, CH13, CH14, CH15, CH16
        }

        public struct CHDEF
        {
            public CHName channel1;
            public CHName channel2;
            public CHName channel3;
            public CHName channel4;
            public CHName channel5;
            public CHName channel6;
            public CHName channel7;
            public CHName channel8;
            public CHName channel9;
            public CHName channel10;
            public CHName channel11;
            public CHName channel12;
            public CHName channel13;
            public CHName channel14;
            public CHName channel15;
            public CHName channel16;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FILT
        {
            /// <summary>
            /// lower border frequency of filter
            /// </summary>
            public float fu;
            /// <summary>
            /// upper border frequency of filter
            /// </summary>
            public float fo;
            /// <summary>
            /// sampling rate
            /// </summary>
            public float fs;
            /// <summary>
            /// filter type 1 - CHEBYSHEV 2 - BUTTERWORTH
            /// </summary>
            public float type;
            /// <summary>
            /// filter order
            /// </summary>
            public float order;

            public override string ToString()
            {
                return string.Format("Type:{0} | Order:{1} | S:{2} | {3}-{4}", type, order, fs, fu, fo);
            }
        };

        [Serializable]
        public struct GND
        {
            public int GND1;
            public int GND2;
            public int GND3;
            public int GND4;
        }

        [Serializable]
        public struct REF
        {
            public int ref1;
            public int ref2;
            public int ref3;
            public int ref4;
        }


        public enum AmpMode : byte
        {
            NORMAL = 0, IMPEDANCE = 1, CALIBRATE = 2, COUNTER = 3
        }

        public enum FilterType
        {
            CHEBYSHEV = 0, BUTTERWORTH = 1, BESSEL = 2
        }

        public enum WAVESHAPE : byte
        {
            SQUARE = 1, SAWTOOTH, SINE, DRL, NOICE
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DAC
        {
            public WAVESHAPE WaveShape;
            public ushort Amplitude;
            public ushort Frequency;
            public ushort Offset;
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct SCALE
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public float[] factor;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public float[] offset;
        }

        #region Data acquisition
        /// <summary>
        /// Opens the specified g.USBamp and returns a handle to it.
        /// The handle must be stored in the program because all function
        /// calls of the API need this handle as an input parameter.
        /// The handle must be deleted when the application is closed
        /// with GT_CloseDevice.
        /// </summary>
        /// <param name="port_number">number of USB port</param>
        /// <returns>Handle of the device. NULL if the opening fails</returns>
        [DllImport(DllPath)]
        private static extern IntPtr GT_OpenDevice(int port_number);

        /// <summary>
        /// Opens the specified g.USBamp and returns a handle to it.
        /// The handle must be stored in the program because all function
        /// calls of the API need this handle as an input parameter.
        /// The handle must be deleted when the application is closed
        /// with GT_CloseDevice.
        /// </summary>
        /// <param name="serial">pointer to a NULL terminated string containing the device
        /// serial e.g. “UA-2005.02.01”</param>
        /// <returns>Handle of the device. NULL if the opening fails</returns>
        [DllImport(DllPath)]
        private static extern IntPtr GT_OpenDeviceEx(string serial);

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public  static extern  bool GetOverlappedResult(IntPtr hFile,
            ref NativeOverlapped lpOverlapped,
            out int lpNumberOfBytesTransferred, bool bWait);

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.Winapi)]
        public static extern void ResetEvent(IntPtr handler);

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.Winapi)]
        public static extern int WaitForSingleObject(IntPtr hHandle, int dwMilliseconds);

        /// <summary>
        /// Extract data from the driver buffer. The function does not block
        /// the calling thread because of the overlapped mode.
        /// The function call returns immediately but data is not valid
        /// until the event in the OVERLAPPED structure is triggered.
        /// Use WaitForSingleObject() to determine if the transfer has finished.
        /// Use GetOverlappedResult() to retrieve the number of bytes that are available.
        /// </summary>
        /// <param name="dev">handle of the device</param>
        /// <param name="data">pointer to buffer to hold data retrieved from the driver</param>
        /// <param name="sz">buffer size defines the number of bytes received
        /// from the driver. dwSzBuffer must correspond to the value wBufferSize
        /// in GT_SetBufferSize. Furthermore HEADER_SIZE bytes precede
        /// the acquired data and have to be discarded. e.g. 16 channels
        /// sampled at 128 Hz, wBufferSize set to 8:
        /// dwSzBuffer = 8 scans * 16 channels * sizeof(float) + HEADER_SIZE;</param>
        /// <param name="ov">ov pointer to OVERLAPPED structure used to 
        /// perform the overlapped I/O transfer</param>
        /// <returns>Status=0 command not successful
        /// Status=1 command successful</returns>
        [DllImport(DllPath)]
        private static extern int GT_GetData(IntPtr dev, IntPtr data, int sz,
            ref NativeOverlapped ov);

        /// <summary>
        /// Closes the g.USBamp identified by the handle hDevice.
        /// The function returns true if the call succeeded otherwise it will return false.
        /// Use GT_OpenDevice to retrieve a handle to the g.USBamp.
        /// </summary>
        /// <param name="dev">handle of the device</param>
        /// <returns>Status=0 not able to close the device
        /// Status=1 device closed successfully</returns>
        [DllImport(DllPath)]
        private static extern int GT_CloseDevice(ref IntPtr dev);

        /// <summary>
        /// Sets the buffer size of the driver.
        /// </summary>
        /// <param name="dev">handle of the device</param>
        /// <param name="num_scan">number of scans to receive per buffer.
        /// Buffer size should be at least 20-30 ms (60 ms recommended).
        /// To calculate a 60 ms buffer use following equation:
        /// (wBufferSize) >= sample rate*60/1000
        /// Example: sample rate = 128 Hz: 128*60/1000 = 7.68 so buffer size is 8 scans.
        /// ***************SHOULD BE LESS THAN 512****************
        /// </param>
        /// <returns>The function returns true if the call succeeded otherwise it will return false.</returns>
        [DllImport(DllPath)]
        private static extern int GT_SetBufferSize(IntPtr dev, ushort num_scan);
        /// <summary>
        /// Set the sampling frequency of the g.USBamp. The sampling frequency
        /// value must correspond to a value defined in the table below. 
        /// The over sampling rate depends directly on the selected sampling
        /// frequency. A small sampling frequency will result in a higher 
        /// over sampling rate
        /// Sampling Rate [Hz]:  32  /  64 / 128 / 256 / 512 / 600 / 1200 / 2400 / 4800 / 9600 / 19200 / 38400
        /// Over sampling Rate: 1200 / 600 / 300 / 150 /  75 /  64 /  32  /  16  /   8  /  4   /  2    /   1 
        /// Buffer size           1  /   2 /   4 /  8  /  16 /  32 / 128  / 256  /  512 /  512 /  512  /  512
        /// </summary>
        /// <param name="dev">handle of the device</param>
        /// <param name="spr">sample rate</param>
        /// <returns></returns>
        [DllImport(DllPath)]
        private static extern int GT_SetSampleRate(IntPtr dev, ushort spr);

        /// <summary>
        /// Start the data acquisition.
        /// Note that the sampling frequency, buffer configuration and channels must be set before.
        /// Note: You have to extract data permanently from driver buffer
        /// to prevent a buffer overrun. Please refer to the sample code
        /// if you are not sure about this topic. See also GT_GetData.
        /// </summary>
        /// <param name="dev">handle of the device</param>
        /// <returns>The function returns true if the call succeeded otherwise it will return false.</returns>
        [DllImport(DllPath)]
        private static extern int GT_Start(IntPtr dev);

        /// <summary>
        /// Stop the acquisition.
        /// </summary>
        /// <param name="dev">handle of the device.</param>
        /// <returns>The function returns true if the call succeeded otherwise it will return false.</returns>
        [DllImport(DllPath)]
        private static extern int GT_Stop(IntPtr dev);

        /// <summary>
        /// Define the channels that should be recorded.
        /// The function returns true if the call succeeded otherwise it will return false.
        /// </summary>
        /// <param name="dev">handle of the device</param>
        /// <param name="channels">define the channels that should be acquired
        /// [1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16] acquires all 16 channels
        /// [1 2 3 4] acquires channels 1 - 4</param>
        /// <param name="num_ch">total number of acquired channels (1-16)</param>
        /// <returns></returns>
        [DllImport(DllPath)]
        private static extern int GT_SetChannels(IntPtr dev, byte[] channels, byte num_ch);

        /// <summary>
        /// Reset the driver data pipe after data transmission error (e.g. time out).
        /// The function returns true if the call succeeded otherwise it will return false.
        /// </summary>
        /// <param name="dev">handle of the device</param>
        /// <returns></returns>
        [DllImport(DllPath)]
        private static extern int GT_ResetTransfer(IntPtr dev);
        #endregion Data acquition

        #region Datital I/O
        /// <summary>
        /// Set digital outputs for g.USBamp version 2.0.
        /// </summary>
        /// <param name="dev">handle of the device</param>
        /// <param name="uc_number">digital output id 1 or 2</param>
        /// <param name="uc_value">TRUE | FALSE</param>
        /// <returns>The function returns true if the call succeeded otherwise it will return false.
        /// The function returns false for g.USBamp version 3.0.</returns>
        [DllImport(DllPath)]
        private static extern int GT_SetDigitalOut(IntPtr dev, byte uc_number, byte uc_value);

        /// <summary>
        /// Set the digital outputs for g.USBamp version 3.0.
        /// </summary>
        /// <param name="dev">handle of the device</param>
        /// <param name="dout">digital output structure members
        /// BOOL SET_0; TRUE if digital OUT0 should be set to VALUE_0
        /// BOOL VALUE_0; TRUE – high; FALSE – low
        /// BOOL SET_1; TRUE if digital OUT1 should be set to VALUE_1
        /// BOOL VALUE_1; TRUE – high; FALSE – low
        /// BOOL SET_2; TRUE if digital OUT2 should be set to VALUE_2
        /// BOOL VALUE_2; TRUE – high; FALSE – low
        /// BOOL SET_3; TRUE if digital OUT3 should be set to VALUE_3
        /// BOOL VALUE_3; TRUE – high; FALSE – low</param>
        /// <returns>The function returns true if the call succeeded otherwise it will return false.
        /// The function returns false for g.USBamp version 2.0</returns>
        //[DllImport(LativeDll)]
        //private static extern int GT_SetDigitalOutEx(IntPtr dev, DigitalOUT dout)


        // for 3.0
        //[DllImport(LativeDll)]
        //private static extern int GT_GetDigitalIO(IntPtr DEV, ref DigitalIO DIO);

        // for 3.0
        //[DllImport(LativeDll)]
        //private static extern int GT_GetDigitalOut(IntPtr hDevice, ref DigitalOUT DOUT);

        /// <summary>
        /// Enable or disable the digital trigger line.
        /// The function returns true if the call succeeded otherwise it will return false.
        /// Note: If enabled, the trigger lines are sampled synchronous with the
        /// analog channels data rate. Therefore an additional float value is
        /// attached to the analog channels values. There is a difference 
        /// between g.USBamp version 3.0 and version 2.0. In version 2.0 there 
        /// is just one trigger line so the values of the trigger channel can 
        /// be 0 (LOW) and 250000 (HIGH). In version 3.0 there are 8 trigger 
        /// lines coded as UINT8 on the trigger channel. If all inputs are HIGH
        /// the value of the channel is 255.0. If e.g. input 0 to 3 are HIGH 
        /// the result is 15.0 ….
        /// </summary>
        /// <param name="dev">handle of the device</param>
        /// <param name="enable">TRUE - enable, FALSE - disable</param>
        /// <returns>BOOL Status=0 command not successful
        /// Status=1 command successful</returns>
        [DllImport(DllPath)]
        private static extern int GT_EnableTriggerLine(IntPtr dev, [MarshalAs(UnmanagedType.Bool)] bool enable);
        #endregion Digital I/O

        #region Filter
        /// <summary>
        /// Read in the available bandpass filter settings.
        /// Use GT_GetNumberOfFilter() to determine the size of the filter specifying data.
        /// Filter description data is copied to *FilterSpec.
        /// </summary>
        /// <param name="filter">filter structure FILT</param>
        /// <returns></returns>
        [DllImport(DllPath)]
        private static extern int GT_GetFilterSpec([Out] FILT[] filter);

        /// <summary>
        /// Read in the total number of available filter settings.
        /// The function returns true if the call succeeded otherwise it will return false.
        /// </summary>
        /// <param name="nof">number of filters</param>
        /// <returns></returns>
        [DllImport(DllPath)]
        private static extern int GT_GetNumberOfFilter(out int nof);

        /// <summary>
        /// Set the digital bandpass filter coefficients for a specific channel.
        /// Note: there is a hardware anti-aliasing filter
        /// </summary>
        /// <param name="dev">handle of the device</param>
        /// <param name="channel">channel number (can be 1 to 16)</param>
        /// <param name="index">filter id. Use GT_GetFilterSpec to get the filter matrix index set index to -1 if no filter should be applied</param>
        /// <returns></returns>
        [DllImport(DllPath)]
        private static extern int GT_SetBandPass(IntPtr dev, byte channel, int index);

        /// <summary>
        /// Read in the available notch filter settings. Use GT_GetNumberOfNotch() to determine the size of the filter specifying data.
        /// Filter description data is copied to *FilterSpec.
        /// </summary>
        /// <param name="FilterSpec">filter structure FILT array</param>
        /// <returns></returns>
        [DllImport(DllPath)]
        private static extern int GT_GetNotchSpec([Out] FILT[] FilterSpec);

        /// <summary>
        /// Read in the total number of available filter settings.
        /// </summary>
        /// <param name="nof">number of filters</param>
        /// <returns></returns>
        [DllImport(DllPath)]
        private static extern int GT_GetNumberOfNotch(out int nof);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dev">handle of the device</param>
        /// <param name="channel">channel number (can be 1 to 16)</param>
        /// <param name="index">filter id. Use GT_GetNotchSpec to get the filter matrix index.
        /// set index to -1 if no filter should be applied</param>
        /// <returns></returns>
        [DllImport(DllPath)]
        private static extern int GT_SetNotch(IntPtr dev, byte channel, int index);
        #endregion Filter

        #region Mode
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dev">handle of the device</param>
        /// <param name="mode">define the operating mode of the amplifier
        /// M_NORMAL: acquire data from the 16 input channels
        /// M_CALIBRATE calibrate the input channels. Applies a calibration signal onto all input channels
        /// M_IMPEDANCE measure the electrode impedance
        /// M_COUNTER if channel 16 is selected there is a counter on this channel (overrun at 1e6)</param>
        /// <returns></returns>
        [DllImport(DllPath)]
        private static extern int GT_SetMode(IntPtr dev, AmpMode mode);

        /// <summary>
        /// Get the operation mode of the device.
        /// </summary>
        /// <param name="dev">handle of the device</param>
        /// <param name="mode"></param>
        /// <returns>The function returns true if the call succeeded otherwise it will return false.</returns>
        [DllImport(DllPath)]
        private static extern int GT_GetMode(IntPtr dev, out AmpMode mode);

        /// <summary>
        /// Connect or disconnect the grounds of the 4 groups (A, B, C, D).
        /// </summary>
        /// <param name="dev">handle of the device</param>
        /// <param name="CommonGround">structure
        /// GND1 BOOL 1 – connect, 0 – disconnect group A to common ground
        /// GND2 BOOL 1 – connect, 0 – disconnect group B to common ground
        /// GND3 BOOL 1 – connect, 0 – disconnect group C to common ground
        /// GND4 BOOL 1 – connect, 0 – disconnect group D to common ground
        /// </param>
        /// <returns></returns>
        [DllImport(DllPath)]
        private static extern int GT_SetGround(IntPtr dev, GND CommonGround);

        /// <summary>
        /// Get the state of the grounds switches of the 4 groups (A, B, C, D).
        /// </summary>
        /// <param name="dev">handle of the device</param>
        /// <param name="CommonGround">structure
        /// GND1 BOOL 1 – connect, 0 – disconnect group A to common ground
        /// GND2 BOOL 1 – connect, 0 – disconnect group B to common ground
        /// GND3 BOOL 1 – connect, 0 – disconnect group C to common ground
        /// GND4 BOOL 1 – connect, 0 – disconnect group D to common ground
        /// </param>
        /// <returns></returns>
        [DllImport(DllPath)]
        private static extern int GT_GetGround(IntPtr dev, out GND CommonGround);

        /// <summary>
        /// Connect or disconnect the references of the 4 groups (A, B, C, D).
        /// </summary>
        /// <param name="dev">handle of the device</param>
        /// <param name="CommonReference">
        /// ref1 1 – connect, 0 – disconnect group A to common reference
        /// ref2 1 – connect, 0 – disconnect group B to common reference
        /// ref3 1 – connect, 0 – disconnect group C to common reference
        /// ref4 1 – connect, 0 – disconnect group D to common reference
        /// </param>
        /// <returns></returns>
        [DllImport(DllPath)]
        private static extern int GT_SetReference(IntPtr dev, REF CommonReference);

        /// <summary>
        /// Get the state of the reference switches of the 4 groups (A, B, C, D).
        /// </summary>
        /// <param name="dev">handle of the device</param>
        /// <param name="CommonReference">
        /// ref1 1 – connect, 0 – disconnect group A to common reference
        /// ref2 1 – connect, 0 – disconnect group B to common reference
        /// ref3 1 – connect, 0 – disconnect group C to common reference
        /// ref4 1 – connect, 0 – disconnect group D to common reference
        /// </param>
        /// <returns></returns>
        [DllImport(DllPath)]
        private static extern int GT_GetReference(IntPtr dev, out REF CommonReference);
        #endregion Mode

        #region Bipolar
        /// <summary>
        /// Define the channels for a bipolar derivation.
        /// </summary>
        /// <param name="dev">handle of the device</param>
        /// <param name="bipoChannel">
        /// Channel[n]:define the channel number for the bipolar derivation with
        ///            channel 1. If channel 2 is selected, g.USBamp performs a 
        ///            bipolar derivation between channel 1 and 2.
        ///            Set the channel number to zero if no bipolar derivation should be performed.
        /// <returns></returns>
        [DllImport(DllPath)]
        private static extern int GT_SetBipolar(IntPtr dev, CHDEF bipoChannel);
        #endregion Bipolar

        #region DRL
        /// <summary>
        /// Define the channels for the DRL calculation.
        /// </summary>
        /// <param name="dev">handle of the device</param>
        /// <param name="drlChannel">for invidual channel, set to 1 
        /// if the channel should be used for driven right leg calculation,
        /// otherwise to 0</param>
        /// <returns></returns>
        [DllImport(DllPath)]
        private static extern int GT_SetDRLChannel(IntPtr dev, CHDEF drlChannel);
        #endregion DRL

        #region Short Cut
        /// <summary>
        /// Enable or disable the short cut function.
        /// If short cut is enabled a high level on the SC input socket of the amplifier disconnects the electrodes from the amplifier input stage.
        /// </summary>
        /// <param name="dev">handle of the device</param>
        /// <param name="enable">TRUE – enable, FALSE – disable</param>
        /// <returns></returns>
        [DllImport(DllPath)]
        private static extern int GT_EnableSC(IntPtr dev, [MarshalAs(UnmanagedType.Bool)]bool enable);
        #endregion Short Cut

        #region Synchronization
        /// <summary>
        /// Set the amplifier to slave/master mode.
        /// Note: To synchronize multiple g.USBamps perform the following steps in your application
        /// 1. There must be only one device configured as master the others must be configured as slave devices.
        /// 2. The sampling rate has to be the same for all amplifiers.
        /// 3. Call GT_Start() for the slave devices first. The master device has to be called last.
        /// 4. During acquisition call GT_GetData() for all devices before invoking WaitForSingleObject()
        /// 5. To stop the acquisition call GT_Stop() for all slave devices first. The master device has to be called last.
        /// </summary>
        /// <param name="dev">handle of the device</param>
        /// <param name="slave">TRUE – slave mode, FALSE – master mode</param>
        /// <returns></returns>
        [DllImport(DllPath)]
        private static extern int GT_SetSlave(IntPtr dev, [MarshalAs(UnmanagedType.Bool)]bool slave);
        #endregion Synchronization

        #region Calibration / DRL
        /// <summary>
        /// Define the calibration/DRL settings.
        /// </summary>
        /// <param name="dev"></param>
        /// <param name="AnalogOut">
        /// WaveShape define the output waveshape
        ///     WS_SQUARE generate square wave signal
        ///     WS_SAWTOOTH generate sawtooth signal
        ///     WS_SINE generate sine wave
        ///     WS_DRL generate DRL signal
        ///     WS_NOISE generate white noise
        /// Amplitude max: 2000 (250mV), min: -2000 (-250mV)
        /// Frequency frequency in Hz
        /// Offset no offset: 2047, min: 0, max 4096</param>
        /// <returns></returns>
        [DllImport(DllPath)]
        private static extern int GT_SetDAC(IntPtr dev, DAC AnalogOut);

        /// <summary>
        /// Define the calibration/DRL settings.
        /// Note: Values are stored in permanent memory.
        /// Calculation: y = (x – d)*k;
        /// y … values retrieved with GT_GetData (calculated values) in uV
        /// x … acquired data d … offset value in uV 
        /// k … factor
        /// </summary>
        /// <param name="dev">handle of the device</param>
        /// <param name="Scaling">SCALE structure, which contains offset and scaling
        /// offset[i] contains the offset of channel i in uV
        /// factor[i] contains the factor for channel i
        /// </param>
        /// <returns></returns>
        [DllImport(DllPath)]
        private static extern int GT_SetScale(IntPtr dev, ref SCALE Scaling);

        /// <summary>
        /// Read factor and offset values for all channels.
        /// </summary>
        /// <param name="dev">handle of the device</param>
        /// <param name="Scaling">SCALE structure, which contains offset and scaling
        /// offset[i] contains the offset of channel i in uV
        /// factor[i] contains the factor for channel i
        /// </param>
        /// <returns></returns>
        [DllImport(DllPath)]
        private static extern int GT_GetScale(IntPtr dev, out SCALE Scaling);

        /// <summary>
        /// Calculate factor and offset values for all channels.
        /// Note: Function call is blocking (~4s). 
        /// Scaling and offset values in permanent memory reset to 1 and 0.
        /// Use GT_SetScale to write new values to storage.
        /// Use GT_GetScale to verify stored values.
        /// 
        /// Function call modifies g.USBamp configuration. You need to configure the device after this call to meet your requirements.
        /// </summary>
        /// <param name="dev">handle of the device</param>
        /// <param name="Scaling">SCALE structure, which contains offset and scaling
        /// offset[i] contains the offset of channel i in uV
        /// factor[i] contains the factor for channel i
        /// </param>
        /// <returns></returns>
        [DllImport(DllPath)]
        private static extern int GT_Calibrate(IntPtr dev, ref SCALE Scaling);
        #endregion Calibration / DRL

        #region Error handling
        /// <summary>
        /// Get the last error message.
        /// </summary>
        /// <param name="err_code">error id</param>
        /// <param name="lastError">error message (max. 1024 characters)</param>
        /// <returns></returns>
        [DllImport(DllPath)]
        private static extern int GT_GetLastError(out ushort err_code, out string lastError);
        #endregion Error handling

        #region General

        /// <summary>
        /// Return the g.USBamp driver version.
        /// </summary>
        /// <returns>g.USBamp driver version</returns>
        [DllImport(DllPath)]
        private static extern float GT_GetDriverVersion();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dev"></param>
        /// <returns>g.USBamp hardware version number (2.0 or 3.0)</returns>
        [DllImport(DllPath)]
        private static extern float GT_GetHWVersion(IntPtr dev);

        /// <summary>
        /// Get the serial number from an opened device.
        /// </summary>
        /// <param name="dev">handle of the device</param>
        /// <param name="buf">to receive the serial string</param>
        /// <param name="size">size of the array</param>
        /// <returns>Status=0 command not successful
        /// Status=1 command successful</returns>
        [DllImport(DllPath)]
        private static extern int GT_GetSerial(IntPtr dev, StringBuilder buf, int size);

        /// <summary>
        /// Measure the electrode impedance for specified channel.
        /// Note: All grounds are connected to common ground.
        /// Impedance is measured between ground and specified channel.
        /// Remark: Function call modifies g.USBamp configuration.
        /// You need to configure the device after this call to meet your requirements.
        /// </summary>
        /// <param name="dev">handle of the device</param>
        /// <param name="channel">channel number (1 … 20)
        /// hannel number 1 ..16 -> Electrode 1 ..16
        /// 17 = REFA 18 = REFB 19 = REFC 20 = REFD
        /// </param>
        /// <param name="impedance">value of electrode impedance on specified channel</param>
        /// <returns></returns>
        [DllImport(DllPath)]
        private static extern int GT_GetImpedance(IntPtr dev, byte channel, out double impedance);
        #endregion General
    }
}
