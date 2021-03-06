#include ".\MultiTimeMotorImagery.h"
#include "..\L_Utilities\ParaReader.h"

CMultiTimeMIModel::CMultiTimeMIModel()
{
	m_nSplStart = m_nSplEnd = 0;
	m_nSamplingRate = 0;

	m_pFilters = NULL;
	m_nNumFilters = 0;

	m_pBandCSPTransfer = NULL;
	m_nModel_m = 2;

	m_nTimeSegment = 1;
	m_pTimeSegStart = NULL; //m_pTimeSegLenth = m_pFBTimeUsed = NULL;

	m_nFeaSel = 0;
	m_pTrainH = NULL;

	m_nTrainClass0 = m_nTrainClass1 = 0;
	m_pTrainDataBuffer = NULL;

	m_nBandMap = 0;

	m_strAllChannel = m_strUsedChannel = NULL;
	m_nAllChannel = m_nUsedChannel = 0;
	m_pUsedChannel = NULL;
	m_pFeaIndex = NULL;
}

CMultiTimeMIModel::~CMultiTimeMIModel()
{
	if (m_pFilters) {
		delete[] m_pFilters;
		m_pFilters = NULL;
		m_nNumFilters = 0;
	}

	if (m_pTimeSegStart) delete[] m_pTimeSegStart;
	if (m_pBandCSPTransfer) delete[] m_pBandCSPTransfer;

	if (m_pTrainDataBuffer) delete[] m_pTrainDataBuffer;

	if (m_pFeaIndex) delete m_pFeaIndex;
}

