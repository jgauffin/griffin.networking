using System;
using System.Security.Principal;
using Griffin.Networking.Protocol.Http.Protocol;
using Griffin.Networking.Protocol.Http.Services.Routing;

namespace Griffin.Networking.Protocol.Http.Server
{
    /// <summary>
    /// Request context information
    /// </summary>
    public interface IHttpContext : IDisposable
    {
        /// <summary>
        /// Incoming request
        /// </summary>
        IRequest Request { get; }

        /// <summary>
        /// Response to send back
        /// </summary>
        IResponse Response { get; }

        /// <summary>
        /// Can be used to store items through this request
        /// </summary>
        /// <remarks>Items which are stored for the current request only</remarks>
        IItemStorage Items { get; }

        /// <summary>
        /// Used to store items for the entire application.
        /// </summary>
        /// <remarks>These items are shared between all requests and users</remarks>
        /// <seealso cref="HttpServer.ApplicationInfo"/>
        IItemStorage Application { get; }

        /// <summary>
        /// USed to store items for the current session (if a session has been started)
        /// </summary>
        /// <remarks>Will be null if a session has not been started.
        /// <para>Shared between all requests for a specific user</para></remarks>
        IItemStorage Session { get; }

        /// <summary>
        /// All exceptions will be logged by the system, but we generally do only keep track of the last one.
        /// </summary>
        Exception LastException { get; set; }

        /// <summary>
        /// Gets or sets currently logged in user.
        /// </summary>
        IPrincipal User { get; set; }

        /// <summary>
        /// Gets information stored for the route.
        /// </summary>
        /// <remarks>For instance used to convert the URI into parameters.</remarks>
        /// <seealso cref="IRequestRouter"/>
        IItemStorage RouteData { get; }

        /// <summary>
        /// Register a callback for the request disposal (i.e. the reply have been sent back and everything is cleaned up)
        /// </summary>
        /// <param name="callback">Callback to invoke</param>
        void RegisterForDisposal(Action<IHttpContext> callback);
    }
}