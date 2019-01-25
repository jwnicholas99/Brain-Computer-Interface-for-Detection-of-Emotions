#include "P300SignalProc.h"
#include "float.h"
#include "LALib.h"

#include "..\L_SVM\csvmtrain.h"

int FindMin(double* pData, int iLength);
int FindMax(double* pData, int iLength);

//#define _DIFFERENCE_MODEL_ 1

//================================================
//	P300SignalProc.cpp
//	Purpose: Class for P300 signal processing, used together
//			 with matlab training tools
//
//  Created by: Cuntai, 2 Feb 2004
//
//	Usage:
//		CP300SignalProc * pCP300SignalProc = new CP300SignalProc()
//		pCP300SignalProc->Initialize();	//used default resource file defined in NeuroComm_public.h
//		
//		pCP300SignalProc->Process(char *fEEGBuffer);
//=================================================

/*
char kszSpellerChar[] = {
	//'A','B','C','D','E','F','G','H','I','J','K','L','M',
	//'N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
	//'1','2','3','4','5','6','7','8','9',' '
	// ccwang
	'1', '2', '3', '4', '5', '6', '7', '8', '9', '0',
	'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J',
	'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T',
	'U', 'V', 'W', 'X', 'Y', 'Z', ',', '.', ':', '!',
	// control characters
	'\1', '\2', '\3', '\4'
};

char kszCards[] = {'2','3','4','5','6','7','8','9','0','J','Q','K','A'};
*/

CP300SignalProc::CP300SignalProc()
{
	mSpellerChars = NULL;
	mfEERRejectThrs = NULL;
	mfDesiredFalseRejThrs=NULL;
	mfGarbageProbMeans=NULL;
	mfTargetProbMeans=NULL;

	mpResources=NULL;
	mpCFiltering=NULL;
	mpPCATransform=NULL;
	mpDownSampTransform=NULL;
	mpDeltaTransform=NULL;
	mszChannelDef=NULL;

	mScoreBuff=NULL;
	mpSampUsedIndex=NULL;
	mpFeatures=NULL;
	mpCSVMClassify=NULL;
	mSpellerChars= NULL;
	miNumEpochPerRound=0;

	m_hOutputResult = NULL;
}

CP300SignalProc::~CP300SignalProc()
{
	if(mpResources)
		delete mpResources;
	if(mpCFiltering)
		delete mpCFiltering;
	if(mpPCATransform)
		delete mpPCATransform;
	if(mpDownSampTransform)
		delete mpDownSampTransform;
	if(mpDeltaTransform)
		delete mpDeltaTransform;
	if(mszChannelDef){
		for(int i=1;i<miNumTotalChannel;i++)
			free(mszChannelDef[i]);
		free(mszChannelDef);
	}

	if(mScoreBuff)
		delete [] mScoreBuff;
	
	if(mpSampUsedIndex)
		delete [] mpSampUsedIndex;

	if(mpFeatures)
		delete [] mpFeatures;

	if(mpCSVMClassify)
		delete mpCSVMClassify;

	if (mSpellerChars != NULL) { //&& mSpellerChars != kszSpellerChar
		delete [] mSpellerChars;
	}

	if(mfEERRejectThrs)
		delete [] mfEERRejectThrs;
}

bool CP300SignalProc::Initialize(LPCSTR mfn)
{
	Reset();

	char *cfn = (char *)mfn;

	mpResources = new Resources();
	if (mpResources->Merge(cfn) != errOkay){
		printf ("Can not open configuration file %s.\n",(char *)CONFIG_FILE_NAME);
		return false;
	}

	m_bVariLenDetectMode=false;
	char szDetectMode[kMaxLine];
	if (mpResources->Get ("EEG","DetectMode", szDetectMode,sizeof(szDetectMode))!= errOkay){
		printf ("EEG:DetectMode undefined. Use Fixed-length detection.\n");
		m_bVariLenDetectMode=false;
	}
	else{
		m_bVariLenDetectMode=(_strcmpi(szDetectMode,"VariLen")==0);
	}

	//if(m_bVariLenDetectMode){
	//	//Get config for variable length detection mode
	//	if (mpResources->Get ("Process","iMinLen_VariLen", &m_iMinLen_VariLen)!= errOkay){
	//		printf("EEG:iMinLen_VariLen undefined.\n");
	//		return false;
	//	}

	//	if (mpResources->Get ("Process","iMaxLen_VariLen", &m_iMaxLen_VariLen)!= errOkay){
	//		printf("EEG:iMaxLen_VariLen undefined.\n");
	//		return false;
	//	}
	//}

	miTaskType = WORDSPELLER ;
	miNumColumn = 1;
	if (mpResources->Get ("P300EEGSignal","NumColumn", &miNumColumn) != errOkay){
		printf ("P300EEGSignal:NumColumn  undefined, set to 1.\n");
		miNumColumn = 1;
	}

	char tmp[2048];
	//mSpellerChars = kszSpellerChar;
	if (mpResources->Get("P300EEGSignal","SpellerChars", tmp, 2048)!= errOkay){
		printf ("Error: %s - P300EEGSignal:SpellerChars undefined.\n", CONFIG_FILE_NAME);
		return false;
	} else {
		int n = 0;
		for (int i = 0; tmp[i]; i++) {
			if (tmp[i] == ',' && (i == 0 || tmp[i-1] != '\\')) n++;
		}
		n++;

		mSpellerChars = new char[n + 1];
		memset(mSpellerChars, 0, n + 1);
		int i0 = 0;

		for (int i = 0, k = 0; k < n; i++)	{
			if (tmp[i] == 0 || (tmp[i] == ',' && (i == 0 || tmp[i-1] != '\\'))) {
				tmp[i] = 0;
				while (isspace(tmp[i0])) i0++;
				if (tmp[i0] == '\\') {
					i0++;
					if (isdigit(tmp[i0])) mSpellerChars[k] = atoi(tmp + i0);
					else mSpellerChars[k] = tmp[i0];
				} else {
					mSpellerChars[k] = tmp[i0];
				}
				i0 = i + 1;
				k++;
			}
		}
	}

	if (mpResources->Get ("EEG","NumEpochPerRound", &miNumEpochPerRound)!= errOkay){
		printf ("EEG:NumEpochPerRound undefined\n");
		return false;
	}
	mScoreBuff = new double [miNumEpochPerRound];


	char szFilename[kMaxLine];
	if (mpResources->Get("Process","P300ParaFile",szFilename, sizeof(szFilename)) != errOkay) {
		if (mpResources->Get("Process","ParameterFile",szFilename, sizeof(szFilename)) != errOkay) {
			printf("Parameter file must be given in configuration file %s under Process:P300ParaFile.\n", CONFIG_FILE_NAME);
			return false;
		}
	}

	bool status =  LoadParameters(szFilename);

	status = status | InitSVM();

	status = status | InitBuffers();

	m_iRejThrBias=0;
	char strBuffer[256];
	if (mpResources->Get ("Process","RejectionThresholdBias",strBuffer,sizeof(strBuffer)))
		printf("RejectionThresholdBias not defined in %s\n", (char *)CONFIG_FILE_NAME);
	else
		sscanf_s(strBuffer, "%d", &m_iRejThrBias);

	m_fRejThreshold_ManualSet = FLT_MIN;

	float fff = 0;
	if (mpResources->Get ("Process","RejectionThreshold_ManualSet", &fff)) {
	} else if (fff != 0) {
		m_fRejThreshold_ManualSet = fff;
	}

	printf("m_fRejThreshold_ManualSet====%f, %lf.\n", fff, m_fRejThreshold_ManualSet);

	return status;
}

void CP300SignalProc::Reset()
{
	mpResources = NULL;
	mpCFiltering = NULL;
	mpPCATransform = NULL;
	mpDownSampTransform = NULL;
	mpDeltaTransform = NULL;
	mszChannelDef = NULL;
}


bool CP300SignalProc::LoadParameters(char *szFilename)
{
	char szString[kMaxLine], szStr[kMaxLine], *context;
	FILE *fp;

	if(fopen_s(&fp, szFilename,"r")){
		printf("Cannot open parameter file %s", szFilename);
		return false;
	}

	if(fgets(szString,sizeof(szString) ,fp) != NULL)
		sscanf_s(szString,"%s %d",szStr, kMaxLine - 1, &miSamplingRate);	//Sampling_Rate
	if(fgets(szString,sizeof(szString) ,fp) != NULL)
		sscanf_s(szString,"%s %g",szStr, kMaxLine - 1, &mfResolution);	//EEG_Resolution
	if(fgets(szString,sizeof(szString) ,fp) != NULL)
		sscanf_s(szString,"%s %f",szStr, kMaxLine - 1, &mfRejectThreshold);	//Reject_Threshold
	if(fgets(szString,sizeof(szString) ,fp) != NULL)
		sscanf_s(szString,"%s %d",szStr, kMaxLine - 1, &miP300ProcStartTime);	//P300_Processing_Start_Time
	if(fgets(szString,sizeof(szString) ,fp) != NULL)
		sscanf_s(szString,"%s %d",szStr, kMaxLine - 1, &miP300ProcEndTime);	//P300_Processing_End_Time
	if(fgets(szString,sizeof(szString) ,fp) != NULL)
		sscanf_s(szString,"%s %d",szStr, kMaxLine - 1, &miNumSampInRawEpochs);	//Number_Samples_in_Raw_Epochs
	// manoj 8/3/2004
	if(fgets(szString,sizeof(szString) ,fp) != NULL)
		sscanf_s(szString,"%s %d", szStr, kMaxLine - 1, &miUseDelta);	//UseDetla

	if(fgets(szString,sizeof(szString) ,fp) != NULL){
		sscanf_s(szString,"%s %d", szStr, kMaxLine - 1, &miNumTotalChannel);	//Channel_Order
		mszChannelDef = (char **) malloc(miNumTotalChannel * sizeof(char *));
		fgets(szString,sizeof(szString) ,fp);
		char *szToken = strtok_s(szString, SEPS, &context);
		for (int i=0;i<miNumTotalChannel;i++){
			mszChannelDef[i] = _strdup(szToken);
			szToken = strtok_s(NULL, SEPS, &context);
		}
	}

	InitChnlSampIndex(fp);
	InitFilterTransform(fp);

	InitRejection(fp);

	fclose(fp);

	return true;
}


int CP300SignalProc::GetChannel(char *szChannelName)
{
	for (int i=0;i<miNumTotalChannel;i++){
		if(strcmp(mszChannelDef[i],szChannelName) == 0)
			return i;
	}

	return -1;
}

bool CP300SignalProc::InitChnlSampIndex(FILE * fp)
{
	mpChannelUsedIndex = new int [miNumTotalChannel];
	memset(mpChannelUsedIndex,0,miNumTotalChannel*sizeof(int));

	char szString[kMaxLine], szStr[kMaxLine], *context;
	int iVal;
	if(fgets(szString,sizeof(szString) ,fp) != NULL){
		sscanf_s(szString,"%s %d",szStr, kMaxLine - 1, &miNumChannelUsed);	//Indice of channels used 
		fgets(szString,sizeof(szString) ,fp);
		char *szToken = strtok_s(szString, SEPS, &context);
		for (int i=0;i<miNumChannelUsed;i++){
			iVal = atoi(szToken);
			mpChannelUsedIndex[iVal-1] = 1;		//Flag: if a channel is used, flag = 1, otherwise = 0
			szToken = strtok_s(NULL, SEPS, &context);
		}
	}

	mpSampUsedIndex = new int [miNumSampInRawEpochs];
	memset(mpSampUsedIndex,0,miNumSampInRawEpochs*sizeof(int));
	if(fgets(szString,sizeof(szString) ,fp) != NULL){
		sscanf_s(szString,"%s %d",szStr, kMaxLine - 1, &miNumSampleUsed);	//Indice of samples used 
		fgets(szString,sizeof(szString) ,fp);
		char *szToken = strtok_s(szString, SEPS, &context);
		for (int i=0;i<miNumSampleUsed;i++){
			iVal = atoi(szToken);
			mpSampUsedIndex[iVal-1] = 1;		//Flag: if a sample is used, flag = 1, otherwise = 0
			szToken = strtok_s(NULL, SEPS, &context);
		}
	}

	return true;
}

bool CP300SignalProc::InitFilterTransform(FILE *fp)
{
	mpCFiltering = new CFiltering();
	char szString[kMaxLine], szStr[kMaxLine], *context;
	double fMyVal;
	if(fgets(szString,sizeof(szString) ,fp) != NULL){
		sscanf_s(szString,"%s %d", szStr, kMaxLine - 1, &miFilterOrderOfB);
		mpCFiltering->InitB(miFilterOrderOfB);

		fgets(szString,sizeof(szString) ,fp);
		char *szToken = strtok_s(szString, SEPS, &context);
		for(int i=0;i<miFilterOrderOfB;i++){
			fMyVal = atof(szToken);
			mpCFiltering->SetBPara(fMyVal, i);
			szToken = strtok_s(NULL, SEPS, &context);
		}

	}
	if(fgets(szString,sizeof(szString) ,fp) != NULL){
		sscanf_s(szString,"%s %d", szStr, kMaxLine - 1, &miFilterOrderOfA);
		mpCFiltering->InitA(miFilterOrderOfA);

		fgets(szString,sizeof(szString) ,fp);
		char *szToken = strtok_s(szString, SEPS, &context);
		for(int i=0;i<miFilterOrderOfA;i++){
			fMyVal = atof(szToken);
			mpCFiltering->SetAPara(fMyVal,i);
			szToken = strtok_s(NULL, SEPS, &context);
		}
	}

	mpPCATransform = new CTransformat();
	if(fgets(szString,sizeof(szString) ,fp) != NULL){
		sscanf_s(szString,"%s %d, %d", szStr, kMaxLine - 1, &miNumPCAMatrixRow, &miNumPCAMatrixCol);
		mpPCATransform->LoadMatrix(miNumPCAMatrixRow, miNumPCAMatrixCol, fp);
	}

	mpDownSampTransform = new CTransformat();
	if(fgets(szString,sizeof(szString) ,fp) != NULL){
		sscanf_s(szString,"%s %d, %d", szStr, kMaxLine - 1, &miNumDownSampTransRow, &miNumDownSampTransCol);
		mpDownSampTransform->LoadMatrix(miNumDownSampTransRow, miNumDownSampTransCol, fp);
	}

	mpDeltaTransform = new CTransformat();
	if(fgets(szString,sizeof(szString) ,fp) != NULL){
		sscanf_s(szString,"%s %d, %d", szStr, kMaxLine - 1, &miNumDeltaTransRow, &miNumDeltaTransCol);
		mpDeltaTransform->LoadMatrix(miNumDeltaTransRow, miNumDeltaTransCol, fp);
	}

	return true;
}


bool CP300SignalProc::InitBuffers()
{
	mpSegment = new float * [miNumTotalChannel];
	for(int i=0;i<miNumTotalChannel;i++)
//		mpSegment[i] = new float [miNumSampInRawEpochs];
		mpSegment[i] = new float [miNumSampInRawEpochs*2]; //*2 to assure enough space for augment in DeltaTransform->ProcessVect

	mpFeatures = new double [miNumPCAMatrixRow * miNumDownSampTransRow * 2 ];

	return true;
}

bool CP300SignalProc::InitSVM()
{
	char szModelFile[kMaxLine], szRangeFile[kMaxLine];
	if(mpResources->Get("Process","SVMModel",szModelFile, sizeof(szModelFile)) != errOkay){
		printf ("SVM model must be given in configuration file %s under P300Process:SVMModel.\n",(char *)CONFIG_FILE_NAME);
		return false;
	}

	if(mpResources->Get("Process","SVMRange",szRangeFile, sizeof(szRangeFile)) != errOkay){
		printf ("Range file must be given in configuration file %s under P300Process:SVMRange.\n",(char *)CONFIG_FILE_NAME);
		return false;
	}

	mpCSVMClassify = new CSvmClassify();

	return mpCSVMClassify->Initialize(szModelFile,szRangeFile);
}


double CP300SignalProc::ProcessEEGBuffer(char *fInEEGBuffer)
{
	CopyRawBuffer((int *)fInEEGBuffer);	//eg, eeg between 0-500, all channel   *** CRASH ***
	return ProcessFullCh(mpSegment);        // *** CRASH ***
}

double CP300SignalProc::ProcessFullCh(float **pInSegment)
{
	//int iLeftEOGChnl = GetChannel("VEO");
	//if (iLeftEOGChnl >= 0)
	//	RemoveEOG(pInSegment,iLeftEOGChnl);

	//int iRightEOGChnl = GetChannel("HEO");
	//if (iRightEOGChnl >=0)
	//	RemoveEOG(pInSegment,iRightEOGChnl);	// *** CRASH ***

	SelectChannels(pInSegment);
	return Process(pInSegment);
}

//===============================================
//Processing slected channels
//Input: pInSegment -- Channel X Sample
//===============================================
double CP300SignalProc::Process(float **pInSegment)
{
	mFeatureSize = ExtractFeatures(pInSegment);

	double fMargin, fPredict;
	mpCSVMClassify->Classify(mpFeatures, mFeatureSize, &fMargin, &fPredict);

	return fMargin;
}

void CP300SignalProc::SelectChannels(float **pInSegment)
{
	int iChId = 0; 
	for(int iCh = 0; iCh<miNumTotalChannel; iCh++){
		if(mpChannelUsedIndex[iCh] == 1){
			if (iCh != iChId) {
				for(int iSamp=0;iSamp<miNumSampInRawEpochs;iSamp++)
					pInSegment[iChId][iSamp] = pInSegment[iCh][iSamp];
			}
			iChId ++;
		}
	}
}

void CP300SignalProc::CutSegment(float **pInSegment)
{
	int iSamp;
	int iSampId = 0;
	for (iSamp=0; iSamp < miNumSampInRawEpochs; iSamp++) {
		if (mpSampUsedIndex[iSamp] == 0)	//skip non-relavant samples
			continue;

		for (int iCh = 0; iCh < miNumChannelUsed; iCh++) {
			pInSegment[iCh][iSampId] = pInSegment[iCh][iSamp];
		}
		iSampId++;
	}

	// ccwang -- modified 2006/08/15
	for (int iCh = 0; iCh < miNumChannelUsed; iCh++) {
		double fMean=0;
		for (iSamp=0; iSamp < iSampId; iSamp++) {
			fMean += pInSegment[iCh][iSamp];
		}
		fMean /= iSampId;
		for (iSamp=0; iSamp < iSampId; iSamp++) {
			pInSegment[iCh][iSamp] -= (float) fMean;
		}
	}
}

