using MelonLoader;
using System;
using System.IO;
using Pastel;

namespace MintyLoader {
    public static class BuildInfo {
        public const string Name = "MintyLoader";
        public const string Author = "Lily";
        public const string Company = "Minty Labs";
        public const string Version = "2.9.1";
        public const string DownloadLink = "https://mintlily.lgbt/mod/loader/MintyLoader.dll";
    }
   
    public class MintyLoader : MelonMod {
        public static MintyLoader Instance;
        internal static readonly DirectoryInfo MintDirectory = new DirectoryInfo($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}UserData{Path.DirectorySeparatorChar}MintMod");
        private static bool _hasQuit;
        internal static bool IsDebug;
        internal static readonly MelonLogger.Instance InternalLogger = new MelonLogger.Instance("MintyLoader", ConsoleColor.Red);

        public override void OnApplicationStart() {
            Instance = this;
            InternalLogger.Msg("Minty".Pastel("9fffe3") + "Loader is starting up!");
            IsDebug = Environment.CommandLine.Contains("--MintyDev"); // Check if running as Debug
#if DEBUG
            Interpreter.PopulateNativeAssembly.Populate(out _); // Create MintyNative
            if (!Interpreter.PopulateNativeAssembly.Failed)
                Interpreter.NativeInterpreter.RunOnStart(); // Start to read MintyNative
#endif

            // Preload
            ModBlacklist.BlacklistedModCheck(); // Check if running blacklisted mod(s)
            ReModCoreLoader.DownloadReModCorePlugin(); // Write ReModCorePlugin.dll to the Plugins folder if they do not have the plugin or Penny's version
            
            LoadManager.ApplyModURL(); // Check to see if running Beta Builds
            UpdateManager.CheckForUpdate(); // Check Loader Version and update if needed, This also loads the Mod
#if DEBUG
            try { Interpreter.NativeInterpreter.Interpreter?.RemoveAssembly(); } // Remove Mint Image from Mono
            catch (Exception r) { InternalLogger.Error(r); }
#endif
        }

        #region MintyCore pass through

        public override void OnLateUpdate() => LoadManager.MintMod?.OnLateUpdate();

        public override void OnSceneWasLoaded(int level, string name) => LoadManager.MintMod?.OnSceneWasLoaded(level, name);

        public override void OnPreferencesSaved() => LoadManager.MintMod?.OnPreferencesSaved();

        public override void OnFixedUpdate() => LoadManager.MintMod?.OnFixedUpdate();

        public override void OnUpdate() => LoadManager.MintMod?.OnUpdate();

        public override void OnApplicationQuit() {
            if (!_hasQuit) {
                _hasQuit = true;
                LoadManager.MintMod?.OnApplicationQuit();
#if DEBUG
                Interpreter.NativeInterpreter.Interpreter?.RunOnAppQuit();
#endif
                InternalLogger.Msg(ConsoleColor.Red, "MintyLoader is stopping...");
            }
        }

        public override void OnGUI() => LoadManager.MintMod?.OnGUI();

        #endregion
    }
}
