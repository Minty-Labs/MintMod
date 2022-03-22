using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;
using VRCSDK2;
using RenderHeads.Media.AVProVideo;
using VRC_Pickup = VRC.SDKBase.VRC_Pickup;
using Object = UnityEngine.Object;
using UnityEngine.Rendering.PostProcessing;
using System.Collections;
using MelonLoader;
using MintMod.Functions;
using MintMod.UserInterface.QuickMenu;
using UnhollowerBaseLib;
using MintyLoader;
using ReMod.Core.UI.QuickMenu;
using Logger = VRC.Core.Logger;

namespace MintMod.Managers {
    public class PedestalManager {
        public GameObject parentPedestal;
        public bool status;
    }
    
    internal class Components : MintSubMod {
        public override string Name => "Component Manager";
        public override string Description => "Basically ComponentToggle";
        
        public static List<PedestalManager> theWorldPedestals;
        private static GameObject[] Pens;

        internal static bool Pickups = true, PickupObjects = true, PostProcessing = true, _Pens = true;
        static bool WorldWasChanged = false;
        private static Il2CppArrayBase<VRC_Pickup> pickups;

        internal override void OnLevelWasLoaded(int buildindex, string SceneName) {
            if (buildindex == -1) {
                WorldWasChanged = true;

                pickups = Object.FindObjectsOfType<VRC_Pickup>();
                foreach (var obj in pickups) {
                    obj.GetComponent<VRC_Pickup>().pickupable = Pickups;
                    obj.gameObject.SetActive(PickupObjects);
                }
            
                if (theWorldPedestals != null)
                    theWorldPedestals.Clear();
                theWorldPedestals = new List<PedestalManager>();
                var AvatarPedestals = UnityEngine.Resources.FindObjectsOfTypeAll<VRC.SDKBase.VRC_AvatarPedestal>();
                foreach (var ap in AvatarPedestals) {
                    theWorldPedestals.Add(new PedestalManager {
                        parentPedestal = ap.gameObject,
                        status = ap.gameObject.activeSelf
                    });
                }
            
                Pens = (from x in Object.FindObjectsOfType<GameObject>()
                    where x.name.ToLower().Contains("pen") | x.name.ToLower().Contains("marker") | x.name.ToLower().Contains("grip")
                    select x).ToArray();
            
                MelonCoroutines.Start(DelayedEvent());
            }
        }
        
        private static IEnumerator DelayedEvent() {
            yield return new WaitForSeconds(5);
            if (WorldWasChanged) {
                WorldWasChanged = false;
                foreach (Camera cam in Camera.allCameras) {
                    if (cam.GetComponent<PostProcessLayer>() != null) {
                        if (PostProcessing != cam.GetComponent<PostProcessLayer>().enabled) {
                            Con.Debug(PostProcessing ? "Auto Removed Post Processing" : "Auto Re-added Post Processing", MintCore.isDebug);
                            cam.GetComponent<PostProcessLayer>().enabled = PostProcessing;
                        }
                    }
                }
            }
        }

        internal static void ComponentToggle(ReMenuCategory c) {
            var pi = c.AddToggle("Pickups", "Toggle Pickup on objects", t => {
                Pickups = t;
                foreach (var obj in pickups) {
                    obj.GetComponent<VRC_Pickup>().pickupable = t;
                    //obj.gameObject.SetActive(t);
                }
            });
            pi.Toggle(Pickups, false, true);
            
            var po = c.AddToggle("Pickup Objects", "Toggle Pickup Objects", t => {
                PickupObjects = t;
                foreach (var obj in pickups) {
                    //obj.GetComponent<VRC_Pickup>().pickupable = t;
                    obj.gameObject.SetActive(t);
                }
            });
            po.Toggle(PickupObjects, false, true);
            
            var pp = c.AddToggle("Post Processing", "Toggle Post Processing", t => {
                PostProcessing = t;
                foreach (Camera cam in Camera.allCameras) {
                    if (cam.GetComponent<PostProcessLayer>() != null) {
                        if (t != cam.GetComponent<PostProcessLayer>().enabled) {
                            Con.Debug(t ? "Removed Post Processing" : "Re-added Post Processing", MintCore.isDebug);
                            cam.GetComponent<PostProcessLayer>().enabled = t;
                        }
                    }
                }
            });
            pp.Toggle(PostProcessing, false, true);
            
            var pe = c.AddToggle("Pens", "Toggle Pens in the world", t => {
                _Pens = t;
                foreach (var p in Pens)
                    p.gameObject.SetActive(t);
            });
            pe.Toggle(_Pens, false, true);
            
            var ap = c.AddToggle("Pedestals", "Toggle Avatar Pedestals in the world", t => {
                if (theWorldPedestals.Count != 0) {
                    if (!t) {
                        foreach (var original in theWorldPedestals) 
                            original.parentPedestal.SetActive(false);
                    } else {
                        foreach (var original in theWorldPedestals) 
                            original.parentPedestal.SetActive(original.status);
                    }
                }
            });
            ap.Toggle(true, false, true);

            var ch = c.AddToggle("Chairs", "Toggle chairs in the world", t => {
                MelonPreferences.GetEntry<bool>(Config.Base.Identifier, Config.CanSitInChairs.Identifier).Value = t;
            });
            ch.Toggle(Config.CanSitInChairs.Value, false, true);
        }
    }
}