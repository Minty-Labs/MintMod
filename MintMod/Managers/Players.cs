using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MintMod.Libraries;
using MintyLoader;
using Newtonsoft.Json;
using UnityEngine;
using VRC.Core;

namespace MintMod.Managers {
    internal class Players : MintSubMod {
        public override string Name => "Player Manager";
        public override string Description => "Find and get player info.";

        #region Players

        public const string LilyID = "usr_6d71d3be-1465-4ae9-a97c-1b304ffab93b";

        #endregion

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
