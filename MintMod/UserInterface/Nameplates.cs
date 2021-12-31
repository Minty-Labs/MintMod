using MelonLoader;
using MintMod.Libraries;
using MintMod.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MintMod.UserInterface.QuickMenu;
using MintyLoader;
using TMPro;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.UI;
using VRC;
using VRC.Core;
using Random = System.Random;

namespace MintMod.UserInterface {
    class Nameplates : MintSubMod {
        public override string Name => "MintyNameplates";
        public override string Description => "Colors Nameplates for certain people.";
        public static Regex methodMatchRegex = new("Method_Public_Void_\\d", RegexOptions.Compiled);

        internal override void OnStart() {
            try { ClassInjector.RegisterTypeInIl2Cpp<MintyNameplateHelper>(); } catch (Exception e) {
                Con.Error("Unable to Inject Nameplatehelper!\n" + e.ToString());
            }
        }

        public static bool ValidatePlayerAvatar(VRCPlayer player) {
            return !(player == null ||
                     player.isActiveAndEnabled == false ||
                     player.field_Internal_Animator_0 == null ||
                     player.field_Internal_GameObject_0 == null ||
                     player.field_Internal_GameObject_0.name.IndexOf("Avatar_Utility_Base_") == 0);
        }

        static void OnAvatarIsReady(VRCPlayer vrcPlayer) {
            if (MintUserInterface.isStreamerModeOn) return;
            if (!Config.EnableCustomNameplateReColoring.Value)
                return;
            if (ValidatePlayerAvatar(vrcPlayer)) {
                //Player player = vrcPlayer._player;

                if (vrcPlayer.field_Public_PlayerNameplate_0 == null)
                    return;

                PlayerNameplate nameplate = vrcPlayer.field_Public_PlayerNameplate_0;
                MintyNameplateHelper helper = nameplate.GetComponent<MintyNameplateHelper>();
                if (helper == null) {
                    helper = nameplate.gameObject.AddComponent<MintyNameplateHelper>();
                    helper.SetNameplate(nameplate);
                    //Logger.Debug("Fetching objects from heirarhcy");
                    helper.uiIconBackground = nameplate.gameObject.transform.Find("Contents/Icon/Background").GetComponent<Image>();
                    helper.uiUserImage = nameplate.gameObject.transform.Find("Contents/Icon/User Image").GetComponent<RawImage>();
                    helper.uiUserImageContainer = nameplate.gameObject.transform.Find("Contents/Icon").gameObject;
                    helper.uiNameBackground = nameplate.gameObject.transform.Find("Contents/Main/Background").GetComponent<ImageThreeSlice>();
                    helper.uiQuickStatsBackground = nameplate.gameObject.transform.Find("Contents/Quick Stats").GetComponent<ImageThreeSlice>();
                    helper.uiName = nameplate.gameObject.transform.Find("Contents/Main/Text Container/Name").GetComponent<TextMeshProUGUI>();
                    //Logger.Debug("Created NameplateHelper on nameplate");
                }

                try { ApplyNameplatesFromValues(nameplate, helper); } 
                catch (Exception n) { Con.Error("Could not apply nameplate color\n" + n.ToString()); }

                helper.OnRebuild();
            }
        }

