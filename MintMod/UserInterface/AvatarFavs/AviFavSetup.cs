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

            public static Favorites Load() {
                if (!File.Exists(final)) {
                    File.WriteAllText(final, JsonConvert.SerializeObject(new Favorites(), Formatting.Indented));
                    try {
                        Get();
                        File.WriteAllText(final, JsonConvert.SerializeObject(_Config, Formatting.Indented));
                    } catch { }
                }
                return JsonConvert.DeserializeObject<Favorites>(File.ReadAllText(final));
            }

            private static FavoriteList _Config { get; set; }
            public static FavoriteList Get() => JsonConvert.DeserializeObject<FavoriteList>(File.ReadAllText(final));

            public static Favorites Instance;
        }
    }
}
