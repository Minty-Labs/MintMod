using System;
using System.Collections;
using System.Linq;
using MelonLoader;
using MintMod.Functions;
using MintMod.Resources;
using ReMod.Core.UI.QuickMenu;
using ReMod.Core.VRChat;
using UnityEngine;
using UnityEngine.UI;
using VRC.UI.Core;
using MintMod.Managers;
using MintyLoader;

namespace MintMod.UserInterface.QuickMenu {
    internal class MintUserInterface : MintSubMod {
        public override string Name => "Mint UI";
        public override string Description => "Builds all of the Mod's menu.";

        private static GameObject TheMintMenuButton;

        private static ReMenuCategory MintCategoryOnLaunchPad, BaseActions/*, MintQuickActionsCat,*/;

        public static ReCategoryPage MintMenu/*, AvatarMenu*/;

        internal static ReMenuSlider FlightSpeedSlider;

        internal static ReMenuToggle 
            MainQMFly, MainQMNoClip, MainQMFreeze, MainQMInfJump,
            MintQAFly, MintQANoClip, MintQAFreeze;

        // internal static bool isStreamerModeOn;

		internal static IEnumerator OnQuickMenu() {
            while (UIManager.field_Private_Static_UIManager_0 == null) yield return null;
            while (GameObject.Find("UserInterface").GetComponentInChildren<VRC.UI.Elements.QuickMenu>(true) == null) yield return null;

            yield return BuildStandard();
            yield return BuildMint();
        }

        // internal static IEnumerator OnSettingsPageInit() {
        //     while (GameObject.Find("UserInterface/MenuContent/Screens/Settings/ComfortSafetyPanel/StreamerModeToggle") == null)
        //         yield return null;
        //     
        //     // var toggle = GameObject.Find("UserInterface/MenuContent/Screens/Settings/ComfortSafetyPanel/StreamerModeToggle").GetComponent<UiSettingConfig>();
        //     // isStreamerModeOn = toggle.Method_Private_Boolean_0();
        //     //
        //     // yield return new WaitForSeconds(15);
        //     // UpdateMintIconForStreamerMode(isStreamerModeOn);
        // }

        private static IEnumerator BuildStandard() {
            var f = QuickMenuEx.Instance.field_Public_Transform_0.Find("Window/QMParent/Menu_Dashboard");
            // MainMenuBackButton = f.Find("Header_H1/LeftItemContainer/Button_Back").gameObject;
            try {
                if (MelonHandler.Mods.Any(i => i.Info.Name != "AdBlocker")) {
                    f.Find("ScrollRect/Viewport/VerticalLayoutGroup/Carousel_Banners").gameObject.SetActive(false);
                    f.Find("ScrollRect/Viewport/VerticalLayoutGroup/VRC+_Banners").gameObject.SetActive(false);
                }
            }
            catch {
                Con.Warn("Action from AdBlocker failed. Ignoring");
            }
            
            var launchPad = new ReCategoryPage(f);
            MintCategoryOnLaunchPad = launchPad.AddCategory(MintCore.Fool ? "Walmart Client" : "MintMod");
            MintCategoryOnLaunchPad!.Active = Config.KeepFlightBTNsOnMainMenu.Value || Config.KeepPhotonFreezesOnMainMenu.Value || Config.KeepInfJumpOnMainMenu.Value;

            MainQMFly = MintCategoryOnLaunchPad?.AddToggle("Flight", "Toggle Flight", b =>
                    MintQAFly.Toggle(b, true, true));

            MainQMNoClip = MintCategoryOnLaunchPad?.AddToggle("No Clip", "Toggle No Clip", b =>
                MintQANoClip.Toggle(b, true, true));

            MainQMFreeze = MintCategoryOnLaunchPad?.AddToggle("Photon Freeze", "Freeze yourself for other players, voice will still work",
                b => MintQAFreeze.Toggle(b, true, true));

            MainQMInfJump = MintCategoryOnLaunchPad?.AddToggle("Infinite Jump", "What is more to say? Infinitely Jump to your heart's content",
                b => InfJump.Toggle(b, true, true));

            MainQMFly!.Active = Config.KeepFlightBTNsOnMainMenu.Value;
            MainQMNoClip!.Active = Config.KeepFlightBTNsOnMainMenu.Value;
            MainQMFreeze!.Active = Config.KeepPhotonFreezesOnMainMenu.Value;
            MainQMInfJump!.Active = Config.KeepInfJumpOnMainMenu.Value;
            Con.Debug("Done Setting up StandardMenus", MintCore.IsDebug);
            yield break;
        }

