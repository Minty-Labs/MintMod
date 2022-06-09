using MelonLoader;
using System;
using System.Collections.Generic;
using System.IO;
using MintyLoader;
using MintMod.Managers;
using Path = Il2CppSystem.IO.Path;

namespace MintMod.Libraries {
    internal class ModCompatibility : MintSubMod {
        public override string Name => "ModCompatibility";
        public override string Description => "Find other mods and do things.";
        public static bool UIX, /*emmVRC,*/ KeyboardPaste, NameplateStats, ReMod, ReModCE, TeleporterVR, SettingsRestart, /*ProPlates,*/ GPrivateServer, Styletor,
            ListCounter;
        // public static bool hasCNP_On, emmVRCPanicMode;

        public static bool MintyNameplates {
            get {
                return MelonHandler.Mods.FindIndex(i => i.Info.Name == "MintyNameplates") != -1 ||
                       File.Exists(Path.Combine(Environment.CurrentDirectory, "Mods", "MintyNameplates.dll"));
            }
        }

        internal override void OnStart() {
            KeyboardPaste = MelonHandler.Mods.FindIndex(i => i.Info.Name == "KeyboardPaste") != -1;
            TeleporterVR = MelonHandler.Mods.FindIndex(i => i.Info.Name == "TeleporterVR") != -1;
            NameplateStats = MelonHandler.Mods.FindIndex(i => i.Info.Name == "NameplateStats") != -1;
            ReMod = MelonHandler.Mods.FindIndex(i => i.Info.Name == "ReMod") != -1 || File.Exists(Path.Combine(Environment.CurrentDirectory, "Mods", "ReMod.Loader.dll"));
            ReModCE = MelonHandler.Mods.FindIndex(i => i.Info.Name == "ReModCE") != -1 || File.Exists(Path.Combine(Environment.CurrentDirectory, "Mods", "ReModCE.Loader.dll"));
            SettingsRestart = MelonHandler.Mods.FindIndex(i => i.Info.Name == "SettingsRestart") != -1;
            // ProPlates = MelonHandler.Mods.FindIndex(i => i.Info.Name == "ProPlates") != -1;
            UIX = MelonHandler.Mods.FindIndex(i => i.Info.Name == "UI Expansion Kit") != -1;
            Styletor = MelonHandler.Mods.FindIndex(i => i.Info.Name == "Styletor") != -1;
            ListCounter = MelonHandler.Mods.FindIndex(i => i.Info.Name == "ListCounter") != -1;

            /*if ((MelonHandler.Mods.FindIndex((MelonMod i) => i.Info.Name == "emmVRC") != -1) || MelonHandler.Mods.FindIndex((MelonMod i) => i.Info.Name == "emmVRCLoader") != -1) {
                emmVRC = true;
                try { GETemmVRCconfig.LoadConfig(); } catch { }
                if (Config.RecolorRanks.Value) {
                    try {
                        Read_emmVRCConfig();
                    } catch (Exception read) {
                        Con.Error(read);
                    }
                    try {
                        if (hasCNP_On)
                            MelonPreferences.GetEntry<bool>(Config.Color.Identifier, Config.RecolorRanks.Identifier).Value = false;
                    } catch (Exception apply) {
                        Con.Error(apply);
                    }
                }
            }*/
        }

        /*private void Read_emmVRCConfig() {
            if (GETemmVRCconfig.ReadConfig().NameplateColorChangingEnabled) 
                hasCNP_On = true;
            if (GETemmVRCconfig.ReadConfig().StealthMode)
                emmVRCPanicMode = true;
        }*/
    }
}
