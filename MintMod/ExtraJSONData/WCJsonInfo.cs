using System.Net.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using MintMod.Libraries;
using MintMod.Resources;
using MintMod.Utils;
using ReMod.Core.VRChat;
using VRC;

namespace MintMod.ExtraJSONData;

public class MonkePlayerDataNumberOne {
    [JsonProperty("Username")] public string UserName { get; }
    [JsonProperty("UsrId")] public string UserId { get; }
    [JsonProperty("Rank")] public string Rank { get; }
    [JsonProperty("CustomTag")] public string CustomTag { get; }
    [JsonProperty("R")] public float Red { get; }
    [JsonProperty("G")] public float Green { get; }
    [JsonProperty("B")] public float Blue { get; }
}

public static class MonkeShitMethods {
    internal static List<MonkePlayerDataNumberOne> WorldClientJsonData { get; set; } = Load();

    private static HttpClient _http;

    private static List<MonkePlayerDataNumberOne> Load() {
        if (WorldClientJsonData != null) return WorldClientJsonData;
        _http = new HttpClient();
        var jsonString = _http.GetStringAsync("https://raw.githubusercontent.com/Hacker1254/WorldClient-Files/main/Peoples.json").GetAwaiter().GetResult();
        var d = JsonConvert.DeserializeObject<List<MonkePlayerDataNumberOne>>(jsonString);
        _http.Dispose();
        return d ?? throw new Exception();
    }

    public static void OnPlayerJoin(Player player) {
        if (WorldClientJsonData.Any(x => x.UserId.Contains(player.GetAPIUser().id))) {
            VrcUiPopups.Notify("Mint Mod", $"A known World Client monke has joined the instance\n{player.GetAPIUser().displayName}", MintyResources.Megaphone, 
                ColorConversion.HexToColor("F60B0E"), 5f);
        }
    }
}