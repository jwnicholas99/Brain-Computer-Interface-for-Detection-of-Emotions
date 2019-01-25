#ifndef _CTRANSFORMATION_H
#define _CTRANSFORMATION_H

#include "../L_Utilities/Utilities.h"

#define SEPS " ,\t\n"

class CTransformat{
public:
	CTransformat();
	~CTransformat();

	bool	LoadMatrix(int iRow, int iCol, FILE *fp);
	bool	LoadMatrixHexFile(FILE *fp);
	void	ProcessVect(float *fVect, int iOffset);
	void	ProcessMat(float **fInMat, int iToltalInCol);
	void	ProcessMat(double **fInMat, int iToltalInCol);
	void	ProcessMat(double *buf, int nrow, double **fInMat, int iToltalInCol);

	// ccwang added on 20110304
	double	ProcessCol(int irow, double *col_In);

public:
	int		miNumRow;
	int		miNumCol;
	double	**mpTransMatrix;
	double	*mfBuff;
public:
	bool LoadMatrixFromMemory(int iRow, int iCol, float* pData);
	void SaveMatrix(FILE* fp, const char* pFormat);
};

#endif /* _CTRANSFORMATION_H */