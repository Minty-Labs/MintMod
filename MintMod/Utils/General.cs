using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MintMod.Functions.Authentication;
using UnityEngine;
using VRC.Core;
using MintyLoader;
using System.Net.Http;

namespace MintMod.Utils {
    class General : MintSubMod {
        public static readonly int setMaxFrameRate240 = 240;

        public static void SetFrameRate() {
            try {
                if (Config.MaxFrameRate.Value >= 240)
                    UnityEngine.Application.targetFrameRate = setMaxFrameRate240;
                else if (Config.MaxFrameRate.Value < 90)
                    UnityEngine.Application.targetFrameRate = 90;
                else
                    UnityEngine.Application.targetFrameRate = Config.MaxFrameRate.Value;
            } catch { MelonLogger.Error("Failed to set a new FrameRate Lock"); }
        }

        internal static void RestartGame() {
            try {
                Process.Start(Environment.CurrentDirectory + "\\VRChat.exe", Environment.CommandLine.ToString());
            } catch (Exception) {
                new Exception();
            }
            QuitGame();
		}

        internal static void QuitGame() => Process.GetCurrentProcess().Kill();

        public static void SetPriority() {
            using Process p = Process.GetCurrentProcess();
            p.PriorityClass = Config.KeepPriorityHighEnabled.Value ? ProcessPriorityClass.High : ProcessPriorityClass.Normal;
        }

        internal override void OnUserInterface() {
            SetFrameRate();
            SetPriority();
        }

        #region Throwback Welcome

        internal override void OnLevelWasLoaded(int buildindex, string SceneName) {
            if (Config.ShowWelcomeMessages.Value && buildindex == -1)
                StartWelcomeMessage();
        }

        private static bool LoadedOneTime = true;

        public static void StartWelcomeMessage() {
            if (!LoadedOneTime) return;
            HttpClient e = new();
            e.DefaultRequestHeaders.Add("X-AUTH-TOKEN", APIUser.CurrentUser.id);
            var task = e.GetStringAsync(ServerAuth.MintAuthJSONURL);
            task.Wait();
            e.Dispose();

            if (!task.IsCompleted || task.Result.Contains("message")) return;

            var d = Newtonsoft.Json.JsonConvert.DeserializeObject<MintyUser>(task.Result);

            MelonCoroutines.Start(Welcome(d.Name));
            LoadedOneTime = false;
        }

        private static IEnumerator Welcome(string name) {
            yield return new WaitForSeconds(10);
            Con.Msg($"Welcome back, {name}");
            VRCUiManager.prop_VRCUiManager_0.InformHudText($"Welcome back, {name}", Color.white);
        }

        #endregion
    }
}
