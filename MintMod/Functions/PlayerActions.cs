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
using MintyLoader;
using UnityEngine;
using VRC;
using VRC.Core;

namespace MintMod.Functions {
    class PlayerActions {
        public static void AvatarDownload() {
            try {
                ThreadStart DLVRCA = delegate () {
                    string subdir = $"{MintCore.MintDirectory}\\Assets\\VRCA\\";

                    VRCPlayer p = PlayerManager.field_Private_Static_PlayerManager_0.GetPlayer(PlayerWrappers.GetSelectedAPIUser().id)._vrcplayer;

                    bool windows = p.field_Private_ApiAvatar_1.platform.ToLower().Contains("Windows");

                    APIUser u = PlayerManager.field_Private_Static_PlayerManager_0.GetPlayer(PlayerWrappers.GetSelectedAPIUser().id).field_Private_APIUser_0;
                    ApiAvatar a = windows ? p.prop_ApiAvatar_1 : p.prop_ApiAvatar_0;

                    string grab_assetUrl, grab_assetName, grab_assetImage, grab_assetPlatform;
                    int grab_assetVersion;

                    grab_assetUrl = a.assetUrl;
                    grab_assetName = a.name;
                    grab_assetVersion = a.version;
                    grab_assetImage = a.imageUrl;
                    grab_assetPlatform = a.platform;

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
                    Con.Msg($"Downloaded VRCA for {grab_assetName}.\nLocated in {subdir}");
                    VRCUiManager.prop_VRCUiManager_0.InformHudText($"Downloaded VRCA for {grab_assetName}", Color.white);
                };
                new Thread(DLVRCA).Start();
            } catch {
                Con.Error("Failed to download VRCA");
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
                        grab_SELF_assetPlatform + "_" + grab_SELF_assetName + "_V" + grab_SELF_assetVersion + ".vrca"))) {
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
                    Con.Msg($"Downloaded VRCA for {grab_SELF_assetName}");
                    VRCUiManager.prop_VRCUiManager_0.InformHudText($"Downloaded VRCA for {grab_SELF_assetName}", Color.white);
                };
                new Thread(DLSELF_VRCA).Start();
            }
            catch {
                Con.Error("Failed to download VRCA");
                VRCUiManager.prop_VRCUiManager_0.InformHudText("Failed to download own VRCA", Color.white);
            }
        }

        public static ApiAvatar SelPAvatar() {
            var a = PlayerManager.field_Private_Static_PlayerManager_0.GetPlayer(PlayerWrappers.GetSelectedAPIUser().id)._vrcplayer;
            if (a.prop_ApiAvatar_1.platform.ToLower().Contains("windows"))
                return a.prop_ApiAvatar_1;
            return a.prop_ApiAvatar_0;
        }

        public static StreamWriter CreateOrAppendToFile(string final) {
            if (File.Exists(final))
                return File.AppendText(final);
            return File.CreateText(final);
        }

        public static void LogAsset() {
            string subdir = $"{MintCore.MintDirectory}\\Logs\\";
            if (!Directory.Exists(subdir))
                Directory.CreateDirectory(subdir);


            VRCPlayer p = PlayerManager.field_Private_Static_PlayerManager_0.GetPlayer(PlayerWrappers.GetSelectedAPIUser().id)._vrcplayer;

            bool windows = p.field_Private_ApiAvatar_1.platform.ToLower().Contains("Windows");

            APIUser u = PlayerManager.field_Private_Static_PlayerManager_0.GetPlayer(PlayerWrappers.GetSelectedAPIUser().id).field_Private_APIUser_0;
            ApiAvatar a = windows ? p.prop_ApiAvatar_1 : p.prop_ApiAvatar_0;

            string playerName, playerStatus, userID, avatarID, assetURL, avatarName, authorID, releaseStatus, playerBio;
            int version;

            playerName = u.displayName;
            playerStatus = u.status;
            userID = u.id;
            avatarID = a.id;
            assetURL = a.assetUrl;
            avatarName = a.name;
            authorID = a.authorId;
            version = a.version;
            releaseStatus = a.releaseStatus;
            playerBio = u.bio;

            string LogTimeDate = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff");

            using (StreamWriter sw = CreateOrAppendToFile(subdir + "SelectedUser_Logged.txt")) {
                sw.WriteLine("Log Time:        " + LogTimeDate);
                sw.WriteLine("Player Name:     " + playerName);
                sw.WriteLine("User ID:         " + userID);
                sw.WriteLine("Player Status:   " + playerStatus);
                sw.WriteLine("Player Bio:      " + playerBio);
                sw.WriteLine("");
                sw.WriteLine("Avatar ID:       " + avatarID);
                sw.WriteLine("Asset URL:       " + assetURL);
                sw.WriteLine("Avatar Name:     " + avatarName);
                sw.WriteLine("Author ID:       " + authorID);
                sw.WriteLine("Version:         " + version);
                sw.WriteLine("Release Status:  " + releaseStatus);
                sw.WriteLine("==================================================================");
                sw.WriteLine("");
            }

            Con.Msg($"Logged {playerName}, Located in {subdir}\\SelectedUser_Logged.txt");
            VRCUiManager.prop_VRCUiManager_0.InformHudText($"Logged {playerName}", Color.white);
        }
    }
}
