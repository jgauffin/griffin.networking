using System.Net;

namespace Griffin.Networking.Protocol.Http.Protocol
{
    /// <summary>
    /// Response to a request.
    /// </summary>
    public interface IResponse : IMessage
    {
        /// <summary>
        /// Gets or set if connection should be kept alive.
        /// </summary>
        bool KeepAlive { get; set; }

        /// <summary>
        /// Gets cookies.
        /// </summary>
        IHttpCookieCollection<IResponseCookie> Cookies { get; }

        /// <summary>
        /// Gets a motivation to why the specified status code were selected.
        /// </summary>
        string StatusDescription { get; set; }

        /// <summary>
        /// Status code that is sent to the client.
        /// </summary>
        /// <remarks>Default is <see cref="HttpStatusCode.OK"/></remarks>
        int StatusCode { get; set; }

        ///<summary>
        /// Gets or sets content type
        ///</summary>
        /// <remarks>Only the mime type</remarks>
        /// <seealso cref="IMessage.ContentEncoding"/>
        string ContentType { get; set; }


        /// <summary>
        /// Redirect user.
        /// </summary>
        /// <param name="uri">Where to redirect to.</param>
        /// <remarks>
        /// Any modifications after a redirect will be ignored.
        /// </remarks>
        void Redirect(string uri);
    }
}