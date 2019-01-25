/*
    File:   Memory.c
    Description:

*/

#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include "Memory.h"
//--------------------------------------------------------------------------------
void MemoryDelete (void * ptr)
{
	char * nptr = (char *) ptr;
	delete nptr;
}

/* #define TRACK */
static long gMemoryAllocated = 0;

/*****************************************************************************
 *
 *  MemoryAlloc(): Allocates a piece of memory.  Performs error checking
 * 
 *  Return value: pointer to memory
 *
 *  Globals Used: none
 *
 ***************************************************************************+*/
Ptr MemoryAlloc (long n)
{
    Ptr pb = (Ptr) malloc(n);
    
#ifdef TRACK
    gMemoryAllocated += n;
#endif
    if (pb == NULL)
      Error ("MemoryAlloc(%x): not enough memory", -1, "Unable to allocate \"%d\" Ptr units", n);
    
    return pb;
}


/*****************************************************************************
 *
 *  MemoryAllocZ(): Allocates a piece of memory, zeroed.  Performs error checking
 * 
 *  Return value: pointer to memory
 *
 *  Globals Used: none
 *
 ***************************************************************************+*/
Ptr MemoryAllocZ (long n)
{
    Ptr pb = (Ptr)calloc(n, 1);
#ifdef TRACK
    gMemoryAllocated += n;
#endif
    if (pb == NULL)
      Error ("MemoryAllocZ(%x): not enough memory", -1, "Unable to allocate \"%d\" Ptr units", n);

    return pb;
}


/*****************************************************************************
 *
 *  MemoryRealloc(): Grows a piece of memory.  Performs error checking
 *  Allocates a piece of memory, zeroed.  Performs error checking
 * 
 *  Return value: pointer to memory
 *
 *  Globals Used: none
 *
 ***************************************************************************+*/
Ptr MemoryRealloc (Ptr ptr, long n)
{
    Ptr pb = (Ptr) realloc(ptr, n);
    
#ifdef TRACK
    gMemoryAllocated += n;
#endif
    if (pb == NULL)
      Error ("MemoryRealloc(%x): not enough memory", -1, "Unable to allocate \"%d\" Ptr units", n);

    return pb;
}

/*****************************************************************************
 *
 *  MemoryFree(): Frees a piece of memory
 *
 *  Return Value: none
 *
 **************************************************************************+*/
void MemoryFree (Ptr ptr)
{
    if (ptr != NULL) {
#ifdef TRACK
	uns32 nfree = *((uns32 *)ptr-2);
	gMemoryAllocated -= nfree;
#endif
	free (ptr);
    }
}

/*****************************************************************************
 *
 *  MemoryUsage(): Returns # bytes allocated so far
 *
 **************************************************************************+*/
uns32 MemoryUsage(void)
{
    return gMemoryAllocated;
}