void CP300SignalProc::RemoveEOG(float **pInSegment, int iRefChannel)
{
	float fMean = 0, fMax= -10000000, fMin = 10000000, fNorm = 0, fMyVal;
	float *pDiffRef = &mfBuffer1[0];
	pDiffRef[0] = pInSegment [iRefChannel][0];
	for(int iSamp=0;iSamp<miNumSampInRawEpochs;iSamp++){
		fMyVal = pInSegment [iRefChannel][iSamp];
		if (fMyVal > fMax) fMax = fMyVal;
		if (fMyVal < fMin) fMin = fMyVal;
		fMean += fMyVal;
#if 1  // _DIFFERENCE_MODEL_
		float fVal2 = fMyVal;
		if(iSamp >0){
			pDiffRef[iSamp] = fMyVal - pInSegment [iRefChannel][iSamp-1];
			fVal2 = pDiffRef[iSamp];
		}
		fNorm += fVal2 * fVal2;
#else
		fNorm += fMyVal * fMyVal;
#endif
	}

	fMean /= miNumSampInRawEpochs;
    fMax = fMax - fMean;
    fMin = fMin - fMean;

	if((fMax > mfRejectThreshold) || (fMin < -mfRejectThreshold)){
		for(int iCh = 0; iCh<miNumTotalChannel;iCh++){
			if(iCh == iRefChannel)
				continue;

			float fAlpha = 0;

#if	1	// _DIFFERENCE_MODEL_ //difference model
			float *pDiffSig = &mfBuffer2[0];
			pDiffSig[0] = pInSegment [iCh][0];
			for(int iSamp=1;iSamp<miNumSampInRawEpochs;iSamp++)
				pDiffSig[iSamp] = pInSegment [iCh][iSamp]-pInSegment [iCh][iSamp-1];

			for(int iSamp=0;iSamp<miNumSampInRawEpochs;iSamp++)
				fAlpha += pDiffSig[iSamp] * pDiffRef[iSamp];

			fAlpha /= fNorm;
			for(int iSamp=0;iSamp<miNumSampInRawEpochs;iSamp++)			
				pInSegment [iCh][iSamp] = pDiffSig[iSamp] - fAlpha * pDiffRef[iSamp];	
#else	
			for(int iSamp=0;iSamp<miNumSampInRawEpochs;iSamp++)
				fAlpha += pInSegment [iCh][iSamp] * pInSegment[iRefChannel][iSamp];
			fAlpha /= fNorm;
			for(int iSamp=0;iSamp<miNumSampInRawEpochs;iSamp++)			
				pInSegment [iCh][iSamp] -= fAlpha * pInSegment[iRefChannel][iSamp];	

#endif

		}
	}

}

int CP300SignalProc::CreateFeatures(float **pInSegment)
{
	int iFeatureSize = 0;
	// manoj 8/3/2004
	int iSampPerCh = miNumDownSampTransRow ;
	if (miUseDelta) iSampPerCh = miNumDownSampTransRow * 2;
	for (int iCh =0;iCh<miNumPCAMatrixRow;iCh++) {
		float *pSegment = pInSegment[iCh];
		for (int iSamp = 0; iSamp< iSampPerCh; iSamp++) {
			mpFeatures[iFeatureSize] = *pSegment++;
			iFeatureSize++;
		}
	}
	return iFeatureSize;
}


void CP300SignalProc::GetResult(double *Score, unsigned short *cStimCode, 
								int iNumStimPerRound, int iNumRound,RESULT *pResult)
{
	//int iTaskType;
	//iTaskType = GetTaskType();
	//if (iTaskType == WORDSPELLER) 
		GetResultWordSpeller(Score, cStimCode, iNumStimPerRound, iNumRound, pResult);
	//else if (iTaskType == CARDGAME) 
	//	GetResultCardGame(Score, cStimCode, iNumStimPerRound, iNumRound, pResult);
	//else
	//	printf ("P300SignalProc::GetResult: Task undefined in %d\n", iTaskType);

	if(m_fRejThreshold_ManualSet != FLT_MIN){
		m_fThd = m_fRejThreshold_ManualSet;
		IsRejection(Score ,cStimCode, iNumStimPerRound, iNumRound, pResult);
		return;
	}
	if (mfDesiredFalseRejThrs) {
		double fThd = log(mfDesiredFalseRejThrs[iNumRound-1]);
		double fTargetThd;
		if (m_iRejThrBias < 0)  fTargetThd=log(mfGarbageProbMeans[iNumRound-1]);
		else  fTargetThd = log(mfTargetProbMeans[iNumRound-1]);
		m_fThd = fThd + (fTargetThd-fThd)*abs(m_iRejThrBias)/100;
		IsRejection(Score ,cStimCode, iNumStimPerRound, iNumRound, pResult);
		return;
	}

	if (mfEERRejectThrs) {
		double fEERThd=log(mfEERRejectThrs[iNumRound-1]);
		double fTargetThd = fEERThd;
		if(m_iRejThrBias < 0)  fTargetThd = log(mfGarbageProbMeans[iNumRound-1]);
		else if(m_iRejThrBias > 0)  fTargetThd = log(mfTargetProbMeans[iNumRound-1]);
		m_fThd = fEERThd + (fTargetThd-fEERThd) * abs(m_iRejThrBias)/100;
		IsRejection(Score ,cStimCode, iNumStimPerRound, iNumRound, pResult);
	}

	return;
}

void CP300SignalProc::GetResultWordSpeller(double *Score, unsigned short *cStimCode, 
								int miNumStimPerRound, int iNumRound,RESULT *pResult)
{
	int iNumStimPerChar = miNumStimPerRound * iNumRound;
	for (int iStim = 0; iStim < miNumStimPerRound; iStim++) mScoreBuff[iStim] = 0;

	for (int iStim = 0; iStim < iNumStimPerChar; iStim++)
		mScoreBuff[cStimCode[iStim]-1] += Score[iStim];

	int iBestCol = 0;
	double fBestColScore = mScoreBuff[0];
	for (int iCol = 1; iCol < miNumColumn; iCol++) {
		if (mScoreBuff[iCol] > fBestColScore) {
			fBestColScore = mScoreBuff[iCol];
			iBestCol = iCol;
		}
	}

	int iBestRow = miNumColumn;
	double fBestRowScore = 0;
	if (miNumColumn < miNumStimPerRound) { // more than one row
		fBestRowScore = mScoreBuff[iBestRow];
		for (int iRow = miNumColumn + 1; iRow < miNumStimPerRound; iRow++) {
			if (mScoreBuff[iRow] > fBestRowScore){
				fBestRowScore = mScoreBuff[iRow];
				iBestRow = iRow;
			}
		}
	}

	pResult->iResult = (iBestRow - miNumColumn) * miNumColumn + iBestCol + 1;
	pResult->szResult = mSpellerChars[pResult->iResult - 1];
	pResult->fConfidence = (float) (fBestColScore + fBestRowScore);
	pResult->iReject = 0;

	// ccwang: copy the scores
	if (miNumStimPerRound < sizeof(pResult->fDispSegments)) {
		pResult->iNumSegments = 1;
		pResult->iSegmentLength = miNumStimPerRound;
		for (int iStim = 0; iStim < miNumStimPerRound; iStim++) {
			pResult->fDispSegments[iStim] = (float) mScoreBuff[iStim];
		}
	}

	return;
}

/*
void CP300SignalProc::GetResultCardGame(double *Score, unsigned short *cStimCode, 
								int miNumStimPerRound, int iNumRound,RESULT *pResult)
{
	int iNumStimPerChar = miNumStimPerRound * iNumRound;
	memset(mScoreBuff,0,miNumEpochPerRound*sizeof(double));
	for(int iStim=0;iStim<iNumStimPerChar;iStim++){
		mScoreBuff[cStimCode[iStim]-1] += Score[iStim];
	}

	int iBestCard = 0;
	double fBestCardScore = -10000;
	for (int iCard=0;iCard<miNumStimPerRound;iCard++){
		if (mScoreBuff[iCard] > fBestCardScore){
			fBestCardScore = mScoreBuff[iCard];
			iBestCard = iCard;
		}
	}

	pResult->iResult = iBestCard;
	pResult->szResult = (char) kszCards[iBestCard];
	pResult->fConfidence = (float) fBestCardScore;
	pResult->iReject = 0;

	return;
}
*/


bool CP300SignalProc::InitRejection(FILE *fp)
{
	printf("Call InitRejection ...\n");
	float fTargetGaussMean, fTargetGaussStd;
	float fNonTargetGaussMean, fNonTargetGaussStd;

	char szString[kMaxLine], szStr[kMaxLine];
	int iVal;
	char *szToken, *context;
	float fVal;

	if(fgets(szString,sizeof(szString) ,fp) == NULL) return false;
	sscanf_s(szString,"%s %d",szStr, kMaxLine - 1,&iVal);	//Rejection_EER_Threshold_for_Multiple_Repeatition 
	miNumRejectThrs=iVal;
	mfEERRejectThrs = new double [miNumRejectThrs];
	fgets(szString,sizeof(szString) ,fp);
	szToken = strtok_s(szString, SEPS, &context);
	for (int i=0;i<iVal;i++){
		double fVal = (double) atof(szToken);
		szToken = strtok_s(NULL, SEPS, &context);
		mfEERRejectThrs[i] = fVal;
	}

	if(fgets(szString,sizeof(szString) ,fp) == NULL) return false;
	sscanf_s(szString,"%s %d",szStr, kMaxLine - 1,&iVal);	//Rejection_Threshold_for_DesiredFalseRejRate 
	miNumRejectThrs=iVal;
	mfDesiredFalseRejThrs = new double [miNumRejectThrs];
	fgets(szString,sizeof(szString) ,fp);
	szToken = strtok_s(szString, SEPS, &context);
	for (int i=0;i<iVal;i++){
		double fVal = (double) atof(szToken);
		szToken = strtok_s(NULL, SEPS, &context);
		mfDesiredFalseRejThrs[i] = fVal;
	}

	if(fgets(szString,sizeof(szString) ,fp) == NULL) return false;
	sscanf_s(szString,"%s %d",szStr, kMaxLine - 1,&iVal);	//Mean_Prob_Target
	if(iVal!=miNumRejectThrs){printf("Number of Rejection Threshold does not agree with that of Mean_Prob_Target.\n");return false;}
	mfTargetProbMeans = new double [miNumRejectThrs];
	fgets(szString,sizeof(szString) ,fp);
	szToken = strtok_s(szString, SEPS, &context);
	for (int i=0;i<iVal;i++){
		double fVal = (double) atof(szToken);
		szToken = strtok_s(NULL, SEPS, &context);
		mfTargetProbMeans[i] = fVal;
	}

	if(fgets(szString,sizeof(szString) ,fp) == NULL) return false;
	sscanf_s(szString,"%s %d",szStr, kMaxLine - 1,&iVal);	//Mean_Prob_Garbage
	if(iVal!=miNumRejectThrs){printf("Number of Rejection Threshold does not agree with that of Mean_Prob_Garbage.\n");return false;}
	mfGarbageProbMeans = new double [miNumRejectThrs];
	fgets(szString,sizeof(szString) ,fp);
	szToken = strtok_s(szString, SEPS, &context);
	for (int i=0;i<iVal;i++){
		double fVal = (double) atof(szToken);
		szToken = strtok_s(NULL, SEPS, &context);
		mfGarbageProbMeans[i] = fVal;
	}

	if(fgets(szString,sizeof(szString) ,fp) == NULL) return false;
	sscanf_s(szString,"%s %d",szStr, kMaxLine - 1,&iVal);	//Target_Gaussian_Mean_Std 
	fgets(szString,sizeof(szString) ,fp);
	szToken = strtok_s(szString, SEPS, &context);
	fVal = (float) atof(szToken);
	szToken = strtok_s(NULL, SEPS, &context);
	fTargetGaussMean = fVal;

	fVal = (float) atof(szToken);
	szToken = strtok_s(NULL, SEPS, &context);
	fTargetGaussStd = fVal;

	if(fgets(szString,sizeof(szString) ,fp) == NULL) return false;
	sscanf_s(szString,"%s %d",szStr, kMaxLine - 1,&iVal);	//Non_Target_Gaussian_Mean_Std 
	fgets(szString,sizeof(szString) ,fp);
	szToken = strtok_s(szString, SEPS, &context);
	fVal = (float) atof(szToken);
	szToken = strtok_s(NULL, SEPS, &context);
	fNonTargetGaussMean = fVal;

	fVal = (float) atof(szToken);
	szToken = strtok_s(NULL, SEPS, &context);
	fNonTargetGaussStd = fVal;

	double fstd=(fTargetGaussStd+fNonTargetGaussStd)/2;
	m_fA = -(fTargetGaussMean-fNonTargetGaussMean)/(fstd*fstd);
	m_fB = -(fNonTargetGaussMean*fNonTargetGaussMean-fTargetGaussMean*fTargetGaussMean)/(2*fstd*fstd);

	char strModelScoreFile[1024];
	if(mpResources->Get("Process","SVMModelScore",strModelScoreFile, sizeof(strModelScoreFile)) != errOkay){
		printf ("SVM model score file shall be given in configuration file %s under P300Process:SVMModelScore.\n",(char *)CONFIG_FILE_NAME);
		printf ("Thus no rejection calibration/re-train will be carried out");
	}
	else{
		int iOrg_NumEpochPerRound;
		FILE* fpScore;
		if (fopen_s(&fpScore, strModelScoreFile,"rb")) {
			printf("ERROR!!!!! Cann't open the SVMModelScore file specified in the configuration.\n");
			return false;
		}
		fread(&iOrg_NumEpochPerRound,sizeof(int),1,fpScore);
		fclose(fpScore);

		if(miNumEpochPerRound==0){printf("ERROR: miNumEpochPerRound not loaded before initializing rejection module!\n");return false;}

		// ccwang: something wrong when miNumEpochPerRound is smaller than old one.
		if (iOrg_NumEpochPerRound < miNumEpochPerRound) {
			printf("####InitRejection: RETRAINING THRESHOLD FOR DIFFERENT # TARGETS\n");
			RetrainRejThr_For_DiffNumTarget();
			printf("####InitRejection: Finished.\n");
		}
	}

	if(m_bVariLenDetectMode){
		int nVariLenPara;
		if(fgets(szString,sizeof(szString) ,fp) == NULL) return false;
		sscanf_s(szString,"%s %d",szStr, kMaxLine - 1,&nVariLenPara);

		if(_strcmpi(szStr,"VariLenDetectParas")!=0) return false;
		for(int iVariLenPara=0;iVariLenPara<nVariLenPara;iVariLenPara++){
			if(fgets(szString,sizeof(szString) ,fp) == NULL) return false;
			sscanf_s(szString,"%s %f",szStr, kMaxLine - 1,&fVal);
			if (_strcmpi(szStr,"MeanScore_TargetEpoch")==0) m_EpochScoreStats.fMean_Target=fVal;
			else if(_strcmpi(szStr,"MeanScore_NonTargetEpoch")==0) m_EpochScoreStats.fMean_NonTarget=fVal;
			else if(_strcmpi(szStr,"MeanScore_IdleEpoch")==0) m_EpochScoreStats.fMean_Idle=fVal;
			else if(_strcmpi(szStr,"StdScore_TargetEpoch")==0) m_EpochScoreStats.fStd_Target=fVal;
			else if(_strcmpi(szStr,"StdScore_NonTargetEpoch")==0) m_EpochScoreStats.fStd_NonTarget=fVal;
			else if(_strcmpi(szStr,"StdScore_IdleEpoch")==0) m_EpochScoreStats.fStd_Idle=fVal;
			else if(_strcmpi(szStr,"L_Control_VariLen")==0) m_fL_Control_VariLen=fVal;
			else if(_strcmpi(szStr,"L_Idle_VariLen")==0) m_fL_Idle_VariLen=fVal;
			else if(_strcmpi(szStr,"MaxLen_VariLen")==0) m_iMaxLen_VariLen=(int)fVal;
			else if(_strcmpi(szStr,"MinLen_VariLen")==0) m_iMinLen_VariLen=(int)fVal;
			else if(_strcmpi(szStr,"EERDetectThr_VariLen")==0) m_fEERDetectThr_VariLen=fVal;
		}
	}

	return true;
}

double CP300SignalProc::CalcPostProb(double *fSVMScores ,int nStim,int nRound)
{
	int i,j,idx,iStim,iRound;
	double *pfPProbScores;
	pfPProbScores= new double [nStim*nRound];
	double *pfNProbScores;
	pfNProbScores = new double [nStim*nRound];

	for(i=0;i<nStim;i++){
		for(j=0;j<nRound;j++){
			idx=i+j*nStim;
			pfPProbScores[idx]=1/(1+(nStim-1)* exp(m_fA*fSVMScores[idx]+m_fB));
			pfNProbScores[idx]=1-pfPProbScores[idx];
//			if(pfPProbScores[idx]<MINPROB) pfPProbScores[idx]=MINPROB;
//			if(pfNProbScores[idx]<MINPROB) pfNProbScores[idx]=MINPROB;
			pfPProbScores[idx]=log(pfPProbScores[idx]);
			pfNProbScores[idx]=log(pfNProbScores[idx]);
		}
	}
	double *pfStimProbs =new double [nStim];
	double fRoundProb=0.0;
	for(iStim=0;iStim<nStim;iStim++){
		pfStimProbs[iStim]=0;
		for(iRound=0;iRound<nRound;iRound++){
			fRoundProb=0;
			for(i=0;i<nStim;i++){
				idx=i+iRound*nStim;
				if(i==iStim) fRoundProb=fRoundProb+pfPProbScores[idx];
				else fRoundProb=fRoundProb+pfNProbScores[idx];
			}
			pfStimProbs[iStim]+=fRoundProb;
		}
	}
	double fPostProb=0;
	for(iStim=0;iStim<nStim;iStim++){
		fPostProb +=exp(pfStimProbs[iStim]);
	}
	delete pfPProbScores;
	delete pfNProbScores;
	delete pfStimProbs;

	return fPostProb;
}


int CP300SignalProc::IsRejection(double *fSVMScores ,unsigned short *cStimCode, int nStim,int nRound, RESULT *result)
{
	int iNumStimPerChar = nStim * nRound;
	double* pScoreBufferInOrder=new double[nStim*nRound];
	memset(pScoreBufferInOrder,0,sizeof(double)*nStim*nRound);
	int idx_roundstart,idx,istimcode;
	for(int iRound=0;iRound<nRound;iRound++){
        idx_roundstart=iRound*nStim;
		for(int iStim=0;iStim<nStim;iStim++){
			idx=idx_roundstart+iStim;
			istimcode=cStimCode[idx]-1;
			pScoreBufferInOrder[idx_roundstart+istimcode]=fSVMScores[idx];
			//Normalize scores
			pScoreBufferInOrder[idx_roundstart+istimcode]=(pScoreBufferInOrder[idx_roundstart+istimcode]+1)*100;
		}
	}

	double fPostProb=log(CalcPostProb(pScoreBufferInOrder,nStim,nRound));
	int iIsReject = 0;
	if (fPostProb < m_fThd)
		iIsReject = 1;

	result->iReject = iIsReject;
	result->fConfidence = (float) fPostProb;	// log scale
	result->fResult = (float) m_fThd; //borrow this variable to store threshold, log scale

	delete pScoreBufferInOrder;
	return iIsReject;
}

/// ----------------------------------------------------------------------------
/// The following functions are coded by Zhang Haihong for training classifiers
/// vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv

