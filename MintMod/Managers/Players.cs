using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
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

        internal static void Toggle(bool state, Player target = null) {
            if (target != null)
                _target = target;
            Rotate = state;
            Physics.gravity = new(0, state ? 0 : -9.81f, 0);
            /*
            Movement.FlightEnabled = state;
            Movement.NoclipEnabled = state;
            if (MintUserInterface.MainQMFly != null)
                MintUserInterface.MainQMFly.Interactable = !state;
            if (MintUserInterface.MintQAFly != null)
                MintUserInterface.MintQAFly.Interactable = !state;
            if (MintUserInterface.MainQMNoClip != null)
                MintUserInterface.MainQMNoClip.Interactable = !state;
            if (MintUserInterface.MintQANoClip != null)
                MintUserInterface.MintQANoClip.Interactable = !state;
            */
        }

        internal override void OnUpdate() {
            if (Rotate && VRCPlayer.field_Internal_Static_VRCPlayer_0 != null) {
                if (Input.GetKey(KeyCode.P) || _target == null) {
                    Toggle(false);
                    return;
                }
                if (Input.GetAxis(ControllerBindings.LeftTrigger) == 1f && Input.GetAxis(ControllerBindings.RightTrigger) == 1f) {
                    Toggle(false);
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

        #region Mint Authentication pt.1

        internal override void OnUserInterface() => FetchCustomPlayerObjects();

        public static void FetchCustomPlayerObjects(bool refreshed = false) {
            if (ModCompatibility.GPrivateServer) return;
            try {
                if (Storage != null && refreshed) Storage.Clear();
            } catch (Exception w) {
                Con.Error(w);
            }
            
            try {
                WebClient w = new();
                w.Headers.Add("X-AUTH-TOKEN", APIUser.CurrentUser.id);
                var data = w?.DownloadString("https://api.potato.moe/api-mint/nameplates");
                w.Dispose();
                Storage = new();
                var c = JsonConvert.DeserializeObject<List<CustomPlayerObjects>>(data);
                foreach (var d in c) {
                    if (!refreshed) Con.Debug($"Adding {d.userID}", MintCore.isDebug);
                    if (!Storage.ContainsKey(d.userID))
                        Storage.Add(d.userID, d);
                }
                Con.Debug($"Counted {Storage.Count} for custom nameplates", MintCore.isDebug);
            } catch (Exception w) {
                Con.Error(w);
            }
        }

        internal static Dictionary<string, CustomPlayerObjects> Storage;
        
        #endregion
    }
    
    #region Mint Authentication pt.2
/*
    public class MintUser {
        public string UserName { get; set; }
        public string LinkedDiscordID { get; set; }
        public string UserID { get; set; }
        public string VoucherName { get; set; }
        public bool MintBanned { get; set; }
        public List<string> AltAccounts { get; set; }
        
        [JsonConstructor]
        public MintUser(string _UserName, string _LinkedDiscordID, string _UserID, string _VoucherName, bool _MintBanned, List<string> _AltAccounts) {
            this.UserName = _UserName;
            this.LinkedDiscordID = _LinkedDiscordID;
            this.UserID = _UserID;
            this.VoucherName = _VoucherName;
            this.MintBanned = _MintBanned;
            this.AltAccounts = _AltAccounts;
        }
    }
    */
    
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
