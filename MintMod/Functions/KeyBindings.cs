using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MintMod.Managers;
using UnityEngine;
using UnityEngine.XR;
using VRC.Core;
using VRC.SDKBase;

namespace MintMod.Functions {
    class KeyBindings : MintSubMod {
        public override string Name => "KeyBindings";
        public override string Description => "Keyboard controls for simple commands.";

        private static bool loaded;
        private static Ray ray;

        internal override void OnUserInterface() => loaded = true;

        internal override void OnUpdate() {
            if (Config.EnableAllKeybindings.Value && loaded) {
                /*if (!XRDevice.isPresent) {
                     {
                        if (MintFunc.FunStuff.HeadFlip.IsHeadflipEnabled == false) {
                            MintFunc.FunStuff.HeadFlip.IsHeadflipEnabled = true;
                            ConsoleLogger.Log("[KeyBind] Head flipper ON");
                        } else {
                            MintFunc.FunStuff.HeadFlip.IsHeadflipEnabled = false;
                            ConsoleLogger.Log("[KeyBind] Head flipper OFF");
                            try { PopupManagerUtils.InformHudText(VRCUiManager.prop_VRCUiManager_0, "Open A full screen menu to fully disable Head Flip", UnityEngine.Color.white); } catch { }
                        }
                    }
                }*/
                if ((Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.T)) || Input.GetKeyDown(KeyCode.Mouse3)) {
                    ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
                    if (Physics.Raycast(ray, out RaycastHit raycastHit))
                        VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position = raycastHit.point;
                } else if ((APIUser.CurrentUser != null && APIUser.CurrentUser.id == "usr_fe9b2a3f-d2e7-41ec-910a-42f1329d8be0") && Input.GetKeyDown(KeyCode.Mouse4)) {
                    ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
                    if (Physics.Raycast(ray, out RaycastHit raycastHit))
                        VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position = raycastHit.point;
                }
                //if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.J))
                //    Utils.AddJump();
            }
        }
    }
}
