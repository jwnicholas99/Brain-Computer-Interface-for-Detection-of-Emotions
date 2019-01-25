// FileName:
//		String.c
//
// Purpose:
//		Defines String operations
//
//////////////////////////////////////////////////////////////////////////////

/* Include declarations files. */
 
#include "String.h"
 
/* Define functions. */
 
//
//    StringDup
//---------------------------------------------------------------------------
char * StringDup (const char * s) 
{
    char * s2 = (char *) malloc(strlen(s) + 1);    

	strcpy_s(s2, strlen(s) + 1, s);
    return s2;
}

//
//    StringUpcase
//---------------------------------------------------------------------------
char * StringUpcase (char * s) 
{
    char ch;
	size_t length = strlen(s);
    for (size_t i = 0; i < length; i++)
    {
        ch = s[i];
		if (ch & 0x80)
		{
			i++;
		}
		else
		{
			if (islower(ch)) ch = toupper(ch);
			s[i] = ch;
		}
    }
    return s;
}

//
//    StringLowcase
//---------------------------------------------------------------------------
char * StringLowcase (char * s) 
{
    char ch;
	size_t length = strlen(s);
    for (size_t i = 0; i < length; i++)
    {
        ch = s[i];
		if (ch & 0x80)
		{
			i++;
		}
		else
		{
			if (isupper(ch)) ch = tolower(ch);
		}
        s[i] = ch;
    }
    return s;
}

//
//    StrChr
//---------------------------------------------------------------------------
char *StrChr (char * inString, char inChar)
{
	size_t length = strlen(inString);
	for (size_t i = 0; i < length; i++) 
	{
		if (inString[i] == inChar)
		{
			return &inString[i];
		}
		if (inString[i] & 0x80)
		{
			i++;
		}
	}
	return NULL;
}



//
//    StrrChr
//---------------------------------------------------------------------------
char *StrrChr (char * inString, char inChar)
{
    char    *pLastFound = NULL, *p = inString - 1;
    while ((p = StrChr (p+1, inChar)))
        pLastFound = p;
	return pLastFound;
}


//
//    StrStr
//---------------------------------------------------------------------------
char* StrStr(char *inString, char *inSubStr)
{
	size_t length = strlen(inString);
	size_t sub_length = strlen(inSubStr);
	for (size_t i = 0; i < length; i++)
	{
        if (!strncmp(&inString[i], inSubStr, sub_length))
		{
			return &inString[i];
		}
        if (inString[i] & 0x80)
        {
            i++;
        }
	}
    return NULL;
}

//
//    StringCopy
//---------------------------------------------------------------------------
void StringCopy (char * toStr, char * frStr, int start)
{
	size_t length = strlen(frStr);
	for (size_t i = 0; i < length; i++) 
	{
		toStr[i+start] = frStr[i];
	}
}


//
//    ExtractParamStrings
//---------------------------------------------------------------------------
void ExtractParamStrings (char * inBuffer, char * outStrings[], int * outCount) 
{
    char * str, buffer[256];
    
	strcpy_s(buffer, inBuffer);
    int count = 0;
	char *next_token = NULL;
	for (str = strtok_s(buffer, " ", &next_token); str; str = strtok_s(NULL, " ", &next_token)) {
	outStrings[count++] = str;
    }
    *outCount = count;
}

bool IsSpace(unsigned char ch)
{
    if ((ch == ' ') || (ch == '\t') || (ch == '\n') || (ch == '\r'))
    {
        return true;
    }
    else
    {
        return false;
    }
}



bool  IsDigitStr(const char *pStr)
{
    int len = (int) strlen(pStr);
    bool isDigit = true;

    const char *p = pStr;
    for (int i=0; i < len; i++, p++)
    {
      if (*p >= '0' && *p <= '9')
        continue;
      else
      {
          isDigit = false;
          break;
      }
    }

    return isDigit;
}
          

/*============================================================================
 *  long Hash (char *str)
 *===========================================================================*/
long Hash (char *str)
{
    unsigned long hval = 0;
    unsigned long bit;
    unsigned long mask = 0xffff;

    while (*str) 
	{
		hval <<= 4;
		bit  = (hval & 0xf0000) >> 16;
		hval = (hval & mask) | bit;
		hval ^= *str++;
    }
    return (hval & mask);
}

/*============================================================================
 *	bool RemoveLanguageTag(char *szInResult, char *szOutResult, char *szOutLanguage)
 *===========================================================================*/
