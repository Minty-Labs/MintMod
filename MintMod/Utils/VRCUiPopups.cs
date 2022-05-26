// using ReMod.Core.Notification;
using UnityEngine;
using VRC.Core;

namespace MintMod.Utils {
    public static class VrcUiPopups {
        public static void Notify(string message, Sprite sprite = null, string title = "MintMod", float time = 3f) {
            var temp = Config.useFakeName.Value ? Config.FakeName.Value : null;
            var final = Config.useFakeName.Value && Config.ShowWelcomeMessages.Value ? message.Replace(APIUser.CurrentUser!.displayName, temp) : message;
            
            Managers.Notification.NotificationController_Mint.Instance.EnqueueNotification(
                new Managers.Notification.NotificationObject(title, final,
                    sprite == null ? Managers.Notification.NotificationSystem.Megaphone : sprite,
                    time));
            //NotificationSystem.EnqueueNotification(title, final, time, sprite == null ? NotificationSystem.DefaultIcon : sprite);
        }
    }
}
