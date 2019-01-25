#pragma once

class CParaReader
{
public:
	CParaReader(void);
	~CParaReader(void);
private:
	// Seperate parameter and value pair
	char m_cSeperator;
public:
	// Set seperator
	void SetSeperator(char sep);
	// par and val uses memory of line.
	bool GetParValue(char * line, char * &  par, char * & val);

	// ccwang: some utilities
	static int StrReadHexDouble(double *data, int dlen, char *line, const char *sep);
};
