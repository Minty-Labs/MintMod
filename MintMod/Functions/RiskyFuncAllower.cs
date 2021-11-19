using MintMod.Reflections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace MintMod.Functions {
    class RiskyFuncAllower : MintSubMod {
        public override string Name => "Risky Function Allower";
        public override string Description => "Forces Mods with Risky Function Checks to work";

        #region ComponentToggle Shit
        internal static readonly string Base = "CTBlockAction_";
        internal readonly string[] names = { $"{Base}1", $"{Base}2", $"{Base}3", $"{Base}4", $"{Base}5", $"{Base}6", $"{Base}7" };
        #endregion
        #region TeleporterVR Shit (And future emmVRC)
        internal readonly string UniEnable = "UniversalRiskyFuncEnable", UniDisable = "UniversalRiskyFuncDisable";
        #endregion

        internal override void OnLevelWasLoaded(int buildIndex, string sceneName) {
            if (UIWrappers.GetWorld() == null) return;
            Scene activeScene = SceneManager.GetActiveScene();
            foreach (GameObject rootGameObject in activeScene.GetRootGameObjects())
                if (rootGameObject.name == "eVRCRiskFuncDisable" || rootGameObject.name == UniDisable ||  rootGameObject.name == names[0] || rootGameObject.name == names[1] || rootGameObject.name == names[2] || rootGameObject.name == names[3] || rootGameObject.name == names[4] || rootGameObject.name == names[5] || rootGameObject.name == names[6])
                    Object.DestroyImmediate(rootGameObject);
            SceneManager.MoveGameObjectToScene(new GameObject("eVRCRiskFuncEnable"), activeScene);
            SceneManager.MoveGameObjectToScene(new GameObject(UniEnable), activeScene);
        }
    }
}
