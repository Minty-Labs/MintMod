using System;
using System.Collections;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using MelonLoader;
using MintMod.Functions;
using MintMod.Resources;
using ReMod.Core.UI.QuickMenu;
using ReMod.Core.VRChat;
using UnityEngine;
using UnityEngine.UI;
using VRC;
using VRC.Core;
using VRC.SDKBase;
using VRC.UI;
using VRC.UI.Core.Styles;
using VRC.UI.Core;
using Button = UnityEngine.UI.Button;
using MintMod.Managers;
using MintMod.Reflections;
using MintMod.Utils;
using MintyLoader;
using MintMod.Functions.Authentication;
using MintMod.Libraries;
using MintMod.Managers.Notification;
using MintMod.UserInterface.AvatarFavs;
using MintMod.UserInterface.OldUI;

namespace MintMod.UserInterface.QuickMenu {
    internal class MintUserInterface : MintSubMod {
        public override string Name => "Mint UI";
        public override string Description => "Builds all of the Mod's menu.";

        private static GameObject MainMenuBackButton, TheMintMenuButton, ShittyAdverts, ShittyAdverts_2, LaunchPadLayoutGroup;

        private static ReMenuCategory MintCategoryOnLaunchPad, BaseActions, /*MintQuickActionsCat,*/ userSelectCategory, playerListCategory;

        public static ReCategoryPage MintMenu, PlayerMenu, WorldMenu, RandomMenu, PlayerListMenu, PlayerListConfig, AvatarMenu;

        private static ReMenuSlider FlightSpeedSlider;

        internal static ReMenuToggle 
            MainQMFly, MainQMNoClip, MainQMFreeze, MainQMInfJump,
            MintQAFly, MintQANoClip, MintQAFreeze;

        //private static Sprite WorldIcon, PlayerIcon;

        internal static ReMenuToggle ItemESP, PlayerESP, DeviceType, FrameSpoof, PingSpoof, PingNegative, bypassRiskyFunc;
        public static ReMenuButton Frame, Ping;

        public static Image MintIcon;

        internal static bool isStreamerModeOn;

        #region Temp TPVR Values

        //public static MelonPreferences_Category melon;
        //public static bool TPVR_active;

        #endregion

		internal static IEnumerator OnQuickMenu() {
            while (UIManager.prop_UIManager_0 == null) yield return null;
            while (UnityEngine.Object.FindObjectOfType<VRC.UI.Elements.QuickMenu>() == null) yield return null;
            
            MelonCoroutines.Start(BuildStandard());
            MelonCoroutines.Start(BuildMint());
            //MelonCoroutines.Start(ReColor.DelayedHfxReColor(ReColor.finalColor));
            if (!isStreamerModeOn) UserSelMenu();
        }

        internal static IEnumerator OnSettingsPageInit() {
            while (GameObject.Find("UserInterface/MenuContent/Screens/Settings/ComfortSafetyPanel/StreamerModeToggle") == null)
                yield return null;
            
            var toggle = GameObject.Find("UserInterface/MenuContent/Screens/Settings/ComfortSafetyPanel/StreamerModeToggle").GetComponent<UiSettingConfig>();
            isStreamerModeOn = toggle.Method_Private_Boolean_0();

            yield return new WaitForSeconds(15);
            UpdateMintIconForStreamerMode(isStreamerModeOn);
        }

        static IEnumerator BuildStandard() {
            
            MainMenuBackButton = GameObject.Find("/UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Dashboard/Header_H1/LeftItemContainer/Button_Back");
            try {
                if (MelonHandler.Mods.Any((i) => i.Info.Name != "AdBlocker")) {
                    ShittyAdverts =
                        GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Carousel_Banners");
                    ShittyAdverts.SetActive(false);
                    ShittyAdverts_2 =
                        GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/VRC+_Banners");
                    ShittyAdverts_2.SetActive(false);
                }
            }
            catch {
                Con.Error("Action from AdBlocker failed. Ignoring");
            }

            LaunchPadLayoutGroup = GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup");
            MintCategoryOnLaunchPad = new("MintMod", LaunchPadLayoutGroup.transform);
            MintCategoryOnLaunchPad.Active = false;
            
            if (Config.KeepFlightBTNsOnMainMenu.Value || Config.KeepPhotonFreezesOnMainMenu.Value || Config.KeepInfJumpOnMainMenu.Value || ModCompatibility.TeleporterVR) 
                MintCategoryOnLaunchPad.Active = true;
            
            if (LaunchPadLayoutGroup != null) {
                MainQMFly = MintCategoryOnLaunchPad.AddToggle("Flight", "Toggle Flight", Movement.Fly);
                MainQMNoClip = MintCategoryOnLaunchPad.AddToggle("No Clip", "Toggle No Clip", Movement.NoClip);
                
                MainQMFreeze = MintCategoryOnLaunchPad.AddToggle("Photon Freeze", "Freeze yourself for other players, voice will still work", PhotonFreeze.ToggleFreeze);
                
                MainQMInfJump = MintCategoryOnLaunchPad.AddToggle("Infinite Jump", "What is more to say? Infinitely Jump to your heart's content",
                    onToggle => PlayerActions.InfinteJump = onToggle);
                
                MainQMFly.Active = Config.KeepFlightBTNsOnMainMenu.Value;
                MainQMNoClip.Active = Config.KeepFlightBTNsOnMainMenu.Value;
                MainQMFreeze.Active = Config.KeepPhotonFreezesOnMainMenu.Value;
                MainQMInfJump.Active = Config.KeepInfJumpOnMainMenu.Value;
                
                /*if (ModCompatibility.TeleporterVR) {
                    var ver = MelonHandler.Mods.Single(m => m.Info.Name.Equals("TeleporterVR")).Info.Version;
                    var p = new Version(ver);
                    if (p >= new Version("4.9.0")) {
                        melon = MelonPreferences.CreateCategory("TeleporterVR", "TeleporterVR");
                        try { MelonPreferences.GetEntry<bool>(melon.Identifier, "ShowUIXTPVRButton").Value = false; }
                        catch (Exception t) { Con.Debug($"Could not set value:\n{t}", MintCore.isDebug); }
                        MintCategoryOnLaunchPad.AddToggle("TeleporterVR", "Toggle TeleporterVR\'s TPVR function.", t => TPVR_active = t);
                    }
                }
                */
            }
            Con.Debug("Done Setting up StandardMenus", MintCore.isDebug);
            yield break;
        }

