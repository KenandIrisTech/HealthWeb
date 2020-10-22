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
    public class GoogleOAuth2 : OAuth2Base
    {
        private static readonly Lazy<GoogleOAuth2> lazy = new Lazy<GoogleOAuth2>(() => new GoogleOAuth2());

        public static GoogleOAuth2 Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        private GoogleOAuth2()
        {
            Initialize();
        }

        void Initialize()
        {
            ProviderName = "Google";
            Description = "Google Login Provider";
            Provider = LoginProvider.Google;
            ClientId = "908832238442-dl5ftrcmnud20fhp0ecbrp61pbfb36oa.apps.googleusercontent.com"; 
            ClientSecret = "m3buL71uXUoXB0iReD6DRMDq";                                             
            Scope = "email profile openid";
            AuthorizationUri = new Uri("https://accounts.google.com/o/oauth2/v2/auth");
            RequestTokenUri = new Uri("https://oauth2.googleapis.com/token");
            RedirectUri = new Uri("https://9952340f9e4c.ngrok.io/Account/OAuth2Callback");
            UserInfoUri = new Uri("https://openidconnect.googleapis.com/v1/userinfo");
            IsUsingNativeUI = true;
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


        //    Dictionary<string, string> dictionary = new Dictionary<string, string> { };
        //    var request = new OAuth2Request("GET", UserInfoUri, dictionary, account);
        //    var response = await request.GetResponseAsync();
        //    if (response != null && response.StatusCode == HttpStatusCode.OK)
        //    {
        //        string userJson = await response.GetResponseTextAsync();
        //        var facebookUser = JsonConvert.DeserializeObject<GoogleUser>(userJson);
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

// Google authentication sample 
// https://timothelariviere.com/2017/09/01/authenticate-users-through-google-with-xamarin-auth/



// Key store C:\Users\ken.li\AppData\Local\Xamarin\Mono for Android\Keystore
// Key tools  C:\Program Files\Android\jdk\microsoft_dist_openjdk_1.8.0.25\bin
// keytool.exe -list -v -keystore "%LocalAppData%\Xamarin\Mono for Android\debug.keystore" -alias androiddebugkey -storepass android -keypass android
// keytool.exe -list -v -keystore debug.keystore -alias androiddebugkey -storepass android -keypass android
// keytool -keystore debug.keystore -list -v
