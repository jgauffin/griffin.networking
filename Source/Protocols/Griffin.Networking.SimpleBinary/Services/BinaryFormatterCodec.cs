using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Griffin.Networking.SimpleBinary.Services
{
    /// <summary>
    /// Encodes and decodes packets using <see cref="BinaryFormatter"/>.
    /// </summary>
    /// <remarks>Class is thread safe (can be used as a singleton)</remarks>
    public class BinaryFormatterCodec : IContentDecoder, IContentEncoder
    {
        /// <summary>
        /// Decode the contents in the stream
        /// </summary>
        /// <param name="type">Type to build</param>
        /// <param name="stream">Contents to be deserialized</param>
        /// <returns>Created object.</returns>
        public object Decode(Type type, Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            var formatter = new BinaryFormatter();
            return formatter.Deserialize(stream);
        }

        /// <summary>
        /// Encode value into a stream
        /// </summary>
        /// <param name="value">Value to encode</param>
        /// <param name="destination">Stream to write the serialized object to.</param>
        public void Encode(object value, Stream destination)
        {
            if (value == null) throw new ArgumentNullException("value");
            if (destination == null) throw new ArgumentNullException("destination");
            var formatter = new BinaryFormatter();
            formatter.Serialize(destination, value);
        }
    }
}
