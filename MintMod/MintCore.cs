using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using MintMod.Functions.Authentication;
using MintMod.Managers.Notification;

namespace MintMod {
    public class MintCore : MelonMod {
        private MelonMod _instance;
        public static class ModBuildInfo {
            public static readonly string Name = DateTime.Now.Date == Con.Foolish ? "Walmart Client" : "MintMod";
            public const string Author = "Lily";
            public const string Company = "Minty Labs";
#if !DEBUG
            public const string Version = "2.29.0";
#endif
#if DEBUG
            public const string Version = "2.29.0";
#endif
            public const string DownloadLink = null;
            public const string UpdatedDate = "12 April 2022";
#if !DEBUG
            internal const string LoaderVer = "2.7.1";
#endif
        }

        internal static bool isDebug, cancelLoad;
        internal static readonly bool Fool = DateTime.Now.Date == Con.Foolish || Environment.CommandLine.Contains("--Foolish");

        internal static List<MintSubMod> mods = new();

        internal static readonly DirectoryInfo MintDirectory = new(Path.Combine(Environment.CurrentDirectory, "UserData", "MintMod"));

        private static int _scenesLoaded = 0;

        public static DateTime GameStartTimer;

        public override void OnApplicationStart() {
            GameStartTimer = DateTime.Now;
#if !DEBUG
            var s = MelonHandler.Mods.Single(m => m.Info.Name.Equals("MintyLoader")).Info.Version;
            //Con.Msg($"Loader Version is: {s}");
            if (s != ModBuildInfo.LoaderVer || MelonHandler.Mods.FindIndex(i => i.Info.Name == "MintyLoader") == -1) {
                cancelLoad = true;
                return;
            }
#endif
            _instance = this;
            if (!Directory.Exists(MintDirectory.FullName))
                Directory.CreateDirectory(MintDirectory.FullName);
#if DEBUG
            isDebug = true;
            if (Environment.CommandLine.Contains("--MintyDev"))
                isDebug = true;
#endif
#if !DEBUG
            if (Environment.CommandLine.Contains("--MintyDev")) isDebug = true;
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
            ModCompatibility.GPrivateServer = MelonHandler.Mods.FindIndex(i => {
                bool name = false, author = false;
                if (i.Info.Name == "PrivateServer")
                    name = true;
                if (i.Info.Author == "[information redacted]")
                    author = true;
                return name && author;
            }) != -1;
            mods.Add(new Config());
            mods.Add(new MintyResources());
            mods.Add(new Patches());
            mods.Add(new ServerAuth());
            mods.Add(new ESP());
            mods.Add(new KeyBindings());
            mods.Add(new Items());
            mods.Add(new LoadingWorldAudio());
            mods.Add(new MasterFinder());
            mods.Add(new ReFavs());
            mods.Add(new RankRecoloring());
            mods.Add(new Movement());
            mods.Add(new HudIcon());
            mods.Add(new ModCompatibility());
            mods.Add(new ReColor());
            mods.Add(new AvatarMenu());
            mods.Add(new SocialMenu());
            mods.Add(new MenuContentBackdrop());
            mods.Add(new SettingsMenu());
            mods.Add(new General());
            mods.Add(new WorldActions());
            mods.Add(new RiskyFuncAllower());
            mods.Add(new MintUserInterface());
            mods.Add(new Nameplates());
            mods.Add(new UserInterface.ActionMenu());
            mods.Add(new Players());
            mods.Add(new Components());
            mods.Add(new NotificationSystem());
            mods.Add(new GetRubyConfig());
            mods.Add(new NetworkEvents());
            mods.Add(new PlayerInfo());
            mods.Add(new StayMute());
            //mods.Add(new );

            ReMod.Core.Unity.EnableDisableListener.RegisterSafe();
            //ReMod.Core.Unity.RenderObjectListener.RegisterSafe();

            mods.ForEach(a => {
                try { /*Con.Debug($"---- Loading {a.Name}", isDebug);*/ a.OnStart(); }
                catch (Exception e) { Con.Error(e); }
            });
            Con.Debug($"Loaded {mods.Count} SubMods", isDebug);
        }

        public override void OnPreferencesSaved() {
#if !DEBUG
            if (cancelLoad) return;
#endif
            mods.ForEach(s => {
                try { s.OnPrefSave(); } catch (Exception e) { Con.Error(e); }
            });
        }

        public override void OnUpdate() {
#if !DEBUG
            if (cancelLoad) return;
#endif
            mods.ForEach(u => {
                try { u.OnUpdate(); } catch (Exception e) { Con.Error(e); }
            });
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName) {
#if !DEBUG
            if (cancelLoad) return;
#endif
            if (_scenesLoaded <= 2) {
                _scenesLoaded++;
                if (_scenesLoaded == 2)
                    MelonCoroutines.Start(ServerAuth.AuthUser());
            }
            
            mods.ForEach(s => {
                try { s.OnLevelWasLoaded(buildIndex, sceneName); } catch (Exception e) { Con.Error(e); }
            });
        }

        public override void OnApplicationQuit() {
            MelonPreferences.Save();
            mods.ForEach(s => {
                try { s.OnApplicationQuit(); } catch (Exception e) { Con.Error(e); }
            });
        }
    }
}
