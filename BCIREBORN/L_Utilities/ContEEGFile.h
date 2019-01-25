#pragma once

struct CntEEGHeader {
	int num_chan;
	int num_evt;
	int _not_used;
	int sample_rate;
	int data_size;
	float resolution;
};

struct StimInfo {
	int code;
	int no;
	int len;
};

struct StimType {
	int code;
	int num;
};

class CContEEGFile
{
public:
	~CContEEGFile(void);
	static CContEEGFile *ReadContEEGFile(const char *fn);

	CntEEGHeader m_hHeader;

	int m_nNumStim;
	StimInfo *m_pStimInfo;

	StimType *m_pStimType;
	int m_nNumType;

	int m_nNumSample;
	int *m_pEEGBuffer;

private:
	CContEEGFile(void);
	int Initialize(const char *fn);

};
