/*
    File:   file.c
    Description:

*/



/*****************************************************************************
 *
 *  FILE.C
 *
 *  Interface to ANSI file I/O routines
 *
 *
 *  Modified:
 *
 *  Functions:
 *    FileOpen            Establish access to a file
 *    FileClose           Discontinue access to a file
 *    FileRead            Read a block of data from a file
 *    FileWrite           Write a block of data to a file
 *    FileGetLine         Read a line from a text file
 *    FileExists          Detects the existence of a file
 *    PromptAndReadChar   Prompts the user and reads in a character
 *    PromptYesOrNo       Prompts and returns YES or NO
 *
 *****************************************************************************/

#include <stdio.h>
#include "Libfile.h"
#include "String.h"

/*****************************************************************************
 *
 *  FILE *FileOpen(char *name, char *mode) 
 *
 *  ARGUMENTS
 *    name        name of file to open
 *    mode        characters representing type of access desired
 *
 *  RETURN VALUE
 *    A handle for use with other file access routines.
 *
 *  DESCRIPTION
 *    See the description of fopen in the compiler documentation. The
 *    only difference is that instead of NULL being returned on failure,
 *    an error is reported.
 *
 ***************************************************************************+*/
FILE *FileOpen(char *name, char *mode)
{
    FILE *pf = NULL;

	fopen_s(&pf, name, mode);

	if (pf == NULL)
		ErrReport("FileOpen(%s,%s) - Failed", name, mode);

    return pf;
} /* end routine FileOpen() */


/*****************************************************************************
 *
 *  void FileClose(FILE *pf)
 *
 *  ARGUMENTS
 *    pf          file handle obtained via FileOpen
 *
 *  RETURN VALUE
 *    None
 *
 *  ERROR MESSAGES
 *   "File close problem"
 *
 *  FUNCTION DESCRIPTION
 *    See the description of fclose in the compiler documentation. The
 *    only difference is that instead of EOF being returned on failure,
 *    an error is reported.
 *
 ***************************************************************************+*/
void FileClose(FILE *pf) 
{
    if (fclose(pf))
        ErrReport("FileClose(%p) - Failed",pf);

} /* end routine FileClose() */


/*****************************************************************************
 *
 *  int FileGetLine(FILE *pf, char *pbuf, size_t bufsize)
 *
 *  ARGUMENTS
 *    pf          file handle obtained via FileOpen
 *    pbuf        address of a buffer to hold an input line
 *    bufsize     the size of this buffer (in bytes)
 *
 *  RETURN VALUE
 *    Either the number of characters (not including terminal null)
 *    read in, or the symbol EOF.
 *
 *  ERROR MESSAGES
 *    "File read problem"
 *
 *  DESCRIPTION
 *    This routine provides a robust mechanism for reading a line from a
 *    text file. No more than bufsize-1 characters will be read in from the
 *    file, and any newline character will be eaten. The number of characters
 *    read in will be returned (or EOF if at end of file). If the whole line
 *    doesn't fit, the file position will still be advanced past the end
 *    of the line.
 *
 ***************************************************************************+*/
int FileGetLine(FILE *pf, char *pbuf, size_t bufsize) 
{
    int ch;
    size_t n = 0;

    /* make robust */
    if (pbuf == NULL)
      bufsize = 0;

    /* get 1st character in case it's EOF - internal EOFs are
       handled differently */

    ch = getc(pf);

    if (ch == EOF)
      return EOF;

    while (ch != '\n') {
	/* if room in buffer, put character there */
	if (bufsize > 1) {
	    bufsize--;
	    n++;
	    *pbuf++ = ch;
	} else
		break;
	
	/* read in next character */
	ch = getc(pf);

	if (ch == EOF)
	  break;
    } /* end while (ch ... */

    /* terminate with 0 if any buffer was supplied */
    if (bufsize)
      *pbuf = '\0';

    /* return # of non-0 characters placed in buffer */
    return (int) n;
} /* end routine FileGetLine() */

/*****************************************************************************
 *
 *  int FileGetString(FILE *pf, char *pbuf, size_t bufsize)
 *
 *  ARGUMENTS
 *    pf          file handle obtained via FileOpen
 *    pbuf        address of a buffer to hold an input string
 *    bufsize     the size of this buffer (in bytes)
 *
 *  RETURN VALUE
 *    Either the number of characters (not including terminal null)
 *    read in, or the symbol EOF.
 *
 *  ERROR MESSAGES
 *    "File read problem"
 *
 *  DESCRIPTION
 *    This routine provides a robust mechanism for reading a string from a
 *    text file. No more than bufsize-1 characters will be read in from the
 *    file, and any newline character will be eaten. The number of characters
 *    read in will be returned (or EOF if at end of file). If the whole string
 *    doesn't fit, the file position will still be advanced past the end
 *    of the string.
 *
 ***************************************************************************+*/
int FileGetString(FILE *pf, char *pbuf, size_t bufsize) 
{
    int ch;
    size_t n = 0;

    if ((pbuf == NULL) || (bufsize == 0)) return 0;

    /* get 1st character in case it's EOF - internal EOFs are
       handled differently */

    ch = ' ';
    while (IsSpace(ch))
    {
        ch = getc(pf);
    }
    if (ch == EOF)  return 0;

    while (!IsSpace(ch) && (bufsize > 1))
    {
	    bufsize--;
	    n++;
	    *pbuf++ = ch;

        ch = getc(pf);
	    if (ch == EOF) break;
    } /* end while (ch ... */

    /* terminate with 0 if any buffer was supplied */
    if (bufsize) *pbuf = '\0';
    else
    {
        pbuf--;
        *pbuf = '\0';
    }

    /* return # of non-0 characters placed in buffer */
    return (int) n;
} /* end routine FileGetString() */


