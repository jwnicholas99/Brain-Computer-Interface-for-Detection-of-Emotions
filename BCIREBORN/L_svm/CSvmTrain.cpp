#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <ctype.h>
#include "CSvmMethods.h"
#include "CSvmTrain.h"
#include ".\csvmtrain.h"

#define Malloc(type,n) (type *)malloc((n)*sizeof(type))

int max_line_len = 4096;

void exit_with_help()
{
	printf(
	"Usage: svm-train [options] training_set_file [model_file]\n"
	"options:\n"
	"-s svm_type : set type of SVM (default 0)\n"
	"	0 -- C-SVC\n"
	"	1 -- nu-SVC\n"
	"	2 -- one-class SVM\n"
	"	3 -- epsilon-SVR\n"
	"	4 -- nu-SVR\n"
	"-t kernel_type : set type of kernel function (default 2)\n"
	"	0 -- linear: u'*v\n"
	"	1 -- polynomial: (gamma*u'*v + coef0)^degree\n"
	"	2 -- radial basis function: exp(-gamma*|u-v|^2)\n"
	"	3 -- sigmoid: tanh(gamma*u'*v + coef0)\n"
	"-d degree : set degree in kernel function (default 3)\n"
	"-g gamma : set gamma in kernel function (default 1/k)\n"
	"-r coef0 : set coef0 in kernel function (default 0)\n"
	"-c cost : set the parameter C of C-SVC, epsilon-SVR, and nu-SVR (default 1)\n"
	"-n nu : set the parameter nu of nu-SVC, one-class SVM, and nu-SVR (default 0.5)\n"
	"-p epsilon : set the epsilon in loss function of epsilon-SVR (default 0.1)\n"
	"-m cachesize : set cache memory size in MB (default 40)\n"
	"-e epsilon : set tolerance of termination criterion (default 0.001)\n"
	"-h shrinking: whether to use the shrinking heuristics, 0 or 1 (default 1)\n"
	"-wi weight: set the parameter C of class i to weight*C, for C-SVC (default 1)\n"
	"-v n: n-fold cross validation mode\n"
	);
	exit(1);
}

void parse_command_line(int argc, char **argv, char *input_file_name, int isz, char *model_file_name, int msz);
void read_problem(const char *filename);
void do_cross_validation();

struct CSvmParameter param;		// set by parse_command_line
struct CSvmProblem prob;		// set by read_problem
struct CSvmModel *model;
struct CSvmNode *x_space;
int cross_validation = 0;
int nr_fold;

int TrainMain(int argc, char **argv)
{
	char input_file_name[1024];
	char model_file_name[1024];
	const char *error_msg;

	parse_command_line(argc, argv, input_file_name, 1024, model_file_name, 1024);
	read_problem(input_file_name);
	error_msg = svm_check_parameter(&prob,&param);

	if(error_msg)
	{
		fprintf(stderr,"Error: %s\n",error_msg);
		exit(1);
	}

	if(cross_validation)
	{
		do_cross_validation();
	}
	else
	{
		model = svm_train(&prob,&param);
		svm_save_model(model_file_name,model);
		svm_destroy_model(model);
	}

	free(prob.y);
	free(prob.x);
	free(x_space);

	return 0;
}

