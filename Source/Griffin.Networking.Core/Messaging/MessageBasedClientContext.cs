using System.Net.Sockets;
using Griffin.Networking.Buffers;
using Griffin.Networking.Servers;

namespace Griffin.Networking.Messaging
{
    /// <summary>
    /// Used by <see cref="MessagingServer"/>.
    /// </summary>
    public class MessageBasedClientContext : ServerClientContext
    {
        private readonly IMessageFormatterFactory _formatterFactory;
        private readonly IMessageBuilder _messageBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBasedClientContext" /> class.
        /// </summary>
        /// <param name="readBuffer">The read buffer.</param>
        /// <param name="formatterFactory">Used to format messages </param>
        public MessageBasedClientContext(IBufferSlice readBuffer, IMessageFormatterFactory formatterFactory) : base(readBuffer)
        {
            _formatterFactory = formatterFactory;
            _messageBuilder = _formatterFactory.CreateBuilder();
        }

        protected override void HandleRead(IBufferSlice slice, int bytesRead)
        {
            if (_messageBuilder.Append(new SliceStream(slice, bytesRead)))
            {
                object message;
                while (_messageBuilder.TryDequeue(out message))
                {
                    TriggerClientReceive(message);
                }
            }
        }

        /// <summary>
        /// Will serialize messages
        /// </summary>
        /// <param name="message"></param>
        public virtual void Write(object message)
        {
            var formatter = _formatterFactory.CreateSerializer();
            var buffer = new BufferSlice(65535);
            var writer = new SliceStream(buffer);
            formatter.Serialize(message, writer);
            writer.Position = 0;
            Send(buffer, (int) writer.Length);
        }
    }
}