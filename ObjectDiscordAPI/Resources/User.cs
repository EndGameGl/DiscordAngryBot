﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.Resources
{
    public class User
    {
        public ulong id { get; set; }
        public string username { get; set; }
        public string discriminator { get; set; }
        public string avatar { get; set; }
        public bool? bot { get; set; }
        public bool? system { get; set; }
        public bool? mfa_enabled { get; set; }
        public string locale { get; set; }
        public bool? verified { get; set; }
        public string email { get; set; }
        public int? flags { get; set; }
        public int? premium_type { get; set; }
        public int? public_flags { get; set; }

        public User() { }
    }
}
