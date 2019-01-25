#ifndef _CMOTORIMAGERY_H
#define _CMOTORIMAGERY_H

#include "SignalProc.h"

#include "../L_SVM/CSvmClassify.h"
#include "Filtering.h"
#include "Transformation.h"
#include "FFT.h"
#include "LPC.h"

#define LINEAR_REGRESSOR	1
#define RBF_REGRESSOR		2

#define KBIGBUFFERSIZE		4096

typedef struct Regressor {
	int		iSelect;	//for LINEAR_REGRESSOR
	float	*fCenter;	//for RBF_REGRESSOR
	float	fSigma;		//for RBF_REGRESSOR
	int		iNumDim;	//for RBF_REGRESSOR
	int		iType;
} Regressor;

class CFeatureSelect{
public:
	CFeatureSelect(int iNumRegressor);
	~CFeatureSelect();
	void	SetRegressorType(int iRegressorIndex, int iType);
	void	SetRegressorDim(int iRegressorIndex, int iDim);
	void	SetRegressorCenter(int iRegressorIndex, int iDim, float fCenter);
	void	SetRegressorSigma(int iRegressorIndex, float fSigma);
	double	GetResponse(double *fInFeat, int iRegressorIndex);

private:
	int miInNumDim;
	int miOutNumDim;
	Regressor *mpSFeatureRegressor;

};

class CMotorImagery : public CSignalProc {
public:

	CMotorImagery();
	~CMotorImagery();
	bool	Initialize();
	double	ProcessEEGBuffer(char *fInEEGBuffer);
	void	GetResult(double Score, RESULT *pResult);

	double	ProcessFullCh(float **pInSegment); //2-D array: Channel X Samples
	float	GetRejectThreshold() { return mfRejectThreshold;};
	int		GetNumChannel() { return miNumChannelUsed;};

	void	SubtractReference(float *pfDataBuffer, int iDataLength,float *pfRefData);

private:
	void	Reset();

	double	Process(float **pInSegment);	//2-D array: Channel X Samples
	bool	LoadParameters(char *szInFilename);
	bool	InitChnlSampIndex(FILE * fp);
	bool	InitFilterTransform(FILE * fp);
	bool	InitChannelWeight(FILE *fp);
	bool	InitBuffers();
	bool	InitSVM();
	void	SelectChannels(float **pInSegment, int iNumSampleUsed);
	void	SelectSegment(float **pInSegment, int iNumChannel);
	void	RemoveEOG(float **pInSegment, int iRefChannel, int iNumSampleUsed);
	int		CreateCSPFeatures(float **pInSegment, int iWinSize, int iOffSet);
	int		GetChannel(char *szChannelName);
	bool	LoadFeatureSelect(FILE *fp);

private:
	Resources		*mpResources;
	CFiltering		*mpCFiltering;
	CTransformat	*mpPCATransform;
	CTransformat	*mpCSPTransform;
	CFeatureSelect	*mpCFeatureSelect;

	CSvmClassify	*mpCSvmClassify;
	LPCCEP			*m_pLPC;

	int				miNumColumn;

	float			mfRejectThreshold;

	int				miTaskType; // manoj 18/March/2004

	int			   *mpSampUsedIndex;

	int				miFilterOrderOfB;
	int				miFilterOrderOfA;
	int				miNumPCAMatrixRow;
	int				miNumPCAMatrixCol;
	int				miNumCSPTransRow;
	int				miNumCSPTransCol;
	char			**mszChannelDef;

	//------------------
	// Processing buffers
	//------------------
	float			mpfPowSpec[KBIGBUFFERSIZE];

	float			mpfBigBuffer[KBIGBUFFERSIZE];
	float			mpfBigBuffer2[KBIGBUFFERSIZE];

	double			*mpCSPFeatures;	//one dimension, miCSPDim components	
	double			*mpOLSFeatures;	//one dimension, miOLSDim components

	int		miWinSize;	//Window_Width_Sample in samples
	int		miWinShift;	//Window_Shift_Sample in samples	
	int		miTotalNumFrame;	//Total_Frame_Per_Epoch
	int		miCSPDim;	//CSP_Dimension "raw" feature from CSP feature extraction
	int		miUseOLS;	//Use_OLS_Selection: 1 use OLS, 0 not use
	int		miOLSDim;	//OLS_Feature_Dimension

	//For cursor control
	int		*miChannelAtr;
	float	*mfXMuWeight;
	float	*mfXBetaWeight;
	float	*mfYMuWeightRS;			//RS: right half screen
	float	*mfYBetaWeightRS;
	float	*mfYMuWeightLS;			//LS: left half screen
	float	*mfYBetaWeightLS;

	float	mfXIntercept;
	float	mfYInterceptRS;
	float	mfYInterceptLS;
	float	mfXMovGain;
	float	mfYMovGain;
	float	mfXAxis;	//incremental x
	float	mfYAxisRS;	//incremental y
	float	mfYAxisLS;	//incremental y
	float	mfCalibPowNorm;
	float	mfCalibLPCNorm;


};

#endif //_CMOTORIMAGERY_H