#include <ctype.h>
#include <string.h>
#include <stdio.h>

#include "parareader.h"

CParaReader::CParaReader(void)
: m_cSeperator(0)
{
}

CParaReader::~CParaReader(void)
{
}

// Set seperator
void CParaReader::SetSeperator(char sep)
{
	m_cSeperator = sep;
}


// par and val uses memory of line.
bool CParaReader::GetParValue(char * line, char * &par, char * &val)
{
	par = line;
	while (isspace(*par)) par++;
	if (*par == '#' || *par == '\n' || *par == 0) return false;

	val = par;
	if (m_cSeperator == 0) {
		while (*val && *val != '\n' && !isspace(*val)) val++;
	} else {
		while (*val && *val != '\n' && *val != m_cSeperator) val++;
	}

	if (*val) {
		*val++ = 0;
		while (isspace(*val)) val++;
	}

	if (*val == 0 || *val == '\n') {
		val = 0;
		return false;
	}
		
	char *pe = val + strlen(val) - 1;
	while (*pe == '\n' || isspace(*pe)) *pe-- = 0;

	return true;
}

int CParaReader::StrReadHexDouble(double *data, int dlen, char *line, const char *sep)
{
	int nfl = 0;
	char *context = NULL;
	char *token = strtok_s(line, sep, &context);
	while (token) {
		if (nfl < dlen) sscanf_s(token, "%I64x", data + nfl);
		nfl++;
		token = strtok_s(NULL, sep, &context);
	}
	return nfl;
}

