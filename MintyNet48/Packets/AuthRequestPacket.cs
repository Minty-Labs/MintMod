using MintyNet48.Core;
using Newtonsoft.Json;

namespace MintyNet48.Packets
{
    public class AuthRequestPacket
    {
        [JsonProperty("PacketType")]public PacketType PacketType { get; set; }
        [JsonProperty("HWID")]public string HWID { get; set; }

        public AuthRequestPacket()
        {
            this.PacketType = PacketType.AUTH_REQUEST;
        }
    }
}