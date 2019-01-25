//================================================
//	MotorImagery.cpp
//	Purpose: Class for motor imagery signal processing, used together
//			 with matlab training tools
//
//  Created by: Cuntai, 30 Mar 2004
//
//	Usage:
//		CMotorImagery * pCMotorImagery = new CMotorImagery()
//		pCMotorImagery->Initialize();	//use default resource file defined in NeuroComm_public.h
//		
//		pCMotorImagery->Process(char *fEEGBuffer);
//=================================================

#include "MotorImagery.h"
#include <math.h>

const double InvLog =(1/log10(2.0));

CMotorImagery::CMotorImagery()
{
	// ccwang: initail values
	mpResources = NULL;
	mpCFiltering = NULL;
	mpPCATransform = NULL;
	mpCSPTransform = NULL;
	mszChannelDef = NULL;
	miChannelAtr = NULL;
	mpCSPFeatures = NULL;
	mpOLSFeatures = NULL;
	mpCSvmClassify = NULL;
	mpCFeatureSelect = NULL;
	mpSampUsedIndex = NULL;
	mfXMuWeight = NULL;
	mfXBetaWeight = NULL;
	mfYMuWeightRS = NULL;
	mfYBetaWeightRS = NULL;
	mfYMuWeightLS = NULL;
	mfYBetaWeightLS = NULL;
}

CMotorImagery::~CMotorImagery()
{
	if(mpResources)
		delete mpResources;
	if(mpCFiltering)
		delete mpCFiltering;
	if(mpPCATransform)
		delete mpPCATransform;
	if(mpCSPTransform)
		delete mpCSPTransform;
	if(mszChannelDef){
		for(int i=1;i<miNumTotalChannel;i++)
			free(mszChannelDef[i]);
		free(mszChannelDef);
	}
	if (miChannelAtr) {
		free(miChannelAtr);
	}

	if(mpCSPFeatures)
		delete [] mpCSPFeatures;
	if(mpOLSFeatures)
		delete [] mpOLSFeatures;

	if(mpCSvmClassify)
		delete mpCSvmClassify;

	if(mpCFeatureSelect)
		delete mpCFeatureSelect;

	if(mpSampUsedIndex)
		delete [] mpSampUsedIndex;

	if(mfXMuWeight)
		delete [] mfXMuWeight;
	if(mfXBetaWeight)
		delete [] mfXBetaWeight;
	if(mfYMuWeightRS)
		delete [] mfYMuWeightRS;
	if(mfYBetaWeightRS)
		delete [] mfYBetaWeightRS;
	if(mfYMuWeightLS)
		delete [] mfYMuWeightLS;
	if(mfYBetaWeightLS)
		delete [] mfYBetaWeightLS;
}

bool CMotorImagery::Initialize()
{
	Reset();

	mpResources = new Resources();
	if (mpResources->Merge ((char *)CONFIG_FILE_NAME) != errOkay){
		printf ("CMotorImagery.Initialize: Can not open configuration file %s.\n", CONFIG_FILE_NAME);
		return false;
	}

	char szTask[kMaxLine];
	if (mpResources->Get ("System", "Task", szTask, sizeof(szTask))){
		printf ("System:Task undefined in %s\n", (char *)CONFIG_FILE_NAME);
			return false;
	}
	if (strcmp (szTask, "MotorImagery") != 0){
		printf ("System:Task must be defined as MotorImagery\n");
		return false;
	}

	// ccwang: move here from FeedBack()
	mpResources->Get("CursorControlPara","XMoveGain",&mfXMovGain);
	mpResources->Get("CursorControlPara","YMoveGain",&mfYMovGain);
	// ccwang: ended here

	char szFilename[kMaxLine];
	if(mpResources->Get("Process", "ParameterFile",szFilename, sizeof(szFilename)) != errOkay){
		printf ("Parameter file must be given in configuration file %s under Process:ParameterFile.\n",(char *)CONFIG_FILE_NAME);
		return false;
	}

	m_pLPC =  new LPCCEP();

	bool status =  LoadParameters(szFilename);
	status = status && InitSVM();
	status = status && InitBuffers();

	return status;
}

