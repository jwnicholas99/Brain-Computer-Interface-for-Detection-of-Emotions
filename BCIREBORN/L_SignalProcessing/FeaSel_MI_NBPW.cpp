#include "AttentionDetection.h"

// FSMINBPW Feature Selection using MI based on Naive Bayes Parzen Window classifier
// output: selected features
// Space allocation
// reuse memory pd_buf
int CAttentionDetection::FeaSel_NBPW(double *pd_fea, int *pi_cls, int ntrial, int ft_len, 
										int *pi_sf, int *pi_CO, double *pd_buf)
{
	int ncls[2] = {0, 0};
	for (int itrial = 0; itrial < ntrial; itrial++) {
		ncls[pi_cls[itrial]]++;
	}

	//
	// total length of doubles: 3 * ntrial * ft_len + 3 * ft_len
	//
	double *ft_cls0 = pd_buf; // class 0 features
	double *ft_cls1 = ft_cls0 + ncls[0] * ft_len; // class 1 features

	double *ft_H0 = ft_cls1 + ncls[1] * ft_len; // H value for class 0 feature
	double *ft_H1 = ft_H0 + ft_len; // H value for class 1 feature

	double *pw0x = ft_H1 + ft_len; // Parsen posibility - class0
	double *pw1x = pw0x + ntrial * ft_len; // Parsen posibility - class1

	double *p_smi = pw1x + ntrial * ft_len; // smi - ft_len

	int c0 = 0, c1 = 0;
	for (int i = 0; i < ntrial; i++) {
		if (pi_cls[i] == 0) {
			memcpy(ft_cls0 + c0 * ft_len, pd_fea + i * ft_len, ft_len * sizeof(double));
			c0++;
		} else {
			memcpy(ft_cls1 + c1 * ft_len, pd_fea + i * ft_len, ft_len * sizeof(double));
			c1++;
		}
	}
	// c0 == ncls0? c1 == ncls1?

	double hs = 1.0;
	double hc0 = 4.0 / (3.0 * ncls[0]);
	hc0 = hs * pow(hc0, 0.2);
	double hc1 = 4.0 / (3.0 * ncls[1]);
	hc1 = hs * pow(hc1, 0.2);

	// calculate std => H
	for (int fi = 0; fi < ft_len; fi++) {
		double mean = 0;
		double *pdata = ft_cls0 + fi;
		for (int ti = 0; ti < ncls[0]; ti++) {
			mean += *pdata;
			pdata += ft_len;
		}
		mean /= ncls[0];

		double fvar = 0;
		pdata = ft_cls0 + fi;
		for (int ti = 0; ti < ncls[0]; ti++) {
			double dv = (*pdata) - mean;
			fvar += dv * dv;
			pdata += ft_len;
		}

		if (ncls[0] > 2) fvar /= (ncls[0] - 1);
		fvar = sqrt(fvar); // std

		fvar *= hc0;
		if (fvar == 0) fvar = 0.005; // use a small value of h instead of zero

		ft_H0[fi] = fvar;

		mean = 0;
		pdata = ft_cls1 + fi;
		for (int ti = 0; ti < ncls[1]; ti++) {
			mean += *pdata;
			pdata += ft_len;
		}
		mean /= ncls[1];

		fvar = 0;
		pdata = ft_cls1 + fi;
		for (int ti = 0; ti < ncls[1]; ti++) {
			double dv = *pdata - mean;
			fvar += dv * dv;
			pdata += ft_len;
		}

		if (ncls[1] > 2) fvar /= (ncls[1] - 1);
		fvar = sqrt(fvar); // std

		fvar *= hc1;
		if (fvar == 0) fvar = 0.005; // use a small value of h instead of zero

		ft_H1[fi] = fvar;
	}

	c0 = 0;
	for (int fti = 0; fti < ft_len; fti++) {
		for (int itrial = 0; itrial < ntrial; itrial++) {
			double pxf0 = CSignalProc::Parzen(pd_fea[itrial * ft_len + fti], ft_cls0, ncls[0], ft_len, fti, ft_H0[fti]);
			double pxf1 = CSignalProc::Parzen(pd_fea[itrial * ft_len + fti], ft_cls1, ncls[1], ft_len, fti, ft_H1[fti]);
			double t = pxf0 + pxf1;
			pw0x[c0] = pxf0 / t;
			pw1x[c0] = pxf1 / t;
			c0++;
		}
	}

	int nf = ft_len / 3; //default number of features to be selected
	int m = 0; // selected number of features

	while (m <= nf) {
		double max_tmi = 0;
		int sf_tmi = -1;

		for (int k = 0; k < ft_len; k++) {
			bool found = false;
			for (int ki = 0; !found && ki < m; ki++) {
				if (pi_sf[ki] == k) {
					found = true;
				}
			}
			if (found) continue;

			// Initialize conditional entropy H(w|x)
			pi_sf[m] = k;

			for (int itrial = 0; itrial < ntrial; itrial++) {
				// Compute conditional probability using all data tuples
				double pxw0 = 1.0;
				double pxw1 = 1.0;
				for (int mi = 0; mi <= m; mi++) {
					pxw0 *= pw0x[pi_sf[mi] * ntrial + itrial];
					pxw1 *= pw1x[pi_sf[mi] * ntrial + itrial];
				}
				double t = pxw0 + pxw1;
				pxw0 /= t;
				pxw1 /= t;

				pi_CO[itrial] = pxw0 < pxw1? 1:0;
			}

			// Compute mutual information from classification output
			double tmi = CSignalProc::MutualInformation(pi_CO, pi_cls, ntrial);
			if (tmi > max_tmi) {
				max_tmi = tmi;
				sf_tmi = k;
			}
		}

		if (m > 0 && abs(p_smi[m-1] - max_tmi) < 0.0001) {
			break;
		} else {
			p_smi[m] = max_tmi;
			pi_sf[m] = sf_tmi;
			m++;
		}
	}

	return m;
}
