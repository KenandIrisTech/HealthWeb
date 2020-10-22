using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.WebApi.Account.Models
{
    public class User
    {
        public int LoginId { get; set; }
        public string Id { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresIn { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public DateTime Birthday { get; set; }
        public string PictureUrl { get; set; }
        public bool LoggedInWithSNSAccount { get; set; }

        public LoginProvider Provider { get; set; }
        public string ConnectionId { get; set; }
    }
}
