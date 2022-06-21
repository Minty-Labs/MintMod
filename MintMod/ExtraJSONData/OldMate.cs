using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MintMod.Libraries;
using Newtonsoft.Json;

namespace MintMod.ExtraJSONData; 

public class Nickname {
    public string UserId { get; set; }

    public string OriginalName { get; set; }

    public string ModifiedName { get; set; }
}

internal class OldMate : MintSubMod {
    public override string Name => "OldMate Json Data";
    public override string Description => "Gets OldMate's JSON data";

    private static List<Nickname> _nicknames = new();
    
    public static bool Contains(string userId) => _nicknames.Where(n => n.UserId == userId).Count() > 0;

    public static string GetModifiedName(string userId) => _nicknames.Where(n => n.UserId == userId).First().ModifiedName;
    
    public static string GetOriginalName(string userId) => _nicknames.Where(n => n.UserId == userId).First().OriginalName;

    internal override void OnStart() {
        if (!ModCompatibility.OldMate) return;
        if (File.Exists(Path.Combine(Environment.CurrentDirectory, "UserData/OldMate.json")))
            _nicknames = JsonConvert.DeserializeObject<List<Nickname>>(File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "UserData/OldMate.json")));
    }
}