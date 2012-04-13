using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Griffin.Networking.Http.Protocol;

namespace Griffin.Networking.Http.Services.BodyDecoders
{
    /// <summary>
    /// Decodes body stream.
    /// </summary>
    public interface IBodyDecoder
    {
        /// <summary>
        /// All content types that the decoder can parse.
        /// </summary>
        /// <returns>A collection of all content types that the decoder can handle.</returns>
        IEnumerable<string> ContentTypes { get; }

        /// <summary>
        /// Decode body stream
        /// </summary>
        /// <param name="message">Contains the body to decode.</param>
        /// <exception cref="FormatException">Body format is invalid for the specified content type.</exception>
        void Decode(IRequest message);
    }
}