        static IEnumerator BuildMint() {
            MintMenu = new ReCategoryPage("MintMenu", Config.useTabButtonForMenu.Value);
            MintMenu.GameObject.SetActive(false);

            if (Config.useTabButtonForMenu.Value)
                ReTabButton.Create("MintTab", "Open the MintMenu", "MintMenu", MintyResources.MintTabIcon);
            else {
                TheMintMenuButton = UnityEngine.Object.Instantiate(MainMenuBackButton, MainMenuBackButton.transform.parent);
                TheMintMenuButton.transform.SetAsLastSibling();
                TheMintMenuButton.transform.Find("Badge_UnfinishedFeature").gameObject.SetActive(false);
                MelonCoroutines.Start(SetTheFuckingSprite());
                TheMintMenuButton.SetActive(true);
                TheMintMenuButton.name = "MintMenuButtonOvertakenBackButton";
                TheMintMenuButton.GetComponent<Button>().onClick.RemoveAllListeners();
                TheMintMenuButton.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
                TheMintMenuButton.GetComponent<Button>().onClick.AddListener(new Action(MintMenu.Open));
            }

            BaseActions = MintMenu.AddCategory("Menus", false);

            MintQuickActions();
            Player();
            World();
            RandomStuff();
            PlayerListMenuSetup();
            PlayerListOptions();
            //MediaControls();
            //BuildAvatarMenu();

            Con.Debug("Done Setting up MintMenus", MintCore.isDebug);
            yield break;
        }

        private static IEnumerator SetTheFuckingSprite() {
            yield return new WaitForFixedUpdate();
            UnityEngine.Object.DestroyImmediate(TheMintMenuButton.transform.Find("Icon").GetComponent<StyleElement>());
            MintIcon = TheMintMenuButton.transform.Find("Icon").GetComponent<Image>();
            MintIcon.sprite = MintyResources.MintIcon;
            MintIcon.color = Color.white;
            var styleElement = TheMintMenuButton.GetComponent<StyleElement>();
            if (styleElement.field_Public_String_0 == "Back") // Ignore Style Changes
                styleElement.field_Public_String_0 = "MintMenuButton";
            else styleElement.field_Public_String_1 = "MintMenuButton";
        }

        #region Quick Actions

        private static void MintQuickActions() {
            var c = MintMenu.AddCategory("Quick Functions", false);
            MintQAFly = c.AddToggle("Flight", "Toggle Flight", Movement.Fly);
            MintQANoClip = c.AddToggle("No Clip", "Toggle No Clip", Movement.NoClip);
            MintQAFreeze = c.AddToggle("Photon Freeze", "Freeze yourself for other players, voice will still work", PhotonFreeze.ToggleFreeze);
            InfJump = c.AddToggle("Infinite Jump", "What is more to say? Infinitely Jump to your heart's content",
                onToggle => PlayerActions.InfinteJump = onToggle);
            
            var sc = MintMenu.AddSliderCategory("Flight Speed");
            FlightSpeedSlider = sc.AddSlider("Min: 0.5", "Control Flight Speed", f => Movement.finalSpeed = f,
                1f, 0.5f, 5f);
            
            Con.Debug("Done Creating QuickActions", MintCore.isDebug);
        }

        #endregion

        #region Player Menu

        private static ReMenuToggle InfJump;
        
        private static void Player() {
            PlayerMenu = BaseActions.AddCategoryPage("Player", "Actions involving players.", MintyResources.people);
            var c = PlayerMenu.AddCategory("General Actions", false);
            PlayerESP = c.AddToggle("Player ESP", "Puts a bubble around each player, and is visible through walls.", ESP.PlayerESPState);
            c.AddButton("Copy Current Avi ID", "Copys your current Avatar ID into your clipboard", () => {
                try {
                    Clipboard.SetText(APIUser.CurrentUser.avatarId);
                }
                catch (Exception c) {
                    Con.Error(c);
                }
            }, MintyResources.clipboard);
            c.AddButton("Go into Avi by ID", "Takes an Avatar ID from your clipboard and changes into that avatar.", () => {
                try {
                    string clip = string.Empty;
                    try { clip = GUIUtility.systemCopyBuffer; }
                    catch { clip = Clipboard.GetText(); }

                    if (clip.Contains("avtr_") && !string.IsNullOrWhiteSpace(clip)) {
                        PageAvatar a = new() { field_Public_SimpleAvatarPedestal_0 = new() };
                        new ApiAvatar { id = clip }.Get(new Action<ApiContainer>(x => {
                                a.field_Public_SimpleAvatarPedestal_0.field_Internal_ApiAvatar_0 = x.Model.Cast<ApiAvatar>();
                                a.ChangeToSelectedAvatar();
                        }));
                    }
                    else {
                        VRCUiPopups.Notify("No Avatar ID in clipboard", NotificationSystem.Alert);
                        //VRCUiManager.field_Private_Static_VRCUiManager_0.InformHudText("No avatar ID in clipboard", Color.white);
                    }
                }
                catch (Exception c) {
                    Con.Error(c);
                }
            }, MintyResources.checkered);
            //var h = PlayerMenu.AddCategory("Head Lamp");
        }

