using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.UI;

namespace MintMod.Resources {
    class MintyResources : MintSubMod {
        public override string Name => "MintyResources";
        public override string Description => "Contains images in asset bundles.";
        private static AssetBundle MintBundle, ActionMenuBundle;

        public static Sprite masterCrown, MintIcon, MintTabIcon;
        public static Texture2D MintIcon2D, FreezeIcon, JumpIcon, ESPIcon, FlyIcon, VRTPIcon;
        public static Texture basicGradient;

        internal override void OnStart() {
            MelonLogger.Msg("Loading AssetBundles");
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MintMod.Resources.mintbundle")) {
                using (var memoryStream = new MemoryStream((int)stream.Length)) {
                    stream.CopyTo(memoryStream);
                    MintBundle = AssetBundle.LoadFromMemory_Internal(memoryStream.ToArray(), 0);
                    MintBundle.hideFlags |= HideFlags.DontUnloadUnusedAsset;

                    try { masterCrown = LoadSprite("masterCrown.png"); } catch { MelonLogger.Error("Resource masterCrown.png failed"); }
                    try { MintIcon = LoadSprite("MintMod.png"); } catch { MelonLogger.Error("Resource MintMod.png failed"); }
                    try { MintTabIcon = LoadSprite("MintMod_flat.png"); } catch { MelonLogger.Error("Resource MintMod_flat.png failed"); }
                    try { basicGradient = LoadTexture("Gradient.png"); } catch { MelonLogger.Error("Resource Gradient.png failed"); }
                }
            }

            using (var stream2 = Assembly.GetExecutingAssembly().GetManifestResourceStream("MintMod.Resources.mintactionmenu.mint")) {
                using (var memoryStream2 = new MemoryStream((int)stream2.Length)) {
                    stream2.CopyTo(memoryStream2);
                    ActionMenuBundle = AssetBundle.LoadFromMemory_Internal(memoryStream2.ToArray(), 0);
                    ActionMenuBundle.hideFlags |= HideFlags.DontUnloadUnusedAsset;

                    try { MintIcon2D = LoadTexture2D("MintIcon.png"); } catch { MelonLogger.Error("Failed to load Texture: MintIcon.png"); }
                    try { FreezeIcon = LoadTexture2D("FreezeIcon.png"); } catch { MelonLogger.Error("Failed to load Texture: FreezeIcon.png"); }
                    try { JumpIcon = LoadTexture2D("JumpIcon.png"); } catch { MelonLogger.Error("Failed to load Texture: JumpIcon.png"); }
                    try { ESPIcon = LoadTexture2D("ESPIcon.png"); } catch { MelonLogger.Error("Failed to load Texture: ESPIcon.png"); }
                    try { FlyIcon = LoadTexture2D("FlyIcon.png"); } catch { MelonLogger.Error("Failed to load Texture: FlyIcon.png"); }
                    try { VRTPIcon = LoadTexture2D("VRTPIcon.png"); } catch { MelonLogger.Error("Failed to load Texture: VRTPIcon.png"); }
                }
            }
            MelonLogger.Msg("Done loading AssetBundles.");
        }

        private static Sprite LoadSprite(string sprite) {
            Sprite sprite2 = MintBundle.LoadAsset(sprite, Il2CppType.Of<Sprite>()).Cast<Sprite>();
            sprite2.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            //sprite2.hideFlags = HideFlags.HideAndDontSave;
            return sprite2;
        }

        private static Texture LoadTexture(string tex) {
            Texture sprite2 = MintBundle.LoadAsset(tex, Il2CppType.Of<Texture>()).Cast<Texture>();
            sprite2.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            //sprite2.hideFlags = HideFlags.HideAndDontSave;
            return sprite2;
        }

        private static Texture2D LoadTexture2D(string tex) {
            Texture2D tex2 = ActionMenuBundle.LoadAsset_Internal(tex, Il2CppType.Of<Texture2D>()).Cast<Texture2D>();
            tex2.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            //tex2.hideFlags = HideFlags.HideAndDontSave;
            return tex2;
        }

        public static Texture2D ConvertToSprite(Sprite s) {
            var croppedTexture = new Texture2D((int)s.rect.width, (int)s.rect.height);
            var pixels = s.texture.GetPixels((int)s.textureRect.x,
                (int)s.textureRect.y,
                (int)s.textureRect.width,
                (int)s.textureRect.height);
            croppedTexture.SetPixels(pixels);
            croppedTexture.Apply();
            return croppedTexture;
        }

        public static Sprite ConvertToTexture2D(Texture2D t) => Sprite.Create(t, new Rect(0, 0, t.width, t.height), Vector2.zero);
    }
}
