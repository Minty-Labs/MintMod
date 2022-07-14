using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;
using MintyLoader;

namespace MintMod.UserInterface.OldUI {
    internal class MenuContentBackdrop : MintSubMod {
        private static GameObject _backDropHiddenText, _image;
        private static bool _canRun;
        private Text _text;

        internal override void OnUserInterface() => MelonCoroutines.Start(Wait());

        private readonly string _backDropText = $"<color=#82ffbe>MintMod</color> <color=white>v{MintCore.ModBuildInfo.Version}</color>";

        private IEnumerator Wait() {
            while (GameObject.Find("UserInterface/MenuContent/Backdrop/Backdrop/EarlyAccessText") == null)
                yield return null;
            try {
                _backDropHiddenText = GameObject.Find("UserInterface/MenuContent/Backdrop/Backdrop/EarlyAccessText");
                _backDropHiddenText.GetComponent<RectTransform>().localPosition = new Vector2(795, 0);
                _backDropHiddenText.gameObject.SetActive(true);
                _backDropHiddenText.GetComponent<RectTransform>().gameObject.SetActive(true);
                _backDropHiddenText.GetComponent<Text>().supportRichText = true;
                _backDropHiddenText.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
                _backDropHiddenText.GetComponent<Text>().fontSize = 44;
                _text = _backDropHiddenText.GetComponent<Text>();
                //BackDropHiddenText.GetComponent<Text>().text = _backDropText;

                _image = GameObject.Find("UserInterface/MenuContent/Backdrop/Backdrop/Image");
                _image.GetComponent<Image>().color = Color.red;
                _canRun = true;
            }
            catch (Exception e) {
                Con.Error(e);
            }
        }

        internal override void OnUpdate() {
            if (_canRun)
                _text.text = _backDropText;
        }

        // internal static void UpdateForStreamerMode(bool o) {
        //     _canRun = !o;
        //     if (_image != null) _image.GetComponent<Image>().color = o ? Color.yellow : Color.red;
        //     _backDropHiddenText!.gameObject.SetActive(!o);
        // }
    }
}
