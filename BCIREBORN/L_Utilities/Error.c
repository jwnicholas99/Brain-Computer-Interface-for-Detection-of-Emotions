/*
extern "C" {
#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <stdarg.h>
};
*/

#include "Error.h"
#include "Types.h"
#include "String.h"

static int gVerbosity = 0;
// ccwang 0 = no log; 1 = log file, 2 = log file + stdout
static FILE *gLogFile = NULL;

#define Stdout NULL

//static char *gLogFileName = NULL;

PtErr
  FatalError(char *function, PtErr code, char *format, ...)
{
    char message[1024];
    va_list args;

    va_start(args, format);
	vsprintf_s(message, format, args);
    va_end(args);

    fflush(stdout);
    fflush(stderr);

    fprintf(stderr, "FATAL ERROR: %s() [%d] %s\n", function, code, message);
    fflush(stderr);

    /* XXX -- Needed to apease the Metrowerks C++ compiler.  ***PSI***  26-Jan-96 */
    return code;
}

#define MAX_ERROR_STACK_SIZE  64

typedef struct ErrorStackEntry {
    PtErr code;
    char *function;
    char *message;
} ErrorStackEntry;


static ErrorStackEntry
  GlobalErrorStack[MAX_ERROR_STACK_SIZE];

static int
  GlobalErrorUpto = 0;

PtErr
  Error(char *function, PtErr code, char *format, ...)
{
    int i = GlobalErrorUpto;
    char buffer[1024];
    va_list args;

	if (i >= MAX_ERROR_STACK_SIZE) {
      i = MAX_ERROR_STACK_SIZE - 1;

	  if (GlobalErrorStack[i].function)
		  free (GlobalErrorStack[i].function);
	  if (GlobalErrorStack[i].message)
		  free (GlobalErrorStack[i].message);

	  GlobalErrorStack[i].code     = 0;
	  GlobalErrorStack[i].function = 0;
	  GlobalErrorStack[i].message  = 0;
	} else {
      GlobalErrorUpto++;
	}

    GlobalErrorStack[i].code = code;

    if (function)
      GlobalErrorStack[i].function = _strdup(function);
    else
      GlobalErrorStack[i].function = 0;

    va_start(args, format);
	vsprintf_s(buffer, format, args);
    va_end(args);

    GlobalErrorStack[i].message = _strdup(buffer);

    return code;
}

void
  ReportErrors()
{
    for (int i=0; i<GlobalErrorUpto; i++)
      if (GlobalErrorStack[i].message)
	printf("ERROR %d: %s: %s\n", 
	       GlobalErrorStack[i].code,
	       GlobalErrorStack[i].function,
	       GlobalErrorStack[i].message);
      else
	printf("ERROR %d: %s\n",
	       GlobalErrorStack[i].code,
	       GlobalErrorStack[i].function);
}

void
  ClearErrors()
{
    for (int i=0; i<GlobalErrorUpto; i++) {

	if (GlobalErrorStack[i].function)
	  free (GlobalErrorStack[i].function);

	if (GlobalErrorStack[i].message)
	  free (GlobalErrorStack[i].message);

	GlobalErrorStack[i].code     = 0;
	GlobalErrorStack[i].function = 0;
	GlobalErrorStack[i].message  = 0;
    }

    GlobalErrorUpto = 0;
}

//////////////////////////////////////////////////////////////////////////////
//
//  Function:		OpenLogFile
//
//  Description:	Open the log file
//
//////////////////////////////////////////////////////////////////////////////
void OpenLogFile(const char *inLogFile)
{
	if (gLogFile != NULL) {
		fclose(gLogFile);
	}

	fopen_s(&gLogFile, inLogFile, "w");
}

//////////////////////////////////////////////////////////////////////////////
//
//  Function:		CloseLogFile
//
//  Description:	Close the log file
//
//////////////////////////////////////////////////////////////////////////////
void CloseLogFile(void)
{
	if (gLogFile != NULL) {
		fclose(gLogFile);
		gLogFile = NULL;
	}
}

//////////////////////////////////////////////////////////////////////////////
//
//  Function:		SetVerbosity
//
//  Description:    SetVerbosity
//
//////////////////////////////////////////////////////////////////////////////
void SetVerbosity(int inVerbosity)
{
    gVerbosity = inVerbosity;
}

//////////////////////////////////////////////////////////////////////////////
//
//  Function:		GetVerbosity
//
//  Description:    GetVerbosity
//
//////////////////////////////////////////////////////////////////////////////
int GetVerbosity(void)
{
    return gVerbosity;
}


/*****************************************************************************
 *
 *  ErrReport(): Reports an error and exit from program
 * 
 *  Return Value: no return
 *
 *  Globals Used: none
 *
 ***************************************************************************+*/
void ErrReport(char *msg, ...)
{
    va_list ap;
    char buf[kMaxLine];

    va_start(ap, msg);
	vsprintf_s(buf, msg, ap);
    va_end(ap);
    PrintMsg("\n%s\n", buf);
	return;

} /* end routine ErrReport() */


/*****************************************************************************
 *
 *  ErrWarn(): Outputs a warning message, and continue
 * 
 *  Globals Used: none
 *
 ***************************************************************************+*/
void ErrWarn(char *msg, ...)
{
    va_list ap;
    char buf[kMaxLine];
  
    va_start(ap, msg);
	vsprintf_s(buf, msg, ap);
    va_end(ap);
    fprintf(stderr, "\nWARNING: %s\n", buf);

    return;
   
} /* end routine ErrWarn() */

void PrintMsg (char * msg, ...)
{
	if (gVerbosity > 0) 
    {
		va_list ap;

		if (strlen(msg) == 0)  return;

		if (gLogFile != NULL) {
			va_start(ap, msg);
			vfprintf(gLogFile, msg, ap);
			fflush(gLogFile);
			va_end(ap);
		}

        if (gVerbosity > 1)
        {
			va_start(ap, msg);
            vprintf(msg, ap);
			va_end(ap);
        }

	}
} /* end routine PrintMsg() */

// ccwang: found memory leak in GlobalErrorStack so add a static object and clear 
// memory in it's destructor. -- 2006/08/21
#ifdef __cplusplus
class CError {
public:
	CError() {};
	~CError();
};

CError::~CError()
{
	::ClearErrors();
}

static CError gError;
#endif

// ccwang: A simple function to set global error message
static char gErrMsg[1024] = {0};
static bool gErrSet = false;
void SetErrorMessage(char *format, ...)
{
	va_list ap;

	va_start(ap, format);
	vsprintf_s(gErrMsg, format, ap);
	va_end(ap);

	puts(gErrMsg);
	gErrSet = true;
}

const char *GetErrorMessage() {
	if (!gErrSet) return NULL;

	gErrSet = false;
	return gErrMsg;
}
