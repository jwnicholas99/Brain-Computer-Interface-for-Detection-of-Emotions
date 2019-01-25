#include "RehabMotorImage.h"
#include "..\L_Utilities\ParaReader.h"

#include <fstream>
using namespace std;

CRehabMotorImage::CRehabMotorImage(void)
: m_pFilter(NULL)
{
	miTaskType = MIREHAB;

	m_pFilter = new CFiltering();
	m_pCSPTransform = NULL;
	m_pHamWin = NULL;
	m_pResLinear = NULL;
}

CRehabMotorImage::~CRehabMotorImage(void)
{
	if (m_pFilter) delete m_pFilter;
	if (m_pCSPTransform) delete m_pCSPTransform;
	if (m_pHamWin) delete [] m_pHamWin;
	if (m_pResLinear) delete [] m_pResLinear;
}

// Initialize model and parameters
bool CRehabMotorImage::Initialize(void)
{
	Resources res;
	if (res.Merge ((char *)CONFIG_FILE_NAME) != errOkay){
		printf ("CMotorImagery.Initialize: Can not open configuration file %s.\n", CONFIG_FILE_NAME);
		return false;
	}

	char szFilename[kMaxLine];
	if(res.Get("Process", "ParameterFile",szFilename, sizeof(szFilename)) != errOkay){
		printf ("Parameter file must be given in configuration file %s under Process:ParameterFile.\n",(char *)CONFIG_FILE_NAME);
		return false;
	}

	if (!LoadParameters(szFilename)) return false;
	if (!InitCommonBuffer()) return false;

	// Hamming window
	m_pHamWin = new double[miNumSampleUsed];
	if (miNumSampleUsed == 1) m_pHamWin[0] = 1;
	else {
		double a = 2 * 3.14159265 / (miNumSampleUsed - 1);
		for (int i = 0; i < miNumSampleUsed; i++)
		{
			m_pHamWin[i] = 0.54 - 0.46 * cos(a*i);
		}
	}

	return true;
}

// Load parameters from parameter file
bool CRehabMotorImage::LoadParameters(LPCSTR fn_par)
{
	FILE *fp;
	if (fopen_s(&fp, fn_par, "rt")) {
		printf("Cannot load parameter file %s!", fn_par);
		return false;
	}

	CParaReader cpar;
	cpar.SetSeperator(':');
	char line[1024];
	// format:
	// par: val
	while (fgets(line, 1024, fp)) {
		char *par, *val, *context;
		if (!cpar.GetParValue(line, par, val)) continue;
		printf("Read '%s' = '%s'\n", par, val);

		if (SetCommParameter(par, val, fp)) continue;

		if (strcmp(par, "Filter_Parameter_A") == 0) {
			int n = atoi(val);
			m_pFilter->InitA(n);
			if (!fgets(line, 1024, fp)) break;
			val = strtok_s(line, SEP, &context);
			int k = 0;
			m_pFilter->SetAPara(val?atof(val) : 0, k++);
			while (k < n) {
				val = strtok_s(NULL, SEP, &context);
				m_pFilter->SetAPara(val?atof(val) : 0, k++);
			}
		} else if (strcmp(par, "Filter_Parameter_B") == 0) {
			int n = atoi(val);
			m_pFilter->InitB(n);
			if (!fgets(line, 1024, fp)) break;
			val = strtok_s(line, SEP, &context);
			int k = 0;
			m_pFilter->SetBPara(val?atof(val) : 0, k++);
			while (k < n) {
				val = strtok_s(NULL, SEP, &context);
				m_pFilter->SetBPara(val?atof(val) : 0, k++);
			}
		} else if (strcmp(par, "CSP_Transform_Matrix") == 0) {
			int row, col;
			sscanf_s(val, "%d, %d", &row, &col);

			m_pCSPTransform = new CTransformat();
			m_pCSPTransform->LoadMatrix(row, col, fp);
		} else if (strcmp(par, "Res_Linear") == 0) {
			int n = atoi(val);
			m_pResLinear = new double[n];
			int k = 0;

			if (!fgets(line, 1024, fp)) break;
			char *pv = strtok_s(line, SEP, &context);
			m_pResLinear[k++] = pv?atof(pv):0;
			while (k < n) {
				pv = strtok_s(NULL, SEP, &context);
				m_pResLinear[k++] = pv?atof(pv):0;
			}
		}
	}

	fclose(fp);

	return true;
}

// return 1 for event, 0 for non-event
int CRehabMotorImage::ProcessEEGBuffer(char* eeg_buf, RESULT* pResult)
{
	memset(pResult, 0, sizeof(RESULT));
	pResult->fConfidence = 0;
	pResult->iReject = 1;
	pResult->szResult = 0;
	pResult->fResult = 0;

	// copy buffer
	CopyRawBuffer((int*)eeg_buf);

	// Select channels
	SelectChannel();

	// Filtering
	for (int iChan = 0; iChan < miNumChannelUsed; iChan++) {
		m_pFilter->Process(mpSegment[iChan], miNumSampleUsed);
	}

	// CSP
	m_pCSPTransform->ProcessMat(mpSegment, miNumSampleUsed);
	int numChan = m_pCSPTransform->miNumRow;

	// Hamming
	for (int iChan = 0; iChan < numChan; iChan++) {
		for (int iSamp = 0; iSamp < miNumSampleUsed; iSamp++)
			mpSegment[iChan][iSamp] = (float) (mpSegment[iChan][iSamp] * m_pHamWin[iSamp]);
	}

	// mean and variance for each channel
	double *fmean = new double[numChan];
	double *fvar = new double[numChan];

	for (int iChan = 0; iChan < numChan; iChan++) {
		double tmp = 0;
		for (int iSamp = 0; iSamp < miNumSampleUsed; iSamp++) {
			tmp += mpSegment[iChan][iSamp];
		}
		fmean[iChan] = tmp / miNumSampleUsed;
	}

	for (int iChan = 0; iChan < numChan; iChan++) {
		double tmp = 0;
		for (int iSamp = 0; iSamp < miNumSampleUsed; iSamp++) {
			double tmp1 = mpSegment[iChan][iSamp] - fmean[iChan];
			tmp += tmp1 * tmp1;
		}
		fvar[iChan] = tmp / (miNumSampleUsed - 0);
	}

	if (m_pResLinear) {
		double tmp = 0;
		for (int i = 0; i < numChan; i++) tmp += fvar[i] * m_pResLinear[i];

		double res_constant = -9.266127218976645e-001;
		double thres = 0;

		pResult->fResult = (float) tmp;
		tmp -= res_constant;
		pResult->fConfidence = (float) tmp;
		pResult->iReject = (tmp < thres);
		pResult->szResult = '0' + (tmp >= thres);
	}

	delete []fmean;
	delete []fvar;

	return pResult->szResult;
}

