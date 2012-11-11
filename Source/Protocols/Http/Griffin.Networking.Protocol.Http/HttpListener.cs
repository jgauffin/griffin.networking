using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using Griffin.Networking.Pipelines.Messages;
using Griffin.Networking.Pipelines;
using Griffin.Networking.Servers;

namespace Griffin.Networking.Http
{
    /// <summary>
    /// Basic HTTP listener.
    /// </summary>
    public class HttpListener : IClientFactory
    {
        private Server _server;
        private ServerConfiguration _config;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpListener" /> class.
        /// </summary>
        /// <param name="maxClients">The maximum number of simultaneous clients.</param>
        public HttpListener(int maxClients)
        {
            _config = new ServerConfiguration() {MaximumNumberOfClients = maxClients};
            _server = new Server(this, _config);
        }

        public void Start(IPEndPoint endPoint)
        {
            if (endPoint == null) throw new ArgumentNullException("endPoint");
            _server.Start(endPoint);
        }

        public void Stop()
        {
            _server.Stop();
        }

        /// <summary>
        /// Create a new client
        /// </summary>
        /// <param name="remoteEndPoint">IP address of the remote end point</param>
        /// <returns>Created client</returns>
        public IServerClient CreateClient(EndPoint remoteEndPoint)
        {
            var client = new HttpServerClient(_config.BufferSliceStack.Pop());
            client.Disconnected += OnClientDisconnected;
            return client;
        }

        private void OnClientDisconnected(object sender, DisconnectEventArgs e)
        {
            var client = (HttpServerClient) sender;
            client.Disconnected -= OnClientDisconnected;
        }
    }

    public class AcceptedClientEventArgs : EventArgs
    {
        
    }

    public class PipelineHttpListener : IUpstreamHandler, IDownstreamHandler
    {
        private readonly IPipelineFactory _clientFactory;
        private Pipelines.Pipeline _pipeline;
        private HttpListener _listener;

        public PipelineHttpListener(IPipelineFactory clientFactory)
        {
            _clientFactory = clientFactory;
            _pipeline = new Pipelines.Pipeline();
            _pipeline.AddDownstreamHandler(this);
            _pipeline.AddUpstreamHandler(this);
            _listener = new HttpListener(100);

        }
        public void Start(IPEndPoint endPoint)
        {
            //_pipeline.SendDownstream(new BindSocket(endPoint));
            _listener.Start(endPoint);
        }

        public void Stop()
        {
            //_pipeline.SendDownstream(new Close());
            _listener.Stop();
        }

        /// <summary>
        /// Handle an message
        /// </summary>
        /// <param name="context">Context unique for this handler instance</param>
        /// <param name="message">Message to process</param>
        /// <remarks>
        /// All messages that can't be handled MUST be send up the chain using <see cref="IPipelineHandlerContext.SendUpstream"/>.
        /// </remarks>
        public void HandleUpstream(IPipelineHandlerContext context, IPipelineMessage message)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Process message
        /// </summary>
        /// <param name="context"></param>
        /// <param name="message"></param>
        /// <remarks>
        /// Should always call either <see cref="IPipelineHandlerContext.SendDownstream"/> or <see cref="IPipelineHandlerContext.SendUpstream"/>
        /// unless the handler really wants to stop the processing.
        /// </remarks>
        public void HandleDownstream(IPipelineHandlerContext context, IPipelineMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
