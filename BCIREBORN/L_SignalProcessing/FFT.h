// Filename : RealFFT.h
// 

#ifndef INC_fft_mayer
#define INC_fft_mayer


#ifdef REAL
#else
#define REAL float
#endif

void fht(REAL *fz,  int n);
void ifft(int n, float *real, float *imag);
void realfft(int n, float *real);
void fft(int n, float *real, float *imag);
void realifft(int n, float *real);
#endif
