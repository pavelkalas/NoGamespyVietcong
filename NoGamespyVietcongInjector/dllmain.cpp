// dllmain.cpp : Definuje vstupní bod pro aplikaci knihovny DLL.
#include "pch.h"

BOOL APIENTRY DllMain(HMODULE hModule, DWORD reason, LPVOID lpReserved) {
	if (reason == DLL_PROCESS_ATTACH) {
		HMODULE logsDLL = GetModuleHandleA("logs.dll");
		if (logsDLL) {
			uintptr_t cnsAddTxtAddr = (uintptr_t)GetProcAddress(logsDLL, "?CNS_AddTxt@@YAXPAD@Z");
			if (cnsAddTxtAddr) {
				auto CNS_AddTxt = ((void(__cdecl*)(const char*))cnsAddTxtAddr);
				CNS_AddTxt("");
				CNS_AddTxt("> NoGamespyVietcong v1.0 by Floxen - https://github.com/pavelkalas/nogamespyvietcong");
				CNS_AddTxt("");
				CNS_AddTxt(" v1.0:");
				CNS_AddTxt(" Nyni by jste uz meli opet videt Vietcong servery ve hre");
				CNS_AddTxt(" mozna bude potreba parkrat poklikat na tlacitko Obnovit");
				CNS_AddTxt("");
				CNS_AddTxt(" V pripade jakychkoliv problemu mi nevahejte napsat na ");
				CNS_AddTxt(" discordu pod jmenem 'swipesznx6.' i s teckou na konci");
				CNS_AddTxt("");
			}
		}
	}

	return TRUE;
}
