using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLib.Models.Authentication
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
