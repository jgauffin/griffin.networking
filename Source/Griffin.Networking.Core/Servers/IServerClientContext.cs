using System;
using System.IO;
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
        /// Send a stream
        /// </summary>
        /// <param name="stream">Stream to send</param>
        /// <remarks>The stream will be owned by the framework, i.e. disposed when sent.</remarks>
        void Send(Stream stream);

        /// <summary>
        /// Close connection and clean up.
        /// </summary>
        void Close();


        /// <summary>
        /// Context has been freed. Reset the state.
        /// </summary>
        void Reset();

        /// <summary>
        /// An unhandled exception was caught during read processing (which always is our entry point since we are a server).
        /// </summary>
        event EventHandler<ClientExceptionEventArgs> UnhandledExceptionCaught;
    }
}