// test a cnt file
void CRehabMotorImage::TestEEGAll(char* eeg_buf, int nSample)
{
	double segsum[40];

	// reallocate databuf
	if (mpSegment != 0) {
		for (int i = 0; i < miNumTotalChannel; i++) {
			delete [] mpSegment[i];
		}
		delete [] mpSegment;
	}

	miNumSampInRawEpochs = nSample;
	InitCommonBuffer();

	// copy buffer
	CopyRawBuffer((int*)eeg_buf);

	// Select channels
	SelectChannel();

	// for compare
	for (int i = 0; i < 40; i++) segsum[i] = 0;
	for (int i = 0; i < miNumChannelUsed; i++) {
		for (int j = 5000; j < 6000; j++) {
			segsum[i] += mpSegment[i][j];
		}
	}

	// Filtering
	for (int iChan = 0; iChan < miNumChannelUsed; iChan++) {
		m_pFilter->Process(mpSegment[iChan], miNumSampInRawEpochs);
	}

	// for compare
	for (int i = 0; i < 40; i++) segsum[i] = 0;
	for (int i = 0; i < miNumChannelUsed; i++) {
		for (int j = 5000; j < 6000; j++) {
			segsum[i] += mpSegment[i][j];
		}
	}

	// CSP
	m_pCSPTransform->ProcessMat(mpSegment, miNumSampInRawEpochs);
	int numChan = m_pCSPTransform->miNumRow;

	// for compare
	for (int i = 0; i < 40; i++) segsum[i] = 0;
	for (int i = 0; i < numChan; i++) {
		for (int j = 5000; j < 6000; j++) {
			segsum[i] += mpSegment[i][j];
		}
	}

	// allocate memory for xbuf
	float **xbuf = new float*[miNumChannelUsed];
	xbuf[0] = new float[numChan * miNumSampleUsed];
	for (int i = 1; i < numChan; i++) xbuf[i] = xbuf[i-1] + miNumSampleUsed;
	int nevt = 0;

	ofstream fout("NewResult.txt");
	fout << "Event detected at index\n";

	double *fmean = new double[numChan];
	double *fvar = new double[numChan];

	for (int iStart = 0; iStart < miNumSampInRawEpochs - miNumSampleUsed; iStart++) {
		// mean and variance for each channel

		if (iStart == 79) {
			printf("Debug here.\n");
		}

		// get window to xbuf
		for (int iChan = 0; iChan < numChan; iChan++) {
			for (int iSamp = 0; iSamp < miNumSampleUsed; iSamp++) {
				xbuf[iChan][iSamp] = mpSegment[iChan][iStart + iSamp];
			}
		}

		// Hamming
		for (int iChan = 0; iChan < numChan; iChan++) {
			for (int iSamp = 0; iSamp < miNumSampleUsed; iSamp++)
				xbuf[iChan][iSamp] = (float) (xbuf[iChan][iSamp] * m_pHamWin[iSamp]);
		}

		for (int iChan = 0; iChan < numChan; iChan++) {
			double tmp = 0;
			for (int iSamp = 0; iSamp < miNumSampleUsed; iSamp++) {
				tmp += xbuf[iChan][iSamp];
			}
			fmean[iChan] = tmp / miNumSampleUsed;
		}

		for (int iChan = 0; iChan < numChan; iChan++) {
			double tmp = 0;
			for (int iSamp = 0; iSamp < miNumSampleUsed; iSamp++) {
				double tmp1 = xbuf[iChan][iSamp] - fmean[iChan];
				tmp += tmp1 * tmp1;
			}
			fvar[iChan] = tmp / (miNumSampleUsed - 0);
		}

		if (m_pResLinear) {
			double tmp = 0;
			for (int i = 0; i < numChan; i++) tmp += fvar[i] * m_pResLinear[i];

			double res_constant = -9.266127218976645e-001;
			double thres = 0;

			tmp -= res_constant;

			int restest = (tmp >= thres);

			if (restest) {
				nevt++;
			} else if (nevt < 50) {
				nevt = 0;
			} else {
				//k-dwell_time+dwell_time_max+1
				int k = (iStart + 51) - nevt + 50 + 1;
				if (k == 2639) {
					printf("Debug here!\n");
				}

				fout << k;
				fout << "\n";

				//k = max(k+refractory_time-dwell_time+dwell_time_max,k+1)
				int n = 250 - nevt + 50 - 1;
				if (n > 0) iStart += n;
				nevt = 0;
			}
		}
	}

	fout.close();

	delete [] xbuf[0];
	delete [] xbuf;

	delete [] fmean;
	delete [] fvar;
}

