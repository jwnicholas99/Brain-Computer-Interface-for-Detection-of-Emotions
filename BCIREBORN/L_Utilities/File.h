// FileName:
//		File.h
//
// Purpose:
//		File Service Packages, based on stream
//////////////////////////////////////////////////////////////////////////////


#ifndef FILE_H
#define FILE_H

#include "SysInc.h"

#include "Types.h"
#include "String.h"
enum { kMaxMagicLen = 64 };
#define kTable   "  "
#define kNewLine "\n"

#ifndef _WIN32_WCE
bool    MakeDirectory(char *szDirPath);
#endif

time_t  GetModificationTime(char *szFileName);
bool    IsFileNameIdentical(char *szFileName1, char *szFileName2);
bool    FileExists(char *szPathName);
bool    GetDirectory(const char *szFullPath, char *outDirectory, int len);
bool    GetFileName(char *szFullPath, char *szFileName);
bool    CopyOneFile(char *szSourceFile, char *szDestFile);
char   *GetFileVersion(char *pszFilename);
class File 
{
public:
    File(void);
    ~File();
    bool       Open(const char *szFileName, char *szMode, char *szMagic = NULL, char encode = 0);
    void       Close(void);
    size_t     ReadString(char* Buffer, size_t nSize);
    bool       Read(uns32 &value);
    bool       Write(uns32 value);
    bool       Read(long &value);
    bool       Write(long value);
    bool       Read(int &value);
    bool       Write(int value);
    bool       Read(uns16 &value);
    bool       Write(uns16 value);
    bool       Read(short &value);
    bool       Write(short value);
    bool       Read(uns8 &value);
    bool       Write(uns8 value);
	bool       Read(char &value);
    bool       Write(char value);
    bool       Read(float &value);
    bool       Write(float value);
    size_t     WriteString(char* Buffer);
    bool       ReadLine(char *Buffer, size_t nSize);
    void       ImportHandle(FILE* hFile, char* szMode);
    FILE *     GetFileHandle(void) { return m_hFile; };
    size_t     GetPosition(void);
    bool       SetPosition(long lPosition, int nOrigin = SEEK_SET);
    size_t     GetFileSize(void);
    bool       IsEOF();
    bool       FindSection (char *section);
    bool       GetLineInSection(char *outLine, size_t nSize);
    bool       GetLine(char *outLine, size_t nSize);
    bool       PutLine(char *inLine = NULL);
    uns32      CountLinesInSection(char *section);
    char*      GetFileName(void);
    bool       IsTextMode(void);
    size_t     ReadData(void *Buffer, size_t nSize, bool encode = true);
    size_t     WriteData(void *Buffer, size_t nSize, bool encode = true);
    void       SetSeparator(char inSeparator);
    uns32      GetLineNumber(void);
	uns32      SetLineNumber(uns32 linenumber);
private:
	bool       RemoveNewLine(char *Buffer);
    void       Clear(void);
	template <size_t _size>
    void       DecimalToString(char (&outString)[_size], uns32 value);
	template <size_t _size>
    void       DecimalToString(char (&outString)[_size], int value);
    char       m_Mode[4];
    char       m_Section[64];
    char       m_Code;
    FILE      *m_hFile;
    char      *m_FileName;
    char       m_Separator;
    uns32      m_LineCount;
	size_t     m_HeadPosition;
	int 	   m_CodeCount;
};

#endif // File_h



