using System;
using MintMod.Libraries;
using MintyLoader;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace MintMod.UserInterface.OldUI; 

internal class Keyboard : MintSubMod {
    public override string Name => "Keyboard";
    public override string Description => "Adds a copy and paste button to the VRChat keyboard.";
    
    private GameObject _keybardPasteButton, _keybardCopyButton;

    internal override void OnUserInterface() {
        if (ModCompatibility.KeyboardPaste) return;
        
        // Paste button
        if (!ModCompatibility.ReMod) {
            try {
                _keybardPasteButton = Object.Instantiate(GameObject.Find("/UserInterface/MenuContent/Popups/InputPopup/ButtonLeft"),
                    VRCUiPopupManager.field_Private_Static_VRCUiPopupManager_0.field_Public_VRCUiPopupInput_0.transform);
                _keybardPasteButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(335f, -275f);
                _keybardPasteButton.GetComponentInChildren<Text>().text = "Paste";
                _keybardPasteButton.name = "Mint_KeyboardPasteButton";
                _keybardPasteButton.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
                _keybardPasteButton.GetComponent<Button>().m_Interactable = true;
                _keybardPasteButton.GetComponent<Button>().onClick.AddListener(new Action(() => {
                    try {
                        if (GUIUtility.systemCopyBuffer.Length < 256)
                            GameObject.Find("UserInterface/MenuContent/Popups/InputPopup/InputField").GetComponent<InputField>().text = GUIUtility.systemCopyBuffer;
                        else
                            Con.Warn("You cannot paste something more than 256 characters long in the keyboard.");
                    }
                    catch (Exception e) {
                        Con.Error($"An error has occurred:\n{e}");
                    }
                }));
            }
            catch (Exception e) {
                Con.Error($"Paste:\n{e}");
            }
        }
        
        // Copy button
        try {
            _keybardCopyButton = Object.Instantiate(GameObject.Find("/UserInterface/MenuContent/Popups/InputPopup/ButtonLeft"),
                VRCUiPopupManager.field_Private_Static_VRCUiPopupManager_0.field_Public_VRCUiPopupInput_0.transform);
            _keybardCopyButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(-335f, -275f);
            _keybardCopyButton.GetComponentInChildren<Text>().text = "Copy";
            _keybardCopyButton.name = "Mint_KeyboardCopyButton";
            _keybardCopyButton.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
            _keybardCopyButton.GetComponent<Button>().m_Interactable = true;
            _keybardCopyButton.GetComponent<Button>().onClick.AddListener(new Action(() => {
                try {
                    GUIUtility.systemCopyBuffer = GameObject.Find("UserInterface/MenuContent/Popups/InputPopup/InputField").GetComponent<InputField>().text;
                }
                catch (Exception e) {
                    Con.Error($"An error has occurred:\n{e}");
                }
            }));
        }
        catch (Exception e) {
            Con.Error($"Copy:\n{e}");
        }
    }
}