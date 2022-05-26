using System;
using System.Linq;
using MintMod.Libraries;
using MintMod.Managers;
using MintMod.Reflections.VRCAPI;
using MintyLoader;
using ReMod.Core.UI;
using ReMod.Core.VRChat;
using UnityEngine;
using UnityEngine.UI;
using VRC.Core;
using VRC.SDKBase.Validation.Performance.Stats;
using AvatarList = Il2CppSystem.Collections.Generic.List<VRC.Core.ApiAvatar>;

namespace MintMod.UserInterface.AvatarFavs {
    internal class ReFavs : MintSubMod, IAvatarListOwner {
        public override string Name => "Avatar Favorites";
        public override string Description => "Complete Rewrite of avatar favoriting";
        
        internal static ReFavs _instance;
        private bool _initStart, _ranOnce;

        internal ReAvatarList _favoriteAvatarList;
        internal ReUiButton _favoriteButton;
        private Button.ButtonClickedEvent _changeButtonEvent;

        internal override void OnStart() => AviFavLogic.OnAppStart();

        internal override void OnUserInterface() {
            _instance = this;
            if (AviFavLogic.AviFavsErrored) return;
            if (ModCompatibility.GPrivateServer) {
                Con.Msg("Extended Avatar Favoriting has been disabled.");
                return;
            }
            Con.Debug("Starting Minty Favorites (Avatar Favorites)", MintCore.IsDebug);
            
            _favoriteAvatarList = new ReAvatarList($"<color=#{(Config.ColorGameMenu.Value ? Config.MenuColorHEX.Value : Colors.defaultMenuColor())}>Minty Favorites</color>",
                    this, false);
            //_favoriteAvatarList.AvatarPedestal.field_Internal_Action_3_String_GameObject_AvatarPerformanceStats_0 =
            _favoriteAvatarList.AvatarPedestal.field_Internal_Action_4_String_GameObject_AvatarPerformanceStats_ObjectPublicBoBoBoBoBoBoBoBoBoBoUnique_0 =
                new Action<string, GameObject, AvatarPerformanceStats, ObjectPublicBoBoBoBoBoBoBoBoBoBoUnique>
                    (OnAvatarInstantiated);
            _favoriteAvatarList.OnEnable += () => _favoriteAvatarList.GameObject.SetActive(Config.AviFavsEnabled.Value);
            
            var parent = GameObject.Find("UserInterface/MenuContent/Screens/Avatar/Favorite Button").transform.parent;
            _favoriteButton = new ReUiButton("Favorite", new Vector2(200f, 375f), new Vector2(0.5f, 1f),
                () => FavoriteAvatar(_favoriteAvatarList.AvatarPedestal.field_Internal_ApiAvatar_0), parent);
            _favoriteButton.GameObject.SetActive(Config.AviFavsEnabled.Value);
            
            var changeButton = GameObject.Find("UserInterface/MenuContent/Screens/Avatar/Change Button");
            if (changeButton != null) {
                var button = changeButton.GetComponent<Button>();
                _changeButtonEvent = button.onClick;

                button.onClick = new Button.ButtonClickedEvent();
                button.onClick.AddListener(new Action(ChangeAvatarChecked));
            }
            _favoriteAvatarList.RefreshAvatars();
            _ranOnce = true;
            _initStart = true;
        }

        internal override void OnPrefSave() {
            if (!_initStart) return;
            if (!Config.AviFavsEnabled.Value) return;
            try {
                if (!_ranOnce)
                    _instance.OnUserInterface();
            } catch (Exception a) { Con.Error($"After game start, Avatar Favorites Start Error\n{a}"); }
            _favoriteAvatarList.RefreshAvatars();
        }

        internal void FavoriteAvatar(ApiAvatar apiAvatar) {
            if (_favoriteAvatarList.AvatarPedestal.field_Internal_ApiAvatar_0.id == apiAvatar.id) {
                if (!HasAvatarFavorited(apiAvatar.id)) {
                    AviFavLogic.GetConfigList(0).Avatars.Insert(0, new AvatarObject(apiAvatar));
                    _favoriteButton.Text = "<color=#fd4544>Unfavorite</color>";
                    if (Config.AviLogFavOrUnfavInConsole.Value)
                        Con.Msg($"Favorited {apiAvatar.name} into Minty Favorites");
                }
                else {
                    AviFavLogic.GetConfigList(0).Avatars.Remove(AviFavLogic.GetConfigList(0).Avatars.Single(avi => avi.id == apiAvatar.id));
                    _favoriteButton.Text = "<color=#9fffe3>Favorite</color>";
                    if (Config.AviLogFavOrUnfavInConsole.Value)
                        Con.Msg($"Removed {apiAvatar.name} from Minty Favorites");
                }
            }
            _favoriteAvatarList.RefreshAvatars();
            AviFavSetup.Favorites.Instance.SaveConfig();
        }
        
        private void ChangeAvatarChecked() {
            if (!_ranOnce) return;
            if (!Config.AviFavsEnabled.Value) {
                _changeButtonEvent.Invoke();
                return;
            }

            var currentAvatar = _favoriteAvatarList.AvatarPedestal.field_Internal_ApiAvatar_0;
            if (!HasAvatarFavorited(currentAvatar.id)) { // this isn't in our list. we don't care about it
                _changeButtonEvent.Invoke();
                return;
            }

            new ApiAvatar { id = currentAvatar.id }.Fetch(new Action<ApiContainer>(ac => {
                var updatedAvatar = ac.Model.Cast<ApiAvatar>();
                switch (updatedAvatar.releaseStatus) {
                    case "private" when updatedAvatar.authorId != APIUser.CurrentUser.id:
                        VRCUiPopupManager.prop_VRCUiPopupManager_0.ShowAlert("MintMod", "This avatar is private and you don't own it. You can't switch into it.");
                        break;
                    case "unavailable":
                        VRCUiPopupManager.prop_VRCUiPopupManager_0.ShowAlert("MintMod", "This avatar has been deleted. You can't switch into it.");
                        break;
                    default:
                        _changeButtonEvent.Invoke();
                        break;
                }
            }), new Action<ApiContainer>(ac => {
                VRCUiPopupManager.prop_VRCUiPopupManager_0.ShowAlert("MintMod", "This avatar has been deleted. You can't switch into it.");
            }));
        }

        private bool HasAvatarFavorited(string id) => AviFavLogic.GetConfigList(0).Avatars.FirstOrDefault(a => a.id == id) != null;

        private void OnAvatarInstantiated(string url, GameObject avatar, AvatarPerformanceStats avatarPerformanceStats,
            ObjectPublicBoBoBoBoBoBoBoBoBoBoUnique unknown) =>
            _favoriteButton.Text = HasAvatarFavorited(_favoriteAvatarList.AvatarPedestal.field_Internal_ApiAvatar_0.id) ? 
                "<color=#fd4544>Unfavorite</color>" : "<color=#9fffe3>Favorite</color>";

        public AvatarList GetAvatars(ReAvatarList avatarList) {
            if (avatarList != _favoriteAvatarList) return null;
            var list = new AvatarList();
            foreach (var avi in AviFavLogic.GetConfigList(0).Avatars.Select(x => x.ToApiAvatar()).ToList()) 
                list.Add(avi);
            return list;
        }

        public void Clear(ReAvatarList avatarList) => avatarList.RefreshAvatars();
    }
}