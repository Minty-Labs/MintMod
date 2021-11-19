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

namespace MintMod.Utils {
    class AssemblyLoader {
        internal static Assembly GetAssenbly(string URL, string modName) {
            Assembly _assenbly = null;
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "MintyLoader");
            try {
                Task<HttpResponseMessage> assyRequest = client.GetAsync(URL);
                assyRequest.Wait();
                HttpResponseMessage message = assyRequest.Result;
                switch (message.StatusCode) {
                    case HttpStatusCode.OK:
                        MelonLogger.Msg(ConsoleColor.Green, $"Successfully grabbed {modName}");
                        Task<byte[]> _bytes = message.Content.ReadAsByteArrayAsync();
                        _bytes.Wait();
                        _assenbly = Assembly.Load(_bytes.Result);
                        break;
                    case HttpStatusCode.InternalServerError:
                        MelonLogger.Error("The DLL on this server was removed, it is probably getting updated, please try again in 30 seconds.");
                        MelonLogger.Error("If you are constantly getting this error, please post in #bug-reports");
                        break;
                    default:
                        MelonLogger.Error($"An unknown error has occured! HTTP Status Code: {message.StatusCode}\nUnable to load {modName}!");
                        break;
                }
            } catch (Exception e) {
                MelonLogger.Error($"An unknown error occured while attempting to retrieve the {modName} assembly! (Loading fallback)");

                try {
                    var WebComunication = (HttpWebRequest)WebRequest.CreateHttp(URL);
                    HttpWebResponse Results = (HttpWebResponse)WebComunication.GetResponse();

                    switch (Results.StatusCode) {
                        case HttpStatusCode.OK:
                            long total = 0;
                            long received = 0;
                            byte[] buffer = new byte[1024];
                            List<byte> realBuffer = new List<byte>();
                            using (Stream input = Results.GetResponseStream()) {
                                total = input.Length;

                                int size = input.Read(buffer, 0, buffer.Length);
                                while (size > 0) {
                                    for (var i = 0; i < size; i++)
                                        realBuffer.Add(buffer[i]);

                                    received += size;

                                    size = input.Read(buffer, 0, buffer.Length);
                                }
                            }

                            buffer = realBuffer.ToArray();
                            _assenbly = Assembly.Load(buffer);
                            MelonLogger.Msg(ConsoleColor.Green, $"Successfully grabbed {modName}");
                            break;
                        case HttpStatusCode.InternalServerError:
                            MelonLogger.Error("The DLL on this server was removed, it is probably getting updated, please try again in 30 seconds.");
                            MelonLogger.Error("If you are constantly getting this error, please post in #bug-reports");
                            break;
                        default:
                            MelonLogger.Error($"An unknown error has occured! HTTP Status Code: {Results.StatusCode}\nUnable to load {modName}!");
                            break;
                    }
                } catch (Exception f) {
                    MelonLogger.Error($"First Error:\n{e}");
                    Console.WriteLine("-----------------------------------------------------------------------------------------------");
                    MelonLogger.Error($"[FALLBACK] An unknown error occured while attempting to retrieve the {modName} assembly!\n{f}");
                }
            }

            return _assenbly;
        }
    }
}
