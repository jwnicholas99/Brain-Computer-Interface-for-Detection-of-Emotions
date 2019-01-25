#pragma once

class CLALib
{
public:
	CLALib(void);
	~CLALib(void);

	int GaussSolution(double a[], double b[], int n);
	static double FindMaxMinInArray(double* pArray,int iLen,bool bFindMax,int* pIdxMax=0);
	static float FindMaxMinInArray(float* pArray,int iLen,bool bFindMax,int* pIdxMax=0);
	static int BubbleSort(double* pArray,int iLen, bool bAscend);
	static double Sum(double* pArray,int iLen);
	static int EstGaussParas(double* pData,int iNumSample,double& fMean,double& fStd);
};
