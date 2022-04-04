using WebSocketSharp.Net.WebSockets;
using WebSocketSharp.Server;

namespace MintyServer.Utils;

public class MintyUserManager
{
    public List<MintyConnection?> Connections { get; set; }
    
    public MintyUserManager()
    {
        Connections = new List<MintyConnection?>();
    }

    public MintyConnection? GetConnectionFromSession(WebSocketContext session)
    {
        WebSocketContext sesh = null;

        return sesh == null ? null : Connections.Find(m => m.Session.Equals(sesh));
    }
        
}

public class MintyConnection
{
    public string User;
    public string IsAdmin;
    public WebSocketContext Session;
}