using System.IO;
using System.Net;
using Griffin.Networking.Buffers;

namespace Griffin.Networking.Messages
{
    /// <summary>
    /// We've received bytes from the remote peer.
    /// </summary>
    /// <remarks>The buffer will be compacted by the channel when the message has been handled. It's thefore important that <see cref="BufferSlice.Position"/> is kept
    /// on the byte after the last read position.</remarks>
    public class Received : IPipelineMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Received"/> class.
        /// </summary>
        /// <param name="remoteEndPoint">The remote end point.</param>
        /// <param name="networkStream">The network stream.</param>
        /// <param name="readBuffer">The read buffer.</param>
        public Received(EndPoint remoteEndPoint, Stream networkStream, BufferSlice readBuffer)
        {
            RemoteEndPoint = remoteEndPoint;
            NetworkStream = networkStream;
            BufferSlice = readBuffer;
        }

        /// <summary>
        /// Gets endpoint that the message is from
        /// </summary>
        public EndPoint RemoteEndPoint { get; private set; }

        /// <summary>
        /// Will likely get removed from the message
        /// </summary>
        protected Stream NetworkStream { get; private set; }

        /// <summary>
        /// Gets buffer with the received data
        /// </summary>
        public BufferSlice BufferSlice { get; private set; }

    }
}