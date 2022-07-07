using MintMod.Reflections;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MintMod.Libraries;
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

        private static GameObject _vrcaButton, _avatarStats, _fallbackInfo;

        internal override void OnUserInterface() {
            #region DL VRCA

            if (Config.AvatarMenuDlVrca.Value) {
                _vrcaButton = Object.Instantiate(GameObject.Find("UserInterface/MenuContent/Screens/Avatar/Change Button"), GameObject.Find("UserInterface/MenuContent/Screens/Avatar").transform);
            _vrcaButton.GetComponentInChildren<Text>().text = "DL VRCA";
            _vrcaButton.GetComponent<RectTransform>().localPosition = new Vector2(-650, -474);
            _vrcaButton.GetComponent<RectTransform>().sizeDelta = new Vector2(164, 80);
            if (!Config.ColorGameMenu.Value) {
                _vrcaButton.GetComponent<Button>().GetComponentInChildren<Image>().color = Minty;
                _vrcaButton.GetComponentInChildren<Image>().color = Minty;
            }
            _vrcaButton.gameObject.name = "Mint_DL-VRCA";
            _vrcaButton.SetActive(true);
            _vrcaButton.GetComponentInChildren<Button>().onClick = new Button.ButtonClickedEvent();
            _vrcaButton.GetComponentInChildren<Button>().onClick.AddListener(new System.Action(() => {
                var aviPed = GameObject.Find("UserInterface/MenuContent/Screens/Avatar/AvatarPreviewBase/MainRoot/MainModel");
                if (aviPed.GetComponent<SimpleAvatarPedestal>().field_Internal_ApiAvatar_0 == null) return;
                try {
                    var avi = aviPed!.GetComponent<SimpleAvatarPedestal>().field_Internal_ApiAvatar_0;
                    
                    var vrcaPath = $"{MintCore.MintDirectory}\\Assets\\VRCA\\";

                    var grabAssetUrl = avi.assetUrl;
                    var grabAssetName = avi.name;
                    var grabAssetVersion = avi.version;
                    var grabAssetImage = avi.imageUrl;
                    var grabAssetPlatform = avi.platform;
                    
                    if (!Directory.Exists(Path.Combine(vrcaPath)))
                        Directory.CreateDirectory(Path.Combine(vrcaPath));
                    
                    var vrcaFile = $"{vrcaPath}{grabAssetPlatform}_{grabAssetName}_V{grabAssetVersion}.vrca";
                    var imageFile = $"{vrcaPath}{grabAssetPlatform}_{grabAssetName}_V{grabAssetVersion}.png";
                    
                    var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:87.0) Gecko/20100101 Firefox/87.0");
                    
                    if (!File.Exists(vrcaFile)) {
                        var a = httpClient.GetByteArrayAsync(grabAssetUrl).GetAwaiter().GetResult();
                        try {
                            Task.Run(async () => {
                                await CustomAsync.File.WriteAllBytesAsync(vrcaFile, a);
                            });
                        }
                        catch { File.WriteAllBytes(vrcaFile, a); }
                    }

                    if (!File.Exists(imageFile)) {
                        var i = httpClient.GetByteArrayAsync(grabAssetImage).GetAwaiter().GetResult();
                        try {
                            Task.Run(async () => {
                                await CustomAsync.File.WriteAllBytesAsync(imageFile, i);
                            });
                        }
                        catch { File.WriteAllBytes(imageFile, i); }
                    }
                    
                    Con.Msg($"Downloaded VRCA for {grabAssetName}.\nLocated in {vrcaPath}");
                    VrcUiPopups.Notify(MintCore.ModBuildInfo.Name, $"Downloaded VRCA for {grabAssetName}");
                } catch (Exception e) { Con.Error(e); }
            }));
            
            _fallbackInfo = GameObject.Find("UserInterface/MenuContent/Screens/Avatar/Fallback Info");
            GameObject.Find("UserInterface/MenuContent/Screens/Avatar/Select Button")
                .GetComponent<Button>().onClick.AddListener(new System.Action(() => _vrcaButton.SetActive(!_fallbackInfo.activeInHierarchy)));
            }
            
            #region Avatar Stats
            _avatarStats = GameObject.Find("UserInterface/MenuContent/Screens/Avatar/AvatarDetails Button");
            _avatarStats.GetComponent<RectTransform>().localPosition = new Vector2(-487, -474);
            _avatarStats.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 80);
            var avatarStatsPerfIcon = GameObject.Find("UserInterface/MenuContent/Screens/Avatar/AvatarDetails Button/PerfIcon");
            var avatarStatsText = GameObject.Find("UserInterface/MenuContent/Screens/Avatar/AvatarDetails Button/Text");
            var avatarStatsPlatformIcon = GameObject.Find("UserInterface/MenuContent/Screens/Avatar/AvatarDetails Button/PlatformIcon");
            avatarStatsText.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
            avatarStatsPerfIcon.GetComponent<RectTransform>().localPosition = new Vector2(-72, 0);
            avatarStatsPlatformIcon.SetActive(false);
            avatarStatsPerfIcon.SetActive(false);
            #endregion
            
            #endregion
        }
    }
}
