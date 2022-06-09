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
        private static UiAvatarList _avatarList;
        private static Text _onlineFriendsText, _avatarsText, _inRoomText;
        private static bool _hasLoadedOnUi, _hasOpenedSocialMenu, _hasOpenedAvatarMenu;

        private static IEnumerator UpdateMembersText(Text textObj, UiUserList online, int total) {
            yield return new WaitForSeconds(1);
        
            textObj.text = $"Online Friends ({online.field_Private_Int32_0}/{total})";
        }
    
        private static IEnumerator UpdateInRoomText(Text textObj) {
            yield return new WaitForSeconds(1);
        
            textObj.text = $"In Room ({PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0.Count})";
        }
    
        private static IEnumerator UpdateAvatarsText(Text textObj, UiAvatarList total) {
            yield return new WaitForSeconds(1);
        
            textObj.text = $"Personal Creations ({total.field_Private_Int32_0})";
        }

        public static void OnOpenSocialMenu() {
            MelonCoroutines.Start(UpdateMembersText(_onlineFriendsText, _onlineFriendsList, _totalFriends));
            MelonCoroutines.Start(UpdateInRoomText(_inRoomText));
            _hasOpenedSocialMenu = true;
        }
        
        public static void OnCloseSocialMenu() => _hasOpenedSocialMenu = false;
    
        public static void OnOpenAvatarMenu() {
            MelonCoroutines.Start(UpdateAvatarsText(_avatarsText, _avatarList));
            _hasOpenedAvatarMenu = true;
        }
    
        public static void OnCloseAvatarMenu() => _hasOpenedAvatarMenu = false;

        internal override void OnUserInterface() {
            _socialMenuAvatarBorder = GameObject.Find("/UserInterface/MenuContent/Screens/UserInfo/AvatarImage/AvatarBorder").gameObject;
            _socialMenuAvatarBorder.GetComponent<Image>().color = ColorConversion.HexToColor("#00FFAA");
            
            #region Online / Total Friends
            
            if (ModCompatibility.ListCounter) return;
            
            var onlineFriendsViewport = GameObject.Find("UserInterface/MenuContent/Screens/Social/Vertical Scroll View/Viewport/Content/OnlineFriends");
            var friendsListTextObj = GameObject.Find("UserInterface/MenuContent/Screens/Social/Vertical Scroll View/Viewport/Content/OnlineFriends/Button/TitleText");
            _totalFriends = APIUser.CurrentUser.friendIDs._size;
            _onlineFriendsList = onlineFriendsViewport.GetComponent<UiUserList>();
            _onlineFriendsText = friendsListTextObj.GetComponent<Text>();
            Con.Debug("Got Friends List");
        
            var inRoomListTextObj = GameObject.Find("UserInterface/MenuContent/Screens/Social/Vertical Scroll View/Viewport/Content/InRoom/Button/TitleText");
            _inRoomText = inRoomListTextObj.GetComponent<Text>();
            Con.Debug("Got In Room List");
        
            var avatarsList = GameObject.Find("UserInterface/MenuContent/Screens/Avatar/Vertical Scroll View/Viewport/Content/Personal Avatar List");
            var avatarCreationTextObj = GameObject.Find("UserInterface/MenuContent/Screens/Avatar/Vertical Scroll View/Viewport/Content/Personal Avatar List/Button/TitleText");
            _avatarList = avatarsList.GetComponent<UiAvatarList>();
            _avatarsText = avatarCreationTextObj.GetComponent<Text>();
            Con.Debug("Got Avatar List");
        
            _hasLoadedOnUi = true;

            #endregion
        }
    }
}