        #endregion

        #region World Menu

        private static void World() {
            WorldMenu = BaseActions.AddCategoryPage("World", "Actions involving the world.", MintyResources.globe);
            var w = WorldMenu.AddCategory("General Actions");
            ItemESP = w.AddToggle("Item ESP", "Puts a bubble around all Pickups, can be seen through walls", ESP.SetItemESPToggle);
            w.AddButton("Add Jump", "Allows you to jump in the world", WorldActions.AddJump, MintyResources.jump);
            //w.AddButton("Legacy Locomotion", "Adds old SDK2 movement in the current SDK3 world",
            //    VRCPlayer.field_Internal_Static_VRCPlayer_0.field_Private_VRCPlayerApi_0.UseLegacyLocomotion, MintyResources.history);
            w.AddSpacer();
            w.AddButton("Download VRCW", "Downloads the world file (.vrcw)", WorldActions.WorldDownload, MintyResources.dl);

            w.AddButton("Copy Instance ID URL", "Copies current instance ID and places it in your system's clipboard.", () => {
                    string id = RoomManager.field_Internal_Static_ApiWorld_0.id;
                    string instance = RoomManager.field_Internal_Static_ApiWorldInstance_0.instanceId;
                    bool faulted = false;
                    try {
                        GUIUtility.systemCopyBuffer = $"https://vrchat.com/home/launch?worldId={id}&instanceId={instance}";
                    }
                    catch {
                        Clipboard.SetText($"https://vrchat.com/home/launch?worldId={id}&instanceId={instance}");
                        faulted = true;
                    }

                    Con.Msg(faulted ? "Failed to copy instance ID" : $"Got ID: {RoomManager.field_Internal_Static_ApiWorldInstance_0.id}");
                }, MintyResources.clipboard);
            w.AddButton("Join Instance by ID", "Join the room of the instance ID", () => {
                try {
                    string clip = string.Empty;
                    try { clip = GUIUtility.systemCopyBuffer; } catch { clip = Clipboard.GetText(); }
                    
                    if (clip.Contains("launch?")) {
                        Networking.GoToRoom(clip
                            .Replace("https://vrchat.com/home/launch?worldId=", "")
                            .Replace("&instanceId=", ":"));
                    } else if (clip.Contains("wrld_")) {
                        Networking.GoToRoom(clip);
                    }
                    else Con.Warn("Clipboard text is probably not a valid join link.");
                }
                catch (Exception j) {
                    Con.Error(j);
                }
            }, MintyResources.globe);
            w.AddSpacer();
            w.AddButton("Log World", "Logs world info (of various data points) in a text file.", WorldActions.LogWorld, MintyResources.list);
            
            w.AddButton("Normal World Mirrors", "Reverts mirrors to their original state", WorldActions.RevertMirrors);
            w.AddButton("Optimize Mirrors", "Make Mirrors only show players", WorldActions.OptimizeMirrors);
            w.AddButton("Beautify Mirrors", "Make Mirrors show everything", WorldActions.BeautifyMirrors);
            w.AddButton("Reset Portal", $"Sets portal timers to {Config.ResetTimerAmount.Value}", () => {
                if (UnityEngine.Object.FindObjectsOfType<PortalInternal>() == null) return;
                var single = default(Il2CppSystem.Single);
                single.m_value = Config.ResetTimerAmount.Value < 30 ? 30 : Config.ResetTimerAmount.Value;
                var @object = single.BoxIl2CppObject();
                var array = UnityEngine.Resources.FindObjectsOfTypeAll<PortalTrigger>();
                foreach (var portal in array) {
                    if (!portal.gameObject.activeInHierarchy) return;
                    if (portal.gameObject.GetComponentInParent<VRC_PortalMarker>() == null) return;
                    Networking.RPC(RPC.Destination.AllBufferOne, portal.gameObject, "SetTimerRPC",
                        new[] { @object });
                }
                /*for (int i = 0; i < array.Length; i++) {
                    if (array[i].gameObject.activeInHierarchy && !(array[i].gameObject.GetComponentInParent<VRC_PortalMarker>() != null)) 
                        Networking.RPC(RPC.Destination.AllBufferOne, array[i].gameObject, "SetTimerRPC",
                            new Il2CppSystem.Object[1] { @object });
                }*/
            }, MintyResources.history);

            var e = WorldMenu.AddCategory("Item Manipulation");
            e.AddButton("Teleport Items to Self", "Teleports all Pickups to your feet.", Items.TPToSelf);
            e.AddButton("Respawn Items", "Respawns All pickups to their original location.", Items.Respawn);
            e.AddButton("Teleport Items Out of World", "Teleports all Pickups an XYZ coord of 1 million", Items.TPToOutWorld);
            
            var ct = WorldMenu.AddCategory("Component Toggle");
            Components.ComponentToggle(ct);
        }

