
#ifndef LPC_h
#define LPC_h

#define MAXFEATURES  64		/* Max components in a feature vector */
#define MAXFRAMESIZE   1024	//
#define MAXLPCORDER		100		// LPC order 12   

#define TPI  6.2831853     /* P*2 */
#define LogZERO  (-1.0E10)   /* ~log(0) */

//---------------------------------
#define	DefaultFs			250				//default sampling rate 250
#define	DefaultFRAMEDUR		200				//default frame duration msec
#define DefaultFRAMEMOVE	100				/* frame period in msecs	  */

#define	DefaultFRAMESIZE	(DefaultFRAMEDUR*DefaultFs/1000)	//frame size
#define DefaultFRAMESHIFT	(DefaultFRAMEMOVE*DefaultFs/1000)  // frame shift
#define DefaultLPCORDER		12

#define PREEMPH		0.97	/* pre-emphasis coef		  */


class LPCCEP
{
public:

	LPCCEP();
    ~LPCCEP();

	void Reset();

	void ProcessFrame (float *pfInData, int iDataLength, float *pFeatBuffer, int iLPCOrder);
	bool LPCconversion(float *buff, float *OutFeature);

	int	GetFrameShift() { return miFrameRate;}
	int	GetFrameLength() {return miFrameSize;}

    int GetNumFrame() { return miTotalFrames;}
    float *GetFrameFeature(int iFrameIndex);

protected:

	float  m_sBuff[MAXFRAMESIZE+1]; 
	float m_aBuff[MAXLPCORDER+1]; 

	float mNewA[MAXFEATURES+1];   //* New LP filter coefficients     
	float mAuto[MAXFEATURES+1];   //* AutoCorrelation Sequence       

	bool	mHamSet;
	float	mSigmaT2;	//for delta cep
	int miLPCOrder;		/* Order of parametric analysis   */
	int miCEPOrder;		/* Actual number of parameters used  */
	int miCepLift;		/* cepstral lifting coef	  */

	/* global data  */
	int miFrameSize;	  /* num speech samples per input frame  */
	int miNumComps;	  /* total number of components per output vector */
	int miFrameRate;   /* frame period in input samples  */
	int miTotalFrames; /* in data  */

/* --------------- Windowing and PreEmphasis ---------------------*/
	float mpHamWin[MAXFRAMESIZE];   /* Current Hamming window */


protected:

	void SetDefault();
	void Initialize();

	void PreEmphasise (float * s);
	void HamWin(float * s);

	void AutoCorrelation ( float * s, float r[]);
	void WaveToLPC(float * s, float *re, float *te);
	void GenHamWindow();

};


#endif