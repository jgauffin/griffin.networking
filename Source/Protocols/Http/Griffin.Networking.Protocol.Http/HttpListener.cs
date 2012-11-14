using System;
using System.Net;
using Griffin.Networking.Servers;

namespace Griffin.Networking.Http
{
    /// <summary>
    /// Basic HTTP listener.
    /// </summary>
    public class HttpListener : IServiceFactory
    {
        private readonly ServerConfiguration _config;
        private readonly Server _server;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpListener" /> class.
        /// </summary>
        /// <param name="maxClients">The maximum number of simultaneous clients.</param>
        public HttpListener(int maxClients)
        {
            _config = new ServerConfiguration {MaximumNumberOfClients = maxClients};
            //_config.
            _server = new Server(this, _config);
        }

        #region IServiceFactory Members

        /// <summary>
        /// Create a new client
        /// </summary>
        /// <param name="remoteEndPoint">IP address of the remote end point</param>
        /// <returns>Created client</returns>
        public IServerService CreateClient(EndPoint remoteEndPoint)
        {
            var client = new HttpServerClientContext(_config.BufferSliceStack.Pop());
            client.Disconnected += OnClientDisconnected;

            //TODO: Create a real client.
            return null;
        }

        #endregion

        public void Start(IPEndPoint endPoint)
        {
            if (endPoint == null) throw new ArgumentNullException("endPoint");
            _server.Start(endPoint);
        }

        public void Stop()
        {
            _server.Stop();
        }

        private void OnClientDisconnected(object sender, DisconnectEventArgs e)
        {
            var client = (HttpServerClientContext) sender;
            client.Disconnected -= OnClientDisconnected;
        }
    }

    public class RequestReceivedEventArgs : EventArgs
    {
    }

    public class AcceptedClientEventArgs : EventArgs
    {
    }
}