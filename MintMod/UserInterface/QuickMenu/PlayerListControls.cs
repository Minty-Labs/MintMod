using MintMod.Libraries;
using MintMod.Resources;
using ReMod.Core.UI.QuickMenu;
using ReMod.Core.VRChat;
using UnityEngine;
using UnityEngine.UI;

namespace MintMod.UserInterface.QuickMenu; 

public static class PlayerListControls {
    private static ReCategoryPage _playerListConfig;
    internal static ReMenuToggle PlEnabled;
    private static ReMenuToggle _wingLocation, _extendList, _roomTimer, _gameTimer, _systemTime, _system24Hour, _showPing, _showFrames, _showPlatform, _showAviPerf;
    private static ReMenuCategory _temp;
    private static ReMenuButton _save, _setHexValue;
    private static ReMenuSliderCategory _colorCat;
    private static ReMenuSlider _red, _green, _blue, _alpha, _textSize;
    private static Color _color;
    
    internal static void PlayerListOptions(ReMenuCategory baseActions) {
        _playerListConfig = baseActions.AddCategoryPage("Player List Config", "Control the player list's options", MintyResources.userlist);
        var c = _playerListConfig.AddCategory("Player List Config", false);
        var o = Config.PLEnabled.Value;
        
        PlEnabled = c.AddToggle("Enabled", "Toggle the PLayer List", b => {
            Config.SavePrefValue(Config.PlayerList, Config.PLEnabled, b);
            var tempToggle = Config.PLEnabled.Value;
            _save.Active = tempToggle;
            _colorCat.Header.Active = tempToggle;
            _red.Active = tempToggle;
            _green.Active = tempToggle;
            _blue.Active = tempToggle;
            _alpha.Active = tempToggle;
            _wingLocation.Active = tempToggle;
            _extendList.Active = tempToggle;
            _textSize.Active = tempToggle;
            _roomTimer.Active = tempToggle;
            _gameTimer.Active = tempToggle;
            _systemTime.Active = tempToggle;
            _system24Hour.Active = tempToggle;
            _showFrames.Active = tempToggle;
            _showPing.Active = tempToggle;
            _showPlatform.Active = tempToggle;
            _showAviPerf.Active = tempToggle;
            _setHexValue.Active = tempToggle;
        }, Config.PLEnabled.Value);
        
        _wingLocation = c.AddToggle("List on Right Side", "Move the list on the left or right wing", b => 
            Config.SavePrefValue(Config.PlayerList, Config.Location, b ? 1 : 0), Config.Location.Value != 0);
        _extendList = c.AddToggle("Extend Player Listing", "Show all players regardless on length of box", b => {
            Config.SavePrefValue(Config.PlayerList, Config.uncapListCount, b);
            PlayerInfo.MoveTheText();
        }, Config.uncapListCount.Value);
        
        _save = c.AddButton("Save Values", "Save the color options below", () => {
            // var color = (Color32)PlayerInfo.BackgroundImage.color;
            Config.SavePrefValue(Config.PlayerList, Config.BackgroundColor, _color);
            Config.SavePrefValue(Config.PlayerList, Config.TextSize, PlayerInfo.GetTextSize());
        }, MintyResources.cog);
        
        _roomTimer = c.AddToggle("Room Timer", "Adds a Room Timer to the player list", b => 
            Config.SavePrefValue(Config.PlayerList, Config.haveRoomTimer, b), Config.haveRoomTimer.Value);
        _gameTimer = c.AddToggle("Game Timer", "Adds a Game Timer to the player list (Shows how long you've been currently playing VRChat)", b => 
            Config.SavePrefValue(Config.PlayerList, Config.haveGameTimer, b), Config.haveGameTimer.Value);
        _systemTime = c.AddToggle("System Time", "Adds your System Time to the player list", b => 
            Config.SavePrefValue(Config.PlayerList, Config.haveSystemTime, b), Config.haveSystemTime.Value);
        _system24Hour = c.AddToggle("24 Hour Format", "Changes the System time to the 24 hour format", b => 
            Config.SavePrefValue(Config.PlayerList, Config.system24Hour, b), Config.system24Hour.Value);
        _showPing = c.AddToggle("Player Ping", "Toggles ping shown on the player list", b =>
            Config.SavePrefValue(Config.PlayerList, Config.showPlayerPing, b), Config.showPlayerPing.Value);
        _showFrames = c.AddToggle("Player Frames", "Toggles frame rate shown on the player list", b =>
            Config.SavePrefValue(Config.PlayerList, Config.showPlayerFrames, b), Config.showPlayerFrames.Value);
        _showPlatform = c.AddToggle("Player Platform", "Toggles platform shown on the player list", b =>
            Config.SavePrefValue(Config.PlayerList, Config.showPlayerPlatform, b), Config.showPlayerPlatform.Value);
        _showAviPerf = c.AddToggle("Player Performance", "Toggles avatar performance shown on the player list", b =>
            Config.SavePrefValue(Config.PlayerList, Config.showPlayerAviPerf, b), Config.showPlayerAviPerf.Value);
        
        _colorCat = _playerListConfig.AddSliderCategory($"Background <color=#{ColorUtility.ToHtmlStringRGB(Config.BackgroundColor.Value)}>Color</color>");
        _red = _colorCat.AddSlider("Red Value", "Shift Red Color Values", f => {
            // var c = new Color(f/255, Config.BackgroundColor.Value.g,
            //     Config.BackgroundColor.Value.b, Config.BackgroundColor.Value.a);
            _color.r = f;
            PlayerInfo.SetBackgroundColor(_color);
            _colorCat.Title = $"Background <color=#{ColorUtility.ToHtmlStringRGB(_color)}>Color</color>";
        }, Config.BackgroundColor.Value.r*255, 0, 255);
        
        _green = _colorCat.AddSlider("Green Value", "Shift Green Color Values", f => {
            // var c = new Color(Config.BackgroundColor.Value.r, f/255,
            //     Config.BackgroundColor.Value.b, Config.BackgroundColor.Value.a);
            _color.g = f;
            PlayerInfo.SetBackgroundColor(_color);
            _colorCat.Title = $"Background <color=#{ColorUtility.ToHtmlStringRGB(_color)}>Color</color>";
        }, Config.BackgroundColor.Value.g*255, 0, 255);
        
        _blue = _colorCat.AddSlider("Blue Value", "Shift Blue Color Values", f => {
            // var c = new Color(Config.BackgroundColor.Value.r, Config.BackgroundColor.Value.g,
            //     f/255, Config.BackgroundColor.Value.a);
            _color.b = f;
            PlayerInfo.SetBackgroundColor(_color);
            _colorCat.Title = $"Background <color=#{ColorUtility.ToHtmlStringRGB(_color)}>Color</color>";
        }, Config.BackgroundColor.Value.b*255, 0, 255);
        
        _alpha = _colorCat.AddSlider("Alpha Value", "Shift Opacity Values", f => {
            // var c = new Color(Config.BackgroundColor.Value.r, Config.BackgroundColor.Value.g,
            //     Config.BackgroundColor.Value.b, f/255);
            _color.a = f;
            PlayerInfo.SetBackgroundColor(_color);
        }, Config.BackgroundColor.Value.a*255, 0, 255);
        
        _textSize = _colorCat.AddSlider("Text Size", "Change the text size of the player list", f => 
            PlayerInfo.UpdateTextSize((int)f), Config.TextSize.Value, 30, 50);
        
        _temp = _playerListConfig.AddCategory("Hidden");
        _temp.Header.GameObject.SetActive(false);
        _setHexValue = _temp.AddButton($"Enter <color=#{ColorUtility.ToHtmlStringRGB(Config.BackgroundColor.Value)}>HEX</color>",
            "Enter a HEX value you know for the Player List Background Color", () => {
                VRCUiPopupManager.field_Private_Static_VRCUiPopupManager_0.ShowInputPopupWithCancel("Enter HEX Value", "", 
                    InputField.InputType.Standard, false, "Continue", (s, Elly_Is, Mega_Cute) => {
                        _colorCat.Title = $"Background <color=#{s.Replace("#", "")}>Color</color>";
                        var color = ColorConversion.HexToColor(s.Replace("#", ""));
                        _color = ColorConversion.HexToColor(s.Replace("#", ""));
                        //Con.Debug($"Color: {s}   {c.r} {c.g} {c.b}", MintCore.isDebug);
                        Config.SavePrefValue(Config.PlayerList, Config.BackgroundColor, new Color(color.r, color.g, color.b, Config.BackgroundColor.Value.a));
                        PlayerInfo.SetBackgroundColor(Config.BackgroundColor.Value);
                        _red.Slide(color.r * 255, false);        // \/                        \/                            \/
                        _green.Slide(color.g * 255, false);      // False Because Unity is Throwing Errors with onValueChanged
                        _blue.Slide(color.b * 255, false);       // /\                        /\                            /\
                        _setHexValue.Text = $"Enter <color=#{ColorUtility.ToHtmlStringRGB(Config.BackgroundColor.Value)}>HEX</color>";
                    }, () => VRCUiPopupManager.field_Private_Static_VRCUiPopupManager_0.HideCurrentPopup());
            }, MintyResources.ColorPicker);
        
        _wingLocation.Active = o;
        _extendList.Active = o;
        _save.Active = o;
        _roomTimer.Active = o;
        _gameTimer.Active = o;
        _systemTime.Active = o;
        _system24Hour.Active = o;
        _showFrames.Active = o;
        _showPing.Active = o;
        _showPlatform.Active = o;
        _showAviPerf.Active = o;
            
        _colorCat.Header.Active = o;
        _red.Active = o;
        _green.Active = o;
        _blue.Active = o;
        _alpha.Active = o;
        _textSize.Active = o;
        _setHexValue.Active = o;
    }
}