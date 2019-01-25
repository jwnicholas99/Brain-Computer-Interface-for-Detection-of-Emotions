#pragma once

#include "stdio.h"
#include "windows.h"
#include "LIMITS.H"
#define MAXFILELISTSIZE 100

typedef struct _CNTFileHeader{
	int iNumCh;
	int iSizeCh;
	int iNumSamplePerBlock;
	int iSamplingRate;
	int iNumByte;
	float fResolution;
}CNTFileHeader;


class CEEGContFile
{
public:
	CEEGContFile();
	~CEEGContFile(void);

	// TODO: add your methods here.
	CNTFileHeader m_CNTHeader;
	BYTE** m_ppAllEpochData;
	int* m_pAllEpochCode;
	int m_iNumAllEpoch;

	//int m_iStartTime;
	//int m_iEndTime;
	//int m_iNumChannels;
	//int m_iSamplingRate;
	//int m_iUsedChannels[1024];
	//int m_iNumUsedChannels;
	//char m_strChannelNames[256][256];
	//int m_iNumEpochPerRound;
	
	int m_iNumTarget;
	char m_TargetCharList[1024];

private:
	bool GetSampleFileConfig(BYTE* pFileData, int iFileLength,int& iNumEpoch, int*& pEpochPositions, int iEpochCodeLB,int iEpochCodeUB);
	void ClearAllocatedMemory();
	bool LoadDataFromFileList(char** strFNameList,void* strDir,int iNumFile, int iEpochCodeLB=1, int iEpochCodeUB=INT_MAX);
	BYTE* m_pAllCNTData;
	int m_iSizeAllCNTData;
	bool AppendAllEpochData(BYTE* pData, BYTE**& ppAllEpochData, int*& pAllEpochCode, int& iNumAllEpoch, int* pNewEpochPositions, int iNumNewEpoch);

public:
	bool LoadDataFileList(const char* strListFile);
	int GetTargetEpochPtrs(int iNumEpochPerRound,int iNumRoundPerTarget, int**& ppTargetEpoch, int*& pTargetEpochCode, char* pCharTable, int iCharTableSize);
	int GetNonTargetEpochPtrs(int iNumEpochPerRound,int iNumRoundPerTarget, int**& ppNonTargetEpoch, int*& pNonTargetEpochCode, char* pCharTable, int iCharTableSize);
	int GetEpochData(int iStartSample,int iNumSample, int** ppTargetEpoch, int iNumTargetEpoch, int**& ppTargetEpochData);
};

