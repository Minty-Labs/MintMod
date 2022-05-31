using System;
using MintMod.UserInterface.QuickMenu;
using MintyLoader;
using UnhollowerBaseLib.Attributes;
using UnhollowerRuntimeLib;
using UnityEngine;
using static MintMod.Reflections.UIWrappers;

#nullable enable
namespace MintMod.Libraries {
    internal class CreateListener : MintSubMod {
        public override string Name => "CreateListener";
        public override string Description => "Creates a component to listen to when a gameobject becomes active.";

        private GameObject? _quickMenuObj;

        internal override void OnStart() {
            bool failed;
            try { ClassInjector.RegisterTypeInIl2Cpp<EnableDisableListener>(); failed = false; }
            catch (Exception e) { Con.Error($"Unable to Inject Custom EnableDisableListener Script!\n{e}"); failed = true; }
            if (!failed) Con.Debug("Finished setting up EnableDisableListener");
        }

        internal override void OnUserInterface() {
            _quickMenuObj = ReMod.Core.VRChat.QuickMenuEx.Instance.field_Public_GameObject_0;

            var listener = _quickMenuObj.GetOrAddComponent<EnableDisableListener>();
            listener.OnEnabled += QmMediaPanel.OnOpen;
            listener.OnDisabled += QmMediaPanel.OnClose;
            
            Con.Debug("Finished Creating QMListener.");
        }
    }
    
    
    public class EnableDisableListener : MonoBehaviour {

        [method:HideFromIl2Cpp]
        public event Action? OnEnabled;
        
        [method:HideFromIl2Cpp]
        public event Action? OnDisabled;

        public EnableDisableListener(IntPtr obj0) : base(obj0) {
        }

        private void OnEnable() => OnEnabled?.Invoke();

        private void OnDisable() => OnDisabled?.Invoke();
    }
}