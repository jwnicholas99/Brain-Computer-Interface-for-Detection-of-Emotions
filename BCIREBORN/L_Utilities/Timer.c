//----------------------------------------------------------------------------    
//    File:   Timer.c
//    Description:
//
//    Timer methods, for wall-time and cpu-time.
//----------------------------------------------------------------------------


#include "Timer.h"

#ifdef _WIN32
extern "C" int gettimeofday(struct timeval *tp, struct timezone *tzp);
extern "C" int getrusage(int who, struct rusage *rusage);
#endif

//============================================================================
//  Timer::Timer
//============================================================================/
Timer::Timer (void)
{
}

//============================================================================
//  Timer::~Timer
//============================================================================/
Timer::~Timer (void)
{
}

//============================================================================
//  Timer::Start
//============================================================================/
long Timer::Start (void)
{
#ifdef	_WIN32

#ifndef UNDER_CE
	ftime( &fStartTime );
	fStartUsage = clock();
#else // define UNDER_CE
//	GetSystemTime(&fStartTime);
	fStartTime = GetTickCount();
#endif // UNDER_CE

#else
    gettimeofday(&fStartTime, 0);
    getrusage(RUSAGE_SELF, &fStartUsage);
#endif

    fElapsedTime = fElapsedCPUTime = (float)0.0;
    return 0;
}

//============================================================================
//  Timer::Finish
//============================================================================/
long Timer::Finish (void)
{
#ifdef	_WIN32

#ifndef UNDER_CE
	ftime( &fEndTime );
	fEndUsage = clock();

	fElapsedTime = (float) (((float) ( fEndTime.time    - fStartTime.time ) +
				   ((float) ( fEndTime.millitm - fStartTime.millitm ) * 0.001 )));

	fElapsedCPUTime = (float) ( fEndUsage - fStartUsage ) / CLOCKS_PER_SEC;
#else // define UNDER_CE
//	GetSystemTime( &fEndTime );
	fEndTime = GetTickCount();
	fElapsedTime = (float)(fEndTime - fStartTime) * 0.001;
/*
	fElapsedTime = (float) (fEndTime.wHour - fStartTime.wHour) * SECONDS_PER_HOUR +
		(float) (fEndTime.wMinute - fStartTime.wMinute) * SECONDS_PER_MINUTE +
		(float) (fEndTime.wSecond - fStartTime.wSecond) +
		(float) (fEndTime.wMilliseconds - fStartTime.wMilliseconds) * 0.001;
*/
		fElapsedCPUTime = fElapsedTime;
#endif // UNDER_CE

#else
    gettimeofday(&fEndTime, 0);
    getrusage(RUSAGE_SELF, &fEndUsage);

    fElapsedTime = (float) (fEndTime.tv_sec - fStartTime.tv_sec)
      + ((float) (fEndTime.tv_usec - fStartTime.tv_usec)) * 1.0e-6;

    fElapsedCPUTime =
      (float)(fEndUsage.ru_utime.tv_sec - fStartUsage.ru_utime.tv_sec)
      + (float)(fEndUsage.ru_stime.tv_sec - fStartUsage.ru_stime.tv_sec)
      + ((float)(fEndUsage.ru_utime.tv_usec - fStartUsage.ru_utime.tv_usec)) * 1.0e-6
      + ((float)(fEndUsage.ru_stime.tv_usec - fStartUsage.ru_stime.tv_usec)) * 1.0e-6;
#endif

    return 0;
}

