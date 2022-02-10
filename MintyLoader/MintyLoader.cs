using MelonLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Pastel;
using UnityEngine;
using UnityEngine.Networking;

namespace MintyLoader {
    public static class BuildInfo {
        public const string Name = "MintyLoader";
        public const string Author = "Lily";
        public const string Company = "Minty Labs";
        public const string Version = "2.6.0";
        public const string DownloadLink = "https://mintlily.lgbt/mod/loader/MintyLoader.dll";
    }
   
    public class MintyLoader : MelonMod {
        public static MintyLoader Instance;
        internal static readonly DirectoryInfo MintDirectory = new DirectoryInfo($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}UserData{Path.DirectorySeparatorChar}MintMod");
        internal static bool hasQuit, isDebug;
        internal static readonly MelonLogger.Instance InternalLogger = new MelonLogger.Instance("MintyLoader", ConsoleColor.Red);

        public override void OnApplicationStart() {
            Instance = this;
            InternalLogger.Msg("Minty".Pastel("9fffe3") + "Loader is starting up!");
            
            LoadManager.ApplyModURL();
            
            isDebug = Environment.CommandLine.Contains("--MintyDev"); // Check if running as Debug
            
            MonkeKiller.BlacklistedModCheck(); // Check if running blacklisted mod(s)
            
            UpdateManager.CheckVersion(); // Check Loader Version and update if needed
        }

        #region MintyCore pass through

        public override void OnLateUpdate() => LoadManager.MintMod?.OnLateUpdate();

        public override void OnSceneWasLoaded(int level, string name) => LoadManager.MintMod?.OnSceneWasLoaded(level, name);

        public override void OnPreferencesSaved() => LoadManager.MintMod?.OnPreferencesSaved();

        public override void OnFixedUpdate() => LoadManager.MintMod?.OnFixedUpdate();

        public override void OnUpdate() => LoadManager.MintMod?.OnUpdate();

        public override void OnApplicationQuit() {
            if (!hasQuit) {
                hasQuit = true;
                LoadManager.MintMod?.OnApplicationQuit();
                InternalLogger.Msg(ConsoleColor.Red, "MintyLoader is stopping...");
            }
        }

        public override void OnGUI() => LoadManager.MintMod?.OnGUI();

        #endregion
    }
}
