using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Griffin.Networking.Buffers;
using Griffin.Networking.Servers;

namespace Griffin.Networking.Pipelines
{
    /// <summary>
    /// Server that uses a pipeline for all client communication
    /// </summary>
    public class PipelineServer : ServerBase
    {
        private readonly IPipelineFactory _factory;
        private readonly ServerConfiguration _configuration;
    
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
            return new PipelineServerClient(_factory.Build());
        }
    }
}
