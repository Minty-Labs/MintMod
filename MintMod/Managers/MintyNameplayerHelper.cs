using System;
using System.Reflection;
using TMPro;
using UnhollowerBaseLib.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace MintMod.Managers {
    public class MintyNameplateHelper : MonoBehaviour {
        //Nameplate object references
        public Graphic uiIconBackground;
        public RawImage uiUserImage;
        public GameObject uiUserImageContainer;
        public GameObject uiQuickStatsGO;
        public ImageThreeSlice uiNameBackground;
        public ImageThreeSlice uiQuickStatsBackground;
        public TextMeshProUGUI uiName;
        public CanvasGroup uiGroup;
        public GameObject localPlayerGO;
        public float fadeMaxRange;
        public float fadeMinRange;
        public bool closeRangeFade = false;
        public bool AlwaysShowQuickInfo = false;

        private PlayerNameplate nameplate = null;
        private VRC.Player player = null;
        private Color nameColour;
        private Color nameColour2;
        private bool setColour;
        private bool colourLerp;

        private Color bgColor;
        private Color bgColor2;
        private bool setBGColor;
        private bool colorBGLerp;
        private bool bgRainbow;

        private string fakeName;

        //Colour lerp stuff
        private bool lerpReverse = false;
        private float lerpValue = 0f;
        private float lerpTransitionTime = 3f;

        public MintyNameplateHelper(IntPtr ptr) : base(ptr) { }

        [HideFromIl2Cpp]
        public void ChangeTranistionValue(float time) { lerpTransitionTime = time; }

        [HideFromIl2Cpp]
        public void SetNameplate(PlayerNameplate nameplate) {
            this.nameplate = nameplate;
        }

        [HideFromIl2Cpp]
        public void SetNameColour(Color color) {
            this.nameColour = color;
            setColour = true;
        }

        [HideFromIl2Cpp]
        public void SetColourLerp(Color color1, Color color2) {
            this.nameColour = color1;
            this.nameColour2 = color2;

            setColour = false;
            colourLerp = true;
        }

        [HideFromIl2Cpp]
        public void SetBGColour(Color color) {
            this.bgColor = color;
            setBGColor = true;
            bgRainbow = false;
        }

        [HideFromIl2Cpp]
        public void SetBGColourLerp(Color color1, Color color2) {
            this.bgColor = color1;
            this.bgColor2 = color2;

            colorBGLerp = true;
            setBGColor = false;
            bgRainbow = false;
        }

        [HideFromIl2Cpp]
        public void SetBGRainbow() { bgRainbow = true; }

        [HideFromIl2Cpp]
        public VRC.Player GetPlayer() {
            if (nameplate != null)
                return nameplate.field_Private_VRCPlayer_0._player;
            else
                return null;
        }

        [HideFromIl2Cpp]
        public void ResetNameplate() {
            setColour = false;
            colourLerp = false;
            AlwaysShowQuickInfo = false;
            closeRangeFade = false;
            if (uiGroup != null)
                uiGroup.alpha = 1;
        }

        [HideFromIl2Cpp]
        public void OnRebuild() {
            if (nameplate != null) {
                if (!string.IsNullOrWhiteSpace(fakeName))
                    SetName(fakeName);

                if (setColour)
                    uiName.color = nameColour;

                if (setBGColor) {
                    uiIconBackground.color = bgColor;
                    uiNameBackground.color = bgColor;
                    uiQuickStatsBackground.color = bgColor;
                }
            }
        }

        [HideFromIl2Cpp]
        public void SetName(string name) {
            fakeName = name;
            if (nameplate != null)
                uiName.text = name;
        }

        public void Update() {
            if (colourLerp || colorBGLerp) {
                if (!lerpReverse)
                    lerpValue += Time.deltaTime;
                else
                    lerpValue -= Time.deltaTime;

                if (lerpValue >= lerpTransitionTime) {
                    lerpValue = lerpTransitionTime;
                    lerpReverse = true;
                }

                if (lerpValue <= 0) {
                    lerpValue = 0f;
                    lerpReverse = false;
                }
            }

            //Check if we should be doing the lerp
            if (colourLerp)
                uiName.color = Color.Lerp(nameColour, nameColour2, lerpValue);

            // Check for bg Lerp
            if (colorBGLerp) {
                if (bgRainbow) {
                    uiIconBackground.color = HSBColor.ToColor(new HSBColor(Mathf.PingPong(Time.time * 0.2f, 1f), 1f, 1f));
                    uiNameBackground.color = HSBColor.ToColor(new HSBColor(Mathf.PingPong(Time.time * 0.2f, 1f), 1f, 1f));
                } else {
                    uiIconBackground.color = Color.Lerp(bgColor, bgColor2, lerpValue);
                    uiNameBackground.color = Color.Lerp(bgColor, bgColor2, lerpValue);
                }
            }
        }

    }
}