        private static IEnumerator BuildMint() {
            MintMenu = new ReCategoryPage(MintCore.Fool ? "WalmartMenu" : "MintMenu", true);
            MintMenu.GameObject.SetActive(false);

            //if (Config.useTabButtonForMenu.Value)
                ReTabButton.Create("MintTab", "Open the MintMenu", MintCore.Fool ? "WalmartMenu" : "MintMenu", 
                    MintCore.Fool ? MintyResources.WalmartTab : MintyResources.MintTabIcon);
            // else {
            //     TheMintMenuButton = Object.Instantiate(MainMenuBackButton, MainMenuBackButton.transform.parent);
            //     TheMintMenuButton.transform.SetAsLastSibling();
            //     TheMintMenuButton.transform.Find("Badge_UnfinishedFeature").gameObject.SetActive(false);
            //     yield return SetTheFuckingSprite();
            //     TheMintMenuButton.SetActive(true);
            //     TheMintMenuButton.name = "MintMenuButtonOvertakenBackButton";
            //     TheMintMenuButton.GetComponent<Button>().onClick.RemoveAllListeners();
            //     TheMintMenuButton.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
            //     TheMintMenuButton.GetComponent<Button>().onClick.AddListener(new Action(MintMenu.Open));
            // }

            BaseActions = MintMenu.AddCategory("Menus", false);

            MintQuickActions();
            PlayerMenu.MenuSetup(BaseActions);
            WorldMenu.BuildWorld(BaseActions);
            UtilityMenu.RandomStuff(BaseActions);
            PlayerListControls.PlayerListOptions(BaseActions);
            //BuildAvatarMenu();
            NameplateMenu.BuildNameplateMenu(BaseActions);
            MintInfo.BuildMenu(BaseActions);
            
            // TEST
            // JumpSelection();
            
            // Build Last
            UserSelectMenu.UserSelMenu();

            if (Config.CopyReModMedia.Value)
                yield return QmMediaPanel.CreateMediaDebugPanel();
            QmMediaPanel.MediaReady = true;

            Con.Debug("Done Setting up MintMenus", MintCore.IsDebug);
        }

        // private static IEnumerator SetTheFuckingSprite() {
        //     yield return null;
        //     Object.DestroyImmediate(TheMintMenuButton.transform.Find("Icon").GetComponent<StyleElement>());
        //     _mintIcon = TheMintMenuButton.transform.Find("Icon").GetComponent<Image>();
        //     _mintIcon.sprite = MintCore.Fool ? MintyResources.WalmartTab : MintyResources.MintIcon;
        //     _mintIcon.color = Color.white;
        //     var styleElement = TheMintMenuButton.GetComponent<StyleElement>();
        //     if (styleElement.field_Public_String_0 == "Back") // Ignore Style Changes
        //         styleElement.field_Public_String_0 = "MintMenuButton";
        //     else styleElement.field_Public_String_1 = "MintMenuButton";
        // }

        #region Quick Actions

        internal static ReMenuToggle InfJump;

