//======================================================
// This is a wrapper for all kinds of signal processing
//======================================================
#include "SignalProc.h"
#include "P300SignalProc.h"
//#include "CardGame.h"
#include "MotorImagery.h"

CSignalProc::CSignalProc()
{
	miTaskType = UNKNOWN_TASK;
	mpChannelUsedIndex = NULL;

	m_pChannelDefString = NULL;
	m_pChannelList = NULL;

	mpChannelUsed = NULL;

	miProcStartSample = 0;
	miProcEndSample = 0;
	miNumSampInRawEpochs = 0;
	miNumSampleUsed = 0;
	miNumChannelUsed = 0;

	mpSegment = NULL;
}

CSignalProc::~CSignalProc()
{
	if (mpChannelUsedIndex)	delete [] mpChannelUsedIndex;
	if (m_pChannelDefString) free(m_pChannelDefString);
	if (m_pChannelList) delete[] m_pChannelList;
	if (mpChannelUsed) delete[] mpChannelUsed;

	if (mpSegment) {
		for (int i=0; i < miNumTotalChannel; i++)
			delete [] mpSegment[i];
		delete [] mpSegment;
	}
}

// static function, depreciated, kepted for compatible with old version
bool CSignalProc::TrainModel(void)
{
	Resources pResources;
	CP300SignalProc* pCP300SignalProc=NULL;
	CP300SignalProc* pCPCardGame=NULL;
	CMotorImagery* pCMotorImagery=NULL;

	if (pResources.Merge ((char *)CONFIG_FILE_NAME)){
		printf ("CSignalProc.TrainModel: Can not open configuration file %s.\n", CONFIG_FILE_NAME);
		return false;
	}

	char szTask[1024];
	if (pResources.Get ("System","Task", szTask, sizeof(szTask))){
		printf ("System:Task undefined in %s\n", (char *)CONFIG_FILE_NAME);
			return false;
	}

	if (strcmp (szTask, "WordSpeller") == 0) {
		pCP300SignalProc = new CP300SignalProc();
	}
	else if (strcmp (szTask, "MotorImagery") == 0) {
		pCMotorImagery = new CMotorImagery();
	}

	bool bRes=false;
	if (pCP300SignalProc){
		bRes=pCP300SignalProc->TrainModel(CONFIG_FILE_NAME);
	}

	if (pCP300SignalProc) delete pCP300SignalProc;
	if(pCPCardGame)	delete pCPCardGame;
	if(pCMotorImagery) delete pCMotorImagery;

	return bRes;
}

bool CSignalProc::CalibrateModel(char* strCalibrationFile)
{
	return false;
}

int CSignalProc::GetChannelIndex(const char *szChannelName)
{
	for (int i = 0; i < miNumTotalChannel; i++){
		if(_stricmp(m_pChannelList[i], szChannelName) == 0) return i;
	}

	return -1;
}

const char *CSignalProc::SEP = " ,\t\n";

bool CSignalProc::SetChannelList(const char *line)
{
	if (miNumTotalChannel > 0) {
		if (m_pChannelDefString) delete m_pChannelDefString;
		if (m_pChannelList) delete [] m_pChannelList;

		m_pChannelDefString = _strdup(line);
		m_pChannelList = new char*[miNumTotalChannel];

		char *context = NULL;
		char *pv = strtok_s(m_pChannelDefString, SEP, &context);
		for (int k = 0; k < miNumTotalChannel; k++) {
			m_pChannelList[k] = pv;
			pv = strtok_s(NULL, SEP, &context);
		}
	}

	return true;
}

