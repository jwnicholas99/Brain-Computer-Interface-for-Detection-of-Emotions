using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace BCILib.Util {
    public class USBVibrator {

// Bit defination of the Options member of configOptions structure
        const int SPI_CONFIG_OPTION_MODE_MASK = 0x00000003;
        const int SPI_CONFIG_OPTION_MODE0 = 0x00000000;
        const int SPI_CONFIG_OPTION_MODE1 = 0x00000000;
        const int SPI_CONFIG_OPTION_MODE2 = 0x00000000;
        const int SPI_CONFIG_OPTION_MODE3 = 0x00000000;

        const int SPI_CONFIG_OPTION_CS_MASK = 0x0000001C;		// 111 00
        const int SPI_CONFIG_OPTION_CS_DBUS3 = 0x00000000;		// 000 00
        const int SPI_CONFIG_OPTION_CS_DBUS4 = 0x00000004;		// 001 00
        const int SPI_CONFIG_OPTION_CS_DBUS5 = 0x00000008;		// 010 00
        const int SPI_CONFIG_OPTION_CS_DBUS6 = 0x0000000C;		// 011 00
        const int SPI_CONFIG_OPTION_CS_DBUS7 = 0x00000010;		// 100 00


        const int SPI_CONFIG_OPTION_CS_ACTIVELOW = 0x00000020;



/* Bit defination of the transferOptions parameter in SPI_Read, SPI_Write & SPI_Transfer  */
/* transferOptions-Bit0: If this bit is 0 then it means that the transfer size provided is in bytes */
        const int SPI_TRANSFER_OPTIONS_SIZE_IN_BYTES = 0x00000000;
/* transferOptions-Bit0: If this bit is 1 then it means that the transfer size provided is in bytes */
        const int SPI_TRANSFER_OPTIONS_SIZE_IN_BITS = 0x00000001;
/* transferOptions-Bit1: if BIT1 is 1 then CHIP_SELECT line will be enables at start of transfer */
        const int SPI_TRANSFER_OPTIONS_CHIPSELECT_ENABLE = 0x00000002;
/* transferOptions-Bit2: if BIT2 is 1 then CHIP_SELECT line will be disabled at end of transfer */
        const int SPI_TRANSFER_OPTIONS_CHIPSELECT_DISABLE = 0x00000004;
        
        [DllImport("libMPSSE.dll")]
        static extern long SPI_GetNumChannels(out int numChannels);

        [DllImport("libMPSSE.dll")]
        static extern long SPI_OpenChannel(int idx, out IntPtr handle);

        [DllImport("libMPSSE.dll")]
        static extern long SPI_InitChannel(IntPtr handle, ref ChannelConfig config);

        [DllImport("libMPSSE.dll")]
        static extern long SPI_Write(IntPtr handle, byte[] buffer, int sizeToTransfer,
            out int sizeTransfered, int options);

        [DllImport("libMPSSE.dll")]
        static extern long SPI_ChangeCS(IntPtr handle, int configOptions);

        [DllImport("libMPSSE.dll")]
        static extern long SPI_CloseChannel(IntPtr handle);

        public USBVibrator()
        {
        }

        IntPtr ftHandle = IntPtr.Zero;
        ChannelConfig channelConf;
        private int NumChannel = 0;

        //byte MCP23S08_BYTE = 0x00; // Select channel
        //byte channel_output = 0x00;   // Configure potentiometer of selected channel
        int changeCS_rst = SPI_CONFIG_OPTION_MODE0 | SPI_CONFIG_OPTION_CS_DBUS6 | SPI_CONFIG_OPTION_CS_ACTIVELOW;
        int CS_default = SPI_CONFIG_OPTION_MODE0 | SPI_CONFIG_OPTION_CS_DBUS3 | SPI_CONFIG_OPTION_CS_ACTIVELOW;
        
        public bool Open()
        {
            long status = SPI_GetNumChannels(out NumChannel);
            if (NumChannel == 0) return false;

            status = SPI_OpenChannel(0, out ftHandle);
            if (ftHandle == IntPtr.Zero) return false;

            channelConf.ClockRate = 50000;
            channelConf.LatencyTimer = 1;
            channelConf.configOptions = CS_default;
            channelConf.Pin = 0x48FF48FF;	// FinalVal-FinalDir-InitVal-InitDir (for dir: 0=in, 1=out)		
                                            // (MSB) GPIO3,      GPIO2,     GPIO1, GPIO0,   CS,   DataIn,  DataOut, CLK (LSB)
                                            //    => ADP225_EN,  RESET,     A0,    A1,      CS',  SDI,     SD0,     SCK
            status = SPI_InitChannel(ftHandle, ref channelConf);
            if (status != 0) return false;

            Init_MCP23S08();

            return true;
        }

        public void Close() {
            if (ftHandle != IntPtr.Zero) {
                long status = SPI_CloseChannel(ftHandle);
                ftHandle = IntPtr.Zero;
            }
        }

        public void Start()
        {
            channelConf.Pin = 0x48FFC8FF; //GPIO4=HIGH
            long status = SPI_InitChannel(ftHandle, ref channelConf);
        }

        public void Stop()
        {
            channelConf.Pin = 0x48FF48FF; //GPIO4=LOW
            long status = SPI_InitChannel(ftHandle, ref channelConf);
        }

        // ccwang: 20130410: added property
        public bool Started
        {
            get
            {
                return ((channelConf.Pin & 0x8000) != 0);
            }
        }

        long MCP23S08_write_byte(byte register_add, byte byte_out)
        {
            long status;

            int tsz = 0;
            byte[] buffer = new byte[3]{0x40, register_add, byte_out};
            //buffer[0] = 0x40; // Opcode to select device 
            //buffer[1] = register_add; // Opcode for IODIR register
            //buffer[2] = byte_out; // Data Packet - Make GPIO pins outputs
            status = SPI_Write(ftHandle, buffer, 24, out tsz,
                SPI_TRANSFER_OPTIONS_SIZE_IN_BITS |
                SPI_TRANSFER_OPTIONS_CHIPSELECT_ENABLE |
                SPI_TRANSFER_OPTIONS_CHIPSELECT_DISABLE);

            return status;
        }

        long AD5160_write_byte(byte byte_write)
        {
            long status;

            int tsz = 0;
            byte[] bout = { byte_write };
            status = SPI_Write(ftHandle, bout, 8, out tsz,
                SPI_TRANSFER_OPTIONS_SIZE_IN_BITS |
                SPI_TRANSFER_OPTIONS_CHIPSELECT_ENABLE |
                SPI_TRANSFER_OPTIONS_CHIPSELECT_DISABLE);

            return status;
        }

        internal void Reset_MCP23S08()
        {
            SPI_ChangeCS(ftHandle, changeCS_rst);   // Reset IO expander.
            AD5160_write_byte(0xFF);                // write a byte to execute the RESET
            SPI_ChangeCS(ftHandle, CS_default);     // Change CS back to default
        }

        public void Channel_Select(int channelnum)
        {
            byte GPIO_byte;

            switch (channelnum)
            {
                case 0: GPIO_byte = 0xFE;
                    break;
                case 1: GPIO_byte = 0xFD;
                    break;
                case 2: GPIO_byte = 0xFB;
                    break;
                case 3: GPIO_byte = 0xF7;
                    break;
                case 4: GPIO_byte = 0xEF;
                    break;
                case 5: GPIO_byte = 0xDF;
                    break;
                case 6: GPIO_byte = 0xBF;
                    break;
                case 7: GPIO_byte = 0x7F;
                    break;
                case 10: GPIO_byte = 0xFC; // 1111 1100 - switch on #1 and #0 together
                    break;
                case 32: GPIO_byte = 0xF3; // 1111 0011 - switch on #3 and #2 together
                    break;
                case 210: GPIO_byte = 0xF8; // 1111 1000 - switch on #2, #1, #0 together
                    break;
                default: GPIO_byte = 0xFE;
                    break;
            }

            MCP23S08_write_byte(0x00, 0x00); // Set all MCP23S08 pins directions to 'Output'
            long status = MCP23S08_write_byte(0x09, GPIO_byte); // Configure the value in GPIO_byte into the GPIO register to select channel.

            
        }

        public void Config_Channel_Output(byte byte_in)
        {          
            AD5160_write_byte(byte_in);
            Reset_MCP23S08();
        }

        internal void Init_MCP23S08()
        {            
            Reset_MCP23S08();

            MCP23S08_write_byte(0x00, 0x00); // Configure IODIR as all output : Write 0x00 to IODIR address(0x00)
            MCP23S08_write_byte(0x05, 0x2E); // Set IOCON 
            MCP23S08_write_byte(0x09, 0xFF); // Initialize all channels
        }

        internal int GetNumChannels()
        {
            return NumChannel;
        }

        public bool IsPresent
        {
            get
            {
                if (ftHandle == IntPtr.Zero) return false;
                long status = SPI_GetNumChannels(out NumChannel);
                if (NumChannel > 0) return true;
                Close();
                return false;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    struct ChannelConfig {
        /// <summary>
        /// This parameter takes the value of the clock rate of the SPI bus in hertz. Valid range for ClockRate is 0 to 30MHz.
        /// </summary>
        public int ClockRate;

        /// <summary>
        /// Required value, in milliseconds, of latency timer. Valid range is 0 – 255. However, FTDI recommend the following ranges of values for the latency timer:
        /// Range for full speed devices (FT2232D): Range 2 – 255
        /// Range for Hi-speed devices (FT232H, FT2232H, FT4232H): Range 1 - 255
        /// </summary>
        public int LatencyTimer;

        /// <summary>
        /// This member provides a way to enable/disable features
        ///specific to the protocol that are implemented in the chip
        ///BIT1-0=CPOL-CPHA:	00 - MODE0 - data are captured on rising edge and propagated on falling edge
        ///                    01 - MODE1 - data are captured on falling edge and propagated on rising edge
        ///                    10 - MODE2 - data are captured on falling edge and propagated on rising edge
        ///                    11 - MODE3 - data are captured on rising edge and propagated on falling edge
        ///BIT4-BIT2: 000 - A/B/C/D_DBUS3=ChipSelect
        ///         : 001 - A/B/C/D_DBUS4=ChipSelect
        ///         : 010 - A/B/C/D_DBUS5=ChipSelect
        ///         : 011 - A/B/C/D_DBUS6=ChipSelect
        ///         : 100 - A/B/C/D_DBUS7=ChipSelect
        ///BIT5: ChipSelect is active high if this bit is 0
        ///BIT6 -BIT31		: Reserved
        /// </summary>
        public int configOptions;

        /// <summary>
        /// BIT7   -BIT0:   Initial direction of the pins
        /// BIT15 -BIT8:   Initial values of the pins
        /// BIT23 -BIT16: Final direction of the pins
        /// BIT31 -BIT24: Final values of the pins
        /// </summary>
        public int Pin;

        public int reserved;
    };
}
