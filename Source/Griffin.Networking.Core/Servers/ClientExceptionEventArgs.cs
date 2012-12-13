using System;
using Griffin.Networking.Buffers;

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
        /// <param name="buffer"></param>
        /// <exception cref="System.ArgumentNullException">clientContext</exception>
        public ClientExceptionEventArgs(IServerClientContext clientContext, Exception exception, BufferSlice buffer)
        {
            if (clientContext == null) throw new ArgumentNullException("clientContext");
            if (exception == null) throw new ArgumentNullException("exception");
            if (buffer == null) throw new ArgumentNullException("buffer");

            ClientContext = clientContext;
            Exception = exception;
            Buffer = buffer;
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
        /// Gets buffer (incoming bytes) that caused the exception
        /// </summary>
        /// <remarks>The buffer can have caused the exception directly or indirectly. Directly most often means protocol failure.
        /// Indirectly means that your code caused the exception.</remarks>
        public BufferSlice Buffer { get; private set; }

        /// <summary>
        /// Gets or sets if processing can be continued or if we should abort
        /// </summary>
        /// <remarks>You still have to disconnect the client yourself using the <see cref="ClientContext"/>.</remarks>
        public bool CanContinue { get; set; }
    }
}