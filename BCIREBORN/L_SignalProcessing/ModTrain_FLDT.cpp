#include "AttentionDetection.h"

int 
CAttentionDetection::ModTrain_FLDT(double *ps_fea, int *pi_cls, int ntrial, int m, 
								   double *net_b, double *net_z,
								   int *pi_CO, double *ps_score, double *pd_buf)
{
	double *pm_mean = pd_buf;
	double *pm_mc0 = pm_mean + m;
	double *pm_mc1 = pm_mc0 + m;
	double *pm_SW = pm_mc1 + m; // m x m
	double *pm_SB = pm_SW + m * m; // m x m
	double *ft_cls = pm_SB + m * m;	// maximum: (cls0_cls1) x m

	// calculate mean
	for (int mi = 0; mi < m; mi++) {
		double mean = 0;
		double *pd = ps_fea + mi;
		for (int itrial = 0; itrial < ntrial; itrial++) {
			mean += *pd;
			pd += m;
		}
		mean /= ntrial;
		pm_mean[mi] = mean;
	}
	// some small difference found here

	// initialize SW/SB
	double *pd0 = pm_SB;
	double *pd1 = pm_SW;
	for (int mi = 0; mi < m; mi++) {
		for (int mj = 0; mj < m; mj++) {
			*pd0++ = 0;
			*pd1++ = 0;
		}
	}

	pd0 = pm_mc0; // to calculate mean
	for (int ci = 0; ci < 2; ci++, ft_cls += m) {
		int c0 = 0;
		pd1 = ps_fea;
		for (int mi = 0; mi < m; mi++) pd0[mi] = 0;

		for (int itrial = 0; itrial < ntrial; itrial++, pd1 += m) {
			if (pi_cls[itrial] == ci) {
				memcpy(ft_cls + c0 * m, pd1, m * sizeof(double));
				c0++;

				for (int mi = 0; mi < m; mi++) {
					pd0[mi] += pd1[mi];
				}
			}
		}

		for (int mi = 0; mi < m; mi++) pd0[mi] /= c0; // mean_i

		for (int mi = 0; mi < m; mi++) {
			for (int mj = 0; mj <= mi; mj++) {
				double cv = 0;
				pd1 = ft_cls;
				for (int itrial = 0; itrial < c0; itrial++, pd1 += m) {
					cv += (pd1[mi] - pd0[mi]) * (pd1[mj] - pd0[mj]); 
				}
				pm_SW[mi * m + mj] += cv;

				cv = (pd0[mi] - pm_mean[mi]) * (pd0[mj] - pm_mean[mj]) * c0;
				pm_SB[mi * m + mj] += cv;
			}
		}

		pd0 += m;
	}

	// populate upper trianger area
	for (int mi = 0; mi < m; mi++) {
		for (int mj = mi + 1; mj < m; mj++) {
			pm_SW[mi * m + mj] = pm_SW[mj * m + mi];
			pm_SB[mi * m + mj] = pm_SB[mj * m + mi];
		}
	}

	// SW -> pA
	double **pA = (double **)ft_cls;
	pd1 = (double *)(pA + m);
	for (int mi = 0; mi < m; mi++) {
		pA[mi] = pd1;
		pd1 += m;

		for (int mj = 0; mj < m; mj++) {
			pA[mi][mj] = pm_SW[mi *m + mj];
		}
	}

	double *pS = pd1;
	pd1 += m;

	double **pV = (double **)pd1;
	pd1 = (double *)(pV + m);
	// use net_z
	for (int mi = 0; mi < m; mi++) {
		pV[mi] = net_z + m * mi;
	}

	CSignalProc::SVD(pA, m, m, pS, pV);

	double tol = m * 1e-20;
	int r = 0; // should be changed according to pS
	for (int mi = 0; mi < m; mi++) {
		if (pS[mi] < tol) break;
		r++;
	}

	if (r > 0) {
		// PINV -> V * s * U'
		// V * s
		for (int mi = 0; mi < m; mi++) {
			for (int mj = 0; mj < r; mj++) {
				pV[mi][mj] /= pS[mj];
			}
		}

		// * U'
		for (int mi = 0; mi < m; mi++) {
			for (int mj = 0; mj < m; mj++) {
				double dv = 0;
				for (int k = 0; k < r; k++) {
					dv += pV[mi][k] * pA[mj][k];
				}
				pS[mj] = dv;
			}

			for (int mj = 0; mj < m; mj++)
				pV[mi][mj] = pS[mj];
		}
	} else {
		// set pV -> 0
	}

	// * SB ==> C ==> pA
	for (int mi = 0; mi < m; mi++) {
		for (int mj = 0; mj < m; mj++) {
			double dv = 0;
			for (int k = 0; k < m; k++)
				dv += pV[mi][k] * pm_SB[k * m + mj];
			pA[mi][mj] = dv;
		}
	}

	// PCACONV ==> pV
	CSignalProc::SVD(pA, m, m, pS, pV);

	for (int itrial = 0; itrial < ntrial; itrial++) {
		double dv = 0;
		for (int k = 0; k < m; k++) {
			dv += ps_fea[itrial * m + k] * pV[k][0];
		}
		ps_score[itrial] = dv;
	}

	// m0, m1 recalculate
	pd0 = pm_mc0;
	for (int ci = 0; ci < 2; ci++) {
		double dv = 0;
		for (int k = 0; k < m; k++) {
			dv += pd0[k] * pV[k][0];
		}
		pd0[0] = dv;
		pd0 += m;
	}

	*net_b = (pm_mc0[0] + pm_mc1[0]) / 2;

	// set ptY
	int na = 0;
	for (int itrial = 0; itrial < ntrial; itrial++) {
		pi_CO[itrial] = ps_score[itrial] < *net_b? 0:1;
		if (pi_CO[itrial] == pi_cls[itrial]) na++;
	}

	if (na < ntrial / 2) {
		printf("Training: reverse all.\n");
		// reverse
		for (int itrial = 0; itrial < ntrial; itrial++) {
			ps_score[itrial] = -ps_score[itrial];
			pi_CO[itrial] = 1 - pi_CO[itrial];
		}

		*net_b = - *net_b;

		for (int mi = 0; mi < m; mi++) {
			for (int mj = 0; mj < m; mj++) {
				pV[mi][mj] = -pV[mi][mj];
			}
		}
		na = ntrial - na;
	}

	return na;
}

int 
CAttentionDetection::ModTest_FLDT(double *ps_fea, int *pi_cls, int ntrial, int m, 
								  double net_b, double *net_z,
								  int *pi_CO, double *ps_score)
{
	// net_z * ps_fea
	double *pdata = ps_fea;
	for (int itrial = 0; itrial < ntrial; itrial++) {
		double dv = 0;
		for (int k = 0; k < m; k++) {
			dv += pdata[k] * net_z[k * m];
		}
		ps_score[itrial] = dv;
		pdata += m;
	}

	// set ptY
	int na = 0;
	for (int itrial = 0; itrial < ntrial; itrial++) {
		pi_CO[itrial] = ps_score[itrial] < net_b? 0:1;
		if (pi_CO[itrial] == pi_cls[itrial]) na++;
	}
	return na;
}
