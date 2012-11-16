using System;
using System.Linq;
using System.Net;
using Griffin.Networking.Buffers;
using Griffin.Networking.Messaging;
using Griffin.Networking.Servers;

namespace Griffin.Networking.Http.Server
{
    /// <summary>
    /// Default HTTP Server implementation
    /// </summary>
    /// <remarks>This implementation uses modules for everything</remarks>
    public class HttpServer : IServiceFactory
    {
        private readonly IBufferSliceStack _bufferSliceStack;
        private readonly IModuleManager _moduleManager ;
        private readonly WorkerConfiguration _workerConfiguration;
        private MessagingServer _server;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpServer" /> class.
        /// </summary>
        /// <param name="moduleManager">The modules are used to process the HTTP requests. You need to specify at least one.</param>
        public HttpServer(IModuleManager moduleManager)
            : this(moduleManager, new MessagingServerConfiguration(new HttpMessageFactory()))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpServer" /> class.
        /// </summary>
        /// <param name="moduleManager">The modules are used to process the HTTP requests. You need to specify at least one.</param>
        /// <param name="configuration">You can override the configuration to your likings. We suggest that you using the <see cref="HttpMessageFactory" /> to produce the messages.</param>
        /// <exception cref="System.ArgumentNullException">moduleManager/configuration</exception>
        public HttpServer(IModuleManager moduleManager, MessagingServerConfiguration configuration)
        {
            if (moduleManager == null) throw new ArgumentNullException("moduleManager");
            if (configuration == null) throw new ArgumentNullException("configuration");
            _moduleManager = moduleManager;
            _server = new MessagingServer(this, configuration);
            _bufferSliceStack = new BufferSliceStack(100, 65535);
            ApplicationInfo = new MemoryItemStorage();
            _workerConfiguration = new WorkerConfiguration
                {
                    Application = ApplicationInfo,
                    BufferSliceStack = _bufferSliceStack,
                    ModuleManager = _moduleManager
                };
        }

        /// <summary>
        /// You can fill this item with application specific information
        /// </summary>
        /// <remarks>It will be supplied for every request in the <see cref="IRequestContext"/>.</remarks>
        public IItemStorage ApplicationInfo { get; set; }

        #region IServiceFactory Members

        /// <summary>
        /// Create a new client
        /// </summary>
        /// <param name="remoteEndPoint">IP address of the remote end point</param>
        /// <returns>Created client</returns>
        public IServerService CreateClient(EndPoint remoteEndPoint)
        {
            return new HttpServerWorker((IPEndPoint)remoteEndPoint, _workerConfiguration);
        }

        #endregion

        /// <summary>
        /// Add a HTTP module
        /// </summary>
        /// <param name="module">Module to include</param>
        /// <remarks>Modules are executed in the order they are added.</remarks>
        public void Add(IHttpModule module)
        {
            _moduleManager.Add(module);
        }
    }
}