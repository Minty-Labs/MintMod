using System;
using System.Collections;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MelonLoader;
using MintMod.Functions;
using MintMod.Resources;
using ReMod.Core.UI.QuickMenu;
using ReMod.Core.VRChat;
using UnityEngine;
using UnityEngine.UI;
using VRC;
using VRC.Core;
using VRC.UI.Core;
using VRC.SDKBase;
using VRC.UI;
using VRC.UI.Core.Styles;
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
using ReMod.Core.UI;
using TMPro;
using UnityEngine.Playables;
using VRC.DataModel;
using Object = UnityEngine.Object;

namespace MintMod.UserInterface.QuickMenu {
    internal class MintUserInterface : MintSubMod {
        public override string Name => "Mint UI";
        public override string Description => "Builds all of the Mod's menu.";

        private static GameObject MainMenuBackButton, TheMintMenuButton;

        private static ReMenuCategory MintCategoryOnLaunchPad, BaseActions, /*MintQuickActionsCat,*/ userSelectCategory, playerListCategory;

        public static ReCategoryPage MintMenu, PlayerMenu, WorldMenu, RandomMenu, PlayerListMenu, PlayerListConfig, /*AvatarMenu*/ NameplateMenu, WorldActionsPage;

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
            while (UIManager.field_Private_Static_UIManager_0 == null) yield return null;
            while (GameObject.Find("UserInterface").GetComponentInChildren<VRC.UI.Elements.QuickMenu>(true) == null) yield return null;

            yield return BuildStandard();
            yield return BuildMint();
        }

        internal static IEnumerator OnSettingsPageInit() {
            while (GameObject.Find("UserInterface/MenuContent/Screens/Settings/ComfortSafetyPanel/StreamerModeToggle") == null)
                yield return null;
            
            var toggle = GameObject.Find("UserInterface/MenuContent/Screens/Settings/ComfortSafetyPanel/StreamerModeToggle").GetComponent<UiSettingConfig>();
            isStreamerModeOn = toggle.Method_Private_Boolean_0();

            yield return new WaitForSeconds(15);
            UpdateMintIconForStreamerMode(isStreamerModeOn);
        }

        private static IEnumerator BuildStandard() {
            var f = QuickMenuEx.Instance.field_Public_Transform_0.Find("Window/QMParent/Menu_Dashboard");
            MainMenuBackButton = f.Find("Header_H1/LeftItemContainer/Button_Back").gameObject;
            try {
                if (MelonHandler.Mods.Any(i => i.Info.Name != "AdBlocker")) {
                    f.Find("ScrollRect/Viewport/VerticalLayoutGroup/Carousel_Banners").gameObject.SetActive(false);
                    f.Find("ScrollRect/Viewport/VerticalLayoutGroup/VRC+_Banners").gameObject.SetActive(false);
                }
            }
            catch {
                Con.Error("Action from AdBlocker failed. Ignoring");
            }
            
            var launchPad = new ReCategoryPage(f);
            MintCategoryOnLaunchPad = launchPad.AddCategory(MintCore.Fool ? "Walmart Client" : "MintMod");
            MintCategoryOnLaunchPad!.Active = false;
            
            if (Config.KeepFlightBTNsOnMainMenu.Value || Config.KeepPhotonFreezesOnMainMenu.Value || Config.KeepInfJumpOnMainMenu.Value) 
                MintCategoryOnLaunchPad!.Active = true;

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
            MintMenu = new ReCategoryPage(MintCore.Fool ? "WalmartMenu" : "MintMenu", Config.useTabButtonForMenu.Value);
            MintMenu.GameObject.SetActive(false);

            if (Config.useTabButtonForMenu.Value)
                ReTabButton.Create("MintTab", "Open the MintMenu", MintCore.Fool ? "WalmartMenu" : "MintMenu", 
                    MintCore.Fool ? MintyResources.WalmartTab : MintyResources.MintTabIcon);
            else {
                TheMintMenuButton = Object.Instantiate(MainMenuBackButton, MainMenuBackButton.transform.parent);
                TheMintMenuButton.transform.SetAsLastSibling();
                TheMintMenuButton.transform.Find("Badge_UnfinishedFeature").gameObject.SetActive(false);
                yield return SetTheFuckingSprite();
                TheMintMenuButton.SetActive(true);
                TheMintMenuButton.name = "MintMenuButtonOvertakenBackButton";
                TheMintMenuButton.GetComponent<Button>().onClick.RemoveAllListeners();
                TheMintMenuButton.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
                TheMintMenuButton.GetComponent<Button>().onClick.AddListener(new Action(MintMenu.Open));
            }

            BaseActions = MintMenu.AddCategory("Menus", false);

            RemoveMintBackButtonDuplicate(MintMenu);

            MintQuickActions();
            Player();
            try {
                World();
            }
            catch (Exception e) {
                if (e.ToString().Contains("AddSpacer"))
                    Con.Error("Please remove the ReMod.Core.dll file from the root of your VRChat game directory. Then, restart the game to fix this error.");
            }
            RandomStuff();
            PlayerListMenuSetup();
            PlayerListOptions();
            //BuildAvatarMenu();
            BuildNameplateMenu();
            BuildWorldActionsMenu();
            
            // Build Last
            UserSelMenu();

            if (Config.CopyReModMedia.Value)
                yield return CreateMediaDebugPanel();
            _mediaReady = true;

            Con.Debug("Done Setting up MintMenus", MintCore.IsDebug);
        }