bool CSignalProc::SetChannelUsed(const char *line0)
{
	char *line = NULL;
	if (miNumChannelUsed == 0) {
		line = _strdup(line0);
		char *context, *tok = strtok_s(line, SEP, &context);
		while (tok) {
			miNumChannelUsed++;
			tok = strtok_s(NULL, SEP, &context);
		}

		if (mpChannelUsed) delete[] mpChannelUsed;
		mpChannelUsed = NULL;
		delete line;
	}

	if (!mpChannelUsed) mpChannelUsed = new int[miNumChannelUsed];

	line = _strdup(line0);
	char *context, *pv = strtok_s(line, SEP, &context);
	for (int k = 0; k < miNumChannelUsed; k++) {
		mpChannelUsed[k] = -1;
		if (pv) {
			int chidx = GetChannelIndex(pv);
			if (chidx >= 0) mpChannelUsed[k] = chidx;
		}
		pv = strtok_s(NULL, SEP, &context);
	}
	delete line;

	// sort mpChannelUsed
	int from = 0, to = miNumChannelUsed - 1;
	while (from < to) {
		for (int i = from; i < to; i++) {
			if (mpChannelUsed[i] > mpChannelUsed[i+1]) {
				int tmp = mpChannelUsed[i];
				mpChannelUsed[i] = mpChannelUsed[i+1];
				mpChannelUsed[i + 1] = tmp;
			}
		}
		to--;

		for (int i = to; i > from; i--) {
			if (mpChannelUsed[i] < mpChannelUsed[i-1]) {
				int tmp = mpChannelUsed[i];
				mpChannelUsed[i] = mpChannelUsed[i-1];
				mpChannelUsed[i-1] = tmp;
			}
		}
		from++;
	}

	return true;
}

bool CSignalProc::SetCommParameter(char* par, char* val, FILE * fpar)
{
	if (strcmp(par, "Sampling_Rate") == 0) miSamplingRate = atoi(val);
	else if (strcmp(par, "EEG_Resolution") == 0) mfResolution =  (float) atof(val);
	else if (strcmp(par, "EEG_Resolution_Hex") == 0) {
		double fv = 0;
		sscanf_s(val, "%I64X", &fv);
		mfResolution = (float) fv;
	}
	else if (strcmp(par, "Channel_Order") == 0) {
		miNumTotalChannel = atoi(val);
		char line[1024];
		if (fgets(line, 1024, fpar))  SetChannelList(line);
	} else if (strcmp(par, "Channel_Used") == 0) {
		miNumChannelUsed = atoi(val);
		char line[1024];
		if (fgets(line, 1024, fpar)) SetChannelUsed(line);
	} else if (strcmp(par, "Processing_Start_Sample") == 0) miProcStartSample = atoi(val);
	else if (strcmp(par, "Processing_End_Sample") == 0) miProcEndSample = atoi(val);
	else if (strcmp(par, "Number_Samples_in_Raw_Epochs") == 0) miNumSampInRawEpochs = atoi(val);
	else {
		return false;
	}

	return true;
}

bool CSignalProc::InitCommonBuffer(void)
{
	miNumSampleUsed = miProcEndSample - miProcStartSample + 1;
	if (miNumSampleUsed <= 0) return false;
	if (miNumSampInRawEpochs <= 0) miNumSampInRawEpochs = miNumSampleUsed;

	// Init mpSecment
	if (miNumTotalChannel <= 0 || miNumSampInRawEpochs <= 0) {
		printf("InitCommonBuffer: parameter error! (total channel = %d, total sample = %d)\n",
			miNumTotalChannel, miNumSampInRawEpochs);
		return false;
	}

	mpSegment = new float*[miNumTotalChannel];
	for (int i = 0; i < miNumTotalChannel; i++) {
		mpSegment[i] = new float[miNumSampInRawEpochs];
	}

	return true;
}

//----------------------------------------------------
// Copy 1-dim eeg data to 2-d array, for all channels
//----------------------------------------------------
bool CSignalProc::CopyRawBuffer(int* piBuff)
{
	for (int iSamp=0; iSamp<miNumSampInRawEpochs; iSamp++) {
		for(int iCh = 0; iCh < miNumTotalChannel; iCh++) {
			if (mfResolution != 0) {
				mpSegment[iCh][iSamp] = piBuff[iCh] * mfResolution;
			} else {
				memcpy(mpSegment[iCh] + iSamp, piBuff + iCh, 4);
			}
		}
		piBuff += miNumTotalChannel;
	}

	return true;
}

