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
        public string Name;
        //public string DiscordID { get; }
        public string UserID;
        //public string VoucherName { get; }
        public string[] AltAccounts;
        public bool isBanned;
    }
}
