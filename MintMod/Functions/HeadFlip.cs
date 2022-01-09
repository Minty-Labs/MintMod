using MintMod;
using MintMod.Reflections;
using UnityEngine;
using UnityEngine.XR;

namespace MintMod.Functions {
    internal class HeadFlip : MintSubMod {
        public override string Name => "Head Flipper";
        public override string Description => "Flip your head";
        
        public static bool IsHeadflipEnabled;

        private NeckMouseRotator _neckMouseRotator;

        internal override void OnUpdate() {
            if (XRDevice.isPresent) {
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.H))
                    IsHeadflipEnabled = !IsHeadflipEnabled;
                if (IsHeadflipEnabled) {
                    //VRC.UserCamera.UserCameraController.field_Internal_Static_UserCameraController_0
                    //    .field_Public_Vector3_0 = new(float.MaxValue, float.MaxValue, float.MaxValue);
                    if (_neckMouseRotator == null)
                        _neckMouseRotator = VRCVrCamera.field_Private_Static_VRCVrCamera_0.GetComponentInChildren<NeckMouseRotator>();
                    _neckMouseRotator.field_Private_Vector3_1 = new(float.MaxValue, float.MaxValue, float.MaxValue);
                }
            }
        }
    }
}