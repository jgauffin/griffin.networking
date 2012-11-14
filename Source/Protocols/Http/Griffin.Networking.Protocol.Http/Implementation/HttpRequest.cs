using System;
using System.IO;
using System.Net;
using Griffin.Networking.Http.Implementation.Infrastructure;
using Griffin.Networking.Http.Protocol;
using Griffin.Networking.Http.Specification;

namespace Griffin.Networking.Http.Implementation
{
    public class HttpRequest : HttpMessage, IRequest
    {
        private readonly IHttpCookieCollection<IHttpCookie> _cookies;
        private readonly IHttpFileCollection _files;
        private readonly ParameterCollection _queryString;
        private IParameterCollection _form;
        private string _pathAndQuery;

        public HttpRequest()
        {
            _cookies = new HttpCookieCollection<HttpCookie>();
            _files = new HttpFileCollection();
            _queryString = new ParameterCollection();
            _form = new ParameterCollection();
        }

        public HttpRequest(string httpMethod, string url, string httpVersion)
            :this()
        {
            if (httpMethod == null) throw new ArgumentNullException("httpMethod");
            if (url == null) throw new ArgumentNullException("url");
            if (httpVersion == null) throw new ArgumentNullException("httpVersion");
            Method = httpMethod;
            _pathAndQuery = url;
            
            Uri = new Uri("http://invalid.uri/" + url);
            ProtocolVersion = httpVersion;
        }

        #region IRequest Members

        /// <summary>
        /// Gets or sets if connection is being kept alive
        /// </summary>
        public bool KeepAlive
        {
            get { return Headers["Connection"].Value.Equals("Keep-Alive", StringComparison.OrdinalIgnoreCase); }
        }

        /// <summary>
        /// Gets content type
        /// </summary>
        /// <remarks>Any extra parameters are stripped. Use <see cref="Headers"/> to get the raw value</remarks>
        public string ContentType
        {
            get { return Headers["Content-Type"].Value; }
        }

        /// <summary>
        /// Gets cookies.
        /// </summary>
        public IHttpCookieCollection<IHttpCookie> Cookies
        {
            get { return _cookies; }
        }

        /// <summary>
        /// Gets all uploaded files.
        /// </summary>
        public IHttpFileCollection Files
        {
            get { return _files; }
        }

        /// <summary>
        /// Gets form parameters.
        /// </summary>
        public IParameterCollection Form
        {
            get { return _form; }
        }

        /// <summary>
        /// Gets where the request originated from.
        /// </summary>
        public IPEndPoint RemoteEndPoint { get; set; }

        /// <summary>
        /// Gets if request is an Ajax request.
        /// </summary>
        public bool IsAjax
        {
            get { return Headers["X-Requested-Width"].Value.Equals("Ajax", StringComparison.OrdinalIgnoreCase); }
        }

        /// <summary>
        /// Gets or sets HTTP method.
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Gets query string.
        /// </summary>
        public IParameterCollection QueryString
        {
            get { return _queryString; }
        }

        private Uri _uri;

        /// <summary>
        /// Gets requested URI.
        /// </summary>
        public Uri Uri
        {
            get { return _uri; }
            set
            {
                _uri = value;
                var decoder = new UrlDecoder();
                _queryString.Clear();
                using (var reader = new StringReader(value.Query.TrimStart('?')))
                {
                    decoder.Parse(reader, QueryString);
                }
            }
        }

        /// <summary>
        /// Create a response for the request.
        /// </summary>
        /// <param name="code">Status code</param>
        /// <param name="reason">Gives the remote end point a hint to why the specified status code as used.</param>
        /// <returns>Created response</returns>
        /// <remarks>Can be used by implementations to transfer context specific information. It's prefered that you use this method
        /// instead of instantianting a response directly.</remarks>
        public IResponse CreateResponse(HttpStatusCode code, string reason)
        {
            if (reason == null) throw new ArgumentNullException("reason");
            return new HttpResponse(ProtocolVersion, code, reason);
        }

        #endregion

        public override void AddHeader(string name, string value)
        {
            if (name.Equals("host", StringComparison.OrdinalIgnoreCase))
            {
                if (!value.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                    Uri = new Uri(string.Format("http://{0}{1}", value, _pathAndQuery));
                else
                    Uri = new Uri(string.Format("{0}{1}", value, _pathAndQuery));
            }
            if (name.Equals("Content-Length", StringComparison.CurrentCultureIgnoreCase))
            {
                ContentLength = int.Parse(value);
            }

            base.AddHeader(name, value);
        }
    }
}