#pragma once
#include "FeedbackProcess.h"
#include "Filtering.h"
#include "Transformation.h"

class CMultiTimeMIModel
{
public:
	CMultiTimeMIModel();
	~CMultiTimeMIModel();

	int m_nSamplingRate;
	int m_nSplStart;
	int m_nSplEnd;

	int m_nNumFilters;
	CFiltering *m_pFilters;

	bool LoadModel_Hex(LPCSTR fn);

	// model
	CTransformat *m_pBandCSPTransfer;
	int m_nModel_m;

	int m_nTimeSegment;
	int *m_pTimeSegStart;
	//int *m_pTimeSegLenth;
	//int *m_pFBTimeUsed;

	int m_nTrainClass0;
	int m_nTrainClass1;
	double *m_pTrainH;

	double *m_pTrainDataBuffer;

	int m_nFeaSel;
	unsigned char *m_pFeaIndex;

	unsigned int m_nBandMap;

	double Parzen(double fval, int ncol, int cls);
	double CalScore(double* fea_data);

	char *m_strAllChannel;
	int m_nAllChannel;
	char *m_strUsedChannel;
	int m_nUsedChannel;
	int *m_pUsedChannel;

	double m_fResolution;
};

class CMultiTimeMI :
	public CFeedbackProcess
{
public:
	CMultiTimeMI(void);
	~CMultiTimeMI(void);

	virtual void SetProcFeedbackHandle(void *ptr);
	virtual bool Initialize(LPCSTR mfn) override;
	virtual int ProcessEEGBuffer(float* eeg_buf, int nch, int nspl, RESULT* pResult) override;
	//virtual int ProcessEEGBuffer(char* eeg_buf, RESULT *pResult);

private:
	CMultiTimeMIModel m_model;

	double *m_pSelEEGBuffer;
	double *m_pSelEEGCopy;
	double **m_pSelEEGData;
	double *m_pTestFeaData;


	void (__stdcall *m_hOutputResult)(float *score, int size);
	float m_fScore[3];
};
