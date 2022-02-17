using MelonLoader;
using System;
using MintyLoader;
using MintMod.Managers;

namespace MintMod.Libraries {
    internal class ModCompatibility : MintSubMod {
        public override string Name => "ModCompatibility";
        public override string Description => "Find other mods and do things.";
        public static bool OGTrustRank, UIX, emmVRC, KeyboardPaste, NameplateStats, ReMod, TeleporterVR, SettingsRestart, ProPlates, GPrivateServer, Styletor;
        public static bool hasCNP_On, emmVRCPanicMode;

        internal override void OnStart() {
            if (MelonHandler.Mods.FindIndex((MelonMod i) => i.Info.Name == "OGTrustRanks") != -1)
                OGTrustRank = true;
            if (MelonHandler.Mods.FindIndex((MelonMod i) => i.Info.Name == "KeyboardPaste") != -1)
                KeyboardPaste = true;
            if (MelonHandler.Mods.FindIndex((MelonMod i) => i.Info.Name == "TeleporterVR") != -1)
                TeleporterVR = true;
            if (MelonHandler.Mods.FindIndex((MelonMod i) => i.Info.Name == "NameplateStats") != -1)
                NameplateStats = true;
            if (MelonHandler.Mods.FindIndex((MelonMod i) => i.Info.Name == "ReMod") != -1)
                ReMod = true;
            if (MelonHandler.Mods.FindIndex((MelonMod i) => i.Info.Name == "SettingsRestart") != -1)
                SettingsRestart = true;
            if (MelonHandler.Mods.FindIndex((MelonMod i) => i.Info.Name == "ProPlates") != -1)
                ProPlates = true;
            GPrivateServer = MelonHandler.Mods.FindIndex(i => {
                bool name = false, author = false;
                if (i.Info.Name == "PrivateServer")
                    name = true;
                if (i.Info.Author == "[information redacted]")
                    author = true;
                return name && author;
            }) != -1;

            if (MelonHandler.Mods.FindIndex((MelonMod i) => i.Info.Name == "UI Expansion Kit") != -1) 
                UIX = true;
            
            if (MelonHandler.Mods.FindIndex((MelonMod i) => i.Info.Name == "Styletor") != -1) 
                Styletor = true;

            if ((MelonHandler.Mods.FindIndex((MelonMod i) => i.Info.Name == "emmVRC") != -1) || MelonHandler.Mods.FindIndex((MelonMod i) => i.Info.Name == "emmVRCLoader") != -1) {
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
            }
        }

        private void Read_emmVRCConfig() {
            if (GETemmVRCconfig.ReadConfig().NameplateColorChangingEnabled) 
                hasCNP_On = true;
            if (GETemmVRCconfig.ReadConfig().StealthMode)
                emmVRCPanicMode = true;
        }

        /*private static IEnumerator ColorUIExpansionKit() {
            yield return null;
            Color color = Config.ColorGameMenu.Value ? Colors.Minty : Colors.defaultMenuColor();
            ColorBlock colors = new ColorBlock {
                colorMultiplier = 1f,
                disabledColor = Color.grey,
                highlightedColor = new Color(color.r * 1.5f, color.g * 1.5f, color.b * 1.5f),
                normalColor = color,
                pressedColor = ColorConversion.HexToColor("#e180ff"),
                fadeDuration = 0.1f
            };
            Transform uiExpansionRoot = UIWrappers.GetQuickMenuInstance().transform.Find("ModUiPreloadedBundleContents");
            foreach (Image img in uiExpansionRoot.GetComponentsInChildren<Image>(true))
                if (img.transform.parent.name != "PinToggle" && img.transform.parent.parent.name != "PinToggle")
                    img.color = new Color(color.r * 0.5f, color.g * 0.5f, color.b * 0.5f);

            foreach (Button btn in uiExpansionRoot.GetComponentsInChildren<Button>(true))
                btn.colors = colors;

            foreach (Toggle tgl in uiExpansionRoot.GetComponentsInChildren<Toggle>(true))
                if (tgl.gameObject.name != "PinToggle")
                    tgl.colors = colors;

            foreach (Image img in uiExpansionRoot.GetComponentsInChildren<Image>(true))
                if (img.transform.name == "Checkmark" && img.transform.parent.name != "PinToggle" && img.transform.parent.parent.name != "PinToggle")
                    img.color = new Color(0.8f, 0.8f, 0.8f, 0.6f);

            foreach (Text tex in uiExpansionRoot.GetComponentsInChildren<Text>(true))
                tex.color = Color.white;
            yield break;
        }
        */
    }
}
