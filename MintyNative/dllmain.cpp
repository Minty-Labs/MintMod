// dllmain.cpp : Defines the entry point for the DLL application.
#include "pch.h"

BOOL APIENTRY DllMain( HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved ) {
    switch (ul_reason_for_call) {
		case DLL_PROCESS_ATTACH:
		case DLL_THREAD_ATTACH:
		case DLL_THREAD_DETACH:
		case DLL_PROCESS_DETACH:
			break;
    }
    return TRUE;
}

#include <Windows.h>
#include <iostream>
#include <io.h>
#include <cstdio>
#include <cstdlib>
#include <cstdint>
#define KEY 0x55
template <unsigned int N>
struct obfuscator
{
    char m_data[N] = { 0 };

    constexpr obfuscator(const char* data) {
        for (unsigned int i = 0; i < N; i++) {
            m_data[i] = data[i] ^ KEY;
        }
    }

    void deobfoscate(unsigned char* des) const {
        int i = 0;
        do {
            des[i] = m_data[i] ^ KEY;
            i++;
        } while (des[i - 1]);
    }
};

extern "C" {
    __declspec(dllexport) float add(float a, float b) { return a + b; }
    __declspec(dllexport) float subtract(float a, float b) { return a - b; }
    __declspec(dllexport) float multiply(float a, float b) { return a * b; }
    __declspec(dllexport) float divide(float a, float b) { return a / b; }
    __declspec(dllexport) float gravity(float g, float m1, float m2, float r) { return g * (m1 * m2 / (r * r)); }

    /*LPCWSTR TFGPT = L"The FitnessGram™ Pacer Test is a multistage aerobic capacity test that progressively gets more difficult as it continues.\nThe 20 - meter pacer test will begin in 30 seconds.Line up at the start. The running speed starts slowly but gets faster each minute after you hear this signal. A single lap should be completed each time you hear this sound. Remember to run in a straight line, and run as long as possible. The second time you fail to complete a lap before the sound, your test is over. The test will begin on the word start. On your mark, get ready, start.";
    __declspec(dllexport) void TestCommand() {
        MessageBox(0, TFGPT, L"Elly is an absolute cutie~", MB_ICONINFORMATION);
    }*/


    BOOLEAN has_run_mod_check;
    void __cdecl IsBadMod(const char* mod, const char* author) {
        obfuscator<256> bad_mods[] = {
            obfuscator<256>("RipperStore"),
            obfuscator<256>("Unchained"),
            obfuscator<256>("Late Night"),
            obfuscator<256>("Late_Night"),
            obfuscator<256>("A.R.E.S"),
            obfuscator<256>("A.R.3.S"),
            obfuscator<256>("Abyss"),
            obfuscator<256>("AbyssLoader"),
            obfuscator<256>("Versa")
        };

        obfuscator<256> bad_authors[] = {
            obfuscator<256>("xAstroBoy"),
            obfuscator<256>("PatchedPlus"),
            obfuscator<256>("LargestBoi"),
            obfuscator<256>("kaaku"),
            obfuscator<256>("L4rg3stBo1"),
            obfuscator<256>("_Unreal"),
            obfuscator<256>("bunny"),
            obfuscator<256>("Stellar"),
            obfuscator<256>("Lady Lucy")
        };

        std::string modStr(mod);
        std::string authorStr(author);

        for (const auto& bad : bad_mods) {
            if (modStr.find(bad) != std::string::npos) {
                //MessageBoxA(0, (MAKE_STRING("Remove \"") + modStr + MAKE_STRING("\" from your Mods directory.")).c_str(), MAKE_STRING("Forbidden Mod detected."), MB_ICONERROR);
                MessageBoxA(0, (obfuscator<256>("Remove \"") + obfuscator<256>(modStr)), "", MB_ICONERROR);
                KillGame();
            }
        }
        for (const auto& bad : bad_authors) {
            if (authorStr.find(bad) != std::string::npos) {
                MessageBoxA(0, (MAKE_STRING("Remove the mod(s) by \"") + authorStr + MAKE_STRING("\" from your Mods directory.")).c_str(), MAKE_STRING("Forbidden Author detected."), MB_ICONERROR);
                KillGame();
            }
        }
        has_run_mod_check = true;
    }

    void __cdecl KillGame() {

    }

    typedef struct image_t {
        int ref_count;
        void* raw_data_handle;
        char* raw_data;
        uint32_t raw_data_len;
        uint8_t raw_buffer_used : 1;
        uint8_t raw_data_allocated : 1;
        uint8_t fileio_used : 1;
#ifdef _WIN32
        uint8_t is_module_handle : 1;
        uint8_t has_entry_point : 1;
#endif
    } image_t;

    __declspec(dllexport) void yeet(uintptr_t img) {
        image_t* image = (image_t*)img;
        image->raw_data_handle = NULL;
        image->raw_data = NULL;
        image->raw_data_len = 0;
    }
}