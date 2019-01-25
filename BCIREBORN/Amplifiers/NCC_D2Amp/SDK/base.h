//Header File: base.h
//2013-07-30
//zmj
//For Sleeping Breath USB
//Version:v1.0.0.0

#ifndef _BASE_H_
#define _BASE_H_

#include <afxwin.h> 


#define IOCTL_EZUSB_EXPAND_RADPAM  0x60   //  ��ȡ��������

#define IOCTL_EZUSB_EXPAND_RADAMP  0x70   //  ��ȡǰ�÷Ŵ�����Ϣ����

#define IOCTL_EZUSB_EXPAND_AMPINF  0x73   //  ˢ��ǰ�÷Ŵ�����Ϣ

#define IOCTL_EZUSB_EXPAND_BEGIN   0x80   //  ��ʼ�ɼ�
#define IOCTL_EZUSB_EXPAND_STOP    0x81   //  ֹͣ�ɼ�

#define IOCTL_EZUSB_EXPAND_FSHFRQ  0x83   //  ��������̼���Ƶ��
#define IOCTL_EZUSB_EXPAND_FSHCTL  0x84   //  ����̼�������
#define IOCTL_EZUSB_EXPAND_RESETBG 0x85   //  ������λ��ʼ
#define IOCTL_EZUSB_EXPAND_RESETED 0x86   //  ������λֹͣ

#define IOCTL_EZUSB_EXPAND_STICTL  0x91   //  �̼�����������

#define IOCTL_EZUSB_EXPAND_SETFRQ  0x94   //  ���ò���Ƶ��  
#define IOCTL_EZUSB_EXPAND_SETCHL  0x95   //  ���õ�ǰ����ͨ��
#define IOCTL_EZUSB_EXPAND_SETMON  0x96   //  �������ͨ������
#define IOCTL_EZUSB_EXPAND_SETIMP  0x97   //  ����PC���迹���Խ��

#define IOCTL_EZUSB_EXPAND_AMPVSN  0x9a   //  ����ǰ�÷Ŵ������к�

#define COMMANDLEN 64 
#define MAX_USB_DEV_NUMBER 64
#define CHANNEL_BYTE_NUM 16
#define IMPEDANCE_BYTE_NUM 16 

#define CTL_CODE( DeviceType, Function, Method, Access ) (                 \
    ((DeviceType) << 16) | ((Access) << 14) | ((Function) << 2) | (Method) \
)

typedef struct _DEVICE_STAT
{
	BYTE  SetState;       //����ִ�н��,0: ����ִ����ȷ1: ��������ִ�й�����2������ִ�д���3: �����ڴ�������г�ʱ
	BYTE  MacState;       //����״̬0: ����1: ���ڲɼ�2: ����δ������Դ
	BYTE  DCJState;       //����̼���״̬0��δ������û�м�⵽�������̼���1������2�����ڴ̼�
	BYTE  SCJState;       //��Ƶ�̼���״̬0��δ������û�м�⵽��Ƶ�̼���1������2�������̼�3����̴̼�4��Oddballģʽ�̼�
	BYTE  SPCJState;      //��Ƶ�̼���״̬0��δ������û�м�⵽��Ƶ�̼���1������2��ģʽ��ת�̼�3�����ģʽ�̼�
    BYTE  OTHERState;     //�����̼���״̬
    BYTE  MacMCUvsn1;     //���غ�MCU�̼������汾��
    BYTE  MacMCUvsn2;     //���غ�MCU���̼��Ĵΰ汾��
    BYTE  MacFPGAvsn1;    //���غ�FPGA���汾��
    BYTE  MacFPGAvsn2;    //���غ�FPGA�ΰ汾��
    BYTE  PreAMPState;    //��ǰ�÷Ŵ�����ͨѶ״̬��0x00�����գ�0x01����
    BYTE  FLASHState;     //����̼���״̬
	BYTE  Reserves[42];   //Ԥ���ֽ�
	BYTE  AnswerCode;     //ǰ��Ӧ��������
	BYTE  AnswerData[3];  //ǰ��Ӧ������
	BYTE  MacSerialNum[4];//�������кţ�Ϊ�޷��ų��������ݸ�ʽ����λ�ں�
	BYTE  MacVersion[2];  //�����汾��Ϣ����2λС����ѹ��BCD�룬���ֽ��ں����磺0x200��ʾ�����汾Ϊ2.00��

} DEVICE_STAT, *PDEVICE_STAT ;  //sizeof(DEVICE_STAT) = 64

