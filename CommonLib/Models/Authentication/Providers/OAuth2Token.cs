using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Models.Authentication.Providers
{
    [JsonObject]
    public class OAuth2Token
    {
        [JsonProperty("authuser")]
        public string AuthUser { get; set; }
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("prompt")]
        public string Prompt { get; set; }
        [JsonProperty("scope")]
        public string Scope { get; set; }
        [JsonProperty("state")]
        public string State { get; set; }
    }

    
}
