/*
    Description:  Heap class method definitions.

*/

#include "Heap.h"


/*============================================================================
 *  Heap::Heap
 *===========================================================================*/
Heap::Heap (long size, long (*compareFunc) (Ptr, Ptr, Ptr), Ptr thirdArg)
{
    fSize = size;
    fNumEntries = 0;
    fGrowSize = size;
    fCompareFunc = compareFunc;
    fEntries = NEW_ARRAYZ (Ptr, size+1);
    fThirdArg = thirdArg;
    
}

/*============================================================================
 *  Heap::~Heap
 *===========================================================================*/
Heap::~Heap (void)
{
    MemoryFree (fEntries);
}

/*============================================================================
 *  Heap::Clear
 *===========================================================================*/
void Heap::Clear (void)
{
    fNumEntries = 0;
}

/*============================================================================
 *  Heap::Grow
 *===========================================================================*/
void Heap::Grow (void)
{
    fSize += fGrowSize;
    GROW_ARRAY (fEntries, Ptr, fSize+1);
}

/*============================================================================
 *  Heap::Insert
 *===========================================================================*/
void Heap::Insert (Ptr entry)
{
    Ptr * entries;
    
    if (fNumEntries == fSize)
      Grow ();

    entries = fEntries;

    long j = ++(fNumEntries);
    long i = fNumEntries >> 1;

    while (i > 0 && (*fCompareFunc)(entry, entries[i], fThirdArg) > 0) {
		entries[j] = entries[i];
		j = i; i >>= 1;
    }

    entries[j] = entry;

}

/*============================================================================
 *  Heap::Adjust
 *===========================================================================*/
void Heap::Adjust (long position)
{
    long numEntries = fNumEntries;
    Ptr * entries = fEntries;
    long (*compareFunc) (Ptr, Ptr, Ptr) = fCompareFunc;

    long j = 2 * position;
    Ptr entry = entries[position];

    while (j <= numEntries) {
	if (j < numEntries && (*compareFunc)(entries[j+1], entries[j], fThirdArg) > 0) {
	    j++;
	}

	if ((*compareFunc)(entry, entries[j], fThirdArg) >= 0) {
	    break;
	}

	entries[j >> 1] = entries[j];
	j <<= 1;
    }
    
    entries[j >> 1] = entry;
}

/*============================================================================
 *  Heap::Pop
 *===========================================================================*/
Ptr Heap::Pop (void)
{
    Ptr item = NULL;

    if (fNumEntries) {
	item = fEntries[1];
	fEntries[1] = fEntries[fNumEntries];
	fNumEntries--;
    
	Adjust (1);
    }

    return item;
}

/*============================================================================
 *  Heap::Tranverse
 *===========================================================================*/
void Heap::Traverse (void (*func)(Ptr, Ptr), Ptr secondArg)
{
    for (long i = 1; i <= fNumEntries; i++) {
	(*func) (fEntries[i], secondArg);
    }
}

/*============================================================================
 *  Heap::MemoryUsage
 *===========================================================================*/
long Heap::MemoryUsage (void)
{
    long usage = sizeof (this);
    usage += sizeof (Ptr) * (fSize + 1);
    return usage;
}
