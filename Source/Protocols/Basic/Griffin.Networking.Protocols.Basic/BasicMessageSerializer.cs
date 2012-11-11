using System;
using System.Text;
using Griffin.Networking.Buffers;
using Griffin.Networking.Messaging;
using Newtonsoft.Json;

namespace Griffin.Networking.Protocols.Basic
{
    /// <summary>
    /// Serializes the message into the <see cref="Packet"/> structure.
    /// </summary>
    public class BasicMessageSerializer : IMessageSerializer
    {
        private static readonly byte[] VersionBuffer = new byte[] {1};

        #region IMessageSerializer Members

        /// <summary>
        /// Serialize a message into something that can be transported over the socket.
        /// </summary>
        /// <param name="message">Message to serialize</param>
        /// <param name="writer">Buffer used to store the message</param>
        public void Serialize(object message, IBufferWriter writer)
        {
            var str = JsonConvert.SerializeObject(message);
            var bodyBytes = Encoding.UTF8.GetBytes(str);

            // version
            writer.Write(VersionBuffer, 0, VersionBuffer.Length);

            //length
            var buffer = BitConverter.GetBytes(bodyBytes.Length);
            writer.Write(buffer, 0, buffer.Length);

            //body
            writer.Write(bodyBytes, 0, bodyBytes.Length);
        }

        #endregion
    }
}