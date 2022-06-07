using MintMod.Libraries;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using MintyLoader;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using VRC.Core;
using Task = Il2CppSystem.Threading.Tasks.Task;

namespace MintMod.UserInterface.OldUI {
    internal class SocialMenu : MintSubMod {
        public override string Name => "Social Menu";
        public override string Description => "Edits on the Social Menu";

        private GameObject _socialMenuAvatarBorder;
        private static int _totalFriends;
        private static UiUserList _onlineFriendsList;
        private static Text _onlineFriendsText;
        private static bool _hasLoadedOnUi;
        private static GameObject _onlineFriendsViewpoint, _friendsList;

        private static IEnumerator UpdateMembersText(Text textObj, UiUserList online, int total) {
            yield return new WaitForSeconds(1);
            textObj.text = $"Online Friends ({online.field_Private_Int32_0}/{total})";
        }

        private static bool _hasOpened;

        public static void OnOpen() {
            MelonCoroutines.Start(UpdateMembersText(_onlineFriendsText, _onlineFriendsList, _totalFriends));
            _hasOpened = true;
        }
        
        public static void OnClose() => _hasOpened = false;

        // internal override void OnUpdate() {
        //     if (!_hasOpened || !_hasLoadedOnUi) return;
        //     _onlineFriendsText.text = $"Online Friends ({_onlineFriendsList.field_Private_Int32_0}/{_totalFriends})";
        // }

        internal override void OnUserInterface() {
            _socialMenuAvatarBorder = GameObject.Find("/UserInterface/MenuContent/Screens/UserInfo/AvatarImage/AvatarBorder").gameObject;
            _socialMenuAvatarBorder.GetComponent<Image>().color = ColorConversion.HexToColor("#00FFAA");
            
            #region Online / Total Friends
            
            _friendsList = GameObject.Find("UserInterface/MenuContent/Screens/Social/Vertical Scroll View/Viewport/Content/OnlineFriends");
            _onlineFriendsViewpoint = GameObject.Find("UserInterface/MenuContent/Screens/Social/Vertical Scroll View/Viewport/Content/OnlineFriends/Button/TitleText");
            _totalFriends = APIUser.CurrentUser.friendIDs._size;
            _onlineFriendsList = _friendsList.GetComponent<UiUserList>();
            _onlineFriendsText = _onlineFriendsViewpoint.GetComponent<Text>();
            _hasLoadedOnUi = true;

            #endregion
        }
    }
}
