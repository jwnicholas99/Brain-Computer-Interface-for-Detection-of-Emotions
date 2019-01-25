//Header File: HardUsb.h
//2013-06-25
//For 12 EEG USB DEVICE
//Legolas_GXY
//Version:v1.0.0.0

#ifdef  HARDUSBDLL_API
#else
#define HARDUSBDLL_API extern "C" __declspec(dllimport)
#endif	
//采集设备信息结构体
typedef struct GatherDeviceInfo_S 
{
	//采集设备序列号
	BYTE  BLicenceCode[3] ;
	//采集设备状态:为0则表示设备处于就绪状态，并且上一条命令执行正确，可以接受新的命令；为1则表示设备处于就绪状态，但上一条命令执行错误，可以接受新的命令
	//为2则表示设备已经准备好传送数据，主机可以开始下载或上传数据；为3则表示设备正忙于处理命令，不能接受新的命令
	BYTE  BDeviceStatus ;
	//采集设备支持的最大配置:0x00表示8导；0x40表示8导+睡眠；0x04表示16导；0x44表示16导+多参；
	//0x08表示32导；0x48表示32导+多参；0x09表示64导；0x0a表示128导
	BYTE  BCfgTypeMax ;
	//采集设备当前采样频率：0表示128Hz，1表示256Hz，2表示512Hz，3表示1024Hz
	BYTE  BFreq ;
	//采集设备当前配置:：0x00表示8导；0x40表示8导+睡眠；0x04表示16导；0x44表示16导+多参；
	//0x08表示32导；0x48表示32导+多参；0x09表示64导；0x0a表示128导
	BYTE  BCfgType;
	//SD卡状态:0x00表示无SD卡；0x01表示有SD卡
	BYTE  BSDStatus;
}GDINFO_S,*PGDINFO_S;

//打开采集和闪光设备,返回值：返回0则表示两个设备均打开成功；返回-1则表示两个设备均打开失败；
//返回1则表示采集设备打开成功，闪光设备打开失败；返回2则表示采集设备打开失败，闪光设备打开成功；
HARDUSBDLL_API int __stdcall OpenDevice(); 

//关闭采集和闪光设备
HARDUSBDLL_API void __stdcall  CloseDevice();

//开始采集数据，成功返回TRUE，失败返回FALSE
HARDUSBDLL_API BOOL __stdcall StartGather();

//停止采集数据，成功返回TRUE，失败返回FALSE
HARDUSBDLL_API BOOL __stdcall StopGather();

//设置采集设备的导联配置，采样率，增益系数(增益系数通常默认为1)
//参数ChConfig=1、2、3、4、5、6、7、8时分别表示导联配置为：8导、8导+睡眠、16导、16导+睡眠、32导、32导+睡眠、64导、128导)
//设置成功返回TRUE，失败返回FALSE
HARDUSBDLL_API BOOL __stdcall SetUpGatherDevice(int ChConfig, int iFreq, int Gain=1);

//设置闪光设备的闪光频率,若设置闪光频率为30Hz，则将参数iFreq设为30即可
//设置成功返回TRUE，失败返回FALSE
HARDUSBDLL_API BOOL __stdcall SetUpFlashDevice(int iFreq);

//开始闪光，成功返回TRUE，失败返回FALSE
HARDUSBDLL_API BOOL __stdcall StartFlash();

//停止闪光，成功返回TRUE，失败返回FALSE
HARDUSBDLL_API BOOL __stdcall StopFlash();

//读取数据,成功则返回读取的字节数，失败则返回-1
HARDUSBDLL_API int __stdcall ReadData(WORD *pData, int nReadLen);

//读取采集设备信息，成功返回TRUE，失败返回FALSE
HARDUSBDLL_API BOOL __stdcall GetGatherDeviceInfo(PGDINFO_S pGdInfo);