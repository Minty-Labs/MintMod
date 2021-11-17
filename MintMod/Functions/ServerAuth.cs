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

namespace MintMod.Functions {
    internal class ServerAuth : MintSubMod{
        public override string Name => "Authentication";
        public override string Description => "Deals with authed used for Mint.";

        internal static bool authed, canLoadMod;

        internal static IEnumerator AuthUser() {
            yield return new WaitForSeconds(1);
            while (true) {
                if (APIUser.CurrentUser == null) yield return null;
                else break;
            }

            string UserID = APIUser.CurrentUser.id;
            string url = $"https://mintlily.lgbt/mod/auth/auth.php?userid={UserID}";
            if (MintCore.isDebug)
                MelonLogger.Msg(url);
            WebClient www = new WebClient();
            if (MintCore.isDebug)
                MelonLogger.Msg("www created");
            while (www.IsBusy)
                yield return null;
            string result = www?.DownloadString(url);
            if (MintCore.isDebug)
                MelonLogger.Msg($"Result: {result}");

            if (!string.IsNullOrWhiteSpace(result)) {
                www.Dispose();
                if (MintCore.isDebug)
                    MelonLogger.Msg("www Disposed");
                switch (result) {
                    case "isAuthedAndCanUseMod":
                        MelonLogger.Msg("Authed for MintMod");
                        canLoadMod = true;
                        MelonCoroutines.Start(GetAssembly.YieldUI());
                        yield break;
                    case "canNotUseMod":
                        MelonLogger.Warning("You are not authorized to use the mod, if you think this is a mistake, please Let Lily know.");
                        canLoadMod = false;
                        MelonCoroutines.Start(LoopNoAuth());
                        yield break;
                    case "illegalUseOfMod":
                        canLoadMod = false;
                        MelonCoroutines.Start(LoopNoAuth());
                        break;
                }
            } else canLoadMod = false;
            if (MintCore.isDebug)
                MelonLogger.Msg("result was null or empty");
        }

        static IEnumerator LoopNoAuth() {
            while (true) {
                MelonLogger.Warning("You are not authorized to use this mod.");
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
                        MelonLogger.Msg("Player is authed for Mint");
                        yield break;
                    case "canNotUseMod":
                        MelonLogger.Msg("Player has no auth for Mint");
                        yield break;
                    case "illegalUseOfMod":
                        MelonLogger.Msg("Player is banned from Mint");
                        yield break;
                }
            }
            if (MintCore.isDebug)
                MelonLogger.Msg("result was null or empty");
        }
    }
}
