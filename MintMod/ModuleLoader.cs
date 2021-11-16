using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Il2CppSystem.Security.Cryptography;
using MintyLoader;
using MelonLoader;

namespace MintMod {
    internal class MintSubMod {
        public virtual string Name => "MOD_NAME";
        public virtual string Description => "MOD_DESCRIPTION";

        internal MintSubMod() => Con.Msg($"{Name} has Loaded. {Description}");

        internal virtual void OnUserInterface() {}

        internal virtual void OnQuickMenu() {}

        internal virtual void OnApplicationStart() {
            new Thread(() => {
                for (; ; ) {
                    Thread.Sleep(1000);
                    OnApplicationStart();
                }
            }) { IsBackground = true }.Start();
        }

        internal virtual void OnUpdate() {}

        internal virtual void OnLevelWasLoaded(int buildindex, string SceneName) {}

        internal virtual void OnGUI() {}

        internal virtual void OnPrefSave() {}
    }
}
