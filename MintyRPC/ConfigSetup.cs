using Newtonsoft.Json;

namespace MintyRPC;

public class JsonSetup {
    [JsonProperty("General Settings")]
    public List<General> GeneralSettings { get; set; }
    [JsonProperty("Presence Details")]
    public List<PresenceInfo> PresenceInfo { get; set; }
}

public class General {
    [JsonProperty("AutoStartOnApplicationOpen")]
    public bool AutoStart { get; set; }
    [JsonProperty("AutoRestartIfCrash")]
    public bool AutoRestart { get; set; }
}

public class PresenceInfo {
    [JsonProperty("PresenceID")]
    public ulong PresenceId { get; set; }
    [JsonProperty("Details")]
    public string? Details { get; set; }
    [JsonProperty("State")]
    public string? State { get; set; }
    [JsonProperty("LargeImageKey")]
    public string? LargeImageKey { get; set; }
    [JsonProperty("LargeImageTooltipText")]
    public string? LargeImageTooltipText { get; set; }
    [JsonProperty("SmallImageKey")]
    public string? SmallImageKey { get; set; }
    [JsonProperty("SmallImageTooltipText")]
    public string? SmallImageTooltipText { get; set; }
    [JsonProperty("StartTimestamp")]
    public long StartTimestamp { get; set; }
    [JsonProperty("CurrentNumber")]
    public int CurrentSize { get; set; }
    [JsonProperty("MaxNumber")]
    public int MaxSize { get; set; }
    [JsonProperty("LobbyID")]
    public string? LobbyId { get; set; }
}

public static class ConfigSetup {
    public static JsonSetup Config { get; internal set; } = Load();
    public static string? staticDetails, staticState;
    
    public static void Setup() {
        if (File.Exists($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Presence.json")) return;

        var general = new General {
            AutoStart = false,
            AutoRestart = false
        };
        
        var info = new PresenceInfo {
            PresenceId = 702767245385924659,
            Details = "Being cute",
            State = "And adorable",
            LargeImageKey = "mint",
            LargeImageTooltipText = "MintyRPC",
            SmallImageKey = "lilylove",
            SmallImageTooltipText = "You're loved",
            StartTimestamp = 0,
            CurrentSize = 69,
            MaxSize = 420,
            LobbyId = ""
        };

        var config = new JsonSetup {
            GeneralSettings = new List<General> { general },
            PresenceInfo = new List<PresenceInfo> { info }
        };
        
        File.WriteAllText($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Presence.json", JsonConvert.SerializeObject(config, Formatting.Indented));
    }

    private static JsonSetup Load() {
        Setup();
        var d = JsonConvert.DeserializeObject<JsonSetup>(File.ReadAllText($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Presence.json"));
        return d ?? throw new Exception();
    }

    public static void Save() {
        File.WriteAllText($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Presence.json", JsonConvert.SerializeObject(Config, Formatting.Indented));
        staticDetails = GetPresenceInfo().Details;
        staticState = GetPresenceInfo().State;
    }

    public static PresenceInfo GetPresenceInfo() => Config.PresenceInfo.Single(x => x.PresenceId != 0);
    public static General GetGeneralnfo() => Config.GeneralSettings[0];
}