        #endregion

        #region Utility Menu

        private static void RandomStuff() {
            RandomMenu = BaseActions.AddCategoryPage("Utilities", "Contains random functions", MintyResources.cog);
            RandomMenu.AddCategory($"MintMod - v<color=#9fffe3>{MintCore.ModBuildInfo.Version}</color>", false);
            var r = RandomMenu.AddCategory("General Actions", false);
            
            /*DeviceType = r.AddToggle("Spoof as Quest", "Spoof your VRChat login as Quest.",
                on => MelonPreferences.GetEntry<bool>(Config.mint.Identifier, Config.SpoofDeviceType.Identifier).Value = on);
            DeviceType.Toggle(Config.SpoofDeviceType.Value);
            
            r.AddSpacer();
            
            */

            PingSpoof = r.AddToggle("Spoof Ping", "Spoof your ping for the monkes.",
                on => MelonPreferences.GetEntry<bool>(Config.mint.Identifier, Config.SpoofPing.Identifier).Value = on);
            PingSpoof.Toggle(Config.SpoofPing.Value);
            
            PingNegative = r.AddToggle("Negative Ping", "Make your spoofed ping negative.",
                on => MelonPreferences.GetEntry<bool>(Config.mint.Identifier, Config.SpoofedPingNegative.Identifier).Value = on);
            PingNegative.Toggle(Config.SpoofedPingNegative.Value);
            
            Ping = r.AddButton($"<color={(Config.SpoofedPingNegative.Value ? "red>-" : "#00ff00>")}{Config.SpoofedPingNumber.Value}</color>", "This is the number of your spoofed ping.", () => {
                VRCUiPopupManager.field_Private_Static_VRCUiPopupManager_0.ShowInputPopupWithCancel("Set Spoofed Ping", "",
                    InputField.InputType.Standard, true, "Set Ping", (_, __, ___) => {
                        int.TryParse(_, out var p);
                        Config.SavePrefValue(Config.mint, Config.SpoofedPingNumber, p);
                        Ping.Text = $"<color={(Config.SpoofedPingNegative.Value ? "red>-" : "#00ff00>")}{p.ToString()}</color>";
                    }, () => VRCUiPopupManager.field_Private_Static_VRCUiPopupManager_0.HideCurrentPopup());
            }, MintyResources.wifi);
            
            bypassRiskyFunc = r.AddToggle("Bypass Risky Func", "Forces Mods with Risky Function Checks to work", 
                on => MelonPreferences.GetEntry<bool>(Config.mint.Identifier, Config.bypassRiskyFunc.Identifier).Value = on);
            bypassRiskyFunc.Toggle(Config.bypassRiskyFunc.Value);
            
            FrameSpoof = r.AddToggle("Spoof Frames", "Spoof your framerate for the monkes.",
                on => MelonPreferences.GetEntry<bool>(Config.mint.Identifier, Config.SpoofFramerate.Identifier).Value = on);
            FrameSpoof.Toggle(Config.SpoofFramerate.Value);
            
            Frame = r.AddButton($"{Config.SpoofedFrameNumber.Value}", "This is the number of your spoofed framerate.", () => {
                VRCUiPopupManager.field_Private_Static_VRCUiPopupManager_0.ShowInputPopupWithCancel("Set Spoofed Framerate", "",
                    InputField.InputType.Standard, true, "Set Frames", (_, __, ___) => {
                        float.TryParse(_, out var f);
                        Config.SavePrefValue(Config.mint, Config.SpoofedFrameNumber, f);
                        Frame.Text = f.ToString();
                    }, () => VRCUiPopupManager.field_Private_Static_VRCUiPopupManager_0.HideCurrentPopup());
            }, MintyResources.tv);

            r.AddButton("Refetch Nameplates",
                "Reloads Mint's custom nameplate addons in case more were added while you're playing",
                () => Players.FetchCustomPlayerObjects(true), MintyResources.extlink);

            r.AddButton("Clear HUD Message Queue", "Clears the HUD Popup Message Queue", () => {
                if (!Config.UseOldHudMessages.Value)
                    NotificationController_Mint.Instance.ClearNotifications();
                else
                    VRCUiManager.prop_VRCUiManager_0.field_Private_List_1_String_0.Clear();
            }, MintyResources.messages);
        }

        #endregion

        #region UserSelect Menu

