using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MelonLoader;

[assembly: AssemblyTitle(MintyLoader.BuildInfo.Name)]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(MintyLoader.BuildInfo.Company)]
[assembly: AssemblyProduct(MintyLoader.BuildInfo.Name)]
[assembly: AssemblyCopyright("Copyright © " + MintyLoader.BuildInfo.Company + " 2022")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: AssemblyVersion(MintyLoader.BuildInfo.Version)]
[assembly: AssemblyFileVersion(MintyLoader.BuildInfo.Version)]
[assembly: MelonInfo(typeof(MintyLoader.MintyLoader),
    MintyLoader.BuildInfo.Name,
    MintyLoader.BuildInfo.Version,
    MintyLoader.BuildInfo.Author,
    MintyLoader.BuildInfo.DownloadLink)]
[assembly: MelonGame("VRChat", "VRChat")]
[assembly: MelonPriority(int.MinValue)]
