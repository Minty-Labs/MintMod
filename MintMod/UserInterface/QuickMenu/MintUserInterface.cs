using System;
using System.Collections;
using System.Windows.Forms;
//using Il2CppSystem;
using System.Collections.Generic;
using System.Linq;
using MelonLoader;
using MintMod.Functions;
using MintMod.Resources;
using ReMod.Core.UI.QuickMenu;
using UnityEngine;
using UnityEngine.UI;
using VRC;
using VRC.Core;
using VRC.SDKBase;
using VRC.UI;
using VRC.UI.Core.Styles;
using Button = UnityEngine.UI.Button;
using MintMod.Managers;
using MintMod.Reflections;
using MintMod.Utils;
using MintyLoader;
using ReMod.Core.VRChat;
using MintMod.Functions.Authentication;
using MintMod.Libraries;

namespace MintMod.UserInterface.QuickMenu {
    class MintUserInterface : MintSubMod {
        public override string Name => "Mint UI";
        public override string Description => "Builds all of the Mod's menu.";

        private static GameObject MainMenuBackButton, TheMintMenuButton, ShittyAdverts, ShittyAdverts_2, LaunchPadLayoutGroup;

        private static ReMenuCategory MintCategoryOnLaunchPad, BaseActions, MintQuickActionsCat, userSelectCategory, playerListCategory;

        public static ReCategoryPage MintMenu, PlayerMenu, WorldMenu, RandomMenu, PlayerListMenu;

        private static QMSlider FlightSpeedSlider; 

        public static ReMenuToggle 
            MainQMFly, MainQMNoClip, MainQMFreeze,
            MintQAFly, MintQANoClip, MintQAFreeze;

        //private static Sprite WorldIcon, PlayerIcon;

        public static ReMenuToggle ItemESP, PlayerESP, DeviceType, FrameSpoof, PingSpoof, PingNegative, bypassRiskyFunc;
        public static ReMenuButton Frame, Ping;

        internal static Image MintIcon;

        internal static bool isOnStreamerMode;

		internal static IEnumerator OnQuickMenu() {
            while (UnityEngine.Object.FindObjectOfType<VRC.UI.Elements.QuickMenu>() == null)
                yield return null;
            MelonCoroutines.Start(BuildMint());
        }

        internal static IEnumerator OnUserSelectMenu() {
            while (UnityEngine.Object.FindObjectOfType<VRC.UI.Elements.Menus.SelectedUserMenuQM>() == null)
                yield return null;
            if (!isOnStreamerMode)
                UserSelMenu();
        }

        internal static IEnumerator OnSettingsPageInit() {
            while (GameObject.Find("UserInterface/MenuContent/Screens/Settings/ComfortSafetyPanel/StreamerModeToggle") == null)
                yield return null;
            
            var toggle = GameObject.Find("UserInterface/MenuContent/Screens/Settings/ComfortSafetyPanel/StreamerModeToggle").GetComponent<UiSettingConfig>();
            isOnStreamerMode = toggle.Method_Private_Boolean_0();

            yield return new WaitForSeconds(15);
            UpdateMintIconForStreamerMode(isOnStreamerMode);
        }

        static IEnumerator BuildMint() {
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

            MintMenu = new ReCategoryPage($"MintMenu - v<color=#9fffe3>{MintCore.ModBuildInfo.Version}</color>");
            MintMenu.GameObject.SetActive(false);
            TheMintMenuButton = UnityEngine.Object.Instantiate(MainMenuBackButton, MainMenuBackButton.transform.parent);
            TheMintMenuButton.transform.SetAsLastSibling();
            TheMintMenuButton.transform.Find("Badge_UnfinishedFeature").gameObject.SetActive(false);
            MelonCoroutines.Start(SetTheFuckingSprite());
            TheMintMenuButton.SetActive(true);
            TheMintMenuButton.name = "MintMenuButtonOvertakenBackButton";
            TheMintMenuButton.GetComponent<Button>().onClick.RemoveAllListeners();
            TheMintMenuButton.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
            TheMintMenuButton.GetComponent<Button>().onClick.AddListener(new Action(MintMenu.Open));

            BaseActions = MintMenu.AddCategory("Menus");

            MintQuickActions();
            Player();
            World();
            RandomStuff();
            PlayerListMenuSetup();

            LaunchPadLayoutGroup = GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup");
            MintCategoryOnLaunchPad = new("MintMod", LaunchPadLayoutGroup.transform);
            MintCategoryOnLaunchPad.Active = false;

            if (Config.KeepFlightBTNsOnMainMenu.Value || Config.KeepPhotonFreezesOnMainMenu.Value)
                MintCategoryOnLaunchPad.Active = true;
            if (LaunchPadLayoutGroup != null) {
                if (Config.KeepFlightBTNsOnMainMenu.Value) {
                    MainQMFly = MintCategoryOnLaunchPad.AddToggle("Flight", "Toggle Flight", Movement.Fly);
                    MainQMNoClip = MintCategoryOnLaunchPad.AddToggle("No Clip", "Toggle No Clip", Movement.NoClip);
                }
                if (Config.KeepPhotonFreezesOnMainMenu.Value)
                    MainQMFreeze = MintCategoryOnLaunchPad.AddToggle("Photon Freeze", "Freeze yourself for other players, voice will still work", PhotonFreeze.ToggleFreeze);
            }

            Con.Debug("Done Setting up Menus", MintCore.isDebug);
            yield break;
        }

