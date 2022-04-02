using MelonLoader;
using System;
using System.IO;
using Pastel;

namespace MintyLoader {
    public static class BuildInfo {
        public const string Name = "MintyLoader";
        public const string Author = "Lily";
        public const string Company = "Minty Labs";
        public const string Version = "2.7.1";
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
            isDebug = Environment.CommandLine.Contains("--MintyDev"); // Check if running as Debug

            Interpreter.PopulateNativeAssembly.Populate(out _); // Create MintyNative
            if (!Interpreter.PopulateNativeAssembly.Failed)
                Interpreter.NativeInterpreter.RunOnStart(); // Start to read MintyNative

            MintyNetClient.Connect();
            // Preload
            ModBlacklist.BlacklistedModCheck(); // Check if running blacklisted mod(s)
            ReMod_Core_Loader.DownloadAndWrite(out _); // Write ReMod.Core.dll to VRC dir root if they do not have ReMod CE or Private
            if (ReMod_Core_Loader.Failed) {
                InternalLogger.Warning("ReMod.Core Failed to load, I am not going to load MintMod!");
                return;
            }
            
            LoadManager.ApplyModURL(); // Check to see if running Beta Builds
            UpdateManager.CheckVersion(); // Check Loader Version and update if needed, This also loads the Mod
            try { Interpreter.NativeInterpreter.Interpreter?.RemoveAssembly(); } // Remove Mint Image from Mono
            catch (Exception r) { InternalLogger.Error(r); }
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
                Interpreter.NativeInterpreter.Interpreter?.RunOnAppQuit();
                InternalLogger.Msg(ConsoleColor.Red, "MintyLoader is stopping...");
                MintyNetClient.Disconnect();
            }
        }

        public override void OnGUI() => LoadManager.MintMod?.OnGUI();

        #endregion
    }
}
