using ExitGames.Client.Photon;
using HarmonyLib;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VRC;

namespace MintMod.Hooks {
    class Patches : MintSubMod {
        public override string Name => "Patches";
        public override string Description => "";
        
        internal static bool useChairs;

        private static void applyPatches(Type type) {
            if (MintCore.isDebug)
                MelonLogger.Msg(ConsoleColor.Cyan, $"Applying {type.Name} patches!");
            try {
                HarmonyLib.Harmony.CreateAndPatchAll(type, "MintMod_Patches");
            } catch (Exception e) {
                MelonLogger.Error($"Failed while patching {type.Name}!\n{e}");
            }
        }

        internal override void OnStart() {
            if (MintCore.isDebug)
                MelonLogger.Msg(ConsoleColor.Cyan, "Setting up patches");

            applyPatches(typeof(NameplatePatches));
            applyPatches(typeof(PingSpoof));
            applyPatches(typeof(FrameSpoof));
            applyPatches(typeof(VRC_Station));

            if (Config.SpoofDeviceType.Value) {
                applyPatches(typeof(PlatformSpoof));
                if (MintCore.isDebug)
                    MelonLogger.Msg(ConsoleColor.Cyan, "Patched DeviceSpoof");
            }
            MelonLogger.Msg($"Device Type: {(Config.SpoofDeviceType.Value ? "Quest" : "PC")}");
        }
    }

    [HarmonyPatch]
    class NameplatePatches // OnRebuild
    {
        /*static IEnumerable<MethodBase> TargetMethods() {
            return typeof(PlayerNameplate).GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(x => 
                NameplateRedo.methodMatchRegex.IsMatch(x.Name)).Cast<MethodBase>();
        }

        static void Postfix(PlayerNameplate __instance) => NameplateRedo.OnRebuild(__instance);*/
    }

    [HarmonyPatch(typeof(VRC_StationInternal))]
    class VRC_Station {
        [HarmonyPrefix]
        [HarmonyPatch("Method_Public_Boolean_Player_Boolean_0")]
        static bool PlayerCanUseStation(ref bool __result, VRC.Player __0, bool __1) {
            if (__0 != null && __0 == VRCPlayer.field_Internal_Static_VRCPlayer_0._player && !Patches.useChairs) {
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