bool CP300SignalProc::LoadSysConfigFile(const char* strConfigFile)
{
	// ccwang: mpResources maybe open already.
	if (mpResources == NULL) {
		mpResources = new Resources;
		if (mpResources->Merge ((char*)strConfigFile)){
			printf ("Can not open configuration file\n");
			return false;
		}
	}

	if (mpResources->Get ("EEG","SamplingRate", &miSamplingRate)){
		//printf ("SamplingRate not specified.\n");
		//return false;
	}

	if (mpResources->Get("EEG", "Resolution", &mfResolution)){
		//printf ("Resolution not specified.\n");
		//return false;
	}

	if (mpResources->Get ("EEG","PreStimDuration", &miP300RecordStartTime)){
		//printf ("EEG.PreStimDuration undefined in %s\n", strConfigFile);
		//return false;
		miP300RecordStartTime = 0;
	}

	if (mpResources->Get ("ModelTrainingSetting","StartProcTimeAftStim", &miP300ProcStartTime)){
		//printf ("ModelTrainingSetting:StartProcTimeAftStim undefined in %s\n", strConfigFile);
		//return false;
		miP300ProcStartTime = 150;
	}
	printf("StartProcTimeAftStim = %d\n", miP300ProcStartTime);
	miP300ProcStartTime -= miP300RecordStartTime; //Now using relative time to the start of recording

	if (mpResources->Get ("ModelTrainingSetting", "EndProcTimeAftStim", &miP300ProcEndTime)){
		//printf ("EEG:PostStimDuration undefined in %s\n", strConfigFile);
		//return false;
		miP300ProcEndTime = 500;
	}
	printf("EndProcTimeAftStim = %d\n", miP300ProcEndTime);
	miP300ProcEndTime -= miP300RecordStartTime; //Now using relative time to the start of recording

	if (mpResources->Get ("EEG", "NumEpochPerRound", &miNumEpochPerRound)) {
		printf ("EEG:NumEpochPerRound undefined\n");
		return false;
	}

	if (mpResources->Get ("EEG","NumRound", &miNumRoundPerTarget)){
		printf ("EEG:NumRoundPerTarget undefined\n");
		return false;
	}

	char strBuffer[1024],strBuffer1[1024];

	if (mpResources->Get("EEG","ChannelNames", strBuffer,sizeof(strBuffer))){
		printf ("EEG:Name of Channels unspecified. %s\n", strConfigFile);
		return false;
	}

	char seps[]   = ",\n";
	char *token, *context;

	miNumTotalChannel = 0;
	memcpy(strBuffer1,strBuffer,sizeof(strBuffer1));
	token = strtok_s(strBuffer1, seps, &context);
	while (token != NULL) {
		miNumTotalChannel++;
		token = strtok_s(NULL, seps, &context);
	} //count the number of defined channels

	mszChannelDef = (char **) malloc(miNumTotalChannel * sizeof(char *));
	token = strtok_s(strBuffer, seps, &context);

	int i = 0;
	while( token != NULL )
	{
		mszChannelDef[i]=(char*)malloc(strlen(token)+1);
		memcpy(mszChannelDef[i],token,strlen(token)+1);
		i++;
		token = strtok_s(NULL, seps, &context);
	}  // save channel names

	mpChannelUsedIndex = new int[miNumTotalChannel];
	memset(mpChannelUsedIndex,0,sizeof(int)*miNumTotalChannel);
	if (mpResources->Get ("EEG", "P300UsedChannels", strBuffer,sizeof(strBuffer)) == 0) {
		printf("Used channels = %s\n", strBuffer);

		miNumChannelUsed=0;
		token = strtok_s(strBuffer, seps, &context);
		while( token != NULL )
		{
			miNumChannelUsed++;
			int iChannel = GetChannel(token);
			if(iChannel == -1) return false;
			mpChannelUsedIndex[iChannel] = 1;
			token = strtok_s(NULL, seps, &context);
		}
	} else {
		// use all channels
		printf("UsedChannels not defined, Use all channels");
		miNumChannelUsed = miNumTotalChannel;
		for (i = 0; i < miNumChannelUsed; i++) {
			mpChannelUsedIndex[i] = 1;
		}
	}


	if (mpResources->Get ("ModelTrainingSetting", "FreqHighStop", strBuffer,sizeof(strBuffer))){
		//printf ("EEG:FreqHighStop unspecified. %s\n", strConfigFile);
		//return false;
		mfFreqHighStop = 25;
	} else {
		sscanf_s(strBuffer,"%f",&mfFreqHighStop);
	}

	if (mpResources->Get ("ModelTrainingSetting", "UseDelta", strBuffer,sizeof(strBuffer))){
		//printf ("EEG:FreqHighStop unspecified. %s\n", strConfigFile);
		//return false;
		miUseDelta = 1;
	} else {
		miUseDelta = _stricmp(strBuffer,"true")==0?1:0;
	}

	//TargetCharListInOrder
	//char szTask[kMaxLine];
	//if (mpResources->Get ("System","Task", szTask, sizeof(szTask))){
	//	printf ("System:Task undefined in %s\n", (char *)CONFIG_FILE_NAME);
	//	return false;
	//}

	//if (strcmp (szTask, "CardGame") == 0) { miTaskType=CARDGAME; return true;}
	//if (strcmp (szTask, "WordSpeller") !=0) { miTaskType=WORDSPELLER;return false;}

	if (mpResources->Get ("P300EEGSignal","NumColumn", &miNumColumn)!= errOkay){
		//printf ("P300EEGSignal:NumColumn  undefined\n");
		//return false;
		miNumColumn = miNumEpochPerRound;
	}

	char pCodedChars[1024]; // i.e. 'a'->'a' ','->'\,' '\'->'\\'
	mSpellerChars = new char[sizeof(pCodedChars)];
	if (mpResources->Get("P300EEGSignal","SpellerChars", pCodedChars, sizeof(pCodedChars))!= errOkay){
		printf ("Warning %s - P300EEGSignal:SpellerChars undefined.\n", CONFIG_FILE_NAME);
		return false;
	}
	if(pCodedChars[0]==',') return false;// the first char cannot be ','

	int iLenCodedStr = (int) strlen(pCodedChars);
	int iChar=0,iCharCode;
	miCharTableSize=0;
	while(iChar<iLenCodedStr){
		if(pCodedChars[iChar]=='\\'){ //special character
			iChar++;
			if(iChar>=iLenCodedStr) return false; //single '\' in the end means something wrong
			if(pCodedChars[iChar]>='0' && pCodedChars[iChar]<='9'){ //If refer to ASCII table
				sscanf_s(pCodedChars+iChar,"%d",&iCharCode);
				mSpellerChars[miCharTableSize]=iCharCode;
				miCharTableSize++;
				//skip next ',' if it exists
				iChar++;
				while(iChar<iLenCodedStr && pCodedChars[iChar]!=',') iChar++;
				iChar++;
			}
			else{ //Use the following character
				mSpellerChars[miCharTableSize]=pCodedChars[iChar];
				miCharTableSize++;
				//skip next ',' if it exists
				iChar++;
				while(iChar<iLenCodedStr && pCodedChars[iChar]!=',') iChar++;
				iChar++;
			}
		}
		else{ //use a char directly
			mSpellerChars[miCharTableSize]=pCodedChars[iChar];
			miCharTableSize++;
			//skip next ',' if it exists
			iChar++;
			while(iChar<iLenCodedStr && pCodedChars[iChar]!=',') iChar++;
			iChar++;
		}
	}

	///------------Initialize SVM parameters-------------------------------------------------------

	m_SVMParam.kernel_type = RBF;
	m_SVMParam.degree = 3;
	m_SVMParam.gamma = 0;	// 1/k
	m_SVMParam.coef0 = 0;
	m_SVMParam.nu = 0.5;
	m_SVMParam.cache_size = 40;
	m_SVMParam.C = 1;
	m_SVMParam.eps = 1e-3;
	m_SVMParam.p = 0.1;
	m_SVMParam.shrinking = 1;
	m_SVMParam.nr_weight = 0;
	m_SVMParam.weight_label = NULL;
	m_SVMParam.weight = NULL;

	if (mpResources->Get ("ModelTrainingSetting", "SVMType", strBuffer,sizeof(strBuffer))){
		//printf ("SVM Type not defined in %s\n", strConfigFile);
		//return true;
		m_SVMParam.svm_type = C_SVC;
	} else {
		if(strcmp(strBuffer,"C_SVC")==0) m_SVMParam.svm_type=C_SVC;
		else if(strcmp(strBuffer,"NU_SVC")==0) m_SVMParam.svm_type=NU_SVC;
		else if(strcmp(strBuffer,"ONE_CLASS")==0) m_SVMParam.svm_type=ONE_CLASS;
		else if(strcmp(strBuffer,"EPSILON_SVR")==0) m_SVMParam.svm_type=EPSILON_SVR;
		else if(strcmp(strBuffer,"NU_SVR")==0) m_SVMParam.svm_type=NU_SVR;
		else {
			printf("SVM Type Error in %s!!!\n",strConfigFile);
			return false;
		}
	}

	if (mpResources->Get ("ModelTrainingSetting","KernelType",strBuffer,sizeof(strBuffer))){
		//printf ("SVM Kernel Type not defined in %s\n", strConfigFile);
		//return true;
		m_SVMParam.kernel_type=RBF;
	} else {
		if(strcmp(strBuffer,"LINEAR")==0) m_SVMParam.kernel_type=LINEAR;
		else if(strcmp(strBuffer,"POLY")==0) m_SVMParam.kernel_type=POLY;
		else if(strcmp(strBuffer,"RBF")==0) m_SVMParam.kernel_type=RBF;
		else if(strcmp(strBuffer,"SIGMOID")==0) m_SVMParam.kernel_type=SIGMOID;
		else {
			printf("SVM Kernel Type Error in %s!!!\n",strConfigFile);
			return false;
		}
	}

	if (mpResources->Get ("ModelTrainingSetting","Degree",strBuffer,sizeof(strBuffer))) {
		printf ("SVM Degree not defined in %s\n", strConfigFile);
		m_SVMParam.degree = 3;
	} else {
		sscanf_s(strBuffer,"%d", &m_SVMParam.degree);
	}

	if (mpResources->Get("ModelTrainingSetting","Gamma",strBuffer,sizeof(strBuffer))) {
		printf ("SVM Gamma not defined in %s\n", strConfigFile);
		m_SVMParam.gamma = 0;
	}
	else sscanf_s(strBuffer,"%d",&m_SVMParam.gamma);

	///------------------------------------------------------------------------------------------------

	/// ccwang: moved here.
	/// Filter Design
	double B[24]={0.008343314888274, -0.025989255084205, 0.046399602997059, -0.055119125703279, 0.057973023502140, -0.055119125703279, 0.046399602997059, -0.025989255084205, 0.008343314888274};
	double A[24]={1.000000000000000, -5.098304723303556, 11.763518682887524, -15.910528136121988, 13.737722522445742, -7.729528046147417, 2.761636568733868, -0.571786472847546, 0.052511702051212};
	int n = 9;

	// ccwang: read from configuration file
	char strbuf[1024];
	char *rn = "ModelTrainingSetting";
	if (mpResources->Get(rn, "Filter_A", strbuf, sizeof(strbuf)) == 0) {
		printf("CP300SignalProc.LoadSysConfigFile: read in filter A=%s\n", strbuf);
		n = ReadDataFromString(strbuf, A, 24);
		if (n <= 0) return false;

		if (mpResources->Get(rn, "Filter_B", strbuf, sizeof(strbuf)) != 0) {
			printf("Error in read Fileb_B\n");
			return false;
		}

		printf("CP300SignalProc.LoadSysConfigFile: read in filter B=%s\n", strbuf);

		if (n != ReadDataFromString(strbuf, B, 24)) {
			printf("Confict A and B!\n");
			return false;
		}
	}
	// ccwang: --end

	mpCFiltering = new CFiltering();
	miFilterOrderOfB = n;
	mpCFiltering->InitB(miFilterOrderOfB);
	for (i = 0; i < miFilterOrderOfB; i++) mpCFiltering->SetBPara(B[i],i);

	miFilterOrderOfA = n;
	mpCFiltering->InitA(miFilterOrderOfA);
	for (i = 0; i < miFilterOrderOfA; i++) mpCFiltering->SetAPara(A[i],i);

	if(m_bVariLenDetectMode){
		//Get config for variable length detection mode
		if (mpResources->Get ("ModelTrainingSetting","iMinLen_VariLen", &m_iMinLen_VariLen)!= errOkay){
			printf("EEG:iMinLen_VariLen undefined.\n");
			return false;
		}

		if (mpResources->Get ("ModelTrainingSetting","iMaxLen_VariLen", &m_iMaxLen_VariLen)!= errOkay){
			printf("EEG:iMaxLen_VariLen undefined.\n");
			return false;
		}
	}
	delete mpResources;mpResources=NULL;
	return true;
}

bool CP300SignalProc::SetupDSPParametersFromConfigFile(const char* strConfigFile)
{
	if(!LoadSysConfigFile(strConfigFile)) return false;
	int i,j;

	//PCA Projection //dumb PCA , EYE matrix
	miNumPCAMatrixRow=miNumChannelUsed;
	miNumPCAMatrixCol=miNumChannelUsed;
	float* pData=new float[miNumPCAMatrixRow*miNumPCAMatrixCol];
	memset(pData,0,sizeof(float)*miNumPCAMatrixRow*miNumPCAMatrixCol);
	for(i=0;i<miNumPCAMatrixRow;i++) pData[i*miNumPCAMatrixCol+i]=1; //creating IDENTITY matrix
	mpPCATransform = new CTransformat();
	mpPCATransform->LoadMatrixFromMemory(miNumPCAMatrixRow, miNumPCAMatrixCol, pData);
	delete pData;

	//Downsampling Projection
	miDownSampleRatio = (int) floor(miSamplingRate*0.5/mfFreqHighStop);
	miNumSampleUsed = (int) floor((miP300ProcEndTime-miP300ProcStartTime*1.0f)*miSamplingRate/1000);
	miNumSampInRawEpochs = (int) floor(miP300ProcEndTime*miSamplingRate/1000.0);
	mpSampUsedIndex = new int[miNumSampInRawEpochs]; //sample indices prior to downsampling
	memset(mpSampUsedIndex,0,sizeof(int)*miNumSampInRawEpochs);

    int iStartSample = miP300ProcStartTime * miSamplingRate / 1000;
	for (i=0;i<miNumSampleUsed;i++) mpSampUsedIndex[i+iStartSample] = 1;

	miNumDownSampTransRow=miNumSampleUsed/miDownSampleRatio;
	miNumDownSampTransCol=miNumSampleUsed;
	pData=new float[miNumDownSampTransRow*miNumDownSampTransCol];
	memset(pData,0,sizeof(float)*miNumDownSampTransRow*miNumDownSampTransCol);
	int istart,iend;
	for(i=0;i<miNumDownSampTransRow;i++){
		istart=i*miDownSampleRatio;iend=istart+miDownSampleRatio-1;
		for(j=istart;j<=iend;j++)
			pData[i*miNumDownSampTransCol+j] = (float) 1.0/miDownSampleRatio;
	}
    mpDownSampTransform = new CTransformat();
	mpDownSampTransform->LoadMatrixFromMemory(miNumDownSampTransRow,miNumDownSampTransCol,pData);
	delete pData;

	//Delta transformation
	miNumDeltaTransRow=miNumDownSampTransRow;
	miNumDeltaTransCol=miNumDownSampTransRow;
	int iWinWidth=2;
	CreateDeltaTformMatrix(pData,miNumDeltaTransRow,miNumDeltaTransCol,iWinWidth);
	mpDeltaTransform = new CTransformat();
	mpDeltaTransform->LoadMatrixFromMemory(miNumDeltaTransRow,miNumDeltaTransCol,pData);
	delete pData;

	return true;
}

bool CP300SignalProc::CreateDeltaTformMatrix(float*& pData,int iRow,int iCol, int iWinWidth)
{
	int i,j,isum;
	if (iWinWidth>3) return false;
	float fInvWinFactor[256];
	for (i=0;i<iWinWidth;i++) {
		isum=0;
		for (j = -(i+1); j <= i+1; j++) isum += j*j;
		fInvWinFactor[i] = 1.0f / isum;
	}

	pData=new float[iRow*iCol];
	memset(pData,0,sizeof(float)*iRow*iCol);
	pData[0] = -1 * fInvWinFactor[0]; 
	pData[1] = fInvWinFactor[0];
	pData[(iRow-1)*iCol+iCol-1] = fInvWinFactor[0];
	pData[(iRow-1)*iCol+iCol-2] = -fInvWinFactor[0];

	if(iWinWidth>=1){
		for(i=1;i<iRow-1;i++){
			pData[i*iCol+(i+1)] = fInvWinFactor[0];
			pData[i*iCol+(i-1)] = -fInvWinFactor[0];
		}
	}
	if(iWinWidth>=2){
		for(i=2;i<iRow-2;i++){
			pData[i*iCol+(i+1)] = fInvWinFactor[1];
			pData[i*iCol+(i+2)] = 2*fInvWinFactor[1];
			pData[i*iCol+(i-1)] = -fInvWinFactor[1];
			pData[i*iCol+(i-2)] = -2*fInvWinFactor[1];
		}
	}
	if(iWinWidth>=3){
		for(i=3;i<iRow-3;i++){
			pData[i*iCol+(i+1)] = fInvWinFactor[3];
			pData[i*iCol+(i+2)] = 2*fInvWinFactor[3];
			pData[i*iCol+(i+3)] = 3*fInvWinFactor[3];
			pData[i*iCol+(i-1)] = -fInvWinFactor[3];
			pData[i*iCol+(i-2)] = -2*fInvWinFactor[3];
			pData[i*iCol+(i-3)] = -3*fInvWinFactor[3];
		}
	}
	return true;
}

