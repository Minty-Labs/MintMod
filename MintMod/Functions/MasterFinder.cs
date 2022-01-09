﻿using System;
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

namespace MintMod.Functions {
    class MasterFinder : MintSubMod {
        public override string Name => "MasterFinder";
        public override string Description => "Shows a crown on top the user's nameplate showing instance master.";

        public static GameObject MasterIcon, oriFriendMarker;
        private static RectTransform r;

        internal static void OnPlayerJoinLeave() {
            if (MintUserInterface.isStreamerModeOn) return;
            if (RoomManager.field_Internal_Static_ApiWorld_0 != null && Config.EnableMasterFinder.Value && MasterIcon == null) {
                if (PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0.Count > 1) {
                    foreach (var obj in PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0) {
                        if (obj._vrcplayer.field_Public_PlayerNameplate_0 != null && obj._vrcplayer.field_Private_VRCPlayerApi_0.isMaster) {
                            var nameplate = obj._vrcplayer.field_Public_PlayerNameplate_0;
                            oriFriendMarker = nameplate.transform.Find("Contents/Friend Marker").gameObject;
                            MasterIcon = UnityEngine.Object.Instantiate(oriFriendMarker, oriFriendMarker.transform.parent);
                            r = MasterIcon.GetComponent<RectTransform>();
                            MasterIcon.GetComponent<Image>().sprite = MintyResources.masterCrown;
                            MasterIcon.gameObject.name = "Mint_MasterIcon";
                            MasterIcon.SetActive(Config.EnableMasterFinder.Value);
                        }
                    }
                } else {
                    MasterIcon.Destroy();
                    MasterIcon = null;
                }
            } else if (MasterIcon != null && !Config.EnableMasterFinder.Value)
                MasterIcon.Destroy();
        }

        internal override void OnUpdate() {
            if (MasterIcon != null && (ModCompatibility.NameplateStats || ModCompatibility.ReMod) && r != null && !MintUserInterface.isStreamerModeOn) {
                if (Patches.IsQMOpen)
                    r.anchoredPosition = new Vector3(0f, 90f, 0f);
                else
                    r.anchoredPosition = new Vector3(0f, 60f, 0f);
            }
        }

        internal override void OnPrefSave() {
            if (MasterIcon != null && !MintUserInterface.isStreamerModeOn)
                MasterIcon.SetActive(Config.EnableMasterFinder.Value);
            //try{ MasterIcon.SetActive(Config.EnableMasterFinder.Value); }catch{}
        }
    }
}
