// inpout32drv.cpp : Defines the entry point for the DLL application.
//

#include "stdafx.h"
#include "hwinterfacedrv.h"
#include "resource.h"
#include <conio.h>
#include <stdlib.h>
#include <stdio.h>

int inst32();
int inst64();
int start(LPCTSTR pszDriver);

//First, lets set the DRIVERNAME depending on our configuraiton.
#define DRIVERNAMEx64 "Inpoutx64"
#define DRIVERNAMEi386 "Inpout32"

char str[10];
int vv;

HANDLE hdriver=NULL;
char path[MAX_PATH];
HINSTANCE hmodule;
SECURITY_ATTRIBUTES sa;
int sysver;

int Opendriver(BOOL bX64);
void Closedriver(void);

BOOL APIENTRY DllMain( HINSTANCE  hModule, 
					  DWORD  ul_reason_for_call, 
					  LPVOID lpReserved
					  )
{

	hmodule = hModule;
	switch(ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
		sysver = SystemVersion();
		if(sysver==2)
		{
			Opendriver(IsXP64Bit());
		}
		break;
	case DLL_PROCESS_DETACH:
		if(sysver==2)
		{
			Closedriver();
		}
		break;
	}
	return TRUE;
}

/***********************************************************************/

void Closedriver(void)
{
	if (hdriver != NULL && hdriver != INVALID_HANDLE_VALUE)
	{
		OutputDebugString("Closing driver...\n");
		CloseHandle(hdriver);
		hdriver = NULL;
	}
}

void _stdcall Out32(int PortAddress, int data)
{

	switch(sysver)
	{
	case 1:
#ifdef _M_IX86
		_outp( PortAddress,data);	//Will ONLY compile on i386 architecture
#endif
		break;
	case 2:
		if (hdriver != NULL && hdriver != INVALID_HANDLE_VALUE) {
			unsigned int error;
			DWORD BytesReturned;        
			BYTE Buffer[3];
			unsigned short * pBuffer;
			pBuffer = (unsigned short *)&Buffer[0];
			*pBuffer = LOWORD(PortAddress);
			Buffer[2] = LOBYTE(data);

			error = DeviceIoControl(hdriver,
				IOCTL_WRITE_PORT_UCHAR,
				&Buffer,
				3,
				NULL,
				0,
				&BytesReturned,
				NULL);
		}
		break;
	}


}

/*********************************************************************/

int _stdcall Inp32(int PortAddress)
{
	BYTE retval(0);
	switch(sysver)
	{
	case 1:
#ifdef _M_IX86
		retval = _inp(PortAddress);
#endif
		return retval;
		break;
	case 2:
		if (hdriver != NULL && hdriver != INVALID_HANDLE_VALUE) {
			unsigned int error;
			DWORD BytesReturned;
			unsigned char Buffer[3];
			unsigned short * pBuffer;
			pBuffer = (unsigned short *)&Buffer;
			*pBuffer = LOWORD(PortAddress);
			Buffer[2] = 0;
			error = DeviceIoControl(hdriver,
				IOCTL_READ_PORT_UCHAR,
				&Buffer,
				2,
				&Buffer,
				1,
				&BytesReturned,
				NULL);

			if (error==0)
			{
				DWORD dwError = GetLastError();
				char szError[255];
				sprintf_s(szError, 255, "Error %d\n", dwError);
				OutputDebugString(szError);
			}
			return((int)Buffer[0]);
		}

		break;
	}
	return 0;
}

/*********************************************************************/

