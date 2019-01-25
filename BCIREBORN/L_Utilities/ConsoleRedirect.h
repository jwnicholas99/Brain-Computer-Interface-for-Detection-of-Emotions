#pragma once

#include <windows.h>
#include <io.h>
#include <stdio.h>

int ConsoleRedirect(HANDLE hf0);
void ConsoleReset();
int GetSavedOutFd();
int GetSavedErrFd();
