using System;
using Griffin.Networking.Servers;

namespace Griffin.Networking.Pipelines.Messages
{
    /// <summary>
    /// A client have connected to our server.
    /// </summary>
    public class ClientConnected : IPipelineMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientConnected" /> class.
        /// </summary>
        /// <param name="client">The client.</param>
        public ClientConnected(IServerClient client)
        {
            if (client == null) throw new ArgumentNullException("client");
            Client = client;
        }

        /// <summary>
        /// Gets client
        /// </summary>
        public IServerClient Client { get; private set; }
    }
}