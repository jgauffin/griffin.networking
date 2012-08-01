using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Griffin.Networking.SimpleBinary.Services
{
    /// <summary>
    /// Used to encode values.
    /// </summary>
    public interface IContentEncoder
    {
        /// <summary>
        /// Encode value into a stream
        /// </summary>
        /// <param name="value">Value to encode</param>
        /// <param name="destination">Stream to write the serialized object to.</param>
        void Encode(object value, Stream destination);
    }
}
