using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MintyLoader;
using Newtonsoft.Json;
using VRC.Core;

namespace MintMod.Functions.Authentication {
    public class MintyUser {
        public string Name { get; }
        //public string DiscordID { get; }
        public string UserID { get; }
        //public string VoucherName { get; }
        public string[] AltAccounts { get; }
        public bool isBanned { get; }
    }
}
