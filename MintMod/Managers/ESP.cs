using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VRC;
using static MintMod.Managers.Colors;

namespace MintMod.Managers {
    class ESP : MintSubMod {
        public override string Name => "ESP";
        public override string Description => "Puts bubbles around target.";

        public static bool isESPEnabled, isPickupESPEnabled;

        public static void PlayerESPState(bool state) {
            isESPEnabled = state;
            GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
            foreach (var e in array)
                HighlightBubble(e, state);
        }

        public static void ClearAllPlayerESP() {
            GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
            foreach (var e in array)
                HighlightBubble(e, false);
        }

        public static void SinglePlayerESP(Player ply, bool state) => HighlightBubble(ply.gameObject, state);

        public static IEnumerator JoinDelay(Player player) {
            if (player == null) yield break;
            int timeout = 0;
            while (player.gameObject == null && timeout < 30) {
                yield return new WaitForSeconds(1f);
                int num = timeout;
                timeout = num + 1;
            }
            Renderer bubbleRenderer = GetBubbleRenderer(player.gameObject);
            HighlightBubble(bubbleRenderer, isESPEnabled);
            SetBubbleColor(bubbleRenderer);
        }

        public static void SetItemESPToggle(bool state) => isPickupESPEnabled = state;

        internal override void OnUpdate() {
            if (!isPickupESPEnabled) return;
            foreach (var vrcPickup in Items.cached) {
                Renderer b = vrcPickup.GetComponent<Renderer>();
                if (b != null)
                    GetHighlightFX().Method_Public_Void_Renderer_Boolean_0(b, isPickupESPEnabled);
            }
        }

        #region ESP Utilites

        static HighlightsFX GetHighlightFX() => HighlightsFX.prop_HighlightsFX_0;

        static void HighlightBubble(GameObject @object, bool state) {
            Renderer bubbleRenderer = GetBubbleRenderer(@object);
            if (bubbleRenderer != null)
                HighlightsFX.prop_HighlightsFX_0.Method_Public_Void_Renderer_Boolean_0(bubbleRenderer, state);
        }

        public static void HighlightBubble(Renderer renderer, bool state) {
            if (renderer == null) return;
            HighlightsFX.prop_HighlightsFX_0.Method_Public_Void_Renderer_Boolean_0(renderer, state);
        }

        public static void SetBubbleColor(Renderer renderer) {
            if (renderer != null) {
                renderer.sharedMaterial.color = Minty;
                renderer.material.color = Minty;
            }
        }

        static Renderer GetBubbleRenderer(GameObject @object) {
            if (@object == null)
                return null;
            Transform transform = @object.transform.Find("SelectRegion");
            if (transform == null)
                return null;
            return transform.GetComponent<Renderer>() ?? null;
        }

        #endregion
    }
}
