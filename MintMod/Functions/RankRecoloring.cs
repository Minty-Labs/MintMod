using static MintMod.Managers.Colors;
using System.Collections;
using UnityEngine;
using MelonLoader;
using MintMod.Libraries;

namespace MintMod.Functions {
    class RankRecoloring : MintSubMod {
        public override string Name => "NameplateColoring";
        public override string Description => "Set Custom Colors for user's ranks.";

        bool _coloredNameplates = true;

        // bool IsOGTrustInstalled() => MelonHandler.Mods.Any(m => m.Info.Name.Equals("OGTrustRanks"));

        // Type OG() => MelonHandler.Mods.First((MelonMod i) => i.Info.Name == "OGTrustRanks").Assembly.GetType("OGTrustRanks.OGTrustRanks");

        internal override void OnUserInterface() => MelonCoroutines.Start(Larp());

        IEnumerator Larp() {
            switch (_coloredNameplates) {
                case true when Config.RecolorRanks.Value:
                    VRCPlayer.field_Internal_Static_Color_1 = FriendsNP;
                    VRCPlayer.field_Internal_Static_Color_2 = VisitorNP;
                    VRCPlayer.field_Internal_Static_Color_3 = NewUserNP;
                    VRCPlayer.field_Internal_Static_Color_4 = UserNP;
                    VRCPlayer.field_Internal_Static_Color_5 = KnownNP;
                    VRCPlayer.field_Internal_Static_Color_6 = TrustedNP;

                    // if (IsOGTrustInstalled()) {
                    //     try {
                    //         OG().GetField("_trustedUserColor", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, TrustedNP);
                    //         OG().GetField("_veteranUserColor", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, VeteranNP);
                    //         OG().GetField("_legendaryUserColor", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, LegendNP);
                    //     } catch (Exception ex) { Con.Error(ex); }
                    // }
                    _coloredNameplates = false;
                    break;
                case false when ModCompatibility.hasCNP_On && !ModCompatibility.emmVRCPanicMode:
                    yield break;
                case true when !Config.RecolorRanks.Value:
                    VRCPlayer.field_Internal_Static_Color_1 = ColorConversion.HexToColor("#FFFF00");
                    VRCPlayer.field_Internal_Static_Color_2 = ColorConversion.HexToColor("#CCCCCC");
                    VRCPlayer.field_Internal_Static_Color_3 = ColorConversion.HexToColor("#1778FF");
                    VRCPlayer.field_Internal_Static_Color_4 = ColorConversion.HexToColor("#2BCE5C");
                    VRCPlayer.field_Internal_Static_Color_5 = ColorConversion.HexToColor("#FF7B42");
                    VRCPlayer.field_Internal_Static_Color_6 = ColorConversion.HexToColor("#8143E6");
                    _coloredNameplates = false;
                    break;
            }

            yield return new WaitForSeconds(0.5f);
        }
    }
}
