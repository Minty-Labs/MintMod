using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MintMod.Functions;
using UnityEngine;
using MintMod.UserInterface.AvatarFavs;
using MintMod.UserInterface.QuickMenu;
using VRC;

namespace MintMod {
    class Config : MintSubMod {
        public override string Name => "Config";
        public override string Description => "Mint's Config system.";
        
        internal override void OnStart() {
            // Info
            MelonPreferences.CreateCategory("MintMod_Info", "MintMod - * (requires game restart)");
            _Base();
            _Color();
            _Menu();
            _Portal();
            //_MRBS();
            _Avatar();
            _Random();
            _Extra();
            _PlayerList();
        }

        // Base
        public static MelonPreferences_Category Base;
        public static MelonPreferences_Entry<bool> KeepPriorityHighEnabled, ShowWelcomeMessages, UseCustomLoadingMusic, EnableMasterFinder, AutoAddJump,
            EnableAllKeybindings, EnablePlayerJoinLeave, FriendsOnlyJoinLeave, HeadsUpDisplayNotifs, CanSitInChairs, UseOldHudMessages;
        public static MelonPreferences_Entry<int> MaxFrameRate;
        static void _Base() {
            // Base
            Base = MelonPreferences.CreateCategory("MintMod_Base", "MintMod - Base");
            KeepPriorityHighEnabled = Base.CreateEntry("KeepPriorityHighEnabled", true, "PriorityHigh", "Make VRChat run on a higher proccess queue");
            ShowWelcomeMessages = Base.CreateEntry("ShowWelcomeMessages", true, "Welcome HUD Message", "Show Welcome message on game start");
            MaxFrameRate = Base.CreateEntry("MaxFrameRate", 240, "Max Framerate", "Changes the max framerate Unity will render VRChat");
            UseCustomLoadingMusic = Base.CreateEntry("UseCustomLoadingMusic", false, "Use Custom Loading Music", "Allows you to use custom loading music");
            EnableMasterFinder = Base.CreateEntry("EnableMasterFinder", false, "Master Finder", "Shows an icon on master's nameplate");
            AutoAddJump = Base.CreateEntry("AutoAddJump", false, "Auto Add Jump", "Auto Add Jumping on non-jumpable worlds");
            EnableAllKeybindings = Base.CreateEntry("EnableAllKeybindings", true, "Keybindings", "Toggle Keyboard shortcut bindings");
            EnablePlayerJoinLeave = Base.CreateEntry("EnablePlayerJoinLeave", false, "Join/Leave Notifs", "Log Player Join/Leave in console");
            FriendsOnlyJoinLeave = Base.CreateEntry("FriendsOnlyJoinLeave", false, "J/L Friends Only", "Friend only Join/Leave logs");
            HeadsUpDisplayNotifs = Base.CreateEntry("HeadsUpDisplayNotifs", false, "J/L HUD Messages", "Player/Friend Join/Leave HUD messages");
            //MintConsoleTitle = Base.CreateEntry("MintConsoleTitle", true, "* Mint Console Title", "*Use Custom Console Title");
            CanSitInChairs = Base.CreateEntry("CanSitInChairs", true, "Can Sit In Chairs");
            UseOldHudMessages = Base.CreateEntry("UseOldHudMessages", false, "Use Old Hud Messages");
        }

// Color
        public static MelonPreferences_Category Color;
        public static MelonPreferences_Entry<bool> RecolorRanks, ColorGameMenu, ColorActionMenu, ColorHUDMuteIcon, EnableCustomNameplateReColoring, ColorLoadingScreen;
        public static MelonPreferences_Entry<string> FriendRankHEX, LegendRankHEX, VeteranRankHEX, TrustedRankHEX, KnownRankHEX, UserRankHEX, NewUserRankKEX,
            VisitorRankHEX, MenuColorHEX;
        static void _Color() {
            // Color
            Color = MelonPreferences.CreateCategory("MintMod_Color", "MintMod - Color");
            RecolorRanks = Color.CreateEntry("RecolorRanks", false, "ReColor Ranks", "ReColor VRChat Ranks");
            FriendRankHEX = Color.CreateEntry("FriendRankHEX", "FFFF00", "HEX: Friends", "Friend Color");
            LegendRankHEX = Color.CreateEntry("LegendRankHEX", "FF6982", "HEX: Legend", "Legend Color");
            VeteranRankHEX = Color.CreateEntry("VeteranRankHEX", "FFC261", "HEX: Veteran", "Veteran Color");
            TrustedRankHEX = Color.CreateEntry("TrustedRankHEX", "8143E6", "HEX: Trusted", "Trusted Color");
            KnownRankHEX = Color.CreateEntry("KnownRankHEX", "FF7B42", "HEX: Known", "Known Color");
            UserRankHEX = Color.CreateEntry("UserRankHEX", "2BCF5C", "HEX: User", "User Color");
            NewUserRankKEX = Color.CreateEntry("NewUserRankKEX", "1778FF", "HEX: NewUser", "New User Color");
            VisitorRankHEX = Color.CreateEntry("VisitorRankHEX", "CCCCCC", "HEX: Visitor", "Visitor Color");

            ColorGameMenu = Color.CreateEntry("ColorGameMenu", false, "* Color Game Menu", "* Color the User Interface");
            MenuColorHEX = Color.CreateEntry("MenuColorHEX", "00ffaa", "* HEX: Menu Color", "Menu Color");
            ColorActionMenu = Color.CreateEntry("ColorActionMenu", false, "* Color Action Menu", "* Color Action Menu");
            ColorHUDMuteIcon = Color.CreateEntry("ColorHUDMuteIcon", false, "* Color HUD Mute Icon", "* Color Mute HUD Icon");
            EnableCustomNameplateReColoring = Color.CreateEntry("EnableCustomNameplateReColoring", true, "Custom Mint Nameplate ReColor", "Custom Nameplate ReColoring");
            ColorLoadingScreen = Color.CreateEntry("ColorLoadingScreen", false, "* Color Loading Environment", "Colors the Loading Environment");
        }

