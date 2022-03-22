﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MelonLoader;
using MintMod.Managers.Notification;
using MintMod.Reflections;
using MintMod.Utils;
using MintyLoader;
using UnhollowerRuntimeLib.XrefScans;
using UnityEngine;
using VRC;
using VRC.Core;
using VRC.SDKBase;

namespace MintMod.Functions {
    class PlayerActions {
        public static void AvatarDownload() {
            try {
                ThreadStart DLVRCA = delegate () {
                    string subdir = $"{MintCore.MintDirectory}\\Assets\\VRCA\\";
                    ApiAvatar a = SelPAvatar();

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
                    VRCUiPopups.Notify($"Downloaded VRCA for {grab_assetName}");
                    //VRCUiManager.prop_VRCUiManager_0.InformHudText($"Downloaded VRCA for {grab_assetName}", Color.white);
                };
                new Thread(DLVRCA).Start();
            } catch {
                Con.Error("Failed to download VRCA");
                VRCUiPopups.Notify("Failed to Download VRCA", NotificationSystem.Alert);
                //VRCUiManager.prop_VRCUiManager_0.InformHudText("Failed to download VRCA", Color.white);
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
                    VRCUiPopups.Notify($"Downloaded VRCA fro {grab_SELF_assetName}");
                    //VRCUiManager.prop_VRCUiManager_0.InformHudText($"Downloaded VRCA for {grab_SELF_assetName}", Color.white);
                };
                new Thread(DLSELF_VRCA).Start();
            }
            catch {
                Con.Error("Failed to download VRCA");
                VRCUiPopups.Notify("Failed to download VRCA", NotificationSystem.Alert);
                //VRCUiManager.prop_VRCUiManager_0.InformHudText("Failed to download own VRCA", Color.white);
            }
        }

        public static ApiAvatar SelPAvatar() {
            var a = PlayerManager.field_Private_Static_PlayerManager_0.GetPlayer(PlayerWrappers.GetSelectedAPIUser().id)._vrcplayer;
            if (a.field_Private_VRCAvatarManager_0.field_Private_AvatarKind_0 == VRCAvatarManager.AvatarKind.Custom)
                return a.field_Private_VRCAvatarManager_0.field_Private_ApiAvatar_0;
            return a.field_Private_VRCAvatarManager_0.field_Private_ApiAvatar_1;
        }

        private static StreamWriter CreateOrAppendToFile(string final) {
            if (File.Exists(final))
                return File.AppendText(final);
            return File.CreateText(final);
        }

        public static void LogAsset() {
            string subdir = $"{MintCore.MintDirectory}\\Logs\\";
            if (!Directory.Exists(subdir))
                Directory.CreateDirectory(subdir);

            APIUser u = PlayerManager.field_Private_Static_PlayerManager_0.GetPlayer(PlayerWrappers.GetSelectedAPIUser().id).field_Private_APIUser_0;
            ApiAvatar a = SelPAvatar();

            string playerName, playerStatus, userID, avatarID, assetURL, avatarName, authorID, releaseStatus, playerBio;
            Il2CppSystem.Collections.Generic.List<string> tags;
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
            tags = u.tags;

            var logTimeDate = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff");
            var sb = new StringBuilder();
            foreach (var tag in tags) 
                sb.Append($"{tag}, ");

            using (var sw = CreateOrAppendToFile(subdir + "SelectedUser_Logged.txt")) {
                sw.WriteLine("Log Time:        " + logTimeDate);
                sw.WriteLine("Player Name:     " + playerName);
                sw.WriteLine("User ID:         " + userID);
                sw.WriteLine("Player Status:   " + playerStatus);
                sw.WriteLine("Player Bio:      " + playerBio);
                sw.WriteLine("Player Tags      " + $"[ {sb} ]");
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
            VRCUiPopups.Notify($"Logged {playerName}");
            //VRCUiManager.prop_VRCUiManager_0.InformHudText($"Logged {playerName}", Color.white);
        }

        public static void Teleport(VRCPlayer player) => PlayerWrappers.GetLocalVRCPlayer().transform.position = player.transform.position;

        #region Jump Manager

        internal static bool InfinteJump;
        public static void JumpyJump() {
            if (InfinteJump && VRCInputManager.Method_Public_Static_VRCInput_String_0("Jump").prop_Boolean_0 &&
                !Networking.LocalPlayer.IsPlayerGrounded()) {
                Vector3 velocity = Networking.LocalPlayer.GetVelocity();
                velocity.y = Networking.LocalPlayer.GetJumpImpulse() + 1f;
                Networking.LocalPlayer.SetVelocity(velocity);
            }
        }

        private static void GravityChange(bool state) {
            Networking.LocalPlayer.SetGravityStrength(state ? 0 : 1);
        }

        public static void UpdateJump() {
            if (InfinteJump) JumpyJump();
        }

        #endregion
    }
}
