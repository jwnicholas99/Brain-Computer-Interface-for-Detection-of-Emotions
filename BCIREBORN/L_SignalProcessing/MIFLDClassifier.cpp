#include "MIFLDClassifier.h"
#include "..\L_Utilities\ParaReader.h"

CMIFLDClassifier::CMIFLDClassifier(void)
{
	m_pProcBuffer = NULL;
	m_pCSPBuffer = NULL;
	m_pShiftBuffer = NULL;
	m_pSumBuffer = NULL;
	m_pSelFeature = NULL;

	m_nShiftPos = 0; 
	m_nShiftNum = 0;
}

CMIFLDClassifier::~CMIFLDClassifier(void)
{
	Clear();
}

void CMIFLDClassifier::Clear()
{
	if (m_pProcBuffer) {
		delete[] m_pProcBuffer;
		m_pProcBuffer = NULL;
	}
}

bool CMIFLDClassifier::Initialize(LPCSTR mfn)
{
	if (!m_FBCSP.LoadModel(mfn)) return false;
	if (!m_FLD.LoadModel(mfn)) return false;

	// Load model
	FILE *fp = NULL;
	if (fopen_s(&fp, mfn, "rt")) {
		printf("%s: Cannot load parameter file %s!\n", __FILE__,  mfn);
		return false;
	}

	printf("%s: load parameters:\n", __FILE__);
	CParaReader cpar;
	cpar.SetSeperator(':');
	char line[1024];

	this->miSamplingRate = 250;
	while (fgets(line, 1024, fp)) {
		char *par, *val;
		if (!cpar.GetParValue(line, par, val)) continue;

		//Common parameters: Sampling_Reate, EEG_Resolution_Hex, Channel_Order, Channel_Used
		if (SetCommParameter(par, val, fp)) {
		} else {
			continue;
		}
		printf("Processed '%s' = '%s'\n", par, val);
	}

	fclose(fp);

	// Time window set to 2 seconds
	miNumSampleUsed = 2 * miSamplingRate;
	if (miNumChannelUsed == 0) {
		miNumChannelUsed = m_FBCSP.GetNumChannels();
	}

	// Further initialize ...
	int nBands = 0;

	int nf_csp = m_FBCSP.m_nModel_m * 2 * m_FBCSP.m_nBands;

	m_pProcBuffer = new double[
		miNumChannelUsed // m_pProcBuffer
			+ nf_csp + nf_csp // m_pSumBuffer, m_pCSPBuffer
			+ m_FBCSP.m_nFeaSel // m_pSelFeature
		+ nf_csp * miNumSampleUsed // m_pShiftBuffer
	];

	m_pSumBuffer = m_pProcBuffer + miNumChannelUsed;
	m_pCSPBuffer = m_pSumBuffer + nf_csp;
	m_pSelFeature = m_pCSPBuffer + nf_csp;
	m_pShiftBuffer = m_pSelFeature + m_FBCSP.m_nFeaSel;

	memset(m_pSumBuffer, 0, nf_csp * sizeof(double));
	
	return true;
}

void CMIFLDClassifier::SetProcFeedbackHandle(LPVOID ptr)
{
	m_funcOutputScore = (void (__stdcall *)(int, double[])) ptr;
}

int CMIFLDClassifier::ProcessEEGBuffer(float *eeg_buf, int nch, int nspl, RESULT *pResult)
{
	int ispl = 0;
	assert(nch == miNumChannelUsed);

	int nf_csp = m_FBCSP.m_nModel_m * 2 * m_FBCSP.m_nBands;
	int nm = m_FBCSP.m_nModel_m * 2;

	while (ispl < nspl) {
		// copy data
		float *pFrom = eeg_buf + ispl;
		for (int ich = 0; ich < nch; ich++) {
			m_pProcBuffer[ich] = *pFrom;
			pFrom += nspl;
		}

		// get feature
		memset(m_pCSPBuffer, 0, nf_csp * sizeof(double));
		m_FBCSP.ProcessSample(m_pProcBuffer, m_pCSPBuffer);

		// add to buffer
		double *pd = m_pShiftBuffer + m_nShiftPos;
		for (int i = 0; i < nf_csp; i++) {
			double v = m_pCSPBuffer[i];
			*pd = v * v;
			m_pSumBuffer[i] += *pd;

			pd += miNumSampleUsed;
		}
		m_nShiftNum++;
		m_nShiftPos++;
		if (m_nShiftPos >= miNumSampleUsed) m_nShiftPos = 0;

		if (m_nShiftNum >= miNumSampleUsed) {
			// output score
			memset(m_pSelFeature, 0, m_FBCSP.m_nFeaSel * sizeof(double));

			int fi = 0;
			for (int ib = 0; ib < m_FBCSP.m_nBands; ib++) {
				double ssum = 0;
				for (int i = 0; i < nm; i++) ssum += m_pSumBuffer[fi + i];

				for (int i = 0; i < m_FBCSP.m_nFeaSel; i++) {
					int bidx = 0, cidx = 0;
					m_FBCSP.GetFeatureIdxs(i, bidx, cidx);

					if (bidx == ib) {
						m_pSelFeature[i] = m_pSumBuffer[fi + cidx] / ssum;
					}
				}

				fi += nm;
			}

			double score = m_FLD.GetScore(m_pSelFeature);

			if (m_funcOutputScore) {
				m_funcOutputScore(1, &score);
			}

			// remove one sample
			pd = m_pShiftBuffer + m_nShiftPos;
			for (int i = 0; i < nf_csp; i++) {
				m_pSumBuffer[i] -= *pd;
				pd += miNumSampleUsed;
			}
			m_nShiftNum--;
		}

		ispl++;
	}

	return 0;
}