typedef struct _PREAMP_STAT
{
	BYTE  Freqency;      //ǰ�÷Ŵ�����ǰ����Ƶ��
	BYTE  FPGAvsn1;      //ǰ�÷Ŵ���FPGA���汾��
	BYTE  FPGAvsn2;      //ǰ�÷Ŵ���FPGA�Ӱ汾��
	BYTE  CPUvsn1;       //ǰ�÷Ŵ���CPU���汾��
	BYTE  CPUvsn2;       //ǰ�÷Ŵ���CPU�Ӱ汾��
	BYTE  Hardwarevsn1;  //ǰ�÷Ŵ���Ӳ�����汾��
	BYTE  Hardwarevsn2;  //ǰ�÷Ŵ���Ӳ���Ӱ汾��
    BYTE  SerialNum[6];  //ǰ�÷Ŵ�����ˮ�ţ����ֽ���ǰ�����ֽ��ں�
    BYTE  CellVotage[2]; //��ص�ѹ����λmv��Ϊ�޷��ų��������ݸ�ʽ����λ�ں�
    BYTE  CellState;     //���״̬��bit0: =1���ڳ�磬bit1: =1��ӵ�Դ����
    BYTE  MaxConfig;     //ǰ���������
    BYTE  ChoiceState[16];//ǰ��ͨ��ѡ��״̬
    BYTE  Reserves[31];  //Ԥ���ֽ�	������
} PREAMP_STAT,*PPREAMP_STAT;//sizeof(PREAMP_STAT) = 64

#define  INPIPE       0
#define  OUTPIPE      1

#define METHOD_BUFFERED                 0
#define METHOD_IN_DIRECT                1
#define METHOD_OUT_DIRECT               2
#define METHOD_NEITHER                  3

