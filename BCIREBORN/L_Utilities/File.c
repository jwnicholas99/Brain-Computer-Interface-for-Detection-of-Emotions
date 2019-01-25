// FileName:
//		File.c
//
// Purpose:
//		File Service Packages, based on stream
//
//////////////////////////////////////////////////////////////////////////////
#include "File.h"
#include "String.h"

#define kDelimiter " \t\n\r,;()"
#define kNewLine   "\n"

#ifdef SWAP_BYTE
// swap byte order for value
void SwapByteOrder(unsigned char* p, int Len)
{
	unsigned char temp;
	if (Len == 2)
	{
		temp = p[0];
		p[0] = p[1];
		p[1] = temp;
	}
	else if (Len == 4)
	{
		temp = p[0];
		p[0] = p[3];
		p[3] = temp;
		temp = p[1];
		p[1] = p[2];
		p[2] = temp;
	}
	else if (Len == 8)
	{
		temp = p[0];
		p[0] = p[7];
		p[7] = temp;
		temp = p[1];
		p[1] = p[6];
		p[6] = temp;
		temp = p[2];
		p[2] = p[5];
		p[5] = temp;
		temp = p[3];
		p[3] = p[4];
		p[4] = temp;
	}
}
#endif

//////////////////////////////////////////////////////////////////////////////
//
//  Function:		FileExists(char *szPathName)
//
//  Description:	Determine whether the file exists or not
//  
//////////////////////////////////////////////////////////////////////////////
bool FileExists(char *szPathName)
{
#ifndef UNDER_CE
    if (_access(szPathName, 0) == 0)
    {
        return true;
    }
    else
    {
        return false;
    }
#else
	unsigned short* pName = AsciiToUnicode(szPathName);
	long lRet = GetFileAttributes(pName);
	delete [] pName;
	if (lRet == -1)
		return false;
	else return true;
#endif
}

#ifndef _WIN32_WCE
//////////////////////////////////////////////////////////////////////////////
//
//  Function:		MakeDirectory(char *szDirPath)
//
//  Description:	Make the directory based on the full path
//  
//////////////////////////////////////////////////////////////////////////////
bool MakeDirectory(char *szDirPath)
{
    char szPath[_MAX_PATH+1];
    int errCode;
    if (strlen(szDirPath) >= _MAX_PATH) return false;
    for (size_t i = 0; i <= strlen(szDirPath); i++)
    {
        szPath[i] = szDirPath[i];
        szPath[i+1] = 0;
        if ((szPath[i] == '\\') || (szPath[i] == '/') || (szPath[i] == 0))
        {
            if (_access(szPath, 0) != 0)
            {
#ifdef WIN32
                errCode = _mkdir(szPath);
#else
				errCode = mkdir(szPath, S_IRWXU | S_IRWXG | S_IRWXO);
#endif
            }
            else
            {
                errCode = 0;
            }
        }
    }
    if (errCode == 0) 
    {
        return true;
    }
    else
    {
        return false;
    }
}
#endif

//////////////////////////////////////////////////////////////////////////////
//
//  Function:		GetDirectory(char *szFullPath, char *outDirectory)
//
//  Description:	Get the directory in the full path
//  
//////////////////////////////////////////////////////////////////////////////
bool GetDirectory(const char *szFullPath, char *outDirectory, int len)
{
    if ((szFullPath == NULL) || (outDirectory == NULL) || (strlen(szFullPath) >= _MAX_PATH))
    {
        return false;
    }
	strcpy_s(outDirectory, len, szFullPath);
	int i;
    for (i = (int) strlen(outDirectory); i >= 0; i--) 
    {
        if ((outDirectory[i] == '\\') || (outDirectory[i] == '/') || (outDirectory[i] == ':'))
        {
			break;
        }
    }
    outDirectory[i] = 0;
    return true;
}

//////////////////////////////////////////////////////////////////////////////
//
//  Function:		GetFileName(char *szFileName)
//
//  Description:	Get the filename in the full path
//  
//////////////////////////////////////////////////////////////////////////////
bool GetFileName(char *szFullPath, char *outFileName, int len)
{
    if ((!szFullPath) || (!outFileName) || (strlen(szFullPath) >= _MAX_PATH))
    {
        return false;
    }
	int i;
    for (i = (int) strlen(szFullPath); i >= 0; i--) 
    {
        if ((szFullPath[i] == '\\') || (szFullPath[i] == '/') || (szFullPath[i] == ':'))
        {
            break;
        }
    }
	strcpy_s(outFileName, len, &szFullPath[i+1]);

    return true;
}

#ifndef _WIN32_WCE 
//////////////////////////////////////////////////////////////////////////////
//
//  Function:		GetModificationTime(char *szFileName)
//
//  Description:	Get the file modification time
//  
//////////////////////////////////////////////////////////////////////////////
time_t GetModificationTime(char *szFileName)
{
	time_t m_time = 0;
	struct stat state;
	if (stat(szFileName, &state) == 0)
	{
		m_time = state.st_mtime;
	}
	return m_time;
}
#else
time_t GetModificationTime(char *szFileName)
{
	return 0;
}
#endif

