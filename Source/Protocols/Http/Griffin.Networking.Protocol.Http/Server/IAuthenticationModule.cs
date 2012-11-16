namespace Griffin.Networking.Http.Server
{
    /// <summary>
    /// The HTTP module is used to authenticate the request (i.e. login the user)
    /// </summary>
    /// <remarks>Invoked directly after <see cref="IHttpModule.BeginRequest"/></remarks>
    public interface IAuthenticationModule : IHttpModule
    {
        /// <summary>
        /// Authorize the request.
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <returns><see cref="ModuleResult.Stop"/> will stop all processing including <see cref="IHttpModule.EndRequest"/>.</returns>
        ModuleResult Authenticate(IRequestContext context);
    }
}