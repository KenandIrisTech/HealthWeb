using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace iHealth2C.Models.Authentication.Providers
{
    [JsonObject]
    public class GoogleUser
    {
        public string Id { get; set; }
        public string Sub { get; set; }
        public string Name { get; set; }
        public string Given_Name { get; set; }
        public string Family_Name { get; set; }
        public string Email { get; set; }
        public string Email_Verified { get; set; }
        public string Picture { get; set; }
        public string Locale { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }
    }
}
