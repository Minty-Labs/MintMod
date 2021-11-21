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
using MintyLoader;
using UnityEngine.Playables;
using static MintMod.UserInterface.AvatarFavs.AviFavSetup;
using UnityEngine.UI;
using static MintMod.Managers.Colors;

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
            Intance = this;
            try {
                //Favorites.Instance = Favorites.Load();
                Favorites.CreateAviFavJSONFile();
            }
            catch (Exception e) {
               Con.Error($"Avatar Favs Failed to load\n{e}");
                AviFavsErrored = true;
            }
        }

        internal override void OnUserInterface() {
            if (AviFavsErrored) return;
            if (ranOnce) return;
            if (!Config.AviFavsEnabled.Value) {
                Con.Msg("Extended Avatar Favoriting has been disabled.");
                return;
            }
            if (!Config.AviFavsEnabled.Value) return;
            Con.Debug("Starting Minty Favorites (Avatar Favorites)", MintCore.isDebug);
            avatarPage = GameObject.Find("UserInterface/MenuContent/Screens/Avatar");
            PublicAvatarList = GameObject.Find("/UserInterface/MenuContent/Screens/Avatar/Vertical Scroll View/Viewport/Content/Public Avatar List");
            currPageAvatar = avatarPage.GetComponent<PageAvatar>();
            if (!Config.AviFavsEnabled.Value) return;
            if (Favorites.Instance.AvatarFavorites.FavoriteLists.Count == 0)
                AddNewList();
            else
                LoadList();

            Con.Debug("Finished Minty Favorites", MintCore.isDebug);
            ranOnce = true;
        }

        internal static AviFavLogic Intance;

        internal override void OnUpdate() {
            if (AviFavsErrored) return;
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

        //private static int ListAvatarCount;

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
                    list2.Text.supportRichText = true;
                    list2.Text.text = $"{(list.name.Contains("<color=") ? $"{list.name}" : $"<color=#9fffe3>{list.name}</color>")} (<color=yellow>{AvatarList.Count}</color>)";
                    /*list.Avatars.ForEach(a => {
                        ListAvatarCount++;
                        if (ListAvatarCount > list.Avatars.Count) return;
                        var s = a.supportedPlatforms;
                        int i = a.supportedPlatforms.Length;
                        if (i >= 2) {
                            var g = list2.UiVRCList.transform.Find($"ViewPort/Content/AvatarUiPrefab2(Clone) {ListAvatarCount}/RoomImageShape/OverlayIcons/MobileIcons").gameObject;
                            g.transform.Find("iconUploaded").gameObject?.Destroy();
                            g.transform.Find("IconPlatformPC").gameObject?.SetActive(false);
                            g.transform.Find("IconPlatformAny").gameObject?.SetActive(true);
                            g.transform.Find("IconPlatformAny").gameObject.GetComponent<RectTransform>().localPosition = new Vector3(113.9f, 73, -0.3f);
                        } else if (i == 1 && s.ToLower().Contains("quest")) {
                            var g = list2.UiVRCList.transform.Find($"ViewPort/Content/AvatarUiPrefab2(Clone) {ListAvatarCount}/RoomImageShape/OverlayIcons/MobileIcons").gameObject;
                            g.transform.Find("iconUploaded").gameObject?.Destroy();
                            g.transform.Find("IconPlatformPC").gameObject?.SetActive(false);
                            g.transform.Find("IconPlatformMobile").gameObject?.SetActive(true);
                            g.transform.Find("IconPlatformMobile").gameObject.GetComponent<RectTransform>().localPosition = new Vector3(113.9f, 73, -0.3f);
                        }
                    });
                    */
                }
            }
            yield break;
        }

        private static void LoadList() {
            foreach (var list in Favorites.Instance.AvatarFavorites.FavoriteLists) {
                if (!FavlistDictonary.ContainsKey(list.ID)) {
                    var newlist = new VRCList(PublicAvatarList.transform.parent, list.name, list.ID);
                    var listofbuttons = new List<MenuButton>();
                    listofbuttons.Add(new MenuButton(newlist.UiVRCList.expandButton.gameObject.transform, MenuButtonType.AvatarFavButton, "Fav/UnFav", 930, 0, () => {
                        if (!list.Avatars.Exists(avi => avi.id == currPageAvatar.field_Public_SimpleAvatarPedestal_0.field_Internal_ApiAvatar_0.id))
                            FavoriteAvatar(currPageAvatar.field_Public_SimpleAvatarPedestal_0.field_Internal_ApiAvatar_0, list.ID);
                        else
                            UnfavoriteAvatar(currPageAvatar.field_Public_SimpleAvatarPedestal_0.field_Internal_ApiAvatar_0, list.ID);
                    }, 2, 1));
                    //listofbuttons.ForEach(b => b.SetActive(false));
                    //newlist.UiVRCList.expandButton.onClick.AddListener(new Action(() => listofbuttons.ForEach(b => b.SetActive(!b.Button.activeSelf))));
                    listofbuttons.ForEach(b => {
                        b.SetActive(true);
                        b.Button.gameObject.GetComponent<Button>().colors = ColorBlock(Minty);
                    });
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
                name = "Minty Favorites"
            });
            LoadList();
        }

        internal static void DestroyList() {
            //var ConfigList = Favorites.Instance.AvatarFavorites.FavoriteLists.Where(list => list.ID == 0).FirstOrDefault();
            var ConfigList = Favorites.Instance.AvatarFavorites.FavoriteLists.Single(list => list.ID == 0);
            var AvatarList = FavlistDictonary[0];
            if (ConfigList != null && AvatarList != null)
                AvatarList.GameObject.Destroy();
        }

        internal static void FavoriteAvatar(ApiAvatar avatar, int ListID) {
            var avatarobject = new AvatarObject(avatar);
            if (GetConfigList(ListID) != null) {
                if (!GetConfigList(ListID).Avatars.Exists(avi => avi.id == avatar.id)) {
                    GetConfigList(ListID).Avatars.Insert(0, avatarobject);
                    if (Config.AviLogFavOrUnfavInConsole.Value)
                        Con.Msg($"Favorited {avatarobject.name} into Minty Favorites");
                }
            }
            MelonCoroutines.Start(RefreshMenu(1f));
            Favorites.Instance.SaveConfig();
        }

        internal static void UnfavoriteAvatar(ApiAvatar avatar, int ListID) {
            if (GetConfigList(ListID) != null) {
                GetConfigList(ListID).Avatars.Remove(GetConfigList(ListID).Avatars.Single(avi => avi.id == avatar.id));
                //GetConfigList(ListID).Avatars.Remove(GetConfigList(ListID).Avatars.Where(avi => avi.id == avatar.id).FirstOrDefault());
                if (Config.AviLogFavOrUnfavInConsole.Value)
                    Con.Msg($"Removed {avatar.name} from Minty Favorites");
            }
            MelonCoroutines.Start(RefreshMenu(1f));
            Favorites.Instance.SaveConfig();
        }

        private static FavoriteList GetConfigList(int ID) {
            return Favorites.Instance.AvatarFavorites.FavoriteLists.Single(List => List.ID == ID);
            //return Favorites.Instance.AvatarFavorites.FavoriteLists.Where(List => List.ID == ID).FirstOrDefault();
        }

        private static VRCList GetVRCList(int ID) => FavlistDictonary[ID];
    }
}
