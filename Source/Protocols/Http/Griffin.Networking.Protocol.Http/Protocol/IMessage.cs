using System.IO;
using System.Text;

namespace Griffin.Networking.Protocol.Http.Protocol
{
    /// <summary>
    /// Base interface for request and response.
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// Gets current protocol version
        /// </summary>
        /// <value>
        /// Default is HTTP/1.1
        /// </value>
        string ProtocolVersion { get; }

        /// <summary>
        /// Gets or sets body stream.
        /// </summary>
        Stream Body { get; set; }

        /// <summary>
        /// Gets number of bytes in the body
        /// </summary>
        int ContentLength { get; set; }

        /// <summary>
        /// Gets or sets content encoding
        /// </summary>
        /// <remarks>Appended to the contentType header as "charset" parameter.</remarks>
        Encoding ContentEncoding { get; set; }

        /// <summary>
        /// Gets headers.
        /// </summary>
        IHeaderCollection Headers { get; }

        /// <summary>
        /// Add a new header
        /// </summary>
        /// <param name="name">HTTP header name</param>
        /// <param name="value">Value</param>
        /// <remarks>Adding an existing header will result in that both values will be merged (comma seperated)</remarks>
        void AddHeader(string name, string value);
    }
}