//////////////////////////////////////////////////////////////////////////////
//
//  Function:		CopyOneFile(char *szSourceFile, char *szDestFile)
//
//  Description:	Copy one file in binary mode
//  
//////////////////////////////////////////////////////////////////////////////
bool CopyOneFile(char *szSourceFile, char *szDestFile)
{
	File mFile1, mFile2;
	bool bOK = true;
	if (!mFile1.Open(szSourceFile, "rb"))
	{
		bOK = false;
	}
	if (!bOK) return false;
	if (!mFile2.Open(szDestFile, "wb"))
	{
		bOK = false;
	}
	if (bOK)
	{
		uns8 ch;
		while (mFile1.Read(ch))
		{
			bOK = mFile2.Write(ch);
		}
	}
	mFile1.Close();
	mFile2.Close();
	return bOK;
}

//////////////////////////////////////////////////////////////////////////////
//
//  Function:		IsFilenameIdentical(char *szFileName1 char *szFileName2)
//
//  Description:	Whether those two files are identical.
//  
//////////////////////////////////////////////////////////////////////////////
#ifdef WIN32
#ifndef _WIN32_WCE
bool IsFileNameIdentical(char *szFileName1, char *szFileName2)
{
	if (!szFileName1 || !szFileName2)
	{
		return false;
	}

	// In case the name is identical
	if (!_stricmp(szFileName1, szFileName2))
	{
		return true;
	}

    // Check whether the files are valid
	int handle = -1;
	_sopen_s(&handle, szFileName1, _O_RDONLY, _SH_DENYNO, _S_IREAD);
	if (handle == -1) return false;
	_close(handle);
	handle = -1;
	_sopen_s(&handle, szFileName2, _O_RDONLY, _SH_DENYNO, _S_IREAD);
	if (handle == -1) return false;
	_close(handle);
    
	// Check whether the files are identical
	int handle1 = -1, handle2 = -1;
	bool result;
	_sopen_s(&handle1, szFileName1, _O_RDONLY, _SH_DENYRW, _S_IREAD);
	_sopen_s(&handle2, szFileName2, _O_RDONLY, _SH_DENYRW, _S_IREAD);

	if ((handle1 != -1) && (handle2 == -1))
	{
		result = true;
	}
	else
	{
		result = false;
	}
	if (handle1 != -1) _close(handle1);
	if (handle2 != -1) _close(handle2);
	return result;
}
#else		// under CE
bool IsFileNameIdentical(char *szFileName1, char *szFileName2)
{
	if (!stricmp(szFileName1, szFileName2))
		return true;
	else
		return false;
}

#endif

#else   // linux
bool IsFileNameIdentical(char *szFileName1, char *szFileName2)
{

	ino_t fNum1, fNum2;

	struct stat state;
	if (stat(szFileName1, &state) == 0)
	{
		fNum1 = state.st_ino;
	}
	else
		return false;

	if (stat(szFileName2, &state) == 0)
	{
		fNum2 = state.st_ino;
	}
	else
		return false;

	if (fNum1 == fNum2)
		return true;
	else
		return false;
}

#endif

//////////////////////////////////////////////////////////////////////////////
//
//  Function:		File()
//
//  Description:	class constructor
//
//////////////////////////////////////////////////////////////////////////////
File::File(void)
{
    Clear();
}


//////////////////////////////////////////////////////////////////////////////
//
//  Function:		~File()
//
//  Description:	class destructor
//
//////////////////////////////////////////////////////////////////////////////
File::~File()
{
    Close();
}

//////////////////////////////////////////////////////////////////////////////
//
//  Function:		Clear()
//
//  Description:	clear all the class members
//
//////////////////////////////////////////////////////////////////////////////
void File::Clear(void)
{
    m_Mode[0]		= 0;
    m_Section[0]	= 0;
    m_hFile			= NULL;
    m_Code			= 0;
    m_Separator		= 0;
    m_LineCount		= 0;
    m_FileName		= NULL;
	m_HeadPosition	= 0;     
#ifdef _ENCR_
	m_CodeCount		= 0;
#endif

}

