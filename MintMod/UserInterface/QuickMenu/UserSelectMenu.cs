using MelonLoader;
using MintMod.Functions;
using MintMod.Functions.Authentication;
using MintMod.Libraries;
using MintMod.Managers;
using MintMod.Reflections;
using MintMod.Resources;
using MintMod.UserInterface.AvatarFavs;
using MintMod.Utils;
using MintyLoader;
using ReMod.Core.UI.QuickMenu;
using ReMod.Core.VRChat;
using UnityEngine;
using VRC;
using VRC.Core;
using VRC.UI;
using System;
using System.Linq;
using MintMod.Reflections.VRCAPI;

namespace MintMod.UserInterface.QuickMenu; 

public class UserSelectMenu {
    private static ReMenuCategory _userSelectCategory;
    
    public static void UserSelMenu() {
            var f = QuickMenuEx.Instance.field_Public_Transform_0.Find("Window/QMParent/Menu_SelectedUser_Local");
            var theUserSelectMenu = f.Find("ScrollRect/Viewport/VerticalLayoutGroup").gameObject;

            _userSelectCategory = new ReMenuCategory("MintMod", theUserSelectMenu.transform);

            _userSelectCategory.AddButton("Download Avatar VRCA", "Downloads the selected user's Avatar .VRCA", async () => await PlayerActions.AvatarDownload(), MintyResources.dl);
            _userSelectCategory.AddButton("Log Asset", "Logs the selected user's information and put it into a text file", PlayerActions.LogAsset, MintyResources.list);
            _userSelectCategory.AddButton("Copy Avatar ID", "Copies the selected user's avatar ID into your clipboard", () => GUIUtility.systemCopyBuffer = PlayerActions.SelPAvatar().id, MintyResources.copy);
            _userSelectCategory.AddButton("Copy User ID", "Copies the selected user's User ID into your clipboard", () => GUIUtility.systemCopyBuffer = PlayerWrappers.GetSelectedAPIUser().id, MintyResources.copy);
            _userSelectCategory.AddButton("Clone Avatar", "Clones the selected user's avatar if public", () => {
                var apiAvatar = PlayerActions.SelPAvatar();
                var avatarIsPublic = apiAvatar.releaseStatus.ToLower().Contains("public");
                if (!avatarIsPublic) {
                    VrcUiPopups.Notify(MintCore.ModBuildInfo.Name, "Avatar is private", MintyResources.Lock);
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
                _userSelectCategory.AddButton("Silent Favorite", "Silently favorites the avatar the selected user is wearing if public.", () => {
                    var user = QuickMenuEx.SelectedUserLocal.field_Private_IUser_0;
                    if (user == null) {
                        Con.Error("Selected User could not be found");
                        return;
                    }

                    var player = PlayerManager.field_Private_Static_PlayerManager_0.GetPlayer(user.prop_String_0);
                    if (player == null) {
                        Con.Error("Player could not be found");
                        return;
                    }

                    var apiAvatar = player.GetApiAvatar();
                    
                    if (!apiAvatar.releaseStatus.ToLower().Contains("private")) {
                        if (!ReFavs._instance.HasAvatarFavorited(apiAvatar.id)) {
                            AviFavLogic.GetConfigList(0).Avatars.Insert(0, new AvatarObject(apiAvatar));
                            ReFavs._instance._favoriteButton.Text = "<color=#fd4544>Unfavorite</color>";
                            if (Config.AviLogFavOrUnfavInConsole.Value)
                                Con.Msg($"Favorited {apiAvatar.name} into Minty Favorites");
                            VrcUiPopups.Notify(MintCore.ModBuildInfo.Name, $"Favorited the avatar: {apiAvatar.name}", MintyResources.star);
                        }
                        else {
                            AviFavLogic.GetConfigList(0).Avatars.Remove(AviFavLogic.GetConfigList(0).Avatars.Single(avi => avi.id == apiAvatar.id));
                            ReFavs._instance._favoriteButton.Text = "<color=#9fffe3>Favorite</color>";
                            if (Config.AviLogFavOrUnfavInConsole.Value)
                                Con.Msg($"Removed {apiAvatar.name} from Minty Favorites");
                        }
                    }
                    else {
                        Con.Warn("Avatar is private, cannot favorite");
                        VrcUiPopups.Notify(MintCore.ModBuildInfo.Name, "Avatar is private, cannot favorite", MintyResources.Lock);
                    }
                }, MintyResources.star);
            }

            _userSelectCategory.AddButton("Teleport to", "Teleport to the selected user", () => {
                var user = QuickMenuEx.SelectedUserLocal.field_Private_IUser_0;
                if (user == null) {
                    Con.Error("Selected User could not be found");
                    return;
                }

                var player = PlayerManager.field_Private_Static_PlayerManager_0.GetPlayer(user.prop_String_0);
                if (player == null) {
                    Con.Error("Player could not be found");
                    return;
                }
                
                PlayerActions.Teleport(player._vrcplayer);
            }, MintyResources.marker_hole);
            var items = _userSelectCategory.AddButton("Teleport pickups to", "Teleport all pickup objects to the selected user", () => {
                var user = QuickMenuEx.SelectedUserLocal.field_Private_IUser_0;
                if (user == null) {
                    Con.Error("Selected User could not be found");
                    return;
                }

                var player = PlayerManager.field_Private_Static_PlayerManager_0.GetPlayer(user.prop_String_0);
                if (player == null) {
                    Con.Error("Player could not be found");
                    return;
                }
                
                Items.TPToPlayer(player);
            }, MintyResources.marker);
            items.Active = ServerAuth.HasSpecialPermissions;

            if (!ModCompatibility.GPrivateServer) {
                if (APIUser.CurrentUser.id.StartsWith("usr_6d71d3be")) {
                    _userSelectCategory.AddButton("Mint Auth Check", "Check to see if the selected user can use MintMod", () => MelonCoroutines.Start(ServerAuth.SimpleAuthCheck(PlayerWrappers.GetSelectedAPIUser().id)), MintyResources.key);
                }
            }

            Con.Debug("Done Setting up User Selected Menu", MintCore.IsDebug);
        }
}