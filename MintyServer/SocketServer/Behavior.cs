using MintyServer.Logger;
using MintyServer.SocketServer.Events;
using WebSocketSharp;
using WebSocketSharp.Server;
using ErrorEventArgs = WebSocketSharp.ErrorEventArgs;

namespace MintyServer.SocketServer;

public class Behavior : WebSocketBehavior
{
    protected override void OnOpen()
    {
        MintyLogger.info("User Connected to Server with address " + Context.UserEndPoint.Address);
    }

    protected override void OnMessage(MessageEventArgs e)
    {
        MessageEventHandler.ProcessPacket(e.Data);
    }

    protected override void OnClose(CloseEventArgs e)
    {
        
    }

    protected override void OnError(ErrorEventArgs e)
    {
        
    }
}