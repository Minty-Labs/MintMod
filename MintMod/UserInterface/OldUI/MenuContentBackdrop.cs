using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;

namespace MintMod.UserInterface.OldUI {
    class MenuContentBackdrop : MintSubMod {
        internal override void OnUserInterface() => MelonCoroutines.Start(Wait());

        private readonly string _backDropText = $"<color=#82ffbe>MintMod</color> <color=white>v{MintCore.ModBuildInfo.Version}</color>";

        IEnumerator Wait() {
            while (GameObject.Find("UserInterface/MenuContent/Backdrop/Backdrop/EarlyAccessText") == null)
                yield return null;
            try {
                GameObject BackDropHiddenText =
                    GameObject.Find("UserInterface/MenuContent/Backdrop/Backdrop/EarlyAccessText");
                BackDropHiddenText.GetComponent<RectTransform>().localPosition = new Vector2(795, 0);
                BackDropHiddenText.gameObject.SetActive(true);
                BackDropHiddenText.GetComponent<RectTransform>().gameObject.SetActive(true);
                BackDropHiddenText.GetComponent<Text>().supportRichText = true;
                BackDropHiddenText.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
                BackDropHiddenText.GetComponent<Text>().fontSize = 44;
                BackDropHiddenText.GetComponent<Text>().text = _backDropText;

                BackDropHiddenText.GetComponentInChildren<RectTransform>().localPosition = new Vector2(795, 0);
                BackDropHiddenText.GetComponentInChildren<RectTransform>().gameObject.SetActive(true);
                BackDropHiddenText.GetComponentInChildren<Text>().supportRichText = true;
                BackDropHiddenText.GetComponentInChildren<Text>().alignment = TextAnchor.MiddleLeft;
                BackDropHiddenText.GetComponentInChildren<Text>().fontSize = 44;
                BackDropHiddenText.GetComponentInChildren<Text>().text = _backDropText;

                GameObject Image = GameObject.Find("UserInterface/MenuContent/Backdrop/Backdrop/Image");
                Image.GetComponent<Image>().color = Color.red;
                Image.GetComponentInChildren<Image>().color = Color.red;
            }
            catch (Exception e) {
                MelonLogger.Error(e);
            }
        }
    }
}
