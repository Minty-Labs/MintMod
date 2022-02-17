using MelonLoader;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle(MintMod.MintCore.ModBuildInfo.Name)]
[assembly: AssemblyDescription("Always Expanding simplistic VRChat Mod, that is done for fun.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(MintMod.MintCore.ModBuildInfo.Company)]
[assembly: AssemblyProduct(MintMod.MintCore.ModBuildInfo.Name)]
[assembly: AssemblyCopyright("Copyright © " + MintMod.MintCore.ModBuildInfo.Company + " 2022")]
[assembly: AssemblyTrademark(MintMod.MintCore.ModBuildInfo.Company)]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: Guid("ec3b33b6-5cee-49fd-85ee-275cfd030fd3")]
[assembly: AssemblyVersion(MintMod.MintCore.ModBuildInfo.Version)]
[assembly: AssemblyFileVersion(MintMod.MintCore.ModBuildInfo.Version)]
[assembly: NeutralResourcesLanguage("en")]
[assembly: MelonInfo(typeof(MintMod.MintCore), MintMod.MintCore.ModBuildInfo.Name,
    MintMod.MintCore.ModBuildInfo.Version, MintMod.MintCore.ModBuildInfo.Author,
    MintMod.MintCore.ModBuildInfo.DownloadLink)]
[assembly: MelonOptionalDependencies("UI Expansion Kit", "ActionMenuApi", "PortableMirrorMod")]
[assembly: MelonColor(System.ConsoleColor.White)]
[assembly: HarmonyDontPatchAll]
[assembly: MelonGame("VRChat", "VRChat")]
