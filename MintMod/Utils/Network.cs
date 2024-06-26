﻿using System;
using MelonLoader;
using MintMod.Functions;
using MintMod.Managers;
using MintyLoader;
using Pastel;
using VRC.Core;
using VRC;
using VRC.SDKBase;
using ReMod.Core.VRChat;
using BuildInfo = MelonLoader.BuildInfo;

namespace MintMod.Utils {
    internal class NetworkEvents : MintSubMod {
        public override string Name => "NetworkHooks";
        public override string Description => "Hooks into VRChat's network events.";
        
        private static Action<Player> _eventHandlerA;
        private static Action<Player> _eventHandlerB;
        private static Action<Player> EventHandlerA {
            get {
                _eventHandlerB ??= OnPlayerLeft;
                return _eventHandlerA ??= OnPlayerJoin;
            }
        }
        private static Action<Player> EventHandlerB {
            get {
                _eventHandlerA ??= OnPlayerLeft;
                return _eventHandlerB ??= OnPlayerJoin;
            }
        }

        internal override void OnUserInterface() {
            NetworkManager.field_Internal_Static_NetworkManager_0.field_Internal_VRCEventDelegate_1_Player_0.
                field_Private_HashSet_1_UnityAction_1_T_0.Add(EventHandlerA);
            NetworkManager.field_Internal_Static_NetworkManager_0.field_Internal_VRCEventDelegate_1_Player_1.
                field_Private_HashSet_1_UnityAction_1_T_0.Add(EventHandlerB);
        }
        
        private static void OnPlayerJoin(Player plr) {
            if (plr == null) return;
            var apiUser = plr.field_Private_APIUser_0;
            if (apiUser == null) return;

            if (apiUser.id != APIUser.CurrentUser.id) {
                if ((plr.prop_VRCPlayerApi_0 != null && plr.prop_VRCPlayerApi_0.isModerator) ||
                    (plr.field_Private_VRCPlayerApi_0 != null && plr.field_Private_VRCPlayerApi_0.isModerator)) {
                    if (Config.EnablePlayerJoinLeave.Value)
                        Con.Msg("[" + "Moderator JOIN".Pastel("3BA55D") + $"] {apiUser.displayName} has joined.");
                    try {
                        if (Config.ModJoinPopup.Value) {
                            VRCUiPopupManager.field_Private_Static_VRCUiPopupManager_0.ShowStandardPopupV2(
                                "Moderator Notice",
                                "A moderator of VRChat, has join the world you are in.\n" +
                                $"VRChat Mod: {apiUser.displayName}\n" +
                                "Would you like to go home?",
                                "Yes, Go Home",
                                () => Networking.GoToRoom(RoomManager.field_Internal_Static_ApiWorldInstance_0.instanceId),
                                "No, Stay", () => {
                                    Config.SavePrefValue(Config.mint, Config.SpoofFramerate, false);
                                    Config.SavePrefValue(Config.mint, Config.SpoofPing, false);
                                    MelonPreferences.Save();
                                    VRCUiPopupManager.field_Private_Static_VRCUiPopupManager_0.HideCurrentPopup();
                                }, null);
                        }
                    }
                    catch (Exception j) {
                        Con.Error($"VRCUiPopupStandard did not show\n{j}");
                    }
                }
                else {
                    if (Config.EnablePlayerJoinLeave.Value) {
                        if (Config.FriendsOnlyJoinLeave.Value && (APIUser.CurrentUser.friendIDs.Contains(apiUser.id) || apiUser.isFriend || APIUser.IsFriendsWith(apiUser.id))) {
                            Con.Msg("[" + "JOIN".Pastel("3BA55D") + $"] {apiUser.displayName} has joined.");
                            if (Config.HeadsUpDisplayNotifs.Value)
                                VrcUiPopups.Notify(MintCore.ModBuildInfo.Name, $"{apiUser.displayName} has joined.");
                        }
                        else {
                            Con.Msg("[" + "JOIN".Pastel("3BA55D") + $"] {apiUser.displayName} has joined.");
                            if (Config.HeadsUpDisplayNotifs.Value)
                                VrcUiPopups.Notify(MintCore.ModBuildInfo.Name, $"{apiUser.displayName} has joined.");
                        }
                    }
                }

                if (plr != null) ESP.SetBubbleColor(plr.gameObject);
                if (ESP.isESPEnabled) MelonCoroutines.Start(ESP.JoinDelay(plr));
            }
            else MasterFinder.OnSelfJoin();
        }

        private static void OnPlayerLeft(Player plr) {
            if (plr == null) return;
            var apiUser = plr.field_Private_APIUser_0;
            if (apiUser == null) return;
            if (apiUser.id != APIUser.CurrentUser.id) {
                MasterFinder.OnPlayerLeft();
                if ((plr.prop_VRCPlayerApi_0 != null && plr.prop_VRCPlayerApi_0.isModerator) ||
                    (plr.field_Private_VRCPlayerApi_0 != null && plr.field_Private_VRCPlayerApi_0.isModerator)) {
                    if (Config.EnablePlayerJoinLeave.Value)
                        Con.Msg("[" + "Moderator LEFT".Pastel("ED4245") + $"] {apiUser.displayName} has left.");
                } else {
                    if (Config.EnablePlayerJoinLeave.Value) {
                        if (Config.FriendsOnlyJoinLeave.Value && (APIUser.CurrentUser.friendIDs.Contains(apiUser.id) || apiUser.isFriend || APIUser.IsFriendsWith(apiUser.id))) {
                            Con.Msg("[" + "LEFT".Pastel("ED4245") + $"] {apiUser.displayName} has left.");
                            if (Config.HeadsUpDisplayNotifs.Value) 
                                VrcUiPopups.Notify(MintCore.ModBuildInfo.Name, $"{apiUser.displayName} has left.");
                        } else {
                            Con.Msg("[" + "LEFT".Pastel("ED4245") + $"] {apiUser.displayName} has left.");
                            if (Config.HeadsUpDisplayNotifs.Value)
                                VrcUiPopups.Notify(MintCore.ModBuildInfo.Name, $"{apiUser.displayName} has left.");
                        }
                    }
                }
            }
        }
    }
}
