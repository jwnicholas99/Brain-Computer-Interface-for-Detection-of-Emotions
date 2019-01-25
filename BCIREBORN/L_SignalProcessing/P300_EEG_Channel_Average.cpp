#include "EEGContFile.h"
#include "../L_Utilities/Resources.h"

#include "P300SignalProc.h"
#include "Filtering.h"

bool CP300SignalProc::EEG_ChannelAverage(const char *train_text)
{
	FILE *fp = NULL;
	fopen_s(&fp, train_text, "rt");

	if (fp == NULL) {
		printf("Cannot read file %s\n", train_text);
		return false;
	}
	char strbuf[1024];
	char *pc = NULL;
	while (fgets(strbuf, 1024, fp) != NULL) {
		pc = strbuf;
		while (isspace(*pc)) pc++;
		if (strncmp(pc, "TrainDir:", 9) == 0) {
			pc += 9;
			while (isspace(*pc))pc++;
			break;
		}
		pc = NULL;
	}
	fclose(fp);

	if (pc == NULL) {
		printf("Invalid training file %s.", train_text);
		return false;
	}

	char file_path[1024] = "";
	strcpy_s(file_path, pc);
	pc = file_path + strlen(pc);
	while (isspace(*(pc-1))) pc--;
	*pc-- = 0;
	if (*pc != '\\' && *pc != '/') strcat_s(file_path, "\\");

	// looking for system.cfg
	// Config/system.cfg in current directory takes precedence, next by file in the 
	// training directory.

	// looking for system.cfg
	// 1. Config/system.cfg in the training directory.
	// 2. current directory
	pc = "Config\\System.cfg";
	strcpy_s(strbuf, file_path);
	strcat_s(strbuf, pc);
	if (_access(strbuf, 0) == -1) {
		if (_access(pc, 0) == -1) {
			printf("Cannot find system configuration file.\n");
			return false;
		}
		strcpy_s(strbuf, pc);
	}
	pc = strbuf;
	printf("Configuration file: %s\n", pc);

	// Read configuration file
	Resources res;
	if (res.Merge(pc) != 0) {
		printf("get_avg_eeg: Cannot read config file.\n");
		return false;
	}

	char *rn;
	char *par;

    CEEGContFile train_eeg;
	if (!train_eeg.LoadDataFileList(train_text)) {
		printf("get_avg_eeg: Failed Loading Data FileList %s.\n", train_text);
		return false;
	}

	int num_epoch_per_round = 0;
	rn = "EEG";
	par = "NumEpochPerRound";
	if (res.Get(rn, par, &num_epoch_per_round) != 0) {
		printf("Read configuration file: cannot read %s/%s.\n", rn, par);
		return false;
	}

	int num_round_per_target = 0;
	par = "NumRound";
	if (res.Get(rn, par, &num_round_per_target) != 0) {
		printf("Read configuration file: cannot read %s/%s.\n", rn, par);
		fclose(fp);
		return false;
	}

	int sampling_rate = 0;
	par = "SamplingRate";
	if (res.Get(rn, par, &sampling_rate) != 0) {
		//printf("Read configuration file: cannot read %s/%s.\n", rn, par);
		//fclose(fp);
		//return false;
		sampling_rate = train_eeg.m_CNTHeader.iSamplingRate;
	}

	int num_samples = 0;
	int durtime = 0;
	par = "PostStimDuration";
	if (res.Get(rn, par, &durtime) != 0) {
		//printf("Read configuration file: cannot read %s/%s.\n", rn, par);
		//fclose(fp);
		//return false;
		durtime = 1000;
	}
	num_samples = durtime * sampling_rate / 1000;

	int num_chanels = train_eeg.m_CNTHeader.iNumCh;
	char channel_names[1024];
	char *chan_name[256];
	//int chan_used[256];

	memset(chan_name, 0, sizeof(chan_name));
	par = "ChannelNames";
	if (res.Get(rn, par, channel_names, sizeof(channel_names)) != 0) {
		//printf("Read configuration file: cannot read %s/%s.\n", rn, par);
		//fclose(fp);
		//return false;
		int k = 0;
		char *pch = channel_names;
		int sz = sizeof(channel_names);
		for (int i = 0; i < num_chanels; i++) {
			sprintf_s(pch, sz, "ch%d", i + 1);
			chan_name[i] = pch;
			int n = strlen(pch) + 1;
			pch += n;
			sz -= n;
		}
	} else {
		char *pc = channel_names, *pc0;
		int k = 0;

		while (isspace(*pc)) pc++;
		pc0 = pc;
		while (*pc0 != 0 && k < num_chanels) {	
			if (*pc == ',' || *pc == 0) {
				if (pc0 < pc) {
					chan_name[k++] = pc0;
					pc0 = pc;
					while (isspace(*--pc)) *pc = 0;
					if (*pc0 != 0) {
						*pc0 = 0;
						while (isspace(*++pc0));
					}
					pc = pc0;
				}
			} else {
				pc++;
			}
		}

		if (num_chanels != k) {
			printf("Number of channels different from actually read! (%d/%d)\n", num_chanels, k);
			num_chanels = k;
		}
	}

	rn = "P300EEGSignal";
	par = "SpellerChars";
	char *speller_chars = NULL;
	int num_chars = 0;
	if (res.Get(rn, par, strbuf, sizeof(strbuf)) != 0) {
		printf("Read configuration file: cannot read %s/%s.\n", rn, par);
		fclose(fp);
		return false;
	} else {
		int n = 0;
		char *tmp = strbuf;
		for (int i = 0; tmp[i]; i++) {
			if (tmp[i] == ',' && (i == 0 || tmp[i-1] != '\\')) n++;
		}
		n++;

		speller_chars = new char[n];
		memset(speller_chars, 0, n);

		int i0 = 0;
		int k = 0;
		for (int i = 0; k < n; i++)	{
			if (tmp[i] == 0 || (tmp[i] == ',' && (i == 0 || tmp[i-1] != '\\'))) {
				tmp[i] = 0;
				while (isspace(tmp[i0])) i0++;
				if (tmp[i0] == '\\') {
					i0++;
					if (isdigit(tmp[i0])) speller_chars[k] = atoi(tmp + i0);
					else speller_chars[k] = tmp[i0];
				} else {
					speller_chars[k] = tmp[i0];
				}
				i0 = i + 1;
				k++;
			}
		}
		num_chars = k;
	}

	rn = "ModelTrainingSetting";
	int start_time = 0, end_time = 500;
	par = "StartProcTimeAftStim";
	res.Get(rn, par, &start_time);

	par = "EndProcTimeAftStim";
	res.Get(rn, par, &end_time);

	// ccwang: show alll time figure
	end_time = durtime;

	int start_sample = start_time * sampling_rate / 1000;
	int end_sample = end_time * sampling_rate / 1000;
	if (end_sample > num_samples) end_sample = num_samples;
	
	//Filter Design
	double B[24] = {0.008343314888274, -0.025989255084205, 0.046399602997059, -0.055119125703279, 0.057973023502140, -0.055119125703279, 0.046399602997059, -0.025989255084205, 0.008343314888274};
	double A[24] = {1.000000000000000, -5.098304723303556, 11.763518682887524, -15.910528136121988, 13.737722522445742, -7.729528046147417, 2.761636568733868, -0.571786472847546, 0.052511702051212};
	int n = 9;

	CFiltering filter;

	// read from configuration file
	par = "Filter_A";
	if (res.Get(rn, par, strbuf, sizeof(strbuf)) == 0) {
		n = ReadDataFromString(strbuf, A, 24);
		if (n <= 0) return false;

		par = "Filter_B";
		if (res.Get(rn, par, strbuf, sizeof(strbuf)) != 0) {
			printf("Error in read Fileb_B\n");
			fclose(fp);
			return false;
		}

		if (n != ReadDataFromString(strbuf, B, 24)) {
			printf("Confict A and B!\n");
			fclose(fp);
			return false;
		}
	}

	printf("Fileter: %d\n", n);
	printf("A:");
	for (int i = 0; i < n; i++) {
		printf("%G ", A[i]);
	}
	printf("\n");
	printf("B:");
	for (int i = 0; i < n; i++) {
		printf("%G ", B[i]);
	}
	printf("\n");

	filter.InitB(n);
	for (int i = 0; i < n; i++) filter.SetBPara(B[i],i);	
	filter.InitA(n);
	for (int i=0; i < n; i++) filter.SetAPara(A[i],i);

	// Check signal range?
	int check_range = 1;
	rn = "P300Training";
	par = "CheckRange";
	if (res.Get(rn, par, &check_range)) {
		printf("CheckRange not defined.\n");
	}

	float range_low = 1.0f, range_high = 200.0f;
	res.Get(rn, "RangeLow", &range_low);
	res.Get(rn, "RangeHigh", &range_high);

	par = "AverageNTAll";
	int avg_non_tar_all = 1;
	res.Get(rn, par, &avg_non_tar_all);

	///----------Check the number of Epoches----------------
	if (num_epoch_per_round * num_round_per_target * train_eeg.m_iNumTarget != 
		train_eeg.m_iNumAllEpoch)
	{
		printf("get_avg_eeg: The expected number of epoches does not agree with that in the data file list. Abort.\n");
		printf("NumEpochPerRound=%d,NumRoundPerTarget=%d,NumTarget=%d, AllEpoch=%d.\n",
			num_epoch_per_round, num_round_per_target, train_eeg.m_iNumTarget, train_eeg.m_iNumAllEpoch);
		return false;
	}

	strcpy_s(strbuf, file_path);
	strcat_s(strbuf, "chan_avg_eeg.ace");
	printf("Output result to %s.\n", strbuf);
	fp = NULL;
	fopen_s(&fp, strbuf, "wb");
	if (!fp) {
		printf("Cannot open output file.\n");
		return false;
	}

	int** ppTargetEpoch=NULL;
	int** ppNonTargetEpoch=NULL;
	int* pTargetEpochCode=NULL;
	int* pNonTargetEpochCode=NULL;

	//Seperate Epochs
	int iNumTargetEpoch, iNumNonTargetEpoch;
	iNumTargetEpoch = train_eeg.GetTargetEpochPtrs(num_epoch_per_round, num_round_per_target,
		ppTargetEpoch, pTargetEpochCode, speller_chars, num_chars);
	iNumNonTargetEpoch = train_eeg.GetNonTargetEpochPtrs(num_epoch_per_round, num_round_per_target,
		ppNonTargetEpoch, pNonTargetEpochCode, speller_chars, num_chars);

	float *pInSegment = new float[num_samples];
	double *pAvgData = new double[num_samples + num_samples];
	int spl_sz = num_chanels + 1;
	int channels = 0;
	double MaxVal = 0., MinVal = 0.;

	fwrite(&num_chanels, 4, 1, fp);
	fwrite(&num_samples, 4, 1, fp);
	fwrite(&MaxVal, 8, 1, fp);
	fwrite(&MinVal, 8, 1, fp);
	fwrite(&start_time, 4, 1, fp);
	fwrite(&end_time, 4, 1, fp);

	srand((unsigned)time(NULL));

	for (int iCh = 0; iCh < num_chanels; iCh++)
	{
		// if (chan_used[iCh] == 0) continue;

		// target data
		for (int iSpl = 0; iSpl < num_samples + num_samples; iSpl++) {
			pAvgData[iSpl] = 0;
		}

		for (int iEpoch = 0; iEpoch < iNumTargetEpoch; iEpoch++) {
			int *pe = ppTargetEpoch[iEpoch] + iCh;
			if (train_eeg.m_CNTHeader.fResolution > 0) {
				for (int iSpl = 0; iSpl < num_samples; iSpl++) {
					pInSegment[iSpl] = (float) (*pe) * train_eeg.m_CNTHeader.fResolution;
					pe += spl_sz;
				}
			} else {
				for (int iSpl = 0; iSpl < num_samples; iSpl++) {
					memcpy(pInSegment + iSpl, pe, 4);
					pe += spl_sz;
				}
			}

			filter.Process(pInSegment, num_samples);

			double fmean = 0;
			for (int iSpl = start_sample; iSpl < end_sample; iSpl++) {
				fmean += pInSegment[iSpl];
			}
			fmean /= end_sample - start_sample;

			for (int iSpl = 0; iSpl < num_samples; iSpl++) {
				pAvgData[iSpl] += pInSegment[iSpl] - (float) fmean;
			}
		}

		for (int iSpl = 0; iSpl < num_samples; iSpl++) {
			pAvgData[iSpl] /= iNumTargetEpoch;
		}

		// nontarget data
		memset(pAvgData, 0, sizeof(pAvgData));
		int avg_sum = 0;
		int avt_stp = iNumNonTargetEpoch / iNumTargetEpoch;
		if (avg_non_tar_all) avt_stp = 1;

		for (int iEpoch = 0; iEpoch < iNumNonTargetEpoch; iEpoch += avt_stp) {

			int sel_epoch = iEpoch;
			if (avt_stp > 1) {
				sel_epoch += rand() * (avt_stp - 1) / RAND_MAX;
			}

			avg_sum++;
			int *pe = ppNonTargetEpoch[sel_epoch] + iCh; //iEpoch
			for (int iSpl = 0; iSpl < num_samples; iSpl++) {
				pInSegment[iSpl] = (float) (*pe) * train_eeg.m_CNTHeader.fResolution;
				pe += spl_sz;
			}

			filter.Process(pInSegment, num_samples);

			double fmean = 0;
			for (int iSpl = start_sample; iSpl < end_sample; iSpl++) {
				fmean += pInSegment[iSpl];
			}
			fmean /= (end_sample - start_sample);

			for (int iSpl = 0; iSpl < num_samples; iSpl++) {
				pAvgData[iSpl + num_samples] += pInSegment[iSpl] - (float)fmean;
			}
		}

		for (int iSpl = 0; iSpl < num_samples; iSpl++) {
			pAvgData[iSpl + num_samples] /= avg_sum; // iNumNonTargetEpoch;
		}

		double CMax, CMin;
		CMax = CMin = pAvgData[start_sample];
		for (int iSpl = start_sample + 1; iSpl < end_sample; iSpl++) {
			if (CMax < pAvgData[iSpl]) CMax = pAvgData[iSpl];
			if (CMin > pAvgData[iSpl]) CMin = pAvgData[iSpl];
		}
		for (int iSpl = num_samples + start_sample + 1; iSpl < num_samples + end_sample; iSpl++) {
			if (CMax < pAvgData[iSpl]) CMax = pAvgData[iSpl];
			if (CMin > pAvgData[iSpl]) CMin = pAvgData[iSpl];
		}

		printf("Channel :%s, range:(%lf, %lf), %lf \n", chan_name[iCh], CMax, CMin, CMax - CMin);
		double r = CMax - CMin;
		if (check_range && (r < range_low || r >= range_high)) {
			printf("\tChannel discarded.\n");
			continue;
		}

		int len = (int) strlen(chan_name[iCh]);
		fwrite(&len, 4, 1, fp);
		fwrite(chan_name[iCh], 1, len, fp);

		fwrite(&CMax, 8, 1, fp);
		fwrite(&CMin, 8, 1, fp);
		fwrite(pAvgData + start_sample, sizeof(double), end_sample - start_sample, fp);
		fwrite(pAvgData + num_samples + start_sample, sizeof(double), end_sample - start_sample, fp);

		if (iCh == 0) {
			MaxVal = CMax;
			MinVal = CMin;
		} else {
			if (MaxVal < CMax) MaxVal = CMax;
			if (MinVal > CMin) MinVal = CMin;
		}

		channels++;
	}

	fseek(fp, 0, 0);
	fwrite(&channels, 4, 1, fp);
	num_samples = end_sample - start_sample;
	fwrite(&num_samples, 4, 1, fp);
	fwrite(&MaxVal, 8, 1, fp);
	fwrite(&MinVal, 8, 1, fp);
	fclose(fp);

	//Clear allocated memory
	if (pInSegment != NULL) delete [] pInSegment;
	if (pAvgData != NULL) delete [] pAvgData;
	if (ppTargetEpoch != NULL) delete ppTargetEpoch;
	if (ppNonTargetEpoch != NULL) delete ppNonTargetEpoch;
	if (pTargetEpochCode != NULL) delete pTargetEpochCode;
	if (pNonTargetEpochCode != NULL) delete pNonTargetEpochCode;
	if (speller_chars != NULL) delete [] speller_chars;

	return true;
}