bool CMultiTimeMIModel::LoadModel_Hex(LPCSTR fn_par)
{
	bool rst = true;

	printf("Load MultiTimeMIModel file %s ...\n", fn_par);

	FILE *fp;
	if (fopen_s(&fp, fn_par, "rt")) {
		printf("Cannot load model file %s!", fn_par);
		return false;
	}

	CParaReader cpar;
	cpar.SetSeperator(':');
	char line[1024];

	// format:
	// par: val
	while (fgets(line, sizeof(line), fp)) {
		char *par, *val;
		if (!cpar.GetParValue(line, par, val)) continue;
		printf("Read '%s' = '%s'\n", par, val);

		if (strcmp(par, "Sampling_Rate") == 0) {
			m_nSamplingRate = atoi(val);
			printf("Sampling_Rate=%d\n", m_nSamplingRate);
		} else if (strcmp(par, "EEG_Resolution_Hex") == 0) {
			sscanf_s(val, "%I64X", &m_fResolution);
			printf("Resolution=%lf\n", m_fResolution);
		} else if (strcmp(par, "Channel_Order") == 0) {
			m_nAllChannel = atoi(val);
			fgets(line, sizeof(line), fp);
			if (m_strAllChannel) delete m_strAllChannel;
			m_strAllChannel = _strdup(line);
			printf("Channel_Order: %s\n", m_strAllChannel);
		} else if (strcmp(par, "Channel_Used") == 0) {
			m_nUsedChannel = atoi(val);
			fgets(line, sizeof(line), fp);
			if (m_strUsedChannel) delete m_strUsedChannel;
			m_strUsedChannel = _strdup(line);
			printf("Channel Used:%s\n", m_strUsedChannel);
		} else if (strcmp(par, "Processing_Start_Sample") == 0) {
			m_nSplStart = atoi(val);
			printf("Processing_Start_Sample:%d\n", m_nSplStart);
		} else if (strcmp(par, "Processing_End_Sample") == 0) {
			m_nSplEnd = atoi(val);
			printf("Processing_End_Sample: %d\n", m_nSplEnd);
		} else if (strcmp(par, "FileterBank") == 0) {
			m_nNumFilters = atoi(val);
			printf("FileterBank nf=%d\n", m_nNumFilters);
			m_pFilters = new CFiltering[m_nNumFilters];
			for (int i = 0; i < m_nNumFilters; i++) {
				printf("Band %d ... ", i+1);
				m_pFilters[i].InitBAZiHexFile(fp);
				printf("Done.\n");
			}
		} else if (strcmp(par, "Model_M") == 0) {
			m_nModel_m = atoi(val);
			printf("Model_M:%d\n", m_nModel_m);
		} else if (strcmp(par, "Model_TimeSeg") == 0) {
			m_nTimeSegment = atoi(val); // e.g. 5,2
			printf("Model_TimeSeg: %d ... \n", m_nTimeSegment);
			if (m_pTimeSegStart) delete[] m_pTimeSegStart;
			m_pTimeSegStart = new int[m_nTimeSegment * 2];
			int *timeseg_len = m_pTimeSegStart + m_nTimeSegment;
			for (int ti = 0; ti < m_nTimeSegment; ti++) {
				fgets(line, sizeof(line), fp);
				double t0, t1;
				sscanf_s(line, "%lf%lf", &t0, &t1);
				int s0 = (int) (t0 * m_nSamplingRate) - 1;
				int s1 = (int)(t1 * m_nSamplingRate) - 1;
				if (m_nSplStart > s0) {
					m_nSplStart = s0;
				} if (m_nSplEnd < s1) {
					m_nSplEnd = s1;
				}
				m_pTimeSegStart[ti] = s0;
				timeseg_len[ti] = s1 - s0 + 1;
				printf("(%f - %f) --> (%d, %d)\n", t0, t1, m_pTimeSegStart[ti], timeseg_len[ti]);
			}
		} else if (strcmp(par, "CSP_Transform_Bands") == 0) { // nf (bands) X nt (segments)
			int nf, nt;
			sscanf_s(val, "%d,%d", &nf, &nt);
			printf("CSP_Transform_Bands: %d, %d ...\n", nf, nt);
			if (nf != m_nNumFilters || nt != m_nTimeSegment) {
				printf("Value wrong!\n");
				rst = false;
				break;
			}

			if (m_pBandCSPTransfer) delete[] m_pBandCSPTransfer;
			int ncsp = nf * nt;
			m_pBandCSPTransfer = new CTransformat[ncsp];
			for (int i = 0; i < ncsp; i++) {
				printf("%d ", i + 1);
				m_pBandCSPTransfer[i].LoadMatrixHexFile(fp);
				if (m_nUsedChannel == 0) {
					m_nUsedChannel = m_pBandCSPTransfer[i].miNumRow;
				}
			}
			printf("\n");
		} else if (strcmp(par, "Model_F") == 0) {
			if (m_pFeaIndex) delete[] m_pFeaIndex;
			int nfall = m_nNumFilters * m_nTimeSegment * m_nModel_m * 2;
			m_pFeaIndex = new unsigned char[nfall];
			memset(m_pFeaIndex, 0, nfall);

			m_nFeaSel = atoi(val);
			printf("Model_F: %d ...\n", m_nFeaSel);
			m_nBandMap = 0;

			if (!fgets(line, sizeof(line), fp)) {
				printf("Error in Model_F\n");
				rst = false;
				break;
			}

			char *context;
			char *tok = strtok_s(line, CSignalProc::SEP, &context);
			for (int i = 0; i < m_nFeaSel; i++) {
				int fno = atoi(tok) - 1;
				tok = strtok_s(NULL, CSignalProc::SEP, &context);

				// iBans * m * 2 * Nseg + iSeg * m * 2 + iM
				int bn = fno / (m_nModel_m + m_nModel_m);
				int fn = bn / m_nTimeSegment; // filter index
				int tn = bn % m_nTimeSegment; // timesegment index

				int mno = fno % (m_nModel_m + m_nModel_m);
				int im = (mno >> 1);
				int id = (mno & 1); // 0 = from start, 1 = from end
				int dx[2] = {im, m_nUsedChannel - im - 1};
				int cn = dx[id]; // csp index

				m_nBandMap |= (1 << fn);
				printf("Feature %d -> (%d,%d,%d)\n", fno, fn, tn, cn);

				m_pFeaIndex[fno] = 1;
			}
		} else if (strcmp(par, "Model_Training_Feature_Hex") == 0) {
			int tn, n;
			sscanf_s(val, "%d,%d", &tn, &n);
			printf("Model_Training_Feature: %d, %d\n", tn, n);
			if (n != m_nFeaSel) {
				printf("Warning: wrong number of features\n");
			}

			if (m_pTrainDataBuffer) delete[] m_pTrainDataBuffer;
			m_pTrainDataBuffer = new double[(tn + 1) * (m_nFeaSel)];
			m_pTrainH = m_pTrainDataBuffer + tn * m_nFeaSel;

			m_nTrainClass0 = m_nTrainClass1 = 0;
			double *pFeaSample = NULL;
			for (int i = 0; i < tn; i++) {
				fgets(line, sizeof(line), fp);
				char *context;
				char *tok = strtok_s(line, CSignalProc::SEP, &context);

				if (atoi(tok) == 0) { // class 0, start from begin
					pFeaSample = m_pTrainDataBuffer + m_nTrainClass0 * m_nFeaSel;
					m_nTrainClass0++;
				} else { // class 1, start from end
					pFeaSample = m_pTrainDataBuffer + (tn - m_nTrainClass1 - 1) * m_nFeaSel;
					m_nTrainClass1++;
				}

				for (int j = 0; j < m_nFeaSel; j++) {
					tok = strtok_s(NULL, CSignalProc::SEP, &context);
					double dv;
					sscanf_s(tok, "%I64X", &dv);
					pFeaSample[j] = dv;
				}
			}

			double hs = 1.0;
			double hc = 4.0 / (3.0 * tn);
			hc = hs * pow(hc, 0.2);

			// calculate std
			for (int fi = 0; fi < m_nFeaSel; fi++) {
				double mean = 0;
				pFeaSample = m_pTrainDataBuffer + fi;
				for (int ti = 0; ti < tn; ti++) {
					mean += *pFeaSample;
					pFeaSample += m_nFeaSel;
				}
				mean /= tn;

				double fvar = 0;
				pFeaSample = m_pTrainDataBuffer + fi;
				for (int ti = 0; ti < tn; ti++) {
					double dv = (*pFeaSample) - mean;
					fvar += dv * dv;
					pFeaSample += m_nFeaSel;
				}

				if (tn > 2) fvar /= (tn - 1);
				fvar = sqrt(fvar); // std

				fvar *= hc;
				if (fvar == 0) fvar = 0.005; // use a small value of h instead of zero

				m_pTrainH[fi] = fvar;
			}
		}
	}

	fclose(fp);

	printf("Load model ends. spl_start=%d, spl_end=%d.\n", m_nSplStart, m_nSplEnd);

	return rst;
}

