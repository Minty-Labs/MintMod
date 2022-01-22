using System.IO.Packaging;
using MintMod.Managers.Notification;
using MintyLoader;
using UnityEngine;
using VRC.Core;

namespace MintMod.Utils {
    public static class VRCUiPopups {
        public static void Notify(string message, Sprite sprite = null, string title = "MintMod", float time = 3f) {
            string temp = Config.useFakeName.Value ? Config.FakeName.Value : null;
            string final = Config.useFakeName.Value && Config.ShowWelcomeMessages.Value ? message.Replace(APIUser.CurrentUser.displayName, temp) : message;
            NotificationController_Mint.Instance.EnqueueNotification(new NotificationObject(title, final, sprite == null ? NotificationSystem.Megaphone : sprite, time));
        }
        /*
        public static void _InformHudText(this VRCUiManager uiManager, string message, Color color, float duration = 5f, float delay = 0f) {
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
            } catch { Con.Error($"Could not display Popup HUD Message => {message}"); }
        }
        */
    }
}
