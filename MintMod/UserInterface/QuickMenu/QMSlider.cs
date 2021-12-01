using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MintMod.Reflections;
using UnityEngine;
using ReMod.Core.UI.QuickMenu;
using TMPro;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace MintMod.UserInterface.QuickMenu {
    public class QMSlider {
        public GameObject _slider;

        public float Value {
            get => _slider.GetComponent<Slider>().value;
            set => _slider.GetComponent<Slider>().value = value;
        }

        public QMSlider(string parentPath, Action<float> onValueChanged, string name, string tooltip, string maxText, float maxVal, string minTextHeader,
            float minVal = 0f, float defaultValue = 1f) {
            var g = GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_AudioSettings/Content/Audio/");
            _slider = GameObject.Instantiate(g, GameObject.Find(parentPath).transform);
            _slider.transform.Find("VolumeSlider_World").gameObject.DestroyImmediate();
            _slider.transform.Find("VolumeSlider_Voices").gameObject.DestroyImmediate();
            _slider.transform.Find("VolumeSlider_Avatars").gameObject.DestroyImmediate();

            _slider.transform.Find("VolumeSlider_Master/Text_QM_H4").GetComponent<TextMeshProUGUI>().text = minTextHeader;
            _slider.transform.Find("VolumeSlider_Master/Text_QM_H4 (1)").GetComponent<TextMeshProUGUI>().text = maxText;

            _slider.transform.Find("VolumeSlider_Master").GetComponent<VRC.UI.Elements.Tooltips.UiTooltip>().field_Public_String_0 = tooltip;
            _slider.transform.Find("VolumeSlider_Master").GetComponent<VRC.UI.Elements.Tooltips.UiTooltip>().field_Public_String_1 = tooltip;

            var s = _slider.transform.Find("VolumeSlider_Master/Slider").gameObject;

            _slider.gameObject.name = $"SliderContainer_{name}";
            s.name = $"MintSlider_{name}";
            s.GetComponent<Slider>().minValue = minVal;
            s.GetComponent<Slider>().maxValue = maxVal;
            s.GetComponent<Slider>().onValueChanged = new Slider.SliderEvent();
            s.GetComponent<Slider>().value = defaultValue;
            s.GetComponent<Slider>().onValueChanged.AddListener(UnhollowerRuntimeLib.DelegateSupport.ConvertDelegate<UnityEngine.Events.UnityAction<float>>(onValueChanged));
            _slider.GetComponent<RectTransform>().sizeDelta = new(850, 150);
            _slider.GetComponent<RectTransform>().anchoredPosition = new(0, -525);
        }

        public QMSlider(string parentPath, Action<float> onChangedValue, string name, string tooltip, string maxText,
            string minTextHeader, float minVal, float maxVal, float defaultValue, Action buttonAction) {
            var g = GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_AudioSettings/Content/Mic/");
            _slider = GameObject.Instantiate(g, GameObject.Find(parentPath).transform);
            
            _slider.transform.Find("CurrentMic/MicText/Text_QM_H4").GetComponent<TextMeshProUGUI>().text = "Action";
            _slider.transform.Find("CurrentMic/MicText/Text_QM_H4 (1)").GetComponent<TextMeshProUGUI>().text = "Reset to Default Values and Slider Settings";
            
            _slider.transform.Find("InputLevel/Sliders/MicSensitivitySlider/Text_QM_H4").GetComponent<TextMeshProUGUI>().text = minTextHeader;
            _slider.transform.Find("InputLevel/Sliders/MicSensitivitySlider/Text_QM_H4 (1)").GetComponent<TextMeshProUGUI>().text = maxText;

            _slider.transform.Find("InputLevel/Sliders/MicLevelSlider").gameObject.Destroy();

            var s = _slider.transform.Find("InputLevel/Sliders/MicSensitivitySlider/Slider");
            s.name = $"MintSlider_{name}";
            s.GetComponent<Slider>().minValue = minVal;
            s.GetComponent<Slider>().maxValue = maxVal;
            s.GetComponent<Slider>().onValueChanged = new Slider.SliderEvent();
            s.GetComponent<Slider>().value = defaultValue;
            s.GetComponent<Slider>().onValueChanged.AddListener(UnhollowerRuntimeLib.DelegateSupport.ConvertDelegate<UnityEngine.Events.UnityAction<float>>(onChangedValue));
            s.GetComponent<VRC.UI.Elements.Tooltips.UiTooltip>().field_Public_String_0 = tooltip;
            s.GetComponent<VRC.UI.Elements.Tooltips.UiTooltip>().field_Public_String_1 = tooltip;

            _slider.transform.Find("CurrentMic/ChangeMic/Text_MM_H3").GetComponent<TextMeshProUGUI>().text = "Reset";
            _slider.transform.Find("CurrentMic/ChangeMic").GetComponent<VRC.UI.Elements.Tooltips.UiTooltip>().field_Public_String_0 = "Reset to Default Values and Slider Settings";
            _slider.transform.Find("CurrentMic/ChangeMic").GetComponent<VRC.UI.Elements.Tooltips.UiTooltip>().field_Public_String_1 = "Reset to Default Values and Slider Settings";
            var b = _slider.transform.Find("CurrentMic/ChangeMic").GetComponent<Button>();
            b.onClick = new Button.ButtonClickedEvent();
            b.onClick.AddListener(buttonAction);
            
            _slider.GetComponent<RectTransform>().sizeDelta = new(850, 150);
            _slider.GetComponent<RectTransform>().anchoredPosition = new(0, -525);
            _slider.gameObject.name = $"SliderContainerWithButton_{name}";
        }
    }
}