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
    public class ApiControllerBase<TEntity> : ApiController where TEntity : class, new()
    {
        protected WebApiResponse<TEntity> webapiResponse = new WebApiResponse<TEntity>();
        protected IRepository<TEntity> Repository;

        [ActionName("GetAll")]
        [HttpPost]
        public virtual HttpResponseMessage GetAll([FromBody]DataManagerRequest options)
        {
            webapiResponse = Repository.GetAll(options);
            if (options.RequiresCounts)
            {
                webapiResponse.EffectCount = Repository.Count(options);
                return Request.CreateResponse<WebApiResponse<TEntity>>(HttpStatusCode.OK, webapiResponse);
            }
            else
            {
                return Request.CreateResponse<WebApiResponse<TEntity>>(HttpStatusCode.OK, webapiResponse);
            }
        }

        [ActionName("Count")]
        [HttpPost]
        public virtual IHttpActionResult Count([FromBody]DataManagerRequest options)
        {
            return Json(new { count = Repository.Count(options) });
        }

        [ActionName("Insert")]
        [HttpPost]
        public virtual HttpResponseMessage Insert([FromBody]InsertModel<TEntity> insert)
        {
            webapiResponse = Repository.Insert(insert);
            return Request.CreateResponse(webapiResponse.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest, webapiResponse);
        }

        [ActionName("Update")]
        [HttpPost]
        public virtual HttpResponseMessage Update([FromBody]UpdateModel<TEntity> update)
        {
            webapiResponse = Repository.Update(update);
            return Request.CreateResponse(webapiResponse.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest, webapiResponse);
        }

        [ActionName("Remove")]
        [HttpPost]
        public virtual HttpResponseMessage Remove([FromBody] RemoveModel<TEntity> remove)
        {
            webapiResponse = Repository.Remove(remove);
            return Request.CreateResponse(webapiResponse.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest, webapiResponse);
        }

        [ActionName("Crud")]
        [HttpPost]
        public virtual HttpResponseMessage Crud([FromBody] CrudModel<TEntity> crud)
        {
            webapiResponse = Repository.Crud(crud);
            return Request.CreateResponse(webapiResponse.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest, webapiResponse);
        }

        [ActionName("UpdateAll")]
        [HttpPost]
        public virtual HttpResponseMessage UpdateAll([FromBody] CrudModel<TEntity> crud)
        {
            webapiResponse = Repository.UpdateAll(crud);
            return Request.CreateResponse(webapiResponse.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest, webapiResponse);
        }

        [ActionName("CancelAll")]
        [HttpPost]
        public virtual HttpResponseMessage CancelAll([FromBody] CrudModel<TEntity> crud)
        {
            webapiResponse = Repository.CancelAll(crud);
            return Request.CreateResponse(webapiResponse.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest, webapiResponse);
        }

        [ActionName("NewForm")]
        [HttpPost]
        public virtual HttpResponseMessage NewForm([FromBody] CrudModel<TEntity> crud)
        {
            webapiResponse = Repository.NewForm(crud);
            return Request.CreateResponse(webapiResponse.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest, webapiResponse);
        }

        [ActionName("GetForm")]
        [HttpPost]
        public virtual HttpResponseMessage GetForm([FromBody] DataManagerRequest options)
        {
            webapiResponse = Repository.GetForm(options);

            return Request.CreateResponse(webapiResponse.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest, webapiResponse);
        }

        [ActionName("UpdateForm")]
        [HttpPost]
        public virtual HttpResponseMessage UpdateForm([FromBody] CrudModel<TEntity> crud)
        {
            webapiResponse = Repository.UpdateForm(crud);
            return Request.CreateResponse(webapiResponse.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest, webapiResponse);
        }


    }
}