        private static IEnumerator SetTheFuckingSprite() {
            yield return new WaitForFixedUpdate();
            UnityEngine.Object.DestroyImmediate(TheMintMenuButton.transform.Find("Icon").GetComponent<StyleElement>());
            MintIcon = TheMintMenuButton.transform.Find("Icon").GetComponent<Image>();
            MintIcon.sprite = MintyResources.MintIcon;
            MintIcon.color = Color.white;
            StyleElement _styleElement = TheMintMenuButton.GetComponent<StyleElement>();
            if (_styleElement.field_Public_String_0 == "Back") // Ignore Style Changes
                _styleElement.field_Public_String_0 = "MintMenuButton";
            else _styleElement.field_Public_String_1 = "MintMenuButton";
        }

        #region Quick Actions

        internal static void MintQuickActions() {
            MintQuickActionsCat = MintMenu.AddCategory("Quick Actions");
            MintQAFly = MintQuickActionsCat.AddToggle("Flight", "Toggle Flight", Movement.Fly);
            MintQANoClip = MintQuickActionsCat.AddToggle("No Clip", "Toggle No Clip", Movement.NoClip);
            MintQAFreeze = MintQuickActionsCat.AddToggle("Photon Freeze", "Freeze yourself for other players, voice will still work", PhotonFreeze.ToggleFreeze);

            MintQuickActionsCat.AddSpacer();
            string tempver = MintCore.ModBuildInfo.Version;
            string text = $"MintMenuv{tempver.Replace(".", "")}";
            string parent = $"UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_ReMod{text}/ScrollRect/Viewport/VerticalLayoutGroup/";
            FlightSpeedSlider = new(parent, f => Movement.finalSpeed = f, "FlightSpeed", "Control Flight Speed", "500%", 5f, 
                "Flight Speed > 0% - 500%", 0f, 1f);
            //if (FlightSpeedSlider != null)
            //    MintMenu.OnOpen += () => FlightSpeedSlider.Value = Movement.finalSpeed;

            Con.Debug("Done Creating QuickActions", MintCore.isDebug);
        }

        #endregion

        #region Player Menu

        private static ReMenuToggle InfJump;
        
        internal static void Player() {
            PlayerMenu = BaseActions.AddCategoryPage("Player Menu", "Actions involving players.");
            var p = PlayerMenu.AddCategory("General Actions");
            PlayerESP = p.AddToggle("Player ESP", "Puts a bubble around each player, and is visible through walls.", ESP.PlayerESPState);
            p.AddButton("Copy Current Avi ID", "Copys your current Avatar ID into your clipboard", () => {
                try {
                    Clipboard.SetText(APIUser.CurrentUser.avatarId);
                }
                catch (Exception c) {
                    MelonLogger.Error(c);
                }
            });
            p.AddButton("Go into Avi by ID", "Takes an Avatar ID from your clipboard and changes into that avatar.", () => {
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
                    else
                        VRCUiManager.field_Private_Static_VRCUiManager_0.InformHudText("No avatar ID in clipboard", Color.white);
                }
                catch (Exception c) {
                    MelonLogger.Error(c);
                }
            });
            InfJump = p.AddToggle("Infinite Jump", "What is more to say? Infinitely Jump to your heart's content", onToggle => PlayerActions.InfinteJump = onToggle);
            //var h = PlayerMenu.AddCategory("Head Lamp");
        }

        #endregion

        #region World Menu

