﻿using MelonLoader;
using System;
using System.Collections;
using System.Diagnostics;
using MintMod.Functions.Authentication;
using UnityEngine;
using MintyLoader;
using MintMod.Resources;
using BuildInfo = MintyLoader.BuildInfo;

namespace MintMod.Utils {
    internal class General : MintSubMod {
        private static readonly int setMaxFrameRate240 = 240;

        public static void SetFrameRate() {
            try {
                if (Config.MaxFrameRate.Value >= 240)
                    UnityEngine.Application.targetFrameRate = setMaxFrameRate240;
                else if (Config.MaxFrameRate.Value < 90)
                    UnityEngine.Application.targetFrameRate = 90;
                else
                    UnityEngine.Application.targetFrameRate = Config.MaxFrameRate.Value;
            } catch { Con.Error("Failed to set a new FrameRate Lock"); }
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

        internal override void OnLevelWasLoaded(int buildindex, string sceneName) {
            if (Config.ShowWelcomeMessages.Value && buildindex == -1)
                StartWelcomeMessage();
        }

        private static bool LoadedOneTime = true;

        private static void StartWelcomeMessage() {
            if (!LoadedOneTime) return;
            MelonCoroutines.Start(Welcome());
            LoadedOneTime = false;
        }

        private static IEnumerator Welcome() {
            yield return new WaitForSeconds(10);
            string data = Config.useFakeName.Value ? Config.FakeName.Value : ServerAuth.MintyData.Name;
            Con.Msg($"Welcome back, {data}");
            VrcUiPopups.Notify(MintCore.ModBuildInfo.Name, $"Welcome back, {data}", MintyResources.MintIcon);
        }

        #endregion
    }
}
