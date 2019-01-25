/*

    Description:  Dynamic list class methods.

*/



#include "Stack16.h"


/*============================================================================
 *  Stack16::Stack16
 *===========================================================================*/
Stack16::Stack16 (long inCount, long growCount)
{
    fCount = 0;
    fRange = 0;
    fMaxCount = inCount;
    fGrowCount = growCount ? growCount : inCount;
    fStorage = NEW_ARRAYZ (Ptr16, inCount);
}

/*============================================================================
 *  Stack16::~Stack16
 *===========================================================================*/
Stack16::~Stack16 (void)
{
    MemoryFree (fStorage);
}

/*============================================================================
 *  Stack16::Clear
 *===========================================================================*/
void Stack16::Clear (void)
{
    fCount = 0;
    fRange = 0;
}

/*============================================================================
 *  Stack16::Push
 *===========================================================================*/
long Stack16::Push (Ptr16 item)
{
    if (fCount == fMaxCount) {
	fMaxCount += fGrowCount;
	GROW_ARRAY (fStorage, Ptr16, fMaxCount);
    }
    
    fStorage[fCount++] = item;
    fRange = fCount;
    
    return fCount;
}

/*============================================================================
 *  Stack16::Pop
 *===========================================================================*/
Ptr16 Stack16::Pop (void)
{
    Ptr16 item = NULL;
    
    if (fCount) {
	item = fStorage[--fCount];
	fRange = fCount;
    }

    return item;
}

/*============================================================================
 *  Stack16::Get
 *===========================================================================*/
Ptr16 Stack16::Get (long index)
{
    return (index < fCount && index >= 0? fStorage[index] : NULL);
}

/*============================================================================
 *  Stack16::Set
 *===========================================================================*/
void Stack16::Set (long index, Ptr16 item)
{
    if (index >= fMaxCount) {
	fMaxCount += index - fMaxCount + fGrowCount;
	GROW_ARRAY (fStorage, Ptr16, fMaxCount);
    }

    if (index >= fCount) {
	fCount++;
	fRange = index+1;
    }
    
    fStorage[index] = item;
}

/*============================================================================
 *  Stack16::Insert
 *===========================================================================*/
void Stack16::Insert (long index, Ptr16 item)
{
    if (index >= fMaxCount) {
	fMaxCount += index - fMaxCount + fGrowCount;
	GROW_ARRAY (fStorage, Ptr16, fMaxCount);
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
 *  Stack16::Copy
 *===========================================================================*/
Stack16 *  Stack16::Copy (Stack16 *  inList)
{
    Stack16 *  list = inList ? inList : new Stack16 (fGrowCount);
    
    for (long i = 0; i < fRange; i++) {
	list->Push (Get (i));
    }
    return list;
}

/*============================================================================
 *  Stack16::MemoryUsage
 *===========================================================================*/
long Stack16::MemoryUsage (void)
{
    return sizeof (this) + sizeof (Ptr16) * fMaxCount;
}
