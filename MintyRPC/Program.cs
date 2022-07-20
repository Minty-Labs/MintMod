using System.Diagnostics;
using Discord;
using System.Text;
using Activity = Discord.Activity;
using Yggdrasil.Logging;
using Yggdrasil.Util.Commands;

namespace MintyRPC;

public static class BuildInfo {
    public const string Name = "MintyRPC";
    public const string Version = "0.0.2";
    public const string Author = "Lily";
    public const string Company = "Minty Labs";
    public static bool IsWindows => Environment.OSVersion.ToString().ToLower().Contains("windows");
}

public class Program {
    private static bool _isRunning, _firstStart;
    private static Discord.Discord? _discord;
    private static Thread _loopThread = new (CallBack);
    private static long _time;
    private static DateTime _startTime;
    private static Activity _activity;
    private static ActivityManager? _activityManager;
    private static ApplicationManager? _applicationManager;
    private static string? _randomLobbyId;
    
    public static void Main(string[] args) {
        if (!BuildInfo.IsWindows) {
            Console.Write("This application is not meant to run on Linux, press any key to exit...");
            Console.ReadLine();
            Process.GetCurrentProcess().Kill();
            return;
        }
        Console.OutputEncoding = Encoding.UTF8;
        Console.Title = $"{BuildInfo.Name} v{BuildInfo.Version} - Not Running";
        Utils.WriteDiscordGameSdkDll();
        ConfigSetup.Setup();

        ConfigSetup.staticDetails = ConfigSetup.GetPresenceInfo().Details;
        ConfigSetup.staticState = ConfigSetup.GetPresenceInfo().State;
        var cmds = new ConsoleCommands();
        
        cmds.Add("start", "Starts the Discord Rich Presence", StartDiscord);
        cmds.Add("setstate", "Sets the state of the Discord Rich Presence", SetState);
        cmds.Add("setdetails", "Sets the details of the Discord Rich Presence", SetDetails);
        cmds.Add("setlargeimage", "Sets the large image of the Discord Rich Presence", SetLargeImage);
        cmds.Add("setlargeimagetext", "Sets the large image text of the Discord Rich Presence", SetLargeImageText);
        cmds.Add("setsmallimage", "Sets the small image of the Discord Rich Presence", SetSmallImage);
        cmds.Add("setsmallimagetext", "Sets the small image text of the Discord Rich Presence", SetSmallImageText);
        cmds.Add("setpartysizecurrent", "Sets the party size current of the Discord Rich Presence", SetPartySizeCurrent);
        cmds.Add("setpartysizemax", "Sets the party size max of the Discord Rich Presence", SetPartySizeMax);
        cmds.Add("kill", "Kills the Discord Rich Presence", KillDiscord);
        cmds.Wait();
        if (ConfigSetup.GetGeneralnfo().AutoStart)
            SetAutoStart("start", new Arguments());
    }

    #region Commands

    private static CommandResult StartDiscord(string command, Arguments args) {
        if (_isRunning) return CommandResult.Break;
        Log.Info("Starting Discord Rich Presence, please wait...");
        
        var processes = Process.GetProcesses();
        var isDiscordAppRunning = processes.Any(p => p.ProcessName is "Discord" or "DiscordPTB" or "DiscordCanary");
        
        if (!isDiscordAppRunning) {
            Log.Warning("Discord is not running.");
            // Process.GetCurrentProcess().Kill();
            return CommandResult.Okay;
        }
        Console.Title = $"{BuildInfo.Name} v{BuildInfo.Version} - Starting, please wait...";
        _startTime = DateTime.Now;
        _time = GetTimeAsLong(ConfigSetup.GetPresenceInfo().StartTimestamp);

        var tempLobbyId = "";
        if (string.IsNullOrWhiteSpace(ConfigSetup.GetPresenceInfo().LobbyId)) {
            tempLobbyId = GenerateRandomString(69);
            ConfigSetup.GetPresenceInfo().LobbyId = tempLobbyId;
            ConfigSetup.Save();
        }
        _randomLobbyId = tempLobbyId;
        
        var clientId = Environment.GetEnvironmentVariable("702767245385924659") ?? "702767245385924659";
        _discord = new Discord.Discord(Int64.Parse(clientId), (UInt64)Discord.CreateFlags.Default);
        
        _discord.SetLogHook(Discord.LogLevel.Debug, (level, message) => {
            Log.Status($"Rich Presence has started with code: {message}");
        });
        
        _applicationManager = _discord.GetApplicationManager();
        
        UpdateActivity();
        
        _loopThread.Start();
        _isRunning = true;
        return CommandResult.Okay;
    }

