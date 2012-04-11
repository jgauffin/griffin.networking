using System;
using Griffin.Networking.Http.Specification;

namespace Griffin.Networking.Http.Protocol
{
    /// <summary>
    /// Request sent to/from a HTTP server.
    /// </summary>
    public interface IRequest : IMessage
    {
        /// <summary>
        /// Gets or sets if connection is being kept alive
        /// </summary>
        bool KeepAlive { get; }

        /// <summary>
        /// Gets cookies.
        /// </summary>
        IHttpCookieCollection<IHttpCookie> Cookies { get; }

        /// <summary>
        /// Gets all uploaded files.
        /// </summary>
        IHttpFileCollection Files { get; }

        /// <summary>
        /// Gets form parameters.
        /// </summary>
        IParameterCollection Form { get; }

        /// <summary>
        /// Gets if request is an Ajax request.
        /// </summary>
        bool IsAjax { get; }

        /// <summary>
        /// Gets or sets HTTP method.
        /// </summary>
        string Method { get; set; }

        /// <summary>
        /// Gets query string.
        /// </summary>
        IParameterCollection QueryString { get; }

        /// <summary>
        /// Gets requested URI.
        /// </summary>
        Uri Uri { get; set; }
    }
}