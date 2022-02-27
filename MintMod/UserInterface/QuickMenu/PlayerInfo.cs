using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Runtime.CompilerServices;
using MelonLoader;
using MintMod;
using MintMod.Reflections;
using MintMod.Resources;
using MintyLoader;
using ReMod.Core.VRChat;
using UnityEngine.Playables;
using VRC;
using VRC.Core;
using VRC.DataModel;
using VRCSDK2;
using VRCSDK2.Validation.Performance;

namespace MintMod.UserInterface.QuickMenu {
    internal class PlayerInfo : MintSubMod {
        public override string Name => "PlayerInfoHUD";
        public override string Description => "Shows Player Info on selected user";

        private static Transform QuickMenu, Wing;
        private static GameObject BackgroundObject, TextObject;
        public static Image BackgroundImage;
        private static Text TheText;
        private bool Initialized;

        internal override void OnStart() => MelonCoroutines.Start(Init());

        private IEnumerator Init() {
            if (!Config.PLEnabled.Value) yield break;
            while (MintyResources.BG_HUD == null) yield return null;
            while (UnityEngine.Object.FindObjectOfType<VRC.UI.Elements.QuickMenu>() == null) yield return null;
            QuickMenu = UnityEngine.Object.FindObjectOfType<VRC.UI.Elements.QuickMenu>().gameObject.transform;
            Wing = QuickMenu.Find($"Container/Window/Wing_{(Config.Location.Value == 0 ? "Left" : "Right")}/");
            
            if (Wing.Find("MintUiHud Panel") == null) {
                BackgroundObject = new GameObject("MintUiHud Panel");
                BackgroundObject.transform.SetParent(Wing, false);
                BackgroundObject.AddComponent<CanvasRenderer>();
                BackgroundImage = BackgroundObject.AddComponent<Image>();
            
                BackgroundObject.GetComponent<RectTransform>().sizeDelta = new Vector2(1200f, 1420);
                SetLocation();
                BackgroundImage.sprite = MintyResources.BG_HUD;
                SetBackgroundColor(Config.BackgroundColor.Value);
            }
            
            TextObject = new GameObject("Player List");
            TextObject.transform.SetParent(BackgroundObject.transform, false);
            TextObject.AddComponent<CanvasRenderer>();
            TheText = TextObject.AddComponent<Text>();
            SetSizeTextObj();
            if (MintyResources.BalooFont != null)
                TheText.font = MintyResources.BalooFont;
            else
                TheText.font = UnityEngine.Resources.GetBuiltinResource<Font>("Arial.ttf");
            UpdateTextSize(40);
            TheText.text = "";
            TheText.alignment = (TextAnchor)TextAlignment.Left;
            Initialized = true;
            
            MelonCoroutines.Start(HUDLoop());
            
            MoveTheText();
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
                    TheText.text = TheText.text.Replace("Aicalas", "<color=#587EE2>Penny</color>");
                if (TheText.text.Contains("jettsd"))
                    TheText.text = TheText.text.Replace("jettsd", "Emy");
                if (TheText.text.Contains("~Silentt~"))
                    TheText.text = TheText.text.Replace("~Silentt~", "<color=#D2A0FF>Elly</color>");
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
                            if (player != null && player.field_Private_VRCPlayerApi_0 != null && num != (Config.uncapListCount.Value ? 128 : 25)) {
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
                        Con.Debug($"[DEBUG ERROR] {e}");
                    }
                }
            }
            return text;
        }

        public static void SetLocation() {
            if (BackgroundObject != null) {
                var t = QuickMenu.Find($"Container/Window/Wing_{(Config.Location.Value == 0 ? "Left" : "Right")}/");
                BackgroundObject.transform.SetParent(t, false);
                BackgroundObject.GetComponent<RectTransform>().localPosition = 
                    new Vector3(Config.Location.Value == 0 ? -1000 : 1000, -500, -0.5f);
            }
        }

        private void SetSizeTextObj() {
            if (TextObject != null)
                TextObject.GetComponent<RectTransform>().sizeDelta = new Vector2(1200, Config.uncapListCount.Value ? 4410 : 1410);
        }

        public static void SetBackgroundColor(Color32 c) {
            if (BackgroundImage != null)
                BackgroundImage.color = c;
        }

        public static void UpdateTextSize(int size) {
            if (TheText != null)
                TheText.fontSize = size;
        }

        public static int GetTextSize() {
            if (TheText != null)
                return TheText.fontSize;
            return 40;
        }

        public static void MoveTheText() {
            if (TheText != null)
                TheText.GetComponent<RectTransform>().anchoredPosition = new Vector2(130, Config.uncapListCount.Value ? -300 : 125);
        }

        internal override void OnPrefSave() {
            if (Initialized && !Config.PLEnabled.Value) {
                var obj = MelonCoroutines.Start(HUDLoop());
                Initialized = false;
                BackgroundObject.Destroy();
                TextObject.Destroy();
                Wing.Find("MintUiHud Panel").gameObject.Destroy();
                //RightWing.Find("MintUiHud Panel")?.Destroy();
                try { MelonCoroutines.Stop(obj); } catch (Exception ee) { Con.Debug($"{ee}"); }
            } else if (!Initialized && Config.PLEnabled.Value) 
                MelonCoroutines.Start(Init());

            if (Initialized && Config.PLEnabled.Value) {
                SetLocation();
                SetSizeTextObj();
            }
        }
    }
}