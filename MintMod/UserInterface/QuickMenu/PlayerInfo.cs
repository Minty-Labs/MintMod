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
using String = Il2CppSystem.String;

namespace MintMod.UserInterface.QuickMenu {
    internal class PlayerInfo : MintSubMod {
        public override string Name => "PlayerInfoHUD";
        public override string Description => "Shows Player Info on selected user";

        private static Transform _quickMenu, _wing;
        private static GameObject _backgroundObject, _foolish, _textObject;
        public static Image BackgroundImage;
        private static Image _bgFool;
        private static Text _theText;
        private bool _initialized;
        private DateTime _timer;

        internal override void OnUserInterface() => MelonCoroutines.Start(Init());

        private IEnumerator Init() {
            if (!Config.PLEnabled.Value) yield break;
            while (MintyResources.BG_HUD == null) yield return null;
            while (UnityEngine.Object.FindObjectOfType<VRC.UI.Elements.QuickMenu>() == null) yield return null;
            
            _quickMenu = UnityEngine.Object.FindObjectOfType<VRC.UI.Elements.QuickMenu>().gameObject.transform;
            _wing = _quickMenu.Find($"Container/Window/Wing_{(Config.Location.Value == 0 ? "Left" : "Right")}/");
            
            var c = new Color(Config.BackgroundColor.Value.r, Config.BackgroundColor.Value.g, Config.BackgroundColor.Value.b, Config.BackgroundColor.Value.a);
            
            if (_wing.Find("MintUiHud Panel") == null) {
                _backgroundObject = new GameObject("MintUiHud Panel");
                _backgroundObject.transform.SetParent(_wing, false);
                _backgroundObject.AddComponent<CanvasRenderer>();
                BackgroundImage = _backgroundObject.AddComponent<Image>();
            
                _backgroundObject.GetComponent<RectTransform>().sizeDelta = new Vector2(1200f, 1420);
                SetLocation();
                BackgroundImage.sprite = MintyResources.BG_HUD;
                //var c = new Color(Config.BackgroundColor.Value.r, Config.BackgroundColor.Value.g, Config.BackgroundColor.Value.b, Config.BackgroundColor.Value.a / 255);
                //Con.Debug($"{Config.BackgroundColor.Value.r} {Config.BackgroundColor.Value.g} {Config.BackgroundColor.Value.b} {Config.BackgroundColor.Value.a} -- {Config.BackgroundColor.Value.a / 255} -- {Config.BackgroundColor.Value.a * 255}");
                SetBackgroundColor(c);
                if (MintCore.Fool) {
                    _foolish = new GameObject("W A L M A R T  C L I E N T");
                    _foolish.transform.SetParent(_wing, false);
                    _foolish.AddComponent<CanvasRenderer>();
                    _bgFool = _foolish.AddComponent<Image>();
            
                    _foolish.GetComponent<RectTransform>().sizeDelta = new Vector2(1200f, 1420);
                    SetLocation();
                    _bgFool.sprite = MintyResources.PlayerListBGWalmart;
                }
            }
            if (_backgroundObject.transform.Find("Player List"))
                yield break;
            _textObject = new GameObject("Player List");
            _textObject.transform.SetParent(_backgroundObject.transform, false);
            _textObject.AddComponent<CanvasRenderer>();
            _theText = _textObject.AddComponent<Text>();
            SetSizeTextObj();
            _theText.font = MintyResources.BalooFont != null ? MintyResources.BalooFont : UnityEngine.Resources.GetBuiltinResource<Font>("Arial.ttf");
            UpdateTextSize(Config.TextSize.Value);
            _theText.text = "";
            _theText.alignment = (TextAnchor)TextAlignment.Left;
            _initialized = true;
            
            MoveTheText();
            SetBackgroundColor(c);
            yield return HUDLoop();
        }

        internal override void OnLevelWasLoaded(int buildindex, string sceneName) {
            if (buildindex == -1) 
                _timer = DateTime.Now;
        }

        private IEnumerator HUDLoop() {
            while (_initialized) {
                //BackgroundObject.GetComponent<RectTransform>().localPosition = new Vector3(-1000, -500, -0.5f);
                yield return new WaitForSeconds(1);
                var t = DateTime.Now - _timer;
                var g = DateTime.Now - MintCore.GameStartTimer;
                _theText.text = string.Concat(new[] {
                    "\n\n",
                    "<b>Player List</b> (" + (WorldReflect.IsInWorld() 
                        ? $"<color=yellow>{PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0.Count}</color>"
                        : "-1") + ")",
                    Config.haveRoomTimer.Value ?  $" - {(Config.hideLabels.Value ? "" : "Room Timer: ")}<color=yellow>{t.ToString(@"hh\.mm\.ss")}</color>" : "",
                    Config.haveGameTimer.Value ?  $" - {(Config.hideLabels.Value ? "" : "Game Timer: ")}<color=yellow>{g.ToString(@"hh\.mm\.ss")}</color>" : "",
                    Config.haveSystemTime.Value ? $" - {(Config.hideLabels.Value ? "" : "System Time: ")}<color=yellow>{(Config.system24Hour.Value ? DateTime.Now.ToString("HH:mm") : DateTime.Now.ToString("h:mm tt"))}</color>" : "",
                    "\n",
                    RenderPlayerList()
                });
                if (_theText.text.Contains("․penny"))
                    _theText.text = _theText.text.Replace("․penny", "<color=#587EE2>Penny</color>");
                if (_theText.text.Contains("Rin_Isnt_Real"))
                    _theText.text = _theText.text.Replace("Rin_Isnt_Real", "<color=#ff9efd>Rin</color>");
                if (_theText.text.Contains("jettsd"))
                    _theText.text = _theText.text.Replace("jettsd", "Emy");
                if (_theText.text.Contains("~Silentt~"))
                    _theText.text = _theText.text.Replace("~Silentt~", "<color=#D2A0FF>Elly</color>");
                if (_theText.text.Contains("~Elly~"))
                    _theText.text = _theText.text.Replace("~Elly~", "<color=#D2A0FF>Elly</color>");
                if (_theText.text.Contains("MintyLily"))
                    _theText.text = _theText.text.Replace("MintyLily", "<color=#9fffe3>Lily</color>");
                if (!string.IsNullOrWhiteSpace(Config.LocalSpoofedName.Value)) 
                    _theText.text = _theText.text.Replace(APIUser.CurrentUser.displayName, Config.LocalSpoofedName.Value);
            }
        }

