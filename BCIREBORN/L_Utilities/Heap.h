/*
 File:   Heap.h
   
 Description:
 
 Heap class definition header file.
*/


#ifndef	Heap_H
#define	Heap_H

#include "ConstDef.h"

class Heap;

typedef Heap * HeapP;

class Heap {
    
  public:

    Heap (long size, long (*compareFunc) (Ptr, Ptr, Ptr), Ptr thirdArg);
    ~Heap (void);
    
    void   Clear (void);
    void   Insert (Ptr entry);
    Ptr    Pop (void);
    void   Adjust (long position);
    void   Traverse (void (*func)(Ptr , Ptr), Ptr secondArg);
    long   MemoryUsage (void);
   
    long   GetNumEntries () { return fNumEntries;}
    Ptr    GetEntry (int i) { return i <= fNumEntries ? fEntries[i] : NULL; }
    void   SetEntry (int i, Ptr entry) { fEntries[i] = entry; }

  private:
    
    long    fSize;
    long    fNumEntries;
    long    fGrowSize;
    Ptr *   fEntries;
    long   (*fCompareFunc)(Ptr, Ptr, Ptr);
    Ptr     fThirdArg;
    
    void   Grow (void);
};


/*===============================================================================
  Public Functions

  Heap (long size, long (*compareFunc) (Ptr, Ptr, Ptr), Ptr thirdArg)
    Constructor.  Makes and returns an instance of a Heap (as in heap sort).  
    Entries in a heap are maintained in a partially sorted stack.

  ~Heap (void)
    Destructor.

  Insert (Ptr entry)
    Inserts an item onto the heap.  The heap will get adjusted accordingly to
    maintain the partial ordering.

  Pop (void)
    Pops the top-most entry (according the prespecified criteria) from the heap.

  Traverse (void (*func)(Ptr, Ptr), Ptr secondArg)
    Maps a function to every element of the heap.
   
  NumEntries ()
    Returns the number of entries in the heap.

================================================================================*/
#endif	/* Heap_H */
