using System;
using Griffin.Networking.Buffers;
using Griffin.Networking.Clients;

namespace Griffin.Networking.Messaging
{
    /// <summary>
    /// A client connection (i.e. a client which connects to a server)
    /// </summary>
    /// <remarks>A client that can connect to the <see cref="MessagingServer"/>. This client needs a messaging
    /// formatter. You could for instance
    /// download the <c>griffin.networking.basic</c> nuget package to get started directly.</remarks>
    /// <seealso cref="IMessageFormatterFactory"/>
    public class MessagingClient : ClientBase
    {
        private readonly IMessageFormatterFactory _formatterFactory;
        private readonly IMessageBuilder _messageBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingClient" /> class.
        /// </summary>
        /// <param name="formatterFactory">The formatter factory.</param>
        public MessagingClient(IMessageFormatterFactory formatterFactory)
        {
            if (formatterFactory == null) throw new ArgumentNullException("formatterFactory");
            _formatterFactory = formatterFactory;
            _messageBuilder = _formatterFactory.CreateBuilder();

        }

        /// <summary>
        /// We've received something from the other end
        /// </summary>
        /// <param name="buffer">Buffer containing the received bytes</param>
        /// <param name="bytesRead">Amount of bytes that we received</param>
        /// <remarks>You have to handle all bytes, anything left will be discarded.</remarks>
        protected override void OnReceived(IBufferSlice buffer, int bytesRead)
        {
            var gotMessage = _messageBuilder.Append(new SliceStream(buffer, bytesRead));

            if (gotMessage)
            {
                object message;
                while (_messageBuilder.TryDequeue(out message))
                {
                    Received(this, new ReceivedMessageEventArgs(message));
                }
            }
        }
        /// <summary>
        /// Send a message
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <remarks>Message will be serialized using the <see cref="IMessageFormatterFactory"/> that you've specified in the constructor.</remarks>
        public void Send(object message)
        {
            if (message == null) throw new ArgumentNullException("message");

            var serializer = _formatterFactory.CreateSerializer();
            var buffer = new BufferSlice(65535);
            var writer = new BufferWriter(buffer);
            serializer.Serialize(message, writer);

            Send(buffer, writer.Count);
        }


        /// <summary>
        /// Received a new message
        /// </summary>
        public event EventHandler<ReceivedMessageEventArgs> Received = delegate { };
   }
}