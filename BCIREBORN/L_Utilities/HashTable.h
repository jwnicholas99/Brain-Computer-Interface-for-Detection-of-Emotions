/*
    File:   HashTable.h
   
    Description:  HashTable class method definitions.

*/

#ifndef	HashTable_H
#define	HashTable_H


#include "ConstDef.h"
#include "Error.h"

class HashTable;
typedef HashTable * HashTableP;

class HashTable {

  public:
    
    HashTable (void);
    HashTable (ulong numSlots, size_t (*getkey)(Ptr, uchar **));
    ~HashTable (void);
    
    void    Initialize (ulong numSlots, size_t (*getkey)(Ptr, uchar **)) ;
    void    Clear (void);
    void    Grow (void);
    ulong   Insert (Ptr prec);
    Ptr     Find (uchar *pkey, size_t keysize); 
    long    FindIndex (uchar *pkey, size_t keysize);
    long    Remove (Ptr prec);
    void    Traverse (void (*pfun)(Ptr, Ptr), Ptr secondArg);
    long    MemoryUsage (void);

    long    GetSize () { return fSize; }	    
    long    GetNumEntries () { return fNumEntries; }	    
    long    GetNumSearches () { return fNumSearches; }
    long    GetNumCompares () { return fNumCompares; }
    Ptr     GetEntry (long index) { return fTable[index]; }
    void    SetEntry (long index, Ptr obj) { fTable[index] = obj; }
	void	SetNumEntries (long n) { fNumEntries = n; }
   	void	SetNumSearches (long n) { fNumSearches = n; }
    void	SetNumCompares (long n) { fNumCompares = n; }

  private:
    
    long    fSize;
    long    fNumEntries;
    ulong   fShift;
    ulong   fMask;
    size_t  (*fGetKey) (Ptr, uchar **);
    Ptr *   fTable;
    long    fNumSearches;
    long    fNumCompares;

	void	Transfer (HashTable * inTable);
	    
};

/*===============================================================================
  Public Functions

  HashTable (ulong inNumSlots, size_t (*getkey)(Ptr, uchar **))
    Constructor.  Makes an instance of HashTable class, given the number of slots
    and a "get key" function.  The get key function specifies how a key is derived
    for an object that is stored in the hash table.

  
  ~HashTable (void)
    Destructor.   
    
  Clear (void)
    Clears a hash table for reuse.  This includes its contents, and all the internal
    indices.
    
  Insert (Ptr prec)
    Inserts an object in the hash table.

  Find (uchar *pkey, size_t keysize)
    Finds and returns the object in the hash table, using the key.

  Traverse (void (*pfun)(Ptr, Ptr), Ptr secondArg)
    Maps a function call to every object in the table. 

  long GetNumEntries ()
    Returns the number of entries in the hash table. 
  
================================================================================*/

#endif	/* HashTable_H */
