using MintMod.Libraries;
using UnityEngine;

namespace MintMod.Managers {
    class Colors {
        public static Color Minty = ColorConversion.HexToColor("#" + Config.MenuColorHEX.Value);
        public static Color defaultMenuColor() => new(0.05f, 0.65f, 0.68f);

        public static Color FriendsNP = ColorConversion.HexToColor("#" + Config.FriendRankHEX.Value);
        public static Color TrustedNP = ColorConversion.HexToColor("#" + Config.TrustedRankHEX.Value);
        public static Color KnownNP = ColorConversion.HexToColor("#" + Config.KnownRankHEX.Value);
        public static Color UserNP = ColorConversion.HexToColor("#" + Config.UserRankHEX.Value);
        public static Color NewUserNP = ColorConversion.HexToColor("#" + Config.NewUserRankKEX.Value);
        public static Color VisitorNP = ColorConversion.HexToColor("#" + Config.VisitorRankHEX.Value);
        public static Color VeteranNP = ColorConversion.HexToColor("#" + Config.VeteranRankHEX.Value);
        public static Color LegendNP = ColorConversion.HexToColor("#" + Config.LegendRankHEX.Value);
    }
}
