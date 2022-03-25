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
    internal class Nameplates : MintSubMod {
        public override string Name => "MintyNameplates";
        public override string Description => "Colors Nameplates for certain people.";
        public static Regex methodMatchRegex = new("Method_Public_Void_\\d", RegexOptions.Compiled);
        private static bool PrivateServerRanNoticeOnce;

        internal override void OnStart() {
            if (ModCompatibility.MintyNameplates) {
                Con.Msg("Standalone MintyNameplates found.");
                return;
            }
            try { ClassInjector.RegisterTypeInIl2Cpp<MintyNameplateHelper>(); } catch (Exception e) {
                Con.Error($"Unable to Inject NameplateHelper!\n{e}");
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
            if (ModCompatibility.MintyNameplates) return;
            if (MintUserInterface.isStreamerModeOn) return;
            if (!Config.EnableCustomNameplateReColoring.Value) return;
            if (ModCompatibility.GPrivateServer) {
                if (!PrivateServerRanNoticeOnce) {
                    Con.Msg("Minty Nameplates are disabled");
                    PrivateServerRanNoticeOnce = true;
                }
                return;
            }
            if (ValidatePlayerAvatar(vrcPlayer)) {
                //Player player = vrcPlayer._player;

                if (vrcPlayer.field_Public_PlayerNameplate_0 == null)
                    return;

                var nameplate = vrcPlayer.field_Public_PlayerNameplate_0;
                var helper = nameplate.GetComponent<MintyNameplateHelper>();
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

                        component.color = TagFontColour == null ? ColorConversion.HexToColor("eeeeee") : TagFontColour.Value;

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

                } catch (Exception e) { if (MintCore.isDebug) Con.Error(e); }
            }
        }

        static void ApplyFriendsRankColor(MintyNameplateHelper helper, Color RankColor) {
            if (helper != null) {
                helper.SetNameColour(RankColor);
                helper.OnRebuild();
            }
        }

        static void ApplyNameplatesFromValues(PlayerNameplate nameplate, MintyNameplateHelper helper) {
            if (ModCompatibility.MintyNameplates) return;
            if (!Config.EnableCustomNameplateReColoring.Value)
                return;
            string npID = nameplate.field_Private_VRCPlayer_0._player.field_Private_APIUser_0.id;

            if (Players.Storage == null) {
                Con.Error("Mint's Database Storage was empty or null");
                return;
            }
            if (Players.Storage.ContainsKey($"{npID}-{APIUser.CurrentUser.id}"))
                npID = $"{npID}-{APIUser.CurrentUser.id}"; // User's Nameplate - target for only user to see

            if (npID.Contains("usr_e1c908e4")) {
                Random rnd = new();
                int num = rnd.Next(0, 10);
                bool chance = num > 8;
                if (chance) npID += "-retarded";
                Con.Debug($"George's random funny shown -> {chance}", MintCore.isDebug);
            }
            
            if (Players.Storage.ContainsKey(npID)) {
                var val = Players.Storage[npID];
                ApplyNameplateColour(nameplate, helper, false, val.nameplateColor1, val.nameplateColor2, val.nameTextColor1, val.nameTextColor2, val.colorShiftLerpTime > 0, val.colorShiftLerpTime, false, val.extraTagText, val.extraTagColor, val.extraTagBackgroundHidden, val.extraTagTextColor, val.nameplateBGHidden, val.fakeName);
            } else if (Config.RecolorRanks.Value)
                if (helper.GetPlayer().field_Private_APIUser_0 != null && APIUser.IsFriendsWith(helper.GetPlayer().field_Private_APIUser_0.id))
                    ApplyFriendsRankColor(helper, ColorConversion.HexToColor(Config.FriendRankHEX.Value));
        }

        public static void OnRebuild(PlayerNameplate nameplate) {
            if (ModCompatibility.MintyNameplates) return;
            if (nameplate == null || nameplate.gameObject == null) return;
            var helper = nameplate.gameObject.GetComponent<MintyNameplateHelper>();
            if (helper != null)
                helper.OnRebuild();
            else {
                //Nameplate doesn't have a helper, lets fix that
                if (nameplate.field_Private_VRCPlayer_0 != null)
                    if (nameplate.field_Private_VRCPlayer_0._player != null && nameplate.field_Private_VRCPlayer_0._player.prop_APIUser_0 != null)
                        OnAvatarIsReady(nameplate.field_Private_VRCPlayer_0);
            }
        }

        public static void OnVRCPlayerAwake(VRCPlayer vrcPlayer) {
            if (ModCompatibility.MintyNameplates) return;
            vrcPlayer.Method_Public_add_Void_OnAvatarIsReady_0(new Action(() => {
                if (vrcPlayer != null) {
                    if (vrcPlayer._player != null)
                        if (vrcPlayer._player.prop_APIUser_0 != null)
                            OnAvatarIsReady(vrcPlayer);
                }
            }));
        }
    }
}
