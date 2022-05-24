﻿using MelonLoader;
using MintMod.Resources;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MintMod.Reflections;
using UnityEngine;
using static MintMod.Managers.Colors;
using MintMod.Libraries;
using MintMod.UserInterface.QuickMenu;
using MintyLoader;
using UnhollowerBaseLib;
using UnityEngine.UI;

namespace MintMod.UserInterface.OldUI {
    internal class ReColor : MintSubMod {
        public override string Name => "OldUiRecolor";
        public override string Description => "Colors the User Interface (Not the QuickMenu)";
        internal static ReColor Intstance;

        private static List<Image> _normalColorImage, _dimmerColorImage, _darkerColorImage;
        private static List<Text> _normalColorText;
        private static GameObject _loadingBackground, _initialLoadingBackground;
        private static Color _finalColor;

        internal override void OnStart() => Intstance = this;

        internal override void OnUserInterface() {
            _finalColor = Config.ColorGameMenu.Value ? Minty : defaultMenuColor();
            if (Config.ColorGameMenu.Value)
                MelonCoroutines.Start(ColorMenu(_finalColor));
            //if (Config.ColorActionMenu.Value)
            //    ColorActionMenu(finalColor);
            if (Config.ColorLoadingScreen.Value) {
                try {
                    ColorLoadingScreenEnvironment(_finalColor);
                }
                catch (Exception e) {
                    if (!MintUserInterface.isStreamerModeOn)
                        Con.Error(e);
                }
            }
        }

