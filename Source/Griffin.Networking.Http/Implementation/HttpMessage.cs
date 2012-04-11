using System.IO;
using System.Linq;
using System.Text;
using Griffin.Networking.Http.Protocol;
using Griffin.Networking.Http.Specification;

namespace Griffin.Networking.Http.Implementation
{
    public class HttpMessage :IMessage
    {
        protected void SetHeader(string name, string value)
        {
            _headers[name] = new HttpHeader(name, value);
        }
        /// <summary>
        /// Gets current protocol version
        /// </summary>
        /// <value>
        /// Default is HTTP/1.1
        /// </value>
        public string ProtocolVersion { get;  set; }

        /// <summary>
        /// Gets or sets body stream.
        /// </summary>
        public Stream Body { get;  set; }

        private int _contentLength;

        /// <summary>
        /// Gets number of bytes in the body
        /// </summary>
        public int ContentLength
        {
            get
            {
                if (_contentLength == 0 && Body != null)
                    return (int)Body.Length;
                return _contentLength;
            }
            set { _contentLength = value; }
        }

        /// <summary>
        /// Gets or sets content encoding
        /// </summary>
        /// <remarks>Appended to the contentType header as "charset" parameter.</remarks>
        public Encoding ContentEncoding { get; set; }

        /// <summary>
        /// Gets headers.
        /// </summary>
        public IHeaderCollection Headers { get { return _headers; } }
        HttpHeaderCollection _headers = new HttpHeaderCollection();

        public void AddHeader(string name, string value)
        {
            _headers.Add(name, value);
        }
    }
}