void do_cross_validation()
{
	int i;
	int total_correct = 0;
	double total_error = 0;
	double sumv = 0, sumy = 0, sumvv = 0, sumyy = 0, sumvy = 0;

	// random shuffle
	for(i=0;i<prob.l;i++)
	{
		int j = i+rand()%(prob.l-i);
		struct CSvmNode *tx;
		double ty;
			
		tx = prob.x[i];
		prob.x[i] = prob.x[j];
		prob.x[j] = tx;

		ty = prob.y[i];
		prob.y[i] = prob.y[j];
		prob.y[j] = ty;
	}

	for(i=0;i<nr_fold;i++)
	{
		int begin = i*prob.l/nr_fold;
		int end = (i+1)*prob.l/nr_fold;
		int j,k;
		struct CSvmProblem subprob;

		subprob.l = prob.l-(end-begin);
		subprob.x = Malloc(struct CSvmNode*,subprob.l);
		subprob.y = Malloc(double,subprob.l);
			
		k=0;
		for(j=0;j<begin;j++)
		{
			subprob.x[k] = prob.x[j];
			subprob.y[k] = prob.y[j];
			++k;
		}
		for(j=end;j<prob.l;j++)
		{
			subprob.x[k] = prob.x[j];
			subprob.y[k] = prob.y[j];
			++k;
		}

		if(param.svm_type == EPSILON_SVR ||
		   param.svm_type == NU_SVR)
		{
			struct CSvmModel *submodel = svm_train(&subprob,&param);
			double error = 0;
			for(j=begin;j<end;j++)
			{
				double v = SvmPredict(submodel,prob.x[j]);
				double y = prob.y[j];
				error += (v-y)*(v-y);
				sumv += v;
				sumy += y;
				sumvv += v*v;
				sumyy += y*y;
				sumvy += v*y;
			}
			svm_destroy_model(submodel);
			printf("Mean squared error = %g\n", error/(end-begin));
			total_error += error;			
		}
		else
		{
			struct CSvmModel *submodel = svm_train(&subprob,&param);
			int correct = 0;
			for(j=begin;j<end;j++)
			{
				double v = SvmPredict(submodel,prob.x[j]);
				if(v == prob.y[j])
					++correct;
			}
			svm_destroy_model(submodel);
			printf("Accuracy = %g%% (%d/%d)\n", 100.0*correct/(end-begin),correct,(end-begin));
			total_correct += correct;
		}

		free(subprob.x);
		free(subprob.y);
	}		
	if(param.svm_type == EPSILON_SVR || param.svm_type == NU_SVR)
	{
		printf("Cross Validation Mean squared error = %g\n",total_error/prob.l);
		printf("Cross Validation Squared correlation coefficient = %g\n",
			((prob.l*sumvy-sumv*sumy)*(prob.l*sumvy-sumv*sumy))/
			((prob.l*sumvv-sumv*sumv)*(prob.l*sumyy-sumy*sumy))
			);
	}
	else
		printf("Cross Validation Accuracy = %g%%\n",100.0*total_correct/prob.l);
}

void parse_command_line(int argc, char **argv, char *input_file_name, int isz, char *model_file_name, int msz)
{
	int i;

	// default values
	param.svm_type = C_SVC;
	param.kernel_type = RBF;
	param.degree = 3;
	param.gamma = 0;	// 1/k
	param.coef0 = 0;
	param.nu = 0.5;
	param.cache_size = 40;
	param.C = 1;
	param.eps = 1e-3;
	param.p = 0.1;
	param.shrinking = 1;
	param.nr_weight = 0;
	param.weight_label = NULL;
	param.weight = NULL;

	// parse options
	for(i=1;i<argc;i++)
	{
		if(argv[i][0] != '-') break;
		++i;
		switch(argv[i-1][1])
		{
			case 's':
				param.svm_type = atoi(argv[i]);
				break;
			case 't':
				param.kernel_type = atoi(argv[i]);
				break;
			case 'd':
				param.degree = atof(argv[i]);
				break;
			case 'g':
				param.gamma = atof(argv[i]);
				break;
			case 'r':
				param.coef0 = atof(argv[i]);
				break;
			case 'n':
				param.nu = atof(argv[i]);
				break;
			case 'm':
				param.cache_size = atof(argv[i]);
				break;
			case 'c':
				param.C = atof(argv[i]);
				break;
			case 'e':
				param.eps = atof(argv[i]);
				break;
			case 'p':
				param.p = atof(argv[i]);
				break;
			case 'h':
				param.shrinking = atoi(argv[i]);
				break;
			case 'v':
				cross_validation = 1;
				nr_fold = atoi(argv[i]);
				if(nr_fold < 2)
				{
					fprintf(stderr,"n-fold cross validation: n must >= 2\n");
					exit_with_help();
				}
				break;
			case 'w':
				++param.nr_weight;
				param.weight_label = (int *)realloc(param.weight_label,sizeof(int)*param.nr_weight);
				param.weight = (double *)realloc(param.weight,sizeof(double)*param.nr_weight);
				param.weight_label[param.nr_weight-1] = atoi(&argv[i-1][2]);
				param.weight[param.nr_weight-1] = atof(argv[i]);
				break;
			default:
				fprintf(stderr,"unknown option\n");
				exit_with_help();
		}
	}

	// determine filenames

	if(i>=argc)
		exit_with_help();

	strcpy_s(input_file_name, isz, argv[i]);

	if(i<argc-1)
		strcpy_s(model_file_name, msz, argv[i+1]);
	else
	{
		char *p = strrchr(argv[i],'/');
		if(p==NULL)
			p = argv[i];
		else
			++p;
		sprintf_s(model_file_name, msz, "%s.model", p);
	}
}