void CMotorImagery::Reset()
{
	mpResources = NULL;
	mpCFiltering = NULL;
	mpPCATransform = NULL;
	mpCSPTransform = NULL;
	mszChannelDef = NULL;
	miChannelAtr = NULL;
	mpCFeatureSelect = NULL;
	mfCalibPowNorm = 0;
	mfCalibLPCNorm = 0;
}


bool CMotorImagery::LoadParameters(char *szFilename)
{
	char szString[kMaxLine], szStr[kMaxLine];
	FILE *fp;

	if (fopen_s(&fp, szFilename, "r")) {
		printf("Cannot open parameter file: %s.\n", szFilename);
		return false;
	}

	if(fgets(szString,sizeof(szString) ,fp) != NULL)
		sscanf_s(szString,"%s %d",szStr, kMaxLine - 1,&miSamplingRate);	//Sampling_Rate
	if(fgets(szString,sizeof(szString) ,fp) != NULL)
		sscanf_s(szString,"%s %g",szStr, kMaxLine - 1,&mfResolution);	//EEG_Resolution
	if(fgets(szString,sizeof(szString) ,fp) != NULL)
		sscanf_s(szString,"%s %f",szStr, kMaxLine - 1,&mfRejectThreshold);	//Reject_Threshold
	if(fgets(szString,sizeof(szString) ,fp) != NULL)
		sscanf_s(szString,"%s %d",szStr, kMaxLine - 1,&miProcStartSample);	//Processing_Start_Sample in samples
	if(fgets(szString,sizeof(szString) ,fp) != NULL)
		sscanf_s(szString,"%s %d",szStr, kMaxLine - 1,&miProcEndSample);	//Processing_End_Sample in samples
	if(fgets(szString,sizeof(szString) ,fp) != NULL)
		sscanf_s(szString,"%s %d",szStr, kMaxLine - 1,&miNumSampInRawEpochs);	//Number_Samples_in_Raw_Epochs in samples

	if(fgets(szString,sizeof(szString) ,fp) != NULL)
		sscanf_s(szString,"%s %d",szStr, kMaxLine - 1,&miWinSize);	//Window_Width_Sample in samples
	if(fgets(szString,sizeof(szString) ,fp) != NULL)
		sscanf_s(szString,"%s %d",szStr, kMaxLine - 1,&miWinShift);	//Window_Shift_Sample in samples
	if(fgets(szString,sizeof(szString) ,fp) != NULL)
		sscanf_s(szString,"%s %d",szStr, kMaxLine - 1,&miTotalNumFrame);	//Total_Frame_Per_Epoch
	if(fgets(szString,sizeof(szString) ,fp) != NULL)
		sscanf_s(szString,"%s %d",szStr, kMaxLine - 1,&miCSPDim);	//CSP_Dimension "raw" feature from CSP feature extraction
	if(fgets(szString,sizeof(szString) ,fp) != NULL)
		sscanf_s(szString,"%s %d",szStr, kMaxLine - 1,&miUseOLS);	//Use_OLS_Selection: 1 use OLS, 0 not use
	if(fgets(szString,sizeof(szString) ,fp) != NULL)
		sscanf_s(szString,"%s %d",szStr, kMaxLine - 1,&miOLSDim);	//OLS_Feature_Dimension
	if (miOLSDim > 0){
		if( LoadFeatureSelect(fp) == false)	//Linear or RBF paprameters
			return false;
	}

	if(fgets(szString,sizeof(szString) ,fp) != NULL){
		sscanf_s(szString,"%s %d",szStr, kMaxLine - 1,&miNumTotalChannel);	//Channel_Order
		mszChannelDef = (char **) malloc(miNumTotalChannel * sizeof(char *));
		miChannelAtr = (int *) malloc(miNumTotalChannel * sizeof(int));
		fgets(szString,sizeof(szString) ,fp);
		char *context, *szToken = strtok_s(szString, SEPS, &context);
		for (int i=0;i<miNumTotalChannel;i++){
			mszChannelDef[i] = _strdup(szToken);
			szToken = strtok_s(NULL, SEPS, &context);

			if(strstr(mszChannelDef[i],"1") || strstr(mszChannelDef[i],"3") ||
			   strstr(mszChannelDef[i],"5") || strstr(mszChannelDef[i],"7") || strstr(mszChannelDef[i],"9"))
				miChannelAtr[i] = kLEFTCHANNEL;
			else
				miChannelAtr[i] = kRIGHTCHANNEL;
		}
	}

	InitChnlSampIndex(fp);	//Channel_Used & Sample_Index_in_Epochs
	InitFilterTransform(fp); //Filter_Parameter_A, Filter_Parameter_B, PCA_Transform_Matrix

	InitChannelWeight(fp);

	fclose(fp);

	return true;
}


