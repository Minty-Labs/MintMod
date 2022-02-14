using System;
using System.Collections.Generic;
using MelonLoader;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using MintyLoader;

namespace MintyLoader {
    public class MonkeKiller {
        internal static class NativeImports {
            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);
        }

        internal static string[] badMods = new[] { "ripperstore", "unchained", "late night", "late_night", "a.r.e.s", "a.r.3.s", "ares", "snaxytag", "unchained", 
            "abyss", "versa", "notorious", "error", "3rror", "fumo", "pasted client", "munchen" };
        internal static string[] badAuthors = new[] { "largestboi", "xastroboy", "patchedplus", "kaaku", "l4rg3stbo1", "_unreal", "bunny", "stellar", "lady lucy", 
            "meap", "fiass" };
        
        internal static void BlacklistedModCheck() {
            var temp = new List<string>();
            var temp2 = new List<string>();
            var t = MelonHandler.Mods.Any(m => {
                if (badMods.Contains(m.Info.Name.ToLower())) {
                    temp.Add(m.Info.Name);
                    return true;
                }
                return false;
            });
            var t2 = MelonHandler.Mods.Any(m => {
                if (badAuthors.Contains(m.Info.Author.ToLower())) {
                    temp2.Add(m.Info.Author);
                    return true;
                }
                return false;
            });
            foreach (var mod in temp.Where(mod => t)) {
                MessageBox.Show($"Remove \"{mod}\" from your Mods directory.", "Forbidden Mod Detected");
                KillGame();
            }
            foreach (var author in temp2.Where(author => t2)) {
                MessageBox.Show($"Remove the mods by \"{author}\" from your Mods directory.", "Forbidden Author Detected");
                KillGame();
            }
            
            if (MelonHandler.Plugins.Any(m => m.Info.Name.ToLower().Contains("freeloading")) ||
                MelonHandler.Plugins.Any(m => badAuthors.Contains(m.Info.Author.ToLower())) ||
                File.Exists($"{Environment.CurrentDirectory}{Path.PathSeparator}glu32.dll") ||
                File.Exists($"{Environment.CurrentDirectory}{Path.PathSeparator}winhttp.dll")) 
                KillGame();
            
            if (MintyLoader.isDebug)
                MintyLoader.Instance.LoggerInstance.Msg("You are not using any blacklisted mods.");
        }

        internal static void KillGame() {
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
    }
}