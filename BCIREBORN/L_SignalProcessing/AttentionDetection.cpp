#include "attentiondetection.h"
#include "..\L_Utilities\ParaReader.h"

CAttentionDetection::CAttentionDetection(void)
{
	m_pAlphaFilter = m_pBetaFilter = NULL;

	m_pSelEEGBuffer = NULL;
	m_pSelEEGCopy = NULL;
	m_pSelEEGData = NULL;

	m_pTestFeaData = NULL;

	m_hOutputResult = NULL;
	//m_bReverseScore = 1;

	m_fEEGMean = m_fSelThreshold = 0;
}

CAttentionDetection::~CAttentionDetection(void)
{
	if (m_pAlphaFilter) delete m_pAlphaFilter;
	if (m_pBetaFilter) delete m_pBetaFilter;

	if (m_pSelEEGBuffer) delete[] m_pSelEEGBuffer;
	if (m_pSelEEGData) delete[] m_pSelEEGData;
	if (m_pSelEEGCopy) delete[] m_pSelEEGCopy;

	if (m_pTestFeaData) delete [] m_pTestFeaData;
}

// Initialize model and parameters
bool CAttentionDetection::Initialize(LPCSTR mfn)
{
	if (!m_Model.LoadModelFile(mfn)) return false;

	m_fSelThreshold = m_Model.m_fSelThreshold;

	//float fv = 0;
	//if (res.Get(rn, "TrialSelThreshold", &fv) == errOkay) {
	//	m_fSelThreshold = fv;
	//	printf("ThialSelThreshold = %lf\n", m_fSelThreshold);
	//}

	m_fEEGMean = m_Model.m_fEEGMean;

	if (miNumChannelUsed != m_Model.m_nChannelUsed ||
		miNumSampleUsed != m_Model.m_nSampleUsed)
	{
		miNumSampleUsed = m_Model.m_nSampleUsed;
		miNumChannelUsed = m_Model.m_nChannelUsed;

		if (m_pSelEEGCopy) delete[] m_pSelEEGCopy;
		if (m_pSelEEGBuffer) delete[] m_pSelEEGBuffer;
		if (m_pSelEEGData) delete[] m_pSelEEGData;
		m_pSelEEGCopy = NULL;
		m_pSelEEGBuffer = NULL;
		m_pSelEEGData = NULL;
	}

	int nch = miNumChannelUsed;
	if (nch < 2) nch = 2;

	if (m_pSelEEGCopy == NULL) {
		m_pSelEEGCopy = new double[nch * miNumSampleUsed]; // backup of EEG data
	}
	if (m_pSelEEGBuffer == NULL) {
		m_pSelEEGBuffer = new double[nch * miNumSampleUsed + miNumSampleUsed]; // processing buffer and csp buffer
	}

	/*
	// alpha power
	int forder = 0;
	if (res.Get(rn, "AF_Order", &forder) == errOkay) {
		char hex_B[512];
		char hex_A[512];
		res.Get(rn, "AF_B", hex_B, sizeof(hex_B));
		res.Get(rn, "AF_A", hex_A, sizeof(hex_A));
		m_pAlphaFilter = CrtFilterFromHexString(forder, hex_B, hex_A);
	}

	// beta power
	forder = 0;
	if (res.Get(rn, "BF_Order", &forder) == errOkay) {
		char hex_B[512];
		char hex_A[512];
		res.Get(rn, "BF_B", hex_B, sizeof(hex_B));
		res.Get(rn, "BF_A", hex_A, sizeof(hex_A));
		m_pBetaFilter = CrtFilterFromHexString(forder, hex_B, hex_A);
	}
	*/

	if (m_pSelEEGData == NULL) {
		m_pSelEEGData = new double*[nch];
	}

	double *pd = m_pSelEEGBuffer;
	for (int ich = 0; ich < nch; ich++) {
		pd += miNumSampleUsed;
		m_pSelEEGData[ich] = pd;
	}

	if (m_pTestFeaData) delete[] m_pTestFeaData;
	m_pTestFeaData = new double[m_Model.m_nFeaSel];

	if (m_fSelThreshold > 0) {
		printf("Segsel Mean = %g, Threadhold = %g\n", m_fEEGMean, m_fSelThreshold);
	}

	return true;
}

