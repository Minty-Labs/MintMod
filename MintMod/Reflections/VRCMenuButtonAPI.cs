using System;
using UnhollowerBaseLib;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MintMod.Reflections.VRCAPI {
    internal class MenuButton {
        public MenuButton(MenuType type, MenuButtonType buttontype, string text, float x_pos, float y_pos, Action listener) {
            try {
                SettingsPage = GameObject.Find("UserInterface/MenuContent/Screens/Settings");
                SocialPage = GameObject.Find("UserInterface/MenuContent/Screens/Social");
                UserInfoPage = GameObject.Find("UserInterface/MenuContent/Screens/UserInfo");
                AvatarPage = GameObject.Find("UserInterface/MenuContent/Screens/Avatar");
                WorldsPage = GameObject.Find("UserInterface/MenuContent/Screens/Worlds");
            } catch (Exception) {
                throw;
            }
            try {
                switch (buttontype) {
                    case MenuButtonType.PlaylistButton:
                        GameObject original = GameObject.Find("MenuContent/Screens/UserInfo/User Panel/Playlists/PlaylistsButton");
                        Button = UnityEngine.Object.Instantiate(original, original.transform);
                        break;

                    case MenuButtonType.AvatarFavButton:
                        GameObject gameObject = GameObject.Find("/UserInterface/MenuContent/Screens/Avatar/Favorite Button");
                        Button = UnityEngine.Object.Instantiate(gameObject, gameObject.transform.parent);
                        break;

                    case MenuButtonType.HeaderButton:
                        GameObject gameObject1 = GameObject.Find("UserInterface/MenuContent/Backdrop/Header/Tabs/ViewPort/Content/WorldsPageTab");
                        Button = UnityEngine.Object.Instantiate(gameObject1, gameObject1.transform.parent);
                        break;

                    default:
                        GameObject gameObject2 = GameObject.Find("/UserInterface/MenuContent/Screens/Avatar/Favorite Button");
                        Button = UnityEngine.Object.Instantiate(gameObject2, gameObject2.transform.parent);
                        break;
                }
            } catch (Exception) {
                throw;
            }
            try {
                switch (type) {
                    case MenuType.UserInfo:
                        Button.transform.SetParent(UserInfoPage.transform);
                        break;

                    case MenuType.AvatarMenu:
                        Button.transform.SetParent(AvatarPage.transform);
                        break;

                    case MenuType.SettingsMenu:
                        Button.transform.SetParent(SettingsPage.transform);
                        break;

                    case MenuType.SocialMenu:
                        Button.transform.SetParent(SocialPage.transform);
                        break;

                    case MenuType.WorldMenu:
                        Button.transform.SetParent(WorldsPage.transform);
                        break;

                    default:
                        Button.transform.SetParent(UserInfoPage.transform);
                        break;
                }
            } catch (Exception) {
                throw;
            }

            // this.Button.GetComponentInChildren<Image>().gameObject.active = false;
            foreach (Text text2 in Button.GetComponentsInChildren<Text>(true)) {
                text2.text = "";
            }
            Button.GetComponentInChildren<Text>().text = text;
            //Button.name = Loader.rm + Loader.rm + text;
            Button.GetComponentInChildren<Button>().onClick = new Button.ButtonClickedEvent();
            Button.GetComponentInChildren<Button>().onClick.AddListener(listener);
            Button.GetComponentInChildren<Button>().m_Interactable = true;
            Button.GetComponent<RectTransform>().anchoredPosition = new Vector2(x_pos, y_pos);
            Button.name = $"MenuButton_{text}_{x_pos}_{y_pos}";
            Button.SetActive(true);
        }

        public MenuButton(MenuType type, MenuButtonType buttontype, string text, float x_pos, float y_pos, Action listener, float xSize, float ySize) {
            try {
                SettingsPage = GameObject.Find("UserInterface/MenuContent/Screens/Settings");
                SocialPage = GameObject.Find("UserInterface/MenuContent/Screens/Social");
                UserInfoPage = GameObject.Find("UserInterface/MenuContent/Screens/UserInfo");
                AvatarPage = GameObject.Find("UserInterface/MenuContent/Screens/Avatar");
                WorldsPage = GameObject.Find("UserInterface/MenuContent/Screens/Worlds");
            } catch (Exception) {
                throw;
            }
            try {
                switch (buttontype) {
                    case MenuButtonType.PlaylistButton:
                        GameObject original = GameObject.Find("MenuContent/Screens/UserInfo/User Panel/Playlists/PlaylistsButton");
                        Button = UnityEngine.Object.Instantiate(original, original.transform);
                        break;

                    case MenuButtonType.AvatarFavButton:
                        GameObject gameObject = GameObject.Find("/UserInterface/MenuContent/Screens/Avatar/Favorite Button");
                        Button = UnityEngine.Object.Instantiate(gameObject, gameObject.transform.parent);
                        break;

                    case MenuButtonType.HeaderButton:
                        GameObject gameObject1 = GameObject.Find("UserInterface/MenuContent/Backdrop/Header/Tabs/ViewPort/Content/WorldsPageTab");
                        Button = UnityEngine.Object.Instantiate(gameObject1, gameObject1.transform.parent);
                        break;

                    default:
                        GameObject gameObject2 = GameObject.Find("/UserInterface/MenuContent/Screens/Avatar/Favorite Button");
                        Button = UnityEngine.Object.Instantiate(gameObject2, gameObject2.transform.parent);
                        break;
                }
            } catch (Exception) {
                throw;
            }
            try {
                switch (type) {
                    case MenuType.UserInfo:
                        Button.transform.SetParent(UserInfoPage.transform);
                        break;

                    case MenuType.AvatarMenu:
                        Button.transform.SetParent(AvatarPage.transform);
                        break;

                    case MenuType.SettingsMenu:
                        Button.transform.SetParent(SettingsPage.transform);
                        break;

                    case MenuType.SocialMenu:
                        Button.transform.SetParent(SocialPage.transform);
                        break;

                    case MenuType.WorldMenu:
                        Button.transform.SetParent(WorldsPage.transform);
                        break;

                    default:
                        Button.transform.SetParent(UserInfoPage.transform);
                        break;
                }
            } catch (Exception) {
                throw;
            }

            // this.Button.GetComponentInChildren<Image>().gameObject.active = false;
            foreach (Text text2 in Button.GetComponentsInChildren<Text>(true)) {
                text2.text = "";
            }
            Button.GetComponentInChildren<Text>().text = text;
            //Button.name = Loader.rm + Loader.rm + text;
            Button.GetComponentInChildren<Button>().onClick = new Button.ButtonClickedEvent();
            Button.GetComponentInChildren<Button>().onClick.AddListener(listener);
            Button.GetComponentInChildren<Button>().m_Interactable = true;
            Button.GetComponent<RectTransform>().anchoredPosition = new Vector2(x_pos, y_pos);
            Button.GetComponent<RectTransform>().sizeDelta /= new Vector2(xSize, ySize);
            Button.name = $"MenuButton_{text}_{x_pos}_{y_pos}";
            Button.SetActive(true);
        }

        public MenuButton(Transform Parent, MenuButtonType buttontype, string text, float x_pos, float y_pos, UnityAction listener) {
            try {
                switch (buttontype) {
                    case MenuButtonType.PlaylistButton:
                        GameObject original = GameObject.Find("MenuContent/Screens/UserInfo/User Panel/Playlists/PlaylistsButton");
                        Button = UnityEngine.Object.Instantiate(original, original.transform);
                        break;

                    case MenuButtonType.AvatarFavButton:
                        GameObject gameObject = GameObject.Find("/UserInterface/MenuContent/Screens/Avatar/Favorite Button");
                        Button = UnityEngine.Object.Instantiate(gameObject, gameObject.transform.parent);
                        break;

                    case MenuButtonType.HeaderButton:
                        GameObject gameObject1 = GameObject.Find("UserInterface/MenuContent/Backdrop/Header/Tabs/ViewPort/Content/WorldsPageTab");
                        Button = UnityEngine.Object.Instantiate(gameObject1, gameObject1.transform.parent);
                        break;

                    default:
                        GameObject gameObject2 = GameObject.Find("/UserInterface/MenuContent/Screens/Avatar/Favorite Button");
                        Button = UnityEngine.Object.Instantiate(gameObject2, gameObject2.transform.parent);
                        break;
                }
                Button.transform.SetParent(Parent);
                //Button.name = Loader.rm + text + Loader.rm; //Better than Favorite Button(Clone) all the time but no one sees this.
                foreach (Text text2 in Button.GetComponentsInChildren<Text>(true)) {
                    text2.text = "";
                }
                Button.GetComponentInChildren<Text>().text = text;
                Button.GetComponentInChildren<Button>().onClick = new Button.ButtonClickedEvent();
                Button.GetComponentInChildren<Button>().onClick.AddListener(listener);
                Button.GetComponentInChildren<Button>().m_Interactable = true;
                Button.GetComponent<RectTransform>().anchoredPosition = new Vector2(x_pos, y_pos);
                Button.name = $"MenuButton_{text}_{x_pos}_{y_pos}";
            } catch (Exception) {
                throw;
            }
            Button.SetActive(true);
        }

        public MenuButton(Transform Parent, MenuButtonType buttontype, string text, float x_pos, float y_pos, Action listener, float xSize, float ySize) {
            try {
                switch (buttontype) {
                    case MenuButtonType.PlaylistButton:
                        GameObject original = GameObject.Find("MenuContent/Screens/UserInfo/User Panel/Playlists/PlaylistsButton");
                        Button = UnityEngine.Object.Instantiate(original, original.transform);
                        break;

                    case MenuButtonType.AvatarFavButton:
                        GameObject gameObject = GameObject.Find("/UserInterface/MenuContent/Screens/Avatar/Favorite Button");
                        Button = UnityEngine.Object.Instantiate(gameObject, gameObject.transform.parent);
                        break;

                    case MenuButtonType.HeaderButton:
                        GameObject gameObject1 = GameObject.Find("UserInterface/MenuContent/Backdrop/Header/Tabs/ViewPort/Content/WorldsPageTab");
                        Button = UnityEngine.Object.Instantiate(gameObject1, gameObject1.transform.parent);
                        break;

                    default:
                        GameObject gameObject2 = GameObject.Find("/UserInterface/MenuContent/Screens/Avatar/Favorite Button");
                        Button = UnityEngine.Object.Instantiate(gameObject2, gameObject2.transform.parent);
                        break;
                }
                Button.transform.SetParent(Parent);
                foreach (Text text2 in Button.GetComponentsInChildren<Text>(true)) {
                    text2.text = "";
                }
                Button.GetComponentInChildren<Text>().text = text;
                Button.GetComponentInChildren<Button>().onClick = new Button.ButtonClickedEvent();
                Button.GetComponentInChildren<Button>().onClick.AddListener(listener);
                Button.GetComponentInChildren<Button>().m_Interactable = true;
                Button.GetComponent<RectTransform>().anchoredPosition = new Vector2(x_pos, y_pos);
                Il2CppReferenceArray<Component> componentsInChildren = Button.GetComponentsInChildren(Il2CppType.Of<Image>());
                Button.GetComponent<RectTransform>().sizeDelta /= new Vector2(xSize, ySize);
                //Button.name = Loader.rm + text + Loader.rm; //Better than Favorite Button(Clone) all the time but no one sees this.
            } catch (Exception) {
                throw;
            }
            Button.SetActive(true);
        }

        public MenuButton(Transform Parent, MenuButtonType buttontype, string text, float x_pos, float y_pos, Action listener) {
            try {
                switch (buttontype) {
                    case MenuButtonType.PlaylistButton:
                        GameObject original = GameObject.Find("MenuContent/Screens/UserInfo/User Panel/Playlists/PlaylistsButton");
                        Button = UnityEngine.Object.Instantiate(original, original.transform);
                        break;

                    case MenuButtonType.AvatarFavButton:
                        GameObject gameObject = GameObject.Find("/UserInterface/MenuContent/Screens/Avatar/Favorite Button");
                        Button = UnityEngine.Object.Instantiate(gameObject, gameObject.transform.parent);
                        break;

                    case MenuButtonType.HeaderButton:
                        GameObject gameObject1 = GameObject.Find("UserInterface/MenuContent/Backdrop/Header/Tabs/ViewPort/Content/WorldsPageTab");
                        Button = UnityEngine.Object.Instantiate(gameObject1, gameObject1.transform.parent);
                        break;

                    default:
                        GameObject gameObject2 = GameObject.Find("/UserInterface/MenuContent/Screens/Avatar/Favorite Button");
                        Button = UnityEngine.Object.Instantiate(gameObject2, gameObject2.transform.parent);
                        break;
                }
                Button.transform.SetParent(Parent);
                //Button.name = Loader.rm + text + Loader.rm; //Better than Favorite Button(Clone) all the time but no one sees this.
                foreach (Text text2 in Button.GetComponentsInChildren<Text>(true)) {
                    text2.text = "";
                }
                Button.GetComponentInChildren<Text>().text = text;
                Button.GetComponentInChildren<Button>().onClick = new Button.ButtonClickedEvent();
                Button.GetComponentInChildren<Button>().onClick.AddListener(listener);
                Button.GetComponentInChildren<Button>().m_Interactable = true;
                Button.GetComponent<RectTransform>().anchoredPosition = new Vector2(x_pos, y_pos);
                Button.name = $"MenuButton_{text}_{x_pos}_{y_pos}";
            } catch (Exception) {
                throw;
            }
            Button.SetActive(true);
        }

        public void SetPos(float x, float y) {
            Button.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
        }

        public void SetText(string Text) {
            Button.GetComponentInChildren<Text>().text = Text;
        }

        public void SetAction(UnityAction listener) {
            Button.GetComponentInChildren<Button>().onClick = new Button.ButtonClickedEvent();
            Button.GetComponentInChildren<Button>().onClick.AddListener(listener);
        }

        public void Delete() {
            UnityEngine.Object.Destroy(Button);
        }

        public void SetActive(bool value) {
            Button.SetActive(value);
        }

        public void SetBackgroundColor(Color color) {
            Button.GetComponentInChildren<Image>().color = color;
        }

        public void SetTextColor(Color color) {
            Button.GetComponentInChildren<Text>().color = color;
        }

        public void SetInteractable(bool result) {
            //if (Button.GetComponentInChildren<Image>().color != Color.gray)
            //     SavedColor = Button.GetComponentInChildren<Image>().color;
            //if (result)
            //{
            //    SetBackgroundColor(Color.gray);
            //}
            //else
            //{
            //    if(SavedColor != null)
            //        SetBackgroundColor(SavedColor);
            //}
            Button.GetComponentInChildren<Button>().interactable = result;
        }
        private Color SavedColor;
        public GameObject Button;
        public GameObject UserInfoPage;
        public GameObject AvatarPage;
        public GameObject SettingsPage;
        public GameObject SocialPage;
        public GameObject WorldsPage;
    }
}
