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

namespace MintMod.Functions {
    class MasterFinder : MintSubMod {
        public override string Name => "MasterFinder";
        public override string Description => "Shows a crown uptop the users's nameplate showing instance master.";

        public static GameObject MasterIcon;

        internal override void OnUserInterface() => MelonCoroutines.Start(Loop());

        IEnumerator Loop() {
            while (Config.EnableMasterFinder.Value) {
                try {
                    if (RoomManager.field_Internal_Static_ApiWorld_0 != null && Config.EnableMasterFinder.Value && MasterIcon == null) {
                        if (PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0.Count > 1) {
                            PlayerWrappers.GetEachPlayer(delegate (Player player) {
                                if (MasterIcon == null && player.GetVRCPlayerApi().isMaster && player.prop_APIUser_0.id != APIUser.CurrentUser.id) {
                                    GameObject gameObject = player._vrcplayer.field_Public_PlayerNameplate_0.transform.Find("Contents/Friend Marker").gameObject;
                                    MasterIcon = UnityEngine.Object.Instantiate<GameObject>(gameObject, gameObject.transform.parent);
                                    if (ModCompatibility.NameplateStats || ModCompatibility.ReMod)
                                        MasterIcon.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 90f, 0f);
                                    else
                                        MasterIcon.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 60f, 0f);
                                    MasterIcon.GetComponent<Image>().sprite = MintyResources.masterCrown;
                                    MasterIcon.gameObject.name = "Mint_MasterIcon";
                                    MasterIcon.SetActive(true);
                                }
                            });
                        } else {
                            MasterIcon.Destroy();
                            MasterIcon = null;
                        }
                    } else if (MasterIcon != null && !Config.EnableMasterFinder.Value)
                        MasterIcon.Destroy();
                } catch {

                }
                yield return new WaitForSeconds(0.25f);
            }
            yield break;
        }

        internal override void OnUpdate() {
            if (MasterIcon != null && (ModCompatibility.NameplateStats || ModCompatibility.ReMod)) {
                if (Patches.IsQMOpen)
                    MasterIcon.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 90f, 0f);
                else
                    MasterIcon.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 60f, 0f);
            }
        }
    }
}
