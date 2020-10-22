using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Models.WebApi
{
    public interface ICommonContext
    {
        DbContext Context { get; set; }
    }
}
