using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MelonLoader;
using MintMod.Hooks;
using MintMod.Resources;

namespace MintMod {

    public class MintCore : MelonMod {
        internal MelonMod Instance;
        public static class ModBuildInfo {
            public const string Name = "MintMod";
            public const string Author = "Lily";
            public const string Company = "LilyMod";
            public const string Version = "2.0.0";
            public const string DownloadLink = null;
            public const string UpdatedDate = "11/4/2021";
            public const string LoaderVer = "2.2.0";
            public static Version TargetMLVersion = new(0, 4, 3);
        }

        internal static bool AviFavsErrored, isDebug;

        internal static List<MintSubMod> mods = new();

        internal static readonly DirectoryInfo MintDirectory = new($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}UserData{Path.DirectorySeparatorChar}MintMod");

        public override void OnApplicationStart() {
            Instance = this;
            if (Environment.CommandLine.Contains("--MintyDev")) isDebug = true;

            var melonVersion = new Version(BuildInfo.Version);
            if (melonVersion != ModBuildInfo.TargetMLVersion) {
                MessageBox.Show("Your MelonLoader is outdated.", "Outdated Mod Loader");
                Process.Start("https://github.com/HerpDerpinstine/MelonLoader/releases/latest");
                MelonLogger.Warning("Your MelonLoader version is out of date, please update it.");
            }

            mods.Add(new Config());
            mods.Add(new GetAssembly());
            mods.Add(new Patches());
            mods.Add(new MintyResources());

            mods.ForEach(a => a.OnApplicationStart());
        }

        public override void OnPreferencesSaved() => mods.ForEach(s => s.OnPrefSave());

        public override void OnUpdate() => mods.ForEach(u => u.OnUpdate());
    }

    class GetAssembly : MintSubMod {
        public override string Name => "Yield Assembly";
        public override string Description => "";

        internal override void OnApplicationStart() => MelonCoroutines.Start(GetVRCAssembly());

        private System.Collections.IEnumerator GetVRCAssembly() {
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

        private System.Collections.IEnumerator WaitForUiManagerInit(Assembly assemblyCSharp) {
            Type vrcUiManager = assemblyCSharp.GetType("VRCUiManager");
            PropertyInfo uiManagerSingleton = vrcUiManager.GetProperties().First(pi => pi.PropertyType == vrcUiManager);

            while (uiManagerSingleton.GetValue(null) == null)
                yield return null;

            //ModCompatibility.RunIgnoreYieldedUI();
            //StartAuthCheck();
        }
    }
}
