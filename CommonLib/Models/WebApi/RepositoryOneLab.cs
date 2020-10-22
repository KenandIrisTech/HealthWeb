using CommonLib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Models.WebApi
{
    public class RepositoryOneLab<TEntity> : RepositoryBase<TEntity>, IRepository<TEntity> where TEntity : class, new()
    {
        public RepositoryOneLab()
        {
            context = new OneLabDB();
        }
    }
}
