#include ".\eegcontfile.h"
#include "..\L_Utilities\resources.h"

CEEGContFile::CEEGContFile()
{
    m_ppAllEpochData=NULL;
	m_pAllCNTData=NULL;
	m_pAllEpochCode=NULL;
	return; 
}

CEEGContFile::~CEEGContFile(void)
{
	ClearAllocatedMemory();
}

void CEEGContFile::ClearAllocatedMemory()
{
	if(m_ppAllEpochData!=NULL) delete m_ppAllEpochData;
	if(m_pAllEpochCode!=NULL) delete m_pAllEpochCode;
	if(m_pAllCNTData!=NULL) delete m_pAllCNTData;
	m_ppAllEpochData=NULL;
	m_pAllCNTData=NULL;
}

bool CEEGContFile::LoadDataFromFileList(char** strFNameList,void* strDir,int iNumFile,int iEpochCodeLB,int iEpochCodeUB)
{
	ClearAllocatedMemory();
	int i;
	char strFileName[1024];
	strcpy_s(strFileName,(char*)strDir);
	strcat_s(strFileName,strFNameList[0]);
	
	FILE* fp;
	if (fopen_s(&fp, strFileName,"rb")) return false;
	fread(&m_CNTHeader,sizeof(CNTFileHeader),1,fp);
	fclose(fp);
	if(m_CNTHeader.iNumByte!=4) return false; ///Do not support formats other than int
	
	/// Header Consistency Check and files' length count
	///---------------------------------------------------
	
	CNTFileHeader tmpHeader;
	bool bConsistentHeader=1;
	int iAllFileSize=0;
	for (i=0; bConsistentHeader && i<iNumFile; i++)
	{
		strcpy_s(strFileName,(char*)strDir);
		strcat_s(strFileName,strFNameList[i]);
		if (fopen_s(&fp, strFileName, "rb")) return false;
		fread(&tmpHeader,sizeof(CNTFileHeader),1,fp);
		bConsistentHeader=bConsistentHeader& (memcmp(&tmpHeader,&m_CNTHeader,sizeof(CNTFileHeader))==0);
		fseek(fp,0,SEEK_END);
		iAllFileSize += ftell(fp);
		fclose(fp);
	}
	if (!bConsistentHeader) return false;

	///---------------------------------------------------

	///Load All Files into CNTData memory block
	m_iSizeAllCNTData=iAllFileSize;
    int* pFilePosInBuffer=new int[iNumFile];
	int* pFileLength=new int[iNumFile];
	m_pAllCNTData=new BYTE[iAllFileSize];
	int iFileLength;
	pFilePosInBuffer[0]=0;
	for(i=0;i<iNumFile;i++){
		strcpy_s(strFileName,(char*)strDir);
		strcat_s(strFileName,strFNameList[i]);
		if (fopen_s(&fp, strFileName, "rb")) return false;
		fseek(fp,0,SEEK_END);iFileLength=ftell(fp);pFileLength[i]=iFileLength;
		if(i<iNumFile-1) pFilePosInBuffer[i+1]=pFilePosInBuffer[i]+iFileLength; //Update the next block position
		fseek(fp,0,SEEK_SET);
		fread(&m_pAllCNTData[pFilePosInBuffer[i]],1,iFileLength,fp);
		fclose(fp);
	}

	int iNumEpoch;int* pEpochPositions;
	m_iNumAllEpoch=0;
	for(i=0;i<iNumFile;i++){
        GetSampleFileConfig(m_pAllCNTData+pFilePosInBuffer[i],pFileLength[i],iNumEpoch,pEpochPositions,iEpochCodeLB,iEpochCodeUB);
		for(int iEpoch=0;iEpoch<iNumEpoch;iEpoch++)
			pEpochPositions[iEpoch]=pEpochPositions[iEpoch]+pFilePosInBuffer[i]; //Positions relative to the start of m_pAllCNTData
		AppendAllEpochData(m_pAllCNTData, m_ppAllEpochData, m_pAllEpochCode, m_iNumAllEpoch, pEpochPositions, iNumEpoch);
		delete pEpochPositions;
	}
	delete pFilePosInBuffer;
	delete pFileLength;
	return true;
}

