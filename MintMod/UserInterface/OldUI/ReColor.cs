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
using UnityEngine.UI;

namespace MintMod.UserInterface.OldUI {
    class ReColor : MintSubMod {
        public override string Name => "OldUiRecolor";
        public override string Description => "Colors the User Interface (Not the QuickMenu)";
        internal static ReColor Intstance;

        private static List<Image> normalColorImage, dimmerColorImage, darkerColorImage;
        private static List<Text> normalColorText;
        private static GameObject loadingBackground, initialLoadingBackground;

        internal override void OnStart() => Intstance = this;

        internal override void OnUserInterface() {
            bool color = Config.ColorGameMenu.Value;
            Color GameUI = Minty, _color = color ? GameUI : defaultMenuColor();
            if (Config.ColorGameMenu.Value)
                MelonCoroutines.Start(ColorMenu(_color));
            if (Config.ColorActionMenu.Value)
                ColorActionMenu(_color);
            if (Config.ColorLoadingScreen.Value) {
                try {
                    ColorLoadingScreenEnvironment(_color);
                }
                catch (Exception e) {
                    if (!MintUserInterface.isStreamerModeOn)
                        Con.Error(e);
                }
            }
            
        }

        IEnumerator ColorMenu(Color color) {
            color = Config.ColorGameMenu.Value ? Minty : defaultMenuColor();

            Color colorT = new Color(color.r, color.g, color.b, 0.7f);
            Color dimmer = new Color(color.r / 0.75f, color.g / 0.75f, color.b / 0.75f);
            Color dimmerT = new Color(color.r / 0.75f, color.g / 0.75f, color.b / 0.75f, 0.9f);
            Color darker = new Color(color.r / 2.5f, color.g / 2.5f, color.b / 2.5f);
            Color darkerT = new Color(color.r / 2.5f, color.g / 2.5f, color.b / 2.5f, 0.9f);

            if (normalColorImage == null || normalColorImage.Count == 0) {
                #region normalColorImage

                normalColorImage = new List<Image>();
                GameObject quickMenu = UIWrappers.GetVRCUiMInstance().menuContent();
                normalColorImage.Add(quickMenu.transform.Find("Screens/Settings_Safety/_Description_SafetyLevel").GetComponent<Image>());
                normalColorImage.Add(quickMenu.transform.Find("Screens/Settings_Safety/_Buttons_SafetyLevel/Button_Custom/ON").GetComponent<Image>());
                normalColorImage.Add(quickMenu.transform.Find("Screens/Settings_Safety/_Buttons_SafetyLevel/Button_None/ON").GetComponent<Image>());
                normalColorImage.Add(quickMenu.transform.Find("Screens/Settings_Safety/_Buttons_SafetyLevel/Button_Normal/ON").GetComponent<Image>());
                normalColorImage.Add(quickMenu.transform.Find("Screens/Settings_Safety/_Buttons_SafetyLevel/Button_Maxiumum/ON").GetComponent<Image>());
                normalColorImage.Add(quickMenu.transform.Find("Popups/InputKeypadPopup/Rectangle/Panel").GetComponent<Image>());
                normalColorImage.Add(quickMenu.transform.Find("Popups/InputKeypadPopup/InputField").GetComponent<Image>());
                normalColorImage.Add(quickMenu.transform.Find("Popups/StandardPopupV2/Popup/Panel").GetComponent<Image>());
                normalColorImage.Add(quickMenu.transform.Find("Popups/StandardPopup/InnerDashRing").GetComponent<Image>());
                normalColorImage.Add(quickMenu.transform.Find("Popups/StandardPopup/RingGlow").GetComponent<Image>());
                normalColorImage.Add(quickMenu.transform.Find("Popups/UpdateStatusPopup/Popup/Panel").GetComponent<Image>());
                normalColorImage.Add(quickMenu.transform.Find("Popups/InputPopup/InputField").GetComponent<Image>());
                normalColorImage.Add(quickMenu.transform.Find("Popups/UpdateStatusPopup/Popup/InputFieldStatus").GetComponent<Image>());
                normalColorImage.Add(quickMenu.transform.Find("Popups/AdvancedSettingsPopup/Popup/Panel").GetComponent<Image>());
                normalColorImage.Add(quickMenu.transform.Find("Popups/AddToPlaylistPopup/Popup/Panel").GetComponent<Image>());
                normalColorImage.Add(quickMenu.transform.Find("Popups/BookmarkFriendPopup/Popup/Panel (2)").GetComponent<Image>());
                normalColorImage.Add(quickMenu.transform.Find("Popups/EditPlaylistPopup/Popup/Panel").GetComponent<Image>());
                normalColorImage.Add(quickMenu.transform.Find("Popups/PerformanceSettingsPopup/Popup/Panel").GetComponent<Image>());
                normalColorImage.Add(quickMenu.transform.Find("Popups/AlertPopup/Lighter").GetComponent<Image>());
                normalColorImage.Add(quickMenu.transform.Find("Popups/RoomInstancePopup/Popup/Panel").GetComponent<Image>());
                normalColorImage.Add(quickMenu.transform.Find("Popups/ReportWorldPopup/Popup/Panel").GetComponent<Image>());
                normalColorImage.Add(quickMenu.transform.Find("Popups/ReportUserPopup/Popup/Panel").GetComponent<Image>());
                normalColorImage.Add(quickMenu.transform.Find("Popups/AddToAvatarFavoritesPopup/Popup/Panel").GetComponent<Image>());
                normalColorImage.Add(quickMenu.transform.Find("Popups/SearchOptionsPopup/Popup/Panel (1)").GetComponent<Image>());
                normalColorImage.Add(quickMenu.transform.Find("Popups/SendInvitePopup/SendInviteMenu/Panel").GetComponent<Image>());
                normalColorImage.Add(quickMenu.transform.Find("Popups/RequestInvitePopup/RequestInviteMenu/Panel").GetComponent<Image>());
                normalColorImage.Add(quickMenu.transform.Find("Popups/ControllerBindingsPopup/Popup/Panel").GetComponent<Image>());
                normalColorImage.Add(quickMenu.transform.Find("Popups/ChangeProfilePicPopup/Popup/PanelBackground").GetComponent<Image>());
                normalColorImage.Add(quickMenu.transform.Find("Popups/ChangeProfilePicPopup/Popup/TitlePanel").GetComponent<Image>());
                normalColorImage.Add(quickMenu.transform.Find("Screens/UserInfo/User Panel/PanelHeaderBackground").GetComponent<Image>());
                //normalColorImage.Add(quickMenu.transform.Find("Screens/UserInfo/User Panel/Panel (1)").GetComponent<Image>());
                foreach (Transform obj in quickMenu.GetComponentsInChildren<Transform>(true).Where(x => x.name.Contains("Panel_Header"))) {
                    foreach (Image img in obj.GetComponentsInChildren<Image>())
                        normalColorImage.Add(img);
                }
                foreach (Transform obj in quickMenu.GetComponentsInChildren<Transform>(true).Where(x => x.name.Contains("Handle"))) {
                    foreach (Image img in obj.GetComponentsInChildren<Image>())
                        normalColorImage.Add(img);
                }
                try {
                    normalColorImage.Add(quickMenu.transform.Find("Popups/LoadingPopup/ProgressPanel/Parent_Loading_Progress/Panel_Backdrop").GetComponent<Image>());
                    normalColorImage.Add(quickMenu.transform.Find("Popups/LoadingPopup/ProgressPanel/Parent_Loading_Progress/Decoration_Left").GetComponent<Image>());
                    normalColorImage.Add(quickMenu.transform.Find("Popups/LoadingPopup/ProgressPanel/Parent_Loading_Progress/Decoration_Right").GetComponent<Image>());
                    normalColorImage.Add(quickMenu.transform.Find("Popups/LoadingPopup/MirroredElements/ProgressPanel (1)/Parent_Loading_Progress/Panel_Backdrop").GetComponent<Image>());
                    normalColorImage.Add(quickMenu.transform.Find("Popups/LoadingPopup/MirroredElements/ProgressPanel (1)/Parent_Loading_Progress/Decoration_Left").GetComponent<Image>());
                    normalColorImage.Add(quickMenu.transform.Find("Popups/LoadingPopup/MirroredElements/ProgressPanel (1)/Parent_Loading_Progress/Decoration_Right").GetComponent<Image>());
                } catch (Exception ex) {
                    Con.Error(ex.ToString());
                }

                #endregion
            }

            if (dimmerColorImage == null || dimmerColorImage.Count == 0) {
                #region dimmerColorImage

                dimmerColorImage = new List<Image>();
                GameObject quickMenu = UIWrappers.GetVRCUiMInstance().menuContent();
                dimmerColorImage.Add(quickMenu.transform.Find("Screens/Settings_Safety/_Buttons_SafetyLevel/Button_Custom/ON/TopPanel_SafetyLevel").GetComponent<Image>());
                dimmerColorImage.Add(quickMenu.transform.Find("Screens/Settings_Safety/_Buttons_SafetyLevel/Button_None/ON/TopPanel_SafetyLevel").GetComponent<Image>());
                dimmerColorImage.Add(quickMenu.transform.Find("Screens/Settings_Safety/_Buttons_SafetyLevel/Button_Normal/ON/TopPanel_SafetyLevel").GetComponent<Image>());
                dimmerColorImage.Add(quickMenu.transform.Find("Screens/Settings_Safety/_Buttons_SafetyLevel/Button_Maxiumum/ON/TopPanel_SafetyLevel").GetComponent<Image>());
                dimmerColorImage.Add(quickMenu.transform.Find("Popups/ChangeProfilePicPopup/Popup/BorderImage").GetComponent<Image>());
                foreach (Transform obj in quickMenu.GetComponentsInChildren<Transform>(true).Where(x => x.name.Contains("Fill"))) {
                    foreach (Image img in obj.GetComponentsInChildren<Image>())
                        if (img.gameObject.name != "Checkmark")
                            dimmerColorImage.Add(img);
                }

                #endregion
            }

            if (darkerColorImage == null || darkerColorImage.Count == 0) {
                #region darkerColorImage

                darkerColorImage = new List<Image>();
                GameObject quickMenu = UIWrappers.GetVRCUiMInstance().menuContent();
                darkerColorImage.Add(quickMenu.transform.Find("Popups/InputKeypadPopup/Rectangle").GetComponent<Image>());
                darkerColorImage.Add(quickMenu.transform.Find("Popups/StandardPopupV2/Popup/BorderImage").GetComponent<Image>());
                darkerColorImage.Add(quickMenu.transform.Find("Popups/StandardPopup/Rectangle").GetComponent<Image>());
                darkerColorImage.Add(quickMenu.transform.Find("Popups/StandardPopup/MidRing").GetComponent<Image>());
                darkerColorImage.Add(quickMenu.transform.Find("Popups/UpdateStatusPopup/Popup/BorderImage").GetComponent<Image>());
                darkerColorImage.Add(quickMenu.transform.Find("Popups/AdvancedSettingsPopup/Popup/BorderImage").GetComponent<Image>());
                darkerColorImage.Add(quickMenu.transform.Find("Popups/AddToPlaylistPopup/Popup/BorderImage").GetComponent<Image>());
                darkerColorImage.Add(quickMenu.transform.Find("Popups/BookmarkFriendPopup/Popup/BorderImage").GetComponent<Image>());
                darkerColorImage.Add(quickMenu.transform.Find("Popups/EditPlaylistPopup/Popup/BorderImage").GetComponent<Image>());
                darkerColorImage.Add(quickMenu.transform.Find("Popups/PerformanceSettingsPopup/Popup/BorderImage").GetComponent<Image>());
                darkerColorImage.Add(quickMenu.transform.Find("Popups/RoomInstancePopup/Popup/BorderImage").GetComponent<Image>());
                darkerColorImage.Add(quickMenu.transform.Find("Popups/RoomInstancePopup/Popup/BorderImage (1)").GetComponent<Image>());
                darkerColorImage.Add(quickMenu.transform.Find("Popups/ReportWorldPopup/Popup/BorderImage").GetComponent<Image>());
                darkerColorImage.Add(quickMenu.transform.Find("Popups/ReportUserPopup/Popup/BorderImage").GetComponent<Image>());
                darkerColorImage.Add(quickMenu.transform.Find("Popups/AddToAvatarFavoritesPopup/Popup/BorderImage").GetComponent<Image>());
                darkerColorImage.Add(quickMenu.transform.Find("Popups/SearchOptionsPopup/Popup/BorderImage").GetComponent<Image>());
                darkerColorImage.Add(quickMenu.transform.Find("Popups/SendInvitePopup/SendInviteMenu/BorderImage").GetComponent<Image>());
                darkerColorImage.Add(quickMenu.transform.Find("Popups/RequestInvitePopup/RequestInviteMenu/BorderImage").GetComponent<Image>());
                darkerColorImage.Add(quickMenu.transform.Find("Popups/ControllerBindingsPopup/Popup/BorderImage").GetComponent<Image>());
                darkerColorImage.Add(quickMenu.transform.Find("Screens/UserInfo/ModerateDialog/Panel/BorderImage").GetComponent<Image>());
                foreach (Transform obj in quickMenu.GetComponentsInChildren<Transform>(true).Where(x => x.name.Contains("Background") && x.name != "PanelHeaderBackground" && !x.transform.parent.name.Contains("UserIcon"))) {
                    foreach (Image img in obj.GetComponentsInChildren<Image>())
                        darkerColorImage.Add(img);
                }

                #endregion
            }

            if (normalColorText == null || normalColorText.Count == 0) {
                #region normalColorText

                normalColorText = new List<Text>();
                GameObject quickMenu = UIWrappers.GetVRCUiMInstance().menuContent();
                foreach (Text txt in quickMenu.transform.Find("Popups/InputPopup/Keyboard/Keys").GetComponentsInChildren<Text>(true))
                    normalColorText.Add(txt);
                foreach (Text txt in quickMenu.transform.Find("Popups/InputKeypadPopup/Keyboard/Keys").GetComponentsInChildren<Text>(true))
                    normalColorText.Add(txt);
                normalColorText.Add(quickMenu.transform.Find("Screens/Settings/VolumePanel/VolumeGameWorld/Label").GetComponentInChildren<Text>(true));
                normalColorText.Add(quickMenu.transform.Find("Screens/Settings/VolumePanel/VolumeGameVoice/Label").GetComponentInChildren<Text>(true));
                normalColorText.Add(quickMenu.transform.Find("Screens/Settings/VolumePanel/VolumeGameAvatars/Label").GetComponentInChildren<Text>(true));
                normalColorText.AddRange(quickMenu.transform.Find("Screens/Social/UserProfileAndStatusSection").GetComponentsInChildren<Text>(true));
                normalColorText.Add(quickMenu.transform.Find("Popups/LoadingPopup/ProgressPanel/Parent_Loading_Progress/Loading Elements/txt_LOADING_Size").GetComponentInChildren<Text>(true));
                normalColorText.Add(quickMenu.transform.Find("Popups/LoadingPopup/MirroredElements/ProgressPanel (1)/Parent_Loading_Progress/Loading Elements/txt_LOADING_Size").GetComponentInChildren<Text>(true));

                #endregion
            }
            
            foreach (var img in normalColorImage)
                if (img != null)
                    img.color = colorT;
            foreach (var img in dimmerColorImage)
                if (img != null)
                    img.color = dimmerT;
            foreach (var img in darkerColorImage)
                if (img != null)
                    img.color = darkerT;
            foreach (var txt in normalColorText)
                if (txt != null)
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

            if (UnityEngine.Resources.FindObjectsOfTypeAll<HighlightsFXStandalone>().Count != 0)
                UnityEngine.Resources.FindObjectsOfTypeAll<HighlightsFXStandalone>().FirstOrDefault().highlightColor = color;

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
                    quickMenu.GetComponentsInChildren<Transform>(true).FirstOrDefault(x => x.name == "Row:4 Column:0").GetComponent<Button>().colors = buttonTheme;
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
                } catch (Exception ex) {
                    Con.Error(ex);
                }
            }
            yield break;
        }

        internal static void ColorActionMenu(Color color) {
            foreach (PedalGraphic grph in UnityEngine.Resources.FindObjectsOfTypeAll<PedalGraphic>())
                grph.color = color;
        }

        internal static void ColorLoadingScreenEnvironment(Color color) {
            if (Config.ColorLoadingScreen.Value && !(MelonHandler.Mods.Any(i => i.Info.Name == "BetterLoadingScreen"))) {
                try {
                    GameObject quickMenu = UIWrappers.GetVRCUiMInstance().menuContent();
                    GameObject loadingBackground = quickMenu.transform.Find("Popups/LoadingPopup/3DElements/LoadingBackground_TealGradient/SkyCube_Baked").gameObject;
                    loadingBackground.GetComponent<MeshRenderer>().material.SetTexture("_Tex", MintyResources.basicGradient);
                    loadingBackground.GetComponent<MeshRenderer>().material.SetColor("_Tint", new Color(color.r / 2f, color.g / 2f, color.b / 2f, color.a));
                    loadingBackground.GetComponent<MeshRenderer>().material.SetTexture("_Tex", MintyResources.basicGradient);

                    GameObject initialLoadingBackground = GameObject.Find("LoadingBackground_TealGradient_Music/SkyCube_Baked");
                    initialLoadingBackground.GetComponent<MeshRenderer>().material.SetTexture("_Tex", MintyResources.basicGradient);
                    initialLoadingBackground.GetComponent<MeshRenderer>().material.SetColor("_Tint", new Color(color.r / 2f, color.g / 2f, color.b / 2f, color.a));
                    initialLoadingBackground.GetComponent<MeshRenderer>().material.SetTexture("_Tex", MintyResources.basicGradient);
                } catch (Exception e) {
                    Con.Error(e);
                }
            }
        }
    }
}
