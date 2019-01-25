#ifndef _CSVMCLASSIFY_H
#define _CSVMCLASSIFY_H

#include <stdio.h>
#include <ctype.h>
#include <stdlib.h>
#include <string.h>
#include "CSvmMethods.h"

#define MAX_NUM_TEST 32768

class CSvmClassify{
public:
	CSvmClassify();
	~CSvmClassify();

	bool	Initialize(char *szModelFile, char *szRangeFile);
	void	Classify(double *fFeature, int iNumDim, double *fMargin, double *fPredict);
	void	ProcessFile(char *szTestFile, char *szModelFile, char *szOutputFile);

private:
	void	Reset();
	char*	ReadOneLine(FILE *input);
	bool	LoadModels(char *szModelFile);
	bool	LoadRange(char *szRangeFile);
	double	ScaleFeature(int index, double fInValue);
	void	ProcessFileData(FILE *input, FILE *output);
	void	ClassifyFile(FILE *input, FILE *output, double *Accuracy, double *MeanSqrErr, double *SqrCorrCoe, int *iNumTest);
	void	ClassifyFileFull(FILE *input, FILE *output, double *Accuracy, double *MeanSqrErr, double *SqrCorrCoe, int *iNumTest);
private:
	struct	CSvmModel*	mSVMmodels;
	struct	CSvmNode*	mSVMnodes;
	
	double	mfMargins[MAX_NUM_TEST];
	float   mfSVMProcTime;

	int		miMaxNoAttr;
	int		miMaxLine;

	char   *mszLine;

	double	mfLower;
	double	mfUpper;
	double	*mFeatureMax; 
	double	*mFeatureMin; 
};

#endif /* _CSVMCLASSIFY_H */
