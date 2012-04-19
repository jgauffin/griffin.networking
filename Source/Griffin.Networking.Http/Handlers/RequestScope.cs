using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Griffin.Networking.Http.Handlers
{
    public class RequestScope : IUpstreamHandler, IDownstreamHandler
    {
        private readonly IScopeListener _listener;
        private Guid _id = Guid.NewGuid();

        public RequestScope(IScopeListener listener)
        {
            _listener = listener;
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
            _listener.ScopeStarted(_id);
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
            _listener.ScopeEnded(_id);
        }
    }

    public interface IScopeListener
    {
        void ScopeStarted(object id);
        void ScopeEnded(object id);
    }
}
