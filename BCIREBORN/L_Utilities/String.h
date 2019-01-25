//---------------------------------------------------------
// FileName:
//		String.h
//
// Purpose:
//		Defines String operations 
//
//////////////////////////////////////////////////////////////////////////////

#ifndef STRING_H
#define STRING_H 
 
#include "ConstDef.h"

#define kDeEncryptPron	"0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ"
#define kEncryptPron	"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"

/* Define function prototypes. */
bool IsSpace(unsigned char ch); 
char * StringDup (const char * s);
char * StringUpcase (char * s);
char * StringLowcase (char * s);
char * StrChr(char *szSrc, char ch);
char * StrrChr(char *szSrc, char ch);
char * StrStr(char *szSrc, char *szSubStr);
void ExtractParamStrings (char * inBuffer, char * outStrings[], int * outCount);
//bool RemoveLanguageTag(char *szInResult, char *szOutResult, char *szOutLanguage);
bool  IsDigitStr(const char *pStr);
long Hash (char *str);

bool isAlphabet(char c);
bool isAsciiString(char *inString);
bool isAlphabetString(char *inString);
bool isMixLingualString(char *inString);
bool isGrammarMixLingualString(char *inString);
bool isDigitString(char *inString);
bool isPinyinString(char *inString);

void EncryptString(char *inString);
void DeEncryptString(char *inString);
char EncryptChar(char inChar);
char DeEncryptChar(char inChar);

int isIdenticalContent(char *inString1, char *inString2);
int StringiCmp(const char* inString1, const char* inString2);

void  Uppercase(char *pszString);
void  Lowercase(char *pszString);

void MultibyteCharSegmentedString(char *pszString);

int  InvertStrCmp(char *inStr1, char *inStr2);

float StringToFloat(const char* inStr);

int ReadDataFromString(char *strbuf, double *d, int m);

#ifdef UNDER_CE
#include "String.h"
#define strdup StringDup
#define stricmp StringiCmp
#define atof  StringToFloat
#define strrchr StrrChr

unsigned short* AsciiToUnicode(const char* pStr);

#endif

#endif // STRING_H
 
