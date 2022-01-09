using System.IO;
using System.Reflection;
using MintyLoader;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace MintMod.Managers.Notification {
    internal class NotificationSystem : MintSubMod {
        public override string Name => "NotificationSystem";
        public override string Description => "Bono's funny Notification System.";
        
        public static NotificationSystem Instance;
        public static Sprite Alert, Crown, Handcuffs, Key, Link, Megaphone, MicrophoneOff;
        
        //AssetBundle Parts
        private AssetBundle _notifBundle;
        private GameObject _notificationPrefab;
        
        //VRCUI
        private GameObject _hudContent;
        
        internal override void OnUserInterface() {
            Instance = new NotificationSystem();
            
            Instance.LoadAssets();
            
            ClassInjector.RegisterTypeInIl2Cpp<NotificationController_Mint>();

            if (Instance._notificationPrefab == null) {
                Con.Error("Unable to load Notification! Notification system will not function!");
                return;
            }
            
            Instance._hudContent = GameObject.Find("/UserInterface/UnscaledUI/HudContent");
            
            //Instantiate prefab and let NotificationController setup!
            var notifInstance = Object.Instantiate(Instance._notificationPrefab, Instance._hudContent.transform);
            notifInstance.AddComponent<NotificationController_Mint>();
        }

        private void LoadAssets() {
            using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MintMod.Managers.Notification.mintnotification")) {
                Con.Debug("Loaded Notification Asset Bundle");
                if (assetStream != null)
                    using (var tempStream = new MemoryStream((int) assetStream.Length)) {
                        assetStream.CopyTo(tempStream);

                        _notifBundle = AssetBundle.LoadFromMemory_Internal(tempStream.ToArray(), 0);
                        _notifBundle.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                    }
            }

            if (_notifBundle != null) {
                //Load Sprites
                Alert = _notifBundle.LoadAsset_Internal("Alert", Il2CppType.Of<Sprite>()).Cast<Sprite>();
                Alert.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                Crown = _notifBundle.LoadAsset_Internal("Crown - Stars", Il2CppType.Of<Sprite>()).Cast<Sprite>();
                Crown.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                Handcuffs = _notifBundle.LoadAsset_Internal("Handcuffs", Il2CppType.Of<Sprite>()).Cast<Sprite>();
                Handcuffs.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                Key = _notifBundle.LoadAsset_Internal("Key", Il2CppType.Of<Sprite>()).Cast<Sprite>();
                Key.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                Link = _notifBundle.LoadAsset_Internal("Link", Il2CppType.Of<Sprite>()).Cast<Sprite>();
                Link.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                Megaphone = _notifBundle.LoadAsset_Internal("Megaphone", Il2CppType.Of<Sprite>()).Cast<Sprite>();
                Megaphone.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                MicrophoneOff = _notifBundle.LoadAsset_Internal("Microphone Off", Il2CppType.Of<Sprite>()).Cast<Sprite>();
                MicrophoneOff.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                
                //Prefab
                _notificationPrefab = _notifBundle.LoadAsset_Internal("Notification", Il2CppType.Of<GameObject>()).Cast<GameObject>();
                _notificationPrefab.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            }

            Con.Debug("Successfully loaded in assets from Notification bundle!");
        }
    }
}