using System;
using Griffin.Networking.Http.Implementation;

namespace Griffin.Networking.Http.Messages
{
    internal class HttpResponseCookie : HttpCookie, IResponseCookie
    {
        /// <summary>
        /// Gets when the cookie expires.
        /// </summary>
        /// <remarks><see cref="DateTime.MinValue"/> means that the cookie expires when the session do so.</remarks>
        public DateTime Expires { get; set; }

        /// <summary>
        /// Gets path that the cookie is valid under.
        /// </summary>
        public string Path { get; set; }
    }
}