        internal static void World() {
            WorldMenu = BaseActions.AddCategoryPage("World Menu", "Actions involving the world.");
            var w = WorldMenu.AddCategory("General Actions");
            ItemESP = w.AddToggle("Item ESP", "Puts a bubble around all Pickups, can be seen through walls", ESP.SetItemESPToggle);
            w.AddButton("Add Jump", "Allows you to jump in the world", WorldActions.AddJump);//, MintyResources.ConvertToTexture2D(MintyResources.JumpIcon));
            w.AddButton("Legacy Locomotion", "Adds old SDK2 movement in the current SDK3 world", Networking.LocalPlayer.UseLegacyLocomotion);
            w.AddButton("Download VRCW", "Downloads the world file (.vrcw)", WorldActions.WorldDownload);

            w.AddButton("Copy Instance ID", "Copies current instance ID and places it in your system's clipboard.", () => {
                    string world = RoomManager.field_Internal_Static_ApiWorld_0.id + ":" + RoomManager.field_Internal_Static_ApiWorldInstance_0.instanceId;
                    bool faulted = false;
                    try {
                        GUIUtility.systemCopyBuffer = world;
                    }
                    catch {
                        Clipboard.SetText(world);
                        faulted = true;
                    }

                    MelonLogger.Msg(faulted ? "Failed to copy intance ID" : $"Got ID: {world}");
                });
            w.AddButton("Join Instance by ID", "Join the room of the instance ID", () => {
                try {
                    string clip = string.Empty;
                    try { clip = GUIUtility.systemCopyBuffer; } catch { clip = Clipboard.GetText(); }

                    if (clip.Contains("launch?")) {
                        MelonLogger.Warning("Invalid Join ID, please do not use web links");
                    }
                    else if ((clip.Contains("~hidden(") || clip.Contains("~friends(") || clip.Contains("~public(") || clip.Contains("~private(")) && clip.Contains("launch?")) {
                        Networking.GoToRoom(clip);
                    }
                }
                catch (Exception j) { MelonLogger.Error(j); }
            });
            w.AddSpacer();
            w.AddButton("Log World", "Logs world info (of various data points) in a text file.", WorldActions.LogWorld);
            
            w.AddButton("Normal World Mirrors", "Reverts mirrors to their original state", WorldActions.RevertMirrors);
            w.AddButton("Optimize Mirrors", "Make Mirrors only show players", WorldActions.OptimizeMirrors);
            w.AddButton("Bueatify Mirrors", "Make Mirrors show everything", WorldActions.BeautifyMirrors);
            w.AddSpacer();

            var e = WorldMenu.AddCategory("Item Manipulation");
            e.AddButton("Teleport Items to Self", "Teleports all Pickups to your feet.", Items.TPToSelf);
            e.AddButton("Respawn Items", "Respawns All pickups to their original location.", Items.Respawn);
            e.AddButton("Teleport Items Out of World", "Teleports all Pickups an XYZ coord of 1 million", Items.TPToOutWorld);
        }

        #endregion

        #region Funny Dev Random Shit