int Opendriver(BOOL bX64)
{
	OutputDebugString("Attempting to open InpOut driver...\n");

	char szFileName[MAX_PATH] = {"\\\\.\\"};
	if (bX64)
		strcat_s(szFileName, DRIVERNAMEx64);	//We are 64bit...
	else
		strcat_s(szFileName, DRIVERNAMEi386);  //We are 32bit...

	hdriver = CreateFile(szFileName, 
		GENERIC_READ | GENERIC_WRITE, 
		0, 
		NULL,
		OPEN_EXISTING, 
		FILE_ATTRIBUTE_NORMAL, 
		NULL);

	if(hdriver == INVALID_HANDLE_VALUE) 
	{
		if(start(bX64 ? DRIVERNAMEx64 : DRIVERNAMEi386))
		{
			if (bX64)
				inst64();	//Install the x64 driver
			else
				inst32();	//Install the i386 driver

			start(bX64 ? DRIVERNAMEx64 : DRIVERNAMEi386);

			hdriver = CreateFile(szFileName, 
				GENERIC_READ | GENERIC_WRITE, 
				0, 
				NULL,
				OPEN_EXISTING, 
				FILE_ATTRIBUTE_NORMAL, 
				NULL);

			if(hdriver != INVALID_HANDLE_VALUE) 
			{
				OutputDebugString("Successfully opened ");
				OutputDebugString(bX64 ? DRIVERNAMEx64 : DRIVERNAMEi386);
				OutputDebugString(" driver");
				return 0;
			}
		}
		return 1;
	}
	OutputDebugString("Successfully opened ");
	OutputDebugString(bX64 ? DRIVERNAMEx64 : DRIVERNAMEi386);
	OutputDebugString(" driver");
	return 0;
}

/***********************************************************************/
int inst32()
{
	char szDriverSys[MAX_PATH];
	strcpy_s(szDriverSys, DRIVERNAMEi386);
	strcat_s(szDriverSys, ".sys");
	
	SC_HANDLE  Mgr;
	SC_HANDLE  Ser;
	GetSystemDirectory(path , sizeof(path));
	HRSRC hResource = FindResource(hmodule, MAKEINTRESOURCE(IDR_INPOUT32), "bin");
	if(hResource)
	{
		HGLOBAL binGlob = LoadResource(hmodule, hResource);

		if(binGlob)
		{
			void *binData = LockResource(binGlob);

			if(binData)
			{
				HANDLE file;
				
				strcat_s(path, sizeof(path), "\\Drivers\\");
				strcat_s(path, sizeof(path), szDriverSys);
			
				file = CreateFile(path,
					GENERIC_WRITE,
					0,
					NULL,
					CREATE_ALWAYS,
					0,
					NULL);

				if (file)
				{
					if (file == INVALID_HANDLE_VALUE)
					{
						//We were unable to create the file - maybe we dont have permissions.


					}

					DWORD size, written;
					size = SizeofResource(hmodule, hResource);
					WriteFile(file, binData, size, &written, NULL);
					CloseHandle(file);
				}
			}
		}
	}

	Mgr = OpenSCManager (NULL, NULL,SC_MANAGER_ALL_ACCESS);
	if (Mgr == NULL)
	{							//No permission to create service
		if (GetLastError() == ERROR_ACCESS_DENIED) 
		{
			return 5;  // error access denied
		}
	}	
	else
	{
		char szFullPath[MAX_PATH] = "System32\\Drivers\\";
		strcat_s(szFullPath, MAX_PATH, szDriverSys);
		Ser = CreateService (Mgr,                      
			DRIVERNAMEi386,                        
			DRIVERNAMEi386,                        
			SERVICE_ALL_ACCESS,                
			SERVICE_KERNEL_DRIVER,             
			SERVICE_SYSTEM_START,               
			SERVICE_ERROR_NORMAL,               
			szFullPath,  
			NULL,                               
			NULL,                              
			NULL,                               
			NULL,                              
			NULL                               
			);
	}
	CloseServiceHandle(Ser);
	CloseServiceHandle(Mgr);

	return 0;
}

