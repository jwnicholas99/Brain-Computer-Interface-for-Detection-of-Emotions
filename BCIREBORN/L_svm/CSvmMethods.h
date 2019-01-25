#ifndef _CSVMMETHODS_H
#define _CSVMMETHODS_H

#ifdef __cplusplus
extern "C" {
#endif

struct CSvmNode
{
	int index;
	double value;
};

struct CSvmProblem
{
	int l;
	double *y;
	struct CSvmNode **x;
};

enum { C_SVC, NU_SVC, ONE_CLASS, EPSILON_SVR, NU_SVR };	/* svm_type */
enum { LINEAR, POLY, RBF, SIGMOID };	/* kernel_type */

struct CSvmParameter
{
	int svm_type;
	int kernel_type;
	double degree;	/* for poly */
	double gamma;	/* for poly/rbf/sigmoid */
	double coef0;	/* for poly/sigmoid */

	/* these are for training only */
	double cache_size; /* in MB */
	double eps;	/* stopping criteria */
	double C;	/* for C_SVC, EPSILON_SVR and NU_SVR */
	int nr_weight;		/* for C_SVC */
	int *weight_label;	/* for C_SVC */
	double* weight;		/* for C_SVC */
	double nu;	/* for NU_SVC, ONE_CLASS, and NU_SVR */
	double p;	/* for EPSILON_SVR */
	int shrinking;	/* use the shrinking heuristics */
};

#ifdef LINUX

typedef struct CSvmModel *(*SVM_TRAIN)(const struct CSvmProblem *,
			       const struct CSvmParameter *);

typedef int (*SVM_SAVE_MODEL)(const char *, const struct CSvmModel *);

typedef struct CSvmModel *(*SVM_LOAD_MODEL)(const char *);

typedef double (*SVM_PREDICT)(const struct CSvmModel *, const struct CSvmNode *, double *);

typedef void (*SVM_DESTROY_MODEL)(struct CSvmModel *);

typedef const char *(*SVM_CHECK_PARAMETER)(const struct CSvmProblem *, 
				   const struct CSvmParameter *);
#else

struct CSvmModel *svm_train(const struct CSvmProblem *prob,
			    const struct CSvmParameter *param);

int svm_save_model(const char *model_file_name, const struct CSvmModel *model);

struct CSvmModel *SvmLoadModel(const char *model_file_name);

double SvmPredict(const struct CSvmModel *model, const struct CSvmNode *x, double *fMargin = 0);

void svm_destroy_model(struct CSvmModel *model);

const char *svm_check_parameter(const struct CSvmProblem *prob, const struct CSvmParameter *param);

#endif

#ifdef __cplusplus
}
#endif

#endif /* _CSVMMETHODS_H */