bool CEEGContFile::GetSampleFileConfig(BYTE* pFileData, int iFileLength,int& iNumEpoch, int*& pEpochPositions, int iEpochCodeLB,int iEpochCodeUB)
{
	int i;
	iNumEpoch=0;
	int iDataLength=iFileLength-sizeof(CNTFileHeader);
    int iSegmentLength=m_CNTHeader.iNumCh+1;
	if(iDataLength%iSegmentLength!=0) return false;
	int iNumSegment=iDataLength/(iSegmentLength*sizeof(int));

	int* pSegment=new int[iSegmentLength];
	//Count the number of Epochs
	BYTE* pEEGData=pFileData+sizeof(CNTFileHeader);
	int iPreEpochCode=0,iCurEpochCode;
	for(i=0;i<iNumSegment;i++)
	{
		pSegment=(int*)pEEGData+i*iSegmentLength;
        iCurEpochCode = pSegment[iSegmentLength-1] & 0xff;
		if(!(iCurEpochCode>=iEpochCodeLB && iCurEpochCode<=iEpochCodeUB)) iCurEpochCode=0;//Discarding the epoch whose code lies out of the given range
		if (iCurEpochCode!=iPreEpochCode && iCurEpochCode !=0)	iNumEpoch++;
		iPreEpochCode=iCurEpochCode;
	}

	//Get Epochs Positions
	pEpochPositions = new int[iNumEpoch];
	pSegment = (int*)pEEGData;
	iPreEpochCode=0;
	int iCount=0;
	for(i=0;i<iNumSegment;i++)
	{
		pSegment=(int*)pEEGData+i*iSegmentLength;
        iCurEpochCode = pSegment[iSegmentLength-1] & 0xff;
		if(!(iCurEpochCode>=iEpochCodeLB && iCurEpochCode<=iEpochCodeUB)) iCurEpochCode=0;//Discarding the epoch whose code lies out of the given range
		if (iCurEpochCode!=iPreEpochCode && iCurEpochCode != 0){
			pEpochPositions[iCount] = (int) ((BYTE*)pSegment - pFileData);
			iCount++;
		}
		iPreEpochCode = iCurEpochCode;
	}
	
	return true;
}

bool CEEGContFile::AppendAllEpochData(BYTE* pData, BYTE**& ppAllEpochData, int*& pAllEpochCode, int& iNumAllEpoch, int* pNewEpochPositions, int iNumNewEpoch)
{	
	int i;

	int iSegmentLength=m_CNTHeader.iNumCh+1;
	int iNumExistingEpoch=m_iNumAllEpoch;
	if(iNumAllEpoch==0)//new all epoch data
	{
		ppAllEpochData=new BYTE*[iNumNewEpoch];
		pAllEpochCode=new int[iNumNewEpoch];
	}
	else{
        BYTE** ppOldAllEpochData=ppAllEpochData;
		ppAllEpochData=new BYTE*[iNumAllEpoch+iNumNewEpoch];
		memcpy(ppAllEpochData,ppOldAllEpochData,iNumAllEpoch*sizeof(BYTE*));
		delete ppOldAllEpochData;

        int* pOldAllEpochCode=pAllEpochCode;
		pAllEpochCode=new int[iNumAllEpoch+iNumNewEpoch];
		memcpy(pAllEpochCode,pOldAllEpochCode,iNumAllEpoch*sizeof(int));
		delete pOldAllEpochCode;
	}

	for(i=0;i<iNumNewEpoch;i++){
		ppAllEpochData[i+iNumAllEpoch]=pData+pNewEpochPositions[i];
		int* pData=(int*)ppAllEpochData[i+iNumAllEpoch];
		pAllEpochCode[i+iNumAllEpoch]=pData[iSegmentLength-1] & 0xff;

		///---------Epoch number recorded in continuous file counts from 1 rather than 0, so set it to 0 as below
		pAllEpochCode[i+iNumAllEpoch]--;
		///------------------------------------------------------------------------------------------------------

	}
	iNumAllEpoch+=iNumNewEpoch;
	return true;
}