CMultiTimeMI::CMultiTimeMI(void)
{
	m_pTestFeaData = NULL;

	m_pSelEEGBuffer = NULL;
	m_pSelEEGCopy = NULL;
	m_pSelEEGData = NULL;

	m_hOutputResult = NULL;
}

CMultiTimeMI::~CMultiTimeMI(void)
{
	if (m_pTestFeaData) delete [] m_pTestFeaData;

	if (m_pSelEEGBuffer) delete[] m_pSelEEGBuffer;
	if (m_pSelEEGData) delete[] m_pSelEEGData;
	if (m_pSelEEGCopy) delete[] m_pSelEEGCopy;
}

// Initialize model
bool CMultiTimeMI::Initialize(LPCSTR mfn)
{
	if (!m_model.LoadModel_Hex(mfn)) return false;
	mfResolution = (float) m_model.m_fResolution;

	// define channels
	miNumTotalChannel = m_model.m_nAllChannel;
	SetChannelList(m_model.m_strAllChannel);
	miNumChannelUsed = m_model.m_nUsedChannel;
	SetChannelUsed(m_model.m_strUsedChannel);

	// define samples
	miSamplingRate = m_model.m_nSamplingRate;
	miProcStartSample = m_model.m_nSplStart;
	miProcEndSample = m_model.m_nSplEnd;

	miNumSampleUsed = miProcEndSample - miProcStartSample + 1;

	if (miNumSampleUsed <= 0) {
		SetErrorMessage("MultiTimmeMI.Initialize: Wrong value miNumSampleUsed = %d.", miNumChannelUsed);
		return false;
	}

	//if (miProcEndSample > miNumSampInRawEpochs) {
	//	SetErrorMessage("MultiTimmeMI.Initialize: epoch length too short for this model (%d, %d)\nCheck config EEG/PreStimDuration,PostStimDuration.",
	//		miProcEndSample, miNumSampInRawEpochs);
	//	return false;
	//}

	// EEG Buffer -- one extra for csp processing
	if (m_pSelEEGBuffer == NULL) {
		m_pSelEEGBuffer = new double[miNumChannelUsed * miNumSampleUsed + miNumSampleUsed];
	}

	// EEG copy
	if (m_pSelEEGCopy == NULL) {
		m_pSelEEGCopy = new double[miNumChannelUsed * miNumSampleUsed];
	}

	if (m_pSelEEGData == NULL) {
		// one extra buffer for time segment selection
		m_pSelEEGData = new double*[miNumChannelUsed * 2];
	}

	double *pd = m_pSelEEGBuffer + miNumSampleUsed;
	for (int ich = 0; ich < miNumChannelUsed; ich++) {
		m_pSelEEGData[ich] = pd;
		pd += miNumSampleUsed;
	}

	// Data buffer
	if (m_pTestFeaData == NULL) {
		m_pTestFeaData = new double[m_model.m_nFeaSel];
	}

	return true;
}

