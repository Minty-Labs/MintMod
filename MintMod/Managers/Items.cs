using MintMod.Reflections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VRC;
using VRC.SDK3.Components;
using VRC.SDKBase;

namespace MintMod.Managers {
    class Items : MintSubMod {
        public override string Name => "Item Manager";
        public override string Description => "";

        public static VRC_Pickup[] cached;

        internal override void OnLevelWasLoaded(int level, string sceneName) {
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
                    if (!Networking.IsOwner(p.prop_VRCPlayerApi_0, vrcPickup.gameObject))
                        Networking.SetOwner(p.prop_VRCPlayerApi_0, vrcPickup.gameObject);
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
            if (UIWrappers.GetWorld() != null && UnityEngine.Resources.FindObjectsOfTypeAll<VRCSceneDescriptor>().Count > 0) {
                var VRCSDK3PickupsSync = UnityEngine.Object.FindObjectsOfType<VRCObjectSync>();
                foreach (VRCObjectSync vrcobjectSync in VRCSDK3PickupsSync) {
                    if (vrcobjectSync != null) {
                        if (!Networking.IsOwner(Player.prop_Player_0.prop_VRCPlayerApi_0, vrcobjectSync.gameObject))
                            Networking.SetOwner(Player.prop_Player_0.prop_VRCPlayerApi_0, vrcobjectSync.gameObject);
                        vrcobjectSync.Respawn();
                    }
                }
                return;
            }
            var PickupsSync = UnityEngine.Object.FindObjectsOfType<VRCSDK2.VRC_ObjectSync>();
            foreach (var vrc_ObjectSync in PickupsSync) {
                if (vrc_ObjectSync != null) {
                    if (!Networking.IsOwner(Player.prop_Player_0.prop_VRCPlayerApi_0, vrc_ObjectSync.gameObject))
                        Networking.SetOwner(Player.prop_Player_0.prop_VRCPlayerApi_0, vrc_ObjectSync.gameObject);
                    vrc_ObjectSync.Respawn();
                }
            }
        }
    }
}
