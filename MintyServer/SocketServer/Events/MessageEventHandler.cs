

using MintyNet48.Core;
using MintyNet48.Packets;
using MintyServer.Logger;
using Newtonsoft.Json;

namespace MintyServer.SocketServer.Events;

public class MessageEventHandler
{
    public static void ProcessPacket(String json)
    {
        var request = JsonConvert.DeserializeObject<dynamic>(json);
        var type = (PacketType)request.PacketType;

        switch (type)
        {
            case(PacketType.AUTH_REQUEST):
                ProcessAuthPacket(json);
                break;
            
        }
        
    }

    public static void ProcessAuthPacket(String json)
    {
        var data = JsonConvert.DeserializeObject<AuthRequestPacket>(json);
        MintyLogger.info("User connected with hwid: " + data.HWID);
    }

}