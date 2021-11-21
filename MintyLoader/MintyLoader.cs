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
        public const string Version = "2.3.0";
        public const string DownloadLink = null;
    }
   
    public class MintyLoader : MelonMod
    {
        public static MintyLoader instance;
        static readonly DirectoryInfo MintDirectory = new DirectoryInfo($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}UserData{Path.DirectorySeparatorChar}MintMod");
        internal MelonMod MintMod;
        internal Assembly Local_mintAssembly;
        static bool hasQuit, isDebug, _localLoadingFailed;
        private string checkedVer, site, final_site, auth_folder, passwords;
        private string[] site_split, splitsite_6;
        private readonly string modURL = "https://mintlily.lgbt/mod/MintMod.dll";

        public override void OnApplicationStart()
        {
            instance = this;
            if (Environment.CommandLine.Contains("--MintyDev")) isDebug = true;
            MelonLogger.Msg(ConsoleColor.Green, "MintyLoader Startup!");
            WebClient checkVer = new WebClient();
            checkedVer = checkVer.DownloadString("https://mintlily.lgbt/mod/loader/version.txt");

            MelonLogger.Msg(ConsoleColor.DarkCyan, $"Loader Build version: {BuildInfo.Version} :: Server pulled: {checkedVer}");

            //RunAndConvert().NoAwait();
            //while (!RunAndConvert().IsCompleted) Task.WaitAll();

            if (isDebug) {
                MelonLogger.Msg(ConsoleColor.DarkRed, $"Final Site: {final_site}");

                try {
                    MelonLogger.Msg(ConsoleColor.Yellow, "Loading Local Mod");
                    Local_mintAssembly = Assembly.LoadFile("MintMod.dll");
                    if (Local_mintAssembly != null) loadModuleCore(Local_mintAssembly);
                    return;
                } catch (Exception e) {
                    _localLoadingFailed = true;
                    MelonLogger.Error($"{e}");
                    ///*
                    if (Local_mintAssembly != null) Local_mintAssembly = null;
                    MelonLogger.Warning("Is not load Local Mod, loading MintMod from the server.");
                    if (!MintDirectory.Exists)
                        MintDirectory.Create();

                    Assembly mintAssembly = getMintAssy();
                    if (mintAssembly != null) loadModuleCore(mintAssembly);
                    //*/
                    return;
                }
            }
            if (checkedVer == BuildInfo.Version && !_localLoadingFailed) {
                MelonLogger.Msg(ConsoleColor.Cyan, "Loader is up to date");
                if (!MintDirectory.Exists)
                    MintDirectory.Create();

                Assembly mintAssembly = getMintAssy();
                if (mintAssembly != null) loadModuleCore(mintAssembly);
            } else MelonLogger.Warning("MintyLoader is not up to date, please update your DLL.");
        }

        private void loadModuleCore(Assembly assy)
        {
            if (assy != null) {
                /*try {
                    if (isDebug)
                        MelonLogger.Msg("Passing to MelonHandler");
                    MelonHandler.LoadFromAssembly(assy);
                    if (isDebug)
                        MelonLogger.Msg("Passed to MelonHandler");
                }
                catch (Exception e) {
                    MelonLogger.Error($"Unable to load MintMod -> If VRChat has been updated, MintMod likely needs to be updated as well.\n{e}");
                }*/


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
                            MelonLogger.Msg("\u001b[0m[\u001b[34mAUTHENTICATOR\u001b[0m] \u001b[36mSuccessfully retrieved MintMod assembly!\u001b[0m");
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
