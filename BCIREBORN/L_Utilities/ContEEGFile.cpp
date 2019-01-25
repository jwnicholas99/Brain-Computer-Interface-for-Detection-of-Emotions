#include <stdio.h>
#include <memory.h>

#include ".\conteegfile.h"

CContEEGFile::CContEEGFile(void)
{
	m_pEEGBuffer = 0;
	m_nNumSample = 0;

	m_pStimInfo = NULL;
	m_nNumStim = 0;

	m_pStimType = NULL;
	m_nNumType = 0;

	memset(&m_hHeader, 0, sizeof(m_hHeader));
}

CContEEGFile::~CContEEGFile(void)
{
	if (m_pEEGBuffer != NULL) delete [] m_pEEGBuffer;
	if (m_pStimInfo != NULL) delete [] m_pStimInfo;
	if (m_pStimType != NULL) delete [] m_pStimType;
}

int CContEEGFile::Initialize(const char *fn)
{
	FILE *fp = NULL;
	fopen_s(&fp, fn, "rb");
	if (fp == NULL) {
		printf("Initialize: Cannot open file %s!\n", fn);
		return 0;
	}

	// get file length
	fseek(fp, 0, SEEK_END);
	int flen = ftell(fp);
	rewind(fp);
	if (flen <= sizeof(m_hHeader)) {
		printf("Initialize: File length error!\n");
		return 0;
	}

	fread(&m_hHeader, sizeof(m_hHeader), 1, fp);

	if (m_hHeader.num_chan <= 0 || m_hHeader.num_evt != 1||
		m_hHeader.sample_rate <= 0 || m_hHeader.resolution < 0 ||
		m_hHeader.data_size != 4)
	{
		printf("Invalid cnt EEG file");
		fclose(fp);
		return 0;
	}


	// read cnt file
	m_nNumSample = (flen - sizeof(m_hHeader)) / 
		((m_hHeader.num_chan + m_hHeader.num_evt) * m_hHeader.data_size);
	if (flen != sizeof(m_hHeader) + m_nNumSample * 
		(m_hHeader.num_chan + m_hHeader.num_evt) * m_hHeader.data_size)
	{
		fclose(fp);
		printf("Data file size error!\n");
		return 0;
	}

	m_pEEGBuffer = new int[m_nNumSample * m_hHeader.num_chan];
	int *pdata = m_pEEGBuffer;
	int *pevt = new int[m_nNumSample];
	int last_code = 0;
	int iv;
	m_nNumStim = 0;
	int codehis[256];
	memset(codehis, 0, sizeof(codehis));

	for (int n = 0; n < m_nNumSample; n++) {
		fread(pdata, m_hHeader.data_size, m_hHeader.num_chan, fp);
		fread(&iv, m_hHeader.data_size, 1, fp);
		iv &= 0xff;
		if (iv != 0 && iv != last_code) {
			m_nNumStim++;
			pevt[n] = iv;
			codehis[iv]++;
		} else {
			pevt[n] = 0;
		}
		last_code = iv;

		pdata += m_hHeader.num_chan;
	}

	m_pStimInfo = new StimInfo[m_nNumStim];
	m_nNumStim = 0;
	for (int i = 0; i < m_nNumSample; i++) {
		if (pevt[i]) {
			m_pStimInfo[m_nNumStim].code = pevt[i];
			m_pStimInfo[m_nNumStim].no = i;
			if (m_nNumStim > 0) m_pStimInfo[m_nNumStim - 1].len = 
				m_pStimInfo[m_nNumStim].no - m_pStimInfo[m_nNumStim - 1].no;
			m_nNumStim++;
		}
	}
	m_pStimInfo[m_nNumStim - 1].len = m_nNumSample - m_pStimInfo[m_nNumStim - 1].no;

	m_nNumType = 0;
	for (int i = 0; i < 256; i++) m_nNumType += (codehis[i] != 0);

	m_pStimType = new StimType[m_nNumType];
	m_nNumType = 0;
	for (int i = 0; i < 256; i++) {
		if (codehis[i]) {
			m_pStimType[m_nNumType].code = i;
			m_pStimType[m_nNumType].num = codehis[i];
			m_nNumType++;
		}
	}

	delete [] pevt;

	fclose(fp);
	return m_nNumSample;
}

CContEEGFile *CContEEGFile::ReadContEEGFile(const char *fn)
{
	CContEEGFile *pcf = new CContEEGFile();
	if (pcf->Initialize(fn) > 0) return pcf;
	else {
		delete pcf;
		return NULL;
	}
}