        private static GameObject TheUserSelectMenu;
        private static void UserSelMenu() {
            TheUserSelectMenu = GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_SelectedUser_Local/ScrollRect/Viewport/VerticalLayoutGroup");

            userSelectCategory = new ReMenuCategory("MintMod", TheUserSelectMenu.transform);

            string u = "selected user";

            userSelectCategory.AddButton("Download Avatar VRCA", $"Downloads {u}'s Avatar .VRCA", PlayerActions.AvatarDownload, MintyResources.dl);
            userSelectCategory.AddButton("Log Asset", $"Logs {u}'s information and put it into a text file", PlayerActions.LogAsset, MintyResources.list);
            userSelectCategory.AddButton("Copy Avatar ID", $"Copies {u}'s avatar ID into your clipboard", () => GUIUtility.systemCopyBuffer = PlayerActions.SelPAvatar().id, MintyResources.copy);
            userSelectCategory.AddButton("Clone Avatar", $"Clones {u}'s avatar if public", () => {
                try {
                    var v = PlayerActions.SelPAvatar();
                    string clip = v.id;
                    if (clip.Contains("avtr_") && !string.IsNullOrWhiteSpace(clip) && v.releaseStatus.ToLower().Contains("public")) {
                        PageAvatar a = new() { field_Public_SimpleAvatarPedestal_0 = new() };
                        new ApiAvatar { id = clip }.Get(new Action<ApiContainer>(x => {
                            a.field_Public_SimpleAvatarPedestal_0.field_Internal_ApiAvatar_0 = x.Model.Cast<ApiAvatar>();
                            a.ChangeToSelectedAvatar();
                        }));
                    } else {
                        VRCUiPopups.Notify("Avatar is private", NotificationSystem.Alert);
                        //VRCUiManager.prop_VRCUiManager_0.InformHudText("Avatar is private", Color.white);
                    }
                } catch (Exception c) {
                    Con.Error(c);
                    Con.Warn("Attempting fallback method");
                    try {
                        var v = PlayerActions.SelPAvatar();
                        PlayerManager.field_Private_Static_PlayerManager_0.field_Private_Player_0._vrcplayer
                            .ChangeToAvatar(v.id);
                    }
                    catch (Exception f) {
                        Con.Error(f);
                    }
                }
            }, MintyResources.clone);

            if (Config.AviFavsEnabled.Value) {
                userSelectCategory.AddButton("Silent Favorite", $"Silently favorites the avatar {u} is wearing if public.", () => {
                    var v = PlayerActions.SelPAvatar();
                    foreach (var f in AvatarFavs.AviFavSetup.Favorites.Instance.AvatarFavorites.FavoriteLists) {
                        if (!v.releaseStatus.ToLower().Contains("private"))
                            AvatarFavs.AviFavLogic.FavoriteAvatar(v, f.ID);
                        else {
                            Con.Warn("Avatar is private, cannot favorite");
                            VRCUiPopups.Notify("Avatar is private, cannot favorite", NotificationSystem.Alert);
                            //VRCUiManager.prop_VRCUiManager_0.InformHudText("Avatar is private, cannot favorite", Color.white);
                        }
                    }
                }, MintyResources.star);
            }

            userSelectCategory.AddButton("Teleport to", $"Teleport to {u}", () => { PlayerActions.Teleport(PlayerWrappers.SelVRCPlayer()); }, MintyResources.marker);
            userSelectCategory.AddButton("Teleport pickups to", $"Teleport all pickup objects to {u}", () => Items.TPToPlayer(PlayerWrappers.SelVRCPlayer()._player), MintyResources.marker_hole);

            if (APIUser.CurrentUser.id == Players.LilyID) {
                userSelectCategory.AddButton("Mint Auth Check", $"Check to see if {u} can use MintMod", () => MelonCoroutines.Start(ServerAuth.SimpleAuthCheck(PlayerWrappers.GetSelectedAPIUser().id)), MintyResources.key);
            }

            Con.Debug("Done Setting up User Selected Menu", MintCore.isDebug);
        }

        #endregion

        #region PlayerList Menu

        private static List<ReMenuButton> PlayerButtons = new();
        private static ReMenuButton SinglePlayerButton;

        private enum PlayerListActions {
            None,
            Teleport,
            OpenQm,
            Esp,
            TeleportObjs,
            OrbitObjs,
            OrbitPlayer
        }

        private static PlayerListActions GetSelectedAction(int type) {
            return type switch {
                1 => PlayerListActions.Teleport,
                2 => PlayerListActions.OpenQm,
                3 => PlayerListActions.Esp,
                4 => PlayerListActions.TeleportObjs,
                5 => PlayerListActions.OrbitObjs,
                6 => PlayerListActions.OrbitPlayer,
                _ => PlayerListActions.None
            };
        }

        private static int SelectedActionNum = 0;

        //private static QMSlider ItemSlider, PlayerSlider;
        private static ReMenuSlider ItemsReSlider_sp, ItemsReSlider_di, PlayerReSlider_sp, PlayerReSlider_di;
        private static ReMenuSliderCategory ItemSliderCat, PlayerSliderCat;
        
        #region Orbit Sliders
        
