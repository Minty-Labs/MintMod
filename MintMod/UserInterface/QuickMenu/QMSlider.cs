/*
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
        public GameObject _slider, internalSlider1, internalSlider2, internalSlider3;

        public float Value {
            get => _slider.GetComponentInChildren<Slider>().value;
            set => _slider.GetComponentInChildren<Slider>().value = value;
        }
        
        public float internalSlider1Value {
            get => internalSlider1.GetComponent<Slider>().value;
            set => internalSlider1.GetComponent<Slider>().value = value;
        }
        
        public float internalSlider2Value {
            get => internalSlider2.GetComponent<Slider>().value;
            set => internalSlider2.GetComponent<Slider>().value = value;
        }
        
        public float internalSlider3Value {
            get => internalSlider3.GetComponent<Slider>().value;
            set => internalSlider3.GetComponent<Slider>().value = value;
        }
        
        public bool Enabled {
            get => _slider.active;
            set => _slider.SetActive(value);
        }

        public QMSlider(Transform parentPath, Action<float> onValueChanged, string name, string tooltip, string maxText, float maxVal, string minTextHeader,
            float minVal = 0f, float defaultValue = 1f) {
            var g = GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_AudioSettings/Content/Audio/");
            _slider = GameObject.Instantiate(g, parentPath);
            _slider.transform.Find("VolumeSlider_World").gameObject.DestroyImmediate();
            _slider.transform.Find("VolumeSlider_Voices").gameObject.DestroyImmediate();
            _slider.transform.Find("VolumeSlider_Avatars").gameObject.DestroyImmediate();

            _slider.transform.Find("VolumeSlider_Master/Text_QM_H4").GetComponent<TextMeshProUGUI>().text = minTextHeader;
            _slider.transform.Find("VolumeSlider_Master/Text_QM_H4 (1)").GetComponent<TextMeshProUGUI>().text = maxText;

            _slider.transform.Find("VolumeSlider_Master").GetComponent<VRC.UI.Elements.Tooltips.UiTooltip>().field_Public_String_0 = tooltip;
            _slider.transform.Find("VolumeSlider_Master").GetComponent<VRC.UI.Elements.Tooltips.UiTooltip>().field_Public_String_1 = tooltip;

            var s = _slider.transform.Find("VolumeSlider_Master/Slider").gameObject;

            /*
            var t = new GameObject($"SliderContainer_{name}");
            var fitter = t.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.useGUILayout = true;
            
            t.transform.SetParent(_slider.transform);
            //s.transform.SetParent(t.transform);
            *

            _slider.gameObject.name = $"Slider_{name}";
            s.name = $"MintSlider_{name}";
            s.GetComponent<Slider>().minValue = minVal;
            s.GetComponent<Slider>().maxValue = maxVal;
            s.GetComponent<Slider>().onValueChanged = new Slider.SliderEvent();
            s.GetComponent<Slider>().value = defaultValue;
            s.GetComponent<Slider>().onValueChanged.AddListener(UnhollowerRuntimeLib.DelegateSupport.ConvertDelegate<UnityEngine.Events.UnityAction<float>>(onValueChanged));
            _slider.GetComponent<RectTransform>().sizeDelta = new(850, 150);
            _slider.GetComponent<RectTransform>().anchoredPosition = new(0, -525);
        }
        
        public QMSlider(Transform parentPath,
            Action<float> onValueChanged,  string name,  string tooltip,  float maxVal,    float minVal,    float defaultValue,
            Action<float> onValue2Changed, string name2, string tooltip2, float maxValue2, float minValue2, float defaultValue2) {
            var g = GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_AudioSettings/Content/Audio/");
            _slider = GameObject.Instantiate(g, parentPath);
            _slider.transform.Find("VolumeSlider_Avatars").gameObject.DestroyImmediate();
            _slider.transform.Find("VolumeSlider_Master").gameObject.DestroyImmediate();

            var g1 = _slider.transform.Find("VolumeSlider_World").gameObject;
            var g2 = _slider.transform.Find("VolumeSlider_Voices").gameObject;
            
            _slider.transform.Find("VolumeSlider_World/Text_QM_H4").GetComponent<TextMeshProUGUI>().text = name;
            _slider.transform.Find("VolumeSlider_Voices/Text_QM_H4").GetComponent<TextMeshProUGUI>().text = name2;

            g1.GetComponent<VRC.UI.Elements.Tooltips.UiTooltip>().field_Public_String_0 = tooltip;
            g1.GetComponent<VRC.UI.Elements.Tooltips.UiTooltip>().field_Public_String_1 = tooltip;
            g2.GetComponent<VRC.UI.Elements.Tooltips.UiTooltip>().field_Public_String_0 = tooltip2;
            g2.GetComponent<VRC.UI.Elements.Tooltips.UiTooltip>().field_Public_String_1 = tooltip2;

            internalSlider1 = _slider.transform.Find("VolumeSlider_World/Slider").gameObject;
            internalSlider2 = _slider.transform.Find("VolumeSlider_Voices/Slider").gameObject;

            _slider.gameObject.name = $"SliderContainer_{name}-{name2}";
            internalSlider1.name = $"MintSlider_{name}";
            internalSlider1.GetComponent<Slider>().minValue = minVal;
            internalSlider1.GetComponent<Slider>().maxValue = maxVal;
            internalSlider1.GetComponent<Slider>().onValueChanged = new Slider.SliderEvent();
            internalSlider1.GetComponent<Slider>().value = defaultValue;
            internalSlider1.GetComponent<Slider>().onValueChanged.AddListener(UnhollowerRuntimeLib.DelegateSupport.ConvertDelegate<UnityEngine.Events.UnityAction<float>>(onValueChanged));
            
            internalSlider2.name = $"MintSlider_{name2}";
            internalSlider2.GetComponent<Slider>().minValue = minValue2;
            internalSlider2.GetComponent<Slider>().maxValue = maxValue2;
            internalSlider2.GetComponent<Slider>().onValueChanged = new Slider.SliderEvent();
            internalSlider2.GetComponent<Slider>().value = defaultValue2;
            internalSlider2.GetComponent<Slider>().onValueChanged.AddListener(UnhollowerRuntimeLib.DelegateSupport.ConvertDelegate<UnityEngine.Events.UnityAction<float>>(onValue2Changed));
            
            _slider.GetComponent<RectTransform>().sizeDelta = new(850, 150);
            _slider.GetComponent<RectTransform>().anchoredPosition = new(0, -525);
        }
        
        public QMSlider(Transform parentPath,
            Action<float> onValueChanged,  string name,  string tooltip,  float maxVal,    float minVal,    float defaultValue, 
            Action<float> onValue2Changed, string name2, string tooltip2, float maxValue2, float minValue2, float defaultValue2,
            Action<float> onValue3Changed, string name3, string tooltip3, float maxValue3, float minValue3, float defaultValue3) {
            var g = GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_AudioSettings/Content/Audio/");
            _slider = GameObject.Instantiate(g, parentPath);
            _slider.transform.Find("VolumeSlider_Master").gameObject.DestroyImmediate();

            var g1 = _slider.transform.Find("VolumeSlider_World").gameObject;
            var g2 = _slider.transform.Find("VolumeSlider_Voices").gameObject;
            var g3 = _slider.transform.Find("VolumeSlider_Avatars").gameObject;
            
            _slider.transform.Find("VolumeSlider_World/Text_QM_H4").GetComponent<TextMeshProUGUI>().text = name;
            _slider.transform.Find("VolumeSlider_Voices/Text_QM_H4").GetComponent<TextMeshProUGUI>().text = name2;
            _slider.transform.Find("VolumeSlider_Avatars/Text_QM_H4").GetComponent<TextMeshProUGUI>().text = name3;

            g1.GetComponent<VRC.UI.Elements.Tooltips.UiTooltip>().field_Public_String_0 = tooltip;
            g1.GetComponent<VRC.UI.Elements.Tooltips.UiTooltip>().field_Public_String_1 = tooltip;
            g2.GetComponent<VRC.UI.Elements.Tooltips.UiTooltip>().field_Public_String_0 = tooltip2;
            g2.GetComponent<VRC.UI.Elements.Tooltips.UiTooltip>().field_Public_String_1 = tooltip2;
            g3.GetComponent<VRC.UI.Elements.Tooltips.UiTooltip>().field_Public_String_0 = tooltip3;
            g3.GetComponent<VRC.UI.Elements.Tooltips.UiTooltip>().field_Public_String_1 = tooltip3;

            internalSlider1 = _slider.transform.Find("VolumeSlider_World/Slider").gameObject;
            internalSlider2 = _slider.transform.Find("VolumeSlider_Voices/Slider").gameObject;
            internalSlider3 = _slider.transform.Find("VolumeSlider_Avatars/Slider").gameObject;

            _slider.gameObject.name = $"SliderContainer_{name}-{name2}-{name3}";
            internalSlider1.name = $"MintSlider_{name}";
            internalSlider1.GetComponent<Slider>().minValue = minVal;
            internalSlider1.GetComponent<Slider>().maxValue = maxVal;
            internalSlider1.GetComponent<Slider>().onValueChanged = new Slider.SliderEvent();
            internalSlider1.GetComponent<Slider>().value = defaultValue;
            internalSlider1.GetComponent<Slider>().onValueChanged.AddListener(UnhollowerRuntimeLib.DelegateSupport.ConvertDelegate<UnityEngine.Events.UnityAction<float>>(onValueChanged));
            
            internalSlider2.name = $"MintSlider_{name2}";
            internalSlider2.GetComponent<Slider>().minValue = minValue2;
            internalSlider2.GetComponent<Slider>().maxValue = maxValue2;
            internalSlider2.GetComponent<Slider>().onValueChanged = new Slider.SliderEvent();
            internalSlider2.GetComponent<Slider>().value = defaultValue2;
            internalSlider2.GetComponent<Slider>().onValueChanged.AddListener(UnhollowerRuntimeLib.DelegateSupport.ConvertDelegate<UnityEngine.Events.UnityAction<float>>(onValue2Changed));
            
            internalSlider3.name = $"MintSlider_{name3}";
            internalSlider3.GetComponent<Slider>().minValue = minValue3;
            internalSlider3.GetComponent<Slider>().maxValue = maxValue3;
            internalSlider3.GetComponent<Slider>().onValueChanged = new Slider.SliderEvent();
            internalSlider3.GetComponent<Slider>().value = defaultValue3;
            internalSlider3.GetComponent<Slider>().onValueChanged.AddListener(UnhollowerRuntimeLib.DelegateSupport.ConvertDelegate<UnityEngine.Events.UnityAction<float>>(onValue3Changed));
            
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
*/