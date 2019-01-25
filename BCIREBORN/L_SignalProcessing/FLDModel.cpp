#include "FLDModel.h"
#include "..\L_Utilities\ParaReader.h"

#include <stdio.h>
#include <string.h>
#include <stdlib.h>

CFLDModel::CFLDModel(void)
{
	m_nFeature = 0;
	m_pBuffer = NULL;

	m_pMean = m_pStd = m_pZ = NULL;
	m_fBias = 0;
	m_nS = 0;
}

CFLDModel::~CFLDModel(void)
{
	Clear();
}

void CFLDModel::Clear()
{
	if (m_pBuffer != NULL) {
		delete[] m_pBuffer;
		m_pBuffer = NULL;
		m_pMean = m_pStd = m_pZ = NULL;
	}
}

bool CFLDModel::LoadModel(const char *mfn)
{
	Clear();

	// Load model
	FILE *fp = NULL;
	if (fopen_s(&fp, mfn, "rt")) {
		printf("%s: Cannot load parameter file %s!\n", __FILE__, mfn);
		return false;
	}

	CParaReader cpar;
	cpar.SetSeperator(':');
	char line[1024];

	printf("%s: load model file %s ...\n", __FILE__, mfn);

	// format par: val
	const char *SEP = " ,\t\n";

	while (fgets(line, 1024, fp)) {
		char *par, *val;
		if (!cpar.GetParValue(line, par, val)) continue;

		// Specific parameters
		if (strcmp(par, "Classification") == 0) {
			if (strcmp(val, "FLD") == 0) {
				// FLD_FeatureMean:
				fgets(line, 1024, fp);
				cpar.GetParValue(line, par, val);

				m_nFeature = atoi(val);
				m_pBuffer = new double[m_nFeature * 3];
				m_pMean = m_pBuffer;
				m_pStd = m_pMean + m_nFeature;
				m_pZ = m_pStd + m_nFeature;

				int n = 0;
				sscanf_s(val, "%d", &n);

				fgets(line, 1024, fp);
				char *context = NULL;
				char *tok = strtok_s(line, SEP, &context);
				double fv;
				for (int i = 0; i < n; i++) {
					sscanf_s(tok, "%I64X", &fv);
					m_pMean[i] = fv;
					tok = strtok_s(NULL, SEP, &context);
				}

				// FLD_FeatureSTD: 
				fgets(line, 1024, fp);
				cpar.GetParValue(line, par, val);
				n = atoi(val);
				fgets(line, 1024, fp);
				tok = strtok_s(line, SEP, &context);
				for (int i = 0; i < n; i++) {
					sscanf_s(tok, "%I64X", &fv);
					m_pStd[i] = fv;
					tok = strtok_s(NULL, SEP, &context);
				}

				// FLD_LinearTransformVector: 
				fgets(line, 1024, fp);
				cpar.GetParValue(line, par, val);
				n = atoi(val);
				fgets(line, 1024, fp);
				tok = strtok_s(line, SEP, &context);
				for (int i = 0; i < n; i++) {
					sscanf_s(tok, "%I64X", &fv);
					m_pZ[i] = fv;
					tok = strtok_s(NULL, SEP, &context);
				}

				//FLD_Bias: 
				fgets(line, 1024, fp);
				cpar.GetParValue(line, par, val);
				sscanf_s(val, "%I64X", &fv);
				m_fBias = fv;

				fgets(line, 1024, fp);
				cpar.GetParValue(line, par, val);
				sscanf_s(val, "%d", &m_nS);

			} else {
				printf("Unknown parameter.\n");
			}
		} else {
			continue;
		}
		printf("Processed: '%s' = '%s'\n", par, val);
	}
	return true;
}

double CFLDModel::GetScore(double *feature)
{
	double score = 0;

	for (int i = 0; i < m_nFeature; i++) {
		double fv = feature[i];
		fv -= m_pMean[i];
		fv /= m_pStd[i];

		score += fv * m_pZ[i];
	}

	score -= m_fBias;

	if (m_nS == 0) score = -score;

	return score;
}