// read in a problem (in svmlight format)

void read_problem(const char *filename)
{
	int elements, max_index, i, j;
	FILE *fp = NULL;
	fopen_s(&fp, filename, "r");
	
	if(fp == NULL)
	{
		fprintf(stderr,"can't open input file %s\n",filename);
		exit(1);
	}

	prob.l = 0;
	elements = 0;
	while(1)
	{
		int c = fgetc(fp);
		switch(c)
		{
			case '\n':
				++prob.l;
				// fall through,
				// count the '-1' element
			case ':':
				++elements;
				break;
			case EOF:
				goto out;
			default:
				;
		}
	}
out:
	rewind(fp);

	prob.y = Malloc(double,prob.l);
	prob.x = Malloc(struct CSvmNode *,prob.l);
	x_space = Malloc(struct CSvmNode,elements);

	max_index = 0;
	j=0;
	for(i=0;i<prob.l;i++)
	{
		double label;
		prob.x[i] = &x_space[j];
		fscanf_s(fp, "%lf", &label);
		prob.y[i] = label;
		while(1)
		{
			int c;
			do {
				c = getc(fp);
				if(c=='\n') goto out2;
			} while(isspace(c));
			ungetc(c,fp);
			fscanf_s(fp, "%d:%lf", &(x_space[j].index), &(x_space[j].value));
			++j;
		}	
out2:
		if(j>=1 && x_space[j-1].index > max_index)
			max_index = x_space[j-1].index;
		x_space[j++].index = -1;
	}

	if(param.gamma == 0)
		param.gamma = 1.0/max_index;

	fclose(fp);
}

CSvmTrain::CSvmTrain()
{
	m_prob.l=0;m_prob.x=NULL;m_prob.y=NULL;
	m_pFeatureMax=NULL;
	m_pFeatureMin=NULL;
	m_model=NULL;
}

CSvmTrain::~CSvmTrain()
{
	if(m_prob.y!=NULL) delete m_prob.y;
	if(m_prob.x!=NULL){
		for(int i=0;i<m_prob.l;i++) delete m_prob.x[i];
		delete m_prob.x;
	}
	m_prob.l=0;m_prob.x=NULL;m_prob.y=NULL;
	if(m_model!=NULL) svm_destroy_model(m_model);m_model=NULL;
	if(m_pFeatureMax!=NULL) delete m_pFeatureMax;m_pFeatureMax=NULL;
	if(m_pFeatureMin!=NULL) delete m_pFeatureMin;m_pFeatureMin=NULL;
}

int CSvmTrain::AddNewSample(double* pFeature, int iLabel)
{
	m_prob.y[m_prob.l]=iLabel;
	m_prob.x[m_prob.l]=new CSvmNode[m_iFeatureDim+1];

	for(int i=0;i<m_iFeatureDim;i++){
		m_prob.x[m_prob.l][i].index=i+1;
		m_prob.x[m_prob.l][i].value=pFeature[i];
	}
	m_prob.x[m_prob.l][m_iFeatureDim].index=-1;

	m_prob.l++;

	//Calculate max and min in each dim
	if(m_prob.l==1){
		for(int i=0;i<m_iFeatureDim;i++){
			m_pFeatureMax[i]=pFeature[i];
			m_pFeatureMin[i]=pFeature[i];
		}
	}
	else{
		for(int i=0;i<m_iFeatureDim;i++){
			m_pFeatureMax[i]=m_pFeatureMax[i]>pFeature[i]?m_pFeatureMax[i]:pFeature[i];
			m_pFeatureMin[i]=m_pFeatureMin[i]<pFeature[i]?m_pFeatureMin[i]:pFeature[i];
		}
	}

	return m_prob.l;
}