bool CP300SignalProc::StoreProcParameters(char* strFileName)
{
	int i;
	if(mpPCATransform==NULL) return false;

	FILE* fp;
	if(fopen_s(&fp, strFileName,"w+")) return false;

	fprintf(fp,"Sampling_Rate: %d\n",miSamplingRate);
	fprintf(fp,"EEG_Resolution: %.10f\n",mfResolution);
	fprintf(fp,"Reject_Threshold: 100\n"); ///XXXXXXXXXXXXXXX : what's this? NO IDEA about the threshold

	fprintf(fp,"P300_Processing_Start_Time: %d\n",miP300ProcStartTime);
	fprintf(fp,"P300_Processing_End_Time: %d\n",miP300ProcEndTime);

	fprintf(fp,"Number_Samples_in_Raw_Epochs: %d\n",miNumSampInRawEpochs);
	fprintf(fp,"UseDelta: 1\n");
	fprintf(fp,"Channel_Order: %d\n",miNumTotalChannel);
	for(i=0;i<miNumTotalChannel;i++) fprintf(fp,"%s, ",mszChannelDef[i]);
	fprintf(fp,"\n");

	fprintf(fp,"Channel_Used: %d\n",miNumChannelUsed);
	for(i=0;i<miNumTotalChannel;i++){if(mpChannelUsedIndex[i]==1) fprintf(fp,"%d, ",i+1);}fprintf(fp,"\n");
	
	fprintf(fp,"Sample_Index_in_Epochs: %d\n",miNumSampleUsed);
	for(i=0;i<miNumSampInRawEpochs;i++){if(mpSampUsedIndex[i]==1) fprintf(fp,"%d, ",i+1);}fprintf(fp,"\n");

	fprintf(fp,"Filter_Parameter_B: %d\n",mpCFiltering->miOrderOfB);
	for(i=0;i<mpCFiltering->miOrderOfB;i++) fprintf(fp,"%f, ",mpCFiltering->mpB[i]);fprintf(fp,"\n");

	fprintf(fp,"Filter_Parameter_A: %d\n",mpCFiltering->miOrderOfA);
	for(i=0;i<mpCFiltering->miOrderOfA;i++) fprintf(fp,"%f, ",mpCFiltering->mpA[i]);fprintf(fp,"\n");

	fprintf(fp,"PCA_Transform_Matrix: %d, %d\n",mpPCATransform->miNumRow,mpPCATransform->miNumCol);
	mpPCATransform->SaveMatrix(fp,"%.2f");

	fprintf(fp,"Down_Sampling_Transform_Matrix: %d, %d\n",mpDownSampTransform->miNumRow,mpDownSampTransform->miNumCol);
	mpDownSampTransform->SaveMatrix(fp,"%.2f");

	fprintf(fp,"Delta_Transform_Matrix: %d, %d\n",mpDeltaTransform->miNumRow,mpDeltaTransform->miNumCol);
	mpDeltaTransform->SaveMatrix(fp,"%.2f");

	if (mfEERRejectThrs != NULL) {
		fprintf(fp,"EEG_Rejection_Threshold_for_Multiple_Repeatition: %d\n",miNumRejectThrs);
		for(i=0;i<miNumRejectThrs;i++) fprintf(fp,"%e, ",mfEERRejectThrs[i]);fprintf(fp,"\n");
		fprintf(fp,"Rejection_Threshold_for_DesiredFalseRejRate:%d\n",miNumRejectThrs);
		for(i=0;i<miNumRejectThrs;i++) fprintf(fp,"%e, ",mfDesiredFalseRejThrs[i]);fprintf(fp,"\n");
		fprintf(fp,"Mean_Prob_Target: %d\n",miNumRejectThrs);
		for(i=0;i<miNumRejectThrs;i++) fprintf(fp,"%e, ",mfTargetProbMeans[i]);fprintf(fp,"\n");
		fprintf(fp,"Mean_Prob_Garbage: %d\n",miNumRejectThrs);
		for(i=0;i<miNumRejectThrs;i++) fprintf(fp,"%e, ",mfGarbageProbMeans[i]);fprintf(fp,"\n");
		fprintf(fp,"Target_Gaussian_Mean_Std: %d\n",2);
		fprintf(fp,"%f, %f, \n",m_fTargetGaussMean,m_fTargetGaussStd);
		fprintf(fp,"Non_Target_Gaussian_Mean_Std: %d\n",2);
		fprintf(fp,"%f, %f, \n",m_fNonTargetGaussMean,m_fNonTargetGaussStd);
	}

	if(m_bVariLenDetectMode){
		fprintf(fp,"VariLenDetectParas %d\n",11);//set number of paras -- must agree with code below
		fprintf(fp,"MeanScore_TargetEpoch %f\n",m_EpochScoreStats.fMean_Target);
		fprintf(fp,"MeanScore_NonTargetEpoch %f\n",m_EpochScoreStats.fMean_NonTarget);
		fprintf(fp,"StdScore_TargetEpoch %f\n",m_EpochScoreStats.fStd_Target);
		fprintf(fp,"StdScore_NonTargetEpoch %f\n",m_EpochScoreStats.fStd_NonTarget);
		fprintf(fp,"MeanScore_IdleEpoch %f\n",m_EpochScoreStats.fMean_Idle);
		fprintf(fp,"StdScore_IdleEpoch %f\n",m_EpochScoreStats.fStd_Idle);
		
		fprintf(fp,"L_Control_VariLen %f\n",m_fL_Control_VariLen);
		fprintf(fp,"L_Idle_VariLen %f\n",m_fL_Idle_VariLen);
		fprintf(fp,"MinLen_VariLen %d\n",m_iMinLen_VariLen);
		fprintf(fp,"MaxLen_VariLen %d\n",m_iMaxLen_VariLen);
		fprintf(fp,"EERDetectThr_VariLen %f\n",m_fEERDetectThr_VariLen);
	}

	fclose(fp);
	return true;
}

bool CP300SignalProc::TrainModel(const char* strConfigFile)
{
	char strModelFileList[256];
	char strEvalFileList[256];//Evaluation files For rejection model training
	char strOutputModelFile[256];
	char strFeatureRangeFile[256];
	char strProcParameterFile[256];
	char strModelScoreFile[256];
	char strIdleFileList_VariLenDetect[256];
	char strEvalFileList_VariLenDetect[256];

	mpResources=new Resources;
	if (mpResources->Merge ((char*)strConfigFile)){
		printf ("Can not open configuration file\n");
		return false;
	}

	if (mpResources->Get ("ModelTrainingSetting","TrainModelFileList",strModelFileList,sizeof(strModelFileList))){
		printf ("ModelFileList not specified.\n");
		return false;
	}
	
	strEvalFileList[0]=0;
	if (mpResources->Get ("ModelTrainingSetting","TrainRejectFileList",strEvalFileList,sizeof(strEvalFileList))){
		printf ("RejectionEvalFileList not specified.\n");
	}

	strIdleFileList_VariLenDetect[0]=0;
	if (mpResources->Get ("ModelTrainingSetting","TrainVariLenDetect_IdleFileList",strIdleFileList_VariLenDetect,sizeof(strIdleFileList_VariLenDetect))){
		printf ("TrainVariDetect_IdleFileList not specified.\n");
		m_bVariLenDetectMode=false;
	}
	else m_bVariLenDetectMode=true;

	if (mpResources->Get ("ModelTrainingSetting","TrainVariLenDetect_EvalFileList",strEvalFileList_VariLenDetect,sizeof(strEvalFileList_VariLenDetect))){
		printf ("TrainVariDetect_EvalFileList not specified.\n");
		m_bVariLenDetectMode=false;
	}
	else m_bVariLenDetectMode=true;

	if (mpResources->Get ("ModelTrainingSetting","OutputProcParameterFile",strProcParameterFile,sizeof(strProcParameterFile))){
		printf ("ProcParameterFile not specified.\n");
		return false;
	}

	if (mpResources->Get ("ModelTrainingSetting","OutputModelFile",strOutputModelFile,sizeof(strOutputModelFile))){
		printf ("ModelFileList not specified.\n");
		return false;
	}
	if (mpResources->Get ("ModelTrainingSetting","OutputFeatureRangeFile",strFeatureRangeFile,sizeof(strFeatureRangeFile))){
		printf ("strFeatureRangeFile not specified.\n");
		return false;
	}

	if (mpResources->Get ("ModelTrainingSetting","OutputModelScoreFile",strModelScoreFile,sizeof(strModelScoreFile))){
		printf ("strModelScoreFile not specified.\n");
		return false;
	}

    CEEGContFile* pEEGContFile=new CEEGContFile;
	if(!pEEGContFile->LoadDataFileList(strModelFileList)){
		printf ("Failed Loading Data FileList %s.\n",strModelFileList);
		return false;
	}

	if(!SetupDSPParametersFromConfigFile(strConfigFile)){
		printf ("Failed Setup DSP Parameters from Config File %s.\n", strConfigFile);
		return false;
	}

	///----------Check the number of Epoches----------------
	int iExpectNumEpoch=miNumEpochPerRound*miNumRoundPerTarget*pEEGContFile->m_iNumTarget;
	if(iExpectNumEpoch!=pEEGContFile->m_iNumAllEpoch){
		printf("The expected number of epoches does not agree with that in the data file list. Training Abort.\n");
		printf("NumEpochPerRound=%d,NumRoundPerTarget=%d,NumTarget=%d, AllEpoch=%d.\n",
			miNumEpochPerRound, miNumRoundPerTarget, pEEGContFile->m_iNumTarget, pEEGContFile->m_iNumAllEpoch);
		delete pEEGContFile;
		return false;
	}
	///-----------------------------------------------------

	InitBuffers();

	miSamplingRate = pEEGContFile->m_CNTHeader.iSamplingRate;
	mfResolution = pEEGContFile->m_CNTHeader.fResolution; // **** Set Resolution to be agree with the file instead from config file
    
	int** ppTargetEpoch=NULL;
	int** ppNonTargetEpoch=NULL;
	int* pTargetEpochCode=NULL;
	int* pNonTargetEpochCode=NULL;

	int** ppTargetEpochData; //All channels , Target Epochs
	int** ppNonTargetEpochData; //All channels , Non-Target Epochs

	//Seperate Epochs
	int iNumTargetEpoch,iNumNonTargetEpoch;
	iNumTargetEpoch=pEEGContFile->GetTargetEpochPtrs(miNumEpochPerRound,miNumRoundPerTarget,ppTargetEpoch,pTargetEpochCode, mSpellerChars,miCharTableSize);
	iNumNonTargetEpoch=pEEGContFile->GetNonTargetEpochPtrs(miNumEpochPerRound,miNumRoundPerTarget,ppNonTargetEpoch,pNonTargetEpochCode,mSpellerChars,miCharTableSize);

	int iStartSample, iNumSample, i;
	// iStartSample = 0; ccwang -->
	iStartSample = miP300RecordStartTime * miSamplingRate / 1000;
	iNumSample = miNumSampInRawEpochs;

	pEEGContFile->GetEpochData(iStartSample, iNumSample, ppTargetEpoch, iNumTargetEpoch, ppTargetEpochData);
	pEEGContFile->GetEpochData(iStartSample, iNumSample, ppNonTargetEpoch, iNumNonTargetEpoch, ppNonTargetEpochData);
	
	int iFeatureSize = ExtractFeaturesFromEEGBuffer(ppTargetEpochData[0],mpSegment);

	///-------------------------------Show some Epoch Data for debug purpose-------------------------
	if (pEEGContFile->m_CNTHeader.fResolution > 0) {
		printf("The first Target Epoch (EpochCode=%d)\n",pTargetEpochCode[0]);
		int iCh = 0;
		while (mpChannelUsedIndex[iCh] == 0) iCh++;
		printf("Channel %d:(first 10 samples) \n",iCh);
		for(i=0;i<10;i++) printf("%d,",ppTargetEpochData[0][i*miNumTotalChannel+iCh]);printf("\n");
		printf("The first Non-Target Epoch (EpochCode=%d)\n",pNonTargetEpochCode[0]);
		printf("Channel %d:(first 10 samples) \n",iCh);
		for(i=0;i<10;i++) printf("%d,",ppNonTargetEpochData[0][i*miNumTotalChannel+iCh]);printf("\n");
		printf("............................\n");

		// iFeatureSize=ExtractFeaturesFromEEGBuffer(ppTargetEpochData[0],mpSegment);
		printf("The feature vector of the first target epoch.\n");
		printf("First ten features:\n");
		for (i=0; i<10; i++) printf("%.2f\t",mpFeatures[i]); printf("\n");
		printf("Last ten features:\n");
		for (i=iFeatureSize-1; i>=iFeatureSize-10; i--) printf("%.2f\t",mpFeatures[i]); printf("\n");
	}

	//iFeatureSize=ExtractFeaturesFromEEGBuffer(ppNonTargetEpochData[0],mpSegment);
	//printf("The feature vector of the first non-target epoch.\n");
	//for(i=0;i<iFeatureSize;i++) printf("%.2f,",mpFeatures[i]);printf("\n");

	///----------------------------------------------------------------------------------------------

	CSvmTrain* pSVMTrain=new CSvmTrain();
	pSVMTrain->Initialize(&m_SVMParam,iNumTargetEpoch+iNumNonTargetEpoch,iFeatureSize);

	for (i=0;i<iNumTargetEpoch;i++) {
		ExtractFeaturesFromEEGBuffer(ppTargetEpochData[i], mpSegment);
		pSVMTrain->AddNewSample(mpFeatures,1);
		//Delete the EpochData here to save memory
		delete ppTargetEpochData[i];ppTargetEpochData[i] = NULL;
	}
	for (i=0;i<iNumNonTargetEpoch;i++) {
		int tFeatureSize=ExtractFeaturesFromEEGBuffer(ppNonTargetEpochData[i],mpSegment);
		/////tFeatureSize must be equal to iFeatureSize!!!!
		pSVMTrain->AddNewSample(mpFeatures,-1);
		//Delete the EpochData here to save memory
		delete ppNonTargetEpochData[i];ppNonTargetEpochData[i]=NULL;
	}

	if (miUseDelta) pSVMTrain->ResetFeatureRanges(miNumChannelUsed * 2);
	else pSVMTrain->ResetFeatureRanges(miNumChannelUsed);

	printf("\n\n------------------------SVM Training--------------------------------\n");
	pSVMTrain->TrainModel();
	pSVMTrain->SaveModel(strOutputModelFile,strFeatureRangeFile);
	printf("^^^^^^^^^^^^^^^^^^^^^^^^^^SVMTraining finished^^^^^^^^^^^^^^^^^^^^^^^^^^\n");

	//Clear allocated memory
	delete ppTargetEpoch;
	delete ppNonTargetEpoch;
	delete pTargetEpochCode;
	delete pNonTargetEpochCode;

	delete [] ppTargetEpochData;
	delete [] ppNonTargetEpochData;
	
	delete pSVMTrain;

	delete mpResources;	mpResources=NULL;


	//----------rejection model training---------------
	if (strEvalFileList[0]!=0)
	{
		TrainRejectionModel(strEvalFileList,pEEGContFile, strOutputModelFile, strFeatureRangeFile, strModelScoreFile);
	}

	printf("TrainModel accomplished.\n");

	// ccwang: Calculate accuracy
	if (strEvalFileList[0] != 0)
	{
		CalculateAccuracy(strEvalFileList, strOutputModelFile, strFeatureRangeFile);
	}


	//--------------vari-len detection model training--------------
	if (m_bVariLenDetectMode)
	{
		printf("---Begin Training for Variable Length P300 Detection---\n"); 
		TrainVariLenDetectModel(strEvalFileList_VariLenDetect,strIdleFileList_VariLenDetect,pEEGContFile, strOutputModelFile, strFeatureRangeFile, strModelScoreFile);
		printf("Done.\n");
	}
	else
		printf("---No Training for Variable Length P300 Detection---\n"); 

	StoreProcParameters(strProcParameterFile);
	delete pEEGContFile;
	return true;
}


int CP300SignalProc::ExtractFeaturesFromEEGBuffer(int* pEEGData, float** pInSegment)
{//Extract features to be stored in mpFeatures ( mFeatureSize )

	CopyRawBuffer(pEEGData);	//eg, eeg between 0-500, all channel   *** CRASH ***

	///XXXXXXXX ---- Temporarily disable RemoveEOG, otherwise somehow the results disagree with that of matlab codes of DIFFREMOVEEOG
	//int iLeftEOGChnl = GetChannel("VEO");
	//if (iLeftEOGChnl >= 0) RemoveEOG(pInSegment,iLeftEOGChnl);
	//int iRightEOGChnl = GetChannel("HEO");
	//if (iRightEOGChnl >=0) RemoveEOG(pInSegment,iRightEOGChnl);	// *** CRASH ***
	///XXXXXXXX -----------------------------------

	SelectChannels(pInSegment);

	mFeatureSize = ExtractFeatures(pInSegment);

	return mFeatureSize;
}

/// ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
/// The ABOVE functions are coded by Zhang Haihong for training classifiers
/// ----------------------------------------------------------------------------
bool CP300SignalProc::TrainRejectionModel(char* strEvalFileList,CEEGContFile* pRefEEGContFile, char* strModelFile, char* strRangeFile, char* strModelScoreFile)
{
    CEEGContFile* pEEGContFile=new CEEGContFile;
	if(!pEEGContFile->LoadDataFileList(strEvalFileList)){delete pEEGContFile;return false;}
	if(memcmp(&pEEGContFile->m_CNTHeader,&pRefEEGContFile->m_CNTHeader,sizeof(_CNTFileHeader))!=0){delete pEEGContFile;return false;}
	if(pEEGContFile->m_iNumTarget<miNumRoundPerTarget) {
		printf("Failed Trainining Rejection Model. Must have more targets than rounds-per-target for simulating garbage pattern.\n");
		delete pEEGContFile; return false;
	}

	int iExpectNumEpoch=miNumEpochPerRound*miNumRoundPerTarget*pEEGContFile->m_iNumTarget;
	if(iExpectNumEpoch!=pEEGContFile->m_iNumAllEpoch){
		printf("The expected number of epoches does not agree with that in the data file list %s. Training Abort.\n",strEvalFileList);
		delete pEEGContFile;
		return false;
	}

	CSvmClassify* pSVM=new CSvmClassify();
	if(!pSVM->Initialize(strModelFile,strRangeFile)){delete pSVM;return false;}

	///-------------------------------Read Raw Data-----------------------------------------
	printf("Loading data files for rejection model...\n");

	int** ppTargetEpoch=NULL;
	int** ppNonTargetEpoch=NULL;
	int* pTargetEpochCode=NULL;
	int* pNonTargetEpochCode=NULL;

	int** ppTargetEpochData; //All channels , Target Epochs
	int** ppNonTargetEpochData; //All channels , Non-Target Epochs

	//Seperate Epochs
	int iNumTargetEpoch,iNumNonTargetEpoch;
	iNumTargetEpoch=pEEGContFile->GetTargetEpochPtrs(miNumEpochPerRound,miNumRoundPerTarget,ppTargetEpoch,pTargetEpochCode, mSpellerChars,miCharTableSize);
	iNumNonTargetEpoch=pEEGContFile->GetNonTargetEpochPtrs(miNumEpochPerRound,miNumRoundPerTarget,ppNonTargetEpoch,pNonTargetEpochCode,mSpellerChars,miCharTableSize);
	
	int iStartSample,iNumSample,i;
	// iStartSample = 0; ccwang -->
	iStartSample = miP300RecordStartTime * miSamplingRate / 1000;
	iNumSample = miNumSampInRawEpochs;

	pEEGContFile->GetEpochData(iStartSample,iNumSample, ppTargetEpoch, iNumTargetEpoch, ppTargetEpochData);
	pEEGContFile->GetEpochData(iStartSample, iNumSample, ppNonTargetEpoch, iNumNonTargetEpoch, ppNonTargetEpochData);
	
	///^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

	///----------------------------Extract Features and Calculate SVM Scores-------------------
	printf("Calculating SVM scores for each epoch...\n");

	double* pTargetSVMScores;
	double* pNonTargetSVMScores;
    pTargetSVMScores=new double[iNumTargetEpoch];
    pNonTargetSVMScores=new double[iNumNonTargetEpoch];
	double fPredict;

	for(i=0;i<iNumTargetEpoch;i++){
		int iFeatureSize=ExtractFeaturesFromEEGBuffer(ppTargetEpochData[i],mpSegment);
		pSVM->Classify(mpFeatures, iFeatureSize,pTargetSVMScores+i, &fPredict);
		pTargetSVMScores[i]=(pTargetSVMScores[i]+1)*100; ///Change Scale, see IsRejection()
		delete ppTargetEpochData[i];ppTargetEpochData[i]=NULL;
	}

	for(i=0;i<iNumNonTargetEpoch;i++){
		int iFeatureSize=ExtractFeaturesFromEEGBuffer(ppNonTargetEpochData[i],mpSegment);
		pSVM->Classify(mpFeatures,iFeatureSize,pNonTargetSVMScores+i,&fPredict);
		pNonTargetSVMScores[i]=(pNonTargetSVMScores[i]+1)*100; ///Change Scale, see IsRejection()
		delete ppNonTargetEpochData[i];ppNonTargetEpochData[i]=NULL;
	}

	///^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

	//Clear some allocated memory
	delete ppTargetEpoch;
	delete ppNonTargetEpoch;
	delete pEEGContFile;

	delete [] ppTargetEpochData;
	delete [] ppNonTargetEpochData;

	delete pSVM;
	//----------------------------

	///----------------------Univariate Gaussian Statistics----------------------
	m_fTargetGaussMean=0;m_fNonTargetGaussMean=0;m_fTargetGaussStd=0;m_fNonTargetGaussStd=0;
	for(i=0;i<iNumTargetEpoch;i++) m_fTargetGaussMean+=pTargetSVMScores[i];
	m_fTargetGaussMean=m_fTargetGaussMean/iNumTargetEpoch;
	for(i=0;i<iNumTargetEpoch;i++) m_fTargetGaussStd+=(pTargetSVMScores[i]-m_fTargetGaussMean)*(pTargetSVMScores[i]-m_fTargetGaussMean);
	m_fTargetGaussStd=sqrt(m_fTargetGaussStd/(iNumTargetEpoch-1));

	for(i=0;i<iNumNonTargetEpoch;i++) m_fNonTargetGaussMean+=pNonTargetSVMScores[i];
	m_fNonTargetGaussMean=m_fNonTargetGaussMean/iNumNonTargetEpoch;
	for(i=0;i<iNumNonTargetEpoch;i++) m_fNonTargetGaussStd+=(pNonTargetSVMScores[i]-m_fNonTargetGaussMean)*(pNonTargetSVMScores[i]-m_fNonTargetGaussMean);
	m_fNonTargetGaussStd=sqrt(m_fNonTargetGaussStd/(iNumNonTargetEpoch-1));

	printf("---------Gaussian statistics--------\n");
	printf("Target: Mean: %f Std: %f\n",m_fTargetGaussMean,m_fTargetGaussStd);
	printf("NonTarget: Mean: %f Std: %f\n",m_fNonTargetGaussMean,m_fNonTargetGaussStd);
	printf("\n");

	///^^^^^^^^^^^^^^^^^^^^^^^^^^^^^Sigmoid Parameters ^^^^^^^^^^^^^^^^^^^^^^^^^^^^
	double fstd=(m_fTargetGaussStd+m_fNonTargetGaussStd)/2;
	m_fA = -(m_fTargetGaussMean-m_fNonTargetGaussMean)/(fstd*fstd);
	m_fB = -(m_fNonTargetGaussMean*m_fNonTargetGaussMean-m_fTargetGaussMean*m_fTargetGaussMean)/(2*fstd*fstd);

	///--------------Get Scores in order and simulate Garbage scores
	double *pScoreBuffer,*pScoreBufferGarbage;
	PutScoreBufferInOrder(pScoreBuffer,pTargetSVMScores, pNonTargetSVMScores, iNumTargetEpoch, iNumNonTargetEpoch, pTargetEpochCode, pNonTargetEpochCode, miNumEpochPerRound);
	SimGarbageScoreBuffer(pScoreBufferGarbage, pScoreBuffer,miNumEpochPerRound,iNumTargetEpoch,miNumRoundPerTarget);//iNumRound=iNumTargetEpoch;

	///-------------Save model scores file------------------
	int iNumEpochPerTarget=miNumEpochPerRound*miNumRoundPerTarget;
	int iNumTarget=(iNumTargetEpoch+iNumNonTargetEpoch)/iNumEpochPerTarget;

	FILE* fp;
	if (fopen_s(&fp, strModelScoreFile,"wb")) return false;
	fwrite(&miNumEpochPerRound,1,sizeof(int),fp);
	fwrite(&miNumRoundPerTarget,1,sizeof(int),fp);
	fwrite(&iNumTarget,1,sizeof(int),fp);
	fwrite(&iNumTargetEpoch,sizeof(int),1,fp);
	fwrite(&iNumNonTargetEpoch,sizeof(int),1,fp);
	fwrite(pTargetSVMScores,sizeof(double),iNumTargetEpoch,fp);
	fwrite(pTargetEpochCode,sizeof(int),iNumTargetEpoch,fp);
	fwrite(pNonTargetSVMScores,sizeof(double),iNumNonTargetEpoch,fp);
	fwrite(pNonTargetEpochCode,sizeof(int),iNumNonTargetEpoch,fp);
	fclose(fp);


	///--------------Calculate the thresholds and equal error rates--------------------------------
	printf("Learning the thresholds and equal error rates....\n");

	double* pPostProbs;double* pPostProbsGarbage;
	//Thresholds for Equal error rate:

	double* fThd_EER=new double[miNumRoundPerTarget]; //Local memory clearance is not needed since the memory will be passed to mfEERRejectThrs
	double* fTargetProbMeans=new double[miNumRoundPerTarget];
	double* fGarbageProbMeans=new double[miNumRoundPerTarget];

	double* fThd_DesiredFalseRej=new double[miNumRoundPerTarget];

	printf("-----------------------------Finding the equal error rates and corresponding thresholds--------------\n");
	for(int iRejWinWidth=1;iRejWinWidth<=miNumRoundPerTarget;iRejWinWidth++){
		int iNumRejWinPerTarget=miNumRoundPerTarget-iRejWinWidth+1;
		double* pScores;double* pScoresGarbage;

		pPostProbs=new double[iNumRejWinPerTarget*iNumTarget]; //numTarget X numRejWin
		pPostProbsGarbage=new double[iNumRejWinPerTarget*iNumTarget];

		for(int iTarget=0;iTarget<iNumTarget;iTarget++){
			for(int iWin=0;iWin<iNumRejWinPerTarget;iWin++){
				int idx_rejwin=iTarget*iNumRejWinPerTarget+iWin;
                pScores=pScoreBuffer+iTarget*iNumEpochPerTarget+iWin*miNumEpochPerRound;//pScoreBuffer: the iTarget-th target the iWin-th round
                pScoresGarbage=pScoreBufferGarbage+iTarget*iNumEpochPerTarget+iWin*miNumEpochPerRound;
				pPostProbs[idx_rejwin]=CalcPostProb(pScores,miNumEpochPerRound,iRejWinWidth);
				pPostProbsGarbage[idx_rejwin]=CalcPostProb(pScoresGarbage,miNumEpochPerRound,iRejWinWidth);
			}
		}
		fTargetProbMeans[iRejWinWidth-1]=0;
		fGarbageProbMeans[iRejWinWidth-1]=0;
		int i;
		for(i=0;i<iNumRejWinPerTarget*iNumTarget;i++) fTargetProbMeans[iRejWinWidth-1]+=pPostProbs[i];
		for(i=0;i<iNumRejWinPerTarget*iNumTarget;i++) fGarbageProbMeans[iRejWinWidth-1]+=pPostProbsGarbage[i];
		fGarbageProbMeans[iRejWinWidth-1]/=iNumRejWinPerTarget*iNumTarget;
		fTargetProbMeans[iRejWinWidth-1]/=iNumRejWinPerTarget*iNumTarget;

		printf("Finding Equal Error Rates....\n");
		double fEER=FindEqualErrorRate(pPostProbs, pPostProbsGarbage, iNumRejWinPerTarget*iNumTarget, iNumRejWinPerTarget*iNumTarget, &fThd_EER[iRejWinWidth-1]);
		float fDesiredFalseRejRate=0.05f;
		printf("Finding Desired False Rejection Threshold...\n");
		double fRate_FalseAcceptAtDesiredFalseRej=FindThd4GivenFalseRejRate(pPostProbs, pPostProbsGarbage, iNumRejWinPerTarget*iNumTarget, iNumRejWinPerTarget*iNumTarget, &fThd_DesiredFalseRej[iRejWinWidth-1],fDesiredFalseRejRate);
		printf("Window width (rounds): %d | Equal Error Rate: %.3f | FalseAcceptRate@LowRej: %.3f(%.3f)\n",iRejWinWidth,fEER,fRate_FalseAcceptAtDesiredFalseRej,fDesiredFalseRejRate);
		printf("Display Bias and Effects....\n");
		//DisplayErrorRateOverThresholdBias(pPostProbs,pPostProbsGarbage,iNumRejWinPerTarget*iNumTarget,iNumRejWinPerTarget*iNumTarget,fThd_DesiredFalseRej[iRejWinWidth-1],fTargetProbMeans[iRejWinWidth-1],fGarbageProbMeans[iRejWinWidth-1]);
		printf("Done...\n");
		delete pPostProbs;delete pPostProbsGarbage;
	}

	//Set the member variables
	miNumRejectThrs = miNumRoundPerTarget;
	mfEERRejectThrs = fThd_EER;
	mfDesiredFalseRejThrs=fThd_DesiredFalseRej;
	mfTargetProbMeans = fTargetProbMeans;
	mfGarbageProbMeans = fGarbageProbMeans;

	//^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

	delete pTargetEpochCode;
	delete pNonTargetEpochCode;

	printf("TrainRejectionModel accomplished.\n");
	return true;
}


/// ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
/// ccwang 20050528
/// ----------------------------------------------------------------------------
void CP300SignalProc::CalculateAccuracy(char* strEvalFileList, char* strModelFile, char* strRangeFile)
{
    CEEGContFile* pEEGContFile = new CEEGContFile;
	if(!pEEGContFile->LoadDataFileList(strEvalFileList)){
		delete pEEGContFile;
		return ;}

	int iExpectNumEpoch = miNumEpochPerRound * miNumRoundPerTarget * pEEGContFile->m_iNumTarget;
	if (iExpectNumEpoch!=pEEGContFile->m_iNumAllEpoch){
		printf("The expected number of epoches does not agree with that in the data file list %s. Training Abort.\n",strEvalFileList);
		delete pEEGContFile;
		return;
	}

	CSvmClassify* pSVM = new CSvmClassify();
	if (!pSVM->Initialize(strModelFile,strRangeFile)){
		delete pSVM;
		return;}

	double * pScore = new double[miNumEpochPerRound];
	int nepoch = 0;
	int nok = 0;
	int iStartSample = miP300RecordStartTime * miSamplingRate / 1000;
	for (int it = 0; it < pEEGContFile->m_iNumTarget; it++) {
		char tch = pEEGContFile->m_TargetCharList[it];
		for (int i = 0; i < miNumEpochPerRound; i++) pScore[i] = .0;

		for (int ir = 0; ir < miNumRoundPerTarget; ir++) {
			for (int ie = 0; ie < miNumEpochPerRound; ie++) {
				int * epos[1];
				epos[0] = (int *)pEEGContFile->m_ppAllEpochData[nepoch];
				int tc = pEEGContFile->m_pAllEpochCode[nepoch];

				int **pdata = NULL;
				pEEGContFile->GetEpochData(iStartSample, miNumSampInRawEpochs, epos, 1, pdata);

				int iFeatureSize = ExtractFeaturesFromEEGBuffer(pdata[0], mpSegment);

				double score, fPredict;
				pSVM->Classify(mpFeatures, iFeatureSize, &score, &fPredict);
				pScore[tc] += (score + 1) * 100;

				delete [] *pdata;
				delete [] pdata;

				nepoch++;
			}
		}

		int ind = 0;
		for (int i = 1; i < miNumEpochPerRound; i++) {
			if (pScore[i] > pScore[ind]) ind = i;
		}

		if (mSpellerChars[ind] == tch) nok++;

		char ic = mSpellerChars[ind];
		if (isprint(ic)) {
			printf("Target %d: %c, identified to %c, score = %f\n", it, tch, ic, pScore[ind]);
		} else {
			printf("Target %d: %c, identified to \\%d, score = %f\n", it, tch, ic, pScore[ind]);
		}
	}

	printf("Accuracy = %.2f\n", nok * 100. / pEEGContFile->m_iNumTarget);
	fflush(stdout);
	fflush(stderr);

	delete []pScore;
	delete pEEGContFile;
	delete pSVM;

}

bool CP300SignalProc::PutScoreBufferInOrder(double*& pScoreBufferInOrder,double* pTargetSVMScores, double* pNonTargetSVMScores, int iNumTargetEpoch, int iNumNonTargetEpoch, int* pTargetEpochCode, int* pNonTargetEpochCode, int iNumStimPerRound)
{
	int iNumRound=(iNumTargetEpoch+iNumNonTargetEpoch)/iNumStimPerRound;
	int iNumScores=iNumRound*iNumStimPerRound;
	pScoreBufferInOrder=new double[iNumScores];
	for(int i=0;i<iNumScores;i++) pScoreBufferInOrder[i]=0;
	int iRound,idx;
	for(int i=0;i<iNumTargetEpoch;i++){
		iRound=i;
		idx=iRound*iNumStimPerRound+pTargetEpochCode[i];
		pScoreBufferInOrder[idx]=pTargetSVMScores[i];
	}
	for(int i=0;i<iNumNonTargetEpoch;i++){
		iRound=i/(iNumStimPerRound-1);
		idx=iRound*iNumStimPerRound+pNonTargetEpochCode[i];
		pScoreBufferInOrder[idx]=pNonTargetSVMScores[i];
	}
	return 0;
}

bool CP300SignalProc::SimGarbageScoreBuffer(double*& pDestBuffer, double* pSrcBuffer, int iNumEpochPerRound, int iNumAllRound, int iNumRoundPerTarget)
{
	int iNumTarget=iNumAllRound/iNumRoundPerTarget;
	if(iNumTarget<iNumRoundPerTarget) return false;
	int randseq[1024];int candlist[1024];
	int icand,iseq,irandcand,iRound;
	pDestBuffer=new double[iNumAllRound*iNumEpochPerRound];
	for(int iTarget=0;iTarget<iNumTarget;iTarget++){
		int iStartRound=iTarget*iNumRoundPerTarget;
		//generate rand sequence in which each element is from different target
		for(icand=0;icand<iNumTarget;icand++) candlist[icand]=icand;
		for(iseq=0;iseq<iNumRoundPerTarget;iseq++){
			irandcand=(iNumTarget-iseq)*rand()/(RAND_MAX+1);
			randseq[iseq]=candlist[irandcand];
			//delete the cand
			for(icand=irandcand;icand<iNumTarget-1;icand++) candlist[icand]=candlist[icand+1];
		}
		for(iRound=iStartRound;iRound<iStartRound+iNumRoundPerTarget;iRound++){
			double* pDest=pDestBuffer+iRound*iNumEpochPerRound;
			double* pSrc=pSrcBuffer+randseq[iRound-iStartRound]*iNumEpochPerRound*iNumRoundPerTarget;
			int iRandRound=iNumRoundPerTarget*rand()/(RAND_MAX+1);
			pSrc=pSrc+iRandRound*iNumEpochPerRound;
			memcpy(pDest,pSrc,iNumEpochPerRound*sizeof(double));
		}
	}
	return true;
}
double CP300SignalProc::FindEqualErrorRate(double* pPostProbs, double* pPostProbsGarbage, int iNumRejWin, int iNumRejWinGarbage, double* pThd_EER)
{
	//Enumerate all possible thresholds in the given probs
	int iNumThdCand=iNumRejWin+iNumRejWinGarbage;
	double* pAllThd=new double[iNumThdCand];
	memcpy(pAllThd,pPostProbs,iNumRejWin*sizeof(double));
	memcpy(pAllThd+iNumRejWin,pPostProbsGarbage,iNumRejWinGarbage*sizeof(double));

	double* pErrs=new double[iNumThdCand];//false rejection rate
	double* pErrsGarbage=new double[iNumThdCand]; //false acceptance rate
	double* pAbsDiffErrs=new double[iNumThdCand];
	int iThd_EER=0;
	for(int iThd=0;iThd<iNumThdCand;iThd++){
		int iWin;
		int iNumErr=0;int iNumErrGarbage=0;
		for(iWin=0;iWin<iNumRejWin;iWin++) iNumErr+=pPostProbs[iWin]>pAllThd[iThd]?0:1;
		for(iWin=0;iWin<iNumRejWinGarbage;iWin++) iNumErrGarbage+=pPostProbsGarbage[iWin]<=pAllThd[iThd]?0:1;
		pErrs[iThd]=iNumErr*1.0/iNumRejWin;
		pErrsGarbage[iThd]=iNumErrGarbage*1.0/iNumRejWinGarbage;
        pAbsDiffErrs[iThd]=fabs(pErrs[iThd]-pErrsGarbage[iThd]);
		if(pAbsDiffErrs[iThd]<pAbsDiffErrs[iThd_EER]) iThd_EER=iThd;
	}

	*pThd_EER=pAllThd[iThd_EER];
	double fEER;
	fEER=(pErrs[iThd_EER]+pErrsGarbage[iThd_EER])/2;

	delete pErrs;
	delete pErrsGarbage;
    return fEER;
}

int FindMin(double* pData, int iLength)
{
	if(iLength<=0) return iLength-1;
	int iMin=0;
	for(int i=0;i<iLength;i++) iMin=(pData[iMin]<pData[i])?iMin:i;
	return iMin;
}

int FindMax(double* pData, int iLength)
{
	if(iLength<=0) return iLength-1;
	int iMax=0;
	for(int i=0;i<iLength;i++) iMax=(pData[iMax]>pData[i])?iMax:i;
	return iMax;
}

int CP300SignalProc::ExtractFeatures(float** pInSegment)
{
	for (int iCh=0; iCh<miNumChannelUsed; iCh++)
		mpCFiltering->Process(pInSegment[iCh],miNumSampInRawEpochs);

	CutSegment(pInSegment);		//eg, cut from 0-500 to 250-450

	for (int iCh=0;iCh<miNumChannelUsed;iCh++)
		mpDownSampTransform->ProcessVect(pInSegment[iCh],0); //result in:  miNumChannelUsed X miNumDownSampTransRow

	mpPCATransform->ProcessMat(pInSegment, miNumDownSampTransRow); //result in: miNumPCAMatrixRow X miNumDownSampTransRow	

	if (miUseDelta){
		for(int iCh=0;iCh<miNumPCAMatrixRow;iCh++)
			mpDeltaTransform -> ProcessVect(pInSegment[iCh],miNumDownSampTransRow);	//result in miNumPCAMatrixRow X miNumDownSampTransRow*2	by appending transformed data after original data
	}

	mFeatureSize = CreateFeatures(pInSegment);
	return mFeatureSize;
}

