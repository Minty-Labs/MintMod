﻿using System.Collections.Generic;
using System.Windows.Forms;
using MintMod.Functions;
using MintMod.Functions.Authentication;
using MintMod.Libraries;
using MintMod.Managers;
using MintMod.Reflections;
using MintMod.Resources;
using MintMod.Utils;
using MintyLoader;
using ReMod.Core.UI.QuickMenu;
using ReMod.Core.VRChat;
using UnityEngine;
using UnityEngine.XR;
using VRC.Core;
using VRC.UI;
using System;

namespace MintMod.UserInterface.QuickMenu {
    internal static class PlayerMenu {
        private static List<ReMenuButton> PlayerButtons = new();
        internal static ReMenuToggle PlayerEsp;
        private static ReMenuButton _singlePlayerButton;
        private static ReCategoryPage _player;//, _playerListMenu;

        private enum PlayerListActions {
            None,
            Teleport,
            OpenQm,
            Esp,
            TeleportObjs,
            OrbitObjs,
            OrbitPlayer
        }

        private static PlayerListActions GetSelectedAction(int type) => type switch {
                1 => PlayerListActions.Teleport,
                2 => PlayerListActions.OpenQm,
                3 => PlayerListActions.Esp,
                4 => PlayerListActions.TeleportObjs,
                5 => PlayerListActions.OrbitObjs,
                6 => PlayerListActions.OrbitPlayer,
                _ => PlayerListActions.None
            };

        private static int _selectedActionNum;
        
        private static ReMenuSlider _itemsReSliderSp, _itemsReSliderDi, _playerReSliderSp, _playerReSliderDi;
        private static ReMenuSliderCategory _itemSliderCat, _playerSliderCat;
        
        #region Orbit Sliders
        
        private static void CreateSliders(ReCategoryPage _page) {
            _itemSliderCat = _page.AddSliderCategory("Items");
            _itemsReSliderSp = _itemSliderCat.AddSlider("Speed", "Change orbit speed of rotation", f => Items.SpinSpeed = f, 1f, 0f, 2f);
            _itemsReSliderDi = _itemSliderCat.AddSlider("Distance", "Change distance of rotation", f => Items.Distance = f, 1f, 0f, 5f);
            _itemSliderCat.Header.GameObject.SetActive(false);
            _itemsReSliderSp.Active = false;
            _itemsReSliderDi.Active = false;
                
            _playerSliderCat = _page.AddSliderCategory("Players");
            _playerReSliderSp = _playerSliderCat.AddSlider("Speed", "Change orbit speed of rotation", f => Players.SelfSpinSpeed = f, 1f, 0f, 2f);
            _playerReSliderDi = _playerSliderCat.AddSlider("Distance", "Change distance of rotation", f => Players.SelfDistance = f, 1f, 0.1f, 5f);
            _playerSliderCat.Header.GameObject.SetActive(false);
            _playerReSliderSp.Active = false;
            _playerReSliderDi.Active = false;
        }

        #endregion
        
