using System;
using Griffin.Networking.Messaging;

namespace Griffin.Networking.Servers
{
    /// <summary>
    /// Represents that class that will handle a connection from the client
    /// </summary>
    /// <remarks>The server will invoke dispose when the client has disconnected.</remarks>
    public interface IServerClient : IDisposable
    {
        /// <summary>
        /// Assign the context which can be used to communicate with the client
        /// </summary>
        /// <param name="context">Context</param>
        void Assign(IServerClientContext context);

        /// <summary>
        /// A new message have been received from the remote end.
        /// </summary>
        /// <param name="message"></param>
        /// <remarks>We'll deserialize messages for you. What you receive here depends on the used <see cref="IMessageFormatterFactory"/>.</remarks>
        void HandleReceive(object message);
    }
}