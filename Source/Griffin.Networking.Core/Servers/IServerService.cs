using System;
using Griffin.Networking.Buffers;
using Griffin.Networking.Messaging;

namespace Griffin.Networking.Servers
{
    /// <summary>
    /// Represents that class that will handle a connection from the client
    /// </summary>
    /// <remarks>The server will invoke dispose when the client has disconnected.</remarks>
    public interface IServerService : IDisposable
    {
        /// <summary>
        /// Assign the context which can be used to communicate with the client
        /// </summary>
        /// <param name="context">Context</param>
        void Assign(IServerClientContext context);

        /// <summary>
        /// A new message have been received from the remote end.
        /// </summary>
        /// <param name="message">Message type depends on the type of client/server you are using. See the remarks.</param>
        /// <remarks><para>A clean <see cref="Server"/> will give you a <see cref="SliceStream"/> here.</para><para>Other server implementations might give you something else.</para></remarks>
        void HandleReceive(object message);
    }
}