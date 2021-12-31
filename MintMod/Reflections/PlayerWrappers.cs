using System;
using System.Linq;
using System.Reflection;
using Il2CppSystem.Collections.Generic;
using UnityEngine;
using VRC;
using VRC.Core;
using MintMod.Managers;
using MintyLoader;
using VRC.SDKBase;
using VRC.UI.Elements.Menus;
using VRC.DataModel.Core;
using Object = UnityEngine.Object;

namespace MintMod.Reflections {
    public static class PlayerWrappers {
        internal static VRCPlayer GetLocalVRCPlayer() => VRCPlayer.field_Internal_Static_VRCPlayer_0;

        public static VRCPlayer GetCurrentPlayer() => VRCPlayer.field_Internal_Static_VRCPlayer_0;

        public static Vector3 GetCurrentPlayerPos() => VRCPlayer.field_Internal_Static_VRCPlayer_0.prop_VRCPlayerApi_0.GetPosition();

        public static Quaternion GetCurrentPlayerRot() => VRCPlayer.field_Internal_Static_VRCPlayer_0.prop_VRCPlayerApi_0.GetRotation();

        public static List<Player> GetAllPlayers(this PlayerManager instance) => instance.field_Private_List_1_Player_0;

        public static APIUser GetAPIUser_alt(this Player player) => player.prop_APIUser_0;

        public static Player GetPlayer(this PlayerManager instance, string UserID) {
            List<Player> allPlayers = instance.GetAllPlayers();
            Player result = null;
            foreach (Player all in allPlayers)
                if (all.GetAPIUser_alt().id == UserID)
                    result = all;
            return result;
        }

        public static Il2CppSystem.Collections.Generic.List<Player> GetAllPlayers() {
            if (PlayerManager.field_Private_Static_PlayerManager_0 == null)
                return null;
            return PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0;
        }

        public static APIUser GetpAPI(this Player p) {
            return p.field_Private_APIUser_0;
        }

        public static Player getPlayerFromPlayerlist(string userID) {
            foreach (var player in PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0) {
                if (player.prop_APIUser_0 != null) {
                    if (player.prop_APIUser_0.id.Equals(userID))
                        return player;
                }
            }

            return null;
        }

        private static SelectedUserMenuQM _selectedUserMenuQM;
        public static APIUser GetSelectedAPIUser() {
            if (_selectedUserMenuQM == null)
                _selectedUserMenuQM = Object.FindObjectOfType<SelectedUserMenuQM>();

            if (_selectedUserMenuQM != null) {
                DataModel<APIUser> user = _selectedUserMenuQM.field_Private_IUser_0.Cast<DataModel<APIUser>>();
                return user.field_Protected_TYPE_0;
            }

            Con.Error("Unable to get SelectedUserMenuQM component!");
            return null;
        }

        public static Player GetPlayer(string id) {
            foreach (Player player in GetAllPlayers()) {
                if (player == null)
                    continue;
                if (player.GetpAPI().id == id)
                    return VRC.Player.prop_Player_0;
            }
            return null;
        }

        public static int GetFrames(this VRCPlayer Instance) {
            if (Instance._playerNet.field_Private_Byte_0 == 0)
                return 0;
            return (int)(1000f / (float)Instance._playerNet.field_Private_Byte_0);
        }

        public static Color GetTrustColor(this APIUser user) {
            if (user.hasLegendTrustLevel) {
                if (user.tags.Contains("system_legend"))
                    return Colors.LegendNP;
                return Colors.VeteranNP;
            }
            if (user.hasVeteranTrustLevel)
                return Colors.TrustedNP;
            if (user.hasTrustedTrustLevel)
                return Colors.KnownNP;
            if (user.hasKnownTrustLevel)
                return Colors.UserNP;
            if (user.hasBasicTrustLevel)
                return Colors.NewUserNP;
            if (user.isUntrusted)
                return Colors.VisitorNP;
            return Color.white;
        }

        public static Player SelPlayer() {
            return PlayerManager.field_Private_Static_PlayerManager_0.GetPlayer(GetSelectedAPIUser().id)._vrcplayer._player;
        }

        public static VRCPlayer SelVRCPlayer() {
            return PlayerManager.field_Private_Static_PlayerManager_0.GetPlayer(GetSelectedAPIUser().id)._vrcplayer;
        }
        
        public static bool isFriend(Player p) => APIUser.IsFriendsWith(p.field_Private_APIUser_0.id);
    }
}
