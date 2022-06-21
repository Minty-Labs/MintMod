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
using VRC.UI.Core;
using VRC.SDKBase;
using VRC.UI;
using VRC.UI.Core.Styles;
using MintMod.Managers;
using MintMod.Reflections;
using MintMod.Utils;
using MintyLoader;
using MintMod.Functions.Authentication;
using MintMod.Libraries;
using MintMod.UserInterface.AvatarFavs;
using ReMod.Core.UI;
using UnityEngine.XR;
using BuildInfo = MintyLoader.BuildInfo;
using Object = UnityEngine.Object;

namespace MintMod.UserInterface.QuickMenu {
    internal class MintUserInterface : MintSubMod {
        public override string Name => "Mint UI";
        public override string Description => "Builds all of the Mod's menu.";

        private static GameObject TheMintMenuButton;

        private static ReMenuCategory MintCategoryOnLaunchPad, BaseActions, /*MintQuickActionsCat,*/ playerListCategory;

        public static ReCategoryPage MintMenu, PlayerMenu, WorldMenu, RandomMenu, PlayerListMenu, /*AvatarMenu*/ NameplateMenu, WorldActionsPage;

        private static ReMenuSlider FlightSpeedSlider;

        internal static ReMenuToggle 
            MainQMFly, MainQMNoClip, MainQMFreeze, MainQMInfJump,
            MintQAFly, MintQANoClip, MintQAFreeze;

        //private static Sprite WorldIcon, PlayerIcon;

        internal static ReMenuToggle ItemESP, PlayerESP, DeviceType, FrameSpoof, PingSpoof, PingNegative, bypassRiskyFunc;
        public static ReMenuButton Frame, Ping;

        private static Image _mintIcon;

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
            Player();
            try {
                World();
            }
            catch (Exception e) {
                if (e.ToString().Contains("AddSpacer"))
                    Con.Error("Please remove the ReMod.Core.dll file from the root of your VRChat game directory. Then, restart the game to fix this error.");
            }
            RandomStuff();
            PlayerListActionSet.MenuSetup(BaseActions);
            PlayerListControls.PlayerListOptions(BaseActions);
            //BuildAvatarMenu();
            BuildNameplateMenu();
            BuildWorldActionsMenu();
            
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
                        VrcUiPopups.Notify(MintCore.ModBuildInfo.Name, "No Avatar ID in clipboard", MintyResources.Alert);
                    }
                }
                catch (Exception c) {
                    Con.Error(c);
                }
            }, MintyResources.checkered);
            c.AddButton("Download Own VRCA", "Downloads the VRCA of the avatar that you're in", async () => await PlayerActions.AvatarSelfDownload(), MintyResources.user);
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
            w.AddButton("Download VRCW", "Downloads the world file (.vrcw)", async () => await WorldActions.WorldDownload(), MintyResources.dl);
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
            e.Active = ServerAuth.HasSpecialPermissions;
            
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
                "Reloads Mint's custom nameplate addons in case more were added while you're playing", () => {
                    Players.FetchCustomPlayerObjects(true);
                    VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
                }, MintyResources.extlink);

            r.AddButton("Clear HUD Message Queue", "Clears the HUD Popup Message Queue", () => {
                if (!Config.UseOldHudMessages.Value) {
                    ReMod.Core.Notification.NotificationSystem.ClearNotification();
                    ReMod.Core.Notification.NotificationSystem.CloseNotification();
                }
                else
                    VRCUiManager.prop_VRCUiManager_0.field_Private_List_1_String_0.Clear();
            }, MintyResources.messages);
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
            InfJump?.Toggle(false, true, true);
            MainQMInfJump?.Toggle(false, true, true);
            _worldToggle?.Toggle(false, true, true);
        }

        internal override void OnUpdate() => PlayerActions.UpdateJump();

        internal override void OnPrefSave() {
            //DeviceType?.Toggle(Config.SpoofDeviceType.Value);
            FrameSpoof?.Toggle(Config.SpoofFramerate.Value);
            PingSpoof?.Toggle(Config.SpoofPing.Value);
            PingNegative?.Toggle(Config.SpoofedPingNegative.Value);
            bypassRiskyFunc?.Toggle(Config.bypassRiskyFunc.Value);
            PlayerListControls.PlEnabled?.Toggle(Config.PLEnabled.Value);
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

            QmMediaPanel.OnPrefSaved();
            
            _mintNameplates?.Toggle(Config.EnableCustomNameplateReColoring.Value);
            _mintTags?.Toggle(Config.EnabledMintTags.Value);
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
