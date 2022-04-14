using MintyServer.Logger;
<<<<<<< HEAD
using MintyServer.Utils;
=======
>>>>>>> 9c372e52e6dc9943620e04e459ae7ec1479f3a42
using WebSocketSharp.Server;

namespace MintyServer.SocketServer;

public class Server
{
    public static WebSocketServer SocketServer { get; set; }
<<<<<<< HEAD
    public static MintyUserManager UserManager { get; set; }
    public static bool isListening = false;
    public static void Start()
    {
        UserManager = new MintyUserManager();
=======
    public static bool isListening = false;
    public static void Start()
    {
>>>>>>> 9c372e52e6dc9943620e04e459ae7ec1479f3a42
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
<<<<<<< HEAD
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
=======
                
>>>>>>> 9c372e52e6dc9943620e04e459ae7ec1479f3a42
            }
        }
    }
}