        private static void MintQuickActions() {
            var c = MintMenu.AddCategory("Quick Functions", false);
            MintQAFly = c.AddToggle("Flight", "Toggle Flight", b => {
                Movement.Fly(b);
                MainQMFly.Toggle(b, false, true);
            });
            MintQANoClip = c.AddToggle("No Clip", "Toggle No Clip", b => {
                Movement.NoClip(b);
                MainQMNoClip.Toggle(b, false, true);
            });
            MintQAFreeze = c.AddToggle("Photon Freeze", "Freeze yourself for other players, voice will still work", b => {
                PhotonFreeze.ToggleFreeze(b);
                MainQMFreeze.Toggle(b, false, true);
            });
            InfJump = c.AddToggle("Infinite Jump", "What is more to say? Infinitely Jump to your heart's content", b => {
                PlayerActions.InfiniteJump = b;
                MainQMInfJump.Toggle(b, false, true);
            });
            
            var sc = MintMenu.AddSliderCategory("Flight Speed");
            FlightSpeedSlider = sc.AddSlider("Flight Speed", "Control Flight Speed", f => Movement.finalSpeed = f,
                1f, 0.5f, 5f);
            sc.Header.Active = false;
            
            Con.Debug("Done Creating QuickActions", MintCore.IsDebug);
        }

        #endregion

        #region Avatar Menu
/*
        private static ReMenuToggle AFToggle, AFConsole, WebOrLocal;
        private static ReMenuButton SendAFToHost;
        private static void BuildAvatarMenu() {
            AvatarMenu = BaseActions.AddCategoryPage("Avatar Favorites", "Avatar Favorites related options", MintyResources.user);
            var c = AvatarMenu.AddCategory("Config Options", false);

            AFToggle = c.AddToggle("Enabled", "Toggle your Mint Avatar Favorites list", b => {
                Config.SavePrefValue(Config.Avatar, Config.AviFavsEnabled, b);
                if (AFConsole != null && WebOrLocal != null) {
                    AFConsole.Active = b;
                    WebOrLocal.Active = b;
                    if (MintCore.isDebug && WebOrLocal != null && SendAFToHost != null) {
                        WebOrLocal.Active = b;
                        SendAFToHost.Active = b;
                    }
                }
            }, Config.AviFavsEnabled.Value);
            
            AFConsole = c.AddToggle("Console Logs", "Toggle whether there is console logging with Un/Favoriting avatars.", b => {
                Config.SavePrefValue(Config.Avatar, Config.AviLogFavOrUnfavInConsole, b);
            }, Config.AviLogFavOrUnfavInConsole.Value);
            
            WebOrLocal = c.AddToggle("Use Webhost List", "Toggle whether you want to use Local List or a webhost saved list..", b => {
                Config.SavePrefValue(Config.Avatar, Config.useWebhostSavedList, b);
            }, Config.useWebhostSavedList.Value);
            WebOrLocal.Active = MintCore.isDebug;

            SendAFToHost = c.AddButton("Send to Webhost", "Sends your current list to Mint's webhost.", () => {
                VRCUiPopupManager.field_Private_Static_VRCUiPopupManager_0.ShowStandardPopupV2("Are you sure?", "Are you sure you want to upload your current list to Mint's webhost?", 
                    "Cancel", VRCUiPopupManager.field_Private_Static_VRCUiPopupManager_0.HideCurrentPopup,
                    "Send", () => {
                        //AviFavSetup.Favorites.SendLocalListToServer();
                        VRCUiPopupManager.field_Private_Static_VRCUiPopupManager_0.HideCurrentPopup();
                    }, null);
            }, MintyResources.extlink);
            SendAFToHost.Active = MintCore.isDebug;
        }
*/
        #endregion

        // #region DEBUG Jump Selection
        //
        // private static void JumpSelection() {
        //     var f = new ReRadioTogglePage("JumpSelection");
        //     f.AddItem("prop_Boolean_0", null, () => PlayerActions.JumpNum = 1);
        //     f.AddItem("prop_Boolean_1", null, () => PlayerActions.JumpNum = 2);
        //     f.AddItem("prop_Boolean_2", null, () => PlayerActions.JumpNum = 3);
        //     f.AddItem("prop_Boolean_3", null, () => PlayerActions.JumpNum = 4);
        //     f.AddItem("prop_Boolean_4", null, () => PlayerActions.JumpNum = 5);
        //     f.AddItem("field_Private_Boolean_0", null, () => PlayerActions.JumpNum = 6);
        //     f.AddItem("field_Private_Boolean_1", null, () => PlayerActions.JumpNum = 7);
        //     f.AddItem("Method_Public_Boolean_0()", null, () => PlayerActions.JumpNum = 8);
        //     f.AddItem("Method_Public_Boolean_1()", null, () => PlayerActions.JumpNum = 9);
        //     f.AddItem("Method_Public_Boolean_2()", null, () => PlayerActions.JumpNum = 10);
        //     f.OnClose += () => Con.Debug($"Jump: {PlayerActions.JumpNum}");
        //     BaseActions.AddButton("Jump Selection", "yes", f.Open);
        // }
        //
        // #endregion