void CAttentionDetection::SetProcFeedbackHandle(void *ptr) {
	m_hOutputResult = (void (__stdcall *)(int, int, double*)) ptr;
}

// ccwang 20100330: use variable length input data
int CAttentionDetection::ProcessEEGBuffer(float* eeg_buf, int nch, int nspl, RESULT* pResult)
{
	memset(pResult, 0, sizeof(RESULT));
	pResult->fConfidence = 0;
	pResult->iReject = 1;
	pResult->szResult = 0;
	pResult->fResult = 0;

	if (nch != m_Model.m_nChannelUsed) {
		printf("ProcessEEGBuffer: number of channels wrong (%d,%d)!\n", nch, m_Model.m_nChannelUsed);
		return 0;
	}

	// can be less, but not more
	if (nspl <= 0) {
		printf("ProcessEEGBuffer: number of sampless wrong (%d,%d)!\n", nspl, m_Model.m_nSampleUsed);
		return 0;
	}

	double *pCopy = m_pSelEEGCopy;

	bool badtrial = false;
	bool allzero = true;

	// offset for copying
	int off0 = nspl - miNumSampleUsed;
	if (off0 < 0) off0 = 0;

	// used number of samples
	m_nCurUsed = miNumSampleUsed;
	if (nspl < m_nCurUsed) m_nCurUsed = nspl;

	float *pInData = eeg_buf + off0;

	for (int ich = 0; ich < miNumChannelUsed; ich++) {
		for (int iSamp = 0; iSamp < m_nCurUsed; iSamp++) {
			pCopy[iSamp] = pInData[iSamp];

			allzero &= (pCopy[iSamp] == 0);
		}
		pInData += nspl;
		pCopy += miNumSampleUsed;
	}

	badtrial |= allzero;

	if (badtrial) {
		if (allzero) printf("No signal.\n");
		else printf("Bad trial no output.\n");
	} else {
		if (m_pAlphaFilter) CalculatePower(m_pAlphaFilter, 1);
		if (m_pBetaFilter) CalculatePower(m_pBetaFilter, 2);

		// last step, as it may do downsampling
		CalculateScore(pResult);
	}

	return 1;
}

void CAttentionDetection::CalculatePower(CFiltering *pflt, int rtype)
{
	double *pCopy = m_pSelEEGCopy;
	double *pSignal = m_pSelEEGData[0];
	double *pPower = m_pSelEEGData[1];

	for (int ich = 0; ich < m_nCurUsed; ich++) {
		memcpy(pSignal, pCopy, m_nCurUsed * sizeof(double));
		pCopy += miNumSampleUsed;

		pflt->Process(pSignal, m_nCurUsed);
		double p = GetVariance(pSignal, m_nCurUsed);
		if (p < 1.0) p = 1.0;
		pPower[ich] = log(p);
	}

	if (m_hOutputResult) m_hOutputResult(rtype, miNumChannelUsed, pPower);
}

