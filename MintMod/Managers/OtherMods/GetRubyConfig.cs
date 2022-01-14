using Newtonsoft.Json;
using System;
using System.IO;
using MintyLoader;

namespace MintMod.Managers {
    public class RubyLaunchConfig { public int SelectedMode { get; set; } }
    internal class GetRubyConfig : MintSubMod {
        public override string Name => "RubyDetector";
        public override string Description => "Detects if Ruby is installed and Running.";
        private readonly string _launch = Path.Combine(Environment.CurrentDirectory, "RubyClient", "LauncherConfig.json");
        private RubyLaunchConfig RubyLaunchConfig { get; set; }
        public static bool HasRubyActive { get; private set; }
        private RubyLaunchConfig ReadConfig() => RubyLaunchConfig;

        internal override void OnUserInterface() {
            if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, "RubyClient"))) return;
            if (File.Exists(_launch)) RubyLaunchConfig = JsonConvert.DeserializeObject<RubyLaunchConfig>(File.ReadAllText(_launch));
            
            HasRubyActive = ReadConfig().SelectedMode == 1;
            
            if (HasRubyActive) Con.Debug("Detected RubyClient");
        }
    }
}