        // Menu
        public static MelonPreferences_Category Menu;
        public static MelonPreferences_Entry<bool> KeepFlightBTNsOnMainMenu, ActionMenuON, KeepPhotonFreezesOnMainMenu, KeepInfJumpOnMainMenu, useTabButtonForMenu;
        public static MelonPreferences_Entry<string> InfoHUDPosition;
        static void _Menu() {
            // Menu
            Menu = MelonPreferences.CreateCategory("MintMod_Menu", "MintMod - Menu");
            useTabButtonForMenu = Menu.CreateEntry("useTabButtonForMenu", false, "* Use Tab Button Menu");
            KeepFlightBTNsOnMainMenu = Menu.CreateEntry("KeepFlightBTNsOnMainMenu", false, "Put Fly/NoClip on Main Quick Menu");
            ActionMenuON = Menu.CreateEntry("ActionMenuON", true, "Mint ActionMenu Controls", "Toggle Action Menu integration");

            //InfoHUDPosition = Menu.CreateEntry("InfoHUDPosition", "3", "Quick Menu Player List Location", "");
            //UIExpansionKit.API.ExpansionKitApi.RegisterSettingAsStringEnum(Menu.Identifier, InfoHUDPosition.Identifier,
            //    new[] { ("off", "Don't Show"), ("1", "Left"), ("2", "Top"), ("3", "Bottom"), ("4", "Right") });
            KeepPhotonFreezesOnMainMenu = Menu.CreateEntry("KeepPhotonFreezeOnMainMenu", false, "Puts a photon freeze toggle on QM");
            KeepInfJumpOnMainMenu = Menu.CreateEntry("KeepInfJumpOnMainMenu", false, "Puts Infinite Jump Toggle on QM");
        }

        // Portal
        public static MelonPreferences_Category Portal;
        //public static MelonPreferences_Entry<string> Portal_1, Portal_2, Portal_3, Portal_4, Portal_5, Portal_6, Portal_7, Portal_8;
        //public static MelonPreferences_Entry<int> FakeOccupiedNumber;
        public static MelonPreferences_Entry<float> ResetTimerAmount;
        static void _Portal() {
            // Portal
            Portal = MelonPreferences.CreateCategory("MintMod_Portal", "MintMod - Portal");
            //Portal_1 = Portal.CreateEntry("Portal_1", "empty", "Custom Portal 1", "");
            //Portal_2 = Portal.CreateEntry("Portal_2", "empty", "Custom Portal 2", "");
            //Portal_3 = Portal.CreateEntry("Portal_3", "empty", "Custom Portal 3", "");
            //Portal_4 = Portal.CreateEntry("Portal_4", "empty", "Custom Portal 4", "");
            //Portal_5 = Portal.CreateEntry("Portal_5", "empty", "Custom Portal 5", "");
            //Portal_6 = Portal.CreateEntry("Portal_6", "empty", "Custom Portal 6", "");
            //Portal_7 = Portal.CreateEntry("Portal_7", "empty", "Custom Portal 7", "");
            //Portal_8 = Portal.CreateEntry("Portal_8", "empty", "Custom Portal 8", "");
            //FakeOccupiedNumber = Portal.CreateEntry("FakeOccupiedNumber", -1, "Fake Portal User Count", "");
            ResetTimerAmount = Portal.CreateEntry("ResetTimerAmount", 3600f, "Reset Portal Timer");
        }

