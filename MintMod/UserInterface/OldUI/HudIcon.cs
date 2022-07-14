using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MintMod.Managers.Colors;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace MintMod.UserInterface.OldUI {
    class HudIcon : MintSubMod {
        public override string Name => "HUD Mute Icon Coloring";
        public override string Description => "Colors the HUD Mute Icon";

        private static HudVoiceIndicator _hudVoiceIndicator;

        internal override void OnUserInterface() {
            if (!Config.ColorHUDMuteIcon.Value) return;
            _hudVoiceIndicator = Object.FindObjectOfType<HudVoiceIndicator>();
            if (_hudVoiceIndicator == null) return;
            
            if (_hudVoiceIndicator.field_Private_Image_0 != null)
                _hudVoiceIndicator.field_Private_Image_0.color = Minty;
            if (_hudVoiceIndicator.field_Private_Image_0 != null)
                _hudVoiceIndicator.field_Private_Image_1.color = Minty;
            if (_hudVoiceIndicator.field_Private_GameObject_0 != null)
                _hudVoiceIndicator.field_Private_GameObject_0.GetComponent<Image>().color = Minty;
            if (_hudVoiceIndicator.field_Private_GameObject_1 != null)
                _hudVoiceIndicator.field_Private_GameObject_1.GetComponent<Image>().color = Minty;
            var talking = _hudVoiceIndicator.transform.Find("VoiceDot");
            if (talking != null)
                talking.GetComponentInChildren<Image>().color = Minty;
        }

        // internal static void UpdateForStreamerMode(bool o) {
        //     Color c = o ? Color.red : Minty;
        //     if (HudVoiceIndicator.field_Private_Image_0 != null)
        //         HudVoiceIndicator.field_Private_Image_0.color = c;
        //     if (HudVoiceIndicator.field_Private_Image_0 != null)
        //         HudVoiceIndicator.field_Private_Image_1.color = c;
        //     if (HudVoiceIndicator.field_Private_GameObject_0 != null)
        //         HudVoiceIndicator.field_Private_GameObject_0.GetComponent<Image>().color = c;
        //     if (HudVoiceIndicator.field_Private_GameObject_1 != null)
        //         HudVoiceIndicator.field_Private_GameObject_1.GetComponent<Image>().color = c;
        // }
    }
}
