using System;
using MelonLoader;
using MintMod.Functions;
using MintMod.Functions.Authentication;
using MintMod.Managers;
using MintMod.Resources;
using MintyLoader;
using ReMod.Core.UI.QuickMenu;
using ReMod.Core.VRChat;
using UnityEngine.UI;

namespace MintMod.UserInterface.QuickMenu; 

public static class UtilityMenu {
    private static ReCategoryPage _randomMenu;
    internal static ReMenuToggle FrameSpoof, PingSpoof, PingNegative, BypassRiskyFunc;
    internal static ReMenuButton Frame, Ping;
    
    internal static void RandomStuff(ReMenuCategory baseActions) {
        _randomMenu = baseActions.AddCategoryPage("Utilities", "Contains random functions", MintyResources.cog);
        _randomMenu.AddCategory($"MintMod - v<color=#9fffe3>{MintCore.ModBuildInfo.Version}</color>", false);
        var r = _randomMenu.AddCategory("General Actions", false);
            
        /*DeviceType = r.AddToggle("Spoof as Quest", "Spoof your VRChat login as Quest.",
            on => MelonPreferences.GetEntry<bool>(Config.mint.Identifier, Config.SpoofDeviceType.Identifier).Value = on);
        DeviceType.Toggle(Config.SpoofDeviceType.Value);
        
        r.AddSpacer();*/
        
        PingSpoof = r.AddToggle("Spoof Ping", "Spoof your ping for the monkes.",
            on => MelonPreferences.GetEntry<bool>(Config.mint.Identifier, Config.SpoofPing.Identifier).Value = on,
            Config.SpoofPing.Value);
            
        PingNegative = r.AddToggle("Negative Ping", "Make your spoofed ping negative.",
            on => MelonPreferences.GetEntry<bool>(Config.mint.Identifier, Config.SpoofedPingNegative.Identifier).Value = on,
            Config.SpoofedPingNegative.Value);
        
        Ping = r.AddButton($"<color={(Config.SpoofedPingNegative.Value ? "red>-" : "#00ff00>")}{Config.SpoofedPingNumber.Value}</color>", "This is the number of your spoofed ping.", () => {
            VRCUiPopupManager.field_Private_Static_VRCUiPopupManager_0.ShowInputPopupWithCancel("Set Spoofed Ping", "",
                InputField.InputType.Standard, true, "Set Ping", (_, __, ___) => {
                    int.TryParse(_, out var p);
                    Config.SavePrefValue(Config.mint, Config.SpoofedPingNumber, p);
                    Ping.Text = $"<color={(Config.SpoofedPingNegative.Value ? "red>-" : "#00ff00>")}{p.ToString()}</color>";
                }, () => VRCUiPopupManager.field_Private_Static_VRCUiPopupManager_0.HideCurrentPopup());
        }, MintyResources.wifi);
        
        BypassRiskyFunc = r.AddToggle("Bypass Risky Func", "Forces Mods with Risky Function Checks to work",
            on => MelonPreferences.GetEntry<bool>(Config.mint.Identifier, Config.bypassRiskyFunc.Identifier).Value = on,
            Config.bypassRiskyFunc.Value);
        
        FrameSpoof = r.AddToggle("Spoof Frames", "Spoof your framerate for the monkes.",
            on => MelonPreferences.GetEntry<bool>(Config.mint.Identifier, Config.SpoofFramerate.Identifier).Value = on,
            Config.SpoofFramerate.Value);
        
        Frame = r.AddButton($"{Config.SpoofedFrameNumber.Value}", "This is the number of your spoofed framerate.", () => {
            VRCUiPopupManager.field_Private_Static_VRCUiPopupManager_0.ShowInputPopupWithCancel("Set Spoofed Framerate", "",
                InputField.InputType.Standard, true, "Set Frames", (_, __, ___) => {
                    float.TryParse(_, out var f);
                    Config.SavePrefValue(Config.mint, Config.SpoofedFrameNumber, f);
                    Frame.Text = f.ToString();
                }, () => VRCUiPopupManager.field_Private_Static_VRCUiPopupManager_0.HideCurrentPopup());
        }, MintyResources.tv);
        
        r.AddButton("Clear HUD Message Queue", "Clears the HUD Popup Message Queue", () => {
            if (!Config.UseOldHudMessages.Value) {
                ReMod.Core.Notification.NotificationSystem.ClearNotification();
                ReMod.Core.Notification.NotificationSystem.CloseNotification();
            }
            else VRCUiManager.prop_VRCUiManager_0.field_Private_List_1_String_0.Clear();
        }, MintyResources.messages);

        try {
            var forceClone = r.AddToggle("ForceClone", "Toggle the ability to force clone a public avatar on someone", 
                Patches.PatchIt, Config.ForceClone.Value);
            forceClone.Active = ServerAuth.HasSpecialPermissions;
        }
        catch (Exception e) {
            if (MintCore.IsDebug) Con.Error(e);
        }
    }
}