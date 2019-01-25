// Public FTP code for fft routines
// Not very good for ifft, theres a buf somewhere

#define GOOD_TRIG
#include "TrigTable.h"
#include "FFT.h"

char fht_version[] = "Brcwl-Hrtly-Ron-dbld";

#define SQRT2_2   0.70710678118654752440084436210484
#define SQRT2     1.41421356237
//#define SQRT2   2*0.70710678118654752440084436210484

void fht(REAL *fz,  int n)
{
 REAL a;
 REAL c1,s1,s2,c2,s3,c3,s4,c4;
 REAL f0,g0,f1,g1,f2,g2,f3,g3;
 int i,k,k1,k2,k3,k4,kx;
 REAL *fi,*fn,*gi;
 int t_lam = 0;
 // TRIG_VARS;

 REAL coswrk[20] = 
    {
     (float) .00000000000000000000000000000000000000000000000000,
     (float) .70710678118654752440084436210484903928483593768847,
     (float) .92387953251128675612818318939678828682241662586364,
     (float) .98078528040323044912618223613423903697393373089333,
     (float) .99518472667219688624483695310947992157547486872985,
     (float) .99879545620517239271477160475910069444320361470461,
     (float) .99969881869620422011576564966617219685006108125772,
     (float) .99992470183914454092164649119638322435060646880221,
     (float) .99998117528260114265699043772856771617391725094433,
     (float) .99999529380957617151158012570011989955298763362218,
     (float) .99999882345170190992902571017152601904826792288976,
     (float) .99999970586288221916022821773876567711626389934930,
     (float) .99999992646571785114473148070738785694820115568892,
     (float) .99999998161642929380834691540290971450507605124278,
     (float) .99999999540410731289097193313960614895889430318945,
     (float) .99999999885102682756267330779455410840053741619428
    };

 REAL sinwrk[20] =
    {
     1.0000000000000000000000000000000000000000000000000,
     (float) .70710678118654752440084436210484903928483593768846,
     (float) .38268343236508977172845998403039886676134456248561,
     (float) .19509032201612826784828486847702224092769161775195,
     (float) .09801714032956060199419556388864184586113667316749,
     (float) .04906767432741801425495497694268265831474536302574,
     (float) .02454122852291228803173452945928292506546611923944,
     (float) .01227153828571992607940826195100321214037231959176,
     (float) .00613588464915447535964023459037258091705788631738,
     (float) .00306795676296597627014536549091984251894461021344,
     (float) .00153398018628476561230369715026407907995486457522,
     (float) .00076699031874270452693856835794857664314091945205,
     (float) .00038349518757139558907246168118138126339502603495,
     (float) .00019174759731070330743990956198900093346887403385,
     (float) .00009587379909597734587051721097647635118706561284,
     (float) .00004793689960306688454900399049465887274686668768
    };

 for (k1=1,k2=0;k1<n;k1++)
    {
      // REAL a;
     for (k=n>>1; (!((k2^=k)&k)); k>>=1);
     if (k1>k2)
	{
	     a=fz[k1];fz[k1]=fz[k2];fz[k2]=a;
	}
    }
 for ( k=0 ; (1<<k)<n ; k++ );
 k  &= 1;
 if (k==0)
    {
	 for (fi=fz,fn=fz+n;fi<fn;fi+=4)
	   {
	     // REAL f0,f1,f2,f3;
	     f1     = fi[0 ]-fi[1 ];
	     f0     = fi[0 ]+fi[1 ];
	     f3     = fi[2 ]-fi[3 ];
	     f2     = fi[2 ]+fi[3 ];
	     fi[2 ] = (f0-f2);	
	     fi[0 ] = (f0+f2);
	     fi[3 ] = (f1-f3);	
	     fi[1 ] = (f1+f3);
	    }
    }
 else
    {
	 for (fi=fz,fn=fz+n,gi=fi+1;fi<fn;fi+=8,gi+=8)
	    {
	      // REAL s1,c1,s2,c2,s3,c3,s4,c4,g0,f0,f1,g1,f2,g2,f3,g3;
	     c1     = fi[0 ] - gi[0 ];
	     s1     = fi[0 ] + gi[0 ];
	     c2     = fi[2 ] - gi[2 ];
	     s2     = fi[2 ] + gi[2 ];
	     c3     = fi[4 ] - gi[4 ];
	     s3     = fi[4 ] + gi[4 ];
	     c4     = fi[6 ] - gi[6 ];
	     s4     = fi[6 ] + gi[6 ];
	     f1     = (s1 - s2);	
	     f0     = (s1 + s2);
	     g1     = (c1 - c2);	
	     g0     = (c1 + c2);
	     f3     = (s3 - s4);	
	     f2     = (s3 + s4);
	     g3     = (float) (SQRT2*c4);		
	     g2     = (float) (SQRT2*c3);
	     fi[4 ] = f0 - f2;
	     fi[0 ] = f0 + f2;
	     fi[6 ] = f1 - f3;
	     fi[2 ] = f1 + f3;
	     gi[4 ] = g0 - g2;
	     gi[0 ] = g0 + g2;
	     gi[6 ] = g1 - g3;
	     gi[2 ] = g1 + g3;
	    }
    }
 if (n<16) return;

 do
    {
     REAL s1,c1;
     k  += 2;
     k1  = 1  << k;
     k2  = k1 << 1;
     k4  = k2 << 1;
     k3  = k2 + k1;
     kx  = k1 >> 1;
	 fi  = fz;
	 gi  = fi + kx;
	 fn  = fz + n;
	 do
	    {
	     REAL g0,f0,f1,g1,f2,g2,f3,g3;
	     f1      = fi[0 ] - fi[k1];
	     f0      = fi[0 ] + fi[k1];
	     f3      = fi[k2] - fi[k3];
	     f2      = fi[k2] + fi[k3];
	     fi[k2]  = f0	  - f2;
	     fi[0 ]  = f0	  + f2;
	     fi[k3]  = f1	  - f3;
	     fi[k1]  = f1	  + f3;
	     g1      = gi[0 ] - gi[k1];
	     g0      = gi[0 ] + gi[k1];
	     g3      = (float) (SQRT2  * gi[k3]);
	     g2      = (float) (SQRT2  * gi[k2]);
	     gi[k2]  = g0	  - g2;
	     gi[0 ]  = g0	  + g2;
	     gi[k3]  = g1	  - g3;
	     gi[k1]  = g1	  + g3;
	     gi     += k4;
	     fi     += k4;
	    } while (fi<fn);

// Add by Ralph
      int  iii;
      for (iii=2 ; iii<=k ; iii++)	
          {coswrk[iii]=costab[iii];sinwrk[iii]=sintab[iii];}		
      t_lam = 0;				
      c1 = 1;					
      s1 = 0;					
// Add by Ralph
//   TRIG_INIT(k,c1,s1);
     for (i=1;i<kx;i++)
        {
	 REAL c2,s2;

     {	
	     int ii,jj;	                    
         (t_lam)++;	  					
         for (ii=0 ; !((1<<ii)&t_lam) ; ii++);	
         ii = k-ii;					
         s1 = sinwrk[ii];			
         c1 = coswrk[ii];			
         if (ii>1)   				
         {	    				
             for (jj=k-ii+2 ; (1<<jj)&t_lam ; jj++);	
             jj	       = k - jj;					
             sinwrk[ii] = halsec[ii] * (sinwrk[ii-1] + sinwrk[jj]);
             coswrk[ii] = halsec[ii] * (coswrk[ii-1] + coswrk[jj]); 
         }                                                    
     }


         c2 = c1*c1 - s1*s1;
         s2 = 2*(c1*s1);
	     fn = fz + n;
	     fi = fz +i;
	     gi = fz +k1-i;
	     do
		{
		 REAL a,b,g0,f0,f1,g1,f2,g2,f3,g3;
		 b       = s2*fi[k1] - c2*gi[k1];
		 a       = c2*fi[k1] + s2*gi[k1];
		 f1      = fi[0 ]    - a;
		 f0      = fi[0 ]    + a;
		 g1      = gi[0 ]    - b;
		 g0      = gi[0 ]    + b;
		 b       = s2*fi[k3] - c2*gi[k3];
		 a       = c2*fi[k3] + s2*gi[k3];
		 f3      = fi[k2]    - a;
		 f2      = fi[k2]    + a;
		 g3      = gi[k2]    - b;
		 g2      = gi[k2]    + b;
		 b       = s1*f2     - c1*g3;
		 a       = c1*f2     + s1*g3;
		 fi[k2]  = f0        - a;
		 fi[0 ]  = f0        + a;
		 gi[k3]  = g1        - b;
		 gi[k1]  = g1        + b;
		 b       = c1*g2     - s1*f3;
		 a       = s1*g2     + c1*f3;
		 gi[k2]  = g0        - a;
		 gi[0 ]  = g0        + a;
		 fi[k3]  = f1        - b;
		 fi[k1]  = f1        + b;
		 gi     += k4;
		 fi     += k4;
		} while (fi<fn);
        }
    } while (k4<n);
}


