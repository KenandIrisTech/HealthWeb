using CommonLib.Models.Authentication;
using CommonLib.Models.Authentication.Providers;
using CommonLib.Models.Authentication.Service;
using IdentityModel.Client;
using iHealth2C.Models.Authentication.Providers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace HealthWeb.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult SignUp()
        {
            return View();
        }

        public ActionResult Forgot()
        {
            return View();
        }

        public ActionResult Associate()
        {
            return View();
        }

        public ActionResult ExternalLogin(LoginProvider provider)
        {
            OAuth2Base oAuth =  OAuth2ProviderFactory.CreateProvider(LoginProvider.Facebook);
            string url = oAuth.CreateAuthorizationRequest();
            return new RedirectResult(oAuth.CreateAuthorizationRequest());

            // https://developers.google.com/identity/protocols/oauth2/web-server#httprest
        }

        public void OAuth2Callback()
        {
            var _serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore
            };

            OAuth2Base oAuth2 = OAuth2ProviderFactory.CreateProvider(LoginProvider.Facebook);
            OAuth2Token oAuth2Token = new OAuth2Token
            {
                AuthUser = Request.QueryString["authuser"],
                Code = Request.QueryString["code"],
                Prompt = Request.QueryString["prompt"],
                Scope = Request.QueryString["scope"],
                State = Request.QueryString["state"],
            };

            HttpClient _client = new HttpClient();
            string data = $"grant_type=authorization_code&client_id={oAuth2.ClientId}&client_secret={oAuth2.ClientSecret}&code={oAuth2Token.Code}&redirect_uri={oAuth2.RedirectUri.ToString()}";
            var content = new StringContent(data);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            HttpResponseMessage response = _client.PostAsync(oAuth2.RequestTokenUri.ToString(), content).Result;

            GoogleUser googleUser;
            if (response.IsSuccessStatusCode)
            {
                string serialized = response.Content.ReadAsStringAsync().Result;
                UserToken result = JsonConvert.DeserializeObject<UserToken>(serialized, _serializerSettings);

                _client.SetBearerToken(result.AccessToken);
                var tokenResponse = _client.GetAsync(oAuth2.UserInfoUri.ToString()).Result;
                if(tokenResponse.IsSuccessStatusCode)
                {
                    string tokenSerialized = tokenResponse.Content.ReadAsStringAsync().Result;
                    googleUser = JsonConvert.DeserializeObject<GoogleUser>(tokenSerialized);
                }
            }

            return;
        }


        async Task HandleResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.Forbidden ||
                    response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new ServiceAuthenticationException(content);
                }

                throw new HttpRequestExceptionEx(response.StatusCode, content);
            }
        }
    }
}