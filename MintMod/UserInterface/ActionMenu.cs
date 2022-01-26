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
    internal class ActionMenu : MintSubMod {
        public override string Name => "MintyActionMenu";
        public override string Description => "Adds some Mint functions to the ActionMenu.";
        
        private static readonly string[] AMApiOutdatedVersions = { "0.1.0", "0.1.2", "0.2.0", "0.2.1", "0.2.2", "0.2.3", "0.3.0", "0.3.1", "0.3.2", "0.3.3", "0.3.4" };
        // Target Version -> 0.3.5
        private static bool ranOnce, hasAMApiInstalled, AMApiOutdated, hasStarted;

        internal override void OnUserInterface() {
            if (MelonHandler.Mods.Any(m => m.Info.Name.Equals("ActionMenuApi")))
                hasAMApiInstalled = true;
            if (ranOnce/* || !MintCore.isDebug*/) return;
            DoAction();
        }

        private static void DoAction() {
            if (!Config.ActionMenuON.Value) return;
            if (MelonHandler.Mods.Single(m => m.Info.Name.Equals("ActionMenuApi")).Info.Version.Equals(AMApiOutdatedVersions)) {
                AMApiOutdated = true;
                Con.Warn("ActionMenuApi Outdated. Older versions are not supported, please update ActionMenuApi to v0.3.5 or above");
                return;
            }

            try {
                BuildMenu();
            }
            catch (Exception d) {
                Con.Error(d);
            }

            ranOnce = true;
        }

        private static void BuildMenu() {
            ActionMenuUtils.subMenu = VRCActionMenuPage.AddSubMenu(ActionMenuPage.Main, "<color=#82ffbe>MintMenu</color>", () => {
                CustomSubMenu.AddToggle("Freeze", PhotonFreeze.isCurrentlyFreezing, PhotonFreeze.ToggleFreeze, MintyResources.FreezeIcon);

                CustomSubMenu.AddToggle("ESP", Managers.ESP.isESPEnabled, Managers.ESP.PlayerESPState, MintyResources.ESPIcon);

                CustomSubMenu.AddButton("Jump", WorldActions.AddJump, MintyResources.JumpIcon);

                CustomSubMenu.AddToggle("Fly", Movement.FlightEnabled, Movement.Fly, MintyResources.FlyIcon);

                //if (MelonHandler.Mods.FindIndex(i => i.Info.Name == "TeleporterVR") != -1) {
                //    CustomSubMenu.AddToggle("VR Teleport\nto Cursor", TeleporterVR.Utils.VRUtils.active, choice =>
                //        TeleporterVR.Utils.VRUtils.active = choice, MintyResources.VRTPIcon);
                //}

                //CustomSubMenu.AddRadialPuppet("Fly Speed", f => Movement.finalSpeed = f * 5, 0.2f);
            }, MintyResources.MintIcon2D);
            hasStarted = true;
        }

        internal override void OnPrefSave() {
            if (!ranOnce && hasAMApiInstalled && Config.ActionMenuON.Value) 
                DoAction();
        }
    }
}
