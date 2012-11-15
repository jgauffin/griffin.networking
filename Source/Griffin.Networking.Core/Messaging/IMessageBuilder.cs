using Griffin.Networking.Buffers;

namespace Griffin.Networking.Messaging
{
    /// <summary>
    /// Used to build a message from buffers
    /// </summary>
    public interface IMessageBuilder
    {
        /// <summary>
        /// Append more bytes to your message building
        /// </summary>
        /// <param name="reader">Contains bytes which was received from the other end</param>
        /// <returns><c>true</c> if a complete message has been built; otherwise <c>false</c>.</returns>
        /// <remarks>You must handle/read everything which is available in the buffer</remarks>
        bool Append(IBufferReader reader);

        /// <summary>
        /// Try to dequeue a message
        /// </summary>
        /// <param name="message">Message that the builder has built.</param>
        /// <returns><c>true</c> if a message was available; otherwise <c>false</c>.</returns>
        bool TryDequeue(out object message);
    }
}