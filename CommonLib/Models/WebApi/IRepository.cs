using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syncfusion.EJ2.Base;
using CommonLib.Models.Data;
using System.Data.Entity;


// https://keepitsimplekarthik.com/sharepoint/entity-framework-webapi-unity/
namespace CommonLib.Models.WebApi
{
    public interface IRepository<TEntity> where TEntity : class
    {
        WebApiResponse<TEntity> GetAll(DataManagerRequest options);
        int Count(DataManagerRequest options);
        int Count();
        WebApiResponse<TEntity> Insert(InsertModel<TEntity> insert);
        WebApiResponse<TEntity> Update(UpdateModel<TEntity> update);
        WebApiResponse<TEntity> Remove(RemoveModel<TEntity> remove);
        WebApiResponse<TEntity> Crud(CrudModel<TEntity> crud);
        WebApiResponse<TEntity> UpdateAll(CrudModel<TEntity> crud);
        WebApiResponse<TEntity> CancelAll(CrudModel<TEntity> crud);
        WebApiResponse<TEntity> NewForm(CrudModel<TEntity> crud);
        WebApiResponse<TEntity> GetForm(DataManagerRequest options);
        WebApiResponse<TEntity> UpdateForm(CrudModel<TEntity> crud);
        DbContext Context { get; }
    }
}
