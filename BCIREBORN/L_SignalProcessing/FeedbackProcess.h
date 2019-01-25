#pragma once
#include "signalproc.h"

class CFeedbackProcess: public CSignalProc
{
public:
	virtual ~CFeedbackProcess() {};

	virtual bool Initialize(LPCSTR mfn) = 0;
	virtual int ProcessEEGBuffer(float* eeg_buf, int nch, int nspl, RESULT* pResult) = 0;
	virtual void SetProcFeedbackHandle(void *ptr) = 0;

	virtual int GetNumSampleUsed() { return miNumSampleUsed; }
	virtual int GetNumChannelUsed() { return miNumChannelUsed; }

protected:
	CFeedbackProcess::CFeedbackProcess(void) {};
};
