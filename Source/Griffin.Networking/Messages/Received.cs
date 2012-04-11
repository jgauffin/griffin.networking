using System.IO;
using System.Net;
using Griffin.Networking.Buffers;

namespace Griffin.Networking.Messages
{
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

        /// <summary>
        /// Gets or sets the number of bytes that 
        /// </summary>
        public int BytesHandled { get; set; }
    }
}