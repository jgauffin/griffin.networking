using System.Net;
using System.Threading;
using Griffin.Networking.Http.Messages;
using Griffin.Networking.Http.Services.Authentication;
using Griffin.Networking.Pipelines;

namespace Griffin.Networking.Http.Handlers
{
    public class AuthenticationHandler : IUpstreamHandler, IDownstreamHandler
    {
        private readonly IAuthenticator _authenticator;
        private readonly IPrincipalFactory _principalFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationHandler"/> class.
        /// </summary>
        /// <param name="authenticator">The authenticator.</param>
        /// <param name="principalFactory">Used to generate the principal which is set for the current thread</param>
        public AuthenticationHandler(IAuthenticator authenticator, IPrincipalFactory principalFactory)
        {
            _authenticator = authenticator;
            _principalFactory = principalFactory;
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
            var msg = message as SendHttpResponse;
            if (msg == null)
            {
                context.SendDownstream(message);
                return;
            }

            if (msg.Response.StatusCode == (int) HttpStatusCode.Unauthorized)
            {
                _authenticator.CreateChallenge(msg.Request, msg.Response);
            }

            context.SendDownstream(message);
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
            var msg = message as ReceivedHttpRequest;
            if (msg == null)
            {
                context.SendUpstream(message);
                return;
            }

            var authHeader = msg.HttpRequest.Headers["Authorization"];
            if (authHeader == null)
            {
                context.SendUpstream(message);
                return;
            }

            IAuthenticationUser user;
            try
            {
                user = _authenticator.Authenticate(msg.HttpRequest);
            }
            catch (HttpException err)
            {
                var response = msg.HttpRequest.CreateResponse(err.StatusCode, err.Message);
                context.SendDownstream(new SendHttpResponse(msg.HttpRequest, response));
                return;
            }

            if (user == null)
            {
                var response = msg.HttpRequest.CreateResponse(HttpStatusCode.Unauthorized,
                                                              "Invalid username or password.");
                context.SendDownstream(new SendHttpResponse(msg.HttpRequest, response));
            }
            else
            {
                var principal =
                    _principalFactory.Create(new PrincipalFactoryContext(msg.HttpRequest, user));
                Thread.CurrentPrincipal = principal;
                context.SendUpstream(message);
            }
        }

        #endregion
    }
}