int inst64()
{
	char szDriverSys[MAX_PATH];
	strcpy_s(szDriverSys, DRIVERNAMEx64);
	strcat_s(szDriverSys, ".sys");
	
	SC_HANDLE  Mgr;
	SC_HANDLE  Ser;
	GetSystemDirectory(path , sizeof(path));
	HRSRC hResource = FindResource(hmodule, MAKEINTRESOURCE(IDR_INPOUTX64), "bin");
	if(hResource)
	{
		HGLOBAL binGlob = LoadResource(hmodule, hResource);

		if(binGlob)
		{
			void *binData = LockResource(binGlob);

			if(binData)
			{
				HANDLE file;
				strcat_s(path, sizeof(path), "\\Drivers\\");
				strcat_s(path, sizeof(path), szDriverSys);
			
				PVOID OldValue;
				DisableWOW64(&OldValue);
				file = CreateFile(path,
					GENERIC_WRITE,
					0,
					NULL,
					CREATE_ALWAYS,
					0,
					NULL);

				if(file)
				{
					DWORD size, written;

					size = SizeofResource(hmodule, hResource);
					WriteFile(file, binData, size, &written, NULL);
					CloseHandle(file);
				}
				RevertWOW64(&OldValue);
			}
		}
	}

	Mgr = OpenSCManager (NULL, NULL,SC_MANAGER_ALL_ACCESS);
	if (Mgr == NULL)
	{							//No permission to create service
		if (GetLastError() == ERROR_ACCESS_DENIED) 
		{
			return 5;  // error access denied
		}
	}	
	else
	{
		char szFullPath[MAX_PATH] = "System32\\Drivers\\";
		strcat_s(szFullPath, szDriverSys);
		Ser = CreateService (Mgr,                      
			DRIVERNAMEx64,                        
			DRIVERNAMEx64,                        
			SERVICE_ALL_ACCESS,                
			SERVICE_KERNEL_DRIVER,             
			SERVICE_SYSTEM_START,               
			SERVICE_ERROR_NORMAL,               
			szFullPath,  
			NULL,                               
			NULL,                              
			NULL,                               
			NULL,                              
			NULL                               
			);
	}
	if (Ser != NULL) CloseServiceHandle(Ser);
	if (Mgr != NULL) CloseServiceHandle(Mgr);

	return 0;
}

/**************************************************************************/
int start(LPCTSTR pszDriver)
{
	SC_HANDLE  Mgr = NULL;
	SC_HANDLE  Ser = NULL;

	Mgr = OpenSCManager (NULL, NULL,SC_MANAGER_ALL_ACCESS);

	if (Mgr == NULL)
	{							//No permission to create service
		if (GetLastError() == ERROR_ACCESS_DENIED) 
		{
			Mgr = OpenSCManager(NULL, NULL, GENERIC_READ);
			Ser = OpenService(Mgr,pszDriver, GENERIC_EXECUTE);
			if (Ser)
			{    // we have permission to start the service
				if (!StartService(Ser, 0, NULL))
				{
					CloseServiceHandle(Ser);
					return 4; // we could open the service but unable to start
				}
			}
		}
	}
	else
	{// Successfuly opened Service Manager with full access
		Ser = OpenService(Mgr,pszDriver,GENERIC_EXECUTE);
		if (Ser)
		{
			if(!StartService(Ser,0,NULL))
			{
				CloseServiceHandle (Ser);
				return 3; // opened the Service handle with full access permission, but unable to start
			}
			else
			{
				CloseServiceHandle (Ser);
				return 0;
			}
		}
	}
	return 1;
}

BOOL _stdcall IsInpOutDriverOpen()
{
	sysver = SystemVersion();
	if (sysver==2)
	{
		if (hdriver!=INVALID_HANDLE_VALUE && hdriver != NULL)
			return TRUE;
	}
	else if (sysver==1)
	{
		return TRUE;
	}
	return FALSE;
}

UCHAR _stdcall DlPortReadPortUchar (ULONG port)
{
	UCHAR retval(0);
	switch(sysver)
	{
	case 1:
#ifdef _M_IX86
		retval = _inp((USHORT)port);
#endif
		return retval;
		break;
	case 2:
		unsigned int error;
		DWORD BytesReturned;
		unsigned char Buffer[3]={NULL};
		unsigned short * pBuffer;
		pBuffer = (unsigned short *)&Buffer;
		*pBuffer = LOWORD(port);
		Buffer[2] = 0;
		error = DeviceIoControl(hdriver,
			IOCTL_READ_PORT_UCHAR,
			&Buffer,
			2,
			&Buffer,
			1,
			&BytesReturned,
			NULL);

		return((UCHAR)Buffer[0]);

		break;
	}
	return 0;
}

