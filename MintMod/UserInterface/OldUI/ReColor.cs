using MelonLoader;
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
using ReMod.Core.VRChat;
using UnhollowerBaseLib;
using UnityEngine.UI;

namespace MintMod.UserInterface.OldUI {
    internal class ReColor : MintSubMod {
        public override string Name => "OldUiRecolor";
        public override string Description => "Colors the User Interface (Not the QuickMenu)";
        internal static ReColor Instance;

        private List<Image> _normalColorImage, _dimmerColorImage, _darkerColorImage;
        private List<Text> _normalColorText, _normalColorTextSettings;
        private GameObject _loadingBackground, _initialLoadingBackground;
        private Color _finalColor;

        internal override void OnStart() => Instance = this;

        internal override void OnUserInterface() {
            _finalColor = Config.ColorGameMenu.Value ? Minty : defaultMenuColor();
            if (Config.ColorGameMenu.Value)
                ColorMenu(_finalColor);
            
            if (!Config.ColorLoadingScreen.Value) return;
            try {
                ColorLoadingScreenEnvironment(_finalColor);
            }
            catch (Exception e) {
                //if (!MintUserInterface.isStreamerModeOn)
                    Con.Error(e);
            }
        }

        private void ColorMenu(Color color) {
            color = Config.ColorGameMenu.Value ? Minty : defaultMenuColor();
                    
            ChangeHfxColor(color);

            var colorT = new Color(color.r, color.g, color.b, 0.7f);
            var dimmerT = new Color(color.r / 0.75f, color.g / 0.75f, color.b / 0.75f, 0.9f);
            var darker = new Color(color.r / 2.5f, color.g / 2.5f, color.b / 2.5f);
            var darkerT = new Color(color.r / 2.5f, color.g / 2.5f, color.b / 2.5f, 0.9f);
            var textBright = new Color(color.r * 1.5f, color.g * 1.5f, color.b * 1.5f, 1f);

            try {
                if (_normalColorImage == null || _normalColorImage.Count == 0) {
                    #region normalColorImage

                    _normalColorImage = new List<Image>();
                    var quickMenu = VRCUiManager.prop_VRCUiManager_0!.MenuContent();
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
                    _normalColorImage.Add(quickMenu.transform.Find("Popups/StandardPopup/ArrowLeft").GetComponent<Image>());
                    _normalColorImage.Add(quickMenu.transform.Find("Popups/StandardPopup/ArrowRight").GetComponent<Image>());
                    foreach (var obj in quickMenu.GetComponentsInChildren<Transform>(true).Where(x => x.name.Contains("Panel_Header"))) {
                        foreach (var img in obj.GetComponentsInChildren<Image>())
                            if (img.gameObject.name != "Checkmark")
                                _normalColorImage.Add(img);
                    }

                    foreach (var obj in quickMenu.GetComponentsInChildren<Transform>(true).Where(x => x.name.Contains("Handle"))) {
                        foreach (var img in obj.GetComponentsInChildren<Image>())
                            if (img.gameObject.name != "Checkmark")
                                _normalColorImage.Add(img);
                    }

                    try {
                        _normalColorImage.Add(quickMenu.transform.Find("Popups/LoadingPopup/ProgressPanel/Parent_Loading_Progress/Panel_Backdrop").GetComponent<Image>());
                        _normalColorImage.Add(quickMenu.transform.Find("Popups/LoadingPopup/ProgressPanel/Parent_Loading_Progress/Decoration_Left").GetComponent<Image>());
                        _normalColorImage.Add(quickMenu.transform.Find("Popups/LoadingPopup/ProgressPanel/Parent_Loading_Progress/Decoration_Right").GetComponent<Image>());
                        _normalColorImage.Add(quickMenu.transform.Find("Popups/LoadingPopup/MirroredElements/ProgressPanel (1)/Parent_Loading_Progress/Panel_Backdrop").GetComponent<Image>());
                        _normalColorImage.Add(quickMenu.transform.Find("Popups/LoadingPopup/MirroredElements/ProgressPanel (1)/Parent_Loading_Progress/Decoration_Left").GetComponent<Image>());
                        _normalColorImage.Add(quickMenu.transform.Find("Popups/LoadingPopup/MirroredElements/ProgressPanel (1)/Parent_Loading_Progress/Decoration_Right").GetComponent<Image>());
                    }
                    catch (Exception ex) {
                        Con.Error(ex.ToString());
                    }

                    #endregion
                }

                if (_dimmerColorImage == null || _dimmerColorImage.Count == 0) {
                    #region dimmerColorImage

                    _dimmerColorImage = new List<Image>();
                    var quickMenu = VRCUiManager.prop_VRCUiManager_0!.MenuContent();
                    _dimmerColorImage.Add(quickMenu.transform.Find("Screens/Settings_Safety/_Buttons_SafetyLevel/Button_Custom/ON/TopPanel_SafetyLevel").GetComponent<Image>());
                    _dimmerColorImage.Add(quickMenu.transform.Find("Screens/Settings_Safety/_Buttons_SafetyLevel/Button_None/ON/TopPanel_SafetyLevel").GetComponent<Image>());
                    _dimmerColorImage.Add(quickMenu.transform.Find("Screens/Settings_Safety/_Buttons_SafetyLevel/Button_Normal/ON/TopPanel_SafetyLevel").GetComponent<Image>());
                    _dimmerColorImage.Add(quickMenu.transform.Find("Screens/Settings_Safety/_Buttons_SafetyLevel/Button_Maxiumum/ON/TopPanel_SafetyLevel").GetComponent<Image>());
                    _dimmerColorImage.Add(quickMenu.transform.Find("Popups/ChangeProfilePicPopup/Popup/BorderImage").GetComponent<Image>());
                    
                    _dimmerColorImage.Add(quickMenu.transform.Find("Screens/Settings/VolumePanel/Panel_Header Side").GetComponent<Image>());
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
                    var quickMenu = VRCUiManager.prop_VRCUiManager_0!.MenuContent();
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
                    
                    _darkerColorImage.Add(quickMenu.transform.Find("Screens/Settings/TitlePanel").GetComponent<Image>());
                    _darkerColorImage.Add(quickMenu.transform.Find("Screens/Settings/ComfortSafetyPanel").GetComponent<Image>());
                    _darkerColorImage.Add(quickMenu.transform.Find("Screens/Settings/VoiceOptionsPanel").GetComponent<Image>());
                    _darkerColorImage.Add(quickMenu.transform.Find("Screens/Settings/MousePanel").GetComponent<Image>());
                    _darkerColorImage.Add(quickMenu.transform.Find("Screens/Settings/OtherOptionsPanel").GetComponent<Image>());
                    _darkerColorImage.Add(quickMenu.transform.Find("Screens/Settings/VolumePanel").GetComponent<Image>());
                    _darkerColorImage.Add(quickMenu.transform.Find("Screens/Settings/AudioDevicePanel").GetComponent<Image>());
                    _darkerColorImage.Add(quickMenu.transform.Find("Screens/Settings/UserVolumeOptions").GetComponent<Image>());
                    _darkerColorImage.Add(quickMenu.transform.Find("Screens/Settings/HeightPanel").GetComponent<Image>());
                    _darkerColorImage.Add(quickMenu.transform.Find("Screens/Avatar/TitlePanel (1)").GetComponentInChildren<Image>(true));
                    foreach (var obj in quickMenu.GetComponentsInChildren<Transform>(true).Where(x =>
                                 (x.name.Contains("Background"))/* || x.name.Contains("TitlePanel"))*/ && x.name != "PanelHeaderBackground" &&
                                 !x.transform.parent.name.Contains("UserIcon") && x!.transform.name != "Button_PerformanceOptions" && !x.name.Contains("Expando"))) {
                        foreach (var img in obj.GetComponentsInChildren<Image>())
                            if (img.gameObject.name != "Checkmark")
                                _darkerColorImage.Add(img);
                    }

                    _darkerColorImage[18].enabled = false;
                    _darkerColorImage[27].enabled = false;

                    #endregion
                }

                if (_normalColorText == null || _normalColorText.Count == 0) {
                    #region normalColorText

                    _normalColorText = new List<Text>();
                    var quickMenu = VRCUiManager.prop_VRCUiManager_0!.MenuContent();
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
                    
                    _normalColorText.Add(quickMenu.transform.Find("Screens/Settings/VolumePanel/TitleText (1)").GetComponentInChildren<Text>(true));
                    _normalColorText.Add(quickMenu.transform.Find("Screens/Settings/AudioDevicePanel/TitleText").GetComponentInChildren<Text>(true));
                    _normalColorText.Add(quickMenu.transform.Find("Screens/Settings/UserVolumeOptions/TitleText (1)").GetComponentInChildren<Text>(true));
                    _normalColorText.Add(quickMenu.transform.Find("Screens/Settings/OtherOptionsPanel/TitleText (1)").GetComponentInChildren<Text>(true));
                    _normalColorText.Add(quickMenu.transform.Find("Screens/Settings/VoiceOptionsPanel/TitleText (1)").GetComponentInChildren<Text>(true));
                    _normalColorText.Add(quickMenu.transform.Find("Screens/Settings/ComfortSafetyPanel/TitleText (1)").GetComponentInChildren<Text>(true));
                    _normalColorText.Add(quickMenu.transform.Find("Screens/Settings/MousePanel/TitleText").GetComponentInChildren<Text>(true));

                    #endregion
                }
                
                if (_normalColorTextSettings == null || _normalColorTextSettings.Count == 0) {
                    #region normalColorTextSettings

                    _normalColorTextSettings = new List<Text>();
                    var quickMenu = VRCUiManager.prop_VRCUiManager_0!.MenuContent();
                    _normalColorTextSettings.Add(quickMenu.transform.Find("Screens/Settings/VolumePanel/TitleText (1)").GetComponentInChildren<Text>(true));
                    _normalColorTextSettings.Add(quickMenu.transform.Find("Screens/Settings/AudioDevicePanel/TitleText").GetComponentInChildren<Text>(true));
                    _normalColorTextSettings.Add(quickMenu.transform.Find("Screens/Settings/UserVolumeOptions/TitleText (1)").GetComponentInChildren<Text>(true));
                    _normalColorTextSettings.Add(quickMenu.transform.Find("Screens/Settings/OtherOptionsPanel/TitleText (1)").GetComponentInChildren<Text>(true));
                    _normalColorTextSettings.Add(quickMenu.transform.Find("Screens/Settings/VoiceOptionsPanel/TitleText (1)").GetComponentInChildren<Text>(true));
                    _normalColorTextSettings.Add(quickMenu.transform.Find("Screens/Settings/ComfortSafetyPanel/TitleText (1)").GetComponentInChildren<Text>(true));
                    _normalColorTextSettings.Add(quickMenu.transform.Find("Screens/Settings/MousePanel/TitleText").GetComponentInChildren<Text>(true));
                    _normalColorTextSettings.Add(quickMenu.transform.Find("Screens/Settings/HeightPanel/TitleText (1)").GetComponentInChildren<Text>(true));
                    
                    #endregion
                }

                foreach (var img in _normalColorImage)
                    if (img.sprite != null && img.sprite.texture != null)
                        img.color = colorT;
                foreach (var img in _dimmerColorImage)
                    if (img.sprite != null && img.sprite.texture != null)
                        img.color = dimmerT;
                foreach (var img in _darkerColorImage)
                    if (img.sprite != null && img.sprite.texture != null)
                        img.color = darkerT;
                foreach (var txt in _normalColorText)
                    txt.color = color;
                foreach (var txt in _normalColorTextSettings)
                    txt.color = textBright;
            }
            catch (Exception co) {
                Con.Error(co);
            }
            
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

            if (VRCUiManager.prop_VRCUiManager_0!.MenuContent() == null) return;
            var quickMenu2 = VRCUiManager.prop_VRCUiManager_0!.MenuContent();
            try {
                var inputHolder = quickMenu2.transform.Find("Popups/InputPopup");
                darker.a = 0.8f;
                inputHolder.Find("Rectangle").GetComponent<Image>().color = darker;
                darker.a = .5f;
                color.a = 0.8f;
                inputHolder.Find("Rectangle/Panel").GetComponent<Image>().color = color;
                color.a = .5f;
                var holder = quickMenu2.transform.Find("Backdrop/Header/Tabs/ViewPort/Content/Search");
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
                quickMenu2.GetComponentsInChildren<Transform>(true).FirstOrDefault(x => x.name == "Row:4 Column:0")!.GetComponent<Button>().colors = buttonTheme;
                color.a = 0.5f;
                darker.a = 1f;
                theme.normalColor = darker;
                foreach (var sldr in quickMenu2.GetComponentsInChildren<Slider>(true))
                    sldr.colors = theme;
                darker.a = 0.5f;
                theme.normalColor = color;
                foreach (var btn in quickMenu2.GetComponentsInChildren<Button>(true))
                    if (btn.gameObject.name != "SubscribeToAddPhotosButton" && btn.gameObject.name != "SupporterButton" && 
                        btn.gameObject.name != "ModerateButton" && btn.transform.parent.name != "VRC+PageTab" &&
                        (btn.gameObject.name != "ReportButton" || btn.transform.parent.name.Contains("WorldInfo")))
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
                
                foreach (var tglbtn in quickMenu2.GetComponentsInChildren<UiToggleButton>(true)) {
                    foreach (var img in tglbtn.GetComponentsInChildren<Image>(true)) 
                        img.color = color;
                }
                foreach (var sldr in quickMenu2.GetComponentsInChildren<Slider>(true)) {
                    sldr.colors = theme;
                    foreach (var img in sldr.GetComponentsInChildren<Image>(true))
                        img.color = color;
                }
                foreach (var tgle in quickMenu2.GetComponentsInChildren<Toggle>(true)) {
                    if (tgle.name is "Button_Visitor" or "Button_New" or "Button_User" or "Button_Trusted" or "Button_Advanced" or "Button_Friends") continue;
                    tgle.colors = theme;
                    foreach (var img in tgle.GetComponentsInChildren<Image>(true))
                        img.color = color;
                }
            } catch (Exception ex) {
                Con.Error(ex);
            }
        }

        private void ChangeHfxColor(Color color) {
            foreach (var highlightsFXStandalone in UnityEngine.Resources.FindObjectsOfTypeAll<HighlightsFXStandalone>())
                highlightsFXStandalone.highlightColor = color;
        }

        private void ColorLoadingScreenEnvironment(Color color) {
            if (Config.ColorLoadingScreen.Value && MelonHandler.Mods.Any(i => i.Info.Name == "BetterLoadingScreen")) return;
            try {
                var quickMenu = VRCUiManager.prop_VRCUiManager_0!.MenuContent();
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
