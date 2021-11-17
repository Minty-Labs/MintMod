using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using MintMod.Managers;
using MintMod.Reflections;
using UnityEngine;
using UnityEngine.UI;

namespace MintMod.Libraries {
    class ModCompatibility : MintSubMod {
        public override string Name => "ModCompatibility";
        public override string Description => "Find other mods and do things.";
        public static bool OGTrustRank, UIX, emmVRC, KeyboardPaste, NameplateStats, ReMod, TeleporterVR, SettingsRestart;
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
            
            if (MelonHandler.Mods.FindIndex((MelonMod i) => i.Info.Name == "UI Expansion Kit") != -1) {
                UIX = true;

                if (Config.LookForUiExpansionKitInstall.Value && Config.InfoHUDPosition.Value == "3")
                    MelonPreferences.GetEntry<string>(Config.Menu.Identifier, Config.InfoHUDPosition.Identifier).Value = "4";

                if (Config.ColorUiExpansionKit.Value && Config.ColorGameMenu.Value) {
                    try {
                        MelonHandler.Mods.First((MelonMod i) => i.Info.Name == "UI Expansion Kit").Assembly.GetType("UIExpansionKit.API.ExpansionKitApi").GetMethod("RegisterWaitConditionBeforeDecorating", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[] { ColorUIExpansionKit() });
                    } catch (Exception e) { MelonLogger.Error($"{e}"); }
                }
            }

            if ((MelonHandler.Mods.FindIndex((MelonMod i) => i.Info.Name == "emmVRC") != -1) || MelonHandler.Mods.FindIndex((MelonMod i) => i.Info.Name == "emmVRCLoader") != -1) {
                emmVRC = true;
                try { GETemmVRCconfig.LoadConfig(); } catch { }
                if (Config.RecolorRanks.Value) {
                    try {
                        Read_emmVRCConfig();
                    } catch (Exception read) {
                        MelonLogger.Error(read.ToString());
                    }
                    try {
                        if (hasCNP_On)
                            MelonPreferences.GetEntry<bool>(Config.Color.Identifier, Config.RecolorRanks.Identifier).Value = false;
                    } catch (Exception apply) {
                        MelonLogger.Error(apply.ToString());
                    }
                }
            }
        }

        private static void Read_emmVRCConfig() {
            if (GETemmVRCconfig.ReadConfig().NameplateColorChangingEnabled == true) {
                hasCNP_On = true;
            }
            if (GETemmVRCconfig.ReadConfig().StealthMode == true)
                emmVRCPanicMode = true;
        }

        private static IEnumerator ColorUIExpansionKit() {
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
    }
}
