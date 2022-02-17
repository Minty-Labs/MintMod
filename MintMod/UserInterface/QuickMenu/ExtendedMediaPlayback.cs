#if DEBUG
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using MintyLoader;
using WindowsMediaController;

namespace MintMod.UserInterface.QuickMenu {
    internal class ExtendedMediaPlayback : MintSubMod {
        public override string Name => "WindowsMediaController";
        public override string Description => "An Extended Library for Windows media playback, made by DubyaDube";
        
        //private static MediaManager.MediaSession? currentSession = null;

        private static readonly string path = Path.Combine(Environment.CurrentDirectory, "UserData", "MintMod", "Dependencies", "Mint.WMC.exe");

        private static ProcessStartInfo wmcInfo = new(path) {
            WindowStyle = ProcessWindowStyle.Hidden,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            StandardOutputEncoding = Encoding.UTF8
        };

        private static Process wmc = new() { StartInfo = wmcInfo };
        internal static Thread wmcThread;
        private static bool hasStarted;
        
        internal override void OnStart() {
            #region Extract from Manifest

            byte[] file;
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(
                       Assembly.GetCallingAssembly().GetManifestResourceNames().First(x => x.EndsWith("Mint.WMC.exe")))) {
                file = new byte[stream.Length];
                stream.Read(file, 0, (int)stream.Length);
                stream.Close();
            }

            if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, "UserData", "MintMod", "Dependencies"))) 
                Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "UserData", "MintMod", "Dependencies"));
            
            if (File.Exists(path)) {
                var existingFiles = File.ReadAllBytes(path);
                if (!file.SequenceEqual(existingFiles)) {
                    File.Delete(path);
                    File.WriteAllBytes(path, file);
                }
            }
            else File.WriteAllBytes(path, file);

            #endregion
        }

        internal override void OnApplicationQuit() {
            if (hasStarted) {
                wmc.Kill();
                wmcThread.Abort();
            }
        }

        public static void RunWMC() {
            try {
                if (File.Exists(path)) {
                    //if (MintUserInterface.MediaPlayback != null)
                    //    MintUserInterface.MediaPlayback.GameObject.GetComponent<UnityEngine.UI.Button>().interactable = false;
                    if (!Config.useWMC.Value) {
                        if (MintUserInterface.MediaPlayback != null)
                            MintUserInterface.MediaPlayback.Active = false;
                        return;
                    }

                    Con.Msg("Starting Mint.WMC");
                    wmc.Start();
                    wmcThread = new(WMC_Loop);
                    wmcThread.Start();
                    hasStarted = true;
                }
            }
            catch (Exception ex) {
                Con.Error(ex);
            }
        }

        private static void WMC_Loop() {
            /*var title = wmc.MainWindowTitle;
            var f = title.Split(':');
            if (MintUserInterface.SongPlaybackName != null)
                MintUserInterface.SongPlaybackName.Header.Title = f[1];
            */
            wmc.OutputDataReceived += POnOutputDataReceived;
            wmc.EnableRaisingEvents = true;
            wmc.BeginOutputReadLine();
            wmc.WaitForExit();
            wmc.CancelOutputRead();
        }

        public static List<string> Sources = new();
        private static void POnOutputDataReceived(object sender, DataReceivedEventArgs e) {
            //var _sources = e.Data.Split(':');
            var s = e.Data.Split(']');
            var _1 = s[1].Replace(".exe", ":").Split(':');
            Sources.Add(_1[0].Replace(" ", ""));
            var t = s[1].Replace(".exe", ": ");
            var f = t.Split(':');
            if (MintUserInterface.SongPlaybackName != null)
                MintUserInterface.SongPlaybackName.Header.Title = f[1].Replace(" is now playing ", "");
        }
    }
}
#endif