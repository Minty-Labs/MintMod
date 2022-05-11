using MelonLoader;
using MintMod.Functions;
using MintMod.Reflections;
using ReMod.Core.UI.QuickMenu;
using UnityEngine;
using VRC.Core;

namespace MintMod.WorldSettings {
    internal class BlackCat : MintSubMod {
        public override string Name => "WorldSettings - Black Cat";
        public override string Description => "Various toggle control for the Black Cat world";
        
        private static readonly string[] OnExitCollidersBooth = {
            "MIRRORS/Mirror (3)/Booth1 mirrors/Booth1 HQ Mirror/onexit",
            "MIRRORS/Mirror (3)/Booth1 mirrors/Booth1 LQ Mirror/onexit (1)",
            "MIRRORS/Mirror (3)/Booth1 mirrors/VRCPlayersOnlyMirror/onexit (2)"
        };
        private static readonly string[] OnExitCollidersTop = {
            "MIRRORS/Mirror (4)/top mirrors/top HQ Mirror/onexit",
            "MIRRORS/Mirror (4)/top mirrors/top LQ Mirror/onexit (1)",
            "MIRRORS/Mirror (4)/top mirrors/VRCPlayersOnlyMirror/onexit (2)"
        };
        private static readonly string[] OnExitCollidersBooth2 = {
            "MIRRORS/Mirror (2)/Booth1 mirrors/Booth1 HQ Mirror/onexit",
            "MIRRORS/Mirror (2)/Booth1 mirrors/Booth1 LQ Mirror/onexit (1)",
            "MIRRORS/Mirror (2)/Booth1 mirrors/VRCPlayersOnlyMirror/onexit (2)"
        };
        private static readonly string[] OnExitCollidersBathroom1 = {
            "MIRRORS/Mirror/Bathroom1 mirrors/B1 HQ Mirror/onexit",
            "MIRRORS/Mirror/Bathroom1 mirrors/B1 LQ Mirror/onexit (1)",
            "MIRRORS/Mirror/Bathroom1 mirrors/VRCPlayersOnlyMirror/onexit (2)"
        };
        private static readonly string[] OnExitCollidersBathroom2 = {
            "MIRRORS/Mirror (1)/Bathroom2 mirrors/B2 HQ Mirror/onexit",
            "MIRRORS/Mirror (1)/Bathroom2 mirrors/B2 LQ Mirror/onexit (1)",
            "MIRRORS/Mirror (1)/Bathroom2 mirrors/VRCPlayersOnlyMirror/onexit (2)"
        };
        
        private static ReMenuToggle _booth1, _booth2, _top, _bathroom1, _bathroom2;

        private static void ToggleComponents(string area, bool toggle, MelonPreferences_Category cat, MelonPreferences_Entry<bool> entry) {
            var areaBundle = area switch {
                "Booth1"    => OnExitCollidersBooth,
                "Booth2"    => OnExitCollidersBooth2,
                "Top"       => OnExitCollidersTop,
                "Bathroom1" => OnExitCollidersBathroom1,
                "Bathroom2" => OnExitCollidersBathroom2,
                _ => null
            };

            if (areaBundle == null) return;
            foreach (var g in areaBundle) {
                var go = GameObject.Find(g);
                //go.GetComponent<UdonBehaviour>().enabled = toggle;
                go.SetActive(toggle);
            }

            Config.SavePrefValue(cat, entry, toggle);
        }

        public static void BuildMenu(ReMenuCategory w) {
            var blackCat = w.AddCategoryPage("Back Cat", "Opens a menu for Specific Bat Cat functions");
            var bc = blackCat.AddCategory("Mirror Collider Toggles");
            
            _booth1 = bc.AddToggle("Booth 1", "Disables onExit game object for the Booth 1 area", 
                b => ToggleComponents("Booth1", b, Config.WorldSettings_BlackCat, Config.BC_Booth1), Config.BC_Booth1.Value);
            
            _booth2 = bc.AddToggle("Booth 2", "Disables onExit game object for the Booth 2 area", 
                b => ToggleComponents("Booth2", b, Config.WorldSettings_BlackCat, Config.BC_Booth2), Config.BC_Booth2.Value);
            
            _top = bc.AddToggle("Up Stairs", "Disables onExit game object for the up stairs area", 
                b => ToggleComponents("Top", b, Config.WorldSettings_BlackCat, Config.BC_Top), Config.BC_Top.Value);
            
            _bathroom1 = bc.AddToggle("Bathroom 1", "Disables onExit game object for the Bathroom 1 area", 
                b => ToggleComponents("Bathroom1", b, Config.WorldSettings_BlackCat, Config.BC_Bathroom1), Config.BC_Bathroom1.Value);
            
            _bathroom2 = bc.AddToggle("Bathroom 2", "Disables onExit game object for the Bathroom 2 area", 
                b => ToggleComponents("Bathroom2", b, Config.WorldSettings_BlackCat, Config.BC_Bathroom2), Config.BC_Bathroom2.Value);
        }

        internal override void OnLevelWasLoaded(int buildindex, string sceneName) {
            if (buildindex != -1) return;
            if (WorldReflect.GetWorldInstance().id != "wrld_4cf554b4-430c-4f8f-b53e-1f294eed230b") return;
            _booth1?.Toggle(Config.BC_Booth1.Value);
            _booth2?.Toggle(Config.BC_Booth2.Value);
            _top?.Toggle(Config.BC_Top.Value);
            _bathroom1?.Toggle(Config.BC_Bathroom1.Value);
            _bathroom2?.Toggle(Config.BC_Bathroom2.Value);
        }
    }
}