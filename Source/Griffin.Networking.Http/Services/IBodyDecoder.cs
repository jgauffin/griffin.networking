using System;
using System.Collections.Generic;
using Griffin.Networking.Http.Protocol;

namespace Griffin.Networking.Http.Services
{
    /// <summary>
    /// Decodes body stream into the Form/Files properties.
    /// </summary>
    public interface IBodyDecoder
    {
        /// <summary>
        /// Decode body stream
        /// </summary>
        /// <param name="message">Contains the body to decode.</param>
        /// <exception cref="BadRequestException">Body format is invalid for the specified content type.</exception>
        void Decode(IRequest message);
    }
}