void _stdcall DlPortWritePortUchar (ULONG port, UCHAR Value)
{
	switch(sysver)
	{
	case 1:
#ifdef _M_IX86
		_outp((UINT)port,Value);	//Will ONLY compile on i386 architecture
#endif
		break;
	case 2:
		unsigned int error;
		DWORD BytesReturned;        
		BYTE Buffer[3]={NULL};
		unsigned short * pBuffer;
		pBuffer = (unsigned short *)&Buffer[0];
		*pBuffer = LOWORD(port);
		Buffer[2] = LOBYTE(Value);

		error = DeviceIoControl(hdriver,
			IOCTL_WRITE_PORT_UCHAR,
			&Buffer,
			3,
			NULL,
			0,
			&BytesReturned,
			NULL);
		break;
	}
}
/*
USHORT DlPortReadPortUshort (ULONG port)
{
	USHORT retval(0);
	switch(sysver)
	{
	case 1:
#ifdef _M_IX86
		retval = _inpw((USHORT)port);
#endif
		return retval;
		break;
	case 2:
		unsigned int error;
		DWORD BytesReturned;
		unsigned short Buffer[3]={NULL};
		unsigned short * pBuffer;
		pBuffer = (unsigned short *)&Buffer;
		*pBuffer = LOWORD(port);
		Buffer[2] = 0;
		error = DeviceIoControl(hdriver,
			IOCTL_READ_PORT_UCHAR,
			&Buffer,
			2,
			&Buffer,
			1,
			&BytesReturned,
			NULL);

		return((int)Buffer[0]);

		break;
	}
	return 0;
}

void DlPortWritePortUshort (ULONG port, USHORT Value)
{
	switch(sysver)
	{
	case 1:
#ifdef _M_IX86
		_outpw((UINT)port,Value);	//Will ONLY compile on i386 architecture
#endif
		break;
	case 2:
		unsigned int error;
		DWORD BytesReturned;        
		BYTE Buffer[3];
		unsigned short * pBuffer;
		pBuffer = (unsigned short *)&Buffer[0];
		*pBuffer = LOWORD(port);
		Buffer[2] = LOBYTE(Value);

		error = DeviceIoControl(hdriver,
			IOCTL_WRITE_PORT_UCHAR,
			&Buffer,
			3,
			NULL,
			0,
			&BytesReturned,
			NULL);
		break;
	}
}

ULONG DlPortReadPortUlong (ULONG port)
{
	ULONG retval(0);
	switch(sysver)
	{
	case 1:
#ifdef _M_IX86
		retval = _inpd((USHORT)port);
#endif
		return retval;
		break;
	case 2:
		unsigned int error;
		DWORD BytesReturned;
		unsigned long Buffer[3]={NULL};
		unsigned short * pBuffer;
		pBuffer = (unsigned short *)&Buffer;
		*pBuffer = LOWORD(port);
		Buffer[2] = 0;
		error = DeviceIoControl(hdriver,
			IOCTL_READ_PORT_UCHAR,
			&Buffer,
			2,
			&Buffer,
			1,
			&BytesReturned,
			NULL);

		return((int)Buffer[0]);

		break;
	}
	return 0;
}

void DlPortWritePortUlong (ULONG port, ULONG Value)
{
	switch(sysver)
	{
	case 1:
#ifdef _M_IX86
		_outpd((UINT)port,Value);	//Will ONLY compile on i386 architecture
#endif
		break;
	case 2:
		unsigned int error;
		DWORD BytesReturned;        
		BYTE Buffer[3];
		unsigned short * pBuffer;
		pBuffer = (unsigned short *)&Buffer[0];
		*pBuffer = LOWORD(port);
		Buffer[2] = LOBYTE(Value);

		error = DeviceIoControl(hdriver,
			IOCTL_WRITE_PORT_UCHAR,
			&Buffer,
			3,
			NULL,
			0,
			&BytesReturned,
			NULL);
		break;
	}
}
*/