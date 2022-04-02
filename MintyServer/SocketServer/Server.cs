using MintyServer.Logger;
using WebSocketSharp.Server;

namespace MintyServer.SocketServer;

public class Server
{
    public static WebSocketServer SocketServer { get; set; }
    public static bool isListening = false;
    public static void Start()
    {
        SocketServer = new WebSocketServer(8088);
        SocketServer.AddWebSocketService<Behavior>("/MintyNet");
        SocketServer.Start();
        isListening = true;
        MintyLogger.info("Listening on port 8088");
        while (isListening)
        {
            var cmd = Console.ReadLine();
            switch (cmd.ToLower())
            {
                
            }
        }
    }
}