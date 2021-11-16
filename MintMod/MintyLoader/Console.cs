using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;

namespace MintyLoader
{
    public static class Con
    {
        private static int errorCount = 0;

        public static void Msg(string s) => MelonLogger.Msg(s);

        public static void Error(string s)
        {
            if (errorCount < 255)
            {
                MelonLogger.Error(s);
                errorCount++;
            }
            if (errorCount == 255)
            {
                MelonLogger.Error("The error limit has been reached.");
                errorCount++;
            }
        }

        public static void Debug(string s)
        {
            if (!Environment.CommandLine.Contains("--lolite.debug")) return;
            MelonLogger.Msg(ConsoleColor.Blue, s);
        }

        public static void Warn(string s) => MelonLogger.Warning(s);
    }
}