#define FILE_DEVICE_UNKNOWN             0x00000022
#define FILE_ANY_ACCESS                 0
#define FILE_READ_ACCESS          ( 0x0001 )    // file & pipe
#define FILE_WRITE_ACCESS         ( 0x0002 )    // file & pipe
enum DATA_TYPE{
	EEG_DATA,
	IMPEDANCE_TEST_THRESHOLD_5K,
	IMPEDANCE_TEST_THRESHOLD_10K,
	IMPEDANCE_TEST_THRESHOLD_15K,
	IMPEDANCE_TEST_THRESHOLD_20K,
	IMPEDANCE_TEST_THRESHOLD_40K,
	INTERNAL_CALIBRA_onetenthF_50uAPP_SINE,
	INTERNAL_CALIBRA_onetenthF_100uAPP_SINE,
	INTERNAL_CALIBRA_onetenthF_200uAPP_SINE,
	INTERNAL_CALIBRA_2F_50uAPP_SINE,
	INTERNAL_CALIBRA_2F_100uAPP_SINE,
	INTERNAL_CALIBRA_2F_200uAPP_SINE,
	INTERNAL_CALIBRA_5F_50uAPP_SINE,
	INTERNAL_CALIBRA_5F_100uAPP_SINE,
	INTERNAL_CALIBRA_5F_200uAPP_SINE,
	INTERNAL_CALIBRA_10F_50uAPP_SINE,
	INTERNAL_CALIBRA_10F_100uAPP_SINE,
	INTERNAL_CALIBRA_10F_200uAPP_SINE,
	INTERNAL_CALIBRA_20F_50uAPP_SINE,
	INTERNAL_CALIBRA_20F_100uAPP_SINE,
	INTERNAL_CALIBRA_20F_200uAPP_SINE,
	INTERNAL_CALIBRA_30F_50uAPP_SINE,
	INTERNAL_CALIBRA_30F_100uAPP_SINE,
	INTERNAL_CALIBRA_30F_200uAPP_SINE,
	INTERNAL_CALIBRA_50F_50uAPP_SINE,
	INTERNAL_CALIBRA_50F_100uAPP_SINE,
	INTERNAL_CALIBRA_50F_200uAPP_SINE,
	INTERNAL_CALIBRA_60F_50uAPP_SINE,
	INTERNAL_CALIBRA_60F_100uAPP_SINE,
	INTERNAL_CALIBRA_60F_200uAPP_SINE,
	INTERNAL_CALIBRA_onetenthF_50uAPP_SQUARE,
	INTERNAL_CALIBRA_onetenthF_100uAPP_SQUARE,
	INTERNAL_CALIBRA_onetenthF_200uAPP_SQUARE,
	INTERNAL_CALIBRA_2F_50uAPP_SQUARE,
	INTERNAL_CALIBRA_2F_100uAPP_SQUARE,
	INTERNAL_CALIBRA_2F_200uAPP_SQUARE,
	INTERNAL_CALIBRA_5F_50uAPP_SQUARE,
	INTERNAL_CALIBRA_5F_100uAPP_SQUARE,
	INTERNAL_CALIBRA_5F_200uAPP_SQUARE,
	INTERNAL_CALIBRA_10F_50uAPP_SQUARE,
	INTERNAL_CALIBRA_10F_100uAPP_SQUARE,
	INTERNAL_CALIBRA_10F_200uAPP_SQUARE,
	INTERNAL_CALIBRA_20F_50uAPP_SQUARE,
	INTERNAL_CALIBRA_20F_100uAPP_SQUARE,
	INTERNAL_CALIBRA_20F_200uAPP_SQUARE,
	INTERNAL_CALIBRA_30F_50uAPP_SQUARE,
	INTERNAL_CALIBRA_30F_100uAPP_SQUARE,
	INTERNAL_CALIBRA_30F_200uAPP_SQUARE,
	INTERNAL_CALIBRA_50F_50uAPP_SQUARE,
	INTERNAL_CALIBRA_50F_100uAPP_SQUARE,
	INTERNAL_CALIBRA_50F_200uAPP_SQUARE,
	INTERNAL_CALIBRA_60F_50uAPP_SQUARE,
	INTERNAL_CALIBRA_60F_100uAPP_SQUARE,
	INTERNAL_CALIBRA_60F_200uAPP_SQUARE
} ;


#define IOCTL_EZUSB_EXPAND_RADSENSOR  0x80   //  ��ȡ���к��豸��Ϣ����






typedef struct _PRESENSOR_STAT
{
	BYTE  FrameHead[2];      //֡ͷ���ݣ��̶�Ϊ0x55aa
	BYTE  FrameLength;       //֡����
	BYTE  ExecuteState;      //����ִ��״̬��0������ִ����ȷ��1����������ִ�У�2������ִ�д���3�����ʱ��4������ ��
	BYTE  DeviceType;        //�豸����:0x01,���ü���ǣ�0x02,��ɸ�ǣ�0x03,�ർ˯�߼��ϵͳ(PSG)��
    BYTE  Hardwarevsn[4];    //Ӳ���汾��
	BYTE  Firmwarevsn[4];    //�̼��汾��
	BYTE  CheckSum;          //У���
    BYTE  Reserves[50];  //Ԥ���ֽ�	������
} PRESENSOR_STAT,*PPRESENSOR_STAT; 



















#endif	