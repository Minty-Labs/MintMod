﻿using System;
using MelonLoader;
using System.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using MintyLoader;

namespace MintyLoader {
    public class MonkeKiller {
        internal static class NativeImports {
            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);
        }
        
        internal static void BlacklistedModCheck() {
            if (MelonHandler.Mods.Any(m => m.Info.Name.Contains("RipperStore")) ||
                 MelonHandler.Mods.Any(m => m.Info.Author.Contains("xAstroBoy")) ||
                 MelonHandler.Mods.Any(m => m.Info.Author.Contains("PatchedPlus")) ||
                 MelonHandler.Mods.Any(m => m.Info.Name.Contains("Unchained")) ||
                 MelonHandler.Mods.Any(m => m.Info.Name.Contains("Late Night")) ||
                 MelonHandler.Mods.Any(m => m.Info.Author.Contains("LargestBoi")) ||
                 MelonHandler.Mods.Any(m => m.Info.Name.Contains("A.R.E.S")) ||
                MelonHandler.Mods.Any(m => m.Info.Name.Contains("ARES")) ||
                MelonHandler.Mods.Any(m => m.Info.Name.Contains("FreeLoading")))
            {
                try {
                    NativeImports.TerminateProcess(Process.GetCurrentProcess().Handle, 0);
                    Marshal.GetDelegateForFunctionPointer<Action>(Marshal.AllocHGlobal(16))();
                    while (true) Thread.Sleep(1000);
                }
                catch (Exception) {
                    NativeImports.TerminateProcess(Process.GetCurrentProcess().Handle, 0);
                    Marshal.GetDelegateForFunctionPointer<Action>(Marshal.AllocHGlobal(16))();
                    while (true) Thread.Sleep(1000);
                }
                finally {
                    NativeImports.TerminateProcess(Process.GetCurrentProcess().Handle, 0);
                    Marshal.GetDelegateForFunctionPointer<Action>(Marshal.AllocHGlobal(16))();
                    while (true) Thread.Sleep(1000);
                }
            }
            if (MintyLoader.isDebug)
                MintyLoader.Instance.LoggerInstance.Msg("You are not using any blacklisted mods.");
        }
    }
}