namespace Griffin.Networking.Http.Server
{
    /// <summary>
    /// Authorize the request (i.e. check the user permissions)
    /// </summary>
    /// <remarks>Invoked after <see cref="IAuthenticationModule"/> and before <see cref="IHttpModule.HandleRequest"/>.</remarks>
    public interface IAuthorizationModule : IHttpModule
    {
        /// <summary>
        /// Authorize the user.
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <returns><see cref="ModuleResult.Stop"/> will stop all processing including <see cref="IHttpModule.EndRequest"/>.</returns>
        ModuleResult Authorize(IRequestContext context);
    }
}