        internal static void RandomStuff() {
            RandomMenu = BaseActions.AddCategoryPage("Utilities", "Contains random functions");
            var r = RandomMenu.AddCategory("General Actions");
            
            DeviceType = r.AddToggle("Spoof as Quest", "Spoof your VRChat login as Quest.",
                on => MelonPreferences.GetEntry<bool>(Config.mint.Identifier, Config.SpoofDeviceType.Identifier).Value = on);
            DeviceType.Toggle(Config.SpoofDeviceType.Value);
            
            r.AddSpacer();
            
            FrameSpoof = r.AddToggle("Spoof Frames", "Spoof your framerate for the monkes.",
                on => MelonPreferences.GetEntry<bool>(Config.mint.Identifier, Config.SpoofFramerate.Identifier).Value = on);
            FrameSpoof.Toggle(Config.SpoofFramerate.Value);
            
            Frame = r.AddButton($"{Config.SpoofedFrameNumber.Value}", "This is the number of your spoofed framerate.", () => {
                VRCUiPopupManager.field_Private_Static_VRCUiPopupManager_0.ShowInputPopupWithCancel("Set Spoofed Framerate", "",
                    InputField.InputType.Standard, true, "Set Frames", (_, __, ___) => {
                        float.TryParse(_, out float f);
                        MelonPreferences.GetEntry<float>(Config.mint.Identifier, Config.SpoofedFrameNumber.Identifier).Value = f;
                        Frame.Text = f.ToString();
                    }, () => VRCUiPopupManager.field_Private_Static_VRCUiPopupManager_0.HideCurrentPopup());
            });

            PingSpoof = r.AddToggle("Spoof Ping", "Spoof your ping for the monkes.",
                on => MelonPreferences.GetEntry<bool>(Config.mint.Identifier, Config.SpoofPing.Identifier).Value = on);
            PingSpoof.Toggle(Config.SpoofPing.Value);
            
            PingNegative = r.AddToggle("Negative Ping", "Make your spoofed ping nagative.",
                on => MelonPreferences.GetEntry<bool>(Config.mint.Identifier, Config.SpoofedPingNegative.Identifier).Value = on);
            PingNegative.Toggle(Config.SpoofedPingNegative.Value);
            
            Ping = r.AddButton($"<color={(Config.SpoofedPingNegative.Value ? "red>-" : "#00ff00>")}{Config.SpoofedPingNumber.Value}</color>", "This is the number of your spoofed ping.", () => {
                VRCUiPopupManager.field_Private_Static_VRCUiPopupManager_0.ShowInputPopupWithCancel("Set Spoofed Ping", "",
                    InputField.InputType.Standard, true, "Set Ping", (_, __, ___) => {
                        int.TryParse(_, out int p);
                        MelonPreferences.GetEntry<int>(Config.mint.Identifier, Config.SpoofedPingNumber.Identifier).Value = p;
                        Ping.Text = $"<color={(Config.SpoofedPingNegative.Value ? "red>-" : "#00ff00>")}{p.ToString()}</color>";
                    }, () => VRCUiPopupManager.field_Private_Static_VRCUiPopupManager_0.HideCurrentPopup());
            });
            
