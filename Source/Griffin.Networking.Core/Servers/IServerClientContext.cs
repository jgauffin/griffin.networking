using System.Net;
using Griffin.Networking.Buffers;

namespace Griffin.Networking.Servers
{
    /// <summary>
    /// Context used by application clients to be able to send stuff.
    /// </summary>
    public interface IServerClientContext
    {
        /// <summary>
        /// Gets address of the remote end point
        /// </summary>
        IPEndPoint RemoteEndPoint { get; }

        /// <summary>
        /// Send something to the remote end point.
        /// </summary>
        /// <param name="slice">Buffer to send</param>
        /// <param name="count">Number of bytes that have actually been written to the buffer.</param>
        void Send(IBufferSlice slice, int count);

        /// <summary>
        /// Close connection and clean up.
        /// </summary>
        void Close();
    }
}