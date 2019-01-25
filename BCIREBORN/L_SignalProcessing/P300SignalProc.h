#ifndef _CP300SIGNALPROC_H
#define _CP300SIGNALPROC_H

#include "../L_SVM/CSvmClassify.h"
#include "..\L_SVM\csvmmethods.h"

#include "Filtering.h"
#include "Transformation.h"
#include "FeedbackProcess.h"

#include "EEGContFile.h"

#define kMaxNumSamplePerEpoch  1024

class CP300SignalProc : public CFeedbackProcess {
public:
	static bool EEG_ChannelAverage(const char *eeg_path);

	bool m_bVariLenDetectMode;

	CP300SignalProc();
	~CP300SignalProc();

	double	ProcessEEGBuffer(char *fInEEGBuffer);
	void	GetResult(double *Score, unsigned short *cStimCode, 
					  int miNumStimPerRound, int iNumRound, RESULT *pResult);
	void	GetResult_OneVariLen(double *OrgScore, unsigned short *cOrgStimCode, int iQHead, int iQTail, int iQTotalLen, int iNumStimPerRound, RESULT *pResult);
	void	GetResult_MultiVariLen(double *OrgScore, unsigned short *cOrgStimCode, int iQHead, int iQTail, int iQTotalLen, int iNumStimPerRound, RESULT *pResult);

	double	ProcessFullCh(float **pInSegment); //2-D array: Channel X Samples
	float	GetRejectThreshold() { return mfRejectThreshold;};
	int		GetNumEpochPerRound() {return miNumEpochPerRound;};
	int		GetNumChannel() { return miNumChannelUsed;};
	double	GetRejectParaThr(int nRound){return mfEERRejectThrs[nRound];}

	int		IsRejection(double *fSVMScores ,unsigned short *cStimCode, int nStim,int nRound, RESULT *result);
	int		IsRejection_OneVariLen(double *fSVMScores ,unsigned short *cStimCode, int nStim,int nRound, RESULT *result);
	double	CalcPostProb(double *fSVMScores ,int nStim,int nRound);

	bool TrainModel(const char* strConfigFile);
	bool StoreProcParameters(char* strFileName);
	int ExtractFeatures(float** pInSegment);
	bool CalibrateRejectionModel(char* strEEGContFile);
	bool RetrainRejThr_For_DiffNumTarget(void);
	bool CalcThreshold_For_EER(double*& fThd_EER, double*& fTargetProbMeans, double*& fGarbageProbMeans, double* pScoreBuffer, double* pScoreBufferGarbage, int iNumRoundPerTarget, int iNumTarget, int iNumEpochPerTarget);
	bool CalcThreshold_For_DesiredFalseRejRate(double*& fThd_DesiredFalseRej, double*& fTargetProbMeans, double*& fGarbageProbMeans, double* pScoreBuffer, double* pScoreBufferGarbage, int iNumRoundPerTarget, int iNumTarget, int iNumEpochPerTarget, float fDesiredFalseRejRate);

	// ccwang: 20100708 abstract class from CFeedbackProcess
	virtual bool Initialize(LPCSTR mfn);
	virtual int ProcessEEGBuffer(float* eeg_buf, int nch, int nspl, RESULT* pResult);
	virtual void SetProcFeedbackHandle(void *ptr);

	virtual int GetNumSampleUsed() override
	{
		return miNumSampInRawEpochs;
	}

private:
	void	Reset();

	double	Process(float **pInSegment);	//2-D array: Channel X Samples
	bool	LoadParameters(char *szInFilename);
	bool	InitChnlSampIndex(FILE * fp);
	bool	InitFilterTransform(FILE * fp);
	bool	InitBuffers();
	bool	InitSVM();
	void	SelectChannels(float **pInSegment);
	void	CutSegment(float **pInSegment);
	void	RemoveEOG(float **pInSegment, int iRefChannel);
	int		CreateFeatures(float **pInSegment);
	int		GetChannel(char *szChannelName);
	void	GetResultWordSpeller(double *Score, unsigned short *cStimCode, 
					  int miNumStimPerRound, int iNumRound, RESULT *pResult);
	//void	GetResultCardGame(double *Score, unsigned short *cStimCode, 
	//				  int miNumStimPerRound, int iNumRound, RESULT *pResult);
	bool	InitRejection(FILE *fp);

	Resources		*mpResources;
	CFiltering		*mpCFiltering;
	CTransformat	*mpPCATransform;
	CTransformat	*mpDownSampTransform;
	CTransformat	*mpDeltaTransform;

	CSvmClassify	*mpCSVMClassify;

	int				miNumEpochPerRound;
	int				miNumRoundPerTarget;
	int				miNumColumn;

	int				miP300ProcStartTime;
	int				miP300ProcEndTime;

