using MintMod.Reflections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace MintMod.Functions {
    class RiskyFuncAllower : MintSubMod {
        public override string Name => "Risky Function Allower";
        public override string Description => "Forces Mods with Risky Function Checks to work";

        internal static readonly string Base = "CTBlockAction_";
        #region TeleporterVR Shit (And future emmVRC)
        internal readonly string UniEnable = "UniversalRiskyFuncEnable", UniDisable = "UniversalRiskyFuncDisable";
        #endregion

        internal override void OnLevelWasLoaded(int buildIndex, string sceneName) {
            //if (UIWrappers.GetWorld() == null) return;
            if (Config.bypassRiskyFunc.Value) {
                var activeScene = SceneManager.GetActiveScene();
                foreach (var rootGameObject in activeScene.GetRootGameObjects())
                    if (rootGameObject.name == "eVRCRiskFuncDisable" || rootGameObject.name == UniDisable || rootGameObject.name.Contains(Base))
                        Object.DestroyImmediate(rootGameObject);
                SceneManager.MoveGameObjectToScene(new GameObject("eVRCRiskFuncEnable"), activeScene);
                SceneManager.MoveGameObjectToScene(new GameObject(UniEnable), activeScene);
            }
        }
    }
}