//bool RemoveLanguageTag(char *szInResult, char *szOutResult, char *szOutLanguage)
//{
//    char szLanguage[5];
//    szLanguage[0] = 0;
//    if (!szInResult || !szOutResult)
//    {
//        return false;
//    }
//    uns32 dwIndex = 0;
//    char *ptr = szInResult;
//    while (*ptr)
//    {
//        if (*ptr & 0x80)
//        {
//            szOutResult[dwIndex] = *ptr;
//            szOutResult[dwIndex + 1] = *(ptr+1);
//            dwIndex += 2;
//            ptr += 2;
//        }
//        else if (*ptr == '\\')
//        {
//            if (szOutLanguage)
//            {
//                strncpy_s(szOutLanguage, ptr, 4);
//                szOutLanguage[4] = 0;
//            }
//            ptr += 4;
//        }
//        else
//        {
//            szOutResult[dwIndex] = *ptr;
//            dwIndex++;
//            ptr++;
//        }
//    }
//    szOutResult[dwIndex] = 0;
//    return true;
//}

/*============================================================================
 *	isAsciiString(char *inString)
 *===========================================================================*/
bool isAsciiString(char *inString)
{
	int length = (int) strlen(inString);
	for(int i = 0; i < length; i++)
	{
		if (inString[i] & 0x80)	return false;
	}
	return true;
}

/*============================================================================
 *	isAlphabetString(char *inString)
 *===========================================================================*/
bool isAlphabet(char c)
{
	if (c & 0x80)		return false;
	if (!isalpha(c))	return false;
	return true;
}


/*============================================================================
 *	isAlphabetString(char *inString)
 *===========================================================================*/
bool isAlphabetString(char *inString)
{
	int length = (int) strlen(inString);

	for(int i = 0; i < length; i++)
	{
		if (inString[i] & 0x80)		return false;
		if (!isalpha(inString[i]))	return false;
	}
	return true;
}

/*============================================================================
 *	isMixLingualString(char *inString)
 *===========================================================================*/
bool isMixLingualString(char *inString)
{
	bool isAscii = false, notAscii = false;
	int  length = (int) strlen(inString);

	for(int i = 0; i < length; i++)
	{
		if (inString[i] & 0x80)
		{
			notAscii = true;
			i++;
		}
		else
		{
			isAscii  = true;
		}
	}
	return (isAscii && notAscii);
}

/*============================================================================
 *	isGrammarMixLingualString(char *inString)
 *===========================================================================*/
bool isGrammarMixLingualString(char *inString)
{
	bool isAscii = false, notAscii = false;
	int  length  = (int) strlen(inString);

	for(int i = 0; i < length; i++)
	{
		if (inString[i] & 0x80)
		{
			notAscii = true;
			i++;
		}
		else
		{
			if( ((i+1)<length) && !(inString[i+1] & 0x80) )
			{
				isAscii  = true;
			}
		}
	}
	return (isAscii && notAscii);
}

/*============================================================================
 *	isDigitString(char *inString)
 *===========================================================================*/
bool isDigitString(char *inString)
{
	int length = (int) strlen(inString);

	for(int i = 0; i < length; i++)
	{
		if (inString[i] & 0x80)		return false;
		if (!isdigit(inString[i]))	return false;
	}
	return true;
}

/*============================================================================
 *	void EncryptString(char *inString);
 *===========================================================================*/
void EncryptString(char *inString)
{
	for(int i = 0; inString[i] != '\0'; i++)
	{
		inString[i] = EncryptChar(inString[i]);
	}
}

/*============================================================================
 *	void DeEncryptString(char *inString)
 *===========================================================================*/
void DeEncryptString(char *inString)
{
	for(int i = 0; inString[i] != '\0'; i++)
	{
		inString[i] = DeEncryptChar(inString[i]);
	}
}

/*============================================================================
 *	char EncryptChar(char inChar)
 *===========================================================================*/
char EncryptChar(char inChar)
{
	char myDeEncryption[kMaxString];
	char myEncryption[kMaxString];

	strcpy_s(myDeEncryption, kDeEncryptPron);
	strcpy_s(myEncryption,   kEncryptPron);

	for(int i=0; myDeEncryption[i] != '\0'; i++)
	{
		if( inChar == myDeEncryption[i])
		{
			return myEncryption[i];
		}
	}

	return inChar;
}

/*============================================================================
 *	char DeEncryptChar(char inChar)
 *===========================================================================*/
char DeEncryptChar(char inChar)
{
	char myDeEncryption[kMaxString];
	char myEncryption[kMaxString];

	strcpy_s(myDeEncryption, kDeEncryptPron);
	strcpy_s(myEncryption,   kEncryptPron);

	for(int i=0; myEncryption[i] != '\0'; i++)
	{
		if( inChar == myEncryption[i])
		{
			return myDeEncryption[i];
		}
	}

	return inChar;
}

/*============================================================================
 *	bool isIdenticalContent(char *inString1, char *inString2)
 *===========================================================================*/
int isIdenticalContent(char *inString1, char *inString2)
{
	if( !inString1 && !inString2)	return 0;
	if( !inString1 ) return 1;
	if( !inString2 ) return -1;

	if( strcmp(inString1,inString2) < 0)	return 1;
	if( strcmp(inString1,inString2) == 0)	return 0;
	return -1;

}

/*============================================================================
 *	void  Uppercase(char *pszString)
 *===========================================================================*/
