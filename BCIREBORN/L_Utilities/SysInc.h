//==========================================================
//
//	All system header files should be included here
//
//==========================================================

#ifndef _SYSTEM_INCLUDE_H_
#define _SYSTEM_INCLUDE_H_

#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <stdarg.h>
#include <math.h>
#include        <limits.h>
#include <ctype.h>

#ifndef UNDER_CE
#include <assert.h>
#include <sys/stat.h>
#include <sys/types.h>
#include <fcntl.h>
#endif


#ifdef		_WIN32		//	Windows	
#define		F_OK	02

#ifndef UNDER_CE
#include	<io.h>
#include <process.h>
#include <time.h>
#include <sys/timeb.h>
#include <direct.h>
#include <share.h>
#else
#include <winbase.h>
#endif

#include <windows.h>

#else			// Unix, Linux, Solaris
	
#include	<unistd.h>	
#include <pthread.h>
#include <sys/time.h>
#include <sys/resource.h>

#ifndef SOLARIS
#include <sys/io.h>
#endif

#endif


#ifdef _WIN32_WCE
// 

// Some useful routines that are missing in Windows CE SDK
extern "C"
{

	int         remove(const char *);
	int         rename(const char *, const char *);
	char *		getenv( const char *varname );
	int			putenv( const char *envstring );
}

#endif

#endif

