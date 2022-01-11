using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;
using MintMod.Reflections;
using MintMod.Resources;
using VRC;
using VRC.Core;
using UnityEngine.UI;
using MintMod.Hooks;
using MelonLoader;
using MintMod.Libraries;
using MintMod.UserInterface;
using MintMod.UserInterface.QuickMenu;
using ReMod.Core.VRChat;

namespace MintMod.Functions {
    internal class MasterFinder : MintSubMod {
        public override string Name => "MasterFinder";
        public override string Description => "Shows a crown on top the user's nameplate showing instance master.";

        public static GameObject MasterIcon, oriFriendMarker;
        private static RectTransform r;
        private static VRCPlayer MasterOfInstance;

        private static void DetectMaster() {
            if (MasterOfInstance == null) {
                foreach (var p in PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0) {
                    if (p.field_Private_VRCPlayerApi_0.isMaster)
                        MasterOfInstance = p._vrcplayer;
                }
            }

            if (!MintUserInterface.isStreamerModeOn && MasterOfInstance != null && RoomManager.field_Internal_Static_ApiWorld_0 != null &&
                Config.EnableMasterFinder.Value && MasterIcon == null) {
                if (PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0.Count > 1) {
                    if (MasterOfInstance.field_Public_PlayerNameplate_0 != null && MasterOfInstance.field_Private_VRCPlayerApi_0.isMaster) {
                        var nameplate = MasterOfInstance.field_Public_PlayerNameplate_0;
                        oriFriendMarker = nameplate.transform.Find("Contents/Friend Marker").gameObject;
                        MasterIcon = UnityEngine.Object.Instantiate(oriFriendMarker, oriFriendMarker.transform.parent);
                        r = MasterIcon.GetComponent<RectTransform>();
                        MasterIcon.GetComponent<Image>().sprite = MintyResources.masterCrown;
                        MasterIcon.gameObject.name = "Mint_MasterIcon";
                        MasterIcon.SetActive(Config.EnableMasterFinder.Value);
                    }
                } else {
                    MasterIcon.Destroy();
                    MasterIcon = null;
                }
            } else if (MasterIcon != null && !Config.EnableMasterFinder.Value)
                MasterIcon.Destroy();
        }

        public static void OnPlayerLeft() => DetectMaster();

        internal override void OnLevelWasLoaded(int buildindex, string SceneName) => MasterOfInstance = null;

        internal override void OnUpdate() {
            if (MasterIcon != null && (ModCompatibility.NameplateStats || ModCompatibility.ReMod) && r != null && !MintUserInterface.isStreamerModeOn) 
                r.anchoredPosition = Patches.IsQMOpen ? new Vector3(0f, 90f, 0f) : new Vector3(0f, 60f, 0f);
        }

        internal override void OnPrefSave() {
            if (MasterIcon != null && !MintUserInterface.isStreamerModeOn)
                MasterIcon.SetActive(Config.EnableMasterFinder.Value);
            //try{ MasterIcon.SetActive(Config.EnableMasterFinder.Value); }catch{}
        }
    }
}
