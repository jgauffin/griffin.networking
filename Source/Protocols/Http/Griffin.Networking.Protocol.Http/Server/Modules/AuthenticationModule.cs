using System;
using System.Net;
using Griffin.Networking.Http.Services.Authentication;

namespace Griffin.Networking.Http.Server.Modules
{
    /// <summary>
    /// Uses <see cref="IAuthenticator"/> to authenticate requests and then <see cref="IPrincipalFactory"/> to generate the user information.
    /// </summary>
    public class AuthenticationModule : IAuthenticationModule
    {
        private readonly IAuthenticator _authenticator;
        private readonly IPrincipalFactory _principalFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationModule" /> class.
        /// </summary>
        /// <param name="authenticator">Used for the actual authentication.</param>
        /// <param name="principalFactory">Used to create the principal that should be used.</param>
        /// <exception cref="System.ArgumentNullException">autheonticator</exception>
        public AuthenticationModule(IAuthenticator authenticator, IPrincipalFactory principalFactory)
        {
            if (authenticator == null) throw new ArgumentNullException("authenticator");
            if (principalFactory == null) throw new ArgumentNullException("principalFactory");
            _authenticator = authenticator;
            _principalFactory = principalFactory;
        }

        #region IAuthenticationModule Members

        /// <summary>
        /// Invoked before anything else
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <remarks>
        /// <para>The first method that is exeucted in the pipeline.</para>
        /// Try to avoid throwing exceptions if you can. Let all modules have a chance to handle this method. You may break the processing in any other method than the Begin/EndRequest methods.</remarks>
        public void BeginRequest(IHttpContext context)
        {
        }

        /// <summary>
        /// End request is typically used for post processing. The response should already contain everything required.
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <remarks>
        /// <para>The last method that is executed in the pipeline.</para>
        /// Try to avoid throwing exceptions if you can. Let all modules have a chance to handle this method. You may break the processing in any other method than the Begin/EndRequest methods.</remarks>
        public void EndRequest(IHttpContext context)
        {
        }

        /// <summary>
        /// Authorize the request.
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <returns><see cref="ModuleResult.Stop"/> will stop all processing including <see cref="IHttpModule.EndRequest"/>.</returns>
        public ModuleResult Authenticate(IHttpContext context)
        {
            var user = _authenticator.Authenticate(context.Request);
            if (user == null)
            {
                _authenticator.CreateChallenge(context.Request, context.Response);
                return ModuleResult.Stop;
            }

            var principal = _principalFactory.Create(new PrincipalFactoryContext(context.Request, user));
            if (principal == null)
                throw new HttpException(HttpStatusCode.InternalServerError,
                                        "Failed to create a principal for " + user.Username);

            context.User = principal;
            return ModuleResult.Continue;
        }

        #endregion
    }
}