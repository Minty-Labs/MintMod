using System.Windows.Forms;
using System;
using MelonLoader;
using MintMod.Functions;
using MintMod.Functions.Authentication;
using MintMod.Managers;
using MintMod.Resources;
using MintyLoader;
using ReMod.Core.UI.QuickMenu;
using ReMod.Core.VRChat;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using Object = UnityEngine.Object;

namespace MintMod.UserInterface.QuickMenu; 

public static class WorldMenu {
    private static ReCategoryPage _worldMenu, _worldActionsPage;
    private static ReMenuToggle _worldToggle;
    internal static ReMenuToggle ItemEsp;
    
    internal static void BuildWorld(ReMenuCategory baseActions) {
        _worldMenu = baseActions.AddCategoryPage("World", "Actions involving the world.", MintyResources.globe);
        var w = _worldMenu.AddCategory("General Actions");
        
        ItemEsp = w.AddToggle("Item ESP", "Puts a bubble around all Pickups, can be seen through walls", ESP.SetItemESPToggle);
        w.AddButton("Download VRCW", "Downloads the world file (.vrcw)", async () => await WorldActions.WorldDownload(), MintyResources.dl);
        w.AddButton("Copy Instance ID URL", "Copies current instance ID and places it in your system's clipboard.", () => {
            var id = RoomManager.field_Internal_Static_ApiWorld_0.id;
            var instance = RoomManager.field_Internal_Static_ApiWorldInstance_0.instanceId;
            var faulted = false;
            try {
                GUIUtility.systemCopyBuffer = $"https://vrchat.com/home/launch?worldId={id}&instanceId={instance}";
            }
            catch {
                Clipboard.SetText($"https://vrchat.com/home/launch?worldId={id}&instanceId={instance}");
                faulted = true;
            }

            Con.Msg(faulted ? "Failed to copy instance ID" : $"Got ID: {RoomManager.field_Internal_Static_ApiWorldInstance_0.id}");
        }, MintyResources.clipboard);
        w.AddButton("Join Instance by ID", "Join the room of the instance ID", () => {
            try {
                string clip;
                try { clip = GUIUtility.systemCopyBuffer; } catch { clip = Clipboard.GetText(); }
                    
                if (clip.Contains("launch?")) {
                    Networking.GoToRoom(clip
                        .Replace("https://vrchat.com/home/launch?worldId=", "")
                        .Replace("&instanceId=", ":"));
                } else if (clip.Contains("wrld_")) {
                    Networking.GoToRoom(clip);
                }
                else Con.Warn("Clipboard text is probably not a valid join link.");
            }
            catch (Exception j) {
                Con.Error(j);
            }
        }, MintyResources.globe);
        
        w.AddButton("Log World", "Logs world info (of various data points) in a text file.", WorldActions.LogWorld, MintyResources.list);
        w.AddButton("Reset Portal", $"Sets portal timers to {Config.ResetTimerAmount.Value}", () => {
            if (Object.FindObjectsOfType<PortalInternal>() == null) return;
            var single = default(Il2CppSystem.Single);
            single.m_value = Config.ResetTimerAmount.Value < 30 ? 30 : Config.ResetTimerAmount.Value;
            var @object = single.BoxIl2CppObject();
            var array = UnityEngine.Resources.FindObjectsOfTypeAll<PortalTrigger>();
            foreach (var portal in array) {
                if (!portal.gameObject.activeInHierarchy) return;
                if (portal.gameObject.GetComponentInParent<VRC_PortalMarker>() == null) return;
                Networking.RPC(RPC.Destination.AllBufferOne, portal.gameObject, "SetTimerRPC",
                    new[] { @object });
            }
            /*for (int i = 0; i < array.Length; i++) {
                if (array[i].gameObject.activeInHierarchy && !(array[i].gameObject.GetComponentInParent<VRC_PortalMarker>() != null)) 
                    Networking.RPC(RPC.Destination.AllBufferOne, array[i].gameObject, "SetTimerRPC",
                        new[] { @object });
            }*/
        }, MintyResources.history);
        
        
        var canSee = ServerAuth.HasSpecialPermissions;
        var e = _worldMenu.AddCategory("Item Manipulation");
        var _1 = e.AddButton("Teleport Items to Self", "Teleports all Pickups to your feet.", Items.TPToSelf);
        var _2 = e.AddButton("Respawn Items", "Respawns All pickups to their original location.", Items.Respawn);
        var _3 = e.AddButton("Teleport Items Out of World", "Teleports all Pickups an XYZ coord of 1 million", Items.TPToOutWorld);
        var _4 = e.AddSpacer();
        _1.Active = canSee; _2.Active = canSee; _3.Active = canSee; _4.Active = canSee;
        e.AddButton("Normal World Mirrors", "Reverts mirrors to their original state", WorldActions.RevertMirrors);
        e.AddButton("Optimize Mirrors", "Make Mirrors only show players", WorldActions.OptimizeMirrors);
        e.AddButton("Beautify Mirrors", "Make Mirrors show everything", WorldActions.BeautifyMirrors);
        
        var ct = _worldMenu.AddCategory("Component Toggle");
        Components.ComponentToggle(ct);
        
        var actionCategory = _worldMenu.AddCategory("Per-world actions");
            
        WorldSettings.BlackCat.BuildMenu(actionCategory);
            
        // var mintActions = _worldMenu.AddCategory("Mint Actions");
        //
        // _worldToggle = mintActions.AddToggle("Mint World Toggles", "Toggle Mint specific objects in the world, if there are any.", b => 
        //     GameObject.Find("_Mint_SetON")!.SetActive(b));
    }

    internal static void OnWorldChange() {
        _worldToggle?.Toggle(false, true, true);
    }
}