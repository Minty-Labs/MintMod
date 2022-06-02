using System.Collections;
using MelonLoader;
using MintMod.Reflections;
using MintyLoader;
using UnityEngine;
using ReMod.Core.VRChat;
using TMPro;
using UnityEngine.UI;
using VRC.UI.Core;

namespace MintMod.UserInterface.QuickMenu {
    public static class QmMediaPanel {
        private static Transform _mediaPanel;
        private static bool _loaded, _qmOpened;
        public static bool MediaReady;
        private static TextMeshProUGUI _reModTextElement, _mediaPanelText;
        private static string _reModHeaderText;
        private static RectTransform _mediaRectTransform;
        
        internal static IEnumerator CreateMediaDebugPanel() {
            while (UIManager.field_Private_Static_UIManager_0 == null) yield return null;
            while (Object.FindObjectOfType<VRC.UI.Elements.QuickMenu>() == null) yield return null;
            yield return new WaitForSeconds(0.25f);
            
            var t = GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMNotificationsArea/DebugInfoPanel/").transform;
            var f = QuickMenuEx.Instance.field_Public_Transform_0.Find("Window/QMParent/Menu_Dashboard");
            _mediaPanel = Object.Instantiate(t.Find("Panel"), t, false);
            _mediaPanel.gameObject.name = "MediaPanel";
            Transform h;
            try {
                h = f.Find("ScrollRect/Viewport/VerticalLayoutGroup/Header_MediaControls/LeftItemContainer/Text_Title");
            }
            catch {
                yield break; // Stop method if failed
            }

            var hasNoReMod = false;

            try { _reModTextElement = h.GetComponent<TextMeshProUGUI>();
            } catch { hasNoReMod = true; } // Stop method if no ReMod

            if (hasNoReMod) {
                _mediaPanel.gameObject.Destroy();
                yield break;
            }
            _reModHeaderText = _reModTextElement.text;
            
            var hasNoBtk = false;
            _mediaPanel.Find("Text_Ping").gameObject.Destroy();
            try {
                _mediaPanel.Find("BTKStatusElement").gameObject!.Destroy();
                _mediaPanel.Find("BTKClockElement").gameObject!.Destroy();
            }
            catch {
                Con.Debug("Did not find BTK Elements to destroy!");
                hasNoBtk = true;
            }
            
            _mediaPanelText = _mediaPanel.Find("Text_FPS").GetComponent<TextMeshProUGUI>();
            _mediaPanelText.text = "";
            _mediaRectTransform = _mediaPanel.GetComponent<RectTransform>();
            _mediaRectTransform.localPosition = new Vector3(-512, 85, 0);

            if (hasNoBtk) {
                // Setup HorizontalLayoutGroup - https://github.com/ddakebono/QMClock/blob/main/QMClock/QMClock.cs#L134
                var horizLayout = _mediaPanel.gameObject.GetOrAddComponent<HorizontalLayoutGroup>();
                horizLayout.padding.left = 20;
                horizLayout.padding.right = 20;
                horizLayout.spacing = 1.5f;
                
                // Expand box size - https://github.com/ddakebono/QMClock/blob/main/QMClock/QMClock.cs#L154
                var mediaPanelRect = _mediaPanel.GetComponent<RectTransform>();
                var adjust = mediaPanelRect.sizeDelta;
                adjust.x = 150f * _mediaPanel.transform.childCount;
                mediaPanelRect.sizeDelta = adjust;
                
                _mediaPanel.Find("Background").gameObject.GetComponent<Image>().enabled = false;
                _mediaPanel.Find("Background").gameObject.SetActive(false);
            }
            
            _loaded = true;
            
            _mediaPanel.gameObject.SetActive(Config.CopyReModMedia.Value); // Toggle
            yield return LoopTextChange(Config.RefreshAmount.Value);
        }

        private static IEnumerator LoopTextChange(float v) {
            while (_loaded) {
                yield return new WaitForSeconds(v);
                _reModHeaderText = _reModTextElement.text;
                _mediaPanelText.text = _reModHeaderText;
                //_mediaRectTransform.localPosition = new Vector3(-512, 85, 0);
            }
        }

        public static void OnOpen() {
            if (!_loaded) return;
            if (!Config.CopyReModMedia.Value && _mediaPanel != null) {
                _mediaPanel.gameObject.DestroyImmediate();
                return;
            }

            _qmOpened = true;
            //_mediaPanel.gameObject.SetActive(Config.CopyReModMedia.Value);
            _reModHeaderText = _reModTextElement.text;
            _mediaPanelText.text = _reModHeaderText;
            _mediaRectTransform.localPosition = new Vector3(-512, 85, 0);
        }

        public static void OnClose() => _qmOpened = false;

        public static void OnPrefSaved() {
            switch (_loaded) {
                case false when MediaReady && Config.CopyReModMedia.Value:
                    MelonCoroutines.Start(CreateMediaDebugPanel());
                    break;
                case true when !Config.CopyReModMedia.Value: {
                    if (_mediaPanel != null)
                        _mediaPanel.gameObject.SetActive(Config.CopyReModMedia.Value);
                    break;
                }
            }
        }
    }
}