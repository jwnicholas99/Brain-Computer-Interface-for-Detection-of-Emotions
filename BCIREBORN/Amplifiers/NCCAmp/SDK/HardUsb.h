//Header File: HardUsb.h
//2013-06-25
//For 12 EEG USB DEVICE
//Legolas_GXY
//Version:v1.0.0.0

#ifdef  HARDUSBDLL_API
#else
#define HARDUSBDLL_API extern "C" __declspec(dllimport)
#endif	
//�ɼ��豸��Ϣ�ṹ��
typedef struct GatherDeviceInfo_S 
{
	//�ɼ��豸���к�
	BYTE  BLicenceCode[3] ;
	//�ɼ��豸״̬:Ϊ0���ʾ�豸���ھ���״̬��������һ������ִ����ȷ�����Խ����µ����Ϊ1���ʾ�豸���ھ���״̬������һ������ִ�д��󣬿��Խ����µ�����
	//Ϊ2���ʾ�豸�Ѿ�׼���ô������ݣ��������Կ�ʼ���ػ��ϴ����ݣ�Ϊ3���ʾ�豸��æ�ڴ���������ܽ����µ�����
	BYTE  BDeviceStatus ;
	//�ɼ��豸֧�ֵ��������:0x00��ʾ8����0x40��ʾ8��+˯�ߣ�0x04��ʾ16����0x44��ʾ16��+��Σ�
	//0x08��ʾ32����0x48��ʾ32��+��Σ�0x09��ʾ64����0x0a��ʾ128��
	BYTE  BCfgTypeMax ;
	//�ɼ��豸��ǰ����Ƶ�ʣ�0��ʾ128Hz��1��ʾ256Hz��2��ʾ512Hz��3��ʾ1024Hz
	BYTE  BFreq ;
	//�ɼ��豸��ǰ����:��0x00��ʾ8����0x40��ʾ8��+˯�ߣ�0x04��ʾ16����0x44��ʾ16��+��Σ�
	//0x08��ʾ32����0x48��ʾ32��+��Σ�0x09��ʾ64����0x0a��ʾ128��
	BYTE  BCfgType;
	//SD��״̬:0x00��ʾ��SD����0x01��ʾ��SD��
	BYTE  BSDStatus;
}GDINFO_S,*PGDINFO_S;

//�򿪲ɼ��������豸,����ֵ������0���ʾ�����豸���򿪳ɹ�������-1���ʾ�����豸����ʧ�ܣ�
//����1���ʾ�ɼ��豸�򿪳ɹ��������豸��ʧ�ܣ�����2���ʾ�ɼ��豸��ʧ�ܣ������豸�򿪳ɹ���
HARDUSBDLL_API int __stdcall OpenDevice(); 

//�رղɼ��������豸
HARDUSBDLL_API void __stdcall  CloseDevice();

//��ʼ�ɼ����ݣ��ɹ�����TRUE��ʧ�ܷ���FALSE
HARDUSBDLL_API BOOL __stdcall StartGather();

//ֹͣ�ɼ����ݣ��ɹ�����TRUE��ʧ�ܷ���FALSE
HARDUSBDLL_API BOOL __stdcall StopGather();

//���òɼ��豸�ĵ������ã������ʣ�����ϵ��(����ϵ��ͨ��Ĭ��Ϊ1)
//����ChConfig=1��2��3��4��5��6��7��8ʱ�ֱ��ʾ��������Ϊ��8����8��+˯�ߡ�16����16��+˯�ߡ�32����32��+˯�ߡ�64����128��)
//���óɹ�����TRUE��ʧ�ܷ���FALSE
HARDUSBDLL_API BOOL __stdcall SetUpGatherDevice(int ChConfig, int iFreq, int Gain=1);

//���������豸������Ƶ��,����������Ƶ��Ϊ30Hz���򽫲���iFreq��Ϊ30����
//���óɹ�����TRUE��ʧ�ܷ���FALSE
HARDUSBDLL_API BOOL __stdcall SetUpFlashDevice(int iFreq);

//��ʼ���⣬�ɹ�����TRUE��ʧ�ܷ���FALSE
HARDUSBDLL_API BOOL __stdcall StartFlash();

//ֹͣ���⣬�ɹ�����TRUE��ʧ�ܷ���FALSE
HARDUSBDLL_API BOOL __stdcall StopFlash();

//��ȡ����,�ɹ��򷵻ض�ȡ���ֽ�����ʧ���򷵻�-1
HARDUSBDLL_API int __stdcall ReadData(WORD *pData, int nReadLen);

//��ȡ�ɼ��豸��Ϣ���ɹ�����TRUE��ʧ�ܷ���FALSE
HARDUSBDLL_API BOOL __stdcall GetGatherDeviceInfo(PGDINFO_S pGdInfo);