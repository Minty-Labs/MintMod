using System;
using System.Collections.ObjectModel;
using System.Text;
using WindowsMediaController;
using Windows.Media.Control;

namespace Mint.WMC {
    class Program {
        static MediaManager mediaManager;

        static void Main() {
            Console.OutputEncoding = Encoding.UTF8;
            mediaManager = new MediaManager();

            mediaManager.OnAnySessionOpened += MediaManager_OnAnySessionOpened;
            mediaManager.OnAnySessionClosed += MediaManager_OnAnySessionClosed;
            mediaManager.OnAnyPlaybackStateChanged += MediaManager_OnAnyPlaybackStateChanged;
            mediaManager.OnAnyMediaPropertyChanged += MediaManager_OnAnyMediaPropertyChanged;

            mediaManager.Start();
            Console.ReadLine();
            mediaManager.Dispose();
        }

        private static string tempSession;
        public static void MediaManager_OnAnySessionOpened(MediaManager.MediaSession session) {
            WriteLineColor("-- New Source: " + session.Id, ConsoleColor.Green);
            tempSession = session.Id.Replace(".exe", "");
        }

        public static void MediaManager_OnAnySessionClosed(MediaManager.MediaSession session) {
            WriteLineColor("-- Removed Source: " + session.Id, ConsoleColor.Red);
        }

        private static bool tempPlaybackStatus;
        public static void MediaManager_OnAnyPlaybackStateChanged(MediaManager.MediaSession sender, GlobalSystemMediaTransportControlsSessionPlaybackInfo args) {
            WriteLineColor($"{sender.Id} is now {args.PlaybackStatus}", ConsoleColor.Yellow);
            tempPlaybackStatus = args.PlaybackStatus.ToString() == "Paused";
            Console.Title = string.Format($"{tempSession}: {tempArtist} - {tempTitle}{(tempPlaybackStatus ? " (Paused)" : "")}");
        }

        private static string tempTitle, tempArtist;
        public static void MediaManager_OnAnyMediaPropertyChanged(MediaManager.MediaSession sender, GlobalSystemMediaTransportControlsSessionMediaProperties args) {
            WriteLineColor($"{sender.Id} is now playing {args.Title} {(String.IsNullOrEmpty(args.Artist) ? "" : $"by {args.Artist}")}", ConsoleColor.Cyan);
            tempTitle = args.Title;
            tempArtist = $"{(String.IsNullOrEmpty(args.Artist) ? "" : $"{args.Artist}")}";
            Console.Title = string.Format($"{tempSession}: {tempArtist} - {tempTitle}{(tempPlaybackStatus ? " (Paused)" : "")}");
        }

        public static void WriteLineColor(object toprint, ConsoleColor color = ConsoleColor.Gray) {
            Console.ForegroundColor = color;
            Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss.fff") + "] " + toprint);
        }
    }
}