        private static void CreateSliders() {
            /*
            var categoryLayoutGroup = PlayerListMenu.GameObject.transform.Find("ScrollRect/Viewport/VerticalLayoutGroup");
            try {
                ItemSlider = new QMSlider(categoryLayoutGroup,
                    f => Items.SpinSpeed = f, "Speed", "Change speed of rotation.", 2f, 0f, 1f,
                    f2 => Items.Distance = f2, "Distance", "Change Distance of rotation", 5f, 0.1f, 1f);
                ItemSlider.Enabled = false;
                
                PlayerSlider = new QMSlider(categoryLayoutGroup,
                    f => Players.SelfSpinSpeed = f, "Speed", "Change speed of rotation.", 2f, 0f, 1f,
                    f2 => Players.SelfDistance = f2, "Distance", "Change Distance of rotation", 5f, 0.1f, 1f);
                PlayerSlider.Enabled = false;
            }
            catch (Exception e) {
                Con.Error(e);
            }
            */
            ItemSliderCat = PlayerListMenu.AddSliderCategory("Items");
            ItemsReSlider_sp = ItemSliderCat.AddSlider("Speed", "Change orbit speed of rotation", f => Items.SpinSpeed = f, 1f, 0f, 2f);
            ItemsReSlider_di = ItemSliderCat.AddSlider("Distance", "Change distance of rotation", f => Items.Distance = f, 1f, 0f, 5f);
            ItemSliderCat.Header.GameObject.SetActive(false);
            ItemsReSlider_sp.Active = false;
            ItemsReSlider_di.Active = false;
                
            PlayerSliderCat = PlayerListMenu.AddSliderCategory("Players");
            PlayerReSlider_sp = PlayerSliderCat.AddSlider("Speed", "Change orbit speed of rotation", f => Players.SelfSpinSpeed = f, 1f, 0f, 2f);
            PlayerReSlider_di = PlayerSliderCat.AddSlider("Distance", "Change distance of rotation", f => Players.SelfDistance = f, 1f, 0.1f, 5f);
            PlayerSliderCat.Header.GameObject.SetActive(false);
            PlayerReSlider_sp.Active = false;
            PlayerReSlider_di.Active = false;
        }

        #endregion
        
        private static void PlayerListMenuSetup() {
            PlayerListMenu = BaseActions.AddCategoryPage("Player List", "Actions to do individually on a player.", MintyResources.address_book);
            var p = PlayerListMenu.AddCategory("Actions");
            CreateSliders();
            var l = PlayerListMenu.AddCategory("Player List (Select an Action)");
            p.AddButton("Teleport", "Teleport to player", () => {
                SelectedActionNum = 1;
                l.Title = "Player List > Teleport";
                //PlayerSlider.Enabled = false;
                //ItemSlider.Enabled = false;
                ItemsReSlider_sp.Active = false;
                ItemsReSlider_di.Active = false;
                PlayerReSlider_sp.Active = false;
                PlayerReSlider_di.Active = false;
            });
            var _1 = p.AddButton("OpenQM", "Open player in Quick Menu", () => {
                SelectedActionNum = 2;
                l.Title = "Player List > OpenQM";
                //PlayerSlider.Enabled = false;
                //ItemSlider.Enabled = false;
                ItemsReSlider_sp.Active = false;
                ItemsReSlider_di.Active = false;
                PlayerReSlider_sp.Active = false;
                PlayerReSlider_di.Active = false;
            });
            p.AddButton("ESP", "Draw a bubble around player", () => {
                SelectedActionNum = 3;
                l.Title = "Player List > ESP";
                //PlayerSlider.Enabled = false;
                //ItemSlider.Enabled = false;
                ItemsReSlider_sp.Active = false;
                ItemsReSlider_di.Active = false;
                PlayerReSlider_sp.Active = false;
                PlayerReSlider_di.Active = false;
            });
            p.AddButton("Teleport\nPickups to", "Teleport all pickups to player", () => {
                SelectedActionNum = 4;
                l.Title = "Player List > Teleport Pickups";
                //PlayerSlider.Enabled = false;
                //ItemSlider.Enabled = false;
                ItemsReSlider_sp.Active = false;
                ItemsReSlider_di.Active = false;
                PlayerReSlider_sp.Active = false;
                PlayerReSlider_di.Active = false;
            });
            var _2 = p.AddButton("Orbit\nPickups", "Orbit pickups around player", () => {
                SelectedActionNum = 5;
                l.Title = "Player List > Orbit Pickups";
                //PlayerSlider.Enabled = false;
                //ItemSlider.Enabled = true;
                ItemsReSlider_sp.Active = true;
                ItemsReSlider_di.Active = true;
                PlayerReSlider_sp.Active = false;
                PlayerReSlider_di.Active = false;
            });
            var _3 = p.AddButton("Orbit\nPlayer", "Orbit around player", () => {
                SelectedActionNum = 6;
                l.Title = "Player List > Orbit Player";
                //PlayerSlider.Enabled = true;
                //ItemSlider.Enabled = false;
                ItemsReSlider_sp.Active = false;
                ItemsReSlider_di.Active = false;
                PlayerReSlider_sp.Active = true;
                PlayerReSlider_di.Active = true;
            });
            // These button are disabled until I add the functions for them
            _1.Interactable = false;
            p.AddButton("<color=#FF96AA>Clear ESPs</color>", "Clears any and all ESP bubbles around players", ESP.ClearAllPlayerESP);
            p.AddButton("<color=#FF96AA>Clear Orbits</color>", "Clear any type of orbiting", () => {
                if (SelectedActionNum == 5) Items.ClearRotating();
                else if (SelectedActionNum == 6) Players.Toggle(false);
                else Con.Warn("Nothing to cancel orbit.");
            });

            PlayerListMenu.OnOpen += () => {
                foreach (var button in PlayerButtons)
                    button.Destroy();
                PlayerButtons.Clear();
                var enumerator2 = PlayerWrappers.GetAllPlayers().GetEnumerator();

                while (enumerator2.MoveNext()) {
                    var player = enumerator2.current;
                    var n = player.field_Private_APIUser_0.displayName;
                    
                    string tempName = String.Empty;
                    if (PlayerWrappers.isFriend(player)) {
                        if (player.field_Private_APIUser_0.id == Players.LilyID)
                            tempName = "<color=#9fffe3>Lily</color>";
                        else
                            tempName = $"<color=#{Config.FriendRankHEX.Value}>{n}</color>";
                    }
                    else {
                        if (player.field_Private_APIUser_0.id == Players.LilyID)
                            tempName = "<color=#9fffe3>Lily</color>";
                        else
                            tempName = $"<color=#{ColorConversion.ColorToHex(VRCPlayer.Method_Public_Static_Color_APIUser_0(player.GetAPIUser()))}>{n}</color>";
                    }
                    
                    SinglePlayerButton = l.AddButton($"{tempName}", "Click to do selected action",
                        () => {
                            switch (GetSelectedAction(SelectedActionNum)) {
                                case PlayerListActions.Teleport:
                                    if (player != PlayerWrappers.GetCurrentPlayer())
                                        PlayerActions.Teleport(player._vrcplayer);
                                    break;
                                case PlayerListActions.OpenQm:
                                    // Action Not Yet Setup
                                    break;
                                case PlayerListActions.Esp:
                                    if (ESP.isESPEnabled)
                                        Con.Warn("Main ESP is already active");
                                    else
                                        if (player != PlayerWrappers.GetCurrentPlayer())
                                            ESP.SinglePlayerESP(player, true);
                                    break;
                                case PlayerListActions.TeleportObjs:
                                    Items.TPToPlayer(player);
                                    break;
                                case PlayerListActions.OrbitObjs:
                                    Items.Toggle(player, !Items.Rotate);
                                    break;
                                case PlayerListActions.OrbitPlayer:
                                    Players.Toggle(!Players.Rotate, player);
                                    break;
                                default:
                                    Con.Warn("Nothing is selected.");
                                    VRCUiPopups.Notify("Noting is selected", NotificationSystem.Alert);
                                    //VRCUiManager.prop_VRCUiManager_0.InformHudText("Nothing is selected", Color.yellow);
                                    break;
                            }
                        });
                    PlayerButtons.Add(SinglePlayerButton);
                }
            };
        }

