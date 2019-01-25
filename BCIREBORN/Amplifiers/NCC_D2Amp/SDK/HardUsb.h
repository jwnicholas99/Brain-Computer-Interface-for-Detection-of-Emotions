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
        
HARDUSBDLL_API BOOL  ReSetStart();//������λ��ʼ
HARDUSBDLL_API BOOL  ReSetStop();//������λֹͣ
HARDUSBDLL_API BOOL  Stop(int iSize);//ֹͣ�ɼ�
HARDUSBDLL_API BOOL  Begin(DATA_TYPE dType);//��ʼ�ɼ�
HARDUSBDLL_API BOOL  SetFlash(int iFreq);//��������Ƶ��
HARDUSBDLL_API BOOL  FlashCtl(int iSize);//�������
HARDUSBDLL_API BOOL  SetFreq(int iFreq);//���ò���Ƶ��
HARDUSBDLL_API BOOL  SetChannel(int *pChannel);//���õ�ǰ����ͨ��// ��ڲ���128ά
HARDUSBDLL_API BOOL  SetMontage(int iSize);//�������ͨ������
HARDUSBDLL_API BOOL  SetImpTest(int *pChannel);//����PC���迹���Խ��// ��ڲ���128ά
HARDUSBDLL_API BOOL  SetAmpVersion(char *pAmpVsn);//����ǰ�÷Ŵ������к�//��ڲ���5ά
HARDUSBDLL_API BOOL  ReadDevStat(PDEVICE_STAT pDeviceStat);//��ȡ��������
HARDUSBDLL_API BOOL  RefreshAmpInfo();//ˢ��ǰ�÷Ŵ�����Ϣ
HARDUSBDLL_API BOOL  ReadAmpInfo(PPREAMP_STAT pPreAMPStat);//��ȡǰ�÷Ŵ�����Ϣ����
HARDUSBDLL_API BOOL  SetStim(int iSize,char *pSetData);//�̼�����������// ��ڲ���64ά �����������Э��
HARDUSBDLL_API UINT  ReadData(LPVOID pBuff, UINT nReadLen);//�����ݺ���
HARDUSBDLL_API BOOL  OpenUsb();//��usb��
HARDUSBDLL_API void  CloseUsb();//�ر�usb��