        private static void ApplyNameplateColour(PlayerNameplate nameplate, MintyNameplateHelper helper,
            bool bgRainbow = false,
            Color? bgColor = null,
            Color? bgColorLerp = null,
            Color? textColor = null,
            Color? textColorLerp = null,
            bool changeLerpTime = false,
            float lerpTime = 3f,
            bool resetToDefaultMat = false,
            string TagText = null,
            Color? TagBGColour = null,
            bool disableBGImage = false,
            Color? TagFontColour = null,
            bool removeBGImg = false,
            string forceFakeName = "") {
            if (helper == null)
                return;
            if (!Config.EnableCustomNameplateReColoring.Value)
                return;

            if (resetToDefaultMat) {
                helper.uiNameBackground.material = null;
                helper.uiQuickStatsBackground.material = null;
                helper.uiIconBackground.material = null;
            }

            //Are we setting BGColor?
            if (bgColor.HasValue && !bgColorLerp.HasValue) {
                Color bgColorMain = bgColor.Value;
                Color quickStatsBGColor = bgColor.Value;

                helper.uiNameBackground.color = bgColorMain;
                helper.uiQuickStatsBackground.color = quickStatsBGColor;

                helper.uiIconBackground.color = bgColor.Value;

                helper.SetBGColour(bgColor.Value);

                if (removeBGImg)
                    helper.uiNameBackground.enabled = false;
                helper.OnRebuild();
            }

            // Check and do bgLerp
            if (bgColor.HasValue && bgColorLerp.HasValue) {
                if (bgRainbow)
                    helper.SetBGRainbow();
                else
                    helper.SetBGColourLerp(bgColor.Value, bgColorLerp.Value);
            }

            //Check if we should set the text colour
            if (textColor.HasValue && !textColorLerp.HasValue) {
                helper.SetNameColour(textColor.Value);
                helper.OnRebuild();
            }

            //Check if we should be doing a colour lerp
            if (textColor.HasValue && textColorLerp.HasValue) {
                if (changeLerpTime)
                    helper.ChangeTranistionValue(lerpTime);
                helper.SetColourLerp(textColor.Value, textColorLerp.Value);
            }

            if (!textColor.HasValue && Config.RecolorRanks.Value) {
                if (helper.GetPlayer().field_Private_APIUser_0 != null && APIUser.IsFriendsWith(helper.GetPlayer().field_Private_APIUser_0.id)) {
                    helper.SetNameColour(ColorConversion.HexToColor(Config.FriendRankHEX.Value));
                    helper.OnRebuild();
                }
            }

            if (!string.IsNullOrWhiteSpace(forceFakeName))
                helper.SetName(forceFakeName);

            // Create and set Extra Text
            if (!string.IsNullOrWhiteSpace(TagText)) {
                try {
                    Transform transform = nameplate.transform.Find("Contents");
                    Transform transform4 = transform.Find("Quick Stats");

                    Transform transform5 = transform.Find("Mint_CustomTag");
                    if (transform5 == null) {
                        transform5 = UnityEngine.Object.Instantiate<Transform>(transform4, transform4.parent, false);
                        transform5.name = "Mint_CustomTag";

                        if (ModCompatibility.ProPlates)
                            transform5.localPosition = new Vector3(0f, -90f, 0f);
                        else
                            transform5.localPosition = new Vector3(0f, -60f, 0f);
                        TextMeshProUGUI component = transform5.Find("Trust Text").GetComponent<TextMeshProUGUI>();
                        component.richText = true;
                        component.text = TagText;

                        component.color = TagFontColour.Value;

                        if (disableBGImage)
                            transform5.GetComponent<ImageThreeSlice>().enabled = false;
                        else {
                            if (TagBGColour.HasValue)
                                transform5.GetComponent<ImageThreeSlice>().color = TagBGColour.Value;
                            else
                                transform5.GetComponent<ImageThreeSlice>().color = Color.white;
                        }

                        for (int i = transform5.childCount; i > 0; i--) {
                            Transform child = transform5.GetChild(i - 1);
                            if (child.name != "Trust Text")
                                UnityEngine.Object.Destroy(child.gameObject);
                        }
                    } else transform5.gameObject.SetActive(true);

                } catch (Exception e) { Con.Debug($"{e}", MintCore.isDebug); }
            }
        }

        static void ApplyFriendsRankColor(MintyNameplateHelper helper, Color RankColor) {
            if (helper != null) {
                helper.SetNameColour(RankColor);
                helper.OnRebuild();
            }
        }

        static void ApplyNameplatesFromValues(PlayerNameplate nameplate, MintyNameplateHelper helper) {
            if (!Config.EnableCustomNameplateReColoring.Value)
                return;
            string npID = nameplate.field_Private_VRCPlayer_0._player.field_Private_APIUser_0.id;

            if (Players.Storage.ContainsKey($"{npID}-{APIUser.CurrentUser.id}"))
                npID = $"{npID}-{APIUser.CurrentUser.id}"; // User's Nameplate - target for only user to see

            if (npID.Contains("usr_e1c908e4")) {
                Random rnd = new();
                int num = rnd.Next(0, 10);
                bool chance = num > 8;
                if (chance) npID += "-retarded";
                Con.Debug($"George's random funny shown -> {chance}", MintCore.isDebug);
            }
            
            // Insert Elly's Special Nameplates for Lily
            if (npID.Contains($"{npID}-usr_6d71d3be")) {
                Random rnd2 = new();
                int num2 = rnd2.Next(1, 4);
                if (num2 == 4) npID += "-4";
                else if (num2 == 3) npID += "-3";
                else if (num2 == 2) npID += "-2";
                else if (num2 == 1) npID += "-1";
                Con.Debug($"Elly's Number of Special Nameplate -> {num2}", MintCore.isDebug);
            }
            
            if (Players.Storage.ContainsKey(npID)) {
                var val = Players.Storage[npID];
                ApplyNameplateColour(nameplate, helper, false, val.nameplateColor1, val.nameplateColor2, val.nameTextColor1, val.nameTextColor2, val.colorShiftLerpTime > 0, val.colorShiftLerpTime, false, val.extraTagText, val.extraTagColor, val.extraTagBackgroundHidden, val.extraTagTextColor, val.nameplateBGHidden, val.fakeName);
            } else if (Config.RecolorRanks.Value)
                if (helper.GetPlayer().field_Private_APIUser_0 != null && APIUser.IsFriendsWith(helper.GetPlayer().field_Private_APIUser_0.id))
                    ApplyFriendsRankColor(helper, ColorConversion.HexToColor(Config.FriendRankHEX.Value));
        }
        