	float			mfRejectThreshold;
	int				miUseDelta; // manoj 8/3/2004

	///ZhangHaihong 12/01/2005
	float			mfFreqHighStop;
	int				miDownSampleRatio;
	int				miCharTableSize;

	int				miP300RecordStartTime;
	///
	///ZhangHaihong
	CSvmParameter	m_SVMParam;


	int			   *mpSampUsedIndex;

	int				miFilterOrderOfB;
	int				miFilterOrderOfA;
	int				miNumPCAMatrixRow;
	int				miNumPCAMatrixCol;
	int				miNumDownSampTransRow;
	int				miNumDownSampTransCol;
	int				miNumDeltaTransRow;
	int				miNumDeltaTransCol;
	char			**mszChannelDef;
	//-------------------
	// Rejection para
	//-------------------
	double	*mfEERRejectThrs;
	double	*mfDesiredFalseRejThrs;
	double	*mfTargetProbMeans;
	double	*mfGarbageProbMeans;
	int		miNumRejectThrs;
	double  m_fA;
	double  m_fB;
	double	m_fThd;
	double	m_fTargetGaussMean;
	double	m_fNonTargetGaussMean;
	double	m_fTargetGaussStd;	
	double	m_fNonTargetGaussStd;
	int		m_iRejThrBias;
	double	m_fRejThreshold_ManualSet;

	struct EpochScoreStatsTag{
		double fMean_Target;
		double fMean_NonTarget;
		double fMean_Idle;
		double fStd_Target;
		double fStd_NonTarget;
		double fStd_Idle;
	}m_EpochScoreStats;

	double m_fL_Control_VariLen;
	double m_fL_Idle_VariLen;
	double m_fEERDetectThr_VariLen;
	int m_iMinLen_VariLen;
	int m_iMaxLen_VariLen;

	//------------------
	// Processing buffers
	//------------------
	float			mfBuffer1[kMaxNumSamplePerEpoch];
	float			mfBuffer2[kMaxNumSamplePerEpoch];
	float			mfBuffer3[kMaxNumSamplePerEpoch];

	double			*mpFeatures;	//Channel X Samples
	int				mFeatureSize;
	double			*mScoreBuff;

	char	*mSpellerChars;
	bool SetupDSPParametersFromConfigFile(const char* strConfigFile);
	bool LoadSysConfigFile(const char* strConfigFile);
	bool CreateDeltaTformMatrix(float*& pData,int iRow,int iCol, int iWinWidth);
	int ExtractFeaturesFromEEGBuffer(int* pEEGData, float** pInSegment);
	bool TrainRejectionModel(char* strEvalFileList,CEEGContFile* pRefEEGContFile, char* strModelFile, char* strRangeFile, char* strModelScoreFile);
	bool PutScoreBufferInOrder(double*& pScoreBufferInOrder,double* pTargetSVMScores, double* pNonTargetSVMScores, int iNumTargetEpoch, int iNumNonTargetEpoch, int* pTargetEpochCode, int* pNonTargetEpochCode, int iNumStimPerRound);
	bool SimGarbageScoreBuffer(double*& pDestBuffer, double* pSrcBuffer, int iNumEpochPerRound, int iNumAllRound, int iNumRoundPerTarget);
	double FindEqualErrorRate(double* pPostProbs, double* pPostProbsGarbage, int iNumRejWin, int iNumRejWinGarbage, double* pThd_EER);

	void CalculateAccuracy(char* strEvalFileList, char* strModelFile, char* strRangeFile);
	void DisplayErrorRateOverThresholdBias(double* pPostProbs, double* pPostProbsGarbage, int iNumRejWin, int iNumRejWinGarbage, double fThd, double fMeanProb_Target, double fMeanProb_Garbage);
	double FindThd4GivenFalseRejRate(double* pPostProbs, double* pPostProbsGarbage, int iNumRejWin, int iNumRejWinGarbage, double* pThd, float fDesiredFalseRejRate);

	int CalcSVMScores_AllEpochs_in_EEGContFile(CEEGContFile* pEEGContFile_Eval,CSvmClassify* pSVM,double* &pTargetSVMScores_Eval,double* &pNonTargetSVMScores_Eval,int* &pTargetEpochCode, int* &pNonTargetEpochCode, int iNumEpochPerRound, int iNumRoundPerTarget, int& iNumTargetEpoch, int& iNumNonTargetEpoch);

	bool TrainVariLenDetectModel(char* strEvalFileList, char* strIdleFileList, CEEGContFile* pRefEEGContFile, char* strModelFile, char* strRangeFile, char* strModelScoreFile);

	int CalcL_Len(double* pScoreBuffer, int iNumEpochPerRound, int iNumRound, double* pL, int iWinWidth);

	int (__stdcall *m_hOutputResult) (double score);
};

#endif //_CP300SIGNALPROC_H