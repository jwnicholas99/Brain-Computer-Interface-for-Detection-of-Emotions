#pragma once
#include "signalproc.h"
#include "Filtering.h"
#include "Transformation.h"

class CRehabMotorImage :
	public CSignalProc
{
public:
	CRehabMotorImage(void);
	~CRehabMotorImage(void);
	// Initialize model and parameters
	bool Initialize(void);
	// Load parameters from parameter file
	bool LoadParameters(LPCSTR fn_par);
	virtual int ProcessEEGBuffer(char* eeg_buf, RESULT *pResult);
	void TestEEGAll(char* eeg_buf, int nSample);

protected:

	CFiltering *m_pFilter;
	CTransformat *m_pCSPTransform;
	double *m_pHamWin;
	double *m_pResLinear;
};
