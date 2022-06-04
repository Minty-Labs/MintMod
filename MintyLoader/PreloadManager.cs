using System;
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
        internal static void BlacklistedModCheck() {
            var http = new HttpClient();
            var content = http.GetStringAsync("https://mintlily.lgbt/mod/loader/hbaovy_tvk_wsbnpu_ishjrspza.txt")
                .GetAwaiter().GetResult().Split('\n');
            
            var temp =  content[0].Split('|').ToList();
            var temp2 = content[1].Split('|').ToList();
            var temp3 = content[2].Split('|').ToList();
            var temp4 = content[3].Split('|').ToList();

            var isBadMod =          MelonHandler.Mods.Any(m =>  temp.Any(mod => mod.ToLower().Equals  (m.Info.Name.ToLower())));
            var isBadAuthor =       MelonHandler.Mods.Any(m => temp2.Any(mod => mod.ToLower().Contains(m.Info.Name.ToLower())));
            var isBadPluginAuthor = MelonHandler.Mods.Any(m => temp3.Any(mod => mod.ToLower().Contains(m.Info.Name.ToLower())));
            var isBadPlugin =       MelonHandler.Mods.Any(m => temp4.Any(mod => mod.ToLower().Equals  (m.Info.Name.ToLower())));
            
            http.Dispose();
            
            foreach (var mod in temp.Where(mod => isBadMod)) {
                MessageBox.Show($"Remove \"{mod}\" from your Mods directory.", "Forbidden Mod Detected");
                KillGame();
            }
            foreach (var author in temp2.Where(author => isBadAuthor)) {
                MessageBox.Show($"Remove the mods by \"{author}\" from your Mods directory.", "Forbidden Mod Author Detected");
                KillGame();
            }
            foreach (var pAuthor in temp4.Where(pAuthor => isBadPluginAuthor)) {
                MessageBox.Show($"Remove plugins by \"{pAuthor}\" from your Plugins directory.", "Forbidden Plugin Author Detected");
                KillGame();
            }
            foreach (var plugin in temp3.Where(plugin => isBadPlugin)) {
                MessageBox.Show($"Remove \"{plugin}\" from your Plugins directory.", "Forbidden Plugin Detected");
                KillGame();
            }
            
            if (File.Exists(Path.Combine(Environment.CurrentDirectory, "glu32.dll")) ||
                /*File.Exists(Path.Combine(Environment.CurrentDirectory, "winhttp.dll")) ||*/
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

        private static void KillGame() {
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
        internal static void DownloadReModCorePlugin() {
            if (MelonHandler.Plugins.FindIndex(p => p.Info.Name == "ReMod.Core.Updater") != -1) return; // Stop if found
            
            if (File.Exists(Path.Combine(Environment.CurrentDirectory, "Mods", "ReMod.Loader.dll")) ||
                File.Exists(Path.Combine(Environment.CurrentDirectory, "Mods", "ReModCE.Loader.dll"))) return; // Stop if found

            if (MelonHandler.Plugins.FindIndex(p => p.Info.Name == "ReModCorePlugin") != -1) return; // Run if not found
            var bytes = new HttpClient().GetByteArrayAsync("https://mintlily.lgbt/mod/loader/ReModCorePlugin.dll")
                .GetAwaiter().GetResult();
            File.WriteAllBytes(Path.Combine(Environment.CurrentDirectory, "Plugins", "ReModCorePlugin.dll"), bytes);
        }
    }
}