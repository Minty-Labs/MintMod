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

namespace MintMod {
    class Config : MintSubMod {
        public override string Name => "Config";
        public override string Description => "Mint's Config system.";

        public static MelonPreferences_Category mint;
        public static MelonPreferences_Entry<bool> SpoofDeviceType, QMStatus, SpoofPing, SpoofedPingNegative, SpoofFramerate, bypassRiskyFunc;
        public static MelonPreferences_Entry<int> SpoofedPingNumber;
        public static MelonPreferences_Entry<float> SpoofedFrameNumber;

        // Base
        public static MelonPreferences_Category Base;
        public static MelonPreferences_Entry<bool> KeepPriorityHighEnabled, ShowWelcomeMessages, UseCustomLoadingMusic, EnableMasterFinder, AutoAddJump,
            EnableAllKeybindings, EnablePlayerJoinLeave, FriendsOnlyJoinLeave, HeadsUpDisplayNotifs, MintConsoleTitle, CanSitInChairs;
        public static MelonPreferences_Entry<int> MaxFrameRate;

        // Color
        public static MelonPreferences_Category Color;
        public static MelonPreferences_Entry<bool> RecolorRanks, ColorGameMenu, ColorActionMenu, ColorMenuText, ColorHUDMuteIcon, ColorUiExpansionKit, EnableCustomNameplateReColoring, ColorLoadingScreen;
        public static MelonPreferences_Entry<string> FriendRankHEX, LegendRankHEX, VeteranRankHEX, TrustedRankHEX, KnownRankHEX, UserRankHEX, NewUserRankKEX,
            VisitorRankHEX, MenuColorHEX, QMTextColorHEX;

        // Menu
        public static MelonPreferences_Category Menu;
        public static MelonPreferences_Entry<bool> KeepFlightBTNsOnMainMenu, ActionMenuON, KeepPhotonFreezesOnMainMenu;
        public static MelonPreferences_Entry<string> InfoHUDPosition;

        // Portal
        public static MelonPreferences_Category Portal;
        public static MelonPreferences_Entry<string> Portal_1, Portal_2, Portal_3, Portal_4, Portal_5, Portal_6, Portal_7, Portal_8;
        public static MelonPreferences_Entry<int> FakeOccupiedNumber;
        public static MelonPreferences_Entry<float> ResetTimerAmount;

        // Midnight Rooftop Button State
        public static MelonPreferences_Category MRBS;
        public static MelonPreferences_Entry<bool> DetObj, Fog, PP, thunder, dust, collider, join, pillow, pillowpickup, chairs, lightbeams, RainWin, RainPart,
            FurCarpet;

        // Avatar
        public static MelonPreferences_Category Avatar;
        public static MelonPreferences_Entry<bool> AviFavsEnabled, AviLogFavOrUnfavInConsole;

        // Extras
        public static MelonPreferences_Category Extras;
        public static MelonPreferences_Entry<bool> BTKLead, ChainloadWaypoints, ChainloadTeleporterVR, ChainloadAMMusic;

