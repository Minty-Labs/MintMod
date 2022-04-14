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

        public static void DestroyImmediate(this GameObject go) => GameObject.DestroyImmediate(go);
        
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component {
            T component = gameObject.GetComponent<T>();
            if (component == null)
                return gameObject.AddComponent<T>();
            return component;
        }
    }
}
