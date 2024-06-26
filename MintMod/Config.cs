﻿using MelonLoader;
using UnityEngine;

namespace MintMod {
    internal class Config : MintSubMod {
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
            _Nameplates();
            _WorldSettings_BlackCat();
            
            GetReModPrefs();
            GetNoPerfStatsPrefs();
        }

        // Base
        public static MelonPreferences_Category Base;
        public static MelonPreferences_Entry<bool> KeepPriorityHighEnabled, ShowWelcomeMessages, UseCustomLoadingMusic, EnableMasterFinder, AutoAddJump,
            EnableAllKeybindings, EnablePlayerJoinLeave, FriendsOnlyJoinLeave, HeadsUpDisplayNotifs, CanSitInChairs, UseOldHudMessages, KeepInfJumpAlwaysOn;
        public static MelonPreferences_Entry<int> MaxFrameRate;

        private static void _Base() {
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
            KeepInfJumpAlwaysOn = Base.CreateEntry("KeepInfJumpAlwaysOn", false, "Keep Infinite Jump Always On");
        }

        // Color
        public static MelonPreferences_Category Color;
        public static MelonPreferences_Entry<bool> RecolorRanks, ColorGameMenu, ColorActionMenu, ColorHUDMuteIcon, ColorLoadingScreen;
        public static MelonPreferences_Entry<string> FriendRankHEX, LegendRankHEX, VeteranRankHEX, TrustedRankHEX, KnownRankHEX, UserRankHEX, NewUserRankKEX,
            VisitorRankHEX, MenuColorHEX;
        private static void _Color() {
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
            ColorLoadingScreen = Color.CreateEntry("ColorLoadingScreen", false, "* Color Loading Environment", "Colors the Loading Environment");
        }
        
        public static MelonPreferences_Category Nameplates;
        public static MelonPreferences_Entry<bool> EnableCustomNameplateReColoring, EnabledMintTags;
        public static MelonPreferences_Entry<float> MintTagVerticleLocation;

        private static void _Nameplates() {
            Nameplates = MelonPreferences.CreateCategory("MintMod_Nameplates", "MintMod - Nameplates");
            EnableCustomNameplateReColoring = Nameplates.CreateEntry("EnableCustomNameplateReColoring", true, "Custom Mint Nameplate ReColor",
                "Custom Nameplate ReColoring");
            EnabledMintTags = Nameplates.CreateEntry("EnabledMintTags", true, "Show Mint Tags", "Toggles Mint Tags on namplates");
            MintTagVerticleLocation = Nameplates.CreateEntry("MintTagVerticleLocation", -60f, "Vertical Location of Mint Tags",
                "Allows yoyu to move the Mint Tags at any point up or down (positive numbers are above the center of nameplate, negative numbers are below)");
        }

        // Menu
        public static MelonPreferences_Category Menu;
        public static MelonPreferences_Entry<bool> KeepFlightBTNsOnMainMenu, EnableActionMenu, KeepPhotonFreezesOnMainMenu, KeepInfJumpOnMainMenu, CopyReModMedia, AvatarMenuDlVrca;
        public static MelonPreferences_Entry<string> InfoHUDPosition;
        public static MelonPreferences_Entry<float> RefreshAmount;
        private static void _Menu() {
            // Menu
            Menu = MelonPreferences.CreateCategory("MintMod_Menu", "MintMod - Menu");
            KeepFlightBTNsOnMainMenu = Menu.CreateEntry("KeepFlightBTNsOnMainMenu", false, "Put Fly/NoClip on Main Quick Menu");
            EnableActionMenu = Menu.CreateEntry("ActionMenuON", true, "Mint ActionMenu Controls", "Toggle Action Menu integration");

            //InfoHUDPosition = Menu.CreateEntry("InfoHUDPosition", "3", "Quick Menu Player List Location", "");
            //UIExpansionKit.API.ExpansionKitApi.RegisterSettingAsStringEnum(Menu.Identifier, InfoHUDPosition.Identifier,
            //    new[] { ("off", "Don't Show"), ("1", "Left"), ("2", "Top"), ("3", "Bottom"), ("4", "Right") });
            KeepPhotonFreezesOnMainMenu = Menu.CreateEntry("KeepPhotonFreezeOnMainMenu", false, "Puts a photon freeze toggle on QM");
            KeepInfJumpOnMainMenu = Menu.CreateEntry("KeepInfJumpOnMainMenu", false, "Puts Infinite Jump Toggle on QM");
            CopyReModMedia = Menu.CreateEntry("CopyReModMedia", false, "Extra Debug Panel for Media Title");
            RefreshAmount = Menu.CreateEntry("ReModMediaRefreshAmount", 1f, "Update increment in seconds");
            AvatarMenuDlVrca = Menu.CreateEntry("AvatarMenuDLVRCA", true, "* Add DLVRCA to Avatar Menu");
        }

