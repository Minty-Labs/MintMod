using MelonLoader;
using MintMod.Reflections;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MintMod.Utils;
using MintyLoader;
using UnityEngine;
using UnityEngine.UI;
using VRC;
using static MintMod.Managers.Colors;
using Exception = System.Exception;

namespace MintMod.UserInterface.OldUI {
    internal class AvatarMenu : MintSubMod {
        public override string Name => "Avatar Menu";
        public override string Description => "Edits on the Avatar Menu";

        private static GameObject VRCAButton, AvatarStats, FallbackInfo;

        internal override void OnUserInterface() {
            #region DL VRCA
            VRCAButton = UnityEngine.Object.Instantiate<GameObject>(GameObject.Find("UserInterface/MenuContent/Screens/Avatar/Change Button"), GameObject.Find("UserInterface/MenuContent/Screens/Avatar").transform);
            VRCAButton.GetComponentInChildren<Text>().text = "DL VRCA";
            VRCAButton.GetComponent<RectTransform>().localPosition = new Vector2(-650, -474);
            VRCAButton.GetComponent<RectTransform>().sizeDelta = new Vector2(164, 80);
            if (!Config.ColorGameMenu.Value) {
                VRCAButton.GetComponent<Button>().GetComponentInChildren<Image>().color = Minty;
                VRCAButton.GetComponentInChildren<Image>().color = Minty;
            }
            VRCAButton.gameObject.name = "Mint_DL-VRCA";
            VRCAButton.SetActive(true);
            VRCAButton.GetComponentInChildren<Button>().onClick = new Button.ButtonClickedEvent();
            VRCAButton.GetComponentInChildren<Button>().onClick.AddListener(new System.Action(() => {
                if (UIWrappers.GetVRCUiMInstance().field_Public_GameObject_0.GetComponentInChildren<SimpleAvatarPedestal>().field_Internal_ApiAvatar_0 != null) {
                    try {
                        ThreadStart DLVRCA = delegate () {
                            var avi = UIWrappers.GetVRCUiMInstance().field_Public_GameObject_0.GetComponentInChildren<SimpleAvatarPedestal>().field_Internal_ApiAvatar_0;
                            string subdir = $"{MintCore.MintDirectory}\\Assets\\VRCA\\";

                            string grab_assetUrl = avi.assetUrl;
                            string grab_assetName = avi.name;
                            int grab_assetVersion = avi.version;
                            string grab_assetImage = avi.imageUrl;
                            string grab_assetImage_Backup = avi.thumbnailImageUrl;
                            string grab_assetPlatform = avi.platform;

                            if (!Directory.Exists(Path.Combine(subdir)))
                                Directory.CreateDirectory(Path.Combine(subdir));
                            if (!File.Exists(Path.Combine(subdir, grab_assetPlatform + "_" + grab_assetName + "_V" + grab_assetVersion + ".vrca"))) {
                                using WebClient webClientVRCA = new WebClient {
                                    Headers =
                                    {
                                        ["User-Agent"] =
                                            "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:87.0) Gecko/20100101 Firefox/87.0"
                                    }
                                };
                                try { webClientVRCA.DownloadFile(grab_assetUrl, Path.Combine(subdir + grab_assetPlatform + "_" + grab_assetName + "_V" + grab_assetVersion + ".vrca")); } 
                                catch (System.Exception e) { Con.Error(e); }
                            }
                            if (!File.Exists(Path.Combine(subdir, grab_assetPlatform + "_" + grab_assetName + "_V" + grab_assetVersion + ".png"))) {
                                using WebClient webClientIMG = new WebClient {
                                    Headers =
                                    {
                                        ["User-Agent"] =
                                            "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:87.0) Gecko/20100101 Firefox/87.0"
                                    }
                                };
                                try { webClientIMG.DownloadFile(grab_assetImage, Path.Combine(subdir + grab_assetPlatform + "_" + grab_assetName + "_V" + grab_assetVersion + ".png")); } catch (System.Exception e) {
                                    try { webClientIMG.DownloadFile(grab_assetImage_Backup, Path.Combine(subdir + grab_assetPlatform + "_" + grab_assetName + "_V" + grab_assetVersion + ".png")); } 
                                    catch (System.Exception b) { Con.Error($"First Error: \n{e}\n========================\nSecond Error: \n{b}"); }
                                }
                            }
                            Con.Msg("Downloaded VRCA for " + grab_assetName + ".\nLocated in /Documents/VRChat/LoliteUtilityMod/Assets/VRCA/");
                            VRCUiPopups.Notify($"Downloaded VRCA for {grab_assetName}");
                            //VRCUiManager.prop_VRCUiManager_0.InformHudText($"Downloaded VRCA for {grab_assetName}", Color.white);
                        };
                        new Thread(DLVRCA).Start();
                    } catch (Exception e) { Con.Error(e); }
                }
            }));
            #endregion

            #region Avatar Stats
            AvatarStats = GameObject.Find("UserInterface/MenuContent/Screens/Avatar/AvatarDetails Button");
            AvatarStats.GetComponent<RectTransform>().localPosition = new Vector2(-487, -474);
            AvatarStats.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 80);
            GameObject AvatarStatsPerfIcon = GameObject.Find("UserInterface/MenuContent/Screens/Avatar/AvatarDetails Button/PerfIcon");
            GameObject AvatarStatsText = GameObject.Find("UserInterface/MenuContent/Screens/Avatar/AvatarDetails Button/Text");
            GameObject AvatarStatsPlatformIcon = GameObject.Find("UserInterface/MenuContent/Screens/Avatar/AvatarDetails Button/PlatformIcon");
            AvatarStatsText.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
            AvatarStatsPerfIcon.GetComponent<RectTransform>().localPosition = new Vector2(-72, 0);
            AvatarStatsPlatformIcon.SetActive(false);
            AvatarStatsPerfIcon.SetActive(false);
            #endregion

            FallbackInfo = GameObject.Find("UserInterface/MenuContent/Screens/Avatar/Fallback Info");
            GameObject.Find("UserInterface/MenuContent/Screens/Avatar/Select Button")
                .GetComponent<Button>().onClick.AddListener(new System.Action(() => {
                if (!FallbackInfo.activeInHierarchy)
                    VRCAButton.SetActive(true);
                else
                    VRCAButton.SetActive(false);
            }));
        }
    }
}
