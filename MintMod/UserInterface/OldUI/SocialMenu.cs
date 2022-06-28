using MintMod.Libraries;
using System.Collections;
using MelonLoader;
using MintyLoader;
using UnityEngine;
using UnityEngine.UI;
using VRC;
using VRC.Core;

namespace MintMod.UserInterface.OldUI {
    internal class SocialMenu : MintSubMod {
        public override string Name => "Social Menu";
        public override string Description => "Edits on the Social Menu";

        private GameObject _socialMenuAvatarBorder;
        private static int _totalFriends, _scenesLoaded;
        private static UiUserList _onlineFriendsList;
        private static Text _onlineFriendsText, _inRoomText;
        private static bool _hasLoadedOnUi, _hasOpenedSocialMenu;

        private static IEnumerator UpdateMembersText(Text textObj, UiUserList online, int total) {
            yield return new WaitForSeconds(1);
        
            textObj.text = $"Online Friends ({online.field_Private_Int32_0}/{total})";
        }
    
        private static IEnumerator UpdateInRoomText(Text textObj) {
            yield return new WaitForSeconds(1);
        
            textObj.text = $"In Room ({PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0.Count})";
        }

        public static void OnOpenSocialMenu() {
            MelonCoroutines.Start(UpdateMembersText(_onlineFriendsText, _onlineFriendsList, _totalFriends));
            MelonCoroutines.Start(UpdateInRoomText(_inRoomText));
            _hasOpenedSocialMenu = true;
        }
        
        public static void OnCloseSocialMenu() => _hasOpenedSocialMenu = false;

        internal override void OnUserInterface() {
            _socialMenuAvatarBorder = GameObject.Find("/UserInterface/MenuContent/Screens/UserInfo/AvatarImage/AvatarBorder").gameObject;
            _socialMenuAvatarBorder.GetComponent<Image>().color = ColorConversion.HexToColor("#00FFAA");
            
            #region Online / Total Friends
            
            if (MelonHandler.Mods.FindIndex(i => i.Info.Name == "ListCounter") != -1) return;
            
            var onlineFriendsViewport = GameObject.Find("UserInterface/MenuContent/Screens/Social/Vertical Scroll View/Viewport/Content/OnlineFriends");
            var friendsListTextObj = GameObject.Find("UserInterface/MenuContent/Screens/Social/Vertical Scroll View/Viewport/Content/OnlineFriends/Button/TitleText");
            _totalFriends = APIUser.CurrentUser.friendIDs._size;
            _onlineFriendsList = onlineFriendsViewport.GetComponent<UiUserList>();
            _onlineFriendsText = friendsListTextObj.GetComponent<Text>();
            Con.Debug("Got Friends List");
        
            var inRoomListTextObj = GameObject.Find("UserInterface/MenuContent/Screens/Social/Vertical Scroll View/Viewport/Content/InRoom/Button/TitleText");
            _inRoomText = inRoomListTextObj.GetComponent<Text>();
            Con.Debug("Got In Room List");
        
            _hasLoadedOnUi = true;

            #endregion
        }
    }
}
