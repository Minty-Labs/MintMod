using System;
using MintMod.Reflections.VRCAPI;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Principal;
using System.Text;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.Ocsp;
using MintyLoader;
using VRC.Core;

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
            private static readonly string final = 
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

            /*
            public static void GetAviFavJSONFromWebhost() {
                if (Config.useWebhostSavedList.Value) {
                    var localUserID = APIUser.CurrentUser.id;
                    var listName = $"AvatarList_{localUserID}.json";
                    var w = new WebClient();
                    var d = w.DownloadString($"https://mintlily.lgbt/mod/userdata/{listName}");
                    
                    Instance = JsonConvert.DeserializeObject<Favorites>(d);
                }
            }
            
            public static void SendLocalListToServer() {
                var localUserID = APIUser.CurrentUser.id;
                var listName = $"AvatarList_{localUserID}.json";

                var t = JsonConvert.DeserializeObject<Favorites>(File.ReadAllText(final)!);
                using (var client = new HttpClient()) {
                    var reps = client.PostAsync($"https://mintlily.lgbt/mod/userdata/{listName}",
                        new StringContent(t?.ToString(), Encoding.UTF8, "application/json")).GetAwaiter().GetResult();
                }

                
                var httpWebRequest = (HttpWebRequest)WebRequest.Create($"https://mintlily.lgbt/mod/userdata/{listName}");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream())) 
                    streamWriter.Write(t);

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()!)) {
                    var result = streamReader.ReadToEnd();
                    Con.Debug(result, MintCore.isDebug);
                }
            }*/
        }
    }
}
