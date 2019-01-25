/*
    File:   MemMgr.h
  
    Description:  memory manager
*/


#ifndef MemMgr_H
#define MemMgr_H

#include "ConstDef.h"
#include "Stack.h"
typedef long ObjectHndl;

template <class Type>
class MemMgr 
{
public:

	  MemMgr (long blockCount, Boolean recycle = false);
      ~MemMgr (void);
      
      void       Clear (void);
      ObjectHndl NewObject (Type ** object);
      Type *     NewObject (void);
	  Type *	 NewBlock (long size);
      void       FreeObject (Type * object);
	  void		 FreeBlock (Type * object, long size);
      Type *     GetObjectNode (ObjectHndl objHndl);
      ObjectHndl GetObjectHndl (Type * object);
      long       MemoryUsage (void);
	  float		 TimeUsage (void);
      
      long       GetCount (void) { return fCount; }
      bool       SetCount (long newCount); 
      long       GetFreeCount (void) { return fRecycleBin ? fRecycleBin->GetLength () : 0; }
	     
protected:     
      long     fCount;
      long     fGrowCount;
      long     fRecordIndex;
	  long     fBlockIndex;
	  long     fBlockCount;
      long     fObjectSize;
      Type  **  fRecords;
  	  Stack * fBlockTable;
      Stack * fRecycleBin;
};

/*============================================================================
 *  MemMgr::MemMgr
 *===========================================================================*/
template <class Type>
MemMgr<Type>::MemMgr (long blockCount, Boolean freeList)
{
	fGrowCount = blockCount;
    fCount = 0;
    fRecycleBin = NULL;
    fRecordIndex = 0;
	fBlockIndex = 0;
	fBlockCount = 0;
	fBlockTable = new Stack (32);

    if (freeList)
    {
        fRecycleBin = new Stack (fGrowCount);
    }

    if (fGrowCount) 
    {
        Type * fBlock = new Type [fGrowCount];
        if (!fBlock)
        {
	        Error ("MemMgr()", -1, "errCannotAllocateMemory");
        }
        else
        {
            fBlockTable->Push((Ptr)fBlock);
			fBlockCount++;
        }
    }
    fObjectSize = sizeof (Type);
}

/*============================================================================
 *  MemMgr::~MemMgr
 *===========================================================================*/
template <class Type>
MemMgr<Type>::~MemMgr (void)
{
    Clear();
    if (fRecycleBin) delete fRecycleBin;
	for (long blkIndex = 0; blkIndex < fBlockCount; blkIndex++)
	{
        Type * fBlock = (Type *)fBlockTable->Get(blkIndex);
		delete [] fBlock;
	}
	fBlockTable->Clear();
	delete fBlockTable;
}

/*============================================================================
 *  MemMgr::Clear
 *===========================================================================*/
template <class Type>
void MemMgr<Type>::Clear (void)
{
	fCount = 0;
    fRecordIndex = 0;
    fBlockIndex = 0;
    if (fRecycleBin)
    {
        fRecycleBin->Clear ();
    }
}

/*============================================================================
 *  MemMgr::NewObject
 *===========================================================================*/
template <class Type>
ObjectHndl MemMgr<Type>::NewObject (Type ** object)
{
    ObjectHndl hndl = -1;
    if (object)	*object = NULL;

    Type * fBlock = NULL;
	if (fRecordIndex == fGrowCount) 
    {
		fRecordIndex = 0;
		fBlockIndex++;
        if (fBlockIndex == fBlockCount)
        {
            fBlock = new Type [fGrowCount];
            if (!fBlock)
            {
	            Error ("MemMgr()", -1, "errCannotAllocateMemory");
            }
            else
            {
                fBlockTable->Push((Ptr)fBlock);
				fBlockCount++;
            }
        }
	}
    fBlock = (Type *)fBlockTable->Get(fBlockIndex);
	if (object && fBlock) 
    {
		*object = fBlock + fRecordIndex;
        fRecordIndex++;
		hndl = fCount++;
	}

    return hndl;
}

/*============================================================================
 *  MemMgr::NewObject
 *===========================================================================*/
template <class Type>
Type * MemMgr<Type>::NewObject (void)
{
    Type * object;
    
    if (fRecycleBin && fRecycleBin->GetLength ()) 
    {
	    object = (Type *) fRecycleBin->Pop ();
    }
    else 
    {
	    NewObject (&object);
    }
    return object;
}

