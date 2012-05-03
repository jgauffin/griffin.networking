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
        public Received(EndPoint remoteEndPoint, Stream networkStream, BufferSlice readBuffer)
        {
            RemoteEndPoint = remoteEndPoint;
            NetworkStream = networkStream;
            BufferSlice = readBuffer;
        }

        public EndPoint RemoteEndPoint { get; private set; }
        public Stream NetworkStream { get; private set; }
        public BufferSlice BufferSlice { get; private set; }

    }
}