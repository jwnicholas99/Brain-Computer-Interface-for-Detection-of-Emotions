#ifndef _CSIGNALPROC_H
#define _CSIGNALPROC_H

#include "../L_Utilities/Utilities.h"
#include "../L_Utilities/NeuroComm_public.h"

#include "Filtering.h"

class CSignalProc {
public:
	virtual ~CSignalProc();

	int	GetChannelIndex(const char *szChannelName);
	static const char *SEP;

protected:
	CSignalProc();

	int	miTaskType; // manoj 18/March/2004

	int	miSamplingRate;
	float mfResolution;
	int	miNumTotalChannel;

	char *m_pChannelDefString;
	char **m_pChannelList;
	int	miNumChannelUsed;

	int	*mpChannelUsed;
	int	*mpChannelUsedIndex;

	int	miProcStartSample;
	int	miProcEndSample;
	int	miNumSampInRawEpochs;
	int	miNumSampleUsed;
	float **mpSegment;	//miNumChannelUsed X miNumSampleInOrigEpochs

	bool SetChannelList(const char *line);
	bool SetChannelUsed(const char *line);

	bool SetCommParameter(char* par, char* val, FILE * fpar);
	bool InitCommonBuffer(void);
	bool CopyRawBuffer(int* piBuff);
	bool SelectChannelSegment(void);


public:
	static bool TrainModel();
	virtual bool CalibrateModel(char* strCalibrationFile);
protected:
	bool SelectChannel(void);
	bool SelectSegment(void);

	// ccwang-detrend linear
	virtual void DetrendLinear(float **pInSegment);

public:
	// ccwang: common used signal processing funcitons
	static double GetVariance(double *buff, int len);
	static CFiltering *CrtFilterFromHexString(int order, const char *hex_B, const char *hex_A);

	static double Entropy(int *s, int sl);
	static double MutualInformation(int *C, int *F, int len);

	static double Parzen(double fval, double *ft_model, int fc_len, int ft_len, int fti, double H);//Parzen

	static void Standize(double *data, int sN, int sD, double *out_mean, double *out_std);

	static int SVD(double **a, int m, int n, double *w, double **v);
};

#endif