        private string RenderPlayerList() {
            var text = "";
            if (RoomManager.field_Internal_Static_ApiWorld_0 == null) return text;
            var num = 0;
            if (PlayerManager.field_Private_Static_PlayerManager_0 == null || PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0 == null) return text;
            try {
                foreach (var player in PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0) {
                    if (player == null || player.field_Private_VRCPlayerApi_0 == null || num == (Config.uncapListCount.Value ? 128 : 25)) continue;
                    text = string.Concat(new[] {
                        text,
                        player.field_Private_VRCPlayerApi_0.isMaster ? ">> " : "".PadRight(6),
                        $"<color=#{ColorUtility.ToHtmlStringRGB(VRCPlayer.Method_Public_Static_Color_APIUser_0(player.prop_APIUser_0))}>{player.field_Private_APIUser_0.displayName}</color>",
                        Config.showPlayerPing.Value ? $" | <color=#{Config.MenuColorHEX.Value}>{player._vrcplayer.prop_Int16_0.ToString()}</color> ms" : "",
                        Config.showPlayerFrames.Value ? $" | {player.GetVRCPlayer().GetFramesColored()} fps" : "",
                        Config.showPlayerPlatform.Value ? $" | {player.GetVRCPlayer().Platform()}" : "",
                        Config.showPlayerAviPerf.Value ? 
                            $" | {(String.IsNullOrWhiteSpace(player.GetVRCPlayer().GetAviPerformance()) ? "<i>Blocked</i>" : player.GetVRCPlayer().GetAviPerformance())}\n" : "\n"
                    });
                    num++;
                }
            }
            catch (Exception e) {
                Con.Debug($"[DEBUG ERROR] {e}");
            }
            return text;
        }

        private static void SetLocation() {
            if (_backgroundObject == null) return;
            var t = _quickMenu.Find($"Container/Window/Wing_{(Config.Location.Value == 0 ? "Left" : "Right")}/");
            _backgroundObject.transform.SetParent(t, false);
            _backgroundObject.GetComponent<RectTransform>().localPosition = 
                new Vector3(Config.Location.Value == 0 ? -1000 : 1000, -500, -0.5f);
            if (_foolish == null || !MintCore.Fool) return;
            var t2 = _quickMenu.Find($"Container/Window/Wing_{(Config.Location.Value == 0 ? "Left" : "Right")}/");
            _foolish.transform.SetParent(t2, false);
            _foolish.GetComponent<RectTransform>().localPosition = 
                new Vector3(Config.Location.Value == 0 ? -1000 : 1000, -500, -0.5f);
        }

        private static void SetSizeTextObj() {
            if (_textObject != null)
                _textObject.GetComponent<RectTransform>().sizeDelta = new Vector2(1200, Config.uncapListCount.Value ? 4410 : 1480);
        }
        
        public static void SetBackgroundColor(Color c) {
            if (BackgroundImage != null)
                BackgroundImage.color = c;
        }
        
        public static void SetBackgroundOpacity(float o) {
            if (BackgroundImage != null)
                BackgroundImage.color = new Color(Config.BackgroundColor.Value.r, Config.BackgroundColor.Value.g, Config.BackgroundColor.Value.b, o);
        }

        public static void UpdateTextSize(int size) {
            if (_theText != null)
                _theText.fontSize = size;
        }

        public static int GetTextSize() => _theText != null ? _theText.fontSize : 40;

        public static void MoveTheText() {
            if (_theText != null)
                _theText.GetComponent<RectTransform>().anchoredPosition = new Vector2(15, Config.uncapListCount.Value ? -150 : 145);
        }

        internal override void OnPrefSave() {
            if (_initialized && !Config.PLEnabled.Value) {
                var obj = MelonCoroutines.Start(HUDLoop());
                _initialized = false;
                _backgroundObject.Destroy();
                if (MintCore.Fool)
                    _foolish.Destroy();
                _textObject.Destroy();
                _wing.Find("MintUiHud Panel").gameObject.Destroy();
                try { MelonCoroutines.Stop(obj); } catch (Exception ee) { Con.Debug($"[ERROR] {ee}"); }
            } else if (!_initialized && Config.PLEnabled.Value) 
                MelonCoroutines.Start(Init());

            if (!_initialized || !Config.PLEnabled.Value) return;
            SetLocation();
            SetSizeTextObj();
            SetBackgroundColor(Config.BackgroundColor.Value);
        }
    }
}