bool CMotorImagery::LoadFeatureSelect(FILE *fp)
{
	float fVal;
	char szString[kMaxLine], szStr[kMaxLine];
	mpCFeatureSelect = new CFeatureSelect(miOLSDim);
	for (int iOLS=0;iOLS<miOLSDim;iOLS++){    
		if(fgets(szString,sizeof(szString) ,fp) != NULL)
			sscanf_s(szString,"%s ",szStr, kMaxLine - 1);	//Regressor type (Linear or RBF)

		char *context, *szToken = strtok_s(szString, SEPS, &context);
		if (_stricmp(szStr,"Linear:")==0){	//Linear regressor
			mpCFeatureSelect->SetRegressorDim(iOLS,1);
			mpCFeatureSelect->SetRegressorType(iOLS, LINEAR_REGRESSOR);
			//sscanf(szString,"%g, ",&fVal);
			fVal = (float)atof( strtok_s(NULL, SEPS, &context));
			mpCFeatureSelect->SetRegressorCenter(iOLS,1,fVal);
		}
		else {
			if (_stricmp(szStr,"RBF:")==0){	//RBF regressor
				mpCFeatureSelect->SetRegressorDim(iOLS,miCSPDim);
				mpCFeatureSelect->SetRegressorType(iOLS, RBF_REGRESSOR);
				for(int iCSP=0;iCSP<miCSPDim;iCSP++){
					//sscanf(szString,"%g, ",&fVal);
					fVal = (float)atof( strtok_s(NULL, SEPS, &context));
					mpCFeatureSelect->SetRegressorCenter(iOLS,iCSP,fVal);
				}
				fVal = (float)atof(strtok_s(NULL, SEPS, &context));
				//sscanf(szString,"%g ",&fVal);
				mpCFeatureSelect->SetRegressorSigma(iOLS,fVal);
			}
			else{
				printf("\nUnidentified type %s\n",szStr);
				return false;
			}
		}
	}
	return true;
}

int CMotorImagery::GetChannel(char *szChannelName)
{
	for (int i=0;i<miNumTotalChannel;i++){
		if(strcmp(mszChannelDef[i],szChannelName) == 0)
			return i;
	}

	return -1;
}

bool CMotorImagery::InitChnlSampIndex(FILE * fp)
{
	int iStartSample, iEndSample;

	mpChannelUsedIndex = new int [miNumTotalChannel];
	mpChannelUsed = new int [miNumTotalChannel];
	memset(mpChannelUsedIndex,0,miNumTotalChannel*sizeof(int));

	char szString[kMaxLine], szStr[kMaxLine];
	int iVal;
	if(fgets(szString,sizeof(szString) ,fp) != NULL){
		sscanf_s(szString,"%s %d",szStr, kMaxLine - 1,&miNumChannelUsed);	//Channel_Used 
		fgets(szString,sizeof(szString) ,fp);
		char *context, *szToken = strtok_s(szString, SEPS, &context);
		for (int i=0;i<miNumChannelUsed;i++){
//			iVal = atoi(szToken);
			iVal = GetChannel(szToken);
			mpChannelUsedIndex[iVal] = 1;		//Flag: if a channel is used, flag = 1, otherwise = 0
			szToken = strtok_s(NULL, SEPS, &context);
			mpChannelUsed[i] = iVal;
		}
	}

	mpSampUsedIndex = new int [miNumSampInRawEpochs];
	memset(mpSampUsedIndex,0,miNumSampInRawEpochs*sizeof(int));
	if(fgets(szString,sizeof(szString) ,fp) != NULL){
		sscanf_s(szString,"%s %d",szStr, kMaxLine - 1,&miNumSampleUsed);	//Sample_Index_in_Epochs
		fgets(szString,sizeof(szString) ,fp);
		sscanf_s(szString,"%d, %d,", &iStartSample, &iEndSample);

		if (iStartSample < 0) iStartSample = 0;
		if (iEndSample >= miNumSampleUsed) iEndSample = miNumSampleUsed - 1;

		for (int i = iStartSample; i <= iEndSample; i++){
			mpSampUsedIndex[i] = 1;		//Flag: if a sample is used, flag = 1, otherwise = 0
		}
	}

	return true;
}