//////////////////////////////////////////////////////////////////////////////
//
//  Function:		Open()
//
//  Description:	Open the file
//  Parameter:      szFileName, the full path of the file
//                  szMode, same as fopen "rt", "rb", "wt", "wb", "w+b", "w+t" 
//                  szMagic, the magic string at the beginning of the file
//                  encode,  the encode to encrypt the file
//  
//////////////////////////////////////////////////////////////////////////////
bool File::Open(const char *szFileName, char *szMode, char *szMagic, char encode)
{
    if ((!szFileName) || (!szMode) || (szFileName[0] == 0) || 
        (strlen(szFileName) > _MAX_PATH) || (strlen(szMode) > 3)) 
    {
        return false;
    }
    
	strcpy_s(m_Mode, szMode);

#ifndef _WIN32_WCE
    _chmod(szFileName, 0777);
#endif

	if (!strcmp(szMode, "rt"))
	{
		fopen_s(&m_hFile, szFileName, "rb");
	}
	else if (!strcmp(szMode, "r+t"))
	{
		fopen_s(&m_hFile, szFileName, "r+b");
	}
	else
	{
		fopen_s(&m_hFile, szFileName, szMode);
	}
    if (m_hFile == NULL) 
	{
#ifdef _WIN32_WCE
		ErrReport("File open error, fopen failed:[%s]\n", szFileName);
#endif
		return false;
	}
/*
#ifdef _WIN32_WCE
	PrintMsg("File open succ:[%s] [handle=%d]\n", szFileName, (int)m_hFile);
#endif
*/
    m_FileName = _strdup(szFileName);
    char mMagic[kMaxMagicLen+1];
    if (szMagic && szMagic[0])
    {
        if (strchr(szMode, 'r'))
        {
            ReadString(mMagic, kMaxMagicLen);
            mMagic[kMaxMagicLen] = 0;
			// we only care about the length of the input magic string
			int length = (int) strlen(szMagic);
			if (strncmp(mMagic, szMagic, length))
			{
				Close();
				return false;
			}
        }
        else
        {
            WriteString(szMagic);
        }
    }
    m_Code = encode;
#ifdef _ENCR_
	m_CodeCount		= 0;
#endif

    m_HeadPosition = ftell(m_hFile); 
    return true;
}

//////////////////////////////////////////////////////////////////////////////
//
//  Function:		Close()
//
//  Description:	Close the file
//  
//////////////////////////////////////////////////////////////////////////////
void File::Close(void)
{
    if (m_hFile)
    {
        fclose(m_hFile);
//		PrintMsg("File closed: [%s], [handle=%d]\n", m_FileName, (int)m_hFile);
        m_hFile = NULL;
        free(m_FileName);
    }
    Clear();
}

//////////////////////////////////////////////////////////////////////////////
//
//  Function:		SetSeparator(char inSeparator)
//
//  Description:	Set the separator
//  
//////////////////////////////////////////////////////////////////////////////
void File::SetSeparator(char inSeparator)
{
    m_Separator = inSeparator;
}

//////////////////////////////////////////////////////////////////////////////
//
//  Function:		IsTextMode()
//
//  Description:	Is text mode or NOT
//  
//////////////////////////////////////////////////////////////////////////////
bool File::IsTextMode(void)
{
    if (strchr(m_Mode, 't'))
    {
        return true;
    }
    else
    {
        return false;
    }
}

//////////////////////////////////////////////////////////////////////////////
//
//  Function:		GetFileName()
//
//  Description:	Get the file name
//  
//////////////////////////////////////////////////////////////////////////////
char *File::GetFileName(void)
{
    return m_FileName;
}


//////////////////////////////////////////////////////////////////////////////
//
//  Function:		ReadData()
//
//  Description:	Read a block of data from the file into the buffer
//  
//////////////////////////////////////////////////////////////////////////////
size_t File::ReadData(void* Buffer, size_t nSize, bool encode)
{
	size_t size = 0;
    if ((m_hFile == NULL) || (Buffer == NULL) || (nSize == 0))
    {
        size = 0;
    }
    else
    {
        size = fread(Buffer, sizeof(char), nSize, m_hFile);
		if (encode)
		{
			char *buffer = (char *)Buffer;
			for (size_t i = 0; i < nSize; i++)
			{
#ifdef _ENCR_
				if(m_CodeCount >= FILERANDLEN) m_CodeCount = 0;
                char Code = 0;
                if(m_Code >0){
				    Code = gFILERAND[m_CodeCount];
				    m_CodeCount ++;
                }
   				buffer[i] ^= Code;
#else
				buffer[i] ^= m_Code;
#endif
			}
		}
    }
	return size;
}

//////////////////////////////////////////////////////////////////////////////
//
//  Function:		WriteData()
//
//  Description:	Write a block of data to the file
//  
//////////////////////////////////////////////////////////////////////////////
size_t File::WriteData(void* Buffer, size_t nSize, bool encode)
{
	size_t size = 0;
    if ((m_hFile == NULL) || (Buffer == NULL) || (nSize == 0))
    {
        size = 0;
    }
    else
    {
		if (encode)
		{
			char *buffer = (char *)malloc(nSize);
			if (buffer)
			{
				memcpy(buffer, Buffer, nSize);
				for (size_t i = 0; i < nSize; i++)
				{
#ifdef _ENCR_
				if(m_CodeCount >= FILERANDLEN) m_CodeCount = 0;
                char Code = 0;
                if(m_Code >0){
				    Code = gFILERAND[m_CodeCount];
				    m_CodeCount ++;
                }
   				buffer[i] ^= Code;
#else
				buffer[i] ^= m_Code;
#endif
				}
				size = fwrite(buffer, sizeof(char), nSize, m_hFile);
				free(buffer);
			}
		}
		else
		{
			size = fwrite(Buffer, sizeof(char), nSize, m_hFile);
		}
    }
	return size;
}

