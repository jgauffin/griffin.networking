using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Griffin.Networking.JsonRpc.Messages;
using Griffin.Networking.JsonRpc.Remoting;

namespace Griffin.Networking.JsonRpc.Handlers
{
    /// <summary>
    /// Handles the RPC requests
    /// </summary>
    public class RequestHandler : IUpstreamHandler
    {
        private readonly IRpcInvoker _rpcInvoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestHandler"/> class.
        /// </summary>
        /// <param name="rpcInvoker">Service used to invoke the RPC method.</param>
        public RequestHandler(IRpcInvoker rpcInvoker)
        {
            _rpcInvoker = rpcInvoker;
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
            var msg = message as ReceivedRequest;
            if (msg == null)
            {
                context.SendUpstream(message);
                return;
            }

            var response = _rpcInvoker.Invoke(msg.Request);
            context.SendDownstream(new SendResponse(response));
        }
    }
}