// return 1 for event, 0 for non-event
int CMultiTimeMI::ProcessEEGBuffer(float* eeg_buf, int nch, int nspl, RESULT* pResult)
{
	memset(pResult, 0, sizeof(RESULT));
	pResult->fConfidence = 0;
	pResult->iReject = 1;
	pResult->szResult = 0;
	pResult->fResult = 0;

	if (nch != miNumChannelUsed) {
		printf("ProcessEEGBuffer: number of channels wrong (%d,%d)!\n", nch, miNumChannelUsed);
		return 0;
	}

	if (nspl < miNumSampleUsed) {
		printf("ProcessEEGBuffer: number of sampless wrong (%d,%d)!\n", nspl, miNumSampleUsed);
		return 0;
	}

	// Copy to buffer of double float type
	double *pCopy = m_pSelEEGCopy;
	float *pInData = eeg_buf;
		// + (nspl - miNumSampleUsed);
	for (int ich = 0; ich < miNumChannelUsed; ich++, pInData += nspl) {
		for(int iSamp = 0; iSamp < miNumSampleUsed; iSamp++, pCopy++) {
			*pCopy = pInData[iSamp];
		}
	}

	// reset feature vector.
	for (int fi = 0; fi < m_model.m_nFeaSel; fi++) m_pTestFeaData[fi] = 0;

	int *ts_start = m_model.m_pTimeSegStart;
	int *ts_len = ts_start + m_model.m_nTimeSegment;

	double** pSelTimeSeg = m_pSelEEGData + miNumChannelUsed;
	int kd = 0;

	// Filter Bank
	int nf_pcsp = m_model.m_nModel_m * 2;
	int nf_pbk = nf_pcsp * m_model.m_nTimeSegment;

	for (int iBank = 0; iBank < m_model.m_nNumFilters; iBank++) {
		if ((m_model.m_nBandMap & (1 << iBank)) == 0) continue; // skip

		// copy data
		memcpy(m_pSelEEGData[0], m_pSelEEGCopy, 
			miNumChannelUsed * miNumSampleUsed * sizeof(double));

		// filtering
		for (int ich = 0; ich < miNumChannelUsed; ich++) {
			m_model.m_pFilters[iBank].FiltFilt(m_pSelEEGData[ich], miNumSampleUsed);
		}

		int fbt0 = iBank * nf_pbk;
		// Time Segment
		for (int its = 0; its < m_model.m_nTimeSegment; its++, fbt0 += nf_pcsp) {
			int nts = 0;
			for (int i = 0; i < nf_pcsp; i++) nts += m_model.m_pFeaIndex[i + fbt0];
			if (nts == 0) continue; // no feature from this time segment, skip

			// Get time segment data
			for (int ci = 0; ci < miNumChannelUsed; ci++) pSelTimeSeg[ci] = m_pSelEEGData[ci] + ts_start[its];
			// Get CSP transfermer
			CTransformat *pcsp =  NULL;
			if (m_model.m_pBandCSPTransfer) pcsp = m_model.m_pBandCSPTransfer + (iBank * m_model.m_nTimeSegment + its);

			double vsum = 0;
			int k0 = kd;
			// CSP
			int tlen = ts_len[its];
			for (int mi = 0; mi < nf_pcsp; mi++) {
				int ncsp = mi / 2;
				if (mi & 1) ncsp = miNumChannelUsed - 1 - ncsp;
				if (pcsp) pcsp->ProcessMat(m_pSelEEGBuffer, ncsp, pSelTimeSeg, tlen);
				double var = GetVariance(m_pSelEEGBuffer, tlen);
				vsum += var;
				if (m_model.m_pFeaIndex[fbt0 + mi]) {
					m_pTestFeaData[kd++] = var;
				}
			}

			// normalize
			for (int mi = k0; mi < kd; mi++) {
				m_pTestFeaData[mi] = log(m_pTestFeaData[mi] / vsum);
			}
		} // End of timesegment
	} // End of filter banks

	// if (kd == m_model.m_nFeaSel) printf("Error here number of features got!!!\n");

	pResult->fConfidence = (float) m_model.CalScore(m_pTestFeaData);

	if (m_hOutputResult) {
		//printf("MTS:output score = %f\n", pResult->fConfidence);
		m_fScore[0] = pResult->fConfidence;
		m_fScore[1] = 1 - m_fScore[0];
		m_hOutputResult(m_fScore, 2);
	} else {
		//printf("MTS:result score = %f\n", pResult->fConfidence);
	}

	return TRUE;
}

