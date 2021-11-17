using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRC.UI;
using MelonLoader;
using System.Collections;
using VRC.Core;
using MintMod.Reflections;
using MintMod.Reflections.VRCAPI;
using static MintMod.UserInterface.AvatarFavs.AviFavSetup;

namespace MintMod.UserInterface.AvatarFavs {
    internal class AviFavLogic : MintSubMod {
        public override string Name => "MintyFavorties";
        public override string Description => "Avatar Favorites.";
        internal static Dictionary<int, VRCList> FavlistDictonary = new Dictionary<int, VRCList>();
        private static bool JustOpened, ranOnce;
        private static GameObject avatarPage;
        public static PageAvatar currPageAvatar;
        private static GameObject PublicAvatarList;
        internal static bool AviFavsErrored;

        internal override void OnStart() {
            try {
                Favorites.Instance = Favorites.Load();
            }
            catch (Exception e) {
               MelonLogger.Error($"Avatar Favs Failed to load\n{e}");
                AviFavsErrored = true;
            }
        }

        internal override void OnUserInterface() {
            if (!AviFavsErrored) return;
            if (!ranOnce) return;
            if (!Config.AviFavsEnabled.Value) {
                MelonLogger.Msg("Extended Avatar Favoriting has been disabled.");
                return;
            }
            if (!Config.AviFavsEnabled.Value) return;
            if (MintCore.isDebug)
                MelonLogger.Msg("Starting Minty Favorites (Avatar Favorites)");
            avatarPage = GameObject.Find("UserInterface/MenuContent/Screens/Avatar");
            PublicAvatarList = GameObject.Find("/UserInterface/MenuContent/Screens/Avatar/Vertical Scroll View/Viewport/Content/Public Avatar List");
            currPageAvatar = avatarPage.GetComponent<PageAvatar>();
            if (!Config.AviFavsEnabled.Value) return;
            if (Favorites.Instance.AvatarFavorites.FavoriteLists.Count == 0)
                AddNewList();
            else
                LoadList();

            if (MintCore.isDebug)
                MelonLogger.Msg("Finished Minty Favorites");
            ranOnce = true;
        }

        internal override void OnUpdate() {
            if (!AviFavsErrored) return;
            if (!Config.AviFavsEnabled.Value) return;
            if (!UIWrappers.IsInWorld()) return;
            try {
                if (avatarPage.activeSelf && !JustOpened) {
                    JustOpened = true;
                    MelonCoroutines.Start(RefreshMenu(1f));
                }

                if (!avatarPage.activeSelf && JustOpened)
                    JustOpened = false;
            } catch { }
        }

        public static IEnumerator RefreshMenu(float v) {
            if (!ranOnce)
                yield break;
            foreach (var list in Favorites.Instance.AvatarFavorites.FavoriteLists) {
                yield return new WaitForSeconds(v);
                var list2 = FavlistDictonary[list.ID];
                if (list2 != null) {
                    Il2CppSystem.Collections.Generic.List<ApiAvatar> AvatarList = new Il2CppSystem.Collections.Generic.List<ApiAvatar>();
                    list.Avatars.ForEach(avi => AvatarList.Add(avi.ToApiAvatar()));
                    list2.RenderElement(AvatarList);
                    list2.Text.text = $"{list.name} ({AvatarList.Count})";// {list.Desciption}";
                }
            }
            yield break;
        }

        private static void LoadList() {
            foreach (var list in Favorites.Instance.AvatarFavorites.FavoriteLists) {
                if (!FavlistDictonary.ContainsKey(list.ID)) {
                    var newlist = new VRCList(PublicAvatarList.transform.parent, list.name, list.ID);
                    var listofbuttons = new List<MenuButton>();
                    listofbuttons.Add(new MenuButton(newlist.UiVRCList.expandButton.gameObject.transform, MenuButtonType.AvatarFavButton, "Fav/UnFav", 600, 0, delegate {
                        if (!list.Avatars.Exists(avi => avi.id == currPageAvatar.field_Public_SimpleAvatarPedestal_0.field_Internal_ApiAvatar_0.id))
                            FavoriteAvatar(currPageAvatar.field_Public_SimpleAvatarPedestal_0.field_Internal_ApiAvatar_0, list.ID);
                        else
                            UnfavoriteAvatar(currPageAvatar.field_Public_SimpleAvatarPedestal_0.field_Internal_ApiAvatar_0, list.ID);
                    }, 2, 1));
                    listofbuttons.ForEach(b => b.SetActive(false));
                    newlist.UiVRCList.expandButton.onClick.AddListener(new Action(() => listofbuttons.ForEach(b => b.SetActive(!b.Button.activeSelf))));
                    newlist.UiVRCList.extendRows = list.Rows;
                    newlist.UiVRCList.expandedHeight += 300 * (list.Rows - 2);
                    FavlistDictonary.Add(list.ID, newlist);
                }
            }
            MelonCoroutines.Start(RefreshMenu(1f));
        }

        internal static void AddNewList() {
            var newID = Favorites.Instance.AvatarFavorites.FavoriteLists.Count;
            Favorites.Instance.AvatarFavorites.FavoriteLists.Add(new FavoriteList() {
                ID = newID,
                Avatars = new List<AvatarObject>(),
                Desciption = "",
                name = "Minty Favorites #" + newID
            });
            LoadList();
        }

        internal static void DestroyList() {
            var ConfigList = Favorites.Instance.AvatarFavorites.FavoriteLists.Where(list => list.ID == 0).FirstOrDefault();
            ;
            var AvatarList = FavlistDictonary[0];
            if (ConfigList != null && AvatarList != null)
                GameObject.Destroy(AvatarList.GameObject);
        }

        internal static void FavoriteAvatar(ApiAvatar avatar, int ListID) {
            var avatarobject = new AvatarObject(avatar);
            if (GetConfigList(ListID) != null) {
                if (!GetConfigList(ListID).Avatars.Exists(avi => avi.id == avatar.id)) {
                    GetConfigList(ListID).Avatars.Insert(0, avatarobject);
                    if (Config.AviLogFavOrUnfavInConsole.Value)
                        MelonLogger.Msg($"Favorited {avatarobject.name} into Minty Favorites");
                }
            }
            MelonCoroutines.Start(RefreshMenu(1f));
            Favorites.Instance.SaveConfig();
        }

        internal static void UnfavoriteAvatar(ApiAvatar avatar, int ListID) {
            if (GetConfigList(ListID) != null) {
                GetConfigList(ListID).Avatars.Remove(GetConfigList(ListID).Avatars.Where(avi => avi.id == avatar.id).FirstOrDefault());
                if (Config.AviLogFavOrUnfavInConsole.Value)
                    MelonLogger.Msg($"Removed {avatar.name} from Minty Favorites");
            }
            MelonCoroutines.Start(RefreshMenu(1f));
            Favorites.Instance.SaveConfig();
        }

        private static AviFavSetup.FavoriteList GetConfigList(int ID) {
            return Favorites.Instance.AvatarFavorites.FavoriteLists.Where(List => List.ID == ID).FirstOrDefault();
        }

        private static VRCList GetVRCList(int ID) {
            return FavlistDictonary[ID];
        }
    }
}
