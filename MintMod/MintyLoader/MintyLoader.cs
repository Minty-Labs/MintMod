using MelonLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace MintyLoader
{
    public static class BuildInfo
    {
        public const string Name = "MintyLoader";
        public const string Author = "Lily & DDAkebono";
        public const string Company = "LilyMod";
        public const string Version = "2.2.0";
        public const string DownloadLink = null;
    }
   
    public class MintyLoader : MelonMod
    {
        public static MintyLoader instance;
        static readonly DirectoryInfo MintDirectory = new DirectoryInfo($"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\VRChat");
        static readonly string AuthFile = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\VRChat\\auth.txt";
        internal MelonMod MintMod;
        internal Assembly Local_mintAssembly;
        static bool hasQuit;
        private string checkedVer, site, final_site, auth_folder, passwords;
        private string[] site_split, splitsite_6;
        private readonly string modURL = "https://mintlily.lgbt/mod/MintMod.dll";

        public override void OnApplicationStart()
        {
            instance = this;
            MelonLogger.Msg(ConsoleColor.Green, "MintyLoader Startup!");
            WebClient checkVer = new WebClient();
            checkedVer = checkVer.DownloadString("https://mintlily.lgbt/mod/loader/version.txt");

            MelonLogger.Msg(ConsoleColor.DarkCyan, "Loader Build version: " + BuildInfo.Version.ToString() + " :: Server pulled: " + checkedVer.ToString());

            //RunAndConvert().NoAwait();
            //while (!RunAndConvert().IsCompleted) Task.WaitAll();

            if (Environment.CommandLine.Contains("--MintyDev")) {
                MelonLogger.Msg(ConsoleColor.DarkRed, $"Final Site: {final_site}");

                try {
                    MelonLogger.Msg(ConsoleColor.Yellow, "Loading Local Mod");
                    Local_mintAssembly = Assembly.LoadFile("MintMod.dll");
                    if (Local_mintAssembly != null) loadModuleCore(Local_mintAssembly);
                    return;
                } catch (Exception e) {
                    MelonLogger.Error($"{e}");
                    /*
                    if (Local_mintAssembly != null) Local_mintAssembly = null;
                    MelonLogger.Warning("Could not load Local Mod, loading MintMod from the server.");
                    if (!MintDirectory.Exists)
                        MintDirectory.Create();

                    Assembly mintAssembly = getMintAssy();
                    if (mintAssembly != null) loadModuleCore(mintAssembly);
                    */
                    return;
                }
            }
            if (checkedVer == BuildInfo.Version) {
                MelonLogger.Msg(ConsoleColor.Cyan, "Loader is up to date");
                if (!MintDirectory.Exists)
                    MintDirectory.Create();

                Assembly mintAssembly = getMintAssy();
                if (mintAssembly != null) loadModuleCore(mintAssembly);
            } else MelonLogger.Warning("MintyLoader is not up to date, please update your DLL.");
        }

        /*
        Task RunAndConvert()
        {
            site = Encoding.UTF8.GetString(Convert.FromBase64String("aHR0cHM6Ly9taW50bGlseS5sZ2J0L21vZC9ZWFYwYUE9PS9saXN0L2NHRnpjM2R2Y21Sei50eHQ="));
            // get (https://mintlily.lgbt/mod/YXV0aA==/list/cGFzc3dvcmRz.txt)
            site_split = site.Split('/');
            //[0] https:
            //[1] 
            //[2] mintlily.lgbt
            //[3] mod
            //[4] YXV0aA==
            //[5] list
            //[6] cGFzc3dvcmRz.txt
            splitsite_6 = site_split[6].Split('.');
            //[0] cGFzc3dvcmRz
            //[1] txt
            // YXV0aA==    to     auth
            auth_folder = Encoding.UTF8.GetString(Convert.FromBase64String(site_split[4]));
            // cGFzc3dvcmRz    to     passwords
            passwords = Encoding.UTF8.GetString(Convert.FromBase64String(splitsite_6[0]));
            final_site = $"{site_split[0]}//{site_split[2]}/{site_split[3]}/{auth_folder}/{site_split[5]}/{passwords}.{splitsite_6[1]}";
            return Task.CompletedTask;
        }
        */

        private void loadModuleCore(Assembly assy)
        {
            if (assy != null) {
                foreach (Type componentType in ((IEnumerable<Type>)assy.GetTypes()).Where(t => t.IsSubclassOf(typeof(MelonMod)))) {
                    try {
                        MelonMod coreLoad = Activator.CreateInstance(componentType) as MelonMod;
                        coreLoad.OnApplicationStart();
                        MintMod = coreLoad;
                    } catch (TypeLoadException e) {
                        MelonLogger.Error(e.Message);
                        MelonLogger.Error("Unable to load MintMod -> If VRChat has been updated, MintMod likely needs to be updated as well.");
                    }
                }
            }
        }

        private string getAuthToken(bool retry = false)
        {
            string auth = "";

            if (File.Exists(AuthFile)) auth = File.ReadAllText(AuthFile); //We gots an auth

            if(string.IsNullOrWhiteSpace(auth)) {
                //No auth file found
                if (!retry)
                    MelonLogger.Msg("\u001b[0m[\u001b[34mAUTHENTICATOR\u001b[0m] \u001b[36mNo MintMod auth.txt found! Please enter your auth key:\u001b[0m");
                else
                    MelonLogger.Msg("\u001b[0m[\u001b[34mAUTHENTICATOR\u001b[0m] \u001b[36mYou don't appear to have entered an auth key! Please enter your auth key:\u001b[0m");

                auth = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(auth)) 
                    File.WriteAllText(AuthFile, auth);
                else 
                    return getAuthToken(true);
            }

            return auth;
        }

        private Assembly getMintAssy()
        {
            Assembly mintAssy = null;
            /*string auth = getAuthToken().Trim();

            //WWW www = new WWW(final_site, null, new Il2CppSystem.Collections.Generic.Dictionary<string, string>());
            WebClient weeeee = new WebClient();
            while (weeeee.IsBusy) Task.WaitAll();
            //string result = www?.text;
            string result = weeeee?.DownloadString(final_site);

            string[] data = Encoding.UTF8.GetString(Convert.FromBase64String(result)).Split('\n');

            int errorCode = 0;
            if (data == null) errorCode = 1;
            if (string.IsNullOrWhiteSpace(result)) errorCode = 2;
            if (!data.Contains(auth)) errorCode = 3;

            if (data != null && !string.IsNullOrWhiteSpace(result) && data.Contains(auth))
            {*/
                using HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", "MintyLoader");
                try
                {
                    Task<HttpResponseMessage> assyRequest = client.GetAsync(modURL);
                    assyRequest.Wait();
                    HttpResponseMessage message = assyRequest.Result;
                    switch (message.StatusCode)
                    {
                        case HttpStatusCode.OK:
                            MelonLogger.Msg("\u001b[0m[\u001b[34mAUTHENTICATOR\u001b[0m] \u001b[36mSuccessfully authenticated and retrieved MintMod assembly!\u001b[0m");
                            Task<byte[]> MintyBytes = message.Content.ReadAsByteArrayAsync();
                            MintyBytes.Wait();
                            mintAssy = Assembly.Load(MintyBytes.Result);
                            break;
                        case HttpStatusCode.InternalServerError:
                            MelonLogger.Error("The DLL on this server was removed, it is probably getting updated, please try again in 30 seconds.");
                            MelonLogger.Error("If you are constantly getting this error, please post in #bug-reports");
                            break;
                        case HttpStatusCode.Forbidden:
                            MelonLogger.Error("\u001b[0m[\u001b[34mHttpResponse\u001b[0m] \u001b[36mUnable to load MintMod! Invalid or missing access token!\u001b[0m");
                            break;
                        default:
                            MelonLogger.Error($"An unknown error has occured! HTTP Status Code: {message.StatusCode}\nUnable to load MintMod!");
                            break;
                    }
                }
                catch (Exception e)
                {
                    MelonLogger.Error("An unknown error occured while attempting to retrieve the MintMod assembly! (Loading fallback)");

                    try
                    {
                        var WebComunication = (HttpWebRequest)WebRequest.CreateHttp(modURL);
                        HttpWebResponse Results = (HttpWebResponse)WebComunication.GetResponse();

                        switch (Results.StatusCode)
                        {
                            case HttpStatusCode.OK:
                                long total = 0;
                                long received = 0;
                                byte[] buffer = new byte[1024];
                                List<byte> realBuffer = new List<byte>();
                                using (Stream input = Results.GetResponseStream())
                                {
                                    total = input.Length;

                                    int size = input.Read(buffer, 0, buffer.Length);
                                    while (size > 0)
                                    {
                                        for (var i = 0; i < size; i++)
                                            realBuffer.Add(buffer[i]);

                                        received += size;

                                        size = input.Read(buffer, 0, buffer.Length);
                                    }
                                }

                                buffer = realBuffer.ToArray();
                                mintAssy = Assembly.Load(buffer);
                                break;
                            case HttpStatusCode.InternalServerError:
                                MelonLogger.Error("The DLL on this server was removed, it is probably getting updated, please try again in 30 seconds.");
                                MelonLogger.Error("If you are constantly getting this error, please post in #bug-reports");
                                break;
                            case HttpStatusCode.Forbidden:
                                MelonLogger.Error("\u001b[0m[\u001b[34mHttpResponse\u001b[0m] \u001b[36mUnable to load MintMod! Invalid or missing access token!\u001b[0m");
                                break;
                            default:
                                MelonLogger.Error($"[FALLBACK] An unknown error has occured! HTTP Status Code: {Results.StatusCode}\nUnable to load MintMod!");
                                break;
                        }
                    }
                    catch (Exception f)
                    {
                        MelonLogger.Error($"First Error:\n{e}");
                        Console.WriteLine("-----------------------------------------------------------------------------------------------");
                        MelonLogger.Error($"[FALLBACK] An unknown error occured while attempting to retrieve the MintMod assembly!\n{f}");
                    }
                }
            /*}
            else
            {
                switch (errorCode)
                {
                    case 1:
                        MelonLogger.Error("data returned null");
                        break;
                    case 2:
                        MelonLogger.Error("result was empty");
                        break;
                    case 3:
                        MelonLogger.Error("data does not have auth");
                        break;
                    default:
                        MelonLogger.Warning("Some other error has occured. MintMod will not load.");
                        break;
                }
            }
            */

            #region Lily's Fix to Bono's edit => with original Lolite server
            // Lily's scuffed shit
            /*
            string url = "http://buttplug.kortyboi.com/";
            var request = (HttpWebRequest)WebRequest.CreateHttp(url);
            request.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;
            request.Headers.Add("lolite", auth);
            HttpWebResponse a = (HttpWebResponse)request.GetResponse();

            switch (a.StatusCode)
            {
                case HttpStatusCode.OK:
                    MelonLogger.Msg("\u001b[0m[\u001b[34mAUTHENTICATOR\u001b[0m] \u001b[36mSuccessfully authenticated and retrieved MintMod assembly!\u001b[0m");
                    long total = 0;
                    long received = 0;
                    byte[] buffer = new byte[1024];
                    List<byte> realBuffer = new List<byte>();
                    using (Stream input = a.GetResponseStream()) {
                        total = input.Length;

                        int size = input.Read(buffer, 0, buffer.Length);
                        while (size > 0)
                        {
                            for (var i = 0; i < size; i++)
                                realBuffer.Add(buffer[i]);

                            received += size;

                            size = input.Read(buffer, 0, buffer.Length);
                        }
                    }

                    buffer = realBuffer.ToArray();
                    mintAssy = Assembly.Load(buffer);
                    break;
                case HttpStatusCode.InternalServerError:
                    MelonLogger.Error("The DLL on this server was removed, it is probably getting updated, please try again in 30 seconds.");
                    MelonLogger.Error("If you are constantly getting this error, please post in #bug-reports");
                    break;
                case HttpStatusCode.Forbidden:
                    MelonLogger.Error("\u001b[0m[\u001b[34mAUTHENTICATOR\u001b[0m] \u001b[36mUnable to load MintMod! Invalid or missing access token!\u001b[0m");
                    break;
                default:
                    MelonLogger.Error($"[FALLBACK] An unknown error has occured! HTTP Status Code: {a.StatusCode}\nUnable to load MintMod!");
                    break;
            }
            */
            #endregion

            #region Bono's First Edit
            // Bono's setup
            /*
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "MintyLoader");
                if (string.IsNullOrWhiteSpace(auth)) { auth = getAuthToken(); Task.Delay(300); }
                client.DefaultRequestHeaders.Add("lolite", auth);

                try
                {
                    Task<HttpResponseMessage> assyRequest = client.GetAsync("http://buttplug.kortyboi.com/");
                    assyRequest.Wait();
                    HttpResponseMessage message = assyRequest.Result;
                    switch (message.StatusCode)
                    {
                        case HttpStatusCode.OK:
                            MelonLogger.Msg("\u001b[0m[\u001b[34mAUTHENTICATOR\u001b[0m] \u001b[36mSuccessfully authenticated and retrieved MintMod assembly!\u001b[0m");
                            Task<byte[]> mintyBytes = message.Content.ReadAsByteArrayAsync();
                            mintyBytes.Wait();
                            mintAssy = Assembly.Load(mintyBytes.Result);
                            break;
                        case HttpStatusCode.InternalServerError:
                            MelonLogger.Error("The DLL on this server was removed, it is probably getting updated, please try again in 30 seconds.");
                            MelonLogger.Error("If you are constantly getting this error, please post in #bug-reports");
                            break;
                        case HttpStatusCode.Forbidden:
                            MelonLogger.Error("\u001b[0m[\u001b[34mAUTHENTICATOR\u001b[0m] \u001b[36mUnable to load MintMod! Invalid or missing access token!\u001b[0m");
                            break;
                        default:
                            MelonLogger.Warning($"Got {message.StatusCode}. Using fallback.");
                            Task<HttpResponseMessage> assyRequest2 = client.GetAsync("https://kortyboi.com/MintMod.dll");
                            assyRequest.Wait();
                            HttpResponseMessage message2 = assyRequest.Result;
                            switch (message2.StatusCode)
                            {
                                case HttpStatusCode.OK:
                                    MelonLogger.Msg("\u001b[0m[\u001b[34mAUTHENTICATOR\u001b[0m] \u001b[36mSuccessfully authenticated and retrieved MintMod assembly!\u001b[0m");
                                    Task<byte[]> mintyBytes2 = message.Content.ReadAsByteArrayAsync();
                                    mintyBytes2.Wait();
                                    mintAssy = Assembly.Load(mintyBytes2.Result);
                                    break;
                                case HttpStatusCode.NotFound:
                                    MelonLogger.Msg(ConsoleColor.Red, "Backup MintMod.dll was not found.");
                                    break;
                                default:
                                    MelonLogger.Error($"[FALLBACK] An unknown error has occured! HTTP Status Code: {message.StatusCode}\nUnable to load MintMod!");
                                    break;
                            }
                            break;
                    }
                }
                catch(Exception e)
                {
                    MelonLogger.Error("An unknown error occured while attempting to retrieve the MintMod assembly!");
                    MelonLogger.Error(e);
                }
            }
            */
            #endregion

            return mintAssy;
        }

        #region CompanionCore pass through

        public override void OnLateUpdate() { if (MintMod != null) MintMod.OnLateUpdate(); }

        public override void OnSceneWasLoaded(int level, string name) { if (MintMod != null) MintMod.OnSceneWasLoaded(level, name); }

        //public override void OnGUI() { if (MintMod != null) MintMod.OnGUI(); }

        //public override void OnPreferencesSaved() { if (MintMod != null) MintMod.OnPreferencesSaved(); }

        public override void OnFixedUpdate() { if (MintMod != null) MintMod.OnFixedUpdate(); }

        public override void OnUpdate() { if (MintMod != null) MintMod.OnUpdate(); }

        public override void OnApplicationQuit()
        {
            if (!hasQuit) {
                hasQuit = true;
                if (MintMod != null) MintMod.OnApplicationQuit();
                MelonLogger.Msg(ConsoleColor.Red, "MintyLoader is stopping...");
            }
        }
        #endregion
    }
}
