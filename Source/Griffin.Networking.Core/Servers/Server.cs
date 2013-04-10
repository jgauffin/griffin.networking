using System;
using System.Net;

namespace Griffin.Networking.Servers
{
    /// <summary>
    /// A basic server. 
    /// </summary>
    /// <remarks>
    /// <para>Will take care of all client management for you.  Uses a <see cref="IServiceFactory"/> to create the classes which will serve each connecting client.</para>
    /// </remarks>
    public class Server : ServerBase
    {
        private readonly IServiceFactory _clientFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="Server" /> class.
        /// </summary>
        /// <param name="clientFactory">The client factory.</param>
        /// <param name="configuration">The configuration.</param>
        /// <exception cref="System.ArgumentNullException">clientFactory</exception>
        public Server(IServiceFactory clientFactory, ServerConfiguration configuration) : base(configuration)
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
        protected override INetworkService CreateClient(EndPoint remoteEndPoint)
        {
            if (remoteEndPoint == null) throw new ArgumentNullException("remoteEndPoint");
            return _clientFactory.CreateClient(remoteEndPoint);
        }
    }
}