        /*
        // Midnight Rooftop Button State
        public static MelonPreferences_Category MRBS;
        public static MelonPreferences_Entry<bool> DetObj, Fog, PP, thunder, dust, collider, join, pillow, pillowpickup, chairs, lightbeams, RainWin, RainPart,
            FurCarpet;
        static void _MRBS() {
            // Midnight Rooftop Button State
            MRBS = MelonPreferences.CreateCategory("MintMod_MidnightRooftop", "MintMod - Midnight Rooftop Button State");
            DetObj = MRBS.CreateEntry("DetObj", true, "Detail Objects");
            Fog = MRBS.CreateEntry("Fog", true, "Fog");
            PP = MRBS.CreateEntry("PP", true, "Post Processing");
            thunder = MRBS.CreateEntry("thunder", false, "Thunder Sounds");
            dust = MRBS.CreateEntry("dust", true, "Dust Particles");
            collider = MRBS.CreateEntry("collider", false, "Colliders");
            join = MRBS.CreateEntry("join", true, "Join Notification Sound");
            pillow = MRBS.CreateEntry("pillow", true, "Pillow Objects");
            pillowpickup = MRBS.CreateEntry("pillowpickup", true, "Pillow Pickup");
            chairs = MRBS.CreateEntry("chairs", true, "Chairs");
            lightbeams = MRBS.CreateEntry("lightbeams", false, "Light Beams");
            RainWin = MRBS.CreateEntry("RainWin", true, "Rainy Windows");
            RainPart = MRBS.CreateEntry("RainPart", true, "Rain Particles");
            FurCarpet = MRBS.CreateEntry("FurCarpet", true, "Fur Carpet");
        }
        */

// Avatar
        public static MelonPreferences_Category Avatar;
        public static MelonPreferences_Entry<bool> AviFavsEnabled, AviLogFavOrUnfavInConsole, useWebhostSavedList, haveCustomPath;
        public static MelonPreferences_Entry<string> customPath;
        static void _Avatar() {
            // Avatar
            Avatar = MelonPreferences.CreateCategory("MintMod_Avatar", "MintMod - Avatar");
            AviFavsEnabled = Avatar.CreateEntry("AviFavsEnabled", true, "Avatar Favorites Enabled");
            AviLogFavOrUnfavInConsole = Avatar.CreateEntry("AviLogFavOrUnfavInConsole", true, "Log Fav/UnFav in console");
            haveCustomPath = Avatar.CreateEntry("haveCustomAvatarListPath", false, "* Custom List Path");
            customPath = Avatar.CreateEntry("AvatarListCustomPath", $"{MintCore.MintDirectory}\\AviFavs.json", "* Custom Path");
        }

        public static MelonPreferences_Category mint;
        public static MelonPreferences_Entry<bool> /*SpoofDeviceType,*/ QMStatus, SpoofPing, SpoofedPingNegative, SpoofFramerate, bypassRiskyFunc, ModJoinPopup;
        public static MelonPreferences_Entry<int> SpoofedPingNumber;
        public static MelonPreferences_Entry<float> SpoofedFrameNumber;
        static void _Random() {
            // Random
            mint = MelonPreferences.CreateCategory("MintMod_Random", "MintMod - Random");
            //SpoofDeviceType = mint.CreateEntry("SpoofDeviceType", false, "Spoof Device to Quest", "");
            //QMStatus = mint.CreateEntry("QMStatus", false, "Dev Status on Quick Menu", "");
            SpoofPing = mint.CreateEntry("SpoofPing", false, "Spoof Ping");
            SpoofedPingNumber = mint.CreateEntry("SpoofedPingNumber", 0, "Spoofed Ping Number");
            SpoofedPingNegative = mint.CreateEntry("SpoofedPingNegative", false, "Fake Ping is Negative");
            SpoofFramerate = mint.CreateEntry("SpoofFramerate", true, "Spoof Framerate");
            SpoofedFrameNumber = mint.CreateEntry("SpoofedFrameNumber", 0f, "Spoofed Frame Number");
            bypassRiskyFunc = mint.CreateEntry("bypassRiskyFunc", false, "Bypasses Mods' Risky Func Checks");
            ModJoinPopup = mint.CreateEntry("ModJoinPopup", true, "Toggle the Mod Join Popup");
        }

