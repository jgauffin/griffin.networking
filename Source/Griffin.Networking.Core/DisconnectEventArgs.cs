using System;
using System.Net.Sockets;

namespace Griffin.Networking
{
    /// <summary>
    /// We've been disconnected
    /// </summary>
    public class DisconnectEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisconnectEventArgs" /> class.
        /// </summary>
        /// <param name="socketError">The socket error that resulted in the disconnection.</param>
        public DisconnectEventArgs(SocketError socketError)
        {
            SocketError = socketError;
        }

        /// <summary>
        /// Gets socket error that resulted in the disconnection.
        /// </summary>
        public SocketError SocketError { get; private set; }
    }
}