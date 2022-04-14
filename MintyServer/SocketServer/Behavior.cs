<<<<<<< HEAD
﻿using MintyNet48.Core;
using MintyNet48.Packets;
using MintyServer.Logger;
using MintyServer.Utils;
using Newtonsoft.Json;
=======
﻿using MintyServer.Logger;
using MintyServer.SocketServer.Events;
>>>>>>> 9c372e52e6dc9943620e04e459ae7ec1479f3a42
using WebSocketSharp;
using WebSocketSharp.Server;
using ErrorEventArgs = WebSocketSharp.ErrorEventArgs;

namespace MintyServer.SocketServer;

public class Behavior : WebSocketBehavior
{
    protected override void OnOpen()
    {
        MintyLogger.info("User Connected to Server with address " + Context.UserEndPoint.Address);
<<<<<<< HEAD
        Server.UserManager.Connections.Add(new MintyConnection()
        {
            Session = Context
        });
=======
>>>>>>> 9c372e52e6dc9943620e04e459ae7ec1479f3a42
    }

    protected override void OnMessage(MessageEventArgs e)
    {
<<<<<<< HEAD
        var request = JsonConvert.DeserializeObject<dynamic>(e.Data);
        var type = (PacketType)request.PacketType;

        switch (type)
        {
            case(PacketType.AUTH_REQUEST):
                ProcessAuthPacket(e.Data, Context.UserEndPoint.Address.ToString());
                break;
            
        }
        
    }

    public void ProcessAuthPacket(String json, String ip)
    {
        var data = JsonConvert.DeserializeObject<AuthRequestPacket>(json);
        MintyLogger.info("User connected with hwid: " + data.HWID);
        var ipa = ip;
        var branch = data.Branch;
        var hwid = data.HWID;
        bool isauthorized = false;
        
        
        if (isauthorized)
        {
            var res = new AuthResponsePacket()
            {
                Assembly = File.ReadAllBytes($"Modules/{branch}/MintMod.dll")
            };
            Send(JsonConvert.SerializeObject(res));
        }
    }
    
=======
        MessageEventHandler.ProcessPacket(e.Data);
    }

>>>>>>> 9c372e52e6dc9943620e04e459ae7ec1479f3a42
    protected override void OnClose(CloseEventArgs e)
    {
        
    }

    protected override void OnError(ErrorEventArgs e)
    {
        
    }
}