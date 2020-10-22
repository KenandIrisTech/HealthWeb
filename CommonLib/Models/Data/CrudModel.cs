using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CommonLib.Models.Data
{
    public class CrudModel<TEntity> where TEntity : class
    {
        [JsonProperty("action")]
        public string Action { get; set; }
        [JsonProperty("key")]
        public object Key { get; set; }
        [JsonProperty("keyColumn")]
        public string KeyColumn { get; set; }

        [JsonProperty("value")]
        public TEntity Value { get; set; }
        [JsonProperty("added")]
        public List<TEntity> Added { get; set; }
        [JsonProperty("changed")]
        public List<TEntity> Changed { get; set; }
        [JsonProperty("deleted")]
        public List<TEntity> Deleted { get; set; }
        [JsonProperty("params")]
        public CustomParams Params { get; set; }
    }
}
