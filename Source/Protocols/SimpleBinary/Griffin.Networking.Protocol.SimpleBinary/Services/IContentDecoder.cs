using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Griffin.Networking.SimpleBinary
{
    /// <summary>
    /// Used to decode the content bytes.
    /// </summary>
    public interface IContentDecoder
    {
        /// <summary>
        /// Decode the contents in the stream
        /// </summary>
        /// <param name="type">Type to build</param>
        /// <param name="stream">Contents to be deserialized</param>
        /// <returns>Created object.</returns>
        object Decode(Type type, Stream stream);
    }
}
