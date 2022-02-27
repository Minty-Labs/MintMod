using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using ColorLib;
using Newtonsoft.Json;
using UnityEngine;
using VRC.Core;

namespace MintyNameplates {
    public class Players {
        internal static bool Z29vZAEE;
        internal static QXR0cmlidXRlcwEE json;

        internal static void UnVuRmlyc3QE() {
            var VersionChecker = new HttpClient();
            VersionChecker.DefaultRequestHeaders.Add("User-Agent", MNSABuildInfo.Name);
            var checkedVer = VersionChecker.GetStringAsync("https://mintlily.lgbt/mod/Modules/mnsaver.json").GetAwaiter().GetResult();

            json = JsonConvert.DeserializeObject<QXR0cmlidXRlcwEE>(checkedVer);
        }
        public static void FetchCustomPlayerObjects(bool refreshed = false) {
            try {
                if (Storage != null && refreshed) Storage.Clear();
            } catch (Exception w) {
                Main.Log.Error(w);
            }
            
            try {
                var w = new WebClient();
                w.Headers.Add("X-AUTH-TOKEN", APIUser.CurrentUser.id);
                var data = w?.DownloadString("https://api.potato.moe/api-mint/nameplates");
                w.Dispose();
                Storage = new Dictionary<string, CustomPlayerObjects>();
                var c = JsonConvert.DeserializeObject<List<CustomPlayerObjects>>(data);
                foreach (var d in c) {
                    if (!Storage.ContainsKey(d.userID))
                        Storage.Add(d.userID, d);
                }
            } catch (Exception w) {
                Main.Log.Error(w);
            }
        }

        internal static Dictionary<string, CustomPlayerObjects> Storage;
    }
    
    public class QXR0cmlidXRlcwEE {
        public string VmVyc2lvbgEE { get; set; }
        public bool Y2FuUnVu { get; set; }
    }
    
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
}