// Calculating score
void CAttentionDetection::CalculateScore(RESULT *pResult)
{
	// assume all models have the same down sampling settings and number of samples
	// Downsampling
	int nspl = m_nCurUsed; // actual number of samples after down sampling if any
	int dr = m_Model.m_nDownRatio;
	if (dr > 1) {
		// filtering
		double *pCopy = m_pSelEEGCopy;
		for (int ich = 0; ich < miNumChannelUsed; ich++) {
			m_Model.m_fltDownSampling.Process(pCopy, m_nCurUsed);
			pCopy += miNumSampleUsed;
		}

		// down sampling
		double *pOrig = m_pSelEEGCopy;
		for (int ich = 0; ich < miNumChannelUsed; ich++) {
			pCopy = pOrig;
			for (int ispl = 0; ispl < m_nCurUsed; ispl += dr) {
				*pCopy++ = pOrig[ispl];
			}
			pOrig += miNumSampleUsed;
		}
		nspl = (m_nCurUsed + dr - 1) / dr;
	}

	int nfea = m_Model.m_nFeaSel;
	int *flt_idx = m_Model.m_pFeaSel + nfea;
	int *csp_idx = flt_idx + nfea;
	for (int i = 0; i < nfea; i++) m_pTestFeaData[i] = 0;

	double *nf_csp = m_pSelEEGBuffer;

	bool zero_channnel = false;

	for (int iBank = 0; iBank < m_Model.m_nNumFilters; iBank++) {
		if ((m_Model.m_nFeaFBMap & (1 << iBank)) == 0) { // skip
			continue;
		}

		// copy data for filtering
		for (int ich = 0; ich < m_Model.m_nChannelUsed; ich++) {
			double *pCopy = m_pSelEEGCopy + miNumSampleUsed * m_Model.m_pChannelIdx[ich];
			memcpy(m_pSelEEGData[ich], pCopy, nspl * sizeof(double));

			int is = 0;
			// while (is < nspl && m_pSelEEGData[ich][is] == 0) is++;
			while (is < nspl && m_pSelEEGData[ich][is] < 10.0) is++;
			if (is == nspl) {
				zero_channnel = true;
				break;
			}

			m_Model.m_pFilters[iBank].FiltFilt(m_pSelEEGData[ich], nspl);
		}

		if (zero_channnel) break;

		for (int nf = 0; nf < nfea; nf++) {
			if (flt_idx[nf] != iBank) continue;

			// CSP with idx csp_idx[nf]
			if (m_Model.m_pBandCSPTransfer) {
				m_Model.m_pBandCSPTransfer[iBank].ProcessMat(nf_csp, csp_idx[nf], m_pSelEEGData, nspl);
			} else {
				memcpy(nf_csp, m_pSelEEGData[csp_idx[nf]], nspl * sizeof(double));
			}

			// log(var(fea))
			double var = GetVariance(nf_csp, nspl);
			if (var < 1.0) var = 1.0;
			m_pTestFeaData[nf] = log(var);
		}
	}

	double score = 0;
	if (zero_channnel) {
		score = -100.0;
	} else {
		for (int nf = 0; nf < nfea; nf++) {
			m_pTestFeaData[nf] -= m_Model.m_pFLDMean[nf];
			m_pTestFeaData[nf] /= m_Model.m_pFLDStd[nf];
			score += m_pTestFeaData[nf] * m_Model.m_pFLDLTV[nf];
		}
		score -= m_Model.m_fFLDBias;

		//if (m_bReverseScore)
			score = -score;

		score -= m_Model.m_fZeroScore;
		score /= (m_Model.m_fThresholdScore - m_Model.m_fZeroScore);
		score *= 60;

		//printf("Score = %f, ", score);
		//if (score < 0) score = 0;
		//else if (score > 100) score = 100;

	}
	//printf("\n");
	pResult->fResult = (float) score;

	if (m_hOutputResult) m_hOutputResult(0, 1, &score);
}

CAttentionModel::CAttentionModel(void)
{
	m_nChannelUsed = 0;
	m_pChannelIdx = NULL;
	m_strChannelUsed = NULL;
	m_nSampleUsed = 0;

	m_nDownRatio = 1;
	m_pFilters = NULL;
	m_nNumFilters = 0;
	m_pBandCSPTransfer = NULL;

	m_nModel_m = 1;
	m_nFeaSel = 0;
	m_pFeaSel = NULL;
	m_nFeaFBMap = 0;

	m_pFLDBuffer = NULL;
	m_pFLDMean = NULL;
	m_pFLDStd = NULL;
	m_pFLDLTV = NULL;

	m_fThresholdScore = m_fZeroScore = 0;
	m_fEEGMean = m_fSelThreshold = 0;
}

CAttentionModel::~CAttentionModel(void)
{
	Clear();
}

void CAttentionModel::Clear()
{
	if (m_strChannelUsed) delete m_strChannelUsed;
	m_strChannelUsed = NULL;

	if (m_pChannelIdx) delete m_pChannelIdx;
	m_pChannelIdx = NULL;

	if (m_pFilters) {
		delete[] m_pFilters;
		m_pFilters = NULL;
		m_nNumFilters = 0;
	}

	if (m_pBandCSPTransfer) {
		delete[] m_pBandCSPTransfer;
		m_pBandCSPTransfer = NULL;
	}

	if (m_pFeaSel) delete[] m_pFeaSel;
	m_pFeaSel = NULL;

	if (m_pFLDBuffer) delete[] m_pFLDBuffer;
	m_pFLDBuffer = NULL;
}

