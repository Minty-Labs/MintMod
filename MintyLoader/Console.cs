using System;
using MelonLoader;

namespace MintyLoader {
    public static class Con {
        private static int errorCount = 0;

        public static void Msg(string s) => MelonLogger.Msg($"[{BuildInfo.Name}] {s}");

        public static void Msg(ConsoleColor c, string s) => MelonLogger.Msg(c, $"[{BuildInfo.Name}] {s}");

        public static void Error(string s) {
            if (errorCount < 255) {
                MelonLogger.Error($"[{BuildInfo.Name}] {s}");
                errorCount++;
            }
            if (errorCount == 255) {
                MelonLogger.Error("The error limit has been reached.");
                errorCount++;
            }
        }

        public static void Debug(string s, bool isDebug = false) {
            if (!Environment.CommandLine.Contains("--MintyDev") || isDebug) return;
            MelonLogger.Msg(ConsoleColor.Cyan, $"[{BuildInfo.Name}] {s}");
        }

        public static void Warn(string s) => MelonLogger.Warning($"[{BuildInfo.Name}] {s}");
    }
}
