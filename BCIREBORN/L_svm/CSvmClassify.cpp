#include "CSvmClassify.h"
#include <time.h>
#include <sys/timeb.h>
#include "../L_Utilities/Utilities.h"

#define SKIP_TARGET\
	while(isspace(*p)) ++p;\
	while(!isspace(*p)) ++p;

#define SKIP_ELEMENT\
	while(*p!=':') ++p;\
	++p;\
	while(isspace(*p)) ++p;\
	while(*p && !isspace(*p)) ++p;

/*-----------------------------------
 IMPORTANT: modified Jan 19, 2004
 In order to return margin, use this functionfor two class only!
 If the above condition dose not meet, margin is meaningless!!!
-----------------------------------*/

//==========================================
//==========================================
CSvmClassify::CSvmClassify()
{
	Reset();
}
//==========================================
//==========================================
CSvmClassify::~CSvmClassify()
{
	if(mSVMmodels)
		svm_destroy_model(mSVMmodels);
	if(mSVMnodes)
		free(mSVMnodes);
	if(mszLine)
		free(mszLine);
	if(mFeatureMax)
		free(mFeatureMax);
	if(mFeatureMin)
		free(mFeatureMin);

}


//==========================================
//==========================================
void CSvmClassify::Reset()
{
	mSVMmodels = NULL;
	mSVMnodes = NULL;
	mfSVMProcTime = 0;

	miMaxNoAttr = 256;
	miMaxLine = 4096;

	mfLower	= -1.0;
	mfUpper	= 1.0;
	mFeatureMax = NULL; 
	mFeatureMin = NULL; 

}

//==========================================
//==========================================
bool CSvmClassify::Initialize(char *szModelFile, char *szRangeFile)
{
	Reset();

	mszLine = (char *) malloc(miMaxLine *sizeof(char));
	mSVMnodes = (struct CSvmNode *) malloc(miMaxNoAttr*sizeof(struct CSvmNode));

	mFeatureMax = (double *) malloc(miMaxNoAttr*sizeof(double));
	mFeatureMin = (double *) malloc(miMaxNoAttr*sizeof(double));

	if(LoadModels(szModelFile) == false){
		printf("\nLoading model %s fails! You need to stop the program!\n", szModelFile);
		return false;
	}

	if(LoadRange(szRangeFile) == false){
		printf("\nLoading feature range %s fails! You need to stop the program!\n", szRangeFile);
		return false;
	}

	return true;
}

//==========================================
// Load SVM models
//==========================================
bool  CSvmClassify::LoadModels(char *szModelFile)
{
	if((mSVMmodels=SvmLoadModel(szModelFile))==0)
	{
		fprintf(stderr,"can't open model file %s\n",szModelFile);
		return false;
	}

	return true;
}

//==========================================
// Load SVM range
//==========================================
bool  CSvmClassify::LoadRange(char *szRangeFile)
{
	FILE *fp = NULL;

	fopen_s(&fp, szRangeFile, "r");
	if (fp == NULL){
		fprintf(stderr,"can't open file %s\n", szRangeFile);
		return false;
	}
	int idx;
	double fmin, fmax;
	while (fscanf_s(fp, "%d %lf %lf\n", &idx, &fmin, &fmax) == 3){
		if(idx >= miMaxNoAttr){
			miMaxNoAttr *= 2;
			mFeatureMax = (double *) realloc(mFeatureMax, miMaxNoAttr*sizeof(double));
			mFeatureMin = (double *) realloc(mFeatureMin, miMaxNoAttr*sizeof(double));
			mSVMnodes = (struct CSvmNode *) realloc(mSVMnodes,miMaxNoAttr*sizeof(struct CSvmNode));
		}
		mFeatureMin[idx-1] = fmin;
		mFeatureMax[idx-1] = fmax;
	}
	fclose(fp);

	return true;
}

