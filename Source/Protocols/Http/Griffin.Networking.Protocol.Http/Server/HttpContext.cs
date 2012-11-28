using System;
using System.Collections.Generic;
using System.Security.Principal;
using Griffin.Networking.Protocol.Http.Protocol;
using Griffin.Networking.Protocol.Http.Services.Routing;

namespace Griffin.Networking.Protocol.Http.Server
{
    /// <summary>
    /// Request context
    /// </summary>
    public class HttpContext : IHttpContext
    {
        private readonly LinkedList<Action<IHttpContext>> _callbacks = new LinkedList<Action<IHttpContext>>();


        /// <summary>
        /// Initializes a new instance of the <see cref="HttpContext" /> class.
        /// </summary>
        public HttpContext()
        {
            RouteData = new MemoryItemStorage();
        }

        #region IHttpContext Members

        /// <summary>
        /// Incoming request
        /// </summary>
        public IRequest Request { get; set; }

        /// <summary>
        /// Response to send back
        /// </summary>
        public IResponse Response { get; set; }

        /// <summary>
        /// Can be used to store items through this request
        /// </summary>
        public IItemStorage Items { get; set; }

        /// <summary>
        /// Used to store items for the entire application.
        /// </summary>
        public IItemStorage Application { get; set; }

        /// <summary>
        /// USed to store items for the current session (if a session has been started)
        /// </summary>
        /// <remarks>Will be null if a session has not been started.</remarks>
        public IItemStorage Session { get; set; }

        /// <summary>
        /// All exceptions will be logged by the system, but we generally do only keep track of the last one.
        /// </summary>
        public Exception LastException { get; set; }

        /// <summary>
        /// Gets or sets currently logged in user.
        /// </summary>
        public IPrincipal User { get; set; }

        /// <summary>
        /// Gets information stored for the route.
        /// </summary>
        /// <remarks>For instance used to convert the URI into parameters.</remarks>
        /// <seealso cref="IRequestRouter"/>
        public IItemStorage RouteData { get; private set; }

        /// <summary>
        /// Register a callback for the request disposal (i.e. the reply have been sent back and everything is cleaned up)
        /// </summary>
        /// <param name="callback">Callback to invoke</param>
        public void RegisterForDisposal(Action<IHttpContext> callback)
        {
            if (callback == null) throw new ArgumentNullException("callback");
            _callbacks.AddLast(callback);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            foreach (var callback in _callbacks)
            {
                callback(this);
            }

            _callbacks.Clear();
        }

        #endregion
    }
}