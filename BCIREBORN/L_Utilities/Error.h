/*
    File:   Error.h

    Header file for global error handling functions.
*/

#ifndef Error_h
#define Error_h

#include <stdarg.h>
typedef int PtErr;

enum 
{
    errOkay = 0
};
void OpenLogFile(const char *inLogFile);
void CloseLogFile(void);
void SetVerbosity(int inVerbosity);
int  GetVerbosity(void);

extern PtErr
  Error(char *function, PtErr code, char *format, ...);

extern PtErr
  FatalError(char *function, PtErr code, char *format, ...);

extern void
  SetErrorsAreFatal(int value);

extern void
  ReportErrors();

extern void
  ClearErrors();

extern void ErrReport(char *msg, ...);
extern void ErrWarn(char *msg, ... );
extern void PrintMsg(char *msg, ... );
extern void Print(char *msg);
#define ErrWarnNoWait ErrWarn

// ccwang: a simple function for error message
void SetErrorMessage(char *format, ...);
const char *GetErrorMessage();

#endif // Error_h