/*============================================================================
 *  MemMgr::NewObject
 *===========================================================================*/
template <class Type>
Type * MemMgr<Type>::NewBlock (long size)
{
    ObjectHndl hndl = -1;
	Type *object = NULL;

	if ( size > fGrowCount ) {
		Error ("MemMgr()", -1, "errSizeLargerThanGrowCount");
		return 0;
	}

	Type * fBlock = (Type *)fBlockTable->Get(fBlockIndex);
	if (fRecordIndex + (size-1) >= fGrowCount) 
    {
		fRecordIndex = 0;
		fBlockIndex++;

		for ( long i=0; i<size; i++ ) {
			FreeObject(fBlock + fRecordIndex + i);
		}

        if (fBlockIndex == fBlockCount)
        {
            fBlock = new Type [fGrowCount];
            if (!fBlock)
            {
	            Error ("MemMgr()", -1, "errCannotAllocateMemory");
            }
            else
            {
                fBlockTable->Push((Ptr)fBlock);
				fBlockCount++;
            }
        }
	}

	fBlock = (Type *)fBlockTable->Get(fBlockIndex);
	if (fBlock) 
    {
		object = fBlock + fRecordIndex;
        fRecordIndex += size;
		fCount += size;
	}

    return object;
}

/*============================================================================
 *  MemMgr::FreeBlock
 *===========================================================================*/
template <class Type>
void MemMgr<Type>::FreeBlock (Type * object, long size)
{
	Type * obj = object;
    if (fRecycleBin) 
    {
		for ( long i=0; i<size; i++ ) {
			fRecycleBin->Push ((Ptr) obj);
			obj++;
		}
    }
}

/*============================================================================
 *  MemMgr::FreeObject
 *===========================================================================*/
template <class Type>
void MemMgr<Type>::FreeObject (Type * object)
{
    if (fRecycleBin) 
    {
        fRecycleBin->Push ((Ptr) object);
    }
}

/*============================================================================
 *  MemMgr::GetObjectNode
 *===========================================================================*/
template <class Type>
Type * MemMgr<Type>::GetObjectNode (ObjectHndl hndl)
{
    Type * object = NULL;
    
    if (hndl < fCount)
	{
		long recIndex = hndl % fGrowCount;
		long blkIndex = hndl / fGrowCount;
        if (blkIndex < fBlockCount)
        {
            Type * fBlock = (Type *)fBlockTable->Get(blkIndex);
		    object = fBlock + recIndex;
        }
	}
    return object;
}

/*============================================================================
 *  MemMgr::GetObjectHndl
 *===========================================================================*/
template <class Type>
ObjectHndl MemMgr<Type>::GetObjectHndl (Type * object)
{
    long recIndex;
    long blkIndex = 0;
    ObjectHndl hndl	= -1;
    while (blkIndex < fBlockCount)
	{
        Type * fBlock = (Type *)fBlockTable->Get(blkIndex);
		if ((object >= fBlock) && (object < fBlock + fGrowCount))
        {
            recIndex = object - fBlock;
            hndl = fGrowCount * blkIndex + recIndex;
			break;
        }
        blkIndex++;
    }
    return hndl;
}

/*============================================================================
 *  MemMgr::ResetIndices
 *===========================================================================*/
template <class Type>
bool MemMgr<Type>::SetCount(long newCount)
{
	long recIndex = newCount % fGrowCount;
	long blkIndex = newCount / fGrowCount;
    if (blkIndex < fBlockCount)
    {
        fCount = newCount;
        fBlockIndex =  blkIndex;
	    fRecordIndex = recIndex;
        return true;
    }
    else
    {
        return false;
    }
}

/*============================================================================
 *  MemMgr::MemoryUsage
 *===========================================================================*/
template <class Type>
long MemMgr<Type>::MemoryUsage (void)
{
    long usage = sizeof (*this);
    long blkIndex = 0;
    while (blkIndex < fBlockCount)
    {
        usage += fObjectSize * fGrowCount;
        blkIndex++;
    }
    usage += fBlockTable->MemoryUsage ();
    if (fRecycleBin)
    {
        usage += fRecycleBin->MemoryUsage ();
    }
    return usage;
}

/*============================================================================
 *  MemMgr::TimeUsage
 *===========================================================================*/
template <class Type>
float MemMgr<Type>::TimeUsage (void)
{
	return 0.0;
}

#endif /* MemMgr_H */
