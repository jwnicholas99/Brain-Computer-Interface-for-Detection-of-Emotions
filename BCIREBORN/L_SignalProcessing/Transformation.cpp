#include "Transformation.h"
#include ".\transformation.h"

CTransformat::CTransformat()
{
	mpTransMatrix = NULL;
	mfBuff = NULL;
}

CTransformat::~CTransformat()
{
	if(mpTransMatrix){
		for (int i=0;i<miNumRow;i++)
			delete [] mpTransMatrix[i];
		delete [] mpTransMatrix;
	}

	if (mfBuff) delete [] mfBuff;
}


bool CTransformat::LoadMatrix(int iRow, int iCol, FILE *fp)
{
	miNumRow = iRow;
	miNumCol = iCol;
	mpTransMatrix = new double * [miNumRow];
	for(int i=0;i<miNumRow;i++)
		mpTransMatrix[i] = new double [miNumCol];
	
	char szString[kMaxLine];
	for (int i=0;i<miNumRow;i++){
		if(fgets(szString,sizeof(szString) ,fp) != NULL){
			char *context = NULL;
			char *szToken = strtok_s(szString, SEPS, &context);
			for(int j=0;j<miNumCol;j++){
				mpTransMatrix[i][j] = (float) atof(szToken);
				szToken = strtok_s(NULL, SEPS, &context);
			}
		}
	}

	mfBuff = new double[MAX(miNumRow,miNumCol)];

	return true;
}


void  CTransformat::ProcessVect(float *fVect, int iOffset)
{
	for(int iRow=0;iRow<miNumRow;iRow++){
		double fVal = 0;
		for(int iCol=0;iCol<miNumCol;iCol++)
			fVal += mpTransMatrix[iRow][iCol] * fVect[iCol];
		mfBuff[iRow] = fVal;
	}

	for(int iRow=0;iRow<miNumRow;iRow++)
		fVect[iRow + iOffset]  = (float) mfBuff[iRow];
}

void  CTransformat::ProcessMat(float **fInMat, int iToltalInCol)
{
	for(int iInCol = 0;iInCol<iToltalInCol;iInCol++){
		for(int iRow=0;iRow<miNumRow;iRow++){
			double fVal = 0;
			for(int iCol=0;iCol<miNumCol;iCol++)
				fVal += mpTransMatrix[iRow][iCol] * fInMat[iCol][iInCol];
			mfBuff[iRow] = fVal;
		}

		for(int iRow=0;iRow<miNumRow;iRow++)
			fInMat[iRow][iInCol] = (float) mfBuff[iRow];
	}
}

void  CTransformat::ProcessMat(double **fInMat, int iToltalInCol)
{
	if (!mfBuff)	mfBuff = new double[MAX(miNumRow,miNumCol)];

	for(int iInCol = 0;iInCol<iToltalInCol;iInCol++){
		for(int iRow=0;iRow<miNumRow;iRow++){
			double fVal = 0;
			for(int iCol=0;iCol<miNumCol;iCol++)
				fVal += mpTransMatrix[iRow][iCol] * fInMat[iCol][iInCol];
			mfBuff[iRow] = fVal;
		}

		for(int iRow=0;iRow<miNumRow;iRow++)
			fInMat[iRow][iInCol]  = mfBuff[iRow];
	}
}

void CTransformat::ProcessMat(double *csp_buf, int iRow, double **fInMat, int iToltalInCol)
{
	for (int iInCol = 0; iInCol < iToltalInCol; iInCol++) {
		double fVal = 0;
		for (int iCol = 0; iCol < miNumCol; iCol++)
			fVal += mpTransMatrix[iRow][iCol] * fInMat[iCol][iInCol];
		csp_buf[iInCol] = fVal;
	}
}

double CTransformat::ProcessCol(int iRow, double *col_In)
{
	double fVal = 0;
	for (int iCol = 0; iCol < miNumCol; iCol++)
		fVal += mpTransMatrix[iRow][iCol] * col_In[iCol];
	return fVal;
}

bool CTransformat::LoadMatrixFromMemory(int iRow, int iCol, float* pData)
{
	miNumRow = iRow;
	miNumCol = iCol;
	mpTransMatrix = new double * [miNumRow];
	for(int i=0; i<miNumRow; i++)
		mpTransMatrix[i] = new double [miNumCol];

	for (int i=0; i<miNumRow; i++){
		for(int j=0;j<miNumCol;j++){
			mpTransMatrix[i][j] = pData[i*miNumCol+j];
		}
	}

	mfBuff = new double[MAX(miNumRow,miNumCol)];
	return true;
}

void CTransformat::SaveMatrix(FILE* fp,const char* pFormat)
{
	int i,j;
	for(i=0;i<miNumRow;i++){
		for(j=0;j<miNumCol;j++){
			fprintf(fp,pFormat,mpTransMatrix[i][j]);
			fprintf(fp,",");
		}
		fprintf(fp,"\n");
	}
}

bool CTransformat::LoadMatrixHexFile(FILE *fp) {
	int k;
	double fv;
	char* tok, *context;
	char line[1024];
	const char *SEP = " ,\t\n";
	const char *COLON = ":";

	fgets(line, 1024, fp);
	tok = strtok_s(line, COLON, &context);
	tok = strtok_s(NULL, SEP, &context);
	miNumRow = atoi(tok);
	tok = strtok_s(NULL, SEP, &context);
	miNumCol = atoi(tok);

	mpTransMatrix = new double * [miNumRow];
	for (int i = 0; i < miNumRow; i++)
		mpTransMatrix[i] = new double [miNumCol];

	for (int i = 0; i < miNumRow; i++) {
		fgets(line, 1024, fp);
		k = 0;
		tok = strtok_s(line, SEP, &context);
		sscanf_s(tok, "%I64X", &fv);
		mpTransMatrix[i][k] = fv;
		k++;
		while (k < miNumCol) {
			tok = strtok_s(NULL, SEP, &context);
			sscanf_s(tok, "%I64X", &fv);
			mpTransMatrix[i][k] = fv;
			k++;
		}
	}

	return true;
}
