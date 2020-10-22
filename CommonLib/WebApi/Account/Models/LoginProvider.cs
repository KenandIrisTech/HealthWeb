using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.WebApi.Account.Models
{
    public enum LoginProvider
    {
        None = -1,
        System = 0,
        Line = 1,
        Facebook = 2,
        WeChat = 3,
        Google = 4
    }
}
