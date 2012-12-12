using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Griffin.Networking.Protocol.Http.Implementation.Infrastructure;
using Griffin.Networking.Protocol.Http.Protocol;

namespace Griffin.Networking.Protocol.Http.Services.BodyDecoders
{
    /// <summary>
    /// Decodes URL encoded values.
    /// </summary>
    public class UrlFormattedDecoder : IBodyDecoder
    {
        /// <summary>
        /// The mimetype that this decoder is for.
        /// </summary>
        /// <value>application/x-www-form-urlencoded</value>
        public const string MimeType = "application/x-www-form-urlencoded";

        /// <summary>
        /// All content types that the decoder can parse.
        /// </summary>
        /// <returns>A collection of all content types that the decoder can handle.</returns>
        public IEnumerable<string> ContentTypes
        {
            get { return new[] {MimeType}; }
        }

        #region IBodyDecoder Members

        /// <summary>
        /// Decode body stream
        /// </summary>
        /// <param name="message">Contains the body to decode.</param>
        /// <exception cref="FormatException">Body format is invalid for the specified content type.</exception>
        public bool Decode(IRequest message)
        {
            if (message == null) throw new ArgumentNullException("message");

            if (!message.ContentType.StartsWith(MimeType, StringComparison.OrdinalIgnoreCase))
                return false;

            try
            {
                var decoder = new UrlDecoder();
                decoder.Parse(new StreamReader(message.Body), message.Form);
                message.Body.Position = 0;
                return true;
            }
            catch (ArgumentException err)
            {
                throw new FormatException(err.Message, err);
            }
        }

        #endregion
    }
}