using System;
using Griffin.Networking.Pipelines;

namespace Griffin.Networking.Http.Handlers
{
    /// <summary>
    /// Used to provide a request scope (typically used by inversion of control containers)
    /// </summary>
    /// <remarks>Should be the first and the last handlers in a queue</remarks>
    public class RequestScope : IUpstreamHandler, IDownstreamHandler
    {
        private readonly Guid _id = Guid.NewGuid();
        private readonly IScopeListener _listener;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestScope"/> class.
        /// </summary>
        /// <param name="listener">The listener.</param>
        public RequestScope(IScopeListener listener)
        {
            _listener = listener;
        }

        #region IDownstreamHandler Members

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

        #endregion

        #region IUpstreamHandler Members

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
            try
            {
                _listener.ScopeStarted(_id);
            }
            catch (Exception)
            {
                _listener.ScopeEnded(_id);
                throw;
            }
        }

        #endregion
    }
}