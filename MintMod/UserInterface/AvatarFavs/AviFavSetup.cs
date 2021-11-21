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
            private static readonly string final = MintCore.MintDirectory + "\\AviFavs.json";
            public AviFavSetup AvatarFavorites = new();

            public void SaveConfig() {
                string contents = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(final, contents);
            }

            public static void CreateAviFavJSONFile() {
                if (!File.Exists(final))
                    File.WriteAllText(final, JsonConvert.SerializeObject(new Favorites(), Formatting.Indented));

                Instance = JsonConvert.DeserializeObject<Favorites>(File.ReadAllText(final));
            }

            public static Favorites Instance;
        }
    }
}