bool CMotorImagery::InitChannelWeight(FILE *fp)
{
	mfXMuWeight = new float [miNumChannelUsed+1];
	mfXBetaWeight = new float [miNumChannelUsed+1];
	mfYMuWeightRS = new float [miNumChannelUsed+1];
	mfYBetaWeightRS = new float [miNumChannelUsed+1];
	mfYMuWeightLS = new float [miNumChannelUsed+1];
	mfYBetaWeightLS = new float [miNumChannelUsed+1];

	char szString[kMaxLine], szStr[kMaxLine];
	int iVal;
	if(fgets(szString,sizeof(szString) ,fp) != NULL){
		sscanf_s(szString,"%s %d",szStr, kMaxLine - 1,&iVal);	//X_Mu_Power_Weight 
		fgets(szString,sizeof(szString) ,fp);
		char *context, *szToken = strtok_s( szString, SEPS, &context);
		for (int i=0;i<miNumChannelUsed;i++){
			float fVal = (float) atof(szToken);
			szToken = strtok_s( NULL, SEPS, &context);
			mfXMuWeight[i] = fVal;
		}
	}

	if(fgets(szString,sizeof(szString) ,fp) != NULL){
		sscanf_s(szString,"%s %d",szStr, kMaxLine - 1,&iVal);	//X_Beta_Power_Weight 
		fgets(szString,sizeof(szString) ,fp);
		char *context, *szToken = strtok_s( szString, SEPS, &context);
		for (int i=0;i<miNumChannelUsed;i++){
			float fVal = (float) atof(szToken);
			szToken = strtok_s( NULL, SEPS, &context);
			mfXBetaWeight[i] = fVal;
		}
	}
	if(fgets(szString,sizeof(szString) ,fp) != NULL){
		sscanf_s(szString,"%s %d",szStr, kMaxLine - 1,&iVal);	//Y_Mu_Power_Weight_RS 
		fgets(szString,sizeof(szString) ,fp);
		char *context, *szToken = strtok_s( szString, SEPS, &context);
		for (int i=0;i<miNumChannelUsed;i++){
			float fVal = (float) atof(szToken);
			szToken = strtok_s( NULL, SEPS, &context);
			mfYMuWeightRS[i] = fVal;
		}
	}

	if(fgets(szString,sizeof(szString) ,fp) != NULL){
		sscanf_s(szString,"%s %d",szStr, kMaxLine - 1,&iVal);	//Y_Beta_Power_Weight_RS 
		fgets(szString,sizeof(szString) ,fp);
		char *context, *szToken = strtok_s( szString, SEPS, &context);
		for (int i=0;i<miNumChannelUsed;i++){
			float fVal = (float) atof(szToken);
			szToken = strtok_s( NULL, SEPS, &context);
			mfYBetaWeightRS[i] = fVal;
		}
	}

	if(fgets(szString,sizeof(szString) ,fp) != NULL){
		sscanf_s(szString,"%s %d",szStr, kMaxLine - 1,&iVal);	//Y_Mu_Power_Weight_LS 
		fgets(szString,sizeof(szString) ,fp);
		char *context, *szToken = strtok_s( szString, SEPS, &context);
		for (int i=0;i<miNumChannelUsed;i++){
			float fVal = (float) atof(szToken);
			szToken = strtok_s(NULL, SEPS, &context);
			mfYMuWeightLS[i] = fVal;
		}
	}

	if(fgets(szString,sizeof(szString) ,fp) != NULL){
		sscanf_s(szString,"%s %d",szStr, kMaxLine - 1,&iVal);	//Y_Beta_Power_Weight_LS 
		fgets(szString,sizeof(szString) ,fp);
		char *context, *szToken = strtok_s(szString, SEPS, &context);
		for (int i=0;i<miNumChannelUsed;i++){
			float fVal = (float) atof(szToken);
			szToken = strtok_s(NULL, SEPS, &context);
			mfYBetaWeightLS[i] = fVal;
		}
	}

	if(fgets(szString,sizeof(szString) ,fp) != NULL){
		sscanf_s(szString,"%s %f",szStr, kMaxLine - 1,&mfXIntercept);	//X_Axis_Intercept 
	}
	if(fgets(szString,sizeof(szString) ,fp) != NULL){
		sscanf_s(szString,"%s %f",szStr, kMaxLine - 1,&mfYInterceptRS);	//Y_Axis_InterceptRS for right screen 
	}
	if(fgets(szString,sizeof(szString) ,fp) != NULL){
		sscanf_s(szString,"%s %f",szStr, kMaxLine - 1,&mfYInterceptLS);	//Y_Axis_InterceptLS for left screen 
	}

	return true;
}

