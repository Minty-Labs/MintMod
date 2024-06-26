﻿using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using Pastel;
using Newtonsoft.Json;

namespace MintyLoader {
    public class LoaderObject {
        [JsonProperty("Enabled")]
        public bool Enabled { get; set; }
        [JsonProperty("Version")]
        public string Version { get; set; }
        [JsonProperty("MessageOfTheDay")]
        public string Motd { get; set; }
    }
    
    internal static class UpdateManager {
        private static HttpClient _updater, _versionChecker;
        private static string _checkedVer;
        
        internal static LoaderObject LoaderDetails { get; set; }

        internal static void CheckForUpdate() {
            _versionChecker = new HttpClient();
            _versionChecker.DefaultRequestHeaders.Add("User-Agent", BuildInfo.Name);
            _checkedVer = _versionChecker.GetStringAsync($"{BuildInfo.BaseURL}object.json").GetAwaiter().GetResult();
            _versionChecker.Dispose();
            
            // Con.Debug(_checkedVer);
            
            LoaderDetails = JsonConvert.DeserializeObject<LoaderObject>(_checkedVer);
            if (LoaderDetails == null) {
                MintyLoader.InternalLogger.Error("JSON data was null, Mint will not load. >> Report this in #bug-reports");
                return;
            }

            if (!LoaderDetails.Enabled) {
                MintyLoader.InternalLogger.Warning("Mint was set to not load by an admin.");
                return;
            }

            if (LoaderDetails.Enabled && LoaderDetails.Version == BuildInfo.Version) {
                MintyLoader.InternalLogger.Msg("Loader Build version: ".Pastel("008B8B") + BuildInfo.Version.Pastel("9fffe3") + 
                                               " :: Server pulled: ".Pastel("008B8B") + LoaderDetails.Version.Pastel("9fffe3"));
                if (MintyLoader.IsDebug)
                    LoadManager.LoadLocal();
                else
                    LoadManager.LoadWebhost();
                return;
            }
            
            MintyLoader.InternalLogger.Warning("Loader is out of date, please wait while we update it. You game will restart!");
            MintyLoader.InternalLogger.Msg("Downloading");
            _updater = new HttpClient();
            _updater.DefaultRequestHeaders.Add("User-Agent", BuildInfo.Name);
            var bytes = _updater.GetByteArrayAsync(BuildInfo.DownloadLink).GetAwaiter().GetResult();
            MintyLoader.InternalLogger.Msg("Writing Changes");
            var good = false;
            try {
                File.WriteAllBytes(Path.Combine(Environment.CurrentDirectory, "Mods", $"{BuildInfo.Name}.dll"), bytes);
                good = true;
                MintyLoader.InternalLogger.Msg($"Finished Updating {BuildInfo.Name}, we\'ll go ahead and restart your game for the new changes.");
            }
            catch (Exception e) {
                good = false;
                MintyLoader.InternalLogger.Error(e);
            }
            _updater.Dispose();
            _versionChecker.Dispose();

            if (!good) return;
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
}