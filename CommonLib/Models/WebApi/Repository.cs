using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLib.Data;

namespace CommonLib.Models.WebApi
{
    public class Repository<TEntity> : RepositoryBase<TEntity>, IRepository<TEntity> where TEntity : class, new()
    {
        public Repository()
        {
            context = new CommonDB();
        }
    }
}
