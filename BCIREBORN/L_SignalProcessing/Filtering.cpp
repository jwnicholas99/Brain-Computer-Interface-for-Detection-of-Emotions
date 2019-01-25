#include "Filtering.h"
#include "..\L_Utilities\ParaReader.h"

CFiltering::CFiltering()
{
	miOrderOfA = miOrderOfB = miOrderOfZi = 0;

	mpB = NULL;
	mpA = NULL;
	mpZ = NULL;
	mpZi = NULL;

	mpExtBuffer = NULL;
}

CFiltering::~CFiltering()
{
	if (mpB) delete [] mpB;
	if (mpA) delete [] mpA;
	if (mpZ) delete [] mpZ;
	if (mpZi) delete [] mpZi;

	if (mpExtBuffer) delete[] mpExtBuffer;
}

bool CFiltering::InitB(int iOrderOfB)
{
	if (mpB) {
		if (miOrderOfB != iOrderOfB) {
			delete[] mpB;
			mpB = NULL;
		}
	}

	if (!mpB) {
		miOrderOfB = iOrderOfB; 
		mpB = new double[miOrderOfB];
	}

	return true;
}


bool CFiltering::SetBPara(double fVal, int iOrder)
{
	mpB[iOrder] = fVal;
	return true;
}

bool CFiltering::InitA(int iOrderOfA)
{
	if (mpA) {
		if (miOrderOfA != iOrderOfA) {
			delete [] mpA;
			mpA = NULL;
		}
	}

	if (!mpA) {
		miOrderOfA = iOrderOfA; 
		mpA = new double[miOrderOfA];
	}

	return true;
}

bool CFiltering::SetAPara(double fVal, int iOrder)
{
	mpA[iOrder] = fVal;
	return true;
}

bool CFiltering::InitZi(int iOrderOfZi)
{
	if (mpZi) {
		if (miOrderOfZi != iOrderOfZi) {
			delete[] mpZi;
			mpZi = NULL;
		}
	}

	if (!mpZi) {
		miOrderOfZi = iOrderOfZi;
		mpZi = new double[miOrderOfZi];
	}
	return true;
}

bool CFiltering::SetZiPara(double fVal, int iOrder)
{
	mpZi[iOrder] = fVal;
	return true;
}

void CFiltering::SetAllPars(double* fA, int nA, double* fB, int nB)
{
	InitA(nA);
	for (int i = 0; i < nA; i++) mpA[i] = fA[i];

	InitB(nB);
	for (int i = 0; i < nB; i++) mpB[i] = fB[i];

	Check();
}


bool CFiltering::Check()
{
	if (miOrderOfA == 0 || miOrderOfB == 0) {
		printf("\nOrders of B and A are different, exit.\n");
		return false;
	}

	int nfilt = miOrderOfA >= miOrderOfB? miOrderOfA : miOrderOfB;
	
	if (miOrderOfA < nfilt) {
		double *ta = new double[nfilt];
		memcpy(ta, mpA, miOrderOfA * sizeof(double));
		memset(ta + miOrderOfA, 0, (nfilt - miOrderOfA) * sizeof(double));
		delete [] mpA;
		mpA = ta;
		miOrderOfA = nfilt;
	}

	if (miOrderOfB < nfilt) {
		double *tb = new double[nfilt];
		memcpy(tb, mpB, miOrderOfB * sizeof(double));
		memset(tb + miOrderOfB, 0, (nfilt - miOrderOfB) * sizeof(double));
		delete mpB;
		mpB = tb;
		miOrderOfB = nfilt;
	}

	if (mpA[0] != 1.0) { // normalize
		for (int i = 0; i < nfilt; i++) {
			mpA[i] /= mpA[0];
			mpB[i] /= mpA[0];
		}
	}

	if (mpZ) delete[] mpZ;
	mpZ = new double[nfilt - 1];
	memset(mpZ, 0, (nfilt - 1) * sizeof(double));

	return true;
}

