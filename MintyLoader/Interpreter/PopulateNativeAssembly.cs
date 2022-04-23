#if DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Il2CppSystem.IO;

namespace MintyLoader.Interpreter {
    internal static class PopulateNativeAssembly {
        internal static bool Failed;
        internal static void Populate(out IntPtr loadedAssembly) {
            var h = new HttpClient();
            if (!File.Exists(Path.Combine(Environment.CurrentDirectory, "MintyNative.dll"))) {
                var data = h.GetByteArrayAsync("https://mintlily.lgbt/mod/loader/MintyNative.dll").GetAwaiter().GetResult();
                try {
                    try { File.WriteAllBytes(Environment.CurrentDirectory, data); } catch {
                        MintyLoader.InternalLogger.Warning(
                            "Failed to write MintyNative assembly, most likely already being used by another mod or process.");
                    }

                    try {
                        if (!NativeInterpreter.AlreadyLoaded) {
                            loadedAssembly = NativeInterpreter.LoadLibrary("MintyNative.dll");
                            NativeInterpreter.AlreadyLoaded = true;
                        }
                    } catch (Exception e) {
                        Failed = true;
                        MintyLoader.InternalLogger.Error($"Unable to Load Native Dependency, MintyNative:\n{e}");
                    }

                    if (!Failed || !NativeInterpreter.AlreadyLoaded)
                        MintyLoader.InternalLogger.Msg(ConsoleColor.Magenta, "Wrote MintyNative to VRC root directory.");
                } catch (Exception e) {
                    Failed = true;
                    loadedAssembly = IntPtr.Zero;
                    MintyLoader.InternalLogger.Error(e);
                }
                h.Dispose();
            }
            loadedAssembly = IntPtr.Zero;
        }
    }
}
#endif