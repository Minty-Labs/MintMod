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
using MintMod.Managers.Notification;

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
            } catch (Exception e) {
                Con.Error(e);
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
            MelonCoroutines.Start(Welcome());
            LoadedOneTime = false;
        }

        private static IEnumerator Welcome() {
            yield return new WaitForSeconds(10);
            Con.Msg($"Welcome back, {ServerAuth.MintyData.Name}");
            VRCUiPopups.Notify($"Welcome back, {ServerAuth.MintyData.Name}", NotificationSystem.Key);
            //VRCUiManager.prop_VRCUiManager_0.InformHudText($"Welcome back, {ServerAuth.MintyData.Name}", Color.white);
        }

        #endregion
    }
}
