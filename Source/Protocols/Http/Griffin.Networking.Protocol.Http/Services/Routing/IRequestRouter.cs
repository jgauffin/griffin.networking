using Griffin.Networking.Http.Server;

namespace Griffin.Networking.Http.Services.Routing
{
    /// <summary>
    /// Route a request
    /// </summary>
    /// <remarks>First router doing something wins, all routes added after it will not be run.</remarks>
    public interface IRequestRouter
    {
        /// <summary>
        /// Route the request.
        /// </summary>
        /// <param name="context">HTTP context used to identify the route</param>
        /// <returns><c>true</c> if we generated some routing; otherwise <c>false</c></returns>
        bool Route(IHttpContext context);
    }
}