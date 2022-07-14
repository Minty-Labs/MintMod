using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MintMod.Libraries;
using MintMod.Reflections;
using MintMod.Resources;
using MintMod.Utils;
using MintyLoader;
using ReMod.Core.VRChat;
using VRC;
using VRC.Core;
using VRC.SDKBase;

namespace MintMod.Functions {
    internal static class PlayerActions {
        public static async Task AvatarDownload(ApiAvatar apiAvatar = null) {
            try {
                var vrcaPath = $"{MintCore.MintDirectory}\\Assets\\VRCA\\";
                apiAvatar ??= SelPAvatar();

                var grabAssetUrl = apiAvatar.assetUrl;
                var grabAssetName = apiAvatar.name;
                var grabAssetVersion = apiAvatar.version;
                var grabAssetImage = apiAvatar.imageUrl;
                var grabAssetPlatform = apiAvatar.platform;
                
                if (!Directory.Exists(Path.Combine(vrcaPath)))
                    Directory.CreateDirectory(Path.Combine(vrcaPath));

                var vrcaFile = $"{vrcaPath}{grabAssetPlatform}_{grabAssetName}_V{grabAssetVersion}.vrca";
                var imageFile = $"{vrcaPath}{grabAssetPlatform}_{grabAssetName}_V{grabAssetVersion}.png";
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:87.0) Gecko/20100101 Firefox/87.0");
                
                if (!File.Exists(vrcaFile)) {
                    var a = httpClient.GetByteArrayAsync(grabAssetUrl).GetAwaiter().GetResult();
                    try { await CustomAsync.File.WriteAllBytesAsync(vrcaFile, a); }
                    catch { File.WriteAllBytes(vrcaFile, a); }
                }

                if (!File.Exists(imageFile)) {
                    var i = httpClient.GetByteArrayAsync(grabAssetImage).GetAwaiter().GetResult();
                    try { await CustomAsync.File.WriteAllBytesAsync(imageFile, i); }
                    catch { File.WriteAllBytes(imageFile, i); }
                }

                httpClient.Dispose();
                Con.Msg($"Downloaded VRCA for {grabAssetName}.\nLocated in {vrcaPath}");
                VrcUiPopups.Notify(MintCore.ModBuildInfo.Name, $"Downloaded VRCA for {grabAssetName}");
            } catch {
                Con.Error("Failed to download VRCA");
                VrcUiPopups.Notify(MintCore.ModBuildInfo.Name, "Failed to Download VRCA", MintyResources.Alert);
            }
        }

        public static void AvatarSelfDownload() {
            if (RoomManager.field_Internal_Static_ApiWorld_0 != null) {
                PlayerWrappers.GetEachPlayer(player => {
                    if (player.prop_APIUser_0.id == APIUser.CurrentUser.id) {
                        Task.Run(async () => {
                            await AvatarDownload(SelPAvatar(APIUser.CurrentUser.id));
                        });
                    }
                });
            }
        }
        
        internal static ApiAvatar SelPAvatar(string id = null) {
            var playerId = string.IsNullOrWhiteSpace(id) ? PlayerWrappers.GetSelectedAPIUser().id : id;
            var a = PlayerManager.field_Private_Static_PlayerManager_0.GetPlayer(playerId)._vrcplayer;
            return a.field_Private_VRCAvatarManager_0.field_Private_AvatarKind_0 == VRCAvatarManager.AvatarKind.Custom ?
                a.field_Private_VRCAvatarManager_0.field_Private_ApiAvatar_0 :
                a.field_Private_VRCAvatarManager_0.field_Private_ApiAvatar_1;
        }

        private static StreamWriter CreateOrAppendToFile(string final) => File.Exists(final) ? File.AppendText(final) : File.CreateText(final);

        public static void LogAsset() {
            var subdir = $"{MintCore.MintDirectory}\\Logs\\";
            if (!Directory.Exists(subdir))
                Directory.CreateDirectory(subdir);

            var u = PlayerManager.field_Private_Static_PlayerManager_0.GetPlayer(PlayerWrappers.GetSelectedAPIUser().id).field_Private_APIUser_0;
            var a = SelPAvatar();

            var playerName = u.displayName;
            var playerStatus = u.status;
            var userID = u.id;
            var avatarID = a.id;
            var assetURL = a.assetUrl;
            var avatarName = a.name;
            var authorID = a.authorId;
            var version = a.version;
            var releaseStatus = a.releaseStatus;
            var playerBio = u.bio;
            Il2CppSystem.Collections.Generic.List<string> tags = u.tags;

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
            VrcUiPopups.Notify(MintCore.ModBuildInfo.Name, $"Logged {playerName}");
        }

        public static void Teleport(VRCPlayer player) => PlayerWrappers.GetLocalVRCPlayer().transform.position = player.transform.position;

        #region Jump Manager

        // public static int JumpNum = 3;

        private static bool JumpInput() {
            // var input = VRCInputManager.Method_Public_Static_VRCInput_String_0("Jump");
            // var j = JumpNum switch {
            //     1 => input.prop_Boolean_0,
            //     2 => input.prop_Boolean_1,
            //     3 => input.prop_Boolean_2,
            //     4 => input.prop_Boolean_3,
            //     5 => input.prop_Boolean_4,
            //     6 => input.field_Private_Boolean_0,
            //     7 => input.field_Private_Boolean_1,
            //     8 => input.Method_Public_Boolean_0(),
            //     9 => input.Method_Public_Boolean_1(),
            //     10 => input.Method_Public_Boolean_2(),
            //     _ => input.prop_Boolean_0
            // };
            //
            // return j;
            return VRCInputManager.Method_Public_Static_VRCInput_String_0("Jump").prop_Boolean_0;
        }

        internal static bool InfiniteJump;
        private static void JumpyJump() {
            if (InfiniteJump && JumpInput() && !Networking.LocalPlayer.IsPlayerGrounded()) {
                var velocity = Networking.LocalPlayer.GetVelocity();
                velocity.y = Networking.LocalPlayer.GetJumpImpulse() + 1f;
                Networking.LocalPlayer.SetVelocity(velocity);
            }
        }

        // private static void GravityChange(bool state) => Networking.LocalPlayer.SetGravityStrength(state ? 0 : 1);

        public static void UpdateJump() {
            if (InfiniteJump) JumpyJump();
        }

        #endregion
        
        public static void AddJump() {
            try {
                VRCPlayer.field_Internal_Static_VRCPlayer_0.field_Private_VRCPlayerApi_0.SetJumpImpulse(2.8f);
            } catch (Exception ex) {
                Con.Error($"Adding Jumping to current world has encountered an Error:\n{ex}");
            }
        }
    }
}