            bypassRiskyFunc = r.AddToggle("Bypass Risky Func", "Forces Mods with Risky Function Checks to work", 
                on => MelonPreferences.GetEntry<bool>(Config.mint.Identifier, Config.bypassRiskyFunc.Identifier).Value = on);
            bypassRiskyFunc.Toggle(Config.bypassRiskyFunc.Value);
        }

        #endregion

        #region UserSelect Menu

        private static GameObject TheUserSelectMenu;
        internal static void UserSelMenu() {
            TheUserSelectMenu = GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_SelectedUser_Local/ScrollRect/Viewport/VerticalLayoutGroup");

            userSelectCategory = new ReMenuCategory("MintMod", TheUserSelectMenu.transform);

            string u = "selected user";

            userSelectCategory.AddButton("Download Avatar VRCA", $"Downloads {u}'s Avatar .VRCA", PlayerActions.AvatarDownload);
            userSelectCategory.AddButton("Log Asset", $"Logs {u}'s information and put it into a text file", PlayerActions.LogAsset);
            userSelectCategory.AddButton("Copy Avatar ID", $"Copies {u}'s avatar ID into your clipboard", () => GUIUtility.systemCopyBuffer = PlayerActions.SelPAvatar().id);
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
                    } else
                        VRCUiManager.prop_VRCUiManager_0.InformHudText("Avatar is private", Color.white);
                } catch (Exception c) {
                    Con.Error($"{c}");
                    Con.Warn("Attempting fallback method");
                    try {
                        var v = PlayerActions.SelPAvatar();
                        PlayerManager.field_Private_Static_PlayerManager_0.field_Private_Player_0._vrcplayer
                            .ChangeToAvatar(v.id);
                    }
                    catch (Exception f) {
                        Con.Error($"{f}");
                    }
                }
            });

            if (Config.AviFavsEnabled.Value) {
                userSelectCategory.AddButton("Silent Favorite", $"Silently favorites the avatar {u} is wearing if public.", () => {
                    var v = PlayerActions.SelPAvatar();
                    foreach (var f in AvatarFavs.AviFavSetup.Favorites.Instance.AvatarFavorites.FavoriteLists) {
                        if (!v.releaseStatus.ToLower().Contains("private"))
                            AvatarFavs.AviFavLogic.FavoriteAvatar(v, f.ID);
                        else {
                            Con.Warn("Avatar is private, cannot favorite");
                            VRCUiManager.prop_VRCUiManager_0.InformHudText("Avatar is private, cannot favorite", Color.white);
                        }
                    }
                });
            }

            userSelectCategory.AddButton("Teleport to", $"Teleport to {u}", () => { PlayerActions.Teleport(PlayerWrappers.SelVRCPlayer()); });
            userSelectCategory.AddButton("Teleport pickups to", $"Teleport all pickup objects to {u}", () => Items.TPToPlayer(PlayerWrappers.SelVRCPlayer()._player));

            if (APIUser.CurrentUser.id == Players.LilyID) {
                userSelectCategory.AddButton("Mint Auth Check", $"Check to see if {u} can use MintMod", () => MelonCoroutines.Start(ServerAuth.SimpleAuthCheck(PlayerWrappers.GetSelectedAPIUser().id)));
            }

            Con.Debug("Finished Creating User Selected Menu", MintCore.isDebug);
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
        
        internal static void PlayerListMenuSetup() {
            PlayerListMenu = BaseActions.AddCategoryPage("Player List Menu", "Actions to do individually on a player.");
            var p = PlayerListMenu.AddCategory("Actions");
            var l = PlayerListMenu.AddCategory("Player List (In Current Room)");
            p.AddButton("Teleport", "Teleport to player", () => {
                SelectedActionNum = 1;
                l.Title = "Player List (In Current Room) > Teleport";
            });
            var _1 = p.AddButton("OpenQM", "Open player in Quick Menu", () => {
                SelectedActionNum = 2;
                l.Title = "Player List (In Current Room) > OpenQM";
            });
            p.AddButton("ESP", "Draw a bubble around player", () => {
                SelectedActionNum = 3;
                l.Title = "Player List (In Current Room) > ESP";
            });
            p.AddButton("Teleport\nPickups to", "Teleport all pickups to player", () => {
                SelectedActionNum = 4;
                l.Title = "Player List (In Current Room) > Teleport Pickups";
            });
            var _2 = p.AddButton("Orbit\nPickups", "Orbit pickups around player", () => {
                SelectedActionNum = 5;
                l.Title = "Player List (In Current Room) > Orbit Pickups";
            });
            var _3 = p.AddButton("Orbit\nPlayer", "Orbit around player", () => {
                SelectedActionNum = 6;
                l.Title = "Player List (In Current Room) > Orbit Player";
            });
            // These button are disabled until I add the functions for them
            _1.Interactable = false;
            _2.Interactable = false;
            _3.Interactable = false;
            p.AddButton("Clear ESPs", "Clears any and all ESP bubbles around players", ESP.ClearAllPlayerESP);
            
            
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
                                    // Action Not Yet Setup
                                    break;
                                case PlayerListActions.OrbitPlayer:
                                    // Action Not Yet Setup
                                    break;
                                default:
                                    Con.Warn("Nothing is selected.");
                                    VRCUiManager.prop_VRCUiManager_0.InformHudText("Nothing is selected", Color.yellow);
                                    break;
                            }
                        });
                    PlayerButtons.Add(SinglePlayerButton);
                }
            };
        }

        #endregion

        internal override void OnLevelWasLoaded(int buildindex, string SceneName) {
            if (buildindex == -1) {
                PhotonFreeze.ToggleFreeze(false);
                ESP.ClearAllPlayerESP();
                if (InfJump != null) InfJump.Toggle(false, true, true);
            }
        }

        internal override void OnUpdate() => PlayerActions.UpdateJump();

        internal override void OnPrefSave() {
            if (DeviceType != null)
                DeviceType.Toggle(Config.SpoofDeviceType.Value);
            if (FrameSpoof != null)
                FrameSpoof.Toggle(Config.SpoofFramerate.Value);
            if (PingSpoof != null)
                PingSpoof.Toggle(Config.SpoofPing.Value);
            if (PingNegative != null)
                PingNegative.Toggle(Config.SpoofedPingNegative.Value);
            if (bypassRiskyFunc != null)
                bypassRiskyFunc.Toggle(Config.bypassRiskyFunc.Value);
            if (Frame != null)
                Frame.Text = $"{Config.SpoofedFrameNumber.Value}";
            if (Ping != null)
                Ping.Text = $"<color={(Config.SpoofedPingNegative.Value ? "red>-" : "#00ff00>")}{Config.SpoofedPingNumber.Value}</color>";
        }

        internal static void UpdateMintIconForStreamerMode(bool o) {
            if (MintIcon != null) {
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
                MelonPreferences.GetEntry<bool>(Config.mint.Identifier, Config.SpoofFramerate.Identifier).Value = false;
            if (Config.SpoofPing.Value)
                MelonPreferences.GetEntry<bool>(Config.mint.Identifier, Config.SpoofPing.Identifier).Value = false;
        }
    }
}