/*****************************************************************************
 *
 *  void FileWrite(FILE *pf, void *addr, size_t nbytes)
 *
 *  ARGUMENTS
 *    pf          file handle obtained via FileOpen
 *    addr        address of a data area
 *    nbytes      number of bytes in this area to write
 *
 *  RETURN VALUE
 *    None
 *
 *  ERROR MESSAGES
 *    "File write problem"
 *
 *  DESCRIPTION
 *    FileWrite will, if possible, write the <nbytes> bytes at <addr> to
 *    the file whose handle is <pf>. An error is reported on failure.
 *
 ***************************************************************************+*/
void FileWrite(FILE *pf, void *addr, size_t nbytes) 
{
    size_t nwritten;

    /* write 1 block of <nbytes> bytes */
    nwritten = fwrite(addr,nbytes,1,pf);

    /* fwrite returns the number of blocks written - should be 1 */
    if (nwritten != 1)
      ErrReport("FileWrite(%p,%p,0x%x) - Failed", pf, addr, nbytes);
} /* end routine FileWrite() */


/*****************************************************************************
 *
 *    size_t FileRead(FILE *pf, void *addr, size_t nbytes, READMODE rm)
 *
 *  ARGUMENTS
 *    pf          file handle obtained via FileOpen
 *    addr        address of a data area
 *    nbytes      number of bytes to read into this area
 *    rm          code saying what to do if end of file
 *
 *  RETURN VALUE
 *    None
 *
 *  ERROR MESSAGES
 *    "File read problem"
 *    "Unexpected end of file"
 *    "Unknown read mode"
 *
 *  DESCRIPTION
 *    FileRead will, if possible, read the <nbytes> bytes at <addr> from
 *    the file whose handle is <pf>. In that case, <nbytes> will be
 *    returned. If fewer than nbytes remain in the file at the time of
 *    the call, what happens depends on the value of <rm>:
 *
 *    If <rm> = RM_EOFOK, the actual # of bytes read in will be returned.
 *
 *    If <rm> = RM_ATEOF, then an error "Unexpected end of file" will be
 *    generated UNLESS the file was positioned at the end at the time of
 *    the call
 *
 *    IF <rm> = RM_EOFBAD, the error "Unexpected end of file" will be
 *    reported.
 *
 ***************************************************************************+*/
size_t FileRead(FILE *pf, void *addr, size_t nbytes, READMODE rm) 
{
    size_t nread;

    /* read <nbytes> blocks of 1 byte */
    nread = fread(addr,1,nbytes,pf);

    /* fread returns the number of blocks read - should be <nbytes> */
    if (nread < nbytes) {
	switch (rm) {
	  case RM_EOFOK:
	    break;
	  case RM_ATEOF:
	    if (nread == 0)
	      break;
	    /* else fall through */
	  case RM_EOFBAD:
	    ErrReport("FileRead(%p,%p,0x%x,RM_EOFBAD): Unexpected end of file",
		      pf,addr,nbytes);
	  default:
	    ErrReport("FileRead(%p,%p,0x%x,%d): Unknown read mode",
		      pf,addr,nbytes,rm);
	} /* end switch (rm) */
    } /* end if (nread ... */
    return nread;
} /* end routine FileRead() */


long FileReadData (FILE *pf, void * addr, size_t nbytes) 
{
    size_t nread;

    /* read <nbytes> blocks of 1 byte */
    nread = fread (addr,1,nbytes,pf);

	if (nread < nbytes)
		return -1;

#ifdef _WIN32
	if ( nbytes == 2 )
		swap_2((char *) addr );
	else if ( nbytes == 4 )
		swap_4((char *) addr );
#endif

	return 0;
}

/*****************************************************************************
 *
 *  void FileSeek(FILE *pf,long nbytes, SEEKMODE mode)
 *
 *  ARGUMENTS
 *    pf          file handle obtained via FileOpen
 *    nbytes      the number of bytes to move the file pointer
 *    mode        from where will the pointer be moved?
 *
 *  RETURN VALUE
 *    None
 *
 *  ERROR MESSAGES
 *    "File seek problem"
 *
 *  DESCRIPTION
 *    FileSeek will position the file access via the handle <pf> according
 *    to the arguments <nbytes> and <mode>. If <mode> is SM_BEGINNING,
 *    FileSeek will move <nbytes> from the beginning of the file. If <mode>
 *    is SM_CURRENT, the move is relative to the current file position.
 *    Finally, if <mode> is SM_END the move is relative to the end of
 *    the file (So <nbytes> should be less than or equal to 0 in this case!).
 *    If the seek is unsuccessful, an error is reported.
 * 
 ***************************************************************************+*/
void FileSeek(FILE *pf, long nbytes, SEEKMODE mode) 
{
    if (fseek(pf, nbytes, mode))
      ErrReport("FileSeek(%p,0x%lx,%d) - File seek problem",
                pf, nbytes, mode);
} /* end routine FileSeek() */


// swap functions
void swap_2 (char * v)
{
	char b0 = v[0];
	v[0] = v[1];
	v[1] = b0;
}

void swap_4 (char * v)
{
	char b0 = v[0];
	char b1 = v[1];

	v[0] = v[3];
	v[1] = v[2];
	v[2] = b1;
	v[3] = b0;
}

