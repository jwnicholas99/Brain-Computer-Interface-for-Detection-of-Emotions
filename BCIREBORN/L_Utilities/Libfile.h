/*
    File:   libfile.h

    Description:
*/


#ifndef _FILE_H_
#define _FILE_H_
#include "Error.h"
#include "Types.h"

extern FILE *   FileOpen(char *name, char *mode);
extern void FileClose(FILE *pf);
extern int  FileGetLine(FILE *pf, char *pbuf, size_t bufsize);
extern int  FileGetString(FILE *pf, char *pbuf, size_t bufsize);
extern void FileWrite(FILE *pf, void *pbuf, size_t nbytes);
typedef enum {RM_EOFOK,RM_ATEOF,RM_EOFBAD} READMODE;
extern size_t   FileRead(FILE *pf, void *addr, size_t nbytes, READMODE rm);
typedef enum {SM_BEGINNING,SM_CURRENT,SM_END} SEEKMODE;
extern void FileSeek(FILE *pf, long nbytes, SEEKMODE mode);
long FileReadData (FILE *pf, void * addr, size_t nbytes); 

void swap_2 (char *);
void swap_4 (char *);


//#ifdef _WIN32
#ifndef SWAP_BYTE

#define FileReadItem(fp, item, type) { 	FileRead((fp), (void *)&(item), sizeof(type), RM_EOFBAD); if ( sizeof( type ) == 2 ) swap_2((char *) &(item) ); else if ( sizeof( type ) == 4 ) swap_4((char *) &(item) ); }

#define FileReadItem2(fp, item) { FileRead((fp), (void *)&(item), sizeof(item), RM_EOFBAD); if ( sizeof( item ) == 2 ) swap_2((char *) &(item) ); else if ( sizeof( item ) == 4 ) swap_4((char *) &(item) ); }

#define FileWriteItem(fp, item, type) {	if ( sizeof( item ) == 2 ) 	swap_2((char *) &(item) ); else if ( sizeof( item ) == 4 ) swap_4((char *) &(item) ); FileWrite((fp), (void *)&(item), sizeof(type)); if ( sizeof( item ) == 2 ) swap_2((char *) &(item) ); else if ( sizeof( item ) == 4 ) swap_4((char *) &(item) ); }

#define FileWriteItem2(fp, item) { if ( sizeof( item ) == 2 ) swap_2((char *) &(item) ); else if ( sizeof( item ) == 4 ) swap_4((char *) &(item) ); FileWrite((fp), (void *)&(item), sizeof(item)); if ( sizeof( item ) == 2 ) swap_2((char *) &(item) ); else if ( sizeof( item ) == 4 ) swap_4((char *) &(item) ); }

#else

	#define FileReadItem(fp, item, type) FileRead((fp), (void *)&(item), sizeof(type), RM_EOFBAD)

	#define FileReadItem2(fp, item) FileRead((fp), (void *)&(item), sizeof(item), RM_EOFBAD)

	#define FileWriteItem(fp, item, type) FileWrite((fp), (void *)&(item), sizeof(type))

	#define FileWriteItem2(fp, item) FileWrite((fp), (void *)&(item), sizeof(item))

#endif	//	_WIN32


#endif /* _FILE_H_ */
