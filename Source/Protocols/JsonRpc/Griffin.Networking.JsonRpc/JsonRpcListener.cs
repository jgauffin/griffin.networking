using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using Griffin.Networking.Channels;
using Griffin.Networking.Messages;
using Griffin.Networking.Pipelines;

namespace Griffin.Networking.JsonRpc
{
    /// <summary>
    /// JSON Rpc Listener implementation
    /// </summary>
    public class JsonRpcListener : IUpstreamHandler, IDownstreamHandler
    {
        private TcpServerChannel _serverChannel;
        private Pipeline _pipeline;


        public JsonRpcListener(IPipelineFactory clientFactory)
        {
            _pipeline = new Pipeline();
            _pipeline.AddDownstreamHandler(this);
            _pipeline.AddUpstreamHandler(this);
            _serverChannel = new TcpServerChannel(_pipeline, clientFactory, 2000);

        }

        public void Start(IPEndPoint endPoint)
        {
            _pipeline.SendDownstream(new BindSocket(endPoint));
        }

        public void Stop()
        {
            _pipeline.SendDownstream(new Close());
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
            var msg = message as PipelineFailure;
            if (msg != null)
                throw new TargetInvocationException("Pipeline failed", msg.Exception);
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
            context.SendDownstream(message);
        }

    }
}