//////////////////////////////////////////////////////////////////////////////
//
//  Function:		ReadString()
//
//  Description:	Read a string from the file
//  
//////////////////////////////////////////////////////////////////////////////
size_t File::ReadString(char* Buffer, size_t nSize)
{
    char ch = ' ';
    uns8 count = 0;
    bool isInBracket = false;

    if ((m_hFile == NULL) || (Buffer == NULL) || (nSize == 0))
    {
        return 0;
    }
    Buffer[0] = 0;
    char prevChar = 0;
    if (strchr(m_Mode, 't'))
    {
        while (ReadData(&ch, sizeof(char), false))
        {   
            if (ch == '\n') m_LineCount++;
            if (!IsSpace(ch)) break;
        }
        if (IsSpace(ch)) return 0;
        while (count < nSize)
        {
            if (ch == '\"')
            {
                if (isInBracket)
                {
                    break;
                }
                else
                {
                    isInBracket = true;
                }
            }
            else
            {
                Buffer[count] = ch;
                count++;
                if (!prevChar && (ch & 0x80)) prevChar = ch;
                else prevChar = 0;
            }
            if (!ReadData(&ch, sizeof(ch), false))
            {
                break;
            }
            if (!isInBracket && !prevChar && IsSpace(ch))
            {
                if (ch == '\n') m_LineCount++;
                break;
            }
        }
    }
    else
    {
        while (ReadData(&ch, sizeof(char), false))
        {
            if (count == nSize - 1) break;  
#ifdef _ENCR_
			if(m_CodeCount >= FILERANDLEN) m_CodeCount = 0;
            char Code = 0;
            if(m_Code >0){
			    Code = gFILERAND[m_CodeCount];
			    m_CodeCount ++;
            }
   			ch ^= Code;
#else
			ch ^= m_Code;
#endif
			Buffer[count] = ch;
			count++;
            if (ch == 0) break;
        }
    }
    if (count < nSize)
    {
        Buffer[count] = 0;
    }
    if ((isInBracket) && (count == 0))
    {
        count = 1;
    }
    return count;
}

//////////////////////////////////////////////////////////////////////////////
//
//  Function:		WriteString()
//
//  Description:	Write a string to the file
//  
//////////////////////////////////////////////////////////////////////////////
size_t File::WriteString(char* Buffer)
{
    uns8 count = 0;
	uns8 ch = 0;

	char szText[kMaxLine+1];
    if ((m_hFile == NULL) || (Buffer && strlen(Buffer) > kMaxLine))
    {
        return 0;
    }
    if (strchr(m_Mode, 't'))
    {
        char szSeparator[2];
        szSeparator[0] = m_Separator;
        szSeparator[1] = 0;
		strcpy_s(szText, szSeparator);
		if (Buffer)
		{
			strcat_s(szText, Buffer);
		}
		strcat_s(szText, szSeparator); 
        count = (uns8) WriteData(szText, strlen(szText), false);
    }
    else
    {
		if (Buffer)
		{
			while (true)
			{
				ch = Buffer[count];
#ifdef _ENCR_
			    if(m_CodeCount >= FILERANDLEN) m_CodeCount = 0;
                char Code = 0;
                if(m_Code >0){
			        Code = gFILERAND[m_CodeCount];
			        m_CodeCount ++;
                }
   	    		ch ^= Code;
#else
				ch ^= m_Code;
#endif
				Write(ch);
				count++;
#ifdef _ENCR_
   	    		ch ^= Code;
#else
				ch ^= m_Code;
#endif
				if (ch == 0) break;
			}
		}
		else
		{
			ch = 0;
#ifdef _ENCR_
		    if(m_CodeCount >= FILERANDLEN) m_CodeCount = 0;
            char Code = 0;
            if(m_Code >0){
		        Code = gFILERAND[m_CodeCount];
		        m_CodeCount ++;
            }
   	   		ch ^= Code;
#else
			ch ^= m_Code;
#endif
			Write(ch);
			count++;
		}
    }
    return count;
}

