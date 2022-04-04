using MintyServer.Logger;
using MintyServer.Utils;
using WebSocketSharp.Server;

namespace MintyServer.SocketServer;

public class Server
{
    public static WebSocketServer SocketServer { get; set; }
    public static MintyUserManager UserManager { get; set; }
    public static bool isListening = false;
    public static void Start()
    {
        UserManager = new MintyUserManager();
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
                case "stop":
                    isListening = false;
                    SocketServer.Stop();
                    break;
                case "clients":
                    MintyLogger.info("Clients: " + SocketServer.WebSocketServices["/MintyNet"].Sessions.Count);
                    SocketServer.WebSocketServices["/MintyNet"].Sessions.Sessions.ToList().ForEach(s=> MintyLogger.info(s.Context.UserEndPoint.ToString() + ":" + s.ConnectionState.ToString()));
                    break;
                default:
                    MintyLogger.info("Unknown command");
                    break;
            }
        }
    }
}