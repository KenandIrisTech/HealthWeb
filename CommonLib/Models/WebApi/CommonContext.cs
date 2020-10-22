using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLib.Data;

namespace CommonLib.Models.WebApi
{
    public class CommonContext : CommonDB, ICommonContext
    {
        public DbContext Context { get; set; }
    }
}
