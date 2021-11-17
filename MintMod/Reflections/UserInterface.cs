using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Reflection;
using VRC.Core;

namespace MintMod.Reflections {
    public static class UIWrappers {
        private static QuickMenu quickmenuInstance;
        public static QuickMenu GetQuickMenuInstance() {
            if (quickmenuInstance == null)
                quickmenuInstance = QuickMenu.prop_QuickMenu_0;
            return quickmenuInstance;
        }

        static VRCUiManager vrcuimInstance;
        public static VRCUiManager GetVRCUiMInstance() {
            if (vrcuimInstance == null)
                vrcuimInstance = VRCUiManager.prop_VRCUiManager_0;
            return vrcuimInstance;
        }

        public static GameObject menuContent(this VRCUiManager mngr) => mngr.field_Public_GameObject_0;

        public static void DestroyComponent<T>(this GameObject go) where T : Component => Component.Destroy(go.GetComponent<T>());

        public static void DestroyComponentInChildren<T>(this GameObject go) where T : Component => Component.Destroy(go.GetComponentInChildren<T>());

        public static void Destroy(this GameObject go) => GameObject.Destroy(go);

        public static bool IsInWorld() => GetWorld() != null || GetWorldInstance() != null;

        public static ApiWorld GetWorld() => RoomManager.field_Internal_Static_ApiWorld_0;

        public static ApiWorldInstance GetWorldInstance() => RoomManager.field_Internal_Static_ApiWorldInstance_0;

        public enum SDKType {
            NONE,
            SDK2,
            SDK3
        }
    }
}

namespace MintMod.Reflections {
    using ActionMenuApi.Pedals;
    class ActionMenuUtils {
        internal static PedalSubMenu subMenu;
    }
}