void CSvmTrain::ResetFeatureRanges(int numChannel)
{
	if (m_iFeatureDim % numChannel != 0) {
		printf("Wrong number of channels!\n");
		return;
	}

	int nseg = m_iFeatureDim / numChannel;
	printf("CSvmTrain::ResetFeatureRanges: %d * %d\n", numChannel, nseg);

	for (int i = 0; i < m_iFeatureDim; i += nseg) {
		int ti = i + nseg;
		double cmin = m_pFeatureMin[i];
		double cmax = m_pFeatureMax[i];
		for (int j = i + 1; j < ti; j++) {
			if (cmin > m_pFeatureMin[j]) cmin = m_pFeatureMin[j];
			if (cmax < m_pFeatureMax[j]) cmax = m_pFeatureMax[j];
		}

		for (int j = i; j < ti; j++) {
			m_pFeatureMin[j] = cmin;
			m_pFeatureMax[j] = cmax;
		}
	}
}

void CSvmTrain::Initialize(CSvmParameter* Para, int iMaxNumSample, int iFeatureDim)
{
	m_iMaxNumSample=iMaxNumSample;
	m_iFeatureDim=iFeatureDim;

	if(m_prob.y!=NULL) delete m_prob.y;
	if(m_prob.x!=NULL){
		for(int i=0;i<m_prob.l;i++) delete m_prob.x[i];
		delete m_prob.x;
	}
    m_prob.y=new double[m_iMaxNumSample];
	m_prob.x=new CSvmNode*[m_iMaxNumSample];

	if(Para!=NULL) memcpy(&m_param,Para,sizeof(CSvmParameter));
	if(m_param.gamma==0) m_param.gamma=1.0/iFeatureDim;
	if(m_model!=NULL) svm_destroy_model(m_model);
	m_model=NULL;
	if(m_pFeatureMax!=NULL) delete m_pFeatureMax;m_pFeatureMax=new double[iFeatureDim];
	if(m_pFeatureMin!=NULL) delete m_pFeatureMin;m_pFeatureMin=new double[iFeatureDim];
}

bool CSvmTrain::TrainModel(void)
{
	NormalizeAllSamples();
	if(m_model!=NULL) svm_destroy_model(m_model);m_model=NULL;
	m_model = svm_train(&m_prob,&m_param);
	return m_model!=NULL;
}

bool CSvmTrain::SaveModel(char* strModelFileName, char* strFeatureRangeFileName)
{
	FILE* fp = NULL;
	fopen_s(&fp, strFeatureRangeFileName, "w+");
	if(fp==NULL) return false;
	for(int i=0;i<m_iFeatureDim;i++){
		fprintf(fp,"%d %f %f\n",i+1,m_pFeatureMin[i],m_pFeatureMax[i]);
	}
	fclose(fp);
	if(m_model==NULL) return false;
	svm_save_model(strModelFileName,m_model);

	return true;
}

void CSvmTrain::NormalizeAllSamples(void)
{
	double fLow=-1;
	double fHigh=1;
	double fDM=fHigh-fLow;
	double* pFeatureDM = new double[m_iFeatureDim];
	for (int iFea = 0; iFea < m_iFeatureDim; iFea++) {
		pFeatureDM[iFea] = m_pFeatureMax[iFea] - m_pFeatureMin[iFea];
	}
	for (int iSample = 0; iSample < m_prob.l; iSample++) {
		for (int iFea=0;iFea<m_iFeatureDim;iFea++) {
			m_prob.x[iSample][iFea].value = fLow + fDM*(m_prob.x[iSample][iFea].value-m_pFeatureMin[iFea])/pFeatureDM[iFea];
		}
	}

	// ccwang 20030314
	delete [] pFeatureDM;
}
