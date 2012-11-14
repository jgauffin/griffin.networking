using System;
using System.Net;
using Griffin.Networking.Http.Messages;
using Griffin.Networking.Http.Protocol;
using Griffin.Networking.Http.Specification;

namespace Griffin.Networking.Http.Implementation
{
    /// <summary>
    /// HTTP response implementation.
    /// </summary>
    public class HttpResponse : HttpMessage, IResponse
    {
        private readonly HttpCookieCollection<HttpResponseCookie> _cookies =
            new HttpCookieCollection<HttpResponseCookie>();

        public HttpResponse(string httpVersion, int code, string reason)
        {
            if (httpVersion == null) throw new ArgumentNullException("httpVersion");
            if (reason == null) throw new ArgumentNullException("reason");
            ProtocolVersion = httpVersion;
            StatusCode = code;
            StatusDescription = reason;
            KeepAlive = httpVersion.Contains("1.1");
        }

        public HttpResponse(string httpVersion, HttpStatusCode code, string reason)
            : this(httpVersion, (int) code, reason)
        {
        }

        #region IResponse Members

        /// <summary>
        /// Gets or set if connection should be kept alive.
        /// </summary>
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
            AddHeader("Location", uri);
            StatusCode = (int) HttpStatusCode.Redirect;
        }

        #endregion
    }
}