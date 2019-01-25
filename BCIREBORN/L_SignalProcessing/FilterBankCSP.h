#pragma once

#include "Filtering.h"
#include "Transformation.h"

class CFilterBankCSP
{
public:
	CFilterBankCSP(void);
	~CFilterBankCSP(void);

	int m_nBands;
	int m_nModel_m;
	int m_nFeaSel;

	bool LoadModel(const char *mfn);

	void Clear();

	void ProcessSample(double *dataIn, double *data_FBCSP);

	void GetFeatureIdxs(int fno, int &bidx, int &cidx)
	{
		if (fno < m_nFeaSel) {
			bidx = fb_idx[fno];
			cidx = csp_idx[fno];
		}
	}

	int GetNumChannels()
	{
		return m_pTransfer[0].miNumCol;
	}

private:
	int *m_pBankSel;
	CFiltering *m_pFilters;
	CTransformat *m_pTransfer;

	int *m_pFeaSel, // include memory for the following 2
		*fb_idx, *csp_idx;
	double *m_pBuffer;
};