bool CEEGContFile::LoadDataFileList(const char* strListFile)
{
	FILE* fp;
	if(fopen_s(&fp, strListFile,"r")) return false;
	char strBuffer[1024];
	char strTrainDir[256];
	char strTrainer[256];
	char strFileList[MAXFILELISTSIZE][256];
	char* strFNameList[MAXFILELISTSIZE];

	int iNumFiles;
	
	int itmppos,iLineWidth;
	char* pStrTmp;
	int i;

	//Scan for the TrainDir
	do{
		fgets(strBuffer,sizeof(strBuffer),fp);
	}while(i=strncmp(strBuffer,"TrainDir:",strlen("TrainDir:"))!=0);
	if(feof(fp)) return false;
	//remove the leading spaces and ending spaces
	pStrTmp=strBuffer+sizeof("TrainDir:");
	iLineWidth = (int) strlen(strBuffer);
	while(pStrTmp<strBuffer+iLineWidth && (*pStrTmp==' ' || *pStrTmp==9 /*TAB*/))  pStrTmp++;
	itmppos = (int)strlen(pStrTmp) - 1;
	while(pStrTmp[itmppos]==' ' || pStrTmp[itmppos]==9 /*TAB*/ || pStrTmp[itmppos]==10)  itmppos--;
	if(itmppos<0) return false;
    strncpy_s(strTrainDir,pStrTmp,itmppos+1);
	strTrainDir[itmppos+1]=0;

	//Scan for the Trainer
	do{
		fgets(strBuffer,sizeof(strBuffer),fp);
	}while(strncmp(strBuffer,"Trainer:",strlen("Trainer:"))!=0);
	if(feof(fp)) return false;
	//remove the leading spaces and ending spaces
	pStrTmp=strBuffer+sizeof("Trainer:");
	iLineWidth = (int) strlen(strBuffer);
	while(pStrTmp<strBuffer+iLineWidth && (*pStrTmp==' ' || *pStrTmp==9 /*TAB*/))  pStrTmp++;
	itmppos = (int) strlen(pStrTmp) - 1;
	while(pStrTmp[itmppos]==' ' || pStrTmp[itmppos]==9 /*TAB*/ || pStrTmp[itmppos]==10)  itmppos--;
	if(itmppos<0) return false;
    strncpy_s(strTrainer,pStrTmp,itmppos+1);
	strTrainer[itmppos+1]=0;

	//Scan for number of files
	do{
		fgets(strBuffer,sizeof(strBuffer),fp);
	}while(strncmp(strBuffer,"Number of Files:",strlen("Number of Files:"))!=0);
	if(feof(fp)) return false;
	//remove the leading spaces and ending spaces
	pStrTmp=strBuffer+sizeof("Number of Files:");
	iLineWidth = (int) strlen(strBuffer);
	while(pStrTmp<strBuffer+iLineWidth && (*pStrTmp==' ' || *pStrTmp==9 /*TAB*/))  pStrTmp++;
	itmppos = (int) strlen(pStrTmp) - 1;
	while(pStrTmp[itmppos]==' ' || pStrTmp[itmppos]==9 /*TAB*/ || pStrTmp[itmppos]==10)  itmppos--;
	if(itmppos<0) return false;
    sscanf_s(pStrTmp,"%d",&iNumFiles);

	//Reading File List
	for(i=0;i<iNumFiles;i++){
		int inumnullchar;
		 //skipping null lines
		do{
			fgets(strBuffer,sizeof(strBuffer),fp);
			inumnullchar=0;
			for(int j=0; j < (int)strlen(strBuffer); j++)
				inumnullchar=inumnullchar+(strBuffer[j]==' ' || strBuffer[j]==9)?1:0;
		}while(inumnullchar==strlen(strBuffer));
		if(feof(fp)) return false;

		//remove the leading spaces and ending spaces
		pStrTmp=strBuffer;
		iLineWidth = (int) strlen(strBuffer);
		while(pStrTmp<strBuffer+iLineWidth && (*pStrTmp==' ' || *pStrTmp==9 /*TAB*/))  pStrTmp++;
		itmppos = (int) strlen(pStrTmp) - 1;
		while(pStrTmp[itmppos]==' ' || pStrTmp[itmppos]==9 /*TAB*/ || pStrTmp[itmppos]==10)  itmppos--;
		if(itmppos<0) return false;
		strncpy_s(strFileList[i],pStrTmp,itmppos+1);
		strFileList[i][itmppos+1]=0;
		strFNameList[i]=strFileList[i];
		//Force the file extension to be 'cnt'
		strFNameList[i][strlen(strFNameList[i])-3]=0;
		strcat_s(strFNameList[i], 256, "cnt");
	}

	//Scan the number of words, should agree with number of files
	do{
		fgets(strBuffer,sizeof(strBuffer),fp);
	}while(strncmp(strBuffer,"Number of Words:",strlen("Number of Words:"))!=0);
	if(feof(fp)) return false;
	//remove the leading spaces and ending spaces
	pStrTmp=strBuffer+sizeof("Number of Words:");
	iLineWidth = (int) strlen(strBuffer);
	while(pStrTmp<strBuffer+iLineWidth && (*pStrTmp==' ' || *pStrTmp==9 /*TAB*/))  pStrTmp++;
	itmppos = (int) strlen(pStrTmp)-1;
	while(pStrTmp[itmppos]==' ' || pStrTmp[itmppos]==9 /*TAB*/ || pStrTmp[itmppos]==10)  itmppos--;
	if(itmppos<0) return false;
	int iNumWords;
    sscanf_s(pStrTmp, "%d", &iNumWords);
	if(iNumWords!=iNumFiles) return false;

	//Reading Word List
	m_iNumTarget=0;
	for(i=0;i<iNumFiles;i++){
		int inumnullchar;
		 //skipping null lines
		do{
			fgets(strBuffer,sizeof(strBuffer),fp);
			inumnullchar=0;
			for(int j=0;j < (int)strlen(strBuffer); j++) inumnullchar=inumnullchar+(strBuffer[j]==' ' || strBuffer[j]==9)?1:0;
		}while(inumnullchar==strlen(strBuffer));
		if(feof(fp)) return false;

		//remove the leading spaces and ending spaces
		pStrTmp=strBuffer;
		iLineWidth = (int) strlen(strBuffer);
		while(pStrTmp<strBuffer+iLineWidth && (*pStrTmp==' ' || *pStrTmp==9 /*TAB*/))  pStrTmp++;
		itmppos= (int) strlen(pStrTmp)-1;
		while(pStrTmp[itmppos]==' ' || pStrTmp[itmppos]==9 /*TAB*/ || pStrTmp[itmppos]==10)  itmppos--;
		if(itmppos<0) return false;
		int iLenStrTmp = (int) strlen(pStrTmp);
		for(int iChar=0;iChar<=itmppos;iChar++){
			m_TargetCharList[m_iNumTarget]=pStrTmp[iChar];
			m_iNumTarget++;
		}
	}
	fclose(fp); //ccwang
	return LoadDataFromFileList(strFNameList,strTrainDir,iNumFiles);
}

