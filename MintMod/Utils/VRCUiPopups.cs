using ReMod.Core.Notification;
using MintMod.Libraries;
using UnityEngine;

namespace MintMod.Utils {
    public static class VrcUiPopups {
        public static void Notify(string title, string message, Sprite sprite = null, Color? color = null, float durationOnScreen = 3f) {
            NotificationSystem.EnqueueNotification(title.Replace("MintyLoader", "MintMod"), 
                message, 
                ColorConversion.HexToColor(Config.MenuColorHEX.Value, true, 0.7f), 
                durationOnScreen, 
                sprite == null ? NotificationSystem.DefaultIcon : sprite);
        }
    }
}
