using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using UnityEngine;
using VRC.Core;
using System.Net;
using System.Net.Http;
using MintMod.Libraries;
using MintMod.Managers.Notification;
using MintMod.UserInterface.QuickMenu;
using MintyLoader;
using MintMod.Utils;
using Newtonsoft.Json;
using Pastel;

namespace MintMod.Functions.Authentication {
    internal class ServerAuth : MintSubMod {
        public override string Name => "Authentication";
        public override string Description => "Deals with authed used for Mint.";

        internal static bool canLoadMod;
        internal const string MintAuthJSONURL = "https://api.potato.moe/api-mint/auth"; // From Bono's API
        internal static MintyUser MintyData;

        internal static IEnumerator AuthUser() {
            yield return new WaitForSeconds(1);
            while (true) {
                if (APIUser.CurrentUser == null) yield return null;
                else break;
            }

            try {
                HttpClient w = new();
                w.DefaultRequestHeaders.Add("X-AUTH-TOKEN", APIUser.CurrentUser.id);
                var task = w?.GetStringAsync(MintAuthJSONURL);
                task.Wait();
                w?.Dispose();

                if (!task.IsCompleted || task.Result.Contains("message")) {
                    canLoadMod = false;
                    Con.Debug("result was null or empty", MintCore.isDebug);
                    yield break;
                }
                // else canLoadMod = true

                MintyData = JsonConvert.DeserializeObject<MintyUser>(task.Result);
                if (MintyData == null) {
                    Con.Error("Mint Authentication failed => Auth halting.");
                    yield break;
                }

                if (MintyData.isBanned) {
                    canLoadMod = false;
                    MelonCoroutines.Start(LoopNoAuth());
                    yield break;
                }
                
                if (MintyData.UserID == APIUser.CurrentUser.id ||
                    MintyData.AltAccounts.Any(x => x == APIUser.CurrentUser.id) ||
                    ModCompatibility.GPrivateServer)
                {
                    Con.Msg("Authed for MintMod".Pastel("9fffe3"));
                    canLoadMod = true;
                    MintCore.mods.ForEach(u => {
                        try { u.OnUserInterface(); }
                        catch (Exception e) { Con.Error(e); }
                    });
                    MelonCoroutines.Start(MintUserInterface.OnQuickMenu());
                    MelonCoroutines.Start(MintUserInterface.OnSettingsPageInit());
                }
            } catch (Exception r) {
                canLoadMod = false;
                Con.Error(r);
            }
        }

        static IEnumerator LoopNoAuth() {
            while (true) {
                Con.Warn("You are not authorized to use this mod.");
                yield return new WaitForSeconds(60);
                Environment.Exit(0);
            }
        }

        internal static IEnumerator SimpleAuthCheck(string id) {
            HttpClient e = new();
            e.DefaultRequestHeaders.Add("X-AUTH-TOKEN", id);
            var task = e.GetStringAsync(MintAuthJSONURL);
            task.Wait();
            e.Dispose();

            if (!task.IsCompleted || task.Result.Contains("message")) {
                Con.Msg("Player has no auth for Mint");
                VRCUiPopups.Notify("Player has no auth for Mint", NotificationSystem.Alert);
                yield break;
            }

            var d = JsonConvert.DeserializeObject<MintyUser>(task.Result);
            if (d == null) {
                Con.Error("Mint Authentication failed => Auth halting.");
                yield break;
            }

            if (d.isBanned) {
                Con.Msg("Player is banned from Mint");
                VRCUiPopups.Notify("Player is banned from Mint", NotificationSystem.Alert);
                yield break;
            }
            Con.Msg("Player is authed for Mint");
            VRCUiPopups.Notify("Player is authed for Mint", NotificationSystem.Alert);
        }
    }
}
