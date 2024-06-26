﻿using MelonLoader;
using MintMod.Libraries;
using MintMod.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Math.Raw;
using MintMod.ExtraJSONData;
using MintMod.UserInterface.QuickMenu;
using MintyLoader;
using ReMod.Core.VRChat;
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
        
        public static readonly Regex MethodMatchRegex = new("Method_Public_Void_\\d", RegexOptions.Compiled);
        private static bool _privateServerRanNoticeOnce;

        internal override void OnStart() {
            if (ModCompatibility.MintyNameplates || ModCompatibility.GPrivateServer) {
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
                     player.field_Internal_GameObject_0.name.IndexOf("Avatar_Utility_Base_", StringComparison.Ordinal) == 0);
        }

        static void OnAvatarIsReady(VRCPlayer vrcPlayer) {
            if (ModCompatibility.MintyNameplates) return;
            // if (MintUserInterface.isStreamerModeOn) return;
            if (!Config.EnableCustomNameplateReColoring.Value) return;
            if (ModCompatibility.GPrivateServer) {
                if (_privateServerRanNoticeOnce) return;
                Con.Msg("Minty Nameplates are disabled");
                _privateServerRanNoticeOnce = true;
                return;
            }

            if (!ValidatePlayerAvatar(vrcPlayer)) return;
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
            catch (Exception n) { Con.Error($"Could not apply nameplate color\n{n}"); }

            helper.OnRebuild();
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
            string tagText = null,
            Color? tagBgColour = null,
            bool disableBgImage = false,
            Color? tagFontColour = null,
            bool removeBgImg = false,
            string forceFakeName = "") {
            
            if (helper == null)
                return;
            if (!Config.EnableCustomNameplateReColoring.Value)
                return;

            #region Nameplate Value Changes

            if (resetToDefaultMat) {
                helper.uiNameBackground.material = null;
                helper.uiQuickStatsBackground.material = null;
                helper.uiIconBackground.material = null;
            }

            //Are we setting BGColor?
            if (bgColor.HasValue && !bgColorLerp.HasValue) {
                var bgColorMain = bgColor.Value;
                var quickStatsBgColor = bgColor.Value;

                helper.uiNameBackground.color = bgColorMain;
                helper.uiQuickStatsBackground.color = quickStatsBgColor;

                helper.uiIconBackground.color = bgColor.Value;

                helper.SetBGColour(bgColor.Value);

                if (removeBgImg)
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

            var userId = nameplate.field_Private_VRCPlayer_0._player.GetAPIUser().id;

            if (ModCompatibility.OldMate && OldMate.Contains(userId))
                helper.SetName(OldMate.GetModifiedName(userId));
            else if (!string.IsNullOrWhiteSpace(forceFakeName))
                helper.SetName(forceFakeName);

            #endregion

            // Create and set Extra Text
            if (!Config.EnabledMintTags.Value) return;
            if (string.IsNullOrWhiteSpace(tagText)) return;
            try {
                var transform = nameplate.transform.Find("Contents");
                var transform4 = transform.Find("Quick Stats");
                
                MintTag = transform.Find("Mint_CustomTag");
                if (MintTag == null) {
                    MintTag = UnityEngine.Object.Instantiate(transform4, transform4.parent, false);
                    MintTag.name = "Mint_CustomTag";
                    
                    MoveMintTag(MintTag, Config.MintTagVerticleLocation.Value);
                    var component = MintTag.Find("Trust Text").GetComponent<TextMeshProUGUI>();
                    component.richText = true;
                    component.text = tagText;
                    component.color = tagFontColour ?? ColorConversion.HexToColor("eeeeee");
                    
                    if (disableBgImage)
                        MintTag.GetComponent<ImageThreeSlice>().enabled = false;
                    else 
                        MintTag.GetComponent<ImageThreeSlice>().color = tagBgColour ?? Color.white;
                    
                    for (var i = MintTag.childCount; i > 0; i--) {
                        var child = MintTag.GetChild(i - 1);
                        if (child.name != "Trust Text")
                            UnityEngine.Object.Destroy(child.gameObject);
                    }
                } else MintTag.gameObject.SetActive(true);
                
            } catch (Exception e) { if (MintCore.IsDebug) Con.Error(e); }
        }

        
        /*private static Transform MonkeTag { get; set; }
        
        private static void AddMonkeTagToPlayer(Component nameplate, MintyNameplateHelper helper,
            string tagText = null,
            Color? tagBgColour = null,
            bool disableBgImage = false,
            Color? tagFontColour = null) {
            
            if (string.IsNullOrWhiteSpace(tagText)) return;
            try {
                var transform = nameplate.transform.Find("Contents");
                var transform4 = transform.Find("Quick Stats");
                
                MonkeTag = transform.Find("KnownMonkeTag");
                if (MonkeTag == null) {
                    MonkeTag = UnityEngine.Object.Instantiate(transform4, transform4.parent, false);
                    MonkeTag.name = "KnownMonkeTag";
                    
                    MonkeTag.transform.localPosition = new Vector3(0f, 210, 0f);
                    var component = MonkeTag.Find("Trust Text").GetComponent<TextMeshProUGUI>();
                    component.richText = true;
                    component.text = tagText;
                    component.color = tagFontColour ?? ColorConversion.HexToColor("eeeeee");
                    
                    if (disableBgImage)
                        MonkeTag.GetComponent<ImageThreeSlice>().enabled = false;
                    else 
                        MonkeTag.GetComponent<ImageThreeSlice>().color = tagBgColour ?? Color.white;
                    
                    for (var i = MonkeTag.childCount; i > 0; i--) {
                        var child = MonkeTag.GetChild(i - 1);
                        if (child.name != "Trust Text")
                            UnityEngine.Object.Destroy(child.gameObject);
                    }
                } else MonkeTag.gameObject.SetActive(true);
            } catch (Exception e) { if (MintCore.IsDebug) Con.Error(e); }
        }*/

        private static Transform MintTag { get; set; }

        internal override void OnPrefSave() {
            if (!Config.EnabledMintTags.Value) return;
            if (MintTag == null) return;
            MoveMintTag(MintTag, Config.MintTagVerticleLocation.Value);
        }

        private static void MoveMintTag(Transform transform, float y) => transform.localPosition = new Vector3(0f, y, 0f);

        private static void ApplyFriendsRankColor(MintyNameplateHelper helper, Color rankColor) {
            if (helper == null) return;
            helper.SetNameColour(rankColor);
            helper.OnRebuild();
        }

        private static void ApplyNameplatesFromValues(PlayerNameplate nameplate, MintyNameplateHelper helper) {
            if (ModCompatibility.MintyNameplates) return;
            if (!Config.EnableCustomNameplateReColoring.Value)
                return;
            var npID = nameplate.field_Private_VRCPlayer_0._player.field_Private_APIUser_0.id;

            if (Players.Storage == null) {
                Con.Error("Mint's Database Storage was empty or null");
                return;
            }
            if (Players.Storage.ContainsKey($"{npID}-{APIUser.CurrentUser.id}"))
                npID = $"{npID}-{APIUser.CurrentUser.id}"; // User's Nameplate - target for only user to see

            if (npID.StartsWith("usr_e1c908e4")) {
                Random rnd = new();
                var num = rnd.Next(0, 10);
                var chance = num > 8;
                if (chance) npID += "-retarded";
                Con.Debug($"George's random funny shown -> {chance}", MintCore.IsDebug);
            }
            
            if (Players.Storage.ContainsKey(npID)) {
                var val = Players.Storage[npID];
                ApplyNameplateColour(nameplate, helper, false, val.nameplateColor1, val.nameplateColor2, val.nameTextColor1, val.nameTextColor2, val.colorShiftLerpTime > 0, val.colorShiftLerpTime, false, val.extraTagText, val.extraTagColor, val.extraTagBackgroundHidden, val.extraTagTextColor, val.nameplateBGHidden, val.fakeName);
            } else if (Config.RecolorRanks.Value)
                if (helper.GetPlayer().field_Private_APIUser_0 != null && APIUser.IsFriendsWith(helper.GetPlayer().field_Private_APIUser_0.id))
                    ApplyFriendsRankColor(helper, ColorConversion.HexToColor(Config.FriendRankHEX.Value));

            /*try {
                if (ExtraJSONData.MonkeShitMethods.WorldClientJsonData.Any(x => x.UserId.Contains(nameplate.field_Private_VRCPlayer_0._player.GetAPIUser().id))) {
                    AddMonkeTagToPlayer(nameplate, helper, "World Client Monke", Color.black, false, ColorConversion.HexToColor("F60B0E"));
                }
            }
            catch (Exception exception) {
                Con.Debug($"[ERROR] Failed to add Monke Tag to World Client cunt\n{exception}");
            }*/
        }

        public static void OnRebuild(PlayerNameplate nameplate) {
            if (ModCompatibility.MintyNameplates) return;
            if (nameplate == null || nameplate.gameObject == null) return;
            var helper = nameplate.gameObject.GetComponent<MintyNameplateHelper>();
            if (helper != null)
                helper.OnRebuild();
            else {
                //Nameplate doesn't have a helper, lets fix that
                if (nameplate.field_Private_VRCPlayer_0 == null) return;
                if (nameplate.field_Private_VRCPlayer_0._player != null && nameplate.field_Private_VRCPlayer_0._player.prop_APIUser_0 != null)
                    OnAvatarIsReady(nameplate.field_Private_VRCPlayer_0);
            }
        }

        public static void OnVRCPlayerAwake(VRCPlayer vrcPlayer) {
            if (ModCompatibility.MintyNameplates) return;
            vrcPlayer.Method_Public_add_Void_OnAvatarIsReady_0(new Action(() => {
                if (vrcPlayer == null) return;
                if (vrcPlayer._player == null) return;
                if (vrcPlayer._player.prop_APIUser_0 != null)
                    OnAvatarIsReady(vrcPlayer);
            }));
        }
    }
}
