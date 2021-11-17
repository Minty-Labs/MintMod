using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MelonLoader;
using MintMod.Reflections;
using MintMod.Utils;
using UnityEngine;

namespace MintMod.Functions {
    class PlayerActions {
        public static void AvatarDownload() {
            try {
                ThreadStart DLVRCA = delegate () {
                    string subdir = $"{MintCore.MintDirectory}\\Assets\\VRCA\\";

                    string grab_assetUrl, grab_assetName, grab_assetImage, grab_assetPlatform;
                    int grab_assetVersion;

                    if (PlayerWrappers.GetSelectedPlayer(QuickMenu.prop_QuickMenu_0)._vrcplayer.prop_ApiAvatar_1.platform.Contains("windows")) {
                        grab_assetUrl = PlayerWrappers.GetSelectedPlayer(QuickMenu.prop_QuickMenu_0)._vrcplayer.prop_ApiAvatar_1.assetUrl;
                        grab_assetName = PlayerWrappers.GetSelectedPlayer(QuickMenu.prop_QuickMenu_0)._vrcplayer.prop_ApiAvatar_1.name;
                        grab_assetVersion = PlayerWrappers.GetSelectedPlayer(QuickMenu.prop_QuickMenu_0)._vrcplayer.prop_ApiAvatar_1.version;
                        grab_assetImage = PlayerWrappers.GetSelectedPlayer(QuickMenu.prop_QuickMenu_0)._vrcplayer.prop_ApiAvatar_1.imageUrl;
                        grab_assetPlatform = PlayerWrappers.GetSelectedPlayer(QuickMenu.prop_QuickMenu_0)._vrcplayer.prop_ApiAvatar_1.platform;
                    } else {
                        grab_assetUrl = PlayerWrappers.GetSelectedPlayer(QuickMenu.prop_QuickMenu_0)._vrcplayer.prop_ApiAvatar_0.assetUrl;
                        grab_assetName = PlayerWrappers.GetSelectedPlayer(QuickMenu.prop_QuickMenu_0)._vrcplayer.prop_ApiAvatar_0.name;
                        grab_assetVersion = PlayerWrappers.GetSelectedPlayer(QuickMenu.prop_QuickMenu_0)._vrcplayer.prop_ApiAvatar_0.version;
                        grab_assetImage = PlayerWrappers.GetSelectedPlayer(QuickMenu.prop_QuickMenu_0)._vrcplayer.prop_ApiAvatar_0.imageUrl;
                        grab_assetPlatform = PlayerWrappers.GetSelectedPlayer(QuickMenu.prop_QuickMenu_0)._vrcplayer.prop_ApiAvatar_0.platform;
                    }

                    if (!Directory.Exists(Path.Combine(subdir)))
                        Directory.CreateDirectory(Path.Combine(subdir));
                    if (!File.Exists(Path.Combine(subdir, grab_assetPlatform + "_" + grab_assetName + "_V" + grab_assetVersion + ".vrca"))) {
                        using (WebClient webClient = new WebClient {
                            Headers =
                            {
                                ["User-Agent"] =
                                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:87.0) Gecko/20100101 Firefox/87.0"
                            }
                        }) {
                            webClient.DownloadFile(grab_assetUrl, Path.Combine(subdir + grab_assetPlatform + "_" + grab_assetName + "_V" + grab_assetVersion + ".vrca"));
                        }
                    }
                    if (!File.Exists(Path.Combine(subdir, grab_assetPlatform + "_" + grab_assetName + "_V" + grab_assetVersion + ".png"))) {
                        using (WebClient webClient = new WebClient {
                            Headers =
                            {
                                ["User-Agent"] =
                                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:87.0) Gecko/20100101 Firefox/87.0"
                            }
                        }) {
                            webClient.DownloadFile(grab_assetImage, Path.Combine(subdir + grab_assetPlatform + "_" + grab_assetName + "_V" + grab_assetVersion + ".png"));
                        }
                    }

                    //Process.Start(grab_assetUrl);
                    MelonLogger.Msg($"Downloaded VRCA for {grab_assetName}.\nLocated in {subdir}");
                    VRCUiManager.prop_VRCUiManager_0.InformHudText($"Downloaded VRCA for {grab_assetName}", Color.white);
                };
                new Thread(DLVRCA).Start();
            } catch { MelonLogger.Error("Failed to download VRCA");
                VRCUiManager.prop_VRCUiManager_0.InformHudText("Failed to download VRCA", Color.white);
            }
        }

        public static void AvatarSELFDownload() {
            try {
                ThreadStart DLSELF_VRCA = delegate() {
                    string subdir = $"{MintCore.MintDirectory}\\Assets\\VRCA\\";

                    string grab_SELF_assetUrl, grab_SELF_assetName, grab_SELF_assetImage, grab_SELF_assetPlatform;
                    int grab_SELF_assetVersion;

                    if (PlayerWrappers.GetCurrentPlayer().prop_ApiAvatar_1.platform.Contains("windows")) {
                        grab_SELF_assetUrl = PlayerWrappers.GetCurrentPlayer().prop_ApiAvatar_1.assetUrl;
                        grab_SELF_assetName = PlayerWrappers.GetCurrentPlayer().prop_ApiAvatar_1.name;
                        grab_SELF_assetVersion = PlayerWrappers.GetCurrentPlayer().prop_ApiAvatar_1.version;
                        grab_SELF_assetImage = PlayerWrappers.GetCurrentPlayer().prop_ApiAvatar_1.imageUrl;
                        grab_SELF_assetPlatform = PlayerWrappers.GetCurrentPlayer().prop_ApiAvatar_1.platform;
                    }
                    else {
                        grab_SELF_assetUrl = PlayerWrappers.GetCurrentPlayer().prop_ApiAvatar_0.assetUrl;
                        grab_SELF_assetName = PlayerWrappers.GetCurrentPlayer().prop_ApiAvatar_0.name;
                        grab_SELF_assetVersion = PlayerWrappers.GetCurrentPlayer().prop_ApiAvatar_0.version;
                        grab_SELF_assetImage = PlayerWrappers.GetCurrentPlayer().prop_ApiAvatar_0.imageUrl;
                        grab_SELF_assetPlatform = PlayerWrappers.GetCurrentPlayer().prop_ApiAvatar_0.platform;
                    }

                    if (!Directory.Exists(Path.Combine(subdir)))
                        Directory.CreateDirectory(Path.Combine(subdir));
                    if (!File.Exists(Path.Combine(subdir,
                        grab_SELF_assetPlatform + "_" + grab_SELF_assetName + "_V" + grab_SELF_assetVersion +
                        ".vrca"))) {
                        using (WebClient webClient = new WebClient {
                            Headers = {
                                ["User-Agent"] =
                                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:87.0) Gecko/20100101 Firefox/87.0"
                            }
                        }) {
                            webClient.DownloadFile(grab_SELF_assetUrl,
                                Path.Combine(subdir + grab_SELF_assetPlatform + "_" + grab_SELF_assetName + "_V" +
                                             grab_SELF_assetVersion + ".vrca"));
                        }
                    }

                    if (!File.Exists(Path.Combine(subdir,
                        grab_SELF_assetPlatform + "_" + grab_SELF_assetName + "_V" + grab_SELF_assetVersion +
                        ".png"))) {
                        using (WebClient webClient = new WebClient {
                            Headers = {
                                ["User-Agent"] =
                                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:87.0) Gecko/20100101 Firefox/87.0"
                            }
                        }) {
                            webClient.DownloadFile(grab_SELF_assetImage,
                                Path.Combine(subdir + grab_SELF_assetPlatform + "_" + grab_SELF_assetName + "_V" + grab_SELF_assetVersion + ".png"));
                        }
                    }

                    //Process.Start(grab_assetUrl);
                    MelonLogger.Msg($"Downloaded VRCA for {grab_SELF_assetName}");
                    VRCUiManager.prop_VRCUiManager_0.InformHudText($"Downloaded VRCA for {grab_SELF_assetName}", Color.white);
                };
                new Thread(DLSELF_VRCA).Start();
            }
            catch {
                MelonLogger.Error("Failed to download VRCA");
                VRCUiManager.prop_VRCUiManager_0.InformHudText("Failed to download own VRCA", Color.white);
            }
        }
    }
}
