using IdentityModel;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Models.Authentication.Providers
{
    public abstract class OAuth2Base
    {
        public string ProviderName { get; set; }
        public string Description { get; set; }
        public LoginProvider Provider { get; protected set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Scope { get; set; }
        public Uri AuthorizationUri { get; set; }
        public Uri RedirectUri { get; set; }
        public Uri RequestTokenUri { get; set; }
        public Uri UserInfoUri { get; set; }
        public bool IsUsingNativeUI { get; set; } = false;

        //public abstract Task<User> GetUserInfoAsync(Account account);
        //public abstract Task<(bool IsRefresh, User User)> RefreshTokenAsync(User user);


        public abstract string CreateAuthorizationRequest();

        protected string CreateCodeChallenge()
        {
            string codeChallenge;

            string codeVerifier = CryptoRandom.CreateUniqueId();
            using (var sha256 = SHA256.Create())
            {
                var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
                codeChallenge = Base64Url.Encode(challengeBytes);
            }
            return codeChallenge;
        }

    }
}
