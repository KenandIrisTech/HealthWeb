using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CommonLib.Models.Data
{
    public class WebApiResponse<TEntity>
    {
        public WebApiResponse()
        {
            ReturnMessage = "";
            Success = true;
        }
        [JsonProperty("returnCode")]
        public int ReturnCode { get; set; }
        [JsonProperty("count")]
        public int EffectCount { get; set; }

        [JsonProperty("entityName")]
        public string EntityName { get; set; }
        [JsonProperty("fieldName")]
        public string FieldName { get; set; }

        [JsonProperty("returnMessage")]
        public string ReturnMessage { get; set; }
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("result")]
        public IEnumerable<TEntity> Results { get; set; }
        [JsonProperty("entity")]
        public TEntity Result { get; set; }
        [JsonProperty("errorId")]
        public int ErrorId { get; set; }
    }
}
