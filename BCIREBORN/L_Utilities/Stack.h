/*
 File:   Stack.h
   
 Description:

 Stack class definition header file.

 This module contains the class definition and member function definitions
 for supporting dynamic list objects.

 */


#ifndef  rStack_H
#define  rStack_H
#include "ConstDef.h"

class Stack;
typedef Stack * rStackP;

const long kNoItem = -1;

// Class definition
class Stack {

  public:

    Stack (long inCount = 50, long growCount = 0);
    ~Stack (void);
    
    long     MemoryUsage (void);
    void     Clear (void);
    long     Push (Ptr item);
    Ptr      Pop (void);
    Ptr      Get (long index);
    void     Set (long index, Ptr item);
    void     Insert (long index, Ptr item);
    Stack * Copy (Stack * inList = NULL);

    Ptr      First (void) { return Get (0); }
    Ptr      Last (void) { return Get (fCount - 1); }
    long     GetLength (void) { return fCount; }
    long     GetCount (void) { return fCount; }
    Ptr *    GetStorage (void) { return fStorage; }
	     
  private:

    long     fCount;
    long     fMaxCount;
    long     fGrowCount;
    long     fRange;
    Ptr *    fStorage;
 
};


/*===============================================================================
  Public Functions

  Stack (long inCount, long growCount = 0)
    Constructor.  Makes an instance of an Stack and returns it.
    The initial size of the list as well as the grow size (for expanding
    the list) are user-specifiable.

  ~Stack (void)
    Destructor

  MemoryUsage (void)
    Returns the memory used by the dynamic list.
  
  Clear (void)
    Clears the list for reuse by reseting it's count index to 0.

  Copy (Stack * inList = NULL)
    Returns a copy of the list.  If the optional inList is provided, copies the list
    into inList, and returns it.
 
  Push (Ptr item)
    Pushes an item onto the end of the list.  Allocates more memory if necessary
    using fGrowCount.
   
  Pop (void)
    Pops an item off the end of the list, and returns.  The item will no longer
    be on the list.

  Get (long index)
    Returns the item at index.  If index is beyond range, returns kNoItem.

  Set (long index, Ptr item)
    Places the item in the list at location provided by index.

  Insert (long index, Ptr item)
    Inserts an item in the list.  Pushes other items down, allocating more
    memory if necessary.

  First (void)
    Returns the first item on the list.

  Last (void)
    Returns the last item on the list.

  Length (void)
    Returns the length of the list.

  Count (void)
    Same as Count ().

===============================================================================*/

#endif // rStack_H
