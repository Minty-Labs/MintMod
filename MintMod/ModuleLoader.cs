using System;
using System.Collections.Generic;
using System.Collections;
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

        internal MintSubMod() {
            if (Name == "MOD_NAME" || Description == "MOD_DESCRIPTION") return;
            Con.Msg($"{Name} has Loaded. {Description}");
        }

        internal virtual void OnUserInterface() {}

        internal virtual void OnStart() {}

        internal virtual void OnUpdate() {}

        internal virtual void OnLevelWasLoaded(int buildindex, string sceneName) {}

        internal virtual void OnGUI() {}

        internal virtual void OnPrefSave() {}

        internal virtual void OnApplicationQuit() {}
    }
}
