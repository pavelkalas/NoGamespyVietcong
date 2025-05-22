// dllmain.cpp : Definuje vstupní bod pro aplikaci knihovny DLL.
#include "pch.h"

BOOL APIENTRY DllMain(HMODULE hModule, DWORD reason, LPVOID lpReserved) {
	if (reason == DLL_PROCESS_ATTACH) {
		HMODULE logsDLL = GetModuleHandleA("logs.dll");
		if (logsDLL) {
			uintptr_t cnsAddTxtAddr = (uintptr_t)GetProcAddress(logsDLL, "?CNS_AddTxt@@YAXPAD@Z");
			if (cnsAddTxtAddr) {
				auto fn = ((void(__cdecl*)(const char*))cnsAddTxtAddr);
				fn("");
				fn("> NoGamespyVietcong v1.0 by Floxen - https://github.com/pavelkalas/nogamespyvietcong");
				fn("");
			}
		}
	}

	return TRUE;
}
