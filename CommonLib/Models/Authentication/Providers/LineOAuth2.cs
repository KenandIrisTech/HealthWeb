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
    public class LineOAuth2 : OAuth2Base
    {
        private static readonly Lazy<LineOAuth2> lazy = new Lazy<LineOAuth2>(() => new LineOAuth2());

        public static LineOAuth2 Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        private LineOAuth2()
        {
            Initialize();
        }

        void Initialize()
        {
            ProviderName = "Line";
            Description = "Line Login Provider";
            Provider = LoginProvider.Line;
            ClientId = "1653891837";
            ClientSecret = "3a543fc8d0a5dc719c57441f5b326961";
            Scope = "profile"; //openid
            AuthorizationUri = new Uri("https://access.line.me/oauth2/v2.1/authorize");
            RequestTokenUri = new Uri("https://api.line.me/oauth2/v2.1/token");
            //RedirectUri = new Uri("http://cs.ehi.com.tw/CS/LineOAuthCallback");
            RedirectUri = new Uri("https://9952340f9e4c.ngrok.io/Account/OAuth2Callback");
            UserInfoUri = new Uri("https://api.line.me/v2/profile");
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
        //    string refreshToke = account.Properties["refresh_token"];
        //    int expriesIn;
        //    int.TryParse(account.Properties["expires_in"], out expriesIn);

        //    Dictionary<string, string> dictionary = new Dictionary<string, string> { { "Authorization", token } };
        //    var request = new OAuth2Request("POST", UserInfoUri, dictionary, account);
        //    var response = await request.GetResponseAsync();
        //    if (response != null && response.StatusCode == HttpStatusCode.OK)
        //    {
        //        string userJson = await response.GetResponseTextAsync();
        //        var lineUser = JsonConvert.DeserializeObject<LineUser>(userJson);
        //        user = new User
        //        {
        //            Id = lineUser.Id,
        //            Token = token,
        //            RefreshToken = refreshToke,
        //            Name = lineUser.Name,
        //            ExpiresIn = DateTime.UtcNow.Add(new TimeSpan(expriesIn)),
        //            PictureUrl = lineUser.ProfileImage,
        //            Provider = LoginProvider.Line,
        //            LoggedInWithSNSAccount = true,
        //        };
        //    }
        //    AppSettings.User = user;
        //    return user;
        //}

        //public override async Task<(bool IsRefresh, User User)> RefreshTokenAsync(User user)
        //{
        //    bool refreshSuccess = false;
        //    //var user = AppSettings.User;
        //    if (user == null)
        //    {
        //        return (refreshSuccess, user);
        //    }

        //    Dictionary<string, string> dictionary = new Dictionary<string, string> { { "grant_type", "refresh_token" }, { "refresh_token", user.RefreshToken }, { "client_id", ClientId } };
        //    var request = new Request("POST", RequestTokenUri, dictionary, null);
        //    var response = await request.GetResponseAsync();
        //    if (response != null && response.StatusCode == HttpStatusCode.OK)
        //    {
        //        // Deserialize the data and store it in the account store
        //        // The users email address will be used to identify data in SimpleDB
        //        string tokenString = await response.GetResponseTextAsync();
        //        JObject jwtDynamic = JsonConvert.DeserializeObject<JObject>(tokenString);
        //        var accessToken = jwtDynamic.Value<string>("access_token");
        //        var refreshToken = jwtDynamic.Value<string>("refresh_token");
        //        var expiresIn = jwtDynamic.Value<int>("expires_in");


        //        user.Token = accessToken;
        //        user.RefreshToken = refreshToken;
        //        user.ExpiresIn = DateTime.UtcNow.Add(new TimeSpan(0, 0, expiresIn));
        //    }

        //    return (refreshSuccess, user);
        //}
        #endregion
    }
}
