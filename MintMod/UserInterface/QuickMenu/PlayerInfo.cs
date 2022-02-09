using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using MelonLoader;
using MintMod;
using MintMod.Reflections;
using MintMod.Resources;
using MintyLoader;
using ReMod.Core.VRChat;
using VRC;
using VRC.Core;
using VRC.DataModel;
using VRCSDK2;
using VRCSDK2.Validation.Performance;
using AvatarPerformanceRating = EnumPublicSealedvaNoExGoMePoVe7v0;

namespace MintMod.UserInterface.QuickMenu {
    internal class PlayerInfo : MintSubMod {
        public override string Name => "PlayerInfoHUD";
        public override string Description => "Shows Player Info on selected user";

        private Transform QuickMenu;
        private GameObject BackgroundObject, TextObject;
        private Image BackgroundImage;
        private Text TheText;
        private bool Initialized;

        internal override void OnStart() => MelonCoroutines.Start(Init());

        private IEnumerator Init() {
            if (!Config.PLEnabled.Value) yield break;
            while (MintyResources.BG_HUD == null) yield return null;
            while (UnityEngine.Object.FindObjectOfType<VRC.UI.Elements.QuickMenu>() == null) yield return null;
            QuickMenu = UnityEngine.Object.FindObjectOfType<VRC.UI.Elements.QuickMenu>().gameObject.transform;
            var LeftWing = QuickMenu.Find("Container/Window/Wing_Left/");
            
            BackgroundObject = new GameObject("MintUIMHUD Panel");
            BackgroundObject.transform.SetParent(LeftWing, false);
            BackgroundObject.AddComponent<CanvasRenderer>();
            BackgroundImage = BackgroundObject.AddComponent<Image>();
            
            BackgroundObject.GetComponent<RectTransform>().sizeDelta = new Vector2(1200f, 1420f);
            BackgroundObject.GetComponent<RectTransform>().localPosition = new Vector3(-1000, -500, -0.5f);
            BackgroundImage.sprite = MintyResources.BG_HUD;
            
            TextObject = new GameObject("Player List");
            TextObject.transform.SetParent(BackgroundObject.transform, false);
            TextObject.AddComponent<CanvasRenderer>();
            TheText = TextObject.AddComponent<Text>();
            TextObject.GetComponent<RectTransform>().sizeDelta = new Vector2(1200, 1410f);
            if (MintyResources.BalooFont != null)
                TheText.font = MintyResources.BalooFont;
            else
                TheText.font = UnityEngine.Resources.GetBuiltinResource<Font>("Arial.ttf");
            TheText.fontSize = 42;
            TheText.text = "";
            TheText.alignment = (TextAnchor)TextAlignment.Left;
            Initialized = true;
            
            MelonCoroutines.Start(HUDLoop());
            
            TheText.GetComponent<RectTransform>().anchoredPosition = new Vector2(130, 125);
        }

        private IEnumerator HUDLoop() {
            while (Initialized) {
                //BackgroundObject.GetComponent<RectTransform>().localPosition = new Vector3(-1000, -500, -0.5f);
                yield return new WaitForSeconds(1);
                TheText.text = string.Concat(new[] {
                    "\n\n",
                    "<b>Player List</b> (" + (WorldReflect.IsInWorld() 
                        ? "<color=yellow>" + PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0.Count + "</color>"
                        : "-1") + ")\n",
                    RenderPlayerList()
                });
                if (TheText.text.Contains("Aicalas"))
                    TheText.text = TheText.text.Replace("Aicalas", "Penny");
                if (TheText.text.Contains("jettsd"))
                    TheText.text = TheText.text.Replace("jettsd", "Emy");
                if (TheText.text.Contains("~Silentt~"))
                    TheText.text = TheText.text.Replace("~Silentt~", "Elly");
                if (!string.IsNullOrWhiteSpace(Config.LocalSpoofedName.Value)) 
                    TheText.text = TheText.text.Replace(APIUser.CurrentUser.displayName, Config.LocalSpoofedName.Value);
            }
        }

        private string RenderPlayerList() {
            string text = "";
            if (RoomManager.field_Internal_Static_ApiWorld_0 != null) {
                int num = 0;
                if (PlayerManager.field_Private_Static_PlayerManager_0 != null && PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0 != null) {
                    try {
                        foreach (Player player in PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0) {
                            if (player != null && player.field_Private_VRCPlayerApi_0 != null && num != 23) {
                                text = string.Concat(new[] {
                                    text,
                                    player.field_Private_VRCPlayerApi_0.isMaster ? ">> " : "".PadRight(6),
                                    $"<color=#{ColorUtility.ToHtmlStringRGB(VRCPlayer.Method_Public_Static_Color_APIUser_0(player.prop_APIUser_0))}>{player.field_Private_APIUser_0.displayName}</color>",
                                    $" | <color=#{Config.MenuColorHEX.Value}>{player._vrcplayer.prop_Int16_0.ToString()}</color> ms",
                                    $" | {player.GetVRCPlayer().GetFramesColored()} fps",
                                    $" | {player.GetVRCPlayer().GetAviPerformance()}\n"
                                });
                                num++;
                            }
                        }
                    }
                    catch (Exception e) {
                        Con.Error(e);
                    }
                }
            }
            return text;
        }

        internal override void OnPrefSave() {
            if (Initialized && !Config.PLEnabled.Value) {
                Initialized = false;
                BackgroundObject.Destroy();
                TextObject.Destroy();
            } else if (!Initialized && Config.PLEnabled.Value) 
                MelonCoroutines.Start(Init());
        }
    }
}