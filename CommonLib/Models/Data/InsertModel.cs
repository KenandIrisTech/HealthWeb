using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CommonLib.Models.Data
{
    public class InsertModel<TEntity> where TEntity : class
    {
        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("value")]
        public TEntity Value { get; set; }
    }
}
