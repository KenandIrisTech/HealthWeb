using CommonLib.Models.Authentication.Providers;
using IdentityModel;
using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Models.Authentication.Service
{
    public class IdentityService : IIdentityService
    {
        readonly IRequestProvider requestProvider;
        string codeVerifier;

        public IdentityService(IRequestProvider requestProvider)
        {
            this.requestProvider = requestProvider;
        }

        public string CreateAuthorizationRequest(OAuth2Base oAuth2)
        {
            // Create URI to authorization endpoint
            var authorizeRequest = new RequestUrl(oAuth2.AuthorizationUri.ToString());

            // Dictionary with values for the authorize request
            var dic = new Dictionary<string, string>();
            dic.Add("client_id", oAuth2.ClientId);
            dic.Add("client_secret", oAuth2.ClientSecret);
            dic.Add("response_type", "code id_token");
            dic.Add("scope", oAuth2.Scope);
            dic.Add("redirect_uri", oAuth2.RedirectUri.ToString());
            dic.Add("nonce", Guid.NewGuid().ToString("N"));
            dic.Add("code_challenge", CreateCodeChallenge());
            dic.Add("code_challenge_method", "S256");

            // Add CSRF token to protect against cross-site request forgery attacks.
            var currentCSRFToken = Guid.NewGuid().ToString("N");
            dic.Add("state", currentCSRFToken);

            var authorizeUri = authorizeRequest.Create(dic);
            return authorizeUri;
        }

        public async Task<UserToken> GetTokenAsync(OAuth2Base oAuth2, string code)
        {
            string data = string.Format("grant_type=authorization_code&code={0}&redirect_uri={1}&code_verifier={2}&client_id={3}", code, WebUtility.UrlEncode(oAuth2.RedirectUri.ToString()), codeVerifier, oAuth2.ClientId);
            //var token = await requestProvider.PostAsync<UserToken>(Constants.TokenUri, data, Constants.ClientId, Constants.ClientSecret);
            var token = await requestProvider.PostAsync<UserToken>(oAuth2.RequestTokenUri.ToString(), data, oAuth2.ClientId, oAuth2.ClientSecret);
            return token;
        }

        public async Task<string> GetAsync(string uri, string accessToken)
        {
            var response = await requestProvider.GetAsync(uri, accessToken);
            return response;
        }

        string CreateCodeChallenge()
        {
            string codeChallenge;

            codeVerifier = CryptoRandom.CreateUniqueId();
            using (var sha256 = SHA256.Create())
            {
                var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
                codeChallenge = Base64Url.Encode(challengeBytes);
            }
            return codeChallenge;
        }
    }
}
