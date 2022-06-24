﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExitGames.Client.Photon;
using MintMod.Libraries;
using MintMod.Resources;
using MintMod.UserInterface;
using MintMod.Utils;
using MintyLoader;
using ReMod.Core.VRChat;
using UnityEngine;
using VRC;
using VRC.SDKBase;
using HarmonyLib;
using MethodType = HarmonyLib.MethodType;

namespace MintMod {
    internal class Patches : MintSubMod {
        public override string Name => "Patches";
        public override string Description => "";

        internal static bool IsQmOpen;
        public static Action OnWorldJoin, OnWorldLeave;

        private static void ApplyPatches(Type type) {
            Con.Debug($"Applying patches for {type.Name}", MintCore.IsDebug);
            try {
                HarmonyLib.Harmony.CreateAndPatchAll(type, "MintMod_Patches");
            } catch (Exception e) {
                Con.Error($"Failed while patching {type.Name}!\n{e}");
            }
        }

        internal override void OnStart() {
            Con.Debug("Setting up patches", MintCore.IsDebug);
            
            if (!ModCompatibility.MintyNameplates) {
                ApplyPatches(typeof(NameplatePatches));
            }
            
            ApplyPatches(typeof(VrcPlayerPatches));
            ApplyPatches(typeof(PingSpoof));
            ApplyPatches(typeof(FrameSpoof));
            ApplyPatches(typeof(VrcStation));
            ApplyPatches(typeof(QuickMenuPatches));
            ApplyPatches(typeof(LeftRoomPatches));
            ApplyPatches(typeof(MonkePatch));

            /*
            if (Config.SpoofDeviceType.Value) {
                applyPatches(typeof(PlatformSpoof));
                Con.Debug("Patched DeviceSpoof", MintCore.isDebug);
            }
            Con.Msg($"Device Type: {(Config.SpoofDeviceType.Value ? "Quest" : "PC")}");
            */
        }
    }

    [HarmonyPatch]
    internal class MonkePatch {
        internal static List<int> _monkes = new();
        
        private static IEnumerable<MethodBase> TargetMethods() {
            return typeof(VRC_EventDispatcherRFC).GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.Name.StartsWith("Method_Public_Void_Player_VrcEvent_VrcBroadcastType_Int32_Single_"));
        }
        
        private static bool Prefix(Player __0, VRC_EventHandler.VrcEvent __1, VRC_EventHandler.VrcBroadcastType __2, int __3, float __4) {
            // (Player player, VRC_EventHandler.VrcEvent evt, VRC_EventHandler.VrcBroadcastType broadcastType, int instagatorId, float fastForward)
            if (!__1.ParameterString.Contains("rtYKZRlV7sTx76sL")) return true;

            if (_monkes.Contains(__3)) return false;
            _monkes.Add(__3);
            Con.Warn($"The user {__0.GetAPIUser().displayName} is a known World Client monke.");
            VrcUiPopups.Notify("Mint Mod", $"A known World Client monke has joined the instance\n{__0.GetAPIUser().displayName}", MintyResources.Megaphone, 
                ColorConversion.HexToColor("F60B0E"), 5f);

            return false;
        }
        
        private static Exception Finalizer() => null;
    }

    [HarmonyPatch(typeof(VRC.UI.Elements.QuickMenu))]
    internal class QuickMenuPatches {
        [HarmonyPostfix]
        [HarmonyPatch("OnEnable")]
        private static void OnQuickMenuEnable() => Patches.IsQmOpen = true;

        [HarmonyPostfix]
        [HarmonyPatch("OnDisable")]
        private static void OnQuickMenuDisable() => Patches.IsQmOpen = false;
    }

    [HarmonyPatch]
    internal class NameplatePatches { // OnRebuild
        static IEnumerable<MethodBase> TargetMethods() {
            return typeof(PlayerNameplate).GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(x => 
                Nameplates.MethodMatchRegex.IsMatch(x.Name)).Cast<MethodBase>();
        }

        static void Postfix(PlayerNameplate __instance) => Nameplates.OnRebuild(__instance);
    }

    [HarmonyPatch(typeof(VRCPlayer))]
    internal class VrcPlayerPatches { // OnPlayerAwake
        [HarmonyPostfix]
        [HarmonyPatch("Awake")]
        static void OnVRCPlayerAwake(VRCPlayer __instance) => Nameplates.OnVRCPlayerAwake(__instance);
    }

    [HarmonyLib.HarmonyPatch(typeof(VRC_StationInternal))]
    internal class VrcStation {
        [HarmonyPrefix]
        [HarmonyPatch("Method_Public_Boolean_Player_Boolean_0")]
        static bool PlayerCanUseStation(ref bool __result, VRC.Player __0, bool __1) {
            if (__0 != null && __0 == VRCPlayer.field_Internal_Static_VRCPlayer_0._player && !Config.CanSitInChairs.Value) {
                __result = false;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(PhotonPeer))]
    internal class PingSpoof {
        [HarmonyPrefix]
        [HarmonyPatch("RoundTripTime", MethodType.Getter)]
        static bool Prefix(ref int __result) {
            if (!Config.SpoofPing.Value)
                return true;
            int num = Config.SpoofedPingNumber.Value;
            __result = Config.SpoofedPingNegative.Value ? -num : num;
            return false;
        }
    }

    [HarmonyPatch(typeof(Time))]
    internal class FrameSpoof {
        [HarmonyPrefix]
        [HarmonyPatch("smoothDeltaTime", MethodType.Getter)]
        static bool Prefix(ref float __result) {
            if (!Config.SpoofFramerate.Value)
                return true;
            __result = 1f / Config.SpoofedFrameNumber.Value;
            return false;
        }
    }

    /*
    [HarmonyLib.HarmonyPatch(typeof(Tools))]
    internal class PlatformSpoof {
        [HarmonyLib.HarmonyPostfix]
        [HarmonyLib.HarmonyPatch("Platform", MethodType.Getter)]
        static void Postfix(ref string __result) {
            try {
                __result = Config.SpoofDeviceType.Value ? "android" : "standalonewindows";
                Tools._platform = __result;
            } catch { }
        }
    }
    */
    
    [HarmonyPatch(typeof(NetworkManager))]
    internal class LeftRoomPatches {
        [HarmonyPostfix]
        [HarmonyPatch("OnLeftRoom")]
        static void Yeet() {
            Patches.OnWorldLeave?.Invoke();
            MonkePatch._monkes.Clear();
        }

        [HarmonyPostfix]
        [HarmonyPatch("OnJoinedRoom")]
        static void JoinedRoom() => Patches.OnWorldJoin?.Invoke();
    }
}