        /*
        static void ApplyCustomNameplates(string playerID, PlayerNameplate nameplate, MintyNameplateHelper helper) {
            if (!Config.EnableCustomNameplateReColoring.Value)
                return;
            switch (playerID) {
                case Players.LilyID:
                    ApplyNameplateColour(nameplate, helper, false, purple, Mint, Mint, Lolite, true, 1f, false, "<color=#ff00ff>she/her</color> MintLily", Mint, true, Mint, false, "Lily");
                    break;
                case Players.ErinID:
                    ApplyNameplateColour(nameplate, helper, false, black, purple, magenta, null, false, 0, false, "Cat", black, false, magenta);
                    break;
                case Players.KaylaID:
                    ApplyNameplateColour(nameplate, helper, false, ColorConversion.HexToColor("#5bcefa"), ColorConversion.HexToColor("#f587c9"), null, null, false, 3, false, "she/her", LightPink, false, magenta);
                    break;
                case Players.PlaceholderID:
                    ApplyNameplateColour(nameplate, helper, false, white, null, ColorConversion.HexToColor("049C97"), ColorConversion.HexToColor("8E2180"), true, 1, false, "DebugText", white, true, white, true);
                    break;
                case Players.BassID:
                    ApplyNameplateColour(nameplate, helper, false, magenta, null, blue, null, false, 3, false, "Fucking Meme", black, false, magenta);
                    break;
                case Players.TechnoLogicID:
                    ApplyNameplateColour(nameplate, helper, false, cyan, null, null, null, false, 3, false, "Cutie", cyan);
                    break;
                case Players.FarmerID:
                    ApplyNameplateColour(nameplate, helper, false, magenta, purple, null, null, false, 3, false, "Undercover Trap", purple);
                    break;
                case Players.EmyID:
                    if (APIUser.CurrentUser.id == Players.RiskiID)
                        ApplyNameplateColour(nameplate, helper, false, null, null, null, null, false, 3, false, "Bread", null, false, LightPink);
                    else if (APIUser.CurrentUser.id == Players.LilyID)
                        ApplyNameplateColour(nameplate, helper, false, null, null, LightPink, null, false, 3, false, "Adorable Floof", black, false, LightPink, false, "Emy");
                    else
                        ApplyNameplateColour(nameplate, helper, false, null, null, LightPink, null, false, 3, false, "Bread", null, false, LightPink, false, "Emy");
                    break;
                case Players.SarahID:
                    ApplyNameplateColour(nameplate, helper, false, null, null, null, null, false, 3, false, "Milk Nation", Color.red, false, ColorConversion.HexToColor("3E9DE8"));
                    break;
                case Players.HerpDerpID:
                    ApplyNameplateColour(nameplate, helper, false, null, null, null, null, false, 3, false, "Melon Man Himself", ColorConversion.HexToColor("FE3C6B"), false, ColorConversion.HexToColor("79F864"));
                    break;
                case Players.JanniID:
                    if (APIUser.CurrentUser.id == Players.LilyID)
                        ApplyNameplateColour(nameplate, helper, false, ColorConversion.HexToColor("FE3C6B"), null, ColorConversion.HexToColor("79F864"), null, false, 3, false,
                        "Melon Floof", ColorConversion.HexToColor("FE3C6B"), false, ColorConversion.HexToColor("79F864"), false, "Janni");
                    else
                        ApplyNameplateColour(nameplate, helper, false, ColorConversion.HexToColor("FE3C6B"), null, ColorConversion.HexToColor("79F864"), null, false, 3, false,
                        "Melon Queen", ColorConversion.HexToColor("FE3C6B"), false, ColorConversion.HexToColor("79F864"));
                    break;
                case Players.RiskiID:
                    ApplyNameplateColour(nameplate, helper, false, null, null, null, null, false, 3, false, "Beat Saber Legacy", ColorConversion.HexToColor("4170C0"), false, ColorConversion.HexToColor("C04141"));
                    break;
                case Players.EmmyID:
                    ApplyNameplateColour(nameplate, helper, false, ColorConversion.HexToColor("DAB5E1"), null, null, null, false, 3, false, "emmVRC Queen", ColorConversion.HexToColor("FC01A1"), false, white);
                    break;
                case Players.loukylorID:
                    ApplyNameplateColour(nameplate, helper, false, null, null, null, null, false, 3, false, "KOS", black, false, Color.red);
                    break;
                case Players.DDAkebonoID:
                    ApplyNameplateColour(nameplate, helper, false, null, null, null, null, false, 3, false, "BTK", purple, false, white);
                    break;
                case Players.HordiniID:
                    ApplyNameplateColour(nameplate, helper, false, null, null, null, null, false, 3, false, "Workaholic", purple, false, white);
                    break;
                case Players.BidlinkID:
                    ApplyNameplateColour(nameplate, helper, false, cyan, magenta, null, null, false, 3, false, "House Cat", cyan, false, LightPink);
                    break;
                case Players.FuslID:
                    ApplyNameplateColour(nameplate, helper, false, null, null, null, null, false, 3, false, "Cutie~", cyan, false, white);
                    break;
                case Players.DubyaID:
                    ApplyNameplateColour(nameplate, helper, false, null, null, null, null, false, 3, false, "Life's Good", ColorConversion.HexToColor("C80651"), false, white);
                    break;
                case Players.rakosiID:
                    ApplyNameplateColour(nameplate, helper, false, null, null, null, null, false, 3, false, null, null, true, null, false, "racoochie");
                    break;
                case Players.DaviID:
                    ApplyNameplateColour(nameplate, helper, false, Mint, purple, null, null, true, 3, false, "Cutie", cyan, false, white, false, "Davivi");
                    break;
                case Players.REDACTED: {
                    Random rnd = new();
                    int num = rnd.Next(0, 100);
                    bool chance = num > 80;
                    Con.Debug($"George's random funny shown -> {chance}", MintCore.isDebug);
                    ApplyNameplateColour(nameplate, helper, false, null, null, null, null, false, 3, false, chance ? "RETARDED" : "REDACTED", null, true, white, false, "REDACTED");
                    break;
                }
                case Players.Silent:
                    ApplyNameplateColour(nameplate, helper, false, null, null, null, null, false, 0, false, null, null, false, null, false, "Elly");
                    break;
                case Players.HarlesBently:
                    if (APIUser.CurrentUser.id == Players.LilyID)
                        ApplyNameplateColour(nameplate, helper,false, HarlesPink, HarlesBlue, HarlesPink, null, false, 3, false, "Cute Floof", Color.black, false, HarlesBlue, false, "Harles");
                    break;
                #region MILK NATION
                case Players.MN_1:
                    ApplyNameplateColour(nameplate, helper, false, null, null, null, null, false, 3, false, "Milk Nation", Color.red, false, ColorConversion.HexToColor("3E9DE8"));
                    break;
                #endregion
                default:
                    if (Config.RecolorRanks.Value)
                        if (helper.GetPlayer().field_Private_APIUser_0 != null && APIUser.IsFriendsWith(helper.GetPlayer().field_Private_APIUser_0.id))
                            ApplyFriendsRankColor(helper, ColorConversion.HexToColor(Config.MenuColorHEX.Value));
                    break;
            }
        }
        */

        public static void OnRebuild(PlayerNameplate nameplate) {
            if (nameplate == null || nameplate.gameObject == null) return;
            MintyNameplateHelper helper = nameplate.gameObject.GetComponent<MintyNameplateHelper>();
            if (helper != null)
                helper.OnRebuild();
            else {
                //Nameplate doesn't have a helper, lets fix that
                if (nameplate.field_Private_VRCPlayer_0 != null)
                    if (nameplate.field_Private_VRCPlayer_0._player != null && nameplate.field_Private_VRCPlayer_0._player.prop_APIUser_0 != null)
                        OnAvatarIsReady(nameplate.field_Private_VRCPlayer_0);
            }
        }

        public static void OnVRCPlayerAwake(VRCPlayer _vrcPlayer) {
            _vrcPlayer.Method_Public_add_Void_OnAvatarIsReady_0(new Action(() => {
                if (_vrcPlayer != null) {
                    if (_vrcPlayer._player != null)
                        if (_vrcPlayer._player.prop_APIUser_0 != null)
                            OnAvatarIsReady(_vrcPlayer);
                }
            }));
        }
    }
}
