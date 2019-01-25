#pragma once
#include "feedbackprocess.h"
#include "FilterBankCSP.h"
#include "FLDModel.h"

class CMIFLDClassifier :
	public CFeedbackProcess
{
public:
	CMIFLDClassifier(void);
	~CMIFLDClassifier(void);

	bool Initialize(LPCSTR mfn);
	int ProcessEEGBuffer(float* eeg_buf, int nch, int nspl, RESULT* pResult);
	void SetProcFeedbackHandle(void *ptr);

private:
	CFilterBankCSP m_FBCSP;
	CFLDModel m_FLD;

	void Clear();

	double* m_pProcBuffer, *m_pCSPBuffer,
		*m_pSumBuffer, *m_pSelFeature, *m_pShiftBuffer;
	int m_nShiftPos, m_nShiftNum;

	void (__stdcall *m_funcOutputScore)(int n, double score[]);
};
