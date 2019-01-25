#pragma once
#include "FeedbackProcess.h"
#include "Transformation.h"

class CAttentionModel
{
public:
	CAttentionModel(void);
	~CAttentionModel(void);

	void Clear();

	bool LoadModelFile(LPCSTR fn_par);

	int m_nChannelUsed;
	int *m_pChannelIdx;
	char *m_strChannelUsed;
	int m_nSampleUsed;

	// model
	CFiltering m_fltDownSampling;
	CFiltering *m_pFilters;
	int m_nNumFilters;
	int m_nDownRatio;

	int m_nModel_m;
	CTransformat *m_pBandCSPTransfer;

	int m_nFeaSel;
	unsigned int m_nFeaFBMap;
	int *m_pFeaSel;

	// socre calculate
	double *m_pFLDMean;
	double *m_pFLDStd;
	double *m_pFLDLTV;
	double m_fFLDBias;
	double m_fFLDScoreMean[2];
	double m_fFLDScoreStd[2];

	double m_fThresholdScore;
	double m_fZeroScore;

	double m_fEEGMean;
	double m_fSelThreshold;

protected:
	double *m_pFLDBuffer;
};

class CAttentionDetection :	public CFeedbackProcess
{
public:
	CAttentionDetection(void);
	~CAttentionDetection(void);

	// new implementation
	virtual bool Initialize(LPCSTR mfn) override;
	virtual int ProcessEEGBuffer(float* eeg_buf, int nch, int nspl, RESULT* pResult) override;
	virtual void SetProcFeedbackHandle(void *ptr) override;

	static bool TrainAttentionModel(const char *fn_model, const char *fn_trcfg, const char *fn_cntlist);
	static const char *FiltFilt_FilterBankString[];
	static const char **GetFilterBankString(int sampling_rate = 250);

	static int FeaSel_NBPW(double *pd_fea, int *pi_cls, int ntrial, int ft_len, 
		int *pi_sf, int *pi_CO, double *pd_buf);
	static int ModTrain_FLDT(double *ps_fea, int *pi_cls, int ntrial, int m, 
		double *net_b, double *net_z, int *pi_CO, double *ps_score, double *pd_buf);
	static int ModTest_FLDT(double *ps_fea, int *pi_cls, int ntrial, int m, 
		double net_b, double *net_z, int *pi_CO, double *ps_score);

private:
	void CalculatePower(CFiltering *pflt, int rtype);
	void CalculateScore(RESULT *pResult);
	CAttentionModel m_Model;

	CFiltering *m_pAlphaFilter;
	CFiltering *m_pBetaFilter;

	double *m_pSelEEGBuffer;
	double *m_pSelEEGCopy;
	double **m_pSelEEGData;
	double *m_pTestFeaData;
	//int m_bReverseScore;

	// 20080919 - threshold for rejection
	double m_fSelThreshold;
	double m_fEEGMean;

	// feedback handler
	//rtype: 0=score, 1=alpha, 2 = beta
	// nval: number of values
	void (__stdcall *m_hOutputResult) (int rtype, int nval, double *pscore);

	// ccwang 20100330: actual used length
	int m_nCurUsed;
};
