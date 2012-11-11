using System;
using System.Net;

namespace Griffin.Networking.Servers
{
    /// <summary>
    /// Uses a <see cref="IClientFactory"/> to create the connection handlers.
    /// </summary>
    public class Server : ServerBase
    {
        private readonly IClientFactory _clientFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="Server" /> class.
        /// </summary>
        /// <param name="clientFactory">The client factory.</param>
        /// <param name="configuration">The configuration.</param>
        /// <exception cref="System.ArgumentNullException">clientFactory</exception>
        public Server(IClientFactory clientFactory, ServerConfiguration configuration) : base(configuration)
        {
            if (clientFactory == null) throw new ArgumentNullException("clientFactory");
            if (configuration == null) throw new ArgumentNullException("configuration");
            _clientFactory = clientFactory;
        }


        /// <summary>
        /// Create a new object which will handle all communication to/from a specific client.
        /// </summary>
        /// <param name="remoteEndPoint">Remote end point</param>
        /// <returns>Created client</returns>
        protected override IServerClient CreateClient(EndPoint remoteEndPoint)
        {
            if (remoteEndPoint == null) throw new ArgumentNullException("remoteEndPoint");
            return _clientFactory.CreateClient(remoteEndPoint);
        }
    }
}