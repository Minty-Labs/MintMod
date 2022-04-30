using System;
using System.Linq;
using System.Reflection;
using DiskWars;
using Il2CppSystem.Collections.Generic;
using LitJson;
using UnityEngine;
using VRC;
using VRC.Core;
using MintMod.Managers;
using MintMod.UserInterface;
using MintyLoader;
using VRC.DataModel;
using VRC.SDKBase;
using VRC.UI.Elements.Menus;
using VRC.DataModel.Core;
using VRC.SDKBase.Validation.Performance;
using Object = UnityEngine.Object;

namespace MintMod.Reflections {
    public static class PlayerWrappers {
        internal static VRCPlayer GetLocalVRCPlayer() => VRCPlayer.field_Internal_Static_VRCPlayer_0;

        public static VRCPlayer GetCurrentPlayer() => VRCPlayer.field_Internal_Static_VRCPlayer_0;

        public static Vector3 GetCurrentPlayerPos() => VRCPlayer.field_Internal_Static_VRCPlayer_0.prop_VRCPlayerApi_0.GetPosition();

        public static Quaternion GetCurrentPlayerRot() => VRCPlayer.field_Internal_Static_VRCPlayer_0.prop_VRCPlayerApi_0.GetRotation();

        private static List<Player> GetAllPlayers(this PlayerManager instance) => instance.field_Private_List_1_Player_0;

        private static APIUser GetAPIUser_alt(this Player player) => player.prop_APIUser_0;

        public static Player GetPlayer(this PlayerManager instance, string userId) {
            var allPlayers = instance.GetAllPlayers();
            Player result = null;
            foreach (Player all in allPlayers)
                if (all.GetAPIUser_alt().id == userId)
                    result = all;
            return result;
        }

        public static List<Player> GetAllPlayers() =>
            PlayerManager.field_Private_Static_PlayerManager_0 == null ? null : PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0;

        private static APIUser GetpAPI(this Player p) => p.field_Private_APIUser_0;

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
                var user = _selectedUserMenuQM.field_Private_IUser_0.Cast<DataModel<APIUser>>();
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

        private static int GetFrames(this VRCPlayer Instance) {
            if (Instance._playerNet.field_Private_Byte_0 == 0)
                return 0;
            return (int)(1000f / (float)Instance._playerNet.field_Private_Byte_0);
        }

        public static Color GetTrustColor(this APIUser user) {
            if (user.hasLegendTrustLevel) 
                return user.tags.Contains("system_legend") ? Colors.LegendNP : Colors.VeteranNP;
            if (user.hasVeteranTrustLevel)
                return Colors.TrustedNP;
            if (user.hasTrustedTrustLevel)
                return Colors.KnownNP;
            if (user.hasKnownTrustLevel)
                return Colors.UserNP;
            if (user.hasBasicTrustLevel)
                return Colors.NewUserNP;
            return user.isUntrusted ? Colors.VisitorNP : Color.white;
        }

        public static Player SelPlayer() =>
            PlayerManager.field_Private_Static_PlayerManager_0.GetPlayer(GetSelectedAPIUser().id)._vrcplayer._player;

        public static VRCPlayer SelVrcPlayer() =>
            PlayerManager.field_Private_Static_PlayerManager_0.GetPlayer(GetSelectedAPIUser().id)._vrcplayer;
        
        public static bool IsFriend(Player p) => APIUser.IsFriendsWith(p.field_Private_APIUser_0.id);
        
        private static Action<Player> _requestedAction;
        public static void GetEachPlayer(Action<Player> act) {
            _requestedAction = act;
            foreach (var plr in PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0)
                _requestedAction.Invoke(plr);
        }
        
        public static string GetFramesColored(this VRCPlayer player) {
            string numAsString;
            if (player.GetFrames() >= 80) numAsString = "<color=#33ff47>";
            else if (player.GetFrames() <= 80 && player.GetFrames() >= 30) numAsString = "<color=#ff8936>";
            else numAsString = "<color=red>";
            return $"{numAsString}{player.GetFrames()}</color>";
        }

        private static int _method = 0;

        public static string GetAviPerformance(this VRCPlayer player) {
            try {
                var p = player.field_Private_VRCAvatarManager_0.prop_AvatarPerformanceStats_0.GetPerformanceRatingForCategory(AvatarPerformanceCategory.Overall);

                if (Config.DisablePerformanceStats.Value)
                    return "<i>Disabled</i>";

                return p switch {
                    PerformanceRating.VeryPoor =>  "<color=#E45A42>VP</color>",
                    PerformanceRating.Poor =>      "<color=#E45A42>P</color>",
                    PerformanceRating.Medium =>    "<color=#E7AA08>M</color>",
                    PerformanceRating.Good =>      "<color=#69A95C>G</color>",
                    PerformanceRating.Excellent => "<color=#6BE855>E</color>",
                    PerformanceRating.None =>      "<i>Loading</i>",
                    _ =>                           "<i>Blocked</i>"
                };
            }
            catch {
                return "<color=red>Broke with Update</color>";
            }
        }
        
        private static bool IsInVR(this VRCPlayer player) => player.prop_VRCPlayerApi_0.IsUserInVR();

        public static string Platform(this VRCPlayer player) {
            if (player.prop_Player_0.prop_APIUser_0.last_platform == "standalonewindows") 
                return player.IsInVR() ? "<color=#3592C4>VR</color>" : "<color=#40B6E0>D</color>";
            return "<color=#7AA45D>Q</color>";
        }
    }
}
