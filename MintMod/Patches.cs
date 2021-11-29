using ExitGames.Client.Photon;
using HarmonyLib;
using MelonLoader;
using MintMod.UserInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MintMod.Functions;
using MintyLoader;
using UnityEngine;
using VRC;

namespace MintMod.Hooks {
    class Patches : MintSubMod {
        public override string Name => "Patches";
        public override string Description => "";

        internal static bool IsQMOpen;

        private static void applyPatches(Type type) {
            Con.Debug($"Applying {type.Name} patches!", MintCore.isDebug);
            try {
                HarmonyLib.Harmony.CreateAndPatchAll(type, "MintMod_Patches");
            } catch (Exception e) {
                Con.Error($"Failed while patching {type.Name}!\n{e}");
            }
        }

        internal override void OnStart() {
            Con.Debug("Setting up patches", MintCore.isDebug);

            applyPatches(typeof(NameplatePatches));
            applyPatches(typeof(PingSpoof));
            applyPatches(typeof(FrameSpoof));
            applyPatches(typeof(VRC_Station));
            applyPatches(typeof(NameplatePatches));
            applyPatches(typeof(VRCPlayerPatches));
            applyPatches(typeof(QuickMenuPatches));

            if (Config.SpoofDeviceType.Value) {
                applyPatches(typeof(PlatformSpoof));
                Con.Debug("Patched DeviceSpoof", MintCore.isDebug);
            }
            Con.Msg($"Device Type: {(Config.SpoofDeviceType.Value ? "Quest" : "PC")}");
        }
    }

    [HarmonyPatch(typeof(VRC.UI.Elements.QuickMenu))]
    class QuickMenuPatches {
        [HarmonyPostfix]
        [HarmonyPatch("OnEnable")]
        private static void OnQuickMenuEnable() {
            Patches.IsQMOpen = true;
        }

        [HarmonyPostfix]
        [HarmonyPatch("OnDisable")]
        private static void OnQuickMenuDisable() {
            Patches.IsQMOpen = false;
        }
    }

    [HarmonyPatch]
    class NameplatePatches // OnRebuild
    {
        static IEnumerable<MethodBase> TargetMethods() {
            return typeof(PlayerNameplate).GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(x => 
                Nameplates.methodMatchRegex.IsMatch(x.Name)).Cast<MethodBase>();
        }

        static void Postfix(PlayerNameplate __instance) => Nameplates.OnRebuild(__instance);
    }

    [HarmonyPatch(typeof(VRCPlayer))]
    class VRCPlayerPatches // OnPlayerAwake
    {
        [HarmonyPostfix]
        [HarmonyPatch("Awake")]
        static void OnVRCPlayerAwake(VRCPlayer __instance) {
            Nameplates.OnVRCPlayerAwake(__instance);
            MasterFinder.OnAvatarIsReady(__instance);
        }
    }

    [HarmonyPatch(typeof(VRC_StationInternal))]
    class VRC_Station {
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
    class PingSpoof {
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
    class FrameSpoof {
        [HarmonyPrefix]
        [HarmonyPatch("smoothDeltaTime", MethodType.Getter)]
        static bool Prefix(ref float __result) {
            if (!Config.SpoofFramerate.Value)
                return true;
            __result = 1f / Config.SpoofedFrameNumber.Value;
            return false;
        }
    }

    [HarmonyPatch(typeof(Tools))]
    class PlatformSpoof {
        [HarmonyPostfix]
        [HarmonyPatch("Platform", MethodType.Getter)]
        static void Postfix(ref string __result) {
            try {
                __result = Config.SpoofDeviceType.Value ? "android" : "standalonewindows";
                Tools._platform = __result;
            } catch { }
        }
    }
}
