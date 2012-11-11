using System;

namespace Griffin.Networking.Messaging
{
    /// <summary>
    /// We have received a new message in the <see cref="MessagingClient"/>.
    /// </summary>
    public class ReceivedMessageEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReceivedMessageEventArgs" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ReceivedMessageEventArgs(object message)
        {
            if (message == null) throw new ArgumentNullException("message");
            Message = message;
        }

        /// <summary>
        /// Gets message that we've received
        /// </summary>
        /// <remarks>The type of message depends on what you've sent from the other end and on the <see cref="IMessageFormatterFactory"/> that you are using.</remarks>
        public object Message { get; private set; }
    }
}