bool CMotorImagery::InitFilterTransform(FILE *fp)
{
	mpCFiltering = new CFiltering();
	char szString[kMaxLine], szStr[kMaxLine];
	double fVal;
	if(fgets(szString,sizeof(szString) ,fp) != NULL){
		sscanf_s(szString,"%s %d",szStr, kMaxLine - 1,&miFilterOrderOfB);  //Filter_Parameter_B
		mpCFiltering->InitB(miFilterOrderOfB);

		fgets(szString,sizeof(szString) ,fp);
		char *context, *szToken = strtok_s( szString, SEPS, &context);
		for(int i=0;i<miFilterOrderOfB;i++){
			fVal = atof(szToken);
			mpCFiltering->SetBPara(fVal, i);
			szToken = strtok_s( NULL, SEPS, &context);
		}

	}
	if(fgets(szString,sizeof(szString) ,fp) != NULL){
		sscanf_s(szString,"%s %d",szStr, kMaxLine - 1,&miFilterOrderOfA); //Filter_Parameter_A
		mpCFiltering->InitA(miFilterOrderOfA);

		fgets(szString,sizeof(szString) ,fp);
		char *context, *szToken = strtok_s( szString, SEPS, &context);
		for(int i=0;i<miFilterOrderOfA;i++){
			fVal = atof(szToken);
			mpCFiltering->SetAPara(fVal,i);
			szToken = strtok_s( NULL, SEPS, &context);
		}
	}

	mpPCATransform = new CTransformat();
	if(fgets(szString,sizeof(szString) ,fp) != NULL){
		sscanf_s(szString,"%s %d, %d",szStr, kMaxLine - 1,&miNumPCAMatrixRow, &miNumPCAMatrixCol);	//PCA_Transform_Matrix
		mpPCATransform->LoadMatrix(miNumPCAMatrixRow, miNumPCAMatrixCol, fp);
	}

	mpCSPTransform = new CTransformat();
	if(fgets(szString,sizeof(szString) ,fp) != NULL){
		sscanf_s(szString,"%s %d, %d",szStr, kMaxLine - 1,&miNumCSPTransRow, &miNumCSPTransCol);	//CSP_Transform_Matrix
		mpCSPTransform->LoadMatrix(miNumCSPTransRow, miNumCSPTransCol, fp);
	}

	return true;
}


bool CMotorImagery::InitBuffers()
{
	mpSegment = new float * [miNumTotalChannel];
	for(int i=0;i<miNumTotalChannel;i++)
		mpSegment[i] = new float [miNumSampInRawEpochs];

	mpCSPFeatures = new double [miCSPDim];
	mpOLSFeatures = new double [miOLSDim];

	return true;
}

bool CMotorImagery::InitSVM()
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

	mpCSvmClassify = new CSvmClassify();

	return mpCSvmClassify->Initialize(szModelFile,szRangeFile);
}


double CMotorImagery::ProcessEEGBuffer(char *fInEEGBuffer)
{
	CopyRawBuffer((int *)fInEEGBuffer);	//eg, eeg between 0-500, all channel
	return ProcessFullCh(mpSegment);
}