//=================================
// IIR filtering
//=================================
//bool CFiltering::Process(float *fSignal, int iLength)
//{
//	if(miOrderOfA != miOrderOfB){
//		printf("\nOrders of B and A are different, exit.\n");
//		return false;
//	}
//
//    int iIIROrder = miOrderOfB;
//
//	// initial
//	memset(mfXBuf,0, iIIROrder*sizeof(double));
//	memset(mfYBuf,0, iIIROrder*sizeof(double));
//
//    // filtering
//	for(int k=0; k<iLength; k++) 
//	{
//		// input a sample
//		mfXBuf[0] = (double) fSignal[k];
//		mfYBuf[0] = 0.0;
//		for(int i=1; i<iIIROrder; i++)
//        {
//			mfYBuf[0] -= mpA[i]*mfYBuf[i];
//		}
//		for(i=0; i<iIIROrder; i++)
//        {
//			mfYBuf[0] += mpB[i]*mfXBuf[i];
//		}
//		
//		// keep old value
//		for(i=iIIROrder-1; i>0; i--)
//        {
//			mfYBuf[i] = mfYBuf[i-1];
//			mfXBuf[i] = mfXBuf[i-1];
//		}
//
//		// ouput a sample
//		fSignal[k] = (float) mfYBuf[0];
//	}
//
//	return true;
//}

// Rewriten by chuanchu to be compatible with matlab filter
bool CFiltering::Process(float *fSignal, int iLength, double *pZi, int lZi)
{
	if (!Check()) return false;

	int nfilt = miOrderOfA;

	// initialize mpZ
	if (pZi && lZi == nfilt - 1) {
		for (int i = 0; i < lZi; i++) mpZ[i] = pZi[i];
	} else {
		for (int i = 0; i < nfilt - 1; i++) mpZ[i] = 0;
	}

    // filtering
	for(int k = 0; k < iLength; k++) 
	{
		// input a sample
		double xi = fSignal[k];
		double yout = mpZ[0] + mpB[0] * xi;
		fSignal[k] = (float) yout;

		// update Z
		for(int i = 0; i < nfilt - 2; i++)
        {
			mpZ[i] = mpZ[i+1] + mpB[i + 1] * xi - mpA[i + 1] * yout;
		}
		mpZ[nfilt - 2] = mpB[nfilt - 1] * xi - mpA[nfilt - 1] * yout;		
	}

	return true;
}

bool CFiltering::Process(double *fSignal, int iLength, double *pZi, int lZi)
{
	if (!Check()) return false;

	int nfilt = miOrderOfA;

	// initialize mpZ
	if (pZi && lZi == nfilt - 1) {
		for (int i = 0; i < lZi; i++) mpZ[i] = pZi[i];
	} else {
		for (int i = 0; i < nfilt - 1; i++) mpZ[i] = 0;
	}

    // filtering
	for(int k = 0; k < iLength; k++) 
	{
		// input a sample
		double xi = fSignal[k];
		double yout = mpZ[0] + mpB[0] * xi;
		fSignal[k] = (float) yout;

		// update Z
		for(int i = 0; i < nfilt - 2; i++)
        {
			mpZ[i] = mpZ[i+1] + mpB[i + 1] * xi - mpA[i + 1] * yout;
		}
		mpZ[nfilt - 2] = mpB[nfilt - 1] * xi - mpA[nfilt - 1] * yout;		
	}

	return true;
}

// ccwang 20071016
double CFiltering::ProcessValue(double xi)
{
	double yout = mpZ[0] + mpB[0] * xi;

	//update z;
	int lz = miOrderOfA - 1;
	for (int i = 0; i < lz - 1; i++) {
		mpZ[i] = mpZ[i + 1] + mpB[i + 1] * xi - mpA[i + 1] * yout;
	}
	mpZ[lz - 1] = mpB[lz] * xi - mpA[lz] * yout;

	return yout;
}

bool CFiltering::FiltFilt(double *fdata, int ldata)
{
	if (!Check()) return false;

	int nfilt = miOrderOfA;
	int nfact = 3 * (nfilt - 1);

	if (ldata < nfact) {
		printf("FiltFilt: Data must have length more than 3 times filter order.\n");
		return false;
	}

	if (!mpExtBuffer) mpExtBuffer = new double[nfact + nfact];

	double *pred = mpExtBuffer;
	double *posd = pred + nfact;

    // y = [2*x(1)-x((nfact+1):-1:2);x;2*x(len)-x((len-1):-1:len-nfact)];
	for (int i = 0; i < nfact; i++) {
		pred[i] = 2 * fdata[0] - fdata[nfact - i];
		posd[i] = 2 * fdata[ldata - 1] - fdata[ldata - 2 - i];
	}

	//% filter, reverse data, filter again, and reverse data again
    //y = filter(b,a,y,[zi*y(1)]);

	// reset from mpZi
	if (mpZi) {
		for (int i = 0; i < nfilt - 1; i++) {
			mpZ[i] = mpZi[i] * pred[0];
		}
	} else {
		for (int i = 0; i < nfilt - 1; i++) {
			mpZ[i] = 0;
		}
	}

	for (int i = 0; i < nfact; i++) {
		pred[i] = ProcessValue(pred[i]);
	}

	for (int i = 0; i < ldata; i++) {
		fdata[i] = ProcessValue(fdata[i]);
	}
	for (int i = 0; i < nfact; i++) {
		posd[i] = ProcessValue(posd[i]);
	}

    //y = y(length(y):-1:1);
    //y = filter(b,a,y,[zi*y(1)]);
    //y = y(length(y):-1:1);

	// rest mpZi
	if (mpZi) {
		for (int i = 0; i < nfilt - 1; i++) {
			mpZ[i] = mpZi[i] * posd[nfact - 1];
		}
	} else {
		for (int i = 0; i < nfilt - 1; i++) {
			mpZ[i] = 0;
		}
	}

	for (int i = nfact - 1; i >= 0; i--) {
		posd[i] = ProcessValue(posd[i]);
	}
	for (int i = ldata - 1; i >= 0; i--) {
		fdata[i] = ProcessValue(fdata[i]);
	}
	//for (int i = nfact - 1; i >= 0; i--) {
	//	pred[i] = ProcessValue(pred[i]);
	//}

	return true;
}

