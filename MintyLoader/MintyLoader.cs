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
using Pastel;
using UnityEngine;
using UnityEngine.Networking;

namespace MintyLoader
{
    public static class BuildInfo
    {
        public const string Name = "MintyLoader";
        public const string Author = "Lily & DDAkebono";
        public const string Company = "LilyMod";
        public const string Version = "2.4.1";
        public const string DownloadLink = null;
    }
   
    public class MintyLoader : MelonMod
    {
        public static MintyLoader instance;
        static readonly DirectoryInfo MintDirectory = new DirectoryInfo($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}UserData{Path.DirectorySeparatorChar}MintMod");
        internal MelonMod MintMod;
        internal Assembly Local_mintAssembly;
        internal static bool hasQuit, isDebug, _localLoadingFailed;
        private string checkedVer, final_site, MintyColor = "9fffe3";
        private readonly string modURL = "https://mintlily.lgbt/mod/MintMod.dll";
        private static readonly MelonLogger.Instance InternalLogger = new MelonLogger.Instance("MintyLoader", ConsoleColor.Red);

        public override void OnApplicationStart()
        {
            instance = this;
            if (Environment.CommandLine.Contains("--MintyDev")) isDebug = true;
            InternalLogger.Msg(ConsoleColor.Green, "Minty".Pastel(MintyColor) + "Loader is starting up!");
            MonkeKiller.BlacklistedModCheck();
            WebClient checkVer = new WebClient();
            checkedVer = checkVer.DownloadString("https://mintlily.lgbt/mod/loader/version.txt");

            InternalLogger.Msg($"Loader Build version: ".Pastel("008B8B") + BuildInfo.Version.Pastel(MintyColor) + " :: Server pulled: ".Pastel("008B8B") + checkedVer.Pastel(MintyColor));

            if (isDebug) {
                try {
                    InternalLogger.Msg(ConsoleColor.Yellow, "Loading Local Mod");
                    if (File.Exists("MintMod.dll")) {
                        Local_mintAssembly = Assembly.LoadFile("MintMod.dll");
                        if (Local_mintAssembly != null) loadModuleCore(Local_mintAssembly);
                    }
                    else {
                        var mintAssembly = getMintAssy();
                        if (mintAssembly != null) loadModuleCore(mintAssembly);
                    }
                    return;
                } catch (Exception e) {
                    _localLoadingFailed = true;
                    InternalLogger.Error($"{e}");
                    if (Local_mintAssembly != null) Local_mintAssembly = null;
                    InternalLogger.Warning("Can not load Local Mod, loading MintMod from the server.");
                    if (!MintDirectory.Exists)
                        MintDirectory.Create();

                    Assembly mintAssembly = getMintAssy();
                    if (mintAssembly != null) loadModuleCore(mintAssembly);
                    return;
                }
            }
            if (checkedVer == BuildInfo.Version && !_localLoadingFailed) {
                InternalLogger.Msg(ConsoleColor.Cyan, "Loader is up to date");
                if (!MintDirectory.Exists)
                    MintDirectory.Create();

                Assembly mintAssembly = getMintAssy();
                if (mintAssembly != null) loadModuleCore(mintAssembly);
            } else InternalLogger.Warning("MintyLoader is not up to date, please update your DLL.");
        }

        private void loadModuleCore(Assembly assy)
        {
            if (assy != null) {
                foreach (Type componentType in ((IEnumerable<Type>)assy.GetTypes()).Where(t => t.IsSubclassOf(typeof(MelonMod)))) {
                    try {
                        MelonMod coreLoad = Activator.CreateInstance(componentType) as MelonMod;
                        coreLoad.OnApplicationStart();
                        MintMod = coreLoad;
                    } catch (TypeLoadException e) {
                        InternalLogger.Error(e.Message);
                        InternalLogger.Error("Unable to load MintMod -> If VRChat has been updated, MintMod likely needs to be updated as well.");
                    }
                }
            }
        }

        private Assembly getMintAssy()
        {
            Assembly mintAssy = null;
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
                            InternalLogger.Msg("[" + "DownloadManager".Pastel("1A6DF6") + "] Successfully retrieved MintMod assembly!");
                            Task<byte[]> MintyBytes = message.Content.ReadAsByteArrayAsync();
                            MintyBytes.Wait();
                            mintAssy = Assembly.Load(MintyBytes.Result);
                            break;
                        case HttpStatusCode.InternalServerError:
                            InternalLogger.Error("The DLL on this server was removed, it is probably getting updated, please try again in 30 seconds.");
                            InternalLogger.Error("If you are constantly getting this error, please post in #bug-reports");
                            break;
                        case HttpStatusCode.Forbidden:
                            InternalLogger.Error("[" + "DownloadManager".Pastel("1A6DF6") + "] Unable to load MintMod! Invalid or missing access token!");
                            break;
                        default:
                            InternalLogger.Warning($"An unknown error has occured! HTTP Status Code: {message.StatusCode} " + "Unable to load MintMod!".Pastel("ff0000"));
                            break;
                    }
                }
                catch (Exception e)
                {
                    InternalLogger.Error("An unknown error occured while attempting to retrieve the MintMod assembly! (Loading fallback)");

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
                                InternalLogger.Error("The DLL on this server was removed, it is probably getting updated, please try again in 30 seconds.");
                                InternalLogger.Error("If you are constantly getting this error, please post in #bug-reports");
                                break;
                            case HttpStatusCode.Forbidden:
                                InternalLogger.Error("[" + "DownloadManager".Pastel("1A6DF6") + "] Unable to load MintMod! Invalid or missing access token!");
                                break;
                            default:
                                InternalLogger.Error($"[FALLBACK] An unknown error has occured! HTTP Status Code: {Results.StatusCode}\nUnable to load MintMod!");
                                break;
                        }
                    }
                    catch (Exception f)
                    {
                        InternalLogger.Error($"First Error:\n{e}");
                        Console.WriteLine("-----------------------------------------------------------------------------------------------");
                        InternalLogger.Error($"[FALLBACK] An unknown error occured while attempting to retrieve the MintMod assembly!\n{f}");
                    }
                }

            return mintAssy;
        }

        #region CompanionCore pass through

        public override void OnLateUpdate() { if (MintMod != null) MintMod.OnLateUpdate(); }

        public override void OnSceneWasLoaded(int level, string name) { if (MintMod != null) MintMod.OnSceneWasLoaded(level, name); }

        public override void OnPreferencesSaved() { if (MintMod != null) MintMod.OnPreferencesSaved(); }

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

        public override void OnGUI() { if (MintMod != null) MintMod.OnGUI(); }

        #endregion
    }
}
