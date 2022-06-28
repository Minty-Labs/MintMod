using MelonLoader;
using System;
using System.IO;
using Path = Il2CppSystem.IO.Path;

namespace MintMod.Libraries {
    internal class ModCompatibility : MintSubMod {
        public override string Name => "ModCompatibility";
        public override string Description => "Find other mods and do things.";

        public static bool UIX, NameplateStats, TeleporterVR, SettingsRestart, GPrivateServer;

        public static bool MintyNameplates =>
            MelonHandler.Mods.FindIndex(i => i.Info.Name == "MintyNameplates") != -1 ||
            File.Exists(Path.Combine(Environment.CurrentDirectory, "Mods", "MintyNameplates.dll"));
        public static bool ReMod =>
            MelonHandler.Mods.FindIndex(i => i.Info.Name == "ReMod") != -1 ||
            File.Exists(Path.Combine(Environment.CurrentDirectory, "Mods", "ReMod.Loader.dll"));
        
        public static bool ReModCE =>
            MelonHandler.Mods.FindIndex(i => i.Info.Name == "ReModCE") != -1 ||
            File.Exists(Path.Combine(Environment.CurrentDirectory, "Mods", "ReModCE.Loader.dll"));
        
        public static bool KeyboardPaste => MelonHandler.Mods.FindIndex(i => i.Info.Name == "KeyboardPaste") != -1;
        
        public static bool ListCounter => MelonHandler.Mods.FindIndex(i => i.Info.Name == "ListCounter") != -1;
        
        public static bool OldMate => MelonHandler.Mods.FindIndex(i => i.Info.Name == "OldMate") != -1;
        
        public static bool Styletor => MelonHandler.Mods.FindIndex(i => i.Info.Name == "Styletor") != -1;

        internal override void OnStart() {
            TeleporterVR = MelonHandler.Mods.FindIndex(i => i.Info.Name == "TeleporterVR") != -1;
            NameplateStats = MelonHandler.Mods.FindIndex(i => i.Info.Name == "NameplateStats") != -1;
            SettingsRestart = MelonHandler.Mods.FindIndex(i => i.Info.Name == "SettingsRestart") != -1;
            // ProPlates = MelonHandler.Mods.FindIndex(i => i.Info.Name == "ProPlates") != -1;
            UIX = MelonHandler.Mods.FindIndex(i => i.Info.Name == "UI Expansion Kit") != -1;
        }
    }
}
