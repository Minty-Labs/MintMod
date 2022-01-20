using System;
using System.Collections;
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
using UnityEngine;
using VRCSDK2;
using VRC_MirrorReflection = VRC.SDKBase.VRC_MirrorReflection;

namespace MintMod.Functions {
    class WorldActions : MintSubMod {
        public static void WorldDownload() {
            try {
                ThreadStart DLVRCW = delegate() {
                    string subdir = $"{MintCore.MintDirectory}\\Assets\\VRCW\\";

                    string grab_assetUrl_vrcw = RoomManager.field_Internal_Static_ApiWorldInstance_0.world.assetUrl;
                    string grab_assetName_vrcw = RoomManager.field_Internal_Static_ApiWorldInstance_0.world.name;
                    int grab_assetVersion_vrcw = RoomManager.field_Internal_Static_ApiWorldInstance_0.world.version;
                    string grab_assetImage_vrcw = RoomManager.field_Internal_Static_ApiWorldInstance_0.world.imageUrl;
                    string grab_assetPlatform_vrcw = RoomManager.field_Internal_Static_ApiWorldInstance_0.world.platform;

                    if (!Directory.Exists(Path.Combine(subdir)))
                        Directory.CreateDirectory(Path.Combine(subdir));
                    if (!File.Exists(Path.Combine(subdir,
                        grab_assetPlatform_vrcw + "_" + grab_assetName_vrcw + "_V" + grab_assetVersion_vrcw + ".vrcw"))) {
                        using (WebClient webClient = new WebClient {
                            Headers = {
                                ["User-Agent"] =
                                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:87.0) Gecko/20100101 Firefox/87.0"
                            }
                        }) {
                            webClient.DownloadFile(grab_assetUrl_vrcw,
                                Path.Combine(subdir + grab_assetPlatform_vrcw + "_" + grab_assetName_vrcw + "_V" +
                                             grab_assetVersion_vrcw + ".vrcw"));
                        }
                    }

                    if (!File.Exists(Path.Combine(subdir,
                        grab_assetPlatform_vrcw + "_" + grab_assetName_vrcw + "_V" + grab_assetVersion_vrcw + ".png"))) {
                        using (WebClient webClient = new WebClient {
                            Headers = {
                                ["User-Agent"] =
                                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:87.0) Gecko/20100101 Firefox/87.0"
                            }
                        }) {
                            webClient.DownloadFile(grab_assetImage_vrcw,
                                Path.Combine(subdir + grab_assetPlatform_vrcw + "_" + grab_assetName_vrcw + "_V" +
                                             grab_assetVersion_vrcw + ".png"));
                        }
                    }

                    Con.Msg($"Downloaded VRCW for {grab_assetName_vrcw}\nLocated in {subdir}");
                    VRCUiPopups.Notify($"Downloaded VRCW for {grab_assetName_vrcw}");
                    //VRCUiManager.prop_VRCUiManager_0.InformHudText($"Downloaded VRCW for {grab_assetName_vrcw}", Color.white);
                };
                new Thread(DLVRCW).Start();
            }
            catch {
                Con.Error("Failed to download VRCW");
                VRCUiPopups.Notify("Failed to download VRCW", NotificationSystem.Alert);
                //VRCUiManager.prop_VRCUiManager_0.InformHudText("Failed to download VRCW", Color.white);
            }
        }

        public static StreamWriter CreateOrAppendToFile(string final) {
            if (File.Exists(final))
                return File.AppendText(final);
            return File.CreateText(final);
        }

        public static void LogWorld() {
            try {
                string subdir = $"{MintCore.MintDirectory}\\Logs\\";
                string final = $"{subdir}LoggedWorlds.txt";

                string worldName = RoomManager.field_Internal_Static_ApiWorldInstance_0.world.name;
                string worldAuthor = RoomManager.field_Internal_Static_ApiWorldInstance_0.world.authorName;
                string worldAuthorID = RoomManager.field_Internal_Static_ApiWorldInstance_0.world.authorId;
                string worldID = RoomManager.field_Internal_Static_ApiWorldInstance_0.world.id;
                string worldAssetURL = RoomManager.field_Internal_Static_ApiWorldInstance_0.world.assetUrl;
                int worldAssetVer = RoomManager.field_Internal_Static_ApiWorldInstance_0.world.version;
                string worldPlatform = RoomManager.field_Internal_Static_ApiWorldInstance_0.world.platform;
                string worldReleaseStatus = RoomManager.field_Internal_Static_ApiWorldInstance_0.world.releaseStatus;
                bool worldIsAdminApproved = RoomManager.field_Internal_Static_ApiWorldInstance_0.world.isAdminApproved;
                string worldUnityVer = RoomManager.field_Internal_Static_ApiWorldInstance_0.world.unityVersion;
                string worldUnityPackageURL = RoomManager.field_Internal_Static_ApiWorldInstance_0.world.unityPackageUrl;

                string LogTimeDate = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff");
                using (StreamWriter swe = CreateOrAppendToFile(final)) {
                    swe.WriteLine("Log Time:        " + LogTimeDate);
                    swe.WriteLine("World Name:      " + worldName);
                    swe.WriteLine("Author UserName: " + worldAuthor);
                    swe.WriteLine("Author ID:       " + worldAuthorID);
                    swe.WriteLine("ID:              " + worldID);
                    swe.WriteLine("AssetURL:        " + worldAssetURL);
                    swe.WriteLine("AssetVersion:    " + worldAssetVer);
                    swe.WriteLine("Platform:        " + worldPlatform);
                    swe.WriteLine("Release Status:  " + worldReleaseStatus);
                    swe.WriteLine("isAdminApproved: " + worldIsAdminApproved);
                    swe.WriteLine("Unity Version:   " + worldUnityVer);
                    swe.WriteLine("UnityPackage:    " + worldUnityPackageURL);
                    swe.WriteLine("==================================================================");
                    swe.WriteLine("");
                }
                Con.Msg($"Logged World: {worldName}\nLocated in {final}");
                VRCUiPopups.Notify($"Logged world: {worldName}");
                //VRCUiManager.prop_VRCUiManager_0.InformHudText($"Logged world: {worldName}", Color.white);
            }
            catch (Exception w) {
                Con.Error($"{w}");
                VRCUiPopups.Notify("Failed to log world", NotificationSystem.Alert);
                //VRCUiManager.prop_VRCUiManager_0.InformHudText("Failed to log world", Color.white);
            }
        }

        public static WorldReflect.SDKType GetWorldTypeSDK() {
            if (WorldReflect.GetWorld() != null && UnityEngine.Resources.FindObjectsOfTypeAll<VRC.SDK3.Components.VRCSceneDescriptor>().Count > 0)
                return WorldReflect.SDKType.SDK3;
            return WorldReflect.SDKType.SDK2;
        }

        public static bool isWorldSDK3() {
            if (GetWorldTypeSDK() == WorldReflect.SDKType.SDK2 || GetWorldTypeSDK() == WorldReflect.SDKType.NONE)
                return false;
            return true;
        }



        public static List<OriginalMirror> originalMirrors = new List<OriginalMirror>();

        private static LayerMask optimizeMask = new LayerMask {
            value = 263680
        };

        private static LayerMask beautifyMask = new LayerMask {
            value = -1025
        };

        internal override void OnLevelWasLoaded(int buildindex, string SceneName) {
            switch (buildindex) {
                case -1:
                    originalMirrors = new List<OriginalMirror>();
                    foreach (var vrcMirrorReflection in UnityEngine.Resources.FindObjectsOfTypeAll<VRC_MirrorReflection>()) {
                        originalMirrors.Add(new OriginalMirror {
                            MirrorInParent = vrcMirrorReflection,
                            OriginalLayers = vrcMirrorReflection.m_ReflectLayers
                        });
                    }
                    if (WorldReflect.GetWorld() != null && Config.AutoAddJump.Value &&
                        VRCPlayer.field_Internal_Static_VRCPlayer_0.field_Private_VRCPlayerApi_0.GetJumpImpulse() < 1)
                        AddJump();
                    break;
            }
        }

        public static void OptimizeMirrors() {
            if (originalMirrors.Count != 0) {
                foreach (var originalMirror in originalMirrors) {
                    originalMirror.MirrorInParent.m_ReflectLayers = optimizeMask;
                }
            }
        }

        public static void BeautifyMirrors() {
            if (originalMirrors.Count != 0) {
                foreach (var originalMirror in originalMirrors) {
                    originalMirror.MirrorInParent.m_ReflectLayers = beautifyMask;
                }
            }
        }

        public static void RevertMirrors() {
            if (originalMirrors.Count != 0) {
                foreach (var originalMirror in originalMirrors) {
                    originalMirror.MirrorInParent.m_ReflectLayers = originalMirror.OriginalLayers;
                }
            }
        }

        public static void AddJump() {
            try {
                VRCPlayer.field_Internal_Static_VRCPlayer_0.field_Private_VRCPlayerApi_0.SetJumpImpulse(2.8f);
            } catch (Exception ex) {
                Con.Error($"Adding Jumping to current world has encountered an Error:\n{ex}");
            }
        }
    }

    public class OriginalMirror {
        public VRC_MirrorReflection MirrorInParent;

        public LayerMask OriginalLayers;
    }
}
