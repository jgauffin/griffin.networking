using System;
using System.Net;

namespace Griffin.Networking.Http
{
    public class HttpException : Exception
    {
        public HttpException(HttpStatusCode statusCode, string message)
            : base(message)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; private set; }
    }
}