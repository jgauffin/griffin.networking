using Griffin.Networking.Servers;

namespace Griffin.Networking.Messaging
{
    /// <summary>
    /// Configuration for <see cref="MessagingServer"/>.
    /// </summary>
    public class MessagingServerConfiguration : ServerConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerConfiguration" /> class.
        /// </summary>
        /// <param name="messageFormatterFactory">The message formatter factory.</param>
        /// <seealso cref="MessageFormatterFactory"/>
        public MessagingServerConfiguration(IMessageFormatterFactory messageFormatterFactory)
        {
            MessageFormatterFactory = messageFormatterFactory;
        }


        /// <summary>
        /// Used to serialize/build the messages that are transferred.
        /// </summary>
        public IMessageFormatterFactory MessageFormatterFactory { get; set; }
    }
}