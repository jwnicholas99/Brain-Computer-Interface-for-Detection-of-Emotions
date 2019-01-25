#pragma unmanaged

#include "StdAfx.h"
#include "inpout32.h"
//void	_stdcall Out32(short PortAddress, short data);
//short	_stdcall Inp32(short PortAddress);
//
//BOOL	_stdcall IsInpOutDriverOpen();
//
//UCHAR   _stdcall DlPortReadPortUchar (ULONG port);
//void    _stdcall DlPortWritePortUchar(ULONG port, UCHAR Value);

#pragma managed

namespace BCILib {
	namespace Util {
		public ref class InpOut32 {

		public:
			static InpOut32();

			static void Out(int address, int data)
			{
				Out32(address, data);
			}

			static void Out(int data)
			{
				Out32(0x378, data);
			}

			static int Inp(int address)
			{
				return Inp32(address);
			}

			static int Inp()
			{
				return Inp32(0x378);
			}
		};
	}
}

static BCILib::Util::InpOut32::InpOut32()
{
	BOOL bResult = IsInpOutDriverOpen();
	if (!bResult)
		MessageBox(NULL, "Unable to install or open the\nInpOut driver HWInterface.sys.\n\nPlease try running as Administrator", "InpOut Installation", 0);
}