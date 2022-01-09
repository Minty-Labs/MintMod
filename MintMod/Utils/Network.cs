using System;
using Il2CppSystem.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using MintMod.Functions;
using MintMod.Managers;
using MintMod.Managers.Notification;
using MintyLoader;
using UnityEngine;
using VRC.Core;
using VRC;
using VRC.SDKBase;
using ReMod.Core.VRChat;

namespace MintMod.Utils {
    class Network {// : MintSubMod {
        //public override string Name => "NetworkHooks";
        //public override string Description => "Hooks into VRChat's network events.";

        private static bool IsInitialized, SeenFire, AFiredFirst;
        public static event Action<Player> OnJoin, OnLeave;

        internal static IEnumerator OnYieldStart() {
            if (IsInitialized) yield break;
            while (ReferenceEquals(NetworkManager.field_Internal_Static_NetworkManager_0, null)) yield return null;

            var field0 = NetworkManager.field_Internal_Static_NetworkManager_0.field_Internal_VRCEventDelegate_1_Player_0;
            var field1 = NetworkManager.field_Internal_Static_NetworkManager_0.field_Internal_VRCEventDelegate_1_Player_1;

            AddDelegate(field0, EventHandlerA);
            AddDelegate(field1, EventHandlerB);

            IsInitialized = true;

            OnJoin += OnPlayerJoin;
            OnLeave += OnPlayerLeft;
        }

        private static void AddDelegate(VRCEventDelegate<Player> field, Action<Player> eventHandlerA) => field.field_Private_HashSet_1_UnityAction_1_T_0.Add(eventHandlerA);

        public static void EventHandlerA(Player player) {
            if (!SeenFire) {
                AFiredFirst = true;
                SeenFire = true;

                Con.Debug("[NetworkUtils] A fired first", MintCore.isDebug);
            }

            (AFiredFirst ? OnJoin : OnLeave)?.Invoke(player);
        }

        public static void EventHandlerB(Player player) {
            if (!SeenFire) {
                AFiredFirst = false;
                SeenFire = true;

                Con.Debug("[NetworkUtils] B fired first", MintCore.isDebug);
            }

            (AFiredFirst ? OnLeave : OnJoin)?.Invoke(player);
        }

        static void OnPlayerJoin(Player plr) {
            if (plr == null) return;
            var apiUser = plr?.prop_APIUser_0;
            if (apiUser == null) return;

            MasterFinder.OnPlayerJoinLeave();
            if (apiUser.id != APIUser.CurrentUser.id) {
                if ((plr.prop_VRCPlayerApi_0 != null && plr.prop_VRCPlayerApi_0.isModerator) ||
                    (plr.field_Private_VRCPlayerApi_0 != null && plr.field_Private_VRCPlayerApi_0.isModerator)) {
                    if (Config.EnablePlayerJoinLeave.Value)
                        Con.Msg($"[Moderator JOIN] {apiUser.displayName} has joined.");
                    try {
                        VRCUiPopupManager.field_Private_Static_VRCUiPopupManager_0.ShowStandardPopupV2("Moderator Notice",
                            "A moderator of VRChat, has join the world you are in.\n" +
                            $"VRChat Mod: {apiUser.displayName}\n" +
                            "Would you like to go home?",
                            "Yes, Go Home", () => Networking.GoToRoom(RoomManager.field_Internal_Static_ApiWorldInstance_0.instanceId));
                    }
                    catch (Exception j) {
                        Con.Error($"VRCUiPopupStandard did not show\n{j}");
                    }
                }
                else {
                    if (Config.EnablePlayerJoinLeave.Value) {
                        if (Config.FriendsOnlyJoinLeave.Value && APIUser.CurrentUser.friendIDs.Contains(apiUser.id)) {
                            Con.Msg($"[JOIN] {apiUser.displayName} has joined.");
                            if (Config.HeadsUpDisplayNotifs.Value) {
                                VRCUiPopups.Notify($"{apiUser.displayName} jas joined.");
                                //VRCUiManager.prop_VRCUiManager_0.InformHudText($"{apiUser.displayName} has joined.", Color.white);
                            }
                        }
                        else {
                            Con.Msg($"[JOIN] {apiUser.displayName} has joined.");
                            if (Config.HeadsUpDisplayNotifs.Value) {
                                VRCUiPopups.Notify($"{apiUser.displayName} has joined.");
                                //VRCUiManager.prop_VRCUiManager_0.InformHudText($"{apiUser.displayName} has joined.", Color.white);
                            }
                        }
                    }
                }
                if (plr != null) ESP.SetBubbleColor(plr.gameObject);
                if (ESP.isESPEnabled) MelonCoroutines.Start(ESP.JoinDelay(plr));
            }
        }

        static void OnPlayerLeft(Player plr) {
            if (plr == null) return;
            var apiUser = plr?.prop_APIUser_0;
            if (apiUser == null) return;
            if (apiUser.id != APIUser.CurrentUser.id) {
                MasterFinder.OnPlayerJoinLeave();
                if ((plr.prop_VRCPlayerApi_0 != null && plr.prop_VRCPlayerApi_0.isModerator) ||
                    (plr.field_Private_VRCPlayerApi_0 != null && plr.field_Private_VRCPlayerApi_0.isModerator)) {
                    if (Config.EnablePlayerJoinLeave.Value)
                        Con.Msg($"[Moderator LEFT] {apiUser.displayName} has left.");
                } else {
                    if (Config.EnablePlayerJoinLeave.Value) {
                        if (Config.FriendsOnlyJoinLeave.Value && APIUser.CurrentUser.friendIDs.Contains(apiUser.id)) {
                            Con.Msg($"[LEFT] {apiUser.displayName} has left.");
                            if (Config.HeadsUpDisplayNotifs.Value) {
                                VRCUiPopups.Notify($"{apiUser.displayName} has left.");
                                //VRCUiManager.prop_VRCUiManager_0.InformHudText($"{apiUser.displayName} has left.", Color.white);
                            }
                        } else if (!Config.FriendsOnlyJoinLeave.Value) {
                            Con.Msg($"[LEFT] {apiUser.displayName} has left.");
                            if (Config.HeadsUpDisplayNotifs.Value)
                                if (Config.EnablePlayerJoinLeave.Value)
                                    Con.Msg($"[LEFT] {apiUser.displayName} has left.");
                        }
                    }
                }
            }
        }
    }
}
