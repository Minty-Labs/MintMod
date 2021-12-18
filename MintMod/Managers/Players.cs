using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MintMod.Functions;
using MintMod.Libraries;
using MintMod.Reflections;
using MintMod.UserInterface.QuickMenu;
using MintyLoader;
using Newtonsoft.Json;
using UnityEngine;
using VRC;
using VRC.Core;

namespace MintMod.Managers {
    internal class Players : MintSubMod {
        public override string Name => "Player Manager";
        public override string Description => "Find and get player info.";

        #region Ro-tat-e

        private static Player _target;
        internal static bool Rotate;
        internal static float SelfSpinSpeed = 1, SelfDistance = 1;

        internal static void Toggle(Player target, bool state) {
            _target = target;
            Rotate = state;
            Movement.FlightEnabled = state;
            Movement.NoclipEnabled = state;
        }

        internal static void ClearRotating() {
            Rotate = false;
            Movement.FlightEnabled = true;
            Movement.NoclipEnabled = true;
            Movement.FlightEnabled = false;
            Movement.NoclipEnabled = false;
            if (MintUserInterface.MainQMFly != null)
                MintUserInterface.MainQMFly.Toggle(false);
            if (MintUserInterface.MintQAFly != null)
                MintUserInterface.MintQAFly.Toggle(false);
            if (MintUserInterface.MainQMNoClip != null)
                MintUserInterface.MainQMNoClip.Toggle(false);
            if (MintUserInterface.MintQANoClip != null)
                MintUserInterface.MintQANoClip.Toggle(false);
        }

        internal override void OnUpdate() {
            if (Rotate && VRCPlayer.field_Internal_Static_VRCPlayer_0 != null) {
                if (Input.GetKey(KeyCode.P) || _target == null) {
                    ClearRotating();
                    return;
                }
                if (Input.GetAxis(ControllerBindings.LeftTrigger) == 1f && Input.GetAxis(ControllerBindings.RightTrigger) == 1f) {
                    ClearRotating();
                    return;
                }

                var g = new GameObject();
                var loc = _target.transform.position;
                g.transform.position = loc;
                
                //Movement.FlightEnabled = true;
                //Movement.NoclipEnabled = true;
                
                g.transform.Rotate(new Vector3(0f, 1f, 0f), Time.time * SelfSpinSpeed * 90f);
                VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position = 
                    g.transform.position + g.transform.forward * SelfDistance;
                g.Destroy();
            }
        }

        #endregion

        #region Players

        public const string LilyID = "usr_6d71d3be-1465-4ae9-a97c-1b304ffab93b";

        #endregion

        #region Mint Authentication pt.1
        
        internal override void OnUserInterface() {
            try {
                WebClient w = new();
                w.Headers.Add("X-AUTH-TOKEN", APIUser.CurrentUser.id);
                string data = w?.DownloadString("https://api.potato.moe/api-mint/nameplates");
                w.Dispose();
                Storage = new();
                List<CustomPlayerObjects> c = JsonConvert.DeserializeObject<List<CustomPlayerObjects>>(data);
                foreach (var d in c) {
                    Con.Debug($"Adding {d.userID}", MintCore.isDebug);
                    if (!Storage.ContainsKey(d.userID))
                        Storage.Add(d.userID, d);
                }
                Con.Debug($"Counted {Storage.Count} for custom nameplates", MintCore.isDebug);
            } catch (Exception w) {
                Con.Error($"{w}");
            }
        }

        internal static Dictionary<string, CustomPlayerObjects> Storage;
        
        #endregion
    }
    
    #region Mint Authentication pt.2
    
    public class CustomPlayerObjects {
        public string userID { get; set; }
        public string fakeName { get; set; }
        public Color? nameplateColor1 { get; set; }
        public Color? nameplateColor2 { get; set; }
        public bool nameplateBGHidden { get; set; }
        public Color? nameTextColor1 { get; set; }
        public Color? nameTextColor2 { get; set; }
        public float colorShiftLerpTime { get; set; } = 3f;
        public string extraTagText { get; set; }
        public bool extraTagBackgroundHidden { get; set; }
        public Color? extraTagColor { get; set; }
        public Color? extraTagTextColor { get; set; }

        [JsonConstructor]
        public CustomPlayerObjects(string userID, string fakeName, string nameplateColor1, string nameplateColor2, bool nameplateBGHidden, string nameTextColor1, string nameTextColor2, float colorShiftLerpTime, string extraTagText, bool extraTagBackgroundHidden, string extraTagColor, string extraTagTextColor) {
            this.userID = userID;
            this.fakeName = fakeName;
            this.nameplateColor1 = ColorConversion.HexToColorNullable(nameplateColor1);
            this.nameplateColor2 = ColorConversion.HexToColorNullable(nameplateColor2);
            this.nameplateBGHidden = nameplateBGHidden;
            this.nameTextColor1 = ColorConversion.HexToColorNullable(nameTextColor1);
            this.nameTextColor2 = ColorConversion.HexToColorNullable(nameTextColor2);
            this.colorShiftLerpTime = colorShiftLerpTime;
            this.extraTagText = extraTagText;
            this.extraTagBackgroundHidden = extraTagBackgroundHidden;
            this.extraTagColor = ColorConversion.HexToColorNullable(extraTagColor);
            this.extraTagTextColor = ColorConversion.HexToColorNullable(extraTagTextColor);
        }
    }
    
    #endregion
}
