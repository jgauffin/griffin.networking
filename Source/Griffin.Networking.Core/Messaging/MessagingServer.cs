using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Griffin.Networking.Buffers;
using Griffin.Networking.Servers;

namespace Griffin.Networking.Messaging
{
    /// <summary>
    /// Sends/Receives messages (POCOs) from one/many clients.
    /// </summary>
    public class MessagingServer : Server
    {
        private readonly IMessageFormatterFactory _formatterFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingServer" /> class.
        /// </summary>
        /// <param name="clientFactory">Used to create the class that you use to handle all client communication.</param>
        /// <param name="configuration">The configuration.</param>
        public MessagingServer(IClientFactory clientFactory, MessagingServerConfiguration configuration) : base(clientFactory, configuration)
        {
            if (clientFactory == null) throw new ArgumentNullException("clientFactory");
            if (configuration == null) throw new ArgumentNullException("configuration");
            _formatterFactory = configuration.MessageFormatterFactory;
        }

        /// <summary>
        /// Creates <see cref="MessageBasedClientContext"/>
        /// </summary>
        /// <param name="readBuffer">Read buffer</param>
        /// <returns>Created client</returns>
        protected override ServerClientContext CreateClientContext(IBufferSlice readBuffer)
        {
            if (readBuffer == null) throw new ArgumentNullException("readBuffer");
            return new MessageBasedClientContext(readBuffer, _formatterFactory);
        }
    }
}