bool CP300SignalProc::CalibrateRejectionModel(char* strEEGContFile)
{
	char strSVMModelFile[kMaxLine], strRangeFile[kMaxLine], strModelScoreFile[kMaxLine];
	if(mpResources->Get("Process","SVMModel",strSVMModelFile, sizeof(strSVMModelFile)) != errOkay){
		printf ("SVM model must be given in configuration file %s under P300Process:SVMModel.\n",(char *)CONFIG_FILE_NAME);
		return false;
	}

	if(mpResources->Get("Process","SVMRange",strRangeFile, sizeof(strRangeFile)) != errOkay){
		printf ("Range file must be given in configuration file %s under P300Process:SVMRange.\n",(char *)CONFIG_FILE_NAME);
		return false;
	}

	if(mpResources->Get("Process","SVMModelScore",strModelScoreFile, sizeof(strModelScoreFile)) != errOkay){
		printf ("SVM model score file must be given in configuration file %s under P300Process:SVMModelScore.\n",(char *)CONFIG_FILE_NAME);
		return false;
	}

    CEEGContFile* pEEGContFile=new CEEGContFile;
	BYTE* pCntFileData=NULL;
	FILE* fp_EEGCnt;
	fopen_s(&fp_EEGCnt, strEEGContFile,"rb");
	FILE* fp_ModelScore;
	fopen_s(&fp_ModelScore, strModelScoreFile,"rb");
	double* pModelScores;
	int** ppEpoch=NULL;
	int** ppEpochData; //All channels
	if(fp_EEGCnt==NULL || fp_ModelScore==NULL) return false;

	CSvmClassify* pSVM=new CSvmClassify();
	if(!pSVM->Initialize(strSVMModelFile,strRangeFile)){delete pSVM;return false;}

	///-------------------------------Read Calibration Data-----------------------------------------
	printf("Loading calibration (garbage) data for rejection model...\n");

	fseek(fp_EEGCnt,0,SEEK_END);
	int iCntFileLen=ftell(fp_EEGCnt);
	fseek(fp_EEGCnt,0,SEEK_SET);
	pCntFileData=new BYTE[iCntFileLen];
	fread(pCntFileData,1,iCntFileLen,fp_EEGCnt);
	BYTE* pCntData=pCntFileData+sizeof(CNTFileHeader);
	memcpy(&pEEGContFile->m_CNTHeader,pCntFileData,sizeof(CNTFileHeader));

	int iSampleSize=(pEEGContFile->m_CNTHeader.iNumCh+1)*pEEGContFile->m_CNTHeader.iNumByte;
	int iNumSample=(iCntFileLen-sizeof(CNTFileHeader))/iSampleSize;

	int iStartSample,iNumSampleInEpoch;
	//iStartSample=0; ccwang -->
	iStartSample = miP300RecordStartTime * miSamplingRate / 1000;
	iNumSampleInEpoch=miNumSampInRawEpochs;
	
	int iNumEpochPerRound;
	int iNumRoundPerTarget;
	int iNumTarget;
	fread(&iNumEpochPerRound,1,sizeof(int),fp_ModelScore);
	fread(&iNumRoundPerTarget,1,sizeof(int),fp_ModelScore);
	fread(&iNumTarget,1,sizeof(int),fp_ModelScore);
	int iNumAllEpoch=iNumEpochPerRound*iNumRoundPerTarget*iNumTarget;
	pModelScores=new double[iNumAllEpoch];
	fread(pModelScores,sizeof(double),iNumAllEpoch,fp_ModelScore);	
	
	//Create dummy epochs from CNT file
    ppEpoch = new int*[iNumAllEpoch];
	int iStartEpochSample = 0;
	int iEndEpochSample = iNumSample - iNumSampleInEpoch;
	int iChunkLen = (iEndEpochSample-iStartEpochSample) / iNumAllEpoch;
	for(int i = 0; i < iNumAllEpoch; i++) {
		int iOffSet = iStartEpochSample + i*iChunkLen;
		ppEpoch[i] = (int*) (pCntData+iSampleSize*iOffSet);
	}

	pEEGContFile->GetEpochData(iStartSample,iNumSampleInEpoch,ppEpoch,iNumAllEpoch,ppEpochData);	

	///----------------------------Extract Features and Calculate SVM Scores-------------------
	printf("Calculating SVM scores for each epoch...\n");

	double* pScores=new double[iNumAllEpoch];
	double fPredict;

	for(int i=0;i<iNumAllEpoch;i++){
		int iFeatureSize=ExtractFeaturesFromEEGBuffer(ppEpochData[i],mpSegment);
		pSVM->Classify(mpFeatures,iFeatureSize,pScores+i,&fPredict);
		pScores[i]=(pScores[i]+1)*100; ///Change Scale, see IsRejection()
		delete ppEpochData[i];ppEpochData[i]=NULL;
	}

	///^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

	//Clear some allocated memory
	delete [] ppEpoch;
	delete pEEGContFile;
	delete [] ppEpochData;

	delete pSVM;
	//----------------------------

	///--------------Calculate the thresholds and equal error rates--------------------------------
	printf("Learning the thresholds and equal error rates....\n");

	double* pPostProbs;double* pPostProbsGarbage;
	//Thresholds for Equal error rate:

	double* fThd_EER=new double[miNumRoundPerTarget]; //Local memory clearance is not needed since the memory will be passed to mfEERRejectThrs
	double* fTargetProbMeans=new double[miNumRoundPerTarget];
	double* fGarbageProbMeans=new double[miNumRoundPerTarget];

	double* fThd_DesiredFalseRej=new double[miNumRoundPerTarget];

	int iNumEpochPerTarget=miNumEpochPerRound*miNumRoundPerTarget;

	printf("-----------------------------Finding the equal error rates and corresponding thresholds--------------\n");
	for(int iRejWinWidth=1;iRejWinWidth<=miNumRoundPerTarget;iRejWinWidth++){
		int iNumRejWinPerTarget=miNumRoundPerTarget-iRejWinWidth+1;
		double* pScoresTarget;double* pScoresGarbage;

		pPostProbs=new double[iNumRejWinPerTarget*iNumTarget]; //numTarget X numRejWin
		pPostProbsGarbage=new double[iNumRejWinPerTarget*iNumTarget];

		for(int iTarget=0;iTarget<iNumTarget;iTarget++){
			for(int iWin=0;iWin<iNumRejWinPerTarget;iWin++){
				int idx_rejwin=iTarget*iNumRejWinPerTarget+iWin;
                pScoresTarget=pModelScores+iTarget*iNumEpochPerTarget+iWin*miNumEpochPerRound;//pScoreBuffer: the iTarget-th target the iWin-th round
                pScoresGarbage=pScores+iTarget*iNumEpochPerTarget+iWin*miNumEpochPerRound;
				pPostProbs[idx_rejwin]=CalcPostProb(pScoresTarget,miNumEpochPerRound,iRejWinWidth);
				pPostProbsGarbage[idx_rejwin]=CalcPostProb(pScoresGarbage,miNumEpochPerRound,iRejWinWidth);
			}
		}
		fTargetProbMeans[iRejWinWidth-1]=0;
		fGarbageProbMeans[iRejWinWidth-1]=0;
		int i;
		for(i=0;i<iNumRejWinPerTarget*iNumTarget;i++) fTargetProbMeans[iRejWinWidth-1]+=pPostProbs[i];
		for(i=0;i<iNumRejWinPerTarget*iNumTarget;i++) fGarbageProbMeans[iRejWinWidth-1]+=pPostProbsGarbage[i];
		fGarbageProbMeans[iRejWinWidth-1]/=iNumRejWinPerTarget*iNumTarget;
		fTargetProbMeans[iRejWinWidth-1]/=iNumRejWinPerTarget*iNumTarget;

		double fEER=FindEqualErrorRate(pPostProbs, pPostProbsGarbage, iNumRejWinPerTarget*iNumTarget, iNumRejWinPerTarget*iNumTarget, &fThd_EER[iRejWinWidth-1]);
		float fDesiredFalseRejRate=0.05f;
		double fRate_FalseAcceptAtDesiredFalseRej=FindThd4GivenFalseRejRate(pPostProbs, pPostProbsGarbage, iNumRejWinPerTarget*iNumTarget, iNumRejWinPerTarget*iNumTarget, &fThd_DesiredFalseRej[iRejWinWidth-1],fDesiredFalseRejRate);
		printf("Window width (rounds): %d | Equal Error Rate: %.3f | FalseAcceptRate@LowRej: %.3f(%.3f)\n",iRejWinWidth,fEER,fRate_FalseAcceptAtDesiredFalseRej,fDesiredFalseRejRate);
		DisplayErrorRateOverThresholdBias(pPostProbs,pPostProbsGarbage,iNumRejWinPerTarget*iNumTarget,iNumRejWinPerTarget*iNumTarget,fThd_DesiredFalseRej[iRejWinWidth-1],fTargetProbMeans[iRejWinWidth-1],fGarbageProbMeans[iRejWinWidth-1]);
		delete pPostProbs;delete pPostProbsGarbage;
	}

	//Set the member variables
	miNumRejectThrs = miNumRoundPerTarget;
	mfEERRejectThrs = fThd_EER;
	mfDesiredFalseRejThrs=fThd_DesiredFalseRej;
	mfTargetProbMeans = fTargetProbMeans;
	mfGarbageProbMeans = fGarbageProbMeans;

	//^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
	return true;
}

double CP300SignalProc::FindThd4GivenFalseRejRate(double* pPostProbs, double* pPostProbsGarbage, int iNumRejWin, int iNumRejWinGarbage, double* pThd, float fDesiredFalseRejRate)
{
	//Enumerate all possible thresholds in the given probs
	int iNumThdCand=iNumRejWin+iNumRejWinGarbage;
	double* pAllThd=new double[iNumThdCand];
	memcpy(pAllThd,pPostProbs,iNumRejWin*sizeof(double));
    memcpy(pAllThd+iNumRejWin,pPostProbsGarbage,iNumRejWinGarbage*sizeof(double));
	bool iBadOrder=true;
	//Sorting from high to low
	while(iBadOrder){
		iBadOrder=false;
		for(int i=0;i<iNumRejWin-1;i++){
			if(pAllThd[i]<pAllThd[i+1]){
				iBadOrder=true;
				double ftmp;
				ftmp=pAllThd[i];
				pAllThd[i]=pAllThd[i+1];
				pAllThd[i+1]=ftmp;
			}
		}
	}

	double fFalseRejRate,fFalseAcceptRate,fRate_FalseAcceptAtDesiredFalseRej;
	int iThd;
	for(iThd=0;iThd<iNumThdCand;iThd++){
		int iWin;
		int iNumErr=0;int iNumErrGarbage=0;
		for(iWin=0;iWin<iNumRejWin;iWin++) iNumErr+=pPostProbs[iWin]>=pAllThd[iThd]?0:1;
		for(iWin=0;iWin<iNumRejWinGarbage;iWin++) iNumErrGarbage+=pPostProbsGarbage[iWin]<pAllThd[iThd]?0:1;
		fFalseRejRate=iNumErr*1.0/iNumRejWin;
        fFalseAcceptRate=iNumErrGarbage*1.0/iNumRejWinGarbage;
		if(fFalseRejRate<fDesiredFalseRejRate) break;
 	}
	*pThd=pAllThd[iThd];
	printf("Thd: %e\n",*pThd);
	fRate_FalseAcceptAtDesiredFalseRej=fFalseAcceptRate;

    return fRate_FalseAcceptAtDesiredFalseRej;
}


void CP300SignalProc::DisplayErrorRateOverThresholdBias(double* pPostProbs, double* pPostProbsGarbage, int iNumRejWin, int iNumRejWinGarbage, double fThd, double fMeanProb_Target, double fMeanProb_Garbage)
{
	double fFalseRejRate,fFalseAcceptRate;
	int iWin,iBias,iNumErr,iNumErrGarbage;
	double fBiasedThd;
	fMeanProb_Garbage=log(fMeanProb_Garbage);
	fMeanProb_Target=log(fMeanProb_Target);
	fThd=log(fThd);

	for (iBias = -100;iBias <= 100;iBias = iBias+10) {
		if (iBias<0) fBiasedThd=(fMeanProb_Garbage-fThd)*(-iBias)/100+fThd;
		else fBiasedThd=(fMeanProb_Target-fThd)*iBias/100+fThd;
		iNumErr=0;iNumErrGarbage=0;
		for (iWin=0;iWin<iNumRejWin;iWin++) iNumErr += log(pPostProbs[iWin])>=fBiasedThd?0:1;
		for (iWin=0;iWin<iNumRejWinGarbage;iWin++) iNumErrGarbage += log(pPostProbsGarbage[iWin])<fBiasedThd?0:1;
		fFalseRejRate = iNumErr*1.0/iNumRejWin;
        fFalseAcceptRate = iNumErrGarbage*1.0/iNumRejWinGarbage;
		printf("\t\tBias %d: Biased Thd: %.2f False Rejection: %.2f False Acceptance: %.2f\n",iBias,fBiasedThd,fFalseRejRate,fFalseAcceptRate);
 	}
}

bool CP300SignalProc::RetrainRejThr_For_DiffNumTarget(void)
{
	char strModelScoreFile[1024];
	if(mpResources->Get("Process","SVMModelScore",strModelScoreFile, sizeof(strModelScoreFile)) != errOkay){
		printf ("SVM model score file must be given in configuration file %s under P300Process:SVMModelScore.\n",(char *)CONFIG_FILE_NAME);
		return false;
	}

	// Data with Org #Target
	int iOrg_NumEpochPerRound;
	int iOrg_NumRoundPerTarget;
	int iOrg_NumTarget;
	int iOrg_NumTargetEpoch;
	int iOrg_NumNonTargetEpoch;
	double* pOrg_TargetSVMScores;
	double* pOrg_NonTargetSVMScores;
	int* pOrg_TargetEpochCode;
	int* pOrg_NonTargetEpochCode;

	// Data with New #Target
	int iNumEpochPerRound;
	int iNumRoundPerTarget;
	int iNumTarget;
	int iNumTargetEpoch;
	int iNumNonTargetEpoch;
	double* pTargetSVMScores;
	double* pNonTargetSVMScores;
	int* pTargetEpochCode;
	int* pNonTargetEpochCode;

	FILE* fp;
	if (fopen_s(&fp, strModelScoreFile,"rb")) return false;

	fread(&iOrg_NumEpochPerRound,sizeof(int),1,fp);
	fread(&iOrg_NumRoundPerTarget,sizeof(int),1,fp);
	fread(&iOrg_NumTarget,sizeof(int),1,fp);
	fread(&iOrg_NumTargetEpoch,sizeof(int),1,fp);
	fread(&iOrg_NumNonTargetEpoch,sizeof(int),1,fp);
	pOrg_TargetSVMScores=new double[iOrg_NumTargetEpoch];
	pOrg_NonTargetSVMScores=new double[iOrg_NumNonTargetEpoch];
    pOrg_TargetEpochCode=new int[iOrg_NumTargetEpoch];
	pOrg_NonTargetEpochCode=new int[iOrg_NumNonTargetEpoch];
	fread(pOrg_TargetSVMScores,sizeof(double),iOrg_NumTargetEpoch,fp);
	fread(pOrg_TargetEpochCode,sizeof(int),iOrg_NumTargetEpoch,fp);
	fread(pOrg_NonTargetSVMScores,sizeof(double),iOrg_NumNonTargetEpoch,fp);
	fread(pOrg_NonTargetEpochCode,sizeof(int),iOrg_NumNonTargetEpoch,fp);
	fclose(fp);


	//Set Initial Data with New #Target
	iNumEpochPerRound=miNumEpochPerRound;//Will not change hereafter
	iNumRoundPerTarget=iOrg_NumRoundPerTarget;//Will not change hereafter
	pNonTargetSVMScores=NULL;
	pNonTargetEpochCode=NULL;

	if(iOrg_NumEpochPerRound!=iOrg_NumTarget) return false; //  || iNumRoundPerTarget>iOrg_NumTarget

	// Assuming miNumEpochPerRound!=iOrg_NumEpochPerRound
	// Simulating Scores with miNumEpochPerRound


	int iEpoch, iRound, iEpochCode;
	int iOrgEpoch,iEpochPos,iOrgEpochPos;
	int	iTargetEpoch, iNonTargetEpoch;

	int iOrgTargetEpoch,iOrgNonTargetEpoch;

	int iNumAllEpoch;

	if(iOrg_NumEpochPerRound<miNumEpochPerRound){
		iNumTarget=iOrg_NumTarget;
		iNumTargetEpoch=iOrg_NumTargetEpoch;
		iNumAllEpoch=iNumEpochPerRound*iNumRoundPerTarget*iNumTarget;
		iNumNonTargetEpoch=iNumAllEpoch-iNumTargetEpoch;
		pNonTargetSVMScores=new double[iNumNonTargetEpoch];
		pNonTargetEpochCode=new int[iNumNonTargetEpoch];
		pTargetSVMScores=new double[iNumTargetEpoch];
		pTargetEpochCode=new int[iNumTargetEpoch];

		for(iRound=0;iRound<iOrg_NumRoundPerTarget*iOrg_NumTarget;iRound++){
			for(iEpoch=0;iEpoch<miNumEpochPerRound-1;iEpoch++){
				iOrgEpoch=iEpoch%(iOrg_NumEpochPerRound-1);
				iEpochPos=iEpoch+iRound*(miNumEpochPerRound-1);
				iOrgEpochPos=iOrgEpoch+iRound*(iOrg_NumEpochPerRound-1);
				pNonTargetSVMScores[iEpochPos]=pOrg_NonTargetSVMScores[iOrgEpochPos];
				if(iEpoch>=iOrg_NumEpochPerRound-1) pNonTargetEpochCode[iEpochPos]=iEpoch;
				else pNonTargetEpochCode[iEpochPos]=pOrg_NonTargetEpochCode[iOrgEpochPos];
			}
		}
		//Copy Target Scores
		memcpy(pTargetSVMScores,pOrg_TargetSVMScores,sizeof(double)*iNumTargetEpoch);
		memcpy(pTargetEpochCode,pOrg_TargetEpochCode,sizeof(int)*iNumTargetEpoch);
	}
	else{
		iNumTarget=min(iOrg_NumTarget,iNumEpochPerRound);
		iNumTargetEpoch=iNumEpochPerRound*iNumRoundPerTarget*iNumTarget;
		iNumAllEpoch=iNumEpochPerRound*iNumRoundPerTarget*iNumTarget;
		iNumNonTargetEpoch=iNumAllEpoch-iNumTargetEpoch;

		pNonTargetSVMScores=new double[iNumNonTargetEpoch];
		pNonTargetEpochCode=new int[iNumNonTargetEpoch];
		pTargetSVMScores=new double[iNumTargetEpoch];
		pTargetEpochCode=new int[iNumTargetEpoch];

		iTargetEpoch=0;iNonTargetEpoch=0;
		for(iOrgTargetEpoch=0;iOrgTargetEpoch<iOrg_NumTargetEpoch;iOrgTargetEpoch++){
			iEpochCode=pOrg_TargetEpochCode[iOrgTargetEpoch];
			if(iEpochCode>iNumTarget) continue;
			pTargetSVMScores[iTargetEpoch]=pOrg_TargetSVMScores[iOrgTargetEpoch];
			pTargetEpochCode[iTargetEpoch]=pOrg_TargetEpochCode[iOrgTargetEpoch];

			iEpoch=iOrgTargetEpoch*(iOrg_NumEpochPerRound-1);
			int iCountEpoch=0;
			for(iOrgNonTargetEpoch=iEpoch;iOrgNonTargetEpoch<iEpoch+(iOrg_NumEpochPerRound-1);iOrgNonTargetEpoch){
                if(pOrg_NonTargetEpochCode[iOrgNonTargetEpoch]>iNumTarget) continue;
                pNonTargetEpochCode[iNonTargetEpoch+iCountEpoch]=pOrg_TargetEpochCode[iOrgNonTargetEpoch];
				pOrg_TargetSVMScores[iNonTargetEpoch+iCountEpoch]=pOrg_TargetSVMScores[iOrgNonTargetEpoch];
				iCountEpoch++;
			}
			if(iCountEpoch!=iNumEpochPerRound-1) return false;
			iNonTargetEpoch+=iCountEpoch;
			iTargetEpoch++;
		}
	}
	delete pOrg_TargetSVMScores;pOrg_TargetSVMScores=NULL;
	delete pOrg_TargetEpochCode;pOrg_TargetEpochCode=NULL;
	delete pOrg_NonTargetSVMScores;pOrg_NonTargetSVMScores=NULL;
	delete pOrg_NonTargetEpochCode;pOrg_NonTargetEpochCode=NULL;

	double *pScoreBuffer,*pScoreBufferGarbage;
	PutScoreBufferInOrder(pScoreBuffer,pTargetSVMScores, pNonTargetSVMScores, iNumTargetEpoch, iNumNonTargetEpoch, pTargetEpochCode, pNonTargetEpochCode, iNumEpochPerRound);
	SimGarbageScoreBuffer(pScoreBufferGarbage, pScoreBuffer,iNumEpochPerRound,iNumTargetEpoch,iNumRoundPerTarget);//iNumRound=iNumTargetEpoch;
	delete pTargetSVMScores;pTargetSVMScores=NULL;
	delete pTargetEpochCode;pTargetEpochCode=NULL;
	delete pNonTargetSVMScores;pNonTargetSVMScores=NULL;
	delete pNonTargetEpochCode;pNonTargetEpochCode=NULL;


	// --------------------------------------------------

	miNumRejectThrs = iNumRoundPerTarget;

	int iNumEpochPerTarget=(iNumTargetEpoch+iNumNonTargetEpoch)/iNumTarget;
	CalcThreshold_For_EER(mfEERRejectThrs, mfTargetProbMeans, mfGarbageProbMeans, pScoreBuffer, pScoreBufferGarbage, iNumRoundPerTarget, iNumTarget,iNumEpochPerTarget);
	delete mfTargetProbMeans; delete mfGarbageProbMeans;
	float fDesiredFalseRejRate=0.05f;
	CalcThreshold_For_DesiredFalseRejRate(mfDesiredFalseRejThrs,mfTargetProbMeans,mfGarbageProbMeans,pScoreBuffer,pScoreBufferGarbage,iNumRoundPerTarget,iNumTarget,iNumEpochPerTarget,fDesiredFalseRejRate);


	return true;
}

