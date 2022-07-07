using System.Diagnostics;
using MintMod.Resources;
using MintyLoader;
using ReMod.Core.UI.QuickMenu;
using ReMod.Core.VRChat;

namespace MintMod.UserInterface.QuickMenu; 

public static class MintInfo {
    private static ReCategoryPage _mintInfo;

    internal static void BuildMenu(ReMenuCategory baseActions) {
        _mintInfo = baseActions.AddCategoryPage("Mint Info", "Information about MintMod", MintyResources.MintTabIcon);
        var m = _mintInfo.AddCategory("MintMod Information");
        
        m.AddButton($"Version {MintCore.ModBuildInfo.Version}", $"MintMod Version {MintCore.ModBuildInfo.Version}", () => { }, MintyResources.clipboard);
        m.AddButton("Users: 46", "46 Currently using MintMod", () => { }, MintyResources.user);
        m.AddButton("MOTD", "Current Message of the day", () =>
            QuickMenuEx.Instance.ShowAlertDialog("Message of the Day!", Con.MessageOfTheDay, "Close", () => { }), MintyResources.Megaphone);
        
        
        var c = _mintInfo.AddCategory("Credits");
        c.AddButton("Lily", "Creator of MintMod", () => 
            QuickMenuEx.Instance.ShowConfirmDialog("Lily", "Creator of MintMod", "GitHub", "Ok", () =>
                OpenWebpage("https://github.com/MintLily"), () => { }), MintyResources.MintIcon);
        
        c.AddButton("Elly", "Server host owner, helper", () =>
            QuickMenuEx.Instance.ShowConfirmDialog("Elly", "Server host owner, helper", "GitHub", "Ok", () =>
                OpenWebpage("https://github.com/EllyVR"), () => { }), MintyResources.user);
        
        c.AddButton("DDAkebono", "Mint Auth / Nameplate API host", () => 
            QuickMenuEx.Instance.ShowConfirmDialog("DDAkebono", "Mint Auth / Nameplate API host", "GitHub", "Ok", () =>
                OpenWebpage("https://github.com/DDAkebono"), () => { }), MintyResources.user);
        
        c.AddButton("Requi", "Creator of ReMod.Core", () => 
            QuickMenuEx.Instance.ShowConfirmDialog("Requi", "Creator of ReMod.Core", "GitHub", "Ok", () =>
                OpenWebpage("https://github.com/RequiDev"), () => { }), MintyResources.user);
        
        c.AddButton("gompocp", "ActionMenuApi creator", () => 
            QuickMenuEx.Instance.ShowConfirmDialog("gompocp", "Creator of ActionMenuApi", "GitHub", "Ok", () =>
                OpenWebpage("https://github.com/gompocp/ActionMenuApi"), () => { }), MintyResources.user);
        
        c.AddButton("Penny", "Idea of the custom auto ReMod.Core Updater", () => 
            QuickMenuEx.Instance.ShowConfirmDialog("Penny", "Idea of the custom auto ReMod.Core Updater", "GitHub", "Ok", () =>
                OpenWebpage("https://github.com/PennyBunny"), () => { }), MintyResources.user);
        
        // c.AddButton("Rin", "Loader auth, mod supplier logic through loader", () => { }, MintyResources.user);
    }
    
    private static void OpenWebpage(string site) => Process.Start("cmd", $"/C start {site}");
}