        private static IEnumerator SetTheFuckingSprite() {
            yield return null;
            Object.DestroyImmediate(TheMintMenuButton.transform.Find("Icon").GetComponent<StyleElement>());
            MintIcon = TheMintMenuButton.transform.Find("Icon").GetComponent<Image>();
            MintIcon.sprite = MintCore.Fool ? MintyResources.WalmartTab : MintyResources.MintIcon;
            MintIcon.color = Color.white;
            var styleElement = TheMintMenuButton.GetComponent<StyleElement>();
            if (styleElement.field_Public_String_0 == "Back") // Ignore Style Changes
                styleElement.field_Public_String_0 = "MintMenuButton";
            else styleElement.field_Public_String_1 = "MintMenuButton";
        }

        #region Quick Actions

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
                PlayerActions.InfinteJump = b;
                MainQMInfJump.Toggle(b, false, true);
            });
            
            var sc = MintMenu.AddSliderCategory("Flight Speed");
            FlightSpeedSlider = sc.AddSlider("Min: 0.5", "Control Flight Speed", f => Movement.finalSpeed = f,
                1f, 0.5f, 5f);
            
            Con.Debug("Done Creating QuickActions", MintCore.IsDebug);
        }

        #endregion

        #region Player Menu

        private static ReMenuToggle InfJump;
        
        private static void Player() {
            PlayerMenu = BaseActions.AddCategoryPage("Player", "Actions involving players.", MintyResources.people);
            var c = PlayerMenu.AddCategory("General Actions", false);
            PlayerESP = c.AddToggle("Player ESP", "Puts a bubble around each player, and is visible through walls.", ESP.PlayerESPState);
            c.AddButton("Copy Current Avi ID", "Copies your current Avatar ID into your clipboard", () => {
                try {
                    Clipboard.SetText(APIUser.CurrentUser.avatarId);
                }
                catch (Exception c) {
                    Con.Error(c);
                }
            }, MintyResources.clipboard);
            c.AddButton("Go into Avi by ID", "Takes an Avatar ID from your clipboard and changes into that avatar.", () => {
                try {
                    string clip;
                    try { clip = GUIUtility.systemCopyBuffer; }
                    catch { clip = Clipboard.GetText(); }

                    if (clip.Contains("avtr_") && !string.IsNullOrWhiteSpace(clip)) {
                        try {
                            PageAvatar a = new() { field_Public_SimpleAvatarPedestal_0 = new() };
                            new ApiAvatar { id = clip }.Get(new Action<ApiContainer>(x => {
                                a.field_Public_SimpleAvatarPedestal_0.field_Internal_ApiAvatar_0 = x.Model.Cast<ApiAvatar>();
                                a.ChangeToSelectedAvatar();
                            }));
                        }
                        catch {
                            VRCPlayer.field_Internal_Static_VRCPlayer_0.ChangeToAvatar(clip);
                        }
                    }
                    else {
                        VRCUiPopups.Notify("No Avatar ID in clipboard", NotificationSystem.Alert);
                    }
                }
                catch (Exception c) {
                    Con.Error(c);
                }
            }, MintyResources.checkered);
            c.AddButton("Download Own VRCA", "Downloads the VRCA of the avatar that you're in", PlayerActions.AvatarSelfDownload, MintyResources.user);
            //var h = PlayerMenu.AddCategory("Head Lamp");
            RemoveMintBackButtonDuplicate(PlayerMenu);
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
            w.AddButton("Download VRCW", "Downloads the world file (.vrcw)", WorldActions.WorldDownload, MintyResources.dl);
            w.AddSpacer();

            w.AddButton("Copy Instance ID URL", "Copies current instance ID and places it in your system's clipboard.", () => {
                    var id = RoomManager.field_Internal_Static_ApiWorld_0.id;
                    var instance = RoomManager.field_Internal_Static_ApiWorldInstance_0.instanceId;
                    var faulted = false;
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
                    string clip;
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
            w.AddButton("Reset Portal", $"Sets portal timers to {Config.ResetTimerAmount.Value}", () => {
                if (Object.FindObjectsOfType<PortalInternal>() == null) return;
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
                            new[] { @object });
                }*/
            }, MintyResources.history);
            w.AddButton("Log World", "Logs world info (of various data points) in a text file.", WorldActions.LogWorld, MintyResources.list);
            
            w.AddButton("Normal World Mirrors", "Reverts mirrors to their original state", WorldActions.RevertMirrors);
            w.AddButton("Optimize Mirrors", "Make Mirrors only show players", WorldActions.OptimizeMirrors);
            w.AddButton("Beautify Mirrors", "Make Mirrors show everything", WorldActions.BeautifyMirrors);

            var e = WorldMenu.AddCategory("Item Manipulation");
            e.AddButton("Teleport Items to Self", "Teleports all Pickups to your feet.", Items.TPToSelf);
            e.AddButton("Respawn Items", "Respawns All pickups to their original location.", Items.Respawn);
            e.AddButton("Teleport Items Out of World", "Teleports all Pickups an XYZ coord of 1 million", Items.TPToOutWorld);
            
            var ct = WorldMenu.AddCategory("Component Toggle");
            Components.ComponentToggle(ct);
            RemoveMintBackButtonDuplicate(WorldMenu);
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
                "Reloads Mint's custom nameplate addons in case more were added while you're playing", () => {
                    Players.FetchCustomPlayerObjects(true);
                    VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
                }, MintyResources.extlink);

            r.AddButton("Clear HUD Message Queue", "Clears the HUD Popup Message Queue", () => {
                if (!Config.UseOldHudMessages.Value)
                    NotificationController_Mint.Instance.ClearNotifications();
                else
                    VRCUiManager.prop_VRCUiManager_0.field_Private_List_1_String_0.Clear();
            }, MintyResources.messages);
            RemoveMintBackButtonDuplicate(RandomMenu);
        }

        #endregion

        #region UserSelect Menu
        
        private static void UserSelMenu() {
            var f = QuickMenuEx.Instance.field_Public_Transform_0.Find("Window/QMParent/Menu_SelectedUser_Local");
            var theUserSelectMenu = f.Find("ScrollRect/Viewport/VerticalLayoutGroup").gameObject;

            userSelectCategory = new ReMenuCategory("MintMod", theUserSelectMenu.transform);

            userSelectCategory.AddButton("Download Avatar VRCA", "Downloads the selected user's Avatar .VRCA", PlayerActions.AvatarDownload, MintyResources.dl);
            userSelectCategory.AddButton("Log Asset", "Logs the selected user's information and put it into a text file", PlayerActions.LogAsset, MintyResources.list);
            userSelectCategory.AddButton("Copy Avatar ID", "Copies the selected user's avatar ID into your clipboard", () => GUIUtility.systemCopyBuffer = PlayerActions.SelPAvatar().id, MintyResources.copy);
            userSelectCategory.AddButton("Copy User ID", "Copies the selected user's User ID into your clipboard", () => GUIUtility.systemCopyBuffer = PlayerWrappers.GetSelectedAPIUser().id, MintyResources.copy);
            userSelectCategory.AddButton("Clone Avatar", "Clones the selected user's avatar if public", () => {
                var apiAvatar = PlayerActions.SelPAvatar();
                var avatarIsPublic = apiAvatar.releaseStatus.ToLower().Contains("public");
                if (!avatarIsPublic) {
                    VRCUiPopups.Notify("Avatar is private", NotificationSystem.Alert);
                    return;
                }
                try {
                    PlayerManager.field_Private_Static_PlayerManager_0.field_Private_Player_0._vrcplayer.ChangeToAvatar(apiAvatar.id);
                }
                catch (Exception fail) {
                    Con.Error(fail);
                    Con.Warn("Attempting fallback method");
                    try {
                        PageAvatar a = new() { field_Public_SimpleAvatarPedestal_0 = new() };
                        new ApiAvatar { id = apiAvatar.id }.Get(new Action<ApiContainer>(x => {
                            a.field_Public_SimpleAvatarPedestal_0.field_Internal_ApiAvatar_0 = x.Model.Cast<ApiAvatar>();
                            a.ChangeToSelectedAvatar();
                        }));
                    }
                    catch (Exception e) { Con.Error(e); }
                }
            }, MintyResources.clone);

            if (Config.AviFavsEnabled.Value) {
                userSelectCategory.AddButton("Silent Favorite", "Silently favorites the avatar the selected user is wearing if public.", () => {
                    var v = PlayerActions.SelPAvatar();
                    if (!v.releaseStatus.ToLower().Contains("private")) {
                        ReFavs._instance.FavoriteAvatar(v);
                        VRCUiPopups.Notify($"Favorited the avatar: {v.name}", NotificationSystem.Alert);
                    }
                    else {
                        Con.Warn("Avatar is private, cannot favorite");
                        VRCUiPopups.Notify("Avatar is private, cannot favorite", NotificationSystem.Alert);
                    }
                    /*foreach (var favoriteList in AviFavSetup.Favorites.Instance.AvatarFavorites.FavoriteLists) {
                        if (!v.releaseStatus.ToLower().Contains("private")) {
                            AviFavLogic.FavoriteAvatar(v, favoriteList.ID);
                            VRCUiPopups.Notify($"Favorited the avatar: {v.name}", NotificationSystem.Alert);
                        }
                        else {
                            Con.Warn("Avatar is private, cannot favorite");
                            VRCUiPopups.Notify("Avatar is private, cannot favorite", NotificationSystem.Alert);
                        }
                    }*/
                }, MintyResources.star);
            }

            userSelectCategory.AddButton("Teleport to", "Teleport to the selected user", () => { PlayerActions.Teleport(PlayerWrappers.SelVrcPlayer()); }, MintyResources.marker_hole);
            userSelectCategory.AddButton("Teleport pickups to", "Teleport all pickup objects to the selected user", () => Items.TPToPlayer(PlayerWrappers.SelVrcPlayer()._player), MintyResources.marker);

            if (!ModCompatibility.GPrivateServer) {
                if (APIUser.CurrentUser.id.StartsWith("usr_6d71d3be")) {
                    userSelectCategory.AddButton("Mint Auth Check", "Check to see if the selected user can use MintMod", () => MelonCoroutines.Start(ServerAuth.SimpleAuthCheck(PlayerWrappers.GetSelectedAPIUser().id)), MintyResources.key);
                }
            }

            Con.Debug("Done Setting up User Selected Menu", MintCore.IsDebug);
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
            /*
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
            */
            p.AddSpacer();
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
            //_1.Interactable = false;
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
                    
                    var tempName = string.Empty;
                    if (PlayerWrappers.IsFriend(player)) {
                        if (player.field_Private_APIUser_0.id.StartsWith("usr_6d71d3be"))
                            tempName = "<color=#9fffe3>Lily</color>";
                        else
                            tempName = $"<color=#{Config.FriendRankHEX.Value}>{n}</color>";
                    }
                    else {
                        if (player.field_Private_APIUser_0.id.StartsWith("usr_6d71d3be"))
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
            RemoveMintBackButtonDuplicate(PlayerListMenu);
        }

        #endregion
        
        #region Player List

        private static ReMenuToggle _plEnabled, _wingLocation, _extendList, _roomTimer, _gameTimer, _systemTime, _system24Hour, _showPing, _showFrames, _showPlatform, _showAviPerf;
        private static ReMenuCategory _temp;
        private static ReMenuButton _save, _setHexValue;
        private static ReMenuSliderCategory _colorCat;
        private static ReMenuSlider _red, _green, _blue, _alpha, _textSize;
        private static void PlayerListOptions() {
            PlayerListConfig = BaseActions.AddCategoryPage("Player List Config", "Control the player list's options", MintyResources.userlist);
            var c = PlayerListConfig.AddCategory("Player List Config", false);
            var o = Config.PLEnabled.Value;
            
            _plEnabled = c.AddToggle("Enabled", "Toggle the PLayer List", b => {
                Config.SavePrefValue(Config.PlayerList, Config.PLEnabled, b);
                var tempToggle = Config.PLEnabled.Value;
                _save.Active = tempToggle;
                _colorCat.Header.Active = tempToggle;
                _red.Active = tempToggle;
                _green.Active = tempToggle;
                _blue.Active = tempToggle;
                _alpha.Active = tempToggle;
                _wingLocation.Active = tempToggle;
                _extendList.Active = tempToggle;
                _textSize.Active = tempToggle;
                _roomTimer.Active = tempToggle;
                _gameTimer.Active = tempToggle;
                _systemTime.Active = tempToggle;
                _system24Hour.Active = tempToggle;
                _showFrames.Active = tempToggle;
                _showPing.Active = tempToggle;
                _showPlatform.Active = tempToggle;
                _showAviPerf.Active = tempToggle;
                _setHexValue.Active = tempToggle;
            }, Config.PLEnabled.Value);
            _wingLocation = c.AddToggle("List on Right Side", "Move the list on the left or right wing", b => 
                Config.SavePrefValue(Config.PlayerList, Config.Location, b ? 1 : 0), Config.Location.Value != 0);
            _extendList = c.AddToggle("Extend Player Listing", "Show all players regardless on length of box", b => {
                Config.SavePrefValue(Config.PlayerList, Config.uncapListCount, b);
                PlayerInfo.MoveTheText();
            }, Config.uncapListCount.Value);
            _save = c.AddButton("Save Values", "Save the color options below", () => {
                var color = (Color32)PlayerInfo.BackgroundImage.color;
                Config.SavePrefValue(Config.PlayerList, Config.BackgroundColor, color);
                Config.SavePrefValue(Config.PlayerList, Config.TextSize, PlayerInfo.GetTextSize());
            }, MintyResources.cog);
            _roomTimer = c.AddToggle("Room Timer", "Adds a Room Timer to the player list", b => 
                Config.SavePrefValue(Config.PlayerList, Config.haveRoomTimer, b), Config.haveRoomTimer.Value);
            _gameTimer = c.AddToggle("Game Timer", "Adds a Game Timer to the player list (Shows how long you've been currently playing VRChat)", b => 
                Config.SavePrefValue(Config.PlayerList, Config.haveGameTimer, b), Config.haveGameTimer.Value);
            _systemTime = c.AddToggle("System Time", "Adds your System Time to the player list", b => 
                Config.SavePrefValue(Config.PlayerList, Config.haveSystemTime, b), Config.haveSystemTime.Value);
            _system24Hour = c.AddToggle("24 Hour Format", "Changes the System time to the 24 hour format", b => 
                Config.SavePrefValue(Config.PlayerList, Config.system24Hour, b), Config.system24Hour.Value);
            _showPing = c.AddToggle("Player Ping", "Toggles ping shown on the player list", b =>
                Config.SavePrefValue(Config.PlayerList, Config.showPlayerPing, b), Config.showPlayerPing.Value);
            _showFrames = c.AddToggle("Player Frames", "Toggles frame rate shown on the player list", b =>
                Config.SavePrefValue(Config.PlayerList, Config.showPlayerFrames, b), Config.showPlayerFrames.Value);
            _showPlatform = c.AddToggle("Player Platform", "Toggles platform shown on the player list", b =>
                Config.SavePrefValue(Config.PlayerList, Config.showPlayerPlatform, b), Config.showPlayerPlatform.Value);
            _showAviPerf = c.AddToggle("Player Performance", "Toggles avatar performance shown on the player list", b =>
                Config.SavePrefValue(Config.PlayerList, Config.showPlayerAviPerf, b), Config.showPlayerAviPerf.Value);
            
            _colorCat = PlayerListConfig.AddSliderCategory($"Background <color=#{ColorUtility.ToHtmlStringRGB(Config.BackgroundColor.Value)}>Color</color>");
            _red = _colorCat.AddSlider("Red Value", "Shift Red Color Values", f => {
                var c = new Color(f/255, Config.BackgroundColor.Value.g,
                    Config.BackgroundColor.Value.b, Config.BackgroundColor.Value.a);
                PlayerInfo.SetBackgroundColor(c);
                _colorCat.Title = $"Background <color=#{ColorUtility.ToHtmlStringRGB(c)}>Color</color>";
            }, Config.BackgroundColor.Value.r*255, 0, 255);
            _green = _colorCat.AddSlider("Green Value", "Shift Green Color Values", f => {
                var c = new Color(Config.BackgroundColor.Value.r, f/255,
                    Config.BackgroundColor.Value.b, Config.BackgroundColor.Value.a);
                PlayerInfo.SetBackgroundColor(c);
                _colorCat.Title = $"Background <color=#{ColorUtility.ToHtmlStringRGB(c)}>Color</color>";
            }, Config.BackgroundColor.Value.g*255, 0, 255);
            _blue = _colorCat.AddSlider("Blue Value", "Shift Blue Color Values", f => {
                var c = new Color(Config.BackgroundColor.Value.r, Config.BackgroundColor.Value.g,
                    f/255, Config.BackgroundColor.Value.a);
                PlayerInfo.SetBackgroundColor(c);
                _colorCat.Title = $"Background <color=#{ColorUtility.ToHtmlStringRGB(c)}>Color</color>";
            }, Config.BackgroundColor.Value.b*255, 0, 255);
            _alpha = _colorCat.AddSlider("Alpha Value", "Shift Opacity Values", f => {
                var c = new Color(Config.BackgroundColor.Value.r, Config.BackgroundColor.Value.g,
                    Config.BackgroundColor.Value.b, f/255);
                PlayerInfo.SetBackgroundColor(c);
            }, Config.BackgroundColor.Value.a*255, 0, 255);
            _textSize = _colorCat.AddSlider("Text Size", "Change the text size of the player list", f => 
                PlayerInfo.UpdateTextSize((int)f), Config.TextSize.Value, 30, 50);

            _temp = PlayerListConfig.AddCategory("Hidden");
            _temp.Header.GameObject.SetActive(false);
            _setHexValue = _temp.AddButton($"Enter <color=#{ColorUtility.ToHtmlStringRGB(Config.BackgroundColor.Value)}>HEX</color>",
                "Enter a HEX value you know for the Player List Background Color", () => {
                    VRCUiPopupManager.field_Private_Static_VRCUiPopupManager_0.ShowInputPopupWithCancel("Enter HEX Value", "", 
                        InputField.InputType.Standard, false, "Continue", (s, Elly_Is, Mega_Cute) => {
                            _colorCat.Title = $"Background <color=#{s.Replace("#", "")}>Color</color>";
                            var color = ColorConversion.HexToColor(s.Replace("#", ""));
                            //Con.Debug($"Color: {s}   {c.r} {c.g} {c.b}", MintCore.isDebug);
                            Config.SavePrefValue(Config.PlayerList, Config.BackgroundColor, new Color(color.r, color.g, color.b, Config.BackgroundColor.Value.a));
                            PlayerInfo.SetBackgroundColor(Config.BackgroundColor.Value);
                            _red.Slide(color.r * 255, false);        // \/                        \/                            \/
                            _green.Slide(color.g * 255, false);      // False Because Unity is Throwing Errors with onValueChanged
                            _blue.Slide(color.b * 255, false);       // /\                        /\                            /\
                            _setHexValue.Text = $"Enter <color=#{ColorUtility.ToHtmlStringRGB(Config.BackgroundColor.Value)}>HEX</color>";
                        }, () => VRCUiPopupManager.field_Private_Static_VRCUiPopupManager_0.HideCurrentPopup());
                }, MintyResources.ColorPicker);
            
            _wingLocation.Active = o;
            _extendList.Active = o;
            _save.Active = o;
            _roomTimer.Active = o;
            _gameTimer.Active = o;
            _systemTime.Active = o;
            _system24Hour.Active = o;
            _showFrames.Active = o;
            _showPing.Active = o;
            _showPlatform.Active = o;
            _showAviPerf.Active = o;
            
            _colorCat.Header.Active = o;
            _red.Active = o;
            _green.Active = o;
            _blue.Active = o;
            _alpha.Active = o;
            _textSize.Active = o;
            _setHexValue.Active = o;
            
            RemoveMintBackButtonDuplicate(PlayerListConfig);
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

        #region Nameplate Menu

        private static ReMenuToggle _mintNameplates, _mintTags;

        private static void BuildNameplateMenu() {
            NameplateMenu = BaseActions.AddCategoryPage("Nameplate Settings","Opens Mint Nameplate Settings Menu", MintyResources.user_nameplate);

            var n = NameplateMenu.AddCategory("Nameplate Settings");
            _mintNameplates = n.AddToggle("Nameplates Changes", "Toggles all Nameplate modifications done by Mint", b => {
                Config.SavePrefValue(Config.Nameplates, Config.EnableCustomNameplateReColoring, b);
                VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
            }, Config.EnableCustomNameplateReColoring.Value);
            
            _mintTags = n.AddToggle("Mint Tags", "Toggles all Nameplate modifications done by Mint", b => {
                Config.SavePrefValue(Config.Nameplates, Config.EnabledMintTags, b);
                VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
            }, Config.EnabledMintTags.Value);
            
            n.AddButton("Tag Location", "Input a number of vertical tag placement", () => {
                
                VRCUiPopupManager.field_Private_Static_VRCUiPopupManager_0.ShowInputPopupWithCancel("Vertical Tag Location",
                    $"{Config.MintTagVerticleLocation.Value}", InputField.InputType.Standard, false, "Submit",
                    (_, EllyIs, MegaAdorable) => {
                        
                        float.TryParse(_, out var final);
                        Config.SavePrefValue(Config.Nameplates, Config.MintTagVerticleLocation, final);
                        VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
                        
                    }, VRCUiPopupManager.field_Private_Static_VRCUiPopupManager_0.HideCurrentPopup, "-60.0");
                
            }, MintyResources.user_nameplate_tag_move);
        }

        #endregion

        #region World Actions
        
        private static ReMenuToggle _worldToggle;

        private static void BuildWorldActionsMenu() {
            WorldActionsPage = BaseActions.AddCategoryPage("World Actions", "Actions involving specific worlds.", MintyResources.globe);
            var actionCategory = WorldActionsPage.AddCategory("Per-world actions");
            
            WorldSettings.BlackCat.BuildMenu(actionCategory);
            
            var mintActions = WorldActionsPage.AddCategory("Mint Actions");

            _worldToggle = mintActions.AddToggle("Mint World Toggles", "Toggle Mint specific objects in the world, if there are any.", b => 
                GameObject.Find("_Mint_SetON")!.SetActive(b));
        }

        #endregion

        #region Media Header

        private static Transform _mediaPanel;
        private static bool _loaded, _mediaReady;
        private static TextMeshProUGUI _reModTextElement, _mediaPanelText;
        private static string _reModHeaderText;
        private static RectTransform _mediaRectTransform;
        
        private static IEnumerator CreateMediaDebugPanel() {
            while (UIManager.field_Private_Static_UIManager_0 == null) yield return null;
            while (Object.FindObjectOfType<VRC.UI.Elements.QuickMenu>() == null) yield return null;
            yield return new WaitForSeconds(0.25f);
            
            var t = GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMNotificationsArea/DebugInfoPanel/").transform;
            var f = QuickMenuEx.Instance.field_Public_Transform_0.Find("Window/QMParent/Menu_Dashboard");
            _mediaPanel = Object.Instantiate(t.Find("Panel"), t, false);
            _mediaPanel.gameObject.name = "MediaPanel";
            Transform h;
            try {
                h = f.Find("ScrollRect/Viewport/VerticalLayoutGroup/Header_MediaControls/LeftItemContainer/Text_Title");
            }
            catch {
                yield break; // Stop method if failed
            }

            var hasNoBtk = false;

            _reModTextElement = h.GetComponent<TextMeshProUGUI>();
            _reModHeaderText = _reModTextElement.text;
            
            _mediaPanel.Find("Text_Ping").gameObject.Destroy();
            try {
                _mediaPanel.Find("BTKStatusElement").gameObject!.Destroy();
                _mediaPanel.Find("BTKClockElement").gameObject!.Destroy();
            }
            catch {
                Con.Debug("Did not find BTK Elements to destroy!");
                hasNoBtk = true;
            }
            
            _mediaPanelText = _mediaPanel.Find("Text_FPS").GetComponent<TextMeshProUGUI>();
            _mediaPanelText.text = "";
            _mediaRectTransform = _mediaPanel.GetComponent<RectTransform>();
            _mediaRectTransform.localPosition = new Vector3(-512, 85, 0);

            if (hasNoBtk) {
                // Setup HorizontalLayoutGroup - https://github.com/ddakebono/QMClock/blob/main/QMClock/QMClock.cs#L134
                var horizLayout = _mediaPanel.gameObject.GetOrAddComponent<HorizontalLayoutGroup>();
                horizLayout.padding.left = 20;
                horizLayout.padding.right = 20;
                horizLayout.spacing = 1.5f;
                
                // Expand box size - https://github.com/ddakebono/QMClock/blob/main/QMClock/QMClock.cs#L154
                var mediaPanelRect = _mediaPanel.GetComponent<RectTransform>();
                var adjust = mediaPanelRect.sizeDelta;
                adjust.x = 150f * _mediaPanel.transform.childCount;
                mediaPanelRect.sizeDelta = adjust;
                
                _mediaPanel.Find("Background").gameObject.GetComponent<Image>().enabled = false;
                _mediaPanel.Find("Background").gameObject.SetActive(false);
            }
            
            _loaded = true;
            
            _mediaPanel.gameObject.SetActive(Config.CopyReModMedia.Value); // Toggle
            yield return LoopTextChange(Config.RefreshAmount.Value);
        }

        private static IEnumerator LoopTextChange(float v) {
            while (_loaded && Config.CopyReModMedia.Value) {
                yield return new WaitForSeconds(v);
                _mediaPanel.gameObject.SetActive(Config.CopyReModMedia.Value);
                if (!Config.CopyReModMedia.Value) {
                    _mediaPanel.gameObject.DestroyImmediate();
                    break;
                }
                _reModHeaderText = _reModTextElement.text;
                _mediaPanelText.text = _reModHeaderText;
                _mediaRectTransform.localPosition = new Vector3(-512, 85, 0);
            }
        }

        #endregion

        internal override void OnLevelWasLoaded(int buildindex, string sceneName) {
            if (buildindex != -1) return;
            PhotonFreeze.ToggleFreeze(false);
            ESP.ClearAllPlayerESP();
            InfJump?.Toggle(false, true, true);
            MainQMInfJump?.Toggle(false, true, true);
        }

        internal override void OnUpdate() {
            PlayerActions.UpdateJump();
            if (!MintCore.IsDebug) return;
            if (Input.GetKeyDown(KeyCode.Home)) UserSelMenu();
        }

        internal override void OnPrefSave() {
            //DeviceType?.Toggle(Config.SpoofDeviceType.Value);
            FrameSpoof?.Toggle(Config.SpoofFramerate.Value);
            PingSpoof?.Toggle(Config.SpoofPing.Value);
            PingNegative?.Toggle(Config.SpoofedPingNegative.Value);
            bypassRiskyFunc?.Toggle(Config.bypassRiskyFunc.Value);
            _plEnabled?.Toggle(Config.PLEnabled.Value);
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
            
            switch (_loaded) {
                case false when _mediaReady && Config.CopyReModMedia.Value:
                    MelonCoroutines.Start(CreateMediaDebugPanel());
                    break;
                case true when !Config.CopyReModMedia.Value: {
                    if (_mediaPanel != null)
                        _mediaPanel.gameObject.SetActive(Config.CopyReModMedia.Value);
                    break;
                }
            }
            _mintNameplates?.Toggle(Config.EnableCustomNameplateReColoring.Value);
            _mintTags?.Toggle(Config.EnabledMintTags.Value);
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

            if (Config.useTabButtonForMenu.Value && o) {
                var msg = "Streamer Mode detected, Mint Tab Button is still visible.";
                Con.Warn(msg);
                VRCUiManager.field_Private_Static_VRCUiManager_0.QueueHudMessage(msg, Color.white, 8f);
            }
        }

        private static void RemoveMintBackButtonDuplicate(UiElement cat) {
            if (Config.useTabButtonForMenu.Value) return;
            try {
                var _ = cat.GameObject.transform.Find("LeftItemContainer/MintMenuButtonOvertakenBackButton")?.gameObject;
                _.DestroyImmediate();
            }
            catch (Exception e) {
                Con.Error(e);
            }
        }
    }
}