        // Portal
        public static MelonPreferences_Category Portal;
        //public static MelonPreferences_Entry<string> Portal_1, Portal_2, Portal_3, Portal_4, Portal_5, Portal_6, Portal_7, Portal_8;
        //public static MelonPreferences_Entry<int> FakeOccupiedNumber;
        public static MelonPreferences_Entry<float> ResetTimerAmount;
        private static void _Portal() {
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
        private  static void _Avatar() {
            // Avatar
            Avatar = MelonPreferences.CreateCategory("MintMod_Avatar", "MintMod - Avatar");
            AviFavsEnabled = Avatar.CreateEntry("AviFavsEnabled", true, "Avatar Favorites Enabled");
            AviLogFavOrUnfavInConsole = Avatar.CreateEntry("AviLogFavOrUnfavInConsole", true, "Log Fav/UnFav in console");
            haveCustomPath = Avatar.CreateEntry("haveCustomAvatarListPath", false, "* Custom List Path");
            customPath = Avatar.CreateEntry("AvatarListCustomPath", $"{MintCore.MintDirectory}\\AviFavs.json", "* Custom Path");
        }

        public static MelonPreferences_Category mint;
        public static MelonPreferences_Entry<bool> /*SpoofDeviceType,*/ QMStatus, SpoofPing, SpoofedPingNegative, SpoofFramerate, bypassRiskyFunc, ModJoinPopup, ForceClone;
        public static MelonPreferences_Entry<int> SpoofedPingNumber;
        public static MelonPreferences_Entry<float> SpoofedFrameNumber;
        private static void _Random() {
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
        private static void _Extra() {
            // Extras
            Extras = MelonPreferences.CreateCategory("MintMod_Extras", "MintMod - Extras");
            useFakeName = Extras.CreateEntry("useFakeName", false, "Use a fake name", "");
            FakeName = Extras.CreateEntry("FakeName_string", "Cutie!~", "Set your fake name", "");
            //BTKLead = Extras.CreateEntry("UseBTKLead", false, "* Use BTKLead", "");
        }

        public static MelonPreferences_Category PlayerList;
        public static MelonPreferences_Entry<bool> PLEnabled, uncapListCount, haveRoomTimer, haveGameTimer, haveSystemTime, system24Hour, hideLabels,
            showPlayerPing, showPlayerFrames, showPlayerPlatform, showPlayerAviPerf;
        public static MelonPreferences_Entry<string> LocalSpoofedName, HudPosition;
        public static MelonPreferences_Entry<Color> BackgroundColor;
        public static MelonPreferences_Entry<int> Location, TextSize;
        private static void _PlayerList() {
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
            showPlayerPing = PlayerList.CreateEntry("showPlayerPing", true, "Show player's ping");
            showPlayerFrames = PlayerList.CreateEntry("showPlayerFrames", true, "Show player's framerate");
            showPlayerPlatform = PlayerList.CreateEntry("showPlayerPlatform", true, "Show player's platform");
            showPlayerAviPerf = PlayerList.CreateEntry("showPlayerAviPerf", true, "Show player's avatar performance rating");
        }

        public static MelonPreferences_Category WorldSettings_BlackCat;
        public static MelonPreferences_Entry<bool> BC_Booth1, BC_Booth2, BC_Top, BC_Bathroom1, BC_Bathroom2;

        private static void _WorldSettings_BlackCat() {
            WorldSettings_BlackCat = MelonPreferences.CreateCategory("MintMod_WorldSettings_BlackCat", "MintMod - World - Black Cat");
            BC_Booth1 = WorldSettings_BlackCat.CreateEntry("Booth1", true, "Booth 1 Exit Collider");
            BC_Booth2 = WorldSettings_BlackCat.CreateEntry("Booth2", true, "Booth 2 Exit Collider");
            BC_Top = WorldSettings_BlackCat.CreateEntry("Top", true, "Up stairs Exit Collider");
            BC_Bathroom1 = WorldSettings_BlackCat.CreateEntry("Bathroom1", true, "Bathroom 1 Exit Collider");
            BC_Bathroom2 = WorldSettings_BlackCat.CreateEntry("Bathroom2", true, "Bathroom 1 Exit Collider");
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

        #region OtherMods

        #region ReMod (Private)

        public static MelonPreferences_Entry<bool> _mediaControlsEnabled, _mediaControlsEnabledCE;

        private static void GetReModPrefs() {
            if (MelonPreferences.GetCategory("ReMod") != null)
                _mediaControlsEnabled = MelonPreferences.GetEntry<bool>("ReMod", "MediaControlsEnabled");
            if (MelonPreferences.GetCategory("ReModCE") != null)
                _mediaControlsEnabledCE = MelonPreferences.GetEntry<bool>("ReModCE", "MediaControlsEnabled");
        }

        #endregion

        #region NoPerfStats

        public static MelonPreferences_Entry<bool> DisablePerformanceStats;
        public static MelonPreferences_Category NoPerformanceStats;
            
        private static void GetNoPerfStatsPrefs() {
            var n = MelonPreferences.GetCategory("NoPerformanceStats");
            if (n == null) return;
            NoPerformanceStats = n;
            DisablePerformanceStats = MelonPreferences.GetEntry<bool>("NoPerformanceStats", "DisablePerformanceStats");
        }

        #endregion

        #endregion
    }
}
