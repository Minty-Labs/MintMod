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
        var currentTime = (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        var finalTime = ConfigSetup.Config.StartTimestamp == 123456789 ? currentTime : ConfigSetup.Config.StartTimestamp;
        _time = finalTime;
        _randomLobbyId = GenerateRandomString(69);
        
        //var clientID = Environment.GetEnvironmentVariable("702767245385924659") ?? "702767245385924659";
        _discord = new Discord.Discord(702767245385924659/*Int64.Parse(clientID)*/, (UInt64)Discord.CreateFlags.Default);
        
        _discord.SetLogHook(Discord.LogLevel.Debug, (level, message) => {
            Log.Status($"Rich Presence has started with code: {message}");
        });
        
        _applicationManager = _discord.GetApplicationManager();
        
        UpdateActivity();
        
        _loopThread.Start();
        _isRunning = true;
        return CommandResult.Okay;
    }
    
    private static CommandResult SetDetails(string command, Arguments args) {
        var sb = new StringBuilder();
        var words = args.GetAll();
        foreach (var word in words) {
            sb.Append($"{word} ").Replace("setdetails ", "");
        }
        
        ConfigSetup.Config.Details = sb.ToString();
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
        
        ConfigSetup.Config.State = sb.ToString();
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
        
        ConfigSetup.Config.LargeImageKey = sb.ToString();
        ConfigSetup.Save();
        
        UpdateActivity();
        
        _activityManager?.UpdateActivity(_activity, result => Log.Info("Updated activity"));
        return CommandResult.Okay;
    }
    
    private static CommandResult SetLargeImageText(string command, Arguments args) {
        var sb = new StringBuilder();
        var words = args.GetAll();
        foreach (var word in words) {
            sb.Append($"{word} ").Replace("setlargeimagetext ", "");
        }
        
        ConfigSetup.Config.LargeImageTooltipText = sb.ToString();
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
        
        ConfigSetup.Config.SmallImageKey = sb.ToString();
        ConfigSetup.Save();
        
        UpdateActivity();
        
        _activityManager?.UpdateActivity(_activity, result => Log.Info("Updated activity"));
        return CommandResult.Okay;
    }
    
    private static CommandResult SetSmallImageText(string command, Arguments args) {
        var sb = new StringBuilder();
        var words = args.GetAll();
        foreach (var word in words) {
            sb.Append($"{word} ").Replace("setsmallimagetext ", "");
        }
        
        ConfigSetup.Config.SmallImageTooltipText = sb.ToString();
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
        
        ConfigSetup.Config.CurrentSize = int.Parse(sb.ToString());
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
        
        ConfigSetup.Config.MaxSize = int.Parse(sb.ToString());
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
                Console.Title = $"{BuildInfo.Name} v{BuildInfo.Version} - Details: {ConfigSetup.Config.Details} - State: {ConfigSetup.Config.State} - Memory Usage: " + Math.Round(GC.GetTotalMemory(false) / 1024f) + " KB";
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

        var noParty = ConfigSetup.Config.CurrentSize == 0 && ConfigSetup.Config.MaxSize == 0;

        if (noParty) {
            _activity = new Activity {
                Name = ConfigSetup.Config.Name ?? "Minty RPC",
                Timestamps = new ActivityTimestamps {
                    Start = _time,
                    End = 0
                },
                Details = ConfigSetup.Config.Details ?? "", /* Top Text */
                State = ConfigSetup.Config.State ?? "",     /* Bottom Text */
                Assets = new ActivityAssets {
                    LargeImage = ConfigSetup.Config.LargeImageKey ?? "",
                    LargeText = ConfigSetup.Config.LargeImageTooltipText ?? "",
                    SmallImage = ConfigSetup.Config.SmallImageKey ?? "",
                    SmallText = ConfigSetup.Config.SmallImageTooltipText ?? ""
                },
                Instance = true
            };
        }
        else {
            _activity = new Activity {
                Name = ConfigSetup.Config.Name ?? "Minty RPC",
                Timestamps = new ActivityTimestamps {
                    Start = _time,
                    End = 0
                },
                Details = ConfigSetup.Config.Details ?? "", /* Top Text */
                State = ConfigSetup.Config.State ?? "",     /* Bottom Text */
                Assets = new ActivityAssets {
                    LargeImage = ConfigSetup.Config.LargeImageKey ?? "",
                    LargeText = ConfigSetup.Config.LargeImageTooltipText ?? "",
                    SmallImage = ConfigSetup.Config.SmallImageKey?? "",
                    SmallText = ConfigSetup.Config.SmallImageTooltipText ?? ""
                },
                Party = {
                    Id = _randomLobbyId!,
                    Size = {
                        CurrentSize = ConfigSetup.Config.CurrentSize,
                        MaxSize = ConfigSetup.Config.MaxSize
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
            Console.Title = $"{BuildInfo.Name} v{BuildInfo.Version} - Details: {ConfigSetup.Config.Details} - State: {ConfigSetup.Config.State}";
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