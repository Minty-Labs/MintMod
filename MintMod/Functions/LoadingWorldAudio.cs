using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;
using UnityEngine;
using System.IO;
using System.Collections;
using MelonLoader;
using MintMod.Reflections;

namespace MintMod.Functions {
    class LoadingWorldAudio : MintSubMod {
        public override string Name => "LoadingWorldAudio";
        public override string Description => "This allows you to change the audio of the loading world scene.";

        private readonly string path = MintCore.MintDirectory + "\\Resources\\";

        internal override void OnUserInterface() => MelonCoroutines.Start(Music());

        IEnumerator Music() {
            if (MintCore.isDebug) MelonLogger.Msg("Processing custom menu music...");
            GameObject gameObject = GameObject.Find("LoadingBackground_TealGradient_Music/LoadingSound");
            GameObject gameObject2 = UIWrappers.GetVRCUiMInstance().field_Public_GameObject_0.transform.Find("Popups/LoadingPopup/LoadingSound").gameObject;

            if (gameObject != null)
                gameObject.GetComponent<AudioSource>().Stop();
            if (gameObject2 != null)
                gameObject2.GetComponent<AudioSource>().Stop();

            string url = string.Format("file://{0}", path + "customloadingsound.ogg").Replace("\\", "/");
            UnityWebRequest audio = UnityWebRequest.Get(url);
            audio.SendWebRequest();
            while (!audio.isDone)
                yield return null;
            AudioClip a = null;
            if (audio.isHttpError)
                MelonLogger.Error($"Error loading audio file: {audio.error}");
            else
                a = WebRequestWWW.InternalCreateAudioClipUsingDH(audio.downloadHandler, audio.url, false, false, AudioType.UNKNOWN);

            if (a != null) {
                if (gameObject != null) {
                    gameObject.GetComponent<AudioSource>().clip = a;
                    gameObject.GetComponent<AudioSource>().Play();
                }
                if (gameObject2 != null) {
                    gameObject2.GetComponent<AudioSource>().clip = a;
                    gameObject2.GetComponent<AudioSource>().Play();
                }
            }
            yield return null;
            yield break;
        }
	}
}
