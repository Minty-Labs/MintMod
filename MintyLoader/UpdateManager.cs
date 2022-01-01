using System.Net;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using MelonLoader.TinyJSON;
using Pastel;

namespace MintyLoader {
    public class UpdateManager {
        private static HttpClient Updater, VersionChecker;
        private static string checkedVer;
        public static bool LoaderIsUpToDate;

        internal static async Task CheckVersion() {
            VersionChecker = new HttpClient();
            VersionChecker.DefaultRequestHeaders.Add("User-Agent", BuildInfo.Name);
            checkedVer = await VersionChecker.GetStringAsync("https://mintlily.lgbt/mod/loader/version.txt");

            if (checkedVer != BuildInfo.Version) {
                LoaderIsUpToDate = false;
                MintyLoader.InternalLogger.Warning("Loader is out of date, please wait while we update it. You game will restart!");
                MintyLoader.InternalLogger.Msg("Downloading");
                Updater = new HttpClient();
                Updater.DefaultRequestHeaders.Add("User-Agent", BuildInfo.Name);
                var bytes = await Updater.GetByteArrayAsync(BuildInfo.DownloadLink);
                MintyLoader.InternalLogger.Msg("Writing Changes");
                bool good = false;
                try {
                    File.WriteAllBytes(Path.Combine(Environment.CurrentDirectory, $"Mods/{BuildInfo.Name}.dll"), bytes);
                    good = true;
                    MintyLoader.InternalLogger.Msg($"Finished Updating {BuildInfo.Name}, we\'ll go ahead and restart your game for the new changes.");
                }
                catch (Exception e) {
                    good = false;
                    MintyLoader.InternalLogger.Error(e);
                }

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
                LoaderIsUpToDate = true;
                MintyLoader.InternalLogger.Msg($"Loader Build version: ".Pastel("008B8B") + BuildInfo.Version.Pastel("9fffe3") + 
                                               " :: Server pulled: ".Pastel("008B8B") + checkedVer.Pastel("9fffe3"));
            }
        }
    }
}