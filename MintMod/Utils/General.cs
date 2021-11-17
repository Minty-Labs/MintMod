using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VRC.Core;

namespace MintMod.Utils {
    class General : MintSubMod {
        public static readonly int setMaxFrameRate240 = 240;
        private static int ConfigFramerate = Config.MaxFrameRate.Value;

        public static void SetFrameRate() {
            try {
                if (ConfigFramerate >= 240)
                    UnityEngine.Application.targetFrameRate = setMaxFrameRate240;
                else if (ConfigFramerate < 90)
                    UnityEngine.Application.targetFrameRate = 90;
                else
                    UnityEngine.Application.targetFrameRate = ConfigFramerate;
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
            if (Config.ShowWelcomeMessages.Value)
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
            string user;
            if (APIUser.CurrentUser.displayName == "KortyBoi" || APIUser.CurrentUser.displayName == "Ｋ")
                user = "Lily";
            else if (APIUser.CurrentUser.displayName == "jettsd")
                user = "Emy";
            else
                user = APIUser.CurrentUser.displayName;
            MelonLogger.Msg($"Welcome back, {user}");
            VRCUiManager.prop_VRCUiManager_0.InformHudText($"Welcome back, {user}", Color.white);
        }
        #endregion
    }
}
