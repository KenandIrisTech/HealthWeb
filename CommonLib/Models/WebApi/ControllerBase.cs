using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net;
using System.Net.Http;
using CommonLib.Models.Data;
using Syncfusion.EJ2.Base;

namespace CommonLib.Models.WebApi
{
    public class ControllerBase<TEntity> : ApiController where TEntity : class, new()
    {
        protected WebApiResponse<TEntity> WebApiResponse = new WebApiResponse<TEntity>();

        private readonly IRepository<TEntity> repository;

        public ControllerBase(IRepository<TEntity> repository)
        {
            this.repository = repository;
        }

        [ActionName("GetAll")]
        [HttpPost]
        public virtual HttpResponseMessage GetAll([FromBody]DataManagerRequest options)
        {
            WebApiResponse = repository.GetAll(options);
            if (options.RequiresCounts)
            {
                WebApiResponse.EffectCount = repository.Count(options);
                return Request.CreateResponse<WebApiResponse<TEntity>>(HttpStatusCode.OK, WebApiResponse);
            }
            else
            {
                return Request.CreateResponse<WebApiResponse<TEntity>>(HttpStatusCode.OK, WebApiResponse);
            }
        }
    }
}
