#include "stdafx.h"
#include <atlstr.h>

#include "..\L_SignalProcessing\P300SignalProc.h"

using namespace System;
using namespace System::Collections;
using namespace System::Runtime::InteropServices;

using namespace BCILib::P300;

namespace BCILib
{
	namespace Processor {
		public ref class P300Util
		{
		public:
			static bool EEGChannelAverage(String ^eeg_path)
			{
				CString path(eeg_path);
				LPCSTR pfn = path;
				return CP300SignalProc::EEG_ChannelAverage(pfn);
			}

			static bool TrainModel(String ^cfg_fn) {
				CString cfg(cfg_fn);
				LPCSTR cfn = cfg;
				CP300SignalProc p300;
				return p300.TrainModel(cfn);
			}

			static P300Result GetResult(CP300SignalProc *proc, array<double> ^score, array <unsigned short> ^code, int num_stim, int num_round) {
				RESULT result;
				pin_ptr<double> pd = &score[0];
				double *pscore = pd;

				pin_ptr<unsigned short> ps = &code[0];
				unsigned short *pcode = ps;

				proc->GetResult(pscore, pcode, num_stim, num_round, &result);
				Console::WriteLine("Get result {0}, confidence = {1} / threshold = {2}, reject = {3}", 
					result.iResult, result.fConfidence, result.fResult, result.iReject);

				P300Result res;
				res.result = result.iResult;
				res.confidence = result.fConfidence;
				res.threshold = result.fResult;
				res.accept = !result.iReject;

				return res;
			}
		};
	}
}