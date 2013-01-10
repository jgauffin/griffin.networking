using System;
using System.Net;
using Griffin.Networking.Protocol.Http.Protocol;

namespace Griffin.Networking.Protocol.Http.Implementation
{
    /// <summary>
    /// HTTP response implementation.
    /// </summary>
    public class HttpResponse : HttpMessage, IResponse
    {
        private readonly HttpCookieCollection<IResponseCookie> _cookies =
            new HttpCookieCollection<IResponseCookie>();

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpResponse" /> class.
        /// </summary>
        /// <param name="httpVersion">The HTTP version ("HTTP/1.1").</param>
        /// <param name="code">HTTP status code.</param>
        /// <param name="reason">Reason to why that specific code was used..</param>
        /// <exception cref="System.ArgumentNullException">httpVersion</exception>
        public HttpResponse(string httpVersion, int code, string reason)
        {
            if (httpVersion == null) throw new ArgumentNullException("httpVersion");
            if (reason == null) throw new ArgumentNullException("reason");
            if (code <= 0) throw new ArgumentOutOfRangeException("code", code, "There are no HTTP codes which are 0 or negative.");

            ProtocolVersion = httpVersion;
            StatusCode = code;
            StatusDescription = reason;
            KeepAlive = httpVersion.Contains("1.1");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpResponse" /> class.
        /// </summary>
        /// <param name="httpVersion">The HTTP version ("HTTP/1.1").</param>
        /// <param name="code">HTTP status code.</param>
        /// <param name="reason">Reason to why that specific code was used..</param>
        public HttpResponse(string httpVersion, HttpStatusCode code, string reason)
            : this(httpVersion, (int) code, reason)
        {
        }

        #region IResponse Members

        /// <summary>
        /// Gets or set if connection should be kept alive.
        /// </summary>
        /// <remarks>Keep alive means that the client should not close the connection
        /// between requests. It makes the HTTP handling a little bit faster.</remarks>
        public bool KeepAlive
        {
            get
            {
                var header = Headers["Connection"];
                return header != null && Headers["Connection"].Is("Keep-Alive");
            }
            set
            {
                var ourValue = value ? "Keep-Alive" : "Close";
                AddHeader("Connection", ourValue);
            }
        }

        /// <summary>
        /// Gets cookies.
        /// </summary>
        public IHttpCookieCollection<IResponseCookie> Cookies
        {
            get { return _cookies; }
        }

        /// <summary>
        /// Gets a motivation to why the specified status code were selected.
        /// </summary>
        public string StatusDescription { get; set; }

        /// <summary>
        /// Status code that is sent to the client.
        /// </summary>
        /// <remarks>Default is <see cref="HttpStatusCode.OK"/></remarks>
        public int StatusCode { get; set; }

        ///<summary>
        /// Gets or sets content type
        ///</summary>
        /// <remarks>Only the mime type</remarks>
        /// <seealso cref="IMessage.ContentEncoding"/>
        public string ContentType { get; set; }

        /// <summary>
        /// Redirect user.
        /// </summary>
        /// <param name="uri">Where to redirect to.</param>
        /// <remarks>
        /// Any modifications after a redirect will be ignored.
        /// </remarks>
        public void Redirect(string uri)
        {
            if (uri == null) throw new ArgumentNullException("uri");

            AddHeader("Location", uri);
            StatusCode = (int) HttpStatusCode.Redirect;
        }

        #endregion
    }
}