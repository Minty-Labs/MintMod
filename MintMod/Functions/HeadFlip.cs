/*using MintMod;
using MintMod.Reflections;
using UnityEngine;
using UnityEngine.XR;
using VRC.DataModel;

namespace MintMod.Functions {
    internal class HeadFlip : MintSubMod {
        public override string Name => "Head Flipper";
        public override string Description => "Flip your head";
        
        public static bool IsHeadflipEnabled;

        private NeckMouseRotator _neckMouseRotator;
        private NeckRange _orginal, _funny = new (-300, 300, 0);
        private bool _setOrginal;

        internal override void OnUpdate() {
            if (XRDevice.isPresent) {
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.H))
                    IsHeadflipEnabled = !IsHeadflipEnabled;
                if (IsHeadflipEnabled) {
                    if (_neckMouseRotator == null)
                        _neckMouseRotator = Object.FindObjectOfType<NeckMouseRotator>();
                    if (!_setOrginal) {
                        _setOrginal = true;
                        _orginal = _neckMouseRotator.field_Public_NeckRange_0;
                    }
                    _neckMouseRotator.field_Public_NeckRange_0 = _funny;
                }
                else {
                    if (_neckMouseRotator == null && _setOrginal)
                        _neckMouseRotator.field_Public_NeckRange_0 = _orginal;
                }
            }
        }
    }
}
*/