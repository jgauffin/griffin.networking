using System;
using System.Collections.Generic;
using Griffin.Networking.Http.Protocol;

namespace Griffin.Networking.Http.Server
{
    /// <summary>
    /// Request context
    /// </summary>
    public class RequestContext : IRequestContext
    {
        private readonly LinkedList<Action<IRequestContext>> _callbacks = new LinkedList<Action<IRequestContext>>();

        #region IRequestContext Members

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
        /// Register a callback for the request disposal (i.e. the reply have been sent back and everything is cleaned up)
        /// </summary>
        /// <param name="callback">Callback to invoke</param>
        public void RegisterForDisposal(Action<IRequestContext> callback)
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