using MintMod.Reflections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MintMod.UserInterface.QuickMenu;
using UnityEngine;
using VRC;
using VRC.Networking;

namespace MintMod.Functions {
    class PhotonFreeze {
        private static Transform CloneObj;
        private static FlatBufferNetworkSerializer Thing;
        //public static bool Freeze;
        public static Vector3 TempPos;
        public static Quaternion TempRot;

        static void Clone(bool Toggle) {
            if (Toggle) {
                CloneObj = UnityEngine.Object.Instantiate<Transform>(PlayerWrappers.GetCurrentPlayer().prop_VRCAvatarManager_0.transform.Find("Avatar"), null, true);
                CloneObj.name = "Cloned Frozen Avatar";
                CloneObj.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position;
                CloneObj.transform.rotation = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.rotation;
                Animator component = CloneObj.GetComponent<Animator>();
                if (component != null && component.isHuman) {
                    Transform boneTransform = component.GetBoneTransform(HumanBodyBones.Head);
                    if (boneTransform != null)
                        boneTransform.localScale = Vector3.one;
                }
                foreach (Component component2 in CloneObj.GetComponents<Component>())
                    if (!(component2 is Transform))
                        UnityEngine.Object.Destroy(component2);
                Tools.SetLayerRecursively(CloneObj.gameObject, LayerMask.NameToLayer("Player"), 0);
                return;
            }
            CloneObj.gameObject.Destroy();
        }

        public static void ToggleFreeze(bool toggle) {
            //Freeze = !Freeze;
            if (VRCPlayer.field_Internal_Static_VRCPlayer_0 == null) return;
            Thing = VRCPlayer.field_Internal_Static_VRCPlayer_0.prop_VRCPlayerApi_0.gameObject.GetComponent<FlatBufferNetworkSerializer>();
            Thing.enabled = !toggle;
            TempPos = PlayerWrappers.GetCurrentPlayerPos();
            TempRot = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.rotation;
            Clone(toggle);
            if (MintUserInterface.MainQMFreeze != null)
                MintUserInterface.MainQMFreeze.Toggle(toggle);
            if (MintUserInterface.MintQAFreeze != null)
                MintUserInterface.MintQAFreeze.Toggle(toggle);
        }
    }
}
