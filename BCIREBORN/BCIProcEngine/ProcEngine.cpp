// This is the main DLL file.

#include "stdafx.h"
#include <atlstr.h>

#include "..\L_SignalProcessing\FeedbackProcess.h"

#include "..\L_SignalProcessing\AttentionDetection.h"
#include "..\L_SignalProcessing\MultiTimeMotorImagery.h"
#include "..\L_SignalProcessing\P300SignalProc.h"
#include "..\L_Utilities\ConsoleRedirect.h"

using namespace System;
using namespace System::Collections;
using namespace System::Runtime::InteropServices;
using namespace BCILib::App;

// Class Delearation
namespace BCILib
{
	namespace Processor {
		public ref class ProcEngine: BCILib::App::BCIEngine
		{
		public:
			ProcEngine(BCIProcType proc_type);
			~ProcEngine();

			virtual void ProcEEGBuf (array<float> ^buf, int nch, int nspl) override;
			virtual bool Initialize(Hashtable ^parameters) override;
			virtual bool Initialize(String ^mdl_fn) override;
			virtual void SetFeedbackHandler(System::Delegate ^handler) override;
			virtual property int NumSampleUsed {
				int get() override;
			}
			virtual property int NumChannelUsed {
				int get() override;
			}

			virtual property IntPtr Processor {
				IntPtr get() override
				{
					IntPtr p((void*) processor); 
					return p;
				}
			}

			virtual void Free() override;

			static void SetRedirectConsole(SafeHandle ^ hout)
			{
				// not implemented yet
				if (hout->IsInvalid) {
					Console::WriteLine("ProcEngine.cpp: SetRedirectConsole called with invalid parameter!");
					return;
				}

				if (console_rout == NULL) {
					Console::WriteLine("Redirect stdout:{0}/{1:X}", _fileno(stdout), (int)stdout);
					Console::WriteLine("Redirect stderr:{0}/{1:X}", _fileno(stderr), (int)stderr);
					console_rout = hout->DangerousGetHandle().ToPointer();
					int rv = ConsoleRedirect(console_rout);
					Console::WriteLine("ProcEngine.cpp: ConsoleRedirect returned value = {0}(0=no error, 1=_open_osfhandle 2=freopen_s)\n", rv);
					Console::WriteLine("ProcEngine.cpp: saved = {0}, {1}\n", GetSavedOutFd(), GetSavedErrFd());
					Console::WriteLine("Redirect stdout:{0}/{1:X}", _fileno(stdout), (int)stdout);
					Console::WriteLine("Redirect stderr:{0}/{1:X}", _fileno(stderr), (int)stderr);
				}
			}

			static void ResetRedirectConsole()
			{
				if (console_rout == NULL) {
					Console::WriteLine("ProcEngine.cpp: ResetRedirectConsole nothing to reset.");
				} else {
					ConsoleReset();
					console_rout = NULL;
					Console::WriteLine("ProcEngine.cpp: ResetRedirectConsole:{0}\n", (int) console_rout);
				}
			}

		private:
			CFeedbackProcess *processor;
			GCHandle ^gch;
			BCIProcType procType;
			static HANDLE console_rout = NULL;
			RESULT *presult;
		};
	}
}

// Implementation
BCILib::Processor::ProcEngine::ProcEngine(BCIProcType proctype)
: processor(NULL), gch(nullptr), procType(proctype)
{
	switch(procType) {
		case BCIProcType::Concentration:
			this->processor = new CAttentionDetection();
			break;
		case BCIProcType::MotorImagery:
			this->processor = new CMultiTimeMI();
			break;
		case BCIProcType::P300:
			this->processor = new CP300SignalProc();
			break;
		default:
			throw gcnew NotImplementedException();
			break;
	}
	presult = new RESULT;
}

BCILib::Processor::ProcEngine::~ProcEngine()
{
	free(presult);
	Free();
}

void BCILib::Processor::ProcEngine::Free()
{
	if (processor != NULL) {
		delete processor;
		processor = NULL;
	}

	if (gch != nullptr) {
		gch->Free();
		gch = nullptr;
	}
}

void BCILib::Processor::ProcEngine::ProcEEGBuf(array<float> ^buf, int nch, int nspl)
{
	if (!buf) return;

	pin_ptr<float> pb = &buf[0];
	float *pc = pb;

	processor->ProcessEEGBuffer(pc, nch, nspl, presult);
}

bool BCILib::Processor::ProcEngine::Initialize(Hashtable ^parameters)
{
	// Check parameters
	String ^ mfn = (String ^) parameters["Model"];
	return Initialize(mfn);
}

bool BCILib::Processor::ProcEngine::Initialize(String ^mfn)
{
	if (processor == NULL) {
		Console::WriteLine("Processor not created!");
		return false;
	}

	if (gch != nullptr && gch->IsAllocated) {
		IntPtr ptr = Marshal::GetFunctionPointerForDelegate((Delegate ^)gch->Target); //handler
		processor->SetProcFeedbackHandle(ptr.ToPointer());
	}

	// task: redirect all subsequent stdout/stderr to Console.Out. How?

	CString fn(mfn);
	LPCTSTR pfn = fn;

	bool result = false;
	try {
		result = processor->Initialize(pfn);
	}
	catch (Exception ^ex) {
		Console::WriteLine("Exception = {0}", ex);
		result = false;
	}

	return result;
}

void BCILib::Processor::ProcEngine::SetFeedbackHandler(Delegate ^handler)
{
	if (gch != nullptr) gch->Free();
	gch = GCHandle::Alloc(handler);
	if (processor != NULL) {
		IntPtr ptr = Marshal::GetFunctionPointerForDelegate((Delegate ^)gch->Target); //handler
		processor->SetProcFeedbackHandle(ptr.ToPointer());
	}
}

int BCILib::Processor::ProcEngine::NumSampleUsed::get()
{
	if (processor == NULL) return 0;
	else return processor->GetNumSampleUsed();
}

int BCILib::Processor::ProcEngine::NumChannelUsed::get()
{
	if (processor == NULL) return 0;
	else return processor->GetNumChannelUsed();
}
