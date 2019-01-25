/*
    Description: Lowest level definition file
*/


#ifndef		rDefs_H
#define		rDefs_H

#include        <limits.h>
#include        <math.h>
#include        "Types.h"
#include        "Memory.h"

#define PI 3.1415926535

enum {k128 = 128};
enum {k256 = 256};
enum {k512 = 512};
enum {k1K = 1024};
enum {k2K = 2048};
enum {k4K = 4069};
enum {k8K = 8192};
enum {k16K = 0x4fff};
enum {k64K = 0xffff};
enum {k128K = 0x2ffff};
enum {k256K = 0x4ffff};
enum {k1M = 0xfffff};

enum {kMaxReturnValue = 64};

/**************************************************************
 *
 *	Type defines
 */

const float kInfiniteLogProb = (float)(1.0e+32);
const float kMinusInfiniteLogProb = (float)(-1.0e+32);

/**************************************************************
 *
 *	Macros
 */
#define LOOP_IN_TIME(_idx, _start, _end, _backp) \
  for ((_idx) = ((_backp) ? (_end) : (_start)); \
       (_backp) ? (_idx) > (_start) : (_idx) < (_end); \
       (_backp) ? (_idx)-- : (_idx)++)

#define SWAP(x, y) { Stack* temp = x; x = y;  y = temp; }
#define SWAP16(x, y) { Stack16* temp = x; x = y;  y = temp; }
#define _getTimeDiff_(a,b)  (float)((1000.0*(float)(b.time-a.time)+((float)(b.millitm-a.millitm))))

#endif /* rDefs_H */
