using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using System.Reflection;
using MelonLoader;
using System.IO;
using Pastel;

namespace MintyLoader {
    internal class LoadManager {
        internal static MelonMod MintMod;
        private static readonly string modURL = "https://mintlily.lgbt/mod/MintMod.dll";
        private static Assembly _localMintAssembly;
        private static bool localLoadingFailed;

        internal static void LoadLocal() {
            try {
                MintyLoader.InternalLogger.Msg(ConsoleColor.Yellow, "Loading Local Mod");
                if (File.Exists("MintMod.dll")) {
                    _localMintAssembly = Assembly.LoadFile("MintMod.dll");
                    if (_localMintAssembly != null) loadModuleCore(_localMintAssembly);
                }
                else { // If No Local DLL, load DLL from webhost
                    var mintAssembly = getMintAssy();
                    if (mintAssembly != null) loadModuleCore(mintAssembly);
                }
            } catch (Exception e) { // If Error, load DLL from webhost
                localLoadingFailed = true;
                MintyLoader.InternalLogger.Error(e);
                _localMintAssembly = null;
                MintyLoader.InternalLogger.Warning("Can not load Local Mod, loading MintMod from the server.");
                LoadWebhost();
            }
        }
        
        internal static void LoadWebhost() {
            if (!localLoadingFailed) { // load DLL from webhost
                MintyLoader.InternalLogger.Msg(ConsoleColor.Cyan, "Loader is up to date");
                if (!MintyLoader.MintDirectory.Exists)
                    MintyLoader.MintDirectory.Create();

                var mintAssembly = getMintAssy();
                if (mintAssembly != null) loadModuleCore(mintAssembly);
            } else MintyLoader.InternalLogger.Warning("MintyLoader is not up to date, please update your DLL.");
        }
        
        private static void loadModuleCore(Assembly aslm) {
            if (aslm != null) {
                foreach (var componentType in aslm.GetTypes().Where(t => t.IsSubclassOf(typeof(MelonMod)))) {
                    try {
                        var coreLoad = Activator.CreateInstance(componentType) as MelonMod;
                        coreLoad?.OnApplicationStart();
                        MintMod = coreLoad;
                    } catch (TypeLoadException e) {
                        MintyLoader.InternalLogger.Error(e.Message);
                        MintyLoader.InternalLogger.Error("Unable to load MintMod -> If VRChat has been updated, MintMod likely needs to be updated as well.");
                    }
                }
            }
        }

        private static Assembly getMintAssy() {
            Assembly mintAslm = null;
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", BuildInfo.Name);
                try {
                    var request = client.GetAsync(modURL);
                    request.Wait();
                    var message = request.Result;
                    switch (message.StatusCode) {
                        case HttpStatusCode.OK:
                            MintyLoader.InternalLogger.Msg("[" + "DownloadManager".Pastel("1A6DF6") + "] Successfully retrieved MintMod assembly!");
                            var mintyBytes = message.Content.ReadAsByteArrayAsync();
                            mintyBytes.Wait();
                            mintAslm = Assembly.Load(mintyBytes.Result);
                            break;
                        case HttpStatusCode.InternalServerError:
                            MintyLoader.InternalLogger.Error("The DLL on this server was removed, it is probably getting updated, please try again in 30 seconds.");
                            MintyLoader.InternalLogger.Error("If you are constantly getting this error, please post in #bug-reports");
                            break;
                        case HttpStatusCode.Forbidden:
                            MintyLoader.InternalLogger.Error("[" + "DownloadManager".Pastel("1A6DF6") + "] Unable to load MintMod! Invalid or missing access token!");
                            break;
                        default:
                            MintyLoader.InternalLogger.Warning($"An unknown error has occured! HTTP Status Code: {message.StatusCode} " + "Unable to load MintMod!".Pastel("ff0000"));
                            break;
                    }
                }
                catch (Exception e) {
                    MintyLoader.InternalLogger.Error("An unknown error occured while attempting to retrieve the MintMod assembly! (Loading fallback)");

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
                                MintyLoader.InternalLogger.Error("The DLL on this server was removed, it is probably getting updated, please try again in 30 seconds.");
                                MintyLoader.InternalLogger.Error("If you are constantly getting this error, please post in #bug-reports");
                                break;
                            case HttpStatusCode.Forbidden:
                                MintyLoader.InternalLogger.Error("[" + "DownloadManager".Pastel("1A6DF6") + "] Unable to load MintMod! Invalid or missing access token!");
                                break;
                            default:
                                MintyLoader.InternalLogger.Error($"[FALLBACK] An unknown error has occured! HTTP Status Code: {results.StatusCode}\nUnable to load MintMod!");
                                break;
                        }
                    }
                    catch (Exception f) {
                        MintyLoader.InternalLogger.Error($"First Error:\n{e}");
                        Console.WriteLine("-----------------------------------------------------------------------------------------------");
                        MintyLoader.InternalLogger.Error($"[FALLBACK] An unknown error occured while attempting to retrieve the MintMod assembly!\n{f}");
                    }
                }

            return mintAslm;
        }
    }
}