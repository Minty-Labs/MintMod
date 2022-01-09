using System;
using System.Collections;
using System.Collections.Generic;
using MelonLoader;
using MintMod.Functions;
using MintMod.Hooks;
using MintMod.Libraries;
using MintMod.Reflections;
using MintMod.Reflections.VRCAPI;
using MintyLoader;
using ReMod.Core.VRChat;
using TMPro;
using UnhollowerBaseLib.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace MintMod.Managers.Notification
{
    public class NotificationController_Mint : MonoBehaviour
    {
        public static NotificationController_Mint Instance;
        
        private Animator _notificationAnimator;
        private Image _iconImage;
        private TextMeshProUGUI _titleText;
        private TextMeshProUGUI _descriptionText;

        private Queue<NotificationObject> _notificationQueue = new Queue<NotificationObject>();
        private bool _isDisplaying;
        private object _timerToken;
        
        //Current NotificationObject details
        private NotificationObject _currentNotification;

        public NotificationController_Mint(IntPtr ptr) : base(ptr) { }

        [HideFromIl2Cpp]
        public void EnqueueNotification(NotificationObject notif)
        {
            if (WorldReflect.GetWorld() == null || VRC.PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0.Count == 0) return;
            
            _notificationQueue.Enqueue(notif);
        }

        [HideFromIl2Cpp]
        public void ClearNotifications() => _notificationQueue.Clear();

        private void Start()
        {
            Instance = this;
            
            _notificationAnimator = gameObject.GetComponent<Animator>();
            _iconImage = gameObject.transform.Find("Content/Icon").gameObject.GetComponent<Image>();
            _titleText = gameObject.transform.Find("Content/Title").gameObject.GetComponent<TextMeshProUGUI>();
            _descriptionText = gameObject.transform.Find("Content/Description").gameObject.GetComponent<TextMeshProUGUI>();

            //Register for WorldLeave and Join to reset notifications
            Patches.OnWorldJoin += OnWorldChange;
            Patches.OnWorldLeave += OnWorldChange;
            
            var background = gameObject.transform.Find("Content/Background").GetComponent<Image>();
            try {
                background.color = ColorConversion.HexToColor(Config.MenuColorHEX.Value, true, 0.7f);
            }
            catch (Exception e) {
                if (MintCore.isDebug) Con.Error(e);
            }
        }

        private void Update()
        {
            if (_notificationQueue.Count <= 0 || _isDisplaying) return;
            if (WorldReflect.GetWorld() == null || VRC.PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0.Count == 0) return;
            
            _currentNotification = _notificationQueue.Dequeue();

            if (Config.UseOldHudMessages.Value) {
                //Using VRChat HUD Messages
                VRCUiManager.prop_VRCUiManager_0.QueueHudMessage($"[{_currentNotification.Title}] {_currentNotification.Description}", Color.white, _currentNotification.DisplayLength);
                return;
            }
            
            //Update UI
            _titleText.text = _currentNotification.Title;
            _descriptionText.text = _currentNotification.Description;
            _iconImage.sprite = _currentNotification.Icon;
            _iconImage.enabled = true;

            if (_currentNotification.Icon == null)
                _iconImage.enabled = false;
            
            OpenNotification();
        }

        [HideFromIl2Cpp]
        private void OnWorldChange()
        {
            _currentNotification = null;
            _notificationQueue.Clear();
            CloseNotification();
        }

        [HideFromIl2Cpp]
        private void OpenNotification()
        {
            _isDisplaying = true;
            if (_timerToken != null)
            {
                MelonCoroutines.Stop(_timerToken);
                _timerToken = null;
            }
                
            //Play slide in animation
            _notificationAnimator.Play("In");

            //Start notification timer
            _timerToken = MelonCoroutines.Start(StartTimer());
        }

        [HideFromIl2Cpp]
        private void CloseNotification()
        {
            if (!_isDisplaying) return;
            
            if (_timerToken != null)
            {
                MelonCoroutines.Stop(_timerToken);
                _timerToken = null;
            }

            _isDisplaying = false;
            //Play slide out
            _notificationAnimator.Play("Out");
        }

        [HideFromIl2Cpp]
        private IEnumerator StartTimer()
        {
            yield return new WaitForSeconds(_currentNotification.DisplayLength);
            CloseNotification();
        }
    }
}