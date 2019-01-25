#ifndef _CFILTERING_H
#define _CFILTERING_H

#include <math.h>
#include "../L_Utilities/Utilities.h"

class CFiltering{
public:
	CFiltering();
	~CFiltering();

	bool	InitA(int iOrderOfA);
	bool	InitB(int iOrderOfB);
	bool	SetBPara(double fVal, int iOrder);
	bool	SetAPara(double fVal, int iOrder);

	bool	InitBAHexFile(FILE* fp);
	bool	InitBAZiHexFile(FILE* fp);

	// ccwang 20071016
	bool	InitZi(int iOrderOfZi);
	bool	SetZiPara(double fVal, int iOrder);

	bool	InitHexString(const char *BString, const char *AString, const char *ZiString);

	// rewritten by ccwang to be compabile with mablab filter
	//bool	Process(float *fSignal, int iLength);
	bool	Process(float *fSignal, int iLength, double *Zi = NULL, int lz = 0);
	bool	Process(double *fSignal, int iLength, double *Zi = NULL, int lz = 0);
	bool	FiltFilt(double *fdata, int ldata);

	void	SetAllPars(double* fA, int nA, double* fB, int nB);

public:

	int			miOrderOfB;
	int			miOrderOfA;
	double	   *mpB;
	double	   *mpA;
	double     *mpZ;
	int			miOrderOfZi;
    double     *mpZi; // for filtfilt

	bool		Check();
	double		ProcessValue(double xi);

private:
	double		*mpExtBuffer;
};

#endif /* _CFILTERING_H */