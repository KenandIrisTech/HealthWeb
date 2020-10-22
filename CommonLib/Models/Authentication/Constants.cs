using System;
using System.Collections.Generic;
using System.Text;

namespace iHealth2C.Models.Authentication
{
    public static class Constants
    {
        //public static string AuthorityUri = "https://demo.identityserver.io";
        //public static string AuthorizeUri = "https://demo.identityserver.io/connect/authorize";
        //public static string TokenUri = "https://demo.identityserver.io/connect/token";
        //public static string RedirectUri = "io.identitymodel.native://callback";
        //public static string ApiUri = "https://demo.identityserver.io/api/";
        //public static string ClientId = "native.hybrid";
        //public static string ClientSecret = "xoxo";
        //public static string Scope = "openid profile api";

        // Line CGU
        //public static string AuthorityUri = "https://access.line.me/oauth2/v2.1/";
        //public static string AuthorizeUri = "https://access.line.me/oauth2/v2.1/authorize";
        //public static string TokenUri = "https://api.line.me/oauth2/v2.1/token";
        //public static string RedirectUri = "https://cs.ehi.com.tw/";
        //public static string ApiUri = "https://api.line.me/v2/";
        //public static string ClientId = "1654171494";
        //public static string ClientSecret = "6ec3544e2437106d8ca20226d9062305";
        //public static string Scope = "openid profile api";

        // Facebook CGU
        //public static string AuthorityUri = "https://www.facebook.com/dialog/";
        //public static string AuthorizeUri = "https://www.facebook.com/dialog/oauth";
        //public static string TokenUri = "https://graph.facebook.com/oauth/access_token";
        ////public static string RedirectUri = "https://www.facebook.com/connect/login_success.html";
        //public static string RedirectUri = "https://www.facebook.com";
        //public static string ApiUri = "https://graph.facebook.com/me";
        //public static string ClientId = "446186719331047";
        //public static string ClientSecret = "126cce8b8401e11c5a99e80a9c155a90";
        //public static string Scope = "email";

        // package com.ehi.iHealth2C

        // Google
        public static string AuthorityUri = "https://accounts.google.com/o/oauth2/v2";
        public static string AuthorizeUri = "https://accounts.google.com/o/oauth2/v2/auth";
        public static string TokenUri = "https://oauth2.googleapis.com/token";
        //public static string RedirectUri = "com.ehi.iHealth2C:/oauth2redirect";
        public static string RedirectUri = "http://localhost";
        public static string ApiUri = "https://openidconnect.googleapis.com/v1/userinfo";
        public static string ClientId = "908832238442-s2qddqgh67asvt6fq876k9hrt5hvf6rc.apps.googleusercontent.com";
        //public static string ClientId = "908832238442-dl5ftrcmnud20fhp0ecbrp61pbfb36oa.apps.googleusercontent.com";
        //public static string ClientSecret = "m3buL71uXUoXB0iReD6DRMDq";
        public static string ClientSecret = string.Empty;
        public static string Scope = "openid profile openid";
    }
}
