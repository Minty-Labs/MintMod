using MintMod.Libraries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace MintMod.UserInterface.OldUI {
    class SocialMenu : MintSubMod {
        public override string Name => "Social Menu";
        public override string Description => "Edits on the Social Menu";

        GameObject SocialMenu_AvatarBorder;

        internal override void OnUserInterface() {
            SocialMenu_AvatarBorder = GameObject.Find("/UserInterface/MenuContent/Screens/UserInfo/AvatarImage/AvatarBorder").gameObject;
            SocialMenu_AvatarBorder.GetComponent<Image>().color = ColorConversion.HexToColor("#00FFAAw");
        }
    }
}
