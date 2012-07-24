using System;
using System.Collections.Generic;
using System.Net;
using Griffin.Networking.Http.Protocol;

namespace Griffin.Networking.Http.Services.BodyDecoders
{
    /// <summary>
    /// Can provide one or more decoders.
    /// </summary>
    /// <remarks>The default implementation constructor uses <see cref="UrlFormattedDecoder"/> and <see cref="MultipartDecoder"/></remarks>
    public class CompositeBodyDecoder : IBodyDecoder
    {
        private readonly Dictionary<string, IBodyDecoder> _decoders = new Dictionary<string, IBodyDecoder>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeBodyDecoder"/> class.
        /// </summary>
        public CompositeBodyDecoder()
        {
            _decoders.Add("application/x-www-form-urlencoded", new UrlFormattedDecoder());
            _decoders.Add(MultipartDecoder.MimeType, new MultipartDecoder());
        }

        /// <summary>
        /// Add another handlers.
        /// </summary>
        /// <param name="mimeType">Mime type</param>
        /// <param name="decoder">The decoder implementation. Must be thread safe.</param>
        public void Add(string mimeType, IBodyDecoder decoder)
        {
            if (mimeType == null) throw new ArgumentNullException("mimeType");
            if (decoder == null) throw new ArgumentNullException("decoder");
            _decoders[mimeType] = decoder;
        }

        /// <summary>
        /// Parses the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <exception cref="FormatException">Body format is invalid for the specified content type.</exception>
        public void Decode(IRequest message)
        {
            IBodyDecoder decoder;
            string contentType = GetContentTypeWithoutCharset(message.ContentType);

            if (!_decoders.TryGetValue(contentType, out decoder))
                throw new HttpException(HttpStatusCode.UnsupportedMediaType, "Unrecognized mime type: " + message.ContentType);

            decoder.Decode(message);
        }

        private string GetContentTypeWithoutCharset(string contentType)
        {
            if (!String.IsNullOrEmpty(contentType))
            {
                int pos = contentType.IndexOf(";");

                if (pos > 0)
                {
                    return contentType.Substring(0, pos).Trim();
                }
            }

            return contentType;
        }
    }
}
