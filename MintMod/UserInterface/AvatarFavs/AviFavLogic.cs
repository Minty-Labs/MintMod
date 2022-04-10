using System;
using System.Linq;
using MintyLoader;
using static MintMod.UserInterface.AvatarFavs.AviFavSetup;

namespace MintMod.UserInterface.AvatarFavs {
    internal class AviFavLogic {
        internal static bool AviFavsErrored;

        internal static void OnAppStart() {
            try {
                Favorites.CreateAviFavJsonFile();
            }
            catch (Exception e) {
               Con.Error($"Avatar Favs Failed to load\n{e}");
                AviFavsErrored = true;
            }
        }
        
        public static FavoriteList GetConfigList(int id) => Favorites.Instance.AvatarFavorites.FavoriteLists.Single(l => l.ID == id);
    }
}
