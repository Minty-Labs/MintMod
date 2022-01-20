using System.Collections;
using Il2CppSystem.Linq;
using MelonLoader;
using MintMod.UserInterface.QuickMenu;
using MintyLoader;
using UnityEngine;

namespace MintMod.Functions {
    internal class TooltipController : MintSubMod {
        public static TooltipController Instance;
        public override string Name => "TooltipControllerHider";
        public override string Description => "Hides the controller ghost when hovering over a button.";

        private GameObject TempLeft, TempRight;
        
        private GameObject LeftGhost {
            get {
                if (TempLeft == null) TempLeft = GameObject.Find("_Application/TrackingVolume/TrackingSteam(Clone)/SteamCamera/[CameraRig]/Controller (left)");
                return TempLeft;
            }
        }
        
        private GameObject RightGhost {
            get {
                if (TempRight == null) TempRight = GameObject.Find("_Application/TrackingVolume/TrackingSteam(Clone)/SteamCamera/[CameraRig]/Controller (right)");
                return TempRight;
            }
        }

        internal override void OnStart() => Instance = this;

        internal override void OnUserInterface() => MelonCoroutines.Start(OnLoad());

        private IEnumerator OnLoad() {
            if (!Config.HideTooltipControllers.Value) yield break;
            Con.Debug("Waiting for controllers to init");
            while (LeftGhost.gameObject == null) yield return new WaitForSeconds(1f);
            Con.Debug("Found controllers");
            Toggle(!Config.HideTooltipControllers.Value);
        }

        public void Toggle(bool value) {
            var cons = new string[] { 
                "_Application/TrackingVolume/TrackingSteam(Clone)/SteamCamera/[CameraRig]/Controller (left)",
                "_Application/TrackingVolume/TrackingSteam(Clone)/SteamCamera/[CameraRig]/Controller (right)"
            };

            foreach (var con in cons) {
                var _ = GameObject.Find(con);
                if (_ != null) {
                    for (int i = 0; i < _.transform.childCount; i++) {
                        var child = _.transform.GetChild(i).gameObject;

                        if (child.name.StartsWith("ControllerUI")) {
                            if (Config.HideTooltipControllers.Value) {
                                foreach (var mesh in child.GetComponentsInChildren<MeshRenderer>(true)) 
                                    mesh.enabled = value;
                            }
                        }
                    }
                }
            }
        }

        private bool tempVal = Config.HideTooltipControllers.Value;

        internal override void OnPrefSave() {
            if (tempVal != Config.HideTooltipControllers.Value) Toggle(!Config.HideTooltipControllers.Value);
            MintUserInterface.ControllerToolTip?.Toggle(!Config.HideTooltipControllers.Value);
        }
    }
}