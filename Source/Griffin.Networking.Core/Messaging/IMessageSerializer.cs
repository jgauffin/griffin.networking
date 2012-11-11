using Griffin.Networking.Buffers;

namespace Griffin.Networking.Messaging
{
    /// <summary>
    /// Serializes the message into the writer
    /// </summary>
    public interface IMessageSerializer
    {
        /// <summary>
        /// Serialize a message into something that can be transported over the socket.
        /// </summary>
        /// <param name="message">Message to serialize</param>
        /// <param name="writer">Buffer used to store the message</param>
        void Serialize(object message, IBufferWriter writer);
    }
}