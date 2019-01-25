#pragma once
#include "FeedbackProcess.h"
#include "AttentionDetection.h"

#include "MultiTimeMotorImagery.h"

class CMIConcentration : public CFeedbackProcess
{
public:
	CMIConcentration(void);
	~CMIConcentration(void);

	virtual void SetProcFeedbackHandle(void *ptr);
	bool Initialize();
	virtual int ProcessEEGBuffer(char* eeg_buf, RESULT *pResult);

protected:
	CMultiTimeMI m_cspMTMI;
	CAttentionDetection m_cspAttention;

private:
	// Output result, evt = stimcode, score = output score
	void (__stdcall *m_hOutputResult)(int evt, float *score);
};
