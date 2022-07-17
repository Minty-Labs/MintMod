using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using MelonLoader;
using UnityEngine;

namespace DiscordAddon;

public static class BuildInfo {
    public const string Name = "MintyRPC";
    public const string Version = "1.0.0";
    public const string Author = "Lily";
    public const string Company = "Minty Labs";
    public const string Description = "Discord Rich Presence Addon for MintMod";
    public const string DownloadLink = "https://mod.mintlily.lgbt/DiscordAddon.dll";
}

public class MintDiscordAddon : MelonPlugin {
    private static readonly MelonLogger.Instance Logger = new ("MintyRPC", ConsoleColor.Magenta);
    public static bool IsDebug => Environment.CommandLine.Contains("--MintyDev");
    private static bool _hasStarted;
    public static MelonPreferences_Category MintRpc;
    public static MelonPreferences_Entry<string> LogoStyle, CustomText;
    public static MelonPreferences_Entry<bool> Enabled, HideLocation, HideName;
    private static MethodInfo _start, _update, _worldChange;
    private static int _scenesLoaded = 0;

    public override void OnPreInitialization() {
        var http = new HttpClient();
        // var file = Path.Combine(Environment.CurrentDirectory, "UserLibs", "DiscordRPC.dll");
        var file2 = Path.Combine(Environment.CurrentDirectory, /*"UserLibs",*/ "discord_game_sdk.dll");
        // if (!File.Exists(file)) {
        //     var data = http.GetByteArrayAsync("https://mod.mintlily.lgbt/Libs/DiscordRPC.dll").GetAwaiter().GetResult();
        //     File.WriteAllBytes(Path.Combine(Environment.CurrentDirectory, "UserLibs", "DiscordRPC.dll"), data);
        // }
        if (!File.Exists(file2)) {
            var data2 = http.GetByteArrayAsync("https://mod.mintlily.lgbt/Libs/discord_game_sdk.dll").GetAwaiter().GetResult();
            File.WriteAllBytes(Path.Combine(Environment.CurrentDirectory, /*"UserLibs",*/ "discord_game_sdk.dll"), data2);
        }
        
        _hasStarted = true;
        http.Dispose();
    }

    public override void OnApplicationStart() {
        if (!_hasStarted) return;
        // var _256 = SHA256.Create();
        // var file = Path.Combine(Environment.CurrentDirectory, "Plugins", "MintyRPC.dll");
        //
        // var http = new HttpClient();
        // var remotePluginBytes = http.GetByteArrayAsync(BuildInfo.DownloadLink).GetAwaiter().GetResult();
        // var localPluginBytes = File.ReadAllBytes(file);
        //
        // var pluginOnDiskHash = GetHash(_256, localPluginBytes);
        // var pluginRemoteHash = GetHash(_256, remotePluginBytes);
        //
        // http.Dispose();
        //
        // if (pluginRemoteHash != pluginOnDiskHash) {
        //     File.WriteAllBytes(file, remotePluginBytes);
        //     Log(ConsoleColor.Green, "ReModCorePlugin.dll updated!");
        // }
        
        MintRpc = MelonPreferences.CreateCategory("MintMod - Discord Rich Presence");
        Enabled = MintRpc.CreateEntry("enabled", true, "Enable Discord Rich Presence");
        LogoStyle = MintRpc.CreateEntry("logoStyle", "0", "Logo Style / Color");
        UIExpansionKit.API.ExpansionKitApi.RegisterSettingAsStringEnum(MintRpc.Identifier, LogoStyle.Identifier, 
            new List<(string SettingsValue, string DisplayName)> {
                ("0", "Normal"),
                ("1", "Red"),
                ("2", "Blue"),
                ("3", "Green"),
                ("4", "Yellow"),
                ("5", "Purple"),
                ("6", "Orange"),
                ("7", "Pink"),
                ("8", "Cyan"),
                ("9", "Black"),
                ("10", "Mint"),
                ("11", "Trans"),
                ("12", "Pride"),
                ("13", "Bi"),
                ("14", "Lesbian"),
                ("15", "Non-binary"),
                ("16", "Pan")
            });
        CustomText = MintRpc.CreateEntry("customText", "Being adorable", "Custom Text in Details");
        HideLocation = MintRpc.CreateEntry("locationHidden", true, "Hide your location?");
        HideName = MintRpc.CreateEntry("localNameHidden", true, "Hide your name?");

        if (Enabled.Value)
            Manager.Start();
    }

    public override void OnApplicationQuit() {
        if (!_hasStarted) return;
        Manager.OnApplicationQuit();
    }

    public override void OnPreferencesSaved() {
        if (!_hasStarted) return;
        Manager.SettingsUpdated();
    }
    
    /*public override void OnUpdate() {
        if (!_hasStarted) return;
        Manager.Update();
    }*/

    public static void Log(object message) => Logger.Msg(message);
    public static void Log(ConsoleColor color, object message) => Logger.Msg(color, message);
    public static void Error(object message) => Logger.Error(message);
    public static void Warn(object message) => Logger.Warning(message);
    public static void Debug(object message) {
        if (IsDebug) {
            Logger.Msg(ConsoleColor.Cyan, message);
        }
    }
    
    private static string GetHash(HashAlgorithm sha256, byte[] data) {
        var rawBytes = sha256.ComputeHash(data);
        var stringBuilder = new StringBuilder();
        foreach (var b in rawBytes)
            stringBuilder.Append(b.ToString("x2"));

        return stringBuilder.ToString();
    }
}