double CMultiTimeMIModel::Parzen(double fval, int ncol, int cls)
{
	int tni = 0;
	double *pFeaSample = m_pTrainDataBuffer;
	int inc = m_nFeaSel;

	switch (cls) {
		case 0:
			tni = m_nTrainClass0;
			break;
		case 1:
			tni = m_nTrainClass1;
			pFeaSample += (m_nTrainClass0 + m_nTrainClass1 - 1) * m_nFeaSel; // from end
			inc = -inc;
			break;
	}

	double px = 0.0;
	double RSQRT_2PI;
	sscanf_s("3FD9884533D43651", "%I64X", &RSQRT_2PI);

	for (int ti = 0; ti < tni; ti++) {
		// kernel((x - t) / h)
		double dv = fval - pFeaSample[ncol];
		dv /= m_pTrainH[ncol];
		dv = exp(-0.5 * dv * dv) * RSQRT_2PI;
		px += dv;
		pFeaSample += inc;
	}

	px /= (tni * m_pTrainH[ncol]);
	return px;
}

double CMultiTimeMIModel::CalScore(double *fea_data) {
	//
	// NBPW: Naive Bayesian Classifier using Parzen function
	//
	// Training Data m_pTrainData, class 0 starting from beginning, class 1 starting from end.
	double pxw0 = ((double) m_nTrainClass0) / (m_nTrainClass0 + m_nTrainClass1);
	double pxw1 = 1.0 - pxw0;

	for (int fi = 0; fi < m_nFeaSel; fi++) {
		// calculate p(x/wi) for each data point
		double p0 = Parzen(fea_data[fi], fi, 0);
		double p1 = Parzen(fea_data[fi], fi, 1);

		if (p0 == 0 && p1 == 0) {
			printf("Warning....p(x) = 0 for class 0 and 1.\n");
			p0 = p1 = 0.5;
		}

		pxw0 *= p0;
		pxw1 *= p1;
	}

	pxw0 /= (pxw0 + pxw1);

	return pxw0;
}

void CMultiTimeMI::SetProcFeedbackHandle(void *ptr) {
	m_hOutputResult = (void (__stdcall *)(float *, int))ptr; 
}

