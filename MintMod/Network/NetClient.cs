using MintyLoader;
using WebSocketSharp;

namespace MintMod.Network
{
    public class NetClient
    {
        public static void Initialize()
        {
            MintyNetClient._Client.OnMessage += ClientOnOnMessage;
            MintyNetClient.ModLoaded = true;
        }

        private static void ClientOnOnMessage(object sender, MessageEventArgs e)
        {
            
        }
    }
}