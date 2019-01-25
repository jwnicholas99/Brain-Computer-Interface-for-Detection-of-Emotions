//----------------------------------------------------------------------------
//    File:   Utilities.h
//----------------------------------------------------------------------------

#ifndef Utilities_h
#define Utilities_h
// The debugger can't handle symbols more than 255 characters long.
// STL often creates symbols longer than that.
// When symbols are longer than 255 characters, the warning is disabled.
#pragma warning(disable:4786)
#include "ConstDef.h"
#include "Stack.h"
#include "Stack16.h"
#include "MemMgr.h"
#include "Error.h"
#include "File.h"
#include "HashTable.h"
#include "Heap.h"
#include "Libfile.h"
#include "Resources.h"
#include "String.h"
#include "Timer.h"
#include "Types.h"


#endif // Utilities_h
