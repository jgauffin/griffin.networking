using System;
using System.Net;
using Griffin.Networking.Http.Protocol;

namespace Griffin.Networking.Http.Server
{
    /// <summary>
    /// One instance per HTTP connection
    /// </summary>
    /// <remarks></remarks>
    public class HttpServerWorker : HttpService
    {
        private readonly WorkerConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpServerWorker" /> class.
        /// </summary>
        /// <param name="remoteEndPoint">The remote end point.</param>
        /// <param name="configuration">The configuration.</param>
        public HttpServerWorker(IPEndPoint remoteEndPoint, WorkerConfiguration configuration)
            : base(configuration.BufferSliceStack)
        {
            if (remoteEndPoint == null) throw new ArgumentNullException("remoteEndPoint");
            if (configuration == null) throw new ArgumentNullException("configuration");
            RemoteEndPoint = remoteEndPoint;
            _configuration = configuration;
        }

        /// <summary>
        /// Gets end point that the client connected from
        /// </summary>
        public IPEndPoint RemoteEndPoint { get; private set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
        }

        /// <summary>
        /// A new message have been received from the remote end.
        /// </summary>
        /// <param name="message"></param>
        /// <remarks>You'll receive <see cref="IRequest"/> or <see cref="IResponse"/> depending on the type of application.</remarks>
        public override void HandleReceive(object message)
        {
            if (message == null) throw new ArgumentNullException("message");
            var context = new HttpContext
                {
                    Application = _configuration.Application,
                    Items = new MemoryItemStorage(),
                    Request = (IRequest) message,
                    Response = ((IRequest) message).CreateResponse(HttpStatusCode.OK, "Okey dokie")
                };

            context.Response.AddHeader("X-Powered-By",
                                       "Griffin.Networking (http://github.com/jgauffin/griffin.networking)");
            _configuration.ModuleManager.Invoke(context);

            Send(context.Response);
        }
    }
}