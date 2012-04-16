using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Griffin.Networking.Http.Messages;
using Griffin.Networking.Http.Services.Authentication;

namespace Griffin.Networking.Http.Handlers
{
    public class AuthenticationHandler : IUpstreamHandler, IDownstreamHandler
    {
        private IAuthenticator _authenticator;

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
            var msg = message as ReceivedHttpRequest;
            if (msg == null)
            {
                context.SendUpstream(message);
                return;
            }

            var authHeader = msg.HttpRequest.Headers["Authenticate"];
            if(authHeader == null)
            {
                context.SendUpstream(message);
                return;
            }

            _authenticator.Authenticate(msg.HttpRequest);
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
            var msg = message as SendHttpResponse;
            if (msg == null)
            {
                context.SendDownstream(message);
                return;
            }

            _authenticator.CreateChallenge(msg.Response);
        }
    }
}
