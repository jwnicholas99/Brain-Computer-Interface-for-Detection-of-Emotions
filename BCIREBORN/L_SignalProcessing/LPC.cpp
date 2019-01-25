/* LPCCEP.cpp is the converter from wave(16bit 8Khz) into LPCCepstrum*/
#include<stdio.h>
#include<stdlib.h>
#include<string.h>
#include<math.h>

#include "LPC.h"

/*-----------------------------------------------------------------
Usage:
	1. Initialize:
   		m_pLPCCEP =  new LPCCEP();
	2. lpc
		m_pLPCCEP->ProcessFrame ((short *) speech_buffer);

-------------------------------------------------------------------*/
LPCCEP :: LPCCEP()
{
	SetDefault();
    Initialize();
}

//-------------------------------------------------------------------------------
// Set defaiult values
// input: 
// ouput: 
// return:
//-------------------------------------------------------------------------------
void LPCCEP :: SetDefault()
{
	miLPCOrder = DefaultLPCORDER;		

	miFrameSize = DefaultFRAMESIZE;
	miFrameRate = DefaultFRAMESHIFT;

	mHamSet = false;
}

//-------------------------------------------------------------------------------
// Initialize values
// input: 
// ouput: 
// return:
//-------------------------------------------------------------------------------
void LPCCEP :: Initialize()
{
	GenHamWindow();

}


//-------------------------------------------------------------------------------
// Reset counters for each utterance
// input: 
// ouput: 
// return:
//-------------------------------------------------------------------------------
void LPCCEP :: Reset()
{
}


LPCCEP :: ~LPCCEP()
{
}


//-------------------------------------------------------------------------------
// Routine for processing each new frame
// input: speech data, one frame
// ouput: static LPC-CEP feature, stored in mFeatureBuffer
// return:
//-------------------------------------------------------------------------------
void LPCCEP :: ProcessFrame(float *pfInData, int iDataLength, float *pFeatBuffer, int iLPCOrder)
{
	miFrameSize = iDataLength;
	miLPCOrder = iLPCOrder;		

	if(LPCconversion(pfInData, pFeatBuffer)==false)
		return;

}

//-------------------------------------------------------------------------------
// Material rortine to cal LPC-CEP based on levison-durbin algorithm
// input: speech data, one frame
// ouput: LPC-CEP 
// return: true, false
//-------------------------------------------------------------------------------
bool LPCCEP :: LPCconversion( float *pfInBuffer, float *OutFeature)
{
	float ResiEng, SpchEng;
	float *pBuff= &m_sBuff[1];
	for (int i=0;i<miFrameSize;i++)
		*pBuff++ = pfInBuffer[i];

	WaveToLPC(m_sBuff, &ResiEng,&SpchEng);
	if(SpchEng <=0)
		return false;

	OutFeature[0]=1;
	for (int i = 1; i<=miLPCOrder; i++) OutFeature[i] = m_aBuff[i];
	OutFeature[miLPCOrder+1]=ResiEng;	//last dim for residusl energy

	return true;
}

//-------------------------------------------------------------------------------
// Pre-emphasis
// input: raw speech
// ouput: filtered speech
// return:
//-------------------------------------------------------------------------------
void LPCCEP :: PreEmphasise (float * s)
{
	for (int i=miFrameSize;i>=2;i--)
		s[i] -= s[i-1]*(float) PREEMPH;
	s[1] *= (float)(1.0-PREEMPH);

}


//-------------------------------------------------------------------------------
// Calculate LPCoef in m_aBuff
// input: speech
// ouput: autoregressive coef, reflection coef, residual energy, signal energy
// return:
//-------------------------------------------------------------------------------
void LPCCEP :: WaveToLPC(float * s, float *re, float *te)
{
	float ki;			//* Current Reflection Coefficient 
	float E;				//* Prediction Error		   
	int i, j;

	AutoCorrelation(s,mAuto);

	E = mAuto[0];
	m_aBuff[1] = 0;
	*te = E;
	for(i=1; i<=miLPCOrder; i++){
		ki = mAuto[i];	//* Calc next reflection coef  
		for(j=1; j<i; j++)
			ki = ki + m_aBuff[j] * mAuto[i-j];

		ki = ki / E;

		E *= 1- ki*ki;    //* Update Error 
		mNewA[i] = -ki;
		for(j=1; j<i; j++)
			mNewA[j] = m_aBuff[j] - ki * m_aBuff[i-j];
      
		for(j=1; j<=i; j++)
			m_aBuff[j] = mNewA[j];
	}

	*re = (float)E;
}

//-------------------------------------------------------------------------------
// Calculate autocorrelation coef 0 to p in r 
// input: speech
// ouput: autocorrelation coef
// return:
//-------------------------------------------------------------------------------
void LPCCEP :: AutoCorrelation ( float * s, float r[])
{
	float sum;
	int k;
	for(int i=0;i<=miLPCOrder;i++){
		sum = 0;
		k= miFrameSize -i;
		for(int j=1; j<=k; j++)
			sum += s[j]*s[j+i];
		r[i] = sum + 1;	//+1 for protection
	}
}


//-------------------------------------------------------------------------------
// Generate hamming window
// input: 
// ouput: hamming window
// return:
//-------------------------------------------------------------------------------
void LPCCEP :: GenHamWindow()
{
	int i;
	float a;
	a = (float)(TPI / (miFrameSize - 1));
	for(i=0; i< miFrameSize; i++)
		mpHamWin[i] = (float)(0.54 - 0.46 * cos(a*i));

}


