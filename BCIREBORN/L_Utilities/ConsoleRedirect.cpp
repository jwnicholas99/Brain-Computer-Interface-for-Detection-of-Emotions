#include "ConsoleRedirect.h"

#include <fcntl.h>

int _wcpy = -1;
int _ecpy = -1;

int ConsoleRedirect(HANDLE hf0)
{
	int fd = _open_osfhandle((intptr_t)hf0, _O_APPEND| _O_TEXT);
	if (fd == -1) {
		printf("redirect_out_handle: error!\n");
		return 1;
	}

	fflush(stdout);
	fflush(stderr);

	int fout = _fileno(stdout);
	int ferr = _fileno(stderr);
	if (fout < 0) {
		FILE *fp = NULL;
		int i = 1;
		char fn[256];
		while (fp == NULL) {
			sprintf_s(fn, "redirectout%d.log", i);
			freopen_s(&fp, fn, "at", stdout);
			i++;
		}
		fout = _fileno(stdout);
		//printf("Redirect stdout to %s\n", fn);
	}
	if (ferr < 0) {
		FILE *fp = NULL;
		int i = 1;
		char fn[256];
		while (fp == NULL) {
			sprintf_s(fn, "redirecterr%d.log", i);
			freopen_s(&fp, fn, "at", stderr);
			i++;
		}
		ferr = _fileno(stderr);
		//fprintf(stderr, "Redirect stderr to %s\n", fn);
	}

	if (_wcpy == -1) {
		_wcpy = _dup(fout);
	}
	_dup2(fd, fout);
	fflush(stdout);
	printf("ConsoleRedirect.cpp: stdout %X/%d\n", stdout, fout);

	if (_ecpy == -1) {
		_ecpy = _dup(ferr);
	}
	_dup2(fd, ferr);
	fflush(stderr);
	fprintf(stderr, "ConsoleRedirect.cpp: stderr %X/%d\n", stderr, ferr);

	//setvbuf(stdout, NULL, _IOLBF, 512);
	//setvbuf(stderr, NULL, _IOLBF, 512);

	return 0;
}

void ConsoleReset()
{
	int fout = _fileno(stdout);
	int ferr = _fileno(stderr);

	if (_wcpy != -1) {
		_dup2(_wcpy, fout);
		_close(_wcpy);
		_wcpy = -1;
	}

	if (_ecpy != -1) {
		_dup2(_ecpy, ferr);
		_close(_ecpy);
		_ecpy = -1;
	}
}

int GetSavedOutFd()
{
	return _wcpy;
}

int GetSavedErrFd()
{
	return _ecpy;
}
