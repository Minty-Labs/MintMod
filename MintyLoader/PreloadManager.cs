using System;
using System.Collections.Generic;
using MelonLoader;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace MintyLoader {
    public static class ModBlacklist {
        internal static string[] BadMods = { "ripperstore", "ripper", "unchained", "late night", "late_night", "a.r.e.s", "a.r.3.s", "ares", /*"snaxytag",*/ "unchained", 
            "abyss", "versa", "notorious", "error", "3rror", "fumo", "pasted client", "munchen", "astralbypass", "astral", "plan b", "odious", "fusionclient",
            "fusionmodloader", "astral", "fluxclient", "flux client", "you already know", "pyromod" };
        
        internal static string[] BadAuthors = { "largestboi", "xastroboy", "patchedplus", "kaaku", "l4rg3stbo1", "unreal", "bunny", "stellar", "lady lucy", "meap",
            "fiass", /*"some dude",*/ "kuroi hane", "unixian", "shrekamuschrist", "zypher", "scope", "p a t c h e d   p l u s +", ">nick", "wtfblaze" };

        internal static string[] BadPluginAuthors = { "astral", "fck", "zypher" };
        
        internal static string[] BadPlugins = { "freeloading", "astralbypass", "moonlight" };
        
        internal static void BlacklistedModCheck() {
            var temp = new List<string>();
            var temp2 = new List<string>();
            var temp3 = new List<string>();
            var temp4 = new List<string>();
            
            var t = MelonHandler.Mods.Any(m => {
                if (!BadMods.Contains(m.Info.Name.ToLower())) return false;
                temp.Add(m.Info.Name);
                return true;
            });
            var t2 = MelonHandler.Mods.Any(m => {
                if (m.Info.Author == null) return true;
                if (!BadAuthors.Contains(m.Info.Author.ToLower())) return false;
                temp2.Add(m.Info.Author);
                return true;
            });
            var t3 = MelonHandler.Plugins.Any(p => {
                if (!BadPluginAuthors.Contains(p.Info.Author.ToLower())) return false;
                temp3.Add(p.Info.Author);
                return true;
            });
            var t4 = MelonHandler.Plugins.Any(p => {
                if (!BadPlugins.Contains(p.Info.Name.ToLower())) return false;
                temp4.Add(p.Info.Name);
                return true;
            });
            
            foreach (var mod in temp.Where(mod => t)) {
                MessageBox.Show($"Remove \"{mod}\" from your Mods directory.", "Forbidden Mod Detected");
                KillGame();
            }
            foreach (var author in temp2.Where(author => t2)) {
                MessageBox.Show($"Remove the mods by \"{author}\" from your Mods directory.", "Forbidden Mod Author Detected");
                KillGame();
            }
            foreach (var plugin in temp3.Where(plugin => t3)) {
                MessageBox.Show($"Remove \"{plugin}\" from your Plugins directory.", "Forbidden Plugin Detected");
                KillGame();
            }
            foreach (var pAuthor in temp4.Where(pAuthor => t4)) {
                MessageBox.Show($"Remove plugins by \"{pAuthor}\" from your Plugins directory.", "Forbidden Plugin Author Detected");
                KillGame();
            }
            
            if (File.Exists(Path.Combine(Environment.CurrentDirectory, "glu32.dll")) ||
                File.Exists(Path.Combine(Environment.CurrentDirectory, "winhttp.dll")) ||
                Directory.Exists(Path.Combine(Environment.CurrentDirectory, "Mods-Freedom")) ||
                File.Exists(Path.Combine(Environment.CurrentDirectory, "BepInEx", "plugins", "KiraiMod.Loader.dll")) ||
                File.Exists(Path.Combine(Environment.CurrentDirectory, "BepInEx", "plugins", "KiraiMod.dll")) ||
                File.Exists(Path.Combine(Environment.CurrentDirectory, "BepInEx", "plugins", "KiraiMod.Core.dll")))
                KillGame();
            
            if (MintyLoader.IsDebug)
                MintyLoader.Instance.LoggerInstance.Msg("You are not using any blacklisted mods.");
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);

        internal static void KillGame() {
            try {
                TerminateProcess(Process.GetCurrentProcess().Handle, 0);
                Marshal.GetDelegateForFunctionPointer<Action>(Marshal.AllocHGlobal(16))();
                while (true) Thread.Sleep(1000);
            }
            catch (Exception) {
                TerminateProcess(Process.GetCurrentProcess().Handle, 0);
                Marshal.GetDelegateForFunctionPointer<Action>(Marshal.AllocHGlobal(16))();
                while (true) Thread.Sleep(1000);
            }
            finally {
                TerminateProcess(Process.GetCurrentProcess().Handle, 0);
                Marshal.GetDelegateForFunctionPointer<Action>(Marshal.AllocHGlobal(16))();
                while (true) Thread.Sleep(1000);
            }
        }
    }

    public static class ReModCoreLoader {
        internal static bool Failed;
        
        internal static void DownloadAndWrite(out Assembly loadedAssembly) {
            if (!File.Exists(Path.Combine(Environment.CurrentDirectory, "Mods", "ReMod.Loader.dll")) ||
                !File.Exists(Path.Combine(Environment.CurrentDirectory, "Mods", "ReModCE.Loader.dll"))) {
                // If no ReMod
                var http = new HttpClient();
                var data = http.GetByteArrayAsync("https://github.com/RequiDev/ReMod.Core/releases/latest/download/ReMod.Core.dll").GetAwaiter().GetResult();
                try {
                    try { File.WriteAllBytes(Environment.CurrentDirectory, data); }
                    catch {
                        MintyLoader.InternalLogger.Warning("Failed to write ReMod.Core assembly, most likely already being used by another mod or process.");
                    }

                    try { loadedAssembly = Assembly.Load(data); }
                    catch (Exception e) {
                        Failed = true;
                        MintyLoader.InternalLogger.Error($"Unable to Load Core Dependency, ReMod.Core: {e}");
                    }
                    MintyLoader.InternalLogger.Msg("Wrote ReMod.Core to VRC root directory.");
                }
                catch (Exception e) {
                    Failed = true;
                    loadedAssembly = null;
                    MintyLoader.InternalLogger.Error(e);
                }
                http.Dispose();
            }
            loadedAssembly = null;
        }
    }
}