void  Uppercase(char *pszString)
{
    for (char *s = pszString; *s ; s++){
		if( *s >= 128 || *s<0 ) {
			s++;
			continue;		//skip two bye characters
		}
		*s = toupper(*s);
	}
}

/*============================================================================
 *	void  Lowercase(char *pszString)
 *===========================================================================*/
void  Lowercase(char *pszString)
{
    for (char *s = pszString; *s ; s++){
		if( *s >= 128 || *s<0 ) {
			s++;
			continue;		//skip two bye characters
		}
		*s = tolower(*s);
	}

}

/*============================================================================
 *	int  InvertStrCmp(char *inStr1, char *inStr2)
 *===========================================================================*/
int  InvertStrCmp(char *inStr1, char *inStr2)
{
	int	 i,j;
	char *myString1  = new char [strlen(inStr1)+1];
	char *myString2  = new char [strlen(inStr2)+1];

	for( j=0, i=(int) strlen(inStr1)-1; i>-1; i--, j++)	myString1[j] = inStr1[i];
	myString1[j] = '\0';

	for( j=0, i=(int) strlen(inStr2)-1; i>-1; i--, j++)	myString2[j] = inStr2[i];
	myString2[j] = '\0';

	int  myRet = strcmp(myString1,myString2);

	delete []myString1;
	delete []myString2;

	return myRet;
}

/*============================================================================
 *	void  MultibyteCharSegmentedString(char *pszString)
 *===========================================================================*/
void MultibyteCharSegmentedString(char *pszString)
{
    if (strlen (pszString) <= 2)
        return;
/*    char    *pszSrc = pszString + 1;
    int      iNeedSegment = 1;
    while (*pszSrc) {
        if ( (0x20 < *(pszSrc - 1)) && (*(pszSrc - 1) < 0x7F) && 
             (0x20 < *pszSrc) && (*pszSrc < 0x7F) ) {
            iNeedSegment = 0;
            break;
        }
        pszSrc ++;
    }
    if (iNeedSegment == 0)
        return;
*/
    char    *pszSrc = _strdup (pszString);
	char    *pszSrcMem = pszSrc;
    while (*pszSrc) {   // discard the preceding blank
        if (*pszSrc != ' ' && *pszSrc != '\t')
            break;
        pszSrc ++;
    }
    while (*pszSrc) {
        if (*pszSrc == ' ' || *pszSrc == '\t') {
            if (*(pszString-1) != ' ' && *(pszString-1) != '\t')
                *pszString ++ = *pszSrc;
        }
        else if (*pszSrc >= 128 || *pszSrc < 0) {
            *pszString ++ = *pszSrc ++;
            *pszString ++ = *pszSrc;
            *pszString ++ = ' ';
        }
        else {
            *pszString ++ = *pszSrc;
            if (*(pszSrc+1) >= 128 || *(pszSrc+1) < 0 || 
                (0x30 <= *pszSrc && *pszSrc <= 0x39))
                *pszString ++ = ' ';
        }
        pszSrc ++;
    }
    *pszString = '\0';
    free (pszSrcMem);
}

// compare two strings without case sensitive
int StringiCmp(const char* inString1, const char* inString2)
{
	if( !inString1 && !inString2)	return 0;
	if( !inString1 ) return -1;
	if( !inString2 ) return 1;

	const char* p1 = inString1;
	const char* p2 = inString2;

	while (true)
	{
		if (!(*p1) && !(*p2))
			return 0;
		if (!(*p1))
			return -1;
		if (!(*p2))
			return 1;

		if (*p1 == *p2)
		{
			p1++; p2++;
			continue;
		}

		if (*p1 > *p2)
			return 1;
		else
			return -1;
	}
}

float StringToFloat(const char* inStr)
{
	float fp = 0.0;
	sscanf_s(inStr, "%f", &fp);
	return fp;
}

// ccwang
int ReadDataFromString(char *strbuf, double *d, int m)
{
	int n = 0;
	while (*strbuf) {
		int i = 0;
		char c;
		bool empty = true;
		while ((c = strbuf[i]) != 0 && c != ',') {
			i++;
			if (empty) empty = (isspace(c) != 0);
		}

		if (empty) {
			printf("Syntax error!\n");
			n = 0;
			break;
		}

		if (n >= m) {
			printf("Maximun order limit reached!\n");
			n = 0;
			break;
		}

		strbuf[i] = 0;
		d[n++] = atof(strbuf);
		strbuf += i;
		if (c == ',') {
			strbuf[0] = c;
			strbuf++;
		}
	}

	return n;
}

#ifdef UNDER_CE
// transfer a ascii string to unicode string.
//	return the unicode string, caller has to free the memory. (delete []
//
unsigned short* AsciiToUnicode(const char* pStr)
{
	int len = strlen(pStr);
	unsigned short* pWStr = new unsigned short [len+1];
	MultiByteToWideChar(CP_ACP, 0, pStr, -1, pWStr, len+1);
	return pWStr;
}
#endif

