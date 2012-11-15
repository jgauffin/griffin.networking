using System;
using System.Net.Sockets;

namespace Griffin.Networking
{
    /// <summary>
    /// We would like to be able to send both <c>byte[]</c> and <c>Stream</c> objects. And to be able to do that
    /// we need to be able to manage the writes.
    /// </summary>
    /// <remarks>Make sure that you dispose the stream or the buffer when dispose is being called by the framework.</remarks>
    public interface ISocketWriterJob : IDisposable
    {
        /// <summary>
        /// Write stuff to our args.
        /// </summary>
        /// <param name="args">Args used when sending bytes to the socket</param>
        /// <remarks>The <see cref="SocketAsyncEventArgs.UserToken"/> contains a <c>IBufferSlice</c> which can be used
        /// for write operations. You are free to use it, but do NOT change the <c>UserToken</c>.</remarks>
        void Write(SocketAsyncEventArgs args);

        /// <summary>
        /// The async write has been completed
        /// </summary>
        /// <param name="bytes">Number of bytes that was sent</param>
        /// <returns><c>true</c> if everything was sent; otherwise <c>false</c>.</returns>
        bool WriteCompleted(int bytes);
    }
}