using MelonLoader;
using System;

namespace MintMod.UserInterface {
    internal class StayMute : MintSubMod {
        private MelonPreferences_Category _stayMute;
        private MelonPreferences_Entry<bool> _enabled;
        internal override void OnStart() {
            _stayMute = MelonPreferences.CreateCategory("StayMute", "StayMute");
            _enabled = _stayMute.CreateEntry("Enabled", false, "StayMute", "Keep yourself from unmuting");
            
            DefaultTalkController.field_Private_Static_Action_0 += new Action(() => {
                if (!_enabled.Value) return;
                DefaultTalkController.Method_Public_Static_Void_Boolean_0(true);
            });
        }
    }
}