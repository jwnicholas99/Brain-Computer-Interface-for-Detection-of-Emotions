#pragma	  once

const char CONFIG_FILE_NAME[] = "./Config/System.cfg";

#define UNKNOWN_TASK 0
#define WORDSPELLER 1
#define CARDGAME 2
#define MOTORIMAGERY 3
#define MIREHAB	4

#define kLEFTCHANNEL -1
#define kRIGHTCHANNEL 1

#define MINPROB 0.001
#define _MIN_FLOAT_		-1.0e-30
#define kMAXNUMPOINTS	65530

typedef struct _epoch_header{
	unsigned short epoch_id;	//epoch id with saved file
	unsigned short event_id;	//epoch stim code received
	int start_time;				//in ms
	int end_time;				//in ms
	unsigned int chan_num;		//number of channels
	unsigned int sample_rate;	//sampling rate in Hz
	unsigned int sample_size;	//width of each sample in bytes
	float resolution;			//actual votage = resolution * sample-value
	unsigned int samples;		//number of samples in this epoch
} epoch_header;

typedef struct _RESULT {
	int iResult; //int: result in integer
	float fResult; //float: result in float
	char szResult; //char: recognition result

	int iReject; //int: 0-accept, 1-reject
	float fConfidence; //float: confidence score
	int iXYZ[3];	//int[3]: results for co-ordinates
	float fDispSegments[kMAXNUMPOINTS]; // iNumSegments X iSegmentLength
	int	iNumSegments;
	int iSegmentLength;
} RESULT;