bool CP300SignalProc::CalcThreshold_For_EER(double*& fThd_EER, double*& fTargetProbMeans, double*& fGarbageProbMeans, double* pScoreBuffer, double* pScoreBufferGarbage, int iNumRoundPerTarget, int iNumTarget, int iNumEpochPerTarget)
{
	printf("Learning the thresholds and equal error rates....\n");

	double* pPostProbs;double* pPostProbsGarbage;
	//Thresholds for Equal error rate:

	fThd_EER=new double[iNumRoundPerTarget]; //Local memory clearance is not needed since the memory will be passed to mfEERRejectThrs
	fTargetProbMeans=new double[iNumRoundPerTarget];
	fGarbageProbMeans=new double[iNumRoundPerTarget];

	printf("-----------------------------Finding the equal error rates and corresponding thresholds--------------\n");
	for(int iRejWinWidth=1;iRejWinWidth<=iNumRoundPerTarget;iRejWinWidth++){
		int iNumRejWinPerTarget=iNumRoundPerTarget-iRejWinWidth+1;
		double* pScores;double* pScoresGarbage;

		pPostProbs=new double[iNumRejWinPerTarget*iNumTarget]; //numTarget X numRejWin
		pPostProbsGarbage=new double[iNumRejWinPerTarget*iNumTarget];

		for(int iTarget=0;iTarget<iNumTarget;iTarget++){
			for(int iWin=0;iWin<iNumRejWinPerTarget;iWin++){
				int idx_rejwin=iTarget*iNumRejWinPerTarget+iWin;
                pScores=pScoreBuffer+iTarget*iNumEpochPerTarget+iWin*miNumEpochPerRound;//pScoreBuffer: the iTarget-th target the iWin-th round
                pScoresGarbage=pScoreBufferGarbage+iTarget*iNumEpochPerTarget+iWin*miNumEpochPerRound;
				pPostProbs[idx_rejwin]=CalcPostProb(pScores,miNumEpochPerRound,iRejWinWidth);
				pPostProbsGarbage[idx_rejwin]=CalcPostProb(pScoresGarbage,miNumEpochPerRound,iRejWinWidth);
			}
		}
		fTargetProbMeans[iRejWinWidth-1]=0;
		fGarbageProbMeans[iRejWinWidth-1]=0;
		int i;
		for(i=0;i<iNumRejWinPerTarget*iNumTarget;i++) fTargetProbMeans[iRejWinWidth-1]+=pPostProbs[i];
		for(i=0;i<iNumRejWinPerTarget*iNumTarget;i++) fGarbageProbMeans[iRejWinWidth-1]+=pPostProbsGarbage[i];
		fGarbageProbMeans[iRejWinWidth-1]/=iNumRejWinPerTarget*iNumTarget;
		fTargetProbMeans[iRejWinWidth-1]/=iNumRejWinPerTarget*iNumTarget;

		printf("Finding Equal Error Rates....\n");
		double fEER=FindEqualErrorRate(pPostProbs, pPostProbsGarbage, iNumRejWinPerTarget*iNumTarget, iNumRejWinPerTarget*iNumTarget, &fThd_EER[iRejWinWidth-1]);
		printf("Window width (rounds): %d | Equal Error Rate: %.3f\n",iRejWinWidth,fEER);
		delete pPostProbs;delete pPostProbsGarbage;
	}

	return true;
}
bool CP300SignalProc::CalcThreshold_For_DesiredFalseRejRate(double*& fThd_DesiredFalseRej, double*& fTargetProbMeans, double*& fGarbageProbMeans, double* pScoreBuffer, double* pScoreBufferGarbage, int iNumRoundPerTarget, int iNumTarget, int iNumEpochPerTarget, float fDesiredFalseRejRate)
{
	printf("Learning the thresholds for given desired false rejection rate....\n");

	
	//Thresholds for Equal error rate:

	fTargetProbMeans=new double[iNumRoundPerTarget];
	fGarbageProbMeans=new double[iNumRoundPerTarget];

	fThd_DesiredFalseRej=new double[iNumRoundPerTarget];

	printf("-----------------------------Finding the thresholds--------------\n");
	for(int iRejWinWidth=1;iRejWinWidth<=iNumRoundPerTarget;iRejWinWidth++){
		double* pPostProbs;double* pPostProbsGarbage;

		int iNumRejWinPerTarget=iNumRoundPerTarget-iRejWinWidth+1;
		double* pScores;double* pScoresGarbage;

		pPostProbs=new double[iNumRejWinPerTarget*iNumTarget]; //numTarget X numRejWin
		pPostProbsGarbage=new double[iNumRejWinPerTarget*iNumTarget];

		for(int iTarget=0;iTarget<iNumTarget;iTarget++){
			for(int iWin=0;iWin<iNumRejWinPerTarget;iWin++){
				int idx_rejwin=iTarget*iNumRejWinPerTarget+iWin;
                pScores=pScoreBuffer+iTarget*iNumEpochPerTarget+iWin*miNumEpochPerRound;//pScoreBuffer: the iTarget-th target the iWin-th round
                pScoresGarbage=pScoreBufferGarbage+iTarget*iNumEpochPerTarget+iWin*miNumEpochPerRound;
				pPostProbs[idx_rejwin]=CalcPostProb(pScores,miNumEpochPerRound,iRejWinWidth);
				pPostProbsGarbage[idx_rejwin]=CalcPostProb(pScoresGarbage,miNumEpochPerRound,iRejWinWidth);
			}
		}
		fTargetProbMeans[iRejWinWidth-1]=0;
		fGarbageProbMeans[iRejWinWidth-1]=0;
		int i;
		for(i=0;i<iNumRejWinPerTarget*iNumTarget;i++) fTargetProbMeans[iRejWinWidth-1]+=pPostProbs[i];
		for(i=0;i<iNumRejWinPerTarget*iNumTarget;i++) fGarbageProbMeans[iRejWinWidth-1]+=pPostProbsGarbage[i];
		fGarbageProbMeans[iRejWinWidth-1]/=iNumRejWinPerTarget*iNumTarget;
		fTargetProbMeans[iRejWinWidth-1]/=iNumRejWinPerTarget*iNumTarget;

		printf("Finding Desired False Rejection Threshold...\n");
		double fRate_FalseAcceptAtDesiredFalseRej=FindThd4GivenFalseRejRate(pPostProbs, pPostProbsGarbage, iNumRejWinPerTarget*iNumTarget, iNumRejWinPerTarget*iNumTarget, &fThd_DesiredFalseRej[iRejWinWidth-1],fDesiredFalseRejRate);
		printf("Window width (rounds): %d | FalseAcceptRate@LowRej: %.3f(%.3f)\n",iRejWinWidth,fRate_FalseAcceptAtDesiredFalseRej,fDesiredFalseRejRate);
		printf("Display Bias and Effects:\n");
		//DisplayErrorRateOverThresholdBias(pPostProbs,pPostProbsGarbage,iNumRejWinPerTarget*iNumTarget,iNumRejWinPerTarget*iNumTarget,fThd_DesiredFalseRej[iRejWinWidth-1],fTargetProbMeans[iRejWinWidth-1],fGarbageProbMeans[iRejWinWidth-1]);
		delete pPostProbs;delete pPostProbsGarbage;
	}

	return true;
}


void CP300SignalProc::GetResult_OneVariLen(double *OrgScore, unsigned short *cOrgStimCode, int iQHead, int iQTail, int iQTotalLen, int iNumStimPerRound, RESULT *pResult)
{
	int iNumEpoch=iQHead-iQTail;
	if(iNumEpoch<=0) iNumEpoch+=iQTotalLen;
	int iNumRound=iNumEpoch/iNumStimPerRound;
	
	/*----Creating arrays of scores and stimcodes in time order---------*/
	double* Score=new double[iNumEpoch];
	USHORT* cStimCode=new USHORT[iNumEpoch];
	int iEpoch,iEpochPos;
	for (iEpoch=0;iEpoch<iNumEpoch;iEpoch++){
		iEpochPos=iEpoch+iQTail;
		if(iEpochPos>=iQTotalLen) iEpochPos=iEpochPos-iQTotalLen;
		Score[iEpoch]=OrgScore[iEpochPos];
		cStimCode[iEpoch]=cOrgStimCode[iEpochPos];
	}

	//int iTaskType;
	//iTaskType = GetTaskType();
	//if (iTaskType == WORDSPELLER) 
		GetResultWordSpeller(Score, cStimCode, iNumStimPerRound, iNumRound, pResult);
	//else if (iTaskType == CARDGAME) 
	//	GetResultCardGame(Score, cStimCode, iNumStimPerRound, iNumRound, pResult);
	//else
	//	printf ("P300SignalProc::GetResult: Task undefined in %d\n", iTaskType);

	if(m_fRejThreshold_ManualSet != FLT_MIN){
		m_fThd = m_fRejThreshold_ManualSet;
		IsRejection_OneVariLen(Score ,cStimCode, iNumStimPerRound, iNumRound, pResult);
	}
	//else if (mfDesiredFalseRejThrs) {
	//	double fThd = log(mfDesiredFalseRejThrs[iNumRound-1]);
	//	double fTargetThd;
	//	if (m_iRejThrBias < 0)  fTargetThd=log(mfGarbageProbMeans[iNumRound-1]);
	//	else  fTargetThd = log(mfTargetProbMeans[iNumRound-1]);
	//	m_fThd = fThd + (fTargetThd-fThd)*abs(m_iRejThrBias)/100;
	//	IsRejection_OneVariLen(Score ,cStimCode, iNumStimPerRound, iNumRound, pResult);
	//}
	else if (mfEERRejectThrs) {
		double fEERThd=m_fEERDetectThr_VariLen;
		double fTargetThd = m_fEERDetectThr_VariLen;
		if(m_iRejThrBias < 0)  fTargetThd = m_fL_Idle_VariLen;
		else if(m_iRejThrBias > 0)  fTargetThd = m_fL_Control_VariLen;
		m_fThd = fEERThd + (fTargetThd-fEERThd) * abs(m_iRejThrBias)/100;
		IsRejection_OneVariLen(Score ,cStimCode, iNumStimPerRound, iNumRound, pResult);
	}

	delete Score;
	delete cStimCode; 
	return;
}

int CP300SignalProc::IsRejection_OneVariLen(double *fSVMScores ,unsigned short *cStimCode, int nStim,int nRound, RESULT *result)
{
	double fPControl=0.5;
	int iNumStimPerChar = nStim * nRound;
	double* pPriorPTarget=new double[nStim];
	double* pLogLikelihoodTarget=new double[nStim];
	double* pScoreBufferInOrder=new double[nStim*nRound];
	double fL=0.0f;
	double fX=0.0f;//the sum of exp (prob(target i, D))
	double fPriorIdle=fPControl;
	double fLogLikelihoodIdle, fMeanLogLikelihoodTarget;
	int idx_roundstart,idx,istimcode, iRound, iStim;

	memset(pScoreBufferInOrder,0,sizeof(double)*nStim*nRound);
	memset(pLogLikelihoodTarget,0,sizeof(double)*nStim);

	if(NULL==cStimCode)
		memcpy(pScoreBufferInOrder,fSVMScores,sizeof(double)*nStim*nRound);
	else
	{
		for(iRound=0;iRound<nRound;iRound++){
			idx_roundstart=iRound*nStim;
			for(iStim=0;iStim<nStim;iStim++){
				idx=idx_roundstart+iStim;
				istimcode=cStimCode[idx]-1;
				pScoreBufferInOrder[idx_roundstart+istimcode]=fSVMScores[idx];
				//Normalize scores
				pScoreBufferInOrder[idx_roundstart+istimcode]=(pScoreBufferInOrder[idx_roundstart+istimcode]+1)*100;
			}
		}
	}

	for (iStim=0;iStim<nStim;iStim++) pPriorPTarget[iStim]=fPControl/nStim; //Uniform distribution over possible targets

	//Calculate the likelihood of idle
	fLogLikelihoodIdle=0.0f;
	for (idx=0;idx<nRound*nStim;idx++)
		fLogLikelihoodIdle+=(-0.5*(pScoreBufferInOrder[idx]-m_EpochScoreStats.fMean_Idle)*(pScoreBufferInOrder[idx]-m_EpochScoreStats.fMean_Idle)/(m_EpochScoreStats.fStd_Idle*m_EpochScoreStats.fStd_Idle))-log(2*PI*m_EpochScoreStats.fStd_Idle);

	fMeanLogLikelihoodTarget=0.0f;
	for(iStim=0;iStim<nStim;iStim++){
		for(idx=0;idx<nRound*nStim;idx++){
			if(idx%nStim==iStim)
				pLogLikelihoodTarget[iStim]+=(-0.5*(pScoreBufferInOrder[idx]-m_EpochScoreStats.fMean_Target)*(pScoreBufferInOrder[idx]-m_EpochScoreStats.fMean_Target)/(m_EpochScoreStats.fStd_Target*m_EpochScoreStats.fStd_Target))-log(2*PI*m_EpochScoreStats.fStd_Target);
			else
				pLogLikelihoodTarget[iStim]+=(-0.5*(pScoreBufferInOrder[idx]-m_EpochScoreStats.fMean_NonTarget)*(pScoreBufferInOrder[idx]-m_EpochScoreStats.fMean_NonTarget)/(m_EpochScoreStats.fStd_NonTarget*m_EpochScoreStats.fStd_NonTarget))-log(2*PI*m_EpochScoreStats.fStd_NonTarget);
		}
		pLogLikelihoodTarget[iStim]+=log(pPriorPTarget[iStim]);//Now pLogLikelihoodTarget contains the joint probability of observation and Target iStim!
		fMeanLogLikelihoodTarget+=pLogLikelihoodTarget[iStim]/nStim;
	}
	
	for(iStim=0;iStim<nStim;iStim++){
		fX+=exp(pLogLikelihoodTarget[iStim]-fMeanLogLikelihoodTarget);
	}
	fL=log(fX)-fMeanLogLikelihoodTarget-fLogLikelihoodIdle-log(fPriorIdle);

	//Tricky: using the largest
	bool bUseMaxL=true;
	if(bUseMaxL){
		fL=-FLT_MAX;
		for(iStim=0;iStim<nStim;iStim++){
			fL=max(fL,pLogLikelihoodTarget[iStim]);
		}
		fL=fL/nRound;
	}
	int iIsReject = 0;
	if (fL < m_fThd)
		iIsReject = 1;

	result->iReject = iIsReject;
	result->fConfidence = (float) fL;	// log scale
	result->fResult = (float) m_fThd; //borrow this variable to store threshold, log scale

	delete pScoreBufferInOrder;
	delete pPriorPTarget;
	delete pLogLikelihoodTarget;

	return iIsReject;
}

void CP300SignalProc::GetResult_MultiVariLen(double *OrgScore, unsigned short *cOrgStimCode, int iQHead, int iQTail, int iQTotalLen, int iNumStimPerRound, RESULT *pResult)
{
	int iNumEpoch=iQHead-iQTail;
	if(iNumEpoch<=0) iNumEpoch+=iQTotalLen;
	int iNumRound=iNumEpoch/iNumStimPerRound;
	iNumRound=min(iNumRound,m_iMaxLen_VariLen);

	bool bIsReject=true;
	double fMaxL=-FLT_MAX;

	int iCurQTail;
	int iResult=1;
	for(int iTestNRound=m_iMinLen_VariLen;iTestNRound<=iNumRound;iTestNRound++){
		iCurQTail=iQHead-iTestNRound*iNumStimPerRound;
		if(iCurQTail<0) iCurQTail+=iQTotalLen;
		GetResult_OneVariLen(OrgScore, cOrgStimCode, iQHead, iCurQTail, iQTotalLen, iNumStimPerRound, pResult);
		bIsReject=bIsReject && (pResult->iReject==1);
		iResult=pResult->fConfidence>fMaxL?pResult->iResult:iResult;

		fMaxL=max(fMaxL,pResult->fConfidence);
	}
	pResult->fConfidence = (float) fMaxL;
	if(bIsReject) pResult->iReject=1;
	else
		pResult->iReject=0;
}