        internal override void OnStart() {
            // Info
            MelonPreferences_Category yes = MelonPreferences.CreateCategory("MintMod_Info", "MintMod - * (requires game restart)");
            _Base();
            _Color();
            _Menu();
            _Portal();
            _MRBS();
            _Avatar();
            _Random();
            _Extra();
        }

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
        }

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
            ColorUiExpansionKit = Color.CreateEntry("ColorUiExpansionKit", true, "* Color UIX", "* Color Ui Expansion Kit");
            ColorMenuText = Color.CreateEntry("ColorMenuText", false, "* Color QuickMenu Text", "* Toggle Quick Menu Text Color");
            QMTextColorHEX = Color.CreateEntry("QMTextColorHEX", "ffff00", "HEX: QM Text", "Quick Menu Text Color");
            EnableCustomNameplateReColoring = Color.CreateEntry("EnableCustomNameplateReColoring", true, "Custom Mint Nameplate ReColor", "Custom Nameplate ReColoring");
            ColorLoadingScreen = Color.CreateEntry("ColorLoadingScreen", false, "* Color Loading Environment", "Colors the Loading Environment");
        }

        static void _Menu() {
            // Menu
            Menu = MelonPreferences.CreateCategory("MintMod_Menu", "MintMod - Menu");

            KeepFlightBTNsOnMainMenu = Menu.CreateEntry("KeepFlightBTNsOnMainMenu", false, "Put Fly/NoClip on Main Quick Menu", "");
            ActionMenuON = Menu.CreateEntry("ActionMenuON", true, "Mint ActionMenu Controls", "Toggle Action Menu integration");

            //InfoHUDPosition = Menu.CreateEntry("InfoHUDPosition", "3", "Quick Menu Player List Location", "");
            //UIExpansionKit.API.ExpansionKitApi.RegisterSettingAsStringEnum(Menu.Identifier, InfoHUDPosition.Identifier,
            //    new[] { ("off", "Don't Show"), ("1", "Left"), ("2", "Top"), ("3", "Bottom"), ("4", "Right") });
            KeepPhotonFreezesOnMainMenu = Menu.CreateEntry("KeepPhotonFreezeOnMainMenu", false, "* Puts a photon freeze toggle on QM");
        }

        static void _Portal() {
            // Portal
            Portal = MelonPreferences.CreateCategory("MintMod_Portal", "MintMod - Portal");
            Portal_1 = Portal.CreateEntry("Portal_1", "empty", "Custom Portal 1", "");
            Portal_2 = Portal.CreateEntry("Portal_2", "empty", "Custom Portal 2", "");
            Portal_3 = Portal.CreateEntry("Portal_3", "empty", "Custom Portal 3", "");
            Portal_4 = Portal.CreateEntry("Portal_4", "empty", "Custom Portal 4", "");
            Portal_5 = Portal.CreateEntry("Portal_5", "empty", "Custom Portal 5", "");
            Portal_6 = Portal.CreateEntry("Portal_6", "empty", "Custom Portal 6", "");
            Portal_7 = Portal.CreateEntry("Portal_7", "empty", "Custom Portal 7", "");
            Portal_8 = Portal.CreateEntry("Portal_8", "empty", "Custom Portal 8", "");
            FakeOccupiedNumber = Portal.CreateEntry("FakeOccupiedNumber", -1, "Fake Portal User Count", "");
            ResetTimerAmount = Portal.CreateEntry("ResetTimerAmount", 3600f, "Reset Portal Timer", "");
        }

        static void _MRBS() {
            // Midnight Rooftop Button State
            MRBS = MelonPreferences.CreateCategory("MintMod_MidnightRooftop", "MintMod - Midnight Rooftop Button State");
            DetObj = MRBS.CreateEntry("DetObj", true, "Detail Objects", "");
            Fog = MRBS.CreateEntry("Fog", true, "Fog", "");
            PP = MRBS.CreateEntry("PP", true, "Post Processing", "");
            thunder = MRBS.CreateEntry("thunder", false, "Thunder Sounds", "");
            dust = MRBS.CreateEntry("dust", true, "Dust Particles", "");
            collider = MRBS.CreateEntry("collider", false, "Colliders", "");
            join = MRBS.CreateEntry("join", true, "Join Notification Sound", "");
            pillow = MRBS.CreateEntry("pillow", true, "Pillow Objects", "");
            pillowpickup = MRBS.CreateEntry("pillowpickup", true, "Pillow Pickup", "");
            chairs = MRBS.CreateEntry("chairs", true, "Chairs", "");
            lightbeams = MRBS.CreateEntry("lightbeams", false, "Light Beams", "");
            RainWin = MRBS.CreateEntry("RainWin", true, "Rainy Windows", "");
            RainPart = MRBS.CreateEntry("RainPart", true, "Rain Particles", "");
            FurCarpet = MRBS.CreateEntry("FurCarpet", true, "Fur Carpet", "");
        }

        static void _Avatar() {
            // Avatar
            Avatar = MelonPreferences.CreateCategory("MintMod_Avatar", "MintMod - Avatar");
            AviFavsEnabled = Avatar.CreateEntry("AviFavsEnabled", true, "Avatar Favorites Enabled", "");
            AviLogFavOrUnfavInConsole = Avatar.CreateEntry("AviLogFavOrUnfavInConsole", true, "Log Fav/UnFav in console", "");
        }

        static void _Random() {
            // Random
            mint = MelonPreferences.CreateCategory("MintMod_Random", "MintMod - Random");
            SpoofDeviceType = mint.CreateEntry("SpoofDeviceType", false, "Spoof Device to Quest", "");
            //QMStatus = mint.CreateEntry("QMStatus", false, "Dev Status on Quick Menu", "");
            SpoofPing = mint.CreateEntry("SpoofPing", false, "Spoof Ping", "");
            SpoofedPingNumber = mint.CreateEntry("SpoofedPingNumber", 0, "Spoofed Ping Number", "");
            SpoofedPingNegative = mint.CreateEntry("SpoofedPingNegative", false, "Fake Ping is Negative", "");
            SpoofFramerate = mint.CreateEntry("SpoofFramerate", true, "Spoof Framerate", "");
            SpoofedFrameNumber = mint.CreateEntry("SpoofedFrameNumber", 0f, "Spoofed Frame Number", "");
            bypassRiskyFunc = mint.CreateEntry("bypassRiskyFunc", false, "Bypasses Mods' Risky Func Checks");
        }

        static void _Extra() {
            // Extras
            Extras = MelonPreferences.CreateCategory("MintMod_Extras", "MintMod - Extras");
            BTKLead = Extras.CreateEntry("UseBTKLead", false, "* Use BTKLead", "");
        }

        internal override void OnPrefSave() {
            Utils.General.SetPriority();
            Utils.General.SetFrameRate();
            MasterFinder.MasterIcon.SetActive(EnableMasterFinder.Value);
            //if ()
            if (AviFavsEnabled.Value) {
                try {
                    AviFavLogic.Intance.OnUserInterface();
                } catch (Exception a) { MelonLogger.Error($"After game start, Avatar Favorites Start Error\n{a}"); }

                try {
                    MelonCoroutines.Start(AviFavLogic.RefreshMenu(1f));
                }
                catch (Exception r) {
                    MelonLogger.Error($"{r}");
                }
            } else {
                try {
                    AviFavLogic.DestroyList();
                }
                catch (Exception d) { MelonLogger.Error($"{d}"); }
            }
        }
    }
}