//////////////////////////////////////////////////////////////////////////////
//
//  Function:		ReadLine()
//
//  Description:	Read a line from the file, only applied for text file
//  
//////////////////////////////////////////////////////////////////////////////
bool File::ReadLine(char* Buffer, size_t nSize)
{
    char ch = '\0';
    if ((m_hFile == NULL) || (Buffer == NULL) || (nSize == 0))
    {
        return false;
    }
	if (strchr(m_Mode, 't'))
	{
		Buffer[0] = 0;
		if (!fgets(Buffer, (int) nSize, m_hFile))
            return false;
		RemoveNewLine(Buffer);
	}
	else
	{
		uns16 count;
		if (!Read(count)) return false;
		if (count > nSize)
		{
			SetPosition(count, SEEK_CUR);
			return false;
		}
		count = (uns16) ReadData(Buffer, count, false); 
		for (uns16 i = 0; i < count; i++)
		{
#ifdef _ENCR_
			if(m_CodeCount >= FILERANDLEN) m_CodeCount = 0;
            char Code = 0;
            if(m_Code >0){
			    Code = gFILERAND[m_CodeCount];
			    m_CodeCount ++;
            }
			Buffer[i] ^= Code;
#else
			Buffer[i] ^= m_Code;
#endif
		}
		Buffer[count] = 0;
	}
    return true;
}

//////////////////////////////////////////////////////////////////////////////
//
//  Function:		Read(uns32 &value)
//
//  Description:	Read a uns32 (unsigned int) from the file
//  
//////////////////////////////////////////////////////////////////////////////
bool File::Read(uns32 &value)
{
    char szText[kMaxLine + 1];

    if (m_hFile == NULL)
    {
        return false;
    }
    if (strchr(m_Mode, 't'))
    {
        if (!ReadString(szText, kMaxLine)) return false;
        value = atol(szText); 
    }
    else
    {
        if (!ReadData(&value, sizeof(value), false)) return false;
#ifdef SWAP_BYTE
	    SwapByteOrder((unsigned char*)&value, sizeof(value));
#endif
    }
    return true;
}

//////////////////////////////////////////////////////////////////////////////
//
//  Function:		Write(uns32 value)
//
//  Description:	Write a uns32 (unsigned int) to the file
//  
//////////////////////////////////////////////////////////////////////////////
bool File::Write(uns32 value)
{
    char szText[kMaxLine + 1];

    if (m_hFile == NULL)
    {
        return false;
    }
    if (strchr(m_Mode, 't'))
    {
        DecimalToString(szText, value);
        if (!WriteString(szText)) return false;
    }
    else
    {
#ifdef SWAP_BYTE
	    SwapByteOrder((unsigned char*)&value, sizeof(value));
#endif
        if (!WriteData(&value, sizeof(value), false)) return false;
    }
    return true;
}

//////////////////////////////////////////////////////////////////////////////
//
//  Function:		Read(long &value)
//
//  Description:	Read a long variable from the file
//  
//////////////////////////////////////////////////////////////////////////////
bool File::Read(long &value)
{
    char szText[kMaxLine + 1];

    if (m_hFile == NULL)
    {
        return false;
    }
    if (strchr(m_Mode, 't'))
    {
        if (!ReadString(szText, kMaxLine)) return false;
        value = atol(szText); 
    }
    else
    {
        if (!ReadData(&value, sizeof(value), false)) return false;
#ifdef SWAP_BYTE
	    SwapByteOrder((unsigned char*)&value, sizeof(value));
#endif
    }
    return true;
}

//////////////////////////////////////////////////////////////////////////////
//
//  Function:		Write(long value)
//
//  Description:	Write a long variable to the file
//  
//////////////////////////////////////////////////////////////////////////////
bool File::Write(long value)
{
    char szText[kMaxLine + 1];

    if (m_hFile == NULL)
    {
        return false;
    }
    if (strchr(m_Mode, 't'))
    {
        DecimalToString(szText, (int)value);
        if (!WriteString(szText)) return false;
    }
    else
    {
#ifdef SWAP_BYTE
	    SwapByteOrder((unsigned char*)&value, sizeof(value));
#endif
        if (!WriteData(&value, sizeof(value), false)) return false;
    }
    return true;
}

//////////////////////////////////////////////////////////////////////////////
//
//  Function:		Read(int &value)
//
//  Description:	Read a int from the file
//  
//////////////////////////////////////////////////////////////////////////////
bool File::Read(int &value)
{
    char szText[kMaxLine + 1];

    if (m_hFile == NULL)
    {
        return false;
    }
    if (strchr(m_Mode, 't'))
    {
        if (!ReadString(szText, kMaxLine)) return false;
        value = atoi(szText); 
    }
    else
    {
        if (!ReadData(&value, sizeof(value), false)) return false;
#ifdef SWAP_BYTE
	    SwapByteOrder((unsigned char*)&value, sizeof(value));
#endif
    }
    return true;
}

//////////////////////////////////////////////////////////////////////////////
//
//  Function:		Write(int value)
//
//  Description:	Write a int to the file
//  
//////////////////////////////////////////////////////////////////////////////
bool File::Write(int value)
{
    char szText[kMaxLine + 1];

    if (m_hFile == NULL)
    {
        return false;
    }
    if (strchr(m_Mode, 't'))
    {
        DecimalToString(szText, value);
        if (!WriteString(szText)) return false;
    }
    else
    {
#ifdef SWAP_BYTE
	    SwapByteOrder((unsigned char*)&value, sizeof(value));
#endif
        if (!WriteData(&value, sizeof(value), false)) return false;
    }
    return true;
}


