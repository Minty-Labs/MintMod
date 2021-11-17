using System;
using System.Linq;
using System.Reflection;
using Il2CppSystem.Collections.Generic;
using UnityEngine;
using VRC;
using VRC.Core;
using MintMod.Managers;
using VRC.SDKBase;

namespace MintMod.Reflections {
    public static class PlayerWrappers {
        public static VRCPlayer GetVRCPlayer(this Player player) => player._vrcplayer;

        public static VRCPlayerApi GetVRCPlayerApi(this Player player) => player.prop_VRCPlayerApi_0;

        public static VRCPlayer GetCurrentPlayer() => VRCPlayer.field_Internal_Static_VRCPlayer_0;

        public static Vector3 GetCurrentPlayerPos() => VRCPlayer.field_Internal_Static_VRCPlayer_0.prop_VRCPlayerApi_0.GetPosition();

        public static Quaternion GetCurrentPlayerRot() => VRCPlayer.field_Internal_Static_VRCPlayer_0.prop_VRCPlayerApi_0.GetRotation();

        public static PlayerManager GetPlayerManager() => PlayerManager.field_Private_Static_PlayerManager_0;

        public static Player[] GetAllPlayersArray(this PlayerManager instance) => instance.prop_ArrayOf_Player_0;

        public static List<Player> GetAllPlayers(this PlayerManager instance) => instance.field_Private_List_1_Player_0;

        public static APIUser GetAPIUser(this Player player) => player.prop_APIUser_0;

        public static Player GetPlayer(this PlayerManager instance, string UserID) {
            List<Player> allPlayers = instance.GetAllPlayers();
            Player result = null;
            foreach (Player all in allPlayers)
                if (all.GetAPIUser().id == UserID)
                    result = all;
            return result;
        }

        public static Player GetSelectedPlayer(this QuickMenu instance) {
            APIUser api = instance.prop_APIUser_0;
            if (instance.prop_APIUser_0 == null)
                api = instance.field_Private_APIUser_0;
            return GetPlayerManager().GetPlayer(api.id);
        }

        private static Action<Player> requestedAction;
        public static void GetEachPlayer(Action<Player> act) {
            requestedAction = act;
            foreach (Player obj in PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0)
                requestedAction(obj);
        }

        public static Il2CppSystem.Collections.Generic.List<Player> GetAllPlayers() {
            if (PlayerManager.field_Private_Static_PlayerManager_0 == null)
                return null;
            return PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0;
        }

        public static APIUser GetpAPI(this Player p) {
            return p.prop_APIUser_0;
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
    }
}