        #endregion
        
        #region Player List

        private static ReMenuToggle PLEnabled, WingLocation, ExtendList;
        private static ReMenuButton Save;
        private static ReMenuSliderCategory ColorCat;
        private static ReMenuSlider Red, Green, Blue, Alpha, TextSize;
        private static void PlayerListOptions() {
            PlayerListConfig = BaseActions.AddCategoryPage("Player List Config", "Control the player list's options", MintyResources.userlist);
            var c = PlayerListConfig.AddCategory("Player List Config", false);
            var o = Config.PLEnabled.Value;
            
            PLEnabled = c.AddToggle("Enabled", "Toggle the PLayer List", b => {
                Config.SavePrefValue(Config.PlayerList, Config.PLEnabled, b);
                var tempToggle = Config.PLEnabled.Value;
                Save.Active = tempToggle;
                ColorCat.Header.Active = tempToggle;
                Red.Active = tempToggle;
                Green.Active = tempToggle;
                Blue.Active = tempToggle;
                Alpha.Active = tempToggle;
                WingLocation.Active = tempToggle;
                ExtendList.Active = tempToggle;
                TextSize.Active = tempToggle;
            }, Config.PLEnabled.Value);
            WingLocation = c.AddToggle("List on Right Side", "Move the list on the left or right wing", b => 
                Config.SavePrefValue(Config.PlayerList, Config.Location, b ? 1 : 0), Config.Location.Value != 0);
            ExtendList = c.AddToggle("Extend Player Listing", "Show all players regardless on length of box", b => {
                Config.SavePrefValue(Config.PlayerList, Config.uncapListCount, b);
                PlayerInfo.MoveTheText();
            }, Config.uncapListCount.Value);
            Save = c.AddButton("Save Values", "Save the color options below", () => {
                var color = (Color32)PlayerInfo.BackgroundImage.color;
                Config.SavePrefValue(Config.PlayerList, Config.BackgroundColor, color);
                Config.SavePrefValue(Config.PlayerList, Config.TextSize, PlayerInfo.GetTextSize());
            }, MintyResources.cog);
            
            ColorCat = PlayerListConfig.AddSliderCategory("Background Color");
            Red = ColorCat.AddSlider("Red Value", "Shift Red Color Values", f => {
                var c = new Color32((byte)f, Config.BackgroundColor.Value.g,
                    Config.BackgroundColor.Value.b, Config.BackgroundColor.Value.a);
                PlayerInfo.SetBackgroundColor(c);
            }, Config.BackgroundColor.Value.r, 0, 255);
            Green = ColorCat.AddSlider("Green Value", "Shift Green Color Values", f => {
                var c = new Color32(Config.BackgroundColor.Value.r, (byte)f,
                    Config.BackgroundColor.Value.b, Config.BackgroundColor.Value.a);
                PlayerInfo.SetBackgroundColor(c);
            }, Config.BackgroundColor.Value.g, 0, 255);
            Blue = ColorCat.AddSlider("Blue Value", "Shift Blue Color Values", f => {
                var c = new Color32(Config.BackgroundColor.Value.r, Config.BackgroundColor.Value.g,
                    (byte)f, Config.BackgroundColor.Value.a);
                PlayerInfo.SetBackgroundColor(c);
            }, Config.BackgroundColor.Value.b, 0, 255);
            Alpha = ColorCat.AddSlider("Alpha Value", "Shift Opacity Values", f => {
                var c = new Color32(Config.BackgroundColor.Value.r, Config.BackgroundColor.Value.g,
                    Config.BackgroundColor.Value.b, (byte)f);
                PlayerInfo.SetBackgroundColor(c);
            }, Config.BackgroundColor.Value.a, 0, 255);
            TextSize = ColorCat.AddSlider("Text Size", "Change the text size of the player list", f => 
                PlayerInfo.UpdateTextSize((int)f), Config.TextSize.Value, 30, 50);
            
            WingLocation.Active = o;
            ExtendList.Active = o;
            Save.Active = o;
            ColorCat.Header.Active = o;
            Red.Active = o;
            Green.Active = o;
            Blue.Active = o;
            Alpha.Active = o;
            TextSize.Active = o;
        }
        
