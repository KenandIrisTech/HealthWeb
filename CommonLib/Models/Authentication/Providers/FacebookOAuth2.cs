using IdentityModel.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Models.Authentication.Providers
{
    public class FacebookOAuth2 : OAuth2Base
    {
        private static readonly Lazy<FacebookOAuth2> lazy = new Lazy<FacebookOAuth2>(() => new FacebookOAuth2());

        public static FacebookOAuth2 Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        private FacebookOAuth2()
        {
            Initialize();
        }

        void Initialize()
        {
            ProviderName = "Facebook";
            Description = "Facebook Login Provider";
            Provider = LoginProvider.Facebook;
            ClientId = "446186719331047";
            ClientSecret = "126cce8b8401e11c5a99e80a9c155a90";
            Scope = "email";
            AuthorizationUri = new Uri("https://www.facebook.com/dialog/oauth");
            RequestTokenUri = new Uri("https://graph.facebook.com/oauth/access_token");
            RedirectUri = new Uri("https://9952340f9e4c.ngrok.io/Account/OAuth2Callback");
            UserInfoUri = new Uri("https://graph.facebook.com/me?fields=id,first_name,last_name,name,picture,email");
        }

        #region Implement Abstract Method

        public override string CreateAuthorizationRequest()
        {
            // Create URI to authorization endpoint
            var authorizeRequest = new RequestUrl(AuthorizationUri.ToString());

            // Dictionary with values for the authorize request
            var dic = new Dictionary<string, string>();
            dic.Add("client_id", ClientId);
            //dic.Add("client_secret", ClientSecret);
            dic.Add("response_type", "code");
            dic.Add("scope", Scope);
            dic.Add("redirect_uri", RedirectUri.ToString());
            //dic.Add("nonce", Guid.NewGuid().ToString("N"));
            //dic.Add("code_challenge", CreateCodeChallenge());
            //dic.Add("code_challenge_method", "S256");

            // Add CSRF token to protect against cross-site request forgery attacks.
            var currentCSRFToken = Guid.NewGuid().ToString("N");
            dic.Add("state", currentCSRFToken);

            var authorizeUri = authorizeRequest.Create(dic);
            return authorizeUri;
        }
        //public override async Task<User> GetUserInfoAsync(Account account)
        //{
        //    User user = null;
        //    string token = account.Properties["access_token"];
        //    int expriesIn;
        //    int.TryParse(account.Properties["expires_in"], out expriesIn);


        //    Dictionary<string, string> dictionary = new Dictionary<string, string> { { "fields", "name,email,picture,first_name,last_name" } };
        //    var request = new OAuth2Request("GET", UserInfoUri, dictionary, account);
        //    var response = await request.GetResponseAsync();
        //    if (response != null && response.StatusCode == HttpStatusCode.OK)
        //    {
        //        string userJson = await response.GetResponseTextAsync();
        //        var facebookUser = JsonConvert.DeserializeObject<FacebookUser>(userJson);
        //        user = new User
        //        {
        //            Id = facebookUser.Id,
        //            Token = token,
        //            RefreshToken = null,
        //            Name = facebookUser.Name,
        //            Email = facebookUser.Email,
        //            ExpiresIn = DateTime.UtcNow.Add(new TimeSpan(expriesIn)),
        //            PictureUrl = facebookUser.Picture.Data.Url,
        //            Provider = LoginProvider.Facebook,
        //            LoggedInWithSNSAccount = true,
        //        };
        //    }
        //    AppSettings.User = user;
        //    return user;
        //}

        //public override async Task<(bool IsRefresh, User User)> RefreshTokenAsync(User user)
        //{
        //    bool refreshSuccess = false;
        //    if (user == null)
        //    {
        //        return (refreshSuccess, user);
        //    }

        //    string code = null;
        //    Uri codeUri = new Uri($"https://graph.facebook.com/oauth/client_code?access_token={user.Token}&client_secret={ClientSecret}&redirect_uri={RedirectUri.AbsoluteUri}&client_id={ClientId}");

        //    var request = new Request("POST", codeUri, null, null);
        //    var response = await request.GetResponseAsync();
        //    if (response != null && response.StatusCode == HttpStatusCode.OK)
        //    {
        //        string tokenString = await response.GetResponseTextAsync();
        //        JObject jwtDynamic = JsonConvert.DeserializeObject<JObject>(tokenString);
        //        code = jwtDynamic.Value<string>("code");
        //    }

        //    if (!string.IsNullOrEmpty(code))
        //    {
        //        Dictionary<string, string> dictionary = new Dictionary<string, string> { { "code", code }, { "client_id", ClientId }, { "redirect_uri", RedirectUri.AbsoluteUri } };
        //        var refreshRequest = new Request("POST", RequestTokenUri, dictionary, null);
        //        var refreshResponse = await refreshRequest.GetResponseAsync();
        //        if (refreshResponse != null && refreshResponse.StatusCode == HttpStatusCode.OK)
        //        {
        //            string tokenString = await refreshResponse.GetResponseTextAsync();
        //            JObject jwtDynamic = JsonConvert.DeserializeObject<JObject>(tokenString);
        //            var accessToken = jwtDynamic.Value<string>("access_token");
        //            var expiresIn = jwtDynamic.Value<int>("expires_in");


        //            user.Token = accessToken;
        //            user.ExpiresIn = DateTime.UtcNow.Add(new TimeSpan(0, 0, expiresIn));

        //            refreshSuccess = true;
        //        }
        //    }
        //    return (refreshSuccess, user);
        //}
        #endregion
    }
}
