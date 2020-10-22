using CommonLib.Models.Authentication;
using CommonLib.Models.Authentication.Providers;
using System;
using System.Collections.Generic;
using System.Text;

namespace iHealth2C.Models.Authentication.Providers
{
    public class OAuth2ProviderFactory
    {
        public static OAuth2Base CreateProvider(LoginProvider provider)
        {
            OAuth2Base oAuth2 = null;

            switch (provider)
            {
                case LoginProvider.Line:
                    oAuth2 = LineOAuth2.Instance;
                    break;
                case LoginProvider.Facebook:
                    oAuth2 = FacebookOAuth2.Instance;
                    break;
                case LoginProvider.Google:
                    oAuth2 = GoogleOAuth2.Instance;
                    break;
            }
            return oAuth2;
        }
    }
}
