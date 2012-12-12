using System;

namespace Griffin.Networking.Servers
{
    /// <summary>
    /// An unhandled exception has been caught for one of the clients in the server
    /// </summary>
    public class ClientExceptionEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientExceptionEventArgs" /> class.
        /// </summary>
        /// <param name="clientContext">The client context.</param>
        /// <param name="exception">The exception.</param>
        /// <exception cref="System.ArgumentNullException">clientContext</exception>
        public ClientExceptionEventArgs(IServerClientContext clientContext, Exception exception)
        {
            if (clientContext == null) throw new ArgumentNullException("clientContext");
            if (exception == null) throw new ArgumentNullException("exception");

            ClientContext = clientContext;
            Exception = exception;
            CanContinue = true;
        }

        /// <summary>
        /// Gets client that the exception was caught for
        /// </summary>
        public IServerClientContext ClientContext { get; private set; }

        /// <summary>
        /// Gets caught exception
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// Gets or sets if processing can be continued or if we should abort
        /// </summary>
        /// <remarks>You still have to disconnect the client yourself using the <see cref="ClientContext"/>.</remarks>
        public bool CanContinue { get; set; }
    }
}