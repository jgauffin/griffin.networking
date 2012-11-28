using System;
using System.Net;
using Griffin.Networking.Protocol.Http.Specification;

namespace Griffin.Networking.Protocol.Http.Protocol
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
        /// Gets content type
        /// </summary>
        /// <remarks>Any extra parameters are stripped. Use <see cref="Headers"/> to get the raw value</remarks>
        string ContentType { get; }

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
        /// Gets where the request originated from.
        /// </summary>
        IPEndPoint RemoteEndPoint { get; }

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

        /// <summary>
        /// Create a response for the request.
        /// </summary>
        /// <param name="code">Status code</param>
        /// <param name="reason">Gives the remote end point a hint to why the specified status code as used.</param>
        /// <returns>Created response</returns>
        /// <remarks>Can be used by implementations to transfer context specific information. It's prefered that you use this method
        /// instead of instantianting a response directly.</remarks>
        IResponse CreateResponse(HttpStatusCode code, string reason);
    }
}