using System.Collections;
using MelonLoader;
using MintyLoader;
using VRC.Core;
using WebSocketSharp;
using MintyNet48.Packets;
using MintyNet48.Core;

namespace MintyHoneyPot
{
    public class MintyHoneyPot : MelonMod
    {
        private static MelonLogger.Instance Logger;
        public override void OnApplicationStart()
        {
            Logger = new MelonLogger.Instance("MintyHoneyPot");
            MintyNetClient._Client.OnMessage += ClientOnOnMessage;
            MintyNetClient.ModLoaded = true;
            MelonCoroutines.Start(WaitForApiUser());
            
        }

        public IEnumerator WaitForApiUser()
        {
            while (APIUser.CurrentUser == null && !APIUser.IsLoggedIn) yield return null;
            var userid = APIUser.CurrentUser.id;
            var username = APIUser.CurrentUser.username;
            var packet = new AuthMessagePacket()
            {
                HWID = null,
                Name = username,
                UserID = userid,
            };

        }

        public static void SendAuthPacket(string id , string username)
        {
            
        }
        private void ClientOnOnMessage(object sender, MessageEventArgs e)
        {
            
        }
    }
}