double CMotorImagery::ProcessFullCh(float **pInSegment)
{
	SelectSegment(pInSegment, miNumTotalChannel);		//Select samples,    leave miNumSampleUsed samples, eg, cut from 0-500 to 250-450

	for(int iCh=0;iCh<miNumTotalChannel;iCh++)	//filtering
		mpCFiltering->Process(pInSegment[iCh],miNumSampleUsed);	

	int iLeftEOGChnl = GetChannel("Fp1");	//Remove EOG
	if (iLeftEOGChnl >= 0)
		RemoveEOG(pInSegment,iLeftEOGChnl, miNumSampleUsed);

	int iRightEOGChnl = GetChannel("Fp2");
	if (iRightEOGChnl >=0)
		RemoveEOG(pInSegment,iRightEOGChnl, miNumSampleUsed);


	SelectChannels(pInSegment, miNumSampleUsed);	//Select channels

	return Process(pInSegment);
}


//===============================================
//Processing slected channels
//Input: pInSegment -- Channel X Sample
//===============================================
double CMotorImagery::Process(float **pInSegment)
{
	//Apply PCA to Channel
	mpPCATransform -> ProcessMat(pInSegment, miNumSampleUsed);	 //result in: miNumPCAMatrixRow X miNumSampleUsed	

	//Apply CSP to channel, so it can be done for whole segment
	mpCSPTransform -> ProcessMat(pInSegment,miNumSampleUsed);	//result in: miNumCSPTransRow X miNumSampleUsed

	double fScore =0;
	int iOffSet=0;
	for (int iFrame=0;iFrame<miTotalNumFrame;iFrame++){

		double * pFeatures = mpCSPFeatures;
		int iFeatureSize = CreateCSPFeatures(pInSegment, miWinSize, iOffSet);
		if (miUseOLS == 1){
			pFeatures = mpOLSFeatures;
			iFeatureSize = miOLSDim;
			for (int iOLS=0;iOLS<miOLSDim;iOLS++)
				mpOLSFeatures[iOLS] = mpCFeatureSelect->GetResponse(mpCSPFeatures,iOLS);
		}

		double fMargin, fPredict;
		mpCSvmClassify->Classify(pFeatures, iFeatureSize, &fMargin, &fPredict);
	
		fScore += fMargin;
		iOffSet += miWinShift;
	}

	return fScore/miTotalNumFrame;
}

void CMotorImagery::SelectChannels(float **pInSegment, int iNumSampleUsed)
{
	int iChId = 0; 
	for (int iCh = 0; iCh < miNumTotalChannel; iCh++) {
		if (mpChannelUsedIndex[iCh] == 1 && iCh != iChId) {
			for(int iSamp=0;iSamp<iNumSampleUsed;iSamp++)
				pInSegment[iChId][iSamp]  = pInSegment[iCh][iSamp];
			iChId++;
		}
	}
}


void CMotorImagery::SelectSegment(float **pInSegment, int iNumChannel)
{
	int iSampId = 0;
	for(int iSamp=0;iSamp<miNumSampInRawEpochs;iSamp++){
		if(mpSampUsedIndex[iSamp] == 0)	//skip non-relavant samples
			continue;

		for(int iCh = 0; iCh<iNumChannel;iCh++){
			pInSegment [iCh][iSampId]  = pInSegment[iCh][iSamp];
		}
		iSampId ++;
	}
}


void CMotorImagery::RemoveEOG(float **pInSegment, int iRefChannel, int iNumSampleUsed)
{
	float fMean = 0, fMax= -1000, fMin = 1000, fNorm = 0;
	for(int iSamp=0;iSamp<iNumSampleUsed;iSamp++){
		float fVal = pInSegment [iRefChannel][iSamp];
		if (fVal > fMax) fMax = fVal;
		if (fVal < fMin) fMin = fVal;
		fMean += fVal;
		fNorm += fVal * fVal;
	}
	fMean /= iNumSampleUsed;
    fMax = fMax - fMean;
    fMin = fMin - fMean;

	if((fMax > mfRejectThreshold) || (fMin < -mfRejectThreshold)){
		for(int iCh = 0; iCh<miNumTotalChannel;iCh++){
			if(iCh == iRefChannel)
				continue;

			float alpha = 0;
			for(int iSamp=0;iSamp<iNumSampleUsed;iSamp++)
				alpha += pInSegment [iCh][iSamp] * pInSegment[iRefChannel][iSamp];
			alpha /= fNorm;

			for(int iSamp=0;iSamp<iNumSampleUsed;iSamp++)
				pInSegment [iCh][iSamp] -= alpha * pInSegment[iRefChannel][iSamp];
		}
	}

}


