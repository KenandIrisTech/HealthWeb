using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLib.Models.Authentication
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