        internal override void OnLevelWasLoaded(int buildindex, string sceneName) {
            if (buildindex != -1) return;
            PhotonFreeze.ToggleFreeze(false);
            ESP.ClearAllPlayerESP();
            InfJump?.Toggle(Config.KeepInfJumpAlwaysOn.Value, true, true);
            MainQMInfJump?.Toggle(Config.KeepInfJumpAlwaysOn.Value, true, true);
            WorldMenu.OnWorldChange();
        }

        internal override void OnUpdate() => PlayerActions.UpdateJump();

        internal override void OnPrefSave() {
            //DeviceType?.Toggle(Config.SpoofDeviceType.Value);
            UtilityMenu.FrameSpoof?.Toggle(Config.SpoofFramerate.Value);
            UtilityMenu.PingSpoof?.Toggle(Config.SpoofPing.Value);
            UtilityMenu.PingNegative?.Toggle(Config.SpoofedPingNegative.Value);
            UtilityMenu.BypassRiskyFunc?.Toggle(Config.bypassRiskyFunc.Value);
            PlayerListControls.PlEnabled?.Toggle(Config.PLEnabled.Value);
            if (UtilityMenu.Frame != null)
                UtilityMenu.Frame.Text = $"{Config.SpoofedFrameNumber.Value}";
            if (UtilityMenu.Ping != null)
                UtilityMenu.Ping.Text = $"<color={(Config.SpoofedPingNegative.Value ? "red>-" : "#00ff00>")}{Config.SpoofedPingNumber.Value}</color>";
            
            if (MainQMFly != null && MainQMNoClip != null) {
                MainQMFly.Active = Config.KeepFlightBTNsOnMainMenu.Value;
                MainQMNoClip.Active = Config.KeepFlightBTNsOnMainMenu.Value;
            }
            if (MainQMFreeze != null)
                MainQMFreeze.Active = Config.KeepPhotonFreezesOnMainMenu.Value;
            if (MainQMInfJump != null)
                MainQMInfJump.Active = Config.KeepInfJumpOnMainMenu.Value;

            QmMediaPanel.OnPrefSaved();
            NameplateMenu.OnPrefSaved();
        }

        // internal static void UpdateMintIconForStreamerMode(bool o) {
        //     // if (MintIcon != null && !Config.useTabButtonForMenu.Value) {
        //     //     MintIcon.sprite = o ? MintyResources.Transparent : MintyResources.MintIcon;
        //     //     MintIcon.color = Color.white;
        //     // }
        //
        //     if (userSelectCategory != null) {
        //         userSelectCategory.RectTransform.gameObject.SetActive(!o);
        //         userSelectCategory.Active = !o;
        //         userSelectCategory.Title = o ? "" : "MintMod";
        //     }
        //
        //     if (MintCategoryOnLaunchPad != null) {
        //         MintCategoryOnLaunchPad.RectTransform.gameObject.SetActive(!o);
        //         MintCategoryOnLaunchPad.Active = !o;
        //         MintCategoryOnLaunchPad.Title = o ? "" : "MintMod";
        //     }
        //     
        //     if (Config.SpoofFramerate.Value)
        //         Config.SavePrefValue(Config.mint, Config.SpoofFramerate, false);
        //     if (Config.SpoofPing.Value)
        //         Config.SavePrefValue(Config.mint, Config.SpoofPing, false);
        //
        //     //if (Config.useTabButtonForMenu.Value && o) {
        //         var msg = "Streamer Mode detected, Mint Tab Button is still visible.";
        //         Con.Warn(msg);
        //         VRCUiManager.field_Private_Static_VRCUiManager_0.QueueHudMessage(msg, Color.white, 8f);
        //     //}
        // }
    }
}