int CMotorImagery::CreateCSPFeatures(float **pInSegment, int iWinSize, int iOffSet)
{
	float fNorm =0;
	for(int iFeat =0;iFeat<miNumCSPTransRow;iFeat++){
		float fVal=0;
		float *pSegment = &pInSegment[iFeat][iOffSet];
		for(int iSamp = 0;iSamp< iWinSize; iSamp++){
			fVal += (*pSegment)*(*pSegment);
			pSegment++;
		}
		mpCSPFeatures[iFeat] = fVal;
		fNorm += fVal;
	}

	for(int iFeat =0;iFeat<miNumCSPTransRow;iFeat++){
		mpCSPFeatures[iFeat] = log(mpCSPFeatures[iFeat]/fNorm);
	}

	return miNumCSPTransRow;
}


void CMotorImagery::GetResult(double Score, RESULT *pResult)
{
	pResult->fConfidence = (float) Score;
	pResult->iReject = 0;
	pResult->iResult = (Score > 0);
	pResult->szResult = pResult->iResult + '0';

	return;
}

void CMotorImagery::SubtractReference(float *pfDataBuffer, int iDataLength,float *pfRefData)
{
	float *pSource = pfDataBuffer;
	float *pReference = pfRefData;
	for (int i=0;i<iDataLength;i++){
		*pSource = (*pSource) - (*pReference);
		pSource ++;
		pReference ++;
	}
}

//===============================================
// Class for CSP feature selection: Linear or RBF
//===============================================
CFeatureSelect::CFeatureSelect(int iNumRegressor)
{
	mpSFeatureRegressor = new Regressor[iNumRegressor];
	memset(mpSFeatureRegressor,0,iNumRegressor*sizeof(Regressor));
}


CFeatureSelect::~CFeatureSelect()
{
	// ccwang
	// need to clear mpSFeatureRegressor[i].fCenter for RBF_REGRESSOR?

	delete [] mpSFeatureRegressor;
}

void CFeatureSelect::SetRegressorType(int iRegressorIndex, int iType)
{
	mpSFeatureRegressor[iRegressorIndex].iType = iType;
}

void CFeatureSelect::SetRegressorDim(int iRegressorIndex, int iDim)
{
	mpSFeatureRegressor[iRegressorIndex].iNumDim = iDim;
}

void CFeatureSelect::SetRegressorCenter(int iRegressorIndex, int iDim, float fCenter)
{
	int iType = mpSFeatureRegressor[iRegressorIndex].iType;
	if (iType == LINEAR_REGRESSOR)
		mpSFeatureRegressor[iRegressorIndex].iSelect = (int) fCenter;
	else {
		if (iType == RBF_REGRESSOR){
			if (mpSFeatureRegressor[iRegressorIndex].fCenter == NULL)
				mpSFeatureRegressor[iRegressorIndex].fCenter = new float [mpSFeatureRegressor[iRegressorIndex].iNumDim];

			mpSFeatureRegressor[iRegressorIndex].fCenter[iDim] = fCenter;
		}
	}
}

void CFeatureSelect::SetRegressorSigma(int iRegressorIndex, float fSigma)
{
	mpSFeatureRegressor[iRegressorIndex].fSigma = fSigma;
}

double CFeatureSelect::GetResponse(double *fInFeat, int iRegressorIndex)
{
	int iType = mpSFeatureRegressor[iRegressorIndex].iType;
	if (iType == LINEAR_REGRESSOR){
		return fInFeat[mpSFeatureRegressor[iRegressorIndex].iSelect];
	}
	else if (iType == RBF_REGRESSOR){
		int iNumDim = mpSFeatureRegressor[iRegressorIndex].iNumDim;
		float *pfCenter = mpSFeatureRegressor[iRegressorIndex].fCenter;
		float fSigma = 2 * mpSFeatureRegressor[iRegressorIndex].fSigma;
		double fVal=0;
		double *pfInFeat = fInFeat;
		for (int i=0;i<iNumDim;i++){
			double fDiff = (*pfInFeat++ - *pfCenter++);
			fVal += fDiff * fDiff;
		}
		fVal /= fSigma;

		return (float)exp(-fVal);
	}
	else {
		return (float) _MIN_FLOAT_;
	}
}
