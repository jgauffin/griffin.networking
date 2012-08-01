using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Griffin.Networking.Http
{
    public class HttpException : Exception
    {
        public HttpStatusCode StatusCode { get; private set; }

        public HttpException(HttpStatusCode statusCode, string message)
            :base(message)
        {
            StatusCode = statusCode;
        }
    }
}
