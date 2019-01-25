//Header File: HardUsb.h
//2010-07-13
//Kevin
//For 13#EEG USB
//Version:v1.0.0.0
#ifdef  HARDUSB_EXPORTS
#define HARDUSBDLL_API extern "C" __declspec(dllexport)
#else
#define HARDUSBDLL_API extern "C" __declspec(dllexport)
#endif	

#include "base.h"

HANDLE  hUsbDev ;              // handler to device
//char    strCommand[COMMANDLEN] ;	
HARDUSBDLL_API DWORD   dwBytesReturned ;     // pointer to variable to receive byte count


BOOL  DevIOCtlEx(HANDLE hDevice,                  // handle to device of interest
		         DWORD dwIoControlCode,           // control code of operation to perform
		         LPVOID lpInBuffer,               // pointer to buffer to supply input data
		         DWORD nInBufferSize,             // size, in bytes, of input buffer
		         LPVOID lpOutBuffer,              // pointer to buffer to receive output data
		         DWORD nOutBufferSize,            // size, in bytes, of output buffer
		         LPDWORD lpBytesReturned,         // pointer to variable to receive byte count
		         LPOVERLAPPED lpOverlapped );     // pointer to structure for asynchronous operation
        
HARDUSBDLL_API BOOL  ReSetStart();//主机复位开始
HARDUSBDLL_API BOOL  ReSetStop();//主机复位停止
HARDUSBDLL_API BOOL  Stop(int iSize);//停止采集
HARDUSBDLL_API BOOL  Begin(DATA_TYPE dType);//开始采集
HARDUSBDLL_API BOOL  SetFlash(int iFreq);//设置闪光频率
HARDUSBDLL_API BOOL  FlashCtl(int iSize);//闪光控制
HARDUSBDLL_API BOOL  SetFreq(int iFreq);//设置采样频率
HARDUSBDLL_API BOOL  SetChannel(int *pChannel);//设置当前采样通道// 入口参数128维
HARDUSBDLL_API BOOL  SetMontage(int iSize);//设置最大通道配置
HARDUSBDLL_API BOOL  SetImpTest(int *pChannel);//设置PC机阻抗测试结果// 入口参数128维
HARDUSBDLL_API BOOL  SetAmpVersion(char *pAmpVsn);//设置前置放大器序列号//入口参数5维
HARDUSBDLL_API BOOL  ReadDevStat(PDEVICE_STAT pDeviceStat);//读取参数命令
HARDUSBDLL_API BOOL  RefreshAmpInfo();//刷新前置放大器信息
HARDUSBDLL_API BOOL  ReadAmpInfo(PPREAMP_STAT pPreAMPStat);//读取前置放大器信息命令
HARDUSBDLL_API BOOL  SetStim(int iSize,char *pSetData);//刺激器控制命令// 入口参数64维 具体内容详见协议
HARDUSBDLL_API UINT  ReadData(LPVOID pBuff, UINT nReadLen);//读数据函数
HARDUSBDLL_API BOOL  OpenUsb();//打开usb口
HARDUSBDLL_API void  CloseUsb();//关闭usb口