using System;
using System.Collections;
using System.Windows;
using System.Windows.Forms;
//using Il2CppSystem;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using MintMod.Functions;
using MintMod.Resources;
using ReMod.Core.UI.QuickMenu;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.UI;
using VRC;
using VRC.Core;
using VRC.SDKBase;
using VRC.UI;
using VRC.UI.Core.Styles;
using VRC.UI.Elements;
using VRCSDK2;
using Button = UnityEngine.UI.Button;
using MintMod.Managers;
using MintMod.Reflections;
using MintMod.Utils;
using MintyLoader;
using ReMod.Core.VRChat;
using MintMod.Functions.Authentication;

namespace MintMod.UserInterface.QuickMenu {
    class MintUserInterface : MintSubMod {
        public override string Name => "Mint UI";
        public override string Description => "Builds all of the Mod's menu.";

        private static GameObject MainMenuBackButton, TheMintMenuButton, ShittyAdverts, ShittyAdverts_2, LaunchPadLayoutGroup;

        private static ReMenuCategory MintCategoryOnLaunchPad, BaseActions, MintQuickActionsCat, userSelectCategory;

        public static ReCategoryPage MintMenu, PlayerMenu, WorldMenu, RandomMenu;

        public static ReMenuToggle 
            MainQMFly, MainQMNoClip, MainQMFreeze,
            MintQAFly, MintQANoClip, MintQAFreeze;

        //private static Sprite WorldIcon, PlayerIcon;

        public static ReMenuToggle ItemESP, PlayerESP, DeviceType, FrameSpoof, PingSpoof, PingNegative, bypassRiskyFunc;
        public static ReMenuButton Frame, Ping;

		internal static IEnumerator OnQuickMenu() {
            while (UnityEngine.Object.FindObjectOfType<VRC.UI.Elements.QuickMenu>() == null)
                yield return null;
            //ReMod.Core.Managers.UiManager.FixLaunchpadScrolling();
            MelonCoroutines.Start(BuildMint());
        }

        internal static IEnumerator OnUserSelectMenu() {
            while (UnityEngine.Object.FindObjectOfType<VRC.UI.Elements.Menus.SelectedUserMenuQM>() == null)
                yield return null;

            UserSelMenu();
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
            Image MintIcon = TheMintMenuButton.transform.Find("Icon").GetComponent<Image>();
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

            Con.Debug("Done Creating QuickActions", MintCore.isDebug);
        }

        #endregion

        #region Player Menu

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
                        VRCUiManager.prop_VRCUiManager_0.InformHudText("No avatar ID in clipboard", Color.white);
                }
                catch (Exception c) {
                    MelonLogger.Error(c);
                }
            });
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
                        MelonPreferences.GetEntry<float>(Config.mint.Identifier, Config.SpoofedFrameNumber.Identifier) .Value = f;
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
                        float.TryParse(_, out float p);
                        MelonPreferences.GetEntry<float>(Config.mint.Identifier, Config.SpoofedPingNumber.Identifier).Value = p;
                        Ping.Text = $"<color={(Config.SpoofedPingNegative.Value ? "red>-" : "#00ff00>")}{p}</color>";
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

            string u = PlayerWrappers.SelPlayer()._vrcplayer.field_Private_VRCPlayerApi_0.displayName;//"selected user";

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
                    MelonLogger.Error(c);
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
            userSelectCategory.AddButton("Teleport pickups to", $"Teleport all picktup objects to {u}", () => Items.TPToPlayer(PlayerWrappers.SelPlayer()));

            if (APIUser.CurrentUser.id == Players.LilyID) {
                userSelectCategory.AddButton("Mint Auth Check", $"Check to see if {u} can use MintMod", () => MelonCoroutines.Start(ServerAuth.SimpleAuthCheck(PlayerWrappers.GetSelectedAPIUser().id)));
            }

            Con.Debug("Finished Creating User Selected Menu", MintCore.isDebug);
        }

        #endregion

        internal override void OnLevelWasLoaded(int buildindex, string SceneName) {
            if (buildindex == -1) {
                PhotonFreeze.ToggleFreeze(false);
            }
        }

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
    }
}
