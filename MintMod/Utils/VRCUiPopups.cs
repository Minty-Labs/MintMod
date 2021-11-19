using System;
using System.Linq;
using System.Reflection;
using Il2CppSystem.Collections.Generic;
using UnhollowerBaseLib.Attributes;
using UnhollowerRuntimeLib.XrefScans;
using UnityEngine;
using UnityEngine.UI;
using MelonLoader;

namespace MintMod.Utils {
    public static class VRCUiPopups {
        public static void InformHudText(this VRCUiManager uiManager, string message, Color color, float duration = 5f, float delay = 0f) {
            uiManager = VRCUiManager.prop_VRCUiManager_0;
            if (uiManager == null) return;
            try {
                uiManager.field_Public_Text_0.color = color; // DisplayTextColor
                uiManager.field_Public_Text_0.text = string.Empty;
                uiManager.field_Public_Text_0.supportRichText = true;
                uiManager.field_Private_Single_0 = 0f; // HudMessageDisplayTime
                uiManager.field_Private_Single_1 = duration; // HudMessageDisplayDuration
                uiManager.field_Private_Single_2 = delay; // DelayBeforeHudMessage
                uiManager.field_Private_List_1_String_0.Add("[MintMod]\n" + message);
                uiManager.field_Public_Text_0.color = Color.white;
            } catch { MelonLogger.Error($"Could not display Popup HUD Message => {message}"); }
        }
    }
}
