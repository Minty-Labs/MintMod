using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using VRC.Core;
using UnityEngine.XR;
using VRC;
using static DiscordAddon.MintDiscordAddon;
using Discord;

namespace DiscordAddon;

internal static class Manager {
    private static bool _isRunning;
    private static string _uuid = "", _roomId = "", _roomSecret = "", _displayName = "";
    private static int _playersInRoom = 0;
    
    private static Discord.Discord _discord;
    private static Thread _loopThread = new (CallBack);
    private static long _time;
    private static Activity _activity;
    private static ActivityManager _activityManager;
    
    internal static void Start() {
        var processes = Process.GetProcesses();
        var isDiscordAppRunning = processes.Any(p => p.ProcessName is "Discord" or "DiscordPTB" or "DiscordCanary");
        
        if (!isDiscordAppRunning) {
            Log("Discord is not running.");
            return;
        }

        try {
            _time = (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            _activity = new Activity {
                Name = "VRChat",
                Timestamps = new ActivityTimestamps {
                    Start = _time,
                    End = 0
                },
                Details = CustomText.Value,
                State = "Game is starting...",
                Assets = new ActivityAssets {
                    LargeImage = "vrc_mint",
                    LargeText = "VRChat",
                    SmallImage = "mint",
                    SmallText = "MintMod"
                }
            };
            _discord = new Discord.Discord(702767245385924659, 0);
            
            _activityManager = _discord.GetActivityManager();
            _activityManager.RegisterSteam(438100);
            
            _loopThread.Start();
            _isRunning = true;

            UpdateActivity();
        }
        catch (Exception e) {
            Error($"Unable to start Discord Rich Presence: \n{e}");
        }
    }

    private static void UpdateActivity() {
        Continue();
        _activityManager.UpdateActivity(_activity, result => {
            if (result != Result.Ok) {
                Error($"Activity update failed: {result}");
            }
        });
    }

    private static void CallBack() {
        try {
            for (;;) {
                if (!_isRunning) return;
                _discord.RunCallbacks();
                Thread.Sleep(5000);
                // UpdateActivity();
            }
        }
        catch (Exception e) {
            Error($"Unable to callBack: \n{e}");
        }
    }

    private static void Continue() {
        if (!_isRunning) return;
        var style = int.Parse(LogoStyle.Value);
        Log($"Logo Style: {style}");

        if (style > 17 /*&& style != 91*/) return;
        switch (style) {
            #region LogoStyle
            case 0:
                _activity.Assets.LargeImage = "vrc_normal";
                _activity.Assets.LargeText = "VRChat";
                break;
            case 1:
                _activity.Assets.LargeImage = "vrc_red";
                _activity.Assets.LargeText = "MintMod";
                break;
            case 2:
                _activity.Assets.LargeImage = "vrc_blue";
                _activity.Assets.LargeText = "MintMod";
                break;
            case 3:
                _activity.Assets.LargeImage = "vrc_green";
                _activity.Assets.LargeText = "MintMod";
                break;
            case 4:
                _activity.Assets.LargeImage = "vrc_yellow";
                _activity.Assets.LargeText = "MintMod";
                break;
            case 5:
                _activity.Assets.LargeImage = "vrc_purple";
                _activity.Assets.LargeText = "MintMod";
                break;
            case 6:
                _activity.Assets.LargeImage = "vrc_orange";
                _activity.Assets.LargeText = "MintMod";
                break;
            case 7:
                _activity.Assets.LargeImage = "vrc_pink";
                _activity.Assets.LargeText = "MintMod";
                break;
            case 8:
                _activity.Assets.LargeImage = "vrc_cyan";
                _activity.Assets.LargeText = "MintMod";
                break;
            case 9:
                _activity.Assets.LargeImage = "vrc_black";
                _activity.Assets.LargeText = "MintMod";
                break;
            case 10:
                _activity.Assets.LargeImage = "vrc_mint";
                _activity.Assets.LargeText = "MintMod";
                break;
            case 11:
                _activity.Assets.LargeImage = "vrc_trans";
                _activity.Assets.LargeText = "MintMod";
                break;
            case 12:
                _activity.Assets.LargeImage = "vrc_pride";
                _activity.Assets.LargeText = "MintMod";
                break;
            case 13:
                _activity.Assets.LargeImage = "vrc_bi";
                _activity.Assets.LargeText = "MintMod";
                break;
            case 14:
                _activity.Assets.LargeImage = "vrc_lesbian";
                _activity.Assets.LargeText = "MintMod";
                break;
            case 15:
                _activity.Assets.LargeImage = "vrc_nonbinary";
                _activity.Assets.LargeText = "MintMod";
                break;
            case 16:
                _activity.Assets.LargeImage = "vrc_pan";
                _activity.Assets.LargeText = "MintMod";
                break;
            /*case 91:
                _activity.Assets.LargeImage = "ellylily";
                _activity.Assets.LargeText = "Elly is best girl~";
                break;*/
            #endregion
        }

        if (!IsDebug && style != 0/* || style == 91*/) return;
        _activity.Assets.LargeImage = "MintDev";
        _activity.Assets.LargeText = "MintMod (Dev)";
    }
    
    private static void DeviceRecall() {
        var model = XRDevice.model;
        if (XRDevice.isPresent && model.ToLower().Contains("oculus") || XRDevice.model.ToLower().Contains("rift")) {
            _activity.Assets.LargeImage = "oculus_headset_blk";
            _activity.Assets.LargeText = "Oculus Rift (S) Headset";
        }
        else if (XRDevice.isPresent && model.ToLower().Contains("quest")) {
            _activity.Assets.LargeImage = "oculus_headset_wht";
            _activity.Assets.LargeText = "Oculus Quest Headset";
        }
        else if (XRDevice.isPresent && model.ToLower().Contains("htc") || XRDevice.model.ToLower().Contains("vive")) {
            _activity.Assets.LargeImage = "oculus_headset_vive";
            _activity.Assets.LargeText = "HTC Vive Headset";
        }
        else if (XRDevice.isPresent && model.ToLower().Contains("valve") || XRDevice.model.ToLower().Contains("index")) {
            _activity.Assets.LargeImage = "index_headset";
            _activity.Assets.LargeText = "Valve Index Headset";
        }
        else if (XRDevice.isPresent) {
            _activity.Assets.LargeImage = "generic_headset";
            _activity.Assets.LargeText = "Playing in Virtual Reality";
        }
        else if (!XRDevice.isPresent) {
            _activity.Assets.LargeImage = "desktop";
            _activity.Assets.LargeText = "Player on Desktop";
        }
    }
    
    private static string RoomChanged(string worldName, string worldAndRoomId, string roomIdWithTags, InstanceAccessType accessType, int maxPlayers, int currentPlayers) {
        if (!_isRunning) return null;
        if (!string.IsNullOrWhiteSpace(worldAndRoomId)) {
            if (accessType is InstanceAccessType.InviteOnly or InstanceAccessType.InvitePlus) {
                _activity.State = "In a private world";
                _activity.Party = new ActivityParty {
                    Id = "0",
                    Size = new PartySize {
                        CurrentSize = 0,
                        MaxSize = 0
                    }
                };
                Continue();
                _activity.Timestamps.Start = 0L;
                _activity.Assets.SmallImage = "invitereq";
                _activity.Assets.SmallText = "Try requesting an invite";
            } else {
                var str = " - Unknown -";
                switch (accessType) {
                    case InstanceAccessType.FriendsOfGuests:
                        str = " ~Friends+";
                        DeviceRecall();
                        break;
                    case InstanceAccessType.FriendsOnly:
                        str = " ~Friends";
                        DeviceRecall();
                        break;
                    case InstanceAccessType.Public:
                        str = " ~Public";
                        DeviceRecall();
                        _activity.Assets.SmallText = "Public Instance - Join Me";
                        break;
                    default: 
                        str = " - Unknown -";
                        break;
                }
                _activity.State = "in " + worldName + str;
                if (HideLocation.Value) {
                    str = "Location Hidden";
                    _activity.State = str;
                }
                _activity.Party = new ActivityParty {
                    Id = worldAndRoomId,
                    Size = new PartySize {
                        CurrentSize = currentPlayers,
                        MaxSize = maxPlayers
                    }
                };
                DeviceRecall();
                _activity.Timestamps.Start = (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            }
        }
        else {
            _activity.State = "Loading a new world";
            Continue();
            _activity.Party = new ActivityParty {
                Id = "",
                Size = new PartySize {
                    CurrentSize = 0,
                    MaxSize = 0
                }
            };
            _activity.Assets.SmallImage = "mint";
            _activity.Assets.SmallText = "Loading...";
            _activity.Timestamps.Start = (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            _activity.Secrets.Join = "";
        }
        // Update();
        UpdateActivity();
        return _activity.Secrets.Join;
    }
    
    private static void UserCountChanged(int userCount, int userMax, string partyId) {
        if (!_isRunning) return;
        _activity.Party = new ActivityParty {
            Id = partyId,
            Size = new PartySize {
                CurrentSize = userCount,
                MaxSize = userMax
            }
        };
        // Update();
        UpdateActivity();
    }

    public static void SettingsUpdated() {
        if (!_isRunning) return;
        if (Enabled.Value && !_isRunning) {
            Start();
        }
        _activity.Details = HideName.Value ? CustomText.Value : $"as {_displayName} ~ {CustomText.Value}";
        // Update();
        UpdateActivity();
    }
    
    private static void UserChanged(string displayName) {
        if (!_isRunning) return;
        if (!string.IsNullOrWhiteSpace(displayName)) {
            _activity.Details = HideName.Value ? CustomText.Value : $"as {displayName} ~ {CustomText.Value}";
            // Update();
            UpdateActivity();
            return;
        }
        _activity.Details = CustomText.Value;
        RoomChanged("", "", "", InstanceAccessType.InviteOnly, 0, 0);
    }
    
    public static void Update() {
        if (!_isRunning)  return;
        if (APIUser.CurrentUser == null) return;
        if (RoomManager.field_Internal_Static_ApiWorld_0 == null || RoomManager.field_Internal_Static_ApiWorldInstance_0 == null) return;
        
        var currentUser = APIUser.CurrentUser;
        var b = currentUser?.id ?? "";
        if (_uuid != b || HideName.Value) {
            _uuid = b;
            var currentUser2 = APIUser.CurrentUser;
            UserChanged(currentUser2?.displayName ?? "");
        }
        _displayName = currentUser?.displayName ?? "";
        var text = "";
        var currentRoom = RoomManager.field_Internal_Static_ApiWorld_0;
        var instance = RoomManager.field_Internal_Static_ApiWorldInstance_0?.instanceId;
        var inRoom = PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0.Count;
        if (currentRoom?.id != null)
            text = currentRoom.id + ":" + instance;
        
        if (_roomId != text) {
            _roomId = text;
            _roomSecret = "";
            
            if (_roomId != "") {
                var instanceAccessType = RoomManager.field_Internal_Static_ApiWorldInstance_0?.type;
                _roomSecret = RoomChanged(currentRoom?.name, currentRoom?.id + ":" + currentRoom?.id, instance,
                    instanceAccessType ?? InstanceAccessType.InviteOnly, currentRoom!.capacity, inRoom);
            }
            else
                RoomChanged("", "", "", InstanceAccessType.InviteOnly, 0, 0);
        }

        if (currentRoom == null) return;
        UserCountChanged(inRoom, currentRoom.capacity, currentRoom.id);
        UpdateActivity();
    }
    
    public static void OnApplicationQuit() {
        if (!_isRunning) return;
        try{_discord.Dispose();}catch (Exception e){Error(e);}
    }
    
    // private static string GenerateRandomString(int length) {
    //     const string text = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    //     var array = new char[length];
    //     var random = new Random();
    //     for (var i = 0; i < length; i++)  {
    //         array[i] = text[random.Next(text.Length)];
    //     }
    //     return new string(array);
    // }
}