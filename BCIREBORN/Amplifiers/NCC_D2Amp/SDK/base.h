//Header File: base.h
//2013-07-30
//zmj
//For Sleeping Breath USB
//Version:v1.0.0.0

#ifndef _BASE_H_
#define _BASE_H_

#include <afxwin.h> 


#define IOCTL_EZUSB_EXPAND_RADPAM  0x60   //  读取参数命令

#define IOCTL_EZUSB_EXPAND_RADAMP  0x70   //  读取前置放大器信息命令

#define IOCTL_EZUSB_EXPAND_AMPINF  0x73   //  刷新前置放大器信息

#define IOCTL_EZUSB_EXPAND_BEGIN   0x80   //  开始采集
#define IOCTL_EZUSB_EXPAND_STOP    0x81   //  停止采集

#define IOCTL_EZUSB_EXPAND_FSHFRQ  0x83   //  设置闪光刺激起频率
#define IOCTL_EZUSB_EXPAND_FSHCTL  0x84   //  闪光刺激器控制
#define IOCTL_EZUSB_EXPAND_RESETBG 0x85   //  主机复位开始
#define IOCTL_EZUSB_EXPAND_RESETED 0x86   //  主机复位停止

#define IOCTL_EZUSB_EXPAND_STICTL  0x91   //  刺激器控制命令

#define IOCTL_EZUSB_EXPAND_SETFRQ  0x94   //  设置采样频率  
#define IOCTL_EZUSB_EXPAND_SETCHL  0x95   //  设置当前采样通道
#define IOCTL_EZUSB_EXPAND_SETMON  0x96   //  设置最大通道配置
#define IOCTL_EZUSB_EXPAND_SETIMP  0x97   //  设置PC机阻抗测试结果

#define IOCTL_EZUSB_EXPAND_AMPVSN  0x9a   //  设置前置放大器序列号

#define COMMANDLEN 64 
#define MAX_USB_DEV_NUMBER 64
#define CHANNEL_BYTE_NUM 16
#define IMPEDANCE_BYTE_NUM 16 

#define CTL_CODE( DeviceType, Function, Method, Access ) (                 \
    ((DeviceType) << 16) | ((Access) << 14) | ((Function) << 2) | (Method) \
)

typedef struct _DEVICE_STAT
{
	BYTE  SetState;       //命令执行结果,0: 命令执行正确1: 命令正在执行过程中2：命令执行错误3: 命令在传输过程中超时
	BYTE  MacState;       //主机状态0: 待机1: 正在采集2: 主机未接主电源
	BYTE  DCJState;       //主电刺激器状态0：未开机或没有检测到主电流刺激器1：待机2：正在刺激
	BYTE  SCJState;       //音频刺激器状态0：未开机或没有检测到音频刺激器1：待机2：短音刺激3：编程刺激4：Oddball模式刺激
	BYTE  SPCJState;      //视频刺激器状态0：未开机或没有检测到视频刺激器1：待机2：模式翻转刺激3：随机模式刺激
    BYTE  OTHERState;     //其他刺激器状态
    BYTE  MacMCUvsn1;     //主控盒MCU固件的主版本号
    BYTE  MacMCUvsn2;     //主控盒MCU本固件的次版本号
    BYTE  MacFPGAvsn1;    //主控盒FPGA主版本号
    BYTE  MacFPGAvsn2;    //主控盒FPGA次版本号
    BYTE  PreAMPState;    //与前置放大器的通讯状态：0x00：接收；0x01正常
    BYTE  FLASHState;     //闪光刺激器状态
	BYTE  Reserves[42];   //预留字节
	BYTE  AnswerCode;     //前置应答命令码
	BYTE  AnswerData[3];  //前置应答数据
	BYTE  MacSerialNum[4];//主机序列号，为无符号长整形数据格式，低位在后。
	BYTE  MacVersion[2];  //主机版本信息，有2位小数的压缩BCD码，低字节在后。例如：0x200表示主机版本为2.00。

} DEVICE_STAT, *PDEVICE_STAT ;  //sizeof(DEVICE_STAT) = 64

typedef struct _PREAMP_STAT
{
	BYTE  Freqency;      //前置放大器当前采样频率
	BYTE  FPGAvsn1;      //前置放大器FPGA主版本号
	BYTE  FPGAvsn2;      //前置放大器FPGA子版本号
	BYTE  CPUvsn1;       //前置放大器CPU主版本号
	BYTE  CPUvsn2;       //前置放大器CPU子版本号
	BYTE  Hardwarevsn1;  //前置放大器硬件主版本号
	BYTE  Hardwarevsn2;  //前置放大器硬件子版本号
    BYTE  SerialNum[6];  //前置放大器流水号，高字节在前，低字节在后
    BYTE  CellVotage[2]; //电池电压，单位mv；为无符号长整形数据格式，低位在后。
    BYTE  CellState;     //电池状态，bit0: =1正在充电，bit1: =1外接电源接入
    BYTE  MaxConfig;     //前置最大配置
    BYTE  ChoiceState[16];//前置通道选择状态
    BYTE  Reserves[31];  //预留字节	无意义
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


#define IOCTL_EZUSB_EXPAND_RADSENSOR  0x80   //  读取传感盒设备信息命令






typedef struct _PRESENSOR_STAT
{
	BYTE  FrameHead[2];      //帧头数据，固定为0x55aa
	BYTE  FrameLength;       //帧长度
	BYTE  ExecuteState;      //命令执行状态。0：命令执行正确，1：命令正在执行，2：命令执行错误，3：命令超时，4：空闲 。
	BYTE  DeviceType;        //设备类型:0x01,家用监测仪；0x02,初筛仪；0x03,多导睡眠监测系统(PSG)。
    BYTE  Hardwarevsn[4];    //硬件版本号
	BYTE  Firmwarevsn[4];    //固件版本号
	BYTE  CheckSum;          //校验和
    BYTE  Reserves[50];  //预留字节	无意义
} PRESENSOR_STAT,*PPRESENSOR_STAT; 



















#endif	