        internal static void MenuSetup(ReMenuCategory baseActions) {
            _player = baseActions.AddCategoryPage("Player", "Actions involving players.", MintyResources.people);
            var c = _player.AddCategory("General Actions", false);
            
            PlayerEsp = c.AddToggle("Player ESP", "Puts a bubble around each player, and is visible through walls.", ESP.PlayerESPState);
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
            // var a = c.AddButton("Download Own VRCA", "Downloads the VRCA of the avatar that you're in", async ()
            //     => await PlayerActions.AvatarSelfDownload(), MintyResources.user);
            c.AddButton("Download Own VRCA", "Downloads the VRCA of the avatar that you're in", PlayerActions.AvatarSelfDownload, MintyResources.user);
            //a.Interactable = false;
            c.AddButton("Add Jump", "Allows you to jump in the world", PlayerActions.AddJump, MintyResources.jump);
            
            c.AddToggle("Keep Inf Jump On", "Toggles whether you want to keep Infinite Jump on regardless of world changes", b => {
                Config.SavePrefValue(Config.Base, Config.KeepInfJumpAlwaysOn, b);
                if (PlayerActions.InfiniteJump) return;
                MintUserInterface.InfJump?.Toggle(true, true, true);
                MintUserInterface.MainQMInfJump?.Toggle(true, true, true);
            }, Config.KeepInfJumpAlwaysOn.Value);
            
            //_playerListMenu = baseActions.AddCategoryPage("Player List", "Actions to do individually on a player.", MintyResources.address_book);
            var p = _player.AddCategory("Actions");
            CreateSliders(_player);
            var l = _player.AddCategory("Player List (Select an Action)");
            p.AddButton("Teleport", "Teleport to player", () => {
                _selectedActionNum = 1;
                l.Title = "Player List > Teleport";
                //PlayerSlider.Enabled = false;
                //ItemSlider.Enabled = false;
                _itemsReSliderSp.Active = false;
                _itemsReSliderDi.Active = false;
                _playerReSliderSp.Active = false;
                _playerReSliderDi.Active = false;
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
                _selectedActionNum = 3;
                l.Title = "Player List > ESP";
                //PlayerSlider.Enabled = false;
                //ItemSlider.Enabled = false;
                _itemsReSliderSp.Active = false;
                _itemsReSliderDi.Active = false;
                _playerReSliderSp.Active = false;
                _playerReSliderDi.Active = false;
            });
            var tpItemsTo = p.AddButton("Teleport\nPickups to", "Teleport all pickups to player", () => {
                _selectedActionNum = 4;
                l.Title = "Player List > Teleport Pickups";
                //PlayerSlider.Enabled = false;
                //ItemSlider.Enabled = false;
                _itemsReSliderSp.Active = false;
                _itemsReSliderDi.Active = false;
                _playerReSliderSp.Active = false;
                _playerReSliderDi.Active = false;
            });
            var orbitPickups = p.AddButton("Orbit\nPickups", "Orbit pickups around player", () => {
                _selectedActionNum = 5;
                l.Title = "Player List > Orbit Pickups";
                //PlayerSlider.Enabled = false;
                //ItemSlider.Enabled = true;
                _itemsReSliderSp.Active = true;
                _itemsReSliderDi.Active = true;
                _playerReSliderSp.Active = false;
                _playerReSliderDi.Active = false;
            });
            var orbitPlayer = p.AddButton("Orbit\nPlayer", "Orbit around player", () => {
                _selectedActionNum = 6;
                l.Title = "Player List > Orbit Player";
                //PlayerSlider.Enabled = true;
                //ItemSlider.Enabled = false;
                _itemsReSliderSp.Active = false;
                _itemsReSliderDi.Active = false;
                _playerReSliderSp.Active = true;
                _playerReSliderDi.Active = true;
            });
            // These button are disabled until I add the functions for them
            //_1.Interactable = false;
            var clearEsp = p.AddButton("<color=#FF96AA>Clear ESPs</color>", "Clears any and all ESP bubbles around players", ESP.ClearAllPlayerESP);
            var clearOrbit = p.AddButton("<color=#FF96AA>Clear Orbits</color>", "Clear any type of orbiting", () => {
                switch (_selectedActionNum) {
                    case 5:
                        Items.ClearRotating();
                        break;
                    case 6:
                        Players.Toggle(false);
                        break;
                    default:
                        Con.Warn("Nothing to cancel orbit.");
                        break;
                }
            });

            var unlock = ServerAuth.HasSpecialPermissions;
            tpItemsTo.Active = unlock;
            orbitPickups.Active = unlock;
            orbitPlayer.Active = unlock;
            clearEsp.Active = unlock;
            clearOrbit.Active = unlock;

            _player.OnOpen += () => {
                foreach (var button in PlayerButtons)
                    button.Destroy();
                PlayerButtons.Clear();
                var enumerator2 = PlayerWrappers.GetAllPlayers().GetEnumerator();

                while (enumerator2.MoveNext()) {
                    var player = enumerator2.current;
                    var n = player.field_Private_APIUser_0.displayName;
                    
                    string tempName;
                    tempName = PlayerWrappers.IsFriend(player) ? 
                        $"<color=#{Config.FriendRankHEX.Value}>{n}</color>" : 
                        $"<color=#{ColorConversion.ColorToHex(VRCPlayer.Method_Public_Static_Color_APIUser_0(player.GetAPIUser()))}>{n}</color>";
                    
                    if (player.field_Private_APIUser_0.id.StartsWith("usr_6d71d3be"))
                        tempName = "<color=#9fffe3>Lily</color>";
                    
                    _singlePlayerButton = l.AddButton($"{tempName}", "Click to do selected action",
                        () => {
                            switch (GetSelectedAction(_selectedActionNum)) {
                                case PlayerListActions.Teleport:
                                    if (PlayerWrappers.GetCurrentPlayer()._player != player)
                                        PlayerActions.Teleport(player._vrcplayer);
                                    break;
                                case PlayerListActions.OpenQm:
                                    // Action Not Yet Setup
                                    break;
                                case PlayerListActions.Esp:
                                    if (ESP.isESPEnabled)
                                        Con.Warn("Main ESP is already active");
                                    else
                                        if (PlayerWrappers.GetCurrentPlayer()._player != player)
                                            ESP.SinglePlayerESP(player, true);
                                    break;
                                case PlayerListActions.TeleportObjs:
                                    Items.TPToPlayer(player);
                                    break;
                                case PlayerListActions.OrbitObjs:
                                    Items.Toggle(player, !Items.Rotate);
                                    ShowInfoPopup();
                                    break;
                                case PlayerListActions.OrbitPlayer:
                                    Players.Toggle(!Players.Rotate, player);
                                    ShowInfoPopup();
                                    break;
                                case PlayerListActions.None:
                                default:
                                    Con.Warn("Nothing is selected.");
                                    VrcUiPopups.Notify(MintCore.ModBuildInfo.Name, "Noting is selected", MintyResources.Alert);
                                    break;
                            }
                        });
                    PlayerButtons.Add(_singlePlayerButton);
                }
            };
        }

        private static void ShowInfoPopup() 
            => VrcUiPopups.Notify(MintCore.ModBuildInfo.Name, $"To disable orbit, press {(XRDevice.isPresent ? "down both triggers" : "the letter \"P\"")}", MintyResources.Alert);
    }
}