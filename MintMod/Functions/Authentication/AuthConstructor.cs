using JetBrains.Annotations;
using Newtonsoft.Json;

namespace MintMod.Functions.Authentication {
    public class MintyUser {
        [JsonProperty("Name")]
        public string Name;
        
        [JsonProperty("UserID")]
        public string UserId;
        
        [JsonProperty("IsBanned")]
        public bool IsBanned;
        
        [JsonProperty("BanReason")]
        public string BanReason;
        
        [JsonProperty("AltAccounts")]
        public string[] AltAccounts;
        
        [JsonProperty("SpecialPermission")]
        public bool SpecialPermission;
    }
}