// make sure mpChannelUsed is in increasted order
bool CSignalProc::SelectChannelSegment(void)
{
	if (miNumChannelUsed <= 0 || mpChannelUsed == NULL || 
		miProcStartSample < 0 || miProcEndSample < miProcStartSample) return false;

	for (int iCh = 0; iCh < miNumChannelUsed; iCh++) {
		int oCh = mpChannelUsed[iCh];
		if (oCh == iCh && miProcStartSample == 0) continue;

		for(int oSamp = miProcStartSample; oSamp <= miProcEndSample; oSamp++) {
			int iSamp = oSamp - miProcStartSample;
			mpSegment[iCh][iSamp] = mpSegment[oCh][oSamp];
		}
	}

	return true;
}

bool CSignalProc::SelectChannel(void)
{
	if (miNumChannelUsed <= 0 || mpChannelUsed == NULL) return false;

	for (int iCh = 0; iCh < miNumChannelUsed; iCh++) {
		int oCh = mpChannelUsed[iCh];
		if (oCh == iCh) continue;

		for(int iSamp = 0; iSamp < miNumSampInRawEpochs; iSamp++) {
			mpSegment[iCh][iSamp] = mpSegment[oCh][iSamp];
		}
	}

	return true;
}

bool CSignalProc::SelectSegment(void)
{
	if (miProcStartSample < 0 || miProcEndSample < miProcStartSample) return false;

	if (miProcStartSample == 0) return true;

	for (int iCh = 0; iCh < miNumChannelUsed; iCh++) {
		for(int oSamp = miProcStartSample; oSamp <= miProcEndSample; oSamp++) {
			int iSamp = oSamp - miProcStartSample;
			mpSegment[iCh][iSamp] = mpSegment[iCh][oSamp];
		}
	}

	return true;
}

//
// should be done after chanel-selection and before segmentation
//
void CSignalProc::DetrendLinear(float **pInSegment)
{
	int sum_t, sum_tsqr;
	sum_t = sum_tsqr = 0;

	for (int it = 0; it < miNumSampInRawEpochs; it++) {
		sum_t += it; // + 1?
		sum_tsqr += it * it;
	}

	for (int ich = 0; ich < miNumChannelUsed; ich++) {
		double sum_data = 0;
		double sum_td = 0;
		for (int it = 0; it < miNumSampInRawEpochs; it++) {
			sum_data += pInSegment[ich][it];
			sum_td += pInSegment[ich][it] * it; // it + 1?
		}

		double a = (sum_td / sum_t - sum_data / miNumSampInRawEpochs) /
			(sum_tsqr / sum_t - sum_t / miNumSampInRawEpochs);
		double b = (sum_data - sum_t * a) / miNumSampInRawEpochs;

		for (int it = 0; it < miNumSampInRawEpochs; it++) {
			pInSegment[ich][it] -= (float) (a * it + b);
		}
	}
}

double CSignalProc::GetVariance(double *nf_csp, int nspl) {
	double fmean = 0;
	for (int isample = 0; isample < nspl; isample++) fmean += nf_csp[isample];
	fmean /= nspl;

	double fvar = 0;
	for (int isample = 0; isample < nspl; isample++) {
		double dv = nf_csp[isample] - fmean;
		fvar += dv * dv;
	}
	if (nspl > 2) fvar /= (nspl - 1);

	return fvar;
}

CFiltering * CSignalProc::CrtFilterFromHexString(int order, const char *hex_B, const char *hex_A) {
	CFiltering *pflt = new CFiltering();
	
	int k = 0;
	double fv;
	char *context, *tok;
	char line[1024];
	//const char *SEP = " ,\t\n";

	// read B
	pflt->InitB(order);
	strcpy_s(line, hex_B);
	tok = strtok_s(line, SEP, &context);
	for (int k = 0; k < order; k++) {
		sscanf_s(tok, "%I64X", &fv);
		pflt->SetBPara(fv, k);
		tok = strtok_s(NULL, SEP, &context);
	}

	// Read A
	pflt->InitA(order);
	strcpy_s(line, hex_A);
	tok = strtok_s(line, SEP, &context);
	for (int k = 0; k < order; k++) {
		sscanf_s(tok, "%I64X", &fv);
		pflt->SetAPara(fv, k);
		tok = strtok_s(NULL, SEP, &context);
	}

	return pflt;
}

