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

        internal static bool IsQMOpen;
        public static Action OnWorldJoin, OnWorldLeave;

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
            applyPatches(typeof(LeftRoomPatches));
            
            if (MelonHandler.Mods.FindIndex(i => i.Info.Name == "PortableMirrorMod") != -1)
                try { HookFbtController(); } catch (Exception e) { Con.Error(e); }

            /*
            if (Config.SpoofDeviceType.Value) {
                applyPatches(typeof(PlatformSpoof));
                Con.Debug("Patched DeviceSpoof", MintCore.isDebug);
            }
            Con.Msg($"Device Type: {(Config.SpoofDeviceType.Value ? "Quest" : "PC")}");
            */
        }

        #region Calibration Mirror
        
        private static HarmonyInstance _harmonyInstance = new("MintMod_Patches_FBT");
        
        internal override void OnUserInterface() {
            foreach (var methodInfo in typeof(VRCTrackingManager).GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)) {
                if (!methodInfo.Name.StartsWith("Method_Public_Virtual_Final_New_Void_") || methodInfo.GetParameters().Length != 0) continue;

                var callees = XrefScanner.XrefScan(methodInfo).Where(it => it.Type == XrefType.Method)
                    .Select(it => it.TryResolve()).Where(it => it != null).ToList();

                if (callees.Count != 1) continue;
                if (callees[0].DeclaringType != typeof(VRCTrackingManager) || callees[0] is not MethodInfo mi || mi.ReturnType != typeof(bool))
                    continue;
                
                _harmonyInstance.Patch(methodInfo, new HarmonyMethod(typeof(Patches), nameof(CalibratePrefix)));
            }
        }
        
        private static void CalibratePrefix(MethodBase __originalMethod) {
            if (Config.CalibrationMirror.Value) {
                Con.Debug("Called calibrate from " + __originalMethod);
                //CalibrationMirror.IsCalibrated = false;
                PortableMirror.Main.ToggleMirror();
            }
        }

        private delegate byte FbbIkInit(IntPtr a, IntPtr b, IntPtr c, IntPtr d, byte e, IntPtr n);
        private static FbbIkInit ourOriginalFbbIkInit;
        private static bool LastCalibrationWasInCustomIk;
        private static VRCFbbIkController LastInitializedController;
        
        private static void HookFbtController() {
            var instance = new HarmonyLib.Harmony("FBTCalibration");
            var fbbIkInit = typeof(VRCFbbIkController).GetMethod(nameof(VRCFbbIkController.Method_Public_Virtual_Final_New_Boolean_VRC_AnimationController_Animator_VRCPlayer_Boolean_0));
            unsafe {
                var ptr = *(IntPtr*)(IntPtr)UnhollowerUtils
                    .GetIl2CppMethodInfoPointerFieldForGeneratedMethod(fbbIkInit).GetValue(null);
                var patch = AccessTools.Method(typeof(Patches), nameof(FbbIkInitReplacement)).MethodHandle
                    .GetFunctionPointer();
                MelonUtils.NativeHookAttach((IntPtr)(&ptr), patch);
                ourOriginalFbbIkInit = Marshal.GetDelegateForFunctionPointer<FbbIkInit>(ptr);
            }
            
            var isCalibrated = AccessTools.Method(typeof(VRCTrackingManager), nameof(VRCTrackingManager.Method_Public_Static_Boolean_String_0));
            instance.Patch(isCalibrated, new HarmonyMethod(typeof(Patches), nameof(IsCalibratedForAvatarPrefix)));
        }
        
        private static byte FbbIkInitReplacement(IntPtr thisPtr, IntPtr vrcAnimController, IntPtr animatorPtr, IntPtr playerPtr, byte isLocalPlayer, IntPtr nativeMethod) {
            var __instance = new VRCFbbIkController(thisPtr);
            var animator = animatorPtr == IntPtr.Zero ? null : new Animator(animatorPtr);
            var __2 = playerPtr == IntPtr.Zero ? null : new VRCPlayer(playerPtr);
            FbbIkInitPrefix(__instance, __2, isLocalPlayer != 0);
            var result = ourOriginalFbbIkInit(thisPtr, vrcAnimController, animatorPtr, playerPtr, isLocalPlayer, nativeMethod);
            FbbIkInitPostfix(__instance, animator, isLocalPlayer != 0);
            return result;
        }
        
        private static void FbbIkInitPrefix(VRCFbbIkController __instance, VRCPlayer? __2, bool __3) {
            var vrcPlayer = __2;
            if (vrcPlayer == null) return;
            var isLocalPlayer = vrcPlayer.prop_Player_0?.prop_APIUser_0?.id == APIUser.CurrentUser?.id;
            if(isLocalPlayer != __3) Con.Warn("Computed IsLocal is different from provided");
            if (!isLocalPlayer) return;
            
            LastCalibrationWasInCustomIk = false;
            LastInitializedController = __instance;
        }
        
        private static void FbbIkInitPostfix(VRCFbbIkController __instance, Animator __1, bool __3) => Calibrate(__1.gameObject);
        
        private static void Calibrate(GameObject avatarRoot) => CalibrateCore(avatarRoot).GetAwaiter().GetResult();
        
        private static async Task CalibrateCore(GameObject avatarRoot) {
            var avatarId = avatarRoot.GetComponent<PipelineManager>().blueprintId;
            await ApplyStoredCalibration(avatarRoot, avatarId);
        }

        private static async Task ApplyStoredCalibration(GameObject avatarRoot, string avatarId) {
            if (Config.CalibrationMirror.Value) {
                //CalibrationMirror.IsCalibrated = true;
                PortableMirror.Main.ToggleMirror();
                //return Task.CompletedTask;
            }
        }
        
        private static bool IsCalibratedForAvatarPrefix(ref bool __result) => !__result;
        
        #endregion
    }

    [HarmonyLib.HarmonyPatch(typeof(VRC.UI.Elements.QuickMenu))]
    class QuickMenuPatches {
        [HarmonyLib.HarmonyPostfix]
        [HarmonyLib.HarmonyPatch("OnEnable")]
        private static void OnQuickMenuEnable() {
            Patches.IsQMOpen = true;
        }

        [HarmonyLib.HarmonyPostfix]
        [HarmonyLib.HarmonyPatch("OnDisable")]
        private static void OnQuickMenuDisable() {
            Patches.IsQMOpen = false;
        }
    }

    [HarmonyLib.HarmonyPatch]
    class NameplatePatches // OnRebuild
    {
        static IEnumerable<MethodBase> TargetMethods() {
            return typeof(PlayerNameplate).GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(x => 
                Nameplates.methodMatchRegex.IsMatch(x.Name)).Cast<MethodBase>();
        }

        static void Postfix(PlayerNameplate __instance) => Nameplates.OnRebuild(__instance);
    }

    [HarmonyLib.HarmonyPatch(typeof(VRCPlayer))]
    class VRCPlayerPatches // OnPlayerAwake
    {
        [HarmonyLib.HarmonyPostfix]
        [HarmonyLib.HarmonyPatch("Awake")]
        static void OnVRCPlayerAwake(VRCPlayer __instance) {
            Nameplates.OnVRCPlayerAwake(__instance);
        }
    }

    [HarmonyLib.HarmonyPatch(typeof(VRC_StationInternal))]
    class VRC_Station {
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
    class PingSpoof {
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
    class FrameSpoof {
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
    class PlatformSpoof {
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
    class LeftRoomPatches {
        [HarmonyLib.HarmonyPostfix]
        [HarmonyLib.HarmonyPatch("OnLeftRoom")]
        static void Yeet() => Patches.OnWorldLeave?.Invoke();

        [HarmonyLib.HarmonyPostfix]
        [HarmonyLib.HarmonyPatch("OnJoinedRoom")]
        static void JoinedRoom() => Patches.OnWorldJoin?.Invoke();
    }
}
