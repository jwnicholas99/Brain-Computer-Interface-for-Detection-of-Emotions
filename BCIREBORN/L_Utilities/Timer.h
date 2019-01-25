//----------------------------------------------------------------------------
// *
// *  Timer.h
// *  
// *  Description: Timer class
// *
// * 
 //----------------------------------------------------------------------------

#ifndef Timer_H
#define Timer_H

#include "SysInc.h"

#define SECONDS_PER_HOUR 3600
#define SECONDS_PER_MINUTE 60

class Timer {

public:

    Timer (void);
    ~Timer (void);

    long Start (void);
    long Finish (void);

    float GetElapsedTime (void) { return fElapsedTime; }
    float GetElapsedCPUTime (void) { return fElapsedCPUTime; }
  
//private:

    float fElapsedTime;
    float fElapsedCPUTime;
  
#ifdef	_WIN32					//	Changed timing structures for Win32 - DL 28/5/97

#ifndef UNDER_CE
	struct timeb	fStartTime;
	struct timeb	fEndTime;
	clock_t			fStartUsage;
	clock_t			fEndUsage;
#else  // define UNDER_CE
//	SYSTEMTIME fStartTime;
//	SYSTEMTIME fEndTime;
	DWORD fStartTime;
	DWORD fEndTime;
#endif // UNDER_CE

#else
    struct timeval fStartTime;
    struct timeval fEndTime;
    struct rusage fStartUsage;
    struct rusage fEndUsage;
#endif
};
  
#endif // Timer_H
