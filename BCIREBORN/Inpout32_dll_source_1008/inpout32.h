#pragma once

//Functions exported from DLL.
//For easy inclusion is user projects.
void	_stdcall Out32(int PortAddress, int data);
int	_stdcall Inp32(int PortAddress);

BOOL	_stdcall IsInpOutDriverOpen();

UCHAR   _stdcall DlPortReadPortUchar (ULONG port);
void    _stdcall DlPortWritePortUchar(ULONG port, UCHAR Value);
/*
USHORT  _stdcall DlPortReadPortUshort (ULONG port)
void    _stdcall DlPortWritePortUshort(ULONG port, USHORT Value)

ULONG	_stdcall DlPortReadPortUlong(ULONG port)
void	_stdcall DlPortReadPortUlong(ULONG port, ULONG Value);
*/
