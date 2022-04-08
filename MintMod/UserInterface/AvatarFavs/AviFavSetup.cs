using System;
using MintMod.Reflections.VRCAPI;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace MintMod.UserInterface.AvatarFavs {
    internal class AviFavSetup {
        public class FavoriteList {
            public int ID { get; set; }

            public string name { get; set; }

            public string Desciption { get; set; }

            public int Rows { get; set; } = 2;

            public List<AvatarObject> Avatars { get; set; }
        }

        public List<FavoriteList> FavoriteLists = new List<FavoriteList>();

        public class Favorites {
            public static Favorites Instance;
            private static readonly string pathpath = $"{MintCore.MintDirectory}\\AviFavs.json";
            internal static readonly string final = 
                Config.haveCustomPath.Value ? 
                string.IsNullOrWhiteSpace(Config.customPath.Value) ? pathpath : Config.customPath.Value : 
                pathpath;
            
            public AviFavSetup AvatarFavorites = new();

            public void SaveConfig() => File.WriteAllText(final, JsonConvert.SerializeObject(this, Formatting.Indented));

            public static void CreateAviFavJSONFile() {
                //if (!Config.useWebhostSavedList.Value) {
                    if (!File.Exists(final))
                        File.WriteAllText(final, JsonConvert.SerializeObject(new Favorites(), Formatting.Indented));
                    Instance = JsonConvert.DeserializeObject<Favorites>(File.ReadAllText(final));
                //}
            }
        }
    }
}
