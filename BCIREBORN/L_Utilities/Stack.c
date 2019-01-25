/*
    Description:  Dynamic list class methods.
*/



#include "Stack.h"


/*============================================================================
 *  Stack::Stack
 *===========================================================================*/
Stack::Stack (long inCount, long growCount)
{
    fCount = 0;
    fRange = 0;
    fMaxCount = inCount;
    fGrowCount = growCount ? growCount : inCount;
    fStorage = NEW_ARRAYZ (Ptr, inCount);
}

/*============================================================================
 *  Stack::~Stack
 *===========================================================================*/
Stack::~Stack (void)
{
    MemoryFree (fStorage);
}

/*============================================================================
 *  Stack::Clear
 *===========================================================================*/
void Stack::Clear (void)
{
    fCount = 0;
    fRange = 0;
}

/*============================================================================
 *  Stack::Push
 *===========================================================================*/
long Stack::Push (Ptr item)
{
    if (fCount == fMaxCount) {
	fMaxCount += fGrowCount;
	GROW_ARRAY (fStorage, Ptr, fMaxCount);
    }
    
    fStorage[fCount++] = item;
    fRange = fCount;
    
    return fCount;
}

/*============================================================================
 *  Stack::Pop
 *===========================================================================*/
Ptr Stack::Pop (void)
{
    Ptr item = NULL;
    
    if (fCount) {
	item = fStorage[--fCount];
	fRange = fCount;
    }

    return item;
}

/*============================================================================
 *  Stack::Get
 *===========================================================================*/
Ptr Stack::Get (long index)
{
    return (index < fCount && index >= 0? fStorage[index] : NULL);
}

/*============================================================================
 *  Stack::Set
 *===========================================================================*/
void Stack::Set (long index, Ptr item)
{
    if (index >= fMaxCount) {
	fMaxCount += index - fMaxCount + fGrowCount;
	GROW_ARRAY (fStorage, Ptr, fMaxCount);
    }

    if (index >= fCount) {
	fCount++;
	fRange = index+1;
    }
    
    fStorage[index] = item;
}

/*============================================================================
 *  Stack::Insert
 *===========================================================================*/
void Stack::Insert (long index, Ptr item)
{
    if (index >= fMaxCount) {
	fMaxCount += index - fMaxCount + fGrowCount;
	GROW_ARRAY (fStorage, Ptr, fMaxCount);
    }

    if (index < fCount) {
	for (long i = fCount; i > index; i--)
	  fStorage[i] = fStorage[i-1];
    }
    
    fCount++;
    fRange = MAX (fCount, index+1);
    
    fStorage[index] = item;
}

/*============================================================================
 *  Stack::Copy
 *===========================================================================*/
rStackP Stack::Copy (rStackP inList)
{
    rStackP list = inList ? inList : new Stack (fGrowCount);
    
    for (long i = 0; i < fRange; i++) {
	list->Push (Get (i));
    }
    return list;
}

/*============================================================================
 *  Stack::MemoryUsage
 *===========================================================================*/
long Stack::MemoryUsage (void)
{
    return sizeof (this) + sizeof (Ptr) * fMaxCount;
}
