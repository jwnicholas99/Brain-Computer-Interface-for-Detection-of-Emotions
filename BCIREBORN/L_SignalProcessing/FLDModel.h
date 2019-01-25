#pragma once

class CFLDModel
{
public:
	CFLDModel(void);
	virtual ~CFLDModel(void);

	bool LoadModel(const char* mfn);
	double GetScore(double *feature);

	int m_nFeature;

	int m_nS;

private:
	void Clear();

	double *m_pBuffer;
	double *m_pMean;
	double *m_pStd;
	double *m_pZ;
	double m_fBias;
};