bool CP300SignalProc::TrainVariLenDetectModel(char* strEvalFileList, char* strIdleFileList_VariLenDetect, CEEGContFile* pRefEEGContFile, char* strModelFile, char* strRangeFile, char* strModelScoreFile)
{
	//Note: we assume that all training files are with the same #epochPerRound, i.e. same #button == miNumEpochPerRound
	int i;
	//------------------Load EvalFileList and IdelFileList---------------
    CEEGContFile* pEEGContFile_Eval=new CEEGContFile;
	CEEGContFile* pEEGContFile_Idle=new CEEGContFile;
	
	printf("TrainVariLenDetectModel: Loading data files of control state...");
	if(!pEEGContFile_Eval->LoadDataFileList(strEvalFileList)){delete pEEGContFile_Eval;return false;}
	printf("\n");
	printf("TrainVariLenDetectModel: Loading data files of idle state...");
	if(!pEEGContFile_Idle->LoadDataFileList(strIdleFileList_VariLenDetect)){delete pEEGContFile_Idle;return false;}
	printf("\n");

	if(pEEGContFile_Eval->m_iNumAllEpoch%(pEEGContFile_Eval->m_iNumTarget* miNumEpochPerRound)!=0)
	{delete pEEGContFile_Eval;printf("Error in TrainVariLenDetectModel: number of Eval epochs.");return false;}
	if(pEEGContFile_Idle->m_iNumAllEpoch%(pEEGContFile_Idle->m_iNumTarget*miNumEpochPerRound)!=0){delete pEEGContFile_Eval;printf("Error in TrainVariLenDetectModel: number of Idle epochs.");return false;}

	if(memcmp(&pEEGContFile_Eval->m_CNTHeader,&pRefEEGContFile->m_CNTHeader,sizeof(_CNTFileHeader))!=0){delete pEEGContFile_Eval;printf("Error in TrainVariLenDetectModel: inconsistent header of eval data.");return false;}
	if(memcmp(&pEEGContFile_Idle->m_CNTHeader,&pRefEEGContFile->m_CNTHeader,sizeof(_CNTFileHeader))!=0){delete pEEGContFile_Idle;printf("Error in TrainVariLenDetectModel: inconsistent header of idle data.");return false;}

	int iNumTarget_Eval=pEEGContFile_Eval->m_iNumTarget;
	int iNumTarget_Idle=pEEGContFile_Idle->m_iNumTarget;
	//------------------Load SVM--and Get Scores---------------------------------------
	printf("TrainVariLenDetectModel: Calculating SVM scores...");
	CSvmClassify* pSVM=new CSvmClassify();
	if(!pSVM->Initialize(strModelFile,strRangeFile)){delete pSVM;return false;}
    
	double* pTargetSVMScores_Eval;
	double* pNonTargetSVMScores_Eval;
	int* pTargetEpochCode_Eval;
	int* pNonTargetEpochCode_Eval;
	double* pTargetSVMScores_Idle;
	double* pNonTargetSVMScores_Idle;
	int* pTargetEpochCode_Idle;
	int* pNonTargetEpochCode_Idle;

	int iNumRoundPerTarget_Eval,iNumTargetEpoch_Eval,iNumNonTargetEpoch_Eval;
	iNumRoundPerTarget_Eval=pEEGContFile_Eval->m_iNumAllEpoch/(miNumEpochPerRound*iNumTarget_Eval);
	CalcSVMScores_AllEpochs_in_EEGContFile(pEEGContFile_Eval,pSVM,pTargetSVMScores_Eval,pNonTargetSVMScores_Eval,pTargetEpochCode_Eval, pNonTargetEpochCode_Eval, miNumEpochPerRound, iNumRoundPerTarget_Eval, iNumTargetEpoch_Eval, iNumNonTargetEpoch_Eval);

	int iNumRoundPerTarget_Idle,iNumTargetEpoch_Idle,iNumNonTargetEpoch_Idle;
	iNumRoundPerTarget_Idle=pEEGContFile_Idle->m_iNumAllEpoch/(miNumEpochPerRound*iNumTarget_Idle);
	CalcSVMScores_AllEpochs_in_EEGContFile(pEEGContFile_Idle,pSVM,pTargetSVMScores_Idle,pNonTargetSVMScores_Idle,pTargetEpochCode_Idle, pNonTargetEpochCode_Idle, miNumEpochPerRound,iNumRoundPerTarget_Idle, iNumTargetEpoch_Idle, iNumNonTargetEpoch_Idle);

	delete pSVM;
	printf("Done.\n");
	///----------------------Univariate Gaussian Statistics----------------------
	printf("TrainVariLenDetectModel: Estimate Gaussian Statistics...");

	memset(&m_EpochScoreStats,0,sizeof(EpochScoreStatsTag));

	double* pAllIdleSVMScores=new double[pEEGContFile_Idle->m_iNumAllEpoch];
	memcpy(pAllIdleSVMScores,pTargetSVMScores_Idle,sizeof(double)*iNumTargetEpoch_Idle);
	memcpy(pAllIdleSVMScores+iNumTargetEpoch_Idle,pNonTargetSVMScores_Idle,sizeof(double)*iNumNonTargetEpoch_Idle);

	CLALib::EstGaussParas(pTargetSVMScores_Eval,iNumTargetEpoch_Eval,m_EpochScoreStats.fMean_Target,m_EpochScoreStats.fStd_Target);//***** Model Paras
	CLALib::EstGaussParas(pNonTargetSVMScores_Eval,iNumNonTargetEpoch_Eval,m_EpochScoreStats.fMean_NonTarget,m_EpochScoreStats.fStd_NonTarget);//***** Model Paras

	printf("---------Gaussian statistics--------\n");
	printf("Target: Mean: %f Std: %f\n",m_EpochScoreStats.fMean_Target,m_EpochScoreStats.fStd_Target);
	printf("NonTarget: Mean: %f Std: %f\n",m_EpochScoreStats.fMean_NonTarget,m_EpochScoreStats.fStd_NonTarget);

	CLALib::EstGaussParas(pAllIdleSVMScores,pEEGContFile_Idle->m_iNumAllEpoch,m_EpochScoreStats.fMean_Idle,m_EpochScoreStats.fStd_Idle);//***** Model Paras
	printf("Idle: Mean: %f Std: %f\n",m_EpochScoreStats.fMean_Idle,m_EpochScoreStats.fStd_Idle);
	delete pAllIdleSVMScores;

	m_EpochScoreStats.fStd_Target*=1.5;
	m_EpochScoreStats.fStd_NonTarget*=1.5;
	//m_EpochScoreStats.fStd_Idle*=2;
	//m_EpochScoreStats.fStd_Target=m_EpochScoreStats.fStd_NonTarget;
	//m_EpochScoreStats.fStd_Target=m_EpochScoreStats.fStd_Idle;
	//m_EpochScoreStats.fStd_NonTarget*=2;
	//m_EpochScoreStats.fStd_Idle*=m_EpochScoreStats.fStd_NonTarget;

	printf("Done.\n");

	//--------------Get Scores in order ------------------------------
	double *pScoreBuffer_Eval,*pScoreBuffer_Idle;
	PutScoreBufferInOrder(pScoreBuffer_Eval,pTargetSVMScores_Eval, pNonTargetSVMScores_Eval, iNumTargetEpoch_Eval, iNumNonTargetEpoch_Eval, pTargetEpochCode_Eval, pNonTargetEpochCode_Eval, miNumEpochPerRound);

	PutScoreBufferInOrder(pScoreBuffer_Idle,pTargetSVMScores_Idle, pNonTargetSVMScores_Idle, iNumTargetEpoch_Idle, iNumNonTargetEpoch_Idle, pTargetEpochCode_Idle, pNonTargetEpochCode_Idle, miNumEpochPerRound);

	delete pTargetSVMScores_Eval;
	delete pNonTargetSVMScores_Eval;
	delete pTargetEpochCode_Eval;
	delete pNonTargetEpochCode_Eval;
	delete pTargetSVMScores_Idle;
	delete pNonTargetSVMScores_Idle;
	delete pTargetEpochCode_Idle;
	delete pNonTargetEpochCode_Idle;

	//--------------Calculate all the L------------------------------------
	double*** pppL_Eval=new double**[iNumTarget_Eval];
	double*** pppL_Idle=new double**[iNumTarget_Idle];
	int iTarget,iPosTarget,iWinWidth;
	int iNumWinWidth=m_iMaxLen_VariLen-m_iMinLen_VariLen+1;

	int iCountAll;
	int iCount_Eval=0;
	int iCount_Idle=0;
	m_fL_Control_VariLen=0.0f;
	m_fL_Idle_VariLen=0.0f;
	for(iTarget=0;iTarget<iNumTarget_Eval;iTarget++){
        pppL_Eval[iTarget]=new double*[iNumWinWidth];
		iPosTarget=iTarget*(iNumRoundPerTarget_Eval*miNumEpochPerRound);
		for(int iIdxWinWidth=0;iIdxWinWidth<iNumWinWidth;iIdxWinWidth++){
			iWinWidth=iIdxWinWidth+m_iMinLen_VariLen;
			pppL_Eval[iTarget][iIdxWinWidth]=new double[iNumRoundPerTarget_Eval-iWinWidth+1];
			CalcL_Len(pScoreBuffer_Eval+iPosTarget, miNumEpochPerRound, iNumRoundPerTarget_Eval, pppL_Eval[iTarget][iIdxWinWidth], iWinWidth);
			m_fL_Control_VariLen+=CLALib::Sum(pppL_Eval[iTarget][iIdxWinWidth],iNumRoundPerTarget_Eval-iWinWidth+1);
			iCount_Eval+=iNumRoundPerTarget_Eval-iWinWidth+1;
		}
	}

	for(iTarget=0;iTarget<iNumTarget_Idle;iTarget++){
        pppL_Idle[iTarget]=new double*[iNumWinWidth];
		iPosTarget=iTarget*(iNumRoundPerTarget_Idle*miNumEpochPerRound);
		for(int iIdxWinWidth=0;iIdxWinWidth<iNumWinWidth;iIdxWinWidth++){
			iWinWidth=iIdxWinWidth+m_iMinLen_VariLen;
			pppL_Idle[iTarget][iIdxWinWidth]=new double[iNumRoundPerTarget_Idle-iWinWidth+1];
			CalcL_Len(pScoreBuffer_Idle+iPosTarget, miNumEpochPerRound, iNumRoundPerTarget_Idle, pppL_Idle[iTarget][iIdxWinWidth], iWinWidth);
			m_fL_Idle_VariLen+=CLALib::Sum(pppL_Idle[iTarget][iIdxWinWidth],iNumRoundPerTarget_Idle-iWinWidth+1);
			iCount_Idle+=iNumRoundPerTarget_Idle-iWinWidth+1;
		}
	}
	iCountAll=iCount_Eval+iCount_Idle;
	m_fL_Control_VariLen/=iCount_Eval;//***** Model Paras
	m_fL_Idle_VariLen/=iCount_Idle;//***** Model Paras

	printf("Mean_L_Control_VariLen:%f\n",m_fL_Control_VariLen);
	printf("Mean_L_Idle_VariLen:%f\n",m_fL_Idle_VariLen);
	double* pAllL=new double[iCountAll];
	iCountAll=0;

	for(iTarget=0;iTarget<iNumTarget_Eval;iTarget++){
		iPosTarget=iTarget*(iNumRoundPerTarget_Eval*miNumEpochPerRound);
		for(int iIdxWinWidth=0;iIdxWinWidth<iNumWinWidth;iIdxWinWidth++){
			iWinWidth=iIdxWinWidth+m_iMinLen_VariLen;
			memcpy(pAllL+iCountAll,pppL_Eval[iTarget][iIdxWinWidth],sizeof(double)*(iNumRoundPerTarget_Eval-iWinWidth+1));
			iCountAll+=iNumRoundPerTarget_Eval-iWinWidth+1;
		}
	}
	for(iTarget=0;iTarget<iNumTarget_Idle;iTarget++){
		iPosTarget=iTarget*(iNumRoundPerTarget_Idle*miNumEpochPerRound);
		for(int iIdxWinWidth=0;iIdxWinWidth<iNumWinWidth;iIdxWinWidth++){
			iWinWidth=iIdxWinWidth+m_iMinLen_VariLen;
			memcpy(pAllL+iCountAll,pppL_Idle[iTarget][iIdxWinWidth],sizeof(double)*(iNumRoundPerTarget_Idle-iWinWidth+1));
			iCountAll+=iNumRoundPerTarget_Idle-iWinWidth+1;
		}
	}

	//Sorting AllL
	CLALib::BubbleSort(pAllL,iCountAll,false);
	//Choose every iShiftStep samples
	int iShiftStep=30;
	int iNumThreshold = (int) floor((iCountAll/iShiftStep)*.8f);
	for(i=0;i<iNumThreshold;i++)
		pAllL[i]=pAllL[i*iShiftStep];
    
	//--------------------------------------------------------------------
	delete pScoreBuffer_Eval;
	delete pScoreBuffer_Idle;

	//-----------------Try each and every threshold------------------------
	int iThr;
	
	float* TAR=new float[iNumThreshold];//True acceptance rate
	float* TRR=new float[iNumThreshold];//True rejection rate
	float* AbsD_TARwTRR=new float[iNumThreshold];

	for(iThr=0;iThr<iNumThreshold;iThr++){
		m_fThd=pAllL[iThr];
		int iCountDetect=0;
		for(iTarget=0;iTarget<iNumTarget_Eval;iTarget++){
			int iStartRound_Segment=0;int iEndRound_Segment=0;
			int iSegmentLen;
			while(iEndRound_Segment<iNumRoundPerTarget_Eval){
				iSegmentLen=iEndRound_Segment-iStartRound_Segment+1;
				double fMaxL=-FLT_MAX;
				iSegmentLen=min(iSegmentLen,m_iMaxLen_VariLen);
				if(iSegmentLen>=m_iMinLen_VariLen){
					while(iSegmentLen>=m_iMinLen_VariLen){
						fMaxL=max(fMaxL,pppL_Eval[iTarget][iSegmentLen-m_iMinLen_VariLen][iEndRound_Segment-iSegmentLen+1]);
						iSegmentLen--;
					}
					if(fMaxL>=pAllL[iThr]){
						iCountDetect++;
						iEndRound_Segment++;
						iStartRound_Segment=iEndRound_Segment;
					}
					else iEndRound_Segment++;
				}
				else iEndRound_Segment++;
			}
		}
		TAR[iThr]=iCountDetect*1.0f/(iNumTarget_Eval*iNumRoundPerTarget_Eval);
	}

	for(iThr=0;iThr<iNumThreshold;iThr++){
		m_fThd=pAllL[iThr];
		int iCountDetect=0;
		for(iTarget=0;iTarget<iNumTarget_Idle;iTarget++){
			int iStartRound_Segment=0;int iEndRound_Segment=0;
			int iSegmentLen;
			while(iEndRound_Segment<iNumRoundPerTarget_Idle){
				iSegmentLen=iEndRound_Segment-iStartRound_Segment+1;
				double fMaxL=-FLT_MAX;
				iSegmentLen=min(iSegmentLen,m_iMaxLen_VariLen);
				if(iSegmentLen>=m_iMinLen_VariLen){
					while(iSegmentLen>=m_iMinLen_VariLen){
						fMaxL=max(fMaxL,pppL_Idle[iTarget][iSegmentLen-m_iMinLen_VariLen][iEndRound_Segment-iSegmentLen+1]);
						iSegmentLen--;
					}
					if(fMaxL>=pAllL[iThr]){
						iCountDetect++;
						iEndRound_Segment++;
						iStartRound_Segment=iEndRound_Segment;
					}
					else iEndRound_Segment++;
				}
				else iEndRound_Segment++;
			}
		}
		TRR[iThr]=1.0f/m_iMinLen_VariLen-(iCountDetect*1.0f/(iNumTarget_Idle*iNumRoundPerTarget_Idle));
	}


	for(iThr=0;iThr<iNumThreshold;iThr++) AbsD_TARwTRR[iThr]=fabs(TAR[iThr]-TRR[iThr]);
	int iIdxMinAbsD;
	CLALib::FindMaxMinInArray(AbsD_TARwTRR,iNumThreshold,false,&iIdxMinAbsD);

	printf("********************Variable length detection results*********************\n");
	printf("SN \t|\t Threshold \t|\t True Acceptance Rate \t|\t True Rejection Rate (1/round)\n");
	for(iThr=0;iThr<iNumThreshold;iThr++) printf("%d \t|\t %f \t|\t %f \t\t|\t %f (%f) \n",iThr,pAllL[iThr],TAR[iThr],TRR[iThr],1.0f/m_iMinLen_VariLen-TRR[iThr]);
	printf("**************************************************************************\n");
	printf("Finally the Equal Error Rate: \n");
	printf("Threshold: %f\n",pAllL[iIdxMinAbsD]);
	printf("True Accept Rate: %f\n",TAR[iIdxMinAbsD]);
	printf("True Rej Rate: %f\n",TRR[iIdxMinAbsD]);

	m_fEERDetectThr_VariLen=pAllL[iIdxMinAbsD];//***** Model Paras

	//----------Memory Clearance--------------
    for(iTarget=0;iTarget<iNumTarget_Eval;iTarget++){
		for(int iIdxWinWidth=0;iIdxWinWidth<iNumWinWidth;iIdxWinWidth++) delete pppL_Eval[iTarget][iIdxWinWidth];
		delete [] pppL_Eval[iTarget];
	}
	delete pppL_Eval;

    for(iTarget=0;iTarget<iNumTarget_Idle;iTarget++){
		for(int iIdxWinWidth=0;iIdxWinWidth<iNumWinWidth;iIdxWinWidth++) delete pppL_Idle[iTarget][iIdxWinWidth];
		delete [] pppL_Idle[iTarget];
	}
	delete pppL_Idle;

	delete pAllL;
	delete AbsD_TARwTRR;
	delete TAR;
	delete TRR;

	printf("Train Variable Length Detection Model accomplished.\n");
	return true;
}

int CP300SignalProc::CalcSVMScores_AllEpochs_in_EEGContFile(CEEGContFile* pEEGContFile,CSvmClassify* pSVM,double* &pTargetSVMScores,double* &pNonTargetSVMScores, int* &pTargetEpochCode, int* &pNonTargetEpochCode, int iNumEpochPerRound, int iNumRoundPerTarget, int& iNumTargetEpoch, int& iNumNonTargetEpoch)
{
	int** ppTargetEpoch=NULL;
	int** ppNonTargetEpoch=NULL;

	int** ppTargetEpochData; //All channels , Target Epochs
	int** ppNonTargetEpochData; //All channels , Non-Target Epochs

	//Separate Epochs
	//int iNumTargetEpoch,iNumNonTargetEpoch;
	iNumTargetEpoch=pEEGContFile->GetTargetEpochPtrs(iNumEpochPerRound,iNumRoundPerTarget,ppTargetEpoch,pTargetEpochCode, mSpellerChars,miCharTableSize);
	iNumNonTargetEpoch=pEEGContFile->GetNonTargetEpochPtrs(iNumEpochPerRound,iNumRoundPerTarget,ppNonTargetEpoch,pNonTargetEpochCode,mSpellerChars,miCharTableSize);
	
	int iStartSample,iNumSample,i;
	// iStartSample = 0; ccwang -->
	iStartSample = miP300RecordStartTime * miSamplingRate / 1000;
	iNumSample = miNumSampInRawEpochs;

	pEEGContFile->GetEpochData(iStartSample,iNumSample, ppTargetEpoch, iNumTargetEpoch, ppTargetEpochData);
	pEEGContFile->GetEpochData(iStartSample, iNumSample, ppNonTargetEpoch, iNumNonTargetEpoch, ppNonTargetEpochData);
	
	///^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

	///----------------------------Extract Features and Calculate SVM Scores-------------------
    pTargetSVMScores=new double[iNumTargetEpoch];
    pNonTargetSVMScores=new double[iNumNonTargetEpoch];
	double fPredict;

	for(i=0;i<iNumTargetEpoch;i++){
		int iFeatureSize=ExtractFeaturesFromEEGBuffer(ppTargetEpochData[i],mpSegment);
		pSVM->Classify(mpFeatures, iFeatureSize,pTargetSVMScores+i, &fPredict);
		pTargetSVMScores[i]=(pTargetSVMScores[i]+1)*100; ///Change Scale, see IsRejection()
		delete ppTargetEpochData[i];ppTargetEpochData[i]=NULL;
	}

	for(i=0;i<iNumNonTargetEpoch;i++){
		int iFeatureSize=ExtractFeaturesFromEEGBuffer(ppNonTargetEpochData[i],mpSegment);
		pSVM->Classify(mpFeatures,iFeatureSize,pNonTargetSVMScores+i,&fPredict);
		pNonTargetSVMScores[i]=(pNonTargetSVMScores[i]+1)*100; ///Change Scale, see IsRejection()
		delete ppNonTargetEpochData[i];ppNonTargetEpochData[i]=NULL;
	}

	///^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

	//Clear some allocated memory
	delete ppTargetEpoch;
	delete ppNonTargetEpoch;
	delete [] ppTargetEpochData;
	delete [] ppNonTargetEpochData;

	return 1;
}

int CP300SignalProc::CalcL_Len(double* pScoreBuffer, int iNumEpochPerRound, int iNumRound, double* pL, int iWinWidth)
{
	int iNumStep=iNumRound-iWinWidth+1;
	RESULT tResult;
	for(int iStep=0;iStep<iNumStep;iStep++){
		IsRejection_OneVariLen(pScoreBuffer+iStep*iNumEpochPerRound ,NULL, iNumEpochPerRound,iWinWidth, &tResult);
		pL[iStep]=tResult.fConfidence;
	}
	return 1;
}

int CP300SignalProc::ProcessEEGBuffer(float *eeg_buf, int nch, int nspl, RESULT *pResult)
{
	if (nch < miNumChannelUsed || nspl < miNumSampInRawEpochs) {
		printf("CP300SignalProc.ProcessEEGBuffer: input data error!");
	}

	float *fi = eeg_buf;
	for (int iCh = 0; iCh < miNumChannelUsed; iCh++) {
		for (int iSamp = 0; iSamp < miNumSampInRawEpochs; iSamp++) {
			mpSegment[iCh][iSamp] = fi[iSamp];
		}
		fi += nspl;
	}

	mFeatureSize = ExtractFeatures(mpSegment);

	double fMargin, fPredict;
	mpCSVMClassify->Classify(mpFeatures, mFeatureSize, &fMargin, &fPredict);

	if (pResult) pResult->fConfidence =(float) fMargin;
	if (m_hOutputResult) m_hOutputResult(fMargin);

	return 1;
}

void CP300SignalProc::SetProcFeedbackHandle(void *hf)
{
	m_hOutputResult = (int (__stdcall *)(double)) hf;
}