using MintMod.Managers;
using MintMod.Resources;
using ReMod.Core.UI.QuickMenu;
using ReMod.Core.VRChat;
using UnityEngine.UI;

namespace MintMod.UserInterface.QuickMenu; 

public static class NameplateMenu {
    private static ReCategoryPage _nameplateMenu;
    private static ReMenuToggle _mintNameplates, _mintTags;
    
    internal static void BuildNameplateMenu(ReMenuCategory baseActions) {
        _nameplateMenu = baseActions.AddCategoryPage("Nameplate","Opens Mint Nameplate Settings Menu", MintyResources.user_nameplate);

        var n = _nameplateMenu.AddCategory("Nameplate Settings");
        _mintNameplates = n.AddToggle("Nameplates Changes", "Toggles all Nameplate modifications done by Mint", b => {
            Config.SavePrefValue(Config.Nameplates, Config.EnableCustomNameplateReColoring, b);
            VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
        }, Config.EnableCustomNameplateReColoring.Value);
        
        _mintTags = n.AddToggle("Mint Tags", "Toggles all Nameplate modifications done by Mint", b => {
            Config.SavePrefValue(Config.Nameplates, Config.EnabledMintTags, b);
            VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
        }, Config.EnabledMintTags.Value);
        
        n.AddButton("Tag Location", "Input a number of vertical tag placement", () => {
                
            QuickMenuEx.Instance.ShowConfirmDialog("Moving the Mint Tag on a Nameplate",
                "Decreasing numbers will move the tag down, while increasing numbers will move them higher", "Continue", "Cancel", () => {
                    
                VRCUiPopupManager.field_Private_Static_VRCUiPopupManager_0.ShowInputPopupWithCancel("Vertical Tag Location",
                    $"{Config.MintTagVerticleLocation.Value}", InputField.InputType.Standard, false, "Submit",
                    (_, EllyIs, MegaAdorable) => {
                        
                        float.TryParse(_, out var final);
                        Config.SavePrefValue(Config.Nameplates, Config.MintTagVerticleLocation, final);
                        VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
                        
                    }, VRCUiPopupManager.field_Private_Static_VRCUiPopupManager_0.HideCurrentPopup, "-60.0");
                
            }, () => { });
                
        }, MintyResources.user_nameplate_tag_move);
        
        n.AddButton("Refetch Nameplates",
            "Reloads Mint's custom nameplate addons in case more were added while you're playing", () => {
                Players.FetchCustomPlayerObjects(true);
                VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
            }, MintyResources.extlink);
    }

    internal static void OnPrefSaved() {
        _mintNameplates?.Toggle(Config.EnableCustomNameplateReColoring.Value);
        _mintTags?.Toggle(Config.EnabledMintTags.Value);
    }
}