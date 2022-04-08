using System.Net;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using MelonLoader.TinyJSON;
using Pastel;

namespace MintyLoader {
    internal static class UpdateManager {
        private static HttpClient Updater, VersionChecker;
        private static string checkedVer;
        //private static bool LoaderIsUpToDate;

        internal static /*async Task*/ void CheckVersion() {
            VersionChecker = new HttpClient();
            VersionChecker.DefaultRequestHeaders.Add("User-Agent", BuildInfo.Name);
            checkedVer = VersionChecker.GetStringAsync("https://mintlily.lgbt/mod/loader/version.txt").GetAwaiter().GetResult();

            if (checkedVer != BuildInfo.Version) {
                //LoaderIsUpToDate = false;
                MintyLoader.InternalLogger.Warning("Loader is out of date, please wait while we update it. You game will restart!");
                MintyLoader.InternalLogger.Msg("Downloading");
                Updater = new HttpClient();
                Updater.DefaultRequestHeaders.Add("User-Agent", BuildInfo.Name);
                var bytes = Updater.GetByteArrayAsync(BuildInfo.DownloadLink).GetAwaiter().GetResult();
                MintyLoader.InternalLogger.Msg("Writing Changes");
                bool good = false;
                try {
                    File.WriteAllBytes(Path.Combine(Environment.CurrentDirectory, "Mods", $"{BuildInfo.Name}.dll"), bytes);
                    good = true;
                    MintyLoader.InternalLogger.Msg($"Finished Updating {BuildInfo.Name}, we\'ll go ahead and restart your game for the new changes.");
                }
                catch (Exception e) {
                    good = false;
                    MintyLoader.InternalLogger.Error(e);
                }
                Updater.Dispose();
                VersionChecker.Dispose();

                if (good) {
                    try {
                        MintyLoader.InternalLogger.Msg("Attempting Restart");
                        Process.Start(Environment.CurrentDirectory + "\\VRChat.exe", Environment.CommandLine);
                        MintyLoader.Instance.OnApplicationQuit();
                        Process.GetCurrentProcess().Kill();
                    }
                    catch (Exception ex) {
                        MintyLoader.InternalLogger.Error(ex);
                    }
                }
            }
            else {
                VersionChecker.Dispose();
                //LoaderIsUpToDate = true;
                MintyLoader.InternalLogger.Msg("Loader Build version: ".Pastel("008B8B") + BuildInfo.Version.Pastel("9fffe3") + 
                                               " :: Server pulled: ".Pastel("008B8B") + checkedVer.Pastel("9fffe3"));
                if (MintyLoader.isDebug)
                    LoadManager.LoadLocal();
                else
                    LoadManager.LoadWebhost();
            }
        }
    }
}