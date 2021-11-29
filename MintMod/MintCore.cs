using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Windows.Forms;
using MelonLoader;
using MintMod.Functions;
using MintMod.Hooks;
using MintMod.Libraries;
using MintMod.Managers;
using MintMod.Resources;
using MintMod.UserInterface;
using MintMod.UserInterface.AvatarFavs;
using MintMod.UserInterface.OldUI;
using MintMod.UserInterface.QuickMenu;
using MintMod.Utils;
using MintyLoader;
using BuildInfo = MelonLoader.BuildInfo;
using MintMod.Functions.Authentication;

namespace MintMod {
    public class MintCore : MelonMod {
        internal MelonMod Instance;
        public static class ModBuildInfo {
            public const string Name = "MintMod";
            public const string Author = "Lily";
            public const string Company = "LilyMod";
            public const string Version = "2.2.1";
            public const string DownloadLink = null;
            public const string UpdatedDate = "11/28/2021";
#if !DEBUG
            public const string LoaderVer = "2.3.0.1";
            public static Version TargetMLVersion = new(0, 4, 3);
#endif
        }

        internal static bool isDebug, canelLoad;

        internal static List<MintSubMod> mods = new();

        internal static readonly DirectoryInfo MintDirectory = new($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}UserData{Path.DirectorySeparatorChar}MintMod");

        public override void OnApplicationStart() {
#if !DEBUG
            var s = MelonHandler.Mods.Single(m => m.Info.Name.Equals("MintyLoader")).Info.Version;
            if (s != "2.3.0.1" || s != ModBuildInfo.LoaderVer) {
                canelLoad = true;
                return;
            }
#endif
            Instance = this;
            if (!Directory.Exists(MintDirectory.FullName))
                Directory.CreateDirectory(MintDirectory.FullName);
#if DEBUG
            isDebug = true;
            if (Environment.CommandLine.Contains("--MintyDev"))
                isDebug = true;
#endif
#if !DEBUG
            if (Environment.CommandLine.Contains("--MintyDev")) isDebug = true;
            var melonVersion = new Version(BuildInfo.Version);
            if (melonVersion != ModBuildInfo.TargetMLVersion) {
                MessageBox.Show("Your MelonLoader is outdated.", "Outdated Mod Loader");
                Process.Start("https://github.com/HerpDerpinstine/MelonLoader/releases/latest");
                Con.Warn("Your MelonLoader version is out of date, please update it.");
            }
#endif

            Con.Msg($"Starting {ModBuildInfo.Name} v{ModBuildInfo.Version}");
            mods.Add(new Config());
            mods.Add(new GetAssembly());
            mods.Add(new Patches());
            mods.Add(new MintyResources());
            mods.Add(new ServerAuth());
            mods.Add(new ESP());
            mods.Add(new KeyBindings());
            mods.Add(new Items());
            mods.Add(new LoadingWorldAudio());
            mods.Add(new MasterFinder());
            mods.Add(new AviFavLogic());
            mods.Add(new RankRecoloring());
            mods.Add(new Movement());
            mods.Add(new HudIcon());
            mods.Add(new ModCompatibility());
            mods.Add(new ReColor());
            mods.Add(new AvatarMenu());
            mods.Add(new SocialMenu());
            mods.Add(new MenuContentBackdrop());
            mods.Add(new UserInterface.OldUI.SettingsMenu());
            mods.Add(new General());
            mods.Add(new WorldActions());
            mods.Add(new RiskyFuncAllower());
            mods.Add(new MintUserInterface());
            mods.Add(new Nameplates());
            mods.Add(new UserInterface.ActionMenu());
            mods.Add(new Players());
            //mods.Add(new );

            MelonCoroutines.Start(Utils.Network.OnYieldStart());

            ReMod.Core.Unity.EnableDisableListener_2.RegisterSafe();
            //ReMod.Core.Unity.RenderObjectListener.RegisterSafe();

            mods.ForEach(a => {
                try { a.OnStart(); }
                catch (Exception e) { Con.Error($"{e}"); }
            });
            Con.Debug($"Loaded {mods.Count} SubMods", isDebug);
        }

        public override void OnPreferencesSaved() {
#if !DEBUG
            if (canelLoad) return;
#endif
            mods.ForEach(s => {
                try { s.OnPrefSave(); } catch (Exception e) { Con.Error($"{e}"); }
            });
        }

        public override void OnUpdate() {
#if !DEBUG
            if (canelLoad) return;
#endif
            mods.ForEach(u => {
                try { u.OnUpdate(); } catch (Exception e) { Con.Error($"{e}"); }
            });
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName) {
#if !DEBUG
            if (canelLoad) return;
#endif
            mods.ForEach(s => {
                try { s.OnLevelWasLoaded(buildIndex, sceneName); } catch (Exception e) { Con.Error($"{e}"); }
            });
        }

        public override void OnApplicationQuit() => MelonPreferences.Save();
    }

    class GetAssembly : MintSubMod {
        public override string Name => "Yield Assembly";
        public override string Description => "Waits for VRCUiManager to be called.";

        internal override void OnStart() => MelonCoroutines.Start(GetVRCAssembly());

        private IEnumerator GetVRCAssembly() {
            Assembly assemblyCSharp = null;
            while (true) {
                assemblyCSharp = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly => assembly.GetName().Name == "Assembly-CSharp");
                if (assemblyCSharp == null)
                    yield return null;
                else
                    break;
            }

            MelonCoroutines.Start(WaitForUiManagerInit(assemblyCSharp));
        }

        private IEnumerator WaitForUiManagerInit(Assembly assemblyCSharp) {
            Type vrcUiManager = assemblyCSharp.GetType("VRCUiManager");
            PropertyInfo uiManagerSingleton = vrcUiManager.GetProperties().First(pi => pi.PropertyType == vrcUiManager);

            while (uiManagerSingleton.GetValue(null) == null)
                yield return null;

            //ModCompatibility.RunIgnoreYieldedUI();
            MelonCoroutines.Start(ServerAuth.AuthUser());
        }

        internal static IEnumerator YieldUI() {
#if !DEBUG
            if (MintCore.canelLoad) yield break;
#endif
            MintCore.mods.ForEach(u => {
                try { u.OnUserInterface(); }
                catch (Exception e) { Con.Error($"{e}"); }
            });
            MelonCoroutines.Start(MintUserInterface.OnQuickMenu());
            MelonCoroutines.Start(MintUserInterface.OnUserSelectMenu());
            yield break;
        }
    }
}
