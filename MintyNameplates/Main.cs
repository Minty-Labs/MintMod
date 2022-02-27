using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using ColorLib;
using HarmonyLib;
using MelonLoader;
using MintMod.Managers;
using TMPro;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.UI;
using VRC;
using VRC.Core;

namespace MintyNameplates {
    public class MNSABuildInfo {
        public const string Name = "MintyNameplates";
        public const string Author = "Lily";
        public const string Company = "Minty Labs";
        public const string Version = "1.0.1";
        public const string DownloadLink = null;
        public const string UpdatedDate = "22 Feb 2022";
    }
    
    public class Main : MelonMod {
        internal static readonly MelonLogger.Instance Log = new MelonLogger.Instance("MintyLoader", ConsoleColor.Magenta);
        public static Regex methodMatchRegex = new Regex("Method_Public_Void_\\d", RegexOptions.Compiled);
        private static bool ProPlates, stoppedLoad;
        private static int _scenesLoaded = 0;

        public override void OnApplicationStart() {
            Players.UnVuRmlyc3QE();
            Log.Msg($"Running MintyNameplatesSA v{MNSABuildInfo.Version} built on {MNSABuildInfo.UpdatedDate}");
            if (!Players.json.Y2FuUnVu || Players.json.VmVyc2lvbgEE != MNSABuildInfo.Version) {
                Log.Warning("Cannot load mod. Mod might be out of date.");
                stoppedLoad = true;
                return;
            }
            
            applyPatches(typeof(NameplatePatches));
            applyPatches(typeof(VRCPlayerPatches));
            try { ClassInjector.RegisterTypeInIl2Cpp<MintyNameplateHelperSA>(); } catch (Exception e) {
                Log.Error($"Unable to Inject NameplateHelper!\n{e}");
            }
            ProPlates = MelonHandler.Mods.FindIndex(i => i.Info.Name == "ProPlates") != -1;
        }
        
        public override void OnSceneWasLoaded(int buildIndex, string sceneName) {
            if (stoppedLoad) return;
            if (_scenesLoaded <= 2) {
                _scenesLoaded++;
                if (_scenesLoaded == 2)
                    Players.FetchCustomPlayerObjects();
            }
        }
        
        private static void applyPatches(Type type) {
            try {
                HarmonyLib.Harmony.CreateAndPatchAll(type, "MintyNameplate_Patches");
            } catch (Exception e) {
                Log.Error(e);
            }
        }
        
        #region Main Nameplate Method
        
        public static bool ValidatePlayerAvatar(VRCPlayer player) {
            return !(player == null ||
                     player.isActiveAndEnabled == false ||
                     player.field_Internal_Animator_0 == null ||
                     player.field_Internal_GameObject_0 == null ||
                     player.field_Internal_GameObject_0.name.IndexOf("Avatar_Utility_Base_") == 0);
        }
        
        static void OnAvatarIsReady(VRCPlayer vrcPlayer) {
            if (stoppedLoad) return;
            if (ValidatePlayerAvatar(vrcPlayer)) {
                if (vrcPlayer.field_Public_PlayerNameplate_0 == null)
                    return;

                PlayerNameplate nameplate = vrcPlayer.field_Public_PlayerNameplate_0;
                MintyNameplateHelperSA helper = nameplate.GetComponent<MintyNameplateHelperSA>();
                if (helper == null) {
                    helper = nameplate.gameObject.AddComponent<MintyNameplateHelperSA>();
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
                catch (Exception n) { Log.Error($"Could not apply nameplate color\n{n}"); }

                helper.OnRebuild();
            }
        }
        
        static void ApplyNameplatesFromValues(PlayerNameplate nameplate, MintyNameplateHelperSA helper) {
            string npID = nameplate.field_Private_VRCPlayer_0._player.field_Private_APIUser_0.id;

            if (Players.Storage.ContainsKey($"{npID}-{APIUser.CurrentUser.id}"))
                npID = $"{npID}-{APIUser.CurrentUser.id}"; // User's Nameplate - target for only user to see

            if (npID.Contains("usr_e1c908e4")) {
                var rnd = new System.Random();
                int num = rnd.Next(0, 10);
                bool chance = num > 8;
                if (chance) npID += "-retarded";
            }
            
            if (Players.Storage.ContainsKey(npID)) {
                var val = Players.Storage[npID];
                ApplyNameplateColour(nameplate, helper, false, val.nameplateColor1, val.nameplateColor2, val.nameTextColor1, val.nameTextColor2, val.colorShiftLerpTime > 0, val.colorShiftLerpTime, false, val.extraTagText, val.extraTagColor, val.extraTagBackgroundHidden, val.extraTagTextColor, val.nameplateBGHidden, val.fakeName);
            } 
            //else if (Config.RecolorRanks.Value)
            //    if (helper.GetPlayer().field_Private_APIUser_0 != null && APIUser.IsFriendsWith(helper.GetPlayer().field_Private_APIUser_0.id))
            //        ApplyFriendsRankColor(helper, ColorConversion.HexToColor(Config.FriendRankHEX.Value));
        }
        
        static void ApplyFriendsRankColor(MintyNameplateHelperSA helper, Color RankColor) {
            if (helper != null) {
                helper.SetNameColour(RankColor);
                helper.OnRebuild();
            }
        }
        
        private static void ApplyNameplateColour(PlayerNameplate nameplate, MintyNameplateHelperSA helper,
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

            //if (!textColor.HasValue && Config.RecolorRanks.Value) {
            //    if (helper.GetPlayer().field_Private_APIUser_0 != null && APIUser.IsFriendsWith(helper.GetPlayer().field_Private_APIUser_0.id)) {
            //        helper.SetNameColour(ColorConversion.HexToColor(Config.FriendRankHEX.Value));
            //        helper.OnRebuild();
            //    }
            //}

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

                        if (ProPlates)
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

                } catch (Exception e) { Log.Error(e); }
            }
        }
        
        public static void OnRebuild(PlayerNameplate nameplate) {
            if (stoppedLoad) return;
            if (nameplate == null || nameplate.gameObject == null) return;
            MintyNameplateHelperSA helper = nameplate.gameObject.GetComponent<MintyNameplateHelperSA>();
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
            if (stoppedLoad) return;
            vrcPlayer.Method_Public_add_Void_OnAvatarIsReady_0(new Action(() => {
                if (vrcPlayer != null) {
                    if (vrcPlayer._player != null)
                        if (vrcPlayer._player.prop_APIUser_0 != null)
                            OnAvatarIsReady(vrcPlayer);
                }
            }));
        }
        
        #endregion
    }

    #region Patches

    [HarmonyPatch]
    class NameplatePatches {
        static IEnumerable<MethodBase> TargetMethods() {
            return typeof(PlayerNameplate).GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(x => 
                Main.methodMatchRegex.IsMatch(x.Name)).Cast<MethodBase>();
        }
        static void Postfix(PlayerNameplate __instance) => Main.OnRebuild(__instance);
    }
    
    [HarmonyPatch(typeof(VRCPlayer))]
    class VRCPlayerPatches {
        [HarmonyPostfix]
        [HarmonyPatch("Awake")]
        static void OnVRCPlayerAwake(VRCPlayer __instance) => Main.OnVRCPlayerAwake(__instance);
    }

    #endregion
}