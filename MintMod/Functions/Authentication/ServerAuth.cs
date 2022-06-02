using System;
using System.Collections;
using System.Linq;
using MelonLoader;
using UnityEngine;
using VRC.Core;
using System.Net.Http;
using MintMod.Libraries;
using MintMod.Resources;
using MintMod.UserInterface.QuickMenu;
using MintyLoader;
using MintMod.Utils;
using Newtonsoft.Json;
using Pastel;
using BuildInfo = MintyLoader.BuildInfo;

namespace MintMod.Functions.Authentication {
    internal class ServerAuth : MintSubMod {
        public override string Name => "Authentication";
        public override string Description => "Deals with authed used for Mint.";

        internal static bool CanLoadMod;
        protected const string MintAuthJsonUrl = "https://api.potato.moe/api-mint/auth"; // From Bono's API
        internal static MintyUser MintyData;

        internal static IEnumerator AuthUser() {
            yield return new WaitForSeconds(1);
            while (APIUser.CurrentUser == null && !APIUser.IsLoggedIn) yield return null;

            try {
                HttpClient onStartAuth = new();
                onStartAuth.DefaultRequestHeaders.Add("X-AUTH-TOKEN", APIUser.CurrentUser?.id);
                var task = onStartAuth.GetStringAsync(MintAuthJsonUrl);
                task.Wait();
                onStartAuth.Dispose();

                if (!task.IsCompleted || task.Result.Contains("message")) {
                    CanLoadMod = false;
                    Con.Debug("result was null or empty", MintCore.IsDebug);
                    yield break;
                }

                MintyData = JsonConvert.DeserializeObject<MintyUser>(task.Result);
                if (MintyData == null) {
                    Con.Error("Mint Authentication failed => Auth halting.");
                    yield break;
                }

                if (MintyData.IsBanned) {
                    CanLoadMod = false;
                    MelonCoroutines.Start(LoopNoAuth());
                    yield break;
                }

                if (MintyData.UserId != APIUser.CurrentUser?.id && 
                    MintyData.AltAccounts.Any(x => x != APIUser.CurrentUser.id) && 
                    !ModCompatibility.GPrivateServer) yield break;
                
                Con.Msg("Authed for MintMod".Pastel("9fffe3"));
                CanLoadMod = true;
                MintCore.Modules.ForEach(u => {
                    try { u.OnUserInterface(); }
                    catch (Exception e) { Con.Error(e); }
                });
                MelonCoroutines.Start(MintUserInterface.OnQuickMenu());
                // MelonCoroutines.Start(MintUserInterface.OnSettingsPageInit());
            } catch (Exception r) {
                if (ModCompatibility.GPrivateServer) {
                    Con.Msg("Authed for MintMod via PrivateServer".Pastel("9fffe3"));
                    CanLoadMod = true;
                    MintCore.Modules.ForEach(u => {
                        try { u.OnUserInterface(); }
                        catch (Exception e) { Con.Error(e); }
                    });
                    MelonCoroutines.Start(MintUserInterface.OnQuickMenu());
                    // MelonCoroutines.Start(MintUserInterface.OnSettingsPageInit());
                }
                else {
                    CanLoadMod = false;
                    Con.Error(r);
                }
            }
        }

        private static IEnumerator LoopNoAuth() {
            while (true) {
                Con.Warn("You are not authorized to use this mod.");
                yield return new WaitForSeconds(60);
                Environment.Exit(0);
            }
        }

        internal static IEnumerator SimpleAuthCheck(string id) {
            if (ModCompatibility.GPrivateServer) {
                Con.Warn("Cannot check while on PrivateServer");
                VrcUiPopups.Notify(BuildInfo.Name, "Cannot check while on PrivateServer", MintyResources.Alert);
                yield break;
            }
            HttpClient oneTimeAuthCheck = new();
            oneTimeAuthCheck.DefaultRequestHeaders.Add("X-AUTH-TOKEN", id);
            var task = oneTimeAuthCheck.GetStringAsync(MintAuthJsonUrl);
            task.Wait();
            oneTimeAuthCheck.Dispose();

            if (!task.IsCompleted || task.Result.Contains("message")) {
                Con.Msg("Player has no auth for Mint");
                VrcUiPopups.Notify(BuildInfo.Name, "Player has no auth for Mint", MintyResources.Lock);
                yield break;
            }

            var mintyUser = JsonConvert.DeserializeObject<MintyUser>(task.Result);
            if (mintyUser == null) {
                Con.Error("Mint Authentication failed => Auth halting.");
                yield break;
            }

            if (mintyUser.IsBanned) {
                Con.Msg("Player is banned from Mint");
                VrcUiPopups.Notify(BuildInfo.Name, "Player is banned from Mint", MintyResources.Lock);
                yield break;
            }
            Con.Msg("Player is authed for Mint");
            VrcUiPopups.Notify(BuildInfo.Name, "Player is authed for Mint", MintyResources.Key);
        }
    }
}
