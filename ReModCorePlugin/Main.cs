using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using MelonLoader;
using BuildInfo = ReModCorePlugin.BuildInfo;

[assembly: AssemblyDescription(BuildInfo.Description)]
[assembly: AssemblyCopyright($"Created by {BuildInfo.Author} from {BuildInfo.Company} Copyright © 2022")]
[assembly: MelonInfo(typeof(ReModCorePlugin.Main), BuildInfo.Name, BuildInfo.Version, BuildInfo.Author, BuildInfo.DownloadLink)]
[assembly: MelonGame("VRChat", "VRChat")]

namespace ReModCorePlugin;

public static class BuildInfo {
    public const string Name = "ReModCorePlugin";
    public const string Version = "1.0.0";
    public const string Author = "Lily";
    public const string Company = "Minty Labs";
    public const string Description = "Load and update ReMod.Core easily.";
    public const string DownloadLink = "https://mintlily.lgbt/mod/loader/ReModCorePlugin.dll";
}

public class Main : MelonPlugin {
    private readonly MelonLogger.Instance _logger = new ("ReModCorePlugin", ConsoleColor.Magenta);
    private bool _isRunning;

    public override void OnPreInitialization() {
        if (MelonHandler.Plugins.FindIndex(p => p.Info.Name == "ReMod.Core.Updater") != -1) {
            _logger.Warning("Not running, ReMod.Core.Updater is already loaded!");
            return;
        }

        if (File.Exists(Path.Combine(Environment.CurrentDirectory, "Mods", "ReMod.Loader.dll")) ||
            File.Exists(Path.Combine(Environment.CurrentDirectory, "Mods", "ReModCE.Loader.dll"))) {
            _logger.Warning("Not running, ReMod is already going to load ReMod.Core!");
            return;
        }

        _isRunning = true;

        if (File.Exists("ReMod.Core.dll")) {
            File.Delete("ReMod.Core.dll");
            _logger.Msg("Removed old ReMod.Core.dll from the root directory!");
        }
        
        var _256 = SHA256.Create();

        var http = new HttpClient();
        var remoteCore = http.GetByteArrayAsync("https://github.com/RequiDev/ReMod.Core/releases/latest/download/ReMod.Core.dll").GetAwaiter().GetResult();
        http.Dispose();
        
        var path = Path.Combine(Environment.CurrentDirectory, "UserLibs", "ReMod.Core.dll");

        if (!File.Exists(path)) {
            File.WriteAllBytes(path, remoteCore);
            _logger.Msg(ConsoleColor.Green, "ReMod.Core.dll downloaded and installed.");
        }
        else {
            var coreOnDisk = File.ReadAllBytes(path);
            var coreOnDiskHash = GetHash(_256, coreOnDisk);
            var remoteCoreHash = GetHash(_256, remoteCore);

            if (coreOnDiskHash != remoteCoreHash) {
                File.WriteAllBytes(path, remoteCore);
                return;
            }
            _logger.Msg(ConsoleColor.Magenta, "ReMod.Core already on latest version.");
        }
    }

    public override void OnApplicationStart() {
        if (!_isRunning) return;
        var _256 = SHA256.Create();
        var file = Path.Combine(Environment.CurrentDirectory, "Plugins", "ReModCorePlugin.dll");

        var http = new HttpClient();
        var remotePluginBytes = http.GetByteArrayAsync(BuildInfo.DownloadLink).GetAwaiter().GetResult();
        var localPluginBytes = File.ReadAllBytes(file);
        
        var pluginOnDiskHash = GetHash(_256, localPluginBytes);
        var pluginRemoteHash = GetHash(_256, remotePluginBytes);
        
        http.Dispose();

        if (pluginRemoteHash == pluginOnDiskHash) return;
        File.WriteAllBytes(file, remotePluginBytes);
        _logger.Msg(ConsoleColor.Green, "ReModCorePlugin.dll updated!");
    }

    private static string GetHash(HashAlgorithm sha256, byte[] data) {
        var rawBytes = sha256.ComputeHash(data);
        var stringBuilder = new StringBuilder();
        foreach (var b in rawBytes)
            stringBuilder.Append(b.ToString("x2"));

        return stringBuilder.ToString();
    }
}