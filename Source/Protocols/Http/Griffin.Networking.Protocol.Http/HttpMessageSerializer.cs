using Griffin.Networking.Buffers;
using Griffin.Networking.Http.Pipeline.Handlers;
using Griffin.Networking.Http.Protocol;
using Griffin.Networking.Messaging;

namespace Griffin.Networking.Http
{
    /// <summary>
    /// Takes HTTP messages and serialize them into bytes.
    /// </summary>
    public class HttpMessageSerializer : IMessageSerializer
    {
        /// <summary>
        /// Serialize a message into something that can be transported over the socket.
        /// </summary>
        /// <param name="message">Message to serialize</param>
        /// <param name="writer">Buffer used to store the message</param>
        public void Serialize(object message, IBufferWriter writer)
        {
            var msg = (IResponse) message;
            var serializer = new HttpHeaderSerializer();
            serializer.SerializeResponse(msg, writer);
            writer.Copy(msg.Body);
        }
    }
}