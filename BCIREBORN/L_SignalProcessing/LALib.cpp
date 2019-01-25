#include ".\lalib.h"
#include "malloc.h"
#include "math.h"
#include "LIMITS.H"
#include "FLOAT.H"

CLALib::CLALib(void)
{
}

CLALib::~CLALib(void)
{
}

int CLALib::GaussSolution(double a[], double b[], int n)
{
	int *js,l,k,i,j,is,p,q;
	double d,t;
	js=(int*)malloc(n*sizeof(int));
	l=1;
	for (k=0;k<=n-2;k++)
	{
		d=0.0;
		for (i=k;i<=n-1;i++)
			for (j=k;j<=n-1;j++)
			{ t=fabs(a[i*n+j]);
			if (t>d) { d=t; js[k]=j; is=i;}
			}
			if (d+1.0==1.0) l=0;
			else
			{ if (js[k]!=k)
			for (i=0;i<=n-1;i++)
			{ p=i*n+k; q=i*n+js[k];
			t=a[p]; a[p]=a[q]; a[q]=t;
			}
			if (is!=k){
			for (j=k;j<=n-1;j++){
				p=k*n+j; q=is*n+j;
				t=a[p]; a[p]=a[q]; a[q]=t;
			}
			t=b[k]; b[k]=b[is]; b[is]=t;
			}
		}
		if (l==0) {free(js);return(0);}
		d=a[k*n+k];
		for (j=k+1;j<=n-1;j++)
		{p=k*n+j; a[p]=a[p]/d;}
		b[k]=b[k]/d;
		for (i=k+1;i<=n-1;i++){
			for (j=k+1;j<=n-1;j++){
				p=i*n+j;
				a[p]=a[p]-a[i*n+k]*a[k*n+j];
			}
			b[i]=b[i]-a[i*n+k]*b[k];
		}
	}
	d=a[(n-1)*n+n-1];
	if (fabs(d)+1.0==1.0)
	{ free(js);
	return(0);
	}
	b[n-1]=b[n-1]/d;
	for (i=n-2;i>=0;i--)
	{ t=0.0;
	for (j=i+1;j<=n-1;j++)
		t=t+a[i*n+j]*b[j];
	b[i]=b[i]-t;
	}
	js[n-1]=n-1;
	for (k=n-1;k>=0;k--){
		if (js[k]!=k)
		{ t=b[k]; b[k]=b[js[k]]; b[js[k]]=t;}
	}
	free(js);
	return(1);
}

double CLALib::FindMaxMinInArray(double* pArray,int iLen,bool bFindMax,int* pIdx)
{
	double fMax=FLT_MIN,fMin=FLT_MAX;
	int iIdxMax,iIdxMin;
	for(int i=0;i<iLen;i++){
		if(pArray[i]>fMax){ fMax=pArray[i];iIdxMax=i;}
		if(pArray[i]<fMin){ fMin=pArray[i];iIdxMin=i;}
	}
	if(bFindMax){
		if(0 != pIdx) *pIdx=iIdxMax;
		return fMax;
	}

	if(0 != pIdx) *pIdx=iIdxMin;
	return fMin;

}

float CLALib::FindMaxMinInArray(float* pArray,int iLen,bool bFindMax,int* pIdx)
{
	float fMax=FLT_MIN,fMin=FLT_MAX;
	int iIdxMax,iIdxMin;
	for(int i=0;i<iLen;i++){
		if(pArray[i]>fMax){ fMax=pArray[i];iIdxMax=i;}
		if(pArray[i]<fMin){ fMin=pArray[i];iIdxMin=i;}
	}
	if(bFindMax){
		if (0 != pIdx) *pIdx=iIdxMax;
		return fMax;
	}

	if(0 != pIdx) *pIdx=iIdxMin;
	return fMin;

}

int CLALib::BubbleSort(double* pArray,int iLen, bool bAscend)
{
	double tmp;
	for(int i=0;i<iLen;i++)
	{
		for(int x=0; x<iLen-1-i; x++)
		{
			tmp=pArray[x]-pArray[x+1];
			if( (tmp>0 && bAscend) || (tmp<0 && !bAscend))
			{
				tmp = pArray[x];
				pArray[x] = pArray[x+1];
				pArray[x+1] = tmp;
			}
		}

	}
	return 1;
}

double CLALib::Sum(double* pArray,int iLen)
{
	double fSum=0.0f;
	for(int i=0;i<iLen;i++)
		fSum+=pArray[i];
	return fSum;
}

int CLALib::EstGaussParas(double* pData,int iNumSample,double& fMean,double& fStd)
{
	int i;
  	for(i=0;i<iNumSample;i++) fMean+=pData[i];
	fMean=fMean/iNumSample;
	for(i=0;i<iNumSample;i++) fStd+=(pData[i]-fMean)*(pData[i]-fMean);
	fStd=sqrt(fStd/(iNumSample-1));
	return 1;
}