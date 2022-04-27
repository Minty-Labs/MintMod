using MintMod.Reflections;
using MintMod.Resources;
using ReMod.Core.UI.QuickMenu;
using UnityEngine;

namespace MintMod.WorldSettings {
    internal class BlackCat : MintSubMod {
        public override string Name => "WorldSettings - Black Cat";
        public override string Description => "Various toggle control for the Black Cat world";
        
        private static string[] _onExitCollidersBooth = {
            "MIRRORS/Mirror (3)/Booth1 mirrors/Booth1 HQ Mirror/onexit",
            "MIRRORS/Mirror (3)/Booth1 mirrors/Booth1 LQ Mirror/onexit (1)",
            "MIRRORS/Mirror (3)/Booth1 mirrors/VRCPlayersOnlyMirror/onexit (2)"
        };
        private static string[] _onExitCollidersTop = {
            "MIRRORS/Mirror (4)/top mirrors/top HQ Mirror/onexit",
            "MIRRORS/Mirror (4)/top mirrors/top LQ Mirror/onexit (1)",
            "MIRRORS/Mirror (4)/top mirrors/VRCPlayersOnlyMirror/onexit (2)"
        };
        private static string[] _onExitCollidersBooth2 = {
            "MIRRORS/Mirror (2)/Booth1 mirrors/Booth1 HQ Mirror/onexit",
            "MIRRORS/Mirror (2)/Booth1 mirrors/Booth1 LQ Mirror/onexit (1)",
            "MIRRORS/Mirror (2)/Booth1 mirrors/VRCPlayersOnlyMirror/onexit (2)"
        };
        private static string[] _onExitCollidersBathroom1 = {
            "MIRRORS/Mirror/Bathroom1 mirrors/B1 HQ Mirror/onexit",
            "MIRRORS/Mirror/Bathroom1 mirrors/B1 LQ Mirror/onexit (1)",
            "MIRRORS/Mirror/Bathroom1 mirrors/VRCPlayersOnlyMirror/onexit (2)"
        };
        private static string[] _onExitCollidersBathroom2 = {
            "MIRRORS/Mirror (1)/Bathroom2 mirrors/B2 HQ Mirror/onexit",
            "MIRRORS/Mirror (1)/Bathroom2 mirrors/B2 LQ Mirror/onexit (1)",
            "MIRRORS/Mirror (1)/Bathroom2 mirrors/VRCPlayersOnlyMirror/onexit (2)"
        };
        
        private static ReMenuToggle _booth1, _booth2, _top, _bathroom1, _bathroom2;

        public static void ToggleComponents(string area, bool toggle) {
            var areaBundle = area switch {
                "Booth1"    => _onExitCollidersBooth,
                "Booth2"    => _onExitCollidersBooth2,
                "Top"       => _onExitCollidersTop,
                "Bathroom1" => _onExitCollidersBathroom1,
                "Bathroom2" => _onExitCollidersBathroom2,
                _ => null
            };

            if (areaBundle == null) return;
            foreach (var g in areaBundle) {
                var go = GameObject.Find(g);
                //go.GetComponent<UdonBehaviour>().enabled = toggle;
                go.SetActive(toggle);
            }
        }

        public static void BuildMenu(ReMenuCategory w) {
            var blackCat = w.AddCategoryPage("Back Cat", "Opens a menu for Specific Bat Cat functions");
            var bc = blackCat.AddCategory("Mirror Collider Toggles");
            
            _booth1 = bc.AddToggle("Booth 1", "Disables onExit game object for the Booth 1 area", b => ToggleComponents("Booth1", b), true);
            _booth2 = bc.AddToggle("Booth 2", "Disables onExit game object for the Booth 2 area", b => ToggleComponents("Booth2", b), true);
            _top = bc.AddToggle("Up Stairs", "Disables onExit game object for the up stairs area", b => ToggleComponents("Top", b), true);
            _bathroom1 = bc.AddToggle("Bathroom 1", "Disables onExit game object for the Bathroom 1 area", b => ToggleComponents("Bathroom1", b), true);
            _bathroom2 = bc.AddToggle("Bathroom 2", "Disables onExit game object for the Bathroom 2 area", b => ToggleComponents("Bathroom2", b), true);
        }

        internal override void OnLevelWasLoaded(int buildindex, string sceneName) {
            if (buildindex != -1) return;
            _booth1?.Toggle(true);
            _booth2?.Toggle(true);
            _top?.Toggle(true);
            _bathroom1?.Toggle(true);
            _bathroom2?.Toggle(true);
        }
    }
}