using System;
using Griffin.Networking.Buffers;

namespace Griffin.Networking.Servers
{
    /// <summary>
    /// Context for <see cref="INetworkService.OnUnhandledException"/>.
    /// </summary>
    public class ServiceExceptionContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceExceptionContext" /> class.
        /// </summary>
        /// <param name="exception">The exception that was caught.</param>
        /// <param name="bufferSlice">The buffer slice that caused the exception (directly = protocol failure, indirectly = the handling failed).</param>
        public ServiceExceptionContext(Exception exception, IBufferSlice bufferSlice)
        {
            if (exception == null) throw new ArgumentNullException("exception");
            if (bufferSlice == null) throw new ArgumentNullException("bufferSlice");
            BufferSlice = bufferSlice;
            Exception = exception;
        }

        /// <summary>
        /// Gets or sets if we may continue to process incoming bytes
        /// </summary>
        /// <remarks>Setting this bool to false will only abort any reads. It will not shut down the client or anything.
        /// Hence setting it to true indicated that you are going to close the connection.</remarks>
        public bool MayContinue { get; set; }

        /// <summary>
        /// Gets or sets if we can dispatch the exception to our unhandled exception events (in <see cref="IServerClientContext"/> and <see cref="ServerBase"/>).
        /// </summary>
        public bool CanExceptionBePropagated { get; set; }

        /// <summary>
        /// Exception that was caught
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// Gets buffer that caused the exception (directly or indirectly)
        /// </summary>
        /// <remarks>(directly = protocol failure, indirectly = the handling failed)</remarks>
        public IBufferSlice BufferSlice { get; private set; }
    }
}