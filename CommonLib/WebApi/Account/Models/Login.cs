﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.WebApi.Account.Models
{
    public class Login
    {
        public int LoginId { get; set; }
        public string LoginName { get; set; }
        public string Email { get; set; }
        public string EmailConfirm { get; set; }
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }
    }
}
