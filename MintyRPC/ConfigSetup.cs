using Newtonsoft.Json;

namespace MintyRPC;

public class Info {
    [JsonProperty("Name")]
    public string? Name { get; set; }
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
}

public static class ConfigSetup {
    public static Info Config { get; internal set; } = Load();
    
    public static void Setup() {
        if (File.Exists($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Presence.json")) return;
        
        var config = new Info {
            Name = "Minty Labs",
            Details = "Being cute",
            State = "And adorable",
            LargeImageKey = "vrc_mint",
            LargeImageTooltipText = "",
            SmallImageKey = "mint",
            SmallImageTooltipText = "MintyRPC",
            StartTimestamp = 0,
            CurrentSize = 69,
            MaxSize = 420
        };
        
        File.WriteAllText($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Presence.json", JsonConvert.SerializeObject(config, Formatting.Indented));
    }

    private static Info Load() {
        Setup();
        var d = JsonConvert.DeserializeObject<Info>(File.ReadAllText($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Presence.json"));
        return d ?? throw new Exception();
    }

    public static void Save() => File.WriteAllText($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Presence.json", JsonConvert.SerializeObject(Config, Formatting.Indented));
}