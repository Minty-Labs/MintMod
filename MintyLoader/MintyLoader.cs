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

namespace MintyLoader {
    public static class BuildInfo {
        public const string Name = "MintyLoader";
        public const string Author = "Lily & DDAkebono";
        public const string Company = "LilyMod";
        public const string Version = "2.5.0";
        public const string DownloadLink = "https://mintlily.lgbt/mod/loader/MintyLoader.dll";
    }
   
    public class MintyLoader : MelonMod {
        public static MintyLoader Instance;
        static readonly DirectoryInfo MintDirectory = new DirectoryInfo($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}UserData{Path.DirectorySeparatorChar}MintMod");
        internal MelonMod MintMod;
        private Assembly _localMintAssembly;
        internal static bool hasQuit, isDebug, localLoadingFailed;
        private readonly string modURL = "https://mintlily.lgbt/mod/MintMod.dll";
        internal static readonly MelonLogger.Instance InternalLogger = new MelonLogger.Instance("MintyLoader", ConsoleColor.Red);

        public override void OnApplicationStart() {
            Instance = this;
            InternalLogger.Msg("Minty".Pastel("9fffe3") + "Loader is starting up!");
            
            isDebug = Environment.CommandLine.Contains("--MintyDev"); // Check if running as Debug
            
            Task.Run(async () => await UpdateManager.CheckVersion()); // Check Loader Version and update if needed
            
            MonkeKiller.BlacklistedModCheck(); // Check if running blacklisted mod(s)

            if (isDebug) { // true, run local DLL
                try {
                    InternalLogger.Msg(ConsoleColor.Yellow, "Loading Local Mod");
                    if (File.Exists("MintMod.dll")) {
                        _localMintAssembly = Assembly.LoadFile("MintMod.dll");
                        if (_localMintAssembly != null) loadModuleCore(_localMintAssembly);
                    }
                    else { // If No Local DLL, load DLL from webhost
                        var mintAssembly = getMintAssy();
                        if (mintAssembly != null) loadModuleCore(mintAssembly);
                    }
                    return;
                } catch (Exception e) { // If Error, load DLL from webhost
                    localLoadingFailed = true;
                    InternalLogger.Error($"{e}");
                    _localMintAssembly = null;
                    InternalLogger.Warning("Can not load Local Mod, loading MintMod from the server.");
                    if (!MintDirectory.Exists)
                        MintDirectory.Create();

                    var mintAssembly = getMintAssy();
                    if (mintAssembly != null) loadModuleCore(mintAssembly);
                    return;
                }
            }
            if (UpdateManager.LoaderIsUpToDate && !localLoadingFailed) { // load DLL from webhost
                InternalLogger.Msg(ConsoleColor.Cyan, "Loader is up to date");
                if (!MintDirectory.Exists)
                    MintDirectory.Create();

                var mintAssembly = getMintAssy();
                if (mintAssembly != null) loadModuleCore(mintAssembly);
            } else InternalLogger.Warning("MintyLoader is not up to date, please update your DLL.");
        }

        private void loadModuleCore(Assembly aslm) {
            if (aslm != null) {
                foreach (var componentType in aslm.GetTypes().Where(t => t.IsSubclassOf(typeof(MelonMod)))) {
                    try {
                        var coreLoad = Activator.CreateInstance(componentType) as MelonMod;
                        coreLoad?.OnApplicationStart();
                        MintMod = coreLoad;
                    } catch (TypeLoadException e) {
                        InternalLogger.Error(e.Message);
                        InternalLogger.Error("Unable to load MintMod -> If VRChat has been updated, MintMod likely needs to be updated as well.");
                    }
                }
            }
        }

        private Assembly getMintAssy() {
            Assembly mintAslm = null;
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", BuildInfo.Name);
                try {
                    var request = client.GetAsync(modURL);
                    request.Wait();
                    var message = request.Result;
                    switch (message.StatusCode) {
                        case HttpStatusCode.OK:
                            InternalLogger.Msg("[" + "DownloadManager".Pastel("1A6DF6") + "] Successfully retrieved MintMod assembly!");
                            var mintyBytes = message.Content.ReadAsByteArrayAsync();
                            mintyBytes.Wait();
                            mintAslm = Assembly.Load(mintyBytes.Result);
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
                catch (Exception e) {
                    InternalLogger.Error("An unknown error occured while attempting to retrieve the MintMod assembly! (Loading fallback)");

                    try {
                        var webComunication = WebRequest.CreateHttp(modURL);
                        var results = (HttpWebResponse)webComunication.GetResponse();

                        switch (results.StatusCode) {
                            case HttpStatusCode.OK:
                                long total = 0;
                                long received = 0;
                                var buffer = new byte[1024];
                                var realBuffer = new List<byte>();
                                using (var input = results.GetResponseStream()) {
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
                                mintAslm = Assembly.Load(buffer);
                                break;
                            case HttpStatusCode.InternalServerError:
                                InternalLogger.Error("The DLL on this server was removed, it is probably getting updated, please try again in 30 seconds.");
                                InternalLogger.Error("If you are constantly getting this error, please post in #bug-reports");
                                break;
                            case HttpStatusCode.Forbidden:
                                InternalLogger.Error("[" + "DownloadManager".Pastel("1A6DF6") + "] Unable to load MintMod! Invalid or missing access token!");
                                break;
                            default:
                                InternalLogger.Error($"[FALLBACK] An unknown error has occured! HTTP Status Code: {results.StatusCode}\nUnable to load MintMod!");
                                break;
                        }
                    }
                    catch (Exception f) {
                        InternalLogger.Error($"First Error:\n{e}");
                        Console.WriteLine("-----------------------------------------------------------------------------------------------");
                        InternalLogger.Error($"[FALLBACK] An unknown error occured while attempting to retrieve the MintMod assembly!\n{f}");
                    }
                }

            return mintAslm;
        }

        #region CompanionCore pass through

        public override void OnLateUpdate() => MintMod?.OnLateUpdate();

        public override void OnSceneWasLoaded(int level, string name) => MintMod?.OnSceneWasLoaded(level, name);

        public override void OnPreferencesSaved() => MintMod?.OnPreferencesSaved();

        public override void OnFixedUpdate() => MintMod?.OnFixedUpdate();

        public override void OnUpdate() => MintMod?.OnUpdate();

        public override void OnApplicationQuit() {
            if (!hasQuit) {
                hasQuit = true;
                MintMod?.OnApplicationQuit();
                InternalLogger.Msg(ConsoleColor.Red, "MintyLoader is stopping...");
            }
        }

        public override void OnGUI() => MintMod?.OnGUI();

        #endregion
    }
}
