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
using MintyLoader;
using MintMod.Utils;

namespace MintMod.Functions {
    internal class ServerAuth : MintSubMod{
        public override string Name => "Authentication";
        public override string Description => "Deals with authed used for Mint.";

        internal static bool canLoadMod;

        internal static IEnumerator AuthUser() {
            yield return new WaitForSeconds(1);
            while (true) {
                if (APIUser.CurrentUser == null) yield return null;
                else break;
            }

            string UserID = APIUser.CurrentUser.id;
            string url = $"https://mintlily.lgbt/mod/auth/auth.php?userid={UserID}";
            Con.Debug(url, MintCore.isDebug);
            WebClient www = new WebClient();
            Con.Debug("www created", MintCore.isDebug);
            while (www.IsBusy)
                yield return null;
            string result = www?.DownloadString(url);
            Con.Debug($"Result: {result}", MintCore.isDebug);

            if (!string.IsNullOrWhiteSpace(result)) {
                www.Dispose();
                Con.Debug("www Disposed", MintCore.isDebug);
                switch (result) {
                    case "isAuthedAndCanUseMod":
                        Con.Msg(ConsoleColor.Green, "Authed for MintMod");
                        canLoadMod = true;
                        MelonCoroutines.Start(GetAssembly.YieldUI());
                        yield break;
                    case "canNotUseMod":
                        Con.Warn("You are not authorized to use the mod, if you think this is a mistake, please Let Lily know.");
                        canLoadMod = false;
                        MelonCoroutines.Start(LoopNoAuth());
                        yield break;
                    case "illegalUseOfMod":
                        canLoadMod = false;
                        MelonCoroutines.Start(LoopNoAuth());
                        break;
                }
            } else canLoadMod = false;
            Con.Debug("result was null or empty", MintCore.isDebug);
        }

        static IEnumerator LoopNoAuth() {
            while (true) {
                Con.Warn("You are not authorized to use this mod.");
                yield return new WaitForSeconds(60);
            }
        }

        internal static IEnumerator SimpleAuthCheck(string id) {
            string url = $"https://mintlily.lgbt/mod/auth/auth.php?userid={id}";
            WebClient www = new WebClient();
            while (www.IsBusy)
                yield return null;
            string result = www?.DownloadString(url);
            if (!string.IsNullOrWhiteSpace(result)) {
                www.Dispose();
                switch (result) {
                    case "isAuthedAndCanUseMod":
                        Con.Msg("Player is authed for Mint");
                        VRCUiManager.prop_VRCUiManager_0.InformHudText("Player is authed for Mint", Color.white);
                        yield break;
                    case "canNotUseMod":
                        Con.Msg("Player has no auth for Mint");
                        VRCUiManager.prop_VRCUiManager_0.InformHudText("Player has no auth for Mint", Color.yellow);
                        yield break;
                    case "illegalUseOfMod":
                        Con.Msg("Player is banned from Mint");
                        VRCUiManager.prop_VRCUiManager_0.InformHudText("Player is banned from Mint", Color.red);
                        yield break;
                }
            }
            Con.Debug("result was null or empty", MintCore.isDebug);
        }
    }
}
