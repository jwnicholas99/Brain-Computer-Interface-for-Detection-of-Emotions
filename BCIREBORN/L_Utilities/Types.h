//----------------------------------------------------------------------------    File:   Types.h
//    Description: C type definitions.
//----------------------------------------------------------------------------

#ifndef Types_H
#define Types_H

#include "SysInc.h"

typedef unsigned char ** Handle;
typedef short Boolean;

typedef char * StringPtr;
typedef char Str255[255];


#define ABS(x) ((x)>=0?(x):-(x))
#define	MIN(a,b) (((a)<(b))?(a):(b))
#define	MAX(a,b) (((a)>(b))?(a):(b))
#define FOREVER for(;;)

typedef unsigned char uns8;
typedef unsigned char uchar;
typedef short int16;
typedef unsigned short uns16;
typedef unsigned short ushort;
typedef long int32;
typedef unsigned long uns32;
typedef unsigned long ulong;
enum {kMaxString = 1024};
enum {kMaxLine = 1024};
enum {kMaxName = 512};
enum {kMaxRule = 64};

#define MMFOURCC( ch0, ch1, ch2, ch3 ) ((uns32)(char)(ch0) | ( (uns32)(char)(ch1) << 8 ) |	((uns32)(char)(ch2) << 16 ) | ((uns32)(char)(ch3) << 24 ) )
#define REVUNS32( data ) ((uns32)(uns8)(data >> 24) | ((uns32)(uns8)(data >> 16) << 8) | ((uns32)(uns8)(data >> 8) << 16) | ((uns32)(uns8)(data) << 24))

#ifndef _WIN32
#include <windows.h>
typedef int boolean;
#define stricmp strcasecmp 
#define Sleep(dwMilliseconds) sleep(dwMilliseconds/1000)
#endif

typedef void * PTR;
typedef void * Ptr;
typedef	unsigned short Ptr16;

typedef long (*FuncPtr)();

#define DELETE_POINTER(_ptr_)	if (_ptr_)  { delete (_ptr_);	_ptr_ = NULL;}
#define WHITESPACE                    " \t\n\r\b"
#endif /* Types_H */
