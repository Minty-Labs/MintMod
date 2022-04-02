using System;
using MintyNet48.Packets;
using Newtonsoft.Json;
using WebSocketSharp;

namespace MintyLoader
{
    public class MintyNetClient
    {
        private static WebSocket _Client { get; set; }
        public static void Connect()
        {
            _Client = new WebSocket("ws://localhost:8088/MintyNet");
            _Client.OnOpen += ClientOnOnOpen;
            _Client.OnMessage += ClientOnOnMessage;
            _Client.OnError += ClientOnOnError;
            _Client.OnClose += ClientOnOnClose;
            _Client.Connect();
        }

        public static void Disconnect()
        {
            _Client.Close();
        }

        private static void ClientOnOnClose(object sender, CloseEventArgs e)
        {
            MintyLoader.InternalLogger.Msg("MintyNetClient: Connection Closed");
            
        }

        private static void ClientOnOnError(object sender, ErrorEventArgs e)
        {
            MintyLoader.InternalLogger.Msg("MintyNetClient: Connection errrored");
        }

        private static void ClientOnOnMessage(object sender, MessageEventArgs e)
        {
            MintyLoader.InternalLogger.Msg("MintyNetClient: Message Received " + e.RawData);
        }

        private static void ClientOnOnOpen(object sender, EventArgs e)
        {
            AuthRequestPacket authRequestPacket = new AuthRequestPacket();
            authRequestPacket.HWID = "testtesttesttestttestttesttest";
            _Client.Send(JsonConvert.SerializeObject(authRequestPacket));
        }
    }
}