        private IEnumerator ColorMenu(Color color) {
            color = Config.ColorGameMenu.Value ? Minty : defaultMenuColor();

            var colorT = new Color(color.r, color.g, color.b, 0.7f);
            //var dimmer = new Color(color.r / 0.75f, color.g / 0.75f, color.b / 0.75f);
            var dimmerT = new Color(color.r / 0.75f, color.g / 0.75f, color.b / 0.75f, 0.9f);
            var darker = new Color(color.r / 2.5f, color.g / 2.5f, color.b / 2.5f);
            var darkerT = new Color(color.r / 2.5f, color.g / 2.5f, color.b / 2.5f, 0.9f);

            if (_normalColorImage == null || _normalColorImage.Count == 0) {
                #region normalColorImage

                _normalColorImage = new List<Image>();
                var quickMenu = UIWrappers.GetVRCUiMInstance().menuContent();
                _normalColorImage.Add(quickMenu.transform.Find("Screens/Settings_Safety/_Description_SafetyLevel").GetComponent<Image>());
                _normalColorImage.Add(quickMenu.transform.Find("Screens/Settings_Safety/_Buttons_SafetyLevel/Button_Custom/ON").GetComponent<Image>());
                _normalColorImage.Add(quickMenu.transform.Find("Screens/Settings_Safety/_Buttons_SafetyLevel/Button_None/ON").GetComponent<Image>());
                _normalColorImage.Add(quickMenu.transform.Find("Screens/Settings_Safety/_Buttons_SafetyLevel/Button_Normal/ON").GetComponent<Image>());
                _normalColorImage.Add(quickMenu.transform.Find("Screens/Settings_Safety/_Buttons_SafetyLevel/Button_Maxiumum/ON").GetComponent<Image>());
                _normalColorImage.Add(quickMenu.transform.Find("Popups/InputKeypadPopup/Rectangle/Panel").GetComponent<Image>());
                _normalColorImage.Add(quickMenu.transform.Find("Popups/InputKeypadPopup/InputField").GetComponent<Image>());
                _normalColorImage.Add(quickMenu.transform.Find("Popups/StandardPopupV2/Popup/Panel").GetComponent<Image>());
                _normalColorImage.Add(quickMenu.transform.Find("Popups/StandardPopup/InnerDashRing").GetComponent<Image>());
                _normalColorImage.Add(quickMenu.transform.Find("Popups/StandardPopup/RingGlow").GetComponent<Image>());
                _normalColorImage.Add(quickMenu.transform.Find("Popups/UpdateStatusPopup/Popup/Panel").GetComponent<Image>());
                _normalColorImage.Add(quickMenu.transform.Find("Popups/InputPopup/InputField").GetComponent<Image>());
                _normalColorImage.Add(quickMenu.transform.Find("Popups/UpdateStatusPopup/Popup/InputFieldStatus").GetComponent<Image>());
                _normalColorImage.Add(quickMenu.transform.Find("Popups/AdvancedSettingsPopup/Popup/Panel").GetComponent<Image>());
                _normalColorImage.Add(quickMenu.transform.Find("Popups/AddToFavoriteListPopup/Popup/Panel").GetComponent<Image>());
                _normalColorImage.Add(quickMenu.transform.Find("Popups/EditFavoriteListPopup/Popup/Panel").GetComponent<Image>());
                _normalColorImage.Add(quickMenu.transform.Find("Popups/PerformanceSettingsPopup/Popup/Panel").GetComponent<Image>());
                _normalColorImage.Add(quickMenu.transform.Find("Popups/AlertPopup/Lighter").GetComponent<Image>());
                _normalColorImage.Add(quickMenu.transform.Find("Popups/RoomInstancePopup/Popup/Panel").GetComponent<Image>());
                _normalColorImage.Add(quickMenu.transform.Find("Popups/ReportWorldPopup/Popup/Panel").GetComponent<Image>());
                _normalColorImage.Add(quickMenu.transform.Find("Popups/ReportUserPopup/Popup/Panel").GetComponent<Image>());
                _normalColorImage.Add(quickMenu.transform.Find("Popups/SearchOptionsPopup/Popup/Panel (1)").GetComponent<Image>());
                _normalColorImage.Add(quickMenu.transform.Find("Popups/SendInvitePopup/SendInviteMenu/Panel").GetComponent<Image>());
                _normalColorImage.Add(quickMenu.transform.Find("Popups/RequestInvitePopup/RequestInviteMenu/Panel").GetComponent<Image>());
                _normalColorImage.Add(quickMenu.transform.Find("Popups/ControllerBindingsPopup/Popup/Panel").GetComponent<Image>());
                _normalColorImage.Add(quickMenu.transform.Find("Popups/ChangeProfilePicPopup/Popup/PanelBackground").GetComponent<Image>());
                _normalColorImage.Add(quickMenu.transform.Find("Popups/ChangeProfilePicPopup/Popup/TitlePanel").GetComponent<Image>());
                _normalColorImage.Add(quickMenu.transform.Find("Screens/UserInfo/User Panel/PanelHeaderBackground").GetComponent<Image>());
                foreach (var obj in quickMenu.GetComponentsInChildren<Transform>(true).Where(x => x.name.Contains("Panel_Header"))) {
                    foreach (var img in obj.GetComponentsInChildren<Image>())
                        _normalColorImage.Add(img);
                }
                foreach (var obj in quickMenu.GetComponentsInChildren<Transform>(true).Where(x => x.name.Contains("Handle"))) {
                    foreach (var img in obj.GetComponentsInChildren<Image>())
                        _normalColorImage.Add(img);
                }
                try {
                    _normalColorImage.Add(quickMenu.transform.Find("Popups/LoadingPopup/ProgressPanel/Parent_Loading_Progress/Panel_Backdrop").GetComponent<Image>());
                    _normalColorImage.Add(quickMenu.transform.Find("Popups/LoadingPopup/ProgressPanel/Parent_Loading_Progress/Decoration_Left").GetComponent<Image>());
                    _normalColorImage.Add(quickMenu.transform.Find("Popups/LoadingPopup/ProgressPanel/Parent_Loading_Progress/Decoration_Right").GetComponent<Image>());
                    _normalColorImage.Add(quickMenu.transform.Find("Popups/LoadingPopup/MirroredElements/ProgressPanel (1)/Parent_Loading_Progress/Panel_Backdrop").GetComponent<Image>());
                    _normalColorImage.Add(quickMenu.transform.Find("Popups/LoadingPopup/MirroredElements/ProgressPanel (1)/Parent_Loading_Progress/Decoration_Left").GetComponent<Image>());
                    _normalColorImage.Add(quickMenu.transform.Find("Popups/LoadingPopup/MirroredElements/ProgressPanel (1)/Parent_Loading_Progress/Decoration_Right").GetComponent<Image>());
                } catch (Exception ex) {
                    Con.Error(ex.ToString());
                }

                #endregion
            }

            if (_dimmerColorImage == null || _dimmerColorImage.Count == 0) {
                #region dimmerColorImage

                _dimmerColorImage = new List<Image>();
                var quickMenu = UIWrappers.GetVRCUiMInstance().menuContent();
                _dimmerColorImage.Add(quickMenu.transform.Find("Screens/Settings_Safety/_Buttons_SafetyLevel/Button_Custom/ON/TopPanel_SafetyLevel").GetComponent<Image>());
                _dimmerColorImage.Add(quickMenu.transform.Find("Screens/Settings_Safety/_Buttons_SafetyLevel/Button_None/ON/TopPanel_SafetyLevel").GetComponent<Image>());
                _dimmerColorImage.Add(quickMenu.transform.Find("Screens/Settings_Safety/_Buttons_SafetyLevel/Button_Normal/ON/TopPanel_SafetyLevel").GetComponent<Image>());
                _dimmerColorImage.Add(quickMenu.transform.Find("Screens/Settings_Safety/_Buttons_SafetyLevel/Button_Maxiumum/ON/TopPanel_SafetyLevel").GetComponent<Image>());
                _dimmerColorImage.Add(quickMenu.transform.Find("Popups/ChangeProfilePicPopup/Popup/BorderImage").GetComponent<Image>());
                foreach (var obj in quickMenu.GetComponentsInChildren<Transform>(true).Where(x => x.name.Contains("Fill"))) {
                    foreach (var img in obj.GetComponentsInChildren<Image>())
                        if (img.gameObject.name != "Checkmark")
                            _dimmerColorImage.Add(img);
                }

                #endregion
            }

            if (_darkerColorImage == null || _darkerColorImage.Count == 0) {
                #region darkerColorImage

                _darkerColorImage = new List<Image>();
                var quickMenu = UIWrappers.GetVRCUiMInstance().menuContent();
                _darkerColorImage.Add(quickMenu.transform.Find("Popups/InputKeypadPopup/Rectangle").GetComponent<Image>());
                _darkerColorImage.Add(quickMenu.transform.Find("Popups/StandardPopupV2/Popup/BorderImage").GetComponent<Image>());
                _darkerColorImage.Add(quickMenu.transform.Find("Popups/StandardPopup/Rectangle").GetComponent<Image>());
                _darkerColorImage.Add(quickMenu.transform.Find("Popups/StandardPopup/MidRing").GetComponent<Image>());
                _darkerColorImage.Add(quickMenu.transform.Find("Popups/UpdateStatusPopup/Popup/BorderImage").GetComponent<Image>());
                _darkerColorImage.Add(quickMenu.transform.Find("Popups/AdvancedSettingsPopup/Popup/BorderImage").GetComponent<Image>());
                _darkerColorImage.Add(quickMenu.transform.Find("Popups/AddToFavoriteListPopup/Popup/BorderImage").GetComponent<Image>());
                _darkerColorImage.Add(quickMenu.transform.Find("Popups/EditFavoriteListPopup/Popup/BorderImage").GetComponent<Image>());
                _darkerColorImage.Add(quickMenu.transform.Find("Popups/PerformanceSettingsPopup/Popup/BorderImage").GetComponent<Image>());
                _darkerColorImage.Add(quickMenu.transform.Find("Popups/RoomInstancePopup/Popup/BorderImage").GetComponent<Image>());
                _darkerColorImage.Add(quickMenu.transform.Find("Popups/RoomInstancePopup/Popup/BorderImage (1)").GetComponent<Image>());
                _darkerColorImage.Add(quickMenu.transform.Find("Popups/ReportWorldPopup/Popup/BorderImage").GetComponent<Image>());
                _darkerColorImage.Add(quickMenu.transform.Find("Popups/ReportUserPopup/Popup/BorderImage").GetComponent<Image>());
                _darkerColorImage.Add(quickMenu.transform.Find("Popups/SearchOptionsPopup/Popup/BorderImage").GetComponent<Image>());
                _darkerColorImage.Add(quickMenu.transform.Find("Popups/SendInvitePopup/SendInviteMenu/BorderImage").GetComponent<Image>());
                _darkerColorImage.Add(quickMenu.transform.Find("Popups/RequestInvitePopup/RequestInviteMenu/BorderImage").GetComponent<Image>());
                _darkerColorImage.Add(quickMenu.transform.Find("Popups/ControllerBindingsPopup/Popup/BorderImage").GetComponent<Image>());
                _darkerColorImage.Add(quickMenu.transform.Find("Screens/UserInfo/ModerateDialog/Panel/BorderImage").GetComponent<Image>());
                foreach (var obj in quickMenu.GetComponentsInChildren<Transform>(true).Where(x => x.name.Contains("Background") && x.name != "PanelHeaderBackground" && !x.transform.parent.name.Contains("UserIcon"))) {
                    foreach (var img in obj.GetComponentsInChildren<Image>())
                        _darkerColorImage.Add(img);
                }

                #endregion
            }

            if (_normalColorText == null || _normalColorText.Count == 0) {
                #region normalColorText

                _normalColorText = new List<Text>();
                var quickMenu = UIWrappers.GetVRCUiMInstance().menuContent();
                foreach (var txt in quickMenu.transform.Find("Popups/InputPopup/Keyboard/Keys").GetComponentsInChildren<Text>(true))
                    _normalColorText.Add(txt);
                foreach (var txt in quickMenu.transform.Find("Popups/InputKeypadPopup/Keyboard/Keys").GetComponentsInChildren<Text>(true))
                    _normalColorText.Add(txt);
                _normalColorText.Add(quickMenu.transform.Find("Screens/Settings/VolumePanel/VolumeGameWorld/Label").GetComponentInChildren<Text>(true));
                _normalColorText.Add(quickMenu.transform.Find("Screens/Settings/VolumePanel/VolumeGameVoice/Label").GetComponentInChildren<Text>(true));
                _normalColorText.Add(quickMenu.transform.Find("Screens/Settings/VolumePanel/VolumeGameAvatars/Label").GetComponentInChildren<Text>(true));
                _normalColorText.AddRange(quickMenu.transform.Find("Screens/Social/UserProfileAndStatusSection").GetComponentsInChildren<Text>(true));
                _normalColorText.Add(quickMenu.transform.Find("Popups/LoadingPopup/ProgressPanel/Parent_Loading_Progress/Loading Elements/txt_LOADING_Size").GetComponentInChildren<Text>(true));
                _normalColorText.Add(quickMenu.transform.Find("Popups/LoadingPopup/MirroredElements/ProgressPanel (1)/Parent_Loading_Progress/Loading Elements/txt_LOADING_Size").GetComponentInChildren<Text>(true));

                #endregion
            }
            
            foreach (var img in _normalColorImage)
                img.color = colorT;
            foreach (var img in _dimmerColorImage)
                img.color = dimmerT;
            foreach (var img in _darkerColorImage)
                img.color = darkerT;
            foreach (var txt in _normalColorText)
                txt.color = color;
            
            var buttonTheme = new ColorBlock {
                colorMultiplier = 1f,
                disabledColor = Color.grey,
                highlightedColor = color * 1.5f,
                normalColor = color / 1.5f,
                pressedColor = new Color(1f, 1f, 1f, 1f),
                fadeDuration = 0.1f,
                selectedColor = color / 1.5f
            };
            color.a = 0.9f;

            if (UIWrappers.GetVRCUiMInstance().menuContent() != null) {
                var quickMenu = UIWrappers.GetVRCUiMInstance().menuContent();
                try {
                    var inputHolder = quickMenu.transform.Find("Popups/InputPopup");
                    darker.a = 0.8f;
                    inputHolder.Find("Rectangle").GetComponent<Image>().color = darker;
                    darker.a = .5f;
                    color.a = 0.8f;
                    inputHolder.Find("Rectangle/Panel").GetComponent<Image>().color = color;
                    color.a = .5f;
                    var holder = quickMenu.transform.Find("Backdrop/Header/Tabs/ViewPort/Content/Search");
                    holder.Find("SearchTitle").GetComponent<Text>().color = color;
                    holder.Find("InputField").GetComponent<Image>().color = color;
                } catch (Exception ex) {
                    Con.Error(ex);
                }

                try {
                    var theme = new ColorBlock {
                        colorMultiplier = 1f,
                        disabledColor = Color.grey,
                        highlightedColor = darker,
                        normalColor = color,
                        pressedColor = Color.gray,
                        fadeDuration = 0.1f
                    };
                    quickMenu.GetComponentsInChildren<Transform>(true).FirstOrDefault(x => x.name == "Row:4 Column:0")!.GetComponent<Button>().colors = buttonTheme;
                    color.a = 0.5f;
                    darker.a = 1f;
                    theme.normalColor = darker;
                    foreach (var sldr in quickMenu.GetComponentsInChildren<Slider>(true))
                        sldr.colors = theme;
                    darker.a = 0.5f;
                    theme.normalColor = color;
                    foreach (var btn in quickMenu.GetComponentsInChildren<Button>(true))
                        if (btn.gameObject.name != "SubscribeToAddPhotosButton" && btn.gameObject.name != "SupporterButton" && 
                            btn.gameObject.name != "ModerateButton" && (btn.gameObject.name != "ReportButton" || btn.transform.parent.name.Contains("WorldInfo")))
                            btn.colors = buttonTheme;
                    try {
                        var quickMenu22 = GameObject.Find("QuickMenu");
                        foreach (var btn in quickMenu22.GetComponentsInChildren<Button>(true)) {
                            if (btn.transform.parent.name != "SocialNotifications" &&
                                btn.transform.parent.parent.name != "EmojiMenu" &&
                                !btn.transform.parent.name.Contains("NotificationUiPrefab"))
                                btn.colors = buttonTheme;
                        }
                    }
                    catch { Console.Write(""); }

                    foreach (var tglbtn in quickMenu.GetComponentsInChildren<UiToggleButton>(true)) {
                        foreach (var img in tglbtn.GetComponentsInChildren<Image>(true)) 
                            img.color = color;
                    }
                    foreach (var sldr in quickMenu.GetComponentsInChildren<Slider>(true)) {
                        sldr.colors = theme;
                        foreach (var img in sldr.GetComponentsInChildren<Image>(true))
                            img.color = color;
                    }
                    foreach (var tgle in quickMenu.GetComponentsInChildren<Toggle>(true)) {
                        if (tgle.name != "Button_Visitor" && tgle.name != "Button_New" && tgle.name != "Button_User" &&
                            tgle.name != "Button_Trusted" && tgle.name != "Button_Advanced" && tgle.name != "Button_Friends") {
                            tgle.colors = theme;
                            foreach (var img in tgle.GetComponentsInChildren<Image>(true))
                                img.color = color;
                        }
                    }
                    /*
                    var NotificationRoot = GameObject.Find("UserInterface/QuickMenu/QuickModeMenus/QuickModeNotificationsMenu/ScrollRect");
                    foreach (var img in NotificationRoot.GetComponentsInChildren<Image>(true)) {
                        if (img.transform.name == "Background")
                            img.color = color;
                    }
                    */
                    //MelonCoroutines.Start(DelayedHfxReColor(color));
                    ChangeHfxColor(color);
                } catch (Exception ex) {
                    Con.Error(ex);
                }
            }
            yield break;
        }

