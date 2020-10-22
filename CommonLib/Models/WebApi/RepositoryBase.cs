using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLib.Models.Data;
using Syncfusion.EJ2.Base;
using System.Data.Entity;
using CommonLib.Data;
using System.Reflection;
using CommonLib.Models.Linq;


// https://keepitsimplekarthik.com/sharepoint/entity-framework-webapi-unity/

namespace CommonLib.Models.WebApi
{
    public class RepositoryBase<TEntity> : IRepository<TEntity>, IDisposable where TEntity : class, new()
    {
        protected DbContext context;
        private bool disposed = false;
        private WebApiResponse<TEntity> webapiResponse;
        private DataOperations Operations;
        private TEntity entity;
        private DateTime UpdatedTime = DateTime.Now;
        private EntityManager EntityReflection;
        private UserInfo UserInfo = new UserInfo();


        public RepositoryBase()
        {
            this.Operations = new DataOperations();
            this.webapiResponse = new WebApiResponse<TEntity>();
            this.entity = new TEntity();
        }

        public virtual WebApiResponse<TEntity> GetAll(DataManagerRequest options)
        {
            // LazyLoadingEnabled = true to get child : false not load instead 
            Context.Configuration.LazyLoadingEnabled = !options.RequiresCounts; //false;
            //webapiResponse.Success = (Context.SaveChanges() > 0);
            webapiResponse.Results = Operations.Execute(Context.Set<TEntity>().AsNoTracking(), options).ToList();
            return webapiResponse;
        }
        public virtual int Count(DataManagerRequest options)
        {
            Context.Configuration.LazyLoadingEnabled = false;
            return Operations.Execute(Context.Set<TEntity>().AsNoTracking(), options).Count();
        }
        public virtual int Count()
        {
            Context.Configuration.LazyLoadingEnabled = false;
            return Operations.Execute(Context.Set<TEntity>().AsNoTracking(), new DataManagerRequest()).Count();
        }
        public virtual WebApiResponse<TEntity> Insert(InsertModel<TEntity> insert)
        {
            webapiResponse.Result = entity;
            return webapiResponse;
        }
        public virtual WebApiResponse<TEntity> Update(UpdateModel<TEntity> update)
        {
            Context.Entry(update.Value).State = EntityState.Modified;
            webapiResponse.Success = (Context.SaveChanges() > 0);
            if (!webapiResponse.Success)
            {
                webapiResponse.ReturnMessage = Context.GetValidationErrors().First().ValidationErrors.First().ErrorMessage;
                webapiResponse.FieldName = Context.GetValidationErrors().First().ValidationErrors.First().PropertyName;
                webapiResponse.EntityName = Context.GetValidationErrors().First().Entry.Entity.GetType().Name;
            }
            webapiResponse.Result = update.Value;
            return webapiResponse;
        }
        public virtual WebApiResponse<TEntity> Remove(RemoveModel<TEntity> remove)
        {
            webapiResponse.Result = remove as TEntity;
            return webapiResponse;
        }
        public virtual WebApiResponse<TEntity> Crud(CrudModel<TEntity> crud)
        {
            switch (crud.Action.ToLower())
            {
                case "insert":
                    UpdateUserDateTime(crud.Value, crud.Action);
                    crud.Value = Context.Set<TEntity>().Add(crud.Value);
                    break;
                case "update":
                    UpdateUserDateTime(crud.Value, crud.Action);
                    Context.Entry(crud.Value).State = EntityState.Modified;
                    break;
                case "remove":

#if debug
                    // 底下的程式沒有 parent-child table 關係可以 刪除資料
                    //TEntity value =  (TEntity)Activator.CreateInstance(typeof(TEntity));
                    {
                        TEntity value = new TEntity();
                        value.GetType().GetProperty(crud.KeyColumn).SetValue(value, crud.Key, null);
                        Context.Entry(value).State = EntityState.Deleted;
                        Context.Set<TEntity>().Remove(value);
                        crud.Value = value;
                    }
#endif 
                    //Context.Configuration.ProxyCreationEnabled = true;
                    //Context.Configuration.LazyLoadingEnabled = false;
                    TEntity value = Context.Set<TEntity>().Where(String.Format("{0}=={1}", crud.KeyColumn, crud.Key), new object[] { }).SingleOrDefault();
                    //Context.Entry(value).State = EntityState.Deleted;
                    Context.Set<TEntity>().Remove(value);
                    break;
            }
            webapiResponse.Success = (Context.SaveChanges() > 0);
            if (!webapiResponse.Success)
            {
                if (Context.GetValidationErrors().Count() > 0)
                {
                    webapiResponse.ReturnMessage = Context.GetValidationErrors().First().ValidationErrors.First().ErrorMessage;
                    webapiResponse.FieldName = Context.GetValidationErrors().First().ValidationErrors.First().PropertyName;
                    webapiResponse.EntityName = Context.GetValidationErrors().First().Entry.Entity.GetType().Name;
                }
                else
                {
                    Type contextType = Context.GetType();
                    webapiResponse.ReturnMessage = contextType.GetProperty("ErrorMessage").GetValue(Context).ToString();
                    webapiResponse.FieldName = "DBUpdate Exception";
                    webapiResponse.EntityName = contextType.GetProperty("EntityName").GetValue(Context).ToString();
                    webapiResponse.ErrorId = Int32.Parse(contextType.GetProperty("ErrorId").GetValue(Context).ToString());
                    webapiResponse.Success = webapiResponse.ErrorId == 0;
                }
            }
            webapiResponse.Result = crud.Value;
            return webapiResponse;
        }
        public virtual WebApiResponse<TEntity> UpdateAll(CrudModel<TEntity> crud)
        {
            crud.Changed.All(entity =>
            {
                Context.Set<TEntity>().Attach(entity);
                UpdateEntity(entity);
                return true;
            });
            webapiResponse.Success = (Context.SaveChanges() > 0);
            if (!webapiResponse.Success)
            {
                webapiResponse.ReturnMessage = Context.GetValidationErrors().First().ValidationErrors.First().ErrorMessage;
                webapiResponse.FieldName = Context.GetValidationErrors().First().ValidationErrors.First().PropertyName;
                webapiResponse.EntityName = Context.GetValidationErrors().First().Entry.Entity.GetType().Name;
            }
            webapiResponse.Results = crud.Changed;
            return webapiResponse;
        }
        public virtual WebApiResponse<TEntity> CancelAll(CrudModel<TEntity> crud)
        {
            webapiResponse.Result = crud.Value;
            return webapiResponse;
        }
        public virtual WebApiResponse<TEntity> NewForm(CrudModel<TEntity> crud)
        {
            webapiResponse.Results = new List<TEntity>() { new TEntity() };
            return webapiResponse;
        }
        public virtual WebApiResponse<TEntity> GetForm(DataManagerRequest options)
        {
            Context.Configuration.LazyLoadingEnabled = true;
            webapiResponse.Results = Operations.Execute(Context.Set<TEntity>().AsNoTracking(), options).ToList();
            return webapiResponse;
        }
        public virtual WebApiResponse<TEntity> UpdateForm(CrudModel<TEntity> crud)
        {
            switch (crud.Action.ToLower())
            {
                case "insert":
                    crud.Added.All(entity =>
                    {
                        Context.Set<TEntity>().Add(entity);
                        UpdateEntity(entity);
                        return true;
                    });
                    break;
                case "update":
                default:
                    crud.Changed.All(entity =>
                    {
                        Context.Set<TEntity>().Attach(entity);
                        UpdateEntity(entity);
                        return true;
                    });
                    break;
            }

            webapiResponse.Success = (Context.SaveChanges() > 0);
            if (!webapiResponse.Success)
            {
                if (Context.GetValidationErrors().Count() > 0)
                {
                    webapiResponse.ReturnMessage = Context.GetValidationErrors().First().ValidationErrors.First().ErrorMessage;
                    webapiResponse.FieldName = Context.GetValidationErrors().First().ValidationErrors.First().PropertyName;
                    webapiResponse.EntityName = Context.GetValidationErrors().First().Entry.Entity.GetType().Name;
                }
                else
                {
                    webapiResponse.Success = true;
                    webapiResponse.ReturnMessage = "無";
                    webapiResponse.FieldName = "資料異動";
                    webapiResponse.EntityName = "資料更新";
                }
            }
            webapiResponse.Results = crud.Changed;
            return webapiResponse;
        }
        public DbContext Context
        {
            get { return this.context; }
        }