//==========================================
// Input:
//   fFeature: a feature buffer
//   iNumDim: feature dimension
// Output:
//   fMargin: margin of svm classifier
//   fPredict: predicted result, MeanSqrErr can be calculated on this
//==========================================
void CSvmClassify::Classify(double *fFeature, int iNumDim, double *fMargin, double *fPredict)
{
	int i,j;
	for (i=0, j=1; i < iNumDim; i++, j++) {
		mSVMnodes[i].index = j;
		mSVMnodes[i].value = ScaleFeature(i,fFeature[i]);
	}
	mSVMnodes[i].index = -1;

	*fPredict = SvmPredict(mSVMmodels,mSVMnodes,fMargin);
}

double CSvmClassify::ScaleFeature(int index, double fInValue)
{
	double value = fInValue;
	/* skip single-valued attribute */
	if(mFeatureMax[index] == mFeatureMin[index])
		return mFeatureMax[index];

	value = mfLower + (mfUpper-mfLower) * 
			(value-mFeatureMin[index])/
			(mFeatureMax[index]-mFeatureMin[index]);
	if (value < mfLower) 
		value = mfLower;

	if (value > mfUpper) 
		value = mfUpper;

	return value;
}



//==========================================
//==========================================
char* CSvmClassify::ReadOneLine(FILE *input)
{
	int len;
	
	if(fgets(mszLine,miMaxLine ,input) == NULL)
		return NULL;

	while(strrchr(mszLine,'\n') == NULL)
	{
		miMaxLine  *= 2;
		mszLine = (char *) realloc(mszLine, miMaxLine );
		len = (int) strlen(mszLine);
		if(fgets(mszLine+len,miMaxLine -len,input) == NULL)
			break;
	}
	return mszLine;
}


//==========================================
// Classify for one file in ASCII format
//==========================================
void  CSvmClassify::ProcessFile(char *szTestFile,char * szModelFile, char * szOutputFile)
{
	FILE *input, *output;
	fopen_s(&input, szTestFile, "r");
	if(input == NULL){
		fprintf(stderr,"can't open input file %s\n",szTestFile);
		return;
	}

	fopen_s(&output, szOutputFile, "w");
	if(output == NULL){
		fprintf(stderr,"can't open output file %s\n",szOutputFile);
		return;
	}

	LoadModels(szModelFile);
	ProcessFileData(input, output);

	fclose(input);
	fclose(output);
}

//==========================================
// Classify features for opened file
//==========================================
void  CSvmClassify::ProcessFileData(FILE *input, FILE *output)
{
	double Accuracy, MeanSqrErr, SqrCorrCoe;
	int iNumTest;
	ClassifyFileFull(input,output, &Accuracy, &MeanSqrErr, &SqrCorrCoe, &iNumTest);
	for(int i=0;i<iNumTest;i++)
		fprintf(output,"%g\n",mfMargins[i]);

}

//==========================================
// If index is fully used
//==========================================
void CSvmClassify::ClassifyFileFull(FILE *input, FILE *output, double *Accuracy, double *MeanSqrErr, double *SqrCorrCoe, int *iNumTest)
{
	Timer timer;

	int correct = 0;
	int total = 0;
	double error = 0;
	double *fFeature = (double *) malloc(miMaxNoAttr *sizeof(double));

	while(ReadOneLine(input)!=NULL)
	{
		int i = 0;
		double target,v;
		int index;
		double value;
		const char *p = mszLine;

		if (sscanf_s(p, "%lf", &target)!=1) break;
		SKIP_TARGET
		while(sscanf_s(p, "%d:%lf", &index,&value)==2)
		{
			fFeature[i] = value;

			SKIP_ELEMENT;
			++i;
			if(i>=miMaxNoAttr-1)	// need one more for index = -1
			{
				miMaxNoAttr *= 2;
				fFeature = (double *) realloc(fFeature, miMaxNoAttr *sizeof(double));
				mSVMnodes = (struct CSvmNode *) realloc(mSVMnodes,miMaxNoAttr*sizeof(struct CSvmNode));
			}
		}

		timer.Start();
		mfMargins[total] = 0;

		Classify(fFeature,i, &mfMargins[total], &v);

		timer.Finish();
		mfSVMProcTime += timer.GetElapsedTime();
		printf("\nTimer = %g sec", mfSVMProcTime);

		if(v == target)
			++correct;
		error += (v-target)*(v-target);
		++total;

		fprintf(output,"%g\n",v);
	}
	*iNumTest = total;
	free(fFeature);
}


