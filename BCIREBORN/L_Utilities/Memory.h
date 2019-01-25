/*
    File:   Memory.h
  
    Description:
*/


#ifndef Memory_h_
#define Memory_h_

#include "Error.h"
#include "Types.h"

extern Ptr MemoryAlloc (long n);
extern Ptr MemoryAllocZ (long n);
extern Ptr MemoryRealloc (Ptr p, long n);
extern Ptr MemoryReallocZ (Ptr p, long n);
extern void  MemoryFree (Ptr p);
extern uns32 MemoryUsage (void);

#define NEW(type) (type *)MemoryAlloc(sizeof(type))
#define NEWZ(type) (type *)MemoryAllocZ(sizeof(type))
#define NEW_ARRAY(type, n) (type *)MemoryAlloc((uns32)((n)*sizeof(type)))
#define NEW_ARRAYZ(type, n) (type *)MemoryAllocZ((uns32)((n)*sizeof(type)))
#define GROW_ARRAY(p, type, n)  (p=(type *)MemoryRealloc(p,(uns32)((n)*sizeof(type))))
#define GROW_ARRAYZ(p, type, n) (p=(type *)MemoryReallocZ(p,(uns32)((n)*sizeof(type))))

#define MemoryClear(p, n) memset((Ptr)(p), 0, (uns32)(n))
#define CLEAR_ARRAY(p, type, n) MemoryClear((p), (uns32)(sizeof(type)*(n)))
#define COPY_ARRAY(type,from,to,n) memcpy(to,from,(uns32)(n)*sizeof(type))
#endif // Memory_h_
