#include "attentiondetection.h"
#include "..\L_Utilities\ParaReader.h"
#include "..\L_Utilities\ContEEGFile.h"

//	bool GetDirectory(char *szFullPath, char *outDirectory);

// Train concentration model
bool 
CAttentionDetection::TrainAttentionModel(const char *fn_model, const char *fn_trcfg, const char *fn_cntlist)
{
	const char NUAMPS_CHLIST[] = "HEOL,HEOR,Fp1,Fp2,VEOU,VEOL,F7,F3,Fz,F4,F8,FT7,FC3,FCz,FC4,FT8,T7,C3,Cz,C4,T8,TP7,CP3,CPz,CP4,TP8,A1,P7,P3,Pz,P4,P8,A2,O1,Oz,O2,FT9,FT10,PO1,PO2";
	const char BR_CHLIST[] = "CH1,CH2,CH3,CH4,CH5,CH6,CH7,CH8";
	const char *sep = " ,\t\n";

	char line[1024];
	char *par, *val;
	CParaReader lreader;

	printf("Read config file: %s ...\n", fn_trcfg);
	FILE *fp = NULL;
	if (fopen_s(&fp, fn_trcfg, "rt") != 0) {
		printf("Cannot open config file %s!\n", fn_trcfg);
		return false;
	}
	
	// read parameters
	double tWinLen = 2.0, tWinShift = 1.0, tScorePt = 0.2;
	int winlen = 0, winshift = 0;

	// buffer channel_list
	char **chan_list = NULL;
	int *pch_idx = NULL;
	bool IsCSP = false;

	char char_buf[1024];
	char *pbuf = char_buf;
	int nch = 0;

	while (!feof(fp)) {
		fgets(line, sizeof(line), fp);

		if (!lreader.GetParValue(line, par, val)) {
			continue;
		}

		// printf("%s = %s\n", par, val);

		if (strcmp(par, "WindowLenSec") == 0) {
			tWinLen = atof(val);
		} else if (strcmp(par, "UsedChannelIdx") == 0) {
			// format: [c1, c2, ...]
			if (*val == '[') {
				val++;
				int l = (int) strlen(val) - 1;
				while (isspace(val[l])) l--;
				if (val[l] == ']') val[l] = 0;
			}

			nch = 0;
			pch_idx = (int*) (((unsigned int)pbuf + 3) & ~3);
			char *token, *context;
			token = strtok_s(val, sep, &context);
			while (token) {
				pch_idx[nch] = atoi(token);
				printf("Sel ch_idx(%d) = %d\n", nch, pch_idx[nch]);
				nch++;
				token = strtok_s(NULL, sep, &context);
			}
			pbuf = (char*) (pch_idx + nch);
		} else if (strcmp(par, "ChannelList") == 0) {
			char *token = NULL, *context;

			token = strtok_s(val, sep, &context);
			nch = 0;
			char *pChStart = pbuf;
			while (token != NULL) {
				strcpy_s(pbuf, char_buf + sizeof(char_buf) - pbuf, token);
				pbuf += strlen(token) + 1;
				nch++;
				token = strtok_s(NULL, sep, &context);
			}
			chan_list = (char **)(((unsigned int)pbuf + 3) & ~3);
			pbuf = (char*) (chan_list + nch);
			for (int i = 0; i < nch; i++) {
				chan_list[i] = pChStart;
				pChStart += strlen(pChStart) + 1;
				printf("Sel ch %d = %s\n", i, chan_list[i]);
			}
		} else if (strcmp(par, "WinShift") == 0) {
			tWinShift = atof(val);
		}
	}

	fclose(fp);

	if (pch_idx == NULL) {
		if (chan_list == NULL) {
			printf("Selected channel not specified, using all channels");
		} else if (nch != 0) {
			// allocate space for pch_idx
			pch_idx = (int *) (((int) pbuf + 3) & ~3);
			pbuf = (char *) (pch_idx + nch);
		}
	}

	// Read cnt files
	fp = NULL;
	if (fopen_s(&fp, fn_cntlist, "rt")) {
		printf("cannot open cnt list file:%s!", fn_cntlist);
		return false;
	}

	char pre_dir[1024], cnt_fn[1024];
	GetDirectory(fn_cntlist, pre_dir, sizeof(pre_dir));

	CntEEGHeader cntHeader;
	cntHeader.num_chan = 0;

	pbuf = (char *)(((unsigned int) pbuf + 3) & ~3);
	CContEEGFile **cnt_list = (CContEEGFile **)pbuf;
	int ncnt = 0;

	// EEG info
	int nsample = 0;

	while (fgets(line, sizeof(line), fp)) {
		par = strtok_s(line, sep, &val);
		if (par == NULL || atoi(par) == 0) continue;

		par = strtok_s(NULL, "\n", &val);
		val = par;

		// if it is an relative path, add prefix
		if (val[1] != ':') {
			sprintf_s(cnt_fn, "%s\\%s", pre_dir, val);
		} else {
			strcpy_s(cnt_fn, val);
		}

		printf("cnt file = %s\n", cnt_fn);
		CContEEGFile *pcnt = CContEEGFile::ReadContEEGFile(cnt_fn);
		if (pcnt == NULL) {
			break;
		}

		if (cntHeader.num_chan == 0) {
			// header and selected channels
			cntHeader = pcnt->m_hHeader;

			if (nch == 0) nch = cntHeader.num_chan;

			if (chan_list) {
				char *cont = cnt_fn; // temporarily using this buffer

				if (cntHeader.num_chan == 40) { // Nuamps
					strcpy_s(cnt_fn, NUAMPS_CHLIST);
				} else if (cntHeader.num_chan == 8) { // BioRadio
					strcpy_s(cnt_fn, BR_CHLIST);
				} else {
					cont = NULL;
					// don't know channel names, just use from first
					for (int i = 0; i < nch; i++) {
						pch_idx[i] = i;
					}
				}

				if (cont) {
					// fill pch_idx
					int jch = 0;
					char* token = strtok_s(cnt_fn, sep, &cont);
					while (token) {
						for (int ich = 0; ich < nch; ich++) {
							if (_strcmpi(chan_list[ich], token) == 0) {
								pch_idx[ich] = jch;
							}
						} // ich
						token = strtok_s(NULL, sep, &cont);
						jch++;
					} // jch
				} // xx_CHLIST
			} // chan_list
		} else {
			// check header consistency?
		}

		cnt_list[ncnt] = pcnt;
		nsample += pcnt->m_nNumSample;

		ncnt++;
	}
	fclose(fp);
	pbuf = (char *) (cnt_list + ncnt);

	if (ncnt == 0) {
		return false;
	}

	// ccwang 20100510: ==> chan_list

	winlen = (int) (cntHeader.sample_rate * tWinLen);
	winshift = (int) (cntHeader.sample_rate * tWinShift);

	// Get data size
	int ntrial = 0;
	for (int icnt = 0; icnt < ncnt; icnt++) {
		CContEEGFile *pcnt = cnt_list[icnt];

		int stim_off = pcnt->m_pStimInfo[0].code / 50 * 50;
		int c0 = 0, c1 = 0;

		for (int istim = 0; istim < pcnt->m_nNumStim; istim++) {
			pcnt->m_pStimInfo[istim].code -= stim_off;

			switch (pcnt->m_pStimInfo[istim].code) {
			case 1: // trial start
				c0 = istim + 1;
				break;
			case 0: // relax
				c1 = istim - 1;

				int clen = pcnt->m_pStimInfo[c1].no - pcnt->m_pStimInfo[c0].no - 1;
				// ccwang: the last -1 is from matlab. I don't think it should be there.

				if (clen < winlen) continue;
				ntrial += (clen - winlen) / winshift + 1;

				int rlen = pcnt->m_nNumSample;
				if (istim < pcnt->m_nNumStim - 1) rlen = pcnt->m_pStimInfo[istim + 1].no;
				rlen -= pcnt->m_pStimInfo[istim].no;
				if (rlen > clen) rlen = clen;
				else if (rlen < winlen) continue;
				ntrial += (rlen - winlen) / winshift + 1;
				break;
			}
		}
	}

	int nfilter = 0;
	while (FiltFilt_FilterBankString[nfilter]) nfilter++;
	nfilter /= 3;

	int ft_len = nfilter * nch;

	int blen = 0;
	blen += ntrial * nch * winlen; // segmented data
	blen += winlen; // processing data
	blen += ntrial * ft_len; // feature

	double *pd_fea = new double[blen];
	double *pd_line = pd_fea + ntrial * ft_len;
	double *pd_eeg = pd_line + winlen; // segmented eeg data

	int* pi_cls = new int[ntrial + ft_len + ntrial + ntrial]; // true 
	int* pi_sf = pi_cls + ntrial; // selected feature
	int* pi_CO = pi_sf + ft_len; // classification output
	int* pj_cls = pi_CO + ntrial; // class label for CV

	int ncls0 = 0, ncls1 = 0;

	// data segmentation
	double *pdata = pd_eeg;
	int *pclass = pi_cls;
	for (int icnt = 0; icnt < ncnt; icnt++) {
		CContEEGFile *pcnt = cnt_list[icnt];

		int stim_off = pcnt->m_pStimInfo[0].code / 50 * 50;
		int c0 = 0, c1 = 0;

		for (int istim = 0; istim < pcnt->m_nNumStim; istim++) {
			switch (pcnt->m_pStimInfo[istim].code) {
			case 1: // trial start
				c0 = istim + 1;
				break;
			case 0: // relax
				c1 = istim - 1;

				int clen = pcnt->m_pStimInfo[c1].no - pcnt->m_pStimInfo[c0].no - 1;
				int nc = 0;
				if (clen >= winlen) {
					nc = (clen - winlen) / winshift + 1;
				}

				if (nc < 1) {
					printf("Anything wrong here?\n");
				}

				int p0 = pcnt->m_pStimInfo[c0].no;
				for (int ti = 0; ti < nc; ti++) { // trial
					for (int ci = 0; ci < nch; ci++) { // channel
						int s_off = p0 * pcnt->m_hHeader.num_chan + pch_idx[ci];
						for (int si = 0; si < winlen; si++) { // sample
							*pdata++ = (pcnt->m_hHeader.resolution > 0)? pcnt->m_pEEGBuffer[s_off] * pcnt->m_hHeader.resolution
								: *((float *) (&pcnt->m_pEEGBuffer[s_off]));
							s_off += pcnt->m_hHeader.num_chan;
						}
					}
					p0 += winshift;
					*pclass++ = 0;
					ncls0++;
				}

				if (nc < 1) {
					printf("Anything wrong here?\n");
				}

				int rlen = pcnt->m_nNumSample;
				if (istim < pcnt->m_nNumStim - 1) rlen = pcnt->m_pStimInfo[istim + 1].no;
				rlen -= pcnt->m_pStimInfo[istim].no;
				if (rlen > clen) rlen = clen;
				nc = 0;
				if (rlen >= winlen) {
					nc = (rlen - winlen) / winshift + 1; // Warning: don't have this in matlab
				}

				p0 = pcnt->m_pStimInfo[istim].no;
				for (int ti = 0; ti < nc; ti++) { // trial
					for (int ci = 0; ci < nch; ci++) { // channel
						int s_off = p0 * pcnt->m_hHeader.num_chan + pch_idx[ci];
						for (int si = 0; si < winlen; si++) { // sample
							*pdata++ = (pcnt->m_hHeader.resolution > 0)? pcnt->m_pEEGBuffer[s_off] * pcnt->m_hHeader.resolution
								: *((float *) (&pcnt->m_pEEGBuffer[s_off]));
							s_off += pcnt->m_hHeader.num_chan;
						}
					}
					p0 += winshift;
					*pclass++ = 1;
					ncls1++;
				}
				break;
			}
		}
	}

	// prepfilterbank_noCSP
	const char **hexs = GetFilterBankString(cntHeader.sample_rate);

	for (int fi = 0; fi < nfilter; fi++) {
		CFiltering filter;

		filter.InitHexString(hexs[0], hexs[1], hexs[2]);
		hexs += 3;

		printf("Filter %d loaded.\n", fi + 1);

		for (int itrial = 0; itrial < ntrial; itrial++) {
			double *pft = pd_fea + (itrial * nfilter + fi) * nch;
			for (int ich = 0; ich < nch; ich++) {
				memcpy(pd_line, pd_eeg + ((itrial * nch + ich) * winlen), winlen * sizeof(double));
				filter.FiltFilt(pd_line, winlen);
				pft[ich] = CSignalProc::GetVariance(pd_line, winlen);
				pft[ich] = log(pft[ich]);
			}
		}
	}

	// clear cnt file
	for (int i = 0; i < ncnt; i++) delete cnt_list[i];

	// select features
	int m = FeaSel_NBPW(pd_fea, pi_cls, ntrial, ft_len, pi_sf, pi_CO, pd_eeg);
	if (m <= 0) return false;

	int na = 0;
	for (int itrial = 0; itrial < ntrial; itrial++) {
		if (pi_CO[itrial] == pi_cls[itrial]) na++;
	}
	printf("Feature selection accuracy = %f.\n", na * 100.0 / ntrial);

	double *ps_mean = pd_eeg; // m
	double *ps_std = ps_mean + m; // m
	double net_b = 0;
	double *net_z = ps_std + m; // m * m

	double *ps_fea = net_z + m * m; // ntrial x m
	double *ps_score = ps_fea + ntrial * m; // ntrial

	// 2-fold cross validation ==> change to n fold, n = 10.
	// Copy selected features
	int nfolder = 10;
	if (nfolder > ntrial) nfolder = ntrial;
	printf("%d folder crossvalidation over %d samples...\n", nfolder, ntrial);
	int cv_true = 0;

	for (int ifolder = 0; ifolder < nfolder; ifolder++) {
		int tstart = ntrial * ifolder / nfolder;
		int tend = ntrial * (ifolder + 1) / nfolder;
		printf("folder %d: %d - %d.\n", ifolder, tstart, tend);

		// copy selected feature only
		// 1. copy training features
		pdata = ps_fea;
		pd_eeg = pd_fea; // borrow this var
		int li = 0;
		for (int itrial = 0; itrial < ntrial; itrial++, pd_eeg += ft_len) {
			// skip testing samples
			if (itrial >= tstart && itrial < tend) continue;

			// copy training samples
			for (int mi = 0; mi < m; mi++) {
				pdata[mi] = pd_eeg[pi_sf[mi]];
			}
			pdata += m;

			// copy training class label
			pj_cls[li++] = pi_cls[itrial];
		}

		// 2. copy testing data
		pd_eeg = pd_fea + ft_len * tstart;
		for (int itrial = tstart; itrial < tend; itrial++) {
			// copy testing samples
			for (int mi = 0; mi < m; mi++) {
				pdata[mi] = pd_eeg[pi_sf[mi]];
			}
			pdata += m;
			pd_eeg += ft_len;

			// copy testing class label
			pj_cls[li++] = pi_cls[itrial];
		}

		// Training
		nsample = ntrial - (tend - tstart);
		CSignalProc::Standize(ps_fea, nsample, m, ps_mean, ps_std);
		na = ModTrain_FLDT(ps_fea, pj_cls, nsample, m, &net_b, net_z, pi_CO, ps_score,
			ps_score + ntrial);
		printf("fold %d: Training completed. Accuracy = %.2f\n", ifolder, na * 100.0 / nsample);

		// Testing
		pdata = ps_fea + nsample * m;
		for (int itrial = nsample; itrial < ntrial; itrial++ ) {
			for (int mi = 0; mi < m; mi++) {
				*pdata -= ps_mean[mi];
				*pdata /= ps_std[mi];
				pdata++;
			}
		}

		na = ModTest_FLDT(ps_fea + nsample * m, pj_cls + nsample, ntrial - nsample, m, net_b, net_z,
			pi_CO + nsample, ps_score + nsample);
		printf("\t Testing accuracy = %.2f.\n", na * 100.0 / (ntrial - nsample));

		cv_true += na;
	}

	printf("CV accuracy = %.2f\n", cv_true * 100.0 / ntrial);

	// Rebuild ps_fea
	pdata = ps_fea;
	pd_eeg = pd_fea; // borrow this var
	for (int itrial = 0; itrial < ntrial; itrial++) {
		for (int mi = 0; mi < m; mi++) {
			pdata[mi] = pd_eeg[pi_sf[mi]];
		}
		pdata += m;
		pd_eeg += ft_len;
	}

	CSignalProc::Standize(ps_fea, ntrial, m, ps_mean, ps_std);
	na = ModTrain_FLDT(ps_fea, pi_cls, ntrial, m, &net_b, net_z, pi_CO, ps_score,
		ps_score + ntrial);
	printf("Training completed. Accuracy = %f\n", na * 100.0 / ntrial);

	//
	// Questions:
	//	1. Instead of computing from cross-validation result, calculate from this model?
	//	2. Reverse Score before calculating?
	//
	double *mean_score = ps_score + ntrial;
	double *std_score = mean_score + 2;

	double *p0_score = std_score + 2; // ncls0

	for (int itrial = 0, c0 = 0; itrial < ntrial; itrial++) {
		double score = - (ps_score[itrial] - net_b);
		// CCWANG: Different from MATLAB - FLDT: cls0: < bias, cls1: > bias. Here reverse

		int ci = pi_cls[itrial];

		if (ci == 0) {
			p0_score[c0] = score;
			c0++;
		}

		mean_score[ci] += score;
	}

	mean_score[0] /= ncls0;
	mean_score[1] /= ncls1;

	std_score[0] = std_score[1] = 0;
	for (int itrial = 0; itrial < ntrial; itrial++) {
		double score = - ps_score[itrial];
		int ci = pi_cls[itrial];
		double dv = score - mean_score[ci];
		std_score[ci] += dv * dv;
	}

	if (ncls0 > 2) std_score[0] /= (ncls0 - 1);
	if (ncls1 > 2) std_score[1] /= (ncls1 - 1);
	std_score[0] = sqrt(std_score[0]);
	std_score[1] = sqrt(std_score[1]);

	double tscore = p0_score[0];
	// sort p0_score
	if (ncls0 > 1) {
		int istart = 0;
		int iend = ncls0 - 1;
		while (istart < iend) {
			for (int c0 = 0; c0 < iend; c0++) {
				if (p0_score[c0 + 1] < p0_score[c0]) {
					double dv = p0_score[c0];
					p0_score[c0] = p0_score[c0 + 1];
					p0_score[c0 + 1] = dv;
				}
			}
			istart++;

			for (int c0 = iend; c0 > istart; c0--) {
				if (p0_score[c0] < p0_score[c0 - 1]) {
					double dv = p0_score[c0];
					p0_score[c0] = p0_score[c0 - 1];
					p0_score[c0 - 1] = dv;
				}
			}
			iend--;
		}

		istart = (int)((1 - tScorePt) * ncls0 - 1);
		if (istart < 0) istart = 0;
		tscore = p0_score[istart];
	}

	// output model file - hex format
	if (fopen_s(&fp, fn_model, "wt")) {
		printf("Cannot open config file %s!\n", fn_trcfg);
	} else {
		fprintf(fp, "Sampling Rate: %d\n", cntHeader.sample_rate);
		fprintf(fp, "EEG_Resolution: %.10lf\n", cntHeader.resolution);
		fprintf(fp, "Channel_Order: %d\n", nch);
		for (int ich = 0; ich < nch; ich++) {
			if (ich > 0) fprintf(fp, ", ");
			if (chan_list) fprintf(fp, chan_list[ich]);
			else fprintf(fp, "CH%d", ich + 1);
		}
		fprintf(fp, "\n");

		fprintf(fp, "Window Length (Seconds): %f\n", tWinLen);

		fprintf(fp, "Model_M: %d\n", 0); // no csp for 2 channels

		fprintf(fp, "Model_F: %d\n", m);
		for (int mi = 0; mi < m; mi++) {
			if (mi != 0) fprintf(fp, ", ");
			fprintf(fp, "%d", pi_sf[mi] + 1);
		}
		fprintf(fp, "\n");

		fprintf(fp, "Classification: FLD\n");

		//Output the net data of FLD
		fprintf(fp, "FLD_FeatureMean: %d\n", m);
		for (int mi = 0; mi < m; mi++) {
			if (mi > 0) fprintf(fp, ", ");
			fprintf(fp,"%I64X", ps_mean[mi]);
		}
		fprintf(fp, "\n");

		fprintf(fp,"FLD_FeatureSTD: %d\n", m);
		for (int mi = 0; mi < m; mi++) {
			if (mi > 0) fprintf(fp, ", ");
			fprintf(fp,"%I64X", ps_std[mi]);
		}
		fprintf(fp, "\n");

		fprintf(fp,"FLD_LinearTransformVector: %d\n", m);
		for (int mi = 0; mi < m; mi++) {
			if (mi > 0) fprintf(fp, ", ");
			fprintf(fp, "%I64X", net_z[mi * m]);
		}
		fprintf(fp, "\n");

		fprintf(fp, "FLD_Bias: %I64X\n", net_b);

		fprintf(fp, "Empirical Scores Mean: %I64X,%I64X\n",
			mean_score[0], mean_score[1]);
		fprintf(fp, "Empirical Scores STD: %I64X,%I64X\n",
			std_score[0], std_score[1]);
		fprintf(fp, "Empirical Threshold Score: %I64X\n", tscore);
		fprintf(fp, "Empirical Zero Score: %I64X\n", (mean_score[0] + mean_score[1]) / 2);

		fclose(fp);
	}

	// output model file - normal
	strcpy_s(line, fn_model);
	par = line + strlen(line);
	while (par > line && *par != '\\' && *par != '/' && *par != '.') par--;
	if (*par == '.') *par = 0;
	strcat_s(line, "_G.txt");

	if (fopen_s(&fp, line, "wt")) {
		printf("Cannot open config file %s!\n", fn_trcfg);
	} else {
		fprintf(fp, "Sampling Rate: %d\n", cntHeader.sample_rate);
		fprintf(fp, "EEG_Resolution: %.10lf\n", cntHeader.resolution);
		fprintf(fp, "Channel_Order: %d\n", nch);
		for (int ich = 0; ich < nch; ich++) {
			if (ich > 0) fprintf(fp, ", ");
			if (chan_list) fprintf(fp, chan_list[ich]);
			else fprintf(fp, "CH%d", ich + 1);
		}
		fprintf(fp, "\n");

		fprintf(fp, "Window Length (Seconds): %f\n", tWinLen);

		fprintf(fp, "Model_M: %d\n", 0); // no csp for 2 channels

		fprintf(fp, "Model_F: %d\n", m);
		for (int mi = 0; mi < m; mi++) {
			if (mi != 0) fprintf(fp, ", ");
			fprintf(fp, "%d", pi_sf[mi] + 1);
		}
		fprintf(fp, "\n");

		fprintf(fp, "Classification: FLD\n");

		//Output the net data of FLD
		fprintf(fp, "FLD_FeatureMean: %d\n", m);
		for (int mi = 0; mi < m; mi++) {
			if (mi > 0) fprintf(fp, ", ");
			fprintf(fp,"%lf", ps_mean[mi]);
		}
		fprintf(fp, "\n");

		fprintf(fp,"FLD_FeatureSTD: %d\n", m);
		for (int mi = 0; mi < m; mi++) {
			if (mi > 0) fprintf(fp, ", ");
			fprintf(fp,"%lf", ps_std[mi]);
		}
		fprintf(fp, "\n");

		fprintf(fp,"FLD_LinearTransformVector: %d\n", m);
		for (int mi = 0; mi < m; mi++) {
			if (mi > 0) fprintf(fp, ", ");
			fprintf(fp, "%lf", net_z[mi * m]);
		}
		fprintf(fp, "\n");

		fprintf(fp, "FLD_Bias: %lf\n", net_b);

		fprintf(fp, "Empirical Scores Mean: %lf,%lf\n",
			mean_score[0], mean_score[1]);
		fprintf(fp, "Empirical Scores STD: %lf,%lf\n",
			std_score[0], std_score[1]);
		fprintf(fp, "Empirical Threshold Score: %lf\n", tscore);
		fprintf(fp, "Empirical Zero Score: %lf\n", (mean_score[0] + mean_score[1]) / 2);

		fclose(fp);
	}

	delete [] pd_fea;
	delete [] pi_cls;

	return true;
}
