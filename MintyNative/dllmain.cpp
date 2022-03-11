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
extern "C" {
    __declspec(dllexport) float add(float a, float b) { return a + b; }
    __declspec(dllexport) float subtract(float a, float b) { return a - b; }
    __declspec(dllexport) float multiply(float a, float b) { return a * b; }
    __declspec(dllexport) float divide(float a, float b) { return a / b; }
    __declspec(dllexport) float gravity(float g, float m1, float m2, float r) { return g * ((m1 * m2) / (r * r)); }

    LPCWSTR TFGPT = L"The FitnessGram™ Pacer Test is a multistage aerobic capacity test that progressively gets more difficult as it continues.\nThe 20 - meter pacer test will begin in 30 seconds.Line up at the start. The running speed starts slowly but gets faster each minute after you hear this signal. A single lap should be completed each time you hear this sound. Remember to run in a straight line, and run as long as possible. The second time you fail to complete a lap before the sound, your test is over. The test will begin on the word start. On your mark, get ready, start.";
    __declspec(dllexport) void TestCommand() {
        MessageBox(0, TFGPT, L"Elly is an absolute cutie~", MB_ICONINFORMATION);
    }
}