int CEEGContFile::GetTargetEpochPtrs(int iNumEpochPerRound,int iNumRoundPerTarget, int**& ppTargetEpoch, int*& pTargetEpochCode, char* pCharTable, int iCharTableSize)
{
	int iTargetEpochCount=0;
	int i,iEpoch,iTargetCode;
	int iNumRound=m_iNumAllEpoch/iNumEpochPerRound;
	ppTargetEpoch=new int*[iNumRound];
	pTargetEpochCode=new int[iNumRound];
	for(iEpoch=0;iEpoch<m_iNumAllEpoch;iEpoch++){
		int iTargetChar=iEpoch/(iNumEpochPerRound*iNumRoundPerTarget);
		char cTargetChar=m_TargetCharList[iTargetChar];
		for(i=0;i<iCharTableSize;i++){
			if(pCharTable[i]==cTargetChar) break;
		}
		if(i==iCharTableSize) {delete ppTargetEpoch; ppTargetEpoch=NULL; return -1;}
		iTargetCode=i;
		if(iTargetCode!=m_pAllEpochCode[iEpoch]) continue;
		pTargetEpochCode[iTargetEpochCount]=m_pAllEpochCode[iEpoch];
		ppTargetEpoch[iTargetEpochCount]=(int*)m_ppAllEpochData[iEpoch];
		iTargetEpochCount++;
	}
	return iTargetEpochCount;
}

