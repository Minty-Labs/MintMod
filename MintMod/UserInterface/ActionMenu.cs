using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActionMenuApi.Api;
using ActionMenuApi.Pedals;
using MintMod.Functions;
using MintMod.Reflections;
using MintMod.Resources;
using MintyLoader;

namespace MintMod.UserInterface {
    class ActionMenu : MintSubMod {
        private static readonly string[] AMApiOutdatedVersions = { "0.1.0", "0.1.2", "0.2.0", "0.2.1", "0.2.2", "0.2.3", "0.3.0", "0.3.1", "0.3.2", "0.3.3", "0.3.4" };
        // Target Version -> 0.3.5
        public static bool hasAMApiInstalled, AMApiOutdated, hasStarted;
        private static PedalOption Freeze, ESP, Jump, Fly, VRTP;
        private static bool ranOnce;

        internal override void OnUserInterface() {
            if (ranOnce) return;
            if (MelonHandler.Mods.Any(m => m.Info.Name.Equals("ActionMenuApi"))) {
                hasAMApiInstalled = true;
                if (!Config.ActionMenuON.Value) return;
                if (MelonHandler.Mods.Single(m => m.Info.Name.Equals("ActionMenuApi")).Info.Version.Equals(AMApiOutdatedVersions)) {
                    AMApiOutdated = true;
                    Con.Warn("ActionMenuApi Outdated. Older verions are not supported, please update ActionMenuApi to v0.3.5 or above");
                    return;
                } else BuildMenu();
                ranOnce = true;
            }
        }

        private static void BuildMenu() {
            ActionMenuUtils.subMenu = VRCActionMenuPage.AddSubMenu(ActionMenuPage.Main, "<color=#82ffbe>MintMenu</color>", () => {
                //Freeze = CustomSubMenu.AddToggle("Freeze", false, PhotonFreeze.ToggleFreeze, MintyResources.FreezeIcon);

                ESP = CustomSubMenu.AddToggle("ESP", false, Managers.ESP.PlayerESPState, MintyResources.JumpIcon);

                Jump = CustomSubMenu.AddButton("Jump", WorldActions.AddJump, MintyResources.JumpIcon);

                Fly = CustomSubMenu.AddToggle("Fly", false, Movement.Fly, MintyResources.FlyIcon);

                if (MelonHandler.Mods.FindIndex((MelonMod i) => i.Info.Name == "TeleporterVR") != -1)
                    VRTP = CustomSubMenu.AddToggle("VR Teleport\nto Cursor", false, choice =>
                        TeleporterVR.Utils.VRUtils.active = choice, MintyResources.VRTPIcon);

                CustomSubMenu.AddRadialPuppet("Fly Speed", f => Movement.finalSpeed = f * 2, 0f);
            }, MintyResources.MintIcon2D);
            hasStarted = true;
        }
    }
}
