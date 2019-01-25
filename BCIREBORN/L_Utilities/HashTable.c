/*
    Description:  HashTable class method.

*/

#include "HashTable.h"

#ifdef	_WIN32			
#define	true	1		
#define	false	0
#endif

const long gMaxRand = 0xffffffff;

ulong NextPowerOf2 (ulong val, ulong *pshift);
ulong HashEval (uchar *pkey, size_t keysize);


/*---------------------------------------------------------------------------
 *  NextPowerOf2
 *---------------------------------------------------------------------------*/
ulong NextPowerOf2 (ulong val, ulong *pshift) 
{
    ulong shift = 0;
    while (val) {
	val >>= 1;
	shift++;
    } /* end while (val) */
    *pshift = shift;
    return (1 << shift);
} /* end routine NextPowerOf2() */

/*---------------------------------------------------------------------------
 *  HashEval
 *---------------------------------------------------------------------------*/
ulong HashEval (uchar *pkey, size_t keysize) 
{
#define ROL1(x) (((x)<<1)+!!((x)&0x80000000)) /*rotate left through carry*/
    static Boolean done = false;
    static ulong randtbl[256];

	ulong longrand;
    if (!done) {
	for (long i = 0; i < 256; i++) {
		longrand = rand();
		longrand <<= 16;
		longrand |= rand();
		randtbl[i] = longrand % gMaxRand;
	}
	done = true;
    }

    ulong val= randtbl[pkey[0]];
    for (ulong i = 1; i < keysize; i++) {
	val = ROL1(val) ^ randtbl[pkey[i]];
    }
    return val;
    
} /* end routine HashEval() */

/*---------------------------------------------------------------------------
 *  HashInsert2
 *---------------------------------------------------------------------------*/
void HashInsert2 (Ptr prec, Ptr secondArg) 
{
    HashTableP pht = (HashTableP) secondArg;
    pht->Insert (prec);
} /* end routine HashInsert2() */


/*============================================================================
 *  HashTable::HashTable
 *===========================================================================*/
HashTable::HashTable (ulong numSlots, size_t (*getkey)(Ptr, uchar **)) 
{
    fSize = 0;
    fTable = NULL;
    Initialize (numSlots, getkey);
}

/*============================================================================
 *  HashTable::HashTable
 *===========================================================================*/
HashTable::HashTable (void)
{
    fSize = 0;
    fTable = NULL;
}

/*============================================================================
 *  HashTable::~HashTable
 *===========================================================================*/
HashTable::~HashTable (void) 
{
	if (fTable)
		delete [] fTable;
}

/*============================================================================
 *  HashTable::Initialize
 *===========================================================================*/
void HashTable::Initialize (ulong numSlots, size_t (*getkey)(Ptr, uchar **)) 
{
    if (fSize == 0) {
        fSize = NextPowerOf2 (numSlots, &fShift);
		fMask = fSize - 1;
		fTable = new Ptr [fSize];
		fGetKey = getkey;
    }
    Clear ();
}

/*============================================================================
 *  HashTable::Clear
 *===========================================================================*/
void HashTable::Clear (void) 
{
    for (long i = 0; i < fSize; i++) {
	fTable[i] = NULL;
    }
    fNumEntries = 0;
    fNumSearches = fNumCompares = 0;
}

/*============================================================================
 *  HashTable::Grow
 *===========================================================================*/
void HashTable::Grow (void)
{
	HashTable * nt = new HashTable (fSize+2, fGetKey);

    Traverse (HashInsert2, nt);

	nt->SetNumEntries (fNumEntries);
	nt->SetNumSearches (fNumSearches);
	nt->SetNumCompares (fNumCompares);

	nt->Transfer (this);

    delete nt;
}

/*============================================================================
 *  HashTable::Transfer
 *===========================================================================*/
void HashTable::Transfer (HashTable * inTable)
{
	inTable->fSize = fSize;
	inTable->fMask = fMask;
	inTable->fShift = fShift;
	inTable->fNumEntries = fNumEntries;
	if (inTable->fTable) {
		delete inTable->fTable;
	}
	inTable->fTable = fTable;
	inTable->fNumSearches = fNumSearches;
	inTable->fNumCompares = fNumCompares;

	fTable = NULL;
}

/*============================================================================
 *  HashTable::Insert
 *===========================================================================*/
ulong HashTable::Insert (Ptr prec)
{
    long hindex;
    Ptr * phash;
    uchar *pkey;
    size_t keysize;

    if (fNumEntries == fSize) {
		PrintMsg ("HashTable::Insert: Table full (%d entries)", fNumEntries);
    }
    // if ((fNumEntries << 1) >= fSize)
    // Grow ();

    keysize = (*fGetKey) (prec,&pkey);

    hindex = HashEval (pkey,keysize) & fMask;
    phash = fTable + hindex;

    while (*phash != NULL) {
	if (++hindex == fSize) {
	    hindex = 0;
	    phash = fTable;
	} /* end if (++hindex ... */
	else phash++;
    } /* end while (*phash ... */

    *phash = prec;
    fNumEntries++;

    return hindex;
}

/*============================================================================
 *  HashTable::Find
 *===========================================================================*/
Ptr HashTable::Find (uchar *pkey, size_t keysize) 
{
    long hindex = HashEval (pkey,keysize) & fMask;
    Ptr * phash = fTable + hindex;

    fNumSearches++;

    while (*phash != NULL) {
	uchar *ptablekey;
	size_t tablekeysize;

	tablekeysize = (*fGetKey) (*phash,&ptablekey);

	fNumCompares++;

	if (tablekeysize == keysize && memcmp (ptablekey,pkey,keysize) == 0)
	    return *phash;

	if (++hindex == fSize) {
	    hindex = 0;
	}
	phash = fTable + hindex;
	
    } /* end while (*phash ... */

    return NULL;
}

/*============================================================================
 *  HashTable::FindIndex
 *===========================================================================*/
long HashTable::FindIndex (uchar *pkey, size_t keysize) 
{
    long hindex = HashEval (pkey,keysize) & fMask;
    Ptr * phash = fTable + hindex;

    fNumSearches++;

    while (*phash != NULL) {
	uchar *ptablekey;
	size_t tablekeysize;

	tablekeysize = (*fGetKey) (*phash,&ptablekey);

	fNumCompares++;

	if (tablekeysize == keysize && memcmp (ptablekey,pkey,keysize) == 0)
	    return hindex;

	if (++hindex == fSize) {
	    hindex = 0;
	} 
	phash = fTable + hindex;
	
    } /* end while (*phash ... */

    return -1;
}

/*============================================================================
 *  HashTable::Remove
 *===========================================================================*/
long HashTable::Remove (Ptr prec) 
{
    uchar *pkey;
    size_t keysize = (*fGetKey) (prec, &pkey);
    long index = FindIndex (pkey, keysize);
    if (index >= 0) {
        SetEntry (index, NULL);
	fNumEntries--;
    }
    return index;
}

/*============================================================================
 *  HashTable::Traverse
 *===========================================================================*/
void HashTable::Traverse (void (*pfun)(Ptr, Ptr), Ptr secondArg) 
{
    Ptr * phash;
    Ptr * pend = fTable + fSize;

    for (phash = fTable; phash < pend; phash++) {
	if (*phash != NULL)
	  (*pfun)(*phash,secondArg);
    } /* end for (phash ... */
}

/*============================================================================
 *  HashTable::MemoryUsage
 *===========================================================================*/
long HashTable::MemoryUsage (void)
{
    long usage = sizeof (*this);
    usage += fSize * sizeof (Ptr);
    return usage;
}