        #endregion

        #region Media Playback
#if DEBUG
/*
        public static ReMenuSlider SongSeeker;
        public static ReMenuCategory MediaApplications, SongPlaybackName;
        private static List<ReMenuButton> PlaybackApplications = new();
        private static ReMenuButton SingleApplication;
        private static void MediaControls() {
            MediaPlayback = BaseActions.AddCategoryPage("Media Playback", "Open Media Playback Menu", MintyResources.m_Menu);

            SongPlaybackName = MediaPlayback.AddCategory("Controls", false);
            
            ExtendedMediaPlayback.RunWMC();

            SongPlaybackName.AddButton("Previous", "Go back a song", Music.PrevTrack, MintyResources.m_Back);
            SongPlaybackName.AddButton("Stop", "Stop songs", Music.Stop, MintyResources.m_Stop);
            SongPlaybackName.AddButton("Play/Pause", "Pause or resume song", Music.PlayPause, MintyResources.m_Play);
            SongPlaybackName.AddButton("Next", "Go to next song", Music.NextTrack, MintyResources.m_Foward);

            MediaApplications = MediaPlayback.AddCategory("Media App");
            
            MediaPlayback.OnOpen += () => {
                MediaApplications.Header.Title = "Media App";
                foreach (var btn in PlaybackApplications)
                    btn.Destroy();
                PlaybackApplications.Clear();
                // TODO: foreach app in active_media_apps => create button
                int i = 0;
                
                foreach (var app in ExtendedMediaPlayback.Sources) {
                    i++;
                    if (i < 1) {
                        SingleApplication = MediaApplications.AddButton($"{(app.Contains("NewSource") ? "null" : app)}", "null", () => {
                            MediaApplications.Header.Title = $"Media App{(app.Contains("NewSource") ? "" : $" > {app}")}";
                        });
                    }
                    PlaybackApplications.Add(SingleApplication);
                }
            };
        }
*/
#endif
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

        internal override void OnLevelWasLoaded(int buildindex, string SceneName) {
            if (buildindex == -1) {
                PhotonFreeze.ToggleFreeze(false);
                ESP.ClearAllPlayerESP();
                InfJump?.Toggle(false, true, true);
                MainQMInfJump?.Toggle(false, true, true);
            }
        }

        internal override void OnUpdate() => PlayerActions.UpdateJump();

        internal override void OnPrefSave() {
            //DeviceType?.Toggle(Config.SpoofDeviceType.Value);
            FrameSpoof?.Toggle(Config.SpoofFramerate.Value);
            PingSpoof?.Toggle(Config.SpoofPing.Value);
            PingNegative?.Toggle(Config.SpoofedPingNegative.Value);
            bypassRiskyFunc?.Toggle(Config.bypassRiskyFunc.Value);
            PLEnabled?.Toggle(Config.PLEnabled.Value);
            if (Frame != null)
                Frame.Text = $"{Config.SpoofedFrameNumber.Value}";
            if (Ping != null)
                Ping.Text = $"<color={(Config.SpoofedPingNegative.Value ? "red>-" : "#00ff00>")}{Config.SpoofedPingNumber.Value}</color>";
            
            if (MainQMFly != null && MainQMNoClip != null) {
                MainQMFly.Active = Config.KeepFlightBTNsOnMainMenu.Value;
                MainQMNoClip.Active = Config.KeepFlightBTNsOnMainMenu.Value;
            }
            if (MainQMFreeze != null)
                MainQMFreeze.Active = Config.KeepPhotonFreezesOnMainMenu.Value;
            if (MainQMInfJump != null)
                MainQMInfJump.Active = Config.KeepInfJumpOnMainMenu.Value;
        }

        internal static void UpdateMintIconForStreamerMode(bool o) {
            if (MintIcon != null && !Config.useTabButtonForMenu.Value) {
                MintIcon.sprite = o ? MintyResources.Transparent : MintyResources.MintIcon;
                MintIcon.color = Color.white;
            }

            if (userSelectCategory != null) {
                userSelectCategory.RectTransform.gameObject.SetActive(!o);
                userSelectCategory.Active = !o;
                userSelectCategory.Title = o ? "" : "MintMod";
            }

            if (MintCategoryOnLaunchPad != null) {
                MintCategoryOnLaunchPad.RectTransform.gameObject.SetActive(!o);
                MintCategoryOnLaunchPad.Active = !o;
                MintCategoryOnLaunchPad.Title = o ? "" : "MintMod";
            }
            
            if (Config.SpoofFramerate.Value)
                Config.SavePrefValue(Config.mint, Config.SpoofFramerate, false);
            if (Config.SpoofPing.Value)
                Config.SavePrefValue(Config.mint, Config.SpoofPing, false);
        }
    }
}
