using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MelonLoader;
using MintMod.Managers.Notification;
using MintMod.Reflections;
using MintMod.Utils;
using MintMod.Libraries;
using MintyLoader;
using UnityEngine;
using VRCSDK2;
using VRC_MirrorReflection = VRC.SDKBase.VRC_MirrorReflection;

namespace MintMod.Functions {
    internal class WorldActions : MintSubMod {
        public static async Task WorldDownload() {
            try {
                var vrcwPath = $"{MintCore.MintDirectory}\\Assets\\VRCW\\";

                var grabAssetUrlVrcw = RoomManager.field_Internal_Static_ApiWorldInstance_0.world.assetUrl;
                var grabAssetNameVrcw = RoomManager.field_Internal_Static_ApiWorldInstance_0.world.name;
                var grabAssetVersionVrcw = RoomManager.field_Internal_Static_ApiWorldInstance_0.world.version;
                var grabAssetImageVrcw = RoomManager.field_Internal_Static_ApiWorldInstance_0.world.imageUrl;
                var grabAssetPlatformVrcw = RoomManager.field_Internal_Static_ApiWorldInstance_0.world.platform;
                
                if (!Directory.Exists(Path.Combine(vrcwPath)))
                    Directory.CreateDirectory(Path.Combine(vrcwPath));
                
                var vrcwFile = $"{vrcwPath}{grabAssetPlatformVrcw}_{grabAssetNameVrcw}_V{grabAssetVersionVrcw}.vrcw";
                var imageFile = $"{vrcwPath}{grabAssetPlatformVrcw}_{grabAssetNameVrcw}_V{grabAssetVersionVrcw}.png";
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:87.0) Gecko/20100101 Firefox/87.0");
                
                if (!File.Exists(vrcwFile)) {
                    var w = await httpClient.GetByteArrayAsync(grabAssetUrlVrcw);
                    try { await CustomAsync.File.WriteAllBytesAsync(vrcwFile, w); }
                    catch { File.WriteAllBytes(vrcwFile, w); }
                }

                if (!File.Exists(imageFile)) {
                    var i =  await httpClient.GetByteArrayAsync(grabAssetImageVrcw);
                    try { await CustomAsync.File.WriteAllBytesAsync(imageFile, i); }
                    catch { File.WriteAllBytes(imageFile, i); }
                }

                httpClient.Dispose();
                Con.Msg($"Downloaded VRCW for {grabAssetNameVrcw}.\nLocated in {vrcwPath}");
                VRCUiPopups.Notify($"Downloaded VRCW for {grabAssetNameVrcw}");
            } catch (Exception e) {
                Con.Error($"Failed to download VRCW\n{e}");
                VRCUiPopups.Notify("Failed to download VRCW", NotificationSystem.Alert);
            }
        }

        private static StreamWriter CreateOrAppendToFile(string file) => File.Exists(file) ? File.AppendText(file) : File.CreateText(file);

        public static void LogWorld() {
            try {
                var logPath = $"{MintCore.MintDirectory}\\Logs\\";
                var final = $"{logPath}LoggedWorlds.txt";

                var worldName = RoomManager.field_Internal_Static_ApiWorldInstance_0.world.name;
                var worldAuthor = RoomManager.field_Internal_Static_ApiWorldInstance_0.world.authorName;
                var worldAuthorID = RoomManager.field_Internal_Static_ApiWorldInstance_0.world.authorId;
                var worldID = RoomManager.field_Internal_Static_ApiWorldInstance_0.world.id;
                var worldAssetURL = RoomManager.field_Internal_Static_ApiWorldInstance_0.world.assetUrl;
                var worldAssetVer = RoomManager.field_Internal_Static_ApiWorldInstance_0.world.version;
                var worldPlatform = RoomManager.field_Internal_Static_ApiWorldInstance_0.world.platform;
                var worldReleaseStatus = RoomManager.field_Internal_Static_ApiWorldInstance_0.world.releaseStatus;
                var worldIsAdminApproved = RoomManager.field_Internal_Static_ApiWorldInstance_0.world.isAdminApproved;
                var worldUnityVer = RoomManager.field_Internal_Static_ApiWorldInstance_0.world.unityVersion;
                var worldUnityPackageURL = RoomManager.field_Internal_Static_ApiWorldInstance_0.world.unityPackageUrl;

                var logTimeDate = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff");
                using (var swe = CreateOrAppendToFile(final)) {
                    swe.WriteLine("Log Time:        " + logTimeDate);
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
            }
            catch (Exception w) {
                Con.Error(w);
                VRCUiPopups.Notify("Failed to log world", NotificationSystem.Alert);
            }
        }

        public static WorldReflect.SDKType GetWorldTypeSDK() {
            if (WorldReflect.GetWorld() != null && UnityEngine.Resources.FindObjectsOfTypeAll<VRC.SDK3.Components.VRCSceneDescriptor>().Count > 0)
                return WorldReflect.SDKType.SDK3;
            return WorldReflect.SDKType.SDK2;
        }

        public static bool IsWorldSDK3() => GetWorldTypeSDK() != WorldReflect.SDKType.SDK2 && GetWorldTypeSDK() != WorldReflect.SDKType.NONE;



        private static List<OriginalMirror> originalMirrors = new();

        private static LayerMask optimizeMask = new LayerMask {
            value = 263680
        };

        private static LayerMask beautifyMask = new LayerMask {
            value = -1025
        };

        internal override void OnLevelWasLoaded(int buildindex, string sceneName) {
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
            if (originalMirrors.Count == 0) return;
            foreach (var originalMirror in originalMirrors) {
                originalMirror.MirrorInParent.m_ReflectLayers = optimizeMask;
            }
        }

        public static void BeautifyMirrors() {
            if (originalMirrors.Count == 0) return;
            foreach (var originalMirror in originalMirrors) {
                originalMirror.MirrorInParent.m_ReflectLayers = beautifyMask;
            }
        }

        public static void RevertMirrors() {
            if (originalMirrors.Count == 0) return;
            foreach (var originalMirror in originalMirrors) {
                originalMirror.MirrorInParent.m_ReflectLayers = originalMirror.OriginalLayers;
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
