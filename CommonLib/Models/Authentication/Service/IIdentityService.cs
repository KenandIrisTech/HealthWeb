using CommonLib.Models.Authentication.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Models.Authentication.Service
{
    public interface IIdentityService
    {
        string CreateAuthorizationRequest(OAuth2Base oAuth2);
        Task<UserToken> GetTokenAsync(OAuth2Base oAuth2, string code);
        Task<string> GetAsync(string uri, string accessToken);
    }
}
