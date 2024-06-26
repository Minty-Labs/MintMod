﻿using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using MintMod.Libraries;
using MintMod.Reflections;
using static MintMod.Managers.Colors;
using System.Diagnostics;
using MintMod.Functions;
using MintMod.Functions.Authentication;
using MintMod.Managers;
using MintMod.UserInterface.AvatarFavs;
using MintMod.UserInterface.QuickMenu;
using UnityEngine.Events;
using MintyLoader;

namespace MintMod.UserInterface.OldUI {
    internal class SettingsMenu : MintSubMod {
        public override string Name => "Settings Menu";
        public override string Description => "Edits on the Settings Menu";

        private static GameObject SettingsRestart, SettingsExit, RealSettingsExit, RealLogoutButton, MintInfoButton, MintInfoPanel, functionsButton, StreammerModeToggle;
        private static GameObject Label4, Label1, Label2, Label3, Label5;

        private static GameObject CopiedFor1, CopiedFor2, CopiedFor3, CopiedFor4, CopiedFor5;
        private static GameObject Tick1, Tick2, Tick3, Tick4, Tick5;

		internal override void OnUserInterface() {
			#region Loading screen restart
			
			try {
				functionsButton = UnityEngine.Object.Instantiate(GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/ButtonMiddle")!.transform, 
					GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup")!.transform).gameObject;
				functionsButton.GetComponent<RectTransform>().anchoredPosition += new Vector2(0f, -128f);
				functionsButton.name = "Mint_LoadingScreenRestart";
				if (!Config.ColorGameMenu.Value) {
					functionsButton.GetComponent<Button>().GetComponentInChildren<Image>().color = Minty;
					functionsButton.GetComponentInChildren<Image>().color = Minty;
				}
				functionsButton.GetComponentInChildren<Text>().text = "Restart Game";
				functionsButton.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
				functionsButton.GetComponent<Button>().onClick.AddListener(new Action(Utils.General.RestartGame));
				// if ((ModCompatibility.emmVRC && GETemmVRCconfig.ReadConfig().ForceRestartButtonEnabled == false) || !ModCompatibility.emmVRC)
					functionsButton.SetActive(true);
				/*else if (ModCompatibility.emmVRC && GETemmVRCconfig.ReadConfig().ForceRestartButtonEnabled)
					functionsButton.SetActive(false);*/
			} catch (Exception ex) {
				Con.Error(ex);
			}

			#endregion

			#region Settings Page
			if (!ModCompatibility.SettingsRestart) {
				#region Exit Button
				RealSettingsExit = GameObject.Find("UserInterface/MenuContent/Screens/Settings/Footer/Exit").gameObject;

				SettingsExit = UnityEngine.Object.Instantiate<GameObject>(GameObject.Find("UserInterface/MenuContent/Screens/Settings/Footer/Exit"), GameObject.Find("UserInterface/MenuContent/Screens/Settings/Footer").transform);
				SettingsExit.GetComponentInChildren<Text>().text = "EXIT";
				SettingsExit.name = "Mint_SettingsExitGame";
				SettingsExit.GetComponent<RectTransform>().localPosition = new Vector2(-90f, -456f);
				SettingsExit.GetComponent<RectTransform>().sizeDelta -= new Vector2(150.0f, 0.0f);
				if (!Config.ColorGameMenu.Value) {
					SettingsExit.GetComponent<Button>().GetComponentInChildren<Image>().color = Minty;
					SettingsExit.GetComponentInChildren<Image>().color = Minty;
				}
				SettingsExit.SetActive(true);
				SettingsExit.GetComponentInChildren<Button>().onClick.AddListener(new System.Action(() => { RealSettingsExit.GetComponent<Button>().onClick.Invoke(); }));
				#endregion

				#region Restart Button
				SettingsRestart = UnityEngine.Object.Instantiate<GameObject>(GameObject.Find("UserInterface/MenuContent/Screens/Settings/Footer/Exit"), GameObject.Find("UserInterface/MenuContent/Screens/Settings/Footer").transform);
				SettingsRestart.transform.SetParent(GameObject.Find("UserInterface/MenuContent/Screens/Settings/Footer/").transform);
				SettingsRestart.name = "Mint_SettingsRestartGame";
				SettingsRestart.GetComponentInChildren<Text>().text = "RESTART";
				SettingsRestart.GetComponent<RectTransform>().localPosition = new Vector2(90f, -456f);
				SettingsRestart.GetComponent<RectTransform>().sizeDelta = new Vector2(200f, 109f);
				if (!Config.ColorGameMenu.Value) {
					SettingsRestart.GetComponent<Button>().GetComponentInChildren<Image>().color = Minty;
					SettingsRestart.GetComponentInChildren<Image>().color = Minty;
				}
				SettingsRestart.SetActive(true);
				SettingsRestart.GetComponentInChildren<Button>().onClick.AddListener(new System.Action(() => {
					//MintMod.Functions.Managers.RestartRestore.GetPosRotWorld();
					try {
						Process.Start(System.Environment.CurrentDirectory + "\\VRChat.exe", System.Environment.CommandLine.ToString());
					} catch (Exception ex) {
						Con.Error(ex);
					}
					RealSettingsExit.GetComponent<Button>().onClick.Invoke();
				}));
				#endregion

				#region StreamerModeToggle

				// var streammerModeToggle = GameObject.Find("UserInterface/MenuContent/Screens/Settings/ComfortSafetyPanel/StreamerModeToggle");
				// var e = streammerModeToggle.GetComponent<Toggle>().onValueChanged;
				// e.AddListener(new Action<bool>((isOn) => {
				// 	// MintUserInterface.isStreamerModeOn = isOn;
				// 	// if (isOn) {
				// 	// 	MintInfoButton!.SetActive(false);
				// 	// 	RealSettingsExit!.SetActive(true);
				// 	// 	SettingsExit!.SetActive(false);
				// 	// 	SettingsRestart!.SetActive(false);
				// 	// 	functionsButton!.SetActive(false);
				// 	// 	MintInfoPanel!.SetActive(false);
				// 	//
				// 	// 	if (Config.AviFavsEnabled.Value) {
				// 	// 		ReFavs._instance._favoriteAvatarList.GameObject.SetActive(Config.AviFavsEnabled.Value);
				// 	// 		ReFavs._instance._favoriteButton.GameObject.SetActive(Config.AviFavsEnabled.Value);
				// 	// 	}
				// 	//
				// 	// 	/*if (AvatarFavs.AviFavLogic.ranOnce) {
				// 	// 		AvatarFavs.AviFavLogic.DestroyList();
				// 	// 		AvatarFavs.AviFavLogic.ranOnce = false;
				// 	// 		MelonCoroutines.Start(AvatarFavs.AviFavLogic.RefreshMenu(1f));
				// 	// 	}*/
				// 	// } else {
				// 	// 	MintInfoButton!.SetActive(true);
				// 	// 	RealSettingsExit!.SetActive(false);
				// 	// 	SettingsExit!.SetActive(true);
				// 	// 	SettingsRestart!.SetActive(true);
				// 	// 	functionsButton!.SetActive(true);
				// 	// 	
				// 	// 	if (Config.AviFavsEnabled.Value)
				// 	// 		ReFavs._instance._favoriteAvatarList.GameObject.SetActive(Config.AviFavsEnabled.Value);
				// 	//
				// 	// 	//if (!AvatarFavs.AviFavLogic.ranOnce)
				// 	// 	//	AvatarFavs.AviFavLogic.Instance.OnUserInterface();
				// 	// }
				// 	MintUserInterface.UpdateMintIconForStreamerMode(isOn);
				// 	MenuContentBackdrop.UpdateForStreamerMode(isOn);
				// 	HudIcon.UpdateForStreamerMode(isOn);
				// 	ReColor.Instance.OnUserInterface();
				// }));

				#endregion
			}

			#region Mint Info
			MintInfoButton = UnityEngine.Object.Instantiate<GameObject>(GameObject.Find("UserInterface/MenuContent/Screens/Settings/Button_AdvancedOptions"), GameObject.Find("UserInterface/MenuContent/Screens/Settings/Footer").transform);
			MintInfoButton.name = "Mint_SettingsInfo";
			MintInfoButton.GetComponentInChildren<Text>().text = "Mint Info";
			//MintInfoButton.GetComponentInChildren<Text>().font = LoliteResources.Resources.BalooFont;
			MintInfoButton.GetComponentInChildren<Text>().fontSize = 26;
			MintInfoButton.GetComponent<RectTransform>().localPosition = new Vector2(-685, 375f);
			MintInfoButton.GetComponent<RectTransform>().sizeDelta -= new Vector2(100.0f, 0.0f);
			MintInfoButton.GetComponent<Image>().color = Color.white;
			//if (!Config.ColorGameMenu.Value) {
				MintInfoButton.GetComponent<Button>().GetComponentInChildren<Image>().color = Minty;
				MintInfoButton.GetComponentInChildren<Image>().color = Minty;
			//}
			MintInfoButton.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
			MintInfoButton.GetComponentInChildren<Button>().onClick.AddListener(new System.Action(() => {
				if (MintInfoPanel.activeInHierarchy)
					MintInfoPanel.SetActive(false);
				else
					MintInfoPanel.SetActive(true);
			}));

			MintInfoButton.SetActive(true);

			MintInfoPanel = UnityEngine.Object.Instantiate<GameObject>(GameObject.Find("UserInterface/MenuContent/Screens/Settings/ComfortSafetyPanel"), GameObject.Find("UserInterface/MenuContent/Screens/Settings").transform);
			MintInfoPanel.name = "Mint_SettingsInfoPanel";
			var name = MintInfoPanel.name;
			MintInfoPanel.GetComponent<RectTransform>().localPosition = new Vector2(-896, 410);
			MintInfoPanel.GetComponent<RectTransform>().localScale = new Vector3(0.65f, 0.75f, 0.75f);
			MintInfoPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(400, 330);
			MintInfoPanel.SetActive(false);

			var title = GameObject.Find($"UserInterface/MenuContent/Screens/Settings/{name}/TitleText (1)");
			title.GetComponent<Text>().text = "Mint Info";

			//Title.GetComponent<Text>().font = LoliteResources.Resources.BalooFont;

			#region Holoport => Server
			CopiedFor1 = UnityEngine.Object.Instantiate<GameObject>(GameObject.Find("UserInterface/MenuContent/Screens/Settings/ComfortSafetyPanel/AllowUntrustedURL"), GameObject.Find($"UserInterface/MenuContent/Screens/Settings/{name}").transform);
			CopiedFor1.gameObject.name = "Server";
			CopiedFor1.GetComponent<RectTransform>().localPosition = new Vector3(-240, -98, 0);
			Tick1 = GameObject.Find($"UserInterface/MenuContent/Screens/Settings/{name}/Server/Background");
			Tick1.gameObject.SetActive(false);
			Component.Destroy(Tick1.GetComponent<UiSettingConfig>());

			Label1 = GameObject.Find($"UserInterface/MenuContent/Screens/Settings/{name}/Server/Label");
			Label1.GetComponent<Text>().supportRichText = true;
			Label1.GetComponent<Text>().text = $"isAuthed -> <color={(ServerAuth.CanLoadMod ? "#00ff00>true" : "red>false")}</color>";
			#endregion

			#region Comfort Turn => IsCute
			CopiedFor2 = UnityEngine.Object.Instantiate<GameObject>(GameObject.Find("UserInterface/MenuContent/Screens/Settings/ComfortSafetyPanel/AllowUntrustedURL"), GameObject.Find($"UserInterface/MenuContent/Screens/Settings/{name}").transform);
			CopiedFor2.gameObject.name = "VPM";
			CopiedFor2.GetComponent<RectTransform>().localPosition = new Vector3(-240, -147, 0);
			Tick2 = GameObject.Find($"UserInterface/MenuContent/Screens/Settings/{name}/VPM/Background");
			Tick2.gameObject.SetActive(false);
			Component.Destroy(Tick2.GetComponent<UiSettingConfig>());

			Label2 = GameObject.Find($"UserInterface/MenuContent/Screens/Settings/{name}/VPM/Label");
			Label2.GetComponent<Text>().supportRichText = true;
			Label2.GetComponent<Text>().text = "isCute -> <color=#00ff00>true</color>";
			#endregion

			#region PersonalSpace => Mod Build Type
			CopiedFor3 = UnityEngine.Object.Instantiate<GameObject>(GameObject.Find("UserInterface/MenuContent/Screens/Settings/ComfortSafetyPanel/AllowUntrustedURL"), GameObject.Find($"UserInterface/MenuContent/Screens/Settings/{name}").transform);
			CopiedFor3.gameObject.name = "ReleaseType";
			CopiedFor3.GetComponent<RectTransform>().localPosition = new Vector3(-240, -196, 0);
			Tick3 = GameObject.Find($"UserInterface/MenuContent/Screens/Settings/{name}/ReleaseType/Background");
			Tick3.gameObject.SetActive(false);
			Component.Destroy(Tick3.GetComponent<UiSettingConfig>());

			Label3 = GameObject.Find($"UserInterface/MenuContent/Screens/Settings/{name}/ReleaseType/Label");
			Label3.GetComponent<Text>().supportRichText = true;
			Label3.GetComponent<Text>().text = $"<color=#82ffbe>MintMod {(MintCore.IsDebug ? "Debug" : "Release")}</color>";
			#endregion

			#region AllowUntrustedURLs => Supporters
			CopiedFor4 = UnityEngine.Object.Instantiate<GameObject>(GameObject.Find("UserInterface/MenuContent/Screens/Settings/ComfortSafetyPanel/AllowUntrustedURL"), GameObject.Find($"UserInterface/MenuContent/Screens/Settings/{name}").transform);
			CopiedFor4.gameObject.name = "Supporters";
			CopiedFor4.GetComponent<RectTransform>().localPosition = new Vector3(-240, -247, 0);
			Tick4 = GameObject.Find($"UserInterface/MenuContent/Screens/Settings/{name}/Supporters/Background");
			Tick4.gameObject.SetActive(false);
			Component.Destroy(Tick4.GetComponent<UiSettingConfig>());

			Label4 = GameObject.Find($"UserInterface/MenuContent/Screens/Settings/{name}/Supporters/Label");
			Label4.GetComponent<Text>().supportRichText = true;
			Label4.GetComponent<Text>().text = " ";
			#endregion

			#region Desrtoy StreamerMode Toggle
			GameObject streamermodebuttontoggle = GameObject.Find($"UserInterface/MenuContent/Screens/Settings/{name}/StreamerModeToggle");
			GameObject.Destroy(streamermodebuttontoggle);
			#endregion

			#region MintVersion
			CopiedFor5 = UnityEngine.Object.Instantiate<GameObject>(GameObject.Find("UserInterface/MenuContent/Screens/Settings/ComfortSafetyPanel/AllowUntrustedURL"), GameObject.Find($"UserInterface/MenuContent/Screens/Settings/{name}").transform);
			CopiedFor5.gameObject.name = "MintVersion";
			CopiedFor5.GetComponent<RectTransform>().localPosition = new Vector3(-240, -295, 0);
			Tick5 = GameObject.Find($"UserInterface/MenuContent/Screens/Settings/{name}/MintVersion/Background");
			Tick5.gameObject.SetActive(false);
			Component.Destroy(Tick5.GetComponent<UiSettingConfig>());

			Label5 = GameObject.Find($"UserInterface/MenuContent/Screens/Settings/{name}/MintVersion/Label");
			Label5.GetComponent<Text>().supportRichText = true;
			Label5.GetComponent<Text>().text = $"MintMod: v<color=#82ffbe>{MintCore.ModBuildInfo.Version}</color>";
			#endregion

			#region Destroy other objects
			try {
				var list = new List<string> {
					$"UserInterface/MenuContent/Screens/Settings/{name}/MuteUsersToggle",
					$"UserInterface/MenuContent/Screens/Settings/{name}/BlockAvatarsToggle",
					$"UserInterface/MenuContent/Screens/Settings/{name}/HeadSetGazeToggle",
					$"UserInterface/MenuContent/Screens/Settings/{name}/KeyboardToggle",
					$"UserInterface/MenuContent/Screens/Settings/{name}/GamepadToggle",
					$"UserInterface/MenuContent/Screens/Settings/{name}/PrimaryInputPanel",
					$"UserInterface/MenuContent/Screens/Settings/{name}/LocomotionInputPanel (1)",
					$"UserInterface/MenuContent/Screens/Settings/{name}/HoloportToggle",
					$"UserInterface/MenuContent/Screens/Settings/{name}/ComfortTurnToggle",
					$"UserInterface/MenuContent/Screens/Settings/{name}/PersonalSpaceToggle",
					$"UserInterface/MenuContent/Screens/Settings/{name}/AllowUntrustedURL"
				};
				foreach (var g in list) {
					GameObject.Find(g)!.Destroy();
				}
			} catch { /**/ }
			#endregion
			#endregion

			#endregion


			MelonCoroutines.Start(FindModCompatibility());
		}

        private static IEnumerator FindModCompatibility() {
            yield return new WaitForSeconds(6f);
            if (!ModCompatibility.SettingsRestart) {
                RealSettingsExit.SetActive(false);
                if (RealSettingsExit.activeInHierarchy) {
                    yield return new WaitForSeconds(2f);
                    RealSettingsExit.SetActive(false);
                }
            }
            
            // if (MintUserInterface.isStreamerModeOn) {
	           //  MintInfoButton!.SetActive(false);
	           //  RealSettingsExit!.SetActive(true);
	           //  SettingsExit!.SetActive(false);
	           //  SettingsRestart!.SetActive(false);
	           //  functionsButton!.SetActive(false);
	           //  MintInfoPanel!.SetActive(false);
            // }
            yield break;
        }
    }
}
