// dllmain.cpp : Definuje vstupní bod pro aplikaci knihovny DLL.
#include "pch.h"
#include <thread>
#include <string>

/// <summary>
/// Struktura příkazu
/// </summary>
struct s_CNS_command {
	int commandId;
	void* unknown1;
	void* unknown2;
	const char* commandName;
	int needAdmin;
	int argCount;
};

/// <summary>
/// Pointer na funkci pro výpis dat do konzole hry
/// </summary>
typedef void(__cdecl* CNS_AddTxt_)(const char*);
CNS_AddTxt_ CNS_AddTxt;

/// <summary>
/// Pointer na funkci pro přidání příkazu do bufferu hry
/// </summary>
typedef void(__cdecl* CNS_AddCommand_)(s_CNS_command*);
CNS_AddCommand_ CNS_AddCommand;

/// <summary>
/// Vytvoří struct pro ignorování příkazu
/// </summary>
/// <param name="name">Jméno příkazu</param>
/// <returns>Navrací struct</returns>
s_CNS_command IgnoreCommand(const char* name) {
	s_CNS_command cmd = { 0 };
	cmd.commandId = 100;
	cmd.unknown1 = nullptr;
	cmd.unknown2 = nullptr;
	cmd.commandName = name;
	cmd.needAdmin = 0;
	cmd.argCount = 0;
	return cmd;
}

void RenderInfoToConsole() {
	CNS_AddTxt("");
	CNS_AddTxt("> NoGamespyVietcong v1.02 by Floxen - https://github.com/pavelkalas/nogamespyvietcong");
	CNS_AddTxt("");
	CNS_AddTxt(" v1.0:");
	CNS_AddTxt("   Nyni by jste uz meli opet videt Vietcong servery ve hre");
	CNS_AddTxt("   mozna bude potreba parkrat poklikat na tlacitko Obnovit");
	CNS_AddTxt("");
	CNS_AddTxt(" v1.01");
	CNS_AddTxt("   Opraven problem s vyskakujicim dialogem firewallu pri ");
	CNS_AddTxt("   kazdem spusteni hry ");
	CNS_AddTxt("");
	CNS_AddTxt("  V pripade jakychkoliv problemu mi nevahejte napsat na ");
	CNS_AddTxt("  discordu pod jmenem 'swipesznx6.' i s teckou na konci");
	CNS_AddTxt("");
}

/// <summary>
/// Funkce co sleduje zadané příkazy
/// </summary>
/// <param name="logsDLL"></param>
void WatchForCommandEntry(HMODULE logsDLL) {
	uintptr_t lastCommandStringPtr = (uintptr_t)logsDLL + 0x36B4FC;
	char* lastCommandString = (char*)lastCommandStringPtr;

	CNS_AddCommand(&IgnoreCommand("info"));
	CNS_AddCommand(&IgnoreCommand("version"));

	strcpy_s(lastCommandString, 128, "nothing");

	while (true) {
		std::string currentCommand = std::string(lastCommandString);

		if (currentCommand != "nothing") {
			strcpy_s(lastCommandString, 128, "nothing");

			if (currentCommand == "info") {
				RenderInfoToConsole();
			}

			else if (currentCommand == "giftfrompterodon") {
				CNS_AddTxt("STOP CHEATING! xD");
			}

			else if (currentCommand == "version") {
				CNS_AddTxt("NoGamespyVietcong v1.02 active - by Floxen");
				CNS_AddTxt("");
			}
		}

		Sleep(50);
	}
}

BOOL APIENTRY DllMain(HMODULE hModule, DWORD reason, LPVOID lpReserved) {
	if (reason == DLL_PROCESS_ATTACH) {
		HMODULE logsDLL = GetModuleHandleA("logs.dll");
		if (logsDLL) {
			CNS_AddTxt = (CNS_AddTxt_)GetProcAddress(logsDLL, "?CNS_AddTxt@@YAXPAD@Z");
			CNS_AddCommand = (CNS_AddCommand_)GetProcAddress(logsDLL, "?CNS_AddCommand@@YAXPAUs_CNS_command@@@Z");

			if (CNS_AddTxt && CNS_AddCommand) {
				std::thread commandWatcher(WatchForCommandEntry, logsDLL);
				commandWatcher.detach();
				RenderInfoToConsole();
			}
		}
	}

	return TRUE;
}
