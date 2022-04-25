using ExitGames.Client.Photon;
using MelonLoader;
using MintMod.UserInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using MintMod.Functions;
using MintMod.Libraries;
using MintMod.UserInterface.QuickMenu;
using MintyLoader;
using UnhollowerBaseLib;
using UnityEngine;
using VRC;
using UnhollowerRuntimeLib.XrefScans;
using VRC.Core;
using AccessTools = HarmonyLib.AccessTools;
using HarmonyMethod = HarmonyLib.HarmonyMethod;
using MethodType = HarmonyLib.MethodType;

namespace MintMod.Hooks {
    internal class Patches : MintSubMod {
        public override string Name => "Patches";
        public override string Description => "";

        internal static bool IsQmOpen;
        public static Action OnWorldJoin, OnWorldLeave;

        private static void ApplyPatches(Type type) {
            Con.Debug($"Applying {type.Name} patches!", MintCore.isDebug);
            try {
                HarmonyLib.Harmony.CreateAndPatchAll(type, "MintMod_Patches");
            } catch (Exception e) {
                Con.Error($"Failed while patching {type.Name}!\n{e}");
            }
        }

        internal override void OnStart() {
            Con.Debug("Setting up patches", MintCore.isDebug);

            if (!ModCompatibility.MintyNameplates) {
                ApplyPatches(typeof(NameplatePatches));
                ApplyPatches(typeof(VrcPlayerPatches));
            }
            ApplyPatches(typeof(PingSpoof));
            ApplyPatches(typeof(FrameSpoof));
            ApplyPatches(typeof(VrcStation));
            ApplyPatches(typeof(QuickMenuPatches));
            ApplyPatches(typeof(LeftRoomPatches));

            /*
            if (Config.SpoofDeviceType.Value) {
                applyPatches(typeof(PlatformSpoof));
                Con.Debug("Patched DeviceSpoof", MintCore.isDebug);
            }
            Con.Msg($"Device Type: {(Config.SpoofDeviceType.Value ? "Quest" : "PC")}");
            */
        }
    }

    [HarmonyLib.HarmonyPatch(typeof(VRC.UI.Elements.QuickMenu))]
    internal class QuickMenuPatches {
        [HarmonyLib.HarmonyPostfix]
        [HarmonyLib.HarmonyPatch("OnEnable")]
        private static void OnQuickMenuEnable() {
            Patches.IsQmOpen = true;
        }

        [HarmonyLib.HarmonyPostfix]
        [HarmonyLib.HarmonyPatch("OnDisable")]
        private static void OnQuickMenuDisable() {
            Patches.IsQmOpen = false;
        }
    }

    [HarmonyLib.HarmonyPatch]
    internal class NameplatePatches { // OnRebuild
        static IEnumerable<MethodBase> TargetMethods() {
            return typeof(PlayerNameplate).GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(x => 
                Nameplates.MethodMatchRegex.IsMatch(x.Name)).Cast<MethodBase>();
        }

        static void Postfix(PlayerNameplate __instance) => Nameplates.OnRebuild(__instance);
    }

    [HarmonyLib.HarmonyPatch(typeof(VRCPlayer))]
    internal class VrcPlayerPatches { // OnPlayerAwake
        [HarmonyLib.HarmonyPostfix]
        [HarmonyLib.HarmonyPatch("Awake")]
        static void OnVRCPlayerAwake(VRCPlayer __instance) {
            Nameplates.OnVRCPlayerAwake(__instance);
        }
    }

    [HarmonyLib.HarmonyPatch(typeof(VRC_StationInternal))]
    internal class VrcStation {
        [HarmonyLib.HarmonyPrefix]
        [HarmonyLib.HarmonyPatch("Method_Public_Boolean_Player_Boolean_0")]
        static bool PlayerCanUseStation(ref bool __result, VRC.Player __0, bool __1) {
            if (__0 != null && __0 == VRCPlayer.field_Internal_Static_VRCPlayer_0._player && !Config.CanSitInChairs.Value) {
                __result = false;
                return false;
            }
            return true;
        }
    }

    [HarmonyLib.HarmonyPatch(typeof(PhotonPeer))]
    internal class PingSpoof {
        [HarmonyLib.HarmonyPrefix]
        [HarmonyLib.HarmonyPatch("RoundTripTime", MethodType.Getter)]
        static bool Prefix(ref int __result) {
            if (!Config.SpoofPing.Value)
                return true;
            int num = Config.SpoofedPingNumber.Value;
            __result = Config.SpoofedPingNegative.Value ? -num : num;
            return false;
        }
    }

    [HarmonyLib.HarmonyPatch(typeof(Time))]
    internal class FrameSpoof {
        [HarmonyLib.HarmonyPrefix]
        [HarmonyLib.HarmonyPatch("smoothDeltaTime", MethodType.Getter)]
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
    
    [HarmonyLib.HarmonyPatch(typeof(NetworkManager))]
    internal class LeftRoomPatches {
        [HarmonyLib.HarmonyPostfix]
        [HarmonyLib.HarmonyPatch("OnLeftRoom")]
        static void Yeet() => Patches.OnWorldLeave?.Invoke();

        [HarmonyLib.HarmonyPostfix]
        [HarmonyLib.HarmonyPatch("OnJoinedRoom")]
        static void JoinedRoom() => Patches.OnWorldJoin?.Invoke();
    }
}