        internal override void OnUpdate() {
            if (!ModCompatibility.ReMod) return;
            if (HfxFound && Hfx.Count != 0)
                Hfx.FirstOrDefault()!.highlightColor = _finalColor;
        }

        private static Il2CppArrayBase<HighlightsFXStandalone> Hfx;
        private static bool HfxFound;

        // internal override void OnLevelWasLoaded(int buildindex, string sceneName) {
        //     if (buildindex == -1 && ModCompatibility.ReMod) {
        //         MelonCoroutines.Start(DelayedHfxReColor(_finalColor));
        //     }
        // }

        private static IEnumerator DelayedHfxReColor(Color color) {
            yield return new WaitForSeconds(5);
            Hfx = UnityEngine.Resources.FindObjectsOfTypeAll<HighlightsFXStandalone>();
            if (Hfx.Count == 0) yield break;
            Hfx.FirstOrDefault()!.highlightColor = color;
            HfxFound = true;
        }

        private static void ChangeHfxColor(Color color) {
            foreach (var highlightsFXStandalone in UnityEngine.Resources.FindObjectsOfTypeAll<HighlightsFXStandalone>())
                highlightsFXStandalone.highlightColor = color;
        }

        //private void ColorActionMenu(Color color) {
        //    if (ModCompatibility.Styletor) return;
        //    foreach (var grph in UnityEngine.Resources.FindObjectsOfTypeAll<PedalGraphic>())
        //        grph.color = color;
        //}

