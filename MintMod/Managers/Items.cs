using MintMod.Reflections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MintMod.Functions;
using UnhollowerBaseLib;
using UnityEngine;
using VRC;
using VRC.SDK3.Components;
using VRC.SDKBase;

namespace MintMod.Managers {
    class Items : MintSubMod {
        public override string Name => "Item Manager";
        public override string Description => "";

        public static VRC_Pickup[] cached;
        public static Il2CppArrayBase<VRCObjectSync> ObjSyncSDK3;
        public static Il2CppArrayBase<VRCSDK2.VRC_ObjectSync> ObjSyncSDK2;

        internal override void OnLevelWasLoaded(int level, string sceneName) => CacheObjects();

        private static void CacheObjects() {
            cached = UnityEngine.Object.FindObjectsOfType<VRC_Pickup>();
        }

        internal static void TPToSelf() {
            foreach (var vrcPickup in cached) {
                if (vrcPickup != null || cached != null) {
                    if (!Networking.IsOwner(Player.prop_Player_0.prop_VRCPlayerApi_0, vrcPickup.gameObject))
                        Networking.SetOwner(Player.prop_Player_0.prop_VRCPlayerApi_0, vrcPickup.gameObject);
                    vrcPickup.gameObject.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.localPosition + new Vector3(0f, 0.1f, 0f);
                }
            }
        }

        internal static void TPToPlayer(Player p) {
            foreach (var vrcPickup in cached) {
                if (vrcPickup != null || cached != null) {
                    if (!Networking.IsOwner(Player.prop_Player_0.prop_VRCPlayerApi_0, vrcPickup.gameObject))
                        Networking.SetOwner(Player.prop_Player_0.prop_VRCPlayerApi_0, vrcPickup.gameObject);
                    vrcPickup.gameObject.transform.position = p.gameObject.transform.localPosition + new Vector3(0f, 0.1f, 0f);
                }
            }
        }

        internal static void TPToOutWorld() {
            foreach (var vrcPickup in cached) {
                if (vrcPickup != null || cached != null) {
                    if (!Networking.IsOwner(Player.prop_Player_0.prop_VRCPlayerApi_0, vrcPickup.gameObject))
                        Networking.SetOwner(Player.prop_Player_0.prop_VRCPlayerApi_0, vrcPickup.gameObject);
                    vrcPickup.gameObject.transform.position = new Vector3(1000000, 1000000, 1000000);
                }
            }
        }

        internal static void Respawn() {
            if (WorldReflect.GetWorld() != null && WorldActions.isWorldSDK3()) {
                ObjSyncSDK3 = UnityEngine.Object.FindObjectsOfType<VRCObjectSync>();
                foreach (VRCObjectSync vrcobjectSync in ObjSyncSDK3) {
                    if (vrcobjectSync != null) {
                        if (!Networking.IsOwner(Player.prop_Player_0.prop_VRCPlayerApi_0, vrcobjectSync.gameObject))
                            Networking.SetOwner(Player.prop_Player_0.prop_VRCPlayerApi_0, vrcobjectSync.gameObject);
                        vrcobjectSync.Respawn();
                    }
                }
                return;
            }
            ObjSyncSDK2 = UnityEngine.Object.FindObjectsOfType<VRCSDK2.VRC_ObjectSync>();
            foreach (var vrc_ObjectSync in ObjSyncSDK2) {
                if (vrc_ObjectSync != null) {
                    if (!Networking.IsOwner(Player.prop_Player_0.prop_VRCPlayerApi_0, vrc_ObjectSync.gameObject))
                        Networking.SetOwner(Player.prop_Player_0.prop_VRCPlayerApi_0, vrc_ObjectSync.gameObject);
                    vrc_ObjectSync.Respawn();
                }
            }
        }

        #region Ro-tat-e

        internal static bool Rotate;
        private static Player _target;
        internal static float SpinSpeed, Distance = 1f;

        internal static void Toggle(Player target, bool state) {
            if (cached == null) CacheObjects();
            _target = target;
            Rotate = state;
        }

        internal static void ClearRotating() => Rotate = false;

        internal override void OnUpdate() {
            if (Rotate && VRCPlayer.field_Internal_Static_VRCPlayer_0 != null) {
                if (_target == null) {
                    ClearRotating();
                    return;
                }

                var g = new GameObject();
                var tr = g.transform;
                tr.position = (_target != null ? _target.transform.position : VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position) + new Vector3(0f, 0.2f, 0f);
                g.transform.Rotate(new Vector3(0f, 360f * Time.time * SpinSpeed, 0f));
                foreach (var vrcPickup in cached) {
                    if (Networking.GetOwner(vrcPickup.gameObject) != Networking.LocalPlayer)
                        Networking.SetOwner(Networking.LocalPlayer, vrcPickup.gameObject);
                    vrcPickup.transform.position = g.transform.position + g.transform.forward * Distance;
                    g.transform.Rotate(new Vector3(0f, cached == null ? 25 : 360 / cached.Length, 0f));
                }
                g.Destroy();
            }
        }

        #endregion
    }
}