int CEEGContFile::GetNonTargetEpochPtrs(int iNumEpochPerRound,int iNumRoundPerTarget, int**& ppNonTargetEpoch, int*& pNonTargetEpochCode, char* pCharTable, int iCharTableSize)
{
	int iNonTargetEpochCount=0;
	int i,iEpoch,iTargetCode;
	int iNumRound=m_iNumAllEpoch/iNumEpochPerRound;
	int iNumNonTargetEpoch=iNumRound*(iNumEpochPerRound-1);
	ppNonTargetEpoch=new int*[iNumNonTargetEpoch];
	pNonTargetEpochCode=new int[iNumNonTargetEpoch];

	for(iEpoch=0;iEpoch<m_iNumAllEpoch;iEpoch++){
		int iTargetChar=iEpoch/(iNumEpochPerRound*iNumRoundPerTarget);
		char cTargetChar=m_TargetCharList[iTargetChar];
		for(i=0;i<iCharTableSize;i++){
			if(pCharTable[i]==cTargetChar) break;
		}
		if(i==iCharTableSize) {delete ppNonTargetEpoch; ppNonTargetEpoch=NULL; return -1;}
		iTargetCode=i;
		if(iTargetCode==m_pAllEpochCode[iEpoch]) continue;
		pNonTargetEpochCode[iNonTargetEpochCount]=m_pAllEpochCode[iEpoch];
		ppNonTargetEpoch[iNonTargetEpochCount]=(int*)m_ppAllEpochData[iEpoch];
		iNonTargetEpochCount++;
		if(iNonTargetEpochCount>iNumNonTargetEpoch) {delete ppNonTargetEpoch; ppNonTargetEpoch=NULL; return -1;}
	}
	return iNonTargetEpochCount;
}

int CEEGContFile::GetEpochData(int iStartSample,int iNumSample, int** ppTargetEpoch, int iNumTargetEpoch, int**& ppTargetEpochData)
{
	int iSegmentLength=m_CNTHeader.iNumCh+1;
	int iEpochDataSize=m_CNTHeader.iNumCh*iNumSample;
	ppTargetEpochData=new int*[iNumTargetEpoch];
	for(int iEpoch=0;iEpoch<iNumTargetEpoch;iEpoch++){
		int* pEpoch=ppTargetEpoch[iEpoch];
		ppTargetEpochData[iEpoch]=new int[iEpochDataSize];
		for(int iSample=iStartSample;iSample<iStartSample+iNumSample;iSample++){
			int ioffset0=iSegmentLength*iSample;
			int ioffset1=m_CNTHeader.iNumCh*(iSample-iStartSample);
			for(int iCh=0;iCh<m_CNTHeader.iNumCh;iCh++){
				ppTargetEpochData[iEpoch][ioffset1+iCh]=pEpoch[ioffset0+iCh];
			}
		}
	}
	return 0;
}
