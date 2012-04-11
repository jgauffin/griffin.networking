using System;

namespace Griffin.Networking.Protocols.Http
{
    public interface IResponseCookie : IHttpCookie
    {
        /// <summary>
        /// Gets when the cookie expires.
        /// </summary>
        /// <remarks><see cref="DateTime.MinValue"/> means that the cookie expires when the session do so.</remarks>
        DateTime Expires { get; set; }

        /// <summary>
        /// Gets path that the cookie is valid under.
        /// </summary>
        string Path { get; set; }
    }
}