bool CFiltering::InitBAHexFile(FILE *fp) {
	int k = 0;
	double fv;
	int norder;
	char* tok, *context;
	char line[1024];
	const char *SEP = " ,\t\n";
	const char *COLON = ":";

	// read B
	fgets(line, 1024, fp);
	tok = strtok_s(line, COLON, &context);
	tok = strtok_s(NULL, SEP, &context);
	norder = atoi(tok);
	InitB(norder);

	fgets(line, 1024, fp);
	k = 0;
	tok = strtok_s(line, SEP, &context);
	sscanf_s(tok, "%I64X", &fv);
	SetBPara(fv, k);
	k++;
	while (k < norder) {
		tok = strtok_s(NULL, SEP, &context);
		sscanf_s(tok, "%I64X", &fv);
		SetBPara(fv, k);
		k++;
	}

	// Read A
	fgets(line, 1024, fp);
	tok = strtok_s(line, COLON, &context);
	tok = strtok_s(NULL, SEP, &context);
	norder = atoi(tok);
	InitA(norder);

	fgets(line, 1024, fp);
	k = 0;
	tok = strtok_s(line, SEP, &context);
	sscanf_s(tok, "%I64X", &fv);
	SetAPara(fv, k);
	k++;
	while (k < norder) {
		tok = strtok_s(NULL, SEP, &context);
		sscanf_s(tok, "%I64X", &fv);
		SetAPara(fv, k);
		k++;
	}

	return true;
}

bool CFiltering::InitBAZiHexFile(FILE *fp) {
	InitBAHexFile(fp);

	int k = 0;
	double fv;
	int norder;
	char* tok, *context;
	char line[1024];
	const char *SEP = " ,\t\n";
	const char *COLON = ":";

	// read Zi
	fgets(line, 1024, fp);
	tok = strtok_s(line, COLON, &context);
	tok = strtok_s(NULL, SEP, &context);
	norder = atoi(tok);
	InitZi(norder);

	fgets(line, 1024, fp);
	k = 0;
	tok = strtok_s(line, SEP, &context);
	sscanf_s(tok, "%I64X", &fv);
	SetZiPara(fv, k);
	k++;
	while (k < norder) {
		tok = strtok_s(NULL, SEP, &context);
		sscanf_s(tok, "%I64X", &fv);
		SetZiPara(fv, k);
		k++;
	}

	return true;
}

bool CFiltering::InitHexString(const char *BString, const char *AString, const char *ZiString)
{
	double pf[100];
	char line[1024];
	const char *sep = " ,\t\n";

	strcpy_s(line, BString);
	int nfl = CParaReader::StrReadHexDouble(pf, 100, line, sep);
	InitB(nfl);
	for (int nfi = 0; nfi < nfl; nfi++)	SetBPara(pf[nfi], nfi);

	strcpy_s(line, AString);
	nfl = CParaReader::StrReadHexDouble(pf, 100, line, sep);
	InitA(nfl);
	for (int nfi = 0; nfi < nfl; nfi++)	SetAPara(pf[nfi], nfi);

	if (ZiString) {
		strcpy_s(line, ZiString);
		nfl = CParaReader::StrReadHexDouble(pf, 100, line, sep);
		InitZi(nfl);
		for (int nfi = 0; nfi < nfl; nfi++)	SetZiPara(pf[nfi], nfi);
	}

	return true;
}

