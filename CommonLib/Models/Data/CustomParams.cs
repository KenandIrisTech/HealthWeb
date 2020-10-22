using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CommonLib.Models.Data
{
    public class CustomParams
    {
        [JsonProperty("Key")]
        public int Key { get; set; }
    }
}
