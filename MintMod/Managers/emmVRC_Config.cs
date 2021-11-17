using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MintMod.Managers {
    public class emmVRCConfig {
        public string @type { get; set; }
        public string LastVersion { get; set; }
        public bool RiskyFunctionsEnabled { get; set; }
        public bool GlobalDynamicBonesEnabled { get; set; }
        public bool FriendGlobalDynamicBonesEnabled { get; set; }
        public bool EveryoneGlobalDynamicBonesEnabled { get; set; }
        public bool emmVRCNetworkEnabled { get; set; }
        public bool GlobalChatEnabled { get; set; }
        public bool VRFlightControls { get; set; }
        public bool UIExpansionKitIntegration { get; set; }
        public bool UnlimitedFPSEnabled { get; set; }
        public bool InfoBarDisplayEnabled { get; set; }
        public bool ClockEnabled { get; set; }
        public bool AvatarFavoritesEnabled { get; set; }
        public bool MasterIconEnabled { get; set; }
        public bool LogoButtonEnabled { get; set; }
        public bool HUDEnabled { get; set; }
        public bool VRHUDInDesktop { get; set; }
        public bool MoveVRHUDIfSpaceFree { get; set; }
        public bool ForceRestartButtonEnabled { get; set; }
        public bool PortalBlockingEnable { get; set; }
        public bool ChairBlockingEnable { get; set; }
        public bool PlayerHistoryEnable { get; set; }
        public bool TrackingSaving { get; set; }
        public int CustomFOV { get; set; }
        public int FPSLimit { get; set; }
        public int FunctionsButtonX { get; set; }
        public int FunctionsButtonY { get; set; }
        public int LogoButtonX { get; set; }
        public int LogoButtonY { get; set; }
        public int UserInteractButtonX { get; set; }
        public int UserInteractButtonY { get; set; }
        public int NotificationButtonPositionX { get; set; }
        public int NotificationButtonPositionY { get; set; }
        public int PlayerActionsButtonX { get; set; }
        public int PlayerActionsButtonY { get; set; }
        public bool StealthMode { get; set; }
        public bool DisableReportWorldButton { get; set; }
        public bool DisableEmojiButton { get; set; }
        public bool DisableEmoteButton { get; set; }
        public bool DisableRankToggleButton { get; set; }
        public bool DisablePlaylistsButton { get; set; }
        public bool DisableAvatarStatsButton { get; set; }
        public bool DisableReportUserButton { get; set; }
        public bool MinimalWarnKickButton { get; set; }
        public bool DisableAvatarHotWorlds { get; set; }
        public bool DisableAvatarRandomWorlds { get; set; }
        public bool DisableAvatarLegacy { get; set; }
        public bool DisableAvatarPublic { get; set; }
        public bool UIColorChangingEnabled { get; set; }
        public string UIColorHex { get; set; }
        public bool UIActionMenuColorChangingEnabled { get; set; }
        public bool UIExpansionKitColorChangingEnabled { get; set; }
        public bool UIMicIconColorChangingEnabled { get; set; }
        public bool UIMicIconPulsingEnabled { get; set; }
        public bool NameplateColorChangingEnabled { get; set; }
        public string FriendNamePlateColorHex { get; set; }
        public string VisitorNamePlateColorHex { get; set; }
        public string NewUserNamePlateColorHex { get; set; }
        public string UserNamePlateColorHex { get; set; }
        public string KnownUserNamePlateColorHex { get; set; }
        public string TrustedUserNamePlateColorHex { get; set; }
        public string VeteranUserNamePlateColorHex { get; set; }
        public string LegendaryUserNamePlateColorHex { get; set; }
        public bool InfoSpoofingEnabled { get; set; }
        public string InfoSpoofingName { get; set; }
        public bool InfoHidingEnabled { get; set; }
        public string AcceptedEULAVersion { get; set; }
        public bool WelcomeMessageShown { get; set; }
        public bool RiskyFunctionsWarningShown { get; set; }
        public bool NameplatesVisible { get; set; }
        public bool UIVisible { get; set; }
        public float UIVolume { get; set; }
        public bool UIVolumeMute { get; set; }
        public float WorldVolume { get; set; }
        public bool WorldVolumeMute { get; set; }
        public float VoiceVolume { get; set; }
        public bool VoiceVolumeMute { get; set; }
        public float AvatarVolume { get; set; }
        public bool AvatarVolumeMute { get; set; }
        public float MaxSpeedIncrease { get; set; }
        public bool EnableKeybinds { get; set; }
        public IList<int> FlightKeybind { get; set; }
        public IList<int> NoclipKeybind { get; set; }
        public IList<int> SpeedKeybind { get; set; }
        public IList<int> ThirdPersonKeybind { get; set; }
        public IList<int> ToggleHUDEnabledKeybind { get; set; }
        public IList<int> RespawnKeybind { get; set; }
        public IList<int> GoHomeKeybind { get; set; }
    }

    public static class GETemmVRCconfig {
        public static string config = Path.Combine(Environment.CurrentDirectory, "UserData/emmVRC/config.json");

        private static emmVRCConfig _Config { get; set; }

        public static void LoadConfig() { _Config = JsonConvert.DeserializeObject<emmVRCConfig>(File.ReadAllText(config)); }

        public static emmVRCConfig ReadConfig() { return _Config; }
    }
}