double
CSignalProc::Entropy(int *s, int sl)
{
//% ENTROPY Function to compute entropy
//%
//%     s = a vector of occurrences of elements
//%
//%   Example
//%     entropy([1 2])
//%   in which element 1 occurs 1 time and element 2 occurs 2 times gives the 
//%   same result as
//%     -1/3*log2(1/3)-2/3*log2(2/3)

	static double rlog2 = 1.0 / log(2.0);

	double ss = 0;
	for (int i = 0; i < sl; i++) {
		if (s[i] == 0) return 0;
		ss += s[i];
	}

	double H = 0;
	for (int i = 0; i < sl; i++) {
		double h1 = s[i] / ss;
        H = H - h1 * log(h1) * rlog2;
	}
	return H;
}

double
CSignalProc::MutualInformation(int *C, int *F, int n)
{
//% MI Function to compute Mutual Information
//%   by Ang Kai Keng (kkang@pmail.ntu.edu.sg)
//%   
//%   Syntax
//%     I=mi(C,F)
//%   where
//%     I : Information gain I(C;F)=H(C)-H(C|F)
//%     C : vector of class 0 or 1
//%     F : vector of feature that are classified as class 0 or 1
//%
//%   See also ENTROPY.

	// number of classes
	int ntC = 0;
	for (int i = 0; i < n; i++) {
		if (ntC < C[i]) ntC = C[i];
	}
	ntC++;

	// number of samples in C
	int* nc = new int[ntC];

	// number of samples
	for (int ic = 0; ic < ntC; ic++) {
		nc[ic] = 0;
	}
	for (int i = 0; i < n; i++) {
		nc[C[i]]++;
	}

	double HC = Entropy(nc, ntC);
	double HCF = 0;

	for (int ic = 0; ic < ntC; ic++) {
		int nfc = 0;
		int nfw = 0;
		for (int i = 0; i < n; i++) {
			if (F[i] == ic) {
				if (C[i] == ic) nfc++;
				else nfw++;
			}
		}
		int nfl[2] = {nfc, nfw};
		HCF += (nfc + nfw) * Entropy(nfl, 2) / n;
	}
	return HC - HCF;
}

double CSignalProc::Parzen(double fval, double *ft_model, int cls_len, int ft_len, int fti, double H)
{
	double RSQRT_2PI;
	sscanf_s("3FD9884533D43651", "%I64X", &RSQRT_2PI);

	double px = 0.0;
	double *pFeaSample = ft_model + fti;

	for (int ti = 0; ti < cls_len; ti++) {
		// kernel((x - t) / h)
		double dv = fval - *pFeaSample;
		dv /= H;
		dv = exp(-0.5 * dv * dv);
		px += dv;
		pFeaSample += ft_len;
	}

	px /= (cls_len * H);
	return px;
}

void CSignalProc::Standize(double *data, int sN, int sD, double *out_mean, double *out_std)
{
	for (int fti = 0; fti < sD; fti++) {
		// mean
		double dmean = 0;
		double *pd = data + fti;
		for (int itrial = 0; itrial < sN; itrial++) {
			dmean += *pd;
			pd += sD;
		}
		dmean /= sN;

		// std
		double dstd = 0;
		pd = data + fti;
		for (int itrial = 0; itrial < sN; itrial++) {
			double fv = *pd - dmean;
			fv *= fv;
			dstd += fv;
			pd += sD;
		}
		if (sN > 1) dstd /= (sN - 1);
		dstd = sqrt(dstd);

		out_mean[fti] = dmean;
		out_std[fti] = dstd;

		pd = data + fti;
		for (int itrial = 0; itrial < sN; itrial++) {
			*pd -= dmean;
			*pd /= dstd;
			pd += sD;
		}
	}
}