        private void ColorLoadingScreenEnvironment(Color color) {
            if (Config.ColorLoadingScreen.Value && MelonHandler.Mods.Any(i => i.Info.Name == "BetterLoadingScreen")) return;
            try {
                var quickMenu = UIWrappers.GetVRCUiMInstance().menuContent();
                _loadingBackground = quickMenu.transform.Find("Popups/LoadingPopup/3DElements/LoadingBackground_TealGradient/SkyCube_Baked").gameObject;
                _loadingBackground.GetComponent<MeshRenderer>().material.SetTexture("_Tex", MintyResources.basicGradient);
                _loadingBackground.GetComponent<MeshRenderer>().material.SetColor("_Tint", new Color(color.r / 2f, color.g / 2f, color.b / 2f, color.a));
                _loadingBackground.GetComponent<MeshRenderer>().material.SetTexture("_Tex", MintyResources.basicGradient);

                _initialLoadingBackground = GameObject.Find("LoadingBackground_TealGradient_Music/SkyCube_Baked");
                _initialLoadingBackground.GetComponent<MeshRenderer>().material.SetTexture("_Tex", MintyResources.basicGradient);
                _initialLoadingBackground.GetComponent<MeshRenderer>().material.SetColor("_Tint", new Color(color.r / 2f, color.g / 2f, color.b / 2f, color.a));
                _initialLoadingBackground.GetComponent<MeshRenderer>().material.SetTexture("_Tex", MintyResources.basicGradient);
            } catch (Exception e) {
                Con.Error(e);
            }
        }
    }
}
