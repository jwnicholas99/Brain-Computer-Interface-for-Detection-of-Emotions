#ifndef _CSVMTRAIN_H
#define _CSVMTRAIN_H

class CSvmTrain{
public:
	CSvmTrain();
	~CSvmTrain();
private:
    struct CSvmParameter m_param;		// set by setparameters
	struct CSvmProblem m_prob;		// set by addnewsample
	struct CSvmModel* m_model;
	int m_iMaxNumSample;
	int m_iFeatureDim;
	double* m_pFeatureMax;
	double* m_pFeatureMin;

public:
	int AddNewSample(double* pFeature, int iLabel);
	void Initialize(CSvmParameter* Para, int iMaxNumSample, int iFeatureDim);
	bool TrainModel(void);
	bool SaveModel(char* strModelFileName, char* strFeatureRangeFileName);
	void ResetFeatureRanges(int numChannel);
private:
	void NormalizeAllSamples(void);
};

int TrainMain(int argc, char **argv);

#endif /* _CSVMTRAIN_H */
