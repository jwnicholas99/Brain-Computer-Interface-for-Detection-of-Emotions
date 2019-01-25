using System;
using System.Collections.Generic;
using System.Text;

using System.Runtime.InteropServices;

namespace BCILib.Amp
{
    partial class BioRadio150
    {
        [StructLayout(LayoutKind.Sequential)]
        struct TDeviceInfo {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
 	        public string PortName; 
        }

        // definition of driver functions
        public const string DllPath = "BioRadio150DLL.dll";

        [DllImport(DllPath)]
        private static extern void GetDLLVersionString(StringBuilder VersionString, int  StrLength);
        //[DllImport("BioRadio150DLL.dll")]
        //private static extern bool FindDevicesSimple(StringBuilder[] DevicePortNameList, out int DeviceCount, int MaxDeviceCount, int PortNameStrLen);
        [DllImport(DllPath)]
        private static extern bool FindDevices([In, Out] TDeviceInfo[] DeviceList, out int DeviceCount, int MaxCount, Delegate UpdateStatusProc);

        [DllImport(DllPath)]
        private static extern uint CreateBioRadio();

        [DllImport(DllPath)]
        private static extern int GetBioRadioModelString(uint BioRadio150, StringBuilder BioRadioModelString, int StrLength);

        [DllImport(DllPath)]
        private static extern int GetFirmwareVersionString(uint BioRadio150, StringBuilder VersionString, int StrLength);

        [DllImport(DllPath)]
        private static extern int GetDeviceIDString(uint BioRadio150, StringBuilder DeviceIDCStr, int StrLength);
        [DllImport(DllPath)]
        private static extern int GetDeviceID(uint BioRadio150, out ushort DeviceID);

        [DllImport(DllPath)]
        private static extern int DestroyBioRadio(uint BioRadio150);
        [DllImport(DllPath)]
        private static extern int StartCommunication(uint BioRadio150, string PortName);
        [DllImport(DllPath)]
        private static extern int StartAcq(uint BioRadio150, byte displayProgressDialog);
        [DllImport(DllPath)]
        private static extern int DisableBuffering(uint BioRadio150);
        [DllImport(DllPath)]
        private static extern int EnableBuffering(uint BioRadio150);
        [DllImport(DllPath)]
        private static extern int StopAcq(uint BioRadio150);
        [DllImport(DllPath)]
        private static extern int StopCommunication(uint BioRadio150);
        //DllImport int __stdcall LoadConfig(DWORD BioRadio150, char *Filename);
        //DllImport int __stdcall PingConfig(DWORD BioRadio150, char displayProgressDialog);
        [DllImport(DllPath)]
        private static extern int ProgramConfig(uint BioRadio150, byte displayProgressDialog, string Filename);
        [DllImport(DllPath)]
        private static extern long GetSampleRate(uint BioRadio150);
        //DllImport long __stdcall GetBitResolution(DWORD BioRadio150);
        //DllImport int __stdcall GetFastSweepsPerPacket(DWORD BioRadio150);
        //DllImport int __stdcall GetSlowSweepsPerPacket(DWORD BioRadio150);
        //int __stdcall GetNumEnabledInputs(DWORD BioRadio150, int *FastMainInputs, int *FastAuxInputs, int *SlowAuxInputs);
        //DllImport int __stdcall GetNumEnabledSlowInputs(DWORD BioRadio150);
        [DllImport(DllPath)]
        private static extern int GetNumEnabledFastInputs(uint BioRadio150);
        //DllImport long __stdcall GetNumChannels(DWORD BioRadio150);
        //DllImport WORD __stdcall GetEnabledFastInputs(DWORD BioRadio150);
        //DllImport char __stdcall GetEnabledSlowInputs(DWORD BioRadio150);
        [DllImport(DllPath)]
        private static extern int TransferBuffer(uint BioRadio150);
        //DllImport int __stdcall ReadScaledData(DWORD BioRadio150, double *Data, int Size, int *NumRead);
        [DllImport(DllPath)]
        private static extern int ReadRawData(uint BioRadio150, [Out] ushort[] Data, int Size, out int NumRead);
        //DllImport int __stdcall ReadScaledFastAndSlowData(DWORD BioRadio150, double *FastInputsData, int FastInputsSize, int *FastInputsNumRead, WORD *SlowInputsData, int SlowInputsSize, int *SlowInputsNumRead);
        //DllImport int __stdcall ReadRawFastAndSlowData(DWORD BioRadio150, WORD *FastInputsData, int FastInputsSize, int *FastInputsNumRead, WORD *SlowInputsData, int SlowInputsSize, int *SlowInputsNumRead);
        [DllImport(DllPath)]
        private static extern int SetBadDataValues(uint BioRadio150, double BadDataValueScaled, ushort BadDataValueRaw);
        //DllImport int __stdcall SetPadWait(DWORD BioRadio150, int numMissingPackets);
        //DllImport int __stdcall GetRFChannel(DWORD BioRadio150);
        //DllImport int __stdcall SetRFChannel(DWORD BioRadio150, int RFChannel);
        //DllImport int __stdcall GetUsableRFChannelList(int UsableRFChannelList[], int Size);
        //DllImport int __stdcall GetFreqHoppingMode(DWORD BioRadio150);
        //DllImport int __stdcall GetFreqHoppingModeIndicator();
        //DllImport int __stdcall SetFreqHoppingMode(DWORD BioRadio150, bool HoppingEnabled);
        //DllImport DWORD __stdcall GetGoodPackets(DWORD BioRadio150);
        //DllImport DWORD __stdcall GetBadPackets(DWORD BioRadio150);
        //DllImport DWORD __stdcall GetDroppedPackets(DWORD BioRadio150);
        //DllImport int __stdcall GetUpRSSI(DWORD BioRadio150);
        //DllImport int __stdcall GetDownRSSI(DWORD BioRadio150);
        //DllImport int __stdcall GetLinkBufferSize(DWORD BioRadio150);
        //DllImport int __stdcall GetBitErrCount(DWORD BioRadio150);
        //DllImport int __stdcall GetBitErrRate(DWORD BioRadio150);
        //DllImport double __stdcall GetBatteryStatus(DWORD BioRadio150);
        //DllImport DWORD __stdcall GetBaseStationID(DWORD BioRadio150);
        //DllImport int __stdcall GetDeviceID(DWORD BioRadio150);
        //DllImport int __stdcall GetFirmwareVersionString(DWORD BioRadio150, char *VersionString, int StrLength);
        //DllImport void __stdcall GetDLLVersionString(char *VersionString, int StrLength);
    }
}
