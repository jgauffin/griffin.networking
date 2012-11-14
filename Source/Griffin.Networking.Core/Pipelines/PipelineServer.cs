using System.Net;
using Griffin.Networking.Servers;

namespace Griffin.Networking.Pipelines
{
    /// <summary>
    /// Server that uses a pipeline for all client communication
    /// </summary>
    public class PipelineServer : ServerBase
    {
        private readonly ServerConfiguration _configuration;
        private readonly IPipelineFactory _factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="PipelineServer" /> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="configuration">The configuration.</param>
        public PipelineServer(IPipelineFactory factory, ServerConfiguration configuration) : base(configuration)
        {
            _factory = factory;
            _configuration = configuration;
        }

        /// <summary>
        /// Create a new object which will handle all communication to/from a specific client.
        /// </summary>
        /// <param name="remoteEndPoint">Remote end point</param>
        /// <returns>Created client</returns>
        protected override IServerService CreateClient(EndPoint remoteEndPoint)
        {
            return new PipelineServerService(_factory.Build());
        }
    }
}