using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using MintMod.Functions;
using MintyLoader;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.UI;

namespace MintMod.Resources {
    class MintyResources : MintSubMod {
        public override string Name => "MintyResources";
        public override string Description => "Contains images in asset bundles.";
        private static AssetBundle MintBundle;

        public static Sprite masterCrown, MintIcon, MintTabIcon, Transparent, BG_HUD;
        public static Sprite address_book, checkered, clipboard, cog, extlink, globe, history, sync, tv, user, wifi, jump, dl, list, messages,
            copy, key, marker, marker_hole, star, userlist, people, clone, ColorPicker;
        public static Texture2D MintIcon2D, FreezeIcon, JumpIcon, ESPIcon, FlyIcon, VRTPIcon;
        public static Texture basicGradient;
        public static Font BalooFont;

        internal override void OnStart() {
            Con.Msg("Loading AssetBundles");
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MintMod.Resources.mintbundle")) {
                using (var memoryStream = new MemoryStream((int)stream.Length)) {
                    stream.CopyTo(memoryStream);
                    MintBundle = AssetBundle.LoadFromMemory_Internal(memoryStream.ToArray(), 0);
                    MintBundle.hideFlags |= HideFlags.DontUnloadUnusedAsset;

                    try { masterCrown = LoadSprite("masterCrown.png"); } catch { Con.Error("Resource masterCrown.png failed"); }
                    try { MintIcon = LoadSprite("MintMod.png"); } catch { Con.Error("Resource MintMod.png failed"); }
                    try { MintTabIcon = LoadSprite("MintMod_flat.png"); } catch { Con.Error("Resource MintMod_flat.png failed"); }
                    try { basicGradient = LoadTexture("Gradient.png"); } catch { Con.Error("Resource Gradient.png failed"); }
                    try { Transparent = LoadSprite("transparent.png"); } catch { Con.Error("Resource transparent.png failed"); }
                    try { MintIcon2D = LoadTexture2D("MintIcon.png"); } catch { Con.Error("Failed to load Texture: MintIcon.png"); }
                    try { FreezeIcon = LoadTexture2D("FreezeIcon.png"); } catch { Con.Error("Failed to load Texture: FreezeIcon.png"); }
                    try { JumpIcon = LoadTexture2D("JumpIcon.png"); } catch { Con.Error("Failed to load Texture: JumpIcon.png"); }
                    try { ESPIcon = LoadTexture2D("ESPIcon.png"); } catch { Con.Error("Failed to load Texture: ESPIcon.png"); }
                    try { FlyIcon = LoadTexture2D("FlyIcon.png"); } catch { Con.Error("Failed to load Texture: FlyIcon.png"); }
                    try { BG_HUD = LoadSprite("PlayerListBackground.png"); } catch { Con.Error("Resources PlayerListBackgrounds.png failed"); }
                    try { BalooFont = LoadFont("Baloo.ttf"); } catch { Con.Error("Resources Baloo.ttf failed"); }
                    
                    try { address_book = LoadSprite("address-book-solid.png"); } catch { Con.Error("Failed to load Texture: address-book-solid.png"); }
                    try { checkered = LoadSprite("chess-board-solid.png"); } catch { Con.Error("Resource chess-board-solid.png failed"); }
                    try { clipboard = LoadSprite("clipboard-list-solid.png"); } catch { Con.Error("Resource clipboard-list-solid.png failed"); }
                    try { clone = LoadSprite("clone-solid.png"); } catch { Con.Error("Resource clone-solid.png failed"); }
                    try { cog = LoadSprite("cog-solid.png"); } catch { Con.Error("Resource cog-solid.png failed"); }
                    try { extlink = LoadSprite("external-link-alt-solid.png"); } catch { Con.Error("Resource external-link-alt-solid.png failed"); }
                    try { globe = LoadSprite("globe-americas-solid.png"); } catch { Con.Error("Resource globe-americas-solid.png failed"); }
                    try { history = LoadSprite("history-solid.png"); } catch { Con.Error("Resource history-solid.png failed"); }
                    try { sync = LoadSprite("sync-alt-solid.png"); } catch { Con.Error("Resource sync-alt-solid.png failed"); }
                    try { tv = LoadSprite("tv-solid.png"); } catch { Con.Error("Resource tv-solid.png failed"); }
                    try { user = LoadSprite("user-solid.png"); } catch { Con.Error("Resource user-solid.png failed"); }
                    try { wifi = LoadSprite("wifi-solid.png"); } catch { Con.Error("Resource wifi-solid.png failed"); }
                    try { jump = LoadSprite("JumpIcon_2.png"); } catch { Con.Error("Resource JumpIcon_2.png failed"); }
                    try { dl = LoadSprite("cloud-download-alt-solid.png"); } catch { Con.Error("Resource cloud-download-alt-solid.png failed"); }
                    try { list = LoadSprite("list-solid.png"); } catch { Con.Error("Resource list-solid.png failed"); }
                    try { copy = LoadSprite("copy-solid.png"); } catch { Con.Error("Resource copy-solid.png failed"); }
                    try { key = LoadSprite("key-solid.png"); } catch { Con.Error("Resource key-solid.png failed"); }
                    try { marker = LoadSprite("map-marker-solid.png"); } catch { Con.Error("Resource map-marker-solid.png failed"); }
                    try { marker_hole = LoadSprite("map-marker-alt-solid.png"); } catch { Con.Error("Resource map-marker-alt-solid.png failed"); }
                    try { star = LoadSprite("star-solid.png"); } catch { Con.Error("Resource star-solid.png failed"); }
                    try { messages = LoadSprite("messages-solid.png"); } catch { Con.Error("Resource messages-solid.png failed"); }
                    try { userlist = LoadSprite("user-list.png"); } catch { Con.Error("Resource user-list.png failed"); }
                    try { people = LoadSprite("people.png"); } catch { Con.Error("Resource people.png failed"); }
                    try { ColorPicker = LoadSprite("color-picker.png"); } catch { Con.Error("Resource color-picker.png failed"); }
                    //try {  = LoadSprite(".png"); } catch { Con.Error("Resource .png failed"); }
                    
                    //try { m_Back = LoadSprite("Back.png"); } catch { Con.Error("Resource Back.png failed"); }
                    //try { m_Foward = LoadSprite("Foward.png"); } catch { Con.Error("Resource Foward.png failed"); }
                    //try { m_Menu = LoadSprite("Menu.png"); } catch { Con.Error("Resource Menu.png failed"); }
                    //try { m_Play = LoadSprite("Play.png"); } catch { Con.Error("Resource Play.png failed"); }
                    //try { m_Stop = LoadSprite("stop.png"); } catch { Con.Error("Resource stop.png failed"); }
                }
            }
            Con.Msg("Done loading AssetBundles.");
        }

        private static Sprite LoadSprite(string sprite) {
            Sprite sprite2 = MintBundle.LoadAsset(sprite, Il2CppType.Of<Sprite>()).Cast<Sprite>();
            sprite2.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            return sprite2;
        }
        
        private static Font LoadFont(string assetToLoad)
        {
            Font loadedFont = MintBundle.LoadAsset_Internal(assetToLoad, Il2CppType.Of<Font>()).Cast<Font>();
            loadedFont.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            return loadedFont;
        }

        private static Texture LoadTexture(string tex) {
            Texture sprite2 = MintBundle.LoadAsset(tex, Il2CppType.Of<Texture>()).Cast<Texture>();
            sprite2.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            return sprite2;
        }

        private static Texture2D LoadTexture2D(string tex) {
            Texture2D tex2 = MintBundle.LoadAsset_Internal(tex, Il2CppType.Of<Texture2D>()).Cast<Texture2D>();
            tex2.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            return tex2;
        }
/*
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
        */
    }
}