//////////////////////////////////////////////////////////////////////////////
//
//  Function:		Read(uns16 &value)
//
//  Description:	Read a uns16 (unsigned short) from the file
//  
//////////////////////////////////////////////////////////////////////////////
bool File::Read(uns16 &value)
{
    char szText[kMaxLine + 1];

    if (m_hFile == NULL)
    {
        return false;
    }
    if (strchr(m_Mode, 't'))
    {
        if (!ReadString(szText, kMaxLine)) return false;
        value = atoi(szText); 
    }
    else
    {
        if (!ReadData(&value, sizeof(value), false)) return false;
#ifdef SWAP_BYTE
	    SwapByteOrder((unsigned char*)&value, sizeof(value));
#endif
    }
    return true;
}

//////////////////////////////////////////////////////////////////////////////
//
//  Function:		Write(uns16 value)
//
//  Description:	Write a uns16 (unsigned short) to the file
//  
//////////////////////////////////////////////////////////////////////////////
bool File::Write(uns16 value)
{
    char szText[kMaxLine + 1];

    if (m_hFile == NULL)
    {
        return false;
    }
    if (strchr(m_Mode, 't'))
    {
        DecimalToString(szText, value);
        if (!WriteString(szText)) return false;
    }
    else
    {
#ifdef SWAP_BYTE
	    SwapByteOrder((unsigned char*)&value, sizeof(value));
#endif
        if (!WriteData(&value, sizeof(value), false)) return false;
    }
    return true;
}


//////////////////////////////////////////////////////////////////////////////
//
//  Function:		Read(short &value)
//
//  Description:	Read a short from the file
//  
//////////////////////////////////////////////////////////////////////////////
bool File::Read(short &value)
{
    char szText[kMaxLine + 1];

    if (m_hFile == NULL)
    {
        return false;
    }
    if (strchr(m_Mode, 't'))
    {
        if (!ReadString(szText, kMaxLine)) return false;
        value = atoi(szText); 
    }
    else
    {
        if (!ReadData(&value, sizeof(value), false)) return false;
#ifdef SWAP_BYTE
	    SwapByteOrder((unsigned char*)&value, sizeof(value));
#endif
    }
    return true;
}

//////////////////////////////////////////////////////////////////////////////
//
//  Function:		Write(short value)
//
//  Description:	Write a short to the file
//  
//////////////////////////////////////////////////////////////////////////////
bool File::Write(short value)
{
    char szText[kMaxLine + 1];

    if (m_hFile == NULL)
    {
        return false;
    }
    if (strchr(m_Mode, 't'))
    {
        DecimalToString(szText, value);
        if (!WriteString(szText)) return false;
    }
    else
    {
#ifdef SWAP_BYTE
	    SwapByteOrder((unsigned char*)&value, sizeof(value));
#endif
        if (!WriteData(&value, sizeof(value), false)) return false;
    }
    return true;
}

//////////////////////////////////////////////////////////////////////////////
//
//  Function:		Read(uns8 &value)
//
//  Description:	Read a uns8 (unsigned char) from the file
//  
//////////////////////////////////////////////////////////////////////////////
bool File::Read(uns8 &value)
{
    char szText[kMaxLine + 1];

    if (m_hFile == NULL)
    {
        return false;
    }
    if (strchr(m_Mode, 't'))
    {
        if (!ReadString(szText, kMaxLine)) return false;
        value = atoi(szText); 
    }
    else
    {
        if (!ReadData(&value, sizeof(value), false)) return false;
    }
    return true;
}

//////////////////////////////////////////////////////////////////////////////
//
//  Function:		Write(uns8 value)
//
//  Description:	Write a uns8 (unsigned char) to the file
//  
//////////////////////////////////////////////////////////////////////////////
bool File::Write(uns8 value)
{
    char szText[kMaxLine + 1];

    if (m_hFile == NULL)
    {
        return false;
    }
    if (strchr(m_Mode, 't'))
    {
        DecimalToString(szText, value);
        if (!WriteString(szText)) return false;
    }
    else
    {
        if (!WriteData(&value, sizeof(value), false)) return false;
    }
    return true;
}

//////////////////////////////////////////////////////////////////////////////
//
//  Function:		Read(char &value)
//
//  Description:	Read a char from the file
//  
//////////////////////////////////////////////////////////////////////////////
bool File::Read(char &value)
{
    char szText[kMaxLine + 1];

    if (m_hFile == NULL)
    {
        return false;
    }
    if (strchr(m_Mode, 't'))
    {
        if (!ReadString(szText, kMaxLine)) return false;
        value = atoi(szText); 
    }
    else
    {
        if (!ReadData(&value, sizeof(value), false)) return false;
    }
    return true;
}

