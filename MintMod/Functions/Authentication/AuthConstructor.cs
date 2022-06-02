using Newtonsoft.Json;

namespace MintMod.Functions.Authentication {
    public class MintyUser {
        [JsonProperty("Name")]
        public string Name;
        
        // public string DiscordID { get; }
        
        [JsonProperty("UserID")]
        public string UserId;
        
        [JsonProperty("IsBanned")]
        public bool IsBanned;
        
        // public string VoucherName { get; }
        
        [JsonProperty("AltAccounts")]
        public string[] AltAccounts;
    }
}