        // Extras
        public static MelonPreferences_Category Extras;
        public static MelonPreferences_Entry<bool> useFakeName;
        public static MelonPreferences_Entry<string> FakeName;
        //public static MelonPreferences_Entry<bool> BTKLead, ChainloadWaypoints, ChainloadTeleporterVR, ChainloadAMMusic;
        static void _Extra() {
            // Extras
            Extras = MelonPreferences.CreateCategory("MintMod_Extras", "MintMod - Extras");
            useFakeName = Extras.CreateEntry("useFakeName", false, "Use a fake name", "");
            FakeName = Extras.CreateEntry("FakeName_string", "Cutie!~", "Set your fake name", "");
            //BTKLead = Extras.CreateEntry("UseBTKLead", false, "* Use BTKLead", "");
        }

        public static MelonPreferences_Category PlayerList;
        public static MelonPreferences_Entry<bool> PLEnabled, uncapListCount, haveRoomTimer, haveGameTimer, haveSystemTime, system24Hour, hideLabels;
        public static MelonPreferences_Entry<string> LocalSpoofedName, HudPosition;
        public static MelonPreferences_Entry<Color> BackgroundColor;
        public static MelonPreferences_Entry<int> Location, TextSize;
        static void _PlayerList() {
            PlayerList = MelonPreferences.CreateCategory("MintMod_PlayerList", "MintMod - PlayerList");
            PLEnabled = PlayerList.CreateEntry("EnablePlayerList", false, "Enable Player List");
            LocalSpoofedName = PlayerList.CreateEntry("LocalSpoofedName", "", "Local Player List Name Spoof", "If empty, nothing will change");
            BackgroundColor = PlayerList.CreateEntry("PLBackgroundColor", new Color(0, 0, 0, 128), "Player List Background Color",
                "This is the [ RR, GG, BB, Alpha ] (from RGBA values 0 - 255) values for the background image on the player list.");
            Location = PlayerList.CreateEntry("PlayerListLocation", 0, "Location Index", "0 = LeftWing; 1 = RightWing");
            uncapListCount = PlayerList.CreateEntry("uncapListCount", false, "Show ALL Players in list");
            TextSize = PlayerList.CreateEntry("PlayerListTextSize", 40, "Player List Text Size");
            haveRoomTimer = PlayerList.CreateEntry("haveRoomTimer", false, "Have a Room Timer on Player List");
            haveGameTimer = PlayerList.CreateEntry("haveGameTimer", false, "Have an overall in-game timer on Player List");
            haveSystemTime = PlayerList.CreateEntry("haveSystemTime", false, "Have system time on Player List");
            system24Hour = PlayerList.CreateEntry("system24HourFormat", false, "System Time has 24 Hour format");
            hideLabels = PlayerList.CreateEntry("hideLabels", false, "Hide the Labels on the timers");
        }

        internal override void OnPrefSave() {
            Utils.General.SetPriority();
            Utils.General.SetFrameRate();
        }

        public static void SavePrefValue(MelonPreferences_Category cat, MelonPreferences_Entry<bool> @bool, bool value) {
            MelonPreferences.GetEntry<bool>(cat.Identifier, @bool.Identifier).Value = value;
            MelonPreferences.Save();
        }

        public static void SavePrefValue(MelonPreferences_Category cat, MelonPreferences_Entry<string> @string, string value) {
            MelonPreferences.GetEntry<string>(cat.Identifier, @string.Identifier).Value = value;
            MelonPreferences.Save();
        }

        public static void SavePrefValue(MelonPreferences_Category cat, MelonPreferences_Entry<float> @float, float value) {
            MelonPreferences.GetEntry<float>(cat.Identifier, @float.Identifier).Value = value;
            MelonPreferences.Save();
        }

        public static void SavePrefValue(MelonPreferences_Category cat, MelonPreferences_Entry<int> @int, int value) {
            MelonPreferences.GetEntry<int>(cat.Identifier, @int.Identifier).Value = value;
            MelonPreferences.Save();
        }
        
        public static void SavePrefValue(MelonPreferences_Category cat, MelonPreferences_Entry<Color> color, Color value) {
            MelonPreferences.GetEntry<Color>(cat.Identifier, color.Identifier).Value = value;
            MelonPreferences.Save();
        }
    }
}
