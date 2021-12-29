using System;
using MelonLoader;
using System.Drawing;
using Pastel;

namespace MintyLoader {
    public class Con {
        private static int errorCount, stackErrorCount;
        public static readonly MelonLogger.Instance Logger = new MelonLogger.Instance("MintMod", ConsoleColor.White);

        public static void Msg(string s) => Logger.Msg(s);

        public static void Msg(ConsoleColor c, string s) => Logger.Msg(c, s);

        public static void Error(string s) {
            if (errorCount < 255) {
                Logger.Error(s);
                errorCount++;
            }
            if (errorCount == 255) {
                Logger.Error("The error limit has been reached.");
                errorCount++;
            }
        }

        public static void Error(object stack) {
            if (stackErrorCount < 20) {
                Logger.Error(stack);
                stackErrorCount++;
            }
            if (stackErrorCount == 20) {
                Logger.Error("The error limit has been reached.");
                stackErrorCount++;
            }
        }

        public static void Error(object stack, object trace) => Logger.Error($"=== STACK ===\n{stack}\n=== TRACE ==={trace}");

        public static void Debug(string s, bool isDebug = false) {
            if (Environment.CommandLine.Contains("--MintyDev") || isDebug || MintyLoader.isDebug)
                Logger.Msg(ConsoleColor.Cyan, s);
        }

        public static void Warn(string s) => Logger.Warning(s);
    }
}
