using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Models.Authentication.Service
{
    public class HttpRequestExceptionEx : HttpRequestException
    {
        public System.Net.HttpStatusCode HttpCode { get; }

        public HttpRequestExceptionEx(HttpStatusCode code)
            : this(code, null, null)
        {
        }

        public HttpRequestExceptionEx(HttpStatusCode code, string message)
            : this(code, message, null)
        {
        }

        public HttpRequestExceptionEx(HttpStatusCode code, string message, Exception inner)
            : base(message, inner)
        {
            HttpCode = code;
        }
    }
}