    private static long GetTimeAsLong(long time) 
        => time switch {
            123456789 => (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds,
            100000001 => (long)DateTime.UtcNow.Subtract(new TimeSpan(_startTime.Hour, _startTime.Minute, _startTime.Second))
                .Subtract(new DateTime(1970, 1, 1)).TotalSeconds,
            1 => (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds,
            _ => ConfigSetup.GetPresenceInfo().StartTimestamp
        };

    private static CommandResult SetDetails(string command, Arguments args) {
        var sb = new StringBuilder();
        var words = args.GetAll();
        foreach (var word in words) {
            sb.Append($"{word} ").Replace("setdetails ", "");
        }
        
        ConfigSetup.GetPresenceInfo().Details = sb.ToString();
        ConfigSetup.Save();
        
        UpdateActivity();
        
        _activityManager?.UpdateActivity(_activity, result => Log.Info("Updated activity"));
        return CommandResult.Okay;
    }

    private static CommandResult SetState(string command, Arguments args) {
        var sb = new StringBuilder();
        var words = args.GetAll();
        foreach (var word in words) {
            sb.Append($"{word} ").Replace("setstate ", "");
        }
        
        ConfigSetup.GetPresenceInfo().State = sb.ToString();
        ConfigSetup.Save();
        
        UpdateActivity();
        
        _activityManager?.UpdateActivity(_activity, result => Log.Info("Updated activity"));
        return CommandResult.Okay;
    }
    
    private static CommandResult SetLargeImage(string command, Arguments args) {
        var sb = new StringBuilder();
        var words = args.GetAll();
        foreach (var word in words) {
            sb.Append($"{word} ").Replace("setlargeimage ", "").Replace(" ", "");
        }
        
        ConfigSetup.GetPresenceInfo().LargeImageKey = sb.ToString().Trim();
        ConfigSetup.Save();
        
        UpdateActivity();
        
        _activityManager?.UpdateActivity(_activity, result => Log.Info("Updated activity"));
        return CommandResult.Okay;
    }
    
    private static CommandResult SetLargeImageText(string command, Arguments args) {
        var sb = new StringBuilder();
        var words = args.GetAll();
        foreach (var word in words) {
            sb.Append($"{word} ").Replace("setlargetooltip ", "");
        }
        
        ConfigSetup.GetPresenceInfo().LargeImageTooltipText = sb.ToString().Trim();
        ConfigSetup.Save();
        
        UpdateActivity();
        
        _activityManager?.UpdateActivity(_activity, result => Log.Info("Updated activity"));
        return CommandResult.Okay;
    }
    
    private static CommandResult SetSmallImage(string command, Arguments args) {
        var sb = new StringBuilder();
        var words = args.GetAll();
        foreach (var word in words) {
            sb.Append($"{word} ").Replace("setsmallimage ", "").Replace(" ", "");
        }
        
        ConfigSetup.GetPresenceInfo().SmallImageKey = sb.ToString().Trim();
        ConfigSetup.Save();
        
        UpdateActivity();
        
        _activityManager?.UpdateActivity(_activity, result => Log.Info("Updated activity"));
        return CommandResult.Okay;
    }
    
    private static CommandResult SetSmallImageText(string command, Arguments args) {
        var sb = new StringBuilder();
        var words = args.GetAll();
        foreach (var word in words) {
            sb.Append($"{word} ").Replace("setsmalltooltip ", "");
        }
        
        ConfigSetup.GetPresenceInfo().SmallImageTooltipText = sb.ToString().Trim();
        ConfigSetup.Save();
        
        UpdateActivity();
        
        _activityManager?.UpdateActivity(_activity, result => Log.Info("Updated activity"));
        return CommandResult.Okay;
    }
    
    private static CommandResult SetPartySizeCurrent(string command, Arguments args) {
        var sb = new StringBuilder();
        var words = args.GetAll();
        foreach (var word in words) {
            sb.Append($"{word} ").Replace("setpartysizecurrent ", "").Replace(" ", "");
        }
        
        ConfigSetup.GetPresenceInfo().CurrentSize = int.Parse(sb.ToString());
        ConfigSetup.Save();
        
        UpdateActivity();
        
        _activityManager?.UpdateActivity(_activity, result => Log.Info("Updated activity"));
        return CommandResult.Okay;
    }
    
    private static CommandResult SetPartySizeMax(string command, Arguments args) {
        var sb = new StringBuilder();
        var words = args.GetAll();
        foreach (var word in words) {
            sb.Append($"{word} ").Replace("setpartysizemax ", "").Replace(" ", "");
        }
        
        ConfigSetup.GetPresenceInfo().MaxSize = int.Parse(sb.ToString());
        ConfigSetup.Save();
        
        UpdateActivity();
        
        _activityManager?.UpdateActivity(_activity, result => Log.Info("Updated activity"));
        return CommandResult.Okay;
    }

    private static CommandResult KillDiscord(string command, Arguments args) {
        _firstStart = false;
        _isRunning = false;
        Console.Title = $"{BuildInfo.Name} v{BuildInfo.Version} - Not Running";
        _activityManager?.ClearActivity(s => Log.Info("Cleared activity"));
        return CommandResult.Okay;
    }
    
    #endregion
    
    private static void CallBack() {
        try {
            while (true) {
                if (!_isRunning) return;
                _discord?.RunCallbacks();
                Console.Title = $"{BuildInfo.Name} v{BuildInfo.Version} - Details: {ConfigSetup.staticDetails} - State: {ConfigSetup.staticState} - Memory Usage: " + Math.Round(GC.GetTotalMemory(false) / 1024f) + " KB";
                Thread.Sleep(1000 / 60);
                // UpdateActivity();
            }
        }
        catch (Exception e) {
            Log.Error($"Unable to callBack: \n{e}");
        }
        finally {
            _discord?.Dispose();
        }
    }

    private static void UpdateActivity() {
        
        _activityManager = _discord?.GetActivityManager();
        var lobbyManager = _discord?.GetLobbyManager();

        var noParty = ConfigSetup.GetPresenceInfo().CurrentSize == 0 && ConfigSetup.GetPresenceInfo().MaxSize == 0;

        if (noParty) {
            _activity = new Activity {
                Name = "MintyRPC",
                Timestamps = new ActivityTimestamps {
                    Start = _time,
                    End = 0
                },
                Details = ConfigSetup.GetPresenceInfo().Details ?? "", /* Top Text */
                State = ConfigSetup.GetPresenceInfo().State ?? "",     /* Bottom Text */
                Assets = new ActivityAssets {
                    LargeImage = ConfigSetup.GetPresenceInfo().LargeImageKey ?? "",
                    LargeText = ConfigSetup.GetPresenceInfo().LargeImageTooltipText ?? "",
                    SmallImage = ConfigSetup.GetPresenceInfo().SmallImageKey ?? "",
                    SmallText = ConfigSetup.GetPresenceInfo().SmallImageTooltipText ?? ""
                },
                Instance = true
            };
        }
        else {
            _activity = new Activity {
                Name = "MintyRPC",
                Timestamps = new ActivityTimestamps {
                    Start = _time,
                    End = 0
                },
                Details = ConfigSetup.GetPresenceInfo().Details ?? "", /* Top Text */
                State = ConfigSetup.GetPresenceInfo().State ?? "",     /* Bottom Text */
                Assets = new ActivityAssets {
                    LargeImage = ConfigSetup.GetPresenceInfo().LargeImageKey ?? "",
                    LargeText = ConfigSetup.GetPresenceInfo().LargeImageTooltipText ?? "",
                    SmallImage = ConfigSetup.GetPresenceInfo().SmallImageKey?? "",
                    SmallText = ConfigSetup.GetPresenceInfo().SmallImageTooltipText ?? ""
                },
                Party = {
                    Id = _randomLobbyId!,
                    Size = {
                        CurrentSize = ConfigSetup.GetPresenceInfo().CurrentSize,
                        MaxSize = ConfigSetup.GetPresenceInfo().MaxSize
                    }
                },
                Instance = true
            };
        }
            
        _activityManager?.UpdateActivity(_activity, result => {
            Log.Info("Activity updated: {0}", result);
            
            if (_firstStart) return;
            
            var t = DateTime.Now - _startTime;
            Log.Status($"Discord Rich Presence started in {t.Milliseconds}ms.");
            Console.Title = $"{BuildInfo.Name} v{BuildInfo.Version} - Details: {ConfigSetup.staticDetails} - State: {ConfigSetup.staticState}";
            _firstStart = true;
        });
    }
    
    private static string GenerateRandomString(int length) {
        const string text = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var array = new char[length];
        var random = new Random();
        for (var i = 0; i < length; i++)  {
            array[i] = text[random.Next(text.Length)];
        }
        return new string(array);
    }
}