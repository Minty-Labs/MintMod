using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRC.SDKBase;

namespace MintMod.Managers {
    class Items : MintSubMod {
        public override string Name => "Item Manager";
        public override string Description => "";

        public static VRC_Pickup[] cached;

        internal override void OnLevelWasLoaded(int level, string sceneName) {
            cached = UnityEngine.Object.FindObjectsOfType<VRC_Pickup>();
        }
    }
}
