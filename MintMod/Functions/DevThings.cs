using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using VRC.Core;

namespace MintMod.Functions {
    internal class DevThings : MintSubMod {
        public override string Name => "Dev Things";
        public override string Description => "Nothing for you to worry about.";
        
        [DllImport("user32.dll", EntryPoint = "SetWindowText")]
        private static extern bool SetWindowText(IntPtr hwnd, String lpString);
        
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(String className, String windowName);

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hwnd, int message, int wParam, IntPtr lParam);

        // [DllImport("kernel32.dll")]
        // private static extern IntPtr GetConsoleWindow();
        
        private const int WmSetIcon = 0x80;
        // private const int IconSmall = 0;
        private const int IconBig = 1;

        internal override void OnUserInterface() {
            if (!APIUser.CurrentUser.id.StartsWith("usr_6d71d3be")) return;
            
            var window = FindWindow(null, "VRChat");
            SetWindowText(window, "VRChat - MintMod (Dev)");

            var http = new HttpClient();
            http.DefaultRequestHeaders.Add("User-Agent", "MintMod; VRChat");
            var bytes = http.GetByteArrayAsync("https://mintlily.lgbt/mod/Images/icon.ico").GetAwaiter().GetResult();
            var stream = new MemoryStream(bytes);
            var icon = new Icon(stream);

            SendMessage(window, WmSetIcon, IconBig, icon.Handle);
        }
    }
}