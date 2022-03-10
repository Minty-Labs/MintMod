using System;
using System.Net;
using System.Reflection;
using MelonLoader;
using MintyLoader;

namespace MintMod.Utils {
    public class ReMod_Core_Loader {
        internal static bool failed;
        public static void LoadReModCore(out Assembly loadedAssembly) {
            byte[] bytes = null;
            var wc = new WebClient();
            try {
                bytes = wc.DownloadData($"https://github.com/RequiDev/ReMod.Core/releases/latest/download/ReMod.Core.dll");
                loadedAssembly = Assembly.Load(bytes);
                Con.Msg("Successfully Loaded ReMod.Core");
            }
            catch (WebException e) {
                failed = true;
                Con.Error($"Unable to Load Core Dependency, ReMod.Core: {e}");
            }
            catch (BadImageFormatException e) {
                failed = true;
                loadedAssembly = null;
            }
            loadedAssembly = null;
        }
    }
}