void ifft(int n, float *real, float *imag)
{
 float a,b,c,d;
 float q,r,s,t;
 int i,j,k;
 fht(real,n);
 fht(imag,n);
 for (i=1,j=n-1,k=n/2;i<k;i++,j--) {
  a = real[i]; b = real[j];  q=a+b; r=a-b;
  c = imag[i]; d = imag[j];  s=c+d; t=c-d;
  imag[i] = (float) ((s+r)*0.5);  imag[j] = (float) ((s-r)*0.5);
  real[i] = (float) ((q-t)*0.5);  real[j] = (float) ((q+t)*0.5);
 }
}



void realfft(int n, float *real)
{
 float a,b;
 int i,j,k;
 fht(real,n);
 for (i=1,j=n-1,k=n/2;i<k;i++,j--) {
  a = real[i];
  b = real[j];
  real[j] = (float) ((a-b)*0.5);
  real[i] = (float) ((a+b)*0.5);
 }
}

void fft(int n, float *real, float *imag)
{
 float a,b,c,d;
 float q,r,s,t;
 int i,j,k;
 for (i=1,j=n-1,k=n/2;i<k;i++,j--) {
  a = real[i]; b = real[j];  q=a+b; r=a-b;
  c = imag[i]; d = imag[j];  s=c+d; t=c-d;
  real[i] = (float) ((q+t)*.5); real[j] = (float) ((q-t)*.5);
  imag[i] = (float) ((s-r)*.5); imag[j] = (float) ((s+r)*.5);
 }
 fht(real,n);
 fht(imag,n);
}

void realifft(int n, float *real)
{
 float a,b;
 int i,j,k;
 for (i=1,j=n-1,k=n/2;i<k;i++,j--) {
  a = real[i];
  b = real[j];
  real[j] = (a-b);
  real[i] = (a+b);
 }
 fht(real,n);
}





