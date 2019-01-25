#include "FilterBankCSP.h"
#include "..\L_Utilities\ParaReader.h"

CFilterBankCSP::CFilterBankCSP(void)
{
	m_nBands = 0;
	m_pBankSel = NULL;
	m_pFilters = NULL;
	m_pTransfer = NULL;

	m_nFeaSel = 0;
	m_pFeaSel = NULL;

	m_pBuffer = NULL;
}

CFilterBankCSP::~CFilterBankCSP(void)
{
	Clear();
}

void CFilterBankCSP::Clear()
{
	if (m_pBankSel) {
		delete [] m_pBankSel;
		m_pBankSel = NULL;
	}

	if (m_pFilters != NULL) {
		delete[] m_pFilters;
		m_pFilters = NULL;
	}

	if (m_pTransfer != NULL) {
		delete[] m_pTransfer;
		m_pTransfer = NULL;
	}

	if (m_pFeaSel != NULL) {
		delete[] m_pFeaSel;
		m_pFeaSel = NULL;
	}

	if (m_pBuffer) {
		delete m_pBuffer;
	}
}

bool CFilterBankCSP::LoadModel(const char *mfn)
{
	Clear();

	// Load model
	FILE *fp = NULL;
	if (fopen_s(&fp, mfn, "rt")) {
		printf("%s: Cannot load parameter file %s!\n", __FILE__, mfn);
		return false;
	}

	CParaReader cpar;
	cpar.SetSeperator(':');
	char line[1024];

	m_nModel_m = 2; // default value

	printf("%s: load model file %s ...\n", __FILE__, mfn);

	// format par: val
	const char *SEP = " ,\t\n";

	while (fgets(line, 1024, fp)) {
		char *par, *val;
		if (!cpar.GetParValue(line, par, val)) continue;

		// Specific parameters
		if (strcmp(par, "SelectedBanks") == 0) {
			m_nBands = atoi(val);
			m_pBankSel = new int[m_nBands];

			fgets(line, 1024, fp);

			char *context = NULL;
			char *tok = strtok_s(line, SEP, &context);

			int nbands = 0;
			while (tok) {
				m_pBankSel[nbands++] = atoi(tok) - 1;
				tok = strtok_s(NULL, SEP, &context);
			}
			assert(nbands == m_nBands);
		} else if (strcmp(par, "FileterBank") == 0) {
			int nbands = atoi(val);
			assert(nbands == m_nBands);
			m_pFilters = new CFiltering[m_nBands];
			for (int i = 0; i < m_nBands; i++) {
				m_pFilters[i].InitBAZiHexFile(fp);
			}
		} else if (strcmp(par, "Model_M") == 0) {
			m_nModel_m = atoi(val);
		} else if (strcmp(par, "CSP_Transform_Bands") == 0) {
			int n = atoi(val);
			assert(n == m_nBands);
			m_pTransfer = new CTransformat[n];
			for (int i = 0; i < n; i++) {
				m_pTransfer[i].LoadMatrixHexFile(fp);
			}
		} else if (strcmp(par, "Model_F") == 0) {
			m_nFeaSel = atoi(val);

			fgets(line, 1024, fp);
			m_pFeaSel = new int[m_nFeaSel * 3];

			char *context = NULL;
			char *tok = strtok_s(line, SEP, &context);
			fb_idx = m_pFeaSel + m_nFeaSel;
			csp_idx = fb_idx + m_nFeaSel;

			int nch = m_pTransfer->miNumRow;

			for (int i = 0; i < m_nFeaSel; i++) {
				int fno = atoi(tok) - 1;
				m_pFeaSel[i] = fno;

				int iband = fno / (m_nModel_m + m_nModel_m);
				int bidx = 0;
				for (; bidx < m_nBands; bidx++) {
					if (iband == m_pBankSel[bidx]) {
						break;
					}
				}
				fb_idx[i] = bidx;

				fno = fno % (m_nModel_m + m_nModel_m);
				csp_idx[i] = fno;

				tok = strtok_s(NULL, SEP, &context);
			}
		} else {
			continue;
		}
		printf("Processed: '%s' = '%s'\n", par, val);
	}

	fclose(fp);

	CFiltering *pFilters = m_pFilters;
	int nch = m_pTransfer[0].miNumCol;
	m_pFilters = new CFiltering[m_nBands * nch];
	int fi = 0;
	for (int ib = 0; ib < m_nBands; ib++) {
		for (int ich = 0; ich < nch; ich++, fi++) {
			m_pFilters[fi].SetAllPars(pFilters[ib].mpA, pFilters[ib].miOrderOfA,
				pFilters[ib].mpB, pFilters[ib].miOrderOfB);
		}
	}
	delete[] pFilters;

	m_pBuffer = new double[nch];

	return true;
}

// processing one sample of nchannel
// return number of csp channel (m * 2) in data
void CFilterBankCSP::ProcessSample(double *dataIn, double *data_FBCSP)
{
	int nch = m_pTransfer[0].miNumCol;
	int fi = 0;
	int di = 0;
	for (int bi = 0; bi < m_nBands; bi++) {
		// filtering
		for (int ich = 0; ich < nch; ich++, fi++) {
			m_pBuffer[ich] = m_pFilters[fi].ProcessValue(dataIn[ich]);
		}

		// CSP
		for (int mi = 0; mi < m_nModel_m * 2; mi++, di++) {
			data_FBCSP[di] = m_pTransfer[bi].ProcessCol(mi, m_pBuffer);
		}
	}
}