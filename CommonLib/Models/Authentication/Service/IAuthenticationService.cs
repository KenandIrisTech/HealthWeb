using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Models.Authentication.Service
{
    public interface IAuthenticationService
    {
        bool IsAuthenticated { get; }

        User AuthenticatedUser { get; }

        Task<bool> LoginAsync(string email, string password);

        Task<bool> LoginWithSNSAsync(LoginProvider provider);

        Task<bool> LoginWithWebAuthAsync(LoginProvider provider);

        Task<bool> UserIsAuthenticatedAndValidAsync();

        Task LogoutAsync();

        //Action<LoginEventArgument> Callback { get; set; }
    }
}