        protected void UpdateUserDateTime(TEntity value, string action)
        {
            EntityManager entityReflection = new EntityManager(value);
            PropertyInfo[] updateUserDateTimeProps = value.GetType().GetProperties().Where(m =>
                            "CREATED_TIME.UPDATED_TIME.CREATED_USER_ID.UPDATED_USER_ID.".Contains(m.Name + ".")).ToArray();

            updateUserDateTimeProps.All(p => {
                switch (action)
                {
                    case "insert":
                        switch (p.Name)
                        {
                            case "CREATED_TIME":
                            case "UPDATED_TIME":
                                p.SetValue(value, UpdatedTime);
                                break;
                            case "CREATED_USER_ID":
                            case "UPDATED_USER_ID":
                                p.SetValue(value, -1);
                                break;
                        }
                        break;
                    case "update":
                        switch (p.Name)
                        {
                            case "UPDATED_TIME":
                                p.SetValue(value, UpdatedTime);
                                break;
                            case "UPDATED_USER_ID":
                                p.SetValue(value, -1);
                                break;
                        }
                        break;
                }
                return true;
            });
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;
            if (disposing)
            {
                context.Dispose();
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        protected void UpdateEntity(object entity)
        {
            EntityReflection = new EntityManager(entity);

            EntityState entityState = (EntityState)EntityReflection.StateProps.FirstOrDefault().GetValue(entity);

            // Update User ID
            EntityReflection.UpdateIdProps.All(p =>
            {
                if (entityState == EntityState.Added)
                {
                    p.SetValue(entity, UserInfo.USER_ID);
                }
                else if (entityState == EntityState.Modified && p.Name == "UPDATE_USER_ID")
                {
                    p.SetValue(entity, UserInfo.USER_ID);
                }
                return true;
            });

            // Update Created & Updated Time
            EntityReflection.UpdateTimeProps.All(p => {
                if (entityState == EntityState.Added)
                    p.SetValue(entity, UpdatedTime);
                else if (entityState == EntityState.Modified && p.Name == "UPDATED_TIME")
                {
                    p.SetValue(entity, UpdatedTime);
                }
                return true;
            });

            // Update Primary Key (not implementing)

            // Update EntityState
            EntityReflection.StateProps.All(p =>
            {
                Context.Entry(entity).State = entityState; //(System.Data.Entity.EntityState)p.GetValue(entity);
                return true;
            });

            // Update Children Entity Collection
            if (EntityReflection.CollectionProps.Length > 0)
            {
                EntityReflection.CollectionProps.All(childCollection =>
                {
                    IEnumerable collectionValue = (IEnumerable)childCollection.GetValue(entity);
                    Type collectionType = collectionValue.GetType().GetGenericArguments()[0];
                    IList listEntity = GetGenericList(collectionValue, collectionType);
                    foreach (var childEntity in listEntity)
                    {
                        UpdateEntity(childEntity);
                    }
                    return true;
                });
            }
        }

        protected IList GetGenericList(IEnumerable value, Type type)
        {
            Type listType = typeof(List<>).MakeGenericType(new[] { type });
            IList listEntity = (IList)Activator.CreateInstance(listType);
            foreach (var childEntity in value)
            {
                listEntity.Add(childEntity);
            }
            return listEntity;
        }
    }
}