bool CAttentionModel::LoadModelFile(LPCSTR fn_par)
{
	Clear();

	bool rst = true;

	FILE *fp = NULL;
	if (fopen_s(&fp, fn_par, "rt")) {
		printf("Cannot load parameter file %s!\n", fn_par);
		return false;
	}

	CParaReader cpar;
	cpar.SetSeperator(':');
	char line[1024];

	int sampling_rate = 250;
	const char *SEP = " ,\t\n";

	// format par: val
	while (fgets(line, 1024, fp)) {
		char *par, *val;
		if (!cpar.GetParValue(line, par, val)) continue;
		printf("Read '%s' = '%s'\n", par, val);

		// Common parameters: EEG_Resolution, Channel_Used
		// if (SetCommParameter(par, val, fp)) continue;

		// Specific parameters
		if (strcmp(par, "Sampling Rate") == 0) {
			sampling_rate = (int) atof(val);
		} else if (strcmp(par, "EEG_Resolution") == 0) {
			// ignore
		} else if (strcmp(par, "Channel_Order") == 0) {
			// it actually should be Channel_Used
			par = "Channel_Used";
			m_nChannelUsed = atoi(val);
			fgets(line, 1024, fp);
			m_strChannelUsed = _strdup(line);
			if (m_pChannelIdx) delete[] m_pChannelIdx;
			m_pChannelIdx = new int[m_nChannelUsed];
			memset(m_pChannelIdx, 0, sizeof(int) * m_nChannelUsed);
		} else if (strcmp(par, "Decimate Sample") == 0) {
			if (strcmp(val, "true")) continue;

			//Org_Sampling_Rate
			fgets(line, 1024, fp);
			cpar.GetParValue(line, par, val);
			sampling_rate = atoi(val);

			//Decimate Ratio
			fgets(line, 1024, fp);
			cpar.GetParValue(line, par, val);
			m_nDownRatio = atoi(val);

			//Down-Sampling Filter
			m_fltDownSampling.InitBAHexFile(fp);
		} else if (strcmp(par, "Window Length (Seconds)") == 0) {
			double nseconds = atof(val);
			m_nSampleUsed = (int) (sampling_rate * nseconds);
		} else if (strcmp(par, "Number of Filter Banks") == 0) {
			m_nNumFilters = atoi(val);
			m_pFilters = new CFiltering[m_nNumFilters];
			for (int i = 0; i < m_nNumFilters; i++) {
				m_pFilters[i].InitBAZiHexFile(fp);
			}
		} else if (strcmp(par, "Model_M") == 0) {
			m_nModel_m = atoi(val);
		} else if (strcmp(par, "CSP_Transform_Bands") == 0) {
			int n = atoi(val);
			m_pBandCSPTransfer = new CTransformat[n];
			for (int i = 0; i < n; i++) {
				m_pBandCSPTransfer[i].LoadMatrixHexFile(fp);
			}
		} else if (strcmp(par, "Model_F") == 0) {
			m_nFeaSel = atoi(val);
			fgets(line, 1024, fp);
			if (m_pFeaSel) delete[] m_pFeaSel;
			m_pFeaSel = new int[m_nFeaSel * 3];
			char *context = NULL;
			char *tok = strtok_s(line, SEP, &context);
			int *flt_idx = m_pFeaSel + m_nFeaSel;
			int *csp_idx = flt_idx + m_nFeaSel;
			//int ncsp = m_pBandCSPTransfer[0].miNumRow;
			int ncsp = m_nChannelUsed;
			m_nFeaFBMap = 0;
			for (int i = 0; i < m_nFeaSel; i++) {
				int fno = atoi(tok) - 1;
				m_pFeaSel[i] = fno;
				if (m_nModel_m > 0) { // use csp
					flt_idx[i] = fno / (m_nModel_m + m_nModel_m);
					fno = fno % (m_nModel_m + m_nModel_m);
					int im = (fno >> 1);
					int id = (fno & 1);
					int dx[2] = {im, ncsp - 1 - im};
					csp_idx[i] = dx[id];
				} else { // no csp
					flt_idx[i] = fno / m_nChannelUsed;
					csp_idx[i] = fno % m_nChannelUsed;
				}

				tok = strtok_s(NULL, SEP, &context);
				m_nFeaFBMap |= (1 << flt_idx[i]);
			}
		} else if (strcmp(par, "Classification") == 0) {
			if (strcmp(val, "FLD") == 0) {
				// can be implemented in a seperate class
				if (m_pFLDBuffer) delete m_pFLDBuffer;
				m_pFLDBuffer = new double[m_nFeaSel * 3];
				m_pFLDMean = m_pFLDBuffer;
				m_pFLDStd = m_pFLDMean + m_nFeaSel;
				m_pFLDLTV = m_pFLDStd + m_nFeaSel;

				// FLD_FeatureMean:
				fgets(line, 1024, fp);
				cpar.GetParValue(line, par, val);
				int n = atoi(val);
				fgets(line, 1024, fp);
				char *context = NULL;
				char *tok = strtok_s(line, SEP, &context);
				double fv;
				for (int i = 0; i < n; i++) {
					sscanf_s(tok, "%I64X", &fv);
					m_pFLDMean[i] = fv;
					tok = strtok_s(NULL, SEP, &context);
				}

				// FLD_FeatureSTD: 
				fgets(line, 1024, fp);
				cpar.GetParValue(line, par, val);
				n = atoi(val);
				fgets(line, 1024, fp);
				tok = strtok_s(line, SEP, &context);
				for (int i = 0; i < n; i++) {
					sscanf_s(tok, "%I64X", &fv);
					m_pFLDStd[i] = fv;
					tok = strtok_s(NULL, SEP, &context);
				}

				// FLD_LinearTransformVector: 
				fgets(line, 1024, fp);
				cpar.GetParValue(line, par, val);
				n = atoi(val);
				fgets(line, 1024, fp);
				tok = strtok_s(line, SEP, &context);
				for (int i = 0; i < n; i++) {
					sscanf_s(tok, "%I64X", &fv);
					m_pFLDLTV[i] = fv;
					tok = strtok_s(NULL, SEP, &context);
				}

				//FLD_Bias: 
				fgets(line, 1024, fp);
				cpar.GetParValue(line, par, val);
				sscanf_s(val, "%I64X", &fv);
				m_fFLDBias = fv;

				//Empirical Scores Mean:
				fgets(line, 1024, fp);
				cpar.GetParValue(line, par, val);
				tok = strtok_s(val, SEP, &context);
				sscanf_s(tok, "%I64X", &fv);
				m_fFLDScoreMean[0] = fv;
				tok = strtok_s(NULL, SEP, &context);
				sscanf_s(tok, "%I64X", &fv);
				m_fFLDScoreMean[1] = fv;

				//Empirical Scores STD
				fgets(line, 1024, fp);
				cpar.GetParValue(line, par, val);
				tok = strtok_s(val, SEP, &context);
				sscanf_s(tok, "%I64X", &fv);
				m_fFLDScoreStd[0] = fv;
				tok = strtok_s(NULL, SEP, &context);				
				sscanf_s(tok, "%I64X", &fv);
				m_fFLDScoreStd[1] = fv;
			} else {
				printf("Unknown parameter.\n");
			}
		} else if (strcmp(par, "Empirical Threshold Score") == 0) {
			sscanf_s(val, "%I64X", &m_fThresholdScore);
		} else if (strcmp(par, "Empirical Zero Score") == 0) {
			sscanf_s(val, "%I64X", &m_fZeroScore);
		} else if (strcmp(par, "SegmentSelection") == 0) {
			sscanf_s(val, "%I64X, %I64X", &m_fEEGMean, &m_fSelThreshold);
			printf_s("Model: SegmentSelection mean = %g, thread = %g.\n", m_fEEGMean, m_fSelThreshold);
		}
	}

	fclose(fp);

	// if filter not defined, using default one
	if (m_pFilters == NULL) {
		m_nNumFilters = 0;
		const char **hexl = CAttentionDetection::GetFilterBankString(sampling_rate);
		while (hexl[m_nNumFilters]) m_nNumFilters++;
		m_nNumFilters /= 3;

		m_pFilters = new CFiltering[m_nNumFilters];
		for (int i = 0; i < m_nNumFilters; i++) {
			m_pFilters[i].InitHexString(hexl[0], hexl[1], hexl[2]);
			hexl += 3;
		}
	}

	return rst;
}
