using Griffin.Networking.Servers;

namespace Griffin.Networking.Messaging
{
    /// <summary>
    /// Service which can send and receive messages.
    /// </summary>
    public abstract class MessagingService : INetworkService
    {
        /// <summary>
        /// Gets context used to send stuff.
        /// </summary>
        public MessagingClientContext Context { get; private set; }

        #region INetworkService Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public virtual void Dispose()
        {
        }

        /// <summary>
        /// Assign the context which can be used to communicate with the client
        /// </summary>
        /// <param name="context">Context</param>
        void INetworkService.Assign(IServerClientContext context)
        {
            Context = (MessagingClientContext) context;
        }

        /// <summary>
        /// A new message have been received from the remote end.
        /// </summary>
        /// <param name="message"></param>
        /// <remarks>We'll deserialize messages for you. What you receive here depends on the used <see cref="IMessageFormatterFactory"/>.</remarks>
        public abstract void HandleReceive(object message);

        /// <summary>
        /// An unhandled exception was caught when handling incoming bytes.
        /// </summary>
        /// <param name="context">Information about the exception that was caught</param>
        public virtual void OnUnhandledException(ServiceExceptionContext context)
        {
        }

        #endregion
    }
}