//////////////////////////////////////////////////////////////////////////////
//
//  Function:		Write(char value)
//
//  Description:	Write a char to the file
//  
//////////////////////////////////////////////////////////////////////////////
bool File::Write(char value)
{
    char szText[kMaxLine + 1];

    if (m_hFile == NULL)
    {
        return false;
    }
    if (strchr(m_Mode, 't'))
    {
        DecimalToString(szText, value);
        if (!WriteString(szText)) return false;
    }
    else
    {
        if (!WriteData(&value, sizeof(value), false)) return false;
    }
    return true;
}

//////////////////////////////////////////////////////////////////////////////
//
//  Function:		Read(float &value)
//
//  Description:	Read a float from the file
//  
//////////////////////////////////////////////////////////////////////////////
bool File::Read(float &value)
{
    char szText[kMaxLine + 1];

    if (m_hFile == NULL)
    {
        return false;
    }
    if (strchr(m_Mode, 't'))
    {
        if (!ReadString(szText, kMaxLine)) return false;
        value = (float)atof(szText); 
    }
    else
    {
        if (!ReadData(&value, sizeof(value), false)) return false;
#ifdef SWAP_BYTE
	    SwapByteOrder((unsigned char*)&value, sizeof(value));
#endif
    }
    return true;
}

//////////////////////////////////////////////////////////////////////////////
//
//  Function:		Write(float value)
//
//  Description:	Write a float to the file
//  
//////////////////////////////////////////////////////////////////////////////
bool File::Write(float value)
{
    char szText[kMaxLine + 1];

    if (m_hFile == NULL)
    {
        return false;
    }
    if (strchr(m_Mode, 't'))
    {
		sprintf_s(szText, "%f", value);
        if (!WriteString(szText)) return false;
    }
    else
    {
#ifdef SWAP_BYTE
	    SwapByteOrder((unsigned char*)&value, sizeof(value));
#endif
        if (!WriteData(&value, sizeof(value), false)) return false;
    }
    return true;
}

//////////////////////////////////////////////////////////////////////////////
//
//  Function:		IsEOF()
//
//  Description:	Determine whether the end of the file is reached.
//  
//////////////////////////////////////////////////////////////////////////////
bool File::IsEOF(void)
{
    if (!m_hFile || feof(m_hFile))
    {
        return true;
    }
    else
    {
        return false;
    }
}

//////////////////////////////////////////////////////////////////////////////
//
//  Function:		GetPosition()
//
//  Description:	Get the position of the file
//  
//////////////////////////////////////////////////////////////////////////////
size_t File::GetPosition(void)
{
    if (m_hFile == NULL)
    {
        return 0;
    }
    else
    {
		return ftell(m_hFile) - m_HeadPosition;
    }
}

//////////////////////////////////////////////////////////////////////////////
//
//  Function:		SetPosition(long lPosition, int nOrigin)
//
//  Description:	Set to the specified position in the file
//  
//////////////////////////////////////////////////////////////////////////////
bool File::SetPosition(long lPosition, int nOrigin)
{
    if (m_hFile == NULL)
    {
        return false;
    }
    else
    {
		long pos = lPosition;
		if (nOrigin == SEEK_SET)
		{
			pos += (long) m_HeadPosition;
		}
		if (fseek(m_hFile, pos, nOrigin) == 0)
		{
			return true;
		}
		else
		{
			return false;
		}
    }
}

//////////////////////////////////////////////////////////////////////////////
//
//  Function:		GetFileSize(void)
//
//  Description:	Get file size 
//  
//////////////////////////////////////////////////////////////////////////////
size_t File::GetFileSize(void)
{
    size_t outSize;
    if (m_hFile == NULL)
    {
        outSize = 0;
    }
    else
    {
        long pos = ftell(m_hFile);
        fseek(m_hFile, 0, SEEK_END);
        outSize = ftell(m_hFile);
        fseek(m_hFile, pos, SEEK_SET);
    }
    return outSize;
}

//////////////////////////////////////////////////////////////////////////////
//
//  Function:		ImportHandle()
//
//  Description:	Import the file handle opened externally by fopen
//  
//////////////////////////////////////////////////////////////////////////////
void File::ImportHandle(FILE* hFile, char *szMode)
{
    m_hFile = hFile;
	strcpy_s(m_Mode, szMode);
}


//////////////////////////////////////////////////////////////////////////////
//
//  Function:		GetLine(char *outLine, size_t nSize)
//
//  Description:	Get a line, only applied to text file
//  
//////////////////////////////////////////////////////////////////////////////
bool File::GetLine(char *outLine, size_t nSize)
{
    if (!outLine) return false;
    while (!IsEOF())
    {
        return ReadLine(outLine, nSize);
    }
    return false;
}

//////////////////////////////////////////////////////////////////////////////
//
//  Function:		GetLineNumber(void)
//
//  Description:	Get the line number
//  
//////////////////////////////////////////////////////////////////////////////
uns32 File::GetLineNumber(void)
{
    return m_LineCount;
}

//////////////////////////////////////////////////////////////////////////////
//
//  Function:		SetLineNumber(uns32 linenumber)
//
//  Description:	Set the line number
//  
//////////////////////////////////////////////////////////////////////////////
uns32 File::SetLineNumber(uns32 linenumber)
{
	m_LineCount = linenumber;
	return m_LineCount;
}

//////////////////////////////////////////////////////////////////////////////
//
//  Function:		PutLine(char *inLine)
//
//  Description:	Put a line, only applied to text file
//  
//////////////////////////////////////////////////////////////////////////////
bool File::PutLine(char *inLine)
{
    if (inLine && inLine[0])
    {
		if (strchr(m_Mode, 't'))
		{
			WriteString(inLine);
		}
		else
		{
			char szText[1024];
			strcpy_s(szText, inLine);
			uns16 count = (uns16) strlen(szText);
			for (uns16 i = 0; i < count; i++)
			{
#ifdef _ENCR_
			    if(m_CodeCount >= FILERANDLEN) m_CodeCount = 0;
                char Code = 0;
                if(m_Code >0){
			        Code = gFILERAND[m_CodeCount];
			        m_CodeCount ++;
                }
   	    		szText[i] ^= Code;
#else
				szText[i] ^= m_Code;
#endif
			}
			if (!Write(count)) return 0;
			count = (uns16) WriteData(szText, count, false); 
		}
    }
    if (!strchr(m_Mode, 't')) return false;
    WriteData((void*) kNewLine, strlen(kNewLine), false);
    return true;
}

//////////////////////////////////////////////////////////////////////////////
//
//  Function:		GetLineInSection(char *outLine, size_t nSize)
//
//  Description:	Get a line within the section, only applied to text file
//  
//////////////////////////////////////////////////////////////////////////////
bool File::GetLineInSection(char *outLine, size_t nSize)
{
    if (!strchr(m_Mode, 't')) return false;
    if (!outLine) return false;
	while (!IsEOF())
    {
        ReadLine(outLine, nSize);
        if ((outLine[0] == 0) || (outLine[0] == '#')) continue;
        if ((m_Section[0] == '<') && (!strcmp(outLine, "%END")))
        {
            break;
        }
        if ((m_Section[0] == '[') && (outLine[0] == '['))
        {
            break;
        }
        return true;
    }
    return false;
}

//////////////////////////////////////////////////////////////////////////////
//
//  Function:		DecimalToString(char *outString, uns32 value)
//
//  Description:	Make the decimal to string
//  
//////////////////////////////////////////////////////////////////////////////
template <size_t _size>
void File::DecimalToString(char (&outString)[_size], uns32 value)
{
	sprintf_s(outString, "%u", value);
}

//////////////////////////////////////////////////////////////////////////////
//
//  Function:		DecimalToString(char *outString, uns32 value)
//
//  Description:	Make the decimal to string
//  
//////////////////////////////////////////////////////////////////////////////
template <size_t _size>
void File::DecimalToString(char (&outString)[_size], int value)
{
	sprintf_s(outString, "%d", value);
}

//////////////////////////////////////////////////////////////////////////////
//
//  Function:		RemoveNewLine()
//
//  Description:	Remove the new line at the end of the buffer
//  
//////////////////////////////////////////////////////////////////////////////
bool File::RemoveNewLine(char* Buffer)
{
	size_t len = strlen(Buffer);
	if (len > 0) len--;
	if (Buffer[len] == '\n')
	{
		Buffer[len] = 0;
		if (len > 0) len--;
		if (Buffer[len] == '\r')
		{
			Buffer[len] = 0;
		}
		m_LineCount++;
		return true;
	}
	else
	{
		return false;
	}
}


#ifdef UNDER_CE
int         remove(const char * szFileName)
{
	unsigned short* pWStr = AsciiToUnicode(szFileName);
	BOOL bRet = DeleteFile(pWStr);
	delete [] pWStr;
	if (bRet)
		return 0;
	else
		return -1;
}

int         rename(const char *pSrc, const char *pDes)
{
	unsigned short* pWSrc = AsciiToUnicode(pSrc);
	unsigned short* pWDes = AsciiToUnicode(pDes);
	
	BOOL bRet = MoveFile(pWSrc, pWDes);

	delete [] pWSrc;
	delete [] pWDes;

	if (bRet)
		return 0;
	else
		return -1;
}
#endif

#ifdef UNDER_CE
// default data folder
char kDataFolder[256] = "/Storage Card/InfoTalk";
char *getenv( const char *varname )
{
	return kDataFolder;
}

int putenv( const char *envstring )
{
	strcpy(kDataFolder, envstring);
	return 0;
}

#endif


