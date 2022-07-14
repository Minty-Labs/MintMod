using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MelonLoader;
using MintMod.Functions;
using MintMod.Libraries;
using MintMod.Managers;
using MintMod.Resources;
using MintMod.UserInterface;
using MintMod.UserInterface.AvatarFavs;
using MintMod.UserInterface.OldUI;
using MintMod.UserInterface.QuickMenu;
using MintMod.Utils;
using MintyLoader;
using MintMod.Functions.Authentication;

namespace MintMod {
    public class MintCore : MelonMod {
        public static class ModBuildInfo {
            public static readonly string Name = DateTime.Now.Date == Con.Foolish || Environment.CommandLine.Contains("--Foolish") ? "Walmart Client" : "MintMod";
            public const string Author = "Lily";
            public const string Company = "Minty Labs";
#if !DEBUG
            public const string Version = "2.36.2";
            internal const string LoaderVer = "2.9.3";
#else
            public const string Version = "2.XX.0";
            internal const string LoaderVer = "2.9.3";
#endif
            public const string DownloadLink = null;
            public const string UpdatedDate = "13 July 2022";
        }

        internal static bool IsDebug, CancelLoad;
        internal static readonly bool Fool = DateTime.Now.Date == Con.Foolish || Environment.CommandLine.Contains("--Foolish");

        internal static List<MintSubMod> Modules = new();

        internal static readonly DirectoryInfo MintDirectory = new(Path.Combine(Environment.CurrentDirectory, "UserData", "MintMod"));

        private static int _scenesLoaded = 0;

        public static DateTime GameStartTimer;

        public override void OnApplicationStart() {
            GameStartTimer = DateTime.Now;
#if !DEBUG
            var s = MelonHandler.Mods.Single(m => m.Info.Name.Equals("MintyLoader")).Info.Version;
            //Con.Msg($"Loader Version is: {s}");
            if (s != ModBuildInfo.LoaderVer || MelonHandler.Mods.FindIndex(i => i.Info.Name == "MintyLoader") == -1) {
                CancelLoad = true;
                return;
            }
#endif
            
            if (!Directory.Exists(MintDirectory.FullName))
                Directory.CreateDirectory(MintDirectory.FullName);
#if DEBUG
            IsDebug = true;
            if (Environment.CommandLine.Contains("--MintyDev"))
                IsDebug = true;
#endif
#if !DEBUG
            if (Environment.CommandLine.Contains("--MintyDev")) IsDebug = true;
            /*
            var melonVersion = new Version(BuildInfo.Version);
            if (melonVersion != ModBuildInfo.TargetMLVersion) {
                MessageBox.Show("Your MelonLoader is outdated.", "Outdated Mod Loader");
                Process.Start("https://github.com/HerpDerpinstine/MelonLoader/releases/latest");
                Con.Warn("Your MelonLoader version is out of date, please update it.");
            }
            */
#endif

            Con.Msg($"Starting {ModBuildInfo.Name} v{ModBuildInfo.Version} - Built on {ModBuildInfo.UpdatedDate}");
            
            Modules.Add(new Config());
            Modules.Add(new ModCompatibility());
            Modules.Add(new MintyResources());
            Modules.Add(new Patches());
            Modules.Add(new ServerAuth());
            Modules.Add(new ESP());
            Modules.Add(new KeyBindings());
            Modules.Add(new Items());
            Modules.Add(new LoadingWorldAudio());
            Modules.Add(new MasterFinder());
            Modules.Add(new ReFavs());
            Modules.Add(new RankRecoloring());
            Modules.Add(new Movement());
            Modules.Add(new HudIcon());
            Modules.Add(new ReColor());
            Modules.Add(new AvatarMenu());
            Modules.Add(new SocialMenu());
            Modules.Add(new MenuContentBackdrop());
            Modules.Add(new SettingsMenu());
            Modules.Add(new General());
            Modules.Add(new WorldActions());
            Modules.Add(new RiskyFuncAllower());
            Modules.Add(new MintUserInterface());
            Modules.Add(new Nameplates());
            Modules.Add(new UserInterface.ActionMenu());
            Modules.Add(new Players());
            Modules.Add(new Components());
            Modules.Add(new GetRubyConfig());
            Modules.Add(new NetworkEvents());
            Modules.Add(new PlayerInfo());
            Modules.Add(new StayMute());
            Modules.Add(new WorldSettings.BlackCat());
            Modules.Add(new CreateListener());
            Modules.Add(new ExtraJSONData.OldMate());
            Modules.Add(new Keyboard());
            
            Modules.Add(new DevThings());
            // mods.Add(new );

            ReMod.Core.Unity.EnableDisableListener.RegisterSafe();

            Modules.ForEach(a => {
                try { /*Con.Debug($"---- Loading {a.Name}", isDebug);*/ a.OnStart(); }
                catch (Exception e) { Con.Error(e); }
            });
            Con.Debug($"Loaded {Modules.Count} SubMods", IsDebug);
        }

        public override void OnPreferencesSaved() {
#if !DEBUG
            if (CancelLoad) return;
#endif
            Modules.ForEach(s => {
                try { s.OnPrefSave(); } catch (Exception e) { Con.Error(e); }
            });
        }

        public override void OnUpdate() {
#if !DEBUG
            if (CancelLoad) return;
#endif
            Modules.ForEach(u => {
                try { u.OnUpdate(); } catch (Exception e) { Con.Error(e); }
            });
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName) {
#if !DEBUG
            if (CancelLoad) return;
#endif
            if (_scenesLoaded <= 2) {
                _scenesLoaded++;
                if (_scenesLoaded == 2)
                    MelonCoroutines.Start(ServerAuth.AuthUser());
            }
            
            Modules.ForEach(s => {
                try { s.OnLevelWasLoaded(buildIndex, sceneName); } catch (Exception e) { Con.Error(e); }
            });
        }

        public override void OnApplicationQuit() {
            MelonPreferences.Save();
            Modules.ForEach(s => {
                try { s.OnApplicationQuit(); } catch